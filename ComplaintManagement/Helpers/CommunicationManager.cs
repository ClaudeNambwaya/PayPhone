using ComplaintManagement.Models;
using System.Data;
using System.Net.Mail;
using System.Text;

/// <summary>
/// Summary description for CommunicationManager
/// </summary>
namespace ComplaintManagement.Helpers
{

    public class CommunicationManagement
    {

        private DBHandler dbhandler;

        public CommunicationManagement(DBHandler mydbhandler)
        {
            dbhandler = mydbhandler;
        }

        public bool SendEmailNotification(string[] emailaddress, string emailsubject, StringBuilder emailbody, string[] attachmentfile, AlternateView htmlview = null, bool ishtml = false)
        {
            DataTable datatable = dbhandler.GetRecords("mailconfigs");
            string mailhost = datatable.Rows[0]["mailhostip"].ToString()!;
            string mailport = datatable.Rows[0]["mailhostport"].ToString()!;
            string mailfrom = datatable.Rows[0]["mailsender"].ToString()!;
            string mailfrompassword = datatable.Rows[0]["mailsenderpass"].ToString()!;
            string myurl = datatable.Rows[0]["mail_creds_url"].ToString()!;

            MailMessage mailmessage = new MailMessage
            {
                From = new MailAddress(mailfrom),
                Subject = emailsubject,
                Body = emailbody.ToString()
            };

            bool messagesent = false;

            try
            {
                if (emailaddress.Length > 0)
                {
                    //populate list of recipients 
                    for (Int32 i = 0; i <= emailaddress.Length - 1; i++)
                    {
                        mailmessage.To.Add(new MailAddress(emailaddress[i]));
                    }

                    //populate list of attachments
                    if (attachmentfile.Length > 0)
                    {
                        for (Int32 j = 0; j <= attachmentfile.Length - 1; j++)
                        {
                            if (attachmentfile[j] != "")
                            {
                                Attachment attachment = new Attachment(attachmentfile[j]);
                                mailmessage.Attachments.Add(attachment);
                            }
                        }
                    }

                    if (htmlview != null)
                        mailmessage.AlternateViews.Add(htmlview);

                    if (ishtml)
                        mailmessage.IsBodyHtml = true;

                    //send message already
                    SmtpClient client = new SmtpClient(mailhost)
                    {
                        //oSmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //oSmtp.UseDefaultCredentials = false;
                        Credentials = new System.Net.NetworkCredential(mailfrom, mailfrompassword),
                        Port = Convert.ToInt32(mailport),
                        EnableSsl = true
                    };

                    client.Send(mailmessage);
                    messagesent = true;
                }

                return messagesent;
            }
            catch (Exception)
            {
                messagesent = false;
                return messagesent;
            }
        }

        public bool ScheduleEmailNotification(string[] emailaddress, string emailsubject, StringBuilder emailbody, string[] attachmentfile, string dispatchdate, string dispatchtime, Boolean scheduled, Boolean ishtml = false)
        {
            bool MessageSentOK = false;
            string filepath = "";
            try
            {
                if (emailaddress.Length > 0)
                {
                    //populate list of recipients 
                    for (Int32 i = 0; i <= emailaddress.Length - 1; i++)
                    {
                        //populate list of attachments
                        if (attachmentfile.Length > 0)
                        {
                            for (Int32 j = 0; j <= attachmentfile.Length - 1; j++)
                            {
                                if (attachmentfile[j] != "")
                                {
                                    if (j == 0) { filepath = attachmentfile[j]; continue; }
                                    filepath = filepath + ";" + attachmentfile[j];
                                }
                            }
                        }

                        //dbhandler.add_email(emailaddress[i], emailsubject, emailbody, filepath, dispatchdate, dispatchtime, scheduled,ishtml);
                    }
                }
                return MessageSentOK;
            }
            catch (Exception ex)
            {
                MessageSentOK = false;
                return MessageSentOK;
            }

        }

        public bool ScheduleSMSNotification(string[] phonerecipients, string smsbody, string dispatchdate, string dispatchtime, Boolean scheduled)
        {
            string minismsbody = "";
            bool MessageSentOK = false;
            //string countrycode = dbhandler.DBAction("Select ItemValue from parameters where ItemKey = 'COUNTRY_CODE'", DataManagement.DBActionType.Scalar).ToString();
            try
            {
                for (Int32 i = 0; i <= phonerecipients.Length - 1; i++)
                {
                    //string phonenumber = countrycode + phonerecipients[i].Trim().Substring(phonerecipients[i].Trim().Length - 9);

                    //add message to table awaiting sending
                    if (smsbody.Trim().Length > 160)
                    {
                        Int16 startindex = 0;
                        Int16 endindex = 160;
                        int stringLength = smsbody.Trim().Length;

                        while (stringLength >= endindex)
                        {
                            minismsbody = smsbody.Substring(startindex, endindex);

                            //dbhandler.add_sms(phonenumber, minismsbody, dispatchdate, dispatchtime, scheduled);

                            //reset variables for the next read
                            startindex = Convert.ToInt16(endindex);
                            if (stringLength > startindex)
                            {
                                if ((stringLength - 160) > 160)
                                {
                                    endindex = 160;
                                }
                                else
                                {
                                    endindex = Convert.ToInt16(stringLength - endindex);
                                }

                            }
                            stringLength = stringLength - startindex;

                        }

                    }
                    else
                    {
                        //dbhandler.add_sms(phonenumber, smsbody, dispatchdate, dispatchtime, scheduled);
                    }
                }
                return MessageSentOK;
            }
            catch (Exception ex)
            {
                MessageSentOK = false;
                return MessageSentOK;
            }
        }
    }
}