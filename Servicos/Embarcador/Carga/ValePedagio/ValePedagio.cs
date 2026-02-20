using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.ValePedagio
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void EnviarEmailTransportador(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, bool notificarTransportadorPorEmail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!notificarTransportadorPorEmail)
                return;

            if (cargaValePedagio.SituacaoIntegracao != SituacaoIntegracao.Integrado && cargaValePedagio.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao)
                return;

            try
            {
                byte[] arquivo = null;
                string retorno = null;
                List<System.Net.Mail.Attachment> anexos = new List<Attachment>();

                StringBuilder mensagem = new StringBuilder()
                    .AppendLine($"Olá {cargaValePedagio.Carga.Empresa.NomeCNPJ}")
                    .AppendLine();

                if (cargaValePedagio.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                {
                    mensagem.AppendLine($"O vale pedágio da carga {cargaValePedagio.Carga.CodigoCargaEmbarcador} foi comprado com sucesso no {cargaValePedagio.TipoIntegracao.DescricaoTipo}.");
                    mensagem.AppendLine();
                    mensagem.AppendLine($"Data: {cargaValePedagio.DataIntegracao.ToString("dd/MM/yyyy")}");
                    mensagem.AppendLine($"Número Vale Pedágio: {cargaValePedagio.NumeroValePedagio}");
                    mensagem.AppendLine($"Valor: {cargaValePedagio.ValorValePedagio.ToString("n2")}.");
                    mensagem.AppendLine();
                    if (cargaValePedagio.ValorValePedagio == 0)
                        mensagem.AppendLine($"Obs.: o valor R$ 0,00 indica que não houve a necessidade de compra para a viagem.");

                    retorno = ObterArquivoValePedagio(cargaValePedagio, ref arquivo, tipoServicoMultisoftware);
                }
                else
                {
                    mensagem.AppendLine($"O vale pedágio da carga {cargaValePedagio.Carga.CodigoCargaEmbarcador} não efetuou a compra no {cargaValePedagio.TipoIntegracao.DescricaoTipo}.");
                    mensagem.AppendLine($"Motivo: {cargaValePedagio.ProblemaIntegracao}.");
                }

                Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Servicos.Embarcador.Notificacao.NotificacaoEmpresa(_unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                {
                    AssuntoEmail = $"Retorno de Compra de Vale Pedágio do Veículo {cargaValePedagio.Carga.Veiculo?.Placa}",
                    Empresa = cargaValePedagio.Carga.Empresa,
                    Mensagem = mensagem.ToString(),
                    NotificarSomenteEmailPrincipal = true,
                    Anexos = anexos,
                };

                if (string.IsNullOrWhiteSpace(retorno) && arquivo != null)
                {
                    MemoryStream stream = new MemoryStream(arquivo);
                    Attachment anexo = new Attachment(stream, cargaValePedagio.NumeroValePedagio + ".pdf", "application/pdf");

                    notificacaoEmpresa.Anexos.Add(anexo);
                }

                servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
            }
            catch (Exception execao)
            {
                Log.TratarErro(execao);
            }
        }

        public string ObterArquivoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaIntegracaoValePedagio, ref byte[] arquivo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            arquivo = null;

            if (cargaIntegracaoValePedagio == null)
                return "Vale Pedágio não encontrado.";

            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo repositorioAnexo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo anexo = repositorioAnexo.BuscarPorCargaIntegracao(cargaIntegracaoValePedagio.Codigo);

            if (anexo != null)
            {
                string caminho = ObterCaminhoPorTipoIntegracaoValePedagio(TipoIntegracaoValePedagioHelper.ObterPastaPorTipoIntegracao(cargaIntegracaoValePedagio.TipoIntegracao.Tipo));
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{anexo.GuidArquivo}");
                arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);

                return string.Empty;
            }

            string mensagemRetorno = string.Empty;

            if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
            {
                Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                arquivo = servicoSemParar.GerarImpressaoValePedagio(cargaIntegracaoValePedagio, _unitOfWork, tipoServicoMultisoftware);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Target)
            {
                Servicos.Embarcador.Integracao.Target.ValePedagio serValePedagioTarget = new Servicos.Embarcador.Integracao.Target.ValePedagio(_unitOfWork);
                serValePedagioTarget.ObterDocumento(cargaIntegracaoValePedagio, ref arquivo, ref mensagemRetorno, tipoServicoMultisoftware, _unitOfWork);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom)
            {
                Servicos.Embarcador.Integracao.Repom.ValePedagio servicoValePedagioRepom = new Servicos.Embarcador.Integracao.Repom.ValePedagio(_unitOfWork);
                arquivo = servicoValePedagioRepom.GerarImpressaoValePedagio(cargaIntegracaoValePedagio, tipoServicoMultisoftware);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PagBem)
            {
                Servicos.Embarcador.CIOT.Pagbem serPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                arquivo = serPagbem.GerarImpressaoValePedagio(cargaIntegracaoValePedagio, tipoServicoMultisoftware, _unitOfWork);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete)
            {
                Servicos.Embarcador.Integracao.EFrete.ValePedagio servicoEFrete = new Servicos.Embarcador.Integracao.EFrete.ValePedagio(_unitOfWork);
                arquivo = servicoEFrete.ObterReciboPdf(cargaIntegracaoValePedagio, tipoServicoMultisoftware, ref mensagemRetorno);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Extratta)
            {
                Servicos.Embarcador.Integracao.Extratta.ValePedagio servicoExtratta = new Servicos.Embarcador.Integracao.Extratta.ValePedagio(_unitOfWork);
                arquivo = servicoExtratta.ObterReciboPdf(cargaIntegracaoValePedagio, tipoServicoMultisoftware, ref mensagemRetorno);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard)
            {
                Integracao.Pamcard.ValePedagio servicoPamcard = new Servicos.Embarcador.Integracao.Pamcard.ValePedagio(_unitOfWork);
                arquivo = servicoPamcard.GerarImpressaoValePedagio(cargaIntegracaoValePedagio, _unitOfWork, tipoServicoMultisoftware);
            }
            else if (cargaIntegracaoValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans)
            {
                Servicos.Embarcador.Integracao.DBTrans.ValePedagio servicoRodoCred = new Servicos.Embarcador.Integracao.DBTrans.ValePedagio(_unitOfWork);
                arquivo = servicoRodoCred.GerarImpressaoValePedagio(cargaIntegracaoValePedagio);
            }
            else
                mensagemRetorno = "Não existe impressão para o tipo de vale pedágio utilizado.";

            return mensagemRetorno;
        }

        public string ObterCaminhoPorTipoIntegracaoValePedagio(string integracao)
        {
            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao", integracao);

            return caminhoArquivo;
        }

        public async Task ConsultarValoresPedagioPendenteAsync(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<int> codigosCargasConsultaValorPedagioIntegracaoPendentes = null)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.ConsultarValoresPedagioPendente);

            if (codigosCargasConsultaValorPedagioIntegracaoPendentes == null)
                codigosCargasConsultaValorPedagioIntegracaoPendentes = servicoOrquestradorFila.Ordenar((limiteRegistros) => repCargaConsultaValePedagio.ConsultaIntegracaoAgIntegracao(limiteRegistros, 3, 5));

            List<Task> tasks = new List<Task>();

            foreach (int codigo in codigosCargasConsultaValorPedagioIntegracaoPendentes)
                tasks.Add(Task.Run(() => ExecutaConsultaValoresPedagioAsync(codigo, tipoServicoMultisoftware)));

            await Task.WhenAll(tasks);
        }

        public async Task GerarIntegracoesValePedagioAsync(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<int> codigosGerarIntegracoesValePedagio = null)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.GerarIntegracoesValePedagio);

            if (codigosGerarIntegracoesValePedagio == null)
                codigosGerarIntegracoesValePedagio = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCargaValePedagio.BuscarCargarAgIntegracaoValePedagio(limiteRegistros, 2, 5));

            List<Task> tasks = new List<Task>();
            SemaphoreSlim semaphore = new SemaphoreSlim(5);

            foreach (int codigo in codigosGerarIntegracoesValePedagio)
                tasks.Add(ProcessarIntegracoesValePedagioAsync(codigo, tipoServicoMultisoftware, semaphore));

            await Task.WhenAll(tasks);
        }

        private async Task ProcessarIntegracoesValePedagioAsync(int codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();

            try
            {
                await WaitWithTimeoutAsync(ExecutaGerarIntegracoesValePedagioAsync(codigo, tipoServicoMultisoftware), TimeSpan.FromMinutes(4));
            }
            catch (TimeoutException ex)
            {
                Servicos.Log.TratarErro($"Timeout ao processar CargaIntegracaoValePedagio {codigo}: {ex.Message}", "ValePedagio");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ValePedagio");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task GerarIntegracoesCancelamentoValePedagioAsync(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<int> codigosGerarIntegracoesCancelamentoValePedagio = null)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            if (codigosGerarIntegracoesCancelamentoValePedagio == null)
                codigosGerarIntegracoesCancelamentoValePedagio = repositorioCargaValePedagio.BuscarPorCargaEmCancelamento();

            List<Task> tasks = new List<Task>();

            foreach (int codigo in codigosGerarIntegracoesCancelamentoValePedagio)
                tasks.Add(Task.Run(() => ExecutaGerarIntegracoesCancelamentoValePedagioAsync(codigo, tipoServicoMultisoftware)));

            await Task.WhenAll(tasks);
        }

        public async Task VerificarRetornosValePedagioAsync(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<int> codigosVerificarRetornosValePedagio = null)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(_unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.VerificarRetornosValePedagio);

            if (codigosVerificarRetornosValePedagio == null)
                codigosVerificarRetornosValePedagio = servicoOrquestradorFila.Ordenar((limiteRegistros) => repositorioCargaValePedagio.BuscarValePedagioAgRetorno(limiteRegistros));

            List<Task> tasks = new List<Task>();

            foreach (int codigo in codigosVerificarRetornosValePedagio)
                tasks.Add(Task.Run(() => ExecutaVerificarRetornosValePedagioAsync(codigo, tipoServicoMultisoftware)));

            await Task.WhenAll(tasks);
        }

        public void ProcessarExtratoValePedagioPendentes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValePedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                servicoValePedagioSemParar.ProcessarExtratoValePedagioPendentes(_unitOfWork, tipoServicoMultisoftware);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("ProcessarExtratoValePedagioPendentes: \n" + ex, "ConsultaExtratoValePedagio");
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void AtualizarValorNotasEPedagio(Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido, Servicos.Embarcador.Carga.RateioFormula serRateioFormula, decimal valorValePedagio, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            decimal pesoTotal = cargaPedidos.Sum(obj => obj.Peso);
            decimal pesoLiquidoTotal = cargaPedidos.Sum(o => o.PesoLiquido);
            int volumeTotal = cargaPedidos.Sum(obj => obj.Pedido.QtVolumes);
            decimal valorTotalNF = cargaPedidos.Sum(obj => obj.Pedido.ValorTotalNotasFiscais);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima;

            decimal valorTotalRateado = 0;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimoFreteCargaPedido = cargaPedidos.LastOrDefault();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                decimal valorNFsPedido = cargaPedido.Pedido.ValorTotalNotasFiscais;
                decimal valorRateioOriginal = 0;
                cargaPedido.ValorPedagio = serRateioFormula.AplicarFormulaRateio(cargaPedido.FormulaRateio, valorValePedagio, cargaPedidos.Count(), 1, pesoTotal, cargaPedido.Peso, valorNFsPedido, valorTotalNF, 0, tipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, cargaPedido.Pedido.PesoLiquidoTotal, pesoLiquidoTotal, cargaPedido.Pedido.QtVolumes, volumeTotal);

                valorTotalRateado += cargaPedido.ValorPedagio;
                if (cargaPedido.Codigo == ultimoFreteCargaPedido.Codigo)
                {
                    decimal diferenca = valorValePedagio - valorTotalRateado;
                    cargaPedido.ValorPedagio = cargaPedido.ValorPedagio + diferenca;
                }

                repCargaPedido.Atualizar(cargaPedido);
            }
        }

        private void AdicionarComponentePedagioCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, decimal valorValePedagio)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponenteFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(_unitOfWork);

            Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorTipoComponenteFrete(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO);
            Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete cargaComponenteFrete = repCargaComponenteFrete.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO, null, false);

            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, componenteFrete);
            bool naoSomarValorTotalAReceber = (destacarComponenteTabelaFrete ? carga.TabelaFrete?.NaoSomarValorTotalAReceber : componenteFrete?.NaoSomarValorTotalAReceber) ?? false;
            bool naoDestacarResultadoConsultaPedagioComoComponente = carga.TabelaFrete?.NaoDestacarResultadoConsultaPedagioComoComponente ?? false;

            if (valorValePedagio > 0 && !naoSomarValorTotalAReceber && !naoDestacarResultadoConsultaPedagioComoComponente)
            {
                if (cargaComponenteFrete == null)
                    cargaComponenteFrete = new Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete();

                cargaComponenteFrete.ValorComponente = valorValePedagio;
                cargaComponenteFrete.TipoComponenteFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO;
                cargaComponenteFrete.Carga = carga;
                cargaComponenteFrete.ComponenteFrete = componenteFrete;
                cargaComponenteFrete.TipoValor = TipoCampoValorTabelaFrete.AumentoValor;
                cargaComponenteFrete.Percentual = 0;
                cargaComponenteFrete.Tipo = TipoCargaComponenteFrete.TabelaFrete;
                cargaComponenteFrete.IncluirBaseCalculoICMS = false;
                cargaComponenteFrete.DescontarValorTotalAReceber = false;
                cargaComponenteFrete.AcrescentaValorTotalAReceber = false;
                cargaComponenteFrete.SomarComponenteFreteLiquido = false;
                cargaComponenteFrete.SempreExtornar = false;
                cargaComponenteFrete.ComponenteFilialEmissora = false;
                cargaComponenteFrete.PorQuantidadeDocumentos = false;
                cargaComponenteFrete.QuantidadeTotalDocumentos = 0;

                if (destacarComponenteTabelaFrete)
                {
                    cargaComponenteFrete.NaoSomarValorTotalAReceber = carga.TabelaFrete?.NaoSomarValorTotalAReceber ?? false;
                    cargaComponenteFrete.DescontarComponenteFreteLiquido = carga.TabelaFrete?.DescontarComponenteFreteLiquido ?? false;
                }
                else
                {
                    cargaComponenteFrete.NaoSomarValorTotalAReceber = false;
                    cargaComponenteFrete.DescontarComponenteFreteLiquido = false;
                }

                if (cargaComponenteFrete.Codigo > 0)
                    repCargaComponenteFrete.Atualizar(cargaComponenteFrete);
                else
                    repCargaComponenteFrete.Inserir(cargaComponenteFrete);
            }
        }

        private async Task ExecutaConsultaValoresPedagioAsync(int codigoCargaConsultaValorPedagioIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repositorioCargaConsultaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repositorioCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);

                Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValepedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                Servicos.Embarcador.Integracao.QualP.ValePedagio servicoValePedagioQualp = new Servicos.Embarcador.Integracao.QualP.ValePedagio();
                Servicos.Embarcador.Integracao.DBTrans.ValePedagio servicoValePedagioDBTrans = new Servicos.Embarcador.Integracao.DBTrans.ValePedagio(unitOfWork);
                Servicos.Embarcador.Carga.RateioFormula servicoRateioFormula = new Servicos.Embarcador.Carga.RateioFormula(unitOfWork);
                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.ConsultarValoresPedagioPendente);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio = repositorioCargaConsultaValePedagio.BuscarPorCodigo(codigoCargaConsultaValorPedagioIntegracao);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaConsultaValePedagio.Carga;

                try
                {
                    unitOfWork.Start();

                    if (carga.Veiculo?.NaoComprarValePedagio ?? false)
                    {
                        cargaConsultaValePedagio.ProblemaIntegracao = "Veículo configurado para não comprar vale pedágio. Informação do Embarcador";
                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                        cargaConsultaValePedagio.NumeroTentativas++;
                        repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);
                    }
                    else
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                        if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
                        {
                            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(carga, tipoServicoMultisoftware);

                            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = servicoValepedagioSemParar.Autenticar(cargaConsultaValePedagio, unitOfWork, tipoServicoMultisoftware);
                            if (credencial.Autenticado)
                            {
                                bool consultarValePedagio = true;
                                if (integracaoSemParar?.ConsultarEComprarPedagioFreteEmbarcador ?? false)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.RetornoConsultaSituacaoVeiculo retornoConsultaVeiculo = servicoValepedagioSemParar.consultarSituacaoVeiculoSemParar(credencial, cargaConsultaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);

                                    if (retornoConsultaVeiculo != null && retornoConsultaVeiculo.status != 0)
                                    {
                                        cargaConsultaValePedagio.ProblemaIntegracao = retornoConsultaVeiculo.Erro;
                                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                                        cargaConsultaValePedagio.DataIntegracao = retornoConsultaVeiculo.Data;
                                        //se ja deu esse erro vamos parar de tentar.
                                        cargaConsultaValePedagio.NumeroTentativas = 3;
                                        carga.IntegrandoValePedagio = false;
                                        carga.ProblemaIntegracaoValePedagio = true;
                                        carga.PossuiPendencia = true;
                                        carga.MotivoPendencia = retornoConsultaVeiculo.Erro;

                                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                                        cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoConsultaVeiculo.JsonRequest, "json", unitOfWork);
                                        cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoConsultaVeiculo.JsonResponse, "json", unitOfWork);
                                        cargaValePedagioIntegracaoArquivo.Data = retornoConsultaVeiculo.Data;

                                        cargaValePedagioIntegracaoArquivo.Mensagem = retornoConsultaVeiculo.Erro;
                                        cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                        repositorioCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                                        cargaConsultaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

                                        repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);

                                        servicoValepedagioSemParar.EnviarEmailTransportadorConsultaValePedagio(cargaConsultaValePedagio, integracaoSemParar, unitOfWork);
                                        consultarValePedagio = false;
                                    }
                                }

                                if (consultarValePedagio)
                                {
                                    decimal valorPedagio = servicoValepedagioSemParar.ConsultaValorPedagio(credencial, cargaConsultaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);
                                    if (valorPedagio > 0)
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                                        if (cargaPedidos.Count > 0)
                                        {
                                            decimal pesoTotal = cargaPedidos.Sum(obj => obj.Peso);
                                            decimal pesoLiquidoTotal = cargaPedidos.Sum(o => o.PesoLiquido);
                                            int volumeTotal = cargaPedidos.Sum(obj => obj.Pedido.QtVolumes);
                                            decimal valorTotalNF = cargaPedidos.Sum(obj => obj.Pedido.ValorTotalNotasFiscais);
                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima;

                                            decimal valorTotalRateado = 0;
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimoFreteCargaPedido = cargaPedidos.LastOrDefault();
                                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                                            {
                                                decimal valorNFsPedido = cargaPedido.Pedido.ValorTotalNotasFiscais;
                                                decimal valorRateioOriginal = 0;
                                                cargaPedido.ValorPedagio = servicoRateioFormula.AplicarFormulaRateio(cargaPedido.FormulaRateio, valorPedagio, cargaPedidos.Count(), 1, pesoTotal, cargaPedido.Peso, valorNFsPedido, valorTotalNF, 0, tipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, cargaPedido.Pedido.PesoLiquidoTotal, pesoLiquidoTotal, cargaPedido.Pedido.QtVolumes, volumeTotal);

                                                valorTotalRateado += cargaPedido.ValorPedagio;
                                                if (cargaPedido.Codigo == ultimoFreteCargaPedido.Codigo)
                                                {
                                                    decimal diferenca = valorPedagio - valorTotalRateado;
                                                    cargaPedido.ValorPedagio = cargaPedido.ValorPedagio + diferenca;
                                                }

                                                repositorioCargaPedido.Atualizar(cargaPedido);
                                            }

                                            //caso consultou, deu certo E TEM VALOR, devemos gerar a compra para o vale pedagio se tem a flag marcada;
                                            if (integracaoSemParar?.ConsultarEComprarPedagioFreteEmbarcador ?? false)
                                                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware, true);
                                        }

                                        carga.ProblemaIntegracaoValePedagio = false;
                                        carga.PossuiPendencia = false;
                                        carga.MotivoPendencia = "";
                                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                        cargaConsultaValePedagio.ValorValePedagio = valorPedagio;
                                    }
                                    else if (cargaConsultaValePedagio.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                                    {
                                        carga.ProblemaIntegracaoValePedagio = false;
                                        carga.PossuiPendencia = false;
                                        carga.MotivoPendencia = "";
                                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                        cargaConsultaValePedagio.ValorValePedagio = 0;
                                    }

                                    repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);

                                    servicoValepedagioSemParar.EnviarEmailTransportadorConsultaValePedagio(cargaConsultaValePedagio, integracaoSemParar, unitOfWork);
                                }
                            }
                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.QualP)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoQualP configQualP = servicoValePedagioQualp.ObterConfiguracaoQualP(cargaConsultaValePedagio, unitOfWork, tipoServicoMultisoftware, true);

                            if (configQualP != null)
                            {
                                decimal valorPedagio = servicoValePedagioQualp.ConsultaValorPedagio(configQualP, cargaConsultaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);
                                if (valorPedagio > 0m)
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                                    if (cargaPedidos.Count > 0)
                                    {
                                        decimal pesoTotal = cargaPedidos.Sum(obj => obj.Peso);
                                        decimal pesoLiquidoTotal = cargaPedidos.Sum(o => o.PesoLiquido);
                                        int volumeTotal = cargaPedidos.Sum(obj => obj.Pedido.QtVolumes);
                                        decimal valorTotalNF = cargaPedidos.Sum(obj => obj.Pedido.ValorTotalNotasFiscais);

                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima;

                                        decimal valorTotalRateado = 0m;

                                        for (int i = 0; i < cargaPedidos.Count; i++)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                                            decimal valorNFsPedido = cargaPedido.Pedido.ValorTotalNotasFiscais;
                                            decimal valorRateioOriginal = 0m;

                                            if ((i + 1) == cargaPedidos.Count)
                                                cargaPedido.ValorPedagio = valorPedagio - valorTotalRateado;
                                            else
                                                cargaPedido.ValorPedagio = servicoRateioFormula.AplicarFormulaRateio(cargaPedido.FormulaRateio, valorPedagio, cargaPedidos.Count(), 1, pesoTotal, cargaPedido.Peso, valorNFsPedido, valorTotalNF, 0, tipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, cargaPedido.Pedido.PesoLiquidoTotal, pesoLiquidoTotal, cargaPedido.Pedido.QtVolumes, volumeTotal);

                                            valorTotalRateado += cargaPedido.ValorPedagio;
                                        }
                                    }
                                }

                                cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                                cargaConsultaValePedagio.NumeroTentativas++;
                                repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);
                            }
                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans)
                        {
                            decimal valorPedagio = servicoValePedagioDBTrans.ConsultaValorPedagio(cargaConsultaValePedagio, carga, tipoServicoMultisoftware);
                            if (valorPedagio > 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                                if (cargaPedidos.Count > 0)
                                {
                                    decimal pesoTotal = cargaPedidos.Sum(obj => obj.Peso);
                                    decimal pesoLiquidoTotal = cargaPedidos.Sum(o => o.PesoLiquido);
                                    int volumeTotal = cargaPedidos.Sum(obj => obj.Pedido.QtVolumes);
                                    decimal valorTotalNF = cargaPedidos.Sum(obj => obj.Pedido.ValorTotalNotasFiscais);
                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.ValorFixoArredondadoParaCima;

                                    decimal valorTotalRateado = 0;
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido ultimoFreteCargaPedido = cargaPedidos.LastOrDefault();
                                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                                    {
                                        decimal valorNFsPedido = cargaPedido.Pedido.ValorTotalNotasFiscais;
                                        decimal valorRateioOriginal = 0;
                                        cargaPedido.ValorPedagio = servicoRateioFormula.AplicarFormulaRateio(cargaPedido.FormulaRateio, valorPedagio, cargaPedidos.Count(), 1, pesoTotal, cargaPedido.Peso, valorNFsPedido, valorTotalNF, 0, tipoValor, 0, 0, ref valorRateioOriginal, 0m, 0m, 0m, 0m, 0m, 0m, false, cargaPedido.Pedido.PesoLiquidoTotal, pesoLiquidoTotal, cargaPedido.Pedido.QtVolumes, volumeTotal);

                                        valorTotalRateado += cargaPedido.ValorPedagio;
                                        if (cargaPedido.Codigo == ultimoFreteCargaPedido.Codigo)
                                        {
                                            decimal diferenca = valorPedagio - valorTotalRateado;
                                            cargaPedido.ValorPedagio = cargaPedido.ValorPedagio + diferenca;
                                        }

                                        repositorioCargaPedido.Atualizar(cargaPedido);
                                    }
                                }

                                carga.ProblemaIntegracaoValePedagio = false;
                                carga.PossuiPendencia = false;
                                carga.MotivoPendencia = "";

                                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                                cargaConsultaValePedagio.ValorValePedagio = valorPedagio;
                                repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);
                            }
                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard)
                        {
                            Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard integracaoPamcard = servicoValePedagio.ObterIntegracaoPamcard(cargaConsultaValePedagio.Carga, tipoServicoMultisoftware);

                            Servicos.Embarcador.Integracao.Pamcard.ValePedagio serValePedagioPamcard = new Servicos.Embarcador.Integracao.Pamcard.ValePedagio(unitOfWork);
                            decimal valorValePedagio = serValePedagioPamcard.ConsultarValorPedagio(cargaConsultaValePedagio, carga, tipoServicoMultisoftware);

                            if (valorValePedagio > 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                                if (cargaPedidos.Count > 0 && (carga.TipoOperacao?.ConsultaDeValoresPedagioAdicionarComponenteFrete ?? true))
                                    AtualizarValorNotasEPedagio(repositorioCargaPedido, servicoRateioFormula, valorValePedagio, cargaPedidos);

                                if (integracaoPamcard?.AdicionarValorConsultadoComoComponentePedagioCarga ?? false)
                                    AdicionarComponentePedagioCarga(carga, valorValePedagio);

                                if (integracaoPamcard?.AcoesPamcard == TipoAcaoPamcard.ConsultaCompra)
                                    Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware, true, true);
                            }

                            carga.ProblemaIntegracaoValePedagio = false;
                            carga.PossuiPendencia = false;
                            carga.MotivoPendencia = "";

                            if (valorValePedagio == 0)
                                cargaConsultaValePedagio.ProblemaIntegracao = "Rota sem pedágio";
                            cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaConsultaValePedagio.ValorValePedagio = valorValePedagio;
                            repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);
                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom)
                        {

                            Servicos.Embarcador.Integracao.Repom.ValePedagio serValePedagioRepom = new Servicos.Embarcador.Integracao.Repom.ValePedagio(unitOfWork);
                            decimal valorValePedagio = serValePedagioRepom.ConsultarValorPedagio(cargaConsultaValePedagio, carga, tipoServicoMultisoftware);

                            if (valorValePedagio > 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                                if (cargaPedidos.Count > 0 && (carga.TipoOperacao?.ConsultaDeValoresPedagioAdicionarComponenteFrete ?? true))
                                    AtualizarValorNotasEPedagio(repositorioCargaPedido, servicoRateioFormula, valorValePedagio, cargaPedidos);

                                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware, true, true);
                            }

                            if (cargaConsultaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                            {
                                carga.ProblemaIntegracaoValePedagio = false;
                                carga.PossuiPendencia = false;
                                carga.MotivoPendencia = "";
                            }
                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo)
                        {
                            Servicos.Embarcador.Integracao.NDDCargo.ValePedagio servicoNDDCargo = new Servicos.Embarcador.Integracao.NDDCargo.ValePedagio(unitOfWork);
                            decimal valorValePedagio = servicoNDDCargo.ConsultarValorPedagio(cargaConsultaValePedagio, carga);

                            if (valorValePedagio > 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                                if (cargaPedidos.Count > 0)
                                    AtualizarValorNotasEPedagio(repositorioCargaPedido, servicoRateioFormula, valorValePedagio, cargaPedidos);

                                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware, true, true);
                            }

                            if (cargaConsultaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                            {
                                carga.ProblemaIntegracaoValePedagio = false;
                                carga.PossuiPendencia = false;
                                carga.MotivoPendencia = "";
                            }
                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Target)
                        {
                            Servicos.Embarcador.Integracao.Target.ValePedagio serValePedagioTarget = new(unitOfWork, tipoServicoMultisoftware);
                            decimal valorValePedagio = serValePedagioTarget.ConsultarValorPedagio(cargaConsultaValePedagio, carga);

                            if (valorValePedagio >= 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                                if (cargaPedidos.Count > 0)
                                    AtualizarValorNotasEPedagio(repositorioCargaPedido, servicoRateioFormula, valorValePedagio, cargaPedidos);

                                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware, true, true);
                            }

                            if (cargaConsultaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                            {
                                carga.ProblemaIntegracaoValePedagio = false;
                                carga.PossuiPendencia = false;
                                carga.MotivoPendencia = "";
                            }

                        }
                        else if (cargaConsultaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete)
                        {
                            Servicos.Embarcador.Integracao.EFrete.ValePedagio serValePedagioEFrete = new(unitOfWork);
                            decimal valorValePedagio = serValePedagioEFrete.ConsultarValorPedagio(cargaConsultaValePedagio, carga, tipoServicoMultisoftware);

                            if (valorValePedagio >= 0)
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                                if (cargaPedidos.Count > 0)
                                    AtualizarValorNotasEPedagio(repositorioCargaPedido, servicoRateioFormula, valorValePedagio, cargaPedidos);

                                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware, true, true);
                            }

                            if (cargaConsultaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
                            {
                                carga.ProblemaIntegracaoValePedagio = false;
                                carga.PossuiPendencia = false;
                                carga.MotivoPendencia = "";
                            }

                        }
                    }

                    if (cargaConsultaValePedagio.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao)
                        carga.IntegrandoValePedagio = false;

                    repositorioCarga.Atualizar(carga);

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCargaConsultaValorPedagioIntegracao);

                    unitOfWork.CommitChanges();
                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();

                    servicoOrquestradorFila.RegistroComFalha(codigoCargaConsultaValorPedagioIntegracao, e.Message);

                    cargaConsultaValePedagio.ProblemaIntegracao = "Falha genérica na consulta de valores do vale pedágio";
                    cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                    cargaConsultaValePedagio.NumeroTentativas++;

                    repositorioCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);

                    carga.ProblemaIntegracaoValePedagio = true;
                    repositorioCarga.Atualizar(carga);

                    Servicos.Log.TratarErro(e);
                }
            }
        }

        private async Task ExecutaGerarIntegracoesValePedagioAsync(int codigoCargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.GerarIntegracoesValePedagio);

                try
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repositorioIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
                    Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);

                    Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValepedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                    Servicos.Embarcador.Integracao.Target.ValePedagio servicoValePedagioTarget = new Servicos.Embarcador.Integracao.Target.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.Repom.ValePedagio servicoValePedagioRepom = new Servicos.Embarcador.Integracao.Repom.ValePedagio(unitOfWork);
                    Servicos.Embarcador.CIOT.Pagbem servicoPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                    Servicos.Embarcador.Integracao.DBTrans.ValePedagio servicoValePedagioDBTrans = new Servicos.Embarcador.Integracao.DBTrans.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.Pamcard.ValePedagio servicoValePedagioPamcard = new Servicos.Embarcador.Integracao.Pamcard.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.EFrete.ValePedagio servicoValePedagioEFrete = new Servicos.Embarcador.Integracao.EFrete.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.Extratta.ValePedagio servicoValePedagioExtratta = new Servicos.Embarcador.Integracao.Extratta.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.DigitalCom.ValePedagio servicoValePedagioDigitalCom = new Servicos.Embarcador.Integracao.DigitalCom.ValePedagio(unitOfWork, tipoServicoMultisoftware);
                    Servicos.Embarcador.Integracao.Ambipar.ValePedagio servicoValePedagioAmbipar = new Servicos.Embarcador.Integracao.Ambipar.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.NDDCargo.ValePedagio servicoValePedagioNDDCargo = new Servicos.Embarcador.Integracao.NDDCargo.ValePedagio(unitOfWork);

                    List<SituacaoValePedagio> listaSituacoesParaNotificacaoSuperApp = new() { SituacaoValePedagio.Comprada, SituacaoValePedagio.Recusada };

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = await repositorioCargaValePedagio.BuscarPorCodigoAsync(codigoCargaValePedagio, false);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaValePedagio.Carga;
                    Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(carga, tipoServicoMultisoftware);


                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (integracaoSemParar?.ConsultarValorPedagioParaRota ?? false))
                    {
                        cargaValePedagio.ProblemaIntegracao = "Está configurado para apenas consultar o valor do pedágio para a rota.";
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repositorioCargaValePedagio.Atualizar(cargaValePedagio);
                    }
                    else if (carga.Veiculo?.NaoComprarValePedagio ?? false)
                    {
                        cargaValePedagio.ProblemaIntegracao = "Veículo configurado para não comprar vale pedágio. Informação do Embarcador";
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaSemCusto;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;
                        repositorioCargaValePedagio.Atualizar(cargaValePedagio);
                    }
                    else
                    {
                        if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada &&
                               cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada &&
                               cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Encerrada &&
                               cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                            {
                                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = servicoValepedagioSemParar.Autenticar(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
                                if (credencial.Autenticado)
                                {
                                    if (cargaValePedagio.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar.RotaFixa || cargaValePedagio.RotaFrete != null)
                                        cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaGerada;

                                    if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaGerada)
                                        servicoValepedagioSemParar.GerarRotaTemporariaValePedagio(credencial, cargaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);

                                    if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaGerada)
                                        servicoValepedagioSemParar.GerarCompraValePedagio(credencial, cargaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);
                                }
                            }

                            if (cargaValePedagio.SituacaoValePedagio == SituacaoValePedagio.Comprada && string.IsNullOrEmpty(cargaValePedagio.CodigoEmissaoValePedagioANTT))
                            {
                                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = servicoValepedagioSemParar.Autenticar(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
                                if (credencial.Autenticado)
                                    servicoValepedagioSemParar.ConsultarIdVpo(credencial, cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
                            }
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Target)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada &&
                                  cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada &&
                                  cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Encerrada &&
                                  cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                            {
                                Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = servicoValePedagioTarget.Autenticar(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
                                if (autenticacao != null)
                                    servicoValePedagioTarget.GerarCompraValePedagio(autenticacao, cargaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);
                            }
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioRepom.GerarCompraValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PagBem)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoPagbem.GerarCompraValePedagio(cargaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioDBTrans.GerarCompraValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);

                            if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada && string.IsNullOrEmpty(cargaValePedagio.CodigoEmissaoValePedagioANTT))
                                servicoValePedagioDBTrans.ConsultarIdVpo(cargaValePedagio, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioPamcard.GerarCompraValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);

                            if (string.IsNullOrWhiteSpace(cargaValePedagio.CodigoEmissaoValePedagioANTT) && cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioPamcard.ConsultarIdVpoPedagio(cargaValePedagio, carga, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioEFrete.GerarCompraValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Extratta)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioExtratta.GerarCompraValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                await servicoValePedagioDigitalCom.GerarCompraValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ambipar)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioAmbipar.ComprarValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);
                        }
                        else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo)
                        {
                            if (cargaValePedagio.SituacaoValePedagio != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                                servicoValePedagioNDDCargo.ComprarValePedagio(cargaValePedagio, carga, tipoServicoMultisoftware);
                        }
                    }

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposManterIntegrandoValePedagio = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo
                    };

                    if (!tiposManterIntegrandoValePedagio.Contains(cargaValePedagio.TipoIntegracao.Tipo))
                        carga.IntegrandoValePedagio = false;

                    if (carga.AgImportacaoCTe && !repositorioCargaValePedagio.VerificarVPNaoCompradoPorCarga(carga.Codigo))
                        Servicos.Embarcador.Carga.PreCTe.VerificarSeLiberaCargaSemIntegrarCTes(carga);

                    repositorioCarga.Atualizar(carga);

                    try
                    {
                        //Devops: #3021 - Geração de Notificação SuperApp
                        if (cargaValePedagio.SituacaoIntegracao == SituacaoIntegracao.Integrado && listaSituacoesParaNotificacaoSuperApp.Contains(cargaValePedagio.SituacaoValePedagio))
                        {
                            TipoNotificacaoApp tipoNotificacaoApp = cargaValePedagio.SituacaoValePedagio == SituacaoValePedagio.Confirmada ? TipoNotificacaoApp.ValePedagioCompradoComSucesso : TipoNotificacaoApp.ValePedagioNaoComprado;
                            new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(unitOfWork).GerarIntegracaoNotificacao(carga, tipoNotificacaoApp);
                        }
                    }
                    catch (Exception ex) //Se deu erro, segue o baile.
                    {
                        Servicos.Log.TratarErro(ex, "NotificacaoSuperAppValePedagio");
                    }

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoCargaValePedagio);
                }
                catch (Exception e)
                {
                    servicoOrquestradorFila.RegistroComFalha(codigoCargaValePedagio, e.Message);

                    Servicos.Log.TratarErro(e);
                }
            }
        }

        private async Task ExecutaGerarIntegracoesCancelamentoValePedagioAsync(int codigoGerarIntegracoesCancelamentoValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

                    Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValepedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                    Servicos.Embarcador.Integracao.Target.ValePedagio servicoValepedagioTarget = new Servicos.Embarcador.Integracao.Target.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.Repom.ValePedagio servicoValepedagioRepom = new Servicos.Embarcador.Integracao.Repom.ValePedagio(unitOfWork);
                    Servicos.Embarcador.CIOT.Pagbem servicoPagBem = new Servicos.Embarcador.CIOT.Pagbem();
                    Servicos.Embarcador.Integracao.DBTrans.ValePedagio servicoValePedagioDBTrans = new Servicos.Embarcador.Integracao.DBTrans.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.Pamcard.ValePedagio servicoValePedagioPamcard = new Servicos.Embarcador.Integracao.Pamcard.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.EFrete.ValePedagio servicoValePedagioEFrete = new Servicos.Embarcador.Integracao.EFrete.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.Extratta.ValePedagio servicoValePedagioExtratta = new Servicos.Embarcador.Integracao.Extratta.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.DigitalCom.ValePedagio servicoValePedagioDigitalCom = new Servicos.Embarcador.Integracao.DigitalCom.ValePedagio(unitOfWork, tipoServicoMultisoftware);
                    Servicos.Embarcador.Integracao.Ambipar.ValePedagio servicoValePedagioAmbipar = new Servicos.Embarcador.Integracao.Ambipar.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Integracao.NDDCargo.ValePedagio servicoValePedagioNDDCargo = new Servicos.Embarcador.Integracao.NDDCargo.ValePedagio(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio = await repositorioCargaValePedagio.BuscarPorCodigoAsync(codigoGerarIntegracoesCancelamentoValePedagio, false);

                    if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = servicoValepedagioSemParar.Autenticar(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
                        if (credencial.Autenticado)
                            servicoValepedagioSemParar.SolicitarCancelamentoValePedagio(credencial, cargaValePedagio, cargaValePedagio.Carga, unitOfWork);
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Target)
                    {
                        Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = servicoValepedagioTarget.Autenticar(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
                        if (autenticacao != null)
                            servicoValepedagioTarget.SolicitarCancelamentoValePedagio(autenticacao, cargaValePedagio, cargaValePedagio.Carga, unitOfWork);
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom)
                        servicoValepedagioRepom.SolicitarCancelamentoValePedagio(cargaValePedagio, cargaValePedagio.Carga, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PagBem)
                        servicoPagBem.CancelarValePedagio(cargaValePedagio, unitOfWork);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans)
                        servicoValePedagioDBTrans.SolicitarCancelamentoValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard)
                        servicoValePedagioPamcard.SolicitarCancelamentoValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete)
                        servicoValePedagioEFrete.SolicitarCancelamentoValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Extratta)
                        servicoValePedagioExtratta.SolicitarCancelamentoValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom)
                        servicoValePedagioDigitalCom.SolicitarCancelamentoValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ambipar)
                        servicoValePedagioAmbipar.CancelarValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NDDCargo)
                        servicoValePedagioNDDCargo.CancelarValePedagio(cargaValePedagio, tipoServicoMultisoftware);
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "ExecutaGerarIntegracoesCancelamentoValePedagio");
            }
        }

        private async Task ExecutaVerificarRetornosValePedagioAsync(int codigoVerificarRetornosValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_unitOfWork.StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                Servicos.Global.OrquestradorFila servicoOrquestradorFila = new Servicos.Global.OrquestradorFila(unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IdentificadorControlePosicaoThread.VerificarRetornosValePedagio);

                try
                {
                    Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

                    Servicos.Embarcador.Integracao.SemParar.ValePedagio servicoValepedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
                    Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(unitOfWork);
                    Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

                    var cargaValePedagio = await repositorioCargaValePedagio.BuscarPorCodigoAsync(codigoVerificarRetornosValePedagio, false);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaValePedagio.Carga;

                    if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = servicoValepedagioSemParar.Autenticar(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                        if (credencial.Autenticado)
                            servicoValepedagioSemParar.ObterReciboCompraValePedagio(credencial, cargaValePedagio, carga, unitOfWork, tipoServicoMultisoftware);
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Target)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoTarget.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();

                            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoTarget.NotificarTransportadorPorEmail, tipoServicoMultisoftware);
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Repom)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRepom integracaoRepom = servicoValePedagio.ObterIntegracaoRepom(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoRepom.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PagBem)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPagbem integracaoPagBem = servicoValePedagio.ObterIntegracaoPagbem(carga.TipoOperacao, carga.Filial, carga.GrupoPessoaPrincipal, carga.FreteDeTerceiro, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoPagBem.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DBTrans)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDBTrans integracaoDBTrans = servicoValePedagio.ObterIntegracaoDBTrans(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoDBTrans.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Pamcard)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPamcard integracaoPamcard = servicoValePedagio.ObterIntegracaoPamcard(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoPamcard.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EFrete)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEFrete integracaoEFrete = servicoValePedagio.ObterIntegracaoEFrete(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoEFrete.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();

                            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoEFrete.NotificarTransportadorPorEmail, tipoServicoMultisoftware);
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Extratta)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio integracaoExtratta = servicoValePedagio.ObterIntegracaoExtratta(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoExtratta.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.DigitalCom)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDigitalComValePedagio integracaoDigitalCom = servicoValePedagio.ObterIntegracaoDigitalCom(carga, tipoServicoMultisoftware)?.IntegracaoDigitalComValePedagio;

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            Dominio.Entidades.Cliente fornecedor = !string.IsNullOrWhiteSpace(cargaValePedagio.CnpjMeioPagamento) ? repositorioCliente.BuscarPorCPFCNPJ(cargaValePedagio.CnpjMeioPagamento.ToDouble()) : integracaoDigitalCom?.FornecedorValePedagio;

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, fornecedor, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else if (cargaValePedagio.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ambipar)
                    {
                        if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada)
                        {
                            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio integracaoAmbipar = servicoValePedagio.ObterIntegracaoAmbipar(carga, tipoServicoMultisoftware);

                            unitOfWork.Start();

                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                            servicoValePedagio.AdicionarCargaValePedagioParaMDFe(cargaValePedagio, integracaoAmbipar.FornecedorValePedagio, unitOfWork);

                            Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);

                            unitOfWork.CommitChanges();
                        }
                    }

                    if (carga.AgImportacaoCTe && !repositorioCargaValePedagio.VerificarVPNaoCompradoPorCarga(carga.Codigo))
                    {
                        Servicos.Embarcador.Carga.PreCTe.VerificarSeLiberaCargaSemIntegrarCTes(carga);

                        if (!carga.AgImportacaoCTe)
                            await repositorioCarga.AtualizarAsync(carga);
                    }

                    servicoOrquestradorFila.RegistroLiberadoComSucesso(codigoVerificarRetornosValePedagio);
                }
                catch (Exception e)
                {
                    servicoOrquestradorFila.RegistroComFalha(codigoVerificarRetornosValePedagio, e.Message);

                    Servicos.Log.TratarErro(e, "ExecutaVerificarRetornosValePedagio");
                }
            }
        }

        private async Task WaitWithTimeoutAsync(Task task, TimeSpan timeout)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            using var cts = new CancellationTokenSource(timeout);

            var delayTask = Task.Delay(Timeout.Infinite, cts.Token);
            var completed = await Task.WhenAny(task, delayTask);

            if (completed == delayTask)
                throw new TimeoutException($"A operação excedeu o tempo limite de {timeout.TotalSeconds} segundos.");

            cts.Cancel();

            await task;
        }

        #endregion
    }
}
