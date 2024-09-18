using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public Color targetColor;
    public float colorChangeSpeed = 0.03f; // Vitesse de changement de couleur
    private readonly System.Random random = new System.Random();

    private float factor = 0.66f; // Facteur de luminosité des couleurs

    private Camera mainCamera;

    private List<Color> colors;

    void Start()
    {
        mainCamera = Camera.main;
        if(PlayerPrefs.GetInt("FirstGame", 1) == 1)
        {
            mainCamera.backgroundColor = NewColor();
            PlayerPrefs.SetInt("FirstGame", 0);
        }
        else {
            mainCamera.backgroundColor = new Color(PlayerPrefs.GetFloat("Red", 0.0f), PlayerPrefs.GetFloat("Green", 0.0f), PlayerPrefs.GetFloat("Blue", 0.0f), 1.0f);
        }
        SaveColor();
        targetColor = mainCamera.backgroundColor;
    }

    void Update()
    {
        if(mainCamera.backgroundColor.r != targetColor.r || mainCamera.backgroundColor.g != targetColor.g || mainCamera.backgroundColor.b != targetColor.b)
            ChangeColor();
        
        if(mainCamera.backgroundColor == targetColor)
            targetColor =  NewColor();
            
        SaveColor();
    }

    private void ChangeColor2()
    {
        float r = mainCamera.backgroundColor.r + (targetColor.r - mainCamera.backgroundColor.r) * colorChangeSpeed;
        float b = mainCamera.backgroundColor.b + (targetColor.b - mainCamera.backgroundColor.b) * colorChangeSpeed;
        float g = mainCamera.backgroundColor.g + (targetColor.g - mainCamera.backgroundColor.g) * colorChangeSpeed;
        if(r < 0.05f && r > -0.05f)
            r = targetColor.r;
        if(g < 0.05f && g > -0.05f)
            g = targetColor.g;
        if(b < 0.05f && b > -0.05f)
            b = targetColor.b;
        mainCamera.backgroundColor = new Color(r, g, b, 1.0f);
    }

    private void ChangeColor()
    {
        float speed = 0.0005f;
        float r = targetColor.r - mainCamera.backgroundColor.r;
        float b = targetColor.b - mainCamera.backgroundColor.b;
        float g = targetColor.g - mainCamera.backgroundColor.g;
        if(r < -speed)
            r = mainCamera.backgroundColor.r -speed;
        else if (r > speed)
            r = mainCamera.backgroundColor.r + speed;
        else 
            if(r < -speed/2)
                r = mainCamera.backgroundColor.r - speed/2;
            else if (r > speed/2)
                r = mainCamera.backgroundColor.r + speed/2;
            else
                r = targetColor.r;
        if(g < -speed)
            g = mainCamera.backgroundColor.g - speed;
        else if (g > 0.05f)
            g = mainCamera.backgroundColor.g + speed;
        else 
            if(g < -speed/2)
                g = mainCamera.backgroundColor.g - speed/2;
            else if (g > speed/2)
                g = mainCamera.backgroundColor.g + speed/2;
            else
                g = targetColor.g;
        if(b < -speed)
            b = mainCamera.backgroundColor.b - speed;
        else if (b > speed)
            b = mainCamera.backgroundColor.b + speed;
        else 
            if(b < -speed/2)
                b = mainCamera.backgroundColor.b - speed/2;
            else if (b > speed/2)
                b = mainCamera.backgroundColor.b + speed/2;
            else
                b = targetColor.b;
        mainCamera.backgroundColor = new Color(r, g, b, 1.0f);
    }

    public Color NewColor()
    {
        return new Color(NewRandValue(), NewRandValue(), NewRandValue(), 1.0f);
    }

    public float NewRandValue()
    {
        float value = Random.value;
        while (value < 0.2f && value > 0.3f)
            value = Random.value;
        return value * factor;
    }

    public void SaveColor() {
        PlayerPrefs.SetFloat("Red", mainCamera.backgroundColor.r);
        PlayerPrefs.SetFloat("Green", mainCamera.backgroundColor.g);
        PlayerPrefs.SetFloat("Blue", mainCamera.backgroundColor.b);
        PlayerPrefs.Save();
    }
}
