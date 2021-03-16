using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using PayEmFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayEmFinal.Controllers
{
    public class HomeController : Controller
    {
        double TotalRewards;
        dbPayEmEntities context = new dbPayEmEntities();
        public ActionResult Index()
        {
            if (Session["PhoneNumber"] != null)
            {
                Session["PhNoToHome"] = Session["PhoneNumber"].ToString();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        //To Generate a 10-digit reference number for every transaction done
        public string GenerateTransactionId()
        {
            string TransactionID = string.Empty;
            var random = new Random();
            for (int i = 0; i < 10; i++)
            {
                TransactionID = String.Concat(TransactionID, random.Next(10).ToString());
            }
            return TransactionID;
        }

        //To generate a 7-digit reference number for a vehicle which is registered under fastag
        public string GenerateFastTag()
        {
            string FastagId = string.Empty;
            var random = new Random();
            for (int i = 0; i < 7; i++)
            {
                FastagId = String.Concat(FastagId, random.Next(10).ToString());
            }
            return FastagId;
        }

        //Cashback for utilizing the services in PayEm
        public double CashBack(double Amount)
        {
            double CashBack, level;
            Random GenerateRandomNumber = new Random();
            level = GenerateRandomNumber.Next(0, 5);
            if (level == 1)
            {
                CashBack = GenerateRandomNumber.Next(10, 20);
                if (CashBack > Amount)
                {
                    CashBack = 0.1 * Amount;
                }
            }
            else if (level == 2)
            {
                CashBack = GenerateRandomNumber.Next(20, 25);
                if (CashBack > Amount)
                {
                    CashBack = 0.14 * Amount;
                }
            }
            else if (level == 3)
            {
                CashBack = GenerateRandomNumber.Next(25, 40);
                if (CashBack > Amount)
                {
                    CashBack = 0.19 * Amount;
                }
            }
            else if (level == 4)
            {
                CashBack = GenerateRandomNumber.Next(40, 50);
                if (CashBack > Amount)
                {
                    CashBack = 0.21 * Amount;
                }
            }
            else if (level == 5)
            {
                CashBack = GenerateRandomNumber.Next(50, 75);
                if (CashBack > Amount)
                {
                    CashBack = 0.25 * Amount;
                }
            }
            else
            {
                CashBack = 0;
            }
            return CashBack;
        }


        [HttpGet]
        public ActionResult ValidateUPIPINForContact()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult ValidateUPIPINForContact(UPI upi)
        {
            string CatchFromContactPhoneNumber = TempData.Peek("PhoneNumber").ToString();
            string CatchToContactPhoneNumber = TempData.Peek("ToContactValidation").ToString();
            double CatchToContactAmount = Convert.ToDouble(TempData.Peek("AmountToTransfer"));
            try
            {
                var ValidateTransaction = context.UPIs.Single(FetchDetails => FetchDetails.UPIPin == upi.UPIPin && FetchDetails.PhoneNumber == CatchFromContactPhoneNumber);
                if (ValidateTransaction != null)
                {
                    //Updating Receiver's Balance
                    var ReceivingUserDetails = context.CustomerBankDetailsConfidentials.Single(ReceiverPhoneNumber => ReceiverPhoneNumber.PhoneNumber == CatchToContactPhoneNumber);
                    double ReceivingUserBalance = Convert.ToDouble(ReceivingUserDetails.Balance);
                    double UpdateReceiverBalance = ReceivingUserBalance + CatchToContactAmount;
                    ReceivingUserDetails.Balance = UpdateReceiverBalance.ToString();
                    context.SaveChanges();
                    //Updating Sender's Balance
                    var SendingUserDetails = context.CustomerBankDetailsConfidentials.Single(SendingUserPhoneNumber => SendingUserPhoneNumber.PhoneNumber == CatchFromContactPhoneNumber);
                    double SendingUserBalance = Convert.ToDouble(SendingUserDetails.Balance);
                    double UpdateSenderBalance = Math.Abs(SendingUserBalance - CatchToContactAmount);
                    SendingUserDetails.Balance = UpdateSenderBalance.ToString();
                    context.SaveChanges();
                    //Adding Transaction History
                    var DetailsOfReceiver = context.Customers.Single(FetchDetails => FetchDetails.PhoneNumber == CatchToContactPhoneNumber);
                    string ReceiversName = DetailsOfReceiver.FirstName + " " + DetailsOfReceiver.LastName;
                    var DetailsOfSender = context.Customers.Single(FetchDetails => FetchDetails.PhoneNumber == CatchFromContactPhoneNumber);
                    string SendersName = DetailsOfSender.FirstName + " " + DetailsOfSender.LastName;
                    string CurrentTransactionId = GenerateTransactionId();
                    string FromCardNo = SendingUserDetails.CardNumber;
                    string phno = CatchFromContactPhoneNumber;
                    string ToCardNo = ReceivingUserDetails.CardNumber;
                    DateTime TransactionTime = DateTime.Now;
                    context.Transactions.Add(new Transaction(ReceiversName, SendersName, CurrentTransactionId, CatchToContactAmount.ToString(), FromCardNo, ToCardNo, TransactionTime, "credit"));
                    context.SaveChanges();
                    ViewBag.Message = "Transferred Successfully.";
                }
            }
            catch(Exception e)
            {
                ViewBag.Message = e;
                //ViewBag.Message = "Transaction failed.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ToContact()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public ActionResult CreateUPIPin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUPIPin(UPI upi)
        {
            string GetPhNo = TempData.Peek("PhoneNumber").ToString();
            var GetDetails = context.UPIs.Single(FetchDetails => FetchDetails.PhoneNumber == GetPhNo);
            GetDetails.UPIPin = upi.UPIPin.ToString();
            context.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult ToContact(DebitCardDetail debitCardDetails)
        {
            string SenderPhoneNumber = (string)TempData.Peek("PhoneNumber");
            try
            {
                var FetchedUserDetails = context.CustomerBankDetailsConfidentials.Single(FetchPhoneNumber => FetchPhoneNumber.PhoneNumber == SenderPhoneNumber);
                var UPIDetailsofSender = context.UPIs.Single(FetchDetails => FetchDetails.PhoneNumber == SenderPhoneNumber);
                double PresentBalance = Convert.ToDouble(FetchedUserDetails.Balance);
                if (UPIDetailsofSender.UPIPin == " ")
                {
                    return RedirectToAction("CreateUPIPin", "Home");
                }
                else
                {
                    if (FetchedUserDetails.PhoneNumber == debitCardDetails.PhoneNumber)
                    {
                        ViewBag.Message = "Cannot transfer to self.";
                    }
                    else if (debitCardDetails.Amount > PresentBalance)
                    {
                        ViewBag.Message = "Insufficient balance.";
                    }
                    else if (debitCardDetails.Amount > 20000)
                    {
                        ViewBag.Message = "Cannot transfer more than Rs.20000.";
                    }
                    else
                    {
                        TempData["ToContactValidation"] = debitCardDetails.PhoneNumber;
                        TempData["AmountToTransfer"] = debitCardDetails.Amount;
                        return RedirectToAction("ValidateUPIPINForContact");
                    }
                }
            }
            catch(Exception e)
            {
                ViewBag.Message = e;
                //ViewBag.Message = "Receiver is not on platform.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ValidateUPIPINForAccount()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult ValidateUPIPINForAccount(UPI upi)
        {
            string CatchFromPhNo = TempData.Peek("PhoneNumber").ToString();
            string CatchToAccNo = TempData.Peek("ReceiversAccNo").ToString();
            string CatchToIFSC = TempData.Peek("ReceiversIFSC").ToString();
            double CatchAmount = Convert.ToDouble(TempData.Peek("AmountToSend"));
            try
            {
                var ValidateTransaction = context.UPIs.Single(FetchDetails => FetchDetails.UPIPin == upi.UPIPin && FetchDetails.PhoneNumber == CatchFromPhNo);
                if (ValidateTransaction != null)
                {
                    //Updating Receiver's Balance
                    var ReceivingUserDetails = context.CustomerBankDetailsConfidentials.Single(ReceiversAccountNumberIFSC => ReceiversAccountNumberIFSC.AccountNumber == CatchToAccNo && ReceiversAccountNumberIFSC.IFSC == CatchToIFSC);
                    double ReceivingUserBalance = Convert.ToDouble(ReceivingUserDetails.Balance);
                    double UpdateReceiverBalance = Math.Abs(ReceivingUserBalance + CatchAmount);
                    ReceivingUserDetails.Balance = UpdateReceiverBalance.ToString();
                    //Updating Sender's Balance
                    var SendingUserDetails = context.CustomerBankDetailsConfidentials.Single(SendingUserPhoneNumber => SendingUserPhoneNumber.PhoneNumber == CatchFromPhNo);
                    double SendingUserBalance = Convert.ToDouble(SendingUserDetails.Balance);
                    double UpdateSenderBalance = Math.Abs(SendingUserBalance - CatchAmount);
                    SendingUserDetails.Balance = UpdateSenderBalance.ToString();
                    //Adding Transaction details
                    //var DetailsOfReceiver = context.BankDetails.Single(ReceiversAccountNumberIFSC => ReceiversAccountNumberIFSC.AccountNumber == CatchToAccNo && ReceiversAccountNumberIFSC.IFSC == CatchToIFSC);
                    string ReceiversPhoneNumber = ReceivingUserDetails.PhoneNumber;
                    var ReceiverDetails = context.Customers.Single(FetchDetails => FetchDetails.PhoneNumber == ReceiversPhoneNumber);
                    string ReceiversName = ReceiverDetails.FirstName + " " + ReceiverDetails.LastName;
                    var DetailsOfSender = context.Customers.Single(FetchDetails => FetchDetails.PhoneNumber == CatchFromPhNo);
                    string SendersName = DetailsOfSender.FirstName + " " + DetailsOfSender.LastName;
                    string CurrentTransactionId = GenerateTransactionId();
                    string FromCardNo = SendingUserDetails.CardNumber;
                    string ToCardNo = ReceivingUserDetails.CardNumber;
                    DateTime TransactionTime = DateTime.Now;
                    context.Transactions.Add(new Transaction(ReceiversName, SendersName, CurrentTransactionId, CatchAmount.ToString(), FromCardNo, ToCardNo, TransactionTime, "credit"));
                    context.SaveChanges();
                    ViewBag.Message = "Transferred Successfully.";
                }
            }
            catch(Exception e)
            {
                ViewBag.Message = e;
                //ViewBag.Message = "Transaction failed.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ToAccount()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult ToAccount(BankDetail bankDetails)
        {
            string SenderPhoneNumber = (string)TempData.Peek("PhoneNumber");
            var ReceiversDetails = context.CustomerBankDetailsConfidentials.Single(ReceiverConfirmation => ReceiverConfirmation.AccountNumber == bankDetails.AccountNumber && ReceiverConfirmation.IFSC == bankDetails.IFSC);
            double PresentBalance = Convert.ToDouble(ReceiversDetails.Balance);
            var SendersDetails = context.CustomerBankDetailsConfidentials.Single(SendingUserPhoneNumber => SendingUserPhoneNumber.PhoneNumber == SenderPhoneNumber);
            var UPIDetailsofSender = context.UPIs.Single(FetchDetails => FetchDetails.PhoneNumber == SenderPhoneNumber);
            if (UPIDetailsofSender.UPIPin == null)
            {
                return RedirectToAction("CreateUPIPin", "Home");
            }
            else if (SendersDetails.AccountNumber == bankDetails.AccountNumber)
            {
                ViewBag.Message = "Cannot transfer to self.";
            }
            else if (bankDetails.Amount > PresentBalance)
            {
                ViewBag.Message = "Insufficient Balance.";
            }
            else if (bankDetails.Amount > 20000)
            {
                ViewBag.Message = "Cannot transfer more than Rs.20000.";
            }
            else
            {
                TempData["ReceiversAccNo"] = ReceiversDetails.AccountNumber;
                TempData["ReceiversIFSC"] = ReceiversDetails.IFSC;
                TempData["AmountToSend"] = bankDetails.Amount;
                return RedirectToAction("ValidateUPIPINForAccount", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult PersonalAccounts()
        {
            if (Session["PhoneNumber"] != null)
            {
                string UsersPhoneNumber = TempData.Peek("PhoneNumber").ToString();
                var PersonalDebitCards = context.CustomerBankDetailsConfidentials.Where(FetchDetails => FetchDetails.PhoneNumber == UsersPhoneNumber).ToList();
                return View(PersonalDebitCards);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpGet]
        public ActionResult ByPass(string CardNo)
        {
            TempData["CardNo"] = CardNo;
            return RedirectToAction("ToSelf", "Home");
        }

        [HttpGet]
        public ActionResult ValidateUPIPINForSelf()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult ValidateUPIPINForSelf(UPI upi)
        {
            string CatchPhoneNumber = TempData.Peek("PhoneNumber").ToString();
            string CatchToDebitCard = TempData.Peek("CardNo").ToString();
            string CatchFromDebitCard = TempData.Peek("FromDebitCardNumber").ToString();
            string CatchFromCVV = TempData.Peek("FromCVV").ToString();
            double CatchTheAmountToTransfer = Convert.ToDouble(TempData.Peek("AmountToTransferForSelf"));
            try
            {
                var ValidateTransaction = context.UPIs.Single(FetchDetails => FetchDetails.UPIPin == upi.UPIPin && FetchDetails.PhoneNumber == CatchPhoneNumber);
                if (ValidateTransaction != null)
                {
                    //Updating To Debit Card Balance
                    var ToDebitCard = context.CustomerBankDetailsConfidentials.Single(FetchDetails => FetchDetails.CardNumber == CatchToDebitCard);
                    double BalanceToBeUpdatedInReceiver = Convert.ToDouble(ToDebitCard.Balance);
                    double UpdateBalanceInReceiver = Math.Abs(BalanceToBeUpdatedInReceiver + CatchTheAmountToTransfer);
                    ToDebitCard.Balance = UpdateBalanceInReceiver.ToString();
                    //Updating From Debit Card Balance
                    var FromDebitCardDetails = context.CustomerBankDetailsConfidentials.Single(FetchDetails => FetchDetails.CardNumber == CatchFromDebitCard && FetchDetails.CVV == CatchFromCVV);
                    var BalanceToBeUpdatedInSender = Convert.ToDouble(FromDebitCardDetails.Balance);
                    double UpdateBalanceInSender = Math.Abs(BalanceToBeUpdatedInSender - CatchTheAmountToTransfer);
                    FromDebitCardDetails.Balance = UpdateBalanceInSender.ToString();
                    //Adding Transaction History
                    var DetailsOfSelf = context.Customers.Single(FetchDetails => FetchDetails.PhoneNumber == CatchPhoneNumber);
                    string Name = DetailsOfSelf.FirstName + " " + DetailsOfSelf.LastName;
                    string CurrentTransactionId = GenerateTransactionId();
                    DateTime TransactionTime = DateTime.Now;
                    context.Transactions.Add(new Transaction(Name, Name, CurrentTransactionId, CatchTheAmountToTransfer.ToString(), CatchFromDebitCard, CatchToDebitCard, TransactionTime, "Transfer"));
                    context.SaveChanges();
                    return RedirectToAction("PersonalAccounts", "Home");
                }
            }
            catch
            {
                ViewBag.Message = "Transaction failed.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ToSelf()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult ToSelf(DebitCardDetail debitCardDetail)
        {
            string ToDebitCardNumber = (string)TempData.Peek("CardNo");
            try
            {
                string SenderPhoneNumber = TempData.Peek("PhoneNumber").ToString();
                var FromDebitCardDetails = context.CustomerBankDetailsConfidentials.Single(FetchDetails => FetchDetails.CardNumber == debitCardDetail.DebitCardNumber && FetchDetails.CVV == debitCardDetail.CVV);
                double PresentBalance = Convert.ToDouble(FromDebitCardDetails.Balance);
                string currenttime = DateTime.Now.ToString("yyyy/MM/dd");
                string expiredate = FromDebitCardDetails.ValidUpto.ToString("yyyy/MM/dd");
                DateTime current, expiry;
                var UPIDetailsofSender = context.UPIs.Single(FetchDetails => FetchDetails.PhoneNumber == SenderPhoneNumber);
                if (UPIDetailsofSender.UPIPin == null)
                {
                    return RedirectToAction("CreateUPIPin", "Home");
                }
                else if (debitCardDetail.DebitCardNumber == ToDebitCardNumber)
                {
                    ViewBag.Message = "Please choose different card.";
                }
                else if (debitCardDetail.Amount > PresentBalance)
                {
                    ViewBag.Message = "Insufficient Balance.";
                }
                else if (debitCardDetail.Amount > 20000)
                {
                    ViewBag.Message = "Cannot transfer more than Rs.20000.";
                }
                else if (DateTime.TryParse(currenttime, out current) && DateTime.TryParse(expiredate, out expiry))
                {
                    if (current.Date > expiry.Date)
                    {
                        ViewBag.Message = "Card expired.";
                    }
                    else if (current.Date <= expiry.Date)
                    {
                        TempData["FromDebitCardNumber"] = FromDebitCardDetails.CardNumber;
                        TempData["FromCVV"] = FromDebitCardDetails.CVV;
                        TempData["AmountToTransferForSelf"] = debitCardDetail.Amount;
                        return RedirectToAction("ValidateUPIPINForSelf", "Home");
                    }
                    else
                    {
                        ViewBag.Message = "Contact your bank.";
                    }
                }
                else
                {
                    ViewBag.Message = "Contact your bank.";
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Invalid Details.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ValidateUPIPINForMobileRecharge()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public double RewardsCount()
        {
            return TotalRewards;
        }

        [HttpPost]
        public ActionResult ValidateUPIPINForMobileRecharge(UPI upi)
        {
            string CatchFromPhoneNumber = TempData.Peek("PhoneNumber").ToString();
            string CatchRechargeePhoneNumber = TempData.Peek("RechargeePhoneNumber").ToString();
            double CatchAmountToRecharge = Convert.ToDouble(TempData.Peek("AmountToRecharge"));
            try
            {
                var ValidateDetails = context.UPIs.Single(FetchDetails => FetchDetails.UPIPin == upi.UPIPin && FetchDetails.PhoneNumber == CatchFromPhoneNumber);
                if (ValidateDetails != null)
                {
                    var RechargersDetails = context.CustomerBankDetailsConfidentials.Single(FetchDetails => FetchDetails.PhoneNumber == CatchFromPhoneNumber);
                    //Updating Mobile Recharge Balance
                    var RechargeeDetails = context.MobileCompanies.Single(FetchDetails => FetchDetails.PhoneNumber == CatchRechargeePhoneNumber);
                    double CurrentMobileBalance = Convert.ToDouble(RechargeeDetails.Balance);
                    double UpdatedBalance = CurrentMobileBalance + CatchAmountToRecharge;
                    RechargeeDetails.Balance = UpdatedBalance.ToString();
                    //Generating CashBank
                    double CashBackEarned = CashBack(CatchAmountToRecharge);
                    TotalRewards = TotalRewards + CashBackEarned;
                    var AddCashBackToAccount = context.CustomerBankDetailsConfidentials.Single(FetchDetails => FetchDetails.PhoneNumber == CatchFromPhoneNumber);
                    double CurrentAccountBalance = Convert.ToDouble(AddCashBackToAccount.Balance);
                    double UpdateAccountBalance = CurrentAccountBalance - CatchAmountToRecharge + CashBackEarned;
                    AddCashBackToAccount.Balance = UpdateAccountBalance.ToString();
                    ViewBag.CashBackEarned = CashBackEarned;
                    ViewBag.TransactionStatus = "Mobile recharged successfully.";
                    context.SaveChanges();
                }
                else
                {
                    ViewBag.TransactionStatus = "Incorrect UPI Pin";
                }
            }
            catch
            {
                ViewBag.TransactionStatus = "Transaction failed.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult MobileRecharge()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult MobileRecharge(MobileCompany mobileCompany)
        {
            try
            {
                var ToRecharge = context.MobileCompanies.Single(FetchDetails => FetchDetails.PhoneNumber == mobileCompany.PhoneNumber);
                if (ToRecharge != null)
                {
                    TempData["RechargeePhoneNumber"] = mobileCompany.PhoneNumber;
                    TempData["AmountToRecharge"] = mobileCompany.Amount;
                    return RedirectToAction("ValidateUPIPINForMobileRecharge", "Home");
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "There is no such phone number.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ValidateUPIPINForFasttag()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult ValidateUPIPINForFasttag(UPI upi)
        {
            string CatchRechargersPhoneNumber = TempData.Peek("PhoneNumber").ToString();
            string CatchVehicleNumber = TempData.Peek("VehicleNumber").ToString();
            double CatchAmountToRecharge = Convert.ToDouble(TempData.Peek("AmountToRechargeFasttag"));
            try
            {
                var ValdiateDetails = context.UPIs.Single(FetchDetails => FetchDetails.UPIPin == upi.UPIPin && FetchDetails.PhoneNumber == CatchRechargersPhoneNumber);
                if (ValdiateDetails != null)
                {
                    //Updating Wallet amount
                    var CatchVehiclesFasttagDetails = context.Fasttags.Single(FetchDetails => FetchDetails.CarNumber == CatchVehicleNumber);
                    double CurrentWalletAmount = Convert.ToDouble(CatchVehiclesFasttagDetails.WalletAmount);
                    double UpdateWalletAmount = CurrentWalletAmount + CatchAmountToRecharge;
                    CatchVehiclesFasttagDetails.WalletAmount = UpdateWalletAmount.ToString();
                    //Updating balance
                    var RechargersDetails = context.CustomerBankDetailsConfidentials.Single(FetchDetails => FetchDetails.PhoneNumber == CatchRechargersPhoneNumber);
                    double CashBackEarned = CashBack(CatchAmountToRecharge);
                    double CurrentAccountBalance = Convert.ToDouble(RechargersDetails.Balance);
                    double UpdatedAccountBalance = CurrentAccountBalance - CatchAmountToRecharge + CashBackEarned;
                    RechargersDetails.Balance = UpdatedAccountBalance.ToString();
                    ViewBag.CashBackEarned = CashBackEarned;
                    //Updating Transaction details
                    var DetailsOfSender = context.Customers.Single(FetchDetails => FetchDetails.PhoneNumber == CatchRechargersPhoneNumber);
                    string SendersName = DetailsOfSender.FirstName + " " + DetailsOfSender.LastName;
                    string CurrentTransactionId = GenerateTransactionId();
                    DateTime TransactionTime = DateTime.Now;
                    context.Transactions.Add(new Transaction("Fastag", SendersName, CurrentTransactionId, CatchAmountToRecharge.ToString(), RechargersDetails.CardNumber, null, TransactionTime, "Paid to"));
                    ViewBag.TransactionStatus = "Fast Tag wallet recharged successfully.";
                    context.SaveChanges();
                }
                else
                {
                    ViewBag.TransactionStatus = "Fast Tag wallet recharged unsuccessfully, please try again.";
                }
            }
            catch
            {
                ViewBag.Message = "Incorrect UPI Pin.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateFastag()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateFastag(Fasttag fastag)
        {
            string FastagId = GenerateFastTag();
            string VehicleNo = fastag.CarNumber;
            string PhNo = TempData.Peek("PhoneNumber").ToString();
            string RechargeAmt = fastag.WalletAmount;
            context.Fasttags.Add(new Fasttag(FastagId,VehicleNo,PhNo,RechargeAmt));
            context.SaveChanges();
            return View();
            ViewBag.Message = "Created Successfully.";
        }

        [HttpGet]
        public ActionResult FastTag()
        {
            if (Session["PhoneNumber"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        [HttpPost]
        public ActionResult FastTag(Fasttag fasttag)
        {
            try
            {
                var fasttagDetails = context.Fasttags.Single(FetchDetails => FetchDetails.CarNumber == fasttag.CarNumber);
                if (fasttagDetails != null)
                {
                    TempData["VehicleNumber"] = fasttagDetails.CarNumber;
                    TempData["AmountToRechargeFasttag"] = fasttag.WalletAmount;
                    return RedirectToAction("ValidateUPIPINForFasttag", "Home");
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Vehicle is not registered under Fastag.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}