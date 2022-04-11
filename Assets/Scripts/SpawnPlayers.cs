using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Spec.Specialisation.SpecMulti
{
    public class SpawnPlayers : MonoBehaviour
    {
        public GameObject playerPrefab;

        public float minX;
        public float maxX;
        public float minY;
        public float maxY;

        private void Start()
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}