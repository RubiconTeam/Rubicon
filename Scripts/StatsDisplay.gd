class_name StatsDisplay

extends Control

var ui_style : UiStyle:
	get:
		var game : RubiconGame = RubiconEngine.GetGameInstance()
		var play_field : PlayField = game.PlayField
		return play_field.UiStyle

func _ready():
	var game : RubiconGame = RubiconEngine.GetGameInstance()
	var play_field : PlayField = game.PlayField
	play_field.StatisticsUpdated.connect(update_stats)

func update_stats(_combo : int, _hit : int, _distance : float) -> void:
	pass

func get_hit_material(hit : int) -> Material:
	match hit:
		0:
			return ui_style.PerfectMaterial
		1:
			return ui_style.GreatMaterial
		2:
			return ui_style.GoodMaterial
		3:
			return ui_style.OkayMaterial
		4:
			return ui_style.BadMaterial
		5:
			return ui_style.MissMaterial
	
	return null

func get_hit_name(hit : int) -> StringName:
	match hit:
		0:
			return &"Perfect"
		1:
			return &"Great"
		2:
			return &"Good"
		3:
			return &"Okay"
		4:
			return &"Bad"
		5:
			return &"Miss"
	
	return &""
