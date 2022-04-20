using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphicsLoader : MonoBehaviour
{
    [SerializeField] SpriteRenderer someSpriteRenderer;

    //public static GameGraphics gameGraphics { private set; get }

    SortedDictionary<System.UInt32, IndexData> indexes;
    SortedDictionary<System.UInt32, Texture2D> textures;
    List<Sprite> sprites;

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
        StartCoroutine(LoadIndex());
    }

    private IEnumerator LoadIndex()
    {
        yield return new WaitForSeconds(0f);

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

        taskStartTime = Time.realtimeSinceStartup;
        StartCoroutine(LoadTextures());
    }

    private IEnumerator LoadTextures()
    {
        yield return new WaitForSeconds(0f);

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
            Debug.LogError("Error Loading Textures");
        else
        {
            float loadTime = Time.realtimeSinceStartup - taskStartTime;
            Debug.Log(
                $"Textures Count {textures.Count} \n" +
                $"Textures Load time: {loadTime.ToString("F2")}s.");
        }

        taskStartTime = Time.realtimeSinceStartup;
        StartCoroutine(LoadSprites());
    }

    private IEnumerator LoadSprites()
    {
        yield return new WaitForSeconds(0f);

        sprites = new List<Sprite>();

        foreach(KeyValuePair<System.UInt32, IndexData> indexData in indexes)
        {
            Rect rect = new Rect(indexData.Value.startX, indexData.Value.startY, indexData.Value.width, indexData.Value.height);
            //Vector2 pivot = new Vector2(rect.width / 2f, rect.height / 2f);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            Sprite sprite = Sprite.Create(textures[indexData.Value.id], rect, pivot);

            if (sprite)
                sprites.Add(sprite);
            else
                Debug.LogWarning(
                    $"Unable to create Sprite: " +
                    $"Images/{indexData.Value.id} " +
                    $"Rect: {rect.ToString()}");
        }

        float loadTime;

        if (sprites.Count < 1)
            Debug.LogError("Error Creating Sprites");
        else
        {
            loadTime = Time.realtimeSinceStartup - taskStartTime;
            Debug.Log(
                $"Sprites Count {textures.Count} \n" +
                $"Sprites Creation time: {loadTime.ToString("F2")}s.");
        }

        loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"Overall Load time: {loadTime.ToString("F2")}s.");

        someSpriteRenderer.sprite = sprites[300];
    }
}
