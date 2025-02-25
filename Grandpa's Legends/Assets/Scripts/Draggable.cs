using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Draggable : MonoBehaviour{
    public int custoMana;
    private Collider2D col;
    private Vector3 startDragPosition;
    private void Start(){
        col = GetComponent<Collider2D>();
    }
    private void OnMouseDown(){
        startDragPosition = transform.position;
        transform.position = GetMousePositionInWorldSpace();
    }
    private void OnMouseDrag(){
        transform.position = GetMousePositionInWorldSpace();
    }
    private void OnMouseUp(){
        col.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        col.enabled = true;
        if(hitCollider != null && hitCollider.TryGetComponent(out ISlotAliado slotAliado)){
            slotAliado.onCardDrop(this);
        }else{
            transform.position = startDragPosition;
        }
    }
    public Vector3 GetMousePositionInWorldSpace(){
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }
}

