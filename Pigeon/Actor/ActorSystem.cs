﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pigeon.Actor;
using System.Collections.Concurrent;

namespace Pigeon.Actor
{
    public class ActorSystem : IActorRefFactory , IDisposable
    {
        private ActorCell rootCell;
        public ActorSystem(string name)
        {
            this.Name = name;
            this.DefaultDispatcher = new ThreadPoolDispatcher();

            this.rootCell = new ActorCell(this,"");            
            this.EventStream = rootCell.ActorOf<EventStreamActor>("EventStream");
            this.DeadLetters = rootCell.ActorOf<DeadLettersActor>("deadLetters");
            this.Guardian = rootCell.ActorOf<GuardianActor>("user");
            this.SystemGuardian = rootCell.ActorOf<GuardianActor>("system");
            this.TempGuardian = rootCell.ActorOf<GuardianActor>("temp");
        }

        public virtual string GetSystemName()
        {
            return this.Name;
        }

        public string Name { get;private set; }
        public LocalActorRef RootGuardian { get; private set; }

        public LocalActorRef EventStream { get; private set; }
        public LocalActorRef DeadLetters { get; private set; }
        public LocalActorRef Guardian { get; private set; }
        public LocalActorRef SystemGuardian { get; private set; }
        public LocalActorRef TempGuardian { get; private set; }

        public void Shutdown()
        {
            RootGuardian.Stop();
        }

        public void Dispose()
        {
            this.Shutdown();
        }

        public LocalActorRef ActorOf(Props props, string name = null)
        {
            return Guardian.Cell.ActorOf(props, name);
        }

        public LocalActorRef ActorOf<TActor>(string name = null) where TActor : ActorBase
        {
            return Guardian.Cell.ActorOf<TActor>( name);
        }

        public ActorSelection ActorSelection(ActorPath actorPath)
        {
            return Guardian.Cell.ActorSelection(actorPath);
        }

        public ActorSelection ActorSelection(string actorPath)
        {
            return rootCell.ActorSelection(actorPath);
        }

        public MessageDispatcher DefaultDispatcher { get; set; }

        internal protected virtual ActorRef GetRemoteRef(ActorCell actorCell, ActorPath actorPath)
        {
            throw new NotImplementedException();
        }

        public virtual string Address()
        {
            return "";
        }
    }
}