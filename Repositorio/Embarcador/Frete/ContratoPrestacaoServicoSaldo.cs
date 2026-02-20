using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Frete
{
    public sealed class ContratoPrestacaoServicoSaldo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo>
    {
        #region Construtores

        public ContratoPrestacaoServicoSaldo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoSaldo filtrosPesquisa)
        {
            var consultaContratoPrestacaoServicoSaldo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo>();

            if (filtrosPesquisa.CodigoContratoPrestacaoServico > 0)
                consultaContratoPrestacaoServicoSaldo = consultaContratoPrestacaoServicoSaldo.Where(o => o.ContratoPrestacaoServico.Codigo == filtrosPesquisa.CodigoContratoPrestacaoServico);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaContratoPrestacaoServicoSaldo = consultaContratoPrestacaoServicoSaldo.Where(o => o.Data >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaContratoPrestacaoServicoSaldo = consultaContratoPrestacaoServicoSaldo.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.TipoLancamento.HasValue)
                consultaContratoPrestacaoServicoSaldo = consultaContratoPrestacaoServicoSaldo.Where(o => o.TipoLancamento == filtrosPesquisa.TipoLancamento);

            return consultaContratoPrestacaoServicoSaldo;
        }

        #endregion

        #region Métodos Públicos

        public void AjustarValorUtilizado(int codigoContratoPrestacaoServico, DateTime dataBase, decimal valor)
        {
            System.Text.StringBuilder hql = new System.Text.StringBuilder();

            hql.Append("update ContratoPrestacaoServicoSaldo LancamentoSaldo ");
            hql.Append("   set LancamentoSaldo.ValorUtilizado = LancamentoSaldo.ValorUtilizado + :Valor ");
            hql.Append(" where LancamentoSaldo.ContratoPrestacaoServico.Codigo = :CodigoContratoPrestacaoServico ");
            hql.Append("   and LancamentoSaldo.Data >= :DataBase ");

            var query = this.SessionNHiBernate.CreateQuery(hql.ToString())
                .SetInt32("CodigoContratoPrestacaoServico", codigoContratoPrestacaoServico)
                .SetDateTime("DataBase", dataBase)
                .SetDecimal("Valor", valor);

            query.ExecuteUpdate();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo BuscarPorCodigo(int codigo)
        {
            var consultaContratoPrestacaoServicoSaldo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo>()
                .Where(o => o.Codigo == codigo);

            return consultaContratoPrestacaoServicoSaldo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoSaldo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoSaldo filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        public decimal ObterValorUtilizadoPorContratoPrestacaoServico(int codigoContratoPrestacaoServico)
        {
            var consultaContratoPrestacaoServicoSaldo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServicoSaldo>()
                .Where(o => o.ContratoPrestacaoServico.Codigo == codigoContratoPrestacaoServico);

            decimal valorTotalEntrada = (from saldo in consultaContratoPrestacaoServicoSaldo where saldo.TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Entrada select (decimal?)saldo.Valor).Sum() ?? 0;
            decimal valorTotalSaida = (from saldo in consultaContratoPrestacaoServicoSaldo where saldo.TipoMovimentacao == TipoMovimentacaoContratoPrestacaoServico.Saida select (decimal?)saldo.Valor).Sum() ?? 0;

            return (valorTotalSaida - valorTotalEntrada);
        }

        #endregion
    }
}
