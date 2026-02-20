using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento
{
    public sealed class AprovacaoAlcadaCargaCancelamento : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento,
        Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao
    >
    {
        #region Construtores

        public AprovacaoAlcadaCargaCancelamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao filtrosPesquisa)
        {
            var consultaCargaCancelamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.CargaCancelamento.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.CargaCancelamento.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.CargaCancelamento.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.CargaCancelamento.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.CargaCancelamento.Carga.DataCriacaoCarga.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.CargaCancelamento.Carga.DataCriacaoCarga.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.CargaCancelamento.Carga.PortoOrigem.Codigo == filtrosPesquisa.CodigoPortoOrigem);

            if (filtrosPesquisa.CodigoPortoDestino > 0)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.CargaCancelamento.Carga.PortoDestino.Codigo == filtrosPesquisa.CodigoPortoDestino);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.CargaCancelamento.Carga.Empresa.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.CpfCnpjTomador > 0)
            {
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o =>
                    o.CargaCancelamento.Carga.Pedidos.Any(p =>
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && p.Pedido.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && p.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && p.Tomador.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && p.Recebedor.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador) ||
                        (p.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && p.Expedidor.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomador)
                    )
                );
            }

            if (filtrosPesquisa.SituacaoCargaCancelamentoSolicitacao.HasValue)
            {
                consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao.Where(o => o.Situacao == filtrosPesquisa.SituacaoCargaCancelamentoSolicitacao.Value);

                if (filtrosPesquisa.SituacaoCargaCancelamentoSolicitacao.Value == SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao)
                    return consultaCargaCancelamentoSolicitacao;
            }

            var consultaAlcadaCargaCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaCargaCancelamento = consultaAlcadaCargaCancelamento.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.TipoAprovadorRegra.HasValue)
                consultaAlcadaCargaCancelamento = consultaAlcadaCargaCancelamento.Where(o => ((TipoAprovadorRegra?)o.TipoAprovadorRegra ?? TipoAprovadorRegra.Usuario) == filtrosPesquisa.TipoAprovadorRegra.Value);

            if (filtrosPesquisa.SituacaoCargaCancelamentoSolicitacao == SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao)
                consultaAlcadaCargaCancelamento = consultaAlcadaCargaCancelamento.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoCargaCancelamentoSolicitacao.HasValue)
                return consultaCargaCancelamentoSolicitacao.Where(o => consultaAlcadaCargaCancelamento.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCargaCancelamentoSolicitacao.Where(o =>
                o.Situacao == SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao ||
                consultaAlcadaCargaCancelamento.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaCancelamentoSolicitacao = Consultar(filtrosPesquisa);

            consultaCargaCancelamentoSolicitacao = consultaCargaCancelamentoSolicitacao
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.Filial)
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.TipoDeCarga)
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.PortoDestino)
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.PortoOrigem)
                .Fetch(o => o.CargaCancelamento).ThenFetch(o => o.Carga).ThenFetch(o => o.DadosSumarizados);

            return ObterLista(consultaCargaCancelamentoSolicitacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao filtrosPesquisa)
        {
            var consultaCargaCancelamentoSolicitacao = Consultar(filtrosPesquisa);

            return consultaCargaCancelamentoSolicitacao.Count();
        }

        #endregion
    }
}
