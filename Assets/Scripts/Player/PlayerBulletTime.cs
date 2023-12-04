using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerBulletTime : MonoBehaviour
{
    public static PlayerBulletTime Instance;
    public float TimePoints { get => timePoints; }

    [Header("Main preferences")]
    [SerializeField] private float duration = 5f;
    [SerializeField][Range(0f, 1f)] private float bulletTimeScale = 0.3f;
    [SerializeField] private float playerSpeedMultiplier = 2f;
    [SerializeField] private bool applySpeedMultiplierToGravity = false;

    [Header("Скорости (скорость измеряется в [доля/секунда]")]
    [SerializeField] private float defaultRegenerationSpeed = 0.25f;
    [SerializeField] private float battleRegenerationSpeed = 0.1f;
    [SerializeField] private float reductionSpeed = 0.25f;
    [SerializeField] private float killBonus = 0.5f;

    [Header("Optional preferences")]
    [SerializeField] private float timeTransitionMultiplier = 10f;
    [SerializeField] private KeyCode key = KeyCode.B;

    private float timePoints = 1f;
    private bool bulletTime = false;
    private Player player;
    private float targetTimeScale = 1f;
    private float targetPlayerSpeedMultiplier = 1f;
    private float currentTargetTime = 0f;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        player = GetComponent<Player>();
        
    }

    private void Update()
    {
        UpdatePoints();

        if (Input.GetKeyDown(key))
            ChangeTimeScale();

        if (bulletTime && timePoints == 0f)
            ExitBulletTime();

        InterpolateValues();
    }
    private void UpdatePoints()
    {
        if (bulletTime)
        {
            timePoints -= Time.unscaledDeltaTime * reductionSpeed;
        }
        else
        {
            timePoints += Time.unscaledDeltaTime * defaultRegenerationSpeed;
        }
        timePoints = Mathf.Clamp(timePoints, 0f, 1f);
    }
    private void InterpolateValues()
    {
        player.Movement.GetWalker.ApplyVelocityMultiplierToGravity = applySpeedMultiplierToGravity;
        if (targetTimeScale != Time.timeScale)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.deltaTime * timeTransitionMultiplier);
        }
        if (targetPlayerSpeedMultiplier != player.Movement.GetWalker.VelocityMultiplier)
            player.Movement.GetWalker.VelocityMultiplier = Mathf.Lerp(player.Movement.GetWalker.VelocityMultiplier, targetPlayerSpeedMultiplier, Time.deltaTime * timeTransitionMultiplier);
    }
    private void ChangeTimeScale()
    {
        if (bulletTime)
            ExitBulletTime();
        else
            EnterBulletTime();
    }
    private void EnterBulletTime()
    {
        currentTargetTime = Time.realtimeSinceStartup + duration;
        targetTimeScale = bulletTimeScale;
        targetPlayerSpeedMultiplier = playerSpeedMultiplier;
        bulletTime = true;

        GlobalPostProcessing.SetProfile(PostProccesingType.bulletTime);
    }
    private void ExitBulletTime()
    {
        targetTimeScale = 1f;
        targetPlayerSpeedMultiplier = 1f;
        bulletTime = false;

        GlobalPostProcessing.SetProfile(PostProccesingType.standart);
    }
    public void AddKillBonus()
    {
        timePoints += killBonus;
    }
}
