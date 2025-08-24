using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class PokerSpriteManager : MonoBehaviour
{
    // 单例实例，确保全局唯一
    public static PokerSpriteManager Instance { get; private set; }

    // 用于存储所有加载的 Sprite，键为图片名称
    private Dictionary<string, Sprite> pokerSprites = new Dictionary<string, Sprite>();

    // Unity 生命周期方法，在对象创建时调用
    private void Awake()
    {
        // 实现单例模式，如果实例已存在则销毁当前对象
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // 确保场景切换时该对象不会被销毁
            DontDestroyOnLoad(this.gameObject);

            // 在游戏开始时立即加载所有扑克牌图片
            LoadAllPokerSprites();
        }
    }

    /// <summary>
    /// 从 Resources/PokersPNG 文件夹加载所有图片并生成 Sprite。
    /// </summary>
    private void LoadAllPokerSprites()
    {
        // 使用 Resources.LoadAll 加载指定路径下的所有 Texture2D
        // 注意：路径不包含 "Resources/"，也不包含文件扩展名
        Texture2D[] textures = Resources.LoadAll<Texture2D>("PokersPNG");

        if (textures.Length == 0)
        {
            Debug.LogError("在 Resources/PokersPNG 文件夹中没有找到任何图片！请确保图片已正确放置。");
            return;
        }

        // 遍历所有加载的 Texture2D
        foreach (Texture2D texture in textures)
        {
            // 创建一个全尺寸的 Sprite
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // 将 Sprite 存入哈希表，键为图片名称（不含 .png）
            pokerSprites[texture.name] = newSprite;
            Debug.Log($"成功加载扑克牌 Sprite: {texture.name}");
        }

        Debug.Log($"共加载了 {pokerSprites.Count} 张扑克牌图片。");
    }

    /// <summary>
    /// 公共方法，供其他脚本根据名称获取 Sprite。
    /// </summary>
    /// <param name="spriteName">扑克牌图片的名称（不含扩展名）</param>
    /// <returns>对应的 Sprite，如果不存在则返回 null。</returns>
    public Sprite GetPokerSprite(string spriteName)
    {
        if (pokerSprites.ContainsKey(spriteName))
        {
            return pokerSprites[spriteName];
        }

        Debug.LogError($"无法找到名为 '{spriteName}' 的扑克牌 Sprite。");
        return null;
    }

    public Sprite GetPokerSprite(Card card)
    {
        return GetPokerSprite(card.ToString());
    }
}