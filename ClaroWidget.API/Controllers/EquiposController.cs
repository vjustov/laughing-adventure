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
        [System.Web.Http.HttpGet]
        public IEnumerable<ClaroWidget.API.Models.Equipo> all()
        {
            ClaroWidget.API.Models.Equipo[] planes = ClaroWidget.API.Models.Equipo.all().ToArray();
            return planes;
        }
        [System.Web.Http.HttpGet]
        public IEnumerable<ClaroWidget.API.Models.Equipo> byCategory(string id)
        {
            ClaroWidget.API.Models.Equipo[] planes = ClaroWidget.API.Models.Equipo.byCategory(id).ToArray();
            return planes;
        }
        [System.Web.Http.HttpGet]
        public IEnumerable<ClaroWidget.API.Models.Equipo> byPlan(string id)
        {
            ClaroWidget.API.Models.Equipo[] planes = ClaroWidget.API.Models.Equipo.byPlan(id).ToArray();
            return planes;
        }
        [System.Web.Http.HttpGet]
        public IEnumerable<ClaroWidget.API.Models.Equipo> compare(string equipos)
        {
            ClaroWidget.API.Models.Equipo[] planes = ClaroWidget.API.Models.Equipo.compare(equipos.Split(',')).ToArray();
            return planes;
        }
        

    }
}
