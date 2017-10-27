namespace EasyProg.Models.Interfaces
{
    public interface IIdentity<TKey>
    {
		TKey Id { get; set; }
    }
}
