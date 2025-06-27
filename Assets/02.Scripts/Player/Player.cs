using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public PlayerStat Stat;
    
    private Dictionary<Type, PlayerAbility> _abilitieCache = new();

    public T GetAbility<T>() where T : PlayerAbility
    {
        var type = typeof(T);

        if(_abilitieCache.TryGetValue(type, out PlayerAbility ability))
        {
            return ability as T;
        }

        // 게으른 초기화/로딩 -> 처음에 곧바로 초기화/로딩을 하는게 아니라
        //                      필요할때만 하는.. 뒤로 미루는 기법
        ability = GetComponent<T>();

        if(ability != null)
        {
            _abilitieCache[ability.GetType()] = ability;
            return ability as T;
        }

        throw new Exception($"어빌리티 {type.Name}를 {gameObject.name}에서 찾을 수 없습니다.");
    }
}
