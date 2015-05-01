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
    member this.``Printing`` () =
        (Money (42, USD)).ToString () |> should equal "42 USD"
        (Money (42.33M, USD)).ToString () |> should equal "42.33 USD"

    [<Test>]
    member this.``Equality`` () = 
        Money (12, USD) |> should equal (Money (10.8M, CHF))
        Money (12, USD) |> should not' (equal (Money (12, CHF)))

    [<Test>]
    member this.``Comparisons`` () =
        Money (3, USD) > Money (1, USD) |> should be True
        Money (2, CHF) > Money (2, USD) |> should be True
        Money (1, XBT) <= Money (200, USD) |> should be True
        Money (1, XBT) <= Money (180, CHF) |> should be True

    [<Test>]
    member this.``Matematical operations`` () =
        Money (3, USD) + Money (1, USD) |> should equal (Money (4, USD))
        Money (40, USD) - Money (9, CHF) |> should equal (Money (30, USD))
        10 * Money (9, XBT) |> should equal (Money (90, XBT))
        Money (5, USD) - Money (9, CHF) * 2M |> should equal (Money (-15, USD))

(*[<TestFixture>]
type ``Books`` () =
    [<Test>]
    member this.``Create a book with correct properties`` () =
        let elem = Element ("Testing with F#", *)

[<TestFixture>]
type ``Elements in a shopping cart`` () =
    [<Test>]
    member this.``Can we add an element to a cart?`` () =
        ()

    [<Test>] 
    member this.``Can we enumerate all items in a cart?`` () =
        ()
