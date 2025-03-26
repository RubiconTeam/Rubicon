class_name GDProgressFunkinHealthBar extends GDFunkinHealthBar

## A Funkin' health bar that utilizes a [ProgressBar]

@export var bar : ProgressBar ## The [ProgressBar] associated with this health bar.

var _left_fill : StyleBox
var _right_fill : StyleBox

func _ready() -> void:
	_left_fill = bar.get_theme_stylebox("theme_override_styles/fill")
	_right_fill = bar.get_theme_stylebox("theme_override_styles/background")

func update_bar() -> void:
	bar.ratio = progress_ratio
	
func change_left_color(left_color : Color) -> void:
	if _left_fill is StyleBoxFlat:
		(_left_fill as StyleBoxFlat).bg_color = left_color
	elif _left_fill is StyleBoxLine:
		(_left_fill as StyleBoxLine).color = left_color
	elif _left_fill is StyleBoxTexture:
		(_left_fill as StyleBoxTexture).modulate_color = left_color

func change_right_color(right_color : Color) -> void:
	if _right_fill is StyleBoxFlat:
		(_right_fill as StyleBoxFlat).bg_color = right_color
	elif _right_fill is StyleBoxLine:
		(_right_fill as StyleBoxLine).color = right_color
	elif _right_fill is StyleBoxTexture:
		(_right_fill as StyleBoxTexture).modulate_color = right_color