using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationManager;
using static EffectsManager.MovingObject;

public class EffectsManager : MonoBehaviour
{
    public List<MovingObject> movingObjects = new();
    readonly float movementThreshold = 1f;

    public List<AnimatingSpriteStruct> animatingSprites = new();

    private void Update()
    {
        HandleMovement();
        HandleSpriteAnimation();
    }

    void HandleMovement()
    {
        for (int i = 0; i < movingObjects.Count; i++)
        {
            MovingObject movingObject = movingObjects[i];

            Vector3 direction = (movingObject.destination.position - movingObject.objectMoving.position).normalized;
            movingObject.objectMoving.Translate(movingObject.speed * Time.deltaTime * direction);

            // Calculate the remaining distance to the destination
            float remainingDistance = Vector3.Distance(movingObject.objectMoving.position, movingObject.destination.position);

            // If the remaining distance is less than the arrival threshold, consider the object has reached its destination
            if (remainingDistance < movementThreshold)
            {
                movingObject.objectMoving.gameObject.SetActive(false);
                movingObject.objectMoving.position = movingObject.startingPosition;

                movingObject.callback();

                movingObjects.RemoveAt(i);
                i--;
            }
        }
    }

    void HandleSpriteAnimation()
    {
        for(int i = 0; i < animatingSprites.Count; i++)
        {
            AnimatingSpriteStruct anim = animatingSprites[i];

            string currentAnimationName = anim.objectToAnimate.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (currentAnimationName == GetIdleAnimationName())
            {
                animatingSprites.RemoveAt(i);
                i--;
            }
        }
    }

    public void TriggerCallback(int sourceId)
    {
        AnimatingSpriteStruct anim = animatingSprites.Find(a => a.objectToAnimate.gameObject.GetInstanceID() ==  sourceId);

        anim.callback();
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
        public Action callback;

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

    public struct AnimatingSpriteStruct
    {
        public Animator objectToAnimate;
        public Action callback; //This callback won't be called at the end of animation, but will be manually called by some animation
        public SpriteAnimation animation;

        public AnimatingSpriteStruct(Animator obj, SpriteAnimation animation, Action callback)
        {
            objectToAnimate = obj;
            this.callback = callback;
            this.animation = animation;
        }
    }
}
