using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static System.Configuration.ConfigurationManager;
using System.Web.Http;
using System.Web.Http.Cors;


namespace Unk.WebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", PreflightMaxAge = 1728000, SupportsCredentials = true)]
    public class BaseApiController: ApiController
    {
        public BaseApiController()
        {
        }
    }
}