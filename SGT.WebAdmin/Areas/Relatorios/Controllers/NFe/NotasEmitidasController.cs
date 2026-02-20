using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NFe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/NFe/NotasEmitidas")]
    public class NotasEmitidasController : BaseController
    {
		#region Construtores

		public NotasEmitidasController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R018_NotasEmitidas;

        #region Métodos Públicos

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeTrabalho);
                List<int> listaCodigosNfes = repNFe.BuscarListaCodigosNFes(filtrosPesquisa);

                if (listaCodigosNfes.Count > 500)
                    return new JsonpResult(false, true, "Quantidade de NF-es para geração de lote inválida (" + listaCodigosNfes.Count + "). É permitido o download de um lote com o máximo de 500 NF-es.");

                Servicos.NFe svcNFe = new Servicos.NFe(unidadeTrabalho);

                return Arquivo(svcNFe.ObterLoteDeXML(listaCodigosNfes, filtrosPesquisa.CodigoEmpresa, unidadeTrabalho), "application/zip", "LoteXML.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Notas Emitidas", "NFe", "NotasEmitidas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NFe.NotasEmitidas servicoRelatorioNotasEmitidas = new Servicos.Embarcador.Relatorios.NFe.NotasEmitidas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioNotasEmitidas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NFe.NotasEmitidas> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Número", "Numero", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Série", "Serie", 2, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 4, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Data Saída", "DataSaidaFormatada", 4, Models.Grid.Align.center, true, true);

            grid.AdicionarCabecalho("CPF/CNPJ Pessoa", "CpfCnpjPessoaFormatado", 5, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 10, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("Status", "DescricaoStatus", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Natureza da Operação", "DescricaoNaturezaOperacao", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Finalidade", "DescricaoFinalidade", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Emissão", "DescricaoTipoEmissao", 3, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Chave", "Chave", 5, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Atividade", "DescricaoAtividade", 5, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Total", "ValorTotal", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Valor dos Produtos", "ValorTotalProdutos", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Valor dos Serviços", "ValorTotalServicos", 3, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Tipo Frete", "DescricaoTipoFrete", 3, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Peso Bruto", "PesoBruto", 3, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Peso Liquido", "PesoLiquido", 3, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Observação Tributária", "ObservacaoTributaria", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("CFOP", "CFOP", 3, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioNotasEmitidas()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                Serie = Request.GetIntParam("Serie"),
                CodigoAtividade = Request.GetIntParam("Atividade"),
                CodigoNaturezaOperacao = Request.GetIntParam("NaturezaOperacao"),
                FormaEmissao = Request.GetIntParam("NotaImportada"),
                CodigosUsuario = Request.GetListParam<int>("Usuario"),
                CnpjPessoa = Request.GetDoubleParam("Pessoa"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataProcessamento = Request.GetDateTimeParam("DataProcessamento"),
                DataSaida = Request.GetDateTimeParam("DataSaida"),
                Status = Request.GetEnumParam<Dominio.Enumeradores.StatusNFe>("Status"),
                TipoEmissao = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoEmissaoNFe>("TipoEmissao"),
                TipoDocumento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNota>("TipoDocumento"),
                Chave = Request.GetStringParam("Chave"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario?.Empresa?.Codigo ?? 0 : 0,
                TipoAmbiente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario?.Empresa?.TipoAmbiente ?? Dominio.Enumeradores.TipoAmbiente.Nenhum : Dominio.Enumeradores.TipoAmbiente.Nenhum,
                ExibirItens = Request.GetBoolParam("ExibirItens"),
                CodigosCFOP = Request.GetListParam<int>("CFOP")
            };
        }

        #endregion
    }
}
