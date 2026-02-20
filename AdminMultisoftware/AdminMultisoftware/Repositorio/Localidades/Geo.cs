using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class Geo : RepositorioBase<Dominio.Entidades.Localidades.Geo>
    {
        public Geo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Localidades.Geo BuscarCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Localidades.Geo>();
            var result = from obj in query where obj.cep == codigo select obj;
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Método para consultar todas os CEPs próximos de uma determinada posição Geográfica.
        /// Será analisado com a latitude x longitude existente na tabela cepbr_geo.
        /// </summary>
        /// <param name="latitudeOrigem">Latitude de origem a ser analisado os CEPs dentro do Raio.</param>
        /// <param name="longitudeOrigem">Longitude de origem a ser analisado os CEPs dentro do Raio.</param>
        /// <param name="raioKm">Raio para ser analizado.</param>
        /// <returns>Lista de CEPs cuja coordenada está com distância inferior ao parâmetro de raioKm informado.</returns>
        public List<Dominio.ObjetosDeValor.Localidades.Geo> BuscarProximosRaio(decimal latitudeOrigem, decimal longitudeOrigem, decimal raioKm)
        {
            string sqlQuery = @"
select tab.cep
	 , latitude
	 , longitude
	 , latitude_nominatim
	 , longitude_nominatim
     , display_name_nominatim
	 , distancia
	 , edr.logradouro
	 , edr.tipo_logradouro
	 , cid.cidade
	 , cid.uf
  from (select c.cep
			 , c.latitude
			 , c.longitude
			 , CONVERT(DECIMAL(20,10), COALESCE(c.latitude_nominatim,  0))  as latitude_nominatim
			 , CONVERT(DECIMAL(20,10), COALESCE(c.longitude_nominatim, 0))  as longitude_nominatim
             , display_name_nominatim
			 , CONVERT(DECIMAL(20,10), dbo.FKG_DISTANCIA ( :lat, :lng, c.latitude, c.longitude)) as distancia
		  from cepbr_geo c ) as tab
     , cepbr_endereco edr
	 , cepbr_cidade	  cid
 where tab.cep = edr.geo
   and edr.id_cidade = cid.id_cidade
   and distancia <= :raio; ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("lat", latitudeOrigem);
            query.SetParameter("lng", longitudeOrigem);
            query.SetParameter("raio", raioKm);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.Geo)));

            List<Dominio.ObjetosDeValor.Localidades.Geo> lista = query.List<Dominio.ObjetosDeValor.Localidades.Geo>().ToList();
            return lista.OrderBy(x => x.distancia).ToList();
        }

        public Dominio.ObjetosDeValor.Localidades.Geo BuscarPorCEP(int cep)
        {
            string sqlQuery = @"
select tab.cep
	 , latitude
	 , longitude
	 , latitude_nominatim
	 , longitude_nominatim
     , display_name_nominatim
	 , distancia
	 , edr.logradouro
	 , edr.tipo_logradouro
	 , cid.cidade
	 , cid.uf
  from (select c.cep
			 , c.latitude
			 , c.longitude
			 , CONVERT(DECIMAL(20,10), COALESCE(c.latitude_nominatim,  0))  as latitude_nominatim
			 , CONVERT(DECIMAL(20,10), COALESCE(c.longitude_nominatim, 0))  as longitude_nominatim
             , display_name_nominatim
			 , CONVERT(DECIMAL(20,10), 0) as distancia
		  from cepbr_geo c ) as tab
     , cepbr_endereco edr
	 , cepbr_cidade	  cid
 where tab.cep = edr.geo
   and edr.id_cidade = cid.id_cidade
   and tab.cep       = :cep; ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("cep", cep);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.Geo)));

            List<Dominio.ObjetosDeValor.Localidades.Geo> lista = query.List<Dominio.ObjetosDeValor.Localidades.Geo>().ToList();
            return lista.OrderBy(x => x.distancia).FirstOrDefault();
        }

        public int Contar(bool somenteNaoGeocodificadoNominatim)
        {
            string sqlQuery = "select count(1) from cepbr_geo";
            if (somenteNaoGeocodificadoNominatim)
                sqlQuery = "select count(1) from cepbr_geo where latitude_nominatim is null";

            return this.SessionNHiBernate.CreateSQLQuery(sqlQuery).UniqueResult<int>();
        }

        public List<Dominio.ObjetosDeValor.Localidades.Geo> BuscarPaginado(int inicio, int qtdeRegistros, bool somenteNaoGeocodificadoNominatim)
        {
            string sqlQuery = @"
select tab.cep
	 , latitude
	 , longitude
	 , CONVERT(DECIMAL(20,10), COALESCE(tab.latitude_nominatim,  0))  as latitude_nominatim
	 , CONVERT(DECIMAL(20,10), COALESCE(tab.longitude_nominatim, 0))  as longitude_nominatim
     , display_name_nominatim
	 , CONVERT(DECIMAL(20,10), 0) as distancia
	 , edr.logradouro
	 , edr.tipo_logradouro
	 , cid.cidade
	 , cid.uf
	 , tab.id
  from cepbr_geo tab
     , cepbr_endereco edr
	 , cepbr_cidade	  cid
 where tab.cep = edr.geo
   and edr.id_cidade = cid.id_cidade ";

            if (somenteNaoGeocodificadoNominatim)
                sqlQuery += "   and latitude_nominatim is null ";

            sqlQuery+= @"
 order by tab.id
 OFFSET :inicio ROWS FETCH NEXT :qtde ROWS ONLY; ";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("inicio", inicio);
            query.SetParameter("qtde", qtdeRegistros);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.Geo)));

            List<Dominio.ObjetosDeValor.Localidades.Geo> lista = query.List<Dominio.ObjetosDeValor.Localidades.Geo>().ToList();
            return lista.OrderBy(x => x.id).ToList();
        }

        public void AtualizarPosicaoGeoNomitatim(List<Dominio.ObjetosDeValor.Localidades.Geo> lista)
        {
            string sqlQuery = @"
update cepbr_geo
   set latitude_nominatim   = :lat 
     , longitude_nominatim	= :lng 
     , display_name_nominatim = :name 
 where cep = :cep;";

            foreach (Dominio.ObjetosDeValor.Localidades.Geo geo in lista)
            {
                if (geo.latitude_nominatim == 0 || geo.longitude_nominatim == 0 || geo.cep == 0)
                    continue;

                var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
                query.SetParameter("lat", geo.latitude_nominatim);
                query.SetParameter("lng", geo.longitude_nominatim);
                query.SetParameter("name", geo.display_name_nominatim);
                query.SetParameter("cep", geo.cep);
                query.ExecuteUpdate();
            }            
        }
    }
}
