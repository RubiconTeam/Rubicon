class_name GDTextureProgressFunkinTimerBar extends GDFunkinTimerBar

## A Funkin' timer bar that utilizes a [TextureProgressBar]

@export var bar : TextureProgressBar ## The [TextureProgressBar] associated with this health bar.
@export var time_label : Label ## The visual text for the time remaining.

var _left_fill : StyleBox
var _right_fill : StyleBox

func update_bar() -> void:
	bar.ratio = progress_ratio

	var time : float = clampf(length - Conductor.GetRawTime(), 0, length)
	var time_string : String = Time.get_time_string_from_unix_time(time)
	var times : PackedFloat64Array = time_string.split_floats(":")
	if times[0] == 0.0:
		time_label.text = "(" + time_string.substr(time_string.find(":") + 1) + ")"
	else:
		time_label.text = "(" + time_string + ")"
	
func change_left_color(left_color : Color) -> void:
	bar.tint_progress = left_color

func change_right_color(right_color : Color) -> void:
	bar.tint_under = right_color