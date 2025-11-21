namespace ProgPoe2.Models
{
    public static class UserRepository
    {
        public static List<User> Users { get; set; } = new List<User>
        {
            new User {
                UserID = 1,
                Name = "HR",
                Surname = "User",
                Email = "hr@university.com",
                Password = "123",
                Role = "HR",
                HourlyRate = 0,
                IsActive = true
            },
            new User {
                UserID = 2,
                Name = "John",
                Surname = "Lecturer",
                Email = "john@university.com",
                Password = "123",
                Role = "Lecturer",
                HourlyRate = 650m,
                IsActive = true
            },
            new User {
                UserID = 3,
                Name = "Sarah",
                Surname = "Coordinator",
                Email = "sarah@university.com",
                Password = "123",
                Role = "Coordinator",
                HourlyRate = 0,
                IsActive = true
            },
            new User {
                UserID = 4,
                Name = "Mike",
                Surname = "Manager",
                Email = "mike@university.com",
                Password = "123",
                Role = "Manager",
                HourlyRate = 0,
                IsActive = true
            }
        };
    }
}