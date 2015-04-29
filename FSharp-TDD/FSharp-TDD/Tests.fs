namespace ShoppingCart.Tests

open System
open NUnit.Framework
open FsUnit
open ShoppingCart

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
type ``Elements`` () =
    [<Test>]
    member this.``Create an element with correct properties`` () =
        ()

[<TestFixture>]
type ``Elements in a shopping cart`` () =
    [<Test>]
    member this.``Can we add an element to a cart?`` () =
        ()

    [<Test>] 
    member this.``Can we enumerate all items in a cart?`` () =
        ()
