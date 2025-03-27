class_name GDProgressFunkinHealthBar extends GDFunkinHealthBar

## A Funkin' health bar that utilizes a [ProgressBar]

@export var bar : ProgressBar ## The [ProgressBar] associated with this health bar.

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