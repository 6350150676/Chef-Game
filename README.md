# Yes Chef!

A one-person kitchen game built for the Tentworks developer test.

**Unity 6000.2.6f2** · URP · Input System · uGUI + TextMeshPro · no third-party plugins.
Built for **desktop at 16:9** (1920x1080 default): a fixed top-down camera with the whole kitchen in frame and no zoom.

## Running

Open `Assets/_Project/Scenes/Kitchen.unity` and press Play. Set the Game view to 16:9 or Full HD.

| Action   | Keyboard            | Gamepad      |
|----------|---------------------|--------------|
| Move     | WASD / Arrow keys   | Left stick   |
| Interact | E / Space           | South button |
| Pause    | Esc / P             | Start        |

Grab an ingredient from the fridge, chop veggies at the table or cook meat on the stove, and serve it at a window whose ticket needs it. The trash clears whatever you're holding.

## Project layout

```
Assets/_Project
├── Data         ScriptableObject assets: ingredients, game config, order generator, ingredient factory
├── Input        ChefInputActions (Gameplay map: Move, Interact, Pause)
├── Materials, Textures, Prefabs, Scenes
├── UI           Sprite kit (9-sliced panels, buttons, icons) and TextMeshPro fonts with outline/shadow presets
├── Scripts      YesChef assembly
│   ├── Core          GameManager (state + round clock), GameConfig, CountdownTimer, CameraFramer
│   ├── Ingredients   IngredientDefinition, Ingredient, IngredientFactory, IIngredientHolder, PreparationMethod
│   ├── Orders        Order, OrderManager, CustomerWindow, OrderGenerator / RandomOrderGenerator
│   ├── Scoring       ScoreManager, IHighScoreRepository / PlayerPrefsHighScoreRepository
│   ├── Stations      InteractableStation, InteractionPrompt, ProcessingStation, ProcessingSlot,
│   │                 RefrigeratorCompartment, TrashBin
│   ├── Player        InputReader, PlayerController, PlayerMovement, PlayerInteractor, PlayerHands
│   ├── Presentation  World feedback: StationFeedbackView, KitchenGuideView, ChefView, CustomerView,
│   │                 SlotActivityView, TransformTweener, Bobber
│   └── UI            UIManager, HUDView, OrdersPanelView, OrderUI, IngredientIconUI, ScorePopupUI,
│       └── Motion    InteractionPromptView, HandsHintView, FloatingTextSpawner, RoundBannerView, ...
└── Tests        EditMode (rules) and PlayMode (full scene) suites
```

## Architecture

**Game flow.** `GameManager` is a small state machine (MainMenu → Playing ⇄ Paused → GameOver) that owns the round clock. It raises `RoundStarted`, `RoundEnded` and `StateChanged` and doesn't reference the systems that react to them. `OrderManager`, `ScoreManager`, the stations and the chef each subscribe and reset themselves.

**Orders.** `Order` is plain C# and holds the rules: delivery matching (duplicates allowed), aging, and `value − floor(seconds)` scoring. `OrderManager` runs the lifecycle for each window: fill every window at round start, age the orders, score completions, and respawn after 5 seconds. `CustomerWindow` only accepts deliveries for its current order.

**Stations.** Every station extends `InteractableStation`. `Interact` is a template method: the station evaluates what would happen, performs it only if it is allowed, then reports the outcome. That evaluation is also what the UI asks for, so the on-screen prompt and the action can never disagree. The chopping table and the stove are the same `ProcessingStation` component configured differently (method, slot count, duration, whether the chef must stay), so a new station such as an oven needs no new code.

**Chef.** `PlayerController` composes the chef's parts. `InputReader` turns input actions into intents, `PlayerMovement` moves a `CharacterController`, `PlayerInteractor` targets the nearest station in front of the chef, and `PlayerHands` carries one ingredient. Stations interact through `IIngredientHolder`, not the player class.

**UI.** The Canvas hierarchy (StartPanel, GameHUD with OrdersPanel/OrderUI_1–4, PausePanel, GameOverPanel) and the station progress bars are authored in the scene. UI scripts are a view layer. They listen to events, read state for values that change every frame (timers, progress), and forward button presses to `GameManager`. No gameplay rules live in UI code. Each `OrderUI` fills its `IngredientsContainer` from the order data and reuses icon instances between orders.

The look is built from a sprite kit in `UI/Sprites`: 9-sliced cards, beveled buttons with normal/highlighted/pressed sprites, order tickets and ingredient, clock, star and trophy icons. The sprites are authored at 2x (200 pixels per unit), so they stay sharp above 1080p. Text uses two OFL fonts with TMP material presets for the outline and drop shadow: Lilita One for headings and numbers, Fredoka for body text. Each ingredient's icon comes from its `IngredientDefinition`, so adding an ingredient still needs no UI code.

Motion comes from four small components in `UI/Motion`. `UIPopIn` animates panels, tickets and badges as they appear. `UIPunchScale` bumps the score and the last-seconds clock. `UIButtonFeedback` handles hover, press and selection. `UIIdleMotion` drives the decorative spin, pulse and bob. They all run on unscaled time, so the pause menu still animates. World objects use `Presentation/TransformTweener` for the same effects plus a shake, kept separate from the UI components because stations, customers and the chef are not `RectTransform`s.

## Telling the player what to do

The kitchen coaches the player instead of relying on the player remembering the rules:

- **Interaction prompt.** A pill above the station in front of the chef shows what Interact will do (`[E] Chop Veggie`, `[E] Serve Cooked Meat`) or why it won't (`Needs cooking first`, `Stove is full`, `Not on this order`). It reads the same evaluation the station uses, so it is never wrong.
- **Guide markers.** Arrows bob over the stations that move the current task forward: where the held ingredient can go, food waiting to be collected, or the fridge compartments an open order still needs.
- **Hint bar.** The bottom bar names what is in hand and the next step for it, or points at the fridge when the hands are empty.
- **Callouts and reactions.** "Chopped!"/"Cooked!" pop over a finished slot, the burner glows and the knife moves while working, stations punch when used and shake when refused, customers redden as they wait and hop when served, and the score popup shows the points an order earned.

## SOLID

| Principle | Where it shows up |
|---|---|
| Single responsibility | Separate managers for game state, orders, and score. The chef is split into input, movement, interaction and hands. Views only present. |
| Open/closed | New ingredients are assets. New stations are configured `ProcessingStation`s. New systems subscribe to round events without changing `GameManager`. |
| Liskov substitution | Any `InteractableStation` or `OrderGenerator` works wherever its base is expected. |
| Interface segregation | `IInteractable`, `IFocusable` and `IIngredientHolder` are small and separate. |
| Dependency inversion | Stations depend on `IIngredientHolder`. Score persistence goes through `IHighScoreRepository`. Order creation goes through the `OrderGenerator` abstraction. |

## Patterns

| Pattern | Used for |
|---|---|
| Observer | C# events connect game flow, orders, score and UI |
| State machine | `GameManager` game states, with guarded transitions |
| Template method | `InteractableStation.Interact`: evaluate, perform, report |
| Strategy | `OrderGenerator` ScriptableObject: swap the asset to change how orders are generated |
| Factory | `IngredientFactory` is the one place ingredients are built (the natural hook for pooling) |
| Repository | `IHighScoreRepository` hides where the high score is stored |
| Composition / mediator | `PlayerController` coordinates independent chef components |
| Data-driven config | `IngredientDefinition` and `GameConfig` ScriptableObjects |

## Design decisions

- **The refrigerator has three compartments** (Veggie, Cheese, Meat). You stand at the one you want, so a single Interact button covers everything.
- **Chopping requires the chef to stay at the table.** The spec calls out that the stove can be left alone, which suggests the table can't. The bar turns grey and the knife stops while chopping is paused. Untick `requiresAttendance` on the table to change this.
- **Ingredients can only be taken off a station once they're ready**, and only ingredients that need that station can be placed on it.
- **Wrong or unprepared ingredients stay in hand** when used on a window, as the spec requires. The prompt says why and the station shakes.
- **Order tickets** show the wait time, the points the order is still worth, and a patience bar that drains from green to red as the wait eats into its value, so it's clear which orders to serve first.
- **The round clock** has a draining ring. In the last 30 seconds it turns red and pulses every second, and a banner calls out the final seconds.
- **Menus support keyboard and gamepad.** The main button of the start and pause menus is focused when they open. The Game Over panel focuses Play Again only after the score count-up, so mashing Interact as the round ends can't restart it straight away.
- **Pause** sets `Time.timeScale = 0`. All timers tick on scaled time, so the round, orders, cooking and popups freeze together.
- **The HUD stays visible** behind the Pause and Game Over panels, dimmed. Both panels have their own Quit button.
- **High score** is stored in PlayerPrefs. A new high score must beat the stored value, which starts at 0.
- **Camera.** A fixed top-down orthographic camera, as the brief asks: no movement, no zoom, whole kitchen in frame. `CameraFramer` keeps it framed at any aspect ratio, and the layout is tuned for 16:9. Order tickets follow their windows through `UIWorldAnchor`.

## Tests

Open **Window → General → Test Runner**.

- **EditMode:** order scoring (including the spec's 14.99s → 26 points example), negative scores, duplicate ingredients, timers, and the 2/3-ingredient order generator.
- **PlayMode:** loads the Kitchen scene and plays through it: the round opens four orders with their UI; chopping, cooking and serving a full order scores it and respawns the window; unprepared food is rejected and can be trashed; prompts report what each station will do and why an action is blocked; pausing freezes the clock.

## Next steps toward a full game

- Pool ingredients and score popups behind `IngredientFactory` and `ScorePopupUI`.
- Pack `UI/Sprites` into a Sprite Atlas to cut UI draw calls. The sprite packer is currently disabled in Project Settings.
- Hook audio and VFX onto the existing events (`OrderCompleted`, `StateChanged`, slot completion).
- Add a difficulty-ramping `OrderGenerator`, plus burn timers as a second stage of `ProcessingSlot`.
- Move to a save system beyond PlayerPrefs through `IHighScoreRepository`.

## Credits

- [Lilita One](https://fonts.google.com/specimen/Lilita+One) by Juan Montoreano and [Fredoka](https://fonts.google.com/specimen/Fredoka) by the Fredoka Project Authors, both under the SIL Open Font License (license files in `Assets/_Project/UI/Fonts`).
