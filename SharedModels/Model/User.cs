using SharedModels.Enum;

namespace SharedModels.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserTypeEnum UserType { get; set; } = UserTypeEnum.DEFAULT;

        public override string ToString()
        {
            return $"User [Id={Id}, Username={Username}, Email={Email}, UserType={UserType}]";
        }
    }
}