using CQRS.Core.Messages;

namespace CQRS.Core.Events
{
  public abstract class BaseEvent : Message
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">Needed for Kafka - the type discriminator</param>
    protected BaseEvent(string type)
    {
      this.Type = type;
    }
    public int Version { get; set; }
    /// <summary>
    /// Needed for Kafka - the type discriminator
    /// </summary>
    public string? Type { get; set; }
  }
}