using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnimationList :MonoBehaviour
{
    [SerializeField] List<GameObject> enemyAnimation;
    [SerializeField] public static List<GameObject> enemyAnimations;
    public void Awake()
    {
        enemyAnimations = enemyAnimation;
    }
}
