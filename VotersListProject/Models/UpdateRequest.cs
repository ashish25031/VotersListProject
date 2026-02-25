namespace VotersListProject.Models
{
    public class UpdateRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string RequestedField { get; set; }

        public string NewValue { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}
