using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace SMUP.GameLogic {
    public class Player{
        List<GameObject> stations;
        List<GameObject> balloons;
		List<GameObject> otherStations;
        Hashtable deliveredItems;  // key: station, value: balloon

        public Player(List<GameObject> balloons, List<GameObject> stations/*, List<GameObject> otherStations*/){
            this.balloons = balloons;
            this.stations = stations;
			//this.otherStations = otherStations;
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
            foreach(GameObject s in stations){
                t.Add((GameObject)this.deliveredItems[s]);
            }
            if(t.Contains(null)){
                return null;
            }
            return t;
        }

        public bool containsBalloon(GameObject station)
        {
            if (deliveredItems[station] != null)
            {
                return true;
            }
            return false;
        }

        // Ci sono due casi di collider exit: uno in cui la stazione era gi� occupata e l'altro in cui la stazione era libera,
        // il metodo removeDeliveredBalloon deve essere chiamato solo nel secondo caso, cio� quando il palloncino che sta uscendo ha lo stesso
        // materiale del palloncino che stava entrando (if material == material). In questo caso il metodo ritorna TRUE
        public bool removeBalloon(GameObject balloon, GameObject station)
        {
            if (containsBalloon(station))
            {
                GameObject obj = (GameObject)deliveredItems[station];
                if (obj.GetComponent<Renderer>().material == balloon.GetComponent<Renderer>().material)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public string GetBalloonColorName(GameObject balloon) {
            if (balloon == null) { return "NULL"; }

            return balloon.GetComponent<Renderer>().material.name;
        }

        public GameObject getBalloon(GameObject station)
        {
            return (GameObject)deliveredItems[station];
        }

        public GameObject getStation(int index)
        {
            return stations[index];
        }
		
		public GameObject getOtherStation(int index) {
			
			return otherStations[index];
		}

        public string GetBalloonsColors() {
            string balloonsColors = "";
            Console_UI.Instance.ConsolePrint("Passing colors");

            for (int i = 0; i < 6; i++) {
                Console_UI.Instance.ConsolePrint("Color:" + (GetBalloonColorName(getBalloon(getStation(i)))));
                balloonsColors += (GetBalloonColorName(getBalloon(getStation(i)))) + ",";
            }
            
            return balloonsColors;
        }
		
		public int getStationIndex(GameObject station) {
			for (int i = 0; i < 6; i++) {
				if (stations[i] == station) {
					return i;
				}
			}
			return 11;
		}
    }
}