class_name GameEnvironment

## Which type of environment the song's stage will spawn in.

const NONE : int = 0 ## Will not spawn a stage at all.
const CANVAS_ITEM : int = 1 ## Spawns a stage that can only display [Node]s deriving from [CanvasItem].
const SPATIAL : int = 2 ## Spawns a stage that can only display [Node]s deriving from [Node3D].