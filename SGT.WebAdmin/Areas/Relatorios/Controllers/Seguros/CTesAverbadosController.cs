using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Seguros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Seguros/CTesAverbados")]
    public class CTesAverbadosController : BaseController
    {
		#region Construtores

		public CTesAverbadosController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R058_CTesAverbados;

        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasDescritivos = (decimal)5.50;
        private decimal TamanhoColunasData = 3;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CT-es Averbados", "Seguros", "CTesAverbados.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.CTesAverbados servicoRelatorioCTesAverbados = new Servicos.Embarcador.Relatorios.CTes.CTesAverbados(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCTesAverbados.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Seguros.CTesAverbados> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Número", "NumeroCTe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "Serie", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Data de Emissão", "DataEmissaoFormatada", TamanhoColunasData, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "StatusAverbacaoCTeFormatada", TamanhoColunasData, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Retorno", "MensagemRetorno", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Início da Prestação", "InicioPrestacao", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Fim da Prestação", "TerminoPrestacao", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Carga", "Carga", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo da Carga", "TipoCarga", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo da Operação", "TipoOperacao", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomadorFormatada", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Seguradora", "Seguradora", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Averbadora", "TipoSeguradoraAverbacaoFormatada", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Apólice", "Apolice", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Averbação", "NumeroAverbacao", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Averbação", "DataAverbacaoFormatada", TamanhoColunasData, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação Fechamento.", "StatusFechamentoFormatada", TamanhoColunasData, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Situação CT-e", "DescricaoStatusCTe", TamanhoColunasData, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do CT-e", "ValorCTe", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("% Desconto", "PercentualDesconto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Total Desconto", "Desconto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Container", "Container", TamanhoColunasData, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", TamanhoColunasData, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº OS", "NumeroOS", TamanhoColunasData, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Forma Averbação", "FormaAverbacaoFormatada", TamanhoColunasData, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cliente", "Cliente", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Provedor", "ClienteProvedorOS", TamanhoColunasDescritivos, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Mod. Doc.", "ModeloDocumentoFiscal", TamanhoColunasData, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Data Serviço", "DataServicoFormatada", TamanhoColunasData, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "CNPJ Empresa" : "CNPJ Transportador", "CNPJTransportadorFormatada", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa" : "Transportador", "Transportador", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilial", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunasDescritivos, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Seguro.FiltroPesquisaRelatorioCTesAverbados()
            {
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                DataServicoInicial = Request.GetDateTimeParam("DataServicoInicial"),
                DataServicoFinal = Request.GetDateTimeParam("DataServicoFinal"),
                CodigoClienteProvedorOS = Request.GetDoubleParam("ClienteProvedorOS"),
                Status = Request.GetEnumParam<Dominio.Enumeradores.StatusAverbacaoCTe>("Status"),
                SituacaoFechamento = Request.GetEnumParam<SituacaoAverbacaoFechamento>("SituacaoFechamento"),
                CodigoSeguradora = Request.GetIntParam("Seguradora"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                GrupoTomador = Request.GetListParam<int>("GrupoTomador"),
                TipoPropriedadeVeiculo = Request.GetStringParam("TipoPropriedadeVeiculo"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)

        };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork);
                    List<int> codigosEmpresa = empresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportador = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigosTransportador = new List<int>() { Usuario.Empresa.Codigo };
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}
