using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityWeapon))]
public class GravityWeaponAnimations : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private Animator anim;
    [SerializeField] private float minSpeedToEnableAnimations = 2f;
    [SerializeField] private float lerpingMovementAnimationsMultiplier = 15f;

    [Header("Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip pointsModeShootClip;
    [SerializeField] private AudioClip gravityModeShootClip;
    [SerializeField] private AudioClip changeModeClip;

    private GravityWeapon gravityWeapon;
    private Vector2 movementBlend = Vector2.zero;

    private void OnEnable()
    {
        if(gravityWeapon == null)
            gravityWeapon = GetComponent<GravityWeapon>();
        if (anim == null)
        {
            enabled = false;
        }
        gravityWeapon.OnPointsModeShooted += PlayPointsModeAttackAnimation;
        gravityWeapon.OnGravityChangingModeShooted += PlayGravityChangingModeAttackAnimation;
        gravityWeapon.OnWorkModeChanged += PlayChangeModeAnimation;
    }
    private void OnDisable()
    {
        gravityWeapon.OnPointsModeShooted -= PlayPointsModeAttackAnimation;
        gravityWeapon.OnGravityChangingModeShooted -= PlayGravityChangingModeAttackAnimation;
        gravityWeapon.OnWorkModeChanged -= PlayChangeModeAnimation;
    }
    private void Update()
    {
        UpdateMovement();
        UpdateShootingMode();
    }
    private void UpdateMovement()
    {
        PlayerMovementData movementData = Player.Instance.Movement.CurrentMovementData;
        Vector2 _target = Vector2.zero;
        if (movementData.isGrounded)
        {
            if (movementData.currentSpeed > minSpeedToEnableAnimations)
            {
                if (movementData.isCrouching)
                    _target.y = -1f;
                else if (movementData.isRunning)
                    _target.y = 2f;
                else
                    _target.y = 1f;
            }
        }
        else
            _target.x = 1f;
        movementBlend = Vector2.Lerp(movementBlend, _target, Time.deltaTime * lerpingMovementAnimationsMultiplier);
        anim.SetFloat("MovementX", movementBlend.x);
        anim.SetFloat("MovementY", movementBlend.y);
    }
    private void UpdateShootingMode()
    {
        anim.SetBool("PointsShooting", gravityWeapon.WorkMode == GravityWeaponWorkMode.Points);
    }
    private void PlayPointsModeAttackAnimation()
    {
        anim.SetTrigger("Attack_Points");
        source.PlayOneShot(pointsModeShootClip);
    }
    private void PlayGravityChangingModeAttackAnimation()
    {
        anim.SetTrigger("Attack_Gravity");
        source.PlayOneShot(gravityModeShootClip);
    }
    private void PlayChangeModeAnimation(GravityWeaponWorkMode mode)
    {
        anim.SetTrigger("ChangeMode");
        source.PlayOneShot(changeModeClip);
    }
}
