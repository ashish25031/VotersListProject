using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VotersListProject.Models;

namespace VotersListProject.Controllers
{
    public class DataShowController : Controller
    {
        public AddDB _db;

        public DataShowController()
        {
            _db = new AddDB();
        }

        // 🔐 Protect Entire Controller (Admin Only)
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
            {
                context.Result = RedirectToAction("Login", "Voters");
            }

            base.OnActionExecuting(context);
        }

        // ===========================
        // Admin Dashboard
        // ===========================
        public IActionResult ShowData(string searchString, int page = 1)
        {
            int pageSize = 5;

            var data = _db.votertable.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                data = data.Where(x =>
                    x.VoterName.Contains(searchString) ||
                    x.Email.Contains(searchString) ||
                    x.VoterId.Contains(searchString) ||
                    x.Aadhaar.Contains(searchString));
            }

            int totalRecords = data.Count();

            var pagedData = data
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = page;

            ViewBag.TotalUsers = _db.votertable.Count();
            ViewBag.TotalAdmins = _db.votertable.Count(x => x.Role == "Admin");
            ViewBag.TotalNormalUsers = _db.votertable.Count(x => x.Role == "User");

            ViewBag.PendingRequests = _db.UpdateRequests
                .Count(x => x.Status == "Pending");

            return View(pagedData);
        }

        // ===========================
        // Delete User
        // ===========================
        public IActionResult Delete(int id)
        {
            var user = _db.votertable.FirstOrDefault(x => x.Id == id);

            if (user != null)
            {
                _db.votertable.Remove(user);
                _db.SaveChanges();
            }

            return RedirectToAction("ShowData");
        }

        // ===========================
        // Edit User (GET)
        // ===========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _db.votertable.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return RedirectToAction("ShowData");

            return View(user);
        }

        // ===========================
        // Edit User (POST)
        // ===========================
        [HttpPost]
        public IActionResult Edit(VoterModel model)
        {
            var user = _db.votertable.FirstOrDefault(x => x.Id == model.Id);

            if (user != null)
            {
                user.VoterName = model.VoterName;
                user.Email = model.Email;
                user.VoterId = model.VoterId;
                user.DoB = model.DoB;
                user.Address = model.Address;
                user.Aadhaar = model.Aadhaar;

                _db.SaveChanges();
            }

            return RedirectToAction("ShowData");
        }

        // ===========================
        // Manage Update Requests
        // ===========================
        public IActionResult ManageRequests()
        {
            var requests = _db.UpdateRequests
                              .Where(x => x.Status == "Pending")
                              .ToList();

            return View(requests);
        }

        // ===========================
        // Approve Request
        // ===========================
        public IActionResult ApproveRequest(int id)
        {
            var request = _db.UpdateRequests.FirstOrDefault(x => x.Id == id);

            if (request == null)
                return RedirectToAction("ManageRequests");

            var user = _db.votertable.FirstOrDefault(x => x.Id == request.UserId);

            if (user != null)
            {
                switch (request.RequestedField)
                {
                    case "VoterName":
                        user.VoterName = request.NewValue;
                        break;
                    case "Email":
                        user.Email = request.NewValue;
                        break;
                    case "Address":
                        user.Address = request.NewValue;
                        break;
                    case "DoB":
                        user.DoB = DateTime.Parse(request.NewValue);
                        break;
                }
            }

            request.Status = "Approved";
            _db.SaveChanges();

            return RedirectToAction("ManageRequests");
        }

        // ===========================
        // Create New Admin (GET)
        // ===========================
        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        // ===========================
        // Create New Admin (POST)
        // ===========================
        [HttpPost]
        public IActionResult CreateAdmin(VoterModel obj)
        {
            
            {
                obj.Role = "Admin";   // 🔐 Force Admin role

                _db.votertable.Add(obj);
                _db.SaveChanges();

                return RedirectToAction("ShowData");
            }

            return View(obj);
        }

        // ===========================
        // Reject Request
        // ===========================
        public IActionResult RejectRequest(int id)
        {
            var request = _db.UpdateRequests.FirstOrDefault(x => x.Id == id);

            if (request != null)
            {
                request.Status = "Rejected";
                _db.SaveChanges();
            }

            return RedirectToAction("ManageRequests");
        }
    }
}