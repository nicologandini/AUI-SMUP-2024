using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SMUP.Multiplayer
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
    #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Console_UI.Instance.ConsolePrint("PhotonNetwork : Loading scene");
            PhotonNetwork.LoadLevel("OutsideScene");
        }

    #endregion

    #region Photon Callbacks
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            Console_UI.Instance.ConsolePrint("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

            if (PhotonNetwork.IsMasterClient)
            {
                Console_UI.Instance.ConsolePrint("OnPlayerEnteredRoom IsMasterClient " + PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player other)
        {
            Console_UI.Instance.ConsolePrint("OnPlayerLeftRoom()" + other.NickName); // seen when other disconnects

            if (PhotonNetwork.IsMasterClient)
            {
                Console_UI.Instance.ConsolePrint("OnPlayerLeftRoom IsMasterClient " + PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

    #endregion
    }
}