using System.Collections.Generic;

//A helper class to store two objects as a pair
public class Tuple<T1, T2> {

    public T1 First { get; set; }
    public T2 Second { get; set; }

    //Constructor for tuple object
    public Tuple(T1 first, T2 second) {
        this.First = first;
        this.Second = second;
    }

    public override string ToString() {
        return string.Format("Tuple contains {0} and {1}", this.First, this.Second);
    }

    public override bool Equals(object obj) {
        if (!(obj is Tuple<T1, T2>)) {
            return false;
        }
        else {
            Tuple<T1, T2> other = (Tuple<T1, T2>)obj;
            return this.Equals(other);
        }
    }

    public bool Equals(Tuple<T1, T2> other) {
        return other.First.Equals(this.First) && other.Second.Equals(this.Second);
    }

    //This is required for generics
    public override int GetHashCode() {
        var hashCode = 43270662;
        hashCode = hashCode * -1521134295 + EqualityComparer<T1>.Default.GetHashCode(First);
        hashCode = hashCode * -1521134295 + EqualityComparer<T2>.Default.GetHashCode(Second);
        return hashCode;
    }
}
