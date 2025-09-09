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
            var smtpElementNullable = ConfigHelper.LoadConfigProperty(SmtpJsonKey);
            if (smtpElementNullable == null || smtpElementNullable.Value.ValueKind == JsonValueKind.Undefined)
                throw new Exception("Missing SMTP config section!");

            var smtp = smtpElementNullable.Value;
            smtpConfig=new SmtpConfig
            {
                Username = smtp.GetProperty("Username").GetString(),
                Password = smtp.GetProperty("Password").GetString(),
                Host = smtp.GetProperty("Host").GetString(),
                Port = smtp.GetProperty("Port").GetInt32(),
                Filled = true
            };
        }
        catch (Exception e)
        {
            MessageBox.Show("Error parssing SMTP config, error : "+e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return smtpConfig;
    }
    public void Export(IEnumerable<IExportable> data, string filePath)
    {
        var smtpConfig = ParseConfig();
        if (!smtpConfig.Filled) return;
        var emailDialog = new EmailPromptWindow();
        if (emailDialog.ShowDialog() != true) return;
        var recipientEmail = emailDialog.EnteredEmail;
        new CsvExportService().Export(data, filePath);
        var newFilePath = filePath + ".csv";
        var message = new MimeMessage();
        var addressFrom = ParseMailboxAddress("");
        if (addressFrom == null) return;
        
        message.From.Add(addressFrom);
        var addressTo = ParseMailboxAddress(recipientEmail);
        if (addressTo == null) return;

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
    }
}