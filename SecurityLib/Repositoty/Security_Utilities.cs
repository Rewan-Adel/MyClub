namespace SecurityLib.Repositoty
{
    public enum Status 
    {
        Cancelled = 0,
        Created = 1,
        Completed = 2,
        Not_completed = 3,
        unUsed = 4
    }

    public enum Roles
    {
        Admin   = 1,
        Member  = 2,
        Trainer = 3
    }

    public enum PageName
    {
        signup = 1,
        login  = 2,
        logout = 3,
       // Home  = 4,
       // Offers = 5,
        Members = 4,
        MemberOffers = 5,
        Services = 6,
        Reports = 7,
    }
    public class Security_Utilities
    {
    }
}