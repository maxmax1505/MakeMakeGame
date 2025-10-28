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
        // '����' ������ ������ ��ٸ���
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        TalkManager.TempTrue = false;
    }


    public class Lightening : ISpell
    {
        public string Name { get; set; } = "���� ����";
        public int ManaCost { get; set; } = 15;

        public IEnumerator CAST(SpellContext context)
        {
            yield return ShowDialog($"{context.caster.Name}�� ���� ������ ���!");

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
                    TalkManager.Instance.ShowTemp($"{context.enemySlots[i].enemy.Name}���� {damage} ������!");
                    totalDamage += damage;

                    for (int V = 0; V < 5; V++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        context.enemySlots[i].marker.gameObject.GetComponent<Image>().color = Color.blue;
                        yield return new WaitForSeconds(0.05f);
                        context.enemySlots[i].marker.gameObject.GetComponent<Image>().color = Color.white;
                    }

                }
                yield return ShowDialog($"����� ���� ���� ������ �����ƴ�. ���� �� ������ : {totalDamage}");
            }
            else
            {
                yield return ShowDialog($"{context.caster.Name}�� ���� ������ ���!");
                int damage;
                float spell_mp = SpellDamageByMana(context.caster, context.Target);
                float spell_dmg = SpellDamageCalc(context.caster, context.Target);
                float spell_dis = SpellDamageByDistance(context.caster);
                damage = Mathf.RoundToInt(spell_dmg * spell_mp * spell_dis);
                context.Target.CurrentHp -= damage;

                context.PlayerMarker.GetChild(1).GetComponent<Slider>().value = (float)context.Target.CurrentHp / context.Target.HP;
                TalkManager.Instance.ShowTemp($"{context.Target.Name}���� {damage} ������!");

                for (int V = 0; V < 5; V++)
                {
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.blue;
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.white;
                }

                yield return ShowDialog($"{context.caster}�� �ֹ� ������ �����ƴ�. ����� ���� ������ : {damage}");
            }
        }
    }

    public class MindShetter : ISpell
    {
        public string Name { get; set; } = "���� �ĸ�";
        public int ManaCost { get; set; } = 5;

        public IEnumerator CAST(SpellContext context)
        {
            ShowDialog($"{context.caster.Name}�� ���� �ĸ��� �����ߴ�!");

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

                    TalkManager.Instance.ShowTemp($"{context.enemySlots[i].enemy.Name}���� {MindDamage}��ŭ�� ���� ����� �־���!");
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
                yield return ShowDialog($"����� ���� �ĸ� ������ �����ƴ�. ��Ƴ��� �� ���ŷ� : {totalDamage}");
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
                TalkManager.Instance.ShowTemp($"{context.Target.Name}���� {MindDamage}��ŭ�� ���� ����� �־���!");

                for (int V = 0; V < 5; V++)
                {
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.purple;
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.white;
                }

                ShowDialog($"{context.caster}�� �ֹ� ������ �����ƴ�. ��Ƴ����� ���ŷ� : {MindDamage}");
            }
        }
    }
}

