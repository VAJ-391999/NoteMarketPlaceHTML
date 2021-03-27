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

        public Profile profileUser { get; set; }

        public NoteReview noteReview { get; set; }


        //From CountryContex

        public Note noteDetails { get; set; }

        public NoteCountry countryDetails { get; set; }

        public NoteCategory categoryDetails { get; set; }

        public NoteType typeDetails { get; set; }

        public Note universityDetails { get; set; }

        public Note courseDetails { get; set; }

        public NoteReview ratingsDetails { get; set; }

        //to CountryContex

        public HttpPostedFileBase UserImage { get; set; }


    }
}