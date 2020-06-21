using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour {

    [SerializeField]
    private float health = 100f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private PlayerController controller;

    private float Health {
        get { return health; }
        set {
            health = value;
            if (health <= 0) {
                Die();
                health = 0;
            }
        }
    }

    private void Die() {
        animator.SetTrigger("Die");
    }

    [SerializeField]//selected item in hotbar
    private GameObject currentItem;

    private GameObject CurrentItem {
        get { return currentItem; }
        set {
            currentItem = value;
            currentWeapon = currentItem.GetComponent<Weapon>();
        }
    }

    private Weapon currentWeapon;

    // Start is called before the first frame update
    void Start() {
        CurrentItem = currentItem;
        currentWeapon.WeaponHolder = this;
    }

    // Update is called once per frame
    void Update() {
        if (currentWeapon != null) {
            if (Input.GetButton("Fire1")) {
                currentWeapon.Fire();
            }
            if (Input.GetButton("Reload")) {
                currentWeapon.Reload();
            }
        }
        else {
            Debug.Log("Weapon null");
        }
            //Inputs
    /*private float xInput = 0f;

    private float yInput = 0f;

    private float mouseX = 0f;

    private float mouseY = 0f;

    private bool sprint;//Input.GetButton("Sprint")

    private bool aim;//Input.GetButton("Fire2")

    private bool jump;//Input.GetButtonDown("Jump")*/
        controller.SetInput(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetButton("Sprint"), Input.GetButton("Fire2"), Input.GetButtonDown("Jump"));
    }

    internal void Damage(float damage) {
        Debug.Log("Damaging");
        Health -= damage;
    }

}
