using System.Data;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ComplaintManagement.Helpers;
using ComplaintManagement.Models;
using static ComplaintManagement.Helpers.CryptoHelper;
using static ComplaintManagement.Helpers.HttpClientHelper;  
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PayPhone.Dtos;
using PayPhone.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Authorization;
using PayPhone.OpaqueTokens;
using Microsoft.AspNetCore.Http.HttpResults;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace ComplaintManagement.Controllers
{
    public class AppAuthController : BaseController
    {
        private static int saltLengthLimit = 32;
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public AppAuthController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        #region LaunchPad GET Method 
        [HttpGet]
        public ActionResult LaunchPad(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Pre Required Method For Login
        //GET: EnsureLoggedOut
        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            //if (Request.IsAuthenticated)
            if (HttpContext.User.Identity!.IsAuthenticated)
                Logout();
        }

        //POST: Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            try
            {
                // First we clean the authentication ticket like always
                //required NameSpace: using System.Web.Security;
                //FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                //required NameSpace: using System.Security.Principal;
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                HttpContext.Session.Clear();
                //System.Web.HttpContext.Current.Session.RemoveAll();

                // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
                // this clears the Request.IsAuthenticated flag since this triggers a new request
                return RedirectToLocal();
            }
            catch
            {
                throw;
            }
        }

        //GET: RedirectToLocal
        private ActionResult RedirectToLocal(string returnURL = "")
        {
            try
            {
                // If the return url starts with a slash "/" we assume it belongs to our site
                // so we will redirect to this "action"
                if (!string.IsNullOrWhiteSpace(returnURL) && Url.IsLocalUrl(returnURL))
                    return Redirect(returnURL);

                // If we cannot verify if the url is local to our host we redirect to a default location
                return RedirectToAction("LaunchPad", "Home");
            }
            catch
            {
                throw;
            }
        }

        //GET: SignInAsync   
        private void SignInRemember(string userName, bool isPersistent = false)
        {
            // Clear any lingering authencation data
            //FormsAuthentication.SignOut();

            // Write the authentication cookie
            //FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }

        #endregion

        #region Admin User SignUp GET Method 
        [HttpGet]
        public ActionResult AdminSignUp(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        
        #region Admin Login GET Method 
        [HttpGet]
        public ActionResult AdminLogin(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Admin Login POST Method
        [HttpPost]

        public IActionResult AdminLogin([FromBody] LoginModel entity)
        {
            try
            {
                string EnctryptionAlgorith = "rijndael";
                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Invalid model state", data = entity });

                string usertype;
                DataTable dt = dbhandler.ValidateUserLogin("ADMIN", entity.email!);
                if (dt.Rows.Count == 0)
                {
                    dt = dbhandler.ValidateUserLogin("CLIENT", entity.email!);
                    if (dt.Rows.Count == 0)
                    {
                        CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                        return Json(new { success = false, message = "Access Denied! Wrong Credentials" });
                    }
                    usertype = "CLIENT";
                }
                else
                {
                    usertype = "ADMIN";
                }

                if (Cryptographer.Encrypt(entity.password!).Replace("=", "") != dt.Rows[0]["password"].ToString())
                {
                    CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                    return Json(new { success = false, message = "Access Denied! Wrong Credentials" });
                }

                if (Convert.ToBoolean(dt.Rows[0]["locked"]))
                {
                    CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! Account is locked for user: " + entity.email);
                    return Json(new { success = false, message = "Access Denied! Account is locked, contact administrator" });
                }

                if (!Convert.ToBoolean(dt.Rows[0]["approved"]))
                {
                    CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! Account is not approved for user: " + entity.email);
                    return Json(new { success = false, message = "Access Denied! Account is not approved, contact administrator" });
                }

                var userInfo = new PortalUsersModel
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    email = dt.Rows[0]["email"].ToString(),
                    avatar = dt.Rows[0]["avatar"].ToString(),
                    name = dt.Rows[0]["name"].ToString(),
                    role_name = dt.Rows[0]["role_name"].ToString(),
                    user_name = dt.Columns.Contains("user_name") ? dt.Rows[0]["user_name"].ToString() : null,
                    role_id = Convert.ToInt32(dt.Rows[0]["role_id"]),
                    menu_layout = dt.Rows[0]["menu_layout"].ToString(),
                    locked = Convert.ToBoolean(dt.Rows[0]["locked"]),
                    google_authenticate = Convert.ToBoolean(dt.Rows[0]["google_authenticate"]),
                    sec_key = dt.Rows[0]["sec_key"].ToString(),
                    mobile = dt.Rows[0]["mobile"].ToString(),
                    created_on = Convert.ToDateTime(dt.Rows[0]["created_on"]),
                    updated_at = Convert.ToDateTime(dt.Rows[0]["updated_at"]),
                    password = dt.Rows[0]["password"].ToString(),
                    approved = Convert.ToBoolean(dt.Rows[0]["approved"]),
                    balance = dt.Rows[0].IsNull("balance") ? 0 : Convert.ToInt64(dt.Rows[0]["balance"])

                };

                HttpContext.Session.SetString("userid", userInfo.id.ToString());
                HttpContext.Session.SetString("email", userInfo.email!);
                HttpContext.Session.SetString("avatar", userInfo.avatar!);
                HttpContext.Session.SetString("name", userInfo.name!);
                HttpContext.Session.SetString("profileid", userInfo.role_id.ToString());
                HttpContext.Session.SetString("menulayout", userInfo.menu_layout!);
                HttpContext.Session.SetString("userType", usertype);

                SignInRemember(entity.email!, entity.isRemember);
                CaptureAuditTrail(entity.email!, "Successful Login", "Successfully logged in with user: " + HttpContext.Session.GetString("email"));

                var token = GenerateJwtToken(userInfo);

                var userDto = new PortalUserDto
                {

                    Name = userInfo.name,
                    UserName = userInfo.user_name,
                    Email = userInfo.email,
                    RoleName = userInfo.role_name,
                    Mobile = userInfo.mobile,
                    Password = userInfo.password,
                    Locked = userInfo.locked,
                    CreatedOn = userInfo.created_on,
                    UpdatedAt = userInfo.updated_at,
                    Approved = userInfo.approved,
                    Balance = userInfo.balance
                };

                return Json(new { success = true, message = "Login successful", token = token, user = userDto });
            }
            catch (Exception ex)
            {
                iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
                CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! System encountered issue while trying to authenticate user: " + entity.email);
                return Json(new { success = false, message = "System encountered issue while trying to authenticate" });
            }


        }

        private string GenerateJwtToken(PortalUsersModel user)
        {
            var secretKey = "your-super-secret-key-that-is-at-least-32-characters-long"; // Update with your secret key
            var key = Encoding.UTF8.GetBytes(secretKey);

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.id.ToString()),
                new Claim("role", user.role_name!)
            };

            var token = new JwtSecurityToken(
                issuer: "your-app",
                audience: "your-app",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool CaptureAuditTrail(string user, string action_type, string action_description)
        {
            AuditTrailModel audittrailmodel = new AuditTrailModel
            {
                user_name = user,
                action_type = action_type,
                action_description = action_description,
                page_accessed = String.Format("{0}://{1}{2}{3}", HttpContext.Request.Scheme, HttpContext.Request.Host, HttpContext.Request.Path, HttpContext.Request.QueryString), /*Request.Url.ToString(),*/
                client_ip_address = Request.HttpContext.Connection.RemoteIpAddress!.ToString(), /*Request.UserHostAddress,*/
                session_id = HttpContext.Session.GetString("userid") /*Session.SessionID*/
            };
            return dbhandler.AddAuditTrail(audittrailmodel);
        }

        #endregion



        #region updatebalance
        [Authorize]
        [HttpPost]
        public IActionResult DepositMoney([FromBody] DepositModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Json(new { success = false, message = "Invalid model state", data = model });

                bool isUpdated = dbhandler.UpdateUserBalance(model.email!, model.amount);
                if (!isUpdated)
                    return Json(new { success = false, message = "Failed to update balance" });

                CaptureAuditTrail(model.email!, "Deposit", $"Successfully deposited {model.amount} to user: {model.email}");

                // Log the deposit success for debugging
                iloggermanager.LogInfo($"Successfully deposited {model.amount} to user: {model.email}");

                // Retrieve the updated user details
                DataTable dt = dbhandler.ValidateUserLogin("CLIENT", model.email!);
                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "User not found after deposit" });
                }

                // Check and log the balance from the DataTable
                var balanceColumnExists = dt.Columns.Contains("balance");
                iloggermanager.LogInfo($"Balance column exists: {balanceColumnExists}");
                var retrievedBalance = balanceColumnExists && !dt.Rows[0].IsNull("balance") ? Convert.ToDecimal(dt.Rows[0]["balance"]) : 0;
                iloggermanager.LogInfo($"Retrieved balance: {retrievedBalance}");

                var userInfo = new PortalUsersModel
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    email = dt.Rows[0]["email"].ToString(),
                    avatar = dt.Rows[0]["avatar"].ToString(),
                    name = dt.Rows[0]["name"].ToString(),
                    role_name = dt.Rows[0]["role_name"].ToString(),
                    user_name = dt.Columns.Contains("user_name") ? dt.Rows[0]["user_name"].ToString() : null,
                    role_id = Convert.ToInt32(dt.Rows[0]["role_id"]),
                    menu_layout = dt.Rows[0]["menu_layout"].ToString(),
                    locked = Convert.ToBoolean(dt.Rows[0]["locked"]),
                    google_authenticate = Convert.ToBoolean(dt.Rows[0]["google_authenticate"]),
                    sec_key = dt.Rows[0]["sec_key"].ToString(),
                    mobile = dt.Rows[0]["mobile"].ToString(),
                    created_on = Convert.ToDateTime(dt.Rows[0]["created_on"]),
                    updated_at = Convert.ToDateTime(dt.Rows[0]["updated_at"]),
                    password = dt.Rows[0]["password"].ToString(),
                    approved = Convert.ToBoolean(dt.Rows[0]["approved"]),
                    balance = Convert.ToInt64(retrievedBalance)
                };

                var userDto = new PortalUserDto
                {
                    Name = userInfo.name,
                    UserName = userInfo.user_name,
                    Email = userInfo.email,
                    RoleName = userInfo.role_name,
                    Mobile = userInfo.mobile,
                    Password = userInfo.password,
                    Locked = userInfo.locked,
                    CreatedOn = userInfo.created_on,
                    UpdatedAt = userInfo.updated_at,
                    Approved = userInfo.approved,
                    Balance = userInfo.balance
                };

                return Json(new { success = true, message = "Deposit successful", user = userDto });
            }
            catch (Exception ex)
            {
                iloggermanager.LogInfo(ex.Message + " " + ex.StackTrace);
                CaptureAuditTrail(model.email!, "Deposit", "System encountered issue while trying to deposit money for user: " + model.email);
                return Json(new { success = false, message = "System encountered issue while trying to deposit money" });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult WithdrawMoney([FromBody] WithdrawModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid model state", data = model });

            try
            {
                // Call stored procedure for withdrawal
                string result;
                bool isWithdrawn = dbhandler.WithdrawUserBalance(model.email!, model.amount, out result);

                if (!isWithdrawn)
                    return Json(new { success = false, message = result });

                CaptureAuditTrail(model.email!, "Withdraw", $"Successfully withdrew {model.amount} from user: {model.email}");
                iloggermanager.LogInfo($"Successfully withdrew {model.amount} from user: {model.email}");

                // Retrieve updated user details
                DataTable dt = dbhandler.ValidateUserLogin("CLIENT", model.email!);
                if (dt.Rows.Count == 0)
                    return Json(new { success = false, message = "User not found after withdrawal" });

                // Extract updated balance
                var updatedBalance = dt.Columns.Contains("balance") && !dt.Rows[0].IsNull("balance") ? Convert.ToDecimal(dt.Rows[0]["balance"]) : 0;
                iloggermanager.LogInfo($"Updated balance: {updatedBalance}");

                var userInfo = new PortalUsersModel
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]),
                    email = dt.Rows[0]["email"].ToString(),
                    avatar = dt.Rows[0]["avatar"].ToString(),
                    name = dt.Rows[0]["name"].ToString(),
                    role_name = dt.Rows[0]["role_name"].ToString(),
                    user_name = dt.Columns.Contains("user_name") ? dt.Rows[0]["user_name"].ToString() : null,
                    role_id = Convert.ToInt32(dt.Rows[0]["role_id"]),
                    menu_layout = dt.Rows[0]["menu_layout"].ToString(),
                    locked = Convert.ToBoolean(dt.Rows[0]["locked"]),
                    google_authenticate = Convert.ToBoolean(dt.Rows[0]["google_authenticate"]),
                    sec_key = dt.Rows[0]["sec_key"].ToString(),
                    mobile = dt.Rows[0]["mobile"].ToString(),
                    created_on = Convert.ToDateTime(dt.Rows[0]["created_on"]),
                    updated_at = Convert.ToDateTime(dt.Rows[0]["updated_at"]),
                    password = dt.Rows[0]["password"].ToString(),
                    approved = Convert.ToBoolean(dt.Rows[0]["approved"]),
                    balance = Convert.ToInt64(updatedBalance)
                };

                var userDto = new PortalUserDto
                {
                    Name = userInfo.name,
                    UserName = userInfo.user_name,
                    Email = userInfo.email,
                    RoleName = userInfo.role_name,
                    Mobile = userInfo.mobile,
                    Password = userInfo.password,
                    Locked = userInfo.locked,
                    CreatedOn = userInfo.created_on,
                    UpdatedAt = userInfo.updated_at,
                    Approved = userInfo.approved,
                    Balance = userInfo.balance
                };

                return Json(new { success = true, message = "Withdrawal successful", user = userDto });
            }
            catch (MySqlException sqlEx)
            {
                iloggermanager.LogError($"SQL Error: {sqlEx.Message} {sqlEx.StackTrace}");
                CaptureAuditTrail(model.email!, "Withdraw", $"SQL Error while withdrawing money for user: {model.email}");
                return Json(new { success = false, message = "Database error occurred while withdrawing money" });
            }
            catch (Exception ex)
            {
                iloggermanager.LogError($"General Error: {ex.Message} {ex.StackTrace}");
                CaptureAuditTrail(model.email!, "Withdraw", $"General error while withdrawing money for user: {model.email}");
                return Json(new { success = false, message = "System encountered an issue while trying to withdraw money" });
            }
        }


        [Authorize]
        [HttpPost]

        public IActionResult ValidateUser([FromBody] TransferModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid model state", data = model });

            try
            {
                DataTable senderDt = dbhandler.ValidateUserLogin("CLIENT", model.SenderEmail!);
                if (senderDt.Rows.Count == 0)
                    return Json(new { success = false, message = "Sender not found" });

                var senderBalance = senderDt.Columns.Contains("balance") && !senderDt.Rows[0].IsNull("balance") ? Convert.ToDecimal(senderDt.Rows[0]["balance"]) : 0;
                var senderName = model.SenderName ?? (senderDt.Columns.Contains("name") && !senderDt.Rows[0].IsNull("name") ? senderDt.Rows[0]["name"].ToString() : "Sender");

                if (senderBalance < model.Amount)
                    return Json(new { success = false, message = "Insufficient balance" });

                DataTable receiverDt = dbhandler.ValidateUserLogin("CLIENT", model.ReceiverEmail!);
                if (receiverDt.Rows.Count == 0)
                    return Json(new { success = false, message = "Receiver not found" });

                var receiverName = model.ReceiverName ?? (receiverDt.Columns.Contains("name") && !receiverDt.Rows[0].IsNull("name") ? receiverDt.Rows[0]["name"].ToString() : "Receiver");

                using (var transaction = dbhandler.BeginTransaction())
                {
                    try
                    {
                        string withdrawResult;
                        bool isWithdrawn = dbhandler.WithdrawUserBalance(model.SenderEmail!, model.Amount, out withdrawResult, transaction);
                        if (!isWithdrawn)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = withdrawResult });
                        }

                        bool isDeposited = dbhandler.UpdateUserBalance(model.ReceiverEmail!, model.Amount, transaction);
                        if (!isDeposited)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "Failed to update receiver's balance" });
                        }

                        transaction.Commit();

                        CaptureAuditTrail(model.SenderEmail!, "Transfer", $"Successfully transferred {model.Amount} to user: {model.ReceiverEmail}");
                        CaptureAuditTrail(model.ReceiverEmail!, "Transfer", $"Successfully received {model.Amount} from user: {model.SenderEmail}");
                        iloggermanager.LogInfo($"Successfully transferred {model.Amount} from {model.SenderEmail} to {model.ReceiverEmail}");

                        DataTable updatedSenderBalanceDt = dbhandler.ValidateUserLogin("CLIENT", model.SenderEmail!);
                        DataTable updatedReceiverBalanceDt = dbhandler.ValidateUserLogin("CLIENT", model.ReceiverEmail!);

                        var updatedSenderDetails = updatedSenderBalanceDt.Rows[0];
                        var updatedReceiverDetails = updatedReceiverBalanceDt.Rows[0];

                        var senderInfo = new
                        {
                            name = updatedSenderDetails["name"],
                            email = updatedSenderDetails["email"],
                            user_name = updatedSenderDetails["user_name"],
                            mobile = updatedSenderDetails["mobile"],
                            role_name = updatedSenderDetails["role_name"],
                            password = updatedSenderDetails["password"],
                            balance = updatedSenderDetails["balance"],
                            created_on = updatedSenderDetails["created_on"],
                            updated_at = updatedSenderDetails["updated_at"]


                        };

                        var receiverInfo = new
                        {
                            name = updatedReceiverDetails["name"],
                            email = updatedReceiverDetails["email"],
                            user_name = updatedReceiverDetails["user_name"],
                            mobile = updatedReceiverDetails["mobile"],
                            role_name = updatedReceiverDetails["role_name"],
                            password = updatedReceiverDetails["password"],
                            balance = updatedReceiverDetails["balance"],
                            created_on = updatedReceiverDetails["created_on"],
                            updated_at = updatedReceiverDetails["updated_at"]
                        };

                        return Json(new
                        {
                            success = true,
                            senderMessage = $"You have successfully transferred {model.Amount} to {receiverName}.",
                            sender = senderInfo,
                            receiverMessage = $"{model.Amount} has been successfully deposited into your account by {senderName}.",
                            receiver = receiverInfo

                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        iloggermanager.LogError($"Error during transfer: {ex.Message} {ex.StackTrace}");
                        CaptureAuditTrail(model.SenderEmail!, "Transfer", $"Error during transfer to user: {model.ReceiverEmail}");
                        return Json(new { success = false, message = "System encountered an issue during transfer" });
                    }
                }
            }
            catch (Exception ex)
            {
                iloggermanager.LogError($"General Error: {ex.Message} {ex.StackTrace}");
                CaptureAuditTrail(model.SenderEmail!, "Transfer", $"General error during transfer to user: {model.ReceiverEmail}");
                return Json(new { success = false, message = "System encountered an issue while trying to transfer money" });
            }
        }


        [Authorize]
        [HttpPost]

        public IActionResult TransferMoney([FromBody] TransferModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid model state", data = model });

            try
            {
                DataTable senderDt = dbhandler.ValidateUserLogin("CLIENT", model.SenderEmail!);
                if (senderDt.Rows.Count == 0)
                    return Json(new { success = false, message = "Sender not found" });

                var senderBalance = senderDt.Columns.Contains("balance") && !senderDt.Rows[0].IsNull("balance") ? Convert.ToDecimal(senderDt.Rows[0]["balance"]) : 0;
                var senderName = model.SenderName ?? (senderDt.Columns.Contains("name") && !senderDt.Rows[0].IsNull("name") ? senderDt.Rows[0]["name"].ToString() : "Sender");

                if (senderBalance < model.Amount)
                    return Json(new { success = false, message = "Insufficient balance" });

                DataTable receiverDt = dbhandler.ValidateUserLogin("CLIENT", model.ReceiverEmail!);
                if (receiverDt.Rows.Count == 0)
                    return Json(new { success = false, message = "Receiver not found" });

                var receiverName = model.ReceiverName ?? (receiverDt.Columns.Contains("name") && !receiverDt.Rows[0].IsNull("name") ? receiverDt.Rows[0]["name"].ToString() : "Receiver");

                using (var transaction = dbhandler.BeginTransaction())
                {
                    try
                    {
                        string withdrawResult;
                        bool isWithdrawn = dbhandler.WithdrawUserBalance(model.SenderEmail!, model.Amount, out withdrawResult, transaction);
                        if (!isWithdrawn)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = withdrawResult });
                        }

                        bool isDeposited = dbhandler.UpdateUserBalance(model.ReceiverEmail!, model.Amount, transaction);
                        if (!isDeposited)
                        {
                            transaction.Rollback();
                            return Json(new { success = false, message = "Failed to update receiver's balance" });
                        }

                        transaction.Commit();

                        CaptureAuditTrail(model.SenderEmail!, "Transfer", $"Successfully transferred {model.Amount} to user: {model.ReceiverEmail}");
                        CaptureAuditTrail(model.ReceiverEmail!, "Transfer", $"Successfully received {model.Amount} from user: {model.SenderEmail}");
                        iloggermanager.LogInfo($"Successfully transferred {model.Amount} from {model.SenderEmail} to {model.ReceiverEmail}");

                        DataTable updatedSenderBalanceDt = dbhandler.ValidateUserLogin("CLIENT", model.SenderEmail!);
                        DataTable updatedReceiverBalanceDt = dbhandler.ValidateUserLogin("CLIENT", model.ReceiverEmail!);

                        var updatedSenderDetails = updatedSenderBalanceDt.Rows[0];
                        var updatedReceiverDetails = updatedReceiverBalanceDt.Rows[0];

                        var senderInfo = new
                        {
                            name = updatedSenderDetails["name"],
                            email = updatedSenderDetails["email"],
                            user_name = updatedSenderDetails["user_name"],
                            mobile = updatedSenderDetails["mobile"],
                            role_name = updatedSenderDetails["role_name"],
                            password = updatedSenderDetails["password"],
                            balance = updatedSenderDetails["balance"],
                            created_on = updatedSenderDetails["created_on"],
                            updated_at = updatedSenderDetails["updated_at"]


                        };

                        var receiverInfo = new
                        {
                            name = updatedReceiverDetails["name"],
                            email = updatedReceiverDetails["email"],
                            user_name = updatedReceiverDetails["user_name"],
                            mobile = updatedReceiverDetails["mobile"],
                            role_name = updatedReceiverDetails["role_name"],
                            password = updatedReceiverDetails["password"],
                            balance = updatedReceiverDetails["balance"],
                            created_on = updatedReceiverDetails["created_on"],
                            updated_at = updatedReceiverDetails["updated_at"]
                        };

                        return Json(new
                        {
                            success = true,
                            senderMessage = $"You have successfully transferred {model.Amount} to {receiverName}.",
                            sender = senderInfo,
                            receiverMessage = $"{model.Amount} has been successfully deposited into your account by {senderName}.",
                            receiver = receiverInfo

                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        iloggermanager.LogError($"Error during transfer: {ex.Message} {ex.StackTrace}");
                        CaptureAuditTrail(model.SenderEmail!, "Transfer", $"Error during transfer to user: {model.ReceiverEmail}");
                        return Json(new { success = false, message = "System encountered an issue during transfer" });
                    }
                }
            }
            catch (Exception ex)
            {
                iloggermanager.LogError($"General Error: {ex.Message} {ex.StackTrace}");
                CaptureAuditTrail(model.SenderEmail!, "Transfer", $"General error during transfer to user: {model.ReceiverEmail}");
                return Json(new { success = false, message = "System encountered an issue while trying to transfer money" });
            }
        }
        #endregion

        #region Admin Reset Password GET Method 
        [HttpGet]
        public ActionResult ResetPassword(string returnURL)
        {
            var userinfo = new LoginModel();

            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();

                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;

                return View(userinfo);
            }
            catch (Exception ex)
            {
                iloggermanager.LogError(ex.StackTrace!);
                throw;
            }
        }
        #endregion

        #region Admin Reset Password POST Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(LoginModel entity)
        {
            try
            {
                string response = "";
                HttpHandler httphandler = new HttpHandler();
                string EnctryptionAlgorith = "rijndael";
                string external_ref_num = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

                //if (!ModelState.IsValid)
                //    return View(entity);

                //bool isLogin = false; //CompareHashValue(entity.password, entity.email, OldHASHValue, SALT);
                DataTable dt = new DataTable();
                dt = dbhandler.ValidateUserLogin("CLIENT", entity.email!);
                if (dt.Rows.Count > 0)
                {
                    RandomKeyGeneratorManagement myrandomkeymanager = new RandomKeyGeneratorManagement();

                    PortalUsersModel existingrecord = dbhandler.GetPortalUsers().Find(mymodel => mymodel.id == Convert.ToInt32(dt.Rows[0]["id"]))!;
                    if (existingrecord != null)
                    {
                        PortalUsersModel mymodel = new PortalUsersModel
                        {
                            id = existingrecord.id,
                            role_id = existingrecord.role_id,
                            mobile = existingrecord.mobile,
                            email = existingrecord.email,
                            name = existingrecord.name
                        };

                        mymodel.password = Cryptographer.Encrypt(myrandomkeymanager.GenerateRandomAlphaNumericString(7)).Replace("=", "");
                        mymodel.locked = existingrecord.locked;
                        mymodel.google_authenticate = existingrecord.google_authenticate;
                        mymodel.sec_key = existingrecord.sec_key;
                        mymodel.avatar = existingrecord.avatar;

                        if (dbhandler.UpdatePortalUser(mymodel))
                        {
                            //make call to SCAPI
                            //make token request
                            string serverapi = dbhandler.GetRecords("parameters", "SCAPI_API_URL").Rows[0]["item_value"].ToString()!;
                            string api_user = dbhandler.GetRecords("parameters", "SCAPI_API_USER").Rows[0]["item_value"].ToString()!;
                            string api_password = dbhandler.GetRecords("parameters", "SCAPI_API_PASSWORD").Rows[0]["item_value"].ToString()!;
                            string mail_cred_subject = dbhandler.GetRecords("parameters", "MAIL_CREDENTIALS_SUBJECT").Rows[0]["item_value"].ToString()!;
                            string portal_url = dbhandler.GetRecords("parameters", "BACKOFFICE_PORTAL_URL").Rows[0]["item_value"].ToString()!;

                            JObject token_data = new JObject
                            {
                                // message_validation
                                {
                                    "message_validation",
                                    new JObject
                                    {
                                        { "api_user", api_user },
                                        { "api_password", api_password }
                                    }
                                },
                                // message_route
                                {
                                    "message_route",
                                    new JObject
                                    {
                                        { "interface", "TOKEN" }
                                    }
                                }
                            };

                            string token_resp_data = httphandler.HttpClientPost(serverapi, token_data);

                            JObject token_resp_data_json = JObject.Parse(token_resp_data);

                            JObject email_data = new JObject
                            {
                                // message_validation
                                {
                                    "message_validation",
                                    new JObject
                                    {
                                        { "api_user", api_user },
                                        { "api_password", api_password },
                                        { "token", token_resp_data_json["error_desc"]!["token"]!.ToString() },
                                    }
                                },
                                // message_route
                                {
                                    "message_route",
                                    new JObject
                                    {
                                        { "interface", "EMAIL" },
                                        { "request_type", "backoffice_credentials" },
                                        { "external_ref_number", external_ref_num }
                                    }
                                },
                                // message_body
                                {
                                    "message_body",
                                    new JObject
                                    {
                                        { "subject", mail_cred_subject },
                                        { "customer", existingrecord.name },
                                        { "email_address", existingrecord.email },
                                        { "user_password", Cryptographer.Decrypt(mymodel.password + "==") },
                                        { "portal_url", portal_url },
                                        { "attachment", "" }
                                    }
                                }
                            };

                            iloggermanager.LogInfo(email_data.ToString());

                            string email_resp_data = httphandler.HttpClientPost(serverapi, email_data);

                            JObject email_resp_data_json = JObject.Parse(email_resp_data);

                            iloggermanager.LogInfo(email_resp_data_json.ToString());

                            if (email_resp_data_json["error_code"]!.ToString() == "00" && email_resp_data_json["error_desc"]![0]!["response_code"]!.ToString() == "00")
                            {
                                response = "Success-" + external_ref_num.Substring(2, 12);
                                CaptureAuditTrail(entity.email!, "Successful password reset", "Successfully sent password reset to email: " + entity.email);
                                Success("New password has been sent to your email address", true);
                            }
                            else if (email_resp_data_json["error_code"]!.ToString() == "00" && email_resp_data_json["error_desc"]![0]!["response_code"]!.ToString() != "00")
                            {
                                response = email_resp_data_json["error_desc"]![0]!["response_desc"]!.ToString();
                                CaptureAuditTrail(entity.email!, "Error on password reset", response + " on password reset to email: " + entity.email);
                                Danger(response, true);
                            }
                            else
                            {
                                response = "Operation could not be completed, kindly contact system admin";
                                CaptureAuditTrail(entity.email!, "Error on password reset", response + " on password reset to email: " + entity.email);
                                Danger(response, true);
                            }
                            //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                            ModelState.Clear();
                        }
                        else
                        {
                            Danger("Could not reset password, kindly contact system admin");
                        }
                    }
                    else
                    {
                        //Login Fail
                        //TempData["ErrorMSG"] = "Access Denied! Wrong Credentials";
                        CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! Wrong credentials for user: " + entity.email);
                        Danger("Access Denied! Wrong Credentials", true);
                    }
                }
                else
                {
                    Danger("Unknown user");
                }
                return View(entity);
            }
            catch (Exception ex)
            {
                iloggermanager.LogError(ex.StackTrace!);
                //TempData["ErrorMSG"] = "System encountered issue while trying to authenticate";
                CaptureAuditTrail(entity.email!, "Invalid Login", "Access denied! System encountered issue while trying to authenticate user: " + entity.email);
                Danger("System encountered issue while trying to authenticate", true);
                return View(entity);
            }

        }
        #endregion 

        #region --> Generate SALT Key

        private static byte[] Get_SALT()
        {
            return Get_SALT(saltLengthLimit);
        }

        private static byte[] Get_SALT(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];

            //Require NameSpace: using System.Security.Cryptography;
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }

        #endregion

        #region --> Generate HASH Using SHA512
        public static string Get_HASH_SHA512(string password, string username, byte[] salt)
        {
            try
            {
                //required NameSpace: using System.Text;
                //Plain Text in Byte
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(password + username);

                //Plain Text + SALT Key in Byte
                byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + salt.Length];

                for (int i = 0; i < plainTextBytes.Length; i++)
                {
                    plainTextWithSaltBytes[i] = plainTextBytes[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    plainTextWithSaltBytes[plainTextBytes.Length + i] = salt[i];
                }

                HashAlgorithm hash = new SHA512Managed();
                byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);
                byte[] hashWithSaltBytes = new byte[hashBytes.Length + salt.Length];

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashWithSaltBytes[i] = hashBytes[i];
                }

                for (int i = 0; i < salt.Length; i++)
                {
                    hashWithSaltBytes[hashBytes.Length + i] = salt[i];
                }

                return Convert.ToBase64String(hashWithSaltBytes);
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion

        #region --> Comapare HASH Value
        public static bool CompareHashValue(string password, string username, string OldHASHValue, byte[] SALT)
        {
            try
            {
                string expectedHashString = Get_HASH_SHA512(password, username, SALT);

                return (OldHASHValue == expectedHashString);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        //Refer(SALT): http://codereview.stackexchange.com/questions/93614/salt-generation-in-c  
        //Refer(SHA512): http://zurb.com/forrst/posts/C_SHA512_Encryption_Hashing-UaL
        //Refer 1:  https://www.mssqltips.com/sqlservertip/4037/storing-passwords-in-a-secure-way-in-a-sql-server-database/
        //Refer 2: https://crackstation.net/hashing-security.htm

    }
}