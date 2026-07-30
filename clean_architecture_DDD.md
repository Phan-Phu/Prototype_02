# Assets/Scripts — Architecture & Workflow

This document describes how the code under `Assets/Scripts` is actually organized and how it behaves at runtime. It reflects the real implementation (verified by reading the code), not just the folder names — where the folder structure suggests a pattern that isn't fully realized (e.g. "CQRS"), that gap is called out explicitly.

## Layers

The code is split into three top-level layers, loosely following Clean Architecture / DDD naming:

```
Domain/            business rules: entities, value objects, domain events, service interfaces
Infrastructure/    implementations of Domain interfaces: repositories, master data, CQRS mediator
Application/       Unity-facing layer: MonoBehaviours, states/screens, popups, UI, commands, GameManager
Extentions/        small static extension helpers (DateTime, PlayerPrefs) — note the project's own spelling
UnityPurchasing/   Unity IAP generated code (receipt validation)
3DEnv/             3D character/environment scripts (movement, camera, spawning)
```

`Domain` has no dependency on Unity/Infrastructure types in the areas inspected, and `Infrastructure` implements `Domain` interfaces — the dependency direction is respected. The main crack in the "clean" boundary is that `Application` code overwhelmingly resolves its dependencies through a global static service locator (`GameManager.Get<T>()`) rather than constructor injection, and cross-cutting notifications go through a static `EventManager` — both are practical/pragmatic for a Unity project, but they mean most of `Application` is not unit-testable in isolation.

## Composition root: `GameManager`

[Application/Common/GameManager.cs](Application/Common/GameManager.cs) is the single composition root and service locator for the whole game (~350 lines). At startup it:

1. Builds a `Microsoft.Extensions.DependencyInjection` `ServiceCollection` in `ConfigServices()`.
2. Registers services as singletons (`AddSingleton<IFoo, Foo>()`).
3. For services that must be `MonoBehaviour`s (e.g. `ICharacterService`, `IShopService`), spins up a `GameObject` + `AddComponent` via `CreateMonoService<T>()` / `InjectMonoService<T, TImpl>()`, then registers the created instance.
4. Registers CQRS command handlers (`AddCommandHandler()`), per-entity repositories (`AddUserRepositories()`), and CSV master data sources (`AddMasterData()`, via chained extensions like `.ConfigureCharacterMasterData()`).
5. Calls `BuildServiceProvider()` once; everything after this resolves via `GameManager.Get<T>()` / `GameManager.Instance.GetService<T>()`.

This is a real DI container under the hood (Microsoft.Extensions.DependencyInjection), but resolution is Service-Locator style at the call sites, not `[Inject]`-attribute or constructor-injection driven — the one exception is CQRS command handlers, whose constructors *are* injected automatically by the container when the mediator resolves them.

## Request flow: Command → Mediator → Handler → Domain/Repository

Write-side operations go through a thin CQRS-flavored mediator:

- [Application/ICommand.cs](Application/ICommand.cs) defines the contracts:
  ```csharp
  public interface ICommand<out R> { }
  public interface ICommandHandler<in C, R> where C : ICommand<R> {
      UniTask<R> Handle(C command);
  }
  public interface IMediator {
      UniTask<R> Send<T, R>(T command) where T : ICommand<R>;
  }
  ```
- [Infrastructure/CQRS/MediatorImpl.cs](Infrastructure/CQRS/MediatorImpl.cs) implements `IMediator.Send` by resolving `ICommandHandler<T,R>` from the service locator, logging the command payload (`JsonConvert.SerializeObject`), and awaiting the handler.
- A typical feature (e.g. [Application/UserData/Commands/ChangePlayerNameCommand.cs](Application/UserData/Commands/ChangePlayerNameCommand.cs)) defines a `Command` (data) + `CommandHandler` (constructor-injected domain service, business logic) pair. Views/States call `mediator.Send<Command, Result>(new SomeCommand(...))`.

**Important gap:** this is command-only. `MediatorImpl.Query<T>` exists but is commented out, and there is no `IQuery`/`IQueryHandler` anywhere in the codebase. Reads happen via plain service calls (`IUserDataService`, `ICharacterService`, …), not through the mediator — so despite the `Infrastructure/CQRS` folder name, only the "C" in CQRS is implemented.

## Domain layer

