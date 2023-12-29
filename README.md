# JustResult
This is a simple implementation of the Result Pattern thought to be as unobstructive as possible with the programming flow.

```csharp
...
public async Task<Result<string>> GetArticleDescription(Guid articleId)
{
  try
  {
    var articleResult = await _repo.GetArticleAsync(articleId);

    if (!articleResult)
    {
      return (List<Error>) articleResult;
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
