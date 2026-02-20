using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoGuarita : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>
    {
        #region Construtores

        public CargaJanelaCarregamentoGuarita(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCarregamentoGuarita filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o =>
                    o.HorarioEntradaDefinido &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            if (filtrosPesquisa.CodigosCentrosCarregamento?.Count > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => filtrosPesquisa.CodigosCentrosCarregamento.Contains(o.CargaJanelaCarregamento.CentroCarregamento.Codigo));

            if (filtrosPesquisa.DataInicialCarregamento.HasValue)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.DataProgramadaParaChegada >= filtrosPesquisa.DataInicialCarregamento.Value.Date);

            if (filtrosPesquisa.DataFinalCarregamento.HasValue)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.DataProgramadaParaChegada <= filtrosPesquisa.DataFinalCarregamento.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.DataInicialChegada.HasValue)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.DataChegadaVeiculo >= filtrosPesquisa.DataInicialChegada.Value.Date);

            if (filtrosPesquisa.DataFinalChegada.HasValue)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.DataChegadaVeiculo <= filtrosPesquisa.DataFinalChegada.Value.Date.Add(DateTime.MaxValue.TimeOfDay));


            if (!filtrosPesquisa.CodigosMotoristas.IsNullOrEmpty())
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Carga.Motoristas.Any(mot => filtrosPesquisa.CodigosMotoristas.Contains(mot.Codigo)) || o.PreCarga.Motoristas.Any(mot => filtrosPesquisa.CodigosMotoristas.Contains(mot.Codigo)));

            if (!filtrosPesquisa.CodigosTransportadores.IsNullOrEmpty())
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => (filtrosPesquisa.CodigosTransportadores.Contains(o.Carga.Empresa.Codigo) || filtrosPesquisa.CodigosTransportadores.Contains(o.PreCarga.Empresa.Codigo)));

            if (filtrosPesquisa.CentroCarregamento > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.CargaJanelaCarregamento.CentroCarregamento.Codigo == filtrosPesquisa.CentroCarregamento);

            if (filtrosPesquisa.Situacao.HasValue)
            {
                if (filtrosPesquisa.Situacao.Value == SituacaoCargaGuarita.AguardandoLiberacao)
                    consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Situacao == SituacaoCargaGuarita.AguardandoLiberacao || o.Situacao == SituacaoCargaGuarita.AgChegadaVeiculo);
                else
                    consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);
            }

            if (filtrosPesquisa.DataAgendada.HasValue)
            {
                var queryAgendamentoColeta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta>();

                queryAgendamentoColeta = queryAgendamentoColeta.Where(o => o.DataEntrega >= filtrosPesquisa.DataAgendada && o.DataEntrega < filtrosPesquisa.DataAgendada.Value.AddDays(1));

                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(obj => queryAgendamentoColeta.Any(o => o.Carga.Codigo == obj.Carga.Codigo));
            }

            if (filtrosPesquisa.CodigosTipoOperacoes?.Count > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => filtrosPesquisa.CodigosTipoOperacoes.Contains(o.Carga.TipoOperacao.Codigo));

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

            if (!string.IsNullOrEmpty(filtrosPesquisa.NumeroCarga))
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Carga.CodigoCargaEmbarcador == filtrosPesquisa.NumeroCarga);

            if (filtrosPesquisa.ListaCodigoCarga?.Count > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => filtrosPesquisa.ListaCodigoCarga.Contains(o.Carga.Codigo));

            if (filtrosPesquisa.CodigosFiliais?.Count > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => filtrosPesquisa.CodigosFiliais.Contains(o.Carga.Filial.Codigo));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Carga.Pedidos.Any(ped => ped.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario));

            if (!filtrosPesquisa.CodigosVeiculos.IsNullOrEmpty())
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => filtrosPesquisa.CodigosVeiculos.Contains(o.Carga.Veiculo.Codigo) || o.Carga.VeiculosVinculados.Any(v => filtrosPesquisa.CodigosVeiculos.Contains(v.Codigo)) || (filtrosPesquisa.CodigosVeiculos.Contains(o.PreCarga.Veiculo.Codigo) || o.PreCarga.Veiculo.VeiculosVinculados.Any(veic => filtrosPesquisa.CodigosVeiculos.Contains(veic.Codigo))));

            return consultaCargaJanelaCarregamentoGuarita;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> ConsultarRelatorioTempoCarregamento(DateTime dataInicioCarregamento, DateTime dataFimCarregamento, int codigoTransportador, int codigoVeiculo, int codigoMotorista, int centroCarregamento)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o =>
                    o.CargaJanelaCarregamento.CentroCarregamento != null &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    (
                        (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                        (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                    )
                );

            if (dataInicioCarregamento != DateTime.MinValue)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.CargaJanelaCarregamento.InicioCarregamento >= dataInicioCarregamento);

            if (dataFimCarregamento != DateTime.MinValue)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.CargaJanelaCarregamento.InicioCarregamento < dataFimCarregamento.AddDays(1));

            if (codigoMotorista > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Carga.Motoristas.Any(mot => mot.Codigo == codigoMotorista));

            if (codigoVeiculo > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Carga.Veiculo.Codigo == codigoVeiculo || o.Carga.VeiculosVinculados.Any(veic => veic.Codigo == codigoVeiculo));

            if (codigoTransportador > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.Carga.Empresa.Codigo == codigoTransportador);

            if (centroCarregamento > 0)
                consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita.Where(o => o.CargaJanelaCarregamento.CentroCarregamento.Codigo == centroCarregamento);

            return consultaCargaJanelaCarregamentoGuarita;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita BuscarPorCodigo(int codigo)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o => o.Codigo == codigo);

            return consultaCargaJanelaCarregamentoGuarita
                .Fetch(o => o.CargaJanelaCarregamento).ThenFetch(o => o.CentroCarregamento)
                .Fetch(o => o.FluxoGestaoPatio)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> BuscarPorCodigos(List<int> codigos)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o => codigos.Contains(o.Codigo));

            return consultaCargaJanelaCarregamentoGuarita
                .Fetch(o => o.CargaJanelaCarregamento).ThenFetch(o => o.CentroCarregamento)
                .Fetch(o => o.FluxoGestaoPatio)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> BuscarPorCargaJanelaCarregamento(int codigoCargaJanelaCarregamento)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o => o.CargaJanelaCarregamento.Codigo == codigoCargaJanelaCarregamento);

            return consultaCargaJanelaCarregamentoGuarita.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita BuscarPorCarga(int codigoCarga)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o =>
                    o.Carga.Codigo == codigoCarga &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.FluxoGestaoPatio.Tipo == TipoFluxoGestaoPatio.Origem
                );

            return consultaCargaJanelaCarregamentoGuarita
                .OrderByDescending(o => o.Codigo)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaCargaJanelaCarregamentoGuarita.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCarregamentoGuarita filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaJanelaCarregamentoGuarita = Consultar(filtrosPesquisa);

            consultaCargaJanelaCarregamentoGuarita = consultaCargaJanelaCarregamentoGuarita
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.Carga).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.Carga).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.Veiculo)
                .Fetch(o => o.PreCarga).ThenFetch(o => o.ModeloVeicularCarga)
                .Fetch(o => o.CargaJanelaCarregamento);

            return ObterLista(consultaCargaJanelaCarregamentoGuarita, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCarregamentoGuarita filtrosPesquisa)
        {
            var consultaCargaJanelaCarregamentoGuarita = Consultar(filtrosPesquisa);

            return consultaCargaJanelaCarregamentoGuarita.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> ConsultarRelatorioTempoCarregamento(DateTime dataInicioCarregamento, DateTime dataFimCarregamento, int codigoTransportador, int codigoVeiculo, int codigoMotorista, int centroCarregamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCargaJanelaCarregamentoGuarita = ConsultarRelatorioTempoCarregamento(dataInicioCarregamento, dataFimCarregamento, codigoTransportador, codigoVeiculo, codigoMotorista, centroCarregamento);

            return ObterLista(consultaCargaJanelaCarregamentoGuarita, parametrosConsulta);
        }

        public int ContarConsultaRelatorioTempoCarregamento(DateTime dataInicioCarregamento, DateTime dataFimCarregamento, int codigoTransportador, int codigoVeiculo, int codigoMotorista, int centroCarregamento)
        {
            var consultaCargaJanelaCarregamentoGuarita = ConsultarRelatorioTempoCarregamento(dataInicioCarregamento, dataFimCarregamento, codigoTransportador, codigoVeiculo, codigoMotorista, centroCarregamento);

            return consultaCargaJanelaCarregamentoGuarita.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> ConsultarVeiculosAtrazados(int inicioRegistros, int maximoRegistros)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o =>
                    (o.Situacao == SituacaoCargaGuarita.AguardandoLiberacao || o.Situacao == SituacaoCargaGuarita.AgChegadaVeiculo) &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.CargaJanelaCarregamento.CentroCarregamento != null &&
                    o.CargaJanelaCarregamento.CentroCarregamento.IndicarTemposVeiculos &&
                    o.CargaJanelaCarregamento.InicioCarregamento <= DateTime.Now &&
                    o.CargaJanelaCarregamento.InicioCarregamento >= DateTime.Now.Date &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            return consultaCargaJanelaCarregamentoGuarita
                .OrderBy(obj => obj.CargaJanelaCarregamento.InicioCarregamento)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> ConsultarVeiculosEmCarregamento(int inicioRegistros, int maximoRegistros)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o =>
                    o.CargaJanelaCarregamento.CentroCarregamento != null &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.Situacao == SituacaoCargaGuarita.Liberada &&
                    o.DataEntregaGuarita >= DateTime.Now.Date &&
                    o.DataFinalCarregamento == null &&
                    (o.Carga != null && o.Carga.OcultarNoPatio == false && o.Carga.SituacaoCarga != SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != SituacaoCarga.Anulada) ||
                    (o.Carga == null && o.PreCarga != null && o.PreCarga.SituacaoPreCarga != SituacaoPreCarga.Cancelada)
                );

            return consultaCargaJanelaCarregamentoGuarita
                .OrderBy(obj => obj.DataEntregaGuarita)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> ConsultarVeiculosEmFaturamento(int inicioRegistros, int maximoRegistros)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o =>
                    o.CargaJanelaCarregamento.CentroCarregamento != null &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Cancelado &&
                    o.Situacao == SituacaoCargaGuarita.Liberada &&
                    o.DataFinalCarregamento != null &&
                    o.Carga != null &&
                    o.Carga.OcultarNoPatio == false &&
                    (o.Carga.SituacaoCarga == SituacaoCarga.AgNFe || o.Carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos)
                );

            return consultaCargaJanelaCarregamentoGuarita
                .OrderBy(obj => obj.DataEntregaGuarita)
                .Fetch(o => o.Carga).ThenFetch(o => o.Empresa)
                .Fetch(o => o.PreCarga)
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> BuscarPorCargasJanelaCarregamento(List<int> codigos)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o => codigos.Contains(o.CargaJanelaCarregamento.Codigo));

            return consultaCargaJanelaCarregamentoGuarita.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita> BuscarSemRegraAprovacaoPorCodigos(List<int> codigosGuarita)
        {
            var consultaCargaJanelaCarregamentoGuarita = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita>()
                .Where(o => codigosGuarita.Contains(o.Codigo) && o.SituacaoPesagemCarga == SituacaoPesagemCarga.SemRegraAprovacao);

            return consultaCargaJanelaCarregamentoGuarita.ToList();
        }

        #endregion
    }
}
