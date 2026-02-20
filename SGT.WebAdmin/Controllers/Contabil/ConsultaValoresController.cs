using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    public class ConsultaValoresController : BaseController
    {
        #region Construtores

        public ConsultaValoresController(Conexao conexao) : base(conexao) { }

        #endregion

        public async Task<IActionResult> BuscarDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Contabeis.ConsultarValores repConsultarValores = new Repositorio.Embarcador.Contabeis.ConsultarValores(unitOfWork);

                unitOfWork.Start();

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                var dynRetorno = new
                {
                    CTesEmitidos = repConsultarValores.ConsultaValores(dataInicial, dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesEmitidos).FirstOrDefault().Valor.ToString("n2"),
                    CTesCancelados = repConsultarValores.ConsultaValores(dataInicial, dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesCancelados).FirstOrDefault().Valor.ToString("n2"),
                    CTesAnulados = repConsultarValores.ConsultaValores(dataInicial, dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesAnulados).FirstOrDefault().Valor.ToString("n2"),
                    CTesFaturados = repConsultarValores.ConsultaValores(dataInicial, dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.CTesFatudados).FirstOrDefault().Valor.ToString("n2"),
                    PosicaoDoDia = repConsultarValores.ConsultaValores(dataInicial, dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultarValores.PosicaoCTes).FirstOrDefault().Valor.ToString("n2"),
                };

                unitOfWork.CommitChanges();
                return new JsonpResult(dynRetorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar valores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterResumoContasAReceber()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Contabeis.ConsultarValores repConsultarValores = new Repositorio.Embarcador.Contabeis.ConsultarValores(unitOfWork);

                decimal valorCTesSemCarga = repConsultarValores.ObterValorCTesEmitidosSemCarga();
                decimal valorCTesNaoFaturados = repConsultarValores.ObterValorCTesNaoFaturados();
                decimal valorTitulosEmAberto = repConsultarValores.ObterValorTitulosAReceberEmAberto();
                decimal valorOutrosTitulosEmAberto = repConsultarValores.ObterValorTitulosAReceberEmAbertoOutros();
                decimal valorFaturaEmAberto = repConsultarValores.ObterValorFaturaEmAberto();

                var retorno = new
                {
                    ValorCTesSemCarga = valorCTesSemCarga.ToString("n2"),
                    CTesNaoFaturados = valorCTesNaoFaturados.ToString("n2"),
                    ValorTitulosEmAberto = valorTitulosEmAberto.ToString("n2"),
                    ValorFaturaEmAberto = valorFaturaEmAberto.ToString("n2"),
                    ValorOutrosTitulosEmAberto = valorOutrosTitulosEmAberto.ToString("n2"),
                    ValorTotal = (valorCTesSemCarga + valorCTesNaoFaturados + valorTitulosEmAberto + valorFaturaEmAberto + valorOutrosTitulosEmAberto).ToString("n2")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar valores.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadRelatorioContasAReceberAnalitico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(false, false, "Funcionalidade desabilitada. Entre em contato com o suporte técnico.");

                //Repositorio.Embarcador.Contabeis.ConsultarValores repConsultarValores = new Repositorio.Embarcador.Contabeis.ConsultarValores(unidadeTrabalho);

                //List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber> listaContasAReceber = repConsultarValores.ConsultarRelatorioAnaliticoContasAReceber();

                //Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet dataSet = new Dominio.Relatorios.Embarcador.ObjetosDeValor.ReportDataSet()
                //{
                //    DataSet = listaContasAReceber,
                //};

                //byte[] relatorio = Servicos.Embarcador.Relatorios.RelatorioSemPadrao.GerarRelatorio(@"Areas\Relatorios\Reports\Default\Financeiros\ResumoContasAReceberAnalitico.rpt", Dominio.Enumeradores.TipoArquivoRelatorio.XLS, dataSet, false);

                //return Arquivo(relatorio, "plain/text", "Contas a Receber Analítico.xls");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadRelatorioPosicaoContasAReceberAnalitico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, "Posicao Contas a Receber");

                Utilidades.File.RemoverArquivos(caminhoArquivos);

                caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, System.IO.Path.GetRandomFileName());

                DateTime.TryParseExact(Request.Params("DataPosicao"), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataPosicao);

                bool utilizarDataBaseLiquidacaoTitulos = Request.GetBoolParam("UtilizarDataBaseLiquidacaoTitulos");

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Iniciou a consulta do posição de contas a receber");

                Repositorio.Embarcador.Contabeis.ConsultarValores repConsultarValores = new Repositorio.Embarcador.Contabeis.ConsultarValores(unidadeTrabalho);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber> listaContasAReceber = repConsultarValores.ConsultarRelatorioAnaliticoPosicaoContasAReceber(dataPosicao, utilizarDataBaseLiquidacaoTitulos);

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Iniciou a geração do arquivo do posição de contas a receber");

                Utilidades.CSV.GerarCSV<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAReceber>(listaContasAReceber, caminhoArquivos);

                listaContasAReceber = null;
                GC.Collect();

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Terminou a geração do arquivo do posição de contas a receber");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivos), "plain/text", "Posição do Contas a Receber Analítico - " + dataPosicao.ToString("dd-MM-yyyy") + ".csv");
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
