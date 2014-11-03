//using UnityEngine;
//using System.Collections;

// commenting this out for now but just saving the code incase
// we try using PhotonNetwork for online mode
// for now lets just try local multiplayer

//public class NetworkManager : MonoBehaviour {

//    // Use this for initialization
//    void Start() {
//        Connect();
//    }

//    void Connect() {
//        PhotonNetwork.ConnectUsingSettings("Space Gods 0.1");
//    }

//    void OnJoinedLobby() {
//        PhotonNetwork.JoinRandomRoom();
//    }

//    void OnPhotonRandomJoinFailed() {
//        PhotonNetwork.CreateRoom(null);
//    }

//    void OnJoinedRoom() {
//        SpawnMyPlayer();
//    }

//    void SpawnMyPlayer() {
//        Vector2 spawn = Random.insideUnitCircle * 5f;
//        GameObject myGod = (GameObject)PhotonNetwork.Instantiate("Zeus", spawn, Quaternion.identity, 0);
//        myGod.GetComponent<GodController>().enabled = true;
//    }

//    void OnGUI() {
//        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
//    }
//}
