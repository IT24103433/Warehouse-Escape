# Weapon audio

The files in `Assets/Resources/Audio/Weapons` are edited CC0 recordings, prepared
on 19 October 2026. No sounds were taken from IGI or another commercial game.

## Firearm reports

[The Free Firearm Sound Library](https://opengameart.org/content/the-free-firearm-sound-library)
by Ben Jaszczak, Brian Nelson, Kevin Heras, and Matthew Nanney. The source page
and authors' notice release the library under
[CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/).

| Game weapon | Source recording | Treatment |
| --- | --- | --- |
| MP5 | Carl Gustav M45, G_20P.wav | 9mm approximation, slightly faster |
| UMP | 1911, A_42P.wav | .45 approximation, slightly slower |
| AK-47 | AK-47, C_28P.wav | Actual AK-47 report |
| SCAR | AR-15, D_32P.wav | Rifle approximation, slightly slower |
| Shotgun | Mossberg shotgun, N_26P.wav | First recorded report at original pitch/timing, 2.2-second natural tail |

These are designed gameplay effects, not exact recordings of every named model.
Stereo 96kHz/24-bit sources were mixed to mono, trimmed to a single onset,
resampled to 44.1kHz/16-bit, attenuated through the tail, normalized below full
scale, and faded at boundaries. Natural report tails intentionally overlap
successive automatic shots; they must not be cut to the weapon's fire interval.

## Mechanical reloads and handling

[Gun reload sounds](https://opengameart.org/content/gun-reload-sounds)
by SpringySpringo, CC0 1.0. The author describes the sources as airsoft recordings.
`gunreload1.wav`, `assaultriflereload1_0.wav`, and `shotguncock_0.wav` provide
recorded magazine and action sounds. Variants are fitted to each existing
reload duration with a small completion margin. Shotgun shell handling is
assembled from short mechanical clicks followed by the recorded pump action.
Empty-trigger and weapon-swap cues use short excerpts of the same handling audio.

The short hit-marker tone remains a synthesized interface cue.

Only the edited gameplay WAV files belong in the game. Original downloads and
the preparation script are kept in the ignored local `scratch` folder. The
uncompressed gameplay WAV files total less than 1 MB.

20 October shotgun update: `Documentation/Tools/prepare_original_shotgun.py` extracts the first report from the original downloaded recording. Only leading silence removal, mono conversion, sample-rate conversion, safe gain and an 8ms end fade are applied. No slowing, pitch shift, synthetic layer or exponential tail damping. Guard shotgun playback uses pitch 1.0. The original file contains three separate reports; the shipped clip contains only the first.
