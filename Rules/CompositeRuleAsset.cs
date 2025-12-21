using System;
using UnityEngine;

namespace antunity.ActionSystems.Rules
{
    public enum LogicalOperation { AND, OR }

    [Serializable]
    public class CompositeRuleAsset : RuleAsset
    {
        [SerializeField] private bool invert;

        [SerializeField] private RuleAsset rule1;

        [SerializeField] private LogicalOperation operation;

        [SerializeField] private RuleAsset rule2;

        public override RuleResult Evaluate(IActionContext context)
        {
            RuleResult result;

            switch (operation)
            {
                case LogicalOperation.AND:
                    result = rule1.Evaluate(context) && rule2.Evaluate(context);
                    break;
                case LogicalOperation.OR:
                    result = rule1.Evaluate(context) || rule2.Evaluate(context);
                    break;
                default:
                    result = RuleResult.UnknownFailure();
                    break;
            }

            if (invert)
                result = !result;

            return result;
        }
    }
}