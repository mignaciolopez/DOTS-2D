using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphicsLoader : MonoBehaviour
{
    SortedDictionary<System.UInt32, Data> m_graphicsData;

    struct Data
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

    float startTime;

    private void Start()
    {
        m_graphicsData = new SortedDictionary<System.UInt32, Data>();

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
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(0f);

        int fileVersion = br.ReadInt32();
        Debug.Log($"FileVersion: {fileVersion}");

        System.UInt32 graphicsCount = br.ReadUInt32();
        Debug.Log($"graphicsCount: {graphicsCount}");

        System.UInt32 index = 0;

        while (index != graphicsCount)
        {
            Data dat = new Data();
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

                dat.id = m_graphicsData[dat.frames[0]].id;
                dat.speed = br.ReadSingle();
                dat.startX = m_graphicsData[dat.frames[0]].startX;
                dat.startY = m_graphicsData[dat.frames[0]].startY;
                dat.width = m_graphicsData[dat.frames[0]].width;
                dat.height = m_graphicsData[dat.frames[0]].height;
                dat.tileWidth = m_graphicsData[dat.frames[0]].tileWidth;
                dat.tileHeight = m_graphicsData[dat.frames[0]].tileHeight;
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

            m_graphicsData.Add(index, dat);
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

        float loadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"Index Load time: {loadTime.ToString("F2")}s.");
    }
}
