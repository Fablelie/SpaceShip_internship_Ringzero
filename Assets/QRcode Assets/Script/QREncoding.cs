
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
/// Encode qr by player data.
/// </summary>
public class QREncoding : MonoBehaviour
{
    /// <summary>
    /// RawImage for render qr output.
    /// </summary>
    public RawImage qr_output;

    private UsingCon _using;

    /// <summary>
    /// Draw qr code on enable.
    /// </summary>
    void OnEnable () 
    {
        _using = GameObject.FindObjectOfType<UsingCon>();
        ZXing.QrCode.QRCodeWriter QRWriter = new ZXing.QrCode.QRCodeWriter();
        BitMatrix encoded = QRWriter.encode( _using.profile.passCode, ZXing.BarcodeFormat.QR_CODE, 512, 512 );
		Texture2D tex = new Texture2D(512,512,TextureFormat.RGBA32,false);
		Color[] pixels = tex.GetPixels();
		int k = 0;
		
        for(int j = 0; j < 512; j++ ) 
        {
			ZXing.Common.BitArray row = new ZXing.Common.BitArray(512);
			row = encoded.getRow(j,null);
			int[] intRow = row.Array;
			for( int i = intRow.Length-1; i >= 0; i-- ) {
				int thirtyTwoPixels = intRow[i];
				for(int b = 31; b >= 0; b--) {
					int pixel = ( (thirtyTwoPixels >> b) & 1 );
					if( pixel == 0 ) {
						pixels[k] = Color.white;
					}
					else {
						pixels[k] = Color.black;
					}
					k++;
				}
			}
		}

        print ( "PassCode for draw QR : " + _using.profile.passCode );

		tex.SetPixels( pixels );
		tex.Apply();
		qr_output.material.mainTexture = tex;
    }
	
}
