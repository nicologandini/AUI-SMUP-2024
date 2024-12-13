using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Player{
    List<GameObject> stations;
    List<GameObject> balloons;
    Hashtable deliveredItems;  // key: station, value: balloon

    public Player(List<GameObject> balloons, List<GameObject> stations){
        this.balloons = balloons;
        this.stations = stations;
        this.deliveredItems = new Hashtable();
        foreach(GameObject s in stations){
            this.deliveredItems.Add(s, null);
        }
    }

    public void addDeliveredBalloon(GameObject balloon, GameObject station){
        if(deliveredItems.ContainsKey(station)){
            deliveredItems[station] = balloon;
        }
    }

    public void removeDeliveredBalloon(GameObject station){
        if(deliveredItems.ContainsKey(station)){
            deliveredItems[station] = null;
        }
    }

    public void flushDeliveredItems(){
        foreach(DictionaryEntry entry in deliveredItems){
            deliveredItems[entry.Value] = null;
        }
    }

    public List<GameObject> getBalloons(){
        return balloons;
    }

    public List<GameObject> getStations(){
        return stations;
    }

    public List<GameObject> getDeliveredBalloons(){
        List<GameObject> t = new List<GameObject>();
        try{
            foreach(KeyValuePair<GameObject, GameObject> pair in this.deliveredItems){
                Debug.Log(pair.Value);
                t.Add(pair.Value);
            }
        }
        catch (InvalidCastException e){
            t = null;
        }
        return t;
    }
}