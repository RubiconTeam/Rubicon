class_name FunkinJudgment

extends GDStatDisplay

@export var atlas : SpriteFrames
@export var graphic_scale : Vector2 = Vector2.ONE

var missed_combo : bool = false
var graphics : Array[Control] = []
var velocities : Dictionary[Control, Vector2] = {}

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	if missed_combo and _hit == 5:
		return
		
	var filtered_graphics : Array[Control] = graphics.filter(func(c:Control)->bool:return c.modulate.a == 0)
	var judgment : Control = filtered_graphics.front() if not filtered_graphics.is_empty() else null
	if judgment == null:
		judgment = Control.new()
		judgment.name = "Instance " + str(graphics.size())

		judgment.anchor_left = 0.474
		judgment.anchor_right = 0.474
		judgment.anchor_top = 0.45
		judgment.anchor_bottom = 0.45
		
		judgment.offset_left = -60
		judgment.offset_right = judgment.offset_left
		judgment.offset_top = -90
		judgment.offset_bottom = offset_top
		
		var combo_graphic : AnimatedSprite2D = AnimatedSprite2D.new()
		combo_graphic.name = "Graphic"
		combo_graphic.centered = false
		
		judgment.add_child(combo_graphic)
		graphics.push_back(judgment)
		add_child(judgment)
		
	var graphic : AnimatedSprite2D = judgment.get_child(0)
	graphic.sprite_frames = atlas
	graphic.animation = get_hit_name(_hit)
	graphic.frame = 0
	graphic.play()
	graphic.material = get_hit_material(_hit)
		
	judgment.move_to_front()
	judgment.scale = graphic_scale
	judgment.modulate.a = 1.0
		
	judgment.offset_left = -60
	judgment.offset_right = judgment.offset_left
	judgment.offset_top = -90
	judgment.offset_bottom = judgment.offset_top
	
	velocities[judgment] = Vector2(randi_range(0, 25), randi_range(-262, -52))

	var fade_tween : Tween = judgment.create_tween()
	fade_tween.tween_property(judgment, "modulate", Color.TRANSPARENT, 0.2).set_delay(60.0 / Conductor.Bpm)
	fade_tween.play()
	
	missed_combo = _hit == 5

func _process(delta: float) -> void:
	for i in graphics.size():
		if graphics[i].modulate.a == 0:
			continue
		
		var judgment : Control = graphics[i]
		var velocity : Vector2i = velocities[judgment]
		
		judgment.position += velocity * delta
		velocities[judgment].y += 825.0 * delta
