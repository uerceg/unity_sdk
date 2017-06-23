using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;

using com.adjust.sdk;

public class TestApp : MonoBehaviour {
    private TestFactory testFactory;
    private static bool isLaunched = false;

    void OnGUI() {
        if (false == isLaunched) {
            string baseUrl = "https://10.0.2.2:8443";

            if (null == this.testFactory) {
                this.testFactory = new TestFactory(baseUrl);
            }

            this.testFactory.InitTestMode();

            isLaunched = true;
        }
    }
}
