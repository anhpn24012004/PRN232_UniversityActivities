namespace WebApp_Client.Mvc;

public static class DisplayNames
{
    public static string ActivityTypeName(int value) => value switch
    {
        1 => "Workshop",
        2 => "Seminar",
        3 => "Competition",
        4 => "Volunteer",
        5 => "ClubEvent",
        _ => value.ToString()
    };

    public static string ActivityStatusName(int value) => value switch
    {
        1 => "Pending",
        2 => "Approved",
        3 => "Rejected",
        4 => "Cancelled",
        _ => value.ToString()
    };

    public static string RegistrationStatusName(int value) => value switch
    {
        1 => "Registered",
        2 => "Attended",
        3 => "Cancelled",
        4 => "Absent",
        _ => value.ToString()
    };
}
