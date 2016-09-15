﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterSelectManager : MonoBehaviour {

	GameObject characterSelectElements;
	public Character p1Character, p2Character;
	Sprite[] p1CharacterPortraits, p2CharacterPortraits, p1InfoPanes, p2InfoPanes;
	public bool charactersSelected;
	private bool p1Selected, p2Selected, p1ButtonDown, p2ButtonDown;
	private int numCharacters;
	// Use this for initialization
	void Start () {
		p1Selected = false;
		p2Selected = false;
		p1ButtonDown = true;
		p2ButtonDown = true;
		charactersSelected = false;
		characterSelectElements = GameObject.Find("CharacterSelectElements");
        characterSelectElements.SetActive(false);
		p1CharacterPortraits = Resources.LoadAll<Sprite>("characterSelect/p1/portraits");
        p1InfoPanes = Resources.LoadAll<Sprite>("characterSelect/p1/summons");
        p2CharacterPortraits = Resources.LoadAll<Sprite>("characterSelect/p2/portraits");
		p2InfoPanes = Resources.LoadAll<Sprite>("characterSelect/p2/summons");
		numCharacters = System.Enum.GetValues(typeof(Character)).Length;
	}

	public void Reset() {
		charactersSelected = false;
		characterSelectElements.SetActive(true);
        p1Character = Character.Loholt;
        p2Character = Character.Orpheus;
	}

	public void CharacterSelectUpdate() {
		CheckDirectionals();
		if(!p1Selected && Input.GetAxis("Horizontal0") != 0 && !p1ButtonDown) {
			p1ButtonDown = true;
    		p1Character = CycleThroughCharacters(p1Character, "Horizontal0");
    	}
		if(!p2Selected && Input.GetAxis("Horizontal1") != 0 && !p2ButtonDown) {
			p2ButtonDown = true;
    		p2Character = CycleThroughCharacters(p2Character, "Horizontal1");
    	}
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(0).gameObject,
    										 p1Character, p1CharacterPortraits, p1InfoPanes);
    	UpdateInfoCharacterSelect(characterSelectElements.transform.GetChild(1).gameObject,
    										 p2Character, p2CharacterPortraits, p2InfoPanes);

    	if(Input.GetButtonUp("ButtonB0")) {
			p1Selected = true;
            Debug.Log("P1 Selected");
        }
    	if(Input.GetButtonUp("ButtonB1")) {
    		p2Selected = true;
            Debug.Log("P2 Selected");
        }
    	if(p1Selected && p2Selected) {
			characterSelectElements.SetActive(false);
			charactersSelected = true;
    	}
    }

    void UpdateInfoCharacterSelect(GameObject player, Character highlightedCharacter,
    														 Sprite[] portraits,
    														 Sprite[] infoPanes) {
    	SpriteRenderer portrait = player.GetComponentInChildren<SpriteRenderer>();
    	portrait.sprite = portraits[(int)highlightedCharacter];
    	SpriteRenderer infoPane = player.GetComponentsInChildren<SpriteRenderer>()[2];
    	infoPane.sprite = infoPanes[(int)highlightedCharacter];
    	Text nameText = player.GetComponentInChildren<Text>();
    	nameText.text = highlightedCharacter.ToString();
    	Text[] bulletDescriptions = player.GetComponentsInChildren<Text>();
    	int firstText = bulletDescriptions.Length - 3;
		bulletDescriptions[firstText + 0].text = "Shield for blocking";//bullets.types[(int)highlightedCharacter].projectileTypes[1].bulletDescription;
		bulletDescriptions[firstText + 1].text = "Tracks enemy";//bullets.types[(int)highlightedCharacter].projectileTypes[2].bulletDescription;
		bulletDescriptions[firstText + 2].text = "A piercing summon";//bullets.types[(int)highlightedCharacter].projectileTypes[0].bulletDescription;   
    }

    Character CycleThroughCharacters(Character character, string axis) {
		if(Input.GetAxisRaw(axis) < 0) {
				int temp = (int)character;
				temp -= 1;
   		 		if(temp < 0) {
   		 			temp = numCharacters -1;
   		 		}
				return (Character)temp;//System.Enum.GetValues(typeof(Character)).GetValue(temp);

   		 	}
    	else if(Input.GetAxisRaw(axis) > 0) {
    			character += 1;
    			if((int)character == numCharacters) {
    				return (Character)System.Enum.GetValues(typeof(Character)).GetValue(0);
    			}
    		}
    	return character;
    }

    void CheckDirectionals() {
    	if(Input.GetAxis("Horizontal0") == 0) {
    		p1ButtonDown = false;
    	}
    	if(Input.GetAxis("Horizontal1") == 0) {
    		p2ButtonDown = false;
    	}
    }
}