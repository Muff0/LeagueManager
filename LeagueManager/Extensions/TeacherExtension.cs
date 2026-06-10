using Data.Model;
using LeagueManager.ViewModel;

namespace LeagueManager.Extensions;

public static class TeacherExtension
{
    public static TeacherViewModel ToTeacherViewModel(this Teacher teacher)
    {
        return new TeacherViewModel
        {
            Id = teacher.Id,
            Name = teacher.Name
        };
    }
}