using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    student.Photo = await UploadToCloudinary(selectedFile);
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
                    student.Photo = await UploadToCloudinary(selectedFile);
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
        public async Task<string> UploadToCloudinary(IFormFile file)
        {
            var cloudName = "dzdwthulr";
            var apiKey = "932547346819814";
            var apiSecret = "kDgJwwA7a1dPDpyBapA9PY0nbes";

            using var client = new HttpClient();

            var byteArray = Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var byteData = ms.ToArray();
            var base64 = Convert.ToBase64String(byteData);

            var content = new MultipartFormDataContent
    {
        { new StringContent(base64), "file" },
        { new StringContent($"https://api.cloudinary.com/v1_1/{cloudName}/image/upload"), "upload_preset" }
    };

            var postUrl = $"https://api.cloudinary.com/v1_1/{cloudName}/image/upload";
            var payload = new MultipartFormDataContent();
            payload.Add(new ByteArrayContent(byteData), "file", file.FileName);
            payload.Add(new StringContent("ml_default"), "upload_preset");

            var response = await client.PostAsync(postUrl, payload);
            var json = await response.Content.ReadAsStringAsync();

            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
            return result.secure_url;
        }


    }
}
