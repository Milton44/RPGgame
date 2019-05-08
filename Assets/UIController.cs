using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("Bar Life")]
    public RectTransform barTexture;
    private float currentLife;
    private float maxScaleXBar, maxLifePlayer;

    [Header("Weapons")]
    [Space(10)]
    public Sprite sword;
    public Sprite magic;
    public Sprite bow;
    public Image imageWeapon;
    private WeapomSystem.TypeWeapom currentWeapom;
    private Player player;
    void Start () {
        player = FindObjectOfType<PlayerController>().player;
        maxLifePlayer = player.getLife();
        maxScaleXBar = barTexture.localScale.x;
        currentWeapom = WeapomSystem.currentWeapom;
    }

	void Update () {
        BarLifeController();
        SetImageWeapom();
	}
    private void BarLifeController()
    {
        if(currentLife != player.getLife())
        {
            if (player.getLife() >= 0)
            {
                float newScaleX = (player.getLife() * maxScaleXBar) / maxLifePlayer;
                barTexture.localScale = new Vector3(newScaleX, barTexture.localScale.y, barTexture.localScale.z);
                currentLife = player.getLife();
            }
            else
            {
                barTexture.localScale = new Vector3(0, barTexture.localScale.y, barTexture.localScale.z);
            }
        }
    }
    private void SetImageWeapom()
    {
        if (currentWeapom != WeapomSystem.currentWeapom)
        {
            currentWeapom = WeapomSystem.currentWeapom;
            switch (currentWeapom)
            {
                case WeapomSystem.TypeWeapom.Bow:
                    imageWeapon.sprite = bow;
                    break;
                case WeapomSystem.TypeWeapom.Sword:
                    imageWeapon.sprite = sword;
                    break;

                case WeapomSystem.TypeWeapom.Magic:
                    imageWeapon.sprite = magic;
                    break;
            }
            
        }
    }

}
