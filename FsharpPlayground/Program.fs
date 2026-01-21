// For more information see https://aka.ms/fsharp-console-apps

open System
open System.Net.Http
open FsToolkit.ErrorHandling

// 💠 Mới copy 1 đoạn code mẫu về async expression của F# trên docs bỏ vô chạy thử trong JetBrains Rider,
// đúng kiểu Functional Programming — đọc code thấy sướng người 😄. F# mang vibe OCaml, nhưng có chút gì đó Haskell.
// 💠 urlList
// • urlList là một value (không phải hàm), có kiểu (string * string) list
// • Mỗi phần tử là tuple (name, url)
let urlList: (string * string) list =
    [ "Microsoft.com", "http://www.microsoft.com/"
      "MSDN", "http://msdn.microsoft.com/"
      "Bing", "http://www.bing.com" ]

// 💠 fetchAsync
// • fetchAsync nhận 2 tham số name, url và trả về Async<unit>
// (Async<'T> trong F# tương đương với Task<T> bên C#, nhưng Lazy).

// • async { ... } là async computation expression
// Bên trong dùng các keyword kiểu monadic comprehension (let!, return, …)

// • use httpClient: vì HttpClient implement IDisposable nên dùng use để auto-dispose khi async workflow kết thúc
// (tương tự using trong C# nhưng an toàn với async)

// • GetStringAsync trả về Task<string> → cần convert sang Async<string> bằng Async.AwaitTask.
// Sau đó dùng let! để unwrap (let! ~~ await trong C#, trong FP gọi là bind())
let fetchAsync (name: string, url: string) : Async<unit> =
    async {
        try
            let uri = System.Uri(url)
            use httpClient = new HttpClient()
            let! html = httpClient.GetStringAsync(uri) |> Async.AwaitTask
            printfn $"Read {html.Length} characters for {name}"
        with (ex: exn) ->
            printfn $"Failed: {ex.Message}"
    }

// 💠 runAll
// • Pipe operator (|>) lấy kết quả bên trái truyền làm tham số đầu tiên cho hàm bên phải
// Về bản chất:
// let inline (|>) x f = f x
// • Seq.map fetchAsync: map list (string * string) thành Seq<Async<unit>>
// • Async.Parallel: gom nhiều Async<unit> thành Async<unit array>
// (tương tự Task.WhenAll bên C#)
// • Async.RunSynchronously: chạy blocking và chờ async hoàn tất
// (tương tự Task.GetAwaiter().GetResult() — thường chỉ dùng trong console/demo)
// • ignore: bỏ qua kết quả, luôn trả về unit
// 💠💠💠
// Tổng thể, code rất “FP đúng nghĩa”:
// • data flow rõ ràng từ trên xuống
// • side-effect bị nhốt trong Async
// • không ceremony, không noise
// Cảm giác đọc rất đã 😄
let runAll () =
    urlList
    |> Seq.map fetchAsync
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore

let sum1 (a: int) (b: int) : int = a + b
let sum2 a b = a + b

let id x = x
let lenOfString (s: string) = s.Length

let rec fib n =
    if n <= 1 then n else fib (n - 1) + fib (n - 2)

[<Struct>]
type UserStruct = { Name: string; Age: int }

type UserClass = { Name: string; Age: int }

let performStep1 () =
    printfn "performStep1 is calling..."
    42

let performStep2 step1Result =
    printfn $"performStep2 is calling with step1Result={step1Result}"
    step1Result * 2 + 24

let expensiveCalculation =
    lazy
        (let step1 = performStep1 ()
         let step2 = performStep2 step1
         $"{step1}-and-{step2}")

let getResult1 () : Result<int, string> =
    let random = Random.Shared.Next()

    if random % 2 = 0 then
        Ok random
    else
        Error $"Got an odd number: {random}"

let getResult2 (result1: int) : Result<int, string> =
    let random = Random.Shared.Next()
    let number = result1 + random

    if number % 2 <> 0 then
        Ok number
    else
        Error $"Got an even number: {number}"

let getResult3 (result1: int) (result2: int) = result1 + result2

let finalResult () : Result<int, string> =
    // Result Computation Expression from FsToolkit.ErrorHandling
    result {
        let! result1 = getResult1 ()
        let! result2 = getResult2 result1
        return getResult3 result1 result2
    }

let finalResult2 () : Result<int, string> =
    getResult1 ()
    |> Result.bind (fun result1 -> getResult2 result1 |> Result.map (getResult3 result1))

[<EntryPoint>]
let main args =
    runAll ()

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

    let user: UserStruct = { Name = "Hoc"; Age = 28 }
    let copiedUser: UserStruct = { user with Age = 30 }
    printfn $"user is {user}"
    printfn $"copiedUser is {copiedUser}"

    let userCls: UserClass = { Name = "Hoc"; Age = 28 }
    let copiedUserCls: UserClass = { userCls with Age = 30 }
    printfn $"userCls is {userCls}"
    printfn $"copiedUserCls is {copiedUserCls}"

    let cal = expensiveCalculation
    printfn $"cal before evaluation"
    let forced1 = cal.Force()
    let forced2 = cal.Value
    printfn $"cal.Force() is {forced1}"
    printfn $"cal.Value is {forced2}"

    let res1 = finalResult ()
    let res2 = finalResult2 ()

    match res1 with
    | Ok resultValue -> printfn $"res1 is ok: {resultValue}"
    | Error errorValue -> printfn $"res1 is error: {errorValue}"

    match res2 with
    | Ok resultValue -> printfn $"res2 is ok: {resultValue}"
    | Error errorValue -> printfn $"res2 is error: {errorValue}"

    // Return 0. This indicates success.
    0
