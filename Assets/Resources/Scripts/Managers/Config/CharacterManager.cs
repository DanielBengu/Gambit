using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static FightManager;


static class CharacterManager
{
    public static RuntimeAnimatorController LoadAnimator(string characterName)
    {
        return Resources.Load<RuntimeAnimatorController>($"Sprites/Characters/{characterName}/Components/{characterName}");
    }

    public static Sprite LoadSprite(string characterName)
    {
        return Resources.Load<Sprite>($"Sprites/Characters/{characterName}/{characterName}");
    }

    public static GameObject LoadCharacter(string characterName)
    {
        return Resources.Load<GameObject>($"Prefabs/Characters/{characterName}/{characterName}");
    }

    public static void ResetCharacter(GameObject characterObject, VisualEffectsManager visualEffectsManager)
    {
        visualEffectsManager.RemoveFromLists(characterObject);

        characterObject.GetComponent<Animator>().runtimeAnimatorController = null;
        characterObject.GetComponent<SpriteRenderer>().sprite = null;
    }
}
