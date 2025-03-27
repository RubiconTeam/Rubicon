class_name GDSongScript extends Node

func _ready() -> void:
	Conductor.MeasureHit.connect(measure_hit)
	Conductor.BeatHit.connect(beat_hit)
	Conductor.StepHit.connect(step_hit)
	RubiconGame.PlayField.NoteHit.connect(note_hit)
	
func measure_hit(_measure : int) -> void:
	pass
	
func beat_hit(_beat : int) -> void:
	pass
	
func step_hit(_step : int) -> void:
	pass
	
func note_hit(_bar_line_name : StringName, _result : NoteResult):
	pass