# Cinematic warehouse visual pass

The visual direction uses cool surveillance lighting against warmer industrial pools, worn steel, rough concrete and red emergency accents. Gameplay layout, health, sound perception, traps and navigation remain unchanged.

## Assets and rendering

- Original beveled electrical enclosure: `Assets/Meshes/Source/IndustrialServicePanel.blend`, exported to `Assets/Meshes/Props/IndustrialServicePanel.fbx`. Captive bolts, inset door, vent slats, hinges, handle and indicator lenses are modeled geometry.
- Reproduce the Blender art using `Documentation/Tools/generate_service_panel.py` inside Blender.
- Panels and wall conduits under `Cinematic_Visual_Details` have no colliders or NavMesh obstacles.
- Existing albedo and normal textures now have packed metallic/occlusion/smoothness maps. Concrete stays rougher than steel; wood remains matte.
- Overhead fixtures retain their original orientation. Independent downward lights sit below the lamp housings.
- Static lighting uses CPU baking, 1024 atlases, 8 texels/metre and two indirect bounces. Five 128px baked reflection probes avoid realtime cubemap capture.
- ACES tone mapping, restrained bloom, warm highlights/cool shadows, mild haze, FXAA and a small vignette. Film grain is subtle; motion blur and depth-of-field are not added to aiming.
- Weapon barrels use recalculated smooth normals; gunmetal normal maps, matte polymer and AK wood grips replace the uniform imported finishes. Guard face fills are reduced to avoid blown highlights.
- Five weak realtime room fills improve character/readability without additional shadow maps.
- Three shadow-casting lights retained; cascades reduced from four to two. Resolution/quality controls remain available.
- These are conventional URP lighting/reflection techniques; no hardware ray tracing or FPS guarantee is implied.

Reapply and bake with `Warehouse/Apply Cinematic Visual Upgrade` (`ApplyCinematicWarehouse.Execute`). It replaces its own decoration root and assigns explicit settings rather than accumulating duplicates.

## Verification

Matching 1920x1080 environment views are captured through the real player camera in all five rooms. The capture excludes screen-space overlay HUD and freezes combat for inspection only; normal game behavior is unchanged. Saved under `scratch/visual-upgrade/before` and `after`.

`VisualQualityCaptureTests` checks visible room renders, baked lighting/reflections, decoration collision isolation and the three-shadow-light budget. Run the existing walkthrough suite to check both escape routes, combat, traps, squad coordination, controls and HUD.

Final validation: 55/55 PlayMode tests passed in the full run, including both card routes, traps, coordinated bosses, weapon handling, controls, HUD, collider regressions and visual captures.

Windows release build succeeded with zero errors and zero warnings. Standalone startup smoke check exited normally with no exceptions.
