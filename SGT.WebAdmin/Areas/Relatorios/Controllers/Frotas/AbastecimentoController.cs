using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/Abastecimento")]
    public class AbastecimentoController : BaseController
    {
		#region Construtores

		public AbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R057_Abastecimento;

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira repIntegracaoAngelLira = new Repositorio.Embarcador.Configuracoes.IntegracaoAngelLira(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAngelLira integracaoAngelLira = await repIntegracaoAngelLira.BuscarAsync();
                bool consultarPosicaoAbastecimento = false;
                if (integracaoAngelLira != null && integracaoAngelLira.ConsultarPosicaoAbastecimento)
                    consultarPosicaoAbastecimento = true;

                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Abastecimentos", "Frotas", "Abastecimento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(consultarPosicaoAbastecimento), relatorio);

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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frotas.Abastecimento servicoRelatorioAbastecimento = new Servicos.Embarcador.Relatorios.Frotas.Abastecimento(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAbastecimento.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Frota.Abastecimento> listaAbastecimento, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAbastecimento);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
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

        private Models.Grid.Grid GridPadrao(bool consultarPosicaoAbastecimento)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Produto", "Produto", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CNPJ Fornecedor", "CpfCnpjFornecedorFormatado", 8, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Categoria", "Categoria", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data", "DataFormatada", 12, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("KM Anterior", "KmAnterior", 8, Models.Grid.Align.center, true, true).NumberFormat("n0");
            grid.AdicionarCabecalho("KM", "Km", 8, Models.Grid.Align.center, true, true).NumberFormat("n0");
            grid.AdicionarCabecalho("KM Total", "KMTotal", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum).NumberFormat("n0");
            grid.AdicionarCabecalho("Litros", "Litros", 8, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum).NumberFormat("n4");
            grid.AdicionarCabecalho("Status", "Status", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, false, true).NumberFormat("n4");
            grid.AdicionarCabecalho("Valor Total", "ValorTotal", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum).NumberFormat("n4");
            grid.AdicionarCabecalho("Nº Acerto", "NumeroAcertoFormatado", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo da Propriedade", "TipoPropriedade", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Proprietário", "Proprietario", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Segmento", "Segmento", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Média", "Media", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.media).NumberFormat("n4");
            grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Equipamento", "Equipamento", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Horímetro Anterior", "HorimetroAnterior", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Horímetro", "Horimetro", 8, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Horímetro Total", "HorimetroTotal", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Média Horímetro", "MediaHorimetro", 8, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.media).NumberFormat("n4");
            grid.AdicionarCabecalho("Média Padrão", "MediaPadrao", 8, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.media).NumberFormat("n4");
            grid.AdicionarCabecalho("Motivo Inconsistência", "MotivoInconsistencia", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Capacidade Tanque", "CapacidadeTanque", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Recebimento", "DescricaoTipoRecebimento", 9, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("KM Original", "KmOriginal", 8, Models.Grid.Align.right, true, false).NumberFormat("n0");
            grid.AdicionarCabecalho("Horímetro Original", "HorimetroOriginal", 8, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoPessoa", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data", "DataSeparada", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Hora", "HoraSeparada", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Fantasia Fornecedor", "FantasiaFornecedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("UF Fornecedor", "UFFornecedor", 4, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 6, Models.Grid.Align.left, true, false, false, false, false);

            if (consultarPosicaoAbastecimento)
            {
                grid.AdicionarCabecalho("Motorista Anterior", "MotoristaAnterior", 10, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("KM Anterior", "KMAnteriorAlteracao", 8, Models.Grid.Align.center, true, false).NumberFormat("n0");
                grid.AdicionarCabecalho("Data Anterior", "DataAnteriorFormatada", 12, Models.Grid.Align.center, true, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("MotoristaAnterior", false);
                grid.AdicionarCabecalho("KMAnteriorAlteracao", false);
                grid.AdicionarCabecalho("DataAnteriorFormatada", false);
            }

            grid.AdicionarCabecalho("País", "Pais", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Moeda", "MoedaDescricao", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data base CRT", "DataBaseCRTFormatada", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor moeda", "ValorMoedaFormatado", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor original moeda", "ValorOriginalMoeda", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Localidade Fornecedor", "LocalidadeFornecedor", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Local de armazenamento", "LocalArmazenamento", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Placa Reboque(s)", "PlacaReboque", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular Tração", "ModeloVeicularTracao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular Reboque", "ModeloVeicularReboque", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Segmento Tração", "SegmentoTracao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Segmento Reboque", "SegmentoReboque", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total TicketLog", "ValorTotalTicketLog", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Litro TicketLog", "ValorLitroTicketLog", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Requisição", "Requisicao", 10, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioAbastecimento()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataBaseCRTInicial = Request.GetDateTimeParam("DataBaseCRTInicial"),
                DataBaseCRTFinal = Request.GetDateTimeParam("DataBaseCRTFinal"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                TipoAbastecimentoInternoExterno = Request.GetIntParam("TipoAbastecimentoInternoExterno"),
                CodigoSegmento = Request.GetIntParam("Segmento"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigosProdutos = Request.GetListParam<int>("Produtos"),
                TipoAbastecimento = Request.GetEnumParam<TipoAbastecimento>("TipoAbastecimento"),
                Fornecedor = Request.GetDoubleParam("Fornecedor"),
                CodigoProprietario = Request.GetDoubleParam("ProprietarioVeiculo"),
                TipoPropriedade = Request.GetStringParam("TipoPropriedade"),
                StatusAbastecimento = Request.GetStringParam("Status"),
                SituacaoAcerto = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.SituacaoAbastecimentoAcertoViagem>("SituacaoAcertoViagem"),
                TiposRecebimento = Request.GetListEnumParam<TipoRecebimentoAbastecimento>("TipoRecebimentoAbastecimento"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0,
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                UFFornecedor = Request.GetStringParam("UFFornecedor"),
                ModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                Paises = Request.GetListParam<int>("Pais"),
                Moedas = Request.GetListEnumParam<MoedaCotacaoBancoCentral>("Moeda"),
                CodigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento"),
                CodigoOrdemCompra = Request.GetIntParam("RequisicaoOC"),
            };
        }

        #endregion
    }
}
