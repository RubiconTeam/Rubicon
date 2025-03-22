class_name DanceHitDistance

extends GDStatDisplay

@export var graphic_scale : Vector2 = Vector2.ONE
@export var label : Label
@export var follow_bar_line : bool = false

var offset : Vector2
var tween : Tween

func initialize() -> void:
	super()
	
	offset = ui_style.HitDistanceOffset

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	_distance = roundf(_distance * 100.0) * 0.01
	label.text = str(_distance) + " ms"
	
	if absf(_distance) > ProjectSettings.get_setting("rubicon/judgments/bad_hit_window"):
		label.text = "Too " + ("late!" if _distance < 0 else "early!")
	
	if tween != null:
		tween.kill()
	
	label.pivot_offset = label.size / 2.0
	label.modulate.a = 1.0
	label.scale = graphic_scale * 1.1
	update_position()
	
	tween = label.create_tween()
	tween.tween_property(label, "scale", graphic_scale, 0.1)
	tween.tween_property(label, "modulate", Color.TRANSPARENT, 0.5).set_delay(1.0)
	tween.play()

func _process(_delta: float) -> void:
	if not follow_bar_line:
		return
	
	update_position()

func update_position() -> void:
	if label == null:
		return

	if play_field == null:
		return
	
	var bar_line : BarLine = play_field.BarLines[play_field.TargetIndex]
	label.anchor_top = bar_line.anchor_top
	label.anchor_bottom = bar_line.anchor_bottom
	label.anchor_left = bar_line.anchor_left
	label.anchor_right = bar_line.anchor_right
	
	label.global_position = bar_line.global_position + (offset * (-1.0 if (UserSettings.GetSetting("Rubicon/Mania/DownScroll") as bool) else 1.0)) - label.pivot_offset
