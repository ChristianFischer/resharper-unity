using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.CallGraph;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using static JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.BurstCodeAnalysis.BurstCodeAnalysisUtil;

namespace JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.BurstCodeAnalysis.Analyzers
{
    [SolutionComponent]
    public class BurstInvocationExpressionAnalyzer : BurstProblemAnalyzerBase<IInvocationExpression>
    {
        protected override bool CheckAndAnalyze(IInvocationExpression invocationExpression,
            IHighlightingConsumer consumer)
        {
            //algorithm:
            //if conditional qualifier is open type
            //    return
            //    REASON: burst allows to instantiate generics only with structures. if it is instantiated with class -
            //    it would be highlighted where generics called. if it is instantiated with structure - 2 possible outcomes.
            //    if there are no generic constraints - only object methods can be called, they are handled in BurstReferenceExpressionAnalyzer.
            //    If there is some interface constraint - then it is ok, Burst allows to call interface methods if they are implemented with struct.
            //    CallGraph would have edges to interface through constraints, not instantiation.
            //else
            //    if condional qualifier is class
            //        if conditiinal qualifier is class instance
            //            return
            //            REASON: I will highlight refereceExpression, which would have ReadAccess. Burst can't access 
            //            managed objects. 
            //        else
            //            if function non static
            //                then it would be highlighted as error
            //            else
            //                ok. burst allows invoking static functions.
            //    else
            //        if conditional qualifier is struct instance
            //            if function is virtual/abstract/override
            //                HIGHLIGHT: invocation expressio WITHOUT parameters
            //                ALSO: struct's method are implicitle sealed!
            //                REASON: burst does not support any invocations that use virtual table.
            //                IMPORTANT: type parameters and open types may have some virtual invocations,
            //                but burst generic system allows only structures/primitives to instatiate generics. 
            //                Structure/primitives DOES support some virtual methods from System.Object.
            //            else
            //                it is ok. burst allows any method from structure
            //        else 
            //            if function non static
            //                then it would be highlighted as error
            //            else
            //                ok. burst alows invoking static functions.
            var invokedMethod = CallGraphUtil.GetCallee(invocationExpression) as IMethod;

            if (invokedMethod == null)
                return false;

            if (IsDebugLog(invokedMethod))
            {
                var argumentList = invocationExpression.ArgumentList.Arguments;

                if (argumentList.Count != 1)
                    return false;
                
                var argument = argumentList[0];
                
                if (IsBurstPermittedString(argument.Expression?.Type()))
                    return false;
                
                consumer?.AddHighlighting(new BurstDebugLogInvalidArgumentWarning(argument.Expression));
                        
                return true;

            }

            if (IsStringFormat(invokedMethod))
            {
                var argumentList = invocationExpression.ArgumentList.Arguments;

                var isWarningPlaced = BurstStringLiteralOwnerAnalyzer.CheckAndAnalyze(invocationExpression,
                    new BurstManagedStringWarning(invocationExpression), consumer);

                if (isWarningPlaced)
                    return true;

                if (argumentList.Count == 0) 
                    return false;
                
                var firstArgument = argumentList[0];
                var cSharpLiteralExpression = firstArgument.Expression as ICSharpLiteralExpression;

                if (cSharpLiteralExpression != null && cSharpLiteralExpression.Literal.GetTokenType().IsStringLiteral)
                    return false;
                
                consumer?.AddHighlighting(new BurstDebugLogInvalidArgumentWarning(firstArgument.Expression));
                return true;

            }

            if (IsObjectMethodInvocation(invocationExpression))
            {
                consumer?.AddHighlighting(new BurstAccessingManagedMethodWarning(invocationExpression,
                    invokedMethod.ShortName, invokedMethod.GetContainingType()?.ShortName));

                return true;
            }

            if (IsReturnValueBurstProhibited(invokedMethod) ||
                HasBurstProhibitedArguments(invocationExpression.ArgumentList))
            {
                consumer?.AddHighlighting(new BurstFunctionSignatureContainsManagedTypesWarning(invocationExpression,
                    invokedMethod.ShortName));

                return true;
            }

            return false;
        }
    }
}