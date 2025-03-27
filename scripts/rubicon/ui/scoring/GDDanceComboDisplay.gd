class_name GDDanceComboDisplay extends GDHitMaterialStatDisplay

@export var atlas : SpriteFrames
@export var spacing : float = 90
@export var bounce_scale : Vector2 = Vector2.ONE * 1.2
@export var bounce_duration : float = 0.2
@export var on_screen_duration : float = 0.8
@export var fade_duration : float = 1.0

var last_rating : int = 0

var graphics : Array[Control] = []
var tweens : Array[Tween] = []

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	if _hit > last_rating:
		last_rating = _hit
	
	for i in tweens.size():
		tweens[i].kill()
	tweens.clear()
	
	var combo_string : String = str(_combo)
	var split_digits : PackedStringArray = combo_string.split("")
	
	var graphics_length : int = graphics.size()
	for i in graphics.size():
		graphics[i].modulate = Color.TRANSPARENT
	
	if split_digits.size() <= graphics_length:
		for i in split_digits.size():
			var combo_spr : Control = graphics[i]
			var graphic : AnimatedSprite2D = combo_spr.get_child(0)
			
			graphic.sprite_frames = atlas
			graphic.animation = split_digits[i]
			graphic.frame = 0
			graphic.play()
			
			combo_spr.modulate.a = 1.0
			combo_spr.scale = Vector2.ONE
	elif split_digits.size() > graphics_length:
		for i in graphics_length:
			var combo_spr : Control = graphics[i]
			var graphic : AnimatedSprite2D = combo_spr.get_child(0)
			
			graphic.sprite_frames = atlas
			graphic.animation = split_digits[i]
			graphic.frame = 0
			graphic.play()
			
			combo_spr.modulate.a = 1.0
			combo_spr.scale = Vector2.ONE
		
		for i in range(graphics_length, split_digits.size()):
			var combo_spr : Control = Control.new()
			var graphic : AnimatedSprite2D = AnimatedSprite2D.new()
			
			graphic.sprite_frames = atlas
			graphic.animation = split_digits[i]
			graphic.frame = 0
			graphic.play()
			
			combo_spr.add_child(graphic)
			combo_spr.modulate.a = 1.0
			combo_spr.scale = Vector2.ONE
			graphics.push_back(combo_spr)
			add_child(combo_spr)
	
	var bar_line : BarLine = try_get_target_barline()
	for i in split_digits.size():
		var combo_spr : Control = graphics[i]
		combo_spr.scale = bounce_scale
		
		if bar_line != null:
			combo_spr.anchor_top = bar_line.anchor_top
			combo_spr.anchor_bottom = bar_line.anchor_bottom
			combo_spr.anchor_left = bar_line.anchor_left
			combo_spr.anchor_right = bar_line.anchor_right
		else:
			combo_spr.anchor_top = anchor_top
			combo_spr.anchor_bottom = anchor_bottom
			combo_spr.anchor_left = anchor_left
			combo_spr.anchor_right = anchor_right
		
		#var start_pos : Vector2 = bar_line.global_position + (offset * (-1.0 if (UserSettings.GetSetting("Rubicon/Mania/DownScroll") as bool) else 1.0)) if bar_line != null else Vector2.ZERO
		combo_spr.position = Vector2(i * spacing - ((split_digits.size() - 1) * spacing / 2.0), 0) - combo_spr.pivot_offset
		
		var graphic : AnimatedSprite2D = combo_spr.get_child(0)
		graphic.material = get_hit_material(last_rating)
		
		var tween : Tween = combo_spr.create_tween()
		tween.tween_property(combo_spr, "scale", Vector2.ONE, bounce_duration)
		tween.tween_property(combo_spr, "modulate", Color.TRANSPARENT, fade_duration).set_delay(on_screen_duration)
		tween.play()
		tweens.push_back(tween)
	
	if _combo == 0:
		last_rating = 0

func try_get_target_barline() -> BarLine:
	if play_field == null:
		return null
	
	return play_field.BarLines[play_field.TargetIndex]
