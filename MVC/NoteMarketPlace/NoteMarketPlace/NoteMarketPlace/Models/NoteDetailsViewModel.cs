using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class NoteDetailsViewModel
    {
        public Note note { get; set; }

        public NoteCountry country { get; set; }

        public User user { get; set; }

        public NoteCategory noteCategory { get; set; }

        public DownloadedNote download { get; set; }

        public ReferenceData referenceData { get; set; }
    }
}