using UnityEngine;
using U3DC.UI.Extension.Common;
using System.Collections;
using System.IO;

public class Test : MonoBehaviour
{
    private string _path = Path.Combine(Application.streamingAssetsPath, "Gif/Sloar_System.gif");
    private GifPlayer _gif;
	void Start ()
	{
	    _gif = gameObject.AddComponent<GifPlayer>();
	    _gif.SetGifPath(_path);
	}

}
