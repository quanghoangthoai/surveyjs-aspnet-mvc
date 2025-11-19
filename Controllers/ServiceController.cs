using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using surveyjs_aspnet_mvc.Services;

namespace surveyjs_aspnet_mvc.Controllers
{

    public class ChangeSurveyModel
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class PostSurveyResultModel
    {
        public string postId { get; set; }
        public string surveyResultText { get; set; }
    }

    [ApiController]
    [Route("api")]
    public class ServiceController : ControllerBase
    {
        private readonly ISurveyRepository _surveyRepository;

        public ServiceController(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working!", timestamp = DateTime.Now });
        }

        [HttpGet("getActive")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var result = await _surveyRepository.GetActiveAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Return detailed error for debugging
                return Content(JsonConvert.SerializeObject(new { 
                    error = ex.Message, 
                    stackTrace = ex.StackTrace 
                }), "application/json");
            }
        }

        [HttpGet("getSurvey")]
        public async Task<IActionResult> GetSurvey(string surveyId)
        {
            if (string.IsNullOrWhiteSpace(surveyId))
            {
                return BadRequest("Survey id is required.");
            }

            var survey = await _surveyRepository.GetSurveyAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        [HttpPost("create")]
        [HttpGet("create")] // Hỗ trợ GET để tương thích với frontend cũ
        public async Task<IActionResult> Create([FromQuery] string name)
        {
            var survey = await _surveyRepository.CreateSurveyAsync(name);
            return Ok(survey);
        }

        [HttpPut("changeName")]
        [HttpGet("changeName")] // Hỗ trợ GET để tương thích với frontend cũ
        public async Task<IActionResult> ChangeName([FromQuery] string id, [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Survey id is required.");
            }

            var success = await _surveyRepository.ChangeNameAsync(id, name);
            if (!success)
            {
                return NotFound();
            }

            return Ok("Ok");
        }

        [HttpPost("changeJson")]
        public async Task<IActionResult> ChangeJson([FromBody] ChangeSurveyModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.id))
            {
                return BadRequest("Survey id is required.");
            }

            var survey = await _surveyRepository.UpdateSurveyJsonAsync(model.id, model.text);
            if (survey == null)
            {
                return NotFound();
            }

            return Ok(survey);
        }

        [HttpDelete("delete")]
        [HttpGet("delete")] // Hỗ trợ GET để tương thích với frontend cũ
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Survey id is required.");
            }

            var success = await _surveyRepository.DeleteSurveyAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return Ok(new { id = id });
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostResult([FromBody] PostSurveyResultModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.postId))
            {
                return BadRequest("postId is required.");
            }

            await _surveyRepository.PostResultAsync(model.postId, model.surveyResultText);
            return Ok(new { });
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetResults(string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
            {
                return BadRequest("postId is required.");
            }

            var results = await _surveyRepository.GetResultsAsync(postId);
            return Ok(results);
        }

        // // GET api/values/5
        // [HttpGet("{id}")]
        // public string Get(int id)
        // {
        //     return "value";
        // }

        // // POST api/values
        // [HttpPost]
        // public void Post([FromBody]string value)
        // {
        // }

        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody]string value)
        // {
        // }

        // // DELETE api/values/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
