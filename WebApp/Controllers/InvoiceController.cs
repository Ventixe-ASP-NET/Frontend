using Microsoft.AspNetCore.Mvc;
using WebApp.Views.Invoices.InvoiceModels;

namespace WebApp.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly HttpClient _invoiceClient;

        public InvoiceController(IHttpClientFactory factory)
        {
            _invoiceClient = factory.CreateClient("invoiceGateway");
        }
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceClient.GetFromJsonAsync<List<ShowInvoiceViewModel>>("api/invoices");
            return View(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInvoice(string id)
        {
            var invoice = await _invoiceClient.GetFromJsonAsync<ShowInvoiceViewModel>("api/update");
            return RedirectToAction("Index", "Invoice");
        }
    }
}
