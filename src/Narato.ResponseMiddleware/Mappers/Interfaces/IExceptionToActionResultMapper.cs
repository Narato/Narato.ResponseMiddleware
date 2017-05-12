using Microsoft.AspNetCore.Mvc;
using System;

namespace Narato.ResponseMiddleware.Mappers.Interfaces
{
    public interface IExceptionToActionResultMapper
    {
        IActionResult Map(Exception ex);
    }
}
