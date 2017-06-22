using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;

using com.adjust.sdk;

public class TestApp : MonoBehaviour {
	private TestFactory testFactory;

    void OnGUI() {
        if (GUI.Button(new Rect(100, Screen.height / 2 - (Screen.height / 20), Screen.width - 200, Screen.height / 10), "START TESTS")) {
            string baseUrl = "https://10.0.2.2:8443";

            if (null == this.testFactory) {
            	this.testFactory = new TestFactory(baseUrl);
            }

            this.testFactory.InitTestMode();
        }
    }
}
