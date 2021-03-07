using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class NoteCountryContex
    {
        public Note noteDetails { get; set; }

        public NoteCountry countryDetails { get; set; }

        public NoteCategory categoryDetails { get; set; }

        public NoteType typeDetails { get; set; }

        public Note universityDetails { get; set; }

        public Note courseDetails { get; set; }

        public NoteReview ratingsDetails { get; set; }

    }
}