using AVFXLib.Models;
using Dalamud.Plugin;
using System.Collections.Generic;
using System.IO;
using VFXEditor.UI.VFX;
using VFXSelect.UI;

namespace VFXEditor {
    public class ReplaceDoc {
        public UIMain Main = null;
        public string WriteLocation;

        public VFXSelectResult Source;
        public VFXSelectResult Replace;

        public void SetAVFX(AVFXBase avfx) {
            Dispose();
            Main = new UIMain( avfx );
        }

        public void Dispose() {
            Main?.Dispose();
            Main = null;
            File.Delete( WriteLocation );
        }
    }

    public class PluginDocumentManager {
        public ReplaceDoc ActiveDoc;
        public List<ReplaceDoc> Docs = new();

        private Dictionary<string, string> GamePathToLocalPath = new();
        private Plugin Plugin;
        private int DOC_ID = 0;

        public PluginDocumentManager( Plugin plugin ) {
            Plugin = plugin;
            NewDoc();
        }

        public ReplaceDoc NewDoc() {
            ReplaceDoc doc = new ReplaceDoc();
            doc.WriteLocation = Path.Combine( Plugin.WriteLocation, "VFXtemp" + ( DOC_ID++ ) + ".avfx" );
            doc.Source = VFXSelectResult.None();
            doc.Replace = VFXSelectResult.None();

            Docs.Add( doc );
            ActiveDoc = doc;
            return doc;
        }

        public void UpdateSource( VFXSelectResult select ) {
            ActiveDoc.Source = select;
        }

        public void UpdateReplace(VFXSelectResult select ) {
            ActiveDoc.Replace = select;
            RebuildMap();
        }

        public void SelectDoc(ReplaceDoc doc ) {
            ActiveDoc = doc;
        }

        public bool RemoveDoc(ReplaceDoc doc ) {
            bool switchDoc = ( doc == ActiveDoc );
            Docs.Remove( doc );
            doc.Dispose();
            RebuildMap();

            if( switchDoc ) {
                ActiveDoc = Docs[0];
                return true;
            }
            return false;
        }

        private void RebuildMap() {
            Dictionary<string, string> newMap = new();
            foreach( var doc in Docs ) {
                if(doc.Replace.Path != "") {
                    newMap[doc.Replace.Path] = doc.WriteLocation;
                }
            }
            GamePathToLocalPath = newMap;
        }

        public void Save() {
            Plugin.SaveLocalFile( ActiveDoc.WriteLocation, ActiveDoc.Main.AVFX );
            if(Configuration.Config.LogAllFiles) {
                PluginLog.Log( $"Saved VFX to {ActiveDoc.WriteLocation}" );
            }
        }

        public bool GetLocalPath(string gamePath, out FileInfo file ) {
            file = null;
            if( GamePathToLocalPath.ContainsKey( gamePath ) ) {
                file = new FileInfo( GamePathToLocalPath[gamePath] );
                return true;
            }
            return false;
        }

        public bool HasReplacePath(bool allDocuments ) {
            if( allDocuments ) {
                foreach(var doc in Docs ) {
                    if( doc.Replace.Path == "" )
                        return false;
                }
                return true;
            }
            return ActiveDoc.Replace.Path != "";
        }

        public void Dispose() {
            foreach(var doc in Docs ) {
                doc.Dispose();
            }
            Docs = null;
            ActiveDoc = null;
            GamePathToLocalPath = null;
        }
    }
}
