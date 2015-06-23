using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Services
{
    public class OperationStatus
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionInnerMessage { get; set; }
        public string ExceptionInnerStackTrace { get; set; }
        public dynamic Data { get; set; }

        public static OperationStatus CreateFromException(string message, Exception ex)
        {
            var opStatus = new OperationStatus
            {
                Status = false,
                Message = message
            };

            if (ex == null)
                return opStatus;
            opStatus.ExceptionMessage = ex.Message;
            opStatus.ExceptionInnerStackTrace = ex.StackTrace;

            if (ex.InnerException == null)
                return opStatus;
            opStatus.ExceptionInnerMessage = ex.InnerException.Message;
            opStatus.ExceptionInnerStackTrace = ex.InnerException.StackTrace;
            return opStatus;
        }
    }
}