using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use in game object name GameController.
/// Execute spawn comet & supply. 
/// Store score, rotation, energy, Area
/// Get & Set value above.
/// Connect value with UI.
/// </summary>
public class GameController : MonoBehaviour, IGameFacade
{
    /// <summary>
    /// Value score.
    /// </summary>
    public int scorePoint;

    /// <summary>
    /// UI text score.
    /// </summary>
    public Text scoreText;

    /// <summary>
    /// Value energy.
    /// </summary>
    public float energyPoint;

    /// <summary>
    /// Energy slider bar.
    /// </summary>
    public Slider energyBar;

    /// <summary>
    /// Energy color bar.
    /// </summary>
    public Image energyFill;

    /// <summary>
    /// Enum area.
    /// </summary>
    public Area area;

    /// <summary>
    /// UI text area.
    /// </summary>
    public Text areaText;

    /// <summary>
    /// Rotation value.
    /// </summary>
    public Vector3 rotation;

    /// <summary>
    /// Rotation x,y,z UI text.
    /// </summary>
    public Text rotationX;
    public Text rotationY;
    public Text rotationZ;

    /// <summary>
    /// Script comet & supply. 
    /// </summary>
    private RotateComet _comet;

    private UsingCon _using;

    /// <summary>
    /// Collision detection value.
    /// </summary>
    private bool _crash;

    private bool _gamePause;

    /// <summary>
    /// Area Enum
    /// </summary>
    public enum Area
    {
        SKYBLUE = 0,
        RED = 1,
        PINK = 2,
        DARKBULE = 3,
        BLUE = 4,
        GREEN = 5,
    }

    /// <summary>
    /// canvas grounp for set alpha.
    /// </summary>
    public CanvasGroup canvasGroup;

    /// <summary>
    /// panel backgound of the option.
    /// </summary>
    public RectTransform panel;

    /// <summary>
    /// option button.
    /// </summary>
    public GameObject option;

    //public GameObject panalCurrentCode;
    //public Text currentCode;

    /// <summary>
    /// close button in panel.
    /// </summary>
    public GameObject close;

    /// <summary>
    /// control speed when useAnimate.
    /// </summary>
    public float speed;

    /// <summary>
    /// use scale change scale panel.
    /// </summary>
    public bool useScale;

    /// <summary>
    /// y is position, scaleY is scale in axis y.
    /// </summary>
    public float y, scaleY;

    public Text debugText;

    /// <summary>
    /// scroll down for read this class.
    /// </summary>
    public ColorProperties colorProp;

    public GameObject panalCurrentCode;
    public InputField textPasscode;

    public GameObject panalCurrentQRCode;

    private bool _clickButton;

    private Material _mat;
    private string _shaderColorValue;
    private string _newLine;
    private int _countLine;

    private bool _isOpenOption;

    public const float DVINE = 2;
    public const int MAX_VALUE_COLOR = 255;
    public const int MAX_POSITION_ON_SCREEN = 700;

    
    public const int WIDTH = 480;
    public const int HEIGHT = 800;

    void Awake ( )
    {
#if (UNITY_STANDALONE)
        Screen.SetResolution ( WIDTH, HEIGHT, false );
#endif
    }

    /// <summary>
    /// Show current passcode.
    /// </summary>
    public void ViewPasscode ( )
    {
        print("afsdklfjas;");
        if ( textPasscode != null )
        {   
            if ( _using.profile.passCode != "" )
                textPasscode.text = _using.profile.passCode;
            else
                textPasscode.text = "Save first!!";
        }
        panalCurrentCode.SetActive ( true );
    }

    /// <summary>
    /// Set disble panel passcode.
    /// </summary>
    public void ClosePasscode ( )
    {
        panalCurrentCode.SetActive ( false );
    }

    /// <summary>
    /// Show current qr code.
    /// </summary>
    public void ViewQRCode ( )
    {
        if ( _using.profile.passCode != "" )
            panalCurrentQRCode.SetActive ( true );
    }

    /// <summary>
    /// Disble qr panel.
    /// </summary>
    public void CloseQRCode ( )
    {
        panalCurrentQRCode.SetActive ( false );
    }


	void Start () 
    {
        scorePoint = 0;
        scoreText.text = "" + scorePoint;
        _using = GameObject.FindObjectOfType<UsingCon>();

        _using.login.Load ( );

         _newLine = System.Environment.NewLine;

        option.GetComponent<Button>().onClick.AddListener(() => Pause());
        close.GetComponent<Button>().onClick.AddListener(() => UnPause());

        // Listener button you can read it in unity api. (it's unity event)
        colorProp.buttonEngine.onClick.AddListener(() => OnClickButton("_ColorG"));
        colorProp.buttonBinder.onClick.AddListener(() => OnClickButton("_ColorB"));
        colorProp.buttonBody.onClick.AddListener(() => OnClickButton("_ColorR"));

        // Listener slider you can read it in unity api. (it's unity event)
        colorProp.R.onValueChanged.AddListener(delegate { OnValueChanged(); });
        colorProp.G.onValueChanged.AddListener(delegate { OnValueChanged(); });
        colorProp.B.onValueChanged.AddListener(delegate { OnValueChanged(); });

        //canvasGroup = panel.GetComponent<CanvasGroup>();

        // set ship material to _mat.
        _mat = colorProp.ship.material;

        SetColorButton("_ColorG", new Color(_mat.GetColor("_ColorG").r, _mat.GetColor("_ColorG").g, _mat.GetColor("_ColorG").b));

        SetColorButton("_ColorB", new Color(_mat.GetColor("_ColorB").r, _mat.GetColor("_ColorB").g, _mat.GetColor("_ColorB").b));

        SetColorButton("_ColorR", new Color(_mat.GetColor("_ColorR").r, _mat.GetColor("_ColorR").g, _mat.GetColor("_ColorR").b));

        OnClickButton("_ColorR");

	}

    void Update ()
    {
        // Change color energy bar. 
        if (energyPoint > 50f)
        {
            float f = (energyPoint - 50) / (100 - 50);
            Color c = Color.Lerp(Color.yellow, Color.red, f);
            energyFill.color = c;
        }
        else if (energyPoint > 1f)
        {
            float f = (energyPoint) / (100 - 50);
            Color c = Color.Lerp(Color.green, Color.yellow, f);
            energyFill.color = c;
        }
        //else if (energyPoint <= 0)
        //{
        //    energyFill.color = Color.clear;

        //    // do something "GameOver Event"
        //}

        // Decrease energy bar.
        if (energyPoint > 0)
            if(!Input.GetMouseButton(0) || _crash)
                energyPoint -= 200 * Time.deltaTime;
        else if (energyPoint <= 0)
            energyPoint += 100 * Time.deltaTime;

        // Limit energy value.
        energyPoint = Mathf.Clamp(energyPoint, 0, 100);
        energyBar.value = energyPoint;

        // Limit score value.
        scorePoint = Mathf.Clamp(scorePoint, 0, int.MaxValue);

        // Show score value.
        scoreText.text = "" + scorePoint;

         // when you use scale it will do this. ( change scale)
        //if (useScale)
        //{
        //    //panel.localScale = new Vector3(1, scaleY, 0);

        //    if (!_gamePause && !_isOpenOption)
        //        panel.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");
        //        //scaleY -= Time.deltaTime * speed;

        //    if ( _gamePause && _isOpenOption )
        //        panel.transform.gameObject.GetComponent<Animator>().SetTrigger("Open");
        //    //scaleY += Time.deltaTime * speed;

        //    //scaleY = Mathf.Clamp(scaleY, 0, 1);
        //}

    }

    	
    /// <summary>
    /// Set game pause.
    /// </summary>
    void Pause()
    {
        _gamePause = !_gamePause;

        //SetGamePause(_gamePause);

        _isOpenOption = !_isOpenOption;

        if (!_gamePause && !_isOpenOption)
            panel.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");
        else
            panel.transform.gameObject.GetComponent<Animator>().SetTrigger("Open");
        
        OnClickButton("_ColorG");
        OnClickButton("_ColorB");
        OnClickButton("_ColorR");
    }

    /// <summary>
    /// Set game unpause.
    /// </summary>
    void UnPause()
    {
        _gamePause = !_gamePause;
        _isOpenOption = !_isOpenOption;
        panel.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");
    }

    
    /// <summary>
    /// Get shader value RGB to slider value RGB.
    /// </summary>
    /// <param name="s"></param>
    void OnClickButton(string s)
    {
        _shaderColorValue = s;
        _clickButton = true;

        colorProp.R.value = _mat.GetColor(s).r * MAX_VALUE_COLOR;
        colorProp.G.value = _mat.GetColor(s).g * MAX_VALUE_COLOR;
        colorProp.B.value = _mat.GetColor(s).b * MAX_VALUE_COLOR;

        SetColorButton(_shaderColorValue, new Color(colorProp.R.value / MAX_VALUE_COLOR, colorProp.G.value / MAX_VALUE_COLOR, colorProp.B.value / MAX_VALUE_COLOR));

        SetColorHandle(new Color(colorProp.R.value / MAX_VALUE_COLOR, colorProp.G.value / MAX_VALUE_COLOR, colorProp.B.value / MAX_VALUE_COLOR));

        _clickButton = false;
    }

    /// <summary>
    /// Set shader value and change color player.
    /// </summary>
    void OnValueChanged()
    {
        if (_shaderColorValue != null && !_clickButton)
        {

            SetColorButton(_shaderColorValue, new Color(colorProp.R.value / MAX_VALUE_COLOR, colorProp.G.value / MAX_VALUE_COLOR, colorProp.B.value / MAX_VALUE_COLOR));

            _mat.SetColor(_shaderColorValue, new Color(colorProp.R.value / MAX_VALUE_COLOR, colorProp.G.value / MAX_VALUE_COLOR, colorProp.B.value / MAX_VALUE_COLOR));

            SetColorHandle(new Color(colorProp.R.value / MAX_VALUE_COLOR, colorProp.G.value / MAX_VALUE_COLOR, colorProp.B.value / MAX_VALUE_COLOR));
        }
    }

