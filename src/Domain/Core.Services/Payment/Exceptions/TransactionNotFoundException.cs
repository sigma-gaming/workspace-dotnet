namespace Core.Services.Payment.Exceptions;

/// <summary>
///     Exception for transaction not found errors
/// </summary>
public class TransactionNotFoundException : PaymentException
{
    public TransactionNotFoundException(string identifier, string transactionType)
        : base($"{transactionType} with identifier {identifier} not found")
    {
        Identifier = identifier;
        Type = transactionType;
    }

    public TransactionNotFoundException(Guid id, string transactionType)
        : this(id.ToString(), transactionType)
    {
    }

    public string Identifier { get; }
    public string Type { get; }
}