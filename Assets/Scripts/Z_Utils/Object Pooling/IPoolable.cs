using UnityEngine;

public delegate void UnspawnDelegate<T> (T obj);

public interface IPoolable<T> where T : Component {
	UnspawnDelegate<T> Unspawn { get; set; }
}
