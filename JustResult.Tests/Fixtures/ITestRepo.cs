
namespace JustResult.Tests.Fixtures
{
	public interface ITestRepo
	{
		Task<Result<Article>> GetArticleById(Guid articleId);
	}
}