using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RestAPIFrameWork
{
    public interface IRestAPI 
    {
        HttpStatusCode Execute();

    }
}
