using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class InicioViagem : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga, IFluxoGestaoPatioEtapaRetornada
    {
        #region Atributos Privados

        private readonly GuaritaBase _guaritaBase;

        #endregion

        #region Construtores

        public InicioViagem(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public InicioViagem(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.InicioViagem, cliente)
        {
            _guaritaBase = new GuaritaBase(unitOfWork, auditado);
        }

        #endregion

        #region Métodos Privados

        private bool IsSituacaoCargaPermiteInicioViagem(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(guarita.FluxoGestaoPatio);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = guarita.Carga;

            if (carga == null)
                return true;

            if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete)
                if (sequenciaGestaoPatio?.GuaritaSaidaIniciarEmissaoDocumentosTransporte ?? false)
                    return true;

            if (!carga.DataFinalizacaoEmissao.HasValue && guarita.FluxoGestaoPatio.DataFaturamento.HasValue)
                return true;

            if (configuracao.EncerrarCargaQuandoFinalizarGestaoPatio && carga.SituacaoCarga != SituacaoCarga.EmTransporte && carga.SituacaoCarga != SituacaoCarga.Encerrada)
                return false;

            return !(
                carga.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos &&
                carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                carga.SituacaoCarga != SituacaoCarga.LiberadoPagamento &&
                !configuracao.PermitirAdicionarCargaFluxoPatio
            );
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            _guaritaBase.Adicionar(fluxoGestaoPatioEtapaAdicionar, EtapaFluxoGestaoPatio.InicioViagem);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            _guaritaBase.DefinirCarga(fluxoGestaoPatio, carga, etapaLiberada);
        }

        public void EtapaRetornada(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                return;

            guarita.Situacao = SituacaoCargaGuarita.Liberada;
            guarita.DataEntregaGuarita = null;

            repositorioGuarita.Atualizar(guarita);
        }

        public bool IsSituacaoCargaPermiteInicioViagem(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

            return IsSituacaoCargaPermiteInicioViagem(guarita, configuracao);
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            _guaritaBase.TrocarCarga(fluxoGestaoPatio, cargaNova);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataInicioViagemPrevista.HasValue)
                fluxoGestaoPatio.DataInicioViagemPrevista = preSetTempoEtapa.DataInicioViagemPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

            if (!configuracao.InformarDadosChegadaVeiculoNoFluxoPatio && !guarita.EtapaGuaritaLiberada)
                throw new ServicoException("A informação de chegada do do veículo ainda não foi autorizada.");

            guarita.Initialize();

            if (!IsSituacaoCargaPermiteInicioViagem(guarita, configuracao))
                throw new ServicoException("Não é possível informar a saída do veículo na atual situação da carga.");

            guarita.DataSaidaGuarita = DateTime.Now;
            guarita.Situacao = SituacaoCargaGuarita.SaidaLiberada;

            if (_auditado != null)
                Auditoria.Auditoria.Auditar(_auditado, guarita, null, "Informou Saída do Veículo", _unitOfWork);

            if (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioViagem)
            {
                LiberarProximaEtapa(fluxoGestaoPatio);

                if (guarita.Carga != null)
                {
                    if (configuracao.PossuiMonitoramento)
                        Monitoramento.Monitoramento.IniciarMonitoramento(guarita.Carga, guarita.DataSaidaGuarita, configuracao, _auditado, _unitOfWork);
                }
            }

            repositorioGuarita.Atualizar(guarita, _auditado);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataInicioViagemPrevista = preSetTempoEtapa.DataInicioViagemPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataInicioViagem.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgInicioViagem = tempoEtapaAnterior;
            fluxoGestaoPatio.DataInicioViagem = data;

            if (fluxoGestaoPatio.PreCarga != null)
            {
                fluxoGestaoPatio.PreCarga.DataInicioViagem = data;

                new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork).Atualizar(fluxoGestaoPatio.PreCarga);
            }

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (guarita != null)
            {
                guarita.EtapaSaidaGuaritaLiberada = true;
                guarita.Situacao = SituacaoCargaGuarita.Liberada;

                repositorioGuarita.Atualizar(guarita);

                return true;
            }

            return false;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioViagem;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataInicioViagemPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita repositorioGuarita = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamentoGuarita(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoGuarita guarita = repositorioGuarita.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            if (guarita != null)
            {
                guarita.EtapaSaidaGuaritaLiberada = false;
                guarita.Situacao = (sequenciaGestaoPatio?.ChegadaVeiculo ?? false) ? SituacaoCargaGuarita.AgChegadaVeiculo : SituacaoCargaGuarita.AguardandoLiberacao;

                repositorioGuarita.Atualizar(guarita);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgInicioViagem = 0;
            fluxoGestaoPatio.DataInicioViagem = null;

            if (fluxoGestaoPatio.PreCarga != null)
            {
                fluxoGestaoPatio.PreCarga.DataInicioViagem = null;

                new Repositorio.Embarcador.PreCargas.PreCarga(_unitOfWork).Atualizar(fluxoGestaoPatio.PreCarga);
            }
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataInicioViagemPrevista.HasValue)
                fluxoGestaoPatio.DataInicioViagemReprogramada = fluxoGestaoPatio.DataInicioViagemPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
