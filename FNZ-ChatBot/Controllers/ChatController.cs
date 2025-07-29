using FNZ_ChatBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FNZ_ChatBot.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        // Injection du service
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // GET: ChatController
        public ActionResult Index()
        {
           // return View();
            return View("GetResponse");

        }
        // API pour envoyer une question et obtenir une réponse

        [HttpPost]
        public IActionResult GetResponse([FromBody] string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return BadRequest("La question ne peut pas être vide.");

            var response = _chatService.GetResponse(userInput);
            return Ok(new { question = userInput, answer = response });
        }

        // GET: ChatController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ChatController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChatController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ChatController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ChatController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ChatController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ChatController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
