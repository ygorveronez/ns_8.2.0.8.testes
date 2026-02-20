using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/CTe/ValePedagio")]
    public class ValePedagioController : BaseController
    {
        #region Construtores

        public ValePedagioController(Conexao conexao) : base(conexao) { }

        #endregion

        private Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R249_RelatorioValePedagio;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                int codigoRelatorio = int.Parse(Request.Params("Codigo"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Vale Pedágio", "CTe", "ValePedagio.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCarga", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa = await ObterFiltrosPesquisa(unidadeDeTrabalho, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                int totalRegistros = await repCTe.ContarConsultaRelatorioValePedagioAsync(filtrosPesquisa, agrupamentos);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio> listValePedagio = totalRegistros > 0 ? await repCTe.ConsultarRelatorioValePedagioAsync(filtrosPesquisa, agrupamentos, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.ValePedagio>();

                grid.AdicionaRows(listValePedagio);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);


                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio> ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio repConfiguracaoRelatorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRelatorio(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRelatorio configuracaoRelatorio = await repConfiguracaoRelatorio.BuscarConfiguracaoPadraoAsync();

            int codigoFilial = Request.GetIntParam("Filial");
            List<int> codigosFiliais = new List<int> { };
            if (codigoFilial > 0)
                codigosFiliais.Add(codigoFilial);

            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaValePedagioRelatorio()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                DataCargaInicial = Request.GetDateTimeParam("DataCargaInicial"),
                DataCargaFinal = Request.GetDateTimeParam("DataCargaFinal"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                NumeroValePedagio = Request.GetListParam<string>("NumeroValePedagio"),
                CodigosFiliais = codigosFiliais.Count() == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFiliais,
                SituacaoValePedagio = Request.GetListEnumParam<SituacaoValePedagio>("SituacaoValePedagio"),
                SituacaoIntegracaoValePedagio = Request.GetListEnumParam<SituacaoIntegracao>("SituacaoIntegracaoValePedagio"),
                ExibirCargasAgrupadas = Request.GetBoolParam("ExibirCargasAgrupadas"),
                ExibirTodasCargasPorPadrao = configuracaoRelatorio?.ExibirTodasCargasNoRelatorioDeValePedagio ?? false,
                DataCompraVPRInicial = Request.GetDateTimeParam("DataCompraVPRInicial"),
                DataCompraVPRFinal = Request.GetDateTimeParam("DataCompraVPRFinal"),
                Transportador = Request.GetIntParam("Transportador"),
                Expedidor = Request.GetDoubleParam("Expedidor"),
                Recebedor = Request.GetDoubleParam("Recebedor"),
                Veiculo = Request.GetIntParam("Veiculo"),
                Motorista = Request.GetIntParam("Motorista")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.Transportador = this.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataCargaFormatada")
                return "DataCarga";
            if (propriedadeOrdenar == "DataRetornoValePedagioFormatada")
                return "DataRetornoValePedagio";

            return propriedadeOrdenar;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            //grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Data carga", "DataCargaFormatada", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tipo da carga", "TipoCarga", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motoristas", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Número VP", "NumeroValePedagio", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação VP", "SituacaoValePedagioDescricao", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor VP", "ValorValePedagio", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação integração VP", "SituacaoIntegracaoValePedagioDescricao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Data retorno VP", "DataRetornoValePedagioFormatada", 5, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Retorno", "RetornoIntegracao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Peso Carga", "PesoCarga", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Carga Agrupada", "NumeroCargaAgrupada", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Placa Veículo", "VeiculosCarga", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilialFormatado", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportadorFormatado", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("N° Vale Pedágio", "NumValePedagio", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Integradora", "TipoIntegracaoDescricao", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo da Compra", "TipoCompraValePedagioDescricao", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percurso", "TipoPercursoVPDescricao", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaDescricao", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota", "RotaFrete", 10, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Transportador", false);
            else
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Modo da Compra", "ModoCompraValePedagioTargetDescricao", 10, Models.Grid.Align.left, false, false, false, false, false);


            return grid;
        }

        #endregion
    }
}
