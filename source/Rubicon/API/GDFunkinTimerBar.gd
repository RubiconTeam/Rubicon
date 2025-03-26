class_name GDFunkinTimerBar extends GDTimerBar

## A template for a Funkin' styled timer bar in GDScript.

func initialize() -> void:
	super.initialize()

	if RubiconGame.Active or RubiconGame.Metadata.Environment == GameEnvironment.NONE:
		if direction == BarDirection.LEFT_TO_RIGHT:
			left_color = Color.GREEN
			right_color = Color.RED
		elif direction == BarDirection.RIGHT_TO_LEFT:
			left_color = Color.RED
			right_color = Color.GREEN

		return

	initialize_characters(play_field.Metadata)	

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

	left_color = data.Color

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

	right_color = data.Color

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