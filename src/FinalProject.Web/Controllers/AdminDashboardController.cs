using FinalProject.Application.Interfaces;
using FinalProject.Web.ViewModels;
using FinalProject.Application.DTOs;
using FinalProject.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminDashboardController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private async Task<AdminDashboardViewModel> GetDashboardViewModelAsync()
        {
            var customers = await _adminService.GetCustomersAsync();
            var workers = await _adminService.GetWorkersAsync();
            var complaints = await _adminService.GetAllComplaintsAsync();
            var pendingWorkers = await _adminService.GetPendingWorkersAsync();

            return new AdminDashboardViewModel
            {
                TotalCustomers = customers.Count(),
                TotalWorkers = workers.Count(),
                PendingComplaintsCount = complaints.Count(c => c.Status == ComplaintStatus.Pending || c.Status == ComplaintStatus.UnderReview),
                PendingWorkersCount = pendingWorkers.Count(),
                Customers = customers.ToList(),
                Workers = workers.ToList(),
                Complaints = complaints.ToList(),
                PendingWorkers = pendingWorkers.ToList()
            };
        }

        public async Task<IActionResult> Index()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> IndexAr()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> Users()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> UsersAr()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int userId, string fullName, string email, string phoneNumber, int age, string? address, string redirectUrl)
        {
            var success = await _adminService.UpdateUserAsync(userId, fullName, email, phoneNumber, age, address);
            if (success)
            {
                TempData["SuccessMessage"] = "User updated successfully.";
                TempData["SuccessMessageAr"] = "تم تحديث بيانات المستخدم بنجاح.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update user.";
                TempData["ErrorMessageAr"] = "فشل في تحديث بيانات المستخدم.";
            }

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId, string redirectUrl)
        {
            var user = await _adminService.GetUserByIdAsync(userId);
            if (user != null)
            {
                bool success;
                if (user.IsActive)
                {
                    success = await _adminService.SuspendAccountAsync(userId);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "User suspended successfully.";
                        TempData["SuccessMessageAr"] = "تم تجميد حساب المستخدم بنجاح.";
                    }
                }
                else
                {
                    success = await _adminService.ActivateAccountAsync(userId);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "User activated successfully.";
                        TempData["SuccessMessageAr"] = "تم تنشيط حساب المستخدم بنجاح.";
                    }
                }
            }

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId, string redirectUrl)
        {
            var success = await _adminService.DeleteAccountAsync(userId);
            if (success)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
                TempData["SuccessMessageAr"] = "تم حذف المستخدم بنجاح.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
                TempData["ErrorMessageAr"] = "فشل في حذف المستخدم.";
            }

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Complaints()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> ComplaintsAr()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveComplaint(int complaintId, string response, string actionType, string redirectUrl)
        {
            bool success;
            if (actionType == "dismiss")
            {
                success = await _adminService.DismissComplaintAsync(complaintId, response);
                if (success)
                {
                    TempData["SuccessMessage"] = "Complaint dismissed successfully.";
                    TempData["SuccessMessageAr"] = "تم رفض الشكوى بنجاح.";
                }
            }
            else
            {
                success = await _adminService.ResolveComplaintAsync(complaintId, response);
                if (success)
                {
                    TempData["SuccessMessage"] = "Complaint resolved successfully.";
                    TempData["SuccessMessageAr"] = "تم حل الشكوى بنجاح.";
                }
            }

            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to process complaint.";
                TempData["ErrorMessageAr"] = "فشل في معالجة الشكوى.";
            }

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction(nameof(Complaints));
        }

        public async Task<IActionResult> WorkerCredentials()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> WorkerCredentialsAr()
        {
            var model = await GetDashboardViewModelAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveWorker(int workerId, string redirectUrl)
        {
            var success = await _adminService.ValidateAccountAsync(workerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Worker credentials approved successfully.";
                TempData["SuccessMessageAr"] = "تم قبول وثائق العامل بنجاح.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to approve worker.";
                TempData["ErrorMessageAr"] = "فشل في قبول العامل.";
            }

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction(nameof(WorkerCredentials));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectWorker(int workerId, string redirectUrl)
        {
            var success = await _adminService.RejectWorkerAsync(workerId);
            if (success)
            {
                TempData["SuccessMessage"] = "Worker credentials rejected and account deleted.";
                TempData["SuccessMessageAr"] = "تم رفض وثائق العامل وحذف حسابه.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to reject worker.";
                TempData["ErrorMessageAr"] = "فشل في رفض العامل.";
            }

            if (!string.IsNullOrEmpty(redirectUrl) && Url.IsLocalUrl(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            return RedirectToAction(nameof(WorkerCredentials));
        }
    }
}
