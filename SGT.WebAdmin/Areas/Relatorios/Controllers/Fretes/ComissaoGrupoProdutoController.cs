using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/ComissaoGrupoProduto")]
    public class ComissaoGrupoProdutoController : BaseController
    {
		#region Construtores

		public ComissaoGrupoProdutoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R025_ComissaoGrupoProduto;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Contrato de Frete", "ContratoFrete", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Inicial do Contrato", "DataInicialContratoFrete", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Final do Contrato", "DataFinalContratoFrete", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Produtos", "GrupoProdutos", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("% Comissão", "PercentualComissao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Comissão por Grupo de Produtos", "Fretes", "ComissaoGrupoProduto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "GrupoProdutos", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                int codigoTransportador, codigoContratoFreteTransportador, codigoGrupoPessoas, codigoGrupoProdutos, codigoProduto;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("GrupoProduto"), out codigoGrupoProdutos);
                int.TryParse(Request.Params("Produto"), out codigoProduto);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unidadeDeTrabalho, cancellationToken);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ComissaoGrupoProduto> listaComissaoGrupoProduto = await repTabelaFreteComissaoGrupoProduto.ConsultarRelatorioComissaoGrupoProdutoAsync(agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(listaComissaoGrupoProduto.Count);

                grid.AdicionaRows(listaComissaoGrupoProduto);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTransportador, codigoContratoFreteTransportador, codigoGrupoPessoas, codigoGrupoProdutos, codigoProduto;
                int.TryParse(Request.Params("Transportador"), out codigoTransportador);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out codigoContratoFreteTransportador);
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);
                int.TryParse(Request.Params("GrupoProduto"), out codigoGrupoProdutos);
                int.TryParse(Request.Params("Produto"), out codigoProduto);

                double cpfCnpjPessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out cpfCnpjPessoa);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioComissaoGrupoProduto(agrupamentos, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioComissaoGrupoProduto(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            int codigoTransportador,
            int codigoContratoFreteTransportador,
            int codigoGrupoProdutos,
            int codigoProduto,
            int codigoGrupoPessoas,
            double cpfCnpjPessoa,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                //Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork, cancellationToken);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Produtos.GrupoProduto repGrupoProdutos = new Repositorio.Embarcador.Produtos.GrupoProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto repTabelaFreteComissaoGrupoProduto = new Repositorio.Embarcador.Frete.TabelaFreteComissaoGrupoProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ComissaoGrupoProduto> listaComissaoGrupoProduto = await repTabelaFreteComissaoGrupoProduto.ConsultarRelatorioComissaoGrupoProdutoAsync(propriedades, codigoTransportador, codigoContratoFreteTransportador, codigoGrupoProdutos, codigoProduto, codigoGrupoPessoas, cpfCnpjPessoa, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);


                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigoTransportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(codigoTransportador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigoContratoFreteTransportador > 0)
                {
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = await repContratoFreteTransportador.BuscarPorCodigoAsync(codigoContratoFreteTransportador, auditavel: false);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContratoFreteTransportador", contrato.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ContratoFreteTransportador", false));

                if (codigoGrupoProdutos > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProduto grupoProdutos = await repGrupoProdutos.BuscarPorCodigoAsync(codigoGrupoProdutos, auditavel: false);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProdutos", grupoProdutos.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProdutos", false));

                if (codigoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repProdutoEmbarcador.BuscarPorCodigo(codigoProduto);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", produto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

                if (cpfCnpjPessoa > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = await repCliente.BuscarPorCPFCNPJAsync(cpfCnpjPessoa);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = await repGrupoPessoas.BuscarPorCodigoAsync(codigoGrupoPessoas);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Fretes/ComissaoGrupoProduto", parametros,relatorioControleGeracao, relatorioTemp, listaComissaoGrupoProduto, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}

