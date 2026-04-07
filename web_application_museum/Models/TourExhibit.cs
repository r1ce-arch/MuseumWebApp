using System.ComponentModel.DataAnnotations.Schema;

namespace MuseumWebApp.Models
{
    public class TourExhibit
    {
        public int TourId { get; set; }

        [ForeignKey(nameof(TourId))]
        public Tour? Tour { get; set; }

        public int ExhibitId { get; set; }

        [ForeignKey(nameof(ExhibitId))]
        public Exhibit? Exhibit { get; set; }
    }
}

