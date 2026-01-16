// For more information see https://aka.ms/fsharp-console-apps

let sum1 (a: int) (b: int) : int = a + b
let sum2 a b = a + b

let rec fib n =
    if n <= 1 then n else fib (n - 1) + fib (n - 2)

[<EntryPoint>]
let main args =
    printfn "Hello from F#"
    printfn "Arguments passed to function : %A" args
    printfn $"Arguments passed to function : %A{args}"

    let fibOf10 = fib 10
    printfn $"Fib(10) is {fibOf10}"

    let sumOf2And3 = sum1 2 3
    let sumOf3And4 = sum2 3 4

    // Return 0. This indicates success.
    0
