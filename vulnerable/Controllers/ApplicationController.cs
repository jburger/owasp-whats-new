using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vulnerable.Domain;

namespace vulnerable.Controllers {
    public class ApplicationController : Controller {
        private IApplicationEvaluator applicationEvaluator;
        private readonly ILogger<ApplicationController> logger;

        public ApplicationController(IApplicationEvaluator applicationEvaluator, ILogger<ApplicationController> logger)
        {
            this.applicationEvaluator = applicationEvaluator;
            this.logger = logger;
        }

        public IActionResult Index() 
        {
            ViewData["Message"] = "Welcome to Piggy Bank Co. To apply for an OinkCoin loan, please upload your details";
            return View();
        }

        [HttpPost]
        public IActionResult Index(IFormFile bankFile, string firstName, string lastName)
        {
            EvaluationResult result = 
                applicationEvaluator.Evaluate(firstName, lastName, bankFile);

            if(result.Status == EvaluationStatus.Accepted) 
            {
                logger.LogInformation($"Accepted application for {firstName} {lastName}");
                return RedirectToAction("Accept", "Application", new {firstName});
            } 
            else 
            {
                TempData["Reason"] = result.RejectReason;
                logger.LogInformation($"Rejected application for {firstName} {lastName}");
                return RedirectToAction("Rejected", "Application", new {firstName, result.RejectReason});
            }
        }

        public IActionResult Accept(string firstName) {
            ViewBag.FirstName = firstName;
            return View();
        }

        public IActionResult Rejected(string firstName, string rejectReason) {
            ViewBag.firstName = firstName;
            ViewBag.rejectReason = rejectReason;
            return View();
        }
    }
}