using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.GestaoPatio
{
    public class AprovacaoAlcadaToleranciaPesagem : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem,
        Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem,
        Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita
    >
    {
        #region Construtores

        public AprovacaoAlcadaToleranciaPesagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao filtrosPesquisa)
        {
            var consultaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCargaEmbarcador))
                consultaCarga = consultaCarga.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.CodigoCargaEmbarcador || o.Carga.CodigosAgrupados.Contains(filtrosPesquisa.CodigoCargaEmbarcador));

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosFilial.Contains(o.Carga.Filial.Codigo));

            if (filtrosPesquisa.CodigosModeloVeicular?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosModeloVeicular.Contains(o.Carga.MotivoSolicitacaoFrete.Codigo));

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaCarga = consultaCarga.Where(o => o.Carga.DataCriacaoCarga.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaCarga = consultaCarga.Where(o => o.Carga.DataCriacaoCarga.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCarga = consultaCarga.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (filtrosPesquisa.SituacaoPesagemCarga.HasValue)
            {
                consultaCarga = consultaCarga.Where(o => o.SituacaoPesagemCarga == filtrosPesquisa.SituacaoPesagemCarga.Value);

                if (filtrosPesquisa.SituacaoPesagemCarga.Value == SituacaoPesagemCarga.SemRegraAprovacao)
                    return consultaCarga;
            }

            var consultaAlcadaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaCarga = consultaAlcadaCarga.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoPesagemCarga == SituacaoPesagemCarga.AguardandoAprovacao)
                consultaAlcadaCarga = consultaAlcadaCarga.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoPesagemCarga.HasValue)
                return consultaCarga.Where(o => consultaAlcadaCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaCarga.Where(o =>
                o.SituacaoPesagemCarga == SituacaoPesagemCarga.SemRegraAprovacao ||
                consultaAlcadaCarga.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem> ConsultaAutorizacoes(int codigoOrigem, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem>()
                 .Where(o => o.OrigemAprovacao.Codigo == codigoOrigem);

            if (parametroConsulta != null)
            {
                if (!string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
                    consulta = consulta.OrderBy(parametroConsulta.PropriedadeOrdenar + (parametroConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

                if (parametroConsulta.InicioRegistros > 0)
                    consulta = consulta.Skip(parametroConsulta.InicioRegistros);

                if (parametroConsulta.LimiteRegistros > 0)
                    consulta = consulta.Take(parametroConsulta.LimiteRegistros);
            }

            return consulta
                .Fetch(obj => obj.RegraAutorizacao)
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            consultaCarga = consultaCarga
                .Fetch(guarita => guarita.Carga).ThenFetch(carga => carga.Filial)
                .Fetch(guarita => guarita.Carga).ThenFetch(carga => carga.ModeloVeicularCarga)
                .Fetch(guarita => guarita.Carga).ThenFetch(carga => carga.TipoDeCarga)
                .Fetch(guarita => guarita.Carga).ThenFetch(carga => carga.Empresa)
                .Fetch(guarita => guarita.Carga).ThenFetch(carga => carga.DadosSumarizados);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaPesagemAprovacao filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> ConsultaPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>();

            consulta = consulta.Where(o => o.OrigemAprovacao.Codigo == codigoCarga);

            if (situacao != null)
                consulta = consulta.Where(o => o.Situacao == situacao);

            return consulta
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga> ConsultaPorCargas(List<int> codigosCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra? situacao)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>();

            consulta = consulta.Where(o => codigosCarga.Contains(o.OrigemAprovacao.Codigo));

            if (situacao != null)
                consulta = consulta.Where(o => o.Situacao == situacao);

            return consulta
                .Fetch(obj => obj.Usuario)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga BuscarPorGuid(string guid)
        {
            var consultaAprovacaoAlcadaCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga>()
                .Where(o => o.GuidCarga == guid);

            return consultaAprovacaoAlcadaCarga.FirstOrDefault();
        }

        #endregion
    }
}
