#nullable enable
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
                .Include(s => s.Supplier)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return surveys.Select(s => ToDefinition(s, s.Supplier?.Id));
        }

        public async Task<SurveyDefinition?> GetSurveyAsync(string surveyId)
        {
            if (!TryParseId(surveyId, out var id))
            {
                return null;
            }

            var survey = await _context.Surveys
                .AsNoTracking()
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(s => s.Id == id);

            return survey == null ? null : ToDefinition(survey, survey.Supplier?.Id);
        }

        public async Task<SurveyDefinition> CreateSurveyAsync(string? name, bool isSupplierEvaluation = true, int? supplierId = null, string? templateId = null)
        {
            var templateJson = "{}";
            var templateName = string.Empty;

            if (!string.IsNullOrWhiteSpace(templateId))
            {
                string? rawTemplateId = null;
                if (templateId.StartsWith("default-", StringComparison.OrdinalIgnoreCase))
                {
                    rawTemplateId = templateId.Substring("default-".Length);
                    var template = SurveyDefinition.GetDefaultSurveys().FirstOrDefault(t => t.id == rawTemplateId);
                    if (template != null)
                    {
                        templateJson = template.json;
                        templateName = template.name;
                    }
                }
                else if (templateId.StartsWith("existing-", StringComparison.OrdinalIgnoreCase))
                {
                    rawTemplateId = templateId.Substring("existing-".Length);
                    if (TryParseId(rawTemplateId, out var templateSurveyId))
                    {
                        var existingTemplate = await _context.Surveys.AsNoTracking().FirstOrDefaultAsync(s => s.Id == templateSurveyId);
                        if (existingTemplate != null)
                        {
                            templateJson = existingTemplate.Json;
                            templateName = existingTemplate.Name;
                        }
                    }
                }
                else
                {
                    rawTemplateId = templateId;
                    var template = SurveyDefinition.GetDefaultSurveys().FirstOrDefault(t => t.id == rawTemplateId);
                    if (template != null)
                    {
                        templateJson = template.json;
                        templateName = template.name;
                    }
                    else if (TryParseId(rawTemplateId, out var templateSurveyId))
                    {
                        var existingTemplate = await _context.Surveys.AsNoTracking().FirstOrDefaultAsync(s => s.Id == templateSurveyId);
                        if (existingTemplate != null)
                        {
                            templateJson = existingTemplate.Json;
                            templateName = existingTemplate.Name;
                        }
                    }
                }
            }

            var survey = new Survey
            {
                Name = string.IsNullOrWhiteSpace(name)
                    ? (string.IsNullOrWhiteSpace(templateName) ? string.Empty : templateName)
                    : name,
                Json = string.IsNullOrWhiteSpace(templateJson) ? "{}" : templateJson,
                CreatedAt = DateTime.UtcNow,
                IsSupplierEvaluation = isSupplierEvaluation
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(templateName))
            {
                survey.Name = $"New Survey {survey.Id}";
                await _context.SaveChangesAsync();
            }

            if (isSupplierEvaluation && supplierId.HasValue)
            {
                var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplierId.Value);
                if (supplier != null)
                {
                    supplier.SurveyId = survey.Id;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        survey.Name = supplier.Name;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            await _context.Entry(survey).Reference(s => s.Supplier).LoadAsync();

            return ToDefinition(survey, survey.Supplier?.Id);
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

            await _context.Entry(survey).Reference(s => s.Supplier).LoadAsync();

            return ToDefinition(survey, survey.Supplier?.Id);
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

            var survey = await _context.Surveys.Include(s => s.Supplier).FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return false;
            }

            if (survey.Supplier != null)
            {
                survey.Supplier.SurveyId = null;
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task PostResultAsync(string postId, string resultJson, int? supplierId = null)
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
                if (!supplierId.HasValue)
                {
                    var supplier = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.SurveyId == id);
                    if (supplier != null)
                    {
                        response.SupplierId = supplier.Id;
                    }
                }
            }

            if (supplierId.HasValue)
            {
                var supplierExists = await _context.Suppliers.AnyAsync(s => s.Id == supplierId.Value);
                if (supplierExists)
                {
                    response.SupplierId = supplierId.Value;
                }
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

        public async Task<IEnumerable<SupplierDefinition>> GetSuppliersAsync()
        {
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.Survey)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Id)
                .ToListAsync();

            return suppliers.Select(ToSupplierDefinition);
        }

        public async Task<SupplierDefinition> CreateSupplierAsync(string name, string? description, int displayOrder, string? surveyId)
        {
            if (displayOrder <= 0)
            {
                var hasSuppliers = await _context.Suppliers.AnyAsync();
                var maxOrder = hasSuppliers ? await _context.Suppliers.MaxAsync(s => s.DisplayOrder) : 0;
                displayOrder = maxOrder + 1;
            }

            var supplier = new Supplier
            {
                Name = name,
                Description = description,
                DisplayOrder = displayOrder
            };

            if (TryParseId(surveyId, out var surveyIdValue))
            {
                var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == surveyIdValue);
                if (survey != null)
                {
                    supplier.SurveyId = survey.Id;
                }
            }

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            await _context.Entry(supplier).Reference(s => s.Survey).LoadAsync();

            return ToSupplierDefinition(supplier);
        }

        public async Task<SupplierDefinition?> AssignSurveyToSupplierAsync(int supplierId, string surveyId)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplierId);
            if (supplier == null)
            {
                return null;
            }

            if (!TryParseId(surveyId, out var surveyIdValue))
            {
                return null;
            }

            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == surveyIdValue);
            if (survey == null)
            {
                return null;
            }

            var previousSupplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SurveyId == survey.Id && s.Id != supplierId);
            if (previousSupplier != null)
            {
                previousSupplier.SurveyId = null;
            }

            supplier.SurveyId = survey.Id;
            survey.IsSupplierEvaluation = true;
            await _context.SaveChangesAsync();

            await _context.Entry(supplier).Reference(s => s.Survey).LoadAsync();

            return ToSupplierDefinition(supplier);
        }

        public async Task<SurveyDefinition?> GetNextSupplierSurveyAsync(string? currentSurveyId)
        {
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .Include(s => s.Survey)
                .Where(s => s.SurveyId != null)
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Id)
                .ToListAsync();

            if (!suppliers.Any())
            {
                return null;
            }

            Supplier? targetSupplier;
            if (string.IsNullOrWhiteSpace(currentSurveyId) || !TryParseId(currentSurveyId, out var currentId))
            {
                targetSupplier = suppliers.FirstOrDefault();
            }
            else
            {
                var index = suppliers.FindIndex(s => s.SurveyId == currentId);
                if (index == -1)
                {
                    targetSupplier = suppliers.FirstOrDefault();
                }
                else if (index + 1 < suppliers.Count)
                {
                    targetSupplier = suppliers[index + 1];
                }
                else
                {
                    targetSupplier = null;
                }
            }

            if (targetSupplier?.Survey == null)
            {
                return null;
            }

            return ToDefinition(targetSupplier.Survey, targetSupplier.Id);
        }

        public Task<IEnumerable<SurveyDefinition>> GetSurveyTemplatesAsync()
        {
            var templates = SurveyDefinition.GetDefaultSurveys()
                .Select(t => new SurveyDefinition
                {
                    id = $"default-{t.id}",
                    name = t.name,
                    json = t.json,
                    isSupplierEvaluation = t.isSupplierEvaluation
                });
            return Task.FromResult(templates);
        }

        private static SurveyDefinition ToDefinition(Survey survey, int? supplierId = null)
        {
            return new SurveyDefinition
            {
                id = survey.Id.ToString(),
                name = survey.Name,
                json = survey.Json,
                isSupplierEvaluation = survey.IsSupplierEvaluation,
                supplierId = supplierId ?? survey.Supplier?.Id
            };
        }

        private static SupplierDefinition ToSupplierDefinition(Supplier supplier)
        {
            return new SupplierDefinition
            {
                id = supplier.Id,
                name = supplier.Name,
                description = supplier.Description,
                displayOrder = supplier.DisplayOrder,
                surveyId = supplier.SurveyId?.ToString()
            };
        }

        private static bool TryParseId(string? value, out int id)
        {
            return int.TryParse(value, out id);
        }
    }
}

