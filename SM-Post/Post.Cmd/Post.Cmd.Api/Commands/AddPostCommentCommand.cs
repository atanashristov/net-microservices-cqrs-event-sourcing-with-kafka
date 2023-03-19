using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
  public class AddPostCommentCommand : BaseCommand
  {
    public string Comment { get; set; } = "";
    public string Username { get; set; } = "";
  }
}