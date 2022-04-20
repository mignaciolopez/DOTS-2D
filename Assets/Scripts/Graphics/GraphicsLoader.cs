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

	bool error = false;
	bool done = false;

	BinaryReader br;
	System.UInt32 index = 0;
	System.UInt32 graphicsCount;

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
			error = true;
			return;
		}

		int fileVersion = br.ReadInt32();
		Debug.Log($"[GraphicsLoader] FileVersion: {fileVersion}");

		graphicsCount = br.ReadUInt32();
		Debug.Log($"[GraphicsLoader] graphicsCount: {graphicsCount}");
	}

    private void Update()
    {
		if (error || done)
			return;

		if (index != graphicsCount)
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
			/*Debug.Log("===========================");
			Debug.Log("Graphic Data:");
			Debug.Log($"index: {index}");
			Debug.Log($"ID: {dat.id}");
			Debug.Log($"startX: {dat.startX}");
			Debug.Log($"startY: {dat.startY}");
			Debug.Log($"width: {dat.width}");
			Debug.Log($"height: {dat.height}");
			Debug.Log($"tileWidth: {dat.tileWidth}");
			Debug.Log($"tileHeight: {dat.tileHeight}");
			Debug.Log($"frames: {dat.frames.Count}");
			Debug.Log($"framesCount: {dat.framesCount}");
			Debug.Log($"speed: {dat.speed}");
			Debug.Log("===========================");
			Debug.Log("");*/
		}
		else
        {
			done = true;
			Destroy(gameObject);
		}
	}
}
