using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TerrariaModCore.API;

namespace TerrariaModCore.Events
{
    /// <summary>
    /// Thread-safe event bus with isolated error boundary on handler execution.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<Type, List<Delegate>> _subscribers = new ConcurrentDictionary<Type, List<Delegate>>();
        private readonly object _lock = new object();

        public EventBus(ILogger logger)
        {
            _logger = logger;
        }

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) return;
            Type type = typeof(TEvent);

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(type, out var list))
                {
                    list = new List<Delegate>();
                    _subscribers[type] = list;
                }
                list.Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null) return;
            Type type = typeof(TEvent);

            lock (_lock)
            {
                if (_subscribers.TryGetValue(type, out var list))
                {
                    list.Remove(handler);
                }
            }
        }

        public void Publish<TEvent>(TEvent eventArgs)
        {
            Type type = typeof(TEvent);
            List<Delegate> handlers;

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(type, out var list) || list.Count == 0)
                {
                    return;
                }
                handlers = new List<Delegate>(list);
            }

            foreach (var del in handlers)
            {
                try
                {
                    if (del is Action<TEvent> action)
                    {
                        action(eventArgs);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Exception in event handler for {type.Name}", ex);
                }
            }
        }
    }
}
