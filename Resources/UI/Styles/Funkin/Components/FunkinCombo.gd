class_name FunkinCombo

extends GDStatDisplay

@export var atlas : SpriteFrames
@export var spacing : float = 54.0
@export var graphic_scale : Vector2 = Vector2.ONE * 0.675

var last_rating : int = 0
var was_zero : bool = false

var graphics : Array[Control] = []
var combo_velocities : Dictionary[Control, Vector2] = {}
var combo_accelerations : Dictionary[Control, int] = {}

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	if _combo == 0 and was_zero:
		return
	
	if _hit > last_rating:
		last_rating = _hit
	
	var combo_string : String = str(_combo)
	while combo_string.length() < 3:
		combo_string = "0" + combo_string
	
	var split_digits : PackedStringArray = combo_string.split("")
	var current_graphics : Array[Control] = []
	current_graphics.resize(split_digits.size())
	for i in split_digits.size():
		var filtered_graphics : Array[Control] = graphics.filter(func(c:Control)->bool:return c.modulate.a == 0 and not current_graphics.has(c))
		var combo_spr : Control = filtered_graphics.front() if not filtered_graphics.is_empty() else null
		if combo_spr == null:
			combo_spr = Control.new()
			combo_spr.name = "Instance " + str(graphics.size())

			combo_spr.anchor_left = 0.507
			combo_spr.anchor_right = 0.507
			combo_spr.anchor_top = 0.48
			combo_spr.anchor_bottom = 0.48
			
			var combo_graphic : AnimatedSprite2D = AnimatedSprite2D.new()
			combo_graphic.name = "Graphic"
			combo_graphic.centered = false
			
			combo_spr.add_child(combo_graphic)
			graphics.push_back(combo_spr)
			add_child(combo_spr)
		
		var graphic : AnimatedSprite2D = combo_spr.get_child(0)
		graphic.sprite_frames = atlas
		graphic.animation = split_digits[i]
		graphic.frame = 0
		graphic.play()
		graphic.material = get_hit_material(last_rating)
		
		combo_spr.move_to_front()
		combo_spr.scale = graphic_scale
		combo_spr.modulate.a = 1.0
		
		combo_spr.offset_left = -97 + (i * spacing)
		combo_spr.offset_right = combo_spr.offset_left
		combo_spr.offset_top = 0
		combo_spr.offset_bottom = 0
		
		combo_velocities[combo_spr] = Vector2(randi_range(1, 15), randi_range(-160, -140))
		combo_accelerations[combo_spr] = randi_range(300, 450)
		
		var fade_tween : Tween = combo_spr.create_tween()
		fade_tween.tween_property(combo_spr, "modulate", Color.TRANSPARENT, 0.2).set_delay(60.0 / Conductor.Bpm * 2.0)
		fade_tween.play()
	
	was_zero = _combo == 0
	if was_zero:
		last_rating = 0

func _process(delta: float) -> void:
	for i in graphics.size():
		if graphics[i].modulate.a == 0:
			continue
		
		var combo_spr : Control = graphics[i]
		var velocity : Vector2i = combo_velocities[combo_spr]
		var accel : int = combo_accelerations[combo_spr]
		
		combo_spr.position += velocity * delta
		combo_velocities[combo_spr].y += accel * delta
