using UnityEngine;

public class SkillTimer : MonoBehaviour
{
    public AddValueOnTimer TargetSkill;

    public bool timerF;
    float maxTimer;
    public float timer;

    public void StartTimer()
    {
        maxTimer = TargetSkill.MaxTimer;
        timer = maxTimer;
        timerF = true;
    }

    void Update()
    {
        if (timerF)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                TargetSkill.FinishSkill();
                Destroy(gameObject);
            }
        }

    }
}
