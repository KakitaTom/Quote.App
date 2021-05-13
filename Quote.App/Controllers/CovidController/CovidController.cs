using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
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
            CountryRoot cou = new CountryRoot();
            IList<CountryRoot> countries = null;

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

                    countries = JsonConvert.DeserializeObject<IList<CountryRoot>>(read.Result);
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/all");
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    cou = JsonConvert.DeserializeObject<CountryRoot>(read.Result);
                    cou.country = "World";
                    countries.Add(cou);

                    countries = countries.OrderByDescending(c => c.cases).ToList();
                }
            }

            return View(countries);
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
            CountryRoot cou = new CountryRoot()
            {
                country = id
            };
            IList<CountryRoot> countries = null;
            string listOfCountries = "";
            ContinentRoot getAllCountries = null;

            //All Or Oceania
            if (id.Equals("All", StringComparison.CurrentCultureIgnoreCase))
            {
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

                        countries = JsonConvert.DeserializeObject<IList<CountryRoot>>(read.Result);
                    }
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://disease.sh");
                    var get = client.GetAsync("/v3/covid-19/all");
                    get.Wait();

                    var result = get.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var read = result.Content.ReadAsStringAsync();
                        read.Wait();

                        cou = JsonConvert.DeserializeObject<CountryRoot>(read.Result);
                        cou.country = "World";

                        countries.Add(cou);

                        countries = countries.OrderByDescending(c => c.cases).ToList();
                    }
                }

                return View("Index", countries);
            }
            else if (id.Equals("Oceania", StringComparison.CurrentCultureIgnoreCase))
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

                    getAllCountries = JsonConvert.DeserializeObject<ContinentRoot>(read.Result);

                    cou.cases = getAllCountries.cases;
                    cou.todayCases = getAllCountries.todayCases;
                    cou.deaths = getAllCountries.deaths;
                    cou.todayDeaths = getAllCountries.todayDeaths;
                    cou.recovered = getAllCountries.recovered;
                    cou.todayRecovered = getAllCountries.recovered;
                    cou.active = getAllCountries.active;
                    cou.critical = getAllCountries.critical;
                    cou.population = getAllCountries.population;
                }
            }

            listOfCountries = string.Join(",", getAllCountries.countries);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://disease.sh");
                var get = client.GetAsync("/v3/covid-19/countries/" + listOfCountries);
                get.Wait();

                var result = get.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();

                    countries = JsonConvert.DeserializeObject<IList<CountryRoot>>(read.Result);

                    countries.Add(cou);
                    countries = countries.OrderByDescending(c => c.cases).ToList();
                }
            }

            return View("Index", countries);
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