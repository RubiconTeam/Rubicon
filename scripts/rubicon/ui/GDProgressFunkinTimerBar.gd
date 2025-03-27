class_name GDProgressFunkinTimerBar extends GDFunkinTimerBar

## A Funkin' timer bar that utilizes a [ProgressBar]

@export var bar : ProgressBar ## The [ProgressBar] associated with this health bar.
@export var time_label : Label ## The visual text for the time remaining.

var _fill_style : StyleBox
var _under_style : StyleBox

var _left_fill : StyleBox
var _right_fill : StyleBox

func _ready() -> void:
	_fill_style = bar.get_theme_stylebox("fill").duplicate()
	_under_style = bar.get_theme_stylebox("background").duplicate()

	bar.add_theme_stylebox_override("fill", _fill_style)
	bar.add_theme_stylebox_override("background", _under_style)

	super()

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

func change_direction(direction : int) -> void:
	match direction:
		BarDirection.LEFT_TO_RIGHT:
			bar.fill_mode = ProgressBar.FILL_BEGIN_TO_END
			_left_fill = _fill_style
			_right_fill = _under_style
		BarDirection.RIGHT_TO_LEFT:
			bar.fill_mode = ProgressBar.FILL_END_TO_BEGIN
			_left_fill = _under_style
			_right_fill = _fill_style

	change_left_color(left_color)
	change_right_color(right_color)		