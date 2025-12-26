using System;
using UnityEngine;

namespace antunity.GameSystems.Rules
{
    public enum LogicalOperation { AND, OR }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_COMPOSITE, menuName = MENU_PATH.RULE_COMPOSITE)]
    public class CompositeRule : Rule
    {
        [SerializeField] private bool invert;

        [SerializeField] private Rule rule1;

        [SerializeField] private LogicalOperation operation;

        [SerializeField] private Rule rule2;

        public override RuleResult Evaluate(IGameContext context)
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