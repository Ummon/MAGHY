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
                    |> Array.mapi (fun i n -> (if i % 2 = 0 then 1I else 3I), n)
                    |> Array.fold (fun result (factor, n) -> result + factor * (bigint (int n))) 0I
                checksum % 10I = 0I
    do
        if not isValid then raise InvalidISBNString

    override this.ToString () =
        String (data |> Array.map (fun n -> (n.ToString ()).[0]))
