
using System.Linq;
using System.Net;
using System.Web;

namespace MyClubLib.Repository
{
    public enum MasterEntity
    {
        Service     = 1,
        Offer       = 2,
        Trainer     = 3,
        Member      = 4,
        MemberOffer = 5,
        //Setting     = 6,
    }

    public enum Service
    {
        Fittnes  = 1,
        Zomba    = 2,
        Sauna    = 3,
        Cardio   = 4,
        Yoga     = 5,
        Swimming = 6,
    }
    public enum Action
    {

        
        Create_Member      = 1,
        Edit_Member        = 2,
        Delete_Member      = 3,
        
        Create_Service     = 4,
        Delete_Service     = 5,
        Enable_Service     = 6,
        Disable_Service    = 7,
        Edit_ServicePrice  = 8,
        Edit_ServiceName   = 9,

        Attendance_Edit         = 11,
        Attendance_Registration = 12,
        Attendance_Confirmation = 13,

        create_MemberOffer = 13,
        Delete_MemberOffer = 14,
        Edit_MemberOffer   = 15,
        Start_MemberOffer  = 16,
        End_MemberOffer    = 17,


        //Create_Profile = 1,
        //Edit_Profile   = 2,
        // Delete_Profile = 3,
        //Apply_Offer        = 25,
        //Create_Trainer     = 4,
        //Edit_Trainer       = 5,
        //Delete_Trainer     = 6,
    }

    public enum ActionType
    {
        Add    = 1,
        Edit   = 2,
        Delete = 3 
    }

    public enum offerStatus
    {
        Active  = 1,
        unctive = 2,

    }
    public class Utilities
    {
       public string GetIpAddress()
       {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            var hostEntry   = Dns.GetHostEntry(hostName);
            var ipAddress   = hostEntry.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

            return ipAddress?.ToString();
        }
    }
}