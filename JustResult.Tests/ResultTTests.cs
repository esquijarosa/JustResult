using JustResult.Tests.Fixtures;
using Moq;

namespace JustResult.Tests;

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

	[Fact]
	public void BooleanFalse_SuccessResult()
	{
		// Act
		var result = FalseResult();

		// Assert
		Assert.True(result);
		Assert.False(result.Value);

		/// While this is a valid implementation for a result, if you need to return
		/// a <see cref="bool"/> user the non generic <see cref="Result"/> instead.
		static Result<bool> FalseResult()
		{
			return false;
		}
	}

	[Fact]
	public void ErrorResult_ThrowsExceptionWhenAccessValue()
	{
		// Act
		var result = FailResult();

		// Assert
		Assert.False(result);
		Assert.Throws<InvalidOperationException>(() => result.Value);

		static Result<string> FailResult()
		{
			return new Error("Generic.Error", "Some error ocurred");
		}
	}
}
