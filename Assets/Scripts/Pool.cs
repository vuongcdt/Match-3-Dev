using System.Collections.Generic;
using GameControllers;
using UnityEngine;

public class Pool : Singleton<Pool>
{
    [SerializeField]private Cell cell;
    private Stack<Cell> _pool = new();

    public Cell Rent()
    {
        return _pool.Count > 0 ? _pool.Pop() : Instantiate(cell, Vector3.zero, Quaternion.identity);
    }

    public void Return(Cell cell)
    {
        _pool.Push(cell);
    }
}