@tool

extends Control

@onready var save_button : Button = get_node("VBoxContainer/Button")

@onready var sprite_frames_path : LineEdit = get_node("VBoxContainer/PathContainer/LineEdit")
@onready var animated_sprite_path : LineEdit = get_node("VBoxContainer/NodePathContainer/LineEdit")
@onready var output_path : LineEdit = get_node("VBoxContainer/OutputContainer/LineEdit")

func _ready():
	save_button.pressed.connect(save_button_pressed)

func save_button_pressed():
	var frames : SpriteFrames = load(sprite_frames_path.text)
	for anim_name in frames.get_animation_names():
		ResourceSaver.save(make_animation(frames, anim_name), output_path.text + "/" + anim_name + ".tres")
		
func make_animation(frames : SpriteFrames, anim_name : String) -> Animation:
	var anim : Animation = Animation.new()
	var path : String = animated_sprite_path.text
	var frame_count : int = frames.get_frame_count(anim_name)
	
	# Make constant values first
	set_initial_value(anim, "sprite_frames", frames)
	set_initial_value(anim, "animation", anim_name)
	set_initial_value(anim, "offset", Vector2.ZERO)
	
	anim.loop_mode = Animation.LOOP_LINEAR if frames.get_animation_loop(anim_name) else Animation.LOOP_NONE
	anim.step = 1.0 / frames.get_animation_speed(anim_name)
	anim.length = anim.step * frame_count
	
	var method_track_idx : int = anim.add_track(Animation.TYPE_METHOD)
	anim.value_track_set_update_mode(method_track_idx, Animation.UPDATE_DISCRETE)
	anim.track_set_path(method_track_idx, path + ":")
	#anim.track_insert_key(method_track_idx, i * anim.step, "play", )
		
	return anim
	
func set_initial_value(anim : Animation, property : String, value : Variant) -> void:
	var sprite_path : String = sprite_frames_path.text
	var track_idx : int = anim.add_track(Animation.TYPE_VALUE)
	anim.value_track_set_update_mode(track_idx, Animation.UPDATE_DISCRETE)
	anim.track_set_path(track_idx, sprite_path + ":" + property)
	anim.track_insert_key(track_idx, 0.0, value)
