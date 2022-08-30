using System.Collections.Generic;
using System.IO;
using System.Linq;
using VFXEditor.Select.Sheets;

namespace VFXEditor.Select {
    public class SheetManager {
        public static string NpcNamesOldPath { get; private set; }
        public static string NpcFilesPath { get; private set; }
        public static string NpcNamesPath { get; private set; }
        public static string MiscVfxPath { get; private set; }
        public static string MiscTmbPath { get; private set; }

        public static ItemSheetLoader Items { get; private set; }
        public static ActionSheetLoader Actions { get; private set; }
        public static NonPlayerActionSheetLoader NonPlayerActions { get; private set; }
        public static CutsceneSheetLoader Cutscenes { get; private set; }
        public static EmoteSheetLoader Emotes { get; private set; }
        public static GimmickSheetLoader Gimmicks { get; private set; }
        public static StatusSheetLoader Statuses { get; private set; }
        public static ZoneSheetLoader Zones { get; private set; }
        public static MountSheeetLoader Mounts { get; private set; }
        public static HousingSheetLoader Housing { get; private set; }
        public static CommonLoader Misc { get; private set; }

        public static ActionTmbSheetLoader ActionTmb { get; private set; }
        public static NonPlayerActionTmbSheetLoader NonPlayerActionTmb { get; private set; }
        public static EmoteTmbSheetLoader EmoteTmb { get; private set; }
        public static CommonTmbLoader MiscTmb { get; private set; }

        public static ActionPapSheetLoader ActionPap { get; private set; }
        public static NonPlayerActionPapSheetLoader NonPlayerActionPap { get; private set; }
        public static EmotePapSheetLoader EmotePap { get; private set; }

        // Contains vfx, tmb, and paps
        public static NpcSheetLoader Npcs { get; private set; }

        public struct RaceStruct {
            public string SkeletonId;
            public int MinFace;
            public int MaxFace;

            public RaceStruct(string skeletonId, int minFace, int maxFace) {
                SkeletonId = skeletonId;
                MinFace = minFace;
                MaxFace = maxFace;
            }
        }

        public static readonly Dictionary<string, RaceStruct> RaceAnimationIds = new() {
            { "Midlander M", new RaceStruct("c0101", 1, 4) },
            { "Midlander F", new RaceStruct("c0201", 1, 4) },
            { "Highlander M", new RaceStruct("c0301", 1, 4) },
            { "Highlander F", new RaceStruct("c0401", 1, 4) },
            { "Elezen M", new RaceStruct("c0501", 1, 4) },
            { "Elezen F", new RaceStruct("c0601", 1, 4) },
            { "Miquote M", new RaceStruct("c0701", 1, 4) },
            { "Miquote F", new RaceStruct("c0801", 1, 4) },
            { "Roegadyn M", new RaceStruct("c0901", 1, 4) },
            { "Roegadyn F", new RaceStruct("c1001", 1, 4) },
            { "Lalafell M", new RaceStruct("c1101", 1, 4) },
            { "Lalafell F", new RaceStruct("c1201", 1, 4) },
            { "Aura M", new RaceStruct("c1301", 1, 4) },
            { "Aura F", new RaceStruct("c1401", 1, 4) },
            { "Hrothgar M", new RaceStruct("c1501", 1, 4) },
            // 1601 coming soon (tm)
            { "Viera M", new RaceStruct("c1701", 1, 4) },
            { "Viera F", new RaceStruct("c1801", 1, 4) },
        };

