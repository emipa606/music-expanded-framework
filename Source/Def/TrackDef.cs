using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace MusicExpanded
{
    public class TrackDef : SongDef
    {
        public bool vanillaLogic = false;
        public Cue cue = Cue.None;
        public string cueData;
        public static MethodInfo vanillaAppropriateNow = AccessTools.Method(typeof(RimWorld.MusicManagerPlay), "AppropriateNow");
        public bool IsBattleTrack => (cue <= Cue.BattleLegendary && cue >= Cue.BattleSmall);
        public bool AppropriateNow(MusicManagerPlay manager, SongDef lastPlayed)
        {
            if (
                lastPlayed == this
                || (cue != Cue.None && cue != Cue.HasColonistNamed)
            )
                return false;

            if (cue == Cue.HasColonistNamed && !Find.CurrentMap.PlayerPawnsForStoryteller.Where((pawn) => Utilities.NameMatches(pawn, cueData)).Any())
                return false;

            if (vanillaLogic)
                return (bool)vanillaAppropriateNow.Invoke(manager, new SongDef[] { this as SongDef });

            return true;
        }
        public static TrackDef FromSong(SongDef song)
        {
            TrackDef track = new TrackDef();
            // Def
            track.defName = song.defName;
            track.label = song.defName.Replace("_", " ");
            track.description = song.description;
            track.generated = true;
            // SongDef
            track.clipPath = song.clipPath;
            track.volume = song.volume;
            track.playOnMap = song.playOnMap;
            track.commonality = song.commonality;
            track.tense = song.tense;
            track.allowedTimeOfDay = song.allowedTimeOfDay;
            track.allowedSeasons = song.allowedSeasons;
            track.clip = song.clip;
            // Since this track is generated via vanilla SongDef, it feels safe to assume it'll use vanillaLogic.
            track.vanillaLogic = true;
            return track;
        }
    }
}
