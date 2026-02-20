using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Embarcador.GestaoPatio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class InformarDoca : FluxoGestaoPatioEtapa, IFluxoGestaoPatioEtapaAdicionar, IFluxoGestaoPatioEtapaAlterarCarga
    {
        #region Construtores

        public InformarDoca(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public InformarDoca(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, EtapaFluxoGestaoPatio.InformarDoca, cliente) { }

        #endregion

        #region Métodos Privados

        private void AtualizarNumeroDocaCarga(Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento)
        {
            if (!docaCarregamento.Carga.IsCarga())
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            if (string.IsNullOrWhiteSpace(docaCarregamento.Carga.NumeroDoca))
                docaCarregamento.Carga.NumeroDoca = docaCarregamento.NumeroDoca;

            docaCarregamento.Carga.NumeroDocaEncosta = docaCarregamento.NumeroDoca;

            repositorioCarga.Atualizar(docaCarregamento.Carga);
            Auditoria.Auditoria.Auditar(_auditado, docaCarregamento.Carga, null, "Alterou número da doca de carregamento", _unitOfWork);

            Servicos.Embarcador.Integracao.Eship.IntegracaoEship serEShip = new Servicos.Embarcador.Integracao.Eship.IntegracaoEship(_unitOfWork);
            serEShip.VerificarIntegracaoEShip(docaCarregamento.Carga);
        }

        private void AtualizarSituacaoCargaJanelaCarregamentoParaProntaParaCarregamento(Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento)
        {
            if (!docaCarregamento.CargaBase.IsCarga())
                return;

            new Logistica.FilaCarregamentoVeiculo(_unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema).AtualizarSituacaoCargaJanelaCarregamentoParaProntaParaCarregamento(docaCarregamento.Carga.Codigo);
        }

        private void AvancarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaAvancarEtapa informarDocaAvancarEtapa)
        {
            if (docaCarregamento == null)
                throw new ServicoException("Não foi possível encontrar o registro.");

            if (!docaCarregamento.EtapaDocaCarregamentoLiberada && (!informarDocaAvancarEtapa?.EtapaAntecipada ?? false))
                throw new ServicoException("A liberação da doca ainda não foi autorizada.");

            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);

            docaCarregamento.Initialize();
            docaCarregamento.DataInformacaoDoca = DateTime.Now;
            docaCarregamento.PossuiLaudo = informarDocaAvancarEtapa?.PossuiLaudo ?? false;
            docaCarregamento.Situacao = SituacaoDocaCarregamento.Informada;

            if (informarDocaAvancarEtapa != null)
            {
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

                if (configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento)
                {
                    Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioLocalCarregamento = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(_unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao localCarregamento = (informarDocaAvancarEtapa.CodigoLocalCarregamento > 0) ? repositorioLocalCarregamento.BuscarPorCodigo(informarDocaAvancarEtapa.CodigoLocalCarregamento) : null;

                    if (docaCarregamento.CargaBase.IsCarga())
                    {
                        if (localCarregamento == null)
                            throw new ServicoException("O local de carregamento deve ser informado.");

                        if (!IsLimiteCargasPorLocalCarregamentoDisponivel(docaCarregamento, localCarregamento))
                            throw new ServicoException("O limite de cargas por local de carregamento foi atingido.");
                    }

                    if (configuracaoGestaoPatio.NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca)
                    {
                        Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamentoExistente = repositorioDocaCarregamento.BuscarPorLocalCarregamento(localCarregamento.Codigo);

                        if (docaCarregamentoExistente != null && docaCarregamento.Carga?.Codigo != docaCarregamentoExistente.Carga?.Codigo)
                            throw new ServicoException($"Não é possível informar essa doca pois a carga {docaCarregamentoExistente.Carga.CodigoCargaEmbarcador} está atribuida para essa doca e não teve o processo de carregamento concluído");
                    }

                    docaCarregamento.LocalCarregamento = localCarregamento;
                    docaCarregamento.NumeroDoca = localCarregamento?.Descricao ?? string.Empty;
                }
                else
                {
                    if (docaCarregamento.CargaBase.IsCarga())
                    {
                        if (string.IsNullOrWhiteSpace(informarDocaAvancarEtapa.NumeroDoca))
                            throw new ControllerException("O número da doca deve ser informado");

                        if (configuracaoGestaoPatio.NaoPermitirInformarMaisDeUmVeiculoPorVezNaDoca)
                        {
                            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamentoExistente = repositorioDocaCarregamento.BuscarPorNumeroDoca(informarDocaAvancarEtapa.NumeroDoca);

                            if (docaCarregamentoExistente != null && docaCarregamento.Carga?.Codigo != docaCarregamentoExistente.Carga?.Codigo)
                                throw new ServicoException($"Não é possível informar essa doca pois a carga {docaCarregamentoExistente.Carga.CodigoCargaEmbarcador} está atribuida para essa doca e não teve o processo de carregamento concluído");
                        }

                        Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

                        if (configuracaoIntegracao?.PossuiIntegracaoGPA ?? false)
                        {
                            Repositorio.Embarcador.Logistica.CentroCarregamentoDoca repositorioCentroCarregamentoDoca = new Repositorio.Embarcador.Logistica.CentroCarregamentoDoca(_unitOfWork);
                            Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca docaCentroCarregamento = repositorioCentroCarregamentoDoca.BuscarPorFilialDescricao(docaCarregamento.FluxoGestaoPatio.Filial?.Codigo ?? 0, informarDocaAvancarEtapa.NumeroDoca);

                            if (docaCentroCarregamento == null)
                                throw new ControllerException($"Doca {informarDocaAvancarEtapa.NumeroDoca} não configurada no centro de carregamento.");
                        }
                    }

                    docaCarregamento.LocalCarregamento = null;
                    docaCarregamento.NumeroDoca = informarDocaAvancarEtapa.NumeroDoca;
                }
            }
            else
            {
                docaCarregamento.NumeroDoca = string.Empty;
                docaCarregamento.LocalCarregamento = null;
            }

            AtualizarNumeroDocaCarga(docaCarregamento);
            LiberarProximaEtapa(fluxoGestaoPatio);
            repositorioDocaCarregamento.Atualizar(docaCarregamento);
            AtualizarSituacaoCargaJanelaCarregamentoParaProntaParaCarregamento(docaCarregamento);
            NotificarMotoristasDocaInformada(docaCarregamento);
        }

        private void NotificarMotoristasDocaInformada(Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento)
        {
            if (!docaCarregamento.CargaBase.IsCarga() || string.IsNullOrWhiteSpace(docaCarregamento.NumeroDoca) || (docaCarregamento.Carga.Motoristas == null))
                return;

            HubsMobile.NotificacaoMobile servicoNotificacaoMobile = new HubsMobile.NotificacaoMobile();

            foreach (Dominio.Entidades.Usuario motorista in docaCarregamento.Carga.Motoristas)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados notificacao = new Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoDados()
                {
                    Assunto = "Doca Informada",
                    Mensagem = $"Doca {docaCarregamento.NumeroDoca} informada para sua carga, favor se dirigir até ela e realizar a leitura do QR Code Início de Carregamento.",
                    Tipo = TipoNotificacaoMobile.Mensagem,
                    Usuario = motorista
                };

                servicoNotificacaoMobile.Notificar(notificacao, _unitOfWork);
            }
        }

        #endregion

        #region Métodos Públicos

        public void Adicionar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoGestaoPatioEtapaAdicionar fluxoGestaoPatioEtapaAdicionar)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Codigo);

            if (docaCarregamento != null)
                return;

            docaCarregamento = new Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento()
            {
                FluxoGestaoPatio = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio,
                Carga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.Carga,
                PreCarga = fluxoGestaoPatioEtapaAdicionar.FluxoGestaoPatio.PreCarga,
                EtapaDocaCarregamentoLiberada = fluxoGestaoPatioEtapaAdicionar.EtapaLiberada,
                Situacao = SituacaoDocaCarregamento.AgInformarDoca
            };

            repositorioDocaCarregamento.Inserir(docaCarregamento);
        }

        public void Avancar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaAvancarEtapa informarDocaAvancarEtapa)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCodigo(informarDocaAvancarEtapa.Codigo);

            AvancarEtapa(docaCarregamento?.FluxoGestaoPatio, docaCarregamento, informarDocaAvancarEtapa);
        }

        public void DefinirCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (docaCarregamento != null)
            {
                docaCarregamento.Carga = carga;

                if (string.IsNullOrWhiteSpace(docaCarregamento.NumeroDoca) && !string.IsNullOrWhiteSpace(fluxoGestaoPatio.PreCarga.NumeroPreCarga))
                {
                    Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();

                    if (configuracaoGestaoPatio.InformarDocaCarregamentoUtilizarLocalCarregamento)
                    {
                        if (IsLimiteCargasPorLocalCarregamentoDisponivel(docaCarregamento, fluxoGestaoPatio.PreCarga.LocalCarregamento))
                        {
                            docaCarregamento.LocalCarregamento = fluxoGestaoPatio.PreCarga.LocalCarregamento;
                            docaCarregamento.NumeroDoca = fluxoGestaoPatio.PreCarga.LocalCarregamento?.Descricao ?? "";
                        }
                    }
                    else
                        docaCarregamento.NumeroDoca = fluxoGestaoPatio.PreCarga.NumeroPreCarga;
                }

                repositorioDocaCarregamento.Atualizar(docaCarregamento);
            }
        }

        public void TrocarCarga(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (docaCarregamento != null)
            {
                docaCarregamento.Carga = cargaNova;
                repositorioDocaCarregamento.Atualizar(docaCarregamento);
            }
        }

        public bool IsLimiteCargasPorLocalCarregamentoDisponivel(Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento, Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao localCarregamento)
        {
            if (localCarregamento == null)
                return true;

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(docaCarregamento.FluxoGestaoPatio.Filial?.Codigo ?? 0);

            if ((centroCarregamento == null) || (centroCarregamento.LimiteCargasPorLocalCarregamento == 0))
                return true;

            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            int totalCargasPorLocalCarregamento = repositorioDocaCarregamento.ContarPorFilialELocalCarregamento(docaCarregamento.Codigo, docaCarregamento.FluxoGestaoPatio.Filial?.Codigo ?? 0, localCarregamento.Codigo);

            return (centroCarregamento.LimiteCargasPorLocalCarregamento > totalCargasPorLocalCarregamento);
        }

        public void AtualizarDocaDadosTransporteCarga(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InformarDocaSalvarDadosTransporteCarga informarDocaSalvarDadosTransporteCarga)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioLocalCarregamento = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorCarga(informarDocaSalvarDadosTransporteCarga.CodigoCarga);

            if (docaCarregamento == null)
                return;

            Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao localCarregamento = repositorioLocalCarregamento.BuscarPorCodigo(informarDocaSalvarDadosTransporteCarga.CodigoLocalCarregamento);

            if (docaCarregamento.LocalCarregamento == localCarregamento)
                return;

            docaCarregamento.Initialize();
            docaCarregamento.DataInformacaoDoca = DateTime.Now;
            docaCarregamento.PossuiLaudo = informarDocaSalvarDadosTransporteCarga?.PossuiLaudo ?? false;
            docaCarregamento.Situacao = SituacaoDocaCarregamento.Informada;

            if (docaCarregamento.CargaBase.IsCarga())
            {
                if (localCarregamento == null)
                    throw new ServicoException("O local de carregamento deve ser informado.");

                if (!IsLimiteCargasPorLocalCarregamentoDisponivel(docaCarregamento, localCarregamento))
                    throw new ServicoException("O limite de cargas por local de carregamento foi atingido.");
            }

            docaCarregamento.LocalCarregamento = localCarregamento;
            docaCarregamento.NumeroDoca = localCarregamento?.Descricao ?? string.Empty;

            AtualizarNumeroDocaCarga(docaCarregamento);
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataDocaInformadaPrevista.HasValue)
                fluxoGestaoPatio.DataDocaInformadaPrevista = preSetTempoEtapa.DataDocaInformadaPrevista.Value;
        }

        public override void Avancar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            AvancarEtapa(fluxoGestaoPatio, docaCarregamento, informarDocaAvancarEtapa: null);
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataDocaInformadaPrevista = preSetTempoEtapa.DataDocaInformadaPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataDocaInformada.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgInformarDoca = tempoEtapaAnterior;
            fluxoGestaoPatio.DataDocaInformada = data;

            return true;
        }

        public override bool Liberar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (docaCarregamento != null)
            {
                docaCarregamento.EtapaDocaCarregamentoLiberada = true;
                repositorioDocaCarregamento.Atualizar(docaCarregamento);
            }

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataDocaInformada;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataDocaInformadaPrevista;
        }

        public override void RemoverLiberacao(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.GestaoPatio.DocaCarregamento repositorioDocaCarregamento = new Repositorio.Embarcador.GestaoPatio.DocaCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.DocaCarregamento docaCarregamento = repositorioDocaCarregamento.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);

            if (docaCarregamento != null)
            {
                docaCarregamento.EtapaDocaCarregamentoLiberada = false;
                repositorioDocaCarregamento.Atualizar(docaCarregamento);
            }
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgInformarDoca = 0;
            fluxoGestaoPatio.DataDocaInformada = null;
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataDocaInformadaPrevista.HasValue)
                fluxoGestaoPatio.DataDocaInformadaReprogramada = fluxoGestaoPatio.DataDocaInformadaPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}