    /// <summary>
    /// Set color handle r, g, b when value change.
    /// </summary>
    /// <param name="color"></param>
    void SetColorHandle(Color color)
    {
        colorProp.HandleR.color = new Color(color.r, 0, 0);
        colorProp.HandleG.color = new Color(0, color.g, 0);
        colorProp.HandleB.color = new Color(0, 0, color.b);
    }

    /// <summary>
    /// Set color button Engine, Binder, Body.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="color"></param>
    void SetColorButton(string s, Color color)
    {
        if (s == "_ColorG")
        {
            colorProp.buttonEngine.gameObject.GetComponent<Image>().color = color;
        }
        else if (s == "_ColorB")
        {
            colorProp.buttonBinder.gameObject.GetComponent<Image>().color = color;
        }
        else if (s == "_ColorR")
        {
            colorProp.buttonBody.gameObject.GetComponent<Image>().color = color;
        }
    }

    /// <summary>
    /// Show text to debug in option.
    /// </summary>
    /// <param name="s"></param>
    public void DebugConsole(string s)
    {
        if (debugText != null)
            debugText.text = "<color=#ffff00ff>" +System.DateTime.Now +"</color>"+ " : " +s + _newLine + debugText.text;
    }

    // ScoreController
    /// <summary>
    /// Increase score.
    /// </summary>
    /// <param name="score"></param>
    public void IncreaseScorePoint(int score)
    {
        scorePoint += score;        
    }

    /// <summary>
    /// Decrease score.
    /// </summary>
    /// <param name="damage"></param>
    public void DecreaseScorePoint(int damage)
    {
        scorePoint -= damage;
    }

    /// <summary>
    /// Get score.
    /// </summary>
    /// <returns></returns>
    public int GetScore()
    {
        return scorePoint;
    }

    /// <summary>
    /// Set score. Use this after load data from database.
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
         scorePoint = score;
    }

    /// <summary>
    /// Get energy.
    /// </summary>
    /// <returns></returns>
    public float GetEnergy()
    {
        return energyPoint;
    }

    /// <summary>
    /// Increase energy.
    /// </summary>
    /// <param name="energy"></param>
    public void IncreaseEnergy(float energy)
    {
        //energyPoint += energy * Time.deltaTime;
        energyPoint += energy * 4 * Time.deltaTime;
        energyBar.value = energyPoint;
    }

    /// <summary>
    /// Decrease energy.
    /// </summary>
    /// <param name="damage"></param>
    public void DecreaseEnergy (float damage)
    {
        //energyPoint = 0;
        energyPoint -= damage * 4;
        energyBar.value = energyPoint;
    }

    public bool IsMoving()
    {
        return energyBar.value > 0;
    }

    /// <summary>
    /// Set crash for do something after collision.
    /// </summary>
    /// <param name="b"></param>
    public void SetCrash (bool b)
    {
        _crash = b;
    }

    /// <summary>
    /// Get crash for check value _crash.
    /// </summary>
    /// <returns></returns>
    public bool GetCrash ()
    {
        return _crash;
    }

    public void SetGamePause (bool b)
    {
        _gamePause = b;
    }

    public bool GetGamePause ()
    {
        return _gamePause;
    }

    /// <summary>
    /// Get area text.
    /// </summary>
    /// <returns></returns>
    public string GetArea()
    {
        return areaText.text;
    }

    /// <summary>
    /// Set area and set it to UI.
    /// </summary>
    /// <param name="e"></param>
    public void SetArea(Area e)
    {
        area = e;
        areaText.text = area.ToString();
    }

    /// <summary>
    /// Get Rotation.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRotation()
    {
        return rotation;
    }

    /// <summary>
    /// Set rotation value and set UI text rotation.
    /// </summary>
    /// <param name="vec3"></param>
    public void SetRotation(Vector3 vec3)
    {
        rotation = vec3;
        rotationX.text = "X : " + rotation.x.ToString("###.00");
        rotationY.text = "Y : " + rotation.y.ToString("###.00");
        rotationZ.text = "Z : " + rotation.z.ToString("###.00");
    }

    /// <summary>
    /// Get player color RGB.
    /// </summary>
    /// <returns></returns>
    public Color GetPlayerColor()
    {
        return Color.clear;
    }

    /// <summary>
    /// Set player color RGB.
    /// </summary>
    /// <param name="color"></param>
    public void SetPlayerColor(Color color)
    {

    }

}

/// <summary>
/// ColorProperties.
/// </summary>
[System.Serializable]
public class ColorProperties
{
    public Image ship;
    public Button buttonEngine, buttonBinder, buttonBody;
    public Slider R, G, B;
    public Image HandleR, HandleG, HandleB;

}
