using Shared.Enum;

namespace Shared.Dto
{
    public class SetReviewStatusInDto
    {
        public IEnumerable<ReviewDto> Reviews { get; set; } = [];
        public ReviewStatus NewStatus { get; set; }
    }
}
