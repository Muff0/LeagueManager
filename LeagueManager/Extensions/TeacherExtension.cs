using Data.Model;
using LeagueManager.ViewModel;
using Shared.Dto;

namespace LeagueManager.Extensions
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
        public static TeacherViewModel ToTeacherViewModel(this Teacher teacher) => new TeacherViewModel()
        {
            Id = teacher.Id,
            Name = teacher.Name
        };
    }
}
