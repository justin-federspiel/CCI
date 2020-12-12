using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Formatting;

namespace CrescendoCollectiveInterviewAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessReviewController : ControllerBase
    {
        private const string InitialRequestURLBase = "https://api.yelp.com/v3/businesses/{0}/reviews"; 
        private const string InitialRequestURLParameters = "?locale=en_US";
        private const string BusinessLocationURLBase = "https://api.yelp.com/v3/businesses/{0}";
        private const string BusinessLocationURLParameters = "?locale=en_US";
        private const string UserLocationURLBase = "{0}"; // https://www.yelp.com/user_details";
        private const string UserLocationURLParameters = "?userid={0}";
        private const string UserLocationScrapingTag1 = "<h3 class=\"user-location alternate\">";
        private const string UserLocationScrapingTag2 = "</h3>";
        private const string GoogleImageAPIURLBase = "";
        private const string GoogleImageAPIURLParameters = "";
        [HttpGet]
        public async Task<JsonResult> Index(string id)
        {
            string yelpKey = Environment.GetEnvironmentVariable("yelp_key");                   
            if(!string.IsNullOrWhiteSpace(yelpKey))
            {
                using HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(string.Format(InitialRequestURLBase, id))
                };
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", yelpKey);

                HttpResponseMessage response = client.GetAsync(InitialRequestURLParameters).Result;
                if (response.IsSuccessStatusCode)
                {
                    YelpReviewAnswer yelpInitialAnswer = await response.Content.ReadAsAsync<YelpReviewAnswer>();
                    List<ReviewResult> resultWithLocations = await GetLocations(yelpInitialAnswer, id, yelpKey);                    
                    return new JsonResult(resultWithLocations);
                }
                else
                {
                    return new JsonResult(new { status_code = response.StatusCode, reason_phrase = response.ReasonPhrase });
                }
            } else
            {
                return new JsonResult(new { status_code = 500, reason_phrase = "Internal Server Error", further_help = "Yelp API key not provided." });
            }
        }
        

        private async Task<List<ReviewResult>> GetLocations(YelpReviewAnswer yelpInitialAnswer, string initialRequestBusinessId, string yelpKey)
        {
            List<ReviewResult> withLocations = new List<ReviewResult>();
            BusinessLocation businessLocation = await GetBusinessLocation(initialRequestBusinessId, yelpKey);
            for(int i = 0; i < yelpInitialAnswer.Reviews.Length; i++)
            {
                withLocations.Add(
                    new ReviewResult { 
                        Business_location = businessLocation, 
                        Content = yelpInitialAnswer.Reviews[i].Text, 
                        Rating = yelpInitialAnswer.Reviews[i].Rating, 
                        Reviewer = await GetReviewerInfo(yelpInitialAnswer.Reviews[i].User) 
                    }
                    );
            }
            return withLocations;
        }

        private async Task<BusinessLocation> GetBusinessLocation(string initialRequestBusinessId, string yelpKey)
        {
            using HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(BusinessLocationURLBase, initialRequestBusinessId))
            };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", yelpKey);

            HttpResponseMessage response = client.GetAsync(InitialRequestURLParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                YelpBusiness yelpAnswer = await response.Content.ReadAsAsync<YelpBusiness>();
                BusinessLocation successfulResult = yelpAnswer.Location;
                return successfulResult;
            }
            else
            {
                throw new ApplicationException("Did not get address for business " + initialRequestBusinessId); //Or should we still send what we have back?
            }        
        }

        private async Task<ResultReviewer> GetReviewerInfo(YelpUser user)
        {
            ResultReviewer reviewerWithNoAddedInfo = new ResultReviewer { Name = user.Name, Avatar_image_url = user.Image_url };
            ResultReviewer reviewerWithLocation = new ResultReviewer { Name = reviewerWithNoAddedInfo.Name, Avatar_image_url = reviewerWithNoAddedInfo.Avatar_image_url };

            using HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(string.Format(UserLocationURLBase, user.Profile_url))
            };            

            HttpResponseMessage response = client.GetAsync("").Result;// (string.Format(UserLocationURLParameters,user.Id)).Result;
            if (response.IsSuccessStatusCode)
            {
                string wholeResponse = await response.Content.ReadAsStringAsync();
                int locationLocation = wholeResponse.IndexOf(UserLocationScrapingTag1);
                if(locationLocation > -1)
                {
                    int lengthOfLocation = wholeResponse.IndexOf(UserLocationScrapingTag2, locationLocation) - locationLocation - UserLocationScrapingTag1.Length;
                    if(lengthOfLocation > 0)
                    {
                        reviewerWithLocation.Location = wholeResponse.Substring(locationLocation + UserLocationScrapingTag1.Length, lengthOfLocation);
                    }
                }                
            }
            if (string.IsNullOrWhiteSpace(reviewerWithLocation.Location))
            {
                reviewerWithLocation.Location = "(Could not find location)";
            }
            return reviewerWithLocation;
        }
    }
}
