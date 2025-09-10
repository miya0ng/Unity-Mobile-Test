using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary> 
/// Responsible for moving the player automatically and 
/// reciving input. 
/// </summary> 
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public VitualJoyStick virtualJoyStick;

    private const float Min = 0.3f;
    private const float Max = 3f;
    public float minSwipeDistance = 0.25f;

    public float maxSwipeTime = 0.25f;

    private float minSwipeDistancePixels;

    private int fingerId = -1;

    private Vector2 fingerTouchStartPosition;

    private float fingerTouchStartTime;

    public float sweepDistance = 1f;
    public float zoomInOutMaxDistance = 2f;
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
#if DEF_DEV
        Debug.unityLogger.logEnabled = false;
#endif
        // Get access to our Rigidbody component 
        rb = GetComponent<Rigidbody>();
        minSwipeDistancePixels = minSwipeDistance * Screen.dpi; // minSwipeDistance: 인치, Screen.dpi: 1인치에 몇픽셀인지 변환됨, 고정된 수라서 매번곱할필요x
    }

    private void Update()
    {
        


        //foreach (Touch touch in Input.touches)
        //{
        //    //손가락 하나만 추적함
        //    switch (touch.phase)
        //    {
        //        case TouchPhase.Began:
        //            if (fingerId == -1) // fingerId는 양수 안들어오기때문에, fingerId 없을때 -1로 // touch 시작한 시간,
        //            {
        //                fingerId = touch.fingerId;
        //                fingerTouchStartPosition = touch.position;
        //                fingerTouchStartTime = Time.time;
        //            }
        //            break;

        //        case TouchPhase.Ended:
        //        case TouchPhase.Moved:

        //        case TouchPhase.Canceled: // 터치가 끝난시간
        //            if( fingerId == touch.fingerId )
        //            {
        //                fingerId = -1;
        //                float distance = Vector2.Distance(touch.position, fingerTouchStartPosition);
        //                float time = Time.time - fingerTouchStartTime;
        //                if( distance < minSwipeDistancePixels && time < maxSwipeTime) // 스와이프 만족하는 조건
        //                {
        //                    Vector2 dir = touch.position - fingerTouchStartPosition; //방향
        //                    dir.Normalize();

        //                   Vector3 direction3 = dir.x > 0f ? Vector3.right : Vector3.left;
        //                    if(!rb.SweepTest(direction3, out RaycastHit hit,sweepDistance ))
        //                    {
        //                        rb.MovePosition(rb.position + direction3 * sweepDistance);
        //                    }
        //                    Debug.Log(dir.x < 0 ? "left " : "right");
        //                }

        //                fingerId = -1;
        //                fingerTouchStartPosition = Vector2.zero;
        //                fingerTouchStartTime = 0f;
        //            }
        //            break;

        //        case TouchPhase.Stationary:
        //            break;
        //    }
        //}

        //움직임체크, 움직일 경우 멀어지고 있는지, 가까워지는지 확인해야함.
        //touch 구조체에 전 프레임 포지션 있음.
        //줌인아웃에 따라 공 스케일을 늘리고 줄이기
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 preTouch1 = touch1.position - touch1.deltaPosition; 
            Vector2 preTouch2 = touch2.position - touch2.deltaPosition; 
            var preDistance = Vector2.Distance( preTouch1, preTouch2 );
            var curDistance = Vector2.Distance(touch1.position, touch2.position);

            float delta = (curDistance - preDistance) / Screen.dpi; // 인치로 변환
            // update안에 있기때문에, 프레임속도에 따라 scale값 계산
            delta *= Time.deltaTime;

            var current = transform.localScale.x;
            current += delta;
            current = Mathf.Clamp(current, Min, Max);
            transform.localScale = Vector3.one * current;
        }

        //
        //if (Input.touchCount == 1)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    var ray = Camera.main.ScreenPointToRay(touch.position);
        //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
        //    {
        //        //hit.collider.gameObject.SetActive(false);
        //        hit.collider.SendMessage("OnTouched", SendMessageOptions.RequireReceiver); //모든컴퍼넌트 순회하면서 OnTouched 함수 호출함. 없으면 에러메세지.

        //        //hit.collider.BroadcastMessage("OnTouched", SendMessageOptions.RequireReceiver); // 자식 순회하면서 함수호출
        //    }
        //}
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
        //if (Input.touchCount == 1)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    var viewportPos = Camera.main.ScreenToViewportPoint(touch.position);
        //    horizontalSpeed = viewportPos.x < 0.5 ? -1f : 1f;
        //    horizontalSpeed *= dodgeSpeed;
        //    Input.GetTouch(0);
        //    touch.position = Vector3.zero;
        //}
#endif
        horizontalSpeed = dodgeSpeed * virtualJoyStick.Input.x;
        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }

    private void OnTouched()
    {
        Destroy(gameObject);
    }
}