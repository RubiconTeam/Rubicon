class_name CharacterHold

## Determines how a character holds notes animation-wise.

const NONE : int = 0 ## The character will just not hold a note at all, similar to V-Slice.
const REPEAT : int = 1 ## The character will repeat on a set interval.
const STEP_REPEAT : int = 2 ## The character will repeat their animation every step, similar to every other Funkin' engine.
const FREEZE : int = 3 ## The character will freeze in place until the hold note is completed.