﻿﻿using System;
using System.Collections;
using ChannelNS;
using EventNS.keepAliveNS;
using SenderStrategyNS;
using UnityEngine;
using UnityEngine.SceneManagement;
 using Utils;

namespace PackageProcessNS.KeepAliveNS {
    public class ClientKeepAlive: MonoBehaviour {
        private KeepAliveChannel _channel;
        private float lastKeepAlive = 0;

        private void Start() {
            _channel = new KeepAliveChannel((b => {
                        Debug.Log("Received KEEP ALIVE");
                lastKeepAlive = CurrentTime.Time;
                }
             ), new ReliableStrategy(0.1f, 20));

            ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int) RegisteredChannels.KeepAliveChannel, _channel);

            StartCoroutine(SendKeepAlive());
        }

        IEnumerator SendKeepAlive() {
            while (true) {
                Debug.Log("SENDING KEEP ALIVE");
                _channel.SendEvent(true);
                yield return new WaitForSeconds(1);
            }
        }
        
        private void Update() {
            Debug.LogWarning("Client time " + (CurrentTime.Time - lastKeepAlive) );
            if (CurrentTime.Time - lastKeepAlive > 5) {
                Debug.LogError("PLAYER IS DEAD");
                SceneManager.LoadScene(0);
            }
        }

        private void OnDestroy() {
            _channel.Dispose();
        }
    }
}