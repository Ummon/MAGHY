module ShoppingCart.Currency

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

type Money (amount : decimal, currency : Currency) = 
    let convertTo toCurrency =
        convert amount currency toCurrency

    new (amount : int, currency : Currency) = Money(decimal amount, currency)
    member this.Amount = amount
    member this.Currency = currency
    member this.Convert toCurrency = Money(convertTo toCurrency, toCurrency)

    member this.Round (nbDecimal: int)  =
        Money(Math.Round (amount, nbDecimal), currency)

    interface IComparable<Money> with
        member this.CompareTo other =
            compare (convertTo USD) (convert other.Amount other.Currency USD)

    interface IComparable with
        member this.CompareTo obj =
            match obj with
            | :? Money as other -> (this :> IComparable<_>).CompareTo other
            | _ -> invalidArg "obj" "not a Money object"

    interface IEquatable<Money> with
        member this.Equals other =
            convertTo USD = convert other.Amount other.Currency USD

    override this.Equals obj =
        match obj with
        | :? Money as other -> (this :> IEquatable<_>).Equals other
        | _ -> invalidArg "obj" "not a Money object"

    override this.GetHashCode () =
        hash (convertTo USD)

    override this.ToString () = 
        sprintf "%O %A" amount currency

    static member (*) (m: Money, factor: decimal) : Money = Money(factor * m.Amount, m.Currency)
    static member (*) (factor: decimal, m: Money) : Money = m * factor
    static member (*) (m: Money, factor: int) : Money = m * (decimal factor)
    static member (*) (factor: int, m: Money) : Money = m * (decimal factor)
    static member (/) (m: Money, divisor: decimal) : Money = Money(m.Amount / divisor, m.Currency)
    static member (/) (m: Money, divisor: int) : Money = m / (decimal divisor)

    static member (+) (m1: Money, m2: Money) =
        if m1.Currency <> m2.Currency
        then m1.Convert USD + m2.Convert USD
        else Money(m1.Amount + m2.Amount, m1.Currency)

    static member (-) (m1: Money, m2: Money) =
        if m1.Currency <> m2.Currency
        then m1.Convert USD - m2.Convert USD
        else Money(m1.Amount - m2.Amount, m1.Currency)
