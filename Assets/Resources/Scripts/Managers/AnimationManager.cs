using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager.MovingObject;

public class AnimationManager : MonoBehaviour
{
    public List<MovingObject> movingObjects = new();

    private void Update()
    {
        float arrivalThreshold = 1f; // Distance threshold to consider the object has reached its destination

        for (int i = 0; i < movingObjects.Count; i++)
        {
            MovingObject movingObject = movingObjects[i];

            Vector3 direction = (movingObject.destination.position - movingObject.objectMoving.position).normalized;
            movingObject.objectMoving.Translate(movingObject.speed * Time.deltaTime * direction);

            // Calculate the remaining distance to the destination
            float remainingDistance = Vector3.Distance(movingObject.objectMoving.position, movingObject.destination.position);

            // If the remaining distance is less than the arrival threshold, consider the object has reached its destination
            if (remainingDistance < arrivalThreshold)
            {
                movingObject.objectMoving.gameObject.SetActive(false);
                movingObject.objectMoving.position = movingObject.startingPosition;

                movingObject.callback();

                movingObjects.RemoveAt(i);
                i--;
            }
        }
    }


    public void StartMovement(Transform source, Transform destination, int speed, TypeOfObject type, Action callback)
    {
        movingObjects.Add(new(source, destination, type, speed, callback));
    }

    public struct MovingObject
    {
        public Transform objectMoving;
        public Vector3 startingPosition;
        public Transform destination;
        public int speed;
        public float timeElapsed;
        public TypeOfObject type;
        public System.Action callback;

        public MovingObject(Transform objectMoving, Transform destination, TypeOfObject type, int speed, Action callback)
        {
            this.objectMoving = objectMoving;
            startingPosition = objectMoving.position;
            this.destination = destination;
            this.speed = speed;
            timeElapsed = 0;
            this.type = type;
            this.callback = callback;
        }

        public enum TypeOfObject
        {
            CardDrawnPlayer,
            CardDrawnEnemy
        }
    }
}
