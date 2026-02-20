using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/RelatoriosBem")]
    public class RelatoriosBemController : BaseController
    {
		#region Construtores

		public RelatoriosBemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarRelatorioTermoResponsabilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int.TryParse(Request.Params("CodigoBem"), out int codigoBem);
                int.TryParse(Request.Params("CodigoTransferencia"), out int codigoTransferencia);
                
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R167_TermoResponsabilidade, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R167_TermoResponsabilidade, TipoServicoMultisoftware, "Relatório de Termo de Responsabilidade", "Patrimonio", "TermoResponsabilidade.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade> dadosBem = repBem.RelatorioTermoResponsabilidade(codigoBem, codigoTransferencia);
                if (dadosBem.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioTermoResponsabilidade(codigoBem, codigoTransferencia, dadosBem, stringConexao, relatorioControleGeracao));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de bens para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarRelatorioTermoRecolhimentoMaterial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int.TryParse(Request.Params("CodigoTransferencia"), out int codigoTransferencia);
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R165_TermoRecolhimentoMaterial, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R165_TermoRecolhimentoMaterial, TipoServicoMultisoftware, "Relatório de Termo de Recolhimento de Material", "Patrimonio", "TermoRecolhimentoMaterial.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoRecolhimentoMaterial> dadosBem = repBem.RelatorioTermoRecolhimentoMaterial(codigoTransferencia);
                if (dadosBem.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioTermoRecolhimentoMaterial(codigoTransferencia, dadosBem, stringConexao, relatorioControleGeracao));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de bens para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BaixarRelatorioTermoBaixaMaterial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int.TryParse(Request.Params("CodigoBaixa"), out int codigoBaixa);
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R166_TermoBaixaMaterial, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R166_TermoBaixaMaterial, TipoServicoMultisoftware, "Relatório de Termo de Baixa de Material", "Patrimonio", "TermoBaixaMaterial.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial> dadosBem = repBem.RelatorioTermoBaixaMaterial(codigoBaixa);
                if (dadosBem.Count > 0)
                {
                    Task.Factory.StartNew(() => GerarRelatorioTermoBaixaMaterial(codigoBaixa, dadosBem, stringConexao, relatorioControleGeracao));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de bens para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorioTermoResponsabilidade(int codigoBem, int codigoTransferencia, IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade> dadosBem, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            ReportRequest.WithType(ReportType.TermoResponsabilidade)
                .WithExecutionType(ExecutionType.Async)
                .AddExtraData("CodigoTransferencia", codigoTransferencia)
                .AddExtraData("DadosBem", dadosBem.ToJson())
                .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                .CallReport();
        }

        private void GerarRelatorioTermoRecolhimentoMaterial(int codigoTransferencia, IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoRecolhimentoMaterial> dadosBem, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                ReportRequest.WithType(ReportType.TermoRecolhimentoMaterial)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DadosBem", dadosBem.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void GerarRelatorioTermoBaixaMaterial(int codigoTransferencia, IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial> dadosBem, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork); ;
            try
            {
                ReportRequest.WithType(ReportType.TermoBaixaMaterial)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DadosBem", dadosBem.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
