using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class SolicitacaoVeiculo : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public SolicitacaoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public SolicitacaoVeiculo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.SolicitacaoVeiculo, cliente) { }

        #endregion

        #region Métodos Privados

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo)
        {
            if (solicitacaoVeiculo == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!solicitacaoVeiculo.EtapaSolicitacaoVeiculoLiberada)
                throw new ServicoException("A solicitação do veículo ainda não foi autorizada.");

            SalvarDadosTransporteCarga(fluxoGestaoPatio, solicitacaoVeiculo);

            solicitacaoVeiculo.Initialize();
            solicitacaoVeiculo.DataSolicitacaoVeiculoIniciada = DateTime.Now;
            solicitacaoVeiculo.Situacao = SituacaoSolicitacaoVeiculo.VeiculoSolicitado;

            LiberarProximaEtapa(fluxoGestaoPatio);

            new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork).Atualizar(solicitacaoVeiculo, _auditado);
        }

        private void SalvarDadosTransporteCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo)
        {
            FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio.Tipo, fluxoGestaoPatio.Filial.Codigo, fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0);

            if (!sequenciaGestaoPatio?.SolicitacaoVeiculoPermitirInformarDadosTransporteCarga ?? false)
                return;

            Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio repDadosTransporteFluxoPatio = new Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            
            Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio dadosTransporteFluxoPatio = repDadosTransporteFluxoPatio.BuscarPorSolicitacaoVeiculo(solicitacaoVeiculo.Codigo);

            if (dadosTransporteFluxoPatio == null)
                throw new ServicoException("Obrigatório informar os Dados de Transporte");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = solicitacaoVeiculo.Carga;

            carga.Veiculo = dadosTransporteFluxoPatio.Veiculo;
            carga.Motoristas = new List<Dominio.Entidades.Usuario>() { dadosTransporteFluxoPatio.Motorista };
            
            repCarga.Atualizar(carga);
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (solicitacaoVeiculo != null)
                return;

            solicitacaoVeiculo = new Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaSolicitacaoVeiculoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoSolicitacaoVeiculo.AguardandoSolicitacaoVeiculo
            };

            repositorioSolicitacaoVeiculo.Inserir(solicitacaoVeiculo);
        }

        public void Avancar(int codigo)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorCodigo(codigo);

            AvancarEtapa(solicitacaoVeiculo?.FluxoGestaoPatio, solicitacaoVeiculo);

            HubsMobile.NotificacaoMobile servicoNotificacaoMobile = new HubsMobile.NotificacaoMobile();

            if (solicitacaoVeiculo.Carga.Motoristas != null)
            {
                foreach (Dominio.Entidades.Usuario motorista in solicitacaoVeiculo.Carga.Motoristas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados notificacao = new Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados()
                    {
                        Assunto = "Veículo Solicitado",
                        Mensagem = "Veículo solicitado, por favor dirija-se a área de carregamento.",
                        Tipo = TipoNotificacaoMobile.Mensagem,
                        Usuario = motorista
                    };

                    servicoNotificacaoMobile.Notificar(notificacao, _unitOfWork);
                }
            }
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (solicitacaoVeiculo != null)
            {
                solicitacaoVeiculo.Carga = carga;
                repositorioSolicitacaoVeiculo.Atualizar(solicitacaoVeiculo);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (solicitacaoVeiculo != null)
            {
                solicitacaoVeiculo.Carga = cargaNova;
                repositorioSolicitacaoVeiculo.Atualizar(solicitacaoVeiculo);
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataSolicitacaoVeiculoPrevista.HasValue)
                fluxoGestaoPatio.DataSolicitacaoVeiculoPrevista = preSetTempoEtapa.DataSolicitacaoVeiculoPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, solicitacaoVeiculo);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataSolicitacaoVeiculoPrevista = preSetTempoEtapa.DataSolicitacaoVeiculoPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataSolicitacaoVeiculo.HasValue)
                return false;

            fluxoGestaoPatio.TempoAguardandoSolicitacaoVeiculo = tempoEtapaAnterior;
            fluxoGestaoPatio.DataSolicitacaoVeiculo = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (solicitacaoVeiculo != null)
            {
                solicitacaoVeiculo.EtapaSolicitacaoVeiculoLiberada = true;
                repositorioSolicitacaoVeiculo.Atualizar(solicitacaoVeiculo);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataSolicitacaoVeiculo;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataSolicitacaoVeiculoPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (solicitacaoVeiculo != null)
            {
                solicitacaoVeiculo.EtapaSolicitacaoVeiculoLiberada = false;
                repositorioSolicitacaoVeiculo.Atualizar(solicitacaoVeiculo);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAguardandoSolicitacaoVeiculo = 0;
            fluxoGestaoPatio.DataSolicitacaoVeiculo = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataSolicitacaoVeiculoPrevista.HasValue)
                fluxoGestaoPatio.DataSolicitacaoVeiculoReprogramada = fluxoGestaoPatio.DataSolicitacaoVeiculoPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
