@tool
extends ReferenceRect
class_name GDAnimatedFont

@export_tool_button("Update Text") var update_bttn:Callable = Callable(self, "update_text")

@export_multiline var text:String = "Text Here"
var letter_array:Array[String]

@export var sprite_frames:SpriteFrames
var frame_index:int = 0

func update_text() -> void:
	print("wow you updated the text and it did nothing")

func get_letter_array() -> Array[String]:
	return text.split("");

func _draw() -> void:
	for letter:String in letter_array:
		if sprite_frames != null and sprite_frames.has_animation(letter):
			if sprite_frames.get_frame_texture(letter, frame_index) is AtlasTexture:
				#atlas texture handling
				return
			
			#texture2d handling
