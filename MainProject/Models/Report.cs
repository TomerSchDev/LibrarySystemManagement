namespace Library_System_Management.Models;

public enum SeverityLevel
{
    INFO = 0,
    LOW = 1,
    MEDIUM = 2 ,
    HIGH = 3 ,
    CRITICAL =4
}
public record Report() : IExportable
{
    public Report(SeverityLevel SeverityLevel, string User,int userId, UserRole Role, DateTime ActionDate, string ReportMessage) : this()
    {
        this.SeverityLevel=SeverityLevel;
        this.User=User;
        this.Role=Role;
        this.ActionDate=ActionDate;
        this.ReportMessage=ReportMessage;
    }

    public SeverityLevel SeverityLevel { get; set; }
    public string User { get; set; }
    public int UserId { get; set; }
    public UserRole Role { get; set; }
    public DateTime ActionDate { get; set; }
    public int ReportID { get; set; }
    public string ReportMessage { get; set; }
    public String RoleString => Role.ToString();
    
    public string SeverityLevelString => SeverityLevel.ToString();
    public string ExportClassName => "Report";
}