using Data.Model;
using Shared.Dto;

namespace Data
{
    public static class TeacherExtension
    {
        public static TeacherDto ToTeacherDto(this Teacher teacher) => new TeacherDto()
        {
            DiscordId = teacher.DiscordId ?? 0,
            Id = teacher.Id,
            Name = teacher.Name,
            Rank = teacher.Rank
        };
    }
}
