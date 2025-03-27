class_name GDDanceHitDistance extends GDStatDisplay

@export var label : Label
@export var animation_player : AnimationPlayer
 
var offset : Vector2
var tween : Tween

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	_distance = roundf(_distance * 100.0) * 0.01
	label.text = str(_distance) + " ms"
	
	if absf(_distance) > ProjectSettings.get_setting("rubicon/judgments/bad_hit_window"):
		label.text = "Too " + ("late!" if _distance < 0 else "early!")
		
	animation_player.play(get_hit_name(_hit))
	animation_player.seek(0, true)
