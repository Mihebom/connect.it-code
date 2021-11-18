using UnityEngine;

public static class CounterStore 
{
    public static Counter[] counters { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]private static void Initialize() => counters = Resources.LoadAll<Counter>("FranceBoardCounters/");


}
