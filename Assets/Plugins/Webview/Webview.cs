using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Webview
{
    private const string WebviewFileName = "webview.html";

    private WebViewObject webViewObject;
    private int left, top, right, bottom;

    // Event to call when webview starts, receives message.
    public Action<string> OnWebviewStarted;

    // Event to call when avatar is created, receives GLB url.
    public Action<string> OnAvatarCreated;

    /// <summary>
    ///     Create webview object attached to a MonoBehaviour object
    /// </summary>
    /// <param name="parent">Parent game object.</param>
    public void CreateWebview(MonoBehaviour parent)
    {
        SetWebviewWindow();
        parent.StartCoroutine(LoadWebviewURL());
        SetScreenPadding(left, top, right, bottom);
    }

    /// <summary>
    ///     Set webview screen padding in pixels.
    /// </summary>
    public void SetScreenPadding(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;

        if (webViewObject)
        {
            webViewObject.SetMargins(left, top, right, bottom);
        }
    }

    private void SetWebviewWindow()
    {
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init(
            cb: LoadedSuccessful,
            started: OnWebviewStarted,
            zoom: false,
            enableWKWebView: true,
            wkContentMode: 0);

            #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            webViewObject.bitmapRefreshCycle = 1;
            #endif

        webViewObject.SetVisibility(true);
    }

    private IEnumerator LoadWebviewURL()
    {
            #if !UNITY_WEBPLAYER && !UNITY_WEBGL
            var src = System.IO.Path.Combine(Application.streamingAssetsPath, WebviewFileName);
            var dst = System.IO.Path.Combine(Application.persistentDataPath, WebviewFileName);
            byte[] result = null;
                
            if (src.Contains("://")) 
            {
                var unityWebRequest = UnityWebRequest.Get(src);
                yield return unityWebRequest.SendWebRequest();
                result = unityWebRequest.downloadHandler.data;
            } 
            else
            {
                result = System.IO.File.ReadAllBytes(src);
            }

            System.IO.File.WriteAllBytes(dst, result);

            webViewObject.LoadURL("file://" + dst);
            #else
            webViewObject.LoadURL("StreamingAssets/" + WebviewFileName);
            #endif

        yield break;
    }

    private void LoadedSuccessful(string message)
    {
        if (message.Contains(".glb"))
        {
            UnityEngine.Object.Destroy(webViewObject);
            OnAvatarCreated?.Invoke(message);
        }
    }
}
