using Infrastructure.Ef.DbEntities;
using Infrastructure.Services.CryptoService;
using System.Collections;

namespace Infrastructure.Ef.Note
{
    public class NoteRepository : INoteRepository
    {
        private readonly ManagementHubContext _context;
        private readonly CryptoService _cryptoService;

        public NoteRepository(ManagementHubContext context, CryptoService cryptoService)
        {
            _context = context;
            _cryptoService = cryptoService;
        }

        public DbNote Create(Guid UserId, string Title, string Content, string Style)
        {
            var note = new DbNote
            {
                UserId = UserId,
                Title = Title,
                Content = _cryptoService.Encrypt(Content),
                Style = Style
            };
            _context.Notes.Add(note);
            _context.SaveChanges();
            return note;
        }

        public List<DbNote> GetAllPinned(Guid userId, int pageNumber, int pageSize)
        {
            var notes = _context.Notes
                .Where(n => n.UserId == userId && n.IsPinned && !n.IsArchived)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var note in notes)
            {
                note.Content = _cryptoService.Decrypt(note.Content);
            }

            return notes;
        }
    }
}
