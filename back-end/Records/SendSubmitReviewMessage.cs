using back_end.Enums;

namespace back_end.Records
{
    public record SendSubmitReviewMessage(
        Guid userId,
        Guid vocabularyId,
        ReviewMethod method,
        string? answer,
        bool isCorrect
    )
    {
    }
}
