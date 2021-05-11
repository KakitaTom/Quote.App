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

        public ActionResult FilterByContinent(string id)
        {


            //All
            if (id.Equals("All", StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToAction("Index", "Covid", null);
            }

            IEnumerable<ContinentRoot> cons = null;
            ContinentRoot oseania = null;

            //Oceania
            if (id.Equals("Oceania", StringComparison.CurrentCultureIgnoreCase))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://corona.lmao.ninja");
                    var get = client.GetAsync("/v2/continents");
                    get.Wait();

                    var result = get.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var read = result.Content.ReadAsStringAsync();
                        read.Wait();

                        cons = JsonConvert.DeserializeObject<IEnumerable<ContinentRoot>>(read.Result);
                    }
                }

                oseania = cons.FirstOrDefault(o => o.continent == "Australia/Oceania");
            }



            ContinentRoot con = null;
            IEnumerable<CountryRoot> cous = null;
            string listCountries;


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://corona.lmao.ninja");
                var get = client.GetAsync("/v2/continents/" + id + "?strict");
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    con = JsonConvert.DeserializeObject<ContinentRoot>(read.Result);
                }
            }

            

            if (id.Equals("Oceania", StringComparison.CurrentCultureIgnoreCase))
            {
                listCountries = string.Join(",", oseania.countries);
                con = oseania;
            }
            else
            {
                listCountries = string.Join(",", con.countries);
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://corona.lmao.ninja");
                var get = client.GetAsync("/v2/countries/" + listCountries);
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    cous = JsonConvert.DeserializeObject<IEnumerable<CountryRoot>>(read.Result);
                }
            }

            

            var viewModel = new ContinentCountriesViewModel()
            {
                continent = con,
                countries = cous
            };

            return View(viewModel);


        }

    }
}