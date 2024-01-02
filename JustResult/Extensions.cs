using System.Reflection;

namespace JustResult
{
	internal static class Extensions
	{
		/// <summary>
		/// Maps the <see cref="Exception"/> hierarchy to a <see cref="List{T}"/> of <see cref="Error"/>s.
		/// </summary>
		/// <param name="exception">The <see cref="Exception"/> to map.</param>
		/// <returns><see cref="List{T}"/> of <see cref="Error"/>s contained in the <see cref="Exception"/>
		/// and its <see cref="Exception.InnerException"/>s.</returns>
		public static List<Error> MapErrors(this Exception exception)
		{
			List<Error> result = [exception.MapError()];

			while (exception.InnerException is not null)
			{
				exception = exception.InnerException;
				result.Add(exception.MapError());
			}

			return result;
		}

		/// <summary>
		/// Maps an <see cref="Exception"/> to its corresponding <see cref="Error"/> using
		/// the <see cref="Exception.TargetSite"/> property as <see cref="Error.Code"/>, with
		/// the format '[class_name].[method_name]'. In case of <see cref="Exception.TargetSite"/>
		/// being <see langword="null"/>, the <see cref="Exception.GetType"/>.Name is used.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static Error MapError(this Exception exception)
		{
			return new Error(exception.TargetSite?.FullName() ?? exception.GetType().Name, exception.Message);
		}

		private static string? FullName(this MethodBase? method)
		{
			if (method is null)
			{
				return null;
			}

			return $"{method!.DeclaringType?.Name ?? string.Empty}.{method.Name}";
		}
	}
}
