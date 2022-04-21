using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameGraphics : MonoBehaviour
{
    public static GameGraphics instance { private set; get; }

    SortedDictionary<System.UInt32, IndexData> indexes;
    SortedDictionary<System.UInt32, Texture2D> textures;
    
    [HideInInspector]
    public List<Sprite> sprites;

    [HideInInspector]
    public List<Animation> animations;

    struct IndexData
    {
        public System.UInt32 id;

        public System.Int16 startX;
        public System.Int16 startY;
        public System.Int16 width;
        public System.Int16 height;
        public float tileWidth;
        public float tileHeight;

        public List<System.UInt32> frames;
        public System.Int16 framesCount;
        public float speed;
    };

    BinaryReader br;
    System.UInt32 graphicsCount;

    float startTime, taskStartTime;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        indexes = new SortedDictionary<System.UInt32, IndexData>();

        TextAsset indexAsset = Resources.Load<TextAsset>("Index");

        if (indexAsset)
        {
            Stream s = new MemoryStream(indexAsset.bytes);
            br = new BinaryReader(s);
        }
        else
        {
            Debug.LogError($"indexAsset {indexAsset}");
            return;
        }

        startTime = Time.realtimeSinceStartup;

        taskStartTime = Time.realtimeSinceStartup;
        if (!LoadIndex())
            return;

        taskStartTime = Time.realtimeSinceStartup;
        if (!LoadTextures())
            return;

        /*
        taskStartTime = Time.realtimeSinceStartup;
        if (!LoadSprites())
            return;
        /*
        taskStartTime = Time.realtimeSinceStartup;
        if (!LoadAnimations())
            return;
        */

        MeasureOverallTime();
    }

    private void MeasureOverallTime()
    {
        float loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"Overall Load time: {loadTime.ToString("F2")}s.");
    }

    private bool LoadIndex()
    { 
        int fileVersion = br.ReadInt32();
        Debug.Log($"FileVersion: {fileVersion}");

        graphicsCount = br.ReadUInt32();
        Debug.Log($"graphicsCount: {graphicsCount}");

        System.UInt32 index = 0;

        while (index != graphicsCount)
        {
            IndexData dat = new IndexData();
            dat.frames = new List<System.UInt32>();

            index = br.ReadUInt32();

            dat.framesCount = br.ReadInt16();

            if (dat.framesCount > 1)
            {
                for (int j = 0; j < dat.framesCount; j++)
                {
                    System.UInt32 f = br.ReadUInt32();
                    dat.frames.Add(f);
                }

                dat.id = indexes[dat.frames[0]].id;
                dat.speed = br.ReadSingle();
                dat.startX = indexes[dat.frames[0]].startX;
                dat.startY = indexes[dat.frames[0]].startY;
                dat.width = indexes[dat.frames[0]].width;
                dat.height = indexes[dat.frames[0]].height;
                dat.tileWidth = indexes[dat.frames[0]].tileWidth;
                dat.tileHeight = indexes[dat.frames[0]].tileHeight;
            }
            else
            {
                dat.id = br.ReadUInt32();
                dat.startX = br.ReadInt16();
                dat.startY = br.ReadInt16();
                dat.width = br.ReadInt16();
                dat.height = br.ReadInt16();

                dat.tileWidth = dat.width / 32;
                dat.tileHeight = dat.height / 32;

                dat.frames.Add(index);
                dat.speed = 0.0f;
            }

            indexes.Add(index, dat);
            /*
			Debug.Log(
				$"Graphic Data: \n" +
				$"index: {index} \n " +
				$"id: {dat.id} \n " +
				$"startX: {dat.startX} \n " +
				$"startY: {dat.startY} \n " +
				$"width: {dat.width} \n " +
				$"height: {dat.height} \n " +
				$"tileWidth: {dat.tileWidth} \n " +
				$"tileHeight: {dat.tileHeight} \n " +
				$"frames: {dat.frames.Count} \n " +
				$"framesCount: {dat.framesCount} \n " +
				$"speed: {dat.speed}");
			*/
        }

        float loadTime = Time.realtimeSinceStartup - taskStartTime;
        Debug.Log($"Index Load time: {loadTime.ToString("F2")}s.");

        return true;
    }

    private bool LoadTextures()
    {
        textures = new SortedDictionary<System.UInt32, Texture2D>();

        foreach (KeyValuePair<System.UInt32, IndexData> indexData in indexes)
        {
            Texture2D texture2D = Resources.Load<Texture2D>("Images/" + indexData.Value.id.ToString());

            if (texture2D != null)
            {
                if (!textures.ContainsKey(indexData.Value.id))
                    textures.Add(indexData.Value.id, texture2D);
            }
            else
                Debug.LogWarning($"Unable to Load: Images/{indexData.Value.id}");
        }

        if (textures.Count < 1)
        {
            Debug.LogError("Error Loading Textures");
            return false;
        }
        else
        {
            float loadTime = Time.realtimeSinceStartup - taskStartTime;
            Debug.Log(
                $"Textures Count {textures.Count} \n" +
                $"Textures Load time: {loadTime.ToString("F2")}s.");
        }

        return true;
    }

    private bool LoadSprites()
    {
        sprites = new List<Sprite>();

        foreach(KeyValuePair<System.UInt32, IndexData> indexData in indexes)
        {
            Rect rect = new Rect(indexData.Value.startX, indexData.Value.startY, indexData.Value.width, indexData.Value.height);
            //Vector2 pivot = new Vector2(rect.width / 2f, rect.height / 2f);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            Sprite sprite = Sprite.Create(textures[indexData.Value.id], rect, pivot);

            if (sprite)
            {
                sprite.name = "Sprite_" + indexData.Value.id.ToString();
                sprites.Add(sprite);
            }
            else
                Debug.LogWarning(
                    $"Unable to create Sprite: " +
                    $"Images/{indexData.Value.id} " +
                    $"Rect: {rect.ToString()}");
        }

        if (sprites.Count < 1)
        {
            Debug.LogError("Error Creating Sprites");
            return false;
        }
        else
        {
            float loadTime = Time.realtimeSinceStartup - taskStartTime;
            Debug.Log(
                $"Sprites Count {sprites.Count} \n" +
                $"Sprites Creation time: {loadTime.ToString("F2")}s.");
        }

        return true;
    }

    private bool LoadAnimations()
    {
        animations = new List<Animation>();

        foreach (KeyValuePair<System.UInt32, IndexData> indexData in indexes)
        {
            if (indexData.Value.frames.Count < 2)
                continue;

            //Animation animation = new Animation();
            
        }

        if (animations.Count < 1)
        {
            Debug.LogWarning("No Animations Created");
            return false;
        }
        else
        {
            float loadTime = Time.realtimeSinceStartup - taskStartTime;
            Debug.Log(
                $"Animation Clips Count {animations.Count} \n" +
                $"Animation Clips Creation time: {loadTime.ToString("F2")}s.");
        }

        return true;
    }
}
