using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCarga : LongRunningProcessBase<IntegracaoCarga>
    {
        int count = 0;

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            //Essa thread na unilever, em dados momementos demora 5~6 minutos pra finalizar um ciclo da rotina.
            //Os logs aqui adicionados, servem para entender qual o causador desse problema. Isso pode ser removido depois, mas precisamos dessas informações por agora.

            Servicos.Log.GravarInfo("______| Ciclo Iniciado |______", "IntegracaoCargaThread");
            Servicos.Log.GravarInfo("Iniciou GerarIntegracoesCarga", "IntegracaoCargaThread");
            await GerarIntegracoesCargaAsync(unitOfWork, cancellationToken);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesCarga' e iniciou 'GerarIntegracoesOcorrencia'", "IntegracaoCargaThread");
            GerarIntegracoesOcorrencia(unitOfWork, _tipoServicoMultisoftware, _stringConexao);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesOcorrencia' e iniciou 'GerarIntegracoesCancelamentoProvisao'", "IntegracaoCargaThread");
            await GerarIntegracoesCancelamentoProvisaoAsync(unitOfWork, cancellationToken);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesCancelamentoProvisao' e iniciou 'GerarIntegracoesPagamento'", "IntegracaoCargaThread");
            await GerarIntegracoesPagamentoAsync(unitOfWork, _clienteUrlAcesso, cancellationToken);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesPagamento' e iniciou 'GerarIntegracoesCancelamentoPagamento'", "IntegracaoCargaThread");
            GerarIntegracoesCancelamentoPagamento(unitOfWork, _tipoServicoMultisoftware, _stringConexao);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesCancelamentoPagamento' e iniciou 'GerarIntegracoesNFSManual'", "IntegracaoCargaThread");
            GerarIntegracoesNFSManual(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesNFSManual' e iniciou 'GerarIntegracoesNFSManualCancelamento'", "IntegracaoCargaThread");
            GerarIntegracoesNFSManualCancelamento(unitOfWork, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesNFSManualCancelamento' e iniciou 'GerarIntegracoesLoteEscrituracao'", "IntegracaoCargaThread");
            GerarIntegracoesLoteEscrituracao(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesLoteEscrituracao' e iniciou 'GerarIntegracoesLoteEscrituracaoCancelamento'", "IntegracaoCargaThread");
            GerarIntegracoesLoteEscrituracaoCancelamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesLoteEscrituracaoCancelamento' e iniciou 'GerarIntegracoesRoteirizacao'", "IntegracaoCargaThread");
            GerarIntegracoesRoteirizacao(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesCargaCTeAgrupado' e iniciou 'GerarIntegracoesCargaCTeAgrupado'", "IntegracaoCargaThread");
            GerarIntegracoesCargaCTeAgrupado(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesCargaMDFeManual' e iniciou 'GerarIntegracoesCargaMDFeManual'", "IntegracaoCargaThread");
            GerarIntegracoesCargaMDFeManual(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesCargaMDFeManualCancelamento' e iniciou 'GerarIntegracoesCargaMDFeManualCancelamento'", "IntegracaoCargaThread");
            GerarIntegracoesCargaMDFeManualCancelamento(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesRoteirizacao' e iniciou 'GerarIntegracoesFluxoPatio'", "IntegracaoCargaThread");
            new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioIntegracao(unitOfWork).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesFluxoPatio' e iniciou 'VerificarIntegracoesCanhotos'", "IntegracaoCargaThread");
            await new Servicos.Embarcador.Canhotos.CanhotoIntegracao(unitOfWork, _tipoServicoMultisoftware, _clienteUrlAcesso, cancellationToken).VerificarIntegracoesCanhotosAsync();

            Servicos.Log.GravarInfo("Finalizou 'VerificarIntegracoesCanhotos' e iniciou 'VerificarIntegracoesComprovanteEntrega'", "IntegracaoCargaThread");
            Servicos.Embarcador.Integracao.Sefaz.LoteComprovanteEntregaCte.VerificarIntegracoesComprovanteEntrega(unitOfWork, _stringConexao, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'VerificarIntegracoesComprovanteEntrega' e iniciou 'GerarIntegracoesLoteContabilizacao'", "IntegracaoCargaThread");
            Servicos.Embarcador.Integracao.Contabilizacao.IntegracaoLoteContabilizacao.GerarIntegracoesLoteContabilizacao(unitOfWork, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesLoteContabilizacao' e iniciou 'VerificarPedidoIntegracoesPendentes'", "IntegracaoCargaThread");
            Servicos.Embarcador.WMS.SeparacaoPedido.VerificarPedidoIntegracoesPendentes(unitOfWork, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'VerificarPedidoIntegracoesPendentes' e iniciou 'VerificarEntregasPendentesNotificacao'", "IntegracaoCargaThread");
            Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.VerificarEntregasPendentesNotificacao(_codigoEmpresa, _tipoServicoMultisoftware, unitOfWork, unitOfWorkAdmin);

            Servicos.Log.GravarInfo("Finalizou 'VerificarEntregasPendentesNotificacao' e iniciou 'GerarIntegracoesLoteCliente'", "IntegracaoCargaThread");
            Servicos.Embarcador.Integracao.IntegracaoLoteCliente.GerarIntegracoesLoteCliente(unitOfWork);

            Servicos.Log.GravarInfo("Finalizou 'GerarIntegracoesLoteCliente' e iniciou 'VerificarPesagemIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Logistica.Pesagem(unitOfWork).VerificarPesagemIntegracoesPendentes(_tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'VerificarPesagemIntegracoesPendentes' e iniciou 'EntregaIntegracao.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            Servicos.Embarcador.Carga.ControleEntrega.EntregaIntegracao.VerificarIntegracoesPendentes(unitOfWork, _tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'EntregaIntegracao.VerificarIntegracoesPendentes' e iniciou 'IntegracaoKuehneNagel.ProcessarArquivosRecebimento'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Integracao.KuehneNagel.IntegracaoKuehneNagel(unitOfWork).ProcessarArquivosRecebimento();

            Servicos.Log.GravarInfo("Finalizou 'IntegracaoKuehneNagel.ProcessarArquivosRecebimento' e iniciou 'VerificarIntegracoesExportacaoPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Integracao.Marfrig.IntegracaoExportacaoMarfrig(unitOfWork).VerificarIntegracoesExportacaoPendentes();

            Servicos.Log.GravarInfo("Finalizou 'VerificarIntegracoesExportacaoPendentes' e iniciou 'IntegracaoPedidoDadosTransporteMaritimo.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'IntegracaoPedidoDadosTransporteMaritimo.VerificarIntegracoesPendentes' e iniciou 'FTPSaintGobain.IniciarProcessamento'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Integracao.SaintGobain.FTPSaintGobain(unitOfWork, _tipoServicoMultisoftware).IniciarProcessamento();

            Servicos.Log.GravarInfo("Finalizou 'FTPSaintGobain.IniciarProcessamento' e iniciou 'FTPAmazon.IniciarProcessamento'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Integracao.FTPAmazon.FTPAmazon(unitOfWork, _tipoServicoMultisoftware).IniciarProcessamento();

            Servicos.Log.GravarInfo("Finalizou 'FTPAmazon.IniciarProcessamento' e iniciou 'IntegracaoProvisao.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            await new Servicos.Embarcador.Integracao.IntegracaoProvisao(unitOfWork, _tipoServicoMultisoftware, cancellationToken).VerificarIntegracoesPendentesAsync();

            Servicos.Log.GravarInfo("Finalizou 'IntegracaoProvisao.VerificarIntegracoesPendentes' e iniciou 'Chamado.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Chamado.Chamado(unitOfWork).VerificarIntegracoesPendentes(unitOfWork, _auditado, _tipoServicoMultisoftware, _clienteMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'Chamado.VerificarIntegracoesPendentes' e iniciou 'TabelaFreteIntegracao.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Frete.TabelaFreteIntegracao(unitOfWork).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'TabelaFreteIntegracao.VerificarIntegracoesPendentes' e iniciou 'TabelaFreteClienteIntegracao.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'TabelaFreteClienteIntegracao.VerificarIntegracoesPendentes' e iniciou 'ContratoFreteTransportador.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Frete.ContratoFreteTransportador(unitOfWork).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'ContratoFreteTransportador.VerificarIntegracoesPendentes' e iniciou 'ContratoTransporteFrete.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Frete.ContratoTransporteFrete(unitOfWork).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'ContratoTransporteFrete.VerificarIntegracoesPendentes' e iniciou 'VerificarIntegracoesAnexosPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Frete.ContratoTransporteFrete(unitOfWork).VerificarIntegracoesAnexosPendentes();

            Servicos.Log.GravarInfo("Finalizou 'VerificarIntegracoesAnexosPendentes' e iniciou 'IntegracaoTransbordo.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Integracao.IntegracaoTransbordo(unitOfWork).VerificarIntegracoesPendentes(_clienteMultisoftware, _auditado, _tipoServicoMultisoftware);

            //Servicos.Log.GravarInfo("Finalizou 'IntegracaoTransbordo.VerificarIntegracoesPendentes' e iniciou 'IntegracaoCargaEvento.VerificarIntegracoesPendentes'", "IntegracaoCargaThread");
            //new Servicos.Embarcador.Integracao.IntegracaoCargaEvento(unitOfWork, _tipoServicoMultisoftware).VerificarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'IntegracaoTransbordo.VerificarIntegracoesPendentes' e iniciou 'ProcessarSocilitacoesTokenPendentesDeGeracao'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Transportadores.SolicitacaoToken(unitOfWork).ProcessarSocilitacoesTokenPendentesDeGeracao(_clienteUrlAcesso);

            /*Servicos.Log.GravarInfo("Finalizou 'ProcessarSocilitacoesTokenPendentesDeGeracao' e iniciou 'IntegrarLoteLiberacaoComercialPedido'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Pedido.LoteLiberacaoComercialPedido(unitOfWork).IntegrarLoteLiberacaoComercialPedido();
            */
            Servicos.Log.GravarInfo("Finalizou 'IntegrarLoteLiberacaoComercialPedido' e iniciou 'VerificarGestaoDadosColetaIntegracoesPendentes'", "IntegracaoCargaThread");
            new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork).VerificarGestaoDadosColetaIntegracoesPendentes(_tipoServicoMultisoftware);

            Servicos.Log.GravarInfo("Finalizou 'VerificarGestaoDadosColetaIntegracoesPendentes' e iniciou 'ProcessarIntegracoesPendentesGestaoDevolucao'", "IntegracaoCargaThread");
            new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, _auditado, _clienteMultisoftware).ProcessarIntegracoesPendentes();

            Servicos.Log.GravarInfo("Finalizou 'ProcessarIntegracoesPendentesGestaoDevolucao'", "IntegracaoCargaThread");


            if (count == 0)//desta forma executa a integração com a angellira a cada 5 minutos.
                GerarIntegracoesFaixaTemperatura(unitOfWork);
            else
            {
                if (count > 60)
                    count = 0;
            }

            count++;
            GerarIntegracaoCarregamento(unitOfWork);
        }

        #region Métodos Privados

        private void GerarIntegracaoCarregamento(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                new Servicos.Embarcador.Integracao.IntegracaoCarregamento(unidadeDeTrabalho).VerificarIntegracoesPendentes();
                new Servicos.Embarcador.Integracao.IntegracaoLoteCarregamento(unidadeDeTrabalho).VerificarIntegracoesPendentes();
                Servicos.Embarcador.Integracao.IntegracaoPedido.ProcessarIntegracaoPedidoCancelamentoReserva(unidadeDeTrabalho);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }
        }

        private void GerarIntegracoesFaixaTemperatura(Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.IntegrarFaixaTemperatura(unidadeTrabalho);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

        }

        private void GerarIntegracoesTemperaturaAtual(Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                Servicos.Embarcador.Integracao.AngelLira.IntegrarCargaAngelLira.IntegrarUltimaTemperatura(unidadeTrabalho);
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
            }

        }

        private void GerarIntegracoesRoteirizacao(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracao.ExigirRotaRoteirizadaNaCarga)
            {
                Servicos.Embarcador.Carga.RotaFrete serRotaFrete = new Servicos.Embarcador.Carga.RotaFrete(unidadeTrabalho);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeTrabalho);
                List<Dominio.Entidades.RotaFrete> rotas = repRotaFrete.BuscaPendentesRoteirizacao(limite: 10, tentativas: configuracao.NumeroTentativasReenvioRotaFrete);
                foreach (Dominio.Entidades.RotaFrete rota in rotas)
                {
                    try
                    {
                        string erro = "";

                        if (rota.Remetente == null && (rota.Localidades == null || rota.LocalidadesOrigem.Count == 0))
                            erro = "Origem não informada";

                        if ((rota.Destinatarios == null || rota.Destinatarios.Count == 0) && (rota.Localidades == null || rota.Localidades.Count == 0))
                            erro = "Nenhum destino informado";

                        if (string.IsNullOrWhiteSpace(erro))
                        {
                            serRotaFrete.Roteirizar(out erro, rota, repRotaFrete, tipoServicoMultisoftware, configuracao);
                        }

                        if (!string.IsNullOrEmpty(erro))
                        {
                            rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro;
                            rota.TentativasIntegracao++;
                            rota.RotaRoteirizada = false;
                            rota.MotivoFalhaRoteirizacao = erro;
                            repRotaFrete.Atualizar(rota);
                            Servicos.Log.TratarErro($"Erro ao gerar rota: {rota.Codigo} - {erro} ");
                        }
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        rota.RotaRoteirizada = false;
                        rota.MotivoFalhaRoteirizacao = "Ocorreu uma falha ao gerar a roteirização.";
                        rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro;
                        rota.TentativasIntegracao++;
                        repRotaFrete.Atualizar(rota);

                    }
                }
            }

        }

        private void GerarIntegracoesLoteEscrituracao(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracao repLoteEscrituracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracao(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao repLoteEscrituracaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao repLoteEscrituracaoIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoIntegracao(unidadeTrabalho);

            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeTrabalho);
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracao serIntegracaoLoteEscrituracao = new Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracao(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao> lancamentosLoteEscrituracao = repLoteEscrituracao.BuscarLotesAgIntegracao(2);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            for (var i = 0; i < lancamentosLoteEscrituracao.Count; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracao lancamentoLoteEscrituracao = lancamentosLoteEscrituracao[i];

                bool integracaoFinalizada = true;

                serIntegracaoLoteEscrituracao.IniciarIntegracoesComDocumentos(lancamentoLoteEscrituracao, unidadeTrabalho);
                serIntegracaoLoteEscrituracao.IniciarIntegracoesComEDI(lancamentoLoteEscrituracao, unidadeTrabalho);

                if (!Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracao.VerificarSePossuiIntegracao(lancamentoLoteEscrituracao, unidadeTrabalho) || !lancamentoLoteEscrituracao.Integracoes.Any() || lancamentoLoteEscrituracao.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (repLoteEscrituracaoEDIIntegracao.ContarPorLoteEscrituracaoESituacaoDiff(lancamentoLoteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0 && repLoteEscrituracaoIntegracao.ContarPorLoteEscrituracaoESituacaoDiff(lancamentoLoteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                        lancamentoLoteEscrituracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracao.Finalizado;

                    integracaoFinalizada = true;
                }
                if (integracaoFinalizada)
                {
                    repLoteEscrituracao.Atualizar(lancamentoLoteEscrituracao);
                    //svcHubCarga.InformarCargaAtualizada(lancamentoLoteEscrituracao.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);
                }
            }
        }

        private void GerarIntegracoesLoteEscrituracaoCancelamento(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento repLoteEscrituracaoCancelamento = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamento(unitOfWork);
            Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao repLoteEscrituracaoCancelamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.LoteEscrituracaoCancelamentoEDIIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracaoCancelamento serIntegracaoLoteEscrituracaoCancelamento = new Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracaoCancelamento();

            List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento> lancamentosLoteEscrituracaoCancelamento = repLoteEscrituracaoCancelamento.BuscarLotesAgIntegracao(2);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            int countIntegracoesPendentes = lancamentosLoteEscrituracaoCancelamento.Count();

            for (var i = 0; i < countIntegracoesPendentes; i++)
            {
                Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoCancelamento lancamentoLoteEscrituracaoCancelamento = lancamentosLoteEscrituracaoCancelamento[i];

                bool integracaoFinalizada = true;

                serIntegracaoLoteEscrituracaoCancelamento.IniciarIntegracoesComEDI(lancamentoLoteEscrituracaoCancelamento, unitOfWork);

                if (!Servicos.Embarcador.Integracao.IntegracaoLoteEscrituracaoCancelamento.VerificarSePossuiIntegracao(lancamentoLoteEscrituracaoCancelamento, unitOfWork) || !lancamentoLoteEscrituracaoCancelamento.Integracoes.Any() || lancamentoLoteEscrituracaoCancelamento.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (repLoteEscrituracaoCancelamentoEDIIntegracao.ContarPorLoteEscrituracaoESituacaoDiff(lancamentoLoteEscrituracaoCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                        lancamentoLoteEscrituracaoCancelamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteEscrituracaoCancelamento.Finalizado;

                    integracaoFinalizada = true;
                }

                if (integracaoFinalizada)
                    repLoteEscrituracaoCancelamento.Atualizar(lancamentoLoteEscrituracaoCancelamento);
            }
        }

        private void GerarIntegracoesNFSManual(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unidadeTrabalho);

            Servicos.Embarcador.Integracao.IntegracaoNFSManual serIntegracaoNFSManual = new Servicos.Embarcador.Integracao.IntegracaoNFSManual(unidadeTrabalho, tipoServicoMultisoftware);
            Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lancamentosNFSManual = repLancamentoNFSManual.BuscarLancamentosAgIntegracao();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            for (var i = 0; i < lancamentosNFSManual.Count; i++)
            {
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = lancamentosNFSManual[i];

                bool integracaoFinalizada = true;

                serIntegracaoNFSManual.IniciarIntegracoesComDocumentos(lancamentoNFSManual);
                serIntegracaoNFSManual.IniciarIntegracoesComEDI(lancamentoNFSManual);

                if (!Servicos.Embarcador.Integracao.IntegracaoNFSManual.VerificarSePossuiIntegracao(lancamentoNFSManual, unidadeTrabalho) || !lancamentoNFSManual.Integracoes.Any() || lancamentoNFSManual.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (
                        (repNFSManualEDIIntegracao.ContarPorLancamentoNFSManualESituacaoDiff(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0) &&
                        (repNFSManualCTeIntegracao.ContarPorLancamentoNFSManualESituacaoDiff(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                    )
                    {
                        lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Finalizada;

                        lancamentoNFSManual = Servicos.Embarcador.Integracao.IntegracaoNFSManual.AtualizarNumeracaoNFSManualIntegracao(lancamentoNFSManual, unidadeTrabalho);
                    }

                    integracaoFinalizada = true;
                }
                else
                {
                    bool possuiIntegracaoAvon = false;
                    bool possuiIntegracaoAvior = false;
                    bool possuiIntegracaoNatura = false;
                    bool possuiIntegracaoEmail = false;
                    bool possuiIntegracaoFTP = false;

                    if (lancamentoNFSManual.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        possuiIntegracaoAvon = true;
                    if (lancamentoNFSManual.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                        possuiIntegracaoAvior = true;
                    if (lancamentoNFSManual.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email))
                        possuiIntegracaoEmail = true;
                    if (lancamentoNFSManual.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                        possuiIntegracaoFTP = true;
                    if (lancamentoNFSManual.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                        possuiIntegracaoNatura = true;

                    if (possuiIntegracaoAvon && repLancamentoNFSManual.PossuiCTeSemIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoAvior && possuiIntegracaoAvon)
                    {
                        //quando é integração com Avon e Avior o guigo fez a boba da gambiarra para enviar só um ct-e com o valor da carga para o Avior, daí nós se fode pra validar essas coisas né mano
                        if (!repLancamentoNFSManual.PossuiCTeComIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }
                    else if (integracaoFinalizada && possuiIntegracaoAvior)
                    {
                        if (repLancamentoNFSManual.PossuiCTeSemIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoNatura)
                    {
                        if (repLancamentoNFSManual.PossuiCTeSemIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoEmail)
                    {
                        if (repLancamentoNFSManual.PossuiCTeSemIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoFTP)
                    {
                        if (repLancamentoNFSManual.PossuiCTeSemIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                            integracaoFinalizada = false;
                    }
                }

                if (integracaoFinalizada)
                {
                    // Integracao com SignalR
                    svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamentoNFSManual.Codigo);

                    lancamentoNFSManual.GerandoIntegracoes = false;
                    repLancamentoNFSManual.Atualizar(lancamentoNFSManual);
                }
            }
        }

        private void GerarIntegracoesNFSManualCancelamento(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeTrabalho);
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeTrabalho);

            Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento serIntegracaoNFSManual = new Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento();

            List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> nfsManualCancelamentos = repNFSManualCancelamento.BuscarLancamentosAgIntegracao();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            for (var i = 0; i < nfsManualCancelamentos.Count; i++)
            {
                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento = nfsManualCancelamentos[i];

                bool integracaoFinalizada = true;

                serIntegracaoNFSManual.IniciarIntegracoesComDocumentos(nfsManualCancelamento, unidadeTrabalho);
                serIntegracaoNFSManual.IniciarIntegracoesComEDI(nfsManualCancelamento, unidadeTrabalho, tipoServicoMultisoftware);

                if (!Servicos.Embarcador.Integracao.IntegracaoNFSManualCancelamento.VerificarSePossuiIntegracao(nfsManualCancelamento, unidadeTrabalho) || !nfsManualCancelamento.Integracoes.Any() || nfsManualCancelamento.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (repNFSManualCancelamentoIntegracaoEDI.ContarPorNFSManualCancelamento(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                        nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.Cancelada;

                    integracaoFinalizada = true;
                }
                else
                {
                    bool possuiIntegracaoAvon = false;
                    bool possuiIntegracaoAvior = false;
                    bool possuiIntegracaoNatura = false;
                    bool possuiIntegracaoEmail = false;
                    bool possuiIntegracaoFTP = false;

                    if (nfsManualCancelamento.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        possuiIntegracaoAvon = true;
                    if (nfsManualCancelamento.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                        possuiIntegracaoAvior = true;
                    if (nfsManualCancelamento.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email))
                        possuiIntegracaoEmail = true;
                    if (nfsManualCancelamento.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                        possuiIntegracaoFTP = true;
                    if (nfsManualCancelamento.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                        possuiIntegracaoNatura = true;

                    if (possuiIntegracaoAvon && repNFSManualCancelamento.PossuiCTeSemIntegracao(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoAvior && possuiIntegracaoAvon)
                    {
                        //quando é integração com Avon e Avior o guigo fez a boba da gambiarra para enviar só um ct-e com o valor da carga para o Avior, daí nós se fode pra validar essas coisas né mano
                        if (!repNFSManualCancelamento.PossuiCTeComIntegracao(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }
                    else if (integracaoFinalizada && possuiIntegracaoAvior)
                    {
                        if (repNFSManualCancelamento.PossuiCTeSemIntegracao(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoNatura)
                    {
                        if (repNFSManualCancelamento.PossuiCTeSemIntegracao(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoEmail)
                    {
                        if (repNFSManualCancelamento.PossuiCTeSemIntegracao(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoFTP)
                    {
                        if (repNFSManualCancelamento.PossuiCTeSemIntegracao(nfsManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                            integracaoFinalizada = false;
                    }
                }

                if (integracaoFinalizada)
                {
                    nfsManualCancelamento.GerandoIntegracoes = false;

                    repNFSManualCancelamento.Atualizar(nfsManualCancelamento);
                }
            }
        }

        private async Task GerarIntegracoesCargaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaEDIIntegracao repositorioCargaEDIIntegracao = new Repositorio.Embarcador.Cargas.CargaEDIIntegracao(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork, cancellationToken);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork, cancellationToken);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
            Servicos.Embarcador.Integracao.IntegracaoCarga servicoCargaIntegracao = new Servicos.Embarcador.Integracao.IntegracaoCarga(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioCarga.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao, true, 10);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao
            };

            for (var i = 0; i < cargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[i];

                bool integracaoFinalizada = true;

                servicoCargaIntegracao.IniciarIntegracoesComDocumentos(carga, unitOfWork, _tipoServicoMultisoftware);
                servicoCargaIntegracao.IniciarIntegracoesComEDI(carga, _tipoServicoMultisoftware, unitOfWork);

                bool integracaoFilialEmissora = false;
                if (carga.EmpresaFilialEmissora != null && !carga.LiberadaParaEmissaoCTeSubContratacaoFilialEmissora)
                    integracaoFilialEmissora = true;

                if (!await servicoCargaIntegracao.VerificarSePossuiIntegracaoAsync(carga, integracaoFilialEmissora, unitOfWork) || (carga.CargaTransbordo && !carga.Integracoes.Any(o => o.TipoIntegracao.IntegrarCargaTransbordo)) || !carga.Integracoes.Any() || carga.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    if (await repositorioCargaEDIIntegracao.ContarPorCargaESituacaoDiffAsync(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado, integracaoFilialEmissora) == 0)
                    {
                        if (!integracaoFilialEmissora)
                        {
                            if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAMDFe))
                            {
                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                                carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                                Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unitOfWork);
                            }
                            else
                            {
                                carga.SituacaoCarga = configuracao.SituacaoCargaAposIntegracao;
                                if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos && (carga.TipoOperacao?.NaoNecessarioConfirmarImpressaoDocumentos ?? false))
                                {
                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte;
                                    carga.DataMudouSituacaoParaEmTransporte = DateTime.Now;
                                    Servicos.Auditoria.Auditoria.Auditar(_auditado, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte.ObterDescricao()}", unitOfWork);
                                }
                            }

                        }
                        else
                            Servicos.Embarcador.Carga.Documentos.LiberarEmissaoFilialEmissora(carga, unitOfWork);
                    }

                    integracaoFinalizada = true;

                    if (servicoCarga.PermitirFinalizarCargaAutomaticamente(carga, configuracao, _tipoServicoMultisoftware, repositorioCargaMDFe) && !integracaoFilialEmissora && (carga.Filial == null || !carga.Filial.EmitirMDFeManualmente))
                        servicoCarga.ValidarCargasFinalizadas(ref carga, _tipoServicoMultisoftware, null, unitOfWork);

                    if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                    {
                        new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(carga, TipoNotificacaoApp.MotoristaPodeSeguirViagem);
                        if (configuracao.QuandoIniciarMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarMonitoramento.AoInformarVeiculoNaCargaECargaEmTransporte)
                            await Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciarAsync(carga, configuracao, null, "Carga em transporte", unitOfWork);
                    }

                }
                else if (!carga.CargaTransbordo)
                {
                    bool possuiIntegracaoAvon = false;
                    bool possuiIntegracaoAvior = false;
                    bool possuiIntegracaoNatura = false;
                    bool possuiIntegracaoFTP = false;
                    bool possuiIntegracaoUnilever = false;
                    bool possuiIntegracaoGPAEscrituracaoCTe = false;

                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        possuiIntegracaoAvon = true;

                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                        possuiIntegracaoAvior = true;

                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                        possuiIntegracaoFTP = true;

                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                        possuiIntegracaoNatura = true;

                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe))
                        possuiIntegracaoGPAEscrituracaoCTe = true;

                    if (carga.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                        possuiIntegracaoUnilever = true;

                    if (possuiIntegracaoAvon && repositorioCargaCTe.PossuiCTeSemIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        integracaoFinalizada = false;

                    //carga de pre-chekin precisa que os ctes que não estao com recusa aprovada tenham integracao
                    if (possuiIntegracaoUnilever && carga.DadosSumarizados.CargaTrecho == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada.Agrupadora && repositorioCargaCTe.PossuiCTeSemIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever, true))
                        integracaoFinalizada = false;

                    if (possuiIntegracaoUnilever && carga.DadosSumarizados.CargaTrecho != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada.Agrupadora && repositorioCargaCTe.PossuiCTeSemIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoAvior && possuiIntegracaoAvon)
                    {
                        //quando é integração com Avon e Avior o guigo fez a boba da gambiarra para enviar só um ct-e com o valor da carga para o Avior, daí nós se fode pra validar essas coisas né mano
                        if (!repositorioCargaCTe.PossuiCTeComIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }
                    else if (integracaoFinalizada && possuiIntegracaoAvior && repositorioCargaCTe.PossuiCTeSemIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoNatura && repositorioCargaCTe.PossuiCTeSemIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoFTP && repositorioCargaCTe.PossuiCTeSemIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoGPAEscrituracaoCTe && repositorioCargaCTe.PossuiDocumentoSemIntegracao(carga.Codigo, Dominio.Enumeradores.TipoDocumento.CTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.GPAEscrituracaoCTe))
                        integracaoFinalizada = false;
                }

                if (integracaoFinalizada)
                {
                    carga.GerandoIntegracoes = false;
                    await repositorioCarga.AtualizarAsync(carga);
                    servicoHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unitOfWork.StringConexao);
                }
            }
        }

        private void GerarIntegracoesOcorrencia(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Servicos.Embarcador.Hubs.Ocorrencia svcHubOcorrencia = new Servicos.Embarcador.Hubs.Ocorrencia();
            Servicos.Embarcador.Integracao.IntegracaoOcorrencia serOcorrenciaIntegracao = new Servicos.Embarcador.Integracao.IntegracaoOcorrencia(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repOcorrencia.BuscarOcorrenciasPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao, true, 10);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoPassarReto = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech
            };

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
            {
                bool integracaoFinalizada = true;

                serOcorrenciaIntegracao.IniciarIntegracoesComDocumentos(ocorrencia, unidadeTrabalho);

                if (!Servicos.Embarcador.Integracao.IntegracaoOcorrencia.VerificarSePossuiIntegracao(ocorrencia, unidadeTrabalho))
                {
                    Servicos.Embarcador.Integracao.IntegracaoOcorrencia.IntegracaoFinalizada(ocorrencia, unidadeTrabalho, tipoServicoMultisoftware);

                    integracaoFinalizada = true;
                }
                else if (!ocorrencia.Integracoes.Any() || ocorrencia.Integracoes.All(o => tiposIntegracaoPassarReto.Contains(o.TipoIntegracao.Tipo)))
                {
                    Servicos.Embarcador.Integracao.IntegracaoOcorrencia.IntegracaoFinalizada(ocorrencia, unidadeTrabalho, tipoServicoMultisoftware);

                    integracaoFinalizada = true;
                }
                else
                {
                    bool possuiIntegracaoAvon = false;
                    bool possuiIntegracaoAvior = false;
                    bool possuiIntegracaoNatura = false;
                    bool possuiIntegracaoEmail = false;
                    bool possuiIntegracaoFTP = false;
                    bool possuiIntegracaoMarfrig = false;
                    bool possuiIntegracaoAX = false;
                    bool possuiIntegracaoUnilever = false;
                    bool possuiIntegracaoUnileverOcorrencia = false;
                    bool possuiIntegracaoUnileverRecusa = false;
                    bool possuiIntegracaoUnileverInfrutifera = false;

                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        possuiIntegracaoAvon = true;
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                        possuiIntegracaoAvior = true;
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email))
                        possuiIntegracaoEmail = true;
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                        possuiIntegracaoFTP = true;
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                        possuiIntegracaoNatura = true;
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig))
                        possuiIntegracaoMarfrig = true;
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX))
                    {
                        possuiIntegracaoAX = true;
                        possuiIntegracaoNatura = false;
                    }
                    if (ocorrencia.Integracoes.Any(o => o.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                        possuiIntegracaoUnilever = true;

                    if (possuiIntegracaoAvon && repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avon))
                        integracaoFinalizada = false;

                    if (integracaoFinalizada && possuiIntegracaoAvior && possuiIntegracaoAvon)
                    {
                        //quando é integração com Avon e Avior o guigo fez a boba da gambiarra para enviar só um ct-e com o valor da ocorrencia para o Avior, daí nós se fode pra validar essas coisas né mano
                        if (!repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }
                    else if (integracaoFinalizada && possuiIntegracaoAvior)
                    {
                        if (repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Avior))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoNatura)
                    {
                        if (repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoEmail)
                    {
                        if (repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Email))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoFTP)
                    {
                        if (repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.FTP))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoAX)
                    {
                        if (repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.AX))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoMarfrig)
                    {
                        //if (repCargaCTe.PossuiCTeSemIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig))
                        //Marfrig gera uma integração para todos CTes
                        if (!repCargaCTe.PossuiCTeComIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoUnilever)
                    {
                        if (repCargaCTe.PossuiCTeComIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoUnileverInfrutifera)
                    {
                        if (repCargaCTe.PossuiCTeComIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverInfrutifera))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoUnileverOcorrencia)
                    {
                        if (repCargaCTe.PossuiCTeComIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverOcorrencias))
                            integracaoFinalizada = false;
                    }

                    if (integracaoFinalizada && possuiIntegracaoUnileverRecusa)
                    {
                        if (repCargaCTe.PossuiCTeComIntegracaoOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.UnileverRecusa))
                            integracaoFinalizada = false;
                    }
                }

                if (integracaoFinalizada)
                {
                    Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaProvisao(ocorrencia, configuracao, tipoServicoMultisoftware, unidadeTrabalho);
                    ocorrencia.GerandoIntegracoes = false;
                    repOcorrencia.Atualizar(ocorrencia);
                    svcHubOcorrencia.InformarOcorrenciaAtualizada(ocorrencia.Codigo);
                }
            }
        }

        private async Task GerarIntegracoesPagamentoAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork, cancellationToken);
            Servicos.Embarcador.Integracao.IntegracaoPagamento servicoIntegracaoPagamento = new Servicos.Embarcador.Integracao.IntegracaoPagamento(unitOfWork, _tipoServicoMultisoftware, cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentos = await repositorioPagamentoIntegracao.BuscarAgIntegracaoAsync(cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentos)
            {
                await servicoIntegracaoPagamento.IniciarIntegracoesComDocumentosAsync(pagamento, clienteURLAcesso);
            }

            await servicoIntegracaoPagamento.VerificarPagamentosIntegradosAsync();
        }

        private void GerarIntegracoesCancelamentoPagamento(Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repCancelamentoPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> cancelamentos = repCancelamentoPagamentoIntegracao.BuscarAgIntegracao();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento cancelamento in cancelamentos)
            {
                Servicos.Embarcador.Integracao.IntegracaoCancelamentoPagamento.IniciarIntegracoesComDocumentos(cancelamento, unidadeTrabalho);
            }

            Servicos.Embarcador.Integracao.IntegracaoCancelamentoPagamento.VerificarProvisoesIntegradas(unidadeTrabalho);
        }

        private async Task GerarIntegracoesCancelamentoProvisaoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao> provisoesCancelamento = await repositorioCancelamentoProvisaoIntegracao.BuscarAgIntegracaoAsync(cancellationToken);

            Servicos.Embarcador.Integracao.IntegracaoCancelamentoProvisao servicoIntegracaoCancelamentoProvisao = new Servicos.Embarcador.Integracao.IntegracaoCancelamentoProvisao(unitOfWork, _tipoServicoMultisoftware, cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao provisaoCancelamento in provisoesCancelamento)
                await servicoIntegracaoCancelamentoProvisao.IniciarIntegracoesComDocumentosAsync(provisaoCancelamento);

            Servicos.Embarcador.Integracao.IntegracaoCancelamentoProvisao.VerificarCancelamentoProvisoesIntegradas(unitOfWork);
        }

        private void GerarIntegracoesCargaCTeAgrupado(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(unidadeTrabalho);

            Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado serIntegracaoCargaCTeAgrupado = new Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado(unidadeTrabalho, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado> cargasCTeAgrupado = repCargaCTeAgrupado.BuscarCargaCTeAgrupadoAgIntegracao(true);

            for (var i = 0; i < cargasCTeAgrupado.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = cargasCTeAgrupado[i];

                bool integracaoFinalizada = true;

                serIntegracaoCargaCTeAgrupado.IniciarIntegracoesComDocumentos(cargaCTeAgrupado);

                if (!Servicos.Embarcador.Integracao.IntegracaoCargaCTeAgrupado.VerificarSePossuiIntegracao(cargaCTeAgrupado, unidadeTrabalho) || !cargaCTeAgrupado.Integracoes.Any())
                {
                    if (repCargaCTeAgrupadoIntegracao.ContarPorCargaCTeAgrupadoESituacaoDiff(cargaCTeAgrupado.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                    {
                        cargaCTeAgrupado.Situacao = SituacaoCargaCTeAgrupado.Finalizado;
                    }

                    integracaoFinalizada = true;
                }


                if (integracaoFinalizada)
                {
                    cargaCTeAgrupado.GerandoIntegracoes = false;
                    repCargaCTeAgrupado.Atualizar(cargaCTeAgrupado);
                }
            }
        }

        private void GerarIntegracoesCargaMDFeManual(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao repCargaMDFeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualIntegracao(unidadeTrabalho);

            Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManual serIntegracaoCargaMDFeManual = new Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManual(unidadeTrabalho, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> cargasMDFeManual = repCargaMDFeManual.BuscarCargaMDFeManualAgIntegracao(true);

            for (var i = 0; i < cargasMDFeManual.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = cargasMDFeManual[i];

                bool integracaoFinalizada = true;

                serIntegracaoCargaMDFeManual.IniciarIntegracoesComDocumentos(cargaMDFeManual);

                if (!Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManual.VerificarSePossuiIntegracao(cargaMDFeManual, unidadeTrabalho) || !cargaMDFeManual.Integracoes.Any())
                {
                    if (repCargaMDFeManualIntegracao.ContarPorCargaMDFeManualESituacaoDiff(cargaMDFeManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                    {
                        cargaMDFeManual.Situacao = SituacaoMDFeManual.Finalizado;
                    }

                    integracaoFinalizada = true;
                }


                if (integracaoFinalizada)
                {
                    cargaMDFeManual.GerandoIntegracoes = false;
                    repCargaMDFeManual.Atualizar(cargaMDFeManual);
                }
            }
        }
        private void GerarIntegracoesCargaMDFeManualCancelamento(Repositorio.UnitOfWork unidadeTrabalho, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

            Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManualCancelamento serIntegracaoCargaMDFeManualCancelamento = new Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManualCancelamento(unidadeTrabalho, tipoServicoMultisoftware);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> cargasMDFeManualCancelamento = repCargaMDFeManualCancelamento.BuscarCargaMDFeManualCancelamentoAgIntegracao(true);

            for (var i = 0; i < cargasMDFeManualCancelamento.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento = cargasMDFeManualCancelamento[i];

                bool integracaoFinalizada = true;

                serIntegracaoCargaMDFeManualCancelamento.IniciarIntegracoesComDocumentos(cargaMDFeManualCancelamento);

                if (!Servicos.Embarcador.Integracao.IntegracaoCargaMDFeManualCancelamento.VerificarSePossuiIntegracao(cargaMDFeManualCancelamento, unidadeTrabalho) || !cargaMDFeManualCancelamento.Integracoes.Any())
                {
                    if (repCargaMDFeManualCancelamentoIntegracao.ContarPorCargaMDFeManualCancelamentoESituacaoDiff(cargaMDFeManualCancelamento.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado) == 0)
                    {
                        cargaMDFeManualCancelamento.CargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Cancelado;
                        cargaMDFeManualCancelamento.CargaMDFeManual.SituacaoCancelamento = cargaMDFeManualCancelamento.CargaMDFeManual.Situacao;
                        cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.Cancelada;

                        repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);
                        repCargaMDFeManual.Atualizar(cargaMDFeManualCancelamento.CargaMDFeManual);
                    }

                    integracaoFinalizada = true;
                }


                if (integracaoFinalizada)
                {
                    cargaMDFeManualCancelamento.GerandoIntegracoes = false;
                    repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);
                }
            }
        }
        #endregion Métodos Privados
    }
}