using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACommerce.BO;
using ACommerce.BO.Portal;
using System.Linq;

namespace ClaroWidget.API.Models
{
    public class Plan
    {
        public class ParCaracteristicas 
        {
            public string Descripcion {get; set;}
            public string Valor {get; set;}
            
            // Needed only for Serialization
            public ParCaracteristicas() { }

            public ParCaracteristicas (string desc, string val)
            {
            Descripcion = desc;
            Valor= val;
            }
        }

        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public string PlanCode { get; set; }
        public List<ParCaracteristicas> Caracteristicas { get; set; }

        // Needed only for Serialization
        public Plan() { }

        public Plan(int id, string planName, string planCode, List<ParCaracteristicas> caracteristicas)
        {
            PlanID = id;
            PlanName= planName;
            PlanCode = planCode;
            Caracteristicas = caracteristicas;
        }

        public static IEnumerable<Plan> all()
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());

            // Selecciona todos los planes activos y luego selecciona todas las caracteristicas 
            //de cada uno de estos planes y las inserta en un diccionario, que luego usa para crear una lista de planes
 
            List<Plan> planList = (from p in portaldb.Planes
                                      where p.IsDeleted == '0'
                                      select new Plan(p.IDPlan, p.PlanDescription, p.Codigo,
                                            (from c in portaldb.PlanSpecs 
                                             join nv in portaldb.NameValues 
                                             on c.nvPlan_Spec equals nv.IDNameValue 
                                             where c.IDPlan == p.IDPlan 
                                             select new ParCaracteristicas( nv.Descripcion, c.Value ))
                                     .ToList()))
                                     .ToList();

            return planList;
        }

        public static IEnumerable<Plan> byCategory(string categoryCode)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());

            //Selecciona todos los planes haciendo join con portalbycategory
            List<Plan> planList = (from pp in portaldb.vw_PortalPlanByCategories
                                   join c in portaldb.PortalCategories
                                     on pp.IDCategory equals c.IDCategory
                                   join p in portaldb.Planes
                                     on pp.IDPlan equals p.IDPlan
                                   where c.CatCode.ToLower() == categoryCode.ToLower()
                                   select new Plan(p.IDPlan, p.PlanDescription, p.Codigo,
                                       (from ps in portaldb.vw_PlanSpecs
                                        where ps.IDPlan == pp.IDPlan
                                        select new ParCaracteristicas(ps.Especificacion, ps.Plan_Value)).ToList()))
                                                    .ToList();

            return planList;
        }


        public static IEnumerable<Plan> byEquipo(string equipoCode)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());


            //Selecciona todos los planes haciendo join con portalbycategory
            List<Plan> planList = (from pe in portaldb.vw_Producto_Plans
                                   join p in portaldb.Productos 
                                   on pe.IDProduct equals p.IDProduct 
                                   where p.Codigo == equipoCode
                                   && pe.Estatus_Produto == "Activo"
                                   && pe.Estatus_Plan == "Activo"
                                   && pe.LineType == "S"
                                   select new Plan(pe.IDPlan, pe.Nombre_Plan, pe.CodigoPlan,
                                       (from ps in portaldb.vw_PlanSpecs
                                        where ps.IDPlan == pe.IDPlan
                                        select new ParCaracteristicas(ps.Especificacion, ps.Plan_Value)).ToList()
                                       ))
                                       .ToList();

            return planList;
        }
    }
}