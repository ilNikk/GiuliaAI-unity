using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//
// JSON data structure
//  {
//    "event_type": "string",
//    "parameter": "string",
//    "action": "string"
//  }
//
// event_type: "animator" | "request" | "scene"
// parameter: "string"
// action: "trig" | "dancelist" | "change"
//
// Example:
//  {
//    "event_type": "animator",
//    "parameter": "dance_TwiceFancy",
//    "action": "trig"
//  }
//
// JSON response structure
//  {
//    "dancelist": ["string", "string", ...]
//  }
//
// Example request and response:
//  Request:    
//  {
//    "event_type": "request",
//    "parameter": "dancelist",
//    "action": ""
//  }
//  Response:
//  {
//    "dancelist": ["dance_TwiceFancy", "dance_Spin", "dance_Spin2"]
//  }
//

public class ApiServer : MonoBehaviour
{
    private GiuliaAIConfig giuliaAIConfig;
    private EmoteDancePopulator emoteDancePopulator;
    private bool running = false;
    private HttpListener listener;
    private Thread listenerThread;
    private Queue<Action<HttpListenerResponse>> responseActions = new Queue<Action<HttpListenerResponse>>();
    private Queue<HttpListenerContext> contextQueue = new Queue<HttpListenerContext>();
    private Queue<Action> actionsToExecute = new Queue<Action>();
    public class QueuedResponse
    {
        public HttpListenerContext Context { get; set; }
        public Action<HttpListenerResponse> ResponseAction { get; set; }
    }
    private Queue<QueuedResponse> queuedResponses = new Queue<QueuedResponse>();

    void Start()
    {
        giuliaAIConfig = UnityEngine.Object.FindFirstObjectByType<GiuliaAIConfig>();
        emoteDancePopulator = UnityEngine.Object.FindFirstObjectByType<EmoteDancePopulator>();

        listener = new HttpListener();
        listener.Prefixes.Add("http://"+ giuliaAIConfig.APIServerDomain + ":" + giuliaAIConfig.APIServerPort.ToString() + "/");
        listener.Start();

        running = true;
        listenerThread = new Thread(HandleRequests);
        listenerThread.Start();
    }
    private void HandleRequests()
    {
        while (running)
        {
            try
            {
                var context = listener.GetContext();
                ProcessRequest(context);
                contextQueue.Enqueue(context);
                HttpListenerResponse response = context.Response;
                if (responseActions.Count > 0)
                {
                    var action = responseActions.Dequeue();
                    action.Invoke(response);
                }
            }
            catch (HttpListenerException)
            {
                // Gestisci eccezione
            }
        }
    }
    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        response.Headers.Add("Access-Control-Allow-Origin", "*");
        response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        if (request.HttpMethod == "OPTIONS")
        {
            response.StatusCode = 200;
            response.OutputStream.Close();
            return;
        }
        
        if (request.HttpMethod == "POST")
        {
            using (System.IO.Stream body = request.InputStream)
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    string json = reader.ReadToEnd();
                    if (giuliaAIConfig.EnableDebugMode){
                        Debug.Log("JSON ricevuto: " + json);
                    }
                    var data = JsonUtility.FromJson<JsonData>(json);
                    QueuedResponse queuedResponse = null;

                    if (data.event_type == "animator" && data.action == "trig")
                    {
                        queuedResponse = new QueuedResponse
                        {
                            Context = context,
                            ResponseAction = (resp) =>
                            {
                                if (giuliaAIConfig.EnableDebugMode){
                                    Debug.Log("Animator trigger: " + data.parameter);
                                }
                        
                                if (data.parameter == "dance_exit")
                                {
                                    if (giuliaAIConfig.EnableDebugMode){
                                        Debug.Log("Called StopDance");
                                    }
                                    emoteDancePopulator.StopDance();
                                }
                                else if("dance_" == data.parameter.Substring(0, 6) && data.parameter.Length > 6)
                                {
                                    if (giuliaAIConfig.EnableDebugMode){
                                        Debug.Log("Dance trigger: " + data.parameter);
                                    }
                                    int animationIndex = giuliaAIConfig.DanceTriggers.IndexOf(data.parameter);
                                    if (giuliaAIConfig.EnableDebugMode){
                                        Debug.Log("Animation index: " + animationIndex);
                                    }
                                    if (animationIndex != -1)
                                    {
                                        giuliaAIConfig.GUIEmoteDropdown.value = animationIndex;
                                        AudioClip clip = giuliaAIConfig.DanceClips[animationIndex];
                                        List<AudioClip> clipList = new List<AudioClip>();
                                        clipList.Add(clip);
                                        if (giuliaAIConfig.EnableDebugMode){
                                            Debug.Log("Animation found: " + data.parameter + " with audio: " + clip.name);
                                        }
                                        emoteDancePopulator.APITriggerAnimation(data.parameter, clipList);
                                    }
                                    else
                                    {
                                        if (giuliaAIConfig.EnableDebugMode){
                                            Debug.Log("Animation not found: " + data.parameter);
                                        }
                                    }
                                }
                                else
                                {
                                    if (giuliaAIConfig.EnableDebugMode){
                                        Debug.Log("Emote trigger: " + data.parameter);
                                    }
                                    emoteDancePopulator.APITriggerAnimation(data.parameter, null);    
                                }

                                StartCoroutine(DelayedResponse(resp));

                            }
                        };
                    }
                    else if (data.event_type == "request" && data.parameter == "dancelist")
                    {
                        if (giuliaAIConfig.EnableDebugMode){
                            Debug.Log("Animation parameters requested");
                        }
                        queuedResponse = new QueuedResponse
                        {
                            Context = context,
                            ResponseAction = (resp) =>
                            {
                                string[] parameterNames = GetAnimatorParameters();
                                AnimatorParameterData data = new AnimatorParameterData { dancelist = parameterNames };
                                string jsonResponse = JsonUtility.ToJson(data);
                                byte[] jsonBuffer = Encoding.UTF8.GetBytes(jsonResponse);
                                var response = context.Response;
                                response.ContentType = "application/json";
                                response.ContentLength64 = jsonBuffer.Length;
                                response.OutputStream.Write(jsonBuffer, 0, jsonBuffer.Length);
                                if (giuliaAIConfig.EnableDebugMode){
                                    Debug.Log("Animation parameters sent: " + jsonResponse);
                                }
                                response.OutputStream.Flush();
                                response.OutputStream.Close();

                            }
                        };
                    }
                    else
                    {
                        queuedResponse = new QueuedResponse
                        {
                            Context = context,
                            ResponseAction = (resp) =>
                            {
                                var responseString = "200";
                                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                                resp.ContentLength64 = buffer.Length;
                                resp.OutputStream.Write(buffer, 0, buffer.Length);
                                resp.OutputStream.Flush();
                            }
                        };
                    }

                    queuedResponses.Enqueue(queuedResponse);
                }
            }
        }
    }

    private void Update()
    {
        while (actionsToExecute.Count > 0)
        {
            var action = actionsToExecute.Dequeue();
            action?.Invoke();
        }
        while (responseActions.Count > 0 && contextQueue.Count > 0)
        {
            var action = responseActions.Dequeue();
            var context = contextQueue.Dequeue();
            if (context?.Response != null)
            {
                action?.Invoke(context.Response);
            }
        }
        while (queuedResponses.Count > 0)
        {
            var queuedResponse = queuedResponses.Dequeue();
            if (queuedResponse?.Context?.Response != null)
            {
                queuedResponse.ResponseAction?.Invoke(queuedResponse.Context.Response);
            }
        }
    }
    
    void OnApplicationQuit()
    {
        running = false;
        listener.Stop();
        listenerThread.Join();
    }
    void OnDisable()
    {
        running = false;
        listener.Stop();
        listenerThread.Join();
    }

    [System.Serializable]
    public class JsonData
    {
        public string event_type;
        public string parameter;
        public string action;
    }
    private string[] GetAnimatorParameters()
    {
        AnimatorControllerParameter[] parameters = giuliaAIConfig.AnimatorController.parameters;
        string[] names = new string[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            names[i] = parameters[i].name;
        }
        if (giuliaAIConfig.EnableDebugMode){
            Debug.Log("Animator parameters: " + string.Join(", ", names));
        }

        return names;
    }
    [System.Serializable]
    public class AnimatorParameterData
    {
        public string[] dancelist;
    }

    private bool responseInProgress = false; 
    private IEnumerator DelayedResponse(HttpListenerResponse response)
    {
        if (responseInProgress) yield break;
        responseInProgress = true;

        yield return new WaitForSeconds(1.5f);
        
        int danceLayerIndex = giuliaAIConfig.AnimatorController.GetLayerIndex("Dance");
        AnimatorStateInfo currentState = giuliaAIConfig.AnimatorController.GetCurrentAnimatorStateInfo(danceLayerIndex);

            float animationDuration = currentState.length;
            var responseString = animationDuration.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            if (giuliaAIConfig.EnableDebugMode){
                Debug.Log("Dance time: " + responseString);
            }

            response.OutputStream.Flush();
            response.OutputStream.Close();

        responseInProgress = false;
    }




}