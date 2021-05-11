using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using Quote.App.Models.Covid19;
using Quote.App.ViewModel;

namespace Quote.App.Controllers.CovidController
{
    public class CovidController : Controller
    {
        // GET: Covid
        public ActionResult Index()
        {
            var viewModel = new AllCountriesViewModel();

            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://corona.lmao.ninja");
                var get = client.GetAsync("/v2/all");
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    viewModel.global = JsonConvert.DeserializeObject<Global>(read.Result);
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://corona.lmao.ninja");
                var get = client.GetAsync("/v2/countries");
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    viewModel.countries = JsonConvert.DeserializeObject<IEnumerable<CountryRoot>>(read.Result);
                } }

            return View(viewModel);
        }

        [Route("covid/country/{id}")]
        public ActionResult OneCountry(string id)
        {
            CountryRoot country = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://corona.lmao.ninja");
                var get = client.GetAsync("/v2/countries/" + id);
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    country = JsonConvert.DeserializeObject<CountryRoot>(read.Result);
                    return View(country);
                }
            }

            return HttpNotFound();
        }
    }
}