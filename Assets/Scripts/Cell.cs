using BaseScripts;
using UnityEngine;

public class Cell : CellBase
{
    // private void OnMouseEnter()
    // {
    //     Debug.Log("OnMouseEnter");
    //     Debug.Log(new Vector2(_column, _row));
    // }
    //
    // private void OnMouseExit()
    // {
    //     Debug.Log("OnMouseExit");
    //     Debug.Log(new Vector2(_column, _row));
    // }
    //
    // private void OnMouseUp()
    // {
    //     Debug.Log("OnMouseUp");
    //     Debug.Log(new Vector2(_column, _row));
    // }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        Debug.Log(new Vector2(Column, Row));
    }
}
