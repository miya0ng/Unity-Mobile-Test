using UnityEngine;

/// <summary> 
/// Responsible for moving the player automatically and 
/// reciving input. 
/// </summary> 
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    /// <summary> 
    /// A reference to the Rigidbody component 
    /// </summary> 
    private Rigidbody rb;

    [Tooltip("How fast the ball moves left/right")]
    public float dodgeSpeed = 5;

    [Tooltip("How fast the ball moves forwards automatically")]
    [Range(0, 10)]
    public float rollSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        // Get access to our Rigidbody component 
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if( Input.touchCount == 1  )
        {
            Touch touch = Input.GetTouch(0);
            var ray = Camera.main.ScreenPointToRay(touch.position);
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                //hit.collider.gameObject.SetActive(false);
                hit.collider.SendMessage("OnTouched", SendMessageOptions.RequireReceiver); //모든컴퍼넌트 순회하면서 OnTouched 함수 호출함. 없으면 에러메세지.

                //hit.collider.BroadcastMessage("OnTouched", SendMessageOptions.RequireReceiver); // 자식 순회하면서 함수호출
            }
        }
    }

    /// <summary>
    /// FixedUpdate is called at a fixed framerate and is a prime place to put
    /// Anything based on time.
    /// </summary>
    void FixedUpdate()
    {
#if UNITY_EDITOR
        // Check if we're moving to the side 
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;
#endif

#if UNITY_ANDROID || UNITY_IOS
        //horizontalSpeed = Input.acceleration.x * dodgeSpeed;
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            var viewportPos = Camera.main.ScreenToViewportPoint(touch.position);
            horizontalSpeed = viewportPos.x < 0.5 ? -1f : 1f;
            horizontalSpeed *= dodgeSpeed;
            Input.GetTouch(0);
            touch.position = Vector3.zero;
        }
#endif
        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }

    private void OnTouched()
    {
        Destroy(gameObject);
    }
}