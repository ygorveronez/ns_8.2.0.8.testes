using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Patrimonio
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Patrimonio/Bem")]
    public class BemController : BaseController
    {
		#region Construtores

		public BemController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R168_Bem;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo").Nome("Código").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Descricao").Nome("Descrição").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("NumeroSerie").Nome("Número Série").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("GrupoProduto").Nome("Grupo Produto").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("CentroResultado").Nome("Centro Resultado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Almoxarifado").Nome("Almoxarifado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);

            grid.Prop("ValorBem").Nome("Valor Patrimônio").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("PercentualDepreciacao").Nome("% Depreciação").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("DepreciacaoAcumulada").Nome("Depreciação Acumulada").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.Prop("DataAquisicaoFormatada").Nome("Data Aquisição").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataAlocadoFormatada").Nome("Data Alocação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("FuncionarioAlocado").Nome("Funcionário Alocado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("DataGarantiaFormatada").Nome("Data Garantia").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("DataEntregaFormatada").Nome("Data Entrega").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("ValorOrcado").Nome("Valor Orçado").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("ValorPago").Nome("Valor Pago").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Defeito").Nome("Defeito").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("Observacao").Nome("Observação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Patrimônios", "Patrimonio", "Bem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Descricao", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.Bem> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                propOrdena = ObterPropriedadeOrdenar(propOrdena);
                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(listaReport);

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
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, cancellationToken);

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

        private async Task GerarRelatorioAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                string propOrdena = ObterPropriedadeOrdenar(relatorioTemp.PropriedadeOrdena);
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.Bem> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Patrimonio/Bem",parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.Bem> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

            DateTime.TryParseExact(Request.Params("DataAquisicaoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAquisicaoInicial);
            DateTime.TryParseExact(Request.Params("DataAquisicaoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAquisicaoFinal);
            DateTime.TryParseExact(Request.Params("DataAlocadoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAlocadoInicial);
            DateTime.TryParseExact(Request.Params("DataAlocadoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataAlocadoFinal);

            int.TryParse(Request.Params("Bem"), out int codigoBem);
            int.TryParse(Request.Params("GrupoProduto"), out int codigoGrupoProduto);
            int.TryParse(Request.Params("Almoxarifado"), out int codigoAlmoxarifado);
            int.TryParse(Request.Params("CentroResultado"), out int codigoCentroResultado);
            int.TryParse(Request.Params("FuncionarioAlocado"), out int codigoFuncionarioAlocado);
            int codigoDefeito = Request.GetIntParam("MotivoDefeito");
            double cpfCnpjPessoa = Request.GetDoubleParam("Pessoa");
            DateTime dataEntrega = Request.GetDateTimeParam("DataEntrega");
            DateTime dataRetorno = Request.GetDateTimeParam("DataRetorno");


            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProdutoTMS = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(unitOfWork);
                Repositorio.Embarcador.Frota.Almoxarifado repAlmoxarifado = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Patrimonio.MotivoDefeito repositorioMotivoDefeito = new Repositorio.Embarcador.Patrimonio.MotivoDefeito(unitOfWork);

                Dominio.Entidades.Cliente pessoa = cpfCnpjPessoa > 0d ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjPessoa) : null;
                Dominio.Entidades.Embarcador.Patrimonio.MotivoDefeito motivoDefeito = codigoDefeito > 0d ? repositorioMotivoDefeito.BuscarPorCodigo(codigoDefeito, false) : null;

                if (dataAquisicaoInicial != DateTime.MinValue || dataAquisicaoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataAquisicaoInicial != DateTime.MinValue ? dataAquisicaoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataAquisicaoFinal != DateTime.MinValue ? "até " + dataAquisicaoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAquisicao", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAquisicao", false));

                if (dataAlocadoInicial != DateTime.MinValue || dataAlocadoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataAlocadoInicial != DateTime.MinValue ? dataAlocadoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataAlocadoFinal != DateTime.MinValue ? "até " + dataAlocadoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAlocado", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAlocado", false));

                if (codigoBem > 0)
                {
                    Dominio.Entidades.Embarcador.Patrimonio.Bem _bem = repBem.BuscarPorCodigo(codigoBem);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bem", _bem.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bem", false));

                if (codigoGrupoProduto > 0)
                {
                    Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS _grupoProduto = repGrupoProdutoTMS.BuscarPorCodigo(codigoGrupoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", _grupoProduto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

                if (codigoAlmoxarifado > 0)
                {
                    Dominio.Entidades.Embarcador.Frota.Almoxarifado _almoxarifado = repAlmoxarifado.BuscarPorCodigo(codigoAlmoxarifado);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Almoxarifado", _almoxarifado.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Almoxarifado", false));

                if (codigoFuncionarioAlocado > 0)
                {
                    Dominio.Entidades.Usuario funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionarioAlocado);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioAlocado", funcionario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FuncionarioAlocado", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", propAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("MotivoDefeito", motivoDefeito?.Descricao));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEntrega", dataEntrega, false));
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataRetorno", dataRetorno, false));
            }
            #endregion
            // TODO: ToList cast
            reportResult = repBem.ConsultarRelatorioBem(codigoEmpresa, dataAquisicaoInicial, dataAquisicaoFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, dataAlocadoInicial, dataAlocadoFinal, codigoFuncionarioAlocado, codigoDefeito, cpfCnpjPessoa, dataEntrega, dataRetorno, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repBem.ContarConsultaRelatorioBem(codigoEmpresa, dataAquisicaoInicial, dataAquisicaoFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, dataAlocadoInicial, dataAlocadoFinal, codigoFuncionarioAlocado, codigoDefeito, cpfCnpjPessoa, dataEntrega, dataRetorno, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataAquisicaoFormatada")
                return "DataAquisicao";

            if (propriedadeOrdenar == "DataAlocadoFormatada")
                return "DataAlocado";

            return propriedadeOrdenar;
        }
    }
}
