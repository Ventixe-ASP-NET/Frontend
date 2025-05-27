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
            var invoices = await _invoiceClient.GetFromJsonAsync<List<ShowInvoiceViewModel>>("api/invoices?code=DRMrLNX-3fbx6GeIP9o3vN_Pa1g21n4gJyNHUfcqjFERAzFu1SZVGw==");
            return View("Invoices/Index", invoices);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInvoice(string id)
        {
            var invoice = await _invoiceClient.GetFromJsonAsync<ShowInvoiceViewModel>("api/update?code=WXgN_-IZC21f1ntwaHtUN6QX6-vuTrwTyRUhc6ycIfbuAzFuwsLiTw==");
            return RedirectToAction("Index", "Invoice");
        }
    }
}

/*
 Functions:

CreateInvoice: [POST]
https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net/api/invoices?code=wset0lxLDluzSYBFqkaPJiytI0TeWV6glv1yuRfSmzudAzFuPAb_iA==

DeleteInvoice: [DELETE]
https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net/api/invoices/{id}?code=tnP9IZE0KG0Gi6oNMyIJMcFN545wEvDtfMeX0-Xa0BiMAzFuviJ2Bg==

GetAllInvoices: [GET]
https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net/api/invoices?code=DRMrLNX-3fbx6GeIP9o3vN_Pa1g21n4gJyNHUfcqjFERAzFu1SZVGw==

GetAllStatuses: [GET]
https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net/api/statuses?code=lCoZvuxLiDzLZZmWrAx_QcNnhW9pJUFmgKVacyBn9JbjAzFuuYdxWQ==

GetInvoiceById: [GET]
https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net/api/invoices/{id}?code=tGq5_bnSYXpoU7clqiGxuPjy8hFe7IlHnKMqtQjaZ7snAzFuLzYjgQ==

UpdateInvoice: [PUT]
https://invoiceservice-gtg3ajc5htdcgpgs.swedencentral-01.azurewebsites.net/api/invoices?code=WXgN_-IZC21f1ntwaHtUN6QX6-vuTrwTyRUhc6ycIfbuAzFuwsLiTw==


INVOICE ID:
ef875fd7-ac84-4552-9493-5e8d49e961e3
 */
