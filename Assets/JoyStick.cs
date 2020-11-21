using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform pad;
    public Transform player;
    Vector3 moveForward;
    Vector3 moveRotate;
    public float moveSpeed;
    public float rotateSpeed;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        transform.localPosition= Vector2.ClampMagnitude(eventData.position - (Vector2)pad.position,pad.rect.width*0.5f);
    
        moveForward = new Vector3(0,0,transform.localPosition.y).normalized;
        moveRotate = new Vector3(0,transform.localPosition.x,0).normalized;


    }
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        moveForward = Vector3.zero;
        moveRotate = Vector3.zero;
        StopCoroutine("PlayerMove");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine("PlayerMove");
    }
    IEnumerator PlayerMove()
    {
        while(true)
        {
            player.Translate(moveForward * moveSpeed*Time.deltaTime);

            if(Mathf.Abs(transform.localPosition.x)> pad.rect.width*0.3f)
            player.Rotate(moveRotate *rotateSpeed*Time.deltaTime);

            yield return null;
        }
    }
}