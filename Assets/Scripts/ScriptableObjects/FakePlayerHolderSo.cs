using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FakePlayerHolderSo", menuName = "Scriptable Objects/FakePlayerHolderSo")]
public class FakePlayerHolderSo : ScriptableObject
{
    public List<FakePlayerSo> fakePlayers = new List<FakePlayerSo>();
}
