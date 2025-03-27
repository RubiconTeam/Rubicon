@tool

class_name SyncedSprite2D extends AnimatedSprite2D

## [AnimatedSprite2D] utility that synchronizes the animation with the provided [AnimationPlayer]

@export var sync : bool:
	get:
		return _syncing
	set(val):
		_syncing = val
		if _syncing:
			play(animation)
			return
		
		pause()

@export var frame_offset : int = 0

@export_group("Sync With")
@export var animation_player : AnimationPlayer ## [AnimationPlayer] to sync with.

var _time : float = 0.0;
var _last_player_position : float = 0.0;
var _syncing : bool = false

func _process(delta: float) -> void:
	var is_on_animation : bool = animation_player != null and (not animation_player.assigned_animation.is_empty() or not animation_player.current_animation.is_empty() or animation_player.is_playing())
	if not is_on_animation:
		return
	
	if is_playing():
		stop()
	
	var current_pos : float = animation_player.current_animation_position
	sync_with_animation_player(current_pos - _last_player_position if _syncing else 0.0)
	_last_player_position = current_pos

func sync_with_animation_player(delta : float) -> void:
	_time += delta
	if _time < 0.0:
		_time = 0.0
	
	var fps : float = sprite_frames.get_animation_speed(animation)
	frame = frame_offset + floori(_time * fps)
	frame_progress = fmod(_time, 1 / fps) * fps
