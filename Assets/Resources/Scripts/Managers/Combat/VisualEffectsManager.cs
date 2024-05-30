using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AnimationManager;
using static VisualEffectsManager.MovingObject;
using Random = UnityEngine.Random;

public class VisualEffectsManager : MonoBehaviour
{
    #region Shake Ground variables

    static readonly float SHAKE_MAGNITUDE = 1.2f;
    static readonly float SHAKE_DURATION = 0.1f;
    float shake_time_elapsed = 0;

    #endregion

    #region Add Gold Variables

    static readonly int REWARD_MOVE_SPEED = 2;
    static readonly int UPDATE_SPEED = 2;
    int startingGoldAmount = 0;
    #endregion

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

            if (currentAnimationName == GetAnimationName(SpriteAnimation.Idle))
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
                case Effects.LightenBlackScreen:
                    ChangeBlackScreenOpacity(false, 0.5f, 0.1f, effect, ref i);
                    break;
                case Effects.DarkenBlackScreen:
                    ChangeBlackScreenOpacity(true, 0.5f, 0.8f, effect, ref i);
                    break;
                case Effects.MenuStartGame:
                    ChangeBlackScreenOpacity(true, 1.2f, 1f, effect, ref i);
                    break;
                case Effects.ShakeFightGround:
                    ShakeGround(effect);
                    break;
                case Effects.AddGoldReward:
                    HandleGoldRewardEffect(effect, ref i);
                    break;
                case Effects.UpdateGoldUI:
                    HandleGoldUIUpdate(effect, ref i);
                    break;
            }
        }
    }

    void ChangeBlackScreenOpacity(bool makeScreenDarker, float scale, float finalOpacity, EffectsStruct effect, ref int i)
    {
        int operatorEffect = makeScreenDarker ? 1 : -1;
        Image startupBlackoutImage = effect.obj.GetComponent<Image>();
        Color currentColor2 = startupBlackoutImage.color;
        currentColor2.a += operatorEffect * Time.deltaTime * scale;
        startupBlackoutImage.color = currentColor2;

        bool isCompleted = false;
        
        if((makeScreenDarker && currentColor2.a >= finalOpacity) || (!makeScreenDarker) && currentColor2.a <= finalOpacity)
            isCompleted = true;

        if(isCompleted)
        {
            effects.Remove(effect);
            foreach (var callback in effect.callback)
                callback();
            i--;
        }
    }

    void ShakeGround(EffectsStruct effect)
    {
        Vector3 originalPosition = (Vector3)effect.parameters[0];

        while (shake_time_elapsed < SHAKE_DURATION)
        {
            float x = originalPosition.x + Random.Range(-0.5f, 0.5f) * SHAKE_MAGNITUDE;
            float y = originalPosition.y + Random.Range(-1f, 1f) * SHAKE_MAGNITUDE;

            effect.obj.transform.position = new Vector3(x, y, effect.obj.transform.position.z);

            shake_time_elapsed += Time.deltaTime;

            return;
        }

        shake_time_elapsed = 0f;
        effects.Remove(effect);
        foreach (var callback in effect.callback)
            callback();
    }

    void HandleGoldRewardEffect(EffectsStruct effect, ref int i)
    {
        GameObject effectObject = effect.obj;
        TextMeshProUGUI text = (TextMeshProUGUI)effect.parameters[1];
        Vector3 targetPosition = (Vector3)effect.parameters[2];
        Vector3 startPosition = (Vector3)effect.parameters[3];

        float totalDistance = Vector3.Distance(startPosition, targetPosition);

        float currentDistance = Vector3.Distance(startPosition, effectObject.transform.position);

        float step = REWARD_MOVE_SPEED * Time.deltaTime;
        effectObject.transform.position = Vector3.MoveTowards(effectObject.transform.position, targetPosition, step);

        // Fade out the text halfway through the movement
        if (currentDistance >= totalDistance / 2)
        {
            float fadeT = (currentDistance - totalDistance / 2) / (totalDistance / 2);
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1f, 0f, fadeT));
        }

        if (effectObject.transform.position == targetPosition)
        {
            effects.Remove(effect);
            foreach (var callback in effect.callback)
                callback();
            i--;
        }
    }

    void HandleGoldUIUpdate(EffectsStruct effect, ref int i)
    {
        TextMeshProUGUI text = (TextMeshProUGUI)effect.parameters[0];
        GameUIManager gameUIManager = (GameUIManager)effect.parameters[1];
        int playerGoldAmountTarget = (int)effect.parameters[2];

        int currentGoldAmountDisplayed = int.Parse(text.text);

        if (startingGoldAmount == 0)
            startingGoldAmount = currentGoldAmountDisplayed;

        int targetGoldAmount = playerGoldAmountTarget;

        // Update the displayed gold amount incrementally
        int newGoldAmount = Mathf.CeilToInt(Mathf.Lerp(currentGoldAmountDisplayed, targetGoldAmount, Time.deltaTime * UPDATE_SPEED));

        // Update the UI
        gameUIManager.UpdateGoldAmount(newGoldAmount);

        // Check if the displayed amount has reached or surpassed the target amount
        if (newGoldAmount >= targetGoldAmount)
        {
            // Ensure the final displayed amount is exactly the target amount
            gameUIManager.UpdateGoldAmount(targetGoldAmount);

            startingGoldAmount = 0;

            effects.Remove(effect);
            foreach (var callback in effect.callback)
                callback();
            i--;
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
        LightenBlackScreen,
        DarkenBlackScreen,
        MenuStartGame,
        ShakeFightGround,
        AddGoldReward,
        UpdateGoldUI
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
        public List<Action> callback;
        public object[] parameters;

        public EffectsStruct(Effects effect, GameObject obj, List<Action> action, object[] param)
        {
            this.effect = effect;
            this.obj = obj;
            callback = action;
            parameters = param;
        }
    }
}
