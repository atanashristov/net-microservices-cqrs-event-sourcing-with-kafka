using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
  public class RemovePostCommentCommand : BaseCommand
  {
    public Guid CommentId { get; set; }
    public string Username { get; set; } = "";

  }
}