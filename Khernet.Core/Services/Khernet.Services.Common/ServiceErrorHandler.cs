using Khernet.Core.Utility;
using System;
//using System.Web.Hosting;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Khernet.Services.Common
{
    public class ServiceErrorHandler : IErrorHandler
    {
        //Allow to make task related to error handling like logging.
        //It is called asynchronously in another process to avoid locking the error sending to client
        public bool HandleError(Exception error)
        {
            LogDumper.WriteLog(error);
            return true;
        }

        //It is called when an error is raised in service, allow the creation of a custom FaultException
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            ErrorInformation fileError = new ErrorInformation();
            fileError.ExceptionName = error.GetType().ToString();
            fileError.Result = false;
            fileError.Message = error.Message;
            fileError.Source = error.TargetSite.Name;

            FaultReason faulReason = new FaultReason(error.Message);

            FaultException<ErrorInformation> exception = new FaultException<ErrorInformation>(fileError, faulReason);

            MessageFault messageFault = exception.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, exception.Action);
        }
    }
}