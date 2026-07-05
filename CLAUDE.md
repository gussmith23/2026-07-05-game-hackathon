# Working in this Unity project

## Static data over runtime code

If something about an object's appearance or layout can be baked in as serialized
data (Transform position/scale, `SpriteRenderer`/`MeshRenderer` fields, a Material's
own color), do that instead of setting it in `Awake()`/`OnValidate()`/`Start()`.

Code-set state only exists once that code has run. Concretely:
- `Awake()`/`Start()` don't run in Edit mode, only Play mode — anything set up there
  is invisible until you press Play, which makes Edit mode and Play mode disagree
  about what a scene actually looks like.
- `MeshRenderer.SetPropertyBlock(...)` (`MaterialPropertyBlock`) is runtime-only and
  is **not serialized** into the scene. It survives until the next domain reload,
  then silently reverts to the shared material's own base color. Use a real
  `Material` asset (or a per-color material asset) instead of a property block for
  anything that needs to persist.
- A `UnityEvent.AddListener(...)` call made in `Awake()` is a live C# delegate, not
  serialized data. A domain reload during Play mode (e.g. from editing/recompiling
  a script while playing) wipes it without re-running `Awake()`, silently breaking
  UI button wiring etc. If a click handler mysteriously stops firing mid-session,
  check whether a script changed while still in Play mode.

Rule of thumb: if you can point at the exact Inspector field that holds a value,
it's fine. If a script has to run first to produce that value, ask whether it
could just be authored/baked in instead.

## Editor tooling and scaffolding scripts

- One-shot scripts that mutate a scene/asset once and are done (migrations, bulk
  conversions, scaffolding a new scene) should **not** live permanently in
  `Assets/Editor/`. Their job is to produce data; once that data is committed in
  the scene/asset files, the script itself has no ongoing purpose and is just
  noise (or a hazard, if it's the kind that wipes/rebuilds a scene from scratch).
- Coplay's `execute_script` tool will run a `.cs` file from **any path**, not just
  files tracked by the Unity project — it compiles on the fly. Prefer keeping
  disposable/one-off tooling scripts outside the repo (a scratch directory ignored
  via `.git/info/exclude`, not the shared `.gitignore`) so they never need to be
  committed or reviewed by anyone else, and clean them up once their result is
  baked into checked-in data.
- For a single object, prefer direct granular tool calls (create object, add
  component, create material, set a field) over writing a reusable helper class
  for a one-time task. Only reach for a small shared utility when the same
  operation is genuinely needed repeatedly.
- Never let a scaffolding script call something equivalent to
  `EditorSceneManager.NewScene(...)` on a scene that has since been hand-edited —
  re-running it will silently discard manual changes (moved/resized objects, added
  GameObjects) with no warning.

## Coplay-specific quirks observed

- `save_scene` has occasionally written a duplicate `.unity` file to `Assets/`
  (project root) instead of the scene's real path — check for and delete stray
  root-level scene files after using it, then reopen the canonical scene.
- If the project's Active Input Handling is set to "Input System Package" only,
  the legacy `UnityEngine.Input` class throws at runtime. Use
  `UnityEngine.InputSystem.Keyboard.current` etc. instead.
- `check_compile_errors` / `open_scene` occasionally return a transient error
  immediately after a Play-mode stop/start or heavy script changes; check editor
  state and retry once before treating it as a real failure.

## Unity `.gitignore` basics

Ignore `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Obj/`, `Build*/`, and IDE/
solution cruft (`*.sln`, `*.csproj`, `.vs/`) — all regenerated automatically.
**Never** ignore `.meta` files under `Assets/` — they hold the GUIDs Unity uses
to track cross-references; losing them breaks every reference in every scene/
prefab for anyone else who clones the repo. `ProjectSettings/` and
`Packages/manifest.json` / `packages-lock.json` are real config, not generated
output — they must be committed.
