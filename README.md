# JustResult
[![.NET](https://github.com/esquijarosa/JustResult/actions/workflows/dotnet.yml/badge.svg)](https://github.com/esquijarosa/JustResult/actions/workflows/dotnet.yml)

This is a simple implementation of the Result Pattern thought to be as unobstructive as possible with the programming flow.

```csharp
/// Application...
...

public async Task ProcessArticle(Guid articleId)
{
  var articleResult = await GetArticleDescription(articleId);

  if (!articleResult)
  {
	// Handle error
	return;
  }

  var articleDescription = (string) articleResult;

  // Do something with the article description
}

...

/// Articles Service...
...

public async Task<Result<string>> GetArticleDescription(Guid articleId)
{
  try
  {
    var articleResult = await _repo.GetArticleAsync(articleId);

    if (!articleResult)
    {
      return articleResult.Errors;
    }

    if ((Article?) articleResult is not Article article)
    {
      return new Error("NotFound", "Article does not exists.");
    }

    return article.Description;
  }
  catch (Exception ex)
  {
    return ex;
  }
}

...

/// Articles Repo...
...

public async Task<Result<Article>> GetArticleAsync(Guid articleId)
{
  try
  {
    return await _db.QueryAsync<Article>(Queries.GetArticleById, new { articleId });
  }
  catch (Exception ex)
  {
    return ex;
  }
}

...
```
