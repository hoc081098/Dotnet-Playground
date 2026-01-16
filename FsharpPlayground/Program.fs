// For more information see https://aka.ms/fsharp-console-apps

[<EntryPoint>]
let main args =
    printfn "Hello from F#"
    printfn "Arguments passed to function : %A" args
    printfn $"Arguments passed to function : %A{args}"
    // Return 0. This indicates success.
    0