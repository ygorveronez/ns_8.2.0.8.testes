using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Escrituracao.AlcadaProvisaoPendente
{
    public sealed class AprovacaoAlcadoTermoQuitacaoFinanceiro : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaTermoQuitacaoFinanceiro,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente,
        Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro
    >
    {
        #region Construtores
        public AprovacaoAlcadoTermoQuitacaoFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion
        #region Metodos Publicos
        public bool PossuiRegrasCadastradas()
        {
            var consultaAlcadaTermo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente>()
                .Where(o => o.Ativo);

            return consultaAlcadaTermo.Any();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao> BuscarPorTermosQuitacao(List<int> codigosTermos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.AprovacaoAlcadaEstornoProvisao>();
            query = from obj in query where codigosTermos.Contains(obj.TermoQuitacaoFinanceiro.Codigo) select obj;
            return query.ToList();
        }
        #endregion

    }
}
