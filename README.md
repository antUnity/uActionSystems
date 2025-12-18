# uActionSystems

**uActionSystems** is a data-driven framework for the Unity Engine designed to handle complex action logic, requirement validation, and rule systems. Built on the **uGameData** indexing toolkit, it decouples gameplay logic from specific implementations, allowing for highly composable and designer-friendly systems.

### Dependencies
This toolkit utilizes and requires [uGameData](https://github.com/antUnity/uGameData) to be installed.

## Core Concepts

### 1. The Action System (`ActionSystem<TAction>`)
The `ActionSystem` is the central hub for managing gameplay logic. It maps specific actions—defined by a generic enum (`TAction`)—to `RuleAssets`. This allows you to change the requirements for an action (e.g., "Can I cast this spell?") directly in the Inspector without modifying C# code.

### 2. The Action Context (`IActionContext`)
Rules are never hard-coded to look at specific components. Instead, they query an `IActionContext`. This context tracks three potential sources of data for any given action:
* **Environment**: The global system or manager state.
* **Instigator**: The entity performing the action (e.g., a Player or Unit).
* **Subject**: The target of the action (e.g., a Chest, an Enemy, or an Item).

### 3. Data Querying (`IQueryable`)
For an entity to interact with the system, it must implement the `IQueryable` interface. This allows the context to resolve values using `uGameData` assets as keys:
```csharp
public T Query<T>(IGameDataBase data);
```

### 4. Rich Rule Results (`RuleResult`)
The `RuleResult` struct provides detailed feedback beyond a simple boolean:
* **Success State**: Evaluated via `if (result)`.
* **Failure Codes**: Specific reasons (e.g., `MinimumRequirementViolation`).
* **Violation Context**: References to the specific data asset and the actual value that caused the failure, making it easy to drive UI tooltips.

---

## Rule Types

The toolkit includes several built-in `RuleAsset` types (ScriptableObjects) that can be combined:

* **CompareToValue**: Compares a context value against a constant (e.g., `Gold >= 100`).
* **CompareToData**: Compares two values within the context (e.g., `Strength > TargetDefense`).
* **HasData**: Checks for the existence of a specific data entry or a boolean flag.
* **CompositeRule**: Combines rules using **AND** / **OR** logic.
* **GroupRule**: A registry of rules where success is returned if *all* rules in the group pass.

These rules can be created as scriptable objects (`RuleAssets`) or serialized within another scriptable object.

---

## Implementation Example

### 1. Defining a Data-Driven Entity
Implement `IQueryable` on your MonoBehaviours to allow the Action System to "read" your entity's stats.



```csharp
using UnityEngine;
using uGameData;
using uActionSystems;

public class UnitStats : MonoBehaviour, IQueryable
{
    [SerializeField] private GameDataValues<StatAsset, float> stats;

    public T Query<T>(IGameDataBase data)
    {
        // Resolve float stats from our internal registry
        if (typeof(T) == typeof(float) && stats.ContainsIndex(data))
            return (T)(object)stats[data];
        
        return default;
    }
}
```

### 2. Evaluating an Action
Trigger logic by passing the Instigator and Subject into your `ActionSystem`.

```csharp
using uActionSystems;
using uActionSystems.Rules;

public enum Actions { Attack, Unlock }

public void OnInteract(Unit player, Chest target)
{
    // The system automatically assembles the context using the provided queryables
    RuleResult result = actionSystem.EvaluateAction(Actions.Unlock, target, player);

    if (result)
    {
        target.Open();
    }
    else
    {
        // Provide specific feedback to the player using the rich result data
        Debug.Log($"Failed: {result.FailureCode} on {result.RequiredData.GetIndex()}");
    }
}
```

---

## Technical Features

### Operator Overloading
`RuleResult` supports logical operators, allowing you to compose complex code-based rules that still provide full failure context:
```csharp
// Returns the first failure context found, or success if both pass
RuleResult combined = ruleA.Evaluate(context) && ruleB.Evaluate(context);
```

### Decoupled Logic
Since `RuleAsset` is a ScriptableObject, you can create a "CanMove" rule once and apply it to Players, NPCs, and AI alike. As long as they implement `IQueryable`, the rules will work seamlessly.
