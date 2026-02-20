using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoDescarregamentoGrupoPessoa : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>
    {
        public PeriodoDescarregamentoGrupoPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> BuscarPorCentroDescarregamento(int codigoCentroDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>()
                .Where(obj => obj.PeriodoDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> BuscarPorPeriodoDescarregamento(int codigoPeriodoDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>();

            var result = from obj in query where obj.PeriodoDescarregamento.Codigo == codigoPeriodoDescarregamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> BuscarPorPeriodosDescarregamento(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>();

            int quantidadePorBusca = 2000;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> resultadoFinal = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>();

            for (int i = 0; i < codigos.Count; i += quantidadePorBusca)
            {
                List<int> codigosAtuais = codigos.Skip(i).Take(quantidadePorBusca).ToList();

                var resultadoParcial = from obj in query where codigosAtuais.Contains(obj.PeriodoDescarregamento.Codigo) select obj;

                resultadoFinal.AddRange(resultadoParcial.ToList());
            }

            return resultadoFinal;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> BuscarPorDiaMes(int codigoCentroDescarregamento, int dia, int mes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>()
                .Where(obj => obj.PeriodoDescarregamento.CentroDescarregamento.Codigo == codigoCentroDescarregamento && obj.PeriodoDescarregamento.DiaDoMes == dia && obj.PeriodoDescarregamento.Mes == mes);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa> BuscarPorExcecao(int codigoExcecaoCapacidadeDescarregamento)
        {
            var consultaPeriodoDescarregamentoGrupoPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoGrupoPessoa>()
                .Where(o => o.PeriodoDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecaoCapacidadeDescarregamento);

            return consultaPeriodoDescarregamentoGrupoPessoa.ToList();
        }
    }
}
