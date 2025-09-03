using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public static class Consts
{
    public const float HANDCARDS_SCALE = 0.8816f;
    public const float PLAYEDCARDS_SCALE = 0.5593f;

    public const float HANDCARDS_OFFSET = 0.57f;
    public const float PLAYEDCARDS_OFFSET = 0.36f;

    public const float Y_HANDCARDS = -2f;
    public const float X_PLAYEDCARDS = 0f;
    public const float Y_PLAYEDCARDS = 0f;

    public const float X_LEFT = -5;
    public const float X_RIGHT = 5;
    public const float X_CENTER = 0;

    public const float Y_TOP = 2.5f;
    public const float Y_MIDDLE = 0.5f;
    public const float Y_BOTTOM = -1.5f;
    public static Vector2 BOTTOM_POS = new Vector2(X_CENTER, Y_BOTTOM);
    public static Vector2 RIGHT_POS = new Vector2(X_RIGHT, Y_MIDDLE);
    public static Vector2 LEFT_POS = new Vector2(X_LEFT, Y_MIDDLE);
    public static Vector2 HANDCARDS_POS = new Vector2(X_CENTER, -3.6f);
    public static Color GREY = new(0.8f, 0.8f, 0.8f, 1f);
    public static Color RED = new(166f, 46f, 18f, 1f);
    public static float GetXByProportionFromRight(float proportion)
    {
        return X_RIGHT - (X_RIGHT - X_LEFT) * proportion;
    }

    public static Vector2[] GetPositionsByPlayerCount(int playerCount)
    {
        if (playerCount == 1)
        {
            return new Vector2[]
            {
                new Vector2(X_CENTER, Y_BOTTOM)
            };
        }
        if (playerCount == 2)
        {
            return new Vector2[]
            {
                new Vector2(X_CENTER, Y_BOTTOM),
                new Vector2(X_CENTER, Y_TOP)
            };
        }
        if (playerCount == 3)
        {
            return new Vector2[]
            {
                new Vector2(X_CENTER, Y_BOTTOM),
                new Vector2(GetXByProportionFromRight(1f/3), Y_TOP),
                new Vector2(GetXByProportionFromRight(2f/3), Y_TOP),
            };
        }
        Vector2[] positions = new Vector2[playerCount];
        positions[0] = new Vector2(X_CENTER, Y_BOTTOM);
        positions[1] = new Vector2(X_RIGHT, Y_MIDDLE);
        positions[playerCount - 1] = new Vector2(X_LEFT, Y_MIDDLE);
        for (int i = 2; i < playerCount - 1; i++)
        {
            positions[i] = new Vector2(GetXByProportionFromRight((float)(i - 1) / (playerCount - 2)), Y_TOP);
        }
        return positions;

    }

}
