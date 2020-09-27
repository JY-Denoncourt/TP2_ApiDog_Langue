using ApiHelper.Models;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiHelper
{
    public class DogApiProcessor
    {
        //(OK) Load list de Breeds
        public static async Task<List<string>> LoadBreedList()
        {
            string url = $"https://dog.ceo/api/breeds/list/all";


            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    BreedModel result = await response.Content.ReadAsAsync<BreedModel>();

                    var enfant = result.message.Values.ToList();
                    var familles = result.message.Keys.ToList();

                    var retValues = new List<string>();

                    foreach (string f in familles)
                    {
                        retValues.Add(f);
                        if (result.message[f].Count > 0)
                        {
                            foreach (string sub in result.message[f])
                            {
                                retValues.Add($"{f}/{sub}");
                            }
                        }
                    }                        
                    return retValues;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

        }


        public static async Task<DogsModel> GetImageUrl(string breed, int nbImg)
        {
            string url = $"https://dog.ceo/api/breed/" + breed + "/images/random/" + nbImg;
            DogsModel DogImg = new DogsModel();

            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    DogImg = await response.Content.ReadAsAsync<DogsModel>();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
                
            return DogImg;
        }
    }
}
