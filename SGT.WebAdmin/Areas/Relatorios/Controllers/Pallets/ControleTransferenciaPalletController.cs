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
	[CustomAuthorize("Relatorios/Pallets/ControleTransferenciaPallet")]
    public class ControleTransferenciaPalletController : BaseController
    {
		#region Construtores

		public ControleTransferenciaPalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R139_ControleTransferenciaPallet;

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
                var relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Transferencia de Pallets", "Pallets", "ControleTransferenciaPallet.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                var filtrosPesquisa = ObterFiltrosPesquisa();
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var propriedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var listaTransferenciaPallet = repositorio.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorio(filtrosPesquisa));

                grid.AdicionaRows((
                    from transferencia in listaTransferenciaPallet
                    select new
                    {
                        transferencia.Codigo,
                        DataSolicitacao = transferencia.Solicitacao.Data.ToString("dd/MM/yyyy"),
                        DataRecebimento = transferencia.Recebimento?.Data.ToString("dd/MM/yyyy") ?? "",
                        FilialSolicitacao = transferencia.Solicitacao.Filial.Descricao,
                        FilialCnpjSolicitacao = transferencia.Solicitacao.Filial.CNPJ_Formatado,
                        FilialCodigoIntegracaoSolicitacao = transferencia.Solicitacao.Filial.CodigoFilialEmbarcador,
                        QuantidadeEnvio = transferencia.Envio?.Quantidade ?? 0,
                        QuantidadeRecebimento = transferencia.Recebimento?.Quantidade ?? 0,
                        QuantidadeSolicitacao = transferencia.Solicitacao.Quantidade,
                        transferencia.Recebimento?.Recebedor,
                        transferencia.Envio?.Remetente,
                        ResponsavelEnvio = transferencia.Envio?.Responsavel,
                        SetorSolicitacao = transferencia.Solicitacao.Setor.Descricao,
                        Situacao = transferencia.Situacao.ObterDescricao(),
                        transferencia.Solicitacao.Solicitante,
                        TurnoSolicitacao = transferencia.Solicitacao.Turno.Descricao
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
                var filtrosPesquisa = ObterFiltrosPesquisa();
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

                _ = Task.Factory.StartNew(() => GerarRelatorioControleTransferenciaPallet(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioControleTransferenciaPallet(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet filtrosPesquisa, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioTransferenciaPallet = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemporario.PropriedadeOrdena);
                var listaTransferenciaPallet = repositorioTransferenciaPallet.ConsultarRelatorio(filtrosPesquisa, propriedadeOrdenar, relatorioTemporario.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleTransferenciaPallet> dataSourceTransferenciaPallet = (
                    from transferencia in listaTransferenciaPallet
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.ControleTransferenciaPallet()
                    {
                        Codigo = transferencia.Codigo,
                        DataSolicitacao = transferencia.Solicitacao.Data,
                        DataRecebimento = transferencia.Recebimento?.Data.ToString("dd/MM/yyyy") ?? "",
                        FilialSolicitacao = transferencia.Solicitacao.Filial.Descricao,
                        FilialCnpjSolicitacao = transferencia.Solicitacao.Filial.CNPJ_Formatado,
                        FilialCodigoIntegracaoSolicitacao = transferencia.Solicitacao.Filial.CodigoFilialEmbarcador,
                        QuantidadeEnvio = transferencia.Envio?.Quantidade ?? 0,
                        QuantidadeRecebimento = transferencia.Recebimento?.Quantidade ?? 0,
                        QuantidadeSolicitacao = transferencia.Solicitacao.Quantidade,
                        Recebedor = transferencia.Recebimento?.Recebedor,
                        Remetente = transferencia.Envio?.Remetente,
                        ResponsavelEnvio = transferencia.Envio?.Responsavel,
                        SetorSolicitacao = transferencia.Solicitacao.Setor.Descricao,
                        Situacao = transferencia.Situacao.ObterDescricao(),
                        Solicitante = transferencia.Solicitacao.Solicitante,
                        TurnoSolicitacao = transferencia.Solicitacao.Turno.Descricao
                    }
                ).ToList();

                var parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/ControleTransferenciaPallet",parametros,relatorioControleGeracao, relatorioTemporario, dataSourceTransferenciaPallet, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "DataSolicitacao", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Solicitante", "Solicitante", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "FilialSolicitacao", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ", "FilialCnpjSolicitacao", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração", "FilialCodigoIntegracaoSolicitacao", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Setor", "SetorSolicitacao", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Turno", "TurnoSolicitacao", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Quantidade", "QuantidadeSolicitacao", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Responsável Envio", "ResponsavelEnvio", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade Envio", "QuantidadeEnvio", 8, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Data Recebimento", "DataRecebimento", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidade Recebimento", "QuantidadeRecebimento", 8, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet ObterFiltrosPesquisa()
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet()
            {
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                ListaCodigoFilial = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Filial")),
                ListaCodigoSetor = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Setor")),
                ListaCodigoTurno = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Turno")),
                Situacao = Request.GetEnumParam<SituacaoTransferenciaPallet>("Situacao")
            };

            return filtrosPesquisa;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaControleTransferenciaPallet filtrosPesquisa)
        {
            var parametros = new List<Parametro>();

            if (filtrosPesquisa.DataInicio.HasValue || filtrosPesquisa.DataLimite.HasValue)
            {
                var periodo = $"{(filtrosPesquisa.DataInicio.HasValue ? $"{filtrosPesquisa.DataInicio.Value.ToString("dd/MM/yyyy")} " : "")}{(filtrosPesquisa.DataLimite.HasValue ? $"até {filtrosPesquisa.DataLimite.Value.ToString("dd/MM/yyyy")}" : "")}";
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

            if (filtrosPesquisa.ListaCodigoSetor?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoSetor.Count == 1)
                {
                    var repositorioSetor = new Repositorio.Setor(unitOfWork);
                    var setor = repositorioSetor.BuscarPorCodigo(filtrosPesquisa.ListaCodigoSetor.First());

                    parametros.Add(new Parametro("Setor", setor.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Setor", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Setor", false));

            if (filtrosPesquisa.ListaCodigoTurno?.Count > 0)
            {
                if (filtrosPesquisa.ListaCodigoTurno.Count == 1)
                {
                    var repositorioTurno = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
                    var turno = repositorioTurno.BuscarPorCodigo(filtrosPesquisa.ListaCodigoTurno.First());

                    parametros.Add(new Parametro("Turno", turno.Descricao, true));
                }
                else
                    parametros.Add(new Parametro("Turno", "Múltiplos Registros Selecionados", true));
            }
            else
                parametros.Add(new Parametro("Turno", false));

            if (filtrosPesquisa.Situacao != SituacaoTransferenciaPallet.Todas)
                parametros.Add(new Parametro("Situacao", filtrosPesquisa.Situacao.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("Situacao", false));

            return parametros;
        }

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "DataSolicitacao")
                return "Solicitacao.Data";

            if (nomePropriedadeOrdenar == "FilialSolicitacao")
                return "Solicitacao.Filial.Descricao";

            if (nomePropriedadeOrdenar == "SetorSolicitacao")
                return "Solicitacao.Setor.Descricao";

            if (nomePropriedadeOrdenar == "TurnoSolicitacao")
                return "Solicitacao.Turno.Descricao";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
