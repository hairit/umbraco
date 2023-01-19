namespace Marcus.Umbraco.Model
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string? ContentName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
    }
}
