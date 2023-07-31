using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

[CreateAssetMenu(fileName = "CharacterBaseStatsSO", menuName = "Characters/Base Stats")]
public class CharacterBaseStats : SO
{
    public float moveSpeed = 4f;
    public int hp = 1;
    public int dodge = 0;

}
