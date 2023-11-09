namespace Expressions.Net.Conversion
{
	public sealed class ConversionResult<T>
	{
		public T Value { get; }
		public bool IsSuccess { get; }
		public string? Error { get; }

		private ConversionResult(T value, bool success, string? error)
		{
			Value = value;
			IsSuccess = success;
			Error = error;
		}

		public static ConversionResult<T> Success(T value) => new ConversionResult<T>(value, true, null);
		public static ConversionResult<T> Failure(string? errorMessage = null) => new ConversionResult<T>(default, false, errorMessage);
	}
}
