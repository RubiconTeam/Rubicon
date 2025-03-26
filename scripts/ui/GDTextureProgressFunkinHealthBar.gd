class_name GDTextureProgressFunkinHealthBar extends GDFunkinHealthBar

## A Funkin' health bar that utilizes a [TextureProgressBar]

@export var bar : TextureProgressBar ## The [TextureProgressBar] associated with this health bar.

var _left_fill : StyleBox
var _right_fill : StyleBox

func update_bar() -> void:
	bar.ratio = progress_ratio
	
func change_left_color(left_color : Color) -> void:
	bar.tint_progress = left_color

func change_right_color(right_color : Color) -> void:
	bar.tint_under = right_color