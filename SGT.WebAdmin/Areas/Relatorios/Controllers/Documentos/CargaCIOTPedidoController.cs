using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Documentos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Documentos/CargaCIOTPedido")]
    public class CargaCIOTPedidoController : BaseController
    {
		#region Construtores

		public CargaCIOTPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R279_CargaCIOTPedido;

        private decimal _tamanhoColunaPequena = 1.75m;
        private decimal _tamanhoColunaGrande = 5.50m;
        private decimal _tamanhoColunaMedia = 3m;
        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                //int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);
                int codigoRelatorio = Request.GetIntParam("Codigo");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Carga CIOT por Pedido", "Documentos", "CargaCIOTPedido.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Numero", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.CargaCIOTPedido servicoRelatorioCargaCIOTPedido = new Servicos.Embarcador.Relatorios.Carga.CargaCIOTPedido(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCargaCIOTPedido.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Documentos.CargaCIOTPedido> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido filtrosPesquisa = ObterFiltrosPesquisa();

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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {


            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", _tamanhoColunaMedia, Models.Grid.Align.center);
            grid.AdicionarCabecalho("Proprietário", "ProprietarioFormatado", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "EmpresaFormatado", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Motorista/Fretista", "MotoristaFormatado", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Nascimento", "MotoristaDataNascimentoFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("PIS/PASEP", "MotoristaPisPasep", _tamanhoColunaMedia, Models.Grid.Align.center, false,  true);
            grid.AdicionarCabecalho("CBO", "MotoristaCBO", _tamanhoColunaMedia, Models.Grid.Align.center, false,  false);
            grid.AdicionarCabecalho("Carga", "Carga", _tamanhoColunaMedia, Models.Grid.Align.center, false,  true);
            grid.AdicionarCabecalho("Pedido", "PedidoFormatado", _tamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Data Carga", "DataCargaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false,  false);
            grid.AdicionarCabecalho("Data Pagto. Adiantamento", "DataPagamentoAdiantamentoFreteFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false,  false);
            grid.AdicionarCabecalho("Data Pagto. Saldo", "DataPagamentoSaldoFreteFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, false,  false);
            grid.AdicionarCabecalho("Peso Bruto", "PesoBruto", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ false);
            grid.AdicionarCabecalho("Valor Mercadoria KG", "ValorMercadoriaKG", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Valor Total Mercadoria", "ValorTotalMercadoria", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Tarifa Frete", "ValorTarifaFrete", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Frete", "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Perc. Tolerância", "PercentualTolerancia", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Perc. Tolerância Superior", "PercentualToleranciaSuperior", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Adiantamento", "ValorAdiantamento", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Seguro", "ValorSeguro", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Pedágio", "ValorPedagio", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Base IRRF", "BaseCalculoIRRF", _tamanhoColunaMedia, Models.Grid.Align.right, false,  true);
            grid.AdicionarCabecalho("Aliquota IRRF", "AliquotaIRRF", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("IRRF", "ValorIRRF", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Base INSS/SEST/SENAT", "BaseCalculoINSS", _tamanhoColunaMedia, Models.Grid.Align.right, false,  true);
            grid.AdicionarCabecalho("INSS", "ValorINSS", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("SEST", "ValorSEST", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("SENAT", "ValorSENAT", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Outros Descontos", "ValorOutrosDescontos", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);

            grid.AdicionarCabecalho("Base IRRF Sem Acumulo", "BaseCalculoIRRFSemAcumulo", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Base IRRF Sem Desc. Dependente", "BaseCalculoIRRFSemDesconto", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor IRRF Sem Desc. Dependente", "ValorIRRFSemDesconto", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor por Dependente", "ValorPorDependente", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Quantidades Dependentes", "QuantidadeDependentes", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor total Dependentes", "ValorTotalDependentes", _tamanhoColunaMedia, Models.Grid.Align.right, false, true);

            grid.AdicionarCabecalho("Saldo", "Saldo", _tamanhoColunaPequena, Models.Grid.Align.right, false, /*Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum,*/ true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", _tamanhoColunaMedia, Models.Grid.Align.center, false,  false);
            grid.AdicionarCabecalho("Mensagem", "MensagemCIOT", _tamanhoColunaGrande, Models.Grid.Align.left, false,  false);
            grid.AdicionarCabecalho("Protocolo Autorização", "ProtocoloAutorizacao", _tamanhoColunaGrande, Models.Grid.Align.left, false,  false);
            grid.AdicionarCabecalho("Véiculo Tração", "VeiculoTracao", _tamanhoColunaGrande, Models.Grid.Align.left, false,  false);
            grid.AdicionarCabecalho("Véiculos Reboques", "VeiculosReboques", _tamanhoColunaGrande, Models.Grid.Align.left, false,  false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", _tamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cidade Destino", "Destino", _tamanhoColunaMedia, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaCIOTPedido()
            {
                Carga = Request.GetNullableStringParam("Carga"),
                DataEncerramentoInicial = Request.GetDateTimeParam("DataEncerramentoInicial"),
                DataEncerramentoFinal = Request.GetDateTimeParam("DataEncerramentoFinal"),
                Numero = Request.GetNullableStringParam("Numero"),
                Proprietario = Request.GetDoubleParam("Proprietario"),
                DataAberturaInicial = Request.GetDateTimeParam("DataAberturaInicial"),
                DataAberturaFinal = Request.GetDateTimeParam("DataAberturaFinal"),
                Veiculo = Request.GetIntParam("Veiculo"),
                Motorista = Request.GetIntParam("Motorista"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT>("Situacao"),
                Transportador = Request.GetIntParam("Transportador"),
            };
        }

        #endregion

    }
}
