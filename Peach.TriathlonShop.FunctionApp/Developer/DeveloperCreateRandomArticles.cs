using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Peach.TriathlonShop.Database;
using Peach.TriathlonShop.Database.Models;
using Peach.TriathlonShop.FunctionApp.Extensions;
using Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions;
using Peach.TriathlonShop.FunctionApp.Http.Developer.Models;
using System.Text.Json;

namespace Peach.TriathlonShop.FunctionApp.Developer
{
    public class DeveloperCreateRandomArticles(TriathlonShopDbContext context)
    {
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        [Function(nameof(DeveloperCreateRandomArticles))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "developer/articles/random")] HttpRequest req)
        {
            using var reader = new StreamReader(req.Body);
            var requestBody = await reader.ReadToEndAsync();

            var model = JsonSerializer.Deserialize<CreateRandomArticlesModel>(requestBody, jsonSerializerOptions);

            if (model == null)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            if (!model.IsValidRequestBody())
                return new BadRequestResult();

            List<Article> articles = [];

            for (int i = 0; i < model.ArticlesCount; i++)
            {
                var article = new Article
                {
                    Number = $"ART-{Guid.NewGuid()}",
                    Name = ArticleExtensions.GetRandomArticle(),
                    UnitPrice = Random.Shared.Next(model.UnitPriceIntervalStart ?? 1, model.UnitPriceIntervalEnd ?? 100)
                };

                if (context.Articles.Any(a => a.Name == article.Name))
                {
                    i--;
                    continue;
                }

                articles.Add(article);
            }

            context.Articles.AddRange(articles);
            await context.SaveChangesAsync();

            return new OkResult();
        }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Http.Developer.Models
{
    public class CreateRandomArticlesModel
    {
        public int? ArticlesCount { get; set; }
        public int? UnitPriceIntervalStart { get; set; }
        public int? UnitPriceIntervalEnd { get; set; }
    }
}

namespace Peach.TriathlonShop.FunctionApp.Http.Developer.Extensions
{
    public static class CreateRandomArticlesExtensions
    {
        public static bool IsValidRequestBody(this CreateRandomArticlesModel model)
        {
            return model != null
                && model.ArticlesCount != null
                && model.ArticlesCount > 0
                && model.UnitPriceIntervalStart != null
                && model.UnitPriceIntervalEnd != null
                && model.UnitPriceIntervalStart < model.UnitPriceIntervalEnd;
        }
    }
}