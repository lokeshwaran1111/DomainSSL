using Domainssl.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Domainssl
{
    public class RemindersJob : IJob
    {
        String strconnection = "Data Source=152.67.29.26,1565;initial catalog=augusta_intern;User ID=lokesh;Password=WvsEnQ4Z;";
        
        public Task Execute(IJobExecutionContext context)
        {
            SendMailForDomain();
            SendMailForSsl();
            return Task.CompletedTask;
        }
        public List<Dates> SendMailForSsl()
        {
            List<Dates> test = new List<Dates>();
            Dates row1 = new Dates();

            var TodayStartTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var TodayEndTime = (DateTime.UtcNow.AddDays(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            

            SqlDataAdapter da = new SqlDataAdapter("select Name,Expires_on from SSL WHERE Isdeleted='False' ", strconnection);
            DataTable dtSource = new DataTable();
            da.Fill(dtSource);
            DataRow[] dr = new DataRow[dtSource.Rows.Count];
            dtSource.Rows.CopyTo(dr, 0);
            foreach (DataRow row in dr)
            {

                row1 = new Dates()
                {
                    Domain_name = row.ItemArray[0].ToString(),
                    Exp = (Int64)row.ItemArray[1],

                };
                test.Add(row1);
            }
            foreach (var i in test )
            {
                var onemonth = i.Exp - 2629743;//to get epoch value before 1 month from exp date
                var b = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(onemonth).ToShortDateString();
                var fifteendays = i.Exp - 1314871;//to get epoch value before 15 days from exp date
                var c = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(fifteendays).ToShortDateString();
                var twoDays = i.Exp - 172800;//to get epoch value before 2 days from exp date
                var d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(twoDays).ToShortDateString();
                var Tommorow = i.Exp - 86400;//to get epoch value before 2 days from exp date
                var e = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(twoDays).ToShortDateString();
                if (onemonth >= TodayStartTime && onemonth <= TodayEndTime)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com",  "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The SSL certificate for the Domain <u><b>(" + i.Domain_name + ")</b></u> is going to expire in 1 month the expiry date is [" + Expdate + "]<br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (fifteendays >= TodayStartTime && fifteendays <= TodayEndTime)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com", "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The SSL certificate for the Domain <u><b>(" + i.Domain_name + ")</b></u> is going to expire in 15 days the expiry date is [" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (twoDays >= TodayStartTime && twoDays <= TodayEndTime)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com", "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The SSL certificate for the Domain <u><b>(" + i.Domain_name + ")</b></u> is going to expire in 2 days the expiry date is [" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (Tommorow >= TodayStartTime && Tommorow <= TodayEndTime)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com",  "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The SSL certificate for the Domain <u><b>(" + i.Domain_name + ")</b></u> is expiring Tommorow[" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }

                if (i.Exp >= TodayStartTime && i.Exp <= TodayEndTime)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com", "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The SSL certificate for the Domain <u><b>(" + i.Domain_name+ ")</b></u> is expiring today[" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
            }
            return test;
        }
        public List<Dates> SendMailForDomain()
        {
            List<Dates> test = new List<Dates>();
            Dates row1 = new Dates();

            var EpochToday = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var EpochTommorow = (DateTime.UtcNow.AddDays(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;


            SqlDataAdapter da = new SqlDataAdapter("select Domain_name,Expires_on from Domain WHERE Isdeleted='False' ", strconnection);
            DataTable dtSource = new DataTable();
            da.Fill(dtSource);
            DataRow[] dr = new DataRow[dtSource.Rows.Count];
            dtSource.Rows.CopyTo(dr, 0);
            foreach (DataRow row in dr)
            {

                row1 = new Dates()
                {
                    Domain_name = row.ItemArray[0].ToString(),
                    Exp = (Int64)row.ItemArray[1],

                };
                test.Add(row1);
            }
            foreach (var i in test)
            {
                var onemonth = i.Exp - 2629743;//to get epoch value before 1 month from exp date
                var b = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(onemonth).ToShortDateString();
                var fifteendays = i.Exp - 1314871;//to get epoch value before 15 days from exp date
                var c = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(fifteendays).ToShortDateString();
                var twoDays = i.Exp - 172800;//to get epoch value before 2 days from exp date
                var d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(twoDays).ToShortDateString();
                var Tommorow = i.Exp - 86400;//to get epoch value before 2 days from exp date
                var e = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(twoDays).ToShortDateString();
                if (onemonth >= EpochToday && onemonth <= EpochTommorow)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com",  "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The Domain <u><b>(" + i.Domain_name + ")</b></u> is going to expire in 1 month the expiry date is [" + Expdate + "]<br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (fifteendays >= EpochToday && fifteendays <= EpochTommorow)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com",  "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The Domain <u><b>(" + i.Domain_name + ")</b></u> is going to expire in 15 days the expiry date is [" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (twoDays >= EpochToday && twoDays <= EpochTommorow)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com",  "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The Domain <u><b>(" + i.Domain_name + ")</b></u> is going to expire in 2 days the expiry date is [" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (Tommorow >= EpochToday && Tommorow <= EpochTommorow)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com",  "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The Domain <u><b>(" + i.Domain_name + ")</b></u> is expiring tommorow [" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
                if (i.Exp >= EpochToday && i.Exp <= EpochTommorow)
                {
                    var Expdate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(i.Exp).ToShortDateString();
                    SendMail("lokeshwaran11112k@gmail.com", "lokeshwaranr@yopmail.com", "<br/><br/>Greetings from AH support!<br/><br/>For your kind attention<br/><br/> The Domain <u><b>(" + i.Domain_name + ")</b></u> is expiring today [" + Expdate + "] <br/><i>this mail is generated on " + DateTime.Now + "</i><br/><br/>Note if any Queries write to:<u>support@augustahitech.com</u><br/><br/><B>Thanks&Regards,</B><br/><br/>AH Support");
                }
            }
            return test;
        }



        public  void SendMail(string ToMailID, string ccMailID, string body)
        {

            MailMessage mail = new MailMessage();
            
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("lokeshwaran.raji@augustahitech.com", "AH Support ");
            mail.To.Add(ToMailID);
            mail.CC.Add(ccMailID);
            mail.IsBodyHtml = true;



            mail.Subject =  " Remainder for certificate expiry! ";

            mail.Body = "Hi,"+body+ " ";
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("lokeshwaran.raji@augustahitech.com", "lokesh@2000");
            SmtpServer.EnableSsl = true;
            //     SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Send(mail);
        }
    }
}
