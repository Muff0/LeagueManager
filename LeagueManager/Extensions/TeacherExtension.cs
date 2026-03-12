using Data.Model;
using LeagueManager.ViewModel;
using Shared.Dto;

namespace LeagueManager.Extensions
{
    public static class TeacherExtension
    {
        public static TeacherViewModel ToTeacherViewModel(this Teacher teacher) => new TeacherViewModel()
        {
            Id = teacher.Id,
            Name = teacher.Name
        };
    }
}
