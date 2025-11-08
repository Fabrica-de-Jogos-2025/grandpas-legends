using UnityEngine.UI;
using UnityEngine;

public static class EffectUtility
{
    public static void ApplyDamageEffect(GameObject target, string name, int turns, int damage, Image icon, string description)
    {
        ImmunityComponent ic = target.GetComponent<ImmunityComponent>();
        if (ic != null) return; // já que está imune a efeitos negativos

        DamageComponent existing = target.GetComponent<DamageComponent>();

        if (existing != null && existing.effectName == name)
        {
            existing.ResetEffect(turns, damage);
        }
        else
        {
            PairImageDescription pair = new PairImageDescription(icon, description);

            EffectRosterComponent ed = target.GetComponent<EffectRosterComponent>();
            if (ed != null)
                ed.Add(pair);

            DamageComponent newEffect = target.AddComponent<DamageComponent>();
            newEffect.Initialize(name, turns, damage);
        }
    }

    public static void ApplyHealEffect(GameObject target, string name, int turns, int healAmount, Image icon, string description)
    {
        HealComponent existing = target.GetComponent<HealComponent>();

        if (existing != null && existing.effectName == name)
        {
            existing.ResetEffect(turns, healAmount);
        }
        else
        {
            PairImageDescription pair = new PairImageDescription(icon, description);

            EffectRosterComponent ed = target.GetComponent<EffectRosterComponent>();
            if (ed != null)
                ed.Add(pair);

            HealComponent newEffect = target.AddComponent<HealComponent>();
            newEffect.Initialize(name, turns, healAmount);
        }
    }

    public static void ApplyPowerModifier(GameObject target, string name, int turns, int modifier, Image icon, string description)
    {
        ImmunityComponent ic = target.GetComponent<ImmunityComponent>();
        if (ic != null && modifier < 0) return; // já que está imune a efeitos negativos
        
        ModifyPowerComponent existing = target.GetComponent<ModifyPowerComponent>();

        if (existing != null && existing.effectName == name)
        {
            existing.ResetEffect(turns, modifier);
        }
        else
        {
            PairImageDescription pair = new PairImageDescription(icon, description);

            EffectRosterComponent ed = target.GetComponent<EffectRosterComponent>();
            if (ed != null)
                ed.Add(pair);

            ModifyPowerComponent newEffect = target.AddComponent<ModifyPowerComponent>();
            newEffect.Initialize(name, turns, modifier);
        }
    }

    // 🛡️ Invulnerabilidade (protege contra dano)
    public static void ApplyInvulnerability(GameObject target, string name, int turns, Image icon, string description)
    {
        InvulnerableComponent existing = target.GetComponent<InvulnerableComponent>();
        if (existing != null && existing.effectName == name)
        {
            existing.ResetEffect(turns);
        }
        else
        {
            PairImageDescription pair = new PairImageDescription(icon, description);
            EffectRosterComponent erc = target.GetComponent<EffectRosterComponent>();
            if (erc != null) erc.Add(pair);

            InvulnerableComponent newEffect = target.AddComponent<InvulnerableComponent>();
            newEffect.Initialize(name, turns);
        }
    }

    // 💀 Morte súbita
    public static void ApplySuddenDeath(GameObject target, string name, int turns, Image icon, string description)
    {
        PairImageDescription pair = new PairImageDescription(icon, description);
        EffectRosterComponent erc = target.GetComponent<EffectRosterComponent>();
        if (erc != null) erc.Add(pair);

        SuddenDeathComponent newEffect = target.AddComponent<SuddenDeathComponent>();
        newEffect.Initialize(name, turns);
    }

    // 💫 Atordoamento
    public static void ApplyStun(GameObject target, string sourceName, int turns, Image icon, string description)
    {
        StunnedComponent existing = target.GetComponent<StunnedComponent>();
        if (existing != null)
        {
            existing.turnsRemaining = turns;
        }
        else
        {
            PairImageDescription pair = new PairImageDescription(icon, description);
            EffectRosterComponent erc = target.GetComponent<EffectRosterComponent>();
            if (erc != null) erc.Add(pair);

            StunnedComponent stunned = target.AddComponent<StunnedComponent>();
            stunned.sourceName = sourceName;
            stunned.turnsRemaining = turns;
        }
    }

    // 🕊️ Reviver (uma vez)
    public static void ApplyRevive(GameObject target, CardBehaviour giver, Image icon, string description)
    {
        ReviveComponent existing = target.GetComponent<ReviveComponent>();
        if (existing == null)
        {
            PairImageDescription pair = new PairImageDescription(icon, description);
            EffectRosterComponent erc = target.GetComponent<EffectRosterComponent>();
            if (erc != null) erc.Add(pair);

            ReviveComponent revive = target.AddComponent<ReviveComponent>();
            revive.Initialize(giver);
        }
    }

    // Imunidade a efeitos negativos
    public static void ApplyImmunity(GameObject target, string name, int turns, Image icon, string description)
    {
        ImmunityComponent existing = target.GetComponent<ImmunityComponent>();
        if (existing != null && existing.effectName == name)
        {
            existing.ResetEffect(turns);
        }
        else
        {
            PairImageDescription pair = new PairImageDescription(icon, description);
            EffectRosterComponent erc = target.GetComponent<EffectRosterComponent>();
            if (erc != null) erc.Add(pair);

            ImmunityComponent newEffect = target.AddComponent<ImmunityComponent>();
            newEffect.Initialize(name, turns);
        }
    }
    
    // Conditional heal Aticupu
    public static void ApplyConditionalHeal(GameObject target, CardBehaviour giver, Image icon, string description)
    {
        PairImageDescription pair = new PairImageDescription(icon, description);

        EffectRosterComponent erc = target.GetComponent<EffectRosterComponent>();
        if (erc != null) erc.Add(pair);

        ReviveComponent revive = target.AddComponent<ReviveComponent>();
        revive.Initialize(giver);
    } 
}



