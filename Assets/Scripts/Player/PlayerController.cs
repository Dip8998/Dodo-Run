using DodoRun.Main;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace DodoRun.Player
{
    public class PlayerController
    {
        private PlayerScriptableObject playerScriptableObject;
        private PlayerView playerView;
        private Vector3 firstTouchPosition;
        private Vector3 lastTouchPosition;
        private float dragDistance;
        private int currentLane = 0;
        

        public PlayerController(PlayerScriptableObject playerScriptableObject)
        {
            this.playerScriptableObject = playerScriptableObject;
            SetupView();
        }

        private void SetupView()
        {
            playerView = Object.Instantiate(playerScriptableObject.Player, playerScriptableObject.SpawnPosition, Quaternion.identity);
            playerView.SetController(this);
            
            dragDistance = Screen.height * 15 / 100;

            GameService.Instance.StartCoroutine(InvokeSpawn());
        }

        private IEnumerator InvokeSpawn()
        {
            yield return null;
            GameService.Instance.EventService.OnPlayerSpawned.InvokeEvent(playerView.transform);
        }

        public void UpdatePlayer()
        {
            HandleSwipeInputs();
            MoveToLane();
        }

        private void MoveToLane()
        {
            float targetX = currentLane * playerScriptableObject.LaneOffset;

            Vector3 newPosition = playerView.transform.position;

            newPosition.x = Mathf.Lerp(playerView.transform.position.x, targetX, Time.deltaTime * playerScriptableObject.SwipeSpeed);

            playerView.transform.position = newPosition;
        }

        private void HandleSwipeInputs()
        {
            if (Input.touchCount != 1)
                return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    firstTouchPosition = touch.position;
                    lastTouchPosition = touch.position;
                    break;
                case TouchPhase.Moved:
                    lastTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                    lastTouchPosition = touch.position;
                    DetectSwipeInputs();
                    break;
            }
        }

        private void DetectSwipeInputs()
        {
            Vector2 differnce = lastTouchPosition - firstTouchPosition;

            bool isSwipe = Mathf.Abs(differnce.x) > dragDistance || Mathf.Abs(differnce.y) > dragDistance;

            if(isSwipe)
            {
                if (Mathf.Abs(differnce.x) > Mathf.Abs(differnce.y))
                {
                    if (lastTouchPosition.x > firstTouchPosition.x)
                    {
                        MoveRight();
                    }
                    else
                    {
                        MoveLeft();
                    }
                }
                else
                {
                    if (lastTouchPosition.y > firstTouchPosition.y)
                    {
                        MoveUp();
                    }
                    else
                    {
                        MoveDown();
                    }
                }
            }
        }

        private void MoveRight()
        {
            if(currentLane < 1)
            {
                currentLane++;
            }
        }

        private void MoveLeft()
        {
            if (currentLane > -1)
                currentLane--;  
        }

        private void MoveUp()
        {

        }

        private void MoveDown()
        {
            
        }
    }
}
