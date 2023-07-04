#r "nuget: Suave, 2.6.2"
#r "nuget: FSharp.Data.Adaptive, 1.2.13"
#r "nuget: CliWrap, 3.6.0"
#r "nuget: Fake.IO.FileSystem, 6.0.0"

open System
open System.IO
open System.Net
open System.Threading
open System.Threading.Tasks
open CliWrap
open CliWrap.EventStream
open Suave
open Suave.Files
open Suave.Filters
open Suave.Operators
open Suave.RequestErrors
open Fake.IO
open Fake.IO.FileSystemOperators

let exitTask = TaskCompletionSource<unit> ()
let cts = new CancellationTokenSource ()

Console.CancelKeyPress.Add (fun _ ->
    printfn "Goodbye"
    cts.Cancel ()
    exitTask.SetResult ()
)

let port = 4000us

let fableArgs =
    "./App.fsproj -e .js -o ./out --fableLib fable-library"

let serveFiles =
    [
        GET >=> path "/" >=> file (__SOURCE_DIRECTORY__ </> "index.html")
        GET >=> browseHome
        NOT_FOUND "Page not found."
    ]

let fableHandle =
    Cli
        .Wrap("dotnet")
        .WithArguments($"fable watch {fableArgs} --define DEBUG")
        .WithWorkingDirectory(__SOURCE_DIRECTORY__)
        .Observe (cancellationToken = cts.Token)
    |> Observable.subscribe (fun ev ->
        match ev with
        | :? StartedCommandEvent as startedEvent -> printfn $"Fable Started: %i{startedEvent.ProcessId}"
        | :? StandardOutputCommandEvent as stdOutEvent -> printfn $"Fable: %s{stdOutEvent.Text}"
        | :? ExitedCommandEvent as exitedEvent -> printfn $"Fable Exited: %A{exitedEvent.ExitCode}"
        | :? StandardErrorCommandEvent as stdErrEvent -> printfn $"Fable ERR: %s{stdErrEvent.Text}"
        | _ -> ()
    )

let server = choose serveFiles

let conf =
    { defaultConfig with
        cancellationToken = cts.Token
        homeFolder = Some __SOURCE_DIRECTORY__
        compressedFilesFolder = Some (Path.GetTempPath ())
        bindings = [ HttpBinding.create HTTP IPAddress.Loopback port ]
    }

let _, suaveServer = startWebServerAsync conf server
Async.Start (suaveServer, cts.Token)
printfn $"Suave server started on http://localhost:%i{port}"

cts.Token.Register (fun _ ->
    fableHandle.Dispose ()
)
|> ignore

exitTask.Task.Wait ()