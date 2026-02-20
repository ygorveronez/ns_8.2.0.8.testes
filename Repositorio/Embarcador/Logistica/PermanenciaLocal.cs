using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PermanenciaLocal : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal>
    {
        public PermanenciaLocal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> BuscarPorLocal(int codigoLocal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal>();
            query = query.Where(obj => obj.Local.Codigo == codigoLocal);
            return query.OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.Fetch(obj => obj.Local).Fetch(obj => obj.Carga).OrderBy(obj => obj.DataInicio).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> BuscarPorCargas(List<int> codigosCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal>();
            query = query.Where(obj => codigosCarga.Contains(obj.Carga.Codigo));
            return query.Fetch(obj => obj.Local).Fetch(obj => obj.Carga).OrderBy(obj => obj.DataInicio).ToList();
        }

        public Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal BuscarAbertaPorLocalECarga(List<int> codigosLocal, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga && codigosLocal.Contains(obj.Local.Codigo) && obj.DataFim == null);
            return query.OrderBy(obj => obj.DataInicio).FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> BuscarObjetoDeValorAbertaPorLocais(List<int> codigosLocal)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal> result = new();

            int take = 100;
            int skip = 0;
            int total = codigosLocal.Count;
            while (skip <= total)
            {
                List<int> tmp = codigosLocal.Skip(skip).Take(take).ToList();

                string sql = $@" select
                                    Permanencia.PLO_CODIGO as CodigoPermanencia,
                                    Locais.LOC_DESCRICAO as Descricao,
                                    Permanencia.PLO_DATA_INICIO as DataInicio,
                                    Permanencia.PLO_TEMPO_SEGUNDOS as TempoSegundos,
                                    Permanencia.LOC_CODIGO as CodigoLocal,
                                    Permanencia.CAR_CODIGO as CodigoCarga
                                from
                                    T_PERMANENCIA_LOCAL Permanencia
                                inner join
                                    T_CARGA Carga
                                        on Permanencia.CAR_CODIGO = Carga.CAR_CODIGO
                                left outer join
                                    T_LOCAIS Locais
                                        on Permanencia.LOC_CODIGO = Locais.LOC_CODIGO
                                where
                                    Locais.LOC_CODIGO in ( {string.Join(",", tmp)} )
                                    and Permanencia.PLO_DATA_FIM is null
                                order by
                                    Permanencia.PLO_DATA_INICIO asc;";

                var query = this.SessionNHiBernate.CreateSQLQuery(sql);
                query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal)));
                query.SetTimeout(600);

                result.AddRange(query.List<Dominio.ObjetosDeValor.Embarcador.Logistica.PermanenciaLocal>());

                skip += take;
            }
            return result;
        }

        public DateTime BuscarDataUltimaSaidaDoLocal(int codigoLocal, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal>();
            query = query.Where(obj => obj.Local.Codigo == codigoLocal && obj.DataFim != null && obj.Carga.Codigo == codigoCarga);
            var result = query.OrderByDescending(obj => obj.DataFim).FirstOrDefault();
            return result?.DataFim ?? DateTime.MinValue;
        }

    }
}
