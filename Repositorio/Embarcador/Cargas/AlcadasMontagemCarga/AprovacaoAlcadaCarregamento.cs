using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.AlcadasMontagemCarga
{
    public sealed class AprovacaoAlcadaCarregamento : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.RegraAutorizacaoCarregamento,
        Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao
    >
    {
        #region Construtores

        public AprovacaoAlcadaCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao filtrosPesquisa)
        {
            var consultaCarregamentoSolicitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao>();

            consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(obj => obj.Carregamento.SituacaoCarregamento != SituacaoCarregamento.Cancelado);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarregamento))
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => o.Carregamento.NumeroCarregamento == filtrosPesquisa.NumeroCarregamento);

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => o.Carregamento.Pedidos.Any(p => filtrosPesquisa.CodigosFilial.Contains(p.Pedido.Filial.Codigo)));

            if (filtrosPesquisa.CodigosModeloVeicularCarga?.Count > 0)
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => filtrosPesquisa.CodigosModeloVeicularCarga.Contains(o.Carregamento.ModeloVeicularCarga.Codigo));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carregamento.TipoDeCarga.Codigo));

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => o.Carregamento.DataCarregamentoCarga.Value.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => o.Carregamento.DataCarregamentoCarga.Value.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoCarregamentoSolicitacao.HasValue)
            {
                consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao.Where(o => o.Situacao == filtrosPesquisa.SituacaoCarregamentoSolicitacao.Value);

                if (filtrosPesquisa.SituacaoCarregamentoSolicitacao.Value == SituacaoCarregamentoSolicitacao.SemRegraAprovacao)
                    return consultaCarregamentoSolicitacao;
            }

            var consultaAlcadaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaCarregamento = consultaAlcadaCarregamento.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoCarregamentoSolicitacao == SituacaoCarregamentoSolicitacao.AguardandoAprovacao)
                consultaAlcadaCarregamento = consultaAlcadaCarregamento.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoCarregamentoSolicitacao.HasValue)
                return consultaCarregamentoSolicitacao.Where(o => consultaAlcadaCarregamento.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCarregamentoSolicitacao.Where(o =>
                o.Situacao == SituacaoCarregamentoSolicitacao.SemRegraAprovacao ||
                consultaAlcadaCarregamento.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }
        
        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoSolicitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarregamentoSolicitacao = Consultar(filtrosPesquisa);

            consultaCarregamentoSolicitacao = consultaCarregamentoSolicitacao
                .Fetch(o => o.Carregamento).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.Carregamento).ThenFetch(o => o.TipoDeCarga);

            return ObterLista(consultaCarregamentoSolicitacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCarregamentoAprovacao filtrosPesquisa)
        {
            var consultaCarregamentoSolicitacao = Consultar(filtrosPesquisa);

            return consultaCarregamentoSolicitacao.Count();
        }
        
        public Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento BuscarPorGuid(string guid)
        {
            var consultaAprovacaoAlcadaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasMontagemCarga.AprovacaoAlcadaCarregamento>()
                .Where(o => o.GuidCarregamento == guid);

            return consultaAprovacaoAlcadaCarregamento.FirstOrDefault();
        }

        #endregion
    }
}
