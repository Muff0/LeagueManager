namespace Shared.Dto;

public class ProcessPaymentDataOutDto
{
    public PaymentDataDto[] MissingRegistrations { get; set; } = [];
    public PaymentDataDto[] NoMatches { get; set; } = [];
}