using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga
{
    public sealed class AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga,
        Dominio.Entidades.Embarcador.Cargas.Carga
    >
    {
        #region Construtores

        public AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga filtrosPesquisa)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Carga>()
                .Where(obj => obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && obj.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCarga = consultaCarga.Where(o => o.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaCarga = consultaCarga.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCarga = consultaCarga.Where(o => o.DataCriacaoCarga.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoLiberacaoEscrituracaoPagamentoCarga.HasValue)
            {
                consultaCarga = consultaCarga.Where(o => o.SituacaoLiberacaoEscrituracaoPagamentoCarga == filtrosPesquisa.SituacaoLiberacaoEscrituracaoPagamentoCarga.Value);

                if (filtrosPesquisa.SituacaoLiberacaoEscrituracaoPagamentoCarga.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao)
                    return consultaCarga;
            }

            var consultaAlcadaLiberacaoEscrituracaoPagamentoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaLiberacaoEscrituracaoPagamentoCarga = consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoLiberacaoEscrituracaoPagamentoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao)
                consultaAlcadaLiberacaoEscrituracaoPagamentoCarga = consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoLiberacaoEscrituracaoPagamentoCarga.HasValue)
                return consultaCarga.Where(o => consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCarga.Where(o =>
                o.SituacaoLiberacaoEscrituracaoPagamentoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao ||
                consultaAlcadaLiberacaoEscrituracaoPagamentoCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> Consultar(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        #endregion
    }
}
