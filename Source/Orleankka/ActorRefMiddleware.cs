﻿using System.Threading.Tasks;

namespace Orleankka
{
    public interface IActorRefMiddleware
    {
        Task<TResult> Send<TResult>(ActorPath actor, object message, Receive sender);
    }

    public abstract class ActorRefMiddleware : IActorRefMiddleware
    {
        readonly IActorRefMiddleware next;

        protected ActorRefMiddleware(IActorRefMiddleware next = null) => 
            this.next = next ?? DefaultActorRefMiddleware.Instance;

        public virtual Task<TResult> Send<TResult>(ActorPath actor, object message, Receive sender) => 
            next.Send<TResult>(actor, message, sender);
    }

    class DefaultActorRefMiddleware : IActorRefMiddleware
    {
        public static readonly DefaultActorRefMiddleware Instance = new DefaultActorRefMiddleware();

        public async Task<TResult> Send<TResult>(ActorPath actor, object message, Receive sender) => 
            (TResult) await sender(message);
    }
}