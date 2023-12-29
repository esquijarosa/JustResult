using System.Reflection;

namespace JustResult
{
	internal static class Extensions
	{
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
