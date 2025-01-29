class_name GDScreen

extends Node

## A main screen class for GDScript classes.

@export_file var resources_to_load : PackedStringArray = [] ## Resources that will be loaded upon entering a loading screen.

func ready_preload() -> void: ## Triggers right after the scene is loaded to add resources to load.
	pass

func on_resource_loaded(_path : String) -> void: ## Triggers upon loading a resource specified in [member resources_to_load]
	pass