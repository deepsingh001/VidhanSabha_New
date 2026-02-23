using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace GlobalVidhanSabha.Helpers
{
    public static class GlobalExceptionHandler
    {
        // ============================================
        // 1. Custom App Exception
        // ============================================
        public class AppException : Exception
        {
            public string ErrorCode { get; private set; }
            public int StatusCode { get; private set; }

            public AppException(string message,
                                string errorCode = "SERVER_ERROR",
                                int statusCode = 500)
                : base(message)
            {
                ErrorCode = errorCode;
                StatusCode = statusCode;
            }

            public AppException(string message,
                                Exception innerException,
                                string errorCode = "SERVER_ERROR",
                                int statusCode = 500)
                : base(message, innerException)
            {
                ErrorCode = errorCode;
                StatusCode = statusCode;
            }

            // Production Ready Factory Methods

            public static AppException Database(Exception ex)
            {
                return new AppException(
                    "Database operation failed.",
                    ex,
                    "DB_ERROR",
                    500
                );
            }

            public static AppException Validation(string message)
            {
                return new AppException(
                    message,
                    "VALIDATION_ERROR",
                    400
                );
            }

            public static AppException Unauthorized()
            {
                return new AppException(
                    "Unauthorized access.",
                    "UNAUTHORIZED",
                    401
                );
            }

            public static AppException NotFound(string message)
            {
                return new AppException(
                    message,
                    "NOT_FOUND",
                    404
                );
            }

            public static AppException FileError(Exception ex)
            {
                return new AppException(
                    "File upload failed.",
                    ex,
                    "FILE_ERROR",
                    500
                );
            }

            public static AppException Network(Exception ex)
            {
                return new AppException(
                    "Network error occurred.",
                    ex,
                    "NETWORK_ERROR",
                    500
                );
            }
        }

      


        // ============================================
        // 2. Base Service Executor
        // ============================================
        public abstract class BaseService
        {
            protected async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
            {
                try
                {
                    return await action();
                }
                catch (SqlException ex)
                {
                    throw new AppException(
                        ex.Message,     
                        "DB_ERROR",
                        400
                    );
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Unauthorized();
                }
                catch (System.IO.IOException ex)
                {
                    throw AppException.FileError(ex);
                }
                catch (AppException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new AppException(
                        "Unexpected server error occurred.",
                        ex,
                        "SERVER_ERROR",
                        500
                    );
                }
            }

            protected async Task ExecuteAsync(Func<Task> action)
            {
                try
                {
                    await action();
                }
                catch (SqlException ex)
                {
                    throw AppException.Database(ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw AppException.Unauthorized();
                }
                catch (System.IO.IOException ex)
                {
                    throw AppException.FileError(ex);
                }
                catch (AppException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new AppException(
                        "Unexpected server error occurred.",
                        ex,
                        "SERVER_ERROR",
                        500
                    );
                }
            }
        }


        // ============================================
        // 3. Global API Exception Filter
        // ============================================
        public class ExceptionFilter : ExceptionFilterAttribute
        {
            public override void OnException(HttpActionExecutedContext context)
            {
                var exception = context.Exception;

                HttpStatusCode status = HttpStatusCode.InternalServerError;
                string message = "Internal server error";
                string errorCode = "SERVER_ERROR";

                if (exception is AppException appEx)
                {
                    status = (HttpStatusCode)appEx.StatusCode;
                    message = appEx.Message;
                    errorCode = appEx.ErrorCode;
                }

                var response = new
                {
                    success = false,
                    errorCode = errorCode,
                    message = message
                };

                context.Response = context.Request.CreateResponse(status, response);
            }
        }

        
    }
    public static class DataReaderExtensions
    {
        public static int GetInt32Safe(this SqlDataReader dr, string column)
        {
            return dr[column] != DBNull.Value ? Convert.ToInt32(dr[column]) : 0;
        }

        public static int? GetNullableInt32Safe(this SqlDataReader dr, string column)
        {
            return dr[column] != DBNull.Value ? Convert.ToInt32(dr[column]) : (int?)null;
        }

        public static string GetStringSafe(this SqlDataReader dr, string column)
        {
            return dr[column] != DBNull.Value ? dr[column].ToString() : null;
        }

        public static DateTime? GetDateTimeSafe(this SqlDataReader dr, string column)
        {
            return dr[column] != DBNull.Value ? Convert.ToDateTime(dr[column]) : (DateTime?)null;
        }

        public static bool GetBoolSafe(this SqlDataReader dr, string column)
        {
            return dr[column] != DBNull.Value && Convert.ToBoolean(dr[column]);
        }
    }
}