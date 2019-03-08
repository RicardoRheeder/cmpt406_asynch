
namespace CardsAndCarnage {
    public delegate void AbilityAction<T1, T2, T3>(ref T1 arg1, T2 arg2, T3 arg3); //required to make the delegate functions work with a reference parameter
    public delegate void PassiveAction<T1, T2>(T1 arg1, T2 arg2);
}
