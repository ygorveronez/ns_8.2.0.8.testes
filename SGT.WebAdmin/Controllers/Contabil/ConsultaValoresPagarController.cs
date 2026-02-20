using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize("Contabils/ConsultaValoresPagar")]
    public class ConsultaValoresPagarController : BaseController
    {
        #region Construtores

        public ConsultaValoresPagarController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadRelatorioPosicaoContasAPagarAnalitico()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos, "Posicao Contas a Pagar");

                Utilidades.File.RemoverArquivos(caminhoArquivos);

                caminhoArquivos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, System.IO.Path.GetRandomFileName());

                DateTime.TryParseExact(Request.Params("DataPosicao"), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataPosicao);

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Iniciou a consulta do posição de contas a pagar");

                Repositorio.Embarcador.Contabeis.ConsultarValores repConsultarValores = new Repositorio.Embarcador.Contabeis.ConsultarValores(unidadeTrabalho);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAPagar> listaResumoContasAPagar = repConsultarValores.ConsultarRelatorioAnaliticoPosicaoContasAPagar(dataPosicao);

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Iniciou a geração do arquivo do posição de contas a pagar");

                Utilidades.CSV.GerarCSV<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ResumoContasAPagar>(listaResumoContasAPagar, caminhoArquivos);

                listaResumoContasAPagar = null;
                GC.Collect();

                Servicos.Log.TratarErro($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")} - Terminou a geração do arquivo do posição de contas a pagar");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivos), "plain/text", "Posição do Contas a Pagar Analítico - " + dataPosicao.ToString("dd-MM-yyyy") + ".csv");
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
