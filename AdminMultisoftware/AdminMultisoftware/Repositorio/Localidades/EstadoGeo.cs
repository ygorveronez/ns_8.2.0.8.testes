using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class EstadoGeo : RepositorioBase<Dominio.Entidades.Localidades.Geo>
    {
        public EstadoGeo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Localidades.EstadoGeo BuscarEstado(int cdIbge)
        {
            string sqlQuery = @"
SELECT ufg.cd_uf            as cd_ibge
     , ufg.nm_uf
	 , ufg.sigla_uf
	 , ufg.nm_regiao
	 , ufg.area_km2
	 , ufg.geom.ToString()  as geom
  FROM T_BR_UF_GEO ufg
 WHERE ufg.cd_uf = :geocod 
 order by ufg.sigla_uf";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("geocod", cdIbge);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.EstadoGeo)));

            return query.UniqueResult<Dominio.ObjetosDeValor.Localidades.EstadoGeo>();
        }

        public List<Dominio.ObjetosDeValor.Localidades.EstadoGeo> BuscarTodosEstados()
        {
            string sqlQuery = @"
SELECT ufg.cd_uf            as cd_ibge
     , ufg.nm_uf
	 , ufg.sigla_uf
	 , ufg.nm_regiao
	 , ufg.area_km2
	 , ufg.geom.ToString()  as geom
  FROM T_BR_UF_GEO ufg
 order by ufg.sigla_uf";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.EstadoGeo)));

            return query.List<Dominio.ObjetosDeValor.Localidades.EstadoGeo>().ToList();
        }

        /// <summary>
        /// Passar as coordenadas no formato LONGITUDE LATITUDE, LONGITUDE LATITUDE ..... , LONGITUDE LATITUDE
        /// Passar coordenadas a cada 10km pois fica muito lento a pesquisa...
        /// </summary>
        /// <param name="coordenadas"></param>
        /// <returns></returns>
        public List<Dominio.ObjetosDeValor.Localidades.EstadoGeo> BuscarEstadosCoordenadas(string coordenadas, string separador = ",", bool somenteEstadosDistintosNaOrdem = true)
        {
            //where ufg.GEOM.STContains( geometry::STGeomFromText(CONCAT ('POINT(', poi.point ,')'), 0)) = 1
            string sqlQuery = @"
SELECT ufg.cd_uf    as cd_ibge
     , ufg.nm_uf
	 , ufg.sigla_uf
	 , ufg.nm_regiao
	 , ufg.area_km2
     , poi.id       indice
     , poi.point
  FROM T_BR_UF_GEO  ufg
     , (select id
	         , elemento as point
		  from FKG_SPLIT( :coordenadas, :separador ) ) poi
 where ufg.GEOM.STContains( cast(CONCAT ('POINT(', poi.point ,')') as geometry)) = 1
 option (maxrecursion 0)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("coordenadas", coordenadas);
            query.SetParameter("separador", separador);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.EstadoGeo)));

            List<Dominio.ObjetosDeValor.Localidades.EstadoGeo> lista = query.List<Dominio.ObjetosDeValor.Localidades.EstadoGeo>().ToList();
            if (!somenteEstadosDistintosNaOrdem)
                return lista;

            List<Dominio.ObjetosDeValor.Localidades.EstadoGeo> novaLista = new List<Dominio.ObjetosDeValor.Localidades.EstadoGeo>();
            foreach (Dominio.ObjetosDeValor.Localidades.EstadoGeo estadoGeo in lista)
            {
                if (novaLista.Count == 0)
                    novaLista.Add(estadoGeo);

                if (novaLista[novaLista.Count - 1].sigla_uf != estadoGeo.sigla_uf)
                    novaLista.Add(estadoGeo);
            }
            return novaLista;
        }
    }
}
