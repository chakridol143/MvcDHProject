using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCCoreDBF.Models;
using MvcDHProject.Models;

namespace MVCCoreDBF.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly MVCCoreDbContext _context;

        private readonly IWebHostEnvironment _webHost;

        public StudentController(MVCCoreDbContext context,IWebHostEnvironment webHost)
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

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Sid,Name,Class,Fees,Photo,Status")] Student student,IFormFile selectedFile)
        {
            if (ModelState.IsValid || ModelState.ErrorCount == 1 && ModelState["selectedFile"].ValidationState == ModelValidationState.Invalid) {

                if (selectedFile != null)
                {
                    student.Photo = await UploadToImgBB(selectedFile);
                }
                student.Status = true;
                if (StudentExists(student.Sid))
                {
                    ModelState.AddModelError("", "Oh Ooh...! The Id you have entered is already exists in our records, Please try another one...");
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sid,Name,Class,Fees,Photo,Status")] Student student,IFormFile selectedFile)
        {
            if (id != student.Sid)
            {
                return NotFound();
            }

            if (ModelState.IsValid || ModelState.ErrorCount==1 && ModelState["selectedFile"].ValidationState== ModelValidationState.Invalid)
            {
                if (selectedFile != null)
                {
                    student.Photo = await UploadToImgBB(selectedFile);
                }
                else if (TempData["Photo"] != null)
                {
                    student.Photo = TempData["Photo"].ToString();
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
                    catch(DbUpdateConcurrencyException)
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
               // _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Sid == id);
        }
        public async Task<string> UploadToImgBB(IFormFile file)
        {
            using var client = new HttpClient();
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64 = Convert.ToBase64String(ms.ToArray());

            var content = new MultipartFormDataContent
    {
        { new StringContent("e1e71cb428283c64caf645cbe2543b38"), "key" },
        { new StringContent(base64), "image" }
    };

            var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
            var json = await response.Content.ReadAsStringAsync();

            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return result.data.url;
        }

    }
}
