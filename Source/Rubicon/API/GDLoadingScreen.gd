class_name GDLoadingScreen

extends Control

## A template for loading screens in GDScript.

@export var opening_animation : StringName ## The transition into the loading screen.
@export var closing_animation : StringName ## The transition out of the loading screen.
@export var animation_player : AnimationPlayer ## The animation player reference.

func _ready() -> void: ## Readies the loading screen!
	animation_player.animation_finished.connect(animation_finished)
	animation_player.play(opening_animation)
	animation_player.seek(0, true)

	ScreenManager.Completed.connect(load_completed)

func animation_finished(anim : StringName) -> void:
	if anim == opening_animation:
		ScreenManager.StartLoading()

	if anim == closing_animation:
		queue_free()

func load_completed() -> void:
	ScreenManager.Completed.disconnect(load_completed)

	animation_player.play(closing_animation)
	animation_player.seek(0, true)
