using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace ClaroWidget.API.Controllers
{
    public class EquiposController : ApiController
    {
        //
        // GET: /Equipos/

        public IEnumerable<ClaroWidget.API.Models.Equipo> Get()
        {
            ClaroWidget.API.Models.Equipo[] planes = ClaroWidget.API.Models.Equipo.all().ToArray();
            return planes;
        }

        public IEnumerable<ClaroWidget.API.Models.Equipo> Get(string id)
        {
            ClaroWidget.API.Models.Equipo[] planes = ClaroWidget.API.Models.Equipo.byCategory(id).ToArray();
            return planes;
        }
    }
}
