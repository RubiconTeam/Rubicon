@tool
extends EditorPlugin

const inspector_scene : PackedScene = preload("res://addons/rubicon_helper/scenes/RubiChartInspector.tscn")

var rubichart_inspector : Control

func _enter_tree() -> void:
	rubichart_inspector = inspector_scene.instantiate()
	add_control_to_dock(DOCK_SLOT_RIGHT_BL, rubichart_inspector)
	pass


func _exit_tree() -> void:
	remove_control_from_docks(rubichart_inspector)
	rubichart_inspector.queue_free()
	pass
