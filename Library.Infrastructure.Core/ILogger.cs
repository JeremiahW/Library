﻿using Library.Infrastructure.Core.Models;
using System;

namespace Library.Infrastructure.Core
{
    public interface ILogger
    {
        void Success(Guid commandUnqiueId, string commandName, string eventName, string message, object data);

        void Error(Guid commandUniqueId, string commandName, string eventName, string message, object data);

        void Info(Guid commandUniqueId, string commandName, string eventName, string message, object data);
    }
}
