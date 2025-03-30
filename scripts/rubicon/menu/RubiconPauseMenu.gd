class_name RubiconPauseMenu extends GDPauseMenu

@export var animation_player : AnimationPlayer
@export var move_cursor : AudioStream
@export var confirm_audio : AudioStream
@export var opener_audio : AudioStream

@export_group("References")
@export var artist_label : Label
@export var top_bar : Control
@export var bottom_bar : Control
@export var top_icon : AnimatedSprite2D
@export var bottom_icon : AnimatedSprite2D

var currently_focused : Control

func _ready() -> void:
	super()
	
	animation_player.animation_finished.connect(animation_finished)
	for focus in focusable:
		focus.mouse_filter = Control.MOUSE_FILTER_IGNORE
		focus.focus_mode = Control.FOCUS_NONE

func open_pause() -> void:
	var meta : SongMeta = RubiconGame.Metadata
	var chart : RubiChart = RubiconGame.Chart
	artist_label.text = meta.Name + "\nBy: " + meta.Artist + "\nCharter: " + chart.Charter
	
	var play_field : PlayField = RubiconGame.PlayField
	var health_bar : Node = play_field.GetHealthBar()
	if health_bar is CsFunkinHealthBar:
		var cs_health_bar : CsFunkinHealthBar = health_bar
		top_bar.modulate = cs_health_bar.LeftColor
		bottom_bar.modulate = cs_health_bar.RightColor
		top_icon.sprite_frames = cs_health_bar.LeftIcon.sprite_frames
		top_icon.texture_filter = cs_health_bar.LeftIcon.texture_filter
		top_icon.scale = cs_health_bar.LeftIcon.scale * 3.0
		bottom_icon.sprite_frames = cs_health_bar.RightIcon.sprite_frames
		bottom_icon.texture_filter = cs_health_bar.RightIcon.texture_filter
		bottom_icon.scale = cs_health_bar.RightIcon.scale * Vector2(-3, 3)
	elif health_bar is GDFunkinHealthBar:
		var gd_health_bar : GDFunkinHealthBar = health_bar
		top_bar.modulate = gd_health_bar.left_color
		bottom_bar.modulate = gd_health_bar.right_color
		top_icon.sprite_frames = gd_health_bar.left_icon.sprite_frames
		top_icon.texture_filter = gd_health_bar.left_icon.texture_filter
		top_icon.scale = gd_health_bar.left_icon.scale * 3.0
		bottom_icon.sprite_frames = gd_health_bar.right_icon.sprite_frames
		bottom_icon.texture_filter = gd_health_bar.right_icon.texture_filter
		bottom_icon.scale = gd_health_bar.right_icon.scale * Vector2(-3, 3)
	
	AudioManager.GetGroup("SoundEffects").Play(opener_audio, true, true, 0.0)
	animation_player.play("open")

func update_icons() -> void:
	pass

func update_selection(focused : Control) -> void:
	super(focused)
	
	currently_focused = focused
	AudioManager.GetGroup("SoundEffects").Play(move_cursor, true, true, 0.0)
	
	for focus in focusable:
		var anim_player : AnimationPlayer = focus.get_node("AnimationPlayer")
		if focus == focused:
			anim_player.play("select")
			continue
		
		if anim_player.assigned_animation == "deselect" or anim_player.assigned_animation == "open":
			continue
		
		anim_player.play("deselect")

func animation_finished(anim : StringName) -> void:
	if anim == "open":
		for focus in focusable:
			focus.mouse_filter = Control.MOUSE_FILTER_STOP
			focus.focus_mode = Control.FOCUS_ALL
		
		initial_focus.grab_focus()
	elif anim == "close":
		RubiconGame.Resume()

func resume() -> void:
	AudioManager.GetGroup("SoundEffects").Play(confirm_audio, true, true, 0.0)
	animation_player.play("close")
	for focus in focusable:
		focus.release_focus()
		focus.mouse_filter = Control.MOUSE_FILTER_IGNORE
		focus.focus_mode = Control.FOCUS_NONE
		
		var fade_tween : Tween = focus.create_tween()
		var mod_tween : PropertyTweener = fade_tween.tween_property(focus, "modulate", Color.TRANSPARENT, 0.2)
		if focus != currently_focused:
			continue
		
		mod_tween.set_delay(0.2)
	
	
