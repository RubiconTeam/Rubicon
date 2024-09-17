using Rubicon.Core.Data;

namespace Rubicon.Core.UI;

/// <summary>
/// A control node that activates upon the player hitting a note, showing their type of rating.
/// </summary>
[GlobalClass]
public partial class Judgment : Control
{
    /// <summary>
    /// Textures to fetch from when displaying judgments.
    /// </summary>
    [Export] public SpriteFrames Atlas;

    /// <summary>
    /// How much to scale the judgment graphics by.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Perfect"/>.
    /// </summary>
    public Material PerfectMaterial; // dokibird glasses

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Great"/>.
    /// </summary>
    public Material GreatMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Good"/>.
    /// </summary>
    public Material GoodMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Okay"/>.
    /// </summary>
    public Material OkayMaterial;
    
    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Bad"/>.
    /// </summary>
    public Material BadMaterial;

    /// <summary>
    /// The material to put on when hitting a <see cref="HitType.Miss"/>.
    /// </summary>
    public Material MissMaterial;

    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="type">The hit type provided</param>
    public virtual void Play(HitType type)
    {
        Play(type, Vector2.Zero);
    }

    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="type">The hit type provided</param>
    /// <param name="offset">A Vector2 that offsets the position</param>
    public virtual void Play(HitType type, Vector2? offset)
    {
        // Makes the judgment anchor at the center probably
        Play(type, 0.5f, 0.5f, 0.5f, 0.5f, offset);
    }
    
    /// <summary>
    /// Plays an animation depending on the hit type provided.
    /// </summary>
    /// <param name="type">The hit type provided</param>
    /// <param name="anchorLeft">The left anchor (usually from 0 to 1)</param>
    /// <param name="anchorTop">The top anchor (usually from 0 to 1)</param>
    /// <param name="anchorRight">The right anchor (usually from 0 to 1)</param>
    /// <param name="anchorBottom">The bottom anchor (usually from 0 to 1)</param>
    /// <param name="pos">Where to place the judgment, in pixels.</param>
    public virtual void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        AnchorLeft = anchorLeft;
        AnchorTop = anchorTop;
        AnchorRight = anchorRight;
        AnchorBottom = anchorBottom;
    }

    /// <summary>
    /// Get a judgment animation name based on the rating.
    /// </summary>
    /// <param name="type">The rating</param>
    /// <returns>The string associated with the Judgment</returns>
    protected string GetJudgmentAnimation(HitType type)
    {
        switch (type)
        {
            default:
                return "perfect";
            case HitType.Great:
                return "great";
            case HitType.Good:
                return "good";
            case HitType.Okay:
                return "okay";
            case HitType.Bad:
                return "bad";
            case HitType.Miss:
                return "miss";
        }
    }

    /// <summary>
    /// Get a judgment material based on the rating.
    /// </summary>
    /// <param name="type">The rating</param>
    /// <returns>The Material associated with the Judgment</returns>
    protected Material GetJudgmentMaterial(HitType type)
    {
        switch (type)
        {
            default:
                return PerfectMaterial;
            case HitType.Great:
                return GreatMaterial;
            case HitType.Good:
                return GoodMaterial;
            case HitType.Okay:
                return OkayMaterial;
            case HitType.Bad:
                return BadMaterial;
            case HitType.Miss:
                return MissMaterial;
        }
    }
}

/*
   ::::::::::::::::::::::::::::::::::::::::::::::+@@@@:::::::::::::::::::::::::::::::::::::::::::::::::
   ::::::::::::::::::::::::::::::::::::::::::@@@@@@@@@@@#:@@@@@@@@:::::::::::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::::::::::@@@@@@@@@@@@@@@@@@@@@@@@@@@::::::::::::::::::::::::::::::::::
   ::::::::::::::::::::::::::::::::::::*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@::::::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::::::@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@=::::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::::%@@@@@@@%@@@@@@@@@@@@@@@@@@@%@@@@@@@@-:::::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::::+%@@@@@%%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@:::::::::::::::::::::::::::::
   ::::::::::::::::::::::::::::::-#@@%@@@@@@@@@@@%%@@@@@@@@@@@@@@@@@@@@@@@@-:::::::::::::::::::::::::::
   :::::::::::::::::::::::::::::=#%@@@@@@@@@@@@@@%@@@@@@@@@@@@@@@@@@@@@@@@@%+-:::::::::::::::::::::::::
   :::::::::::::::::::::::::::-**%@@@@@@@@@@@@@%%@@@@@@@%%%%@@@@@@@@@@@@@@@@--:::::::::::::::::::::::::
   ::::::::::::::::::::::::::::-@@@@@%%@@@%@@%%@@@@@@%##***#%@@@@@@@@@@@@@@@%%:::::::::::::::::::::::::
   ::::::::::::::::::::::::::-=*@@@@%@@@@@@@%%%%%%%@#*+====+#@@@@@@@@@@@%@@%=-=-:::::::::::::::::::::::
   ::::::::::::::::::::::::::*-*@%@@@@@@@@%@%%%%%#%**==----=+%@@@@@@@@@@@%%@#-:::::::::::::::::::::::::
   ::::::::::::::::::::::::::-*@%%@@@@@@%%@%%%%%#@+==--:----=*@#@@@@@@@@@@@@@@-::::::::::::::::::::::::
   ::-----------::-------:-::*@@@@%@@@@@@@%%%%%*+=----::--==++*##%@@@@@@@@@@@%=-:::::-:-::-----------::
   ------------------------#@@@%@@%@@@@@@%%%%@%%%**=----=+*%%%#%%#%%@@@@@@@@@#+*-::--------------------
   ---------------------------%@@%@@@%@@##+=--=++**+===+###+====*%%%@@@@@@@@%+-------------------------
   ------------------------%+@@%@@%@@%@*++#@=@+@%#*+-:=***@*@@#@%##%@@@@@@@@@+%-:----------------------
   -------------------------+--#@@@%%@%====-++++++==---*++=+**##%**#@@@@@@@@%#%------------------------
   ----------------------------+=@#@@@#=----===-====-:-++====+**+=+*@@@@@@@%%=-------------------------
   ----------------------------+==%@@@==----------==-:=+*+=========*#@@%@@*-=+-------------------------
   ----------------------------#--#%%@+==----:-:-===-:-+**========+*#@@@@%+----------------------------
   ------------------------------+%*%@*===------==--::-++*+=====+*#*%@@@#*+----------------------------
   -------------------------------%@*%*=====---==+*@=+*%%%=====+**#*%%#*@-=----------------------------
   -------------------------------%-#@@========--==++*=*+++==++***##@#*-==-----------------------------
   -------------------------------===@@@+=====-=-===-=-=+++++++**##*%+=--------------------------------
   --------------------------------=@=@@@========+##*+*#%#***+*###@#-----------------------------------
   ----------------------------=====#*@*@++======++=+=++*#****###=@%-----------------------------------
   --=====-==-===-=-===---=--------=-**=%+++======*#%%%%#****###%==++==---=-------=====-=====--====--==
   ==============================------=*+++%+=====-===++*++*#@%%@=----==============================-=
   ====================================@@+=++#%==-=-====+++#@%@%%@@#===================================
   =================================#@@@%+=+++*##*+++***##@@%%%%%%@@@%=================================
   ==============================#@@%@@%++===+++##%%%%%@@@@%%%####%@@@@@%==============================
   ============================@@@@@@@@#*++====+*+*##%%%@@%%##*##%%@@@@@@@@============================
   =========================*@@@@@@@@@@#+=======++++*+*####*****###*@@@@@@@@@#=========================
   ===================#@@@@@@@@@@@@@@@##+========++++++**++*********@@@@@%@@@@@@@@@*===================
   ==============-@@%@@%@%@@@@@@@@%@@@#*===========+=++++++**++++++*@@@@@@%@@@@@@@@@*@@@-==============
   ==========@@%%@@@@%%@@@@@@@%%@@@@@@#*==========+=++++++*++++++++*@@@@@@@@%@@@@@%@%@@@@@@@*==========
   ===@@%@@@@%@@@@@@%%@@@%@@%%%%%%@@@@#++++=======+++++++=++++++===+#@@@@@@%%%%@@@@@@%@@@@@@@@@@@@@+===
   ==-@@@@@@%%@@@@@%%@%%@@@@%%%%%%%%@@%+=--------==++==+===========+*@%@@%%@%%%%%@@%@@%@@@@@@@@@@@@@===
   ==@%%%@@%%%@@@@%%@@@@@@@%%%%%%%%@@@%+=----------=====---========+*@@%%%%%%%%%%%@%%@@%@@@@@@@@@@%%@==
   +@%%%%%%%%%%%@%%:=:@@%%%%%%%%#@@@@%%+=---------======---========+*@@@@@%%%%%%%%%%:+-@%@@@@@@@@@%@@@=
   @%%%%%%@%%%%%%#%%*@@%%%%%%%@@@%@@@%%+==-------=======---=---====+:@@@@@@@%%%%%%@@@%#%#@@@@@@@@%%@@@@
   %%%%%#%@@@@@%%%@@@**%%%@@%@%%%%%%%%:+=---------====--------=====*:@@@%%%%%@%%%%%%@@@@@@@@@@@@@%%@@%@
   %%%@%%%@%@@@%%%%@@@@@#%%%%%%%##%%@%:+==--------===----------====::@@%%%%%%%%%@*@@@@@@@@@@@@@@@%@@@%@
   %##%*%%%%@@@%%%%%%%%%%%%%%#%####%@%-.==----------------------==.::@%%%%%%%%%#%%%@*@@@@@@@@@@@@%@@@%@
   @##%%%%%%@@@%%%%#%*%%%%%%########%%+..==---------------------=:.:.@%%%%%%%%%%%%%%#%%%@@@@@@@@@%@@@%@
   @*#*#%%%%@@@@%#:.%%##%%%##***###%%%:...=---------------------...:.@%%%%%%%%%%%%%%%#=:*%@@@@@@@@@@%%@
   %#%##@@%%@@@##...####%%%%####*###%%.....===--=====---------=.....#@%%%%%%%%%###%%%%..:#%*@@@@@@@@%@@
   #%*%#%@%@@@@@%%#*###%%%%%###**###%%%......=========-----==.......@@%%%%%%%%%####%%%%%%%@@@@@@@@@%@@@
   ##%@%@@@@@@@@%%@#+##%%%%%%#*****####-........+++++=====-.......::@@%%%%%%%%%*#####%%%#@@@@@@@@@@@@@@
   ###%@@@@@@@@@%%@@%*-#%%%%%#*******#%-............................@@@%%%%%%%##**##%%*@@@@@@@@@@@@@@@@
   ##%@%@@@@@@@@%%@@@@#####%%%#******##=..................:........@@@@%%%%%%%***+%=@@@@@@@@@@@@@@@@@@@
   #%%@@@@@@@@@@%%@@@@@@%###%%%##******%...........................@@@%%##%%%%##+##*@@@@@@@@@@@@@@@@@@@
   @%*%%@@@@@@@@%@@@@@@@@#%%%#%%%%##**#%-..........................@@%##*#%%%##%#%@@@@@@@@@@@@@@@@@@@@@
*/