extends GDSongEvent

# This is a template for events you place in the chart editor.
# This can also act as a Node! So yes, you will have access to such things like _ready() and _process(delta).

# This is basically your _ready() function.
# Do note that you can access the PlayField just by getting play_field!
func initialize() -> void:
	pass

# Called when the event controller reaches this event.
func call_event(time : float, args : Dictionary[StringName, Variant]): 
	pass
