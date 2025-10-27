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
    public IReadOnlyList<(RectTransform marker, RectTransform endpoint, ICharacter enemy, Slider slider)> enemySlots;
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

   

    public static float SpellDamageByMana(ICharacter attaker, ICharacter deffender)
    {
        float lerp;
        lerp = 1 - Mathf.InverseLerp(0, deffender.Mp, deffender.CurrentMp) + 0.3f;

        return lerp;
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
            ShowDialog($"{context.caster.Name}은 전격 방출을 썼다!");

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
                    damage = Mathf.RoundToInt((context.caster.Mp % 20) * SpellDamageByMana(context.caster, context.enemySlots[i].enemy));
                    context.enemySlots[i].enemy.CurrentHp -= damage;

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
                int damage;
                damage = Mathf.RoundToInt((context.caster.Mp % 20) * SpellDamageByMana(context.caster, context.Target));
                context.Target.CurrentHp -= damage;

                TalkManager.Instance.ShowTemp($"{context.Target.Name}에게 {damage} 데미지!");

                for (int V = 0; V < 5; V++)
                {
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.blue;
                    yield return new WaitForSeconds(0.05f);
                    context.PlayerMarker.gameObject.GetComponent<Image>().color = Color.white;
                }

                ShowDialog($"{context.caster}는 주문 시전을 끝마쳤다. 당신의 입은 데미지 : {damage}");
            }
        }
    }
}

