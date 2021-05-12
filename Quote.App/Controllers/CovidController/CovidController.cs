using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quote.App.Models.Covid19;
using Quote.App.Models.Covid19.Historical;
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
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/all");
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
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/countries");
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
            CountryHistoricalViewModel viewModel = new CountryHistoricalViewModel()
            {
                cou = null,
                historical = null
            };


            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/countries/" + id);
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    viewModel.cou = JsonConvert.DeserializeObject<CountryRoot>(read.Result);
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/historical/" + id + "?lastdays=all");
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    viewModel.historical = JsonConvert.DeserializeObject<HistoricalRoot>(read.Result);

                    dynamic stuff = JObject.Parse(read.Result);
                    viewModel.historical.timeline.cases.dCase = JsonConvert.DeserializeObject<Dictionary<DateTime, long>>((stuff.timeline.cases).ToString());
                    viewModel.historical.timeline.deaths.dDeath = JsonConvert.DeserializeObject<Dictionary<DateTime, long>>((stuff.timeline.deaths).ToString());
                    viewModel.historical.timeline.recovered.dRecovered = JsonConvert.DeserializeObject<Dictionary<DateTime, long>>((stuff.timeline.recovered).ToString());
                }
            }
            return View(viewModel);
        }

        public ActionResult FilterByContinent(string id)
        {
            //All
            if (id.Equals("All", StringComparison.CurrentCultureIgnoreCase))
            {
                return RedirectToAction("Index", "Covid", null);
            }

            IEnumerable<ContinentRoot> cons = null;
            ContinentRoot con = null;
            IEnumerable<CountryRoot> cous = null;
            string listCountries;

            //Oceania
            if (id.Equals("Oceania", StringComparison.CurrentCultureIgnoreCase))
            {
                id = "Australia%2FOceania";

            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/continents/" + id + "?strict=true");
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    con = JsonConvert.DeserializeObject<ContinentRoot>(read.Result);
                }
            }

            listCountries = string.Join(",", con.countries);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/countries/" + listCountries);
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

        public ActionResult GetHistorical(string id)
        {
            HistoricalRoot his = null;
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://corona.lmao.ninja");
                var get = client.GetAsync("v2/historical/" + id + "?lastdays=3");
                get.Wait();

                var result = get.Result;
                if(result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    his = JsonConvert.DeserializeObject<HistoricalRoot>(read.Result);

                    dynamic stuff = JObject.Parse(read.Result);
                    his.timeline.cases.dCase = JsonConvert.DeserializeObject<Dictionary<string, long>>((stuff.timeline.cases).ToString());
                    his.timeline.deaths.dDeath = JsonConvert.DeserializeObject<Dictionary<string, long>>((stuff.timeline.deaths).ToString());
                    his.timeline.recovered.dRecovered = JsonConvert.DeserializeObject<Dictionary<string, long>>((stuff.timeline.recovered).ToString());

                    return View(his);
                }
            }

            return HttpNotFound();

        }

    }
}