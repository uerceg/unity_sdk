using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;

using com.adjust.sdk;

public class CommandExecutor {
    private string basePath;
    private TestFactory testFactory;

    public CommandExecutor(TestFactory testFactory) {
        this.testFactory = testFactory;
    }

    public void ExecuteCommand(string className, string methodName, JSONNode parameters) {
        if (className.Equals("Adjust")) {
            switch (methodName) {
                case "factory" : Factory(parameters); break;
                case "teardown" : Teardown(parameters); break;
                case "start" : Start(parameters); break;
                case "trackEvent" : TrackEvent(parameters); break;
                case "resume" : Resume(parameters); break;
                case "pause" : Pause(parameters); break;
                case "setEnabled" : SetEnabled(parameters); break;
                case "sendReferrer" : SetReferrer(parameters); break;
                case "setOfflineMode" : SetOfflineMode(parameters); break;
                case "sendFirstPackages" : SendFirstPackages(parameters); break;
                case "addSessionCallbackParameter" : AddSessionCallbackParameter(parameters); break;
                case "addSessionPartnerParameter" : AddSessionPartnerParameter(parameters); break;
                case "removeSessionCallbackParameter" : RemoveSessionCallbackParameter(parameters); break;
                case "removeSessionPartnerParameter" : RemoveSessionPartnerParameter(parameters); break;
                case "resetSessionCallbackParameters" : ResetSessionCallbackParameters(parameters); break;
                case "resetSessionPartnerParameters" : ResetSessionPartnerParameters(parameters); break;
                case "setPushToken" : SetPushToken(parameters); break;
                // case "openDeeplink" : OpenDeepLink(parameters); break;
                case "testBegin" : TestBegin(parameters); break;
                case "testEnd" : TestEnd(parameters); break;
                default : CommandNotFound(className, methodName); break;
            }
        }
    }

    private void Factory(JSONNode parameters) {
        string basePathParam = parameters["basePath"][0].Value;
        string timerInterval = parameters["timerInterval"][0].Value;
        string timerStart = parameters["timerStart"][0].Value;
        string sessionInterval = parameters["sessionInterval"][0].Value;
        string subsessionInterval = parameters["subsessionInterval"][0].Value;

        if (!String.IsNullOrEmpty(basePathParam)) {
            this.basePath = basePathParam;
        }

        if (!String.IsNullOrEmpty(timerInterval)) {
            long timerIntervalValue = 0;

            if (Int64.TryParse(timerInterval, out timerIntervalValue)) {
                this.testFactory.SetTimerInterval(timerIntervalValue);
            } else {
                Debug.Log("adjust test: Failed to convert timerInterval string to long");
            }
        }

        if (!String.IsNullOrEmpty(timerStart)) {
            long timerStartValue = 0;

            if (Int64.TryParse(timerStart, out timerStartValue)) {
                this.testFactory.SetTimerStart(timerStartValue);
            } else {
                Debug.Log("adjust test: Failed to convert timerStart string to long");
            }
        }

        if (!String.IsNullOrEmpty(sessionInterval)) {
            long sessionIntervalValue = 0;

            if (Int64.TryParse(sessionInterval, out sessionIntervalValue)) {
                this.testFactory.SetSessionInterval(sessionIntervalValue);
            } else {
                Debug.Log("adjust test: Failed to convert sessionInterval string to long");
            }
        }

        if (!String.IsNullOrEmpty(subsessionInterval)) {
            long subsessionIntervalValue = 0;

            if (Int64.TryParse(subsessionInterval, out subsessionIntervalValue)) {
                this.testFactory.SetSubsessionInterval(subsessionIntervalValue);
            } else {
                Debug.Log("adjust test: Failed to convert subsessionInterval string to long");
            }
        }
    }

    private void Teardown(JSONNode parameters) {
        string deleteState = parameters["deleteState"][0].Value;

        if (null != deleteState) {
            this.testFactory.Teardown(deleteState.Equals("true"));
        }
    }

    private AdjustConfig GetConfig(JSONNode parameters) {
        AdjustConfig adjustConfig = null;

        string appToken = parameters["appToken"][0].Value;
        string environment = parameters["environment"][0].Value;
        string logLevel = parameters["logLevel"][0].Value;
        string sdkPrefix = parameters["sdkPrefix"][0].Value;
        string defaultTracker = parameters["defaultTracker"][0].Value;
        string delayStart = parameters["delayStart"][0].Value;
        string deviceKnown = parameters["deviceKnown"][0].Value;
        string eventBufferingEnabled = parameters["eventBufferingEnabled"][0].Value;
        string sendInBackground = parameters["sendInBackground"][0].Value;
        string userAgent = parameters["userAgent"][0].Value;
        string attributionCallback = parameters["attributionCallbackSendAll"];
        string sessionSuccessCallback = parameters["sessionCallbackSendSuccess"];
        string sessionFailureCallback = parameters["sessionCallbackSendFailure"];
        string eventSuccessCallback = parameters["eventCallbackSendSuccess"];
        string eventFailureCallback = parameters["eventCallbackSendFailure"];

        if (!String.IsNullOrEmpty(appToken)) {
            if (appToken.Equals("null")) {
                adjustConfig = new AdjustConfig(null, environment.Equals("sandbox") ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production);
            } else {
                adjustConfig = new AdjustConfig(appToken, environment.Equals("sandbox") ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production);
            }
        } else {
            adjustConfig = new AdjustConfig(null, environment.Equals("sandbox") ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production);
        }

        if (!String.IsNullOrEmpty(logLevel)) {
            // Set log level in proper way.
            // Forcing VERBOSE right now.
            adjustConfig.setLogLevel(AdjustLogLevel.Verbose);
        }

        if (!String.IsNullOrEmpty(defaultTracker)) {
            if (defaultTracker.Equals("null")) {
                adjustConfig.setDefaultTracker(null);
            } else {
                adjustConfig.setDefaultTracker(defaultTracker);
            }
        }

        if (!String.IsNullOrEmpty(delayStart)) {
            try {
                float delayValue = float.Parse(delayStart, CultureInfo.InvariantCulture.NumberFormat);

                adjustConfig.setDelayStart(delayValue);
            } catch (Exception e) {
                Debug.Log("adjust test: " + e.ToString());
            }
        }

        if (!String.IsNullOrEmpty(deviceKnown)) {
            // adjustConfig.setDeviceKnown(deviceKnown.Equals("true"));
        }

        if (!String.IsNullOrEmpty(eventBufferingEnabled)) {
            adjustConfig.setEventBufferingEnabled(eventBufferingEnabled.Equals("true"));
        }

        if (!String.IsNullOrEmpty(sendInBackground)) {
            adjustConfig.setSendInBackground(sendInBackground.Equals("true"));
        }

        if (null != userAgent) {
            if (userAgent.Equals("null")) {
                adjustConfig.setUserAgent(null);
            } else {
                adjustConfig.setUserAgent(userAgent);
            }
        }

        if (null != attributionCallback) {
            adjustConfig.setAttributionChangedDelegate(AttributionChangedCallback);
        }

        if (null != eventSuccessCallback) {
            adjustConfig.setEventSuccessDelegate(EventSuccessCallback);
        }

        if (null != eventFailureCallback) {
            adjustConfig.setEventFailureDelegate(EventFailureCallback);
        }

        if (null != sessionSuccessCallback) {
            adjustConfig.setSessionSuccessDelegate(SessionSuccessCallback);
        }

        if (null != sessionFailureCallback) {
            adjustConfig.setSessionFailureDelegate(SessionFailureCallback);
        }

        return adjustConfig;
    }

    private AdjustEvent GetEvent(JSONNode parameters) {
        AdjustEvent adjustEvent = null;

        string eventToken = parameters["eventToken"][0].Value;
        string revenue = parameters["revenue"][1].Value;
        string currency = parameters["revenue"][0].Value;
        string orderId = parameters["orderId"][0].Value;
        
        JSONNode callbackParameters = parameters["callbackParams"];
        JSONNode partnerParameters = parameters["partnerParams"];

        if (!String.IsNullOrEmpty(eventToken)) {
            if (eventToken.Equals("null")) {
                adjustEvent = new AdjustEvent(null);
            } else {
                adjustEvent = new AdjustEvent(eventToken);
            }
        }

        if (!String.IsNullOrEmpty(revenue)) {
            try {
                float revenueValue = float.Parse(revenue, CultureInfo.InvariantCulture.NumberFormat);

                if (!String.IsNullOrEmpty(currency)) {
                    if (currency.Equals("null")) {
                        adjustEvent.setRevenue(revenueValue, null);
                    } else {
                        adjustEvent.setRevenue(revenueValue, currency);
                    }
                }
            } catch (Exception e) {
                Debug.Log("adjust test: " + e.ToString());
            }
        }

        if (!String.IsNullOrEmpty(orderId)) {
            if (orderId.Equals("null")) {
                adjustEvent.setTransactionId(null);
            } else {
                adjustEvent.setTransactionId(orderId);
            }
        }

        if (null != callbackParameters) {
            for (int i = 0; i < callbackParameters.Count; i += 2) {
                string paramKey = callbackParameters[i].Value;
                string paramValue = callbackParameters[i+1].Value;

                if (paramKey.Equals("null")) {
                    paramKey = null;
                }

                if (paramValue.Equals("null")) {
                    paramValue = null;
                }

                adjustEvent.addCallbackParameter(paramKey, paramValue);
            }
        }

        if (null != partnerParameters) {
            for (int i = 0; i < partnerParameters.Count; i += 2) {
                string paramKey = partnerParameters[i].Value;
                string paramValue = partnerParameters[i+1].Value;

                if (paramKey.Equals("null")) {
                    paramKey = null;
                }

                if (paramValue.Equals("null")) {
                    paramValue = null;
                }

                adjustEvent.addPartnerParameter(paramKey, paramValue);
            }
        }

        return adjustEvent;
    }

    private void Start(JSONNode parameters) {
        AdjustConfig adjustConfig = GetConfig(parameters);

        adjustConfig.SetBasePath(this.basePath);

        Adjust.start(adjustConfig);
    }

    private void TrackEvent(JSONNode parameters) {
        AdjustEvent adjustEvent = GetEvent(parameters);

        Adjust.trackEvent(adjustEvent);
    }

    private void Resume(JSONNode parameters) {
        Adjust.OnResume();
    }

    private void Pause(JSONNode parameters) {
        Adjust.OnPause();
    }

    private void SetEnabled(JSONNode parameters) {
        string enabled = parameters["enabled"][0].Value;

        Adjust.setEnabled(enabled.Equals("true"));
    }

    private void SetReferrer(JSONNode parameters) {
        string referrer = parameters["referrer"][0].Value;

        Adjust.setReferrer(referrer);
    }

    private void SetOfflineMode(JSONNode parameters) {
        string enabled = parameters["enabled"][0].Value;

        Adjust.setOfflineMode(enabled.Equals("true"));
    }

    private void SendFirstPackages(JSONNode parameters) {
        Adjust.sendFirstPackages();
    }

    private void AddSessionCallbackParameter(JSONNode parameters) {
        if (null != parameters["KeyValue"]) {
            JSONNode list = parameters["KeyValue"];

            for (int i = 0; i < list.Count; i += 2) {
                string paramKey = list[i].Value;
                string paramValue = list[i+1].Value;

                if (paramKey.Equals("null")) {
                    paramKey = null;
                }

                if (paramValue.Equals("null")) {
                    paramValue = null;
                }

                Adjust.addSessionCallbackParameter(paramKey, paramValue);
            }
        }
    }

    private void AddSessionPartnerParameter(JSONNode parameters) {
        if (null != parameters["KeyValue"]) {
            JSONNode list = parameters["KeyValue"];

            for (int i = 0; i < list.Count; i += 2) {
                string paramKey = list[i].Value;
                string paramValue = list[i+1].Value;

                if (paramKey.Equals("null")) {
                    paramKey = null;
                }

                if (paramValue.Equals("null")) {
                    paramValue = null;
                }

                Adjust.addSessionPartnerParameter(paramKey, paramValue);
            }
        }
    }

    private void RemoveSessionCallbackParameter(JSONNode parameters) {
        if (null != parameters["key"]) {
            JSONNode list = parameters["key"];

            for (int i = 0; i < list.Count; i++) {
                string key = parameters["key"][i].Value;

                if (!String.IsNullOrEmpty(key)) {
                    if (key.Equals("null")) {
                        Adjust.removeSessionCallbackParameter(null);
                    } else {
                        Adjust.removeSessionCallbackParameter(key);
                    }
                }
            }
        }
    }

    private void RemoveSessionPartnerParameter(JSONNode parameters) {
        if (null != parameters["key"]) {
            JSONNode list = parameters["key"];

            for (int i = 0; i < list.Count; i++) {
                string key = parameters["key"][i].Value;

                if (!String.IsNullOrEmpty(key)) {
                    if (key.Equals("null")) {
                        Adjust.removeSessionPartnerParameter(null);
                    } else {
                        Adjust.removeSessionPartnerParameter(key);
                    }
                }
            }
        }
    }

    private void ResetSessionCallbackParameters(JSONNode parameters) {
        Adjust.resetSessionCallbackParameters();
    }

    private void ResetSessionPartnerParameters(JSONNode parameters) {
        Adjust.resetSessionPartnerParameters();
    }

    private void SetPushToken(JSONNode parameters) {
        string pushToken = parameters["pushToken"][0].Value;

        if (!String.IsNullOrEmpty(pushToken)) {
            if (pushToken.Equals("null")) {
                Adjust.setDeviceToken(null);
            } else {
                Adjust.setDeviceToken(pushToken);
            }
        }
    }

    // Still unavailable.
    /*
    private void OpenDeepLink(JSONNode parameters) {
        string deepLink = parameters["deeplink"][0].Value;

        Adjust.AppWillOpenUrl(deepLink);
    }
    */

    private void TestBegin(JSONNode parameters) {
        string path = parameters["basePath"][0].Value;

        if (!String.IsNullOrEmpty(path)) {
            this.basePath = path;
        }

        this.testFactory.Teardown(true);
        this.testFactory.SetTimerInterval(-1);
        this.testFactory.SetTimerStart(-1);
        this.testFactory.SetSessionInterval(-1);
        this.testFactory.SetSubsessionInterval(-1);
    }

    private void TestEnd(JSONNode parameters) {
        this.testFactory.Teardown(true);
    }

    private void CommandNotFound(string className, string methodName) {
        Debug.Log("adjust test: Method '" + methodName + "' not found for class '" + className + "'");
    }

    private void AttributionChangedCallback(AdjustAttribution attributionData) {
        this.testFactory.AddInfoToSend("trackerToken", attributionData.trackerToken);
        this.testFactory.AddInfoToSend("trackerName", attributionData.trackerName);
        this.testFactory.AddInfoToSend("network", attributionData.network);
        this.testFactory.AddInfoToSend("campaign", attributionData.campaign);
        this.testFactory.AddInfoToSend("adgroup", attributionData.adgroup);
        this.testFactory.AddInfoToSend("creative", attributionData.creative);
        this.testFactory.AddInfoToSend("clickLabel", attributionData.clickLabel);
        this.testFactory.AddInfoToSend("adid", attributionData.adid);

        this.testFactory.SendInfoToServer();
    }

    private void EventSuccessCallback(AdjustEventSuccess eventSuccessData) {
        this.testFactory.AddInfoToSend("message", eventSuccessData.Message);
        this.testFactory.AddInfoToSend("timestamp", eventSuccessData.Timestamp);
        this.testFactory.AddInfoToSend("adid", eventSuccessData.Adid);
        this.testFactory.AddInfoToSend("eventToken", eventSuccessData.EventToken);
        
        if (eventSuccessData.JsonResponse != null) {
            this.testFactory.AddInfoToSend("jsonResponse", AdjustUtils.GetJsonResponseCompact(eventSuccessData.JsonResponse));
        }

        this.testFactory.SendInfoToServer();
    }

    private void EventFailureCallback(AdjustEventFailure eventFailureData) {
        this.testFactory.AddInfoToSend("message", eventFailureData.Message);
        this.testFactory.AddInfoToSend("timestamp", eventFailureData.Timestamp);
        this.testFactory.AddInfoToSend("adid", eventFailureData.Adid);
        this.testFactory.AddInfoToSend("eventToken", eventFailureData.EventToken);
        this.testFactory.AddInfoToSend("willRetry", eventFailureData.WillRetry.ToString().ToLower());
        
        if (eventFailureData.JsonResponse != null) {
            this.testFactory.AddInfoToSend("jsonResponse", AdjustUtils.GetJsonResponseCompact(eventFailureData.JsonResponse));
        }

        this.testFactory.SendInfoToServer();
    }

    private void SessionSuccessCallback(AdjustSessionSuccess sessionSuccessData) {
        this.testFactory.AddInfoToSend("message", sessionSuccessData.Message);
        this.testFactory.AddInfoToSend("timestamp", sessionSuccessData.Timestamp);
        this.testFactory.AddInfoToSend("adid", sessionSuccessData.Adid);
        
        if (sessionSuccessData.JsonResponse != null) {
            this.testFactory.AddInfoToSend("jsonResponse", AdjustUtils.GetJsonResponseCompact(sessionSuccessData.JsonResponse));
        }

        this.testFactory.SendInfoToServer();
    }

    private void SessionFailureCallback(AdjustSessionFailure sessionFailureData) {
        this.testFactory.AddInfoToSend("message", sessionFailureData.Message);
        this.testFactory.AddInfoToSend("timestamp", sessionFailureData.Timestamp);
        this.testFactory.AddInfoToSend("adid", sessionFailureData.Adid);
        this.testFactory.AddInfoToSend("willRetry", sessionFailureData.WillRetry.ToString().ToLower());
        
        if (sessionFailureData.JsonResponse != null) {
            this.testFactory.AddInfoToSend("jsonResponse", AdjustUtils.GetJsonResponseCompact(sessionFailureData.JsonResponse));
        }

        this.testFactory.SendInfoToServer();
    }
}
