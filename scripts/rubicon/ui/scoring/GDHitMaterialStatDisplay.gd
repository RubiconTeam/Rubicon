class_name GDHitMaterialStatDisplay extends GDStatDisplay

@export_group("Materials")
@export var perfect_material : Material
@export var great_material : Material
@export var good_material : Material
@export var okay_material : Material
@export var bad_material : Material
@export var miss_material : Material

func get_hit_material(rating : int) -> Material:
	match rating:
		Judgment.PERFECT:
			return perfect_material
		Judgment.GREAT:
			return great_material
		Judgment.GOOD:
			return good_material
		Judgment.OKAY:
			return okay_material
		Judgment.BAD:
			return bad_material
		Judgment.MISS:
			return miss_material
		_:
			return null
