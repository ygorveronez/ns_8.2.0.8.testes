using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/DespesaOrdemServico")]
    public class DespesaOrdemServicoController : BaseController
    {
		#region Construtores

		public DespesaOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R152_DespesaOrdemServico;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken  cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Despesa por Ordem de Serviço", "Frotas", "DespesaOrdemServico.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Modelo", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.DespesaOrdemServico servicolatorioDespesaOrdemServico = new Servicos.Embarcador.Relatorios.Frotas.DespesaOrdemServico(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicolatorioDespesaOrdemServico.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaOrdemServico> listaExtratoConta, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaExtratoConta);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
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

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioDespesaOrdemServico()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroOS = Request.GetIntParam("NumeroOS"),
                MarcaVeiculo = Request.GetIntParam("MarcaVeiculo"),
                ModeloVeiculo = Request.GetListParam<int>("ModeloVeiculo"),
                Veiculo = Request.GetIntParam("Veiculo"),
                TipoOrdemServico = Request.GetListParam<long>("Tipo"),
                LocalManutencao = Request.GetDoubleParam("LocalManutencao"),
                Situacoes = Request.GetListEnumParam<SituacaoOrdemServicoFrota>("Situacao"),
                Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Usuario.Empresa.Codigo : 0,
                Servico = Request.GetListParam<int>("Servico"),
                Produto = Request.GetListParam<int>("Produto"),
                GrupoProduto= Request.GetListParam<int>("GrupoProduto"),
                Equipamento = Request.GetListParam<int>("Equipamento"),
            };
        }       

        private Models.Grid.Grid GridPadrao()
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Placa").Nome("Placa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Modelo").Nome("Modelo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Marca").Nome("Marca").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Ano").Nome("Ano do Veículo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("LocalManutencao").Nome("Local Manutenção").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(false, true).Visibilidade(false);
            grid.Prop("DataFormatada").Nome("Data").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("MesAnoOS").Nome("Mês e ano OS").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("Numero").Nome("Número OS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("ValorProdutos").Nome("Vlr. Produtos Orçado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorServicos").Nome("Vlr. Serviços Orçado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorTotal").Nome("Vlr. Total Orçado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("Servico").Nome("Serviços").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Produto").Nome("Produtos").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("GrupoProduto").Nome("Grupo de Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ValorProdutosFechamento").Nome("Vlr. Produtos Realizado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorServicosFechamento").Nome("Vlr. Serviços Realizado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("ValorTotalFechamento").Nome("Vlr. Total Realizado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("Tipo").Nome("Tipo").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("DescricaoSituacao").Nome("Situação").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(false, true).Visibilidade(false);
            grid.Prop("Equipamento").Nome("Equipamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);

            return grid;
        }

        #endregion
    }
}
