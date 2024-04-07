#nullable enable
using System;
using System.Linq;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using Spectre.Console;

namespace TOTOllyGeek.Awesome.News;

internal class NewsExecutor(string? searchTag = null) : OperationExecutor
{
    private const string NewsApiKeyVariableName = "NEWS_API_KEY";
    public override string OperationName => "What are the latest news?";

    public override int Execute()
    {
        var key = GetApiKeyOrThrow();
        var searchQuery = searchTag ?? "NASA";
        
        var newsApiClient = new NewsApiClient(key);
        var articlesResponse = newsApiClient.GetEverything(new EverythingRequest
        {
            Q = searchQuery,
            SortBy = SortBys.Popularity,
            Language = Languages.EN,
            From = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
            Page = 0,
            PageSize = 10
        });

        if (articlesResponse.Status != Statuses.Ok) return 1;

        var articles =
            articlesResponse.Articles
                .Where(a => !a.Title.Equals("[Removed]", StringComparison.OrdinalIgnoreCase))
                .ToArray();
        
        AnsiConsole.Write(BuildTable(articles));

        return 0;
    }

    private static Table BuildTable(Article[] articles)
    {
        var table = 
            new Table()
                .Border(TableBorder.Double)
                .Centered()
                .Width(200);

        table.AddColumn(
            new TableColumn("Article")
                .Centered()
                .Width(50));
        table.AddColumn(new TableColumn("Description")
            .Width(100));

        foreach (var article in articles)
        {
            var titlePanel = new Panel(article.Title.EscapeMarkup()).Border(BoxBorder.Ascii);
            var infoColumns = 
                new Columns(
                new Markup($"[lime underline]{article.Author.EscapeMarkup()}[/]"),
                new Markup($"[lime underline]{article.PublishedAt:f}[/]"));
            
            var infoRows = new Rows(titlePanel, infoColumns);
            var descriptionPanel = new Panel(article.Description.EscapeMarkup()).Border(BoxBorder.Rounded);

            table.AddRow(infoRows, descriptionPanel);
        }

        return table;
    }

    private static string GetApiKeyOrThrow()
    {
        var key = Environment.GetEnvironmentVariable(NewsApiKeyVariableName);

        if (key == null)
        {
            throw new InvalidOperationException(
                $"NewsAPI key was not provided and also is not set into \"{NewsApiKeyVariableName}\" environment variable.");
        }

        return key;
    }
}