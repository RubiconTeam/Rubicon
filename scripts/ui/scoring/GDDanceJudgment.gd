class_name GDDanceJudgment extends GDStatDisplay

@export var animation_player : AnimationPlayer

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	super.update_stats(_combo, _hit, _distance)

	animation_player.play(get_hit_name(_hit))
	animation_player.seek(0, true)
