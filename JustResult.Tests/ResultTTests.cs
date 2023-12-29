using JustResult.Tests.Fixtures;
using Moq;

namespace JustResult.Tests
{
	public class ResultTTests
	{
		[Fact]
		public async Task Success_Result()
		{
			// Arrange
			var repoMoq = new Mock<ITestRepo>();

			repoMoq
				.Setup(r => r.GetArticleById(It.IsAny<Guid>()))
				.ReturnsAsync(new Article { Id = Guid.NewGuid(), Title = "Test Article", Description = "Test description." });

			// Act
			var articleResult = await repoMoq.Object.GetArticleById(Guid.NewGuid());

			// Assert
			Assert.True(articleResult);
			Assert.IsType<Article>((Article) articleResult!);
		}

		[Fact]
		public async Task Fail_Result()
		{
			// Arrange
			var repoMoq = new Mock<ITestRepo>();

			repoMoq
				.Setup(r => r.GetArticleById(It.IsAny<Guid>()))
				.ReturnsAsync(new Error("NotFound", "The article does not exists."));

			// Act
			var articleResult = await repoMoq.Object.GetArticleById(Guid.NewGuid());

			// Assert
			Assert.False(articleResult);
			Assert.IsType<List<Error>>((List<Error>) articleResult!);
			Assert.Equal("NotFound", ((List<Error>) articleResult!)[0].Code);
		}

		[Fact]
		public async Task FailException_Result()
		{
			// Arrange
			var repoMoq = new Mock<ITestRepo>();

			repoMoq
				.Setup(r => r.GetArticleById(It.IsAny<Guid>()))
				.ReturnsAsync(new InvalidOperationException("The article does not exists."));

			// Act
			var articleResult = await repoMoq.Object.GetArticleById(Guid.NewGuid());

			// Assert
			Assert.False(articleResult);
			Assert.IsType<List<Error>>((List<Error>) articleResult!);
			Assert.Equal("InvalidOperationException", ((List<Error>) articleResult!)[0].Code);
			Assert.Equal("The article does not exists.", ((List<Error>) articleResult!)[0].Description);
		}
	}
}
