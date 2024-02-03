using System;
using System.Threading.Tasks;

namespace JustResult.Tests.NET472.Fixtures
{
	internal interface ITestRepo
	{
		Task<Result<Article>> GetArticleById(Guid articleId);
	}
}
