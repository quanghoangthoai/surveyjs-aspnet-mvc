using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public int? supplierId { get; set; }
    }

    public class CreateSupplierModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public int displayOrder { get; set; }
        public string surveyId { get; set; }
    }

    public class AssignSupplierSurveyModel
    {
        public int supplierId { get; set; }
        public string surveyId { get; set; }
    }

    [Route("/api")]
    public class ServiceController : Controller
    {
        private readonly ISurveyRepository _surveyRepository;

        public ServiceController(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        [HttpGet("getActive")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _surveyRepository.GetActiveAsync();
            return Json(result);
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

            return Json(survey);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(string name = null, int? supplierId = null, bool isSupplierEvaluation = true)
        {
            var survey = await _surveyRepository.CreateSurveyAsync(name, isSupplierEvaluation, supplierId);
            return Json(survey);
        }

        [HttpGet("changeName")]
        public async Task<IActionResult> ChangeName(string id, string name)
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

            return Json("Ok");
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

            return Json(survey);
        }

        [HttpGet("delete")]
        public async Task<IActionResult> Delete(string id)
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

            return Json(new { id = id });
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostResult([FromBody] PostSurveyResultModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.postId))
            {
                return BadRequest("postId is required.");
            }

            await _surveyRepository.PostResultAsync(model.postId, model.surveyResultText, model.supplierId);
            return Json(new { });
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetResults(string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
            {
                return BadRequest("postId is required.");
            }

            var results = await _surveyRepository.GetResultsAsync(postId);
            return Json(results);
        }

        [HttpGet("suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            var suppliers = await _surveyRepository.GetSuppliersAsync();
            return Json(suppliers);
        }

        [HttpPost("suppliers")]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.name))
            {
                return BadRequest("Supplier name is required.");
            }

            var supplier = await _surveyRepository.CreateSupplierAsync(model.name, model.description, model.displayOrder, model.surveyId);
            return Json(supplier);
        }

        [HttpPost("suppliers/assign")]
        public async Task<IActionResult> AssignSupplierSurvey([FromBody] AssignSupplierSurveyModel model)
        {
            if (model == null || model.supplierId <= 0 || string.IsNullOrWhiteSpace(model.surveyId))
            {
                return BadRequest("supplierId and surveyId are required.");
            }

            var supplier = await _surveyRepository.AssignSurveyToSupplierAsync(model.supplierId, model.surveyId);
            if (supplier == null)
            {
                return NotFound();
            }

            return Json(supplier);
        }

        [HttpGet("suppliers/next")]
        public async Task<IActionResult> GetNextSupplierSurvey(string currentSurveyId = null)
        {
            var survey = await _surveyRepository.GetNextSupplierSurveyAsync(currentSurveyId);
            return Json(survey);
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