- **Entities** — `Domain/Core/Entity.cs` (base `Id`) is subclassed by concrete entities such as `PlayerCharacter` ([Domain/Character/Character.cs](Domain/Character/Character.cs)) and `Equipment` ([Domain/Inventory/Equipment.cs](Domain/Inventory/Equipment.cs)).
- **Value Objects** — built on `CSharpFunctionalExtensions`, with self-validation via `Result`/`Maybe`. See [Domain/UserData/ValueTypes.cs](Domain/UserData/ValueTypes.cs) (e.g. `Username : ValueType<string>`, private constructor + static `Create()` factory that chains validators and returns a `Result`).
- **Domain services** — interfaces defined in `Domain/*` (e.g. `ICharacterService`, `IDungeonService`, `IInventoryService`), implemented in the matching `Infrastructure/*` folder. These are closer to application/repository-style services (CRUD + business rules over `UniTask<Result<...>>`) than pure stateless domain logic, but the interface/implementation split is real.
- **Domain events** — plain data classes deriving from `GameEvent` (e.g. `CharacterLevelUpEvent`, `UserItemChangedEvent` in `Domain/Character/CharacterEvents.cs` / `Domain/Inventory`), broadcast through `EventManager.Broadcast(...)`. Note `GameEvent` itself is defined in `Application/Common/EventManager.cs`, i.e. physically in the Application layer despite being the base type Domain events depend on.

## Persistence: Repository pattern (local, not server-backed)

- Abstraction: [Domain/Core/IRepository.cs](Domain/Core/IRepository.cs) — `GetById`, `GetAllAsync`, `Insert`, `Update`, `Delete`, `SaveChangesAsync`, all `UniTask`/`Result`-based.
- Implementation: [Infrastructure/Repository/LocalRepository.cs](Infrastructure/Repository/LocalRepository.cs) — **one generic class used for every entity type**. It keeps an in-memory `List<T>`, serializes an `EntityCollection<T>` to JSON (`Newtonsoft.Json`), and persists it to `PlayerPrefs`. There is no server/database-backed repository implementation in this codebase — `GameManager.AddUserRepositories()` registers `LocalRepository<T>` for each entity (`UserCharacter`, etc.).

## Master data (CSV-driven, not ScriptableObjects)

- `Domain/MasterData/IMasterDataManager.cs` exposes `GetData<T>()` / `GetById<T>(int)`.
- `Infrastructure/MasterData/CsvMasterDataManagerImpl.cs` implements it by reading **CSV files** via `CsvHelper`, keyed by a `Dictionary<Type, string>` of file paths assembled in `GameManager.AddMasterData()` through chained extension methods (`.ConfigureCharacterMasterData()`, `.ConfigureDungeonMasterData()`, …). CSVs live under `StreamingAssets` (per the root project readme).

## Application layer: states, views, popups

Use view controller: In Controller implement state pattern (start scene, update scene, exit scene). View connect with controller (get data). The data in controller get from Infrastructure layer. 

## Cross-cutting: async & eventing

- **UniTask** (`Cysharp.Threading.Tasks`) is the async primitive used everywhere — services, handlers, repositories, resource loaders all return `UniTask`/`UniTask<T>`. There is no UniRx/R3/MessagePipe/`IObservable` anywhere in the codebase.
- **Eventing** is a hand-rolled, static, type-keyed event bus: [Application/Common/EventManager.cs](Application/Common/EventManager.cs) — `EventManager.AddListener<T>(Action<T>)` / `EventManager.Broadcast(GameEvent)`, keyed by `Dictionary<Type, Action<GameEvent>>`. Domain event types are broadcast through this after commands complete, and UI/popups subscribe to react (e.g. refresh currency display after `UserItemChangedEvent`).

## Summary vs. the root `readme.md`

- **DDD**: mostly real — Entities, Value Objects (via `CSharpFunctionalExtensions`), Repository interfaces, and Domain Events are genuine distinct types, not just folder dressing.
- **CQRS**: only the Command side is implemented; the Query side is dead/commented-out code, and the mediator is a thin service-locator wrapper with no pipeline behaviors (validation, logging middleware, etc. beyond the one hardcoded JSON log line).
- **Clean Architecture**: the dependency direction (Domain ← Infrastructure ← Application) is respected, but `Application`'s reliance on `GameManager.Get<T>()` as a global service locator and `EventManager` as a global static bus means most of the Application layer is tightly coupled to these two statics rather than being independently testable.

When adding a new feature, the existing convention to follow is: interface in `Domain/<Feature>`, implementation in `Infrastructure/<Feature>`, registration in `GameManager.ConfigServices()`, and (for writes) a `Command`/`CommandHandler` pair under `Application/<Feature>/Commands` sent through `IMediator`.
