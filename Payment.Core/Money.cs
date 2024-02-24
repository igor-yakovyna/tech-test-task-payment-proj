using System;

namespace Payment.Core;

public struct Money : IEquatable<Money>
{
    public long Value { get; }
    public Currency Currency { get; }

    public Money(long amount, Currency currency)
    {
        Value = amount;
        Currency = currency;
    }

    public static Money Euro(long amount)
    {
        return new Money(amount, Currency.EUR);
    }

    public static Money Usd(long amount)
    {
        return new Money(amount, Currency.USD);
    }

    public static Money operator +(Money a, Money b) => new Money(a.Value + b.Value, b.Currency);
    public static bool operator ==(Money a, Money b) => a.Equals(b);
    public static bool operator !=(Money a, Money b) => !(a == b);
    public static Money operator -(Money a) => new Money(-a.Value, a.Currency);

    public bool Equals(Money other)
    {
        return Value == other.Value && Currency == other.Currency;
    }

    public override bool Equals(object obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Value.GetHashCode() * 397) ^ (int)Currency;
        }
    }
}