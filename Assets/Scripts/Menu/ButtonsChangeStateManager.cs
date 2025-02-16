using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsChangeStateManager : MonoBehaviour
{
    [Header("Buttons")] 
    [SerializeField] private Image[] buttons;
    [Header("Buttons Sprites")]
    [SerializeField] private Sprite[] buttonSprites;


    private List<Tuple<Sprite,Sprite>> _buttonSprites;


    void Awake()
    {
        _buttonSprites = new List<Tuple<Sprite, Sprite>>();

        if(buttons == null || buttons.Length < 1) {return;}

        for(int i = 0; i<buttons.Length; i++) {
            if((i+1)*2 > buttonSprites.Length) {break;}

            _buttonSprites.Add(new Tuple<Sprite, Sprite>(buttonSprites[i*2], buttonSprites[i*2+1]));
        }   
    }

    public void ButtonPressed(int buttonIndex) {
        for(int i = 0; i<buttons.Length; i++) {
            if(i == buttonIndex) {
                buttons[buttonIndex].sprite = _buttonSprites[buttonIndex].Item1;    //set active sprite
            } else {
                buttons[i].sprite = _buttonSprites[i].Item2;                        //set inactive sprite
            }
        }
    }
}