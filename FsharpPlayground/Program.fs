// For more information see https://aka.ms/fsharp-console-apps

open System.Linq

let sum1 (a: int) (b: int) : int = a + b
let sum2 a b = a + b

let id x = x
let lenOfString (s: string) = s.Length

let rec fib n =
    if n <= 1 then n else fib (n - 1) + fib (n - 2)

[<Struct>]
type UserStruct = { name: string; age: int }

type UserClass = { name: string; age: int }

[<EntryPoint>]
let main args =
    printfn "Hello from F#"
    printfn "Arguments passed to function : %A" args
    printfn $"Arguments passed to function : %A{args}"

    let fibOf10 = fib 10
    printfn $"Fib(10) is {fibOf10}"

    let sumOf2And3 = sum1 2 3
    let sumOf3And4 = sum2 3 4

    let idOfInt: int = id 2
    let idOfStr: string = id "hello"
    let idOfTuple: (int * int * int) list = id [ 1, 2, 3 ]
    let idOfList: int list = id [ 1; 2; 3 ]

    let user: UserStruct = { name = "Hoc"; age = 28 }
    let copiedUser: UserStruct = { user with age = 30 }
    printfn $"user is {user}"
    printfn $"copiedUser is {copiedUser}"

    // Return 0. This indicates success.
    0
