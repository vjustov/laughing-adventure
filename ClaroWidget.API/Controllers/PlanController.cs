using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ClaroWidget.API.Controllers
{
    public class PlansController : ApiController
    {
        //
        // GET: /Plan/

        public IEnumerable<ClaroWidget.API.Models.Plan> all()
        {
            ClaroWidget.API.Models.Plan[] planes = ClaroWidget.API.Models.Plan.all().ToArray();
            return planes;
        }
        [System.Web.Http.HttpGet]
        public IEnumerable<ClaroWidget.API.Models.Plan> byCategory(string id)
        {
            ClaroWidget.API.Models.Plan[] planes = ClaroWidget.API.Models.Plan.byCategory(id).ToArray();
            return planes;
        }

        public IEnumerable<ClaroWidget.API.Models.Plan> byProduct(string id)
        {
            ClaroWidget.API.Models.Plan[] planes = ClaroWidget.API.Models.Plan.byEquipo(id).ToArray();
            return planes;
        }
    }
}
