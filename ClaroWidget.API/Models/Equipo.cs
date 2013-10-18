using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using ACommerce.BO;

namespace ClaroWidget.API.Models
{
    public class Equipo
    {
        public class ParCaracteristicas
        {
            public string Descripcion { get; set; }
            public string Valor { get; set; }

            // Needed only for Serialization
            public ParCaracteristicas() { }

            public ParCaracteristicas(string desc, string val)
            {
                Descripcion = desc;
                Valor = val;
            }
        }

        public int EquipoID { get; set; }
        public string EquipoName { get; set; }
        public string EquipoCode { get; set; }
        public string EquipoDescripcion { get; set; }
        public int? PhotoEquipo { get; set; }
        public string EquipoManufacturer { get; set; }
        public decimal? PriceEquipo { get; set; }
        public int? InventarioEquipo { get; set; }
        public List<ParCaracteristicas> Caracteristicas { get; set; }

        // Needed only for Serialization
        public Equipo() { }

        public Equipo(int id, string equipoName, string equipoCode, List<ParCaracteristicas> caracteristicas)
        {
            EquipoID = id;
            EquipoName = equipoName;
            EquipoCode = equipoCode;
            Caracteristicas = caracteristicas;
        }

        public Equipo(int id, string equipoName, string equipoCode, string equipoDescripcion, 
                      int? photoEquipo, string equipoManufacturer, decimal? priceEquipo, 
                      int? inventarioEquipo, List<ParCaracteristicas> caracteristicas)
        {
            EquipoID = id;
            EquipoName = equipoName;
            EquipoCode = equipoCode;
            EquipoDescripcion = equipoDescripcion;
            PhotoEquipo = photoEquipo;
            EquipoManufacturer = equipoManufacturer;
            PriceEquipo = priceEquipo;
            InventarioEquipo = inventarioEquipo;
            Caracteristicas = caracteristicas;

        }

        public static IEnumerable<Equipo> all()
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());

            // Selecciona todos los planes activos y luego selecciona todas las caracteristicas 
            //de cada uno de estos planes y las inserta en un diccionario, que luego usa para crear una lista de planes

            List<Equipo> equiposList = (from p in portaldb.Productos
                                        join nv in portaldb.NameValues
                                        on p.nvManufacturer equals nv.IDNameValue
                                        where p.IsDeleted == '0' 
                                        && p.nvTipo_Producto == 52
                                        select new Equipo(p.IDProduct, p.ProductName, p.Codigo, p.ProductDescription, 
                                                        p.IDPhotoDefault, nv.Descripcion, p.ProductPrice1, 
                                                        p.ProductStock,new List<ParCaracteristicas>())
                                     )
                                     .ToList();

            return equiposList;
        }

        public static IEnumerable<Equipo> byCategory(string categoryCode)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());

            //Selecciona todos los planes haciendo join con portalbycategory
            List<Equipo> planList = (from pp in portaldb.vw_PortalPlanByCategories
                                   join c in portaldb.PortalCategories
                                     on pp.IDCategory equals c.IDCategory
                                   join p in portaldb.Planes
                                     on pp.IDPlan equals p.IDPlan
                                   where c.CatCode.ToLower() == categoryCode.ToLower()
                                   select new Equipo(p.IDPlan, p.PlanDescription, p.Codigo,
                                       (from ps in portaldb.PlanSpecs
                                        join nv in portaldb.NameValues
                                        on ps.nvPlan_Spec equals nv.IDNameValue
                                        where ps.IDPlan == p.IDPlan
                                        select new ParCaracteristicas(nv.Descripcion, ps.Value))
                                                         .ToList()))
                                                    .ToList();

            return planList;
        }

        public static IEnumerable<Equipo> byPlan(string planCode)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());

            //Selecciona todos los planes haciendo join con portalbycategory
            List<Equipo> planList = (from pe in portaldb.vw_Producto_Plans
                                     join p in portaldb.Productos
                                     on pe.IDProduct equals p.IDProduct
                                     where pe.CodigoPlan == planCode
                                     select new Equipo(p.IDProduct, pe.Nombre_Producto, p.Codigo, p.ProductDescription,p.IDPhotoDefault,
                                         (from nv in portaldb.NameValues
                                          where nv.IDNameValue == p.nvManufacturer
                                          select nv.Descripcion).ToString(),
                                         p.ProductPrice1,p.ProductStock,
                                         (from c in portaldb.PlanSpecs
                                          join nv in portaldb.NameValues
                                          on c.nvPlan_Spec equals nv.IDNameValue
                                          where c.IDPlan == pe.IDPlan
                                          select new ParCaracteristicas(nv.Descripcion, c.Value)).ToList()
                                         ))
                                       .ToList();

            return planList;
        }
    }
}