
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Threading;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;
//using com.google.zxing.qrcode;

/// <summary>
/// Decoding qr code.
/// </summary>
public class WebcamDecoding : MonoBehaviour 
{
    /// <summary>
    /// RawImage for render webcam texture.
    /// </summary>
    public RawImage webcam;

    private PlayerPrefLogin _playerPrefLogin;
    private WebCamTexture _camTexture;
    private Color32[] _c;
	private sbyte[] _d;
	private int _W, _H, _WxH;
	private int _x, _y, _z;

	void Start () 
    { 
        _camTexture = new WebCamTexture();
		OnEnable();
        webcam.material.mainTexture = _camTexture;

        StartCoroutine ( DecodeQR ( ) );
	}

    void Update () 
    {
        _c = _camTexture.GetPixels32 ( );
    }

    void OnEnable () 
    {
		if(_camTexture != null) 
        {
			_camTexture.Play ( );
			_W = _camTexture.width;
			_H = _camTexture.height;
			_WxH = _W * _H;
		}
	}

    /// <summary>
    /// Check color pixel by pixel in qr code.
    /// </summary>
    /// <returns></returns>
    IEnumerator DecodeQR ( )
    {
        yield return new WaitForEndOfFrame ( );
	    try 
        {
		    _d = new sbyte[_WxH];
		    _z = 0;
		    for(_y = _H - 1; _y >= 0; _y--) 
			    for(_x = 0; _x < _W; _x++) 
				    _d[_z++] = (sbyte)(((int)_c[_y * _W + _x].r) << 16 | ((int)_c[_y * _W + _x].g) << 8 | ((int)_c[_y * _W + _x].b));
            
            //string s = new com.google.zxing.qrcode.QRCodeReader().decode(_d, _W, _H).Text;
            string s = "";
            _camTexture.Stop ( );
            _playerPrefLogin.QRDecoding ( s );
	    }
	    catch 
        {
	    }
	    
        StartCoroutine ( DecodeQR ( ) );
    }

}
