class_name GDPauseMenu extends GDMenu

## A template for a pause menu in Rubicon.

@export var pause_action : String = "game_pause" ## The action in the input map that pauses the game.
signal pause_opened() ## Triggers after the pause action is invoked.

func _input(event: InputEvent) -> void:
	if not event.is_action_pressed(pause_action) or not RubiconGame.Active:
		return
		
	if RubiconGame.Paused or RubiconGame.PlayField.HasFailed():
		return
		
	RubiconGame.Pause()
	open_pause()
	pause_opened.emit()
	
func open_pause() -> void: ## Invokes when the pause action is invoked.
	pass