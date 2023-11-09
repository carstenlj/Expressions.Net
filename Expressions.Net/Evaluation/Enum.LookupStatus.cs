namespace Expressions.Net.Evaluation
{
	public enum LookupStatus
	{
		ExistsSingle,
		ExistAmbigous,
		FunctionDoesNotExist,
		FunctionDoesNotSupportArgCount,
		FunctionDoesNotSupportArgTypes
	}
}
