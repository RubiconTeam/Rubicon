class_name GDFunkinHealthBar extends GDHealthBar

## A template for a classic Funkin' styled health bar in C#.

@export var size_lerp_weight : float = 9.0 ## How fast the icon sizes go back to normal.
@export var left_icon : AnimatedSprite2D ## The icon on the left side.
@export var right_icon : AnimatedSprite2D ## The icon on the right side.
@export var icon_container : PathFollow2D ## The container for both icons.

@export_group("Time")
@export var time_type : int: ## The time of type to go by with [member BounceTime]
	get:
		return _time_type
	set(val):
		_time_type = val
		if beat_syncer != null:
			beat_syncer.Type = _time_type

@export var bounce_time : float: ## How often to bounce.
	get:
		if beat_syncer != null:
			return beat_syncer.Value
		
		return _bounce_value
	set(val):
		_bounce_value = val
		if beat_syncer != null:
			beat_syncer.Value = _bounce_value

var beat_syncer : BeatSyncer ## The node that controls when the icons bounce.

var _bounce_value : float = 1.0 / 4.0
var _time_type : int = TimeValue.MEASURE

var _previous_health : float = 0.0
var _previous_direction : int = BarDirection.RIGHT_TO_LEFT

func initialize() -> void:
	super.initialize()
	
	beat_syncer = BeatSyncer.new()
	beat_syncer.Type = _time_type
	beat_syncer.Value = _bounce_value
	beat_syncer.Name = "Bumper"
	beat_syncer.Bumped.connect(bump)
	add_child(beat_syncer)
	
	if RubiconGame.Active or RubiconGame.Metadata.Environment == GameEnvironment.NONE:
		if direction == BarDirection.LEFT_TO_RIGHT:
			left_color = Color.GREEN
			right_color = Color.RED
		elif direction == BarDirection.RIGHT_TO_LEFT:
			left_color = Color.RED
			right_color = Color.GREEN
			
		return
	
	initialize_characters(play_field.Metadata)	

func _process(_delta: float) -> void:
	super._process(_delta)
	
	var _scale : Vector2 = icon_container.scale
	if not _scale.is_equal_approx(Vector2.ONE):
		var next_scale : Vector2 = _scale.lerp(Vector2.ONE, size_lerp_weight * _delta)
		icon_container.scale = Vector2.ONE if next_scale.is_equal_approx(Vector2.ONE) else next_scale
		
	if play_field == null or (_previous_health == play_field.Health and _previous_direction == direction):
		return
			
	var value : float = 1.0 - progress_ratio if direction != BarDirection.LEFT_TO_RIGHT else progress_ratio
	icon_container.progress_ratio = value
		
	var player_winning : bool = play_field.Health > floori(play_field.MaxHealth * 0.8)
	var player_losing : bool = play_field.Health < floori(play_field.MaxHealth * 0.2)
	
	var player_anim : StringName = &"win" if player_winning else (&"lose" if player_losing else &"neutral")
	var opponent_anim : StringName = &"lose" if player_winning else (&"win" if player_losing else &"neutral")
	
	var player_icon : AnimatedSprite2D = left_icon if BarDirection.LEFT_TO_RIGHT else right_icon
	var opponent_icon : AnimatedSprite2D = right_icon if BarDirection.LEFT_TO_RIGHT else left_icon
	
	if player_icon != null and player_icon.sprite_frames.has_animation(player_anim):
		player_icon.play(player_anim)
		
	if opponent_icon != null and opponent_icon.sprite_frames.has_animation(opponent_anim):
		opponent_icon.play(opponent_anim)
	
	_previous_direction = direction
	_previous_health = play_field.Health

func bump() -> void:
	icon_container.scale = Vector2.ONE * 1.2
	
func set_left_character(character : Node) -> void:
	var data : CharacterIconData = null
	if character is Character2D:
		var character_2d : Character2D = character as Character2D
		data = character_2d.Icon
	elif character is Character3D:
		var character_3d : Character3D = character as Character3D
		data = character_3d.Icon
	
	if data == null:
		return
		
	left_icon.sprite_frames = data.Icon
	left_color = data.Color
	left_icon.offset = data.Offset
	left_icon.scale = data.Scale
	left_icon.texture_filter = data.Filter as CanvasItem.TextureFilter

func set_right_character(character : Node) -> void:
	var data : CharacterIconData = null
	if character is Character2D:
		var character_2d : Character2D = character as Character2D
		data = character_2d.Icon
	elif character is Character3D:
		var character_3d : Character3D = character as Character3D
		data = character_3d.Icon

	if data == null:
		return

	right_icon.sprite_frames = data.Icon
	right_color = data.Color
	right_icon.offset = data.Offset
	right_icon.scale = data.Scale
	right_icon.texture_filter = data.Filter as CanvasItem.TextureFilter
	
func initialize_characters(song_meta : SongMeta) -> void:
	if not RubiconGame.Active:
		return
		
	match song_meta.Environment:
		GameEnvironment.CANVAS_ITEM:
			var space : CanvasItemSpace = RubiconGame.CanvasItemSpace
			var player_group_name : StringName = play_field.TargetBarLine
			var player : Character2D = space.GetCharacterGroup(player_group_name).Characters.front()
			
			var opponent_name : StringName = play_field.BarLines.filter(func(b:BarLine)->bool:return b.name != player_group_name).front().name
			var opponent : Character2D = space.GetCharacterGroup(opponent_name).Characters.front()
			
			if direction == BarDirection.LEFT_TO_RIGHT:
				set_left_character(player)
				set_right_character(player)
			elif direction == BarDirection.RIGHT_TO_LEFT:
				set_left_character(opponent)
				set_right_character(player)
			
		GameEnvironment.SPATIAL:
			var space : SpatialSpace = RubiconGame.SpatialSpace
			var player_group_name : StringName = play_field.TargetBarLine
			var player : Character3D = space.GetCharacterGroup(player_group_name).Characters.front()

			var opponent_name : StringName = play_field.BarLines.filter(func(b:BarLine)->bool:return b.name != player_group_name).front().name
			var opponent : Character3D = space.GetCharacterGroup(opponent_name).Characters.front()

			if direction == BarDirection.LEFT_TO_RIGHT:
				set_left_character(player)
				set_right_character(player)
			elif direction == BarDirection.RIGHT_TO_LEFT:
				set_left_character(opponent)
				set_right_character(player)