# Turn-Based Strategy Course

A Unity turn-based tactics game (hex grid, action-point unit abilities, enemy AI) built with URP and Cinemachine.

## Table of Contents

- [Project Structure](#project-structure)
- [Game Workflow](#game-workflow)
- [Features](#features)
- [Architecture & Design Patterns](#architecture--design-patterns)

## Project Structure

```
Assets/Scripts/
├── Action/   — one component per unit ability (Move, Shoot, Sword, Grenade, Spin, Interact)
├── Grid/     — hex grid data structures and tile visuals
├── UI/       — turn panel, action buttons, world-space unit UI
└── (root)    — turn system, level grid, pathfinding, units, camera, input, AI, VFX, interactables
```

Key root-level files by responsibility:

| Area | Files |
|---|---|
| Turn / flow | `TurnSystem.cs`, `UnitActionSystem.cs`, `EnemyAI.cs`, `EnemyAIAction.cs` |
| Grid / level state | `LevelGrid.cs`, `Pathfinding.cs`, `PathNode.cs`, `PathfindingUpdater.cs`, `UnitVision.cs` |
| Unit | `Unit.cs`, `UnitManager.cs`, `UnitAnimator.cs`, `UnitSelectedVisual.cs`, `UnitRagdoll.cs`, `UnitRagdollSpawner.cs`, `HealthSystem.cs` |
| Camera / input | `CameraController.cs`, `CameraManager.cs`, `InputManager.cs`, `WorldMouse.cs`, `LookAtCamera.cs` |
| Projectiles / VFX | `BulletProjectile.cs`, `GrenadeProjectile.cs`, `ScreenShake.cs`, `ScreenShakeAction.cs` |
| World / interactables | `Door.cs`, `Room.cs`, `LevelScript.cs`, `InteractSphere.cs`, `IInteractable.cs`, `DestructibleCrate.cs` |

Rendering uses URP (`Assets/Settings/*.asset` pipeline/volume profiles) rather than custom post-processing code.

> Note: `Assets/Scripts/App`, `Domain`, `Infrastructure` are empty folders left over from an unused DI-style refactor attempt. `Assets/Plugins/MicrosoftDI` and `UniTask` are present in the project but not referenced by any script.

## Game Workflow

### Boot

All core systems are scene-resident singletons that register themselves in `Awake()`:

1. `LevelGrid` builds a `GridSystemHex<GridObject>` representing the hex grid, then `Pathfinding` raycasts every cell to bake walkability into a parallel `GridSystemHex<PathNode>`.
2. Each `Unit` registers itself into `LevelGrid` on `Start()` and fires the static `Unit.OnAnyUnitSpwaned` event, which `UnitManager` uses to build its friendly/enemy unit lists.
3. `UnitActionSystem` auto-selects a default unit, firing `OnSelectedUnitChanged` for UI and visual listeners.
4. `GridSystemVisual` instantiates one tile visual per grid cell (hidden until an action needs to highlight valid positions).

### Per-Frame Input Loop

`UnitActionSystem.Update()` is the central input router, run every frame:

1. Bail out if busy, not the player's turn, or the pointer is over UI.
2. Raycast for a unit under the mouse — if hit, select it.
3. Otherwise, if a unit is already selected, try to execute its currently selected `BaseAction` at the grid cell under the mouse.

Executing an action: `Unit.TrySpendActionPointToTakeAction()` checks/spends AP → `UnitActionSystem.SetBusy()` → `BaseAction.TakeAction(gridPosition, onComplete)` → the concrete action runs its own state machine to completion → `ActionComplete()` fires `onActionComplete` and the static `BaseAction.OnAnyActionCompleted` event.

`CameraController` handles pan/rotate/zoom independently every frame, regardless of turn state.

### Turn Handling

`TurnSystem` is a minimal two-phase (player/enemy) turn counter with an `onTurnChanged` event. Ending the player's turn is manual, via the UI's end-turn button — nothing auto-advances it.

On the enemy phase, `EnemyAI` runs its own small state machine (`WaitingForEnemyTurn → TakingTurn → Busy`):

1. Iterate every enemy unit's every `BaseAction`.
2. Score each valid `(action, gridPosition)` pair via `BaseAction.GetEnemyAIAction()`.
3. Execute the single best-scoring pair across all units/actions.
4. Repeat until no enemy unit can act, then call `TurnSystem.NextTurn()` to hand control back to the player.

`Unit` refills each unit's action points at the start of its own faction's turn (subscribed to `TurnSystem.onTurnChanged`).

## Features

- **Hexagonal grid** with world↔grid coordinate conversion, occupancy tracking, and tile highlighting for valid move/attack ranges.
- **A\* pathfinding** over a dedicated walkability grid, dynamically updated when doors open/close or crates are destroyed.
- **Action-point unit system** — units carry a budget of action points spent on abilities, refilled each turn.
- **Unit abilities**: Move, Shoot, Sword, Grenade, Spin, Interact — each a self-contained component with its own valid-position query, AP cost, and animation timing.
- **Mouse-driven selection and targeting** with ground-plane raycasting (`WorldMouse`) and a per-unit selection ring visual.
- **Free-look strategy camera** (pan/rotate/zoom via Cinemachine) plus a scripted over-the-shoulder "action camera" that engages automatically during shoot actions.
- **Character animation** driven by ability events (walk, shoot, sword swing) with matching weapon-mesh visibility toggling.
- **Combat & death**: hit-point tracking, ragdoll physics on death (with explosion force), bullet and grenade projectiles (grenades use area damage and can destroy crates).
- **Camera-shake feedback** on shoot/sword-hit/grenade-explode events via Cinemachine impulse.
- **Interactable world objects**: doors (also gate pathfinding) and interact spheres, accessed through a common `IInteractable` interface.
- **Scripted level triggers**: `LevelScript` freezes/unfreezes ambush enemies in specific rooms as doors are opened.
- **Enemy AI** that evaluates and executes the best-scoring action across all its units each enemy turn.
- **In-world and screen-space UI**: turn indicator/end-turn button, per-unit action button bar, "busy" spinner, and floating unit health/name UI.

## Architecture & Design Patterns

**Singleton (hand-rolled `static Instance`)**
Used for nearly every top-level system — `TurnSystem`, `UnitActionSystem`, `LevelGrid`, `Pathfinding`, `UnitManager`, `InputManager`, `GridSystemVisual`, `CameraManager`, `LevelScript`. Cross-system calls go through `X.Instance.Method(...)` rather than injected references; this is the primary mechanism for systems to reach each other.

**Observer / C# events**
Decouples reactive systems from the code that triggers them. Two flavors are used throughout:
- Instance events for a single object's lifecycle, e.g. `TurnSystem.onTurnChanged`, `HealthSystem.OnDead`, `Door.OnOpenDoor`.
- Static "OnAny…" broadcasts for cross-cutting concerns, e.g. `Unit.OnAnyUnitSpwaned/OnAnyUnitDead`, `BaseAction.OnAnyActionStarted/OnAnyActionCompleted`, `ShootAction.OnAnyShoot`.

UI, camera, VFX, and animation systems all subscribe to these instead of being polled or called directly — e.g. `CameraManager` swaps to the action camera purely by listening to `BaseAction.OnAnyActionStarted/Completed`.

**Strategy + Template Method (unit abilities)**
Abstract `BaseAction` defines the contract (`TakeAction`, `GetValidActionPositionList`, `IsValidActionGridPosition`, `GetActionPointCost`, `GetEnemyAIAction`) plus protected `ActionStart`/`ActionComplete` template hooks. Concrete abilities (`MoveAction`, `ShootAction`, `SwordAction`, `GrenadeAction`, `SpinAction`, `InteractAction`) implement it as plain components attached to the unit prefab — **composition over inheritance** for unit capabilities. `Unit.GetAction<T>()` does a simple runtime type lookup over its own components.

**Finite State Machines**
Several classes drive themselves with a private `enum State` + timer, ticked in `Update()`:
- `ShootAction`: Aiming → Shooting → Cooloff
- `SwordAction`: SwingingSwordBeforeHit → SwingingSwordAfterHit
- `EnemyAI`: WaitingForEnemyTurn → TakingTurn → Busy

**Generic data structure reuse**
`GridSystemHex<TGridObject>` is a generic hex-grid container parameterized by a `Func<..., TGridObject>` factory. It's instantiated twice with different payloads: `LevelGrid` uses it for `GridObject` (unit occupancy, interactables), while `Pathfinding` uses a separate instance for `PathNode` (A* walkability/costs).

**MVC-ish UI**
UI classes (`UnitActionSystemUI`, `TurnSystemUI`, `ActionButtonUI`, `ActionBusyUI`, `UnitWorldUI`) never mutate game state directly — they only render current state and call into singleton methods on click (e.g. `ActionButtonUI` calls `UnitActionSystem.Instance.SetSelectedAction(...)`), refreshing themselves by subscribing to the events above.

**Interface-based polymorphism**
`IInteractable`, implemented by `Door` and `InteractSphere`, is looked up per grid cell and invoked generically by `InteractAction` without the action needing to know the concrete type.

**Callback-based completion (no coroutines/async)**
Action and interaction completion use plain `Action` delegate callbacks (`onActionComplete`, `onInteractComplete`) chained through `BaseAction.ActionStart`/`ActionComplete`, rather than coroutines or UniTask.

No ScriptableObjects are used for data — all tunables are `[SerializeField]` fields on MonoBehaviours.
