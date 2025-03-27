extends GDSongScript

# This is a template for a song script in GDScript.
# This can also act as a Node! So yes, you will have access to such things like _Process(delta).

# Triggers every measure.
func measure_hit(_measure : int) -> void:
	pass

# Triggers every beat.
func beat_hit(_beat : int) -> void:
	pass

# Triggers every step.
func step_hit(_step : int) -> void:
	pass

# Triggers every time a note is hit.
#func note_hit(_bar_line_name : StringName, _result : NoteResult):
#	pass
