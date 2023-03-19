using CQRS.Core.Commands;

namespace Post.Cmd.Api.Commands
{
  public class EditPostMessageCommand : BaseCommand
  {
    public string Message { get; set; } = "";
  }
}