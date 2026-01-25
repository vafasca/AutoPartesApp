using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.ValueObjects
{
    public class Money : IEquatable<Money>

    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "USD";

        // Validaciones de negocio
        public bool IsPositive() => Amount > 0;
        public bool IsZeroOrNegative() => Amount <= 0;
        // Constructor privado requerido por EF Core
        private Money() { }

        // Constructor público para crear instancias
        public Money(decimal amount, string currency = "USD")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("La moneda no puede estar vacía.", nameof(currency));

            if (currency.Length != 3)
                throw new ArgumentException("La moneda debe tener 3 caracteres (ISO).", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpper();
        }

        // Métodos de utilidad
        public override string ToString() => $"{Amount:0.00} {Currency}";

        // Comparación de igualdad (Value Object)
        public override bool Equals(object? obj) => Equals(obj as Money);

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override int GetHashCode() => HashCode.Combine(Amount, Currency);

        // Operaciones básicas
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("No se pueden sumar montos con diferentes monedas.");
            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException("No se pueden restar montos con diferentes monedas.");
            return new Money(Amount - other.Amount, Currency);
        }

    }
}
