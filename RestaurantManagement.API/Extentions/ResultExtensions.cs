#region Not in use
// using RestaurantManagement.Domain.Shared;

// namespace RestaurantManagement.API.Extentions;

// public static class ResultExtensions
// {
//     public static IResult ToProblemDetails(this Result result)
//     {
//         if (result.IsSuccess)
//         {
//             throw new InvalidOperationException("Can't convert success result to problem");
//         }

//         return Results.Problem(
//             statusCode: StatusCodes.Status400BadRequest,
//             title: "Bad Request",
//             type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
//             extensions: new Dictionary<string, object?>
//             {
//                 { "errors", result.Errors }
//             });

//     }

//     public static IResult? ToProblemDetails<T>(this Result<T> result)
//     {
//         if (result.IsSuccess)
//         {
//             throw new InvalidOperationException("Can't convert success result to problem");
//         }

//         return Results.Problem(
//             statusCode: StatusCodes.Status400BadRequest,
//             title: "Bad Request",
//             type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
//             extensions: new Dictionary<string, object?>
//             {
//                 { "errors", result.Errors }
//             });
//     }

//     public static IResult? NoContent<T>(this Result<T> result)
//     {
//         if (result.IsSuccess)
//         {
//             throw new InvalidOperationException("Can't convert success result to problem");
//         }

//         return Results.Problem(
//             statusCode: StatusCodes.Status204NoContent,
//             title: "No Content",
//             type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
//             extensions: new Dictionary<string, object?>
//             {
//                 { "errors", result.Errors }
//             });
//     }

// }

#endregion