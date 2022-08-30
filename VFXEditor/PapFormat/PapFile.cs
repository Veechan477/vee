using Dalamud.Logging;
using ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VFXEditor.FileManager;
using VFXEditor.Utils;
using VFXEditor.Interop;

namespace VFXEditor.PapFormat {
    public class PapFile : FileDropdown<PapAnimation> {
        private readonly string HkxTempLocation;

        private short ModelId;
        private byte BaseId;
        private byte VariantId;
        private readonly List<PapAnimation> Animations = new();

        public bool Verified = true;

        public PapFile( BinaryReader reader, string hkxTemp, bool checkOriginal = true ) : base( true ) {
            HkxTempLocation = hkxTemp;

            var startPos = reader.BaseStream.Position;

            byte[] original = null;
            if( checkOriginal ) {
                original = FileUtils.ReadAllBytes( reader );
                reader.BaseStream.Seek( startPos, SeekOrigin.Begin );
            }

            reader.ReadInt32(); // magic
            reader.ReadInt32(); // version
            var numAnimations = reader.ReadInt16();
            ModelId = reader.ReadInt16();
            BaseId = reader.ReadByte();
            VariantId = reader.ReadByte();

            reader.ReadInt32(); // info offset
            var havokPosition = reader.ReadInt32(); // from beginning
            var footerPosition = reader.ReadInt32();

            for( var i = 0; i < numAnimations; i++ ) {
                Animations.Add( new PapAnimation( reader, HkxTempLocation ) );
            }

            // ... do something about havok data ...
            var havokDataSize = footerPosition - havokPosition;
            reader.BaseStream.Seek( havokPosition, SeekOrigin.Begin );
            var havokData = reader.ReadBytes( havokDataSize );
            File.WriteAllBytes( HkxTempLocation, havokData );

            reader.BaseStream.Seek( footerPosition, SeekOrigin.Begin );
            for( var i = 0; i < numAnimations; i++ ) {
                Animations[i].ReadTmb( reader );
                reader.ReadBytes( Padding( reader.BaseStream.Position, i, numAnimations ) );
            }

            if( checkOriginal ) { // Check if output matches the original
                Verified = FileUtils.CompareFiles( original, ToBytes() );
            }
        }

        public byte[] ToBytes() {
            var havokData = File.ReadAllBytes( HkxTempLocation );
            var tmbData = Animations.Select( x => x.GetTmbBytes() );

            using MemoryStream dataMs = new();
            using BinaryWriter writer = new( dataMs );
            writer.BaseStream.Seek( 0, SeekOrigin.Begin );

            var startPos = writer.BaseStream.Position;

            writer.Write( 0x20706170 );
            writer.Write( 0x00020001 );
            writer.Write( ( short )Animations.Count );
            writer.Write( ModelId );
            writer.Write( BaseId );
            writer.Write( VariantId );

            var offsetPos = writer.BaseStream.Position; // coming back here later
            writer.Write( 0 ); // placeholders, will come back later
            writer.Write( 0 );
            writer.Write( 0 );

            var infoPos = writer.BaseStream.Position;
            foreach( var anim in Animations ) anim.Write( writer );

            var havokPos = writer.BaseStream.Position;
            writer.Write( havokData );

            FileUtils.PadTo( writer, writer.BaseStream.Position, 16 );

            var timelinePos = writer.BaseStream.Position;
            var idx = 0;
            foreach( var tmb in tmbData ) {
                writer.Write( tmb );

                var padding = Padding( writer.BaseStream.Position, idx, tmbData.Count() );
                for( var j = 0; j < padding; j++ ) writer.Write( ( byte )0 );

                idx++;
            }

            // go back and write sizes
            writer.BaseStream.Seek( offsetPos, SeekOrigin.Begin );
            writer.Write( ( int )( infoPos - startPos ) );
            writer.Write( ( int )( havokPos - startPos ) );
            writer.Write( ( int )( timelinePos - startPos ) );

            return dataMs.ToArray();
        }

        public void Draw( string id ) {
            if( ImGui.BeginTabBar($"{id}-MainTabs", ImGuiTabBarFlags.NoCloseWithMiddleMouseButton) ) {
                if( ImGui.BeginTabItem($"Parameters{id}")) {
                    FileUtils.ShortInput( $"Model Id{id}", ref ModelId );
                    FileUtils.ByteInput( $"Base Id{id}", ref BaseId );
                    FileUtils.ByteInput( $"Variant Id{id}", ref VariantId );

                    if( ImGui.Button( $"Export all Havok data{id}" ) ) {
                        FileDialogManager.SaveFileDialog( "Select a Save Location", ".hkx", "", "hkx", ( bool ok, string res ) => {
                            if( ok ) File.Copy( HkxTempLocation, res, true );
                        } );
                    }

                    ImGui.EndTabItem();
                }

                if( ImGui.BeginTabItem( $"Animations{id}" ) ) {
                    DrawDropDown( id, separatorBefore: false );

                    if( Selected != null ) {
                        Selected.Draw( $"{id}{Animations.IndexOf( Selected )}" );
                    }
                    else {
                        ImGui.Text( "Select an animation..." );
                    }

                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
        }

        private static int Padding( long position, int idx, int max ) { // Don't pad the last element
            if( max > 1 && idx < max - 1 ) {
                var leftOver = position % 4;
                return ( int )( leftOver == 0 ? 0 : 4 - leftOver );
            }
            return 0;
        }

        protected override List<PapAnimation> GetOptions() => Animations;

        protected override string GetName( PapAnimation item, int idx ) => item.GetName();

        protected override void OnNew() {
            FileDialogManager.OpenFileDialog( "Select a File", ".hkx,.*", ( bool ok, string res ) => {
                if( ok ) {
                    PapManager.IndexDialog.OnOk = ( int idx ) => {
                        var newAnim = new PapAnimation( HkxTempLocation );
                        newAnim.ReadTmb( Path.Combine( VfxEditor.RootLocation, "Files", "default_pap_tmb.tmb" ) );
                        Animations.Add( newAnim );
                        RefreshHavokIndexes();

                        HavokInterop.AddHavokAnimation( HkxTempLocation, res, idx, HkxTempLocation );

                        UiUtils.OkNotification( "Havok data imported" );
                    };
                    PapManager.IndexDialog.Show();
                }
            } );
        }

        protected override void OnDelete( PapAnimation item ) {
            var index = Animations.IndexOf( item );
            if( index == -1 ) return;

            Animations.RemoveAt( index );
            RefreshHavokIndexes();

            HavokInterop.RemoveHavokAnimation( HkxTempLocation, index, HkxTempLocation );

            UiUtils.OkNotification( "Havok data removed" );
        }

        public List<string> GetPapIds() => Animations.Select( x => x.GetName() ).ToList();

        private void RefreshHavokIndexes() {
            for( var i = 0; i < Animations.Count; i++ ) {
                Animations[i].HavokIndex = ( short )i;
            }
        }
    }
}
