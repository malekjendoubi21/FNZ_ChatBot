using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FNZ_ChatBot.Data;
using FNZ_ChatBot.Models;

namespace FNZ_ChatBot.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KnowledgeBaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public KnowledgeBaseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: KnowledgeBase
        public async Task<IActionResult> Index(string searchTerm = "", bool showInactive = false)
        {
            var query = _context.KnowledgeBase.AsQueryable();

            if (!showInactive)
            {
                query = query.Where(k => k.IsActive);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(k => k.Question.Contains(searchTerm) || k.Response.Contains(searchTerm));
            }

            var knowledgeItems = await query
                .OrderByDescending(k => k.ModifiedDate)
                .ToListAsync();

            ViewData["SearchTerm"] = searchTerm;
            ViewData["ShowInactive"] = showInactive;
            ViewData["TotalActive"] = await _context.KnowledgeBase.CountAsync(k => k.IsActive);
            ViewData["TotalInactive"] = await _context.KnowledgeBase.CountAsync(k => !k.IsActive);

            return View(knowledgeItems);
        }

        // GET: KnowledgeBase/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var knowledgeBase = await _context.KnowledgeBase
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (knowledgeBase == null)
            {
                return NotFound();
            }

            return View(knowledgeBase);
        }

        // GET: KnowledgeBase/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KnowledgeBase/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KnowledgeBase knowledgeBase)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                knowledgeBase.CreatedBy = userId!;
                knowledgeBase.ModifiedBy = userId!;
                knowledgeBase.CreatedDate = DateTime.UtcNow;
                knowledgeBase.ModifiedDate = DateTime.UtcNow;
                knowledgeBase.IsActive = true;

                _context.Add(knowledgeBase);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Question ajoutée avec succès.";
                return RedirectToAction(nameof(Index));
            }
            return View(knowledgeBase);
        }

        // GET: KnowledgeBase/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var knowledgeBase = await _context.KnowledgeBase.FindAsync(id);
            if (knowledgeBase == null)
            {
                return NotFound();
            }
            return View(knowledgeBase);
        }

        // POST: KnowledgeBase/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KnowledgeBase knowledgeBase)
        {
            if (id != knowledgeBase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    knowledgeBase.ModifiedBy = userId!;
                    knowledgeBase.ModifiedDate = DateTime.UtcNow;

                    _context.Update(knowledgeBase);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Question modifiée avec succès.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KnowledgeBaseExists(knowledgeBase.Id))
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
            return View(knowledgeBase);
        }

        // POST: KnowledgeBase/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var knowledgeBase = await _context.KnowledgeBase.FindAsync(id);
            if (knowledgeBase == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            knowledgeBase.IsActive = !knowledgeBase.IsActive;
            knowledgeBase.ModifiedBy = userId!;
            knowledgeBase.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            var status = knowledgeBase.IsActive ? "activée" : "désactivée";
            TempData["Success"] = $"Question {status} avec succès.";
            
            return RedirectToAction(nameof(Index));
        }

        // POST: KnowledgeBase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var knowledgeBase = await _context.KnowledgeBase.FindAsync(id);
            if (knowledgeBase != null)
            {
                _context.KnowledgeBase.Remove(knowledgeBase);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Question supprimée avec succès.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool KnowledgeBaseExists(int id)
        {
            return _context.KnowledgeBase.Any(e => e.Id == id);
        }
    }
}