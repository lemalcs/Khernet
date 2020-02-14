using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Khernet.Services.Messages;

namespace Khernet.Services.Contracts
{
    [ServiceContract(Namespace = "http://contract.khernet", Name = "NotifierCallBack")]
    public interface IEventNotifierCallBack
    {
        [OperationContract(IsOneWay = true)]
        void ProcessNotification(Notification info);
    }
}
