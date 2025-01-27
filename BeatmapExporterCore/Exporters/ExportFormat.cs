﻿namespace BeatmapExporterCore.Exporters
{
    /// <summary>
    /// All available modes of exporting.
    /// </summary>
    public enum ExportFormat { Beatmap, Audio, Background, Replay, Folder, CollectionDb };

    public static class ExportFormatExtensions
    {
        /// <summary>
        /// A string describing what this export format targets. Simple, for inlining, for example: "beatmap backgrounds"
        /// </summary>
        public static string UnitName(this ExportFormat format) => format switch
        {
            ExportFormat.Beatmap => "osu! beatmaps (.osz)",
            ExportFormat.Audio => "audio (.mp3)",
            ExportFormat.Background => "beatmap backgrounds",
            ExportFormat.Replay => "score replays (.osr)",
            ExportFormat.Folder => "osu!stable \"Songs\" folder",
            ExportFormat.CollectionDb => "osu!stable \"collection.db\"",
            _ => throw new NotImplementedException()
        };

        /// <summary>
        /// A string describing the actions that the export mode will perform.
        /// </summary>
        public static string Descriptor(this ExportFormat format) => format switch
        {
            ExportFormat.Beatmap => "Beatmaps will be exported in osu! archive format (.osz).",
            ExportFormat.Audio => "Beatmap audio files will be renamed, tagged and exported (.mp3 format).",
            ExportFormat.Background => "Only beatmap background images will be exported (original format).",
            ExportFormat.Replay => "Player score replays will be exported (.osr).",
            ExportFormat.Folder => "Beatmaps will be exported as an unarchived \"Songs\" folder ready for use with osu! stable.",
            ExportFormat.CollectionDb => "Collections will be exported as the collection.db file used by osu! stable. Beatmap filters will still be applied.",
            _ => throw new NotImplementedException() 
        };

        /// <summary>
        /// The 'next' ExportFormat, based on the natural ordering of the ExportFormat enum.
        /// </summary>
        public static ExportFormat Next(this ExportFormat format)
        {
            var max = Enum.GetValues(typeof(ExportFormat)).Length;
            var iNext = ((int)format + 1) % max;
            return (ExportFormat)iNext;
        }
    }

    public static class ExportFormats
    {
        /// <summary>
        /// Array of all ExportFormat enum values.
        /// </summary>
        public static IEnumerable<ExportFormat> All() => (ExportFormat[])Enum.GetValues(typeof(ExportFormat));
    }
}
