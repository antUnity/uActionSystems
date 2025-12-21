using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.ActionSystems.Rules
{
    public interface IRuleMetric<TResult>
    {
        TResult Calculate(IActionContext context);
    }

    [Serializable]
    [CreateAssetMenu(fileName = FILE_NAME.RULE_METRIC, menuName = MENU_PATH.RULE_METRIC)]
    public class RuleMetric : GameDataAsset<uint>, IRuleMetric<float>   
    {
        [SerializeReference, SubclassSelector] private IRuleMetric<float> metric;

        public float Calculate(IActionContext context) => metric.Calculate(context);
    }
}