using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fatura
{
	[Area("Relatorios")]
	[CustomAuthorize("Faturas/Fatura")]
    public class FaturaRelatorioController : BaseController
    {
		#region Construtores

		public FaturaRelatorioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoFatura = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repositorioFatura.BuscarPorCodigo(codigoFatura);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Servicos.Embarcador.Fatura.FaturaImpressao servicoFaturaImpressao = Servicos.Embarcador.Fatura.FaturaImpressaoFactory.Criar(fatura, unitOfWork, TipoServicoMultisoftware);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoFaturaImpressao.ObterRelatorio();

                if (fatura.Carga?.TipoOperacao?.ConfiguracaoImpressao?.AlterarLayoutDaFaturaIncluirTipoServico ?? false)
                    relatorio.Titulo += " - " + fatura.Carga.TipoOperacao.TipoPropostaMultimodal.ObterDescricao();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork, codigoFatura);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoFaturaImpressao.ObterRelatorioTemporario(relatorio);
                string stringConexao = _conexao.StringConexao;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura = ConfiguracaoEmbarcador.TipoImpressaoFatura;

                _ = Task.Factory.StartNew(() => GerarRelatorioFatura(fatura, relatorioControleGeracao, relatorioTemporario, stringConexao, TipoServicoMultisoftware, tipoImpressaoFatura));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioFatura(Dominio.Entidades.Embarcador.Fatura.Fatura fatura, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImpressaoFatura? tipoImpressaoFatura)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao))
            {
                Servicos.Embarcador.Fatura.FaturaImpressao servicoFaturaImpressao = Servicos.Embarcador.Fatura.FaturaImpressaoFactory.Criar(fatura, unitOfWork, TipoServicoMultisoftware);

                servicoFaturaImpressao.GerarRelatorio(fatura, relatorioControleGeracao, relatorioTemporario, tipoImpressaoFatura);
            }
        }

        #endregion
    }
}
