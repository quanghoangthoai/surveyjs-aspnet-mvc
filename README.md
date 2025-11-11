# SurveyJS + .NET Core Demo Example

This demo shows how to integrate [SurveyJS](https://surveyjs.io/) components with a .NET Core backend.

[View Demo Online](https://surveyjs-aspnet-core.azurewebsites.net/)

## Disclaimer

This demo must not be used as a real service as it doesn't cover such real-world survey service aspects as authentication, authorization, user management, access levels, and different security issues. These aspects are covered by backend-specific articles, forums, and documentation.

## Run the Application

Install [.NET](https://dotnet.microsoft.com/en-us/download) on your machine. After that, run the following commands:

```bash
git clone https://github.com/surveyjs/surveyjs-aspnet-mvc.git
cd surveyjs-aspnet-mvc
dotnet tool install --global dotnet-ef # optional if not already installed
dotnet restore
dotnet ef database update
dotnet build
dotnet run
```

Open http://localhost:5000 in your web browser.

## Database Configuration

The backend stores survey definitions and responses in SQL Server using Entity Framework Core.

- Update the `DefaultConnection` string in `appsettings.json` or `appsettings.Development.json` to point to your SQL Server instance.
- To generate new migrations after making model changes, run `dotnet ef migrations add <MigrationName>`.
- Apply pending migrations with `dotnet ef database update`.
- The application seeds default surveys and sample results automatically on startup.
- Supplier-specific tables (`Suppliers`, `SurveyResponses.SupplierId`) are created automatically starting from the `AddSuppliers` migration.

## Supplier Workflow API

Suppliers (NCC) can be defined ahead of time and linked to surveys so that respondents complete one supplier survey after another until the list is exhausted.

- `GET /api/suppliers` &mdash; list suppliers in display order. Each entry indicates the linked survey (if any).
- `POST /api/suppliers` &mdash; create a supplier. Payload: `{ "name": "...", "description": "...", "displayOrder": 1, "surveyId": "3" }`.
- `POST /api/suppliers/assign` &mdash; attach an existing survey to a supplier (replaces previous associations if needed).
- `GET /api/suppliers/next?currentSurveyId=2` &mdash; retrieve the next supplier survey in sequence. Omit `currentSurveyId` to get the first survey.
- `GET /api/create?name=...&supplierId=1&isSupplierEvaluation=true` &mdash; create a new survey and bind it to a supplier. Set `isSupplierEvaluation=false` to create a standalone survey that is not tied to any supplier.
- `POST /api/post` accepts an optional `supplierId` alongside the existing payload so responses can be attributed to a supplier.

## Client-Side App

The client-side part is the `surveyjs-react-client` React application. The current project includes only the application's build artifacts. Refer to the [surveyjs-react-client](https://github.com/surveyjs/surveyjs-react-client) repo for full code and information about the application.

## Other Resources

The [Endatix Platform](https://endatix.com/?utm_source=surveyjs&utm_medium=partner+link) is an open-source solution that provides a fully integrated server-side API for managing SurveyJS forms and their results within .NET environments. Endatix provides a complete backend application structure, from API requests to database integration and admin management, allowing users to quickly implement a fully operational server environment with minimal coding. Endatix is an ideal choice for developers looking for an efficient way to implement SurveyJS without extensive server-side development. 

**Endatix Platform repo:** https://github.com/endatix/endatix

