using System;
using LaeringslivCore;
using UnityEngine;

/// <summary>
/// Manager handling all swipe events
/// </summary>
public class Swipe : MonoBehaviour
{
    public static Swipe Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    GUIManager guiManager => GUIManager.Instance;


    private bool tap,
                 holdLeft,
                 holdRight,
                 swipeUp,
                 holdDown;

    private bool isDragging;

    private Vector2 startTouch,
                    swipeDelta;

    public bool Tap => tap;

    public Vector2 SwipeDelta => swipeDelta;

    public bool SwipeLeft => holdLeft;

    public bool SwipeRight => holdRight;

    public bool SwipeUp => swipeUp;

    public bool SwipeDown => holdDown;

    public float deadZoneMagnitude = 100f;

    /// <summary>
    /// Is there currently a card in transition?
    /// </summary>
    bool isSliding;

    /// <summary>
    /// Are we in a state that allows swiping
    /// </summary>
    bool swipingAllowed;


    public enum HoldDirection
    {
        Left,
        Right,
        Down,
        Up,
        None
    }

    /// <summary>
    /// The currently active hold direction
    /// </summary>
    HoldDirection currentHoldDirection = HoldDirection.None;

    /// <summary>
    /// The swipe text object we are currently holding
    /// </summary>
    private HoldDirection currentSwipeTextObject;

    private void Update()
    {
        if (swipingAllowed && !isSliding && GameManager.Instance.currentGameState == GameManager.GameState.Running)
        {
            tap = holdLeft = holdRight = swipeUp = holdDown = false;

            #region Standalone Inputs

            if (Input.GetMouseButtonDown(0))
            {
                // Mouse Down
                tap = true;
                isDragging = true;
                startTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                // Mouse Up
                Refresh();
            }

            #endregion

            #region Mobile Inputs

            if (Input.touches.Length > 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    // Finger down
                    isDragging = true;
                    tap = true;
                    startTouch = Input.touches[0].position;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    // Finger up
                    Refresh();
                }
            }

            #endregion

            // Calculate the distance
            swipeDelta = Vector2.zero;
            if (isDragging)
            {
                if (Input.touches.Length > 0)
                {
                    swipeDelta = Input.touches[0].position - startTouch;
                }
                else if (Input.GetMouseButton(0))
                {
                    swipeDelta = (Vector2) Input.mousePosition - startTouch;
                }
            }
            if (swipingAllowed)
            {
                // Did we cross the deadzone?
                if (swipeDelta.magnitude < deadZoneMagnitude)
                {
                    if (currentHoldDirection == HoldDirection.None)
                    {
                        float x = swipeDelta.x;
                        float y = swipeDelta.y;
                        Drag(x, y);
                    }
                    else if (swipeDelta.magnitude < deadZoneMagnitude)
                    {
                        // Gone back below deadzone threshold from a hold, release hold
                        ResetHold();
                    }
                }
                else if (swipeDelta.magnitude > deadZoneMagnitude && swipeDelta.y < Mathf.Abs(swipeDelta.x))
                {
                    if (currentHoldDirection == HoldDirection.None)
                    {
                        // Which direction?
                        float x = swipeDelta.x;
                        float y = swipeDelta.y;

                        //if (Mathf.Abs(x) > Mathf.Abs(y)) {
                        if (Mathf.Abs(x) > -y)
                        {
                            // Left or right
                            if (x < 0)
                            {
                                holdLeft = true;
                                Holdcard(HoldDirection.Left);
                            }
                            else
                            {
                                holdRight = true;
                                Holdcard(HoldDirection.Right);
                            }
                        }
                        else
                        {
                            if (y < 0)
                            {
                                holdDown = true;
                                Holdcard(HoldDirection.Down);
                            }
                            else
                            {
                                swipeUp = true;
                                Holdcard(HoldDirection.Up);

                                // Somethingsoemthing
                            }
                        }
                    }
                }
            }
            if ((holdLeft && currentHoldDirection != HoldDirection.Left) ||
                (holdRight && currentHoldDirection != HoldDirection.Right) ||
                (holdDown && currentHoldDirection != HoldDirection.Down))
            {
                Holdcard(currentHoldDirection);
            }
        }
    }

    /// <summary>
    /// Hold the card
    /// </summary>
    /// <param name="direction"></param>
    private void Holdcard(HoldDirection direction)
    {
        GameManager.Instance.CoachStartHoldEvent(direction);
        currentHoldDirection = direction;
        switch (direction)
        {
            case (HoldDirection.Left):
                guiManager.HoldCard(0);
                break;
            case (HoldDirection.Right):
                guiManager.HoldCard(1);
                break;
            case (HoldDirection.Down):
                guiManager.HoldCard(2);
                break;
            case (HoldDirection.Up):
                break;
            case (HoldDirection.None):
                guiManager.HoldCard(-1);
                break;
        }
    }

    /// <summary>
    /// Reset card back to original position
    /// </summary>
    private void ResetHold()
    {
        if (currentHoldDirection != HoldDirection.None) {
            GameManager.Instance.CoachStopHoldEvent(currentHoldDirection);
        }
        currentHoldDirection = HoldDirection.None;
    }

    /// <summary>
    ///     Swipe a card in given direction
    /// </summary>
    /// <param name="direction"> The direction to swipe </param>
    private void PerformSwipe(HoldDirection direction)
    {
        //PlayCardChoice playCardChoice =
        //    CardManager.Instance.GetCard(GameManager.CurrentCardID).GetSwipeEvents()[(int)direction];

        //PlayerManager.Instance.ChangeStats(playCardChoice.Score);

        GameEventManager.Instance.SwipeEvent(direction);
        isSliding = true;
        
        GameEventManager.Instance.HandleChoiceEventLogging((int)direction);
        //// Report to server that the player has made a choice
        //EventLog eventLog = new EventLog
        //{
        //    Time = DateTime.UtcNow,

        //    // TODO: Change when player profiles exist 
        //    PlayerID = 0,

        //    Event = EventLog.EventType.PlayerChoice,
            
        //    CardID = GameManager.CurrentCardID,

        //    Choice = (int) direction

        //};


        //Server.Instance.BundleEvent(eventLog);
        ////Server.LogEvent(eventLog);

        
        
        
    }

    /// <summary>
    /// Screen released, act accordingly (Perform swipe or reset card based on hold state)
    /// </summary>
    private void Refresh()
    {
        if (isDragging)
        {
            switch (currentHoldDirection)
            {
                case (HoldDirection.Left):
                case (HoldDirection.Right):
                case (HoldDirection.Down):
                    GameManager.Instance.CoachStopHoldEvent(currentHoldDirection);
                    if (GameManager.Instance.SwipeCoachReview()) {
                        PerformSwipe(currentHoldDirection);
                    } else {
                        guiManager.ResetCard();
                    }
                    
                    break;
                case (HoldDirection.Up):
                    break;
                case (HoldDirection.None):
                    guiManager.ResetCard();
                    break;
            }
        }

        currentHoldDirection = HoldDirection.None;
        startTouch = swipeDelta = Vector2.zero;
        isDragging = false;
    }

    public void SetSliding(bool _isSliding)
    {
        isSliding = _isSliding;
    }

    public void SetSwipingAllowed(bool _swipingAllowed)
    {
        swipingAllowed = _swipingAllowed;
    }


    public float GetDeadZoneThreshold()
    {
        return deadZoneMagnitude;
    }


    // Testing Lerp
    public void Drag(float deltaX, float deltaY)
    {
        guiManager.DragCard(deltaX, deltaY);
    }
}