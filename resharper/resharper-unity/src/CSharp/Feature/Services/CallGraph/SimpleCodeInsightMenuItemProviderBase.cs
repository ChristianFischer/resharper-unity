using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.Application.Threading;
using JetBrains.Application.UI.Controls.BulbMenu.Items;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Resources;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.CallGraph;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.CallGraphStage;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.ContextSystem;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Unity.CSharp.Feature.Services.CallGraph
{
    public abstract class SimpleCodeInsightMenuItemProviderBase : ICallGraphCodeInsightMenuItemProvider
    {
        private readonly ISolution mySolution;
        private readonly SolutionAnalysisConfiguration mySolutionAnalysisConfiguration;

        protected SimpleCodeInsightMenuItemProviderBase(ISolution solution)
        {
            mySolution = solution;
            mySolutionAnalysisConfiguration = solution.GetComponent<SolutionAnalysisConfiguration>();
        }

        public IEnumerable<BulbMenuItem> GetMenuItems(IMethodDeclaration methodDeclaration, ITextControl textControl, IReadOnlyCallGraphContext context)
        {
            methodDeclaration.GetPsiServices().Locks.AssertReadAccessAllowed();
            
            if (!CheckCallGraph(methodDeclaration, context))
                return EmptyList<BulbMenuItem>.Enumerable;
            
            var bulbActions = GetActions(methodDeclaration);
            var result = new LocalList<BulbMenuItem>();
            
            foreach (var bulb in bulbActions)
            {
                var item = UnityCallGraphUtil.BulbActionToMenuItem(bulb, textControl, mySolution, BulbThemedIcons.ContextAction.Id);
                
                result.Add(item);
            }

            return result.ResultingList();
        }

        protected virtual bool CheckCallGraph([NotNull] IMethodDeclaration methodDeclaration, IReadOnlyCallGraphContext context)
        {
            return UnityCallGraphUtil.IsCallGraphReady(mySolutionAnalysisConfiguration);
        }

        protected abstract IEnumerable<IBulbAction> GetActions([NotNull] IMethodDeclaration methodDeclaration);
    }
}