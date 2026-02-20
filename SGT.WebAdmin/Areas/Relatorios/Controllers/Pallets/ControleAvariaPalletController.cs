using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/ControleAvariaPallet")]
    public class ControleAvariaPalletController : BaseController
    {
		#region Construtores

		public ControleAvariaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R142_ControleAvariaPallet;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                var codigoRelatorio = Request.GetIntParam("Codigo");
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Avaria de Pallets", "Pallets", "ControleAvariaPallet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                var gridRelatorio = new Models.Grid.Relatorio();
                var dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                var filtrosPesquisa = ObterFilstrosPesquisa();
                var repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var listaAvariaPallet = repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorio(filtrosPesquisa));

                grid.AdicionaRows((
                    from avaria in listaAvariaPallet
                    select new
                    {
                        avaria.Codigo,
                        Data = avaria.Data.ToString("dd/MM/yyyy"),
                        Filial = avaria.Filial?.Descricao,
                        FilialCnpj = avaria.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = avaria.Filial?.CodigoFilialEmbarcador,
                        Transportador = avaria.Transportador?.Descricao,
                        TransportadorCnpj = avaria.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = avaria.Transportador?.CodigoIntegracao,
                        MotivoAvaria = avaria.MotivoAvaria.Descricao,
                        avaria.Numero,
                        avaria.Observacao,
                        Quantidade = (from quantidadeAvariada in avaria.QuantidadesAvariadas select quantidadeAvariada.Quantidade).Sum(),
                        Setor = avaria.Setor?.Descricao,
                        Situacao = avaria.Situacao.ObterDescricao()
                    }
                ).ToList());

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
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var filtrosPesquisa = ObterFilstrosPesquisa();
                var repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                var dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                var relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                var relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                var relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                var mdlRelatorio = new Models.Grid.Relatorio();
                var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var stringConexao = _conexao.StringConexao;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propriedadeOrdenar, relatorioTemporario.PropriedadeAgrupa);

                _ = Task.Factory.StartNew(() => GerarRelatorioControleAvariaPallet(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioControleAvariaPallet(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet filtrosPesquisa, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioAvariaPallet = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var listaAvariaPallet = repositorioAvariaPallet.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleAvariaPallet> dataSourceAvariaPallet = (
                    from avaria in listaAvariaPallet
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleAvariaPallet()
                    {
                        Codigo = avaria.Codigo,
                        Data = avaria.Data,
                        Filial = avaria.Filial?.Descricao,
                        FilialCnpj = avaria.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = avaria.Filial?.CodigoFilialEmbarcador,
                        Transportador = avaria.Transportador?.Descricao,
                        TransportadorCnpj = avaria.Transportador?.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = avaria.Transportador?.CodigoIntegracao,
                        MotivoAvaria = avaria.MotivoAvaria.Descricao,
                        Numero = avaria.Numero,
                        Observacao = avaria.Observacao,
                        Quantidade = (from quantidadeAvariada in avaria.QuantidadesAvariadas select quantidadeAvariada.Quantidade).Sum(),
                        Setor = avaria.Setor?.Descricao,
                        Situacao = avaria.Situacao.ObterDescricao()
                    }
                ).ToList();

                var dataSourceQuantidadesAvariadas = new List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleAvariaPalletQuantidadeAvariada>();

                if (filtrosPesquisa.ExibirQuantidadesAvariadas)
                {
                    var listaQuantidadesAvariadas = (from avaria in listaAvariaPallet from quantidadeAvariada in avaria.QuantidadesAvariadas select quantidadeAvariada);

                    dataSourceQuantidadesAvariadas = (
                        from quantidadeAvariada in listaQuantidadesAvariadas
                        select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleAvariaPalletQuantidadeAvariada()
                        {
                            CodigoAvaria = quantidadeAvariada.AvariaPallet.Codigo,
                            Descricao = quantidadeAvariada.SituacaoDevolucaoPallet.Descricao,
                            Quantidade = quantidadeAvariada.Quantidade,
                            ValorTotal = (quantidadeAvariada.Quantidade * quantidadeAvariada.SituacaoDevolucaoPallet.ValorUnitario),
                            ValorUnitario = quantidadeAvariada.SituacaoDevolucaoPallet.ValorUnitario
                        }
                    ).ToList();
                }
                var dataSourceSubreport = new List<KeyValuePair<string, dynamic>>() { new KeyValuePair<string, dynamic>("ControleAvariaPalletQuantidadesAvariadas.rpt", dataSourceQuantidadesAvariadas) };
                var parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/ControleAvariaPallet",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceAvariaPallet, unitOfWork, null, dataSourceSubreport, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet ObterFilstrosPesquisa()
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                ExibirQuantidadesAvariadas = Request.GetBoolParam("ExibirQuantidadesAvariadas"),
                ListaCodigoFilial = Request.GetListParam<int>("Filial"),
                ListaCodigoMotivoAvaria = Request.GetListParam<int>("MotivoAvaria"),
                ListaCodigoSetor = Request.GetListParam<int>("Setor"),
                ListaCodigoTransportador = Request.GetListParam<int>("Transportador"),
                Situacao = Request.GetEnumParam("Situacao", SituacaoAvariaPallet.Todas)
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true, false, false, false, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("Empresa/Filial", "Transportador", 18, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Empresa/Filial", "TransportadorCnpj", 9, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Código integração Empresa/Filial", "TransportadorCodigoIntegracao", 9, Models.Grid.Align.center, false, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("Filial", "Filial", 18, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Filial", "FilialCnpj", 9, Models.Grid.Align.center, false, false, false, false, false);
                grid.AdicionarCabecalho("Código integração Filial", "FilialCodigoIntegracao", 9, Models.Grid.Align.center, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true, false, false, true, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Setor", "Setor", 18, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Motivo", "MotivoAvaria", 18, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 18, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 18, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 7, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleAvariaPallet filtrosPesquisa)
        {
            var parametros = new List<Parametro>();

            if (filtrosPesquisa.DataInicial.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                var periodo = $"{(filtrosPesquisa.DataInicial.HasValue ? $"{filtrosPesquisa.DataInicial.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
                parametros.Add(new Parametro("Periodo", periodo, true));
            }
            else
                parametros.Add(new Parametro("Periodo", false));

            if (filtrosPesquisa.ListaCodigoFilial?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoFilial.Count == 1)
                {
                    var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                    var filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.ListaCodigoFilial.First());

                    parametros.Add(new Parametro("Filial", filial.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Filial", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Filial", false));

            if (filtrosPesquisa.ListaCodigoMotivoAvaria?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoMotivoAvaria.Count == 1)
                {
                    var repositorioFilial = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
                    var motivoAvaria = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.ListaCodigoMotivoAvaria.First());

                    parametros.Add(new Parametro("MotivoAvaria", motivoAvaria.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("MotivoAvaria", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("MotivoAvaria", false));

            if (filtrosPesquisa.ListaCodigoSetor?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoSetor.Count == 1)
                {
                    var repositorioFilial = new Repositorio.Setor(unitOfWork);
                    var setor = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.ListaCodigoSetor.First());

                    parametros.Add(new Parametro("Setor", setor.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Setor", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Setor", false));

            if (filtrosPesquisa.ListaCodigoTransportador?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoTransportador.Count == 1)
                {
                    Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.ListaCodigoTransportador.First());

                    parametros.Add(new Parametro("Transportador", transportador.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Transportador", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Transportador", false));

            if (filtrosPesquisa.ExibirQuantidadesAvariadas)
                parametros.Add(new Parametro("ExibirQuantidadesAvariadas", "Sim", visivel: true));
            else
                parametros.Add(new Parametro("ExibirQuantidadesAvariadas", visivel: false));

            if (filtrosPesquisa.Situacao != SituacaoAvariaPallet.Todas)
                parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao.ObterDescricao(), visivel: true));
            else
                parametros.Add(new Parametro("Situacao", visivel: false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (nomePropriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
