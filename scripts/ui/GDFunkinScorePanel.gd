class_name GDFunkinScorePanel extends GDHudElement

## Score panel that imitates something you would find in a typical FNF mod.

@export var text_label : Label ## The label to set the score text on.
@export var divider : String = " / " ## The seperator for the text.

func _process(_delta: float) -> void:
	var score_tracker : ScoreTracker = play_field.ScoreTracker
	text_label.text = "Score: " + str(score_tracker.Score) + divider + "Accuracy: " + str(snappedf(score_tracker.Accuracy, 0.01)) + "%" + divider + "Misses: " + str(score_tracker.Misses) + divider + "Rank: " + get_rank_text(score_tracker.Rank) + " (" + get_clear_text(score_tracker.Clear) + ")"

func get_rank_text(rank : int) -> String:
	match rank:
		ScoreRank.P:
			return "P"
		ScoreRank.SSS:
			return "SSS"
		ScoreRank.SS:
			return "SS"
		ScoreRank.S:
			return "S"
		ScoreRank.A:
			return "A"
		ScoreRank.B:
			return "B"
		ScoreRank.C:
			return "C"
		ScoreRank.D:
			return "D"
		_:
			return "F"

func get_clear_text(clear : int) -> String:
	match clear:
		ClearRank.CLEAR:
			return "Clear"
		ClearRank.FULL_COMBO:
			return "FC"
		ClearRank.GREAT_FULL_COMBO:
			return "Great FC"
		ClearRank.PERFECT:
			return "Perfect"
		_:
			return "Failure"