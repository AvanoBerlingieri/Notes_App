using NotesApp.DTO.Collaborators;

namespace NotesApp.Service.Collab;

public interface ICollabService
{
    Task<CollaboratorResponseDto> AddCollaborator(CreateCollaboratorDto createCollaboratorResponse, Guid noteOwnerId);

    Task RemoveCollaborator(DeleteCollaboratorDto dto, Guid noteOwnerId);
    
    Task<CollaboratorResponseDto> UpdateRole(UpdateRoleDto dto, Guid noteOwnerId);
    Task<List<CollaboratorResponseDto>> GetCollaborators(Guid noteId, Guid userId);
}