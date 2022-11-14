using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.DITest
{
    public class OperationLogger
    {
        private readonly ITransientOperation _transientOperation;
        private readonly IScopedOperation _scopedOperation;
        private readonly ISingletonOperation _singletonOperation;
        private ILogger<OperationLogger> _logger;

        public OperationLogger(
            ITransientOperation transientOperation,
            IScopedOperation scopedOperation,
            ISingletonOperation singletonOperation, ILogger<OperationLogger> logger)
        {
            (_transientOperation, _scopedOperation, _singletonOperation) =
                (transientOperation, scopedOperation, singletonOperation);

            _logger = logger;
        }

        public void LogOperations(string scope)
        {
            LogOperation(_transientOperation, scope, "Always different");
            LogOperation(_scopedOperation, scope, "Changes only with scope");
            LogOperation(_singletonOperation, scope, "Always the same");
        }


        private void LogOperation<T>(T operation, string scope, string message)
            where T : IOperation
        {
            _logger.LogDebug(
                $"{scope}: {typeof(T).Name,-19} [ {operation.OperationId}...{message,-23} ]");
        }
    }
}
