
namespace CardsAndCarnage {
    public delegate void AbilityAction<T1, T2, T3, T4>(ref T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void PassiveAction<T1, T2>(T1 arg1, T2 arg2);
    public delegate bool CardEffect<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
