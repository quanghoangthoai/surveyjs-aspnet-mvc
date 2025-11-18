using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using surveyjs_aspnet_mvc;
using surveyjs_aspnet_mvc.Data;
using surveyjs_aspnet_mvc.Models;

namespace surveyjs_aspnet_mvc.Services
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly ApplicationDbContext _context;

        public SurveyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SurveyDefinition>> GetActiveAsync()
        {
            var surveys = await _context.Surveys
                .AsNoTracking()
                .OrderBy(s => s.Id)
                .ToListAsync();

            return surveys.Select(ToDefinition);
        }

        public async Task<SurveyDefinition?> GetSurveyAsync(string surveyId)
        {
            if (!TryParseId(surveyId, out var id))
            {
                return null;
            }

            var survey = await _context.Surveys
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            return survey == null ? null : ToDefinition(survey);
        }

        public async Task<SurveyDefinition> CreateSurveyAsync(string? name)
        {
            var survey = new Survey
            {
                Name = string.IsNullOrWhiteSpace(name) ? string.Empty : name,
                Json = "{}",
                CreatedAt = DateTime.UtcNow
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(name))
            {
                survey.Name = $"New Survey {survey.Id}";
                await _context.SaveChangesAsync();
            }

            return ToDefinition(survey);
        }

        public async Task<SurveyDefinition?> UpdateSurveyJsonAsync(string surveyId, string json)
        {
            if (!TryParseId(surveyId, out var id))
            {
                return null;
            }

            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return null;
            }

            survey.Json = string.IsNullOrWhiteSpace(json) ? "{}" : json;
            await _context.SaveChangesAsync();

            return ToDefinition(survey);
        }

        public async Task<bool> ChangeNameAsync(string surveyId, string name)
        {
            if (!TryParseId(surveyId, out var id))
            {
                return false;
            }

            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return false;
            }

            survey.Name = string.IsNullOrWhiteSpace(name) ? $"New Survey {survey.Id}" : name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSurveyAsync(string surveyId)
        {
            if (!TryParseId(surveyId, out var id))
            {
                return false;
            }

            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return false;
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task PostResultAsync(string postId, string resultJson)
        {
            var response = new SurveyResponse
            {
                PostId = postId,
                ResultJson = string.IsNullOrWhiteSpace(resultJson) ? "{}" : resultJson,
                CreatedAt = DateTime.UtcNow
            };

            if (TryParseId(postId, out var id))
            {
                response.SurveyId = id;
            }

            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();
        }

        public async Task<SurveyResultsDefinition> GetResultsAsync(string postId)
        {
            var results = new SurveyResultsDefinition { id = postId };

            var entries = await _context.SurveyResponses
                .AsNoTracking()
                .Where(r => r.PostId == postId)
                .OrderBy(r => r.Id)
                .Select(r => r.ResultJson)
                .ToListAsync();

            foreach (var entry in entries)
            {
                results.data.Add(entry);
            }

            return results;
        }

        private static SurveyDefinition ToDefinition(Survey survey)
        {
            return new SurveyDefinition
            {
                id = survey.Id.ToString(),
                name = survey.Name,
                json = survey.Json
            };
        }

        private static bool TryParseId(string? value, out int id)
        {
            return int.TryParse(value, out id);
        }
    }
}

