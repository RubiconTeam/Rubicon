extends GDStatDisplay

# This is a template for certain UI elements that hook into the game (i.e. combos, judgment).
# This can also act as a Node! So yes, you will have access to such things like _Process(delta).

# This is basically your _ready() function.
# Do note that you can access the PlayField just by getting play_field!
func initialize() -> void:
	pass

# Called whenever the player hits or misses a note.
func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	pass
