class_name DdrJudgment

extends GDStatDisplay

@export var atlas : SpriteFrames
@export var graphic_scale : Vector2 = Vector2.ONE * 0.667
@export var opacity : float = 0.5
@export var follow_bar_line : bool = false

var offset : Vector2 = Vector2.ZERO

var container : Control
var graphic : AnimatedSprite2D
var tween : Tween

func _ready() -> void:
	super._ready()
	
	offset = ui_style.JudgmentOffset

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	if container == null:
		container = Control.new()
		container.name = &"Judgment Container"
		container.size = Vector2.ZERO
		
		graphic = AnimatedSprite2D.new()
		graphic.name = &"Graphic"
		
		container.add_child(graphic)
		add_child(container)
	
	if tween != null:
		tween.kill()
	
	graphic.sprite_frames = atlas
	graphic.animation = get_hit_name(_hit)
	graphic.frame = 0
	graphic.play()
	graphic.material = get_hit_material(_hit)
	
	container.scale = graphic_scale * 1.1
	container.modulate.a = opacity
	container.pivot_offset = container.size / 2.0
	
	update_position()
	
	tween = container.create_tween()
	tween.tween_property(container, "scale", graphic_scale, 0.1)
	tween.tween_property(container, "modulate", Color.TRANSPARENT, 0.5).set_delay(0.4)
	tween.play()
	
func _process(_delta: float) -> void:
	if not follow_bar_line:
		return
	
	update_position()

func update_position() -> void:
	if container == null:
		return
	
	var game : RubiconGame = RubiconEngine.GetGameInstance()
	if game == null:
		return
	
	var play_field : PlayField = game.PlayField
	if play_field == null:
		return
	
	var bar_line : BarLine = play_field.BarLines[play_field.TargetIndex]
	container.anchor_top = bar_line.anchor_top
	container.anchor_bottom = bar_line.anchor_bottom
	container.anchor_left = bar_line.anchor_left
	container.anchor_right = bar_line.anchor_right
	
	container.global_position = bar_line.global_position + (offset * (-1.0 if (UserSettings.GetSetting("Gameplay/DownScroll") as bool) else 1.0))
