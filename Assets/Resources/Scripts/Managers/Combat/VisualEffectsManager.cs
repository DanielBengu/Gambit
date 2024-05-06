using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static AnimationManager;
using static VisualEffectsManager.MovingObject;

public class VisualEffectsManager : MonoBehaviour
{
    public List<MovingObject> movingObjects = new();
    readonly float movementThreshold = 1f;

    public List<AnimatingSpriteStruct> animatingSprites = new();

    public List<EffectsStruct> effects = new();

    public Dictionary<int, Action> dictionaryCallback = new();

    private void Update()
    {
        HandleMovement();
        HandleSpriteAnimation();
        HandleEffects();
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

            if (currentAnimationName == GetAnimationName(SpriteAnimation.UnitIdle))
            {
                anim.callback();
                if(animatingSprites.Contains(anim))
                    animatingSprites.Remove(anim);
                i--;
            }
        }
    }

    void HandleEffects()
    {
        for (int i = 0; i < effects.Count;i++)
        {
            EffectsStruct effect = effects[i];
            switch (effect.effect)
            {
                case Effects.FightStartup:
                    Image startupBlackoutImage = effect.obj.GetComponent<Image>();
                    Color currentColor = startupBlackoutImage.color;
                    currentColor.a -= Time.deltaTime * 0.5f; // Adjust the rate of transparency loss as needed
                    startupBlackoutImage.color = currentColor;
                    if (currentColor.a <= 0.1f)
                    {
                        effect.callback();

                        startupBlackoutImage.gameObject.SetActive(false);
                        effects.RemoveAt(i);
                        i--;
                    }
                    break;
                case Effects.MenuStartGame:
                    startupBlackoutImage = effect.obj.GetComponent<Image>();
                    Color currentColor2 = startupBlackoutImage.color;
                    currentColor2.a += Time.deltaTime * 1.2f; // Adjust the rate of transparency loss as needed
                    startupBlackoutImage.color = currentColor2;
                    if (currentColor2.a >= 1f)
                    {
                        effect.callback();
                        effects.RemoveAt(i);
                        i--;
                    }
                    break;
            }
        }
    }

    public void RemoveFromLists(GameObject obj)
    {
        Animator anim = obj.GetComponent<Animator>();
        movingObjects.RemoveAll(o => o.objectMoving == obj.transform);
        animatingSprites.RemoveAll(s => s.objectToAnimate == anim);
    }

    public void StartMovement(Transform source, Transform destination, int speed, TypeOfObject type, Action callback)
    {
        movingObjects.Add(new(source, destination, type, speed, callback));
    }

    public enum Effects
    {
        FightStartup,
        MenuStartGame
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
        public string animation;

        public AnimatingSpriteStruct(Animator obj, string animation, Action callback)
        {
            objectToAnimate = obj;
            this.callback = callback;
            this.animation = animation;
        }
    }

    public struct EffectsStruct
    {
        public Effects effect;
        public GameObject obj;
        public Action callback;

        public EffectsStruct(Effects effect, GameObject obj, Action action)
        {
            this.effect = effect;
            this.obj = obj;
            callback = action;
        }
    }
}
