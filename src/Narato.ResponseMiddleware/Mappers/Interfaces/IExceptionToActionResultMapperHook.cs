using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Narato.ResponseMiddleware.Mappers.Interfaces
{
    // This interface has the same signature as IExceptionToActionResultMapper
    // because it basically is an extension point that handles exactly the same problem.
    // It gives you the ability to short-circuit the default implementations.
    // TODO: all current "implementations" should also be turned into hooks. Only the fall-back mechanism should remain as-is
    public interface IExceptionToActionResultMapperHook : IExceptionToActionResultMapper
    {
    }
}
