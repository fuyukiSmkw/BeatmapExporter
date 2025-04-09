﻿using BeatmapExporterCore.Utilities;
using Realms;

// Original source file (modified by kabii) Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
namespace BeatmapExporterCore.Exporters.Lazer.LazerDB.Schema
{
    public class BeatmapSet : RealmObject
    {
        [PrimaryKey]
        public Guid ID { get; set; } = Guid.NewGuid();
        [Indexed]
        public int OnlineID { get; set; } = -1;
        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset? DateSubmitted { get; set; }
        public DateTimeOffset? DateRanked { get; set; }
        public IList<Beatmap> Beatmaps { get; } = null!;
        public IList<RealmNamedFileUsage> Files { get; } = null!;
        public int Status { get; set; } = -3;
        public bool DeletePending { get; set; }
        public string Hash { get; set; } = string.Empty;
        public bool Protected { get; set; }

        // Author kabii
        IList<Beatmap>? selected = null; // backing field for SelectedBeatmaps

        [Ignored]
        public string BeatmapID => OnlineID != -1 ? $"{OnlineID}" : "";

        /// <summary>
        /// Collection containing only the beatmaps from this set which are currently selected by the user.
        /// </summary>
        [Ignored]
        public IList<Beatmap> SelectedBeatmaps
        {
            get
            {
                return selected switch
                {
                    not null => selected,
                    null => Beatmaps
                };
            }
            set { selected = value; }
        }

        /// <summary>
        /// Collection containing only file hashes of difficulties which are not selected from this beatmap set
        /// For beatmap set export, every arbitrary file in the set, potentially hundreds, is exported except for these difficulty files
        /// </summary>
        [Ignored]
        public IList<string> ExcludedDiffHashes => Beatmaps
            .Where(b => !SelectedBeatmaps.Contains(b))
            .Select(b => b.Hash)
            .ToList();

        [Ignored]
        public IList<Score> AllScores => Beatmaps.SelectMany(b => b.Scores).ToList();

        /// <summary>
        /// The BeatmapMetadata taken from the first diff of this mapset.
        /// Other diffs could contain different metadata - but often we need to identify a mapset as a whole.
        /// </summary>
        [Ignored]
        public BeatmapMetadata? DiffMetadata => Beatmaps.FirstOrDefault()?.Metadata;

        /// <summary>
        /// A string which identifies this beatmap set including the beatmap id, song info, mapper name, and star rating of each difficulty.
        /// </summary>
        public string DiffSummary()
        {
            BeatmapMetadata metadata = Beatmaps.First().Metadata;
            var difficulties = SelectedBeatmaps.Select(b => b.StarRating).OrderBy(r => r).Select(r => r.ToString("0.00"));
            string difficultySpread = string.Join(", ", difficulties);

            return
                $"{OnlineID}: {metadata.Artist} - {metadata.Title} ({metadata.Author.Username} - {difficultySpread} stars)";
        }

        [Ignored]
        public IList<RealmNamedFileUsage> NamedFiles => Files.Where(f => !string.IsNullOrWhiteSpace(f.Filename)).ToList();

        /// <summary>
        /// The full filename to be used for exporting this mapset as a whole.
        /// </summary>
        public string ArchiveFilename()
        {
            BeatmapMetadata metadata = SelectedBeatmaps.First().Metadata;
            return
                $"{BeatmapID}{metadata.Artist.Trunc(30)} - {metadata.Title.Trunc(40)} ({metadata.Author.Username.Trunc(30)}).osz"
                .RemoveFilenameCharacters();
        }

        /// <summary>
        /// The filename to be used when exporting this mapset as a folder
        /// </summary>
        public string SongFolderName()
        {
            BeatmapMetadata metadata = SelectedBeatmaps.First().Metadata;
            return
                $"{BeatmapID} {metadata.Artist} - {metadata.Title}"
                .RemoveFilenameCharacters()
                .Trunc(155);
        }
    }
}
