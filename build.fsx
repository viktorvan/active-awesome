#r "paket:
nuget FSharp.Core 4.5.4
nuget FSharp.Data
nuget Fake.Api.Slack
nuget Fake.Tools.Git
nuget Fake.Core.Target //"
#load "./.fake/build.fsx/intellisense.fsx"


open Fake.Core
open Fake.IO
open Fake.Api
open Fake.Tools
open Fake.Core.TargetOperators
open FSharp.Data


let webhookUrl = "https://hooks.slack.com/services/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"
let repositoryUrl = "https://github.com/viktorvan/active-awesome"
let repositoryDir = "active-awesome"

Target.create "Checkout" (fun _ ->
    Shell.cleanDir repositoryDir
    Git.Repository.clone "." repositoryUrl repositoryDir
)

Target.create "Slack" (fun _ ->
    let getMessage = "log -1 --pretty=%B"
    let getAuthor = "--no-pager show -s --format='%an' HEAD~1"
    let lastCommitMsg = Git.CommandHelper.runSimpleGitCommand repositoryDir getMessage
    let lastCommitAuthor = Git.CommandHelper.runSimpleGitCommand repositoryDir getAuthor

    if lastCommitMsg.StartsWith("tip:") then
        let payload = lastCommitMsg.[4..]

        let text = sprintf "New tip from %s!\n%s\n%s" lastCommitAuthor payload repositoryUrl
        Slack.sendNotification webhookUrl (fun p ->
            { p with
                Text = text
                Channel = "#active-awesome-test"
                IconEmoji = ":exclamation:"
            }) |> printfn "Result: %s"
    else ()
)

"Checkout" 
    ==> "Slack"

Target.runOrDefault "Slack"