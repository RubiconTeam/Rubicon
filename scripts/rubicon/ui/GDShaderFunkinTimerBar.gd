class_name GDShaderFunkinTimerBar extends GDFunkinTimerBar

## A Funkin' timer bar that utilizes a bar with a shader applied to it.

@export var bar : CanvasItem ## The CanvasItem that has the shader applied to it.
@export var time_label : Label ## The visual text for the time remaining.
@export var value_property : String = "value" ## Whatever the value property is called in the shader.
@export var left_shader_property : String = "black" ## Whatever the left color property is called in the shader.
@export var right_shader_property : String = "white" ## Whatever the right color property is called in the shader. 

var _material : ShaderMaterial

func _ready() -> void:
	_material = bar.material as ShaderMaterial

func update_bar() -> void:
	var time : float = clampf(length - Conductor.GetRawTime(), 0, length)
	var time_string : String = Time.get_time_string_from_unix_time(time)
	var times : PackedFloat64Array = time_string.split_floats(":")
	if times[0] == 0.0:
		time_label.text = "(" + time_string.substr(time_string.find(":") + 1) + ")"
	else:
		time_label.text = "(" + time_string + ")"
	
	if _material == null:
		_material = bar.material
		if _material == null:
			return
		
	_material.set_shader_parameter(value_property, progress_ratio)
	
func change_left_color(left_color : Color) -> void:
	if _material == null:
		_material = bar.material
		if _material == null:
			return
		
	_material.set_shader_parameter(left_shader_property, left_color)

func change_right_color(right_color : Color) -> void:
	if _material == null:
		_material = bar.material
		if _material == null:
			return

	_material.set_shader_parameter(right_shader_property, right_color)
