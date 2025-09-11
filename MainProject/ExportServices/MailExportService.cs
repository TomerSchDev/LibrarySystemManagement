using System.IO;
using System.Windows;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using LibrarySystemModels.Services;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Library_System_Management.ExportServices;

public class SmtpConfig
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public bool Filled { get; set; }
}
public class MailExportService:IDataExportService
{
    private const string SmtpJsonKey = "Smtp";
    private static readonly CsvExportService CsvExportService = new();

    public string Name => "Mail";

    private static MailboxAddress? ParseMailboxAddress(string mailboxAddress)
    {
        try
        {
            return MailboxAddress.Parse(mailboxAddress);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Error parsing Mail address {mailboxAddress} , error : "+e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    private static SmtpConfig ParseConfig()
    {
        var smtpConfig = new SmtpConfig();
        try
        {
            
            smtpConfig=new SmtpConfig
            {
                Username =ConfigHelper.GetString(SmtpJsonKey+ ":Username"),
                Password = ConfigHelper.GetString(SmtpJsonKey+ ":Password"),
                Host = ConfigHelper.GetString(SmtpJsonKey+ ":Host"),
                Port = Convert.ToInt32(ConfigHelper.GetString(SmtpJsonKey+ ":Port")),
                Filled = true
            };
        }
        catch (Exception e)
        {
            MessageBox.Show("Error parsing SMTP config, error : "+e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ReportingService.ReportEventAsync(FlowSide.Client,SeverityLevel.MEDIUM,"Error parsing SMTP config file");
        }

        return smtpConfig;
    }

    public bool Export(IEnumerable<IExportable> data, string filePath)
    {
        var smtpConfig = ParseConfig();
        if (!smtpConfig.Filled) return false;
        var emailDialog = new EmailPromptWindow();
        if (emailDialog.ShowDialog() != true) return false;
        var recipientEmail = emailDialog.EnteredEmail;

        var message = new MimeMessage();
        var addressFrom = ParseMailboxAddress(smtpConfig.Username ?? string.Empty);
        if (addressFrom == null) return false;

        message.From.Add(addressFrom);
        var addressTo = ParseMailboxAddress(recipientEmail);
        if (addressTo == null) return false;

        message.To.Add(addressTo);
        message.Subject = "Exported Data from Library System";
        message.Body = new TextPart("plain") { Text = "See attachment" };
        var multipart = new Multipart("mixed");
        multipart.Add(message.Body);
        CsvExportService.Export(data, filePath);
        var newFilePath = filePath + ".csv";
        var fileBytes = File.ReadAllBytes(newFilePath);
        var attachment = new MimePart("application", "octet-stream")
        {
            Content = new MimeContent(new MemoryStream(fileBytes)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(newFilePath)
        };
        File.Delete(newFilePath);
        multipart.Add(attachment);
        message.Body = multipart;
        using var client = new SmtpClient();
        try
        {
            client.Connect(smtpConfig.Host, smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);

            // Use your Gmail address and app password:
            client.Authenticate(smtpConfig.Username, smtpConfig.Password);
            client.Send(message);
            client.Disconnect(true);
        }
        catch (Exception e)
        {
            MessageBox.Show("Error sending email, error : " + e.Message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            ReportingService.ReportEventAsync(FlowSide.Client,SeverityLevel.HIGH, "Error exporting data by mail");
            return false;
        }
        return true;
    }

}