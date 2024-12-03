namespace Pawsome.Models
{
    public class ManagePenaltiesViewModel
    {
        public List<User> Users { get; set; } // List of users
        public List<PenaltyAssignment> PenaltyAssignments { get; set; } // List of penalty assignments
        public List<PenaltyAssignment> PaidPenalties { get; set; } // List of paid penalties
        public List<PenaltyFine> PenaltyFines { get; set; } // List of available penalty fines
    }

}
