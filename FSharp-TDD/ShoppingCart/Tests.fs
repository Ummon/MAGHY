namespace ShoppingCart.Tests

open System
open NUnit.Framework
open FsUnit

open ShoppingCart
open Currency

[<TestFixture>]
type ``ISBN-13`` () =
    [<Test>]
    member this.``Creating an ISBN from a correct string`` () =
        let validISBN = "9781784391232"
        (ISBN13 validISBN).ToString () |> should equal validISBN

    [<Test>]
    member this.``Trying to create ISBNs from incorrect strings`` () =
        (fun () -> ISBN13 "9781784391233" |> ignore) |> should throw typeof<InvalidISBNString>
        (fun () -> ISBN13 "" |> ignore) |> should throw typeof<InvalidISBNString>
        (fun () -> ISBN13 "abcdef" |> ignore) |> should throw typeof<InvalidISBNString>
        (fun () -> ISBN13 null |> ignore) |> should throw typeof<InvalidISBNString>

[<TestFixture>]
type ``Currency`` () =
    do
        USD_CHF_rate <- 0.9M
        USD_XBT_rate <- 0.005M

    [<Test>]
    member this.``Equality`` () = 
        { Amount = 12M; Currency = USD } |> should equal { Amount = 10.8M; Currency = CHF }
        { Amount = 12M; Currency = USD } |> should not' (equal { Amount = 12M; Currency = CHF })

    [<Test>]
    member this.``Comparisons`` () =
        { Amount = 3M; Currency = USD } > { Amount = 1M; Currency = USD } |> should be True
        { Amount = 2M; Currency = CHF } > { Amount = 2.1M; Currency = USD } |> should be True
        { Amount = 1M; Currency = XBT } <= { Amount = 200M; Currency = USD } |> should be True
        { Amount = 1M; Currency = XBT } <= { Amount = 180M; Currency = CHF } |> should be True

    [<Test>]
    member this.``Matematical operations`` () =
        { Amount = 3M; Currency = USD } + { Amount = 1M; Currency = USD } |> should equal { Amount = 4M; Currency = USD }
        { Amount = 40M; Currency = USD } - { Amount = 9M; Currency = CHF } |> should equal { Amount = 30M; Currency = USD }
        10M * { Amount = 9M; Currency = XBT } |> should equal { Amount = 90M; Currency = XBT }
        { Amount = 5M; Currency = USD } - { Amount = 9M; Currency = CHF } * 2M |> should equal { Amount = -15M; Currency = USD }

(*[<TestFixture>]
type ``Elements`` () =
    [<Test>]
    member this.``Create an element with correct properties`` () =
        let elem = Element ("Testing with F#", *)

[<TestFixture>]
type ``Elements in a shopping cart`` () =
    [<Test>]
    member this.``Can we add an element to a cart?`` () =
        ()

    [<Test>] 
    member this.``Can we enumerate all items in a cart?`` () =
        ()
