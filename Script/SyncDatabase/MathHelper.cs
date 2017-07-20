using UnityEngine;
using System.Collections;

/// <summary>
/// Passcode.
/// Encode & Decode.
/// </summary>
public class MathHelper : MonoBehaviour 
{
    public Color body, engine, binder;
    public Vector3 rotate;
    public int score;

    /// <summary>
    /// Base 
    /// </summary>
    public int basev = 36;

    /// <summary>
    /// Make passcode by data.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="bin"></param>
    /// <param name="engine"></param>
    /// <param name="rot"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    public string MakePass ( Color body, Color bin, Color engine, Vector3 rot, int score )
    {
        string s =  Encoding ( score, basev ) +
                    Encoding ( body,basev ) + 
                    Encoding ( bin,basev ) + 
                    Encoding ( engine,basev ) + 
                    Encoding ( rot, basev );
        
        int hex = s.GetHashCode();
        s = hex.ToString().Substring(hex.ToString().Length-1,1)+s;
        return s;
    }

    /// <summary>
    /// Receive passcode for decode.
    /// </summary>
    /// <param name="pass"></param>
    /// <returns></returns>
    public bool FromPass ( string pass )
    {
        string data = pass.Substring ( 1, pass.Length - 1 );
        string hex = data.GetHashCode ( ).ToString ( );
        string first = pass.Substring ( 0, 1 );

        if ( first.Equals ( hex.Substring ( hex.Length - 1, 1 ) ) )
        {
            string bodyS = data.Substring ( 5, 4 );
            string binS = data.Substring ( 9, 4 );
            string engineS = data.Substring ( 13, 4 );
            string rotS = data.Substring ( 17, 6 );
            string scoreS = data.Substring ( 0, 5 );
            

            Color body = Decoding(bodyS,basev);
            Color bin = Decoding(binS,basev);
            Color engine = Decoding(engineS,basev);
            Vector3 rot = AngleIntToVec(Parse(rotS,basev));
            int score = Parse ( scoreS, basev );

            this.body = body;
            this.engine = engine;
            this.binder = bin;
            this.rotate = rot;
            this.score = score;

            return true;
        }
        else
        {
            Debug.Log("Error");
            return false;
        }
    }

    /// <summary>
    /// Check passcode.
    /// </summary>
    /// <param name="pass"></param>
    /// <returns></returns>
    public bool CheckPass ( string pass )
    {
        string data = pass.Substring(1,pass.Length-1);
        string hex = data.GetHashCode().ToString();
        string first = pass.Substring(0,1);

        if (first.Equals(hex.Substring(hex.Length - 1, 1)))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Encode color part.
    /// </summary>
    /// <param name="c"></param>
    /// <param name="basev"></param>
    /// <returns></returns>
    public static string Encoding ( Color c, int basev )
    {
        string s = "";

        int cval = ColorToInt( c );
        cval = cval / 16; //lose data
        s = Convert ( cval, basev );
        s = FillNulls(s, 4);

        //print ( "Encod : " + s );
        return s; // 00000
    }

    /// <summary>
    /// Encode score part.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="basev"></param>
    /// <returns></returns>
    public static string Encoding ( int i, int basev )
    {
        string s = "";
        s = Convert ( i, basev );
        s = FillNulls ( s, 5 );
        return s;
    }

    /// <summary>
    /// Encode position part.
    /// </summary>
    /// <param name="rot"></param>
    /// <param name="basev"></param>
    /// <returns></returns>
    public static string Encoding ( Vector3 rot, int basev )
    {
        string s = "";
        s = Convert ( AngleVecToInt ( rot ), basev );
        s = FillNulls ( s, 6 );
        return s;
    }

    /// <summary>
    /// Decode color part.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="basev"></param>
    /// <returns></returns>
    public static Color32 Decoding ( string s, int basev )
    {
        int nr = Parse(s,basev);
        nr = nr * 16;
        
        Color32 cc = ColorToInt ( nr );

        return cc;
    }

    /// <summary>
    /// Convert string (char) back to number.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="basev"></param>
    /// <returns></returns>
    public static int Parse(string i, int basev)
    {
        int back = 0;
        i = Reverse(i);

        for ( int index = 0 ; index < i.Length ; index++ )
        {
            char c = System.Convert.ToChar ( i.Substring ( index, 1 ) );
            int num = c;
            if ( c <= '9' )
            {
                num = num - '0';
            }
            else if ( c > '9' )
            {
                num = num - 55;
            }

            num = num * (int)(Mathf.Pow ( basev, index ));

            back += num;
        }

        return back;
    }

    /// <summary>
    /// Fill 0 into null part.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static string FillNulls(string s, int max){
        string back = "";
        if ( s.Length != max )
        {
                int loop = max - s.Length;
                for ( int ind = 0; ind < loop; ind++ )
                {
                    back += '0';
                }
        }
         return back+s;
    }

    /// <summary>
    /// Convert number to string(char).
    /// </summary>
    /// <param name="i"></param>
    /// <param name="basev"></param>
    /// <returns></returns>
    public static string Convert(int i, int basev)
    {
        if (basev != 0) {

            string back = "";
            int current = i;

            for (int pos = 0; current > 0; pos++)
            {
                int leftover = current % basev;
                current = current - leftover;
                current = current / basev;
                char c = (char)(leftover+'0');
                if(c > '9')
                    c = (char)(c + 7);
                if(c > 'Z')
                    c = (char)(c + 6);
                back += c;
            }
            
            
            back = Reverse(back);
            return back;
        }
        else
        {
            return "Error";
        }
    }

    /// <summary>
    /// Reverse string pack.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string Reverse( string s )
    {
        char[] charArray = s.ToCharArray();
        System.Array.Reverse( charArray );
        return new string( charArray );
    }
	
    /// <summary>
    /// Convert int to color.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static Color32 ColorToInt(int i)
    {
        int r = i & 0xFF0000;
        r = r >> 16;
        int g = i & 0x00FF00;
        g = g >> 8;
        int b = i & 0x0000FF;
        return new Color32((byte)r,(byte)g,(byte)b,0xFF);
    }

    /// <summary>
    /// Convert color to int.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static int ColorToInt(Color32 c)
    {
        int back = 0;
        back = back | c.r;
        back = back << 8;  // >> 16
        back = back | c.g;
        back = back << 8; // >> 8
        back = back | c.b;

        return back;
    }

    /// <summary>
    /// Convert vector3 to int.
    /// </summary>
    /// <param name="angles"></param>
    /// <returns></returns>
    public static int AngleVecToInt(Vector3 angles)
    {
        int x = (int)(angles.x*2);
        int y = (int)(angles.y*2);
        int z = (int)(angles.z*2);
        //print ( x +", "+ y+", "+ z );
        //print ( (x<<20)|(y<<10)|z );
        return (x<<20)|(y<<10)|z;
    }

    /// <summary>
    /// Convert int to vector3.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static Vector3 AngleIntToVec(int i)
    {
        int x = (int)(i & 0x3FF00000);
        x = x >> 20;
        int y = (int)(i & 0xFFC00);
        y = y >> 10;
        int z = (int)(i & 0x3FF);

        return new Vector3(((float)x)/2,((float)y)/2,((float)z)/2);
    }

}
