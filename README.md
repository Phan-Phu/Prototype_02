# Turn-Based Strategy

A Unity turn-based tactics game (hex grid, action-point unit abilities, enemy AI) built with URP and Cinemachine.

## Table of Contents

- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Game Workflow](#game-workflow)
- [Features](#features)
- [Architecture & Design Patterns](#architecture--design-patterns)
- [Conventions](#conventions)

## Getting Started

Always start Play mode from **`Assets/Scenes/InitScene.unity`** (build index 0). It hosts the system objects the gameplay scenes depend on and loads `GameScene` from its start button. Pressing Play directly in `GameScene` / `GameScene_Hex` fails because those system objects are missing.

| Scene | Contents |
|---|---|
| `InitScene` | System objects: `GameManager`, `TurnSystem`, `InputManager` (all `DontDestroyOnLoad`) |
| `GameScene` | The playable level: grid, units, camera, UI, level script |
| `GameScene_Hex` | Alternate copy of the level |

## Project Structure

```
Assets/Scripts/
├── Domain/             engine-free rules: value objects, events, service contracts
│   ├── Common/         IValueObject, GameEvent, ICommand/ICommandHandler/IMediator, IInteractable
│   ├── Grid/           GridPosition, IGridSystemHex<T> (storage, bounds, hex distance/neighbours)
│   └── Turn/           Turn (value object), ITurnService, TurnChangedEvent
├── Infrastructure/     implementations of Domain contracts (may use UnityEngine)
│   ├── CQRS/           MediatorImpl
│   ├── Grid/           GridSystemHex<T>, IGridSystemHexWorld<T>, IGridSystemHexFactory
│   └── Turn/           TurnServiceImpl
├── AStarPathfinding/   standalone, grid-agnostic A* library (AStarSearch, IAStarGrid<T>) - outside the layers
└── Application/        Unity-facing layer, one folder per feature (MVVM inside, see below)
    ├── Turn/
    │   ├── ViewModel/  TurnSystem
    │   └── View/       TurnSystemUI
    ├── UnitAction/
    │   ├── ViewModel/  UnitActionSystem (selection, busy state, player input routing)
    │   ├── View/       UnitActionSystemUI, ActionButtonUI, ActionBusyUI
    │   ├── Events/     SelectedUnitChangedEvent, SelectedActionChangedEvent, BusyChangedEvent
    │   └── Commands/   SelectUnit, SpendActionPoint (+ handlers)
    ├── Unit/
    │   ├── ViewModel/  Unit, UnitManager, HealthSystem
    │   ├── View/       UnitWorldUI, UnitAnimator, UnitRagdoll, UnitRagdollSpawner, UnitSelectedVisual, LookAtCamera
    │   └── Events/     UnitSpawnedEvent, UnitDiedEvent, UnitActionPointsChangedEvent, HealthDamagedEvent, HealthDepletedEvent
    ├── Grid/
    │   ├── ViewModel/  LevelGrid, GridCell
    │   ├── View/       GridSystemVisual, GridSystemVisualSingle
    │   └── Events/     UnitGridPositionChangedEvent
    ├── Action/
    │   ├── ViewModel/  BaseAction, Move/Shoot/Sword/Grenade/Interact/Spin* actions, GrenadeProjectile
    │   ├── View/       BulletProjectile
    │   └── Events/     Action/Move/Shoot/Sword events, GrenadeExplodedEvent
    │                   *SpinAction exists but is not on the unit prefabs yet
    ├── Level/          Door, DestructibleCrate, LevelScript, Room
    │   └── Events/     DoorStateChangedEvent, CrateDestroyedEvent
    ├── Camera/         CameraController, CameraManager, ScreenShake, ScreenShakeAction
    ├── Input/          InputManager, WorldMouse, PlayerInputActions (+ .inputactions asset)
    ├── Pathfinding/    Pathfinding (walkability map + hex adapter over AStarPathfinding), PathfindingUpdater
    ├── AI/             EnemyAI, EnemyAIAction
    ├── Common/         GameManager (DI composition root), EventManager (event bus)
    └── Debug/          test/debug-only scripts, not used by the shipped game (see below)
```

### Application folder rules

- **One folder per feature/entity** (`Turn`, `Unit`, `Grid`, …). Inside it, MVVM:
  - **Model** is the Domain + Infrastructure layers, so there is no `Model/` folder in Application.
  - **`ViewModel/`** — scene systems and bridges to the Model that hold state for Views (e.g. `TurnSystem`, `Unit`, `LevelGrid`). Gameplay rules that still need Unity (actions, health, AP) live here for now; pure rules can move down to Domain later.
  - **`View/`** — UI and visual effects only; they display ViewModel state and forward player input.
  - **`Events/`** — one file per `GameEvent`, kept in the feature that raises it.
  - **`Commands/`** — mediator commands and handlers for state-changing requests.
- Small features without UI (`Level`, `Camera`, `Input`, `Pathfinding`, `AI`) stay flat; only `Events/` is split out.

### Debug scripts

`Application/Debug/` holds scripts used only for testing layouts and systems. None of them is placed on a unit or created by gameplay code:

| Script | Purpose |
|---|---|
| `GridSystemDebugSpawner`, `IGridDebugVisual`, `GridDebugObject`, `PathfindingGridDebugObject` | Per-cell debug labels (cell contents, walkability). Their prefabs are assigned on `LevelGrid` / `Pathfinding` but nothing spawns them yet. |
| `InteractSphere` | Test `IInteractable` that toggles colour; not placed in any scene. |
| `UnitVision` | Line-of-sight helper prototype. |
| `Testing` | Scratch MonoBehaviour for manual tests. |

## Game Workflow

### Boot

1. `InitScene` loads. `GameManager` (execution order -1000) builds the DI container, then `TurnSystem` and `InputManager` initialise. All three survive scene loads.
2. The start button calls `GameManager.ChangeSceneName("GameScene")`.
3. In `GameScene`, `LevelGrid` creates the hex grid of `GridCell`s (via `IGridSystemHexFactory`), and `Pathfinding` raycasts every cell to bake a walkability map.
4. Each `Unit` registers itself in `LevelGrid` and broadcasts `UnitSpawnedEvent`; `UnitManager` builds the friendly/enemy lists from it.
5. `UnitActionSystem` selects the default unit (`SelectedUnitChangedEvent`), and `GridSystemVisual` spawns one tile visual per cell.

### Player Turn

`UnitActionSystem.Update()` routes input every frame:

1. Bail out if busy, not the player's turn, or the pointer is over UI.
2. Clicking a friendly unit sends a `SelectUnitCommand` through the mediator.
3. Otherwise, clicking a valid cell for the selected action sends a `SpendActionPointCommand`; on success the system goes busy (`BusyChangedEvent`) and calls `BaseAction.TakeAction(gridPosition, onComplete)`.
4. The action runs its own state machine and finishes with `ActionComplete()`, which clears busy and broadcasts `ActionCompletedEvent`.

The End Turn button calls `TurnSystem.NextTurn()`.

### Turn Handling

`TurnSystem` wraps `ITurnService` (resolved from DI) and broadcasts `TurnChangedEvent` whenever the turn advances. `Unit` refills its action points at the start of its own side's turn.

On the enemy turn, `EnemyAI` runs a small state machine (`WaitingForEnemyTurn → TakingTurn → Busy`): for every non-frozen enemy it scores each `(action, gridPosition)` pair via `BaseAction.GetEnemyAIAction()`, executes the best one, and repeats until no enemy can act, then calls `TurnSystem.NextTurn()`.

## Features

- **Hexagonal grid** ("odd-r" layout) with grid↔world conversion, occupancy tracking, and tile highlighting for valid move/attack ranges.
- **A\* pathfinding** with a hex-distance heuristic, updated when doors open/close or crates are destroyed.
- **Action-point system** — abilities cost AP, refilled each turn.
- **Unit abilities**: Move, Shoot, Sword, Grenade, Interact.
- **Mouse-driven selection and targeting** with a per-unit selection ring.
- **Strategy camera** (pan/rotate/zoom via Cinemachine) plus an over-the-shoulder action camera during shooting.
- **Combat & death**: health, ragdolls with explosion force, bullet and grenade projectiles, destructible crates.
- **Camera shake** on shoot, sword hit and grenade explosion.
- **Interactables**: doors (which also gate pathfinding) via `IInteractable`.
- **Scripted rooms**: `LevelScript` freezes/unfreezes enemies as their room's door opens or closes.
- **Enemy AI** picking the best-scoring action across all enemies each turn.

## Architecture & Design Patterns

### Layers

`Application → Infrastructure → Domain`. Domain has no UnityEngine dependency; anything that needs Unity types (e.g. grid↔world conversion with `Vector3`) lives in Infrastructure.

> **Note — single assembly by design.** All layers compile into one assembly (`Scripts.asmdef`), so the layer boundaries are enforced by convention and code review, not by the compiler. Splitting into per-layer asmdefs (`Domain`, `Infrastructure`, `Application`) is deliberately postponed: the project is small and the split would add setup cost without much benefit yet. Revisit it if the codebase grows or more people work on it.

### Dependency access rules

| What | How to reach it |
|---|---|
| Scene MonoBehaviour systems (`LevelGrid`, `Pathfinding`, `UnitActionSystem`, `UnitManager`, `TurnSystem`, `InputManager`, …) | static `X.Instance` |
| Plain C# services (`ITurnService`, `IGridSystemHexFactory`, `IMediator`) | `GameManager.Instance.Get<T>()` |
| State-changing player requests | `IMediator.Send<TCommand, TResult>(...)` |
| Notifications between systems | `EventManager` |

**Dependency Injection** — `GameManager` is the composition root (`Microsoft.Extensions.DependencyInjection`). Only plain C# services are registered; scene objects are never put in the container because it outlives scene loads.

**Command / Mediator (CQRS write side)** — commands (`SelectUnitCommand`, `SpendActionPointCommand`) are sent through `IMediator`; `MediatorImpl` resolves the matching `ICommandHandler<TCommand, TResult>` from the injected `IServiceProvider`. Callers use `async UniTaskVoid` + `.Forget()`, never `async void`.

**Event bus** — `EventManager` is a static, type-keyed bus. Every game event is a class deriving from `GameEvent` (e.g. `TurnChangedEvent`, `UnitDiedEvent`, `ShootEvent`, `DoorStateChangedEvent`). Listeners that only care about one object filter on the event payload (e.g. `@event.Unit != unit`). There are no C# `event` fields for game events.

**Strategy + Template Method (unit abilities)** — abstract `BaseAction` defines `TakeAction`, `GetValidActionPositionList`, `IsValidActionGridPosition`, `GetActionPointCost`, `GetEnemyAIAction`, plus `ActionStart`/`ActionComplete` hooks. Concrete abilities are components on the unit prefab (composition over inheritance); `Unit.GetAction<T>()` looks them up.

**Finite State Machines** — `ShootAction` (Aiming → Shooting → Cooloff), `SwordAction` (SwingingSwordBeforeHit → SwingingSwordAfterHit), `EnemyAI`.

**Domain modelling** — `GridPosition` and `Turn` are Value Objects (immutable, compared by value, marked `IValueObject`). Application classes that hold Unity objects, such as `GridCell` (units and interactable on a cell), never derive from Domain types.

**Generic grid** — `GridSystemHex<T>` is created through `IGridSystemHexFactory`; `LevelGrid` stores one `GridCell` per cell and exposes hex distance/neighbours to the rest of the game.

**Standalone A\*** — `AStarPathfinding/` is a separate, grid-agnostic library (no Domain or Unity dependency) and sits outside the Clean Architecture layers. `Pathfinding` keeps the walkability map and adapts the hex grid to `IAStarGrid<GridPosition>` (neighbours and hex-distance heuristic come from `LevelGrid`).

## Known Performance Notes

Not optimised on purpose — the maps and unit counts are small, so none of these is a problem today. Revisit them if levels grow or the enemy turn starts to stutter.

| Where | What | Possible fix |
|---|---|---|
| `MoveAction.GetValidActionPositionList` | Runs A\* twice per candidate cell (`HasPath` + `GetPathLength`), up to 121 cells per call. | Run one search per cell, or a single Dijkstra/BFS flood fill from the unit. |
| `GridSystemVisual` | Recomputes the selected action's valid cells on every `UnitGridPositionChangedEvent`, including enemy moves. | Only refresh on the player's turn, or when the selected unit/action changes. |
| `EnemyAI` + `MoveAction`/`GrenadeAction.GetEnemyAIAction` | Scores every reachable cell by calling `ShootAction.GetValidActionPositionList` (with raycasts) from it, for every enemy and every decision. | Cache per turn, or score only cells near player units. |
| `AStarSearch` | Open list is a `List<T>` (`Contains`/`Remove`/min scan are O(n)); `GetNeighbours` allocates a new list per node. | Priority queue + `HashSet` for membership; reuse a neighbour buffer. |
| `GridSystemVisualSingle.Show`, `InteractSphere` | Assigning `renderer.material` creates a new material instance each time. | Use `sharedMaterial`. |
| `InputManager` | `PlayerInputActions` is never disabled/disposed (lives for the whole app via `DontDestroyOnLoad`). | `Disable()` + `Dispose()` in `OnDestroy`. |

## Conventions

- Event classes: `<Subject><Something>Event`, deriving from `GameEvent`.
- Event handlers: `On` + event class name, parameter `@event` — e.g. `private void OnTurnChangedEvent(TurnChangedEvent @event)`.
- Every `EventManager.AddListener` is paired with `RemoveListener` (`OnEnable`/`OnDisable`, or `Start`/`OnDestroy` for objects that deactivate themselves).
- No underscores in names, except `static`/`const` values written as `UPPER_SNAKE_CASE`.
