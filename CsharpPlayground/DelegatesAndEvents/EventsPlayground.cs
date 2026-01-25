namespace CsharpPlayground.DelegatesAndEvents;

// ============================================================================
// EVENTS PLAYGROUND
// Demonstrates the Event pattern in C# using:
// - EventHandler<T>
// - Publisher/Subscriber pattern
// - Multicast delegates
// - Subscription / unsubscription flow
// - How to invoke events safely
// ============================================================================
//
// This example mirrors typical .NET event usage.
//
// Notes:
// - EventHandler<T> in .NET 9+ no longer requires T : EventArgs
// - Events are multicast delegates under the hood
// - += and -= do NOT mutate the delegate; they create a NEW immutable multicast delegate chain
//   (Delegate.Combine / Delegate.Remove under the hood → always returns a new delegate instance)
// - Publisher invokes event via ?.Invoke(sender, args)
// - The `event` keyword enforces ENCAPSULATION for multicast delegates:
//     * Outside code can only += or -= (subscribe/unsubscribe)
//     * Outside code CANNOT invoke the delegate (no caller outside the publisher can do `OnEvent(...)`)
//     * Outside code CANNOT replace the entire delegate instance (no assignment `OnEvent = handler`)
//     * Only the declaring class can raise the event via OnEvent?.Invoke(...)
//   → This prevents accidental or malicious invocation from subscribers and preserves publisher authority.
// ============================================================================

public static class EventsPlayground
{
    public static void Run()
    {
        var publisher = new EventPublisher<string>();
        Console.WriteLine("=== EVENTS DEMO ===");
        Console.WriteLine("Publisher: " + publisher.GetHashCode());

        EventHandler<string> handler1 = (sender, args) =>
            Console.WriteLine($"[Handler 1] Event received with args: {args} from {sender?.GetHashCode()}");

        EventHandler<string> handler2 = (sender, args) =>
            Console.WriteLine($"[Handler 2] Event received with args: {args} from {sender?.GetHashCode()}");

        publisher.EventReceived += handler1;
        publisher.EventReceived += handler2;
        publisher.EventReceived += Handle3;

        publisher.Emit("Event 1 " + DateTimeOffset.Now);
        publisher.Emit("Event 2 " + DateTimeOffset.Now);

        publisher.EventReceived -= handler1;
        publisher.Emit("Event 3 " + DateTimeOffset.Now);

        publisher.EventReceived -= handler2;
        publisher.Emit("Event 4 " + DateTimeOffset.Now);

        publisher.EventReceived -= Handle3;
        publisher.Emit("An event with no handlers " + DateTimeOffset.Now);

        // Without `event` keyword, this would be possible. But with `event`, this line causes a compile-time error:
        // ```
        // The event 'EventReceived' can only appear on the left hand side of += or -=
        // (except when used from within the class 'CsharpPlayground.DelegatesAndEvents.EventsPlayground.EventPublisher<T>')
        // ```
        // publisher.EventReceived = null;
    }

    private static void Handle3(object? sender, string value) =>
        Console.WriteLine($"[Handler 3] Event received with value: {value} from {sender?.GetHashCode()}");

    private sealed class EventPublisher<T>
    {
        // `event` keyword enforces encapsulation for multicast delegates.
        // - Only this class can invoke or modify (reassign) the delegate instance.
        // - Outside code can only subscribe/unsubscribe via += and -=.
        public event EventHandler<T>? EventReceived;

        public void Emit(T @event)
        {
            // EventReceived is null if there are no subscribers
            EventReceived?.Invoke(this, @event);
        }
    }
}