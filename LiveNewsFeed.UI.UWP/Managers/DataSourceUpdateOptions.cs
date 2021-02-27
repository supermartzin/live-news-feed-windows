using System;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public sealed class DataSourceUpdateOptions
    {
        public DateTime? Before { get; set; }

        public DateTime? After { get; set; }

        public Category? Category { get; set; }

        public Tag? Tag { get; set; }

        public bool? Important { get; set; }

        public int? Count { get; set; }
    }
}