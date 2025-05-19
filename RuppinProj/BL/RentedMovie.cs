using RuppinProj.BL;

namespace RuppinProj.BL
{
    public class RentedMovie
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentStart { get; set; }
        public DateTime RentEnd { get; set; }
        public double TotalPrice { get; set; }   // ← הוסף את זה
        public DateTime? DeletedAt { get; set; } // ← הוסף את זה (Nullable כי זה יכול להיות null)
    }


}
