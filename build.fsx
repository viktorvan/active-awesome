#r "paket:
nuget FSharp.Core 4.5.4
nuget Fake.Api.Slack
nuget Fake.Tools.Git
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"


open Fake.Core
open Fake.Api
open Fake.Tools

let webhookUrl = "https://hooks.slack.com/services/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"
let repositoryUrl = "https://github.com/viktorvan/active-awesome"

Target.create "Checkout" (fun _ ->
    Git.Repository.clone "." repositoryUrl "active-awesome" 
)

Target.create "Slack" (fun _ ->
    let command = "git log -1 --pretty=%B"
    let lastCommitMsg = 
        match Git.CommandHelper.runGitCommand repositoryUrl command with
        | true, stringList, str -> 
            sprintf "%A : %s" stringList str
        | false, _, _ -> ""

    lastCommitMsg |> printf "%s"

    // Slack.sendNotification webhookUrl (fun p ->
    //     { p with
    //         Text = "New tip!\n<https://github.com/viktorvan/active-awesome>!"
    //         Channel = "#active-awesome-test"
    //         IconEmoji = ":exclamation:"
    //     }) |> printfn "Result: %s"
)

Target.runOrDefault "Slack"