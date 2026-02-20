using System.Collections.Generic;
using System.Linq;

namespace AdminMultisoftware.Repositorio.Localidades
{
    public class  LocalidadeGeo : RepositorioBase<Dominio.Entidades.Localidades.Geo>
    {
        public LocalidadeGeo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Localidades.LocalidadeGeo BuscarLocalidade(int cdIbge)
        {
            string sqlQuery = @"
select geocod as cd_ibge
     , nome
     , uf_geocod as uf_cd_ibge
	 , uf_sigla
	 , uf_nome
	 , gr_regiao as regiao
	 , geom.ToString() as geom
  from t_localidades_geo 
 where geocod = :geocod 
 order by nome";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("geocod", cdIbge);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.LocalidadeGeo)));

            return query.UniqueResult<Dominio.ObjetosDeValor.Localidades.LocalidadeGeo>();
        }

        public List<Dominio.ObjetosDeValor.Localidades.LocalidadeGeo> BuscarLocalidades(string ufSigla)
        {
            string sqlQuery = @"
select geocod as cd_ibge
     , nome
     , uf_geocod as uf_cd_ibge
	 , uf_sigla
	 , uf_nome
	 , gr_regiao as regiao
	 , geom.ToString() as geom
  from t_localidades_geo 
 where uf_sigla  =  :uf_sigla 
 order by nome";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetParameter("uf_sigla", ufSigla);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.LocalidadeGeo)));

            return query.List<Dominio.ObjetosDeValor.Localidades.LocalidadeGeo>().ToList();
        }

        public List<Dominio.ObjetosDeValor.Localidades.LocalidadeGeo> BuscarLocalidadesPorPosicao(decimal latitude, decimal longitude, bool retornarGeometria = false)
        {
            string sqlQuery = string.Format(@"
select geocod as cd_ibge
     , nome
     , uf_geocod as uf_cd_ibge
	 , uf_sigla
	 , uf_nome
	 , gr_regiao as regiao
	 , geom.ToString() as geom
  from t_localidades_geo loc
 where loc.GEOM.STContains( cast(CONCAT ('POINT(', {0}, ' ', {1},')') as geometry)) = 1
 order by nome", longitude.ToString().Replace(",", "."), latitude.ToString().Replace(",", "."));

            if (!retornarGeometria)
                sqlQuery = sqlQuery.Replace("geom.ToString() as geom", "null as geom");

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            query.SetTimeout(120000);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Localidades.LocalidadeGeo)));

            return query.List<Dominio.ObjetosDeValor.Localidades.LocalidadeGeo>().ToList();
        }
    }
}
