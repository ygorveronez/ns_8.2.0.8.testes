using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoDescarregamentoGrupoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>
    {
        public PeriodoDescarregamentoGrupoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>()
                .Where(obj => obj.PeriodoDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return query
                .Fetch(obj => obj.PeriodoDescarregamento)
                .ThenFetch(obj => obj.CentroDescarregamento)
                .Fetch(obj => obj.GrupoProduto)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> BuscarPorPeriodoDescarregamento(int codigoPeriodoDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>();

            var result = from obj in query where obj.PeriodoDescarregamento.Codigo == codigoPeriodoDescarregamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> BuscarPorPeriodosDescarregamento(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>();

            int quantidadePorBusca = 2000;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> resultadoFinal = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>();

            for (int i = 0; i < codigos.Count; i += quantidadePorBusca)
            {
                List<int> codigosAtuais = codigos.Skip(i).Take(quantidadePorBusca).ToList();

                var resultadoParcial = from obj in query where codigosAtuais.Contains(obj.PeriodoDescarregamento.Codigo) select obj;

                resultadoFinal.AddRange(resultadoParcial.ToList());
            }

            return resultadoFinal;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto> BuscarPorExcecao(int codigoExcecaoCapacidadeDescarregamento)
        {
            var consultaPeriodoDescarregamentoGrupoProduto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoProduto>()
                .Where(o => o.PeriodoDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecaoCapacidadeDescarregamento);

            return consultaPeriodoDescarregamentoGrupoProduto.ToList();
        }
    }
}
