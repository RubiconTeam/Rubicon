@tool

extends Control

@onready var button : Button = get_node("VBoxContainer/Button")
@onready var frames_path : LineEdit = get_node("VBoxContainer/PathContainer/LineEdit")
@onready var node_path : LineEdit = get_node("VBoxContainer/NodePathContainer/LineEdit")
@onready var output_path : LineEdit = get_node("VBoxContainer/OutputContainer/LineEdit")

func _ready():
	button.pressed.connect(button_pressed)

func button_pressed():
	var frames : SpriteFrames = load(frames_path.text)
	for anim_name in frames.get_animation_names():
		ResourceSaver.save(make_animation(frames, name), output_path.text + "/anim_name.tres")
		
func make_animation(frames : SpriteFrames, anim_name : String) -> Animation:
	var anim : Animation = Animation.new()
	var path : String = node_path.text
	var frame_count : int = frames.get_frame_count(anim_name)
	
	# Make constant values first
	set_initial_value(anim, "sprite_frames", frames)
	set_initial_value(anim, "animation", name)
	set_initial_value(anim, "offset", Vector2.ZERO)
	
	anim.loop_mode = Animation.LOOP_LINEAR if frames.get_animation_loop(anim_name) else Animation.LOOP_NONE
	anim.step = frames.get_animation_speed(anim_name)
	anim.length = anim.step * frame_count
	
	# Now with the frames
	var frame_track_idx : int = anim.add_track(Animation.TYPE_VALUE)
	anim.value_track_set_update_mode(frame_track_idx, Animation.UPDATE_DISCRETE)
	anim.track_set_path(frame_track_idx, path + ":frame")
	for i in frame_count:
		anim.track_insert_key(frame_track_idx, i * anim.step, i)
		
	return anim
	
func set_initial_value(anim : Animation, property : String, value : Variant) -> void:
	var sprite_path : String = node_path.text
	var track_idx : int = anim.add_track(Animation.TYPE_VALUE)
	anim.value_track_set_update_mode(track_idx, Animation.UPDATE_DISCRETE)
	anim.track_set_path(track_idx, sprite_path + ":" + property)
	anim.track_insert_key(track_idx, 0.0, value)
