using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Rulesets.Mania;

public partial class ManiaNote : Note
{
	/// <summary>
	/// The note skin associated with this note.
	/// </summary>
	[Export] public ManiaNoteSkin NoteSkin;

	/// <summary>
	/// The Note graphic for this note.
	/// </summary>
	public AnimatedSprite2D Note; // Perhaps it'd be a good idea to make an AnimatedTextureRect?

	/// <summary>
	/// The hold control that contains everything related to the hold graphics.
	/// </summary>
	public Control HoldContainer;
	
	/// <summary>
	/// The Hold graphic.
	/// </summary>
	public TextureRect Hold;

	/// <summary>
	/// The Tail graphic for this note.
	/// </summary>
	public AnimatedSprite2D Tail;

	private double _tailOffset = 0d;
	
	/// <summary>
	/// Sets up this hit object for usage alongside a <see cref="ManiaNoteManager"/>.
	/// </summary>
	/// <param name="noteData">The note data</param>
	/// <param name="parentManager">The parent manager</param>
	/// <param name="noteSkin">The note skin</param>
	public void Setup(NoteData noteData, ManiaNoteManager parentManager, ManiaNoteSkin noteSkin)
	{
		Position = new Vector2(5000, 0);
		Info = noteData;
		ParentManager = parentManager;
		ChangeNoteSkin(noteSkin);
		
		HoldContainer.Visible = Info.MsLength > 0;
		if (Info.MsLength <= 0)
			return;
		
		AdjustInitialTailSize();
		AdjustTailLength(Info.MsLength);
	}

	public override void _Process(double delta)
	{
		ManiaNoteManager parent = GetParentManiaNoteManager();
		if (!Active || parent == null || !Visible || Info == null)
			return;

		Color modulate = Modulate;
		modulate.A = Missed ? 0.5f : 1f;
		Modulate = modulate;
		
		// Updating position and all that, whatever the base class does.
		base._Process(delta);

		double songPos = Conductor.Time * 1000d;
		bool isHeld = parent.NoteHeld == Info;
		if (Info.MsLength > 0)
		{
			HoldContainer.Rotation = parent.DirectionAngle;
			
			if (isHeld)
			{
				AdjustTailLength(Info.MsTime + Info.MsLength - songPos);
				Note.Visible = false;
			}
		}

		if (Info.MsTime + Info.MsLength - songPos <= -1000f && !isHeld)
		{
			Active = false;
			Visible = false;
		}
	}
	
	/// <inheritdoc/>
	public override void UpdatePosition()
	{
		if (ParentManager is not ManiaNoteManager maniaNoteManager)
			return;
		
		float startingPos = ParentManager.ParentBarLine.DistanceOffset * ParentManager.ScrollSpeed;
		SvChange svChange = ParentManager.ParentBarLine.Chart.SvChanges[Info.StartingScrollVelocity];
		float distance = (float)(svChange.Position + Info.MsTime - svChange.MsTime - _tailOffset) * ParentManager.ScrollSpeed;
		Vector2 posMult = new Vector2(Mathf.Cos(maniaNoteManager.DirectionAngle), Mathf.Sin(maniaNoteManager.DirectionAngle));
		Position = maniaNoteManager.NoteHeld != Info ? (startingPos + distance) * posMult : Vector2.Zero;
	}

	/// <summary>
	/// Changes the note skin of this note.
	/// </summary>
	/// <param name="noteSkin">The provided note skin.</param>
	public void ChangeNoteSkin(ManiaNoteSkin noteSkin)
	{
		if (NoteSkin == noteSkin)
			return;
		
		NoteSkin = noteSkin;

		int lane = ParentManager.Lane;
		int laneCount = ParentManager.ParentBarLine.Chart.Lanes;
		string direction = noteSkin.GetDirection(lane, laneCount).ToLower();
		
		// Initialize graphic object here, if they're null.
		if (Note == null)
		{
			Note = new AnimatedSprite2D();
			Note.Name = "Note Graphic";
			AddChild(Note);
		}
		
		if (HoldContainer == null)
		{
			HoldContainer = new Control();
			HoldContainer.Name = "Hold Container";
			HoldContainer.ClipContents = true;
			AddChild(HoldContainer);
			
			// Test
			/*
			ColorRect thing = new ColorRect();
			HoldContainer.AddChild(thing);
			thing.SetAnchorsPreset(LayoutPreset.FullRect);
			thing.Color = Colors.Lavender;*/
		}
		
		// hold
		if (Hold == null)
		{
			Hold = new TextureRect();
			Hold.Name = "Hold Graphic";
			HoldContainer.AddChild(Hold);
			HoldContainer.MoveChild(Hold, 0);
		}
		
		// The tail
		if (Tail == null)
		{
			Tail = new AnimatedSprite2D();
			Tail.Name = "Tail Graphic";
			HoldContainer.AddChild(Tail);
			Tail.MoveToFront();
		}
		
		// Do actual note skin graphic setting
		Note.SpriteFrames = noteSkin.NoteAtlas;
		Note.Scale = Vector2.One * NoteSkin.Scale;
		Note.Play($"{direction}NoteNeutral");
		Note.Visible = true;

		Texture2D holdTexture = NoteSkin.HoldAtlas.GetFrameTexture($"{direction}NoteHold", 0);
		HoldContainer.Modulate = new Color(1f, 1f, 1f, 0.5f);
		HoldContainer.Size = new Vector2(0f, holdTexture.GetHeight());
		HoldContainer.Scale = NoteSkin.Scale;
		HoldContainer.PivotOffset = new Vector2(0f, HoldContainer.Size.Y / 2f);
		HoldContainer.Position = new Vector2(0f, -HoldContainer.Size.Y / 2f);

		Hold.Texture = holdTexture;
		Hold.StretchMode = noteSkin.UseTiledHold && holdTexture is not AtlasTexture
			? TextureRect.StretchModeEnum.Tile
			: TextureRect.StretchModeEnum.Scale;

		Tail.Centered = false;
		Tail.SpriteFrames = noteSkin.HoldAtlas;
		Tail.Play($"{direction}NoteTail");
		Tail.Visible = true;
	}
	
	/// <summary>
	/// Resizes the hold's initial size to match the scroll speed and scroll velocities.
	/// </summary>
	public void AdjustInitialTailSize()
	{
		if (ParentManager is not ManiaNoteManager maniaNoteManager)
			return;
		
		// Rough code, might clean up later if possible
		string direction = maniaNoteManager.Direction;
		int tailTexWidth = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame()).GetWidth();

		float holdWidth = GetOnScreenHoldLength(Info.MsLength) * ParentManager.ScrollSpeed;
		Hold.Size = new Vector2((holdWidth - tailTexWidth) / HoldContainer.Scale.X, Hold.Size.Y);
		
		if (maniaNoteManager.NoteHeld != Info)
			AdjustTailLength(Info.MsLength);
	}

	/// <summary>
	/// Resizes the entire hold in general according to the length provided.
	/// </summary>
	public void AdjustTailLength(double length)
	{
		if (ParentManager is not ManiaNoteManager maniaNoteManager)
			return;
		
		// Rough code, might clean up later if possible
		string direction = maniaNoteManager.Direction;
		float initialHoldWidth = GetOnScreenHoldLength(Info.MsLength) * ParentManager.ScrollSpeed;
		float holdWidth = GetOnScreenHoldLength(length) * ParentManager.ScrollSpeed;

		Vector2 holdContainerScale = HoldContainer.Scale;
		Vector2 holdContainerSize = HoldContainer.Size;
		HoldContainer.Size = new Vector2(holdWidth / holdContainerScale.X, holdContainerSize.Y);
		
		Vector2 holdPos = Hold.Position;
		holdPos.X = HoldContainer.Size.X - (initialHoldWidth / holdContainerScale.X);
		Hold.Position = holdPos;
		
		Texture2D tailFrame = Tail.SpriteFrames.GetFrameTexture($"{direction}NoteTail", Tail.GetFrame());
		Vector2 tailTexSize = tailFrame.GetSize();
		Tail.Position = new Vector2((initialHoldWidth - tailTexSize.X) / holdContainerScale.X + holdPos.X, Hold.Texture.GetHeight() - tailTexSize.Y);
	}

	public ManiaNoteManager GetParentManiaNoteManager()
	{
		if (ParentManager is ManiaNoteManager a)
			return a;

		return null;
	}
	
	/// <summary>
	/// Usually called when the note was let go too early.
	/// </summary>
	public void UnsetHold()
	{
		// Should be based on time, NOT note Y position
		_tailOffset = GetStartingPoint() + ParentManager.ParentBarLine.DistanceOffset;
	}

	/// <inheritdoc/>
	public override void Reset()
	{
		base.Reset();
		Note.Visible = true;
		_tailOffset = 0d;
	}
	
	/// <summary>
	/// Gets the on-screen length of the hold note
	/// </summary>
	/// <param name="length">The current length of the note</param>
	/// <returns>The on-screen length</returns>
	private float GetOnScreenHoldLength(double length)
	{
		SvChange[] svChangeList = ParentManager.ParentBarLine.Chart.SvChanges;
		double startTime = Info.MsTime + (Info.MsLength - length);
		int startIndex = Info.StartingScrollVelocity;
		for (int i = startIndex; i <= Info.EndingScrollVelocity; i++)
		{
			if (svChangeList[i].MsTime > startTime)
				break;
			
			startIndex = i;
		}
		
		SvChange startingSvChange = svChangeList[startIndex];
		double startingPosition = startingSvChange.Position + ((startTime - startingSvChange.MsTime) * startingSvChange.Multiplier);

		return (float)(GetEndingPoint() - startingPosition);
	}
}
