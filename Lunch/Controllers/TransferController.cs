using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Lunch.Entity;
using Lunch.Helper;
using Lunch.Models;
using Lunch.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lunch.Controllers
{
    public class TransferController : UserBaseController
    {
        private EncryptProcessor encryptor = new EncryptProcessor();
        public INotificationService NotificationService { get; set; }

        public TransferController()
        {
            NotificationService = new SlackClient();
        }

        // GET: Balance
        public ActionResult Index()
        {
            var user = Session["User"] as Customer;
            if (user != null)
            {
                var resentbalance = LunchRepository.RecentBalance(user.Name);
                var transferViewModel = new TransferViewModel(user.CustomerId);
                ViewBag.balance = resentbalance;

                return View(transferViewModel);
            }
            return RedirectToAction("Index", "ClientOrder", new { LGstatusname = user.Name, LGstatusCustomerId = user.CustomerId });
        }

        public ActionResult PartialTransferList()
        {
            var user = Session["User"] as Customer;
            if (user != null)
            {
                var resentbalance = LunchRepository.RecentBalance(user.Name);
                var transferViewModel = new TransferViewModel(user.CustomerId);
                ViewBag.balance = resentbalance;
                return PartialView(transferViewModel);
            }
            return RedirectToAction("Index", "ClientOrder", new { LGstatusname = user.Name, LGstatusCustomerId = user.CustomerId });
        }

        [HttpPost]
        public ActionResult TransferCancel(int orderId, string transPw)
        {
            var user = Session["User"] as Customer;
            var msg = "information wrong, no data be updated.";
            var error = 1;
            var list = new List<JObject>();
            if (user != null)
            {
                var orderInfo = LunchRepository.GetTransferList().FirstOrDefault(x => x.OrderId == orderId && x.CustomerId == user.CustomerId);
                if (orderInfo != null)
                {
                    error = LunchRepository.UpdateTransferSettle(orderId);
                    LogHelper.Log(string.Format("TransferCancel:: UpdateTransferSettle( {0} )->error:{1}", orderId, error));
                    error = LunchRepository.UpdateTransferInfo(orderId, (int)EnumTransferStatus.Cancel);
                    LogHelper.Log(string.Format("TransferCancel:: UpdateTransferInfo({0} ,{1} )->error:{2}", orderId, (int)EnumTransferStatus.Cancel, error));
                    msg = error == 0
                                    ? "Request has Canceled"
                                    : "Cancel failed. No data be updated";
                    list.Add(new JObject(
                                     new JProperty("error", error),
                                     new JProperty("msg", msg)
                                    ));

                }
                else
                {
                    msg = "request invalid";
                }
            }

            SetInfoMsg(list, error, msg);
            return RedirectToAction("Index");

        }

        [HttpPost]
        public string TransferConfirm(int orderId, string status, string pw)
        {
            var user = Session["User"] as Customer;
            pw = encryptor.getEncryptedStr(pw);
            var msg = "information wrong, no data be updated.";
            var error = 1;
            var list = new List<JObject>();

            int transferStatus;
            if (status == "approve")
            {
                transferStatus = (int)EnumTransferStatus.Approve;
            }
            else if (status == "reject")
            {
                transferStatus = (int)EnumTransferStatus.Reject;
            }
            else
            {
                return "error status";
            }

            if (user != null)
            {
                var custInfo = LunchRepository.GetLoginInformation(user.Name, pw);
                if (custInfo == null && transferStatus != (int)EnumTransferStatus.Reject)
                {
                    list.Add(new JObject(
                   new JProperty("error", error),
                   new JProperty("msg", "Password wrong, no data be updated.")
                   ));
                    TempData["InfoMsg"] = JsonConvert.SerializeObject(list, Formatting.Indented);
                    return "error wrong password";
                }
                var orderInfo = LunchRepository.GetTransferList().FirstOrDefault(x => x.OrderId == orderId && x.RestaurantId == user.CustomerId);
                if (orderInfo != null)
                {
                    error = transferStatus != (int)EnumTransferStatus.Reject ? TransferMoney(orderInfo.Cost, orderInfo.Meal, orderInfo.RestaurantId, orderInfo.CustomerId) : 0;
                    if (error == 0)
                    {
                        LunchRepository.UpdateTransferSettle(orderId);
                        LunchRepository.UpdateTransferInfo(orderId, transferStatus);
                    }
                    var userName = LunchRepository.GetNameById(orderInfo.CustomerId);
                    msg = CheckSuccess(error, transferStatus, msg, orderInfo, userName);
                    list.Add(new JObject(
                                     new JProperty("error", error),
                                     new JProperty("msg", msg)
                                    ));
                }
                else
                {
                    msg = "request invalid";
                }
            }

            SetInfoMsg(list, error, msg);
            return "success";

        }

        private void SetInfoMsg(List<JObject> list, int error, string msg)
        {
            var transferResult = JsonConvert.SerializeObject(list, Formatting.Indented);
            list.Add(new JObject(
                new JProperty("error", error),
                new JProperty("msg", msg)
                ));
            TempData["InfoMsg"] = transferResult != "" && transferResult != "[]"
                ? transferResult
                : JsonConvert.SerializeObject(list, Formatting.Indented);
        }

        private string CheckSuccess(int error, int transferStatus, string msg, OrderInfo orderInfo, string userName)
        {
            if (error == 0)
            {
                switch (transferStatus)
                {
                    case (int)EnumTransferStatus.Approve:
                        msg = "transferred " + orderInfo.Cost + " to " + userName + " successful.";
                        sendTransferMessage(string.Format("order id:{0}, {1} transferred {2} to {3}.",
                            orderInfo.OrderId, orderInfo.RestaurantName, orderInfo.Cost, userName));
                        break;
                    case (int)EnumTransferStatus.Reject:
                        msg = "Has rejected to transfer " + orderInfo.Cost + " to " + userName + ".";
                        break;
                }
            }else if (error == 3)
            {
                msg= "transferred " + orderInfo.Cost + " to " + userName + " failed. your balance below your transfer amount.";
            }
            else
            {
                switch (transferStatus)
                {
                    case (int)EnumTransferStatus.Approve:
                        msg = "transferred " + orderInfo.Cost + " to " + userName + " failed. no data be updated.";
                        break;
                    case (int)EnumTransferStatus.Reject:
                        msg = "Rejected failed. no data be updated.";
                        break;
                }
            }
            return msg;
        }

        [HttpPost]
        public ActionResult TransferEvent(string usernamelist, int cost, string transPw, string memo, string transferType)
        {
            var user = Session["User"] as Customer;
            var error = 1;
            var msg = "information wrong, no data be updated.";
            var transferResult = string.Empty;
            var list = new List<JObject>();
            transPw = encryptor.getEncryptedStr(transPw);
            if (user != null && cost > 0)
            {
                var custInfo = LunchRepository.GetLoginInformation(user.Name, transPw);
                if (custInfo != null || transferType == "request")
                {
                    var custList = JsonConvert.DeserializeObject<List<TransCust>>(usernamelist);
                    switch (transferType)
                    {
                        case "submit":
                            foreach (var cust in custList)
                            {
                                error = TransferMoney(cost, memo, user.CustomerId, cust.id);
                                if (error == 0)
                                {
                                    msg = "transferred " + cost + " to " + cust.name + " successful.";
                                    //sendTransferMessage(string.Format("{0} transferred {1} to {2}.", user.Name, cost, cust.name));
                                }else if (error == 2)
                                {
                                    msg = "your balance below 1000. Please deposit.";
                                }else if (error == 3)
                                {
                                    msg = "your balance below your transfer amount. Please enter money again.";
                                }
                                else { msg = "transferred " + cost + " to " + cust.name + " failed. No data be updated"; }
                                list.Add(new JObject(
                                     new JProperty("error", error),
                                     new JProperty("msg", msg)
                                    ));
                            }
                            transferResult = JsonConvert.SerializeObject(list, Formatting.Indented);
                            break;
                        case "request":
                            foreach (var item in custList)
                            {
                                error = TransferRequest(cost, user, item, memo);
                                if (error == 0)
                                {
                                    msg = "Has request to " + item.name + ".";
                                }
                                else
                                {
                                    msg = "Request to " + item.name + " failed!";
                                }
                                list.Add(new JObject(
                                     new JProperty("error", 0),
                                     new JProperty("msg", msg)
                                    ));
                            }
                            transferResult = JsonConvert.SerializeObject(list, Formatting.Indented);
                            break;
                    }
                }
                else
                {
                    msg = "password wrong, no data be updated";
                }
            }
            list.Add(new JObject(
                new JProperty("error", error),
                new JProperty("msg", msg)
                ));
            TempData["InfoMsg"] = transferResult != "" && transferResult != "[]"
                ? transferResult
                : JsonConvert.SerializeObject(list, Formatting.Indented);
            return RedirectToAction("Index");
        }

        private void sendTransferMessage(string msg)
        {
            var alertBody = new SlackPayload { text = msg };
            NotificationService.SendAlert(alertBody, SlackChannel.TransferMessage);
        }

        private int TransferMoney(int cost, string memo, int fromCustId, int ToCustId)
        {
            var error = SendMoneyToUser(cost, fromCustId, ToCustId, memo);
            LogHelper.Log(string.Format("TransferMoney({0} ,{1} ,{2} ,{3} )->error:{4}", cost, fromCustId, ToCustId, memo, error));
            if (error == 0) error = UpdateUserBalance(cost, fromCustId, ToCustId, memo);
            LogHelper.Log(string.Format("UpdateUserBalance({0} ,{1} ,{2} ,{3} )->error:{4}", cost, fromCustId, ToCustId, memo, error));
            return error;
        }

        private int TransferRequest(int cost, Customer user, TransCust item, string memo)
        {
            var meal = LunchRepository.GetNameById(item.id) + " Transfer to " + LunchRepository.GetNameById(user.CustomerId);
            if (memo.Any())
            {
                meal += ": " + memo;
            }
            var error = LunchRepository.InsertTransferRequest(user.CustomerId, item.id, meal, cost);
            LogHelper.Log(string.Format("InsertTransferRequest({0} ,{1} ,{2} ,{3} )->error:{4}", user.CustomerId, item.id, meal, cost, error));
            return error;
        }

        private int SendMoneyToUser(int cost, int fromCustId, int toCustId, string memo)
        {
            var userName = LunchRepository.GetNameById(fromCustId);
            var balance = LunchRepository.RecentBalance(userName);
            if (balance < 0)
            {
                return 2;
            }
            if (balance < 0 || balance < cost)
            {
                return 3;
            }
            var meal = "To " + LunchRepository.GetNameById(toCustId);
            if (memo.Any())
            {
                meal = (memo.IndexOf("Transfer to ", StringComparison.Ordinal) != -1) ? memo : meal + ": " + memo;
            }
            balance -= cost;
            var error = LunchRepository.UpdateSummary(fromCustId, meal, -cost, balance, userName);
            LogHelper.Log(string.Format("SendMoneyToUser::UpdateSummary({0} ,{1} ,{2} ,{3} ,{4} )->error:{5}", fromCustId, meal, -cost, balance, userName, error));
            return error;
        }

        private int UpdateUserBalance(int cost, int fromCustId, int toCustId, string memo)
        {
            var userName = LunchRepository.GetNameById(toCustId);
            var balance = LunchRepository.RecentBalance(userName);
            var meal = "From " + LunchRepository.GetNameById(fromCustId);
            if (memo.Any())
            {
                meal = (memo.IndexOf("Transfer to ", StringComparison.Ordinal) != -1) ? memo : meal + ": " + memo;
            }
            balance += cost;
            var error = LunchRepository.UpdateSummary(toCustId, meal, cost, balance, userName);
            LogHelper.Log(string.Format("UpdateUserBalance::UpdateSummary({0} ,{1} ,{2} ,{3} ,{4} )->error:{5}", toCustId, meal, cost, balance, userName, error));
            return error;
        }
    }
}