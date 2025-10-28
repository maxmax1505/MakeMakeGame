using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public struct SpellContext
{
    public ICharacter caster;
    public ICharacter Target;
    public IReadOnlyList<(RectTransform marker, RectTransform endpoint, ICharacter enemy, Slider slider, Slider manaslider)> enemySlots;
    public RectTransform PlayerMarker;
    public bool casterIsPlayer;
}
public interface ISpell
{
    string Name { get; set; }
    int ManaCost { get; set; }

    IEnumerator CAST(SpellContext context);
}


public class MagicSpellChart : MonoBehaviour
{

    public static float SpellDamageByDistance(ICharacter MustBeEnemy)
    {
        return 1 - Mathf.InverseLerp(0, BattleManager.gameMax, MustBeEnemy.Distance);
    }

    public static float SpellDamageByMana(ICharacter attaker, ICharacter deffender)
    {
        float lerp;
        lerp = 1 - Mathf.InverseLerp(0, deffender.Mp, deffender.CurrentMp) + 0.3f;

        return lerp;
    }

    public static float SpellDamageCalc(ICharacter caster, ICharacter defender)
    {
        return caster.Mp % 20 + caster.WillPower;
    }

    public static IEnumerator ShowDialog(string say)
    {
        TalkManager.Instance.ShowTemp(say);
        yield return null;
        // '새로' 눌리는 순간을 기다린다
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        TalkManager.TempTrue = false;
    }


    public class Lightening : ISpell
    {
        public string Name { get; set; } = "전격 방출";
        public int ManaCost { get; set; } = 15;

        public IEnumerator CAST(SpellContext context)
        {
            yield return ShowDialog($"{context.caster.Name}은 전격 방출을 썼다!");

            if (context.casterIsPlayer == true)
            {
                int totalDamage = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (context.enemySlots[i].enemy == null)
                    {
                        continue;
                    }

                    int damage;
                    float spell_mp = SpellDamageByMana(context.caster, context.enemySlots[i].enemy);
                    float spell_dmg = SpellDamageCalc(context.caster, context.enemySlots[i].enemy);
                    float spell_dis = SpellDamageByDistance(context.enemySlots[i].enemy);
                    damage = Mathf.RoundToInt(spell_dmg * spell_mp * spell_dis);
                    context.enemySlots[i].enemy.CurrentHp -= damage;

                    context.enemySlots[i].slider.value = (float)context.enemySlots[i].enemy.CurrentHp / context.enemySlots[i].enemy.HP;
                    TalkManager.Instance.ShowTemp($"{context.enemySlots[i].enemy.Name}에게 {damage} 데미지!");
                    totalDamage += damage;

                    for (int V = 0; V < 5; V++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        context.enemySlots[i].marker.gameObject.GetComponent<Image>().color = Color.blue;
                        yield return new WaitForSeconds(0.05f);
                        context.enemySlots[i].marker.gameObject.GetComponent<Image>().color = Color.white;
                    }

                }
                yield return ShowDialog($"당신은 전격 방출 시전을 끝마쳤다. 가한 총 데미지 : {totalDamage}");
            }
            else
            {
                yield return ShowDialog($"{context.caster.Name}은 전격 방출을 썼다!");
                int damage;
                float spell_mp = SpellDamageByMana(context.caster, context.Target);
                float spell_dmg = SpellDamageCalc(context.caster, context.Target);
                float spell_dis = SpellDamageByDistance(context.caster);
                damage = Mathf.RoundToInt(spell_dmg * spell_mp * spell_dis);
                context.Target.CurrentHp -= damage;

                context.PlayerMarker.GetChild(1).GetComponent<Slider>().value = (float)context.Target.CurrentHp / context.Target.HP;
                TalkManager.Instance.ShowTemp($"{context.Target.Name}에게 {damage} 데미지!");

                for (int V = 0; V < 5; V++)
                {
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.blue;
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.white;
                }

                yield return ShowDialog($"{context.caster}는 주문 시전을 끝마쳤다. 당신의 입은 데미지 : {damage}");
            }
        }
    }

    public class MindShetter : ISpell
    {
        public string Name { get; set; } = "정신 파멸";
        public int ManaCost { get; set; } = 5;

        public IEnumerator CAST(SpellContext context)
        {
            ShowDialog($"{context.caster.Name}은 정신 파멸을 시전했다!");

            if (context.casterIsPlayer == true)
            {
                int totalDamage = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (context.enemySlots[i].enemy == null)
                    {
                        continue;
                    }

                    int MindDamage;
                    float spell_mp = SpellDamageByMana(context.caster, context.enemySlots[i].enemy);
                    float spell_dmg = SpellDamageCalc(context.caster, context.enemySlots[i].enemy);
                    float spell_dis = SpellDamageByDistance(context.enemySlots[i].enemy);
                    MindDamage = Mathf.RoundToInt(spell_dmg * 2 * spell_mp * spell_dis);
                    context.enemySlots[i].enemy.CurrentMp -= MindDamage;

                    TalkManager.Instance.ShowTemp($"{context.enemySlots[i].enemy.Name}에게 {MindDamage}만큼의 정신 충격을 주었다!");
                    context.enemySlots[i].manaslider.value = (float)context.enemySlots[i].enemy.CurrentMp / context.enemySlots[i].enemy.Mp;

                    totalDamage += MindDamage;

                    for (int V = 0; V < 5; V++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        context.enemySlots[i].marker.gameObject.GetComponent<Image>().color = Color.purple;
                        yield return new WaitForSeconds(0.05f);
                        context.enemySlots[i].marker.gameObject.GetComponent<Image>().color = Color.white;
                    }

                }
                yield return ShowDialog($"당신은 정신 파멸 시전을 끝마쳤다. 깎아내린 총 정신력 : {totalDamage}");
            }
            else
            {
                int MindDamage;
                float spell_mp = SpellDamageByMana(context.caster, context.Target);
                float spell_dmg = SpellDamageCalc(context.caster, context.Target);
                float spell_dis = SpellDamageByDistance(context.caster);
                MindDamage = Mathf.RoundToInt(spell_dmg * 2* spell_mp * spell_dis);
                context.Target.CurrentHp -= MindDamage;

                context.PlayerMarker.GetChild(0).GetComponent<Slider>().value = (float)context.Target.CurrentMp / context.Target.Mp;
                TalkManager.Instance.ShowTemp($"{context.Target.Name}에게 {MindDamage}만큼의 정신 충격을 주었다!");

                for (int V = 0; V < 5; V++)
                {
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.purple;
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.white;
                }

                ShowDialog($"{context.caster}는 주문 시전을 끝마쳤다. 깎아내려진 정신력 : {MindDamage}");
            }
        }
    }
}

