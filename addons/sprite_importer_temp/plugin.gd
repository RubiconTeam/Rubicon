@tool
extends EditorPlugin

var scene : PackedScene = preload("res://addons/sprite_importer_temp/PluginScene.tscn")
var editor_node : Node

func _enter_tree() -> void:
	editor_node = scene.instantiate()
	add_control_to_dock(EditorPlugin.DOCK_SLOT_LEFT_UR, editor_node)
	pass


func _exit_tree() -> void:
	remove_control_from_docks(editor_node)
	editor_node.queue_free()
	pass
