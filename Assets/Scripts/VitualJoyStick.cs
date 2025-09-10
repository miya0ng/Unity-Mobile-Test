using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.EventSystems;

public class VitualJoyStick : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RectTransform background;
    public RectTransform handle;

    private float radius = 0;
    public Vector2 Input {  get; private set; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var touchPosition = eventData.position;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle( //@
            background, touchPosition, eventData.enterEventCamera, out Vector2 position))
        {
            var delta = position;
            position = Vector2.ClampMagnitude(position, radius); // 반경 클램프 , radius보다 작으면 그대로사용, 크면 radius로
            handle.anchoredPosition = position;
            
            Input = delta / radius;
        }
        throw new System.NotImplementedException();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //AndroidJoystick
        radius = background.rect.width * 0.5f;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input);
    }
}
