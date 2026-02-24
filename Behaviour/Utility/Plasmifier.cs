using Architect.Placements;
using UnityEngine;

namespace Architect.Behaviour.Utility;

public class Plasmifier : MonoBehaviour
{
    public int mode;
    public string id;
    public int heal;

    private bool _plasmified;

    public void Plasmify()
    {
        if (_plasmified) return;

        _plasmified = true;
        
        if (!PlacementManager.Objects.TryGetValue(id, out var target)) return;
        
        var dupe = target.GetComponent<ObjectDuplicator>();
        if (dupe) dupe.plasmifier = this;
        else Plasmify(target);
    }

    public void Plasmify(GameObject target)
    {
        var hm = target.GetComponentInChildren<HealthManager>();
        if (!hm) return;

        target.AddComponent<LifebloodState>().healAmount = heal;
    }

    private void Update()
    {
        if (mode == 0 && !_plasmified) Plasmify();
    }

    public class LifebloodState : MonoBehaviour
    {
      public int healAmount = 5;
      private bool dead;
      private bool healingIsActive = true;
      private int maxHP;
      private float timer;
      private HealthManager healthManager;
      private SpriteFlash spriteFlash;
      private GameObject healEffect;

      private void Start()
      {
        healthManager = gameObject.GetComponent<HealthManager>();
        spriteFlash = gameObject.GetComponent<SpriteFlash>();

        maxHP = healthManager.hp;
      }

      private void Update()
      {
        if (dead || !healingIsActive) return;
        var hp = healthManager.hp;
        if (hp < maxHP)
        {
          if (timer < 0.75)
          {
            timer += Time.deltaTime;
          }
          else
          {
            var num = hp + healAmount;
            if (num > maxHP)
              num = maxHP;
            healthManager.hp = num;
            healEffect.SetActive(true);
            spriteFlash.flashHealBlue();
            timer -= 0.75f;
          }
        }
        else
          timer = 0.0f;
      }

      public void TakeDamage() => timer = 0.0f;

      public void SetIsLifebloodHealing(bool set)
      {
        timer = 0.0f;
        healingIsActive = set;
      }
    }
}