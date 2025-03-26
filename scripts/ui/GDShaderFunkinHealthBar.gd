class_name GDShaderFunkinHealthBar extends GDFunkinHealthBar

## A Funkin' health bar that utilizes a bar with a shader applied to it.

@export var bar : CanvasItem ## The CanvasItem that has the shader applied to it.
@export var value_property : String = "value" ## Whatever the value property is called in the shader.
@export var left_shader_property : String = "black" ## Whatever the left color property is called in the shader.
@export var right_shader_property : String = "white" ## Whatever the right color property is called in the shader. 

var _material : ShaderMaterial

func _ready() -> void:
	_material = bar.material as ShaderMaterial

func update_bar() -> void:
	if _material == null:
		return
		
	_material.set_shader_parameter(value_property, progress_ratio)
	
func change_left_color(left_color : Color) -> void:
	if _material == null:
		return
		
	_material.set_shader_parameter(left_shader_property, left_color)

func change_right_color(right_color : Color) -> void:
	if _material == null:
		return

	_material.set_shader_parameter(right_shader_property, right_color)