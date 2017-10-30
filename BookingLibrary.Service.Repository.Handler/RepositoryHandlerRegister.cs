using System;
using System.Reflection;
using System.Linq;
using BookingLibrary.Domain.Core;
using BookingLibrary.Domain.Core.Commands;
using BookingLibrary.Infrastructure.Messaging.RabbitMQ;
using BookingLibrary.Infrastructure.InjectionFramework;
using BookingLibrary.Domain.Core.DataAccessor;
using BookingLibrary.Domain.Core.Messaging;
using BookingLibrary.Infrastructure.Messaging.SignalR;

namespace BookingLibrary.Service.Repository.Handler
{
    public class RepositoryHandlerRegister
    {
        public RepositoryHandlerRegister()
        {

        }

        public void RegisterAndStart()
        {
            var connectionString = InjectContainer.GetInstance<IEventDBConnectionStringProvider>().ConnectionString;
            InjectContainer.RegisterType<ICommandTracker, SignalRCommandTracker>();

            Console.WriteLine($"Handler starting...");
            Console.WriteLine($"Event DB Connection String: {connectionString}");

            RegisterAndStartCommandHandlers();
            RegisterAndStartEventHandlers();

            Console.WriteLine($"Handler started.");
        }

        public void RegisterAndStartCommandHandlers()
        {
            var assembly = Assembly.Load("BookingLibrary.Service.Repository.Domain");

            var allCommands = assembly.GetExportedTypes().Where(p => p.GetInterface("ICommand") != null);
            foreach (var command in allCommands)
            {
                var register = new RabbitMQCommandSubscriber("amqp://localhost:5672");
                var registerMethod = register.GetType().GetMethod("Subscribe");

                var cmd = Activator.CreateInstance(command);
                Console.WriteLine($"Find command {command.FullName}.");
                registerMethod.MakeGenericMethod(command).Invoke(register, new object[1] { cmd });
            }
        }

        public void RegisterAndStartEventHandlers()
        {

            var assembly = Assembly.Load("BookingLibrary.Service.Repository.Domain");

            var allEvents = assembly.GetExportedTypes().Where(p => p.GetInterface("IDomainEvent") != null);
            foreach (var @event in allEvents)
            {
                var register = new RabbitMQEventSubscriber("amqp://localhost:5672", InjectContainer.GetInstance<ICommandTracker>());
                var registerMethod = register.GetType().GetMethod("Subscribe");

                var cmd = Activator.CreateInstance(@event);
                Console.WriteLine($"Find event {@event.FullName}.");
                registerMethod.MakeGenericMethod(@event).Invoke(register, new object[1] { cmd });
            }
        }
    }
}