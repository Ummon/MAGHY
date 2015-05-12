namespace ShoppingCart

open System

exception InvalidISBNString

/// An ISBN-13, see: http://en.wikipedia.org/wiki/International_Standard_Book_Number
type ISBN13 (str: string) =
    let data = (if str = null then [||] else str.ToCharArray ()) |> Array.choose (fun c ->
        match Char.GetNumericValue c with
        | n when n <> -1.0 -> Some (byte n)
        | _ -> None)
    let isValid =
            if data.Length <> 13 
            then false
            else
                let checksum =
                    data
                    |> Array.mapi (fun i n -> (if i % 2 = 0 then 1 else 3), n)
                    |> Array.fold (fun result (factor, n) -> result + factor * (int n)) 0
                checksum % 10 = 0
    do
        if not isValid then raise InvalidISBNString

    member private this.Data = data

    interface IEquatable<ISBN13> with
        member this.Equals other =
            data = other.Data

    override this.Equals obj =
        match obj with
        | :? ISBN13 as other -> (this :> IEquatable<_>).Equals other
        | _ -> invalidArg "obj" "not a Book object"

    override this.GetHashCode () =
        hash data

    override this.ToString () =
        String(data |> Array.map (fun n -> (string n).[0]))

exception InvalidBookData of string

type Book (title: string, author : string, year : int, ISBN13 : ISBN13, price : Currency.Money) =
    do
        if author = "" then raise <| InvalidBookData "Author is empty"
        if author = null then raise <| InvalidBookData "Author is null"
        if title = "" then raise <| InvalidBookData "Title is empty"
        if title = null then raise <| InvalidBookData "Title is null"
        if price.Amount = 0M then raise <| InvalidBookData "Price is 0" 
        if price.Amount < 0M then raise <| InvalidBookData "Price is less than 0"
    member this.Title = title
    member this.Author = author
    member this.Year = year
    member this.ISBN13 = ISBN13
    member this.Price = price

    interface IEquatable<Book> with
        member this.Equals other =
            ISBN13 = other.ISBN13

    override this.Equals obj =
        match obj with
        | :? Book as other -> (this :> IEquatable<_>).Equals other
        | _ -> invalidArg "obj" "not a Book object"

    override this.GetHashCode () =
        hash ISBN13


