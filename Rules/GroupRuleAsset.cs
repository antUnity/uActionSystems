using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.ActionSystems.Rules
{
    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_GROUP, menuName = MENU_PATH.RULE_GROUP)]
    public class GroupRuleAsset : RuleAsset
    {
        [SerializeField] private GameDataRegistry<RuleAsset> rules;

        public override RuleResult Evaluate(IActionContext context)
        {
            foreach (var rule in rules)
            {
                var result = rule.Evaluate(context);
                if (result.IsSuccess)
                    return result;
            }

            return RuleResult.Success();
        }
    }
}