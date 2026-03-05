using NotesApp.DTO.Notes;

namespace NotesApp.Service.Notes;

public interface INoteService
{
    Task<NoteResponseDto> CreateNoteAsync(Guid userId, CreateNoteDto dto);
    Task DeleteNoteAsync(Guid userId, Guid noteId);
    Task<UpdateNoteDto> EditNoteAsync(Guid userId, Guid noteId, string newContent);
    Task<NoteResponseDto> GetNoteAsync(Guid userId, Guid noteId);
    Task<List<NoteResponseDto>> GetAllNotesAsync(Guid userId);
}