using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe
{
    public sealed class AprovacaoAlcadaIntegracaoCTe : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe,
        Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe,
        Dominio.Entidades.Embarcador.Cargas.Carga
    >
    {
        #region Construtores

        public AprovacaoAlcadaIntegracaoCTe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe filtrosPesquisa)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(obj => obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consultaCarga = consultaCarga.Where(o => o.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCarga = consultaCarga.Where(o => o.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaCarga = consultaCarga.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoAutorizacaoIntegracaoCTe.HasValue)
                consultaCarga = consultaCarga.Where(o => o.SituacaoAutorizacaoIntegracaoCTe == filtrosPesquisa.SituacaoAutorizacaoIntegracaoCTe.Value);

            var consultaAlcadaLiberacaoEscrituracaoPagamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaLiberacaoEscrituracaoPagamentoCarga = consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoAutorizacaoIntegracaoCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao)
                consultaAlcadaLiberacaoEscrituracaoPagamentoCarga = consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoAutorizacaoIntegracaoCTe.HasValue)
                return consultaCarga.Where(o => consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCarga.Where(o => consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            consultaCarga = consultaCarga
                .Fetch(o => o.Filial)
                .Fetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.TipoDeCarga)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.DadosSumarizados);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        #endregion
    }
}
