using Akka.Persistence;

namespace AkkaNpgsqlError;

public class TestPersistentActor : UntypedPersistentActor
{
    public override string PersistenceId => "example";
    private object _state = new();

    private void UpdateState(string e)
    {
        Console.WriteLine($"Updating state with {e}");
        _state = new { Data = e };
    }

    protected override void OnCommand(object message)
    {
        if (message is SaveSnapshotSuccess s)
        {
            Console.WriteLine($"{s.Metadata.SequenceNr} saved");
        }
        else if (message is SaveSnapshotFailure f)
        {
            Console.WriteLine($"{f.Metadata.SequenceNr} failed");
        }
        else
        {
            Persist(
                $"evt-{message}",
                e =>
                {
                    UpdateState(e);
                    SaveSnapshot(_state);
                }
            );
        }
    }

    protected override void OnRecover(object message)
    {
        if (message is SnapshotOffer offeredSnapshot)
        {
            _state = offeredSnapshot.Snapshot;
        }
        else if (message is RecoveryCompleted)
        {
            Console.WriteLine("Recovery completed");
        }
        else
        {
            Console.WriteLine($"Recovering {message.GetType()}");
        }
    }
}
