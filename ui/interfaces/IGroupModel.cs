public interface IGroupModel
{
    public class GroupModel
    ;
    public string Name ;
    public int CreatedByUserId ;
    public ICollection<UserGroup> UserGropus ;
}