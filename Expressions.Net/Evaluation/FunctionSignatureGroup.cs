using System.Collections.Generic;
using System.Reflection;

namespace Expressions.Net.Evaluation
{

    public class FunctionSignatureGroup
    {
        public MethodInfo MethodInfo { get; }
        public IList<FunctionSignature> Signatures { get; } = new List<FunctionSignature>();

        public FunctionSignatureGroup(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }
    }

}
