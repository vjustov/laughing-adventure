﻿using System;
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
        public string PhotoEquipo { get; set; }
        public string EquipoManufacturer { get; set; }
        public decimal? PriceEquipoPostPago { get; set; }
        public decimal? PriceEquipoPrePago { get; set; }
        public int? InventarioEquipo { get; set; }
        public int? Rating { get; set; }
        public List<ParCaracteristicas> Caracteristicas { get; set; }
        public string Link { get; set; }

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
                      string photoEquipo, string equipoManufacturer, decimal? priceEquipoPostPago, 
                      decimal? priceEquipoPrePago, int? inventarioEquipo, int? rating, 
                      List<ParCaracteristicas> caracteristicas, string link)
        {
            EquipoID = id;
            EquipoName = equipoName;
            EquipoCode = equipoCode;
            EquipoDescripcion = equipoDescripcion;
            PhotoEquipo = photoEquipo;
            EquipoManufacturer = equipoManufacturer;
            PriceEquipoPostPago = priceEquipoPostPago;
            PriceEquipoPrePago = priceEquipoPrePago;
            InventarioEquipo = inventarioEquipo;
            Rating = rating;
            Caracteristicas = caracteristicas;
            Link = link;

        }

        public static IEnumerable<Equipo> all()
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());
            SecurityDataContext secdb = new SecurityDataContext(ACommerce.BO.Comun.GetConnString());

            string parametro = secdb.Parametros.Where(p => p.Codigo.Equals("URL_head")).FirstOrDefault().Valor.ToString();

            // Selecciona todos los planes activos y luego selecciona todas las caracteristicas 
            //de cada uno de estos planes y las inserta en un diccionario, que luego usa para crear una lista de planes

            List<Equipo> equiposList = (from p in portaldb.Productos
                                        join nv in portaldb.NameValues
                                        on p.nvManufacturer equals nv.IDNameValue
                                        where p.IsDeleted == '0' 
                                        && p.nvTipo_Producto == 52
                                        select new Equipo(p.IDProduct, p.ProductName, p.Codigo, p.ProductDescription,
                                                        p.IDPhotoDefault.HasValue ? parametro + "/Lib/Images.aspx?ID=" + p.IDPhotoDefault : "", nv.Descripcion, p.ProductPrice1, p.ProductPrice2,
                                                        p.ProductStock, (from pr in portaldb.ProductReviews
                                                                         where pr.IDProduct == p.IDProduct
                                                                         select pr.Score).FirstOrDefault(),
                                         (from ps in portaldb.vw_ProductSpecs
                                          where ps.IDProduct == p.IDProduct
                                          select new ParCaracteristicas(ps.Especificacion, ps.Value)).ToList()
                                         , parametro + "Master/Claro/Secciones/ShowProductMovil.aspx?ID=" + p.IDProduct)
                                     )
                                     .ToList();

            return equiposList;
        }

        public static IEnumerable<Equipo> byCategory(string categoryCode)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());
            SecurityDataContext secdb = new SecurityDataContext(ACommerce.BO.Comun.GetConnString());

            string parametro = secdb.Parametros.Where(p => p.Codigo.Equals("URL_head")).FirstOrDefault().Valor.ToString();

            //Selecciona todos los planes haciendo join con portalbycategory
            List<Equipo> equiposList = (from pp in portaldb.vw_PortalProductByCategories
                                   join c in portaldb.PortalCategories
                                     on pp.IDCategory equals c.IDCategory
                                    join p in portaldb.Productos
                                    on pp.IDProduct equals p.IDRecurso
                                        join nv in portaldb.NameValues
                                            on p.nvManufacturer equals nv.IDNameValue
                                   where c.CatCode.ToLower() == categoryCode.ToLower()
                                   select new Equipo(p.IDProduct, p.ProductName, p.Codigo, p.ProductDescription,
                                                        p.IDPhotoDefault.HasValue ? parametro + "/Lib/Images.aspx?ID=" + p.IDPhotoDefault : "", nv.Descripcion, p.ProductPrice1, p.ProductPrice2,
                                                        p.ProductStock, (from pr in portaldb.ProductReviews
                                                                         where pr.IDProduct == p.IDProduct
                                                                         select pr.Score).FirstOrDefault(),
                                         (from ps in portaldb.vw_ProductSpecs
                                          where ps.IDProduct == p.IDProduct
                                          select new ParCaracteristicas(ps.Especificacion, ps.Value)).ToList()
                                         , parametro + "Master/Claro/Secciones/ShowProductMovil.aspx?ID=" + p.IDProduct))
                                                    .ToList();

            return equiposList;
        }

        public static IEnumerable<Equipo> byPlan(string planCode)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());
            SecurityDataContext secdb = new SecurityDataContext(ACommerce.BO.Comun.GetConnString());
            TiendaDataContext tiendadb = new TiendaDataContext(ACommerce.BO.Comun.GetConnString());

            // From i In tiendadb.ProductReviews Where i.IDProduct = _idProduct Select i.Score
            string parametro = secdb.Parametros.Where(p => p.Codigo.Equals("URL_head")).FirstOrDefault().Valor.ToString();


            //Selecciona todos los planes haciendo join con portalbycategory
            List<Equipo> equiposList = (from pe in portaldb.vw_Producto_Plans
                                     join p in portaldb.Productos
                                     on pe.IDProduct equals p.IDProduct
                                     where pe.CodigoPlan == planCode
                                     && pe.Estatus_Produto == "Activo"
                                     && pe.Estatus_Plan == "Activo"
                                     && pe.LineType == "S"
                                     select new Equipo(p.IDProduct, pe.Nombre_Producto, p.Codigo, HtmlRemoval.StripTagsCharArray(p.ProductDescription),
                                         p.IDPhotoDefault.HasValue ? parametro + "/Lib/Images.aspx?ID=" + p.IDPhotoDefault + "&thum=1": "",
                                         (from nv in portaldb.NameValues
                                          where nv.IDNameValue == p.nvManufacturer
                                          select nv.Descripcion).FirstOrDefault().ToString(),
                                         p.ProductPrice1, p.ProductPrice2, p.ProductStock, (from pr in portaldb.ProductReviews
                                                                                            where pr.IDProduct == p.IDProduct
                                                                                            select pr.Score).FirstOrDefault(),
                                         (from ps in portaldb.vw_ProductSpecs
                                          where ps.IDProduct == p.IDProduct
                                          select new ParCaracteristicas(ps.Especificacion, ps.Value)).ToList()
                                         , parametro + "Master/Claro/Secciones/ShowProductMovil.aspx?ID=" + p.IDProduct))
                                       .ToList();

            return equiposList;
        }

        public static IEnumerable<Equipo> compare(string[] equiposCodes)
        {
            PortalDataContext portaldb = new PortalDataContext(ACommerce.BO.Comun.GetConnString());
            SecurityDataContext secdb = new SecurityDataContext(ACommerce.BO.Comun.GetConnString());
            TiendaDataContext tiendadb = new TiendaDataContext(ACommerce.BO.Comun.GetConnString());
            // From i In tiendadb.ProductReviews Where i.IDProduct = _idProduct Select i.Score
            string parametro = secdb.Parametros.Where(p => p.Codigo.Equals("URL_head")).FirstOrDefault().Valor.ToString();

            //ACommerce.BO.usp_GetProductByCategory3Result source =  tiendadb.usp_GetProductByCategory3(-1, "", -1, -1, -1);
            
            //decimal Precio = Math.Round(source.Productprice1 ?? 0, 0, MidpointRounding.AwayFromZero);
            //decimal PrecioAnterior = source.Productprice1ant == null ? 0 : Math.Round(source.Productprice1ant ?? 0, 0, MidpointRounding.AwayFromZero);
            //decimal PrecioPre = Math.Round(source.ProductpricePREP, 0, MidpointRounding.AwayFromZero);
            //decimal PrecioAnteriorPre = source.ProductpricePREPant == null ? 0: Math.Round(source.ProductpricePREPant, 0, MidpointRounding.AwayFromZero);
           


            //Selecciona todos los planes haciendo join con portalbycategory
            List<Equipo> equiposList = (from p in portaldb.vw_Productos
                                        join nv in portaldb.NameValues
                                        on p.nvManufacturer equals nv.IDNameValue
                                        where equiposCodes.Contains(p.Codigo)
                                        && p.nvTipo_Producto == 52
                                        select new Equipo(p.IDProduct, p.Producto, p.Codigo, HtmlRemoval.StripTagsCharArray(p.Descripcion),
                                            p.IDPhotoDefault.HasValue ? parametro + "/Lib/Images.aspx?ID=" + p.IDPhotoDefault + "&thum=1" : "",
                                            nv.Descripcion, p.ProductPrice1, p.ProductPrice2, p.ProductStock,
                                                (from pr in portaldb.ProductReviews
												where pr.IDProduct == p.IDProduct
												select pr.Score).FirstOrDefault(),
                                            (from ps in portaldb.vw_ProductSpecs
                                             where ps.IDProduct == p.IDProduct
                                             select new ParCaracteristicas(ps.Especificacion, ps.Value)).ToList()
                                            , parametro + "Master/Claro/Secciones/ShowProductMovil.aspx?ID=" + p.IDProduct))
                                       .ToList();

            return equiposList;
        }
    }
}