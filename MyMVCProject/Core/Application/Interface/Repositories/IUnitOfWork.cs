namespace MyMVCProject.Core.Application.Interface.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
