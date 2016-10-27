﻿using UnityEngine;
using System.Collections;


public class PlayerStats : MonoBehaviour {
	public float health;
	public float maxHealth;
	public Color playerColor;
	public int shotCooldown;
	public int meleeCooldown;
	public int number;
	public Character character;
	public GameObject lifeBar;
	public GameObject bufferBar;
	private BarController bufferBarController;

	void Start() {
		bufferBarController = bufferBar.GetComponent<BarController>();
	}

	public void TakeDamage(float damage) {
		health -= damage;

        if (health > 0) {
			lifeBar.GetComponent<BarController>().LerpChange(health / maxHealth);

		}
		else {
            lifeBar.GetComponent<BarController>().LerpChange(0);
            health = 0;
        }
	}

	public void UpdateBufferBar(float scale) {
		bufferBarController.LerpChange(scale);
	}
}
