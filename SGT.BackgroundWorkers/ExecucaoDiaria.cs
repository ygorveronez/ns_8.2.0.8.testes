using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 86400000)]

    public class ExecucaoDiaria : LongRunningProcessBase<ExecucaoDiaria>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            var horarioExecucao = ObterHorarioExecucao(unitOfWork);
            Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Transportadores.Motorista servicoMotorista = new Servicos.Embarcador.Transportadores.Motorista(unitOfWork);
            Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, _auditado);

            DateTime dataBaseProximaExcecucao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, horarioExecucao.Hours, horarioExecucao.Minutes, horarioExecucao.Seconds);
            DateTime dataProximaExecucao = dataBaseProximaExcecucao;

            if ((dataProximaExecucao - DateTime.Now).TotalMilliseconds < 0)
                dataProximaExecucao = dataProximaExecucao.AddDays(1);

            DateTime dataMinimaExcecucao = dataBaseProximaExcecucao.AddMinutes(-5);
            DateTime dataMaximaExcecucao = dataBaseProximaExcecucao.AddMinutes(25);
            bool dataAtualDentroIntervaloExecucao = (DateTime.Now >= dataMinimaExcecucao) && (DateTime.Now <= dataMaximaExcecucao);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            if (dataAtualDentroIntervaloExecucao)
            {
                serCTe.IntegrarCTeAnteriorEMPAsync(unitOfWork);//var integrarCTeAnterior = new Servicos.Embarcador.CTe.CTe(_stringConexao).IntegrarCTeAnteriorEMPAsync(unitOfWork);
                serCTe.IntegrarCTesFaturasDiaAnteriorEMPAsync(unitOfWork);
                serCarga.IntegrarCargasDiaAnteriorEMPAsync(unitOfWork);
                Servicos.Embarcador.CIOT.CIOT.ConciliarCIOTs(_tipoServicoMultisoftware, unitOfWork);
                Servicos.Empresa.EnviarCertificados(unitOfWork);
                NotificarContratosTransportadorVencendo(unitOfWork);
                NotificarDespachanteMercante(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware.RazaoSocial);
                Servicos.Embarcador.NFSe.NFSAutomatica.GerarNFSAutomatica(_tipoServicoMultisoftware, unitOfWork, _auditado);
                servicoMotorista.NotificarVencimentoLicencasMobile(_tipoServicoMultisoftware);
                servicoPallet.NotificarPalletsPendentes();

                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.Log.TratarErro("Iniciou envio e-mail transportadores encerramento mdf-e");

                    if (configuracaoEmbarcador.EnviarEmailEncerramentoMDFeTransportador)
                        Servicos.Embarcador.MDFe.Encerramento.EnviarEmailsEncerramentoTransportadores(_clienteMultisoftware.Codigo, unitOfWork, unitOfWorkAdmin, _tipoServicoMultisoftware);

                    Servicos.Log.TratarErro("finalizou envio e-mail transportadores encerramento mdf-e");
                }
            }

            Servicos.Embarcador.Frete.ContratoFreteTransportador.ValidarContratosVencidos(unitOfWork);
            ObterFaturasTransportadores(unitOfWork);
            NotificarDiariasContainer(unitOfWork, _tipoServicoMultisoftware);
            NotificarCanhotosPendentes(unitOfWork, _clienteMultisoftware.RazaoSocial);
            NotificarTrasnportadorCanhotosRejeitados(unitOfWork);
            NotificarVencimentoApolicesSeguro(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware.RazaoSocial);
            NotificarVencimentoCertificadoDigital(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware.RazaoSocial);
            NotificarVencimentoAntt(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware.RazaoSocial);
            NotificarVencimentoCnh(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware.RazaoSocial);
            NotificarVencimentoRegraICMS(unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware.RazaoSocial);
            DesabiltarVeiculosSemUtilizacao(unitOfWork, _tipoServicoMultisoftware);
            EncerramentoMDFeAutomatico(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            VerificarDadosCadastraisTransportadores(unitOfWork, _auditado, _tipoServicoMultisoftware);
            NotificarCanhotosPendentesPorTipoOperacao(unitOfWork);
            NotificarContratoNotaFiscalPendenteAceiteTransportadores(unitOfWork, unitOfWorkAdmin, _tipoServicoMultisoftware);
            ValidarLicencasVeiculoVencidas(unitOfWork, _tipoServicoMultisoftware);
            GerarCargaImportacaoProgramacaoColeta(configuracaoEmbarcador, unitOfWork, _auditado, _tipoServicoMultisoftware);
            GerarLiquidacaoPalletAutomaticamente(configuracaoEmbarcador, unitOfWork, _auditado, _tipoServicoMultisoftware);
            VerificarFilasCarregamentoVeiculoAlterarDataProgramada(unitOfWork, _tipoServicoMultisoftware);
            GerarPlanejamentoDeFrotaAutomatizado(unitOfWork);
            EnviarEmailTransportadorComEntregaEmAtraso(unitOfWork);
            InativarUsuariosAposXDiasSemAcesso(unitOfWork, _tipoServicoMultisoftware);
            EnviarEmailAprovadoresTarifasPendentes(unitOfWork);
            DefinirNaoComparecimentoJanelaDescarregamento(unitOfWork, _clienteMultisoftware, configuracaoEmbarcador);
            EnviarEmailResumoNCPendentes(unitOfWork);
            EnviarEmailNFSePendente(unitOfWork);
            NotificarTransportadorDevolucaoPendente(unitOfWork, _auditado, _clienteMultisoftware);
            AlterarDevolucoesAutomaticamenteAoEsgotarTempo(unitOfWork, _auditado, _clienteMultisoftware);
            FinalizarSituacaoAbertaVeiculos(configuracaoEmbarcador, configuracaoVeiculo, unitOfWork, _tipoServicoMultisoftware);
            BuscarValorCotacaoMoedaDiaria(configuracaoEmbarcador, unitOfWork);

            if (dataAtualDentroIntervaloExecucao)
                IntegrarOrdensEmbarquePendentes(unitOfWork, _codigoEmpresa);

            _tempoAguardarProximaExecucao = (int)(dataProximaExecucao - DateTime.Now).TotalMilliseconds;
        }

        #region Métodos Privados

        private void IntegrarOrdensEmbarquePendentes(Repositorio.UnitOfWork unitOfWork, int codigoEmpresa)
        {
            Servicos.Log.TratarErro($"Integração de OE Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Empresa: {_codigoEmpresa}");

            Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao repositorioCargaOrdemEmbarqueIntegracao = new Repositorio.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao(unitOfWork);
            Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig servicoIntegracaoOrdemEmbarqueMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork);
            List<int> codigosOrdemEmbarqueAguardandoIntegracao = repositorioCargaOrdemEmbarqueIntegracao.BuscarCodigosOrdemEmbarquePendentes();

            foreach (int codigoOrdemEmbarque in codigosOrdemEmbarqueAguardandoIntegracao)
            {
                unitOfWork.FlushAndClear();
                unitOfWork.Start();

                Servicos.Log.TratarErro($"Integração da OE {codigoOrdemEmbarque} Iniciada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Empresa: {codigoEmpresa}");

                Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.CargaOrdemEmbarqueIntegracao cargaOrdemEmbarqueIntegracao = repositorioCargaOrdemEmbarqueIntegracao.BuscarPorCodigo(codigoOrdemEmbarque, auditavel: false);
                servicoIntegracaoOrdemEmbarqueMarfrig.IntegrarCargaOrdemEmbarque(cargaOrdemEmbarqueIntegracao);

                unitOfWork.CommitChanges();

                Servicos.Log.TratarErro($"Integração da OE {codigoOrdemEmbarque} Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Empresa: {codigoEmpresa}");
            }

            Servicos.Log.TratarErro($"Integração de OE Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} | Empresa: {_codigoEmpresa}");
        }

        private void NotificarCanhotosPendentes(Repositorio.UnitOfWork unitOfWork, string razaoSocial)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

                if (config.NotificarCanhotosPendentes)
                {
                    Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread controle = ObterControleNotificacaoThread(unitOfWork);

                    int dowNow = ((int)DateTime.Now.DayOfWeek) + 1;
                    int dowConfig = (int)config.DiaSemanaNotificarCanhotosPendentes;

                    if (controle.DataUltimoProcessamento != DateTime.Today && (dowNow == dowConfig || configuracaoCanhoto.NotificarCanhotosPendentesTodosOsDias))
                    {
                        Servicos.Embarcador.Canhotos.Canhoto.NotificarTransportadoresDeCanhotosPendentes(unitOfWork, false, null, razaoSocial);
                    }
                }

                if (configuracaoCanhoto.NotificarTransportadorCanhotosQueEstaoComDigitalizacaoRejeitada)
                {
                    //Eu sei que o nome da configuração está como Digitalização Rejeitada somente, porém.
                    //Após algumas alterações com o analista, ela é usada pra pegar Canhotos com Digitalização Rejeitada ou Pendente, e Recebimento Fisíco do Canhoto pendente.

                    Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfiguracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                    Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfiguracaoEmail.BuscarEmailEnviaDocumentoAtivo();

                    Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repositorioCanhoto.BuscarCanhotosComDigitalizacaoRejeitadaEAguardandoRecebimentoFisicoERecebimentoFisicoPendente();

                    List<Dominio.Entidades.Empresa> transportadores = canhotos.Select(o => o.Empresa).Distinct().ToList();

                    foreach (var transportador in transportadores)
                    {
                        System.Text.StringBuilder mensagemEmail = new System.Text.StringBuilder();

                        string assuntoEmail = "Canhoto Pendente";

                        List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosTransportador = canhotos.Where(o => o.Empresa?.Codigo == transportador.Codigo).ToList();

                        mensagemEmail.AppendLine($"{transportador.Descricao}, por meio deste informamos que os canhotos relacionados no arquivo anexo estão pendentes de digitalização e/ou tiveram sua digitalização rejeitada.");
                        mensagemEmail.AppendLine("Portanto necessitam de vossa atenção para envio de imagem e/ou revisão da mesma. Favor atentar para o prazo de envio de canhotos convencionado com a Unilever.");

                        List<Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado> planilhaEmailCanhotoRejeitado = new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado>();

                        foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosTransportador)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado planilhaCriacao = new Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado()
                            {
                                Numero = canhoto.Numero,
                                DataEmissao = canhoto.XMLNotaFiscal?.DataEmissao,
                                NumeroNotaFiscal = canhoto.XMLNotaFiscal?.Numero,
                                ChaveAcessoNotaFiscal = canhoto.XMLNotaFiscal?.Chave,
                                Destinatario = canhoto.Destinatario.Descricao,
                                NumeroCarga = canhoto.Carga?.CodigoCargaEmbarcador,
                                SituacaoDigitalizacaoCanhoto = canhoto.DescricaoDigitalizacao,
                                SituacaoRecebimentoCanhoto = canhoto.DescricaoSituacao,
                                DiferencaDiasDesdeEmissaoNotaFiscal = (DateTime.UtcNow - canhoto.XMLNotaFiscal.DataEmissao).Days
                            };

                            planilhaEmailCanhotoRejeitado.Add(planilhaCriacao);
                        }


                        planilhaEmailCanhotoRejeitado = planilhaEmailCanhotoRejeitado.OrderBy(o => o.Numero).ToList();

                        byte[] byteArrayObjeto = Utilidades.CSV.GerarCSV(planilhaEmailCanhotoRejeitado);

                        System.IO.MemoryStream mscsv = new System.IO.MemoryStream(byteArrayObjeto);

                        List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();
                        anexos.Add(new System.Net.Mail.Attachment(mscsv, "CanhotosPendentes.CSV"));

                        string corpoEmail = mensagemEmail.ToString();

                        Servicos.Email.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, transportador.EmailEnvioCanhoto, null, null, assuntoEmail, corpoEmail, configuracaoEmail.Smtp, out string mensagemErro, configuracaoEmail.DisplayEmail, anexos, "", configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, unitOfWork);
                    }

                    unitOfWork.FlushAndClear();
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarCanhotosPendentesPorTipoOperacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            DateTime dataInicial = new DateTime(2020, 5, 1);
            DateTime dataFinal = DateTime.Now.Date.AddDays(-1);
            IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> canhotos = repositorioCanhoto.ConsultaCanhotosNotificacaoPorTipoOperacao(dataInicial, dataFinal);
            List<int> codigosTipoOperacao = canhotos.Select(c => c.CodigoTipoOperacao).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposOperacao = codigosTipoOperacao.Count > 0 ? repositorioTipoOperacao.BuscarPorCodigos(codigosTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread controle = ObterControleNotificacaoThread(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao in tiposOperacao)
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaDaSemana = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)(((int)DateTime.Now.DayOfWeek) + 1);

                if (!tipoOperacao.ConfiguracaoCanhoto.NotificarCanhotosPendentesDiariamente && tipoOperacao.ConfiguracaoCanhoto.DiaSemanaNotificarCanhotosPendentes != diaDaSemana)
                    continue;

                if (controle.DataUltimoProcessamento == DateTime.Today)
                    continue;

                List<string> razoesSociaisNotificar = canhotos.Where(c => c.CodigoTipoOperacao == tipoOperacao.Codigo).Select(c => c.DescricaoTransportador).Distinct().ToList();

                foreach (string razaoSocial in razoesSociaisNotificar)
                    Servicos.Embarcador.Canhotos.Canhoto.NotificarTransportadoresDeCanhotosPendentes(unitOfWork, true, tipoOperacao, razaoSocial);
            }
        }

        private Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread ObterControleNotificacaoThread(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.ControleNotificacaoThread repositorioControleNotificacaoThread = new Repositorio.Embarcador.Canhotos.ControleNotificacaoThread(unitOfWork);
            Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread controle = repositorioControleNotificacaoThread.BuscarPadrao();

            if (controle == null)
            {
                controle = new Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread()
                {
                    DataUltimoProcessamento = DateTime.Today.AddDays(-1)
                };

                repositorioControleNotificacaoThread.Inserir(controle);
            }

            return controle;
        }

        private void NotificarDiariasContainer(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                {
                    Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);

                    IList<Dominio.ObjetosDeValor.Embarcador.Pedido.ColetaContainerRetornoCalculoDiaria> ColetasContainerDiariasSumarizadas = servicoColetaContainer.ObterContainersComDiariaExcedidaNotificacao();
                    if (ColetasContainerDiariasSumarizadas?.Count > 0)
                        servicoColetaContainer.NotificarDiariasContainer(ColetasContainerDiariasSumarizadas);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }

        }

        private void NotificarContratosTransportadorVencendo(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Frete.ContratoFreteTransportador.VerificarContratosVencendo(unitOfWork);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void NotificarVencimentoAntt(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            try
            {
                Servicos.Empresa.AlertarVencimentoAntt(unitOfWork, tipoServicoMultisoftware, razaoSocial);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarVencimentoApolicesSeguro(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            try
            {
                Servicos.Embarcador.Seguro.Seguro.AlertarVencimento(unitOfWork, tipoServicoMultisoftware, razaoSocial);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarVencimentoCertificadoDigital(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            try
            {
                Servicos.Empresa.AlertarVencimentoCertificadoDigital(unitOfWork, tipoServicoMultisoftware, razaoSocial);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarVencimentoCnh(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            try
            {
                Servicos.Embarcador.Transportadores.Motorista.AlertarVencimentoCnh(unitOfWork, tipoServicoMultisoftware, razaoSocial);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarDespachanteMercante(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            try
            {
                Servicos.Embarcador.Transportadores.Mercante.NotificarDespachanteMercante(unitOfWork, tipoServicoMultisoftware, razaoSocial);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void DesabiltarVeiculosSemUtilizacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Veiculo.Veiculo.InativarVeiculosSemUtilizacao(unitOfWork, tipoServicoMultisoftware, _auditado);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void FinalizarSituacaoAbertaVeiculos(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                if (!configuracao.NaoControlarSituacaoVeiculoOrdemServico)
                    Servicos.Embarcador.Veiculo.Veiculo.FinalizarSituacaoAbertaVeiculos(unitOfWork, tipoServicoMultisoftware, _auditado);

                if (configuracaoVeiculo.AtualizarHistoricoSituacaoVeiculo)
                    Servicos.Embarcador.Veiculo.Veiculo.AtualizarHistoricoSituacaoVeiculo(unitOfWork, tipoServicoMultisoftware, _auditado);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void EncerramentoMDFeAutomatico(Repositorio.UnitOfWork unitOfWork, string stringConexao, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe repConfiguracaoEncerramentoAutomatico = new Repositorio.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoEncerramentoAutomaticoMDFe config = repConfiguracaoEncerramentoAutomatico.BuscarConfiguracaoPadrao();

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador && config != null && config.DiasEncerramentoAutomaticoMDFE > 0)
            {
                Servicos.Log.TratarErro("Iniciando encerramento automatico MDFe", "ExecucaoDiariaEncerramentoMDFeAutomatico");
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);
                Servicos.AverbacaoMDFe svcAverbacao = new Servicos.AverbacaoMDFe(unitOfWork);


                Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa Empresa = repEmpresa.BuscarEmpresaPai();

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.BuscarPorStatusEDataAutorizacao(Dominio.Enumeradores.StatusMDFe.Autorizado, DateTime.Now.AddDays(-config.DiasEncerramentoAutomaticoMDFE), 0, Empresa.TipoAmbiente);
                Servicos.Log.TratarErro(mdfes.Count().ToString() + " pendentes de encerramento.", "ExecucaoDiariaEncerramentoMDFeAutomatico");

                foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfes)
                {

                    if (mdfe.Empresa.DataFinalCertificado >= DateTime.Today)
                    {
                        Servicos.Log.TratarErro("Encerrando MDFe " + mdfe.Chave, "EncerramentoMDFeAutomatico");

                        Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamentoMDFe = repMunicipioDescarregamento.BuscarPrimeiroPorMDFe(mdfe.Codigo);

                        mdfe.MunicipioEncerramento = (municipioDescarregamentoMDFe != null && municipioDescarregamentoMDFe.Municipio != null) ? municipioDescarregamentoMDFe.Municipio : mdfe.Empresa.Localidade;

                        if (mdfe.MunicipioEncerramento != null)
                        {
                            repMDFe.Atualizar(mdfe);

                            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);

                            DateTime dataEncerramento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

                            if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unitOfWork, dataEncerramento))
                            {
                                //if (svcAverbacao.Emitir(mdfe, Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento, unitOfWork))
                                svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, "Encerramento automatico MDFe emitido com mais de 30 dias", unitOfWork);
                            }
                        }
                    }
                    else
                        Servicos.Log.TratarErro("Encerrando MDFe " + mdfe.Chave + " Certificado digital vencido.", "ExecucaoDiariaEncerramentoMDFeAutomatico");
                }
            }
        }

        private void VerificarDadosCadastraisTransportadores(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)//Por enquanto só embarcador vai usar
                return;

            try
            {
                Servicos.Empresa servicoEmpresa = new Servicos.Empresa(unitOfWork);

                servicoEmpresa.AtualizarIntegracoes();
                servicoEmpresa.GerarIntegracoes();
                servicoEmpresa.VerificarIntegracoesPendentes(auditado);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarContratoNotaFiscalPendenteAceiteTransportadores(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.EmpresaContrato repEmpresaContrato = new Repositorio.EmpresaContrato(unitOfWork);

            List<Dominio.Entidades.Empresa> transportadores = repEmpresaContrato.ConsultarEmpresaContratosPendentesParaNotificarPorEmail();

            if (transportadores.Count == 0)
                return;

            AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso repClienteURLAcesso = new AdminMultisoftware.Repositorio.Pessoas.ClienteURLAcesso(unitOfWorkAdmin);
            AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteURLAcesso = repClienteURLAcesso.BuscarPorClienteETipoProducao(_clienteMultisoftware.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);

            Servicos.Email svcEmail = new Servicos.Email(unitOfWork);

            foreach (Dominio.Entidades.Empresa empresa in transportadores)
            {
                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(empresa.Email))
                    emails.AddRange(empresa.Email.Split(';').ToList());

                if (emails.Count == 0)
                    continue;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("<p>Sua transportadora possui o termo de aceite do sistema pendente<br /><br />");
                sb.Append("Para aceitar o termo acesse o portal do MultiCTe");
                if (clienteURLAcesso != null)
                    sb.Append("https://" + clienteURLAcesso.URLAcesso);
                sb.Append("<br /><br />By MultiSoftware");

                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, emails[0], "", "", "Termo de Aceite Pendente", sb.ToString(), string.Empty, null, null, true, "cte@multisoftware.com.br", 0, unitOfWork);
            }
        }

        private void ObterFaturasTransportadores(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataInicial = DateTime.Today.AddDays(-3);
            DateTime dataFinal = DateTime.Today;

            Servicos.Embarcador.CIOT.CIOT serCIOT = new Servicos.Embarcador.CIOT.CIOT();
            serCIOT.ObterFaturasTransportadores(dataInicial, dataFinal, unitOfWork);
        }

        private void ValidarLicencasVeiculoVencidas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Veiculo.LicencaVeiculo servicoLicencaVeiculo = new Servicos.Embarcador.Veiculo.LicencaVeiculo(unitOfWork, tipoServicoMultisoftware);
            servicoLicencaVeiculo.ValidarLicencasVeiculoVencidas();
        }

        private void GerarCargaImportacaoProgramacaoColeta(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta repositorioImportacaoProgramacaoColeta = new Repositorio.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork);

            Servicos.Embarcador.Logistica.ImportacaoProgramacaoColeta servicoImportacaoProgramacaoColeta = new Servicos.Embarcador.Logistica.ImportacaoProgramacaoColeta(unitOfWork, tipoServicoMultisoftware);

            List<Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta> importacoesProgramacaoColeta = repositorioImportacaoProgramacaoColeta.BuscarProgramacoesEmAndamento();

            foreach (Dominio.Entidades.Embarcador.Logistica.ImportacaoProgramacaoColeta importacaoProgramacaoColeta in importacoesProgramacaoColeta)
                servicoImportacaoProgramacaoColeta.GeracaoProximasCargas(importacaoProgramacaoColeta, configuracao, auditado);
        }

        private static TimeSpan ObterHorarioExecucao(Repositorio.UnitOfWork unitOfWork)
        {
            string horarioExecucaoConfigurado = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().HorarioExecucaoThreadDiaria;

            if (!string.IsNullOrWhiteSpace(horarioExecucaoConfigurado))
            {
                string[] horarioExecucaoConfiguradoParticionado = horarioExecucaoConfigurado.Split(':');

                if (horarioExecucaoConfiguradoParticionado.Length == 2)
                {
                    int horas = horarioExecucaoConfiguradoParticionado[0].ToInt();
                    int minutos = horarioExecucaoConfiguradoParticionado[1].ToInt();

                    return new TimeSpan(hours: horas, minutes: minutos, seconds: 0);
                }
            }

            return new TimeSpan(hours: 0, minutes: 15, seconds: 0);
        }

        private void GerarLiquidacaoPalletAutomaticamente(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware)
        {

            Repositorio.Embarcador.Pallets.DevolucaoPallet repositorioDevolucaoPallet = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoCargaOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes repConfiguracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = repConfiguracaoPaletes.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet> listaPallet = repositorioDevolucaoPallet.BuscarPalletPendentes();

            if (configuracaoPaletes.LiquidarPalletAutomaticamente)
                foreach (Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucaoPallet in listaPallet)
                    servicoCargaOcorrencia.GerarOcorrenciaDevolucaoPallet(devolucaoPallet, configuracaoPaletes.TipoOcorrencia, configuracao, tipoServicoMultisoftware, _clienteMultisoftware, unitOfWork, configuracaoPaletes.QteDiasParaLiquidarPallet);


        }

        private void VerificarFilasCarregamentoVeiculoAlterarDataProgramada(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            unitOfWork.FlushAndClear();

            try
            {
                new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema).VerificarDataProgramadaAlterarAutomaticamente(tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void GerarPlanejamentoDeFrotaAutomatizado(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = TipoAuditado.Sistema,
                    OrigemAuditado = OrigemAuditado.Sistema,
                    Texto = $"Servicos.Embarcador.Frotas.GeradorDeFrotaMensal; Data: {DateTime.Now.ToDateString()}"
                };
                Servicos.Embarcador.Frotas.PlanejamentoFrotaMes servicoSugestaoMensal = new Servicos.Embarcador.Frotas.PlanejamentoFrotaMes(unitOfWork, auditado);

                servicoSugestaoMensal.GerarSugestaoFrotaAutomaticamente();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarTrasnportadorCanhotosRejeitados(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> canhotos = repositorioCanhoto.ConsultaCanhotosRejeitadosPorTipoOperacao();
                List<int> codigosTransportadores = canhotos.Select(c => c.Transportador).Distinct().ToList();
                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                foreach (int transportador in codigosTransportadores)
                {
                    System.Text.StringBuilder mensagemEmail = new System.Text.StringBuilder();
                    List<Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto> canhotosDoTransportador = canhotos.Where(c => c.Transportador == transportador).ToList();
                    List<Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado> planilhaEmailCanhotoRejeitado = new List<Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado>();
                    Dominio.ObjetosDeValor.Embarcador.Canhoto.NotificacaoCanhoto canhotoBase = canhotosDoTransportador.Count > 0 ? canhotosDoTransportador.FirstOrDefault() : null;

                    foreach (var canhoto in canhotosDoTransportador)
                    {
                        dynamic planilhaCriacao = new Dominio.ObjetosDeValor.Embarcador.Canhoto.PlanilhaEmailCanhotoRejeitado()
                        {
                            Numero = canhoto.Numero,
                            DataEmissao = canhoto.DataEmissao,
                            Destinatario = canhoto.Cliente,
                            NumeroCarga = canhoto.NumeroCarga,
                            SituacaoDigitalizacaoCanhoto = canhoto.SituacaoDigitalizacao.ObterDescricao(),
                            SituacaoRecebimentoCanhoto = canhoto.SituacaoCanhoto.ObterDescricao()
                        };

                        planilhaEmailCanhotoRejeitado.Add(planilhaCriacao);
                    }

                    planilhaEmailCanhotoRejeitado = planilhaEmailCanhotoRejeitado.OrderBy(o => o.Numero).ToList();

                    byte[] byteArrayObjeto = Utilidades.CSV.GerarCSV(planilhaEmailCanhotoRejeitado); ;

                    System.IO.MemoryStream mscsv = new System.IO.MemoryStream(byteArrayObjeto);

                    List<System.Net.Mail.Attachment> anexos = new List<System.Net.Mail.Attachment>();
                    anexos.Add(new System.Net.Mail.Attachment(mscsv, "CanhotosPendentes.CSV"));

                    mensagemEmail.AppendLine($"{canhotoBase.DescricaoTransportador}, Por meio deste email informamos que os canhotos relacionados no arquivo anexo tiveram sua digitalização rejeitada.");
                    string corpoEmail = mensagemEmail.ToString();

                    string emailBase = string.IsNullOrEmpty(canhotoBase.EmailEnvioCanhotoTransportador) ? canhotoBase.EmailTransportador : canhotoBase.EmailEnvioCanhotoTransportador;

                    serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, emailBase, "", "", "Canhotos Rejeitados", mensagemEmail.ToString(), string.Empty, anexos, string.Empty, false, string.Empty, 0, unitOfWork);
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarVencimentoRegraICMS(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            try
            {
                new Servicos.Embarcador.ICMS.RegraICMS(unitOfWork, tipoServicoMultisoftware, razaoSocial).AlertarRegraForaDeVigencia();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void EnviarEmailTransportadorComEntregaEmAtraso(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Pedido.Pedido serPedido = new Servicos.Embarcador.Pedido.Pedido();

                serPedido.EnviarEmailPedidoComEntregaPendente(unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }
        private void EnviarEmailNFSePendente(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.NFSe.NFSManual NfsManual = new Servicos.Embarcador.NFSe.NFSManual(unitOfWork);

                NfsManual.VerificarNFSePendentes(unitOfWork);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void InativarUsuariosAposXDiasSemAcesso(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Login.Login servicoLogin = new Servicos.Embarcador.Login.Login(unitOfWork);

                servicoLogin.InativarUsuariosAposXDiasSemAcesso(unitOfWork, tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void EnviarEmailAprovadoresTarifasPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente serConfiguracaoDescargaCliente = new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);

                serConfiguracaoDescargaCliente.VerificarTarifasPendentesAprovacao();
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void DefinirNaoComparecimentoJanelaDescarregamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Servicos.Embarcador.Logistica.JanelaDescarga servicoJanelaDescarga = new Servicos.Embarcador.Logistica.JanelaDescarga(unitOfWork, _auditado, _tipoServicoMultisoftware, null, configuracaoEmbarcador);
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> listaCargasPendentes = repCargaJanelaDescarregamento.BuscarParaNaoComparecimento();

                if (listaCargasPendentes == null || listaCargasPendentes.Count == 0)
                    return;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaPendete in listaCargasPendentes)
                {
                    unitOfWork.Start();

                    Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);
                    Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                    Repositorio.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento repHistoricoSituacaoJanelaDescarregamento = new Repositorio.Embarcador.Logistica.HistoricoSituacaoCargaJanelaDescarregamento(unitOfWork);

                    Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaPendete.Carga.Codigo);

                    servicoCargaJanelaDescarregamento.InserirHistoricoCargaJanelaDescarregamento(cargaPendete, SituacaoCargaJanelaDescarregamento.NaoComparecimento);

                    cargaPendete.Situacao = SituacaoCargaJanelaDescarregamento.NaoComparecimento;
                    repCargaJanelaDescarregamento.Atualizar(cargaPendete);

                    Servicos.Auditoria.Auditoria.Auditar(_auditado, cargaPendete, null, "Não Comparecimento definido automaticamente.", unitOfWork);
                    try
                    {
                        servicoJanelaDescarga.NaoComparecimentoCargaDevolvida(new List<int> { cargaPendete.Codigo }, TipoGatilhoNotificacao.NoShowNaoComparecimento, true, true);
                    }
                    catch (BaseException excecao)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(excecao.Message + $" Codigo cargaJanela {cargaPendete.Codigo}");
                        continue;
                    }

                    servicoJanelaDescarregamento.AdicionarIntegracaoComAtualizacao(cargaPendete.Carga, unitOfWork);

                    unitOfWork.CommitChanges();

                    EnviarEmailNaoComparecimento(agendamentoColeta, unitOfWork, cliente);
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void EnviarEmailNaoComparecimento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailNaoComparecimento(agendamentoColeta, emails, cliente);
        }

        private void EnviarEmailResumoNCPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Start();
                Servicos.Embarcador.NotaFiscal.NaoConformidade svcNaoConformidade = new Servicos.Embarcador.NotaFiscal.NaoConformidade(unitOfWork);

                svcNaoConformidade.VerificarNaoConformidadesPendentesDeEnvioResumo();
                unitOfWork.CommitChanges();
                unitOfWork.FlushAndClear();
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void NotificarTransportadorDevolucaoPendente(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, auditado, clienteMultisoftware);
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> DevolucoesNotificacao = repositorioGestaoDevolucao.ConsultaDevolucaoNotificarTransportador(72);

            foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao devolucao in DevolucoesNotificacao)
                servicoGestaoDevolucao.EnviarEmailNotificacaoTransportadorDevolucao(devolucao, _clienteMultisoftware);
        }

        //talvez migrar para thread que executa mais vezes ao dia;
        private void AlterarDevolucoesAutomaticamenteAoEsgotarTempo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(unitOfWork, auditado, clienteMultisoftware);
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa repositorioGestaoDevolucaoEtapa = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(unitOfWork).BuscarPrimeiroRegistro();
            try
            {
                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> DevolucoesAterarTipoAutomaticamento = repositorioGestaoDevolucao.ConsultaDevolucaoComTempoEsgotadoSemTipo(72, configuracaoPaletes.LimiteDiasParaDevolucaoDePallet);

                foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao in DevolucoesAterarTipoAutomaticamento)
                {
                    unitOfWork.Start();

                    gestaoDevolucao.Initialize();

                    servicoGestaoDevolucao.DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.Permuta);
                    servicoGestaoDevolucao.DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.OrdemeRemessa);//ja esta avançando para a 3 etapa da permuta.
                    servicoGestaoDevolucao.FinalizarEtapasAnteriores(gestaoDevolucao, gestaoDevolucao.EtapaAtual);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), "Avançou etapa automaticamente ao exceder o tempo limite do transportador", unitOfWork);

                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void BuscarValorCotacaoMoedaDiaria(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (!configuracaoEmbarcador.UtilizaMoedaEstrangeira)
                    return;

                Servicos.Embarcador.Moedas.Cotacao servicoCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);

                DateTime dataBase = DateTime.Today.AddDays(-1);

                servicoCotacao.AdicionarCotacaoDiaria(MoedaCotacaoBancoCentral.DolarCompra, dataBase, unitOfWork);
                servicoCotacao.AdicionarCotacaoDiaria(MoedaCotacaoBancoCentral.DolarVenda, dataBase, unitOfWork);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion
    }
}
