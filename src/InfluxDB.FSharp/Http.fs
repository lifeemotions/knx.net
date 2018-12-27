module InfluxDB.FSharp.Http

open System
open System.Net
open InfluxDB.FSharp.AsyncChoice

do if ServicePointManager.Expect100Continue then ServicePointManager.Expect100Continue <- false

type HttpMethod =
  | Get
  | Post of body: string

type Request =
    { Uri     : Uri
      Method  : HttpMethod
      Query   : Map<string,string>
      Timeout : int<msec>
      Proxy   : Proxy option }

type Response =
    { StatusCode : HttpStatusCode
      Headers    : Map<string,string>
      Body       : string option }

let createRequest ``method`` (uri : Uri) =
    { Uri = uri
      Method = ``method``
      Query = Map.empty
      Timeout = 100000<msec> // https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.timeout%28v=vs.110%29.aspx
      Proxy = None }

let withQueryStringItem name value request =
    { request with Query = request.Query.Add(name,value) }

let withQueryStringItems items request =
    items |> List.fold (fun req (n, v) -> withQueryStringItem n v req) request

let withProxy proxy request =
    { request with Proxy = Some proxy }

let private writeBody (body: string) (webRequest: HttpWebRequest) = asyncChoice {
    use! reqStream = webRequest.GetRequestStreamAsync() |> Async.AwaitTask |> Async.Catch
    let bytes = Text.Encoding.UTF8.GetBytes(body)
    return!
        reqStream.WriteAsync(bytes, 0, bytes.Length)
        |> Async.AwaitTaskVoid
        |> Async.Catch
}

let private toWebRequest request =
    let uri =
        let b = UriBuilder request.Uri
        b.Query <-
            request.Query
            |> Map.toSeq
            |> Seq.map (fun (k,v) -> sprintf "%s=%s" (Uri.EscapeDataString k) (Uri.EscapeDataString v))
            |> String.concat "&"
        b.Uri

    let webRequest = HttpWebRequest.Create(uri) :?> HttpWebRequest
    webRequest.AllowAutoRedirect <- true
    webRequest.Timeout <- int request.Timeout

    request.Proxy |> Option.iter (fun proxy ->
        let webProxy = WebProxy(proxy.Address, int proxy.Port)
        match proxy.Credentials with
        | ProxyCredentials.No -> webProxy.Credentials <- null
        | ProxyCredentials.Default -> webProxy.UseDefaultCredentials <- true
        | ProxyCredentials.Custom { Username = name; Password = pwd } ->
            webProxy.Credentials <- NetworkCredential(name, pwd)
        webRequest.Proxy <- webProxy)

    match request.Method with
    | Get ->
        webRequest.Method <- "GET"
        asyncChoice.Return webRequest
    | Post body ->
        webRequest.Method <- "POST"
        writeBody body webRequest
        |> AsyncChoice.map (fun () -> webRequest)
        |> AsyncChoice.mapFail HttpError.otherErrorExn

let private getResponseNoException (webRequest: HttpWebRequest) = async {
    let! webResponse = webRequest.AsyncGetResponse() |> Async.Catch
    return
        match webResponse with
        | Ok webResponse -> Ok (webResponse :?> HttpWebResponse)
        | Fail e ->
            match e with
            | :? WebException as wex ->
                if isNull wex.Response then
                    Fail (OtherError (e.Message, Some e))
                else
                    Ok (wex.Response :?> HttpWebResponse)
            | e -> Fail (OtherError (e.Message, Some e))
}

let private getHeaders (response: HttpWebResponse) =
    response.Headers.Keys
    |> Seq.cast<string>
    |> Seq.map (fun key -> key, response.Headers.Item(key))
    |> Map.ofSeq

let private readResponseBody (webResponse: HttpWebResponse) = asyncChoice {
    if webResponse.ContentLength > 0L then
        use stream = webResponse.GetResponseStream()
        use sr = new IO.StreamReader(stream)
        let! body =
            sr.ReadToEndAsync()
            |> Async.AwaitTask
            |> Async.Catch
            <!!> HttpError.otherErrorExn
        return Some body
    else
        return None
}

let getResponse request : Async<Choice<Response,HttpError>> = asyncChoice {
    let! webRequest = toWebRequest request
    let! webResponse = getResponseNoException webRequest
    let! responseBody = readResponseBody webResponse
    return
        { StatusCode = webResponse.StatusCode
          Headers = getHeaders webResponse
          Body = responseBody }
}
