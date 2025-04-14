using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MVCCoreDBF.Models;
using MvcDHProject.Models;

namespace MVCCoreDBF.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly MVCCoreDbContext _context;
        private readonly IWebHostEnvironment _webHost;

        public StudentController(MVCCoreDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Student
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Students.ToListAsync());
        }

        // GET: Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Sid == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Sid,Name,Class,Fees,Photo,Status")] Student student, IFormFile selectedFile)
        {
            if (ModelState.IsValid || ModelState.ErrorCount == 1 && ModelState["selectedFile"].ValidationState == ModelValidationState.Invalid)
            {
                if (selectedFile != null)
                {
                    student.Photo = SaveFileToServer(selectedFile);
                }
                student.Status = true;
                if (StudentExists(student.Sid))
                {
                    ModelState.AddModelError("", "Oh Ooh...! The Id you have entered already exists in our records, Please try another one...");
                    return View(student);
                }
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            TempData["Photo"] = student.Photo;
            return View(student);
        }

        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sid,Name,Class,Fees,Photo,Status")] Student student, IFormFile selectedFile)
        {
            if (id != student.Sid)
            {
                return NotFound();
            }

            if (ModelState.IsValid || ModelState.ErrorCount == 1 && ModelState["selectedFile"].ValidationState == ModelValidationState.Invalid)
            {
                if (selectedFile != null)
                {
                    student.Photo = SaveFileToServer(selectedFile);
                }
                else if (TempData["Photo"] != null)
                {
                    student.Photo = TempData["Photo"].ToString();
                }
                try
                {
                    student.Status = true;
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Sid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Sid == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                student.Status = false;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Sid == id);
        }

        // Method to save file locally to the server
        public string SaveFileToServer(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            // Generate a unique file name to avoid name conflicts
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            fileName = $"{Guid.NewGuid()}{extension}";  // Add a unique ID to the file name

            // Get the path where the file should be saved
            var uploadPath = Path.Combine(_webHost.WebRootPath, "images", fileName);

            // Save the file to the server
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"images/{fileName}";  // Return the relative path
        }
    }
}
