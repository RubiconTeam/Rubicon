using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Array = Godot.Collections.Array;

namespace Rubicon.Extras;

using Godot;

public partial class FunkinChartPlugin : EditorImportPlugin
{
	public override string _GetImporterName() => "com.binpuki.rubiconfunkinchart";

	public override string _GetVisibleName() => "Rubicon Chart (Funkin')";

	public override string[] _GetRecognizedExtensions() => ["json"];

	public override string _GetSaveExtension() => "res";

	public override string _GetResourceType() => "RubiChart";

	public override int _GetPresetCount() => 1;

	public override string _GetPresetName(int presetIndex) => "Default";

	public override Array<Dictionary> _GetImportOptions(string path, int presetIndex)
	{
		return
		[
			new Dictionary
			{
				{ "name", "StoreExternally" },
				{ "default_value", false }
			},
			new Dictionary
			{
				{ "name", "CreateSongMeta" },
				{ "default_value", true }
			},
		];
	}

	public override float _GetPriority() => 1f;

	public override int _GetImportOrder() => 16;

	public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

	public override Error _Import(string sourceFile, string savePath, Dictionary options, Array<string> platformVariants, Array<string> genFiles)
	{
		using FileAccess file = FileAccess.Open(sourceFile, FileAccess.ModeFlags.Read);
		if (file.GetError() != Error.Ok)
			return Error.Failed;

		Dictionary json = Json.ParseString(file.GetAsText()).AsGodotDictionary();
		if (!json.ContainsKey("song"))
		{
			GD.PrintErr($"The JSON file given at {sourceFile.GetBaseName()} is not a Funkin' Chart!");	
			return Error.InvalidData;
		}
		
		RubiChart chart = new RubiChart();
        Dictionary swagSong = json["song"].AsGodotDictionary();
        Array<BpmInfo> bpmChanges =
        [
	        new BpmInfo { Time = 0, Bpm = (float)swagSong["bpm"].AsDouble() }
        ];

        if (swagSong.ContainsKey("speed"))
            chart.ScrollSpeed = swagSong["speed"].AsSingle() * 0.45f * 1.5f;

        Array<NoteData> playerNotes = [];
        Array<NoteData> opponentNotes = [];
        Array<NoteData> speakerNotes = [];

        Array<EventData> cameraChanges = [];
            
        int lastCamera = 0;
        double measureTime = 0f;
        Array sections = swagSong["notes"].AsGodotArray();
        for (int i = 0; i < sections.Count; i++)
        {
            Dictionary curSection = sections[i].AsGodotDictionary();
            if (bpmChanges.Where(x => x.Time == i).Count() == 0 && curSection.ContainsKey("changeBPM") && curSection["changeBPM"].AsBool() == true)
                bpmChanges.Add(new BpmInfo { Time = i, Bpm = (float)curSection["bpm"].AsDouble() });
                    
            double measureBpm = bpmChanges.Last(x => x.Time <= i).Bpm;

            bool playerSection = curSection["mustHitSection"].AsBool();
            int sectionCamera = playerSection ? 1 : 0;
                
            bool gfSection = curSection.ContainsKey("gfSection") ? curSection["gfSection"].AsBool() : false;
            if (gfSection)
                sectionCamera = 2;
                
            if (lastCamera != sectionCamera)
                cameraChanges.Add(new EventData { Time = i, Name = "Set Camera Focus", Arguments = [ sectionCamera ] });
                
            lastCamera = sectionCamera;
                
            Array notes = curSection["sectionNotes"].AsGodotArray();
            for (int n = 0; n < notes.Count; n++)
            {
                Array parsedNote = notes[n].AsGodotArray();
                NoteData note = new NoteData()
                {
                    Time = ((parsedNote[0].AsDouble() - measureTime) / (60d / measureBpm * 4d) / 1000d) + i,
                    Lane = parsedNote[1].AsInt32() % 4,
                    Length = parsedNote[2].AsDouble() / (60d / measureBpm * 4d) / 1000d,
                    Type = parsedNote.Count > 3 ? parsedNote[3].AsString() : "normal"
                };

                if (parsedNote[0].AsDouble() < measureTime)
                    GD.Print($"Measure {i}, note {n}, lane {parsedNote[1].AsUInt32()}: time of {parsedNote[0].AsDouble()} exceeds calculated measure start time of {measureTime}! Calculated milliseconds will be {parsedNote[0].AsDouble() - measureTime}, measure {note.MsTime}");

                uint lane = parsedNote[1].AsUInt32();
                if (lane <= 3)
                {
                    if (playerSection) playerNotes.Add(note);
                    else
                    {
                        if (gfSection) speakerNotes.Add(note);
                        else opponentNotes.Add(note);
                    }
                }
                else if (lane <= 7)
                {
                    if (playerSection) opponentNotes.Add(note);
                    else playerNotes.Add(note);
                }
                else speakerNotes.Add(note);
            }
            
            measureTime += ConductorUtility.MeasureToMs(1d, measureBpm, 4d);
        }

        chart.BpmInfo = bpmChanges.ToArray();

        bool speakerHasNotes = speakerNotes.Count > 0;
        if (speakerHasNotes)
        {
	        chart.Charts = [
		        new IndividualChart
		        {
			        Name = "Opponent",
			        Notes = opponentNotes.ToArray(),
			        Lanes = 4
		        },
		        new IndividualChart
		        {
			        Name = "Player",
			        Notes = playerNotes.ToArray(),
			        Lanes = 4
		        },
		        new IndividualChart 
		        {
			        Name = "Speaker",
			        Notes = speakerNotes.ToArray(),
			        Lanes = 4
		        }
	        ];
        }
        else
        {
	        chart.Charts = [
		        new IndividualChart
		        {
			        Name = "Opponent",
			        Notes = opponentNotes.ToArray(),
			        Lanes = 4
		        },
		        new IndividualChart
		        {
			        Name = "Player",
			        Notes = playerNotes.ToArray(),
			        Lanes = 4
		        }
	        ];
        }
        
        chart.Format();

        if (options.ContainsKey("CreateSongMeta") && options["CreateSongMeta"].AsBool())
        {
	        SongMeta meta = new SongMeta();
	        //meta.Events = cameraChanges.ToArray();
	        meta.Stage = swagSong.ContainsKey("stage") ? swagSong["stage"].AsString() : "stage";
	        meta.PlayableCharts = ["Player", "Opponent"];
	        meta.Characters =
	        [
		        new CharacterMeta
		        {
			        Character = swagSong.ContainsKey("player2") ? swagSong["player2"].AsString() : "Missing",
			        BarLine = "Opponent",
			        Nickname = "Opponent"
		        },
		        new CharacterMeta
		        {
			        Character = swagSong.ContainsKey("player1") ? swagSong["player1"].AsString() : "Missing",
			        BarLine = "Player",
			        Nickname = "Player"
		        },
		        new CharacterMeta
		        {
			        Character = swagSong.ContainsKey("gfVersion") ? swagSong["gfVersion"].AsString() : "Missing",
			        BarLine = speakerHasNotes ? "Speaker" : "",
			        Nickname = "Speaker"
		        }
	        ];

	        string metaFileName = sourceFile.GetBaseDir() + "/meta.tres";
	        if (!FileAccess.FileExists(metaFileName))
		        ResourceSaver.Save(meta, metaFileName);
        }
        
        swagSong.Dispose();

        if (options["StoreExternally"].AsBool())
	        return ResourceSaver.Save(chart, $"{sourceFile.GetBaseName()}.tres");
        
        return ResourceSaver.Save(chart, $"{savePath}.res", ResourceSaver.SaverFlags.Compress);
	}
}