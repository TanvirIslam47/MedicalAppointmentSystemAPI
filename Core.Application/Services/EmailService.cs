using Core.Application.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;
using System.Net;
using System.Net.Mail;
using Document = iTextSharp.text.Document;

namespace Core.Application.Services
{
    public class EmailService : IEmailService
    {

        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAppointmentEmailAsync(AppointmentListDto appointment)
        {
            if (string.IsNullOrWhiteSpace(appointment.PatientEmail))
                return;

            string subject = $"Your Appointment Confirmation - {appointment.AppointmentDate:dd-MMM-yyyy}";

            string plainTextBody = $@"
                        Hello {appointment.PatientName},

                        Your appointment has been confirmed.

                        Date: {appointment.AppointmentDate:dd-MMM-yyyy hh:mm tt}
                        Doctor: {appointment.DoctorName}
                        Visit Type: {appointment.VisitType}
                        Diagnosis: {appointment.Diagnosis}
                        ";

                                    string htmlBody = $@"
                        <html>
                        <body style='font-family:Arial, sans-serif;'>
                          <h2 style='color:#2d89ef;'>Appointment Confirmation</h2>
                          <p>Hello <strong>{appointment.PatientName}</strong>,</p>
                          <p>Your appointment has been confirmed with the following details:</p>
                          <ul>
                            <li><b>Date:</b> {appointment.AppointmentDate:dd-MMM-yyyy hh:mm tt}</li>
                            <li><b>Doctor:</b> {appointment.DoctorName}</li>
                            <li><b>Visit Type:</b> {appointment.VisitType}</li>
                            <li><b>Diagnosis:</b> {appointment.Diagnosis}</li>
                          </ul>
                        </body>
                        </html>
                        ";

            var pdfBytes = GenerateAppointmentPdf(appointment);

            var smtpSettings = _configuration.GetSection("Smtp");
            using var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"]!))
            {
                Credentials = new NetworkCredential(smtpSettings["UserName"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["EnableSsl"]!)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpSettings["UserName"]!, "Medical Appointment System"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(appointment.PatientEmail);

            // attach pdf
            mail.Attachments.Add(new Attachment(new MemoryStream(pdfBytes), "Appointment.pdf", "application/pdf"));

            await client.SendMailAsync(mail);
        }



        public async Task SendEmailAsync(string recipientEmail, string subject, string plainTextBody, string htmlBody)
        {
            var smtpSettings = _configuration.GetSection("Smtp");

            string host = smtpSettings["Host"]!;
            int port = int.Parse(smtpSettings["Port"]!);
            bool enableSsl = bool.Parse(smtpSettings["EnableSsl"]!);
            string userName = smtpSettings["UserName"]!;
            string password = smtpSettings["Password"]!;

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage
            {
                From = new MailAddress(userName, "Hospital Appointment System"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mail.To.Add(recipientEmail);

            var altView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain");
            mail.AlternateViews.Add(altView);

            await client.SendMailAsync(mail);
        }

        public byte[] GenerateAppointmentPdf(AppointmentListDto appointment)
        {
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                doc.Add(new Paragraph("Appointment Confirmation", titleFont));
                doc.Add(new Paragraph($"Date: {appointment.AppointmentDate:dd-MMM-yyyy hh:mm tt}", normalFont));
                doc.Add(new Paragraph($"Patient: {appointment.PatientName}", normalFont));
                doc.Add(new Paragraph($"Doctor: {appointment.DoctorName}", normalFont));
                doc.Add(new Paragraph($"Visit Type: {appointment.VisitType}", normalFont));
                doc.Add(new Paragraph($"Diagnosis: {appointment.Diagnosis}", normalFont));
                //doc.Add(new Paragraph($"Notes: {appointment.Notes}", normalFont));
                doc.Add(new Paragraph(" "));

                // Prescription Table
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.AddCell("Medicine");
                table.AddCell("Dosage");
                table.AddCell("Start Date");
                table.AddCell("End Date");
                table.AddCell("Notes");

                foreach (var p in appointment.Prescriptions)
                {
                    table.AddCell(p.MedicineName);
                    table.AddCell(p.Dosage);
                    table.AddCell(p.StartDate.ToString("dd-MMM-yyyy"));
                    table.AddCell(p.EndDate.ToString("dd-MMM-yyyy"));
                    table.AddCell(p.Notes);
                }

                doc.Add(table);

                doc.Close();
                return ms.ToArray();
            }
        }


    }
}
