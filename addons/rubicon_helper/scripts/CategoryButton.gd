@tool

extends Button

@export var button_text : String:
	get:
		return _text
	set(value):
		_text = value
		text = "[" + ("\\/" if category_visible else ">") + "] " + _text

@export var category_visible : bool:
	get:
		return _visible
	set(value):
		_visible = value
		button_text = _text
		
		if category != null:
			category.visible = _visible

@export var category : Control

var _text : String
var _visible : bool = false

func _ready() -> void:
	pressed.connect(on_pressed)

func on_pressed() -> void:
	category_visible = not category_visible
