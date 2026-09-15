using back_end.Enums;

namespace back_end.Exceptions
{
    public static class ReviewMethodExtensions
    {
        public static int GetScore(this ReviewMethod method)
        {
            return method switch
            {
                ReviewMethod.Flashcard => 1,
                ReviewMethod.MultipleChoice => 2,
                ReviewMethod.TranslateWord => 3,
                ReviewMethod.TranslateSentence => 4,
                ReviewMethod.ListenAndWrite => 5,
                _ => 0
            };
        }
    }
}
