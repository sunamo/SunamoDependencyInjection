namespace SunamoDependencyInjection;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class StaticLogger
{
    protected static ILogger Logger { get; set; } = NullLogger.Instance;
}