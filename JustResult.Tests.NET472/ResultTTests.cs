using JustResult.Tests.NET472.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JustResult.Tests.NET472
{
	public class ResultTTests
	{
		/// While this is a valid implementation for a result, if you need to return
		/// a <see cref="bool"/> user the non generic <see cref="Result"/> instead.
		private static Result<bool> FalseResult()
		{
			return false;
		}

		private static Result<string> FailResult()
		{
			return new Error("Generic.Error", "Some error ocurred");
		}

		private static Result<string> FailResultMultipleErrors()
		{
			return new Error[]
			{
				new Error("Generic.Error", "Some error ocurred"),
				new Error("Generic.Error2", "Some other error ocurred")
			};
		}

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
			Assert.IsType<Article>((Article) articleResult);
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
			Assert.IsType<List<Error>>((List<Error>) articleResult);
			Assert.Equal("NotFound", ((List<Error>) articleResult)[0].Code);
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
			Assert.IsType<List<Error>>((List<Error>) articleResult);
			Assert.Equal("InvalidOperationException", ((List<Error>) articleResult)[0].Code);
			Assert.Equal("The article does not exists.", ((List<Error>) articleResult)[0].Description);
		}

		[Fact]
		public void BooleanFalse_SuccessResult()
		{
			// Act
			var result = FalseResult();

			// Assert
			Assert.True(result);
			Assert.False(result.Value);
		}

		[Fact]
		public void ErrorResult_ThrowsExceptionWhenAccessValue()
		{
			// Act
			var result = FailResult();

			// Assert
			Assert.False(result);
			Assert.Throws<InvalidOperationException>(() => result.Value);
		}

		[Fact]
		public void MultipleErrorsResult_CanReadAllErros()
		{
			// Act
			var result = FailResultMultipleErrors();

			// Assert
			Assert.False(result);
			Assert.IsType<List<Error>>((List<Error>) result);
			Assert.Equal("Generic.Error", ((List<Error>) result)[0].Code);
			Assert.Equal("Generic.Error2", ((List<Error>) result)[1].Code);
			Assert.Throws<InvalidOperationException>(() => result.Value);
		}
	}
}
