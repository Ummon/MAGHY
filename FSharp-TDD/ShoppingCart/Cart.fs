namespace ShoppingCart

open Currency

exception WrongAmountToPaidWith
exception Can'tPaidForAnEmptyCart
exception Can'tChangeAPaidCart

type Cart = 
    | Empty 
    | Active of Book list
    | PaidFor of Book list * Money

    static member Create = 
        Empty

    member this.Size : int = 
        match this with
        | Empty -> 0
        | Active books -> List.length books
        | PaidFor (books, _) -> List.length books

    member this.Add (book : Book) : Cart =
        match this with
        | Empty -> Active [book]
        | Active books -> Active (book :: books)
        | PaidFor _ -> raise Can'tChangeAPaidCart

    member this.Books : Book seq = 
        match this with
        | Empty -> Seq.empty
        | Active books -> List.toSeq books
        | PaidFor (books, _) -> List.toSeq books

    member this.Remove (book : Book) : Cart = 
        match this with
        | Empty -> this
        | Active books -> match List.filter ((<>) book) books with
                          | [] -> Empty
                          | books -> Active books
        | PaidFor _ -> raise Can'tChangeAPaidCart

    // TODO: Round to two decimals.
    member this.Amount : Money =
        let amount = List.map (fun (b : Book) -> b.Price) >> List.reduce (+)
        match this with
        | Empty -> Money (0, USD)
        | Active books -> amount books
        | PaidFor (books, _) -> amount books

    member this.Paid (amount : Money) : Cart =
        match this with
        | Empty -> raise Can'tPaidForAnEmptyCart
        | Active books -> if this.Amount = amount 
                          then PaidFor (books, amount)
                          else raise WrongAmountToPaidWith
        | PaidFor (books, _) -> raise Can'tChangeAPaidCart

    member this.AmountPaid : Money =
        match this with
        | Empty -> Money (0, USD)
        | Active books -> Money (0, USD)
        | PaidFor (_, amount) -> amount



