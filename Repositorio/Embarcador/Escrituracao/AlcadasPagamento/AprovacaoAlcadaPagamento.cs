using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Escrituracao.AlcadasPagamento
{
    public sealed class AprovacaoAlcadaPagamento : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.RegraAutorizacaoPagamento,
        Dominio.Entidades.Embarcador.Escrituracao.Pagamento
    >
    {
        #region Construtores

        public AprovacaoAlcadaPagamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao filtrosPesquisa)
        {
            var consultaPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>()
                .Where(o => 
                    (o.Carga == null || (o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada))
                );

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaPagamento = consultaPagamento.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaPagamento = consultaPagamento.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaPagamento = consultaPagamento.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaPagamento = consultaPagamento.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaPagamento = consultaPagamento.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoPagamento.HasValue)
            {
                consultaPagamento = consultaPagamento.Where(o => o.Situacao == filtrosPesquisa.SituacaoPagamento.Value);

                if (filtrosPesquisa.SituacaoPagamento.Value == SituacaoPagamento.SemRegraAprovacao)
                    return consultaPagamento;
            }

            var consultaAlcadaPagamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaPagamento = consultaAlcadaPagamento.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoPagamento == SituacaoPagamento.AguardandoAprovacao)
                consultaAlcadaPagamento = consultaAlcadaPagamento.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoPagamento.HasValue)
                return consultaPagamento.Where(o => consultaAlcadaPagamento.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaPagamento.Where(o =>
                o.Situacao == SituacaoPagamento.SemRegraAprovacao ||
                consultaAlcadaPagamento.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamentoAprovacao filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        #endregion
    }
}
