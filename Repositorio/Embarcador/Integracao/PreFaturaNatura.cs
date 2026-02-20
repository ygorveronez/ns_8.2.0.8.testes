using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class PreFaturaNatura : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura>
    {
        public PreFaturaNatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura BuscarPorPreFatura(long numPreFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura>();

            query = query.Where(o => o.NumeroPreFatura == numPreFatura);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura? situacao, int codigoEmpresa, long numeroPreFatura, DateTime dataInicial, DateTime dataFinal, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura>();

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (numeroPreFatura > 0)
                query = query.Where(o => o.NumeroPreFatura == numeroPreFatura);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataPreFatura >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataPreFatura < dataFinal.AddDays(1).Date);

            return query.Fetch(o => o.Fatura)
                        .OrderBy(propOrdena + " " + dirOrdena)
                        .Skip(inicio)
                        .Take(limite)
                        .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura? situacao, int codigoEmpresa, long numeroPreFatura, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura>();

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            if (codigoEmpresa > 0)
                query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (numeroPreFatura > 0)
                query = query.Where(o => o.NumeroPreFatura == numeroPreFatura);

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataPreFatura >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataPreFatura < dataFinal.AddDays(1).Date);

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura BuscarPorFatura(int codigoFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.PreFaturaNatura>();

            query = query.Where(o => o.Fatura.Codigo == codigoFatura);

            return query.FirstOrDefault();
        }
    }
}
