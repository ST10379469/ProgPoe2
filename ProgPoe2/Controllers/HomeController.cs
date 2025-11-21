<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Files.Shares;
using ProgPoe2.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
=======
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProgPoe2.Models;
>>>>>>> ab259e5b3cf525d4f6aff2a46ee9ae8bf67240c1

namespace ProgPoe2.Controllers
{
    public class HomeController : Controller
    {
<<<<<<< HEAD
        private readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=cldvst10043611;AccountKey=A1DGT56hmblnV8FX6ISHlF9HU9v/o8z6thDkC+Sr/NCqJxdyk1A8xZykTdH16+LGacXmi+4vBKHa+ASt2pl1HA==;EndpointSuffix=core.windows.net";
        private readonly string shareName = "uploadedfiles";

        private User GetCurrentUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId.HasValue ?
                UserRepository.Users.FirstOrDefault(u => u.UserID == userId.Value) : null;
        }

        private bool IsAuthorized(string requiredRole)
        {
            var user = GetCurrentUser();
            return user != null && user.Role == requiredRole;
=======
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
>>>>>>> ab259e5b3cf525d4f6aff2a46ee9ae8bf67240c1
        }

        public IActionResult Index()
        {
<<<<<<< HEAD
            var user = GetCurrentUser();
            if (user != null)
            {
                return RedirectToRolePage(user.Role);
            }
            return View();
        }

        private IActionResult RedirectToRolePage(string role)
        {
            return role switch
            {
                "HR" => RedirectToAction("HRDashboard"),
                "Lecturer" => RedirectToAction("SubmitClaim"),
                "Coordinator" or "Manager" => RedirectToAction("ApprovalDashboard"),
                _ => RedirectToAction("Index")
            };
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            if (!IsAuthorized("Lecturer"))
            {
                return RedirectToAction("Index");
            }

            var user = GetCurrentUser();
            var claim = new Claim
            {
                UserId = user.UserID,
                LecturerName = $"{user.Name} {user.Surname}",
                HourlyRate = user.HourlyRate
            };

            return View(claim);
        }

        [HttpGet]
        public IActionResult ViewClaims()
        {
            if (!IsAuthorized("Lecturer"))
            {
                return RedirectToAction("Index");
            }

            var user = GetCurrentUser();
            var userClaims = ClaimRepository.Claims.Where(c => c.UserId == user.UserID).ToList();
            return View(userClaims);
        }

        [HttpGet]
        public IActionResult ApprovalDashboard()
        {
            if (!IsAuthorized("Coordinator") && !IsAuthorized("Manager"))
            {
                return RedirectToAction("Index");
            }

            var pendingClaims = ClaimRepository.Claims.Where(c => c.Status == "Pending").ToList();
            return View(pendingClaims);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile file)
        {
            if (!IsAuthorized("Lecturer"))
            {
                return RedirectToAction("Index");
            }

            var user = GetCurrentUser();

            if (claim.HoursWorked > 180)
            {
                ModelState.AddModelError("HoursWorked", "Hours worked cannot exceed 180 hours per month.");
                return View(claim);
            }

            claim.Id = ClaimRepository.Claims.Count + 1;
            claim.UserId = user.UserID;
            claim.LecturerName = $"{user.Name} {user.Surname}";
            claim.HourlyRate = user.HourlyRate;
            claim.SubmissionDate = DateTime.Now;

            if (file != null && file.Length > 0)
            {
                if (file.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("file", "File size must be less than 5MB.");
                    return View(claim);
                }

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("file", "Only PDF, Word, and Excel documents are allowed.");
                    return View(claim);
                }

                claim.SupportingDocumentName = await UploadFileToAzure(file);
            }

            ClaimRepository.Claims.Add(claim);
            return RedirectToAction("ViewClaims");
        }

        [HttpGet]
        public IActionResult HRDashboard()
        {
            if (!IsAuthorized("HR"))
            {
                return RedirectToAction("Index");
            }
            
            var users = UserRepository.Users.Where(u => u.Role != "HR").ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            if (!IsAuthorized("HR"))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (!IsAuthorized("HR"))
            {
                return RedirectToAction("Index");
            }

            user.UserID = UserRepository.Users.Count + 1;
            user.IsActive = true;
            UserRepository.Users.Add(user);
            return RedirectToAction("HRDashboard");
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            if (!IsAuthorized("HR"))
            {
                return RedirectToAction("Index");
            }

            var user = UserRepository.Users.FirstOrDefault(u => u.UserID == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User updatedUser)
        {
            if (!IsAuthorized("HR"))
            {
                return RedirectToAction("Index");
            }

            var user = UserRepository.Users.FirstOrDefault(u => u.UserID == updatedUser.UserID);
            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Surname = updatedUser.Surname;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;
                user.HourlyRate = updatedUser.HourlyRate;
                user.IsActive = updatedUser.IsActive;
            }
            return RedirectToAction("HRDashboard");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = UserRepository.Users.FirstOrDefault(u =>
                u.Email == email && u.Password == password && u.IsActive);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserID);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", $"{user.Name} {user.Surname}");

                return RedirectToRolePage(user.Role);
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password.";
                return View("Index");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        private async Task<string> UploadFileToAzure(IFormFile file)
        {
            try
            {
                var shareClient = new ShareClient(connectionString, shareName);
                var directoryClient = shareClient.GetRootDirectoryClient();
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var fileClient = directoryClient.GetFileClient(fileName);

                await using (var stream = file.OpenReadStream())
                {
                    await fileClient.CreateAsync(stream.Length);
                    await fileClient.UploadAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                return null;
            }
        }

        [HttpPost]
        public IActionResult UpdateClaimStatus(int claimId, string status)
        {
            if (!IsAuthorized("Coordinator") && !IsAuthorized("Manager"))
            {
                return RedirectToAction("Index");
            }

            var claim = ClaimRepository.Claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = status;
            }
            return RedirectToAction(nameof(ApprovalDashboard));
        }

        public IActionResult EditClaim(int id)
        {
            var claim = ClaimRepository.Claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        [HttpPost]
        public async Task<IActionResult> EditClaim(int id, Claim updatedClaim, IFormFile file)
        {
            var claim = ClaimRepository.Claims.FirstOrDefault(c => c.Id == id);
            if (claim == null)
            {
                return NotFound();
            }

            claim.LecturerName = updatedClaim.LecturerName;
            claim.HoursWorked = updatedClaim.HoursWorked;
            claim.HourlyRate = updatedClaim.HourlyRate;
            claim.Notes = updatedClaim.Notes;
            claim.Status = updatedClaim.Status;

            if (file != null && file.Length > 0)
            {
                if (file.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("file", "File size must be less than 5MB.");
                    return View(claim);
                }

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("file", "Only PDF, Word, and Excel documents are allowed.");
                    return View(claim);
                }

                claim.SupportingDocumentName = await UploadFileToAzure(file);
            }

            return RedirectToAction("ViewClaims");
        }

        [HttpPost]
        public IActionResult RemoveClaim(int id)
        {
            var claim = ClaimRepository.Claims.FirstOrDefault(c => c.Id == id);
            if (claim != null)
            {
                ClaimRepository.Claims.Remove(claim);
            }
            return RedirectToAction("ViewClaims");
        }

        public IActionResult GenerateHRReport()
        {
            if (!IsAuthorized("HR"))
            {
                return RedirectToAction("Index");
            }

            var allClaims = ClaimRepository.Claims;
            var report = new HRReport
            {
                Claims = allClaims,
                TotalClaims = allClaims.Count,
                ApprovedClaimsCount = allClaims.Count(c => c.Status == "Approved"),
                PendingClaimsCount = allClaims.Count(c => c.Status == "Pending"),
                RejectedClaimsCount = allClaims.Count(c => c.Status == "Rejected"),
                TotalAmount = allClaims
                    .Where(c => c.Status == "Approved")
                    .Sum(c => (decimal)c.HoursWorked * c.HourlyRate)
            };

            return View(report);
        }

=======
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

>>>>>>> ab259e5b3cf525d4f6aff2a46ee9ae8bf67240c1
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> ab259e5b3cf525d4f6aff2a46ee9ae8bf67240c1
