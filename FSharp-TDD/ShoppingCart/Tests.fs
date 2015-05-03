namespace ShoppingCart.Tests

open System
open NUnit.Framework
open FsUnit

open ShoppingCart
open Currency

[<TestFixture>]
type ``ISBN-13`` () =
    [<Test>]
    member this.``Creating an ISBN from a correct string should be OK`` () =
        let validISBN = "9781784391232"
        (ISBN13 validISBN).ToString () |> should equal validISBN

    [<Test>]
    member this.``Trying to create an ISBN from an incorrect number should raise an exception`` () =
        (fun () -> ISBN13 "9781784391233" |> ignore) |> should throw typeof<InvalidISBNString>

    [<Test>]
    member this.``Trying to create an ISBN from an empty string should raise an exception`` () =
        (fun () -> ISBN13 "" |> ignore) |> should throw typeof<InvalidISBNString>

    [<Test>]
    member this.``Trying to create an ISBN from a string containing non-digits characters should raise an exception`` () = 
        (fun () -> ISBN13 "abcéà$_:;" |> ignore) |> should throw typeof<InvalidISBNString>

    [<Test>]
    member this.``Trying to create an ISBN from a null string should raise an exception`` () =
        (fun () -> ISBN13 null |> ignore) |> should throw typeof<InvalidISBNString>

[<TestFixture>]
type ``Currency`` () =
    [<TestFixtureSetUp>]
    member this.SetUp () =
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

[<TestFixture>]
type ``Books`` () =
    let title = "Doune"
    let author = "Frank Henenbert"
    let year = 1965
    let ISBN = ISBN13 "9781784391232"
    let price = Money (19.90M, USD)

    [<Test>]
    member this.``Create a book with correct properties`` () =
        let book = Book (title, author, year, ISBN, price)
        book.Title |> should equal title
        book.Author |> should equal author
        book.Year |> should equal year
        book.ISBN13 |> should equal ISBN
        book.Price |> should equal price

    [<Test>]
    member this.``Create a book with an empty title should raise an exception`` () = 
        (fun () -> Book ("", author, year, ISBN, price) |> ignore) |> should throw typeof<InvalidBookData>

    [<Test>]
    member this.``Create a book with a null title should raise an exception`` () = 
        (fun () -> Book (null, author, year, ISBN, price) |> ignore) |> should throw typeof<InvalidBookData>
        
    [<Test>]
    member this.``Create a book with a empty author should raise an exception`` () = 
        (fun () -> Book (title, "", year, ISBN, price) |> ignore) |> should throw typeof<InvalidBookData>

    [<Test>]
    member this.``Create a book with a nulll author should raise an exception`` () = 
        (fun () -> Book (title, null, year, ISBN, price) |> ignore) |> should throw typeof<InvalidBookData>

    [<Test>]
    member this.``Create a book with a zero price should raise an exception`` () = 
        (fun () -> Book (title, author, year, ISBN, Money (0, USD)) |> ignore) |> should throw typeof<InvalidBookData>

    [<Test>]
    member this.``Create a book with a negative price should raise an exception`` () = 
        (fun () -> Book (title, author, year, ISBN, Money (-42, USD)) |> ignore) |> should throw typeof<InvalidBookData>
        

[<TestFixture>]
type ``Shopping cart`` () =
    let book1 = Book ("Doune", "Frank Henenbert", 1965, ISBN13 "9782266233200", Money (19.90M, USD))
    let book2 = Book ("Doune²", "Frank Henenbert", 1968, ISBN13 "9782266235815", Money (0.1M, XBT))
    let book3 = Book ("Superion", "Pierre Simon", 1983, ISBN13 "9782266252584", Money (9, CHF))

    [<TestFixtureSetUp>]
    member this.SetUp () =
        USD_CHF_rate <- 0.9M
        USD_XBT_rate <- 0.005M

    [<Test>]
    member this.``We should be able to add an element to a cart`` () =
        let emptyCart = Cart.Create
        let cartWithOneBook = emptyCart.Add book1
        cartWithOneBook.Size |> should equal 1

    //[<Test>]
    //member this.``We shouldn't be able to add an element to a paid cart`` () =

    [<Test>] 
    member this.``We should be able to enumerate all items in a cart`` () =
        let cart = (Cart.Create.Add book1).Add book2
        let books = cart.Books
        Seq.length books |> should equal 2
        books |> should contain book1
        books |> should contain book2
        books |> should not' (contain book3)

    [<Test>]
    member this.``We should be able to remove a book from a non-empty cart`` () =
        let cart = ((Cart.Create.Add book1).Add book2).Remove book1
        let books = cart.Books
        Seq.length books |> should equal 1
        books |> should not' (contain book1)
        books |> should contain book2
        books |> should not' (contain book3)

    [<Test>]
    member this.``We should be able to remove a book from an empty cart`` () =
        let cart = Cart.Create.Remove book1
        let books = cart.Books
        Seq.length books |> should equal 0
        books |> should not' (contain book1)
        books |> should not' (contain book2)
        books |> should not' (contain book3)

    [<Test>]
    member this.``We should be able to ask the total amount of a given cart`` () =
        let cart1 = Cart.Create.Add book2
        let cart2 = ((Cart.Create.Add book1).Add book2).Add book3
        Cart.Create.Amount |> should equal (Money (0, USD)) 
        cart1.Amount |> should equal (Money (0.1M, XBT))
        cart2.Amount |> should equal (Money (49.9M, USD))

    [<Test>]
    member this.``We should be able to paid a non-empty cart with the right amount in any currency`` () =
        let cart = ((Cart.Create.Add book1).Add book2).Add book3
        (cart.Paid (Money (49.9M, USD))).AmountPaid |> should equal (Money (49.9M, USD))
        (cart.Paid (Money (49.9M, USD))).AmountPaid |> should equal (Money (0.2495M, XBT))

    [<Test>]
    member this.``We shouldn't be able to paid a non-empty cart with the wrong amount`` () =
        let cart = ((Cart.Create.Add book1).Add book2).Add book3
        (fun () -> cart.Paid (Money (39.9M, USD)) |> ignore) |> should throw typeof<WrongAmountToPaidWith>

    [<Test>]
    member this.``We shouldn't be able to paid an empty cart`` () =
        (fun () -> Cart.Create.Paid (Money (10, USD)) |> ignore) |> should throw typeof<Can'tPaidForAnEmptyCart>

    [<Test>]
    member this.``We shouldn't be able to paid an already paid cart`` () =
        let cart = ((Cart.Create.Add book1).Add book2).Add book3
        let paidCart = cart.Paid (Money (49.9M, USD))
        (fun () -> paidCart.Paid (Money (49.9M, USD)) |> ignore) |> should throw typeof<Can'tChangeAPaidCart>

    [<Test>]
    member this.``We shouldn't be able to add a book to an paid cart`` () =
        let cart = ((Cart.Create.Add book1).Add book2).Add book3
        let paidCart = cart.Paid (Money (49.9M, USD))
        (fun () -> paidCart.Add book3 |> ignore) |> should throw typeof<Can'tChangeAPaidCart>

    [<Test>]
    member this.``We shouldn't be able to remove a book to an paid cart`` () =
        let cart = ((Cart.Create.Add book1).Add book2).Add book3
        let paidCart = cart.Paid (Money (49.9M, USD))
        (fun () -> paidCart.Remove book3 |> ignore) |> should throw typeof<Can'tChangeAPaidCart>

    
