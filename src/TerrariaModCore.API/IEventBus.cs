using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Event bus allowing decoupled communication and event subscriptions across mods and core hooks.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribes a listener to a specific event type.
        /// </summary>
        void Subscribe<TEvent>(Action<TEvent> handler);

        /// <summary>
        /// Unsubscribes a listener from a specific event type.
        /// </summary>
        void Unsubscribe<TEvent>(Action<TEvent> handler);

        /// <summary>
        /// Publishes an event to all registered listeners.
        /// </summary>
        void Publish<TEvent>(TEvent eventArgs);
    }
}
