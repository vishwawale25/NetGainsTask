using System.ComponentModel.DataAnnotations;

namespace NetGainsTask.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Qualification { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
    }
}
