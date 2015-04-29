module Currency

open System

let mutable USD_CHF_rate = 0.95M // CHF per 1 USD.
let mutable USD_XBT_rate = 0.00441M // Bitcoin per 1 USD.

type Currency = USD | CHF | XBT

let convert amount fromCurrency toCurrency =
    match fromCurrency, toCurrency with
    | USD, CHF -> amount * USD_CHF_rate
    | CHF, USD -> amount / USD_CHF_rate
    | USD, XBT -> amount * USD_XBT_rate
    | XBT, USD -> amount / USD_XBT_rate
    | _ -> amount

[<CustomComparison; CustomEquality>]
type Money = 
    { Amount : decimal; Currency : Currency }

    member this.Convert currency = { Amount = convert this.Amount this.Currency currency; Currency = currency }

    interface IComparable<Money> with
        member this.CompareTo other =
            compare (this.Convert USD).Amount (other.Convert USD).Amount

    interface IComparable with
        member this.CompareTo obj =
            match obj with
            | :? Money as other -> (this :> IComparable<_>).CompareTo other
            | _ -> invalidArg "obj" "not a Money"

    interface IEquatable<Money> with
        member this.Equals other =
            (this.Convert USD).Amount = (other.Convert USD).Amount

    override this.Equals obj =
        match obj with
        | :? Money as other -> (this :> IEquatable<_>).Equals other
        | _ -> invalidArg "obj" "not a Money"

    override this.GetHashCode () =
        hash (this.Convert USD).Amount

    static member (*) (m: Money, factor: decimal) : Money =
        { m with Amount = m.Amount * factor }

    static member (*) (factor: decimal, m: Money) : Money =
        m * factor

    static member (+) (m1: Money, m2: Money) =
        if m1.Currency <> m2.Currency
        then { Amount = (m1.Convert USD).Amount + (m2.Convert USD).Amount; Currency = USD }
        else { m1 with Amount = m1.Amount + m2.Amount }

    static member (-) (m1: Money, m2: Money) =
        if m1.Currency <> m2.Currency
        then { Amount = (m1.Convert USD).Amount - (m2.Convert USD).Amount; Currency = USD }
        else { m1 with Amount = m1.Amount - m2.Amount }

    override this.ToString () = 
        sprintf "%A %A" this.Amount this.Currency
