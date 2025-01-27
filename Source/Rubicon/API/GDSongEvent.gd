class_name GDSongEvent

extends Node

## A template for a song event in GDScript. Must replace the function [method call_event]

var _initialized : bool = false

func _ready() -> void: ## Used to initialize this event for the first time.
	if _initialized:
		return
		
	var game : RubiconGame = RubiconEngine.GetGameInstance()
	if game == null:
		queue_free()
		return
	
	var event_ctrl : SongEventController = game.EventController
	event_ctrl.EventCalled.connect(on_event_called)
	_initialized = true

func call_event(time : float, args : Dictionary[StringName, Variant]): ## This function is called when the event controller reaches it. Should be inherited.
	pass

func on_event_called(event_name : StringName, time : float, args : Dictionary[StringName, Variant]):
	if event_name != name:
		return
	
	call_event(time, args)
