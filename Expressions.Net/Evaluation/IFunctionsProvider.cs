namespace Expressions.Net.Evaluation
{
	public partial interface IFunctionsProvider
	{
		LookupFunctionInfoResult LookupFunctionInfo(string fuctionName, params IValueType[] argTypes);
	}
}
