using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HttpServer : MonoBehaviour
{
    public string url = "http://localhost:";
    public int port = 4988;
    public Animator animator;
    public AudioController audioController;

    private bool running = false;
    private HttpListener listener;
    private Thread listenerThread;
    private Queue<Action<HttpListenerResponse>> responseActions = new Queue<Action<HttpListenerResponse>>();
    private Queue<HttpListenerContext> contextQueue = new Queue<HttpListenerContext>();
    private Queue<Action> actionsToExecute = new Queue<Action>();
    private Queue<string> scenesToLoad = new Queue<string>();

    public class QueuedResponse
    {
        public HttpListenerContext Context { get; set; }
        public Action<HttpListenerResponse> ResponseAction { get; set; }
    }
    private Queue<QueuedResponse> queuedResponses = new Queue<QueuedResponse>();

    void Start()
    {
        listener = new HttpListener();
        listener.Prefixes.Add(url + port.ToString() + "/");
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
                //response.OutputStream.Close();

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
            response.StatusCode = 200; // OK
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
                    Debug.Log("JSON ricevuto: " + json);

                    var data = JsonUtility.FromJson<JsonData>(json);
                    QueuedResponse queuedResponse = null;

                    if (data.event_type == "animator" && data.action == "trig")
                    {
                        queuedResponse = new QueuedResponse
                        {
                            Context = context,
                            ResponseAction = (resp) =>
                            {
                                Debug.Log("Animator trigger: " + data.parameter);
                                animator.SetTrigger(data.parameter);
                                audioController.PlayAudioForTrigger(data.parameter);

                                StartCoroutine(DelayedResponse(resp));
                            }
                        };
                    }
                    else if (data.event_type == "request" && data.parameter == "dancelist")
                    {
                        Debug.Log("Animation parameters requested");
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
                                response.OutputStream.Flush();
                                response.OutputStream.Close();

                            }
                        };
                    }
                    else if (data.event_type == "scene" && data.action == "change")
                    {
                        string sceneName = data.parameter;
                        // Add the scene name to the queue
                        scenesToLoad.Enqueue(sceneName);
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
            //context.Response.OutputStream.Close();
        }
        while (queuedResponses.Count > 0)
        {
            var queuedResponse = queuedResponses.Dequeue();
            if (queuedResponse?.Context?.Response != null)
            {
                queuedResponse.ResponseAction?.Invoke(queuedResponse.Context.Response);
            }
            //queuedResponse.Context.Response.OutputStream.Close();
        }
        while (scenesToLoad.Count > 0)
        {
            string sceneName = scenesToLoad.Dequeue();
            SceneManager.LoadScene(sceneName);
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
        AnimatorControllerParameter[] parameters = animator.parameters;
        string[] names = new string[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            names[i] = parameters[i].name;
        }

        // Debug log per verificare i nomi dei parametri
        Debug.Log("Nomi dei parametri dell'animatore: " + string.Join(", ", names));

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
        if (responseInProgress)
            yield break; 
        responseInProgress = true;
        yield return new WaitForSeconds(1f);
        if (responseInProgress)
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            float animationDuration = currentState.length;
            var responseString = animationDuration.ToString(); 
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Flush();
            response.OutputStream.Close();

            responseInProgress = false;
        }
    }

}