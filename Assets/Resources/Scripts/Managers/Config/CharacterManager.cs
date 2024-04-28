using UnityEngine;


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
}
