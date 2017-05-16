using Microsoft.AspNetCore.Http;
using Narato.ResponseMiddleware.Correlations.Interfaces;
using System;

namespace Narato.ResponseMiddleware.Correlations
{
    public class CorrelationIdProvider : ICorrelationIdProvider
    {
        private const string CORRELATION_ID_HEADER_NAME = "Nar-Correlation-Id";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCorrelationId()
        {
            if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.Request == null)
            {
                throw new Exception("Correlation Id was asked when not in a request context");
            }
            Guid guid;
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(CORRELATION_ID_HEADER_NAME))
            {
                var correlationId = _httpContextAccessor.HttpContext.Request.Headers[CORRELATION_ID_HEADER_NAME];
                if (Guid.TryParse(correlationId, out guid))
                    return guid;
            }
            if (_httpContextAccessor.HttpContext.Response.Headers.ContainsKey(CORRELATION_ID_HEADER_NAME))
            {
                var correlationId = _httpContextAccessor.HttpContext.Response.Headers[CORRELATION_ID_HEADER_NAME];
                if (Guid.TryParse(correlationId, out guid))
                    return guid;
            }

            guid = Guid.NewGuid();
            // TODO: warning: this is NOT safe.... if response has already been (partly) sent, this will cause some weird stuff to happen...
            _httpContextAccessor.HttpContext.Response.Headers.Add(CORRELATION_ID_HEADER_NAME, guid.ToString());
            return guid;
        }
    }
}
