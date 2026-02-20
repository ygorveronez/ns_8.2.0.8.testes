using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class ContaPagar : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.ContaPagar>
    {
        #region Constructores

        public ContaPagar(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Financeiro.ContaPagar> Consultar(DateTime? dataInicial, DateTime? dataFinal, SituacaoProcessamentoArquivo situacaoProcessamento, int codigoTermo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametros)
        {
            var consulta = _Consultar(dataInicial, dataFinal, situacaoProcessamento, codigoTermo);
            return ObterLista(consulta, parametros);
        }

        public int ContarConsultar(DateTime? dataInicial, DateTime? dataFinal, SituacaoProcessamentoArquivo situacaoProcessamento,int codigoTermo)
        {
            var consulta = _Consultar(dataInicial, dataFinal, situacaoProcessamento, codigoTermo);
            return consulta.Count();
        }

        public List<int> ObterContaApagarAguardandoProcessamento()
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.ContaPagar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContaPagar>();
            query = query.Where(q => q.Situacao == SituacaoProcessamentoArquivo.AguardandoProcessamento);

            return query.Select(x => x.Codigo).Take(50).ToList();
        }
        #endregion

        #region Metodos Privados
        public IQueryable<Dominio.Entidades.Embarcador.Financeiro.ContaPagar> _Consultar(DateTime? dataInicial, DateTime? dataFinal, SituacaoProcessamentoArquivo situacaoProcessamento, int codigoTermo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.ContaPagar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.ContaPagar>();

            if (dataInicial.HasValue)
                query = query.Where(q => q.DataIntegracao >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(q => q.DataIntegracao <= dataFinal.Value.AddDays(1));

            if (situacaoProcessamento != SituacaoProcessamentoArquivo.Todos)
                query = query.Where(q => q.Situacao == situacaoProcessamento);

            if (codigoTermo > 0)
                query = query.Where(q => q.TermoQuitacaoFinanceiro.Codigo == codigoTermo);

            return query;
        }

        #endregion
    }
}
