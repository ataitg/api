namespace Identity.Enum
{
    [Flags]
    public enum UserRoles
    {
        None = 0,
        User = 1,
        Teacher = 2 | User,
        Admin = 4, 
    }
}
