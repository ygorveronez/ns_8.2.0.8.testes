using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/TituloSemMovimento")]
    public class TituloSemMovimentoController : BaseController
    {
		#region Construtores

		public TituloSemMovimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R131_TituloSemMovimento;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Nº Título", "Codigo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("CPF/CNPJ", "CPFCNPJPessoaFormatado", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Nome Pessoa", "NomePessoa", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo", "TipoTitulo", 8, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Status", "StatusTitulo", 8, Models.Grid.Align.center, true, true);

            grid.AdicionarCabecalho("Emissão", "DataEmissao", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Vencimento", "DataVencimento", 12, Models.Grid.Align.center, true, false, false, true, true);

            grid.AdicionarCabecalho("Tipo Doc", "TipoDocumento", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Doc.", "NumeroDocumento", 8, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Parcela", "Parcela", 5, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Valor Pendente", "ValorPendente", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Título", "ValorTitulo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Acréscimo", "ValorAcrescimo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Desconto", "ValorDesonto", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor Saldo", "ValorSaldo", 8, Models.Grid.Align.right, false, true);

            grid.AdicionarCabecalho("Títulos Agrupados", "TitulosAgrupados", 8, Models.Grid.Align.right, false, false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Títulos sem Movimentação", "Financeiros", "TituloSemMovimento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", Codigo, unitOfWork, false, false);
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
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo? tipo = null;
                if (Enum.TryParse(Request.Params("TipoTitulo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoAux))
                    tipo = tipoAux;

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo> status = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo>>(Request.Params("StatusTitulo"));

                double.TryParse(Request.Params("Pessoa"), out double cnpjPessoa);

                DateTime.TryParse(Request.Params("DataInicialEmissao"), out DateTime dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out DateTime dataFinalEmissao);
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out DateTime dataInicialVencimento);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out DateTime dataFinalVencimento);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int codigoEmpresa = this.Usuario.Empresa.Codigo;
                Dominio.Enumeradores.TipoAmbiente? tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, propAgrupa);

                if (propAgrupa == "CPFCNPJPessoaFormatado")
                    propAgrupa = "CNPJCPF";

                if (propOrdena == "CPFCNPJPessoaFormatado")
                    propOrdena = "CNPJCPF";

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloSemMovimento> listaTituloSemMovimento = repTitulo.ConsultarRelatorioTitulosSemMovimentos(agrupamentos, codigoEmpresa, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, tipoAmbiente, TipoServicoMultisoftware, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsultaRelatorioTitulosSemMovimentos(agrupamentos, codigoEmpresa, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, tipoAmbiente, TipoServicoMultisoftware, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite));

                grid.AdicionaRows(listaTituloSemMovimento);

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
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo? tipo = null;
                if (Enum.TryParse(Request.Params("TipoTitulo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo tipoAux))
                    tipo = tipoAux;

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo> status = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo>>(Request.Params("StatusTitulo"));

                double.TryParse(Request.Params("Pessoa"), out double cnpjPessoa);

                DateTime.TryParse(Request.Params("DataInicialEmissao"), out DateTime dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out DateTime dataFinalEmissao);
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out DateTime dataInicialVencimento);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out DateTime dataFinalVencimento);

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int codigoEmpresa = this.Usuario.Empresa.Codigo;
                Dominio.Enumeradores.TipoAmbiente? tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                mdlRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, propAgrupa);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioTituloSemMovimento(agrupamentos, codigoEmpresa, tipoAmbiente, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, TipoServicoMultisoftware, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioTituloSemMovimento(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente? tipoAmbiente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo? tipo, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo> status, double cnpjPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string propOrdena = relatorioTemp.PropriedadeOrdena;

                if (propAgrupa == "CPFCNPJPessoaFormatado")
                    propAgrupa = "CNPJCPF";
                if (propOrdena == "CPFCNPJPessoaFormatado")
                    propOrdena = "CNPJCPF";

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TituloSemMovimento> listaTituloSemMovimento = repTitulo.ConsultarRelatorioTitulosSemMovimentos(agrupamentos, codigoEmpresa, tipo, status, cnpjPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, tipoAmbiente, tipoServicoMultisoftware, propAgrupa, relatorioTemp.OrdemAgrupamento, propOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                if (cnpjPessoa > 0)
                {
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (tipo.HasValue)
                {
                    if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Pagar)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", "A Pagar", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", "A Receber", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", false));

                if (status != null && status.Count > 0)
                {
                    List<string> descricaoStatus = new List<string>();

                    foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusTitulo in status)
                    {
                        if (statusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado)
                            descricaoStatus.Add("Cancelado");
                        else if (statusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                            descricaoStatus.Add("Em Aberto");
                        else if (statusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
                            descricaoStatus.Add("Quitado");
                    }

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", string.Join(", ", descricaoStatus), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", false));

                if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy") + " até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", false));

                if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "De " + dataInicialVencimento.ToString("dd/MM/yyyy") + " até " + dataFinalVencimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "De " + dataInicialVencimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "Até " + dataFinalVencimento.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", false));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/TituloSemMovimento", parametros,relatorioControleGeracao, relatorioTemp, listaTituloSemMovimento, unitOfWork, identificacaoCamposRPT);
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
