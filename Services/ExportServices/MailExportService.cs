using System.IO;
using System.Net.Mail;
using System.Text.Json;
using System.Windows;
using Library_System_Management.Helpers;
using Library_System_Management.Models;
using Library_System_Management.Views.PopUpDIalogs;
using MimeKit;

namespace Library_System_Management.Services.ExportServices;
using MailKit.Net.Smtp;

public class SmtpConfig
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; }
    public bool Filled { get; set; }
}
public class MailExportService : IDataExportService
{
    private const string SmtpJsonKey = "Smtp";

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
                Username = ConfigHelper.GetString(SmtpJsonKey+":Username"),
                Password = ConfigHelper.GetString(SmtpJsonKey+":Password"),
                Host = ConfigHelper.GetString(SmtpJsonKey+":Host"),
                Port = int.Parse(ConfigHelper.GetString(SmtpJsonKey+":Port") ?? "0"),
                Filled = true
            };
        }
        catch (Exception e)
        {
            MessageBox.Show("Error parsing SMTP config, error : "+e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        new CsvExportService().Export(data, filePath);
        var newFilePath = filePath + ".csv";
        var message = new MimeMessage();
        if (smtpConfig.Username == null) return false;
        var addressFrom = ParseMailboxAddress(smtpConfig.Username);
        if (addressFrom == null) return false;
        
        message.From.Add(addressFrom);
        var addressTo = ParseMailboxAddress(recipientEmail);
        if (addressTo == null) return false;

        message.To.Add(addressTo);
        message.Subject = "Exported Data from Library System";
        message.Body = new TextPart("plain") { Text = "See attachment" };

        var attachment = new MimePart("application", "octet-stream")
        {
            Content = new MimeContent(File.OpenRead(newFilePath)),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = Path.GetFileName(filePath)
        };

        var multipart = new Multipart("mixed");
        multipart.Add(message.Body);
        multipart.Add(attachment);
        message.Body = multipart;
        using var client = new SmtpClient();
        client.Connect(smtpConfig.Host, smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);

        // Use your Gmail address and app password:
        client.Authenticate(smtpConfig.Username, smtpConfig.Password);
        client.Send(message);
        client.Disconnect(true);
        return true;
    }
}