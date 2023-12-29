namespace JustResult;

public readonly record struct Result
{
	private readonly List<Error>? _errors = null;

	private Result(Error error)
	{
		IsError = true;
		_errors = [error];
	}

	private Result(List<Error> errors)
	{
		IsError = true;
		_errors = new(errors);
	}

	private Result(Exception exception)
	{
		IsError = true;
		_errors = exception.MapErrors();
	}

	public bool IsError { get; } = false;

	public bool IsSuccess => !IsError;

	public static implicit operator Result(Error error) => new(error);

	public static implicit operator Result(Error[] errors) => new([.. errors]);

	public static implicit operator Result(List<Error> errors) => new(errors);

	public static implicit operator Result(Exception exception) => new(exception);

	public static implicit operator Result(int _) => new();

	public static implicit operator bool(Result result) => !result.IsError;

	public static explicit operator List<Error>?(Result result) => result.IsError ? result._errors : default;

	public void Switch(Action onSuccess, Action<List<Error>> onError)
	{
		if (IsError)
		{
			onError(_errors!);
			return;
		}

		onSuccess();
	}

	public async Task SwitchAsync(Func<Task> onSuccess, Func<List<Error>, Task> onError)
	{
		if (IsError)
		{
			await onError(_errors!);
			return;
		}

		await onSuccess();
	}

	public List<Error> Match(Func<List<Error>, List<Error>> onError) => IsError ? _errors! : [];

	public async Task<List<Error>> MatchAsyn(Func<List<Error>, Task<List<Error>>> failure)
		=> IsError ? await failure(_errors!) : [];
}
