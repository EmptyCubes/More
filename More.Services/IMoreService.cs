using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using More.Engine.Model;

namespace More.Services
{
    
    [ServiceContract]
    public interface IMoreService
    {        

        [OperationContract]
        RuleBookTraceInformation[] Execute(DateTime effectiveDate, string ruleBook, Dictionary<string,object> inputs);

    }
}
