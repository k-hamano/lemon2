﻿namespace Lemon

module RequestModule =
  open System
  open System.IO
  open System.Web
  open System.Xml.Linq
  
  let nameValueCollections2List (rawHeaders: System.Collections.Specialized.NameValueCollection)  =
    [ for key in rawHeaders.Keys -> (key , rawHeaders.[key])]

  let existHeader (name:string) (headers:(string * string) list) = List.exists (fun kvp -> fst kvp = name) headers

  let readText (st:Stream) = use reader = new StreamReader (st) in reader.ReadToEnd ()

  let readXml  (st:Stream) = readText st |> XElement.Parse

  let readParameters (st:Stream) = readText st |> HttpUtility.ParseQueryString |> nameValueCollections2List

  let (|Has|_|) key (kvp : (string * string) list) =
    let kvp = dict kvp
    if kvp.ContainsKey key then
      kvp.[key] |> Some
    else
      None

  let (|Last|_|) (pathes:string list) =
   if not <| List.isEmpty pathes then
     List.rev pathes |> List.head |> Some
   else
     None

  let (|RawUrl|) (req:Request) = req.RawUrl

  let (|Url|) (req:Request) =
    let pathes = req.Url.LocalPath.Split ([| "/" |], StringSplitOptions.RemoveEmptyEntries)
    List.ofArray pathes

  let (|Params|) (req:Request) =
    req.QueryString |> nameValueCollections2List
  
  let (|Headers|) (req:Request) =
    req.Headers |> nameValueCollections2List

  let (|GET|_|) (req:Request) =
    if req.HttpMethod = "GET" then
      Some req
    else
      None

  let (|POST|_|) (req:Request) =
    if req.HttpMethod = "POST" then
      Some (req, req.InputStream)
    else
      None

  let (|PUT|_|) (req:Request) =
    if req.HttpMethod = "PUT" then
      Some (req, req.InputStream)
    else
      None

  let (|DELETE|_|) (req:Request) =
    if req.HttpMethod = "DELETE" then
      Some req
    else
      None
    