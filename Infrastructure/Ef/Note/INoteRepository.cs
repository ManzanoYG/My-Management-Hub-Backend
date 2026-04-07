using Infrastructure.Ef.DbEntities;
using System.Collections;

namespace Infrastructure.Ef.Note
{
    public interface INoteRepository
    {
        DbNote Create(Guid UserId, string Title, string Content, string Style);
        List<DbNote> GetAllPinned(Guid userId, int pageNumber, int pageSize);
    }
}
