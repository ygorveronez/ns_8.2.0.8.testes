using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 5000)]

    public class ControleCargaEmEmissao : LongRunningProcessBase<ControleCargaEmEmissao>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            SolicitarEmissaoCargasEmEmissao(unitOfWork);
            SolicitarEmissaoCargasRejeitadas(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
            FecharCargasEmFechamento(unitOfWork, _webServiceConsultaCTe, _stringConexao, _tipoServicoMultisoftware, _clienteMultisoftware);
            SolicitarEmissaoNFeRemessaEmEmissao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware);
        }

        #endregion Métodos Protegidos

        #region Métodos Privados

        private void SolicitarEmissaoCargasEmEmissao(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

                Servicos.Global.OrquestradorFila orquestrador = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.SolicitarEmissaoCargasEmEmissao);

                List<int> cargas = orquestrador.Ordenar((limiteRegistros) => repositorioCarga.BuscarCodigosCargasEmEmissao(maximoRegistros: limiteRegistros, controlePorLote: false));

                int codigoCarga = 0;

                for (int i = 0; i < cargas.Count; i++)
                {
                    try
                    {
                        unitOfWork.Start();
                        codigoCarga = cargas[i];
                        servicoCargaCTe.EmitirCTes(codigoCarga);
                        unitOfWork.CommitChanges();
                        orquestrador.RegistroLiberadoComSucesso(codigoCarga);

                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        orquestrador.RegistroComFalha(codigoCarga, ex.Message);
                        Servicos.Log.TratarErro(ex);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void SolicitarEmissaoNFeRemessaEmEmissao(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Cargas.CargaNFe repCargaNFe = new Repositorio.Embarcador.Cargas.CargaNFe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                if (configuracao.EmitirNFeRemessaNaCarga)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaNFe> listaCargas = repCargaNFe.BuscarCargasEmEmissao(10);

                    foreach (var cargaEmitida in listaCargas)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaEmitida.Carga.Codigo);
                        if (cargaEmitida.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Emitido || cargaEmitida.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Rejeitado)
                        {
                            string retorno = EmitirNFe(cargaEmitida.NotaFiscal.Codigo, unitOfWork, "", Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), carga.Operador, tipoServicoMultisoftware);
                            if (string.IsNullOrWhiteSpace(retorno))
                            {
                                carga.MotivoPendencia = string.Empty;
                                carga.PossuiPendencia = false;
                                carga.EmitindoNFeRemessa = true;
                                carga.problemaEmissaoNFeRemessa = false;

                                repCarga.Atualizar(carga);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void SolicitarEmissaoCargasRejeitadas(Repositorio.UnitOfWork unitOfWork, string stringConexao, string webServiceConsultaCTe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador.NumeroTentativasReenvioCteRejeitado <= 0)
                    return;

                int tempoAguardarParaReenviarEmMinutos = configuracaoEmbarcador.TempoMinutosParaReenviarCancelamento > 0 ? configuracaoEmbarcador.TempoMinutosParaReenviarCancelamento : 5;
                DateTime dataLimiteReenvio = DateTime.Now.AddMinutes(-tempoAguardarParaReenviarEmMinutos);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem> listaCargas = repositorioCarga.BuscarCodigosCargasCTesRejeitados(10, configuracaoEmbarcador.NumeroTentativasReenvioCteRejeitado, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm"), dataLimiteReenvio.ToString("yyyy-MM-dd HH:mm"));

                foreach (Dominio.ObjetosDeValor.Embarcador.Carga.CargaMensagem cargaRejeitada in listaCargas)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(cargaRejeitada.CodigoCarga);

                    if (carga != null && carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos && carga.problemaCTE)
                    {
                        Servicos.Log.TratarErro($"Carga código {carga.Codigo}: Reenviado CTes rejeitados.", "ReenvioCTesRejeitadosCarga");

                        carga.problemaCTE = false;
                        carga.PossuiPendencia = false;
                        carga.MotivoPendencia = string.Empty;
                        carga.EmitindoCTes = true;

                        repositorioCarga.Atualizar(carga);
                    }
                }
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void FecharCargasEmFechamento(Repositorio.UnitOfWork unitOfWork, string webServiceConsultaCTe, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultiSoftware)
        {
            try
            {
                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.Global.OrquestradorFila orquestrador = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.FecharCargasEmFechamentoWorker);
                List<int> listaCargas = orquestrador.Ordenar((limiteRegistros) => repCarga.BuscarCodigosCargasEmFechamento(limiteRegistros));

                int codigoCarga = default(int);

                for (var i = 0; i < listaCargas.Count; i++)
                {
                    try
                    {
                        codigoCarga = listaCargas[i];
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                        if (carga != null)
                        {

                            Servicos.Log.TratarErro("Fechando carga " + carga.CodigoCargaEmbarcador, "FechamentoCarga");

                            new Servicos.Embarcador.Carga.Carga(unitOfWork).FecharCarga(carga, unitOfWork, tipoServicoMultisoftware, ClienteMultiSoftware);

                            carga.FechandoCarga = false;
                            carga.CargaFechada = true;
                            Servicos.Log.TratarErro("21 - Fechou Carga (" + carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

                            repCarga.Atualizar(carga);
                            orquestrador.RegistroLiberadoComSucesso(codigoCarga);

                            Servicos.Log.TratarErro("Carga " + carga.CodigoCargaEmbarcador + " fechada.", "FechamentoCarga");
                        }
                    }
                    catch (Exception ex)
                    {
                        orquestrador.RegistroComFalha(codigoCarga, $"FechamentoCarga {ex.Message}");
                        Servicos.Log.TratarErro(ex, "FechamentoCarga");
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "FechamentoCarga");
            }
        }

        private string EmitirNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string mensagem = "";
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNota(codigoNFe);

            if (nfe == null)
                mensagem = "O NF-e informado não foi localizado";
            else if (nfe != null && (itens == null || itens.Count == 0))
                mensagem = "A NF-e não possui nenhum item lançado.";
            else
            {
                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                mensagem = z.CriarEnviarNFe(codigoNFe, unitOfWork, tipoServicoMultisoftware, relatorio, caminhoRelatoriosEmbarcador, usuario, "55", 1, false, false, null, "");
            }

            return mensagem;
        }

        #endregion Métodos Privados
    }
}
