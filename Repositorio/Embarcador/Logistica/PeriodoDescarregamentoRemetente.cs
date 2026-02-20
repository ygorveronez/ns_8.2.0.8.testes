using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PeriodoDescarregamentoRemetente : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente>
    {
        public PeriodoDescarregamentoRemetente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> BuscarPorPeriodoDescarregamento(int codigoPeriodoDescarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente>();

            var result = from obj in query where obj.PeriodoDescarregamento.Codigo == codigoPeriodoDescarregamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> BuscarPorPeriodosDescarregamento(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente>();

            int quantidadePorBusca = 2000;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> resultadoFinal = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente>();

            for (int i = 0; i < codigos.Count; i += quantidadePorBusca)
            {
                List<int> codigosAtuais = codigos.Skip(i).Take(quantidadePorBusca).ToList();

                var resultadoParcial = from obj in query where codigosAtuais.Contains(obj.PeriodoDescarregamento.Codigo) select obj;

                resultadoFinal.AddRange(resultadoParcial.ToList());
            }

            return resultadoFinal;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente> BuscarPorExcecao(int codigoExcecaoCapacidadeDescarregamento)
        {
            var consultaPeriodoDescarregamentoRemetente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamentoRemetente>()
                .Where(o => o.PeriodoDescarregamento.ExcecaoCapacidadeDescarregamento.Codigo == codigoExcecaoCapacidadeDescarregamento);

            return consultaPeriodoDescarregamentoRemetente.ToList();
        }
    }
}
