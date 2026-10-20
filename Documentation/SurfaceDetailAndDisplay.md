# Prop surfaces, lighting and display selection

## Visible display options

Press Escape to pause. The pause box now has separate 1280×720 and 1920×1080 buttons, highlights the selected output, and shows the active resolution. F1 remains a shortcut. Standalone output remains borderless fullscreen; the selection is remembered for the next launch. First launch defaults to 720p. Editor tests do not write display preferences.

## Object detail

- 347 eligible primitive prop meshes now have subtle 18mm maximum bevels, matched to their dimensions. Each repeated size shares a generated mesh. Walls, floors, signs, gameplay indicators, actors and existing imported models are preserved.
- 23 wooden crate blocks receive steel bands and fasteners; 26 barrels receive shaped reinforcing rings.
- Small hardware is combined into at most one mesh per room, with no colliders or extra shadow casting. Existing gameplay colliders and transforms are unchanged.
- Uniforms/vests, barrels, forklifts and wrapped cargo receive appropriate existing albedo/normal surface maps. Skin and leather specularity is tempered.
- Source textures retain mipmaps and use trilinear filtering plus 8× anisotropic filtering; full-resolution mip level is enabled. Import quality is raised without upscaling source images.
- Room readability fills are slightly stronger. Static lighting is rebaked for the beveled geometry. Five room reflection probes increase from 128px to 256px and remain baked.

This is a broad surface/prop-family upgrade, not a replacement of every asset with a new high-poly model. It preserves the existing warehouse layout and agent behavior.

Reapply through `DetailWarehouseSurfaces.Execute`. The generated meshes are under `Assets/Meshes/SurfaceDetail`; decorative hardware is grouped under `Warehouse_Surface_Details`.

## Verification

`DisplayAndSurfaceTests` checks both pause buttons, pointer hit routing, pause/resume, batching, collider isolation, 300+ beveled props, filtering and baked probe resolution. Batch UI previews temporarily render the same canvas through the player camera into a 1920×1080 target; the live game uses its normal canvas mode. Existing walkthrough and combat suites verify gameplay stability.

Final validation: 57/57 tests passed in the complete PlayMode suite. The pause menu was inspected in its rendered 1080p preview. Both route walkthroughs, guard coordination, traps, health, shooting, HUD and collision regressions passed.

Windows release build succeeded with zero errors; one existing unused-field warning remains. Standalone startup confirmed borderless fullscreen at 1280×720 and exited normally without exceptions.
