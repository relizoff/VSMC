using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VSMC.Services.Data
{
    [Table("Videos")]
    public class VideoDomain
    {
        public int Id { get; set; }

        public string ProviderVideoId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Duration TimeSpan saved in milliseconds.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Uploaded DateTime saved as file time.
        /// </summary>
        public long Uploaded { get; set; }

        /// <summary>
        /// Saved DateTime saved as file time.
        /// </summary>
        public long Saved { get; set; }
        
        /// <summary>
        /// Viewed DateTime saved as file time.
        /// </summary>
        public long Viewed { get; set; }

        public double ViewPosition { get; set; }

        public string ThumbnailUrl { get; set; }

        public ChannelDomain Channel { get; set; }
        
        public int ChannelId { get; set; }

    }
}