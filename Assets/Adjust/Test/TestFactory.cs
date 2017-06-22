using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;

using com.adjust.sdk;

public class TestFactory {
    private string baseUrl;
    private AndroidJavaClass ajcAdjustFactory;
    private AndroidJavaObject ajoTestLibrary;
    private AndroidJavaObject ajoCurrentActivity;

    private static CommandExecutor commandExecutor;
    private static CommandListener onCommandReceivedListener;

    public TestFactory(string baseUrl) {
        this.baseUrl = baseUrl;
        this.ajcAdjustFactory = new AndroidJavaClass("com.adjust.sdk.AdjustFactory");

        AndroidJavaClass ajcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        this.ajoCurrentActivity = ajcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        commandExecutor = new CommandExecutor(this);
        onCommandReceivedListener = new CommandListener();
    }

	public void InitTestMode() {
        ajcAdjustFactory.CallStatic("setTestingMode", this.baseUrl);

        if (ajoTestLibrary == null) {
            ajoTestLibrary = new AndroidJavaObject("com.adjust.testlibrary.TestLibrary", this.baseUrl, onCommandReceivedListener);
        }

        ajoTestLibrary.Call("initTestSession", "unity4.11.3@android4.11.4");
    }

    public void Teardown(bool deleteState) {
        Adjust.Teardown();

        ajcAdjustFactory.CallStatic("teardown", ajoCurrentActivity, deleteState);
    }

    public void SetTimerInterval(long interval) {
        ajcAdjustFactory.CallStatic("setTimerInterval", interval);
    }

    public void SetTimerStart(long interval) {
        ajcAdjustFactory.CallStatic("setTimerStart", interval);
    }

    public void SetSessionInterval(long interval) {
        ajcAdjustFactory.CallStatic("setSessionInterval", interval);
    }

    public void SetSubsessionInterval(long interval) {
        ajcAdjustFactory.CallStatic("setSubsessionInterval", interval);
    }

    public void AddInfoToSend(string key, string paramValue) {
        ajoTestLibrary.Call("addInfoToSend", key, paramValue);
    }

    public void SendInfoToServer() {
        ajoTestLibrary.Call("sendInfoToServer");
    }

    private class CommandListener : AndroidJavaProxy {
        public CommandListener() : base("com.adjust.testlibrary.ICommandRawJsonListener") {}

        public void executeCommand(string json) {
            if (json == null) {
                return;
            }

            var command = JSON.Parse(json);

            string className = command["className"].Value;
            string functionName = command["functionName"].Value;
            JSONNode parameters = command["params"];

            commandExecutor.ExecuteCommand(className, functionName, parameters);
        }
    }
}
