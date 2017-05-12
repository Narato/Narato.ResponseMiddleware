using System;

namespace Narato.ResponseMiddleware.Correlations.Interfaces
{
    public interface ICorrelationIdProvider
    {
        Guid GetCorrelationId();
    }
}
