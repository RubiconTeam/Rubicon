class_name GDTextureProgressFunkinHealthBar extends GDFunkinHealthBar

## A Funkin' health bar that utilizes a [TextureProgressBar]

@export var bar : TextureProgressBar ## The [TextureProgressBar] associated with this health bar.

func update_bar() -> void:
	match bar.fill_mode:
		TextureProgressBar.FILL_BILINEAR_LEFT_AND_RIGHT:
			determine_ratio_from_direction()
		TextureProgressBar.FILL_BILINEAR_TOP_AND_BOTTOM:
			determine_ratio_from_direction()
		TextureProgressBar.FILL_CLOCKWISE_AND_COUNTER_CLOCKWISE:
			determine_ratio_from_direction()
		_:
			bar.ratio = progress_ratio
	
func change_left_color(left_color : Color) -> void:
	match direction:
		BarDirection.LEFT_TO_RIGHT:
			match bar.fill_mode:
				TextureProgressBar.FILL_BILINEAR_LEFT_AND_RIGHT:
					pass
				TextureProgressBar.FILL_BILINEAR_TOP_AND_BOTTOM:
					pass
				TextureProgressBar.FILL_CLOCKWISE_AND_COUNTER_CLOCKWISE:
					pass
				_:
					bar.tint_progress = left_color
		BarDirection.RIGHT_TO_LEFT:
			match bar.fill_mode:
				TextureProgressBar.FILL_BILINEAR_LEFT_AND_RIGHT:
					pass
				TextureProgressBar.FILL_BILINEAR_TOP_AND_BOTTOM:
					pass
				TextureProgressBar.FILL_CLOCKWISE_AND_COUNTER_CLOCKWISE:
					pass
				_:
					bar.tint_under = left_color

func change_right_color(right__color : Color) -> void:
	match direction:
		BarDirection.LEFT_TO_RIGHT:
			match bar.fill_mode:
				TextureProgressBar.FILL_BILINEAR_LEFT_AND_RIGHT:
					pass
				TextureProgressBar.FILL_BILINEAR_TOP_AND_BOTTOM:
					pass
				TextureProgressBar.FILL_CLOCKWISE_AND_COUNTER_CLOCKWISE:
					pass
				_:
					bar.tint_under = right_color
		BarDirection.RIGHT_TO_LEFT:
			match bar.fill_mode:
				TextureProgressBar.FILL_BILINEAR_LEFT_AND_RIGHT:
					pass
				TextureProgressBar.FILL_BILINEAR_TOP_AND_BOTTOM:
					pass
				TextureProgressBar.FILL_CLOCKWISE_AND_COUNTER_CLOCKWISE:
					pass
				_:
					bar.tint_progress = right_color
	
func change_direction(direction : int) -> void:
	match direction:
		BarDirection.LEFT_TO_RIGHT:
			match bar.fill_mode:
				TextureProgressBar.FILL_LEFT_TO_RIGHT:
					bar.fill_mode = TextureProgressBar.FILL_LEFT_TO_RIGHT
				TextureProgressBar.FILL_RIGHT_TO_LEFT:
					bar.fill_mode = TextureProgressBar.FILL_LEFT_TO_RIGHT	
				TextureProgressBar.FILL_CLOCKWISE:
					bar.fill_mode = TextureProgressBar.FILL_CLOCKWISE
				TextureProgressBar.FILL_COUNTER_CLOCKWISE:
					bar.fill_mode = TextureProgressBar.FILL_CLOCKWISE
		BarDirection.RIGHT_TO_LEFT:
			match bar.fill_mode:
				TextureProgressBar.FILL_LEFT_TO_RIGHT:
					bar.fill_mode = TextureProgressBar.FILL_RIGHT_TO_LEFT
				TextureProgressBar.FILL_RIGHT_TO_LEFT:
					bar.fill_mode = TextureProgressBar.FILL_RIGHT_TO_LEFT
				TextureProgressBar.FILL_CLOCKWISE:
					bar.fill_mode = TextureProgressBar.FILL_COUNTER_CLOCKWISE
				TextureProgressBar.FILL_COUNTER_CLOCKWISE:
					bar.fill_mode = TextureProgressBar.FILL_COUNTER_CLOCKWISE
		
	change_left_color(left_color)
	change_right_color(right_color)
	
func determine_ratio_from_direction() -> void:
	match direction:
		BarDirection.LEFT_TO_RIGHT:
			bar.ratio = progress_ratio
		BarDirection.RIGHT_TO_LEFT:
			bar.ratio = 1.0 - progress_ratio