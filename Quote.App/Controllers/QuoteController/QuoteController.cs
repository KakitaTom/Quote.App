using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Quote.App.Controllers.QuoteController
{
    public class QuoteController : Controller
    {
        // GET: Quote
        public ActionResult Index()
        {
            IEnumerable<Models.Quote> quotes = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var getAll = client.GetAsync("/api/quote/all");
                getAll.Wait();

                var result = getAll.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    quotes = JsonConvert.DeserializeObject<List<Models.Quote>>(read.Result);
                }
                else
                {
                    quotes = null;
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(quotes);
        }


        public ActionResult Details(string Id)
        {
            Models.Quote quote = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var getRandom = client.GetAsync("/api/quote/detail/" + Id);

                var result = getRandom.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    quote = JsonConvert.DeserializeObject<Models.Quote>(read.Result);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(quote);
        }

        public ActionResult Random(string Id)
        {
            Models.Quote quote = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var getRandom = client.GetAsync("api/quote/random");
                getRandom.Wait();

                var result = getRandom.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    quote = JsonConvert.DeserializeObject<Models.Quote>(read.Result);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(quote);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Models.Quote quote)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var create = client.PostAsJsonAsync("/api/quote/create", quote);
                create.Wait();

                var result = create.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Quote", null);
                }
                
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        public ActionResult Edit(string Id)
        {
            Models.Quote quote = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var find = client.GetAsync("/api/quote/detail/" + Id);
                find.Wait();

                var result = find.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsAsync<Models.Quote>();
                    read.Wait();

                    quote = read.Result;
                    return View(quote);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [Route("quote/edit/{Id}")]
        public ActionResult Edit(string Id, Models.Quote quote)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var edit = client.PostAsJsonAsync("/api/quote/edit/" + quote.id, quote);
                edit.Wait();

                var result = edit.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Details", "Quote", new {id = quote.id});
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult Delete(string Id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://lamapimongodb.somee.com");
                var delete = client.DeleteAsync("/api/quote/delete/" + Id);

                var result = delete.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Quote", null);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }
    }
}