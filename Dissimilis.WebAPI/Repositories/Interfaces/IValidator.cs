namespace Dissimilis.WebAPI.Repositories
{
    public interface IValidator<T>
    {
        bool IsValid(T obj);
    }
}