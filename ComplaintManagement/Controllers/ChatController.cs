    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using ComplaintManagement.Helpers;
    using ComplaintManagement.Models;
    using Microsoft.AspNetCore.Authorization;
    using PayPhone.Models;

    namespace PayPhone.Controllers
    {
        public class ChatController : Controller
        {
            private readonly IWebHostEnvironment ihostingenvironment;
            private readonly ILoggerManager iloggermanager;
            private readonly DBHandler dbhandler;

            public ChatController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
            {
                iloggermanager = logger;
                ihostingenvironment = environment;
                dbhandler = mydbhandler;
            }

            public class chat_record
            {
                public int id { get; set; }
                public string? chatName { get; set; }
                public string? sender { get; set; }
                public string? receiver { get; set; }
                public string? latestMessage { get; set; }
                public DateTime created_on { get; set; }
                public DateTime updated_at { get; set; }
            }

            public class Processingresponse
            {
                public string? error_desc { get; set; }
                public chat_record? data { get; set; }
            }


        [Authorize]
        [HttpPost]
        public ActionResult CreateChat([FromBody] chat_record record)
        {
            var response = new Processingresponse();

            if (HttpContext.Session.GetString("name") == null)
                return RedirectToAction("AdminLogin", "AppAuth");

            try
            {
                // Check for existing record by ID
                var existingrecord = dbhandler.GetChat().FirstOrDefault(mymodel => mymodel.id == record.id);
                if (existingrecord != null)
                {
                    // Update existing record
                    existingrecord.chatName = record.chatName;
                    existingrecord.sender = record.sender;
                    existingrecord.receiver = record.receiver;
                    existingrecord.latestMessage = record.latestMessage;
                    // existingrecord.created_on = DateTime.UtcNow; // Do not update creation time
                    existingrecord.updated_at = DateTime.UtcNow;

                    if (dbhandler.UpdateChat(existingrecord))
                    {
                        CaptureAuditTrail("Updated chat", $"Updated chat: {existingrecord.chatName}");
                        ModelState.Clear();
                        response.error_desc = "Updated chat successfully";
                        var updatedRecord = dbhandler.GetChat().FirstOrDefault(mymodel => mymodel.id == record.id);
                        response.data = MapToChatRecord(updatedRecord!); // Map to chat_record
                    }
                    else
                    {
                        response.error_desc = "Could not update chat, kindly contact system admin";
                    }
                }
                else
                {
                    // Create new record
                    var mymodel = new ChatModel
                    {
                        chatName = record.chatName,
                        sender = record.sender,
                        receiver = record.receiver,
                        latestMessage = record.latestMessage,
                        created_on = DateTime.UtcNow,
                        updated_at = DateTime.UtcNow
                    };

                    if (dbhandler.AddChat(mymodel))
                    {
                        CaptureAuditTrail("Created chat", $"Created chat: {mymodel.chatName}");
                        ModelState.Clear();
                        response.error_desc = "Chat successfully created";
                        var newRecord = dbhandler.GetChat().FirstOrDefault(mymodel => mymodel.id == mymodel.id);
                        response.data = MapToChatRecord(newRecord!); // Map to chat_record
                    }
                    else
                    {
                        response.error_desc = "Could not create chat, kindly contact system admin";
                    }
                }
            }

            catch (Exception ex)
            {
                iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
                response.error_desc = "Could not process chat, kindly contact system admin";
            }

            return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
        }

        public static chat_record MapToChatRecord(ChatModel model)
        {
            return new chat_record
            {
                id = model.id,
                chatName = model.chatName,
                sender = model.sender,
                receiver = model.receiver,
                latestMessage = model.latestMessage,
                created_on = model.created_on,
                updated_at = model.updated_at
            };
        }


        //[Authorize]
        //[HttpPost]
        //public ActionResult CreateChat([FromBody] chat_record record)
        //{
        //    var response = new Processingresponse();

        //    if (HttpContext.Session.GetString("name") == null)
        //        return RedirectToAction("AdminLogin", "AppAuth");

        //    try
        //    {
        //        // Check for existing record by ID
        //        var existingrecord = dbhandler.GetChat().FirstOrDefault(mymodel => mymodel.id == record.id);
        //        if (existingrecord != null)
        //        {
        //            // Update existing record
        //            existingrecord.chatName = record.chatName;
        //            existingrecord.sender = record.sender;
        //            existingrecord.receiver = record.receiver;
        //            existingrecord.latestMessage = record.latestMessage;
        //            //existingrecord.created_on = DateTime.UtcNow;
        //            existingrecord.updated_at = DateTime.UtcNow;

        //            if (dbhandler.UpdateChat(existingrecord))
        //            {
        //                CaptureAuditTrail("Updated chat", $"Updated chat: {existingrecord.chatName}");
        //                ModelState.Clear();
        //                response.error_desc = "Updated chat successfully";

        //            response.data = dbhandler.GetChat().FirstOrDefault(mymodel => mymodel.id == record.id); // Fetch updated record
        //        }
        //            else
        //            {
        //                response.error_desc = "Could not update chat, kindly contact system admin";
        //            }
        //        }
        //        else
        //        {
        //            // Create new record
        //            var mymodel = new ChatModel
        //            {
        //                chatName = record.chatName,
        //                sender = record.sender,
        //                receiver = record.receiver,
        //                latestMessage = record.latestMessage,
        //                created_on = DateTime.UtcNow,
        //                updated_at = DateTime.UtcNow
        //            };

        //            if (dbhandler.AddChat(mymodel))
        //            {
        //                CaptureAuditTrail("Created chat", $"Created chat: {mymodel.chatName}");
        //                ModelState.Clear();
        //                response.error_desc = "Chat successfully created";
        //            //response.data = record; // Return newly created record
        //            response.data = dbhandler.GetChat().FirstOrDefault(mymodel => mymodel.id == mymodel.id); // Fetch new record
        //        }
        //            else
        //            {
        //                response.error_desc = "Could not create chat, kindly contact system admin";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
        //        response.error_desc = "Could not process chat, kindly contact system admin";
        //    }

        //    return Content(JsonConvert.SerializeObject(response, Formatting.Indented), "application/json");
        //}

        public bool CaptureAuditTrail(string action_type, string action_description)
            {
                var audittrailmodel = new AuditTrailModel
                {
                    user_name = HttpContext.Session.GetString("email")!,
                    action_type = action_type,
                    action_description = action_description,
                    page_accessed = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}",
                    session_id = HttpContext.Session.Id
                };

                return dbhandler.AddAuditTrail(audittrailmodel);
            }
        }
    }
