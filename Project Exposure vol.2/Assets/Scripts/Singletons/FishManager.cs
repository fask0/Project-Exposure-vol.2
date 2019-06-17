using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    private List<FishAvoider> globalFishAvoiders = new List<FishAvoider>();
    private List<FishZone> fishZones = new List<FishZone>();

    private List<FishBehaviourParent> _player = new List<FishBehaviourParent>();
    private List<FishBehaviourParent> _sharks = new List<FishBehaviourParent>();
    private List<FishBehaviourParent> _whales = new List<FishBehaviourParent>();
    private List<FishBehaviourParent> _jellyFish = new List<FishBehaviourParent>();
    private List<FishBehaviourParent> _eels = new List<FishBehaviourParent>();
    private List<FishBehaviourParent> _swordFish = new List<FishBehaviourParent>();
    private List<FishBehaviourParent> _killerWhales = new List<FishBehaviourParent>();

    public enum AvoidableCreatures
    {
        Player,
        Sharks,
        Whales,
        Jellyfish,
        Eels,
        Swordfish,
        Killerwhales,
        Other
    };

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.FishManager = this;
    }

    public void AddAvoidableCreature(AvoidableCreatures creatureType, FishBehaviourParent creature)
    {
        switch (creatureType)
        {
            case AvoidableCreatures.Player:
                _player.Add(creature);
                break;
            case AvoidableCreatures.Sharks:
                _sharks.Add(creature);
                break;
            case AvoidableCreatures.Whales:
                _whales.Add(creature);
                break;
            case AvoidableCreatures.Jellyfish:
                _jellyFish.Add(creature);
                break;
            case AvoidableCreatures.Eels:
                _eels.Add(creature);
                break;
            case AvoidableCreatures.Swordfish:
                _swordFish.Add(creature);
                break;
            case AvoidableCreatures.Killerwhales:
                _killerWhales.Add(creature);
                break;
        }
    }

    public List<FishBehaviourParent> GetAvoidableCreatures(AvoidableCreatures creatureType)
    {
        switch (creatureType)
        {
            case AvoidableCreatures.Player:
                return _player;
            case AvoidableCreatures.Sharks:
                return _sharks;
            case AvoidableCreatures.Whales:
                return _whales;
            case AvoidableCreatures.Jellyfish:
                return _jellyFish;
            case AvoidableCreatures.Eels:
                return _eels;
            case AvoidableCreatures.Swordfish:
                return _swordFish;
            case AvoidableCreatures.Killerwhales:
                return _killerWhales;
            default:
                return new List<FishBehaviourParent>();
        }
    }

    public List<FishAvoider> GetGlobalFishAvoiders()
    {
        return globalFishAvoiders;
    }

    public void AddGlobalFishAvoider(FishAvoider fishAvoider) { globalFishAvoiders.Add(fishAvoider); }
    public void AddFishZone(FishZone fishZone) { fishZones.Add(fishZone); }
}
