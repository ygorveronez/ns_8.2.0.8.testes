using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	public class TituloAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public TituloAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R093_TituloAcrescimoDesconto;

        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Acréscimos e Descontos em Títulos", "Financeiros", "TituloAcrescimoDesconto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.TituloAcrescimoDesconto servicoRelatorioTituloAcrescimoDesconto = new Servicos.Embarcador.Relatorios.Financeiros.TituloAcrescimoDesconto(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioTituloAcrescimoDesconto.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloAcrescimoDesconto> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Codigo").Visibilidade(false);
            grid.Prop("Titulo").Nome("Título").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("DataEmissao").Nome("Data de Emissão").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("DataLiquidacao").Nome("Data de Liquidação").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("DataBaseLiquidacao").Nome("Data Base Liquidação").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("ValorTitulo").Nome("Valor do Título").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(false).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("TipoDocumento").Nome("Tipo do Documento").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("ModeloDocumento").Nome("Modelo do Documento").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("NumeroDocumento").Nome("Número do Documento").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("DataEmissaoDocumentos").Nome("Data Emissão Doc.").Tamanho(TamanhoColunaPequena).Agr(true).Visibilidade(false);
            grid.Prop("ValorDocumento").Nome("Valor do Documento").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(false).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("Empresa").Nome("Empresa/Filial").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("CPFCNPJPessoaFormatado").Nome("CPF/CNPJ Pessoa").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("Pessoa").Nome("Pessoa").Tamanho(TamanhoColunaGrande).Agr(true);
            grid.Prop("GrupoPessoas").Nome("Grupo de Pessoas").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("SituacaoTitulo").Nome("Situação do Título").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("NumeroFatura").Nome("Fatura").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("NumeroBordero").Nome("Borderô").Tamanho(TamanhoColunaPequena).Visibilidade(false).Agr(true);
            grid.Prop("Justificativa").Nome("Justificativa").Tamanho(TamanhoColunaGrande).Agr(true);
            grid.Prop("Observacao").Nome("Observação").Tamanho(TamanhoColunaGrande);
            grid.Prop("Valor").Nome("Valor").Tamanho(TamanhoColunaPequena).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("TipoJustificativa").Nome("Tipo").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("Tipo").Nome("Aplicado Em").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("Usuario").Nome("Operador/Usuário Desconto/Acréscimo").Tamanho(TamanhoColunaMedia).Visibilidade(false).Agr(true);
            grid.Prop("DataAplicacaoFormatada").Nome("Data Desconto/Acréscimo").Tamanho(TamanhoColunaMedia).Visibilidade(false).Agr(true);
            grid.Prop("CPFMotoristaFormatado").Nome("CPF do Motorista").Tamanho(TamanhoColunaMedia).Visibilidade(false).Agr(false);
            grid.Prop("NomeMotorista").Nome("Nome do Motorista").Tamanho(TamanhoColunaMedia).Visibilidade(false).Agr(false);
            grid.Prop("CodigoIntegracaoMotorista").Nome("Cod. Integração do Motorista").Tamanho(TamanhoColunaMedia).Visibilidade(false).Agr(false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTituloAcrescimoDesconto()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataLiquidacaoInicial = Request.GetDateTimeParam("DataLiquidacaoInicial"),
                DataLiquidacaoFinal = Request.GetDateTimeParam("DataLiquidacaoFinal"),
                DataBaseLiquidacaoInicial = Request.GetDateTimeParam("DataBaseLiquidacaoInicial"),
                DataBaseLiquidacaoFinal = Request.GetDateTimeParam("DataBaseLiquidacaoFinal"),
                CodigoCTe = Request.GetIntParam("CTe"),
                CodigoFatura = Request.GetIntParam("Fatura"),
                CodigoBordero = Request.GetIntParam("Bordero"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigosJustificativa = Request.GetListParam<int>("Justificativa"),
                CpfCnpjPessoa = Request.GetDoubleParam("Pessoa"),
                SituacaoTitulo = Request.GetEnumParam<StatusTitulo>("SituacaoTitulo"),
                TipoJustificativa = Request.GetNullableEnumParam<TipoJustificativa>("TipoJustificativa"),
                Tipo = Request.GetNullableEnumParam<EnumTipoAcrescimoDescontoTituloDocumento>("Tipo"),
                TipoDeTitulo = Request.GetEnumParam<TipoTitulo>("TipoDeTitulo")
            };
        }

        #endregion
    }
}