        public static readonly Dictionary<string, string> JobAnimationIds = new() {
            { "Warrior", "bt_2ax_emp" },
            { "Paladin", "bt_swd_sld" },
            { "Gunbreaker", "bt_2gb_emp" },
            { "Dark Knight", "bt_2sw_emp" },
            { "Astrologian", "bt_2gl_emp" },
            { "Sage", "bt_2ff_emp" },
            { "Scholar", "bt_2bk_emp" },
            { "White Mage", "bt_stf_sld" },
            { "Machinist", "bt_2gn_emp" },
            { "Dancer", "bt_chk_chk" },
            { "Bard", "bt_2bw_emp" },
            { "Samurai", "bt_2kt_emp" },
            { "Dragoon", "bt_2sp_emp" },
            { "Monk", "bt_clw_clw" },
            { "Ninja", "bt_dgr_dgr" },
            { "Reaper", "bt_2km_emp" },
            { "Red Mage", "bt_2rp_emp" },
            { "Black Mage", "bt_stf_sld" },
            { "Summoner", "bt_2bk_emp" },
            { "Blue Mage", "bt_rod_emp" },
        };

        public static void Initialize() {
            NpcNamesOldPath = Path.Combine( Plugin.RootLocation, "Files", "npc_names_old.csv" );
            NpcFilesPath = Path.Combine( Plugin.RootLocation, "Files", "npc_files.json" );
            NpcNamesPath = Path.Combine( Plugin.RootLocation, "Files", "npc_names.json" );
            MiscVfxPath = Path.Combine( Plugin.RootLocation, "Files", "vfx_misc.txt" );
            MiscTmbPath = Path.Combine( Plugin.RootLocation, "Files", "tmb_misc.txt" );

            Items = new ItemSheetLoader();
            Actions = new ActionSheetLoader();
            NonPlayerActions = new NonPlayerActionSheetLoader();
            Cutscenes = new CutsceneSheetLoader();
            Emotes = new EmoteSheetLoader();
            Gimmicks = new GimmickSheetLoader();
            Npcs = new NpcSheetLoader();
            Statuses = new StatusSheetLoader();
            Zones = new ZoneSheetLoader();
            Mounts = new MountSheeetLoader();
            Housing = new HousingSheetLoader();

            ActionTmb = new ActionTmbSheetLoader();
            NonPlayerActionTmb = new NonPlayerActionTmbSheetLoader();
            EmoteTmb = new EmoteTmbSheetLoader();
            MiscTmb = new CommonTmbLoader();

            ActionPap = new ActionPapSheetLoader();
            NonPlayerActionPap = new NonPlayerActionPapSheetLoader();
            EmotePap = new EmotePapSheetLoader();

            Misc = new CommonLoader();
        }

        public static Dictionary<string, string> FileExistsFilter( Dictionary<string, string> dict ) => dict.Where( x => Plugin.DataManager.FileExists( x.Value ) ).ToDictionary( x => x.Key, x => x.Value );

        public static string GetSkeletonpath( string skeletonId, string path ) => $"chara/human/{skeletonId}/animation/a0001/{path}";

        public static Dictionary<string, string> GetAllSkeletonPaths( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, string>();
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetSkeletonpath( x.Value.SkeletonId, path ) );
        }

        public static Dictionary<string, string> GetAllJobSkeletonPaths( string jobId, string path ) => FileExistsFilter( GetAllSkeletonPaths( $"{jobId}/{path}.pap" ) );

        public static Dictionary<string, Dictionary<string, string>> GetAllJobPaths( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, Dictionary<string, string>>();
            return JobAnimationIds.ToDictionary( x => x.Key, x => GetAllJobSkeletonPaths( x.Value, path ) );
        }

        public static Dictionary<string, string> GetAllModelFacePaths( string modelId, string path, int minFace, int maxFace ) {
            Dictionary<string, string> ret = new();
            for( var face = minFace; face <= maxFace; face++ ) {
                ret.Add( $"Face {face}", $"chara/human/{modelId}/animation/f{face:D4}/nonresident/{path}.pap" );
            }

            return FileExistsFilter( ret );
        }

        public static Dictionary<string, Dictionary<string, string>> GetAllFacePaths( string path ) {
            if( string.IsNullOrEmpty( path ) ) return new Dictionary<string, Dictionary<string, string>>();
            return RaceAnimationIds.ToDictionary( x => x.Key, x => GetAllModelFacePaths( x.Value.SkeletonId, path, x.Value.MinFace, x.Value.MaxFace ) );
        }
    }
}
