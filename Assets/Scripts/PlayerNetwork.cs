using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NGOTutorial
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private Transform spawnedObjectPrefab;
        private Transform spawnedObjectReference;
        
        private struct CustomDataStruct : INetworkSerializable
        {
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref TheInt);
                serializer.SerializeValue(ref TheBool);
                serializer.SerializeValue(ref TheString);
            }
            
            public int TheInt;
            public bool TheBool;
            public FixedString512Bytes TheString;
        }

        private readonly NetworkVariable<CustomDataStruct> _networkDataStruct = new();
        private readonly NetworkVariable<int> _networkInt = new(1);

        public override void OnNetworkSpawn()
        {
            _networkInt.OnValueChanged += (int previousValue, int newValue) =>
            {
                Debug.Log($"{OwnerClientId}: {newValue}");
            };
            
            _networkDataStruct.OnValueChanged += (CustomDataStruct previousValue, CustomDataStruct newValue) =>
            {
                Debug.Log($"{OwnerClientId}: {newValue}");
            };
        }

        /// <summary>
        /// Must end with Rpc suffix
        /// </summary>
        [ServerRpc]
        private void TestServerRpc(string message)
        {
            Debug.Log($"Test Server RPC {OwnerClientId}: {message}");
        }

        [ClientRpc]
        private void TestClientRpc(ClientRpcParams clientRpcParams)
        {
            Debug.Log($"Test Client RPC {OwnerClientId}");
        }
        
        [SerializeField] private float speed = 3.0f;
        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKey(KeyCode.Alpha4))
            {
                spawnedObjectReference = Instantiate(spawnedObjectPrefab);
                spawnedObjectReference.GetComponent<NetworkObject>().Spawn(true);
            }

            if (Input.GetKey(KeyCode.Alpha5))
            {
                Destroy(spawnedObjectReference.gameObject);
            }
            
            if (Input.GetKey(KeyCode.Alpha3))
            {
                TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams{ TargetClientIds = new List<ulong> { 1u } } });
            }

            if (Input.GetKey(KeyCode.Alpha2))
            {
                TestServerRpc("Hoorah");
            }

            if (Input.GetKey(KeyCode.Alpha0))
            {
                _networkInt.Value = Random.Range(0, 100);
            }

            if (Input.GetKey(KeyCode.Alpha1))
            {
                var value = _networkDataStruct.Value;
                value.TheInt = Random.Range(100, 200);
                value.TheBool = !value.TheBool;
                value.TheString = $"I see Dead People {new DateTime()}";
                _networkDataStruct.Value = value;
            }
            
            Vector3 moveDir = new();
            
            if (Input.GetKey(KeyCode.W))
            {
                moveDir += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDir += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDir += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDir += Vector3.right;
            }
            
            if (moveDir != Vector3.zero)
            {
                transform.position += moveDir.normalized * (speed * Time.deltaTime);
            }
            
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position += Vector3.up * (speed * Time.deltaTime);
            }
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.position += Vector3.down * (speed * Time.deltaTime);
            }
        }
    }
}