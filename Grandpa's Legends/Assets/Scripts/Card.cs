using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Card : MonoBehaviour{
    private Collider2D col;
    public Vector3 startDragPosition;
    public Transform startParent; // Guarda o pai original
    public int custoMana;
    private void Start(){
        col = GetComponent<Collider2D>();
        startParent = transform.parent; // Armazena o pai inicial (Cartas)
    }

    private void OnMouseDown(){
        startDragPosition = transform.position;

        // Se a carta está em um slot, remova a relação antes de arrastar
        if (transform.parent != null){ 
            transform.SetParent(startParent); // Retorna a "Cartas" antes de mover
        }
    }

    private void OnMouseDrag(){
        transform.position = GetMousePositionInWorldSpace();
    }

    private void OnMouseUp(){
        col.enabled = false;
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position);
        col.enabled = true;

        if(hitCollider != null && hitCollider.TryGetComponent(out ICardDropArea cardDropArea)){
            cardDropArea.onCardDrop(this);
        } else {
            transform.position = startDragPosition; // Volta para a posição inicial
            transform.SetParent(startParent); // Mantém a organização na hierarquia
        }
    }

    public Vector3 GetMousePositionInWorldSpace(){
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f;
        return p;
    }
}
