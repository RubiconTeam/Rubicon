class_name GDModChart

extends Node

func _ready() -> void:
	Conductor.MeasureHit.connect(measure_hit)
	Conductor.BeatHit.connect(beat_hit)
	Conductor.StepHit.connect(step_hit)
	
func measure_hit(_measure : int) -> void:
	pass
	
func beat_hit(_beat : int) -> void:
	pass
	
func step_hit(_step : int) -> void:
	pass