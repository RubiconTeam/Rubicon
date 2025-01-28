class_name DdrComboDisplay

extends GDStatDisplay

@export var atlas : SpriteFrames
@export var spacing : float = 60.0
@export var graphic_scale : Vector2 = Vector2.ONE * 0.667
@export var follow_bar_line : bool = false

var offset : Vector2 = Vector2.ZERO
var last_rating : int = 0

var graphics : Array[Control] = []
var tweens : Array[Tween] = []

func _ready() -> void:
	super._ready()
	
	offset = ui_style.ComboOffset

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
			
			combo_spr.modulate.a = 0.5
			combo_spr.scale = Vector2.ONE
	elif split_digits.size() > graphics_length:
		for i in graphics_length:
			var combo_spr : Control = graphics[i]
			var graphic : AnimatedSprite2D = combo_spr.get_child(0)
			
			graphic.sprite_frames = atlas
			graphic.animation = split_digits[i]
			graphic.frame = 0
			graphic.play()
			
			combo_spr.modulate.a = 0.5
			combo_spr.scale = Vector2.ONE
		
		for i in range(graphics_length, split_digits.size()):
			var combo_spr : Control = Control.new()
			var graphic : AnimatedSprite2D = AnimatedSprite2D.new()
			
			graphic.sprite_frames = atlas
			graphic.animation = split_digits[i]
			graphic.frame = 0
			graphic.play()
			
			combo_spr.add_child(graphic)
			combo_spr.modulate.a = 0.5
			combo_spr.scale = Vector2.ONE
			graphics.push_back(combo_spr)
			add_child(combo_spr)
	
	var bar_line : BarLine = try_get_target_barline()
	for i in split_digits.size():
		var combo_spr : Control = graphics[i]
		combo_spr.modulate.a = 0.5
		combo_spr.scale = Vector2.ONE * 1.2 * graphic_scale
		
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
		
		var start_pos : Vector2 = bar_line.global_position + (offset * (-1.0 if (UserSettings.GetSetting("Gameplay/DownScroll") as bool) else 1.0)) if bar_line != null else Vector2.ZERO
		combo_spr.global_position = start_pos + Vector2(i * spacing - ((split_digits.size() - 1) * spacing / 2.0), 0) - combo_spr.pivot_offset
		
		var graphic : AnimatedSprite2D = combo_spr.get_child(0)
		graphic.material = get_hit_material(last_rating)
		
		var tween : Tween = combo_spr.create_tween()
		tween.tween_property(combo_spr, "scale", graphic_scale, 0.2)
		tween.tween_property(combo_spr, "modulate", Color.TRANSPARENT, 1.0).set_delay(0.8)
		tween.play()
		tweens.push_back(tween)
	
	if _combo == 0:
		last_rating = 0

func _process(_delta: float) -> void:
	if not follow_bar_line:
		return
	
	update_position()

func try_get_target_barline() -> BarLine:
	var game : RubiconGame = RubiconEngine.GetGameInstance()
	if game == null:
		return null
	
	var play_field : PlayField = game.PlayField
	if play_field == null:
		return null
	
	return play_field.BarLines[play_field.TargetIndex]

func update_position() -> void:
	var bar_line : BarLine = try_get_target_barline()
	if bar_line == null:
		return
	
	var start_pos : Vector2 = bar_line.global_position + (offset * (-1.0 if (UserSettings.GetSetting("Gameplay/DownScroll") as bool) else 1.0))
	
	var combo_count : int = graphics.filter(func(c:Control)->bool:return c.modulate.a > 0.0).size()
	for i in combo_count:
		var cur : Control = graphics[i]
		if cur.modulate.a == 0.0:
			continue
		
		cur.anchor_top = bar_line.anchor_top
		cur.anchor_bottom = bar_line.anchor_bottom
		cur.anchor_left = bar_line.anchor_left
		cur.anchor_right = bar_line.anchor_right
		
		cur.global_position = start_pos + Vector2(i * spacing - ((combo_count - 1) - spacing / 2.0), 0) - cur.pivot_offset
