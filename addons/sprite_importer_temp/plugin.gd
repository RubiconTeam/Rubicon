@tool
extends EditorPlugin

var scene : PackedScene = preload("res://addons/sprite_importer_temp/PluginScene.tscn")
var editor_node : Node

func _enter_tree() -> void:
	editor_node = scene.instantiate()
	add_control_to_bottom_panel(editor_node, "Sprite Importer")
	pass


func _exit_tree() -> void:
	remove_control_from_bottom_panel(editor_node)
	editor_node.queue_free()
	pass
