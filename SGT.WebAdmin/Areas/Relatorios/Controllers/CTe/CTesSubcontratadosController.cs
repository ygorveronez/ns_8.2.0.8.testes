using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/CTesSubcontratados")]
    public class CTesSubcontratadosController : BaseController
    {
		#region Construtores

		public CTesSubcontratadosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R268_CTesSubcontratados;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CT-e(s) Subcontratados", "CTe", "CTesSubcontratacao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork, cancellationToken), relatorio);

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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.CTeSubcontratado servicoRelatorioCTesSubcontratados = new Servicos.Embarcador.Relatorios.CTes.CTeSubcontratado(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCTesSubcontratados.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.CTes.CTeSubcontratado> listaCTesSubcontratados, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaCTesSubcontratados);
                grid.setarQuantidadeTotal(countRegistros);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                int codigoEmpresa = Empresa?.Codigo ?? 0;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await svcRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTesSubcontratados()
            {
                CodigoEmpresaCTeOriginal = Request.GetIntParam("EmpresaCTeOriginal"),
                CodigoEmpresaCTeSubcontratado = Request.GetIntParam("EmpresaCTeSubcontratado"),
                NumeroCarga = Request.GetStringParam("Carga"),
                DataInicialEmissao = Request.GetNullableDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetNullableDateTimeParam("DataFinalEmissao"),
                TipoServicoMultisoftware = TipoServicoMultisoftware
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                    filtrosPesquisa.CodigosTransportadores = await new Repositorio.Empresa(unitOfWork, cancellationToken).BuscarCodigoMatrizEFiliaisAsync(Usuario.Empresa?.CNPJ_SemFormato);
                else
                    filtrosPesquisa.CodigosTransportadores = new List<int>() { Usuario.Empresa.Codigo };
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                filtrosPesquisa.TransportadorTerceiro = Usuario.ClienteTerceiro?.CPF_CNPJ ?? 0f;

            return filtrosPesquisa;
        }

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Número CTe", "NumeroCTe", 1.75m, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Série CTe", "SerieCTe", 1.75m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Nº da Carga", "NumeroCarga", 3m, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Documento", "DocumentoDescricao", 3m, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Alíquota ISS", "AliquotaISS", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor ISS", "ValorISS", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor a Receber", "ValorReceber", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Valor Total Sem Imposto", "ValorTotalSemImposto", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", 3m, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("RPS", "RPS", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº da Carga Agrupamento", "NumeroCargaAgrupamento", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº da Pré Carga", "PreCarga", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CMD ID", "CMDID", 3m, Models.Grid.Align.right, false, false, false, true, true);
            grid.AdicionarCabecalho("Codigo do Navio", "CodigoNavio", 3m, Models.Grid.Align.right, false, false, false, true, true);


            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Nº do Pedido Embarcador", "NumeroPedido", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            else
                grid.AdicionarCabecalho("Nº do Pedido", "NumeroPedido", 1.75m, Models.Grid.Align.left, true, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Nº do Pedido Interno", "NumeroPedidoInterno", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            else
                grid.AdicionarCabecalho("NumeroPedidoInterno", false);

            grid.AdicionarCabecalho("Tipo do CT-e", "DescricaoTipoCTe", 1.75m, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Serviço", "DescricaoTipoServico", 1.75m, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo do Tomador", "DescricaoTipoTomador", 1.75m, Models.Grid.Align.center, true, false, false, false, false);

            grid.AdicionarCabecalho("Status", "StatusCTe", 1.75m, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 1.75m, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Ano Emissão", "AnoEmissao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Mês Emissão", "MesEmissao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacaoFormatada", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Anulação", "DataAnulacao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Importação", "DataImportacao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Vínculo Carga", "DataVinculoCarga", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Entrega", "DataEntrega", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCarga", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação Carga", "SituacaoCargaFormatada", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Situação Título", "DescricaoStatusTitulo", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº Pré Fatura", "NumeroPreFatura", 1.75m, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilial", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Filial", "Filial", 5.50m, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Código Remetente", "CodigoRemetente", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Remetente", "IERemetente", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 5.50m, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Endereço Remetente", "CodigoEnderecoRemetente", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Endereço Remetente", "EnderecoRemetente", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Loc. Remetente", "LocalidadeRemetente", 3m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Remetente", "UFRemetente", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Remetente", "GrupoRemetente", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Remetente", "CategoriaRemetente", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Remetente", "CodigoDocumentoRemetente", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Expedidor", "CodigoExpedidor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Expedidor", "CPFCNPJExpedidor", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Expedidor", "IEExpedidor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Expedidor", "Expedidor", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Expedidor", "LocalidadeExpedidor", 3m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Expedidor", "UFExpedidor", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Expedidor", "CodigoDocumentoExpedidor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Recebedor", "CodigoRecebedor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Recebedor", "CPFCNPJRecebedor", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Recebedor", "IERecebedor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Recebedor", "LocalidadeRecebedor", 3m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Recebedor", "UFRecebedor", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rua Recebedor", "RuaRecebedor", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Recebedor", "NumeroRecebedor", 1.75m, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Recebedor", "BairroRecebedor", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Recebedor", "CodigoDocumentoRecebedor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Código Destinatário", "CodigoDestinatario", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatario", "CPFCNPJDestinatario", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Destinatário", "IEDestinatario", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 5.50m, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Endereço Destinatário", "CodigoEnderecoDestinatario", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Endereço Destinatário", "EnderecoDestinatario", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bairro Destinatário", "BairroDestinatario", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CEP Destinatário", "CEPDestinatario", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Loc. Destinatário", "LocalidadeDestinatario", 3m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Destinatário", "UFDestinatario", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Destinatário", "GrupoDestinatario", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Categoria Destinatário", "CategoriaDestinatario", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Destinatário", "CodigoDocumentoDestinatario", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomador", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("IE Tomador", "IETomador", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Tomador", "UFTomador", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Cód. Documentação Tomador", "CodigoDocumentoTomador", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("IBGE início da Prestação", "IBGEInicioPrestacao", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Início da Prestação", "InicioPrestacao", 3m, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Início", "UFInicioPrestacao", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("IBGE fim da Prestação", "IBGEFimPrestacao", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Fim da Prestação", "FimPrestacao", 3m, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Fim", "UFFimPrestacao", 1.75m, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportadorFormatada", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Razão Social Transp.", "RazaoSocialTransportador", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nome Fantasia Transp.", "NomeFantasiaTransportador", 5.50m, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Tipo Pagamento", "DescricaoTipoPagamento", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Frota", "Frota", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso (kg)", "PesoKg", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso Líquido (kg)", "PesoLiquidoKg", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Volumes", "Volumes", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Metros Cúbicos", "MetrosCubicos", 1.75m, Models.Grid.Align.left, false, false, false, false, TipoSumarizacao.nenhum, 0, 6);
            grid.AdicionarCabecalho("Pallets", "Pallets", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CFOP", "CFOP", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CST", "CSTFormatada", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("B.C. do ICMS", "BaseCalculoICMS", 1.75m, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ISS Retido", "ValorISSRetido", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Alíquota PIS", "AliquotaPIS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do PIS", "ValorPIS", 1.75m, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota COFINS", "AliquotaCOFINS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor do COFINS", "ValorCOFINS", 1.75m, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Prestação", "ValorPrestacao", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Valor Total com Imposto Parcial", "ValorSemImposto", 1.75m, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Mercadoria", "ValorMercadoria", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Proprietário do Veículo", "NomeProprietarioVeiculo", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Segmento do Veículo", "SegmentoVeiculo", 5.50m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", 1.75m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NumeroNotaFiscal", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Chave NFe", "ChaveNotaFiscal", 1.75m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Emissão Doc Anterior", "DataNFEmissao", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Doc Anterior", "NumeroDocumentoAnterior", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Aut.", "ProtocoloAutorizacao", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Protocolo Inut./Canc.", "ProtocoloInutilizacaoCancelamento", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CST IBS/CBS", "CSTIBSCBS", 1.75m, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Classificação Tributária IBS/CBS", "ClassificacaoTributariaIBSCBS", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Base de Cálculo IBS/CBS", "BaseCalculoIBSCBS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota IBS Estadual", "AliquotaIBSEstadual", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução IBS Estadual", "PercentualReducaoIBSEstadual", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução IBS Estadual", "ValorReducaoIBSEstadual", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor IBS Estadual", "ValorIBSEstadual", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota IBS Municipal", "AliquotaIBSMunicipal", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução IBS Municipal", "PercentualReducaoIBSMunicipal", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução IBS Municipal", "ValorReducaoIBSMunicipal", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor IBS Municipal", "ValorIBSMunicipal", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Alíquota CBS", "AliquotaCBS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Percentual Redução CBS", "PercentualReducaoCBS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Redução CBS", "ValorReducaoCBS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor CBS", "ValorCBS", 1.75m, Models.Grid.Align.right, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Minuta", "NumeroMinuta", 1.75m, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Observação", "Observacao", 5.50m, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Contrato de Frete", "ContratoFrete", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Fechamento de Frete", "NumeroFechamentoFrete", 5.50m, Models.Grid.Align.left, false, false, false, true, false);
            }
            else
                grid.AdicionarCabecalho("ContratoFrete", false);

            grid.AdicionarCabecalho("Distância", "KmRodado", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Valor Contrato Franquia KM", "ValorKMContrato", 1.75m, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Km Consumido", "KmConsumido", 1.75m, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Valor Frete Franquia KM", "ValorFreteFranquiaKM", 1.75m, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Valor Franquia KM Excedido", "ValorKMExcedenteContrato", 1.75m, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Km Consumido Excedente", "KmConsumidoExcedente", 1.75m, Models.Grid.Align.right, false, false);
                grid.AdicionarCabecalho("Valor Frete Franquia KM Excedido", "ValorFreteFranquiaKMExcedido", 1.75m, Models.Grid.Align.right, false, false);
            }
            else
            {
                grid.AdicionarCabecalho("ValorKMContrato", false);
                grid.AdicionarCabecalho("ValorKMExcedenteContrato", false);
                grid.AdicionarCabecalho("KmConsumido", false);
                grid.AdicionarCabecalho("ValorFreteFranquiaKM", false);
                grid.AdicionarCabecalho("KmConsumidoExcedente", false);
                grid.AdicionarCabecalho("ValorFreteFranquiaKMExcedido", false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Pago", "Pago", 1.75m, Models.Grid.Align.left, true, false, false, true, true);
            else
                grid.AdicionarCabecalho("Pago", false);

            grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", 5.50m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 5.50m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Pessoa", "GrupoTomador", 5.50m, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rotas", "Rotas", 5.50m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 1.75m, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 1.75m, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 5.50m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Ocorrência Final", "DataOcorrenciaFinal", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Veículo do Último MDF-e", "VeiculoUltimoMDFe", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº do Último MDF-e", "NumeroUltimoMDFe", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Última Ocorrência", "DescricaoUltimaOcorrencia", 5.50m, Models.Grid.Align.left, false, false);

            if (await repositorioTipoIntegracao.ExistePorTipoAsync(new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura }))
                grid.AdicionarCabecalho("DT Natura", "NumeroDTNatura", 1.75m, Models.Grid.Align.left, false, false);
            else
                grid.AdicionarCabecalho("NumeroDTNatura", false);

            grid.AdicionarCabecalho("Nº OCA Doc. Orig.", "NumeroOCADocumentoOriginario", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Operador", "Operador", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Data Coleta", "DataColeta", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Log", "Log", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Previsão de Entrega", "DataPrevistaEntrega", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº DI", "NumeroDI", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº DTA", "NumeroDTA", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cód. Referência", "CodigoReferencia", 1.75m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Cód. Importação", "CodigoImportacao", 1.75m, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Nº Pedido Nota Fiscal", "NumeroPedidoNotaFiscal", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Vale Pedágio", "NumeroValePedagio", 3m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Vale Pedágio", "ValorValePedagio", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Tabela Frete", "TabelaFrete", 3m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tabela Frete Cliente", "TabelaFreteCliente", 3m, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Nº Escrituração", "NumeroEscrituracao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Nº Pagamento", "NumeroPagamento", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Nº Contabilização", "NumeroContabilizacao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Nº Escrituração Cancelado", "NumeroLoteCancelamento", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo Cancelamento", "MotivoCancelamento", 5.50m, Models.Grid.Align.left, true, false, false, false, false);

            if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
            {
                grid.AdicionarCabecalho("Nº Booking", "NumeroBooking", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº OS", "NumeroOS", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Proposta", "TipoProposta", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Número Proposta", "NumeroProposta", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Qtd. NF", "QuantidadeNF", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Viagem", "Viagem", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº Lacre", "NumeroLacre", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tara", "Tara", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Container", "Container", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Container", "TipoContainer", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Data Fatura", "DataFatura", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Nº Boleto", "NumeroBoleto", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Data Boleto", "DataBoleto", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Porto Origem", "PortoOrigem", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Porto Destino", "PortoDestino", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Porto Transbordo", "PortoTransbordo", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Navio Transbordo", "NavioTransbordo", 3m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Possui CC-e", "PossuiCartaCorrecao", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Foi Anulado", "FoiAnulado", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Possui CT-e Comp.", "PossuiCTeComplementar", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Foi Substituído", "FoiSubstituido", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Previsão Saída Navio", "ETS", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Previsão Chegada Navio", "ETA", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Tipo Serviço Multimodal", "TipoServicoMultimodal", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número Manifesto", "NumeroManifesto", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número CE Mercante", "NumeroCEMercante", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Afretamento", "DescricaoAfretamento", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número Protocolo ANTAQ", "NumeroProtocoloANTAQ", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número CE FEEDER", "NumeroCEANTAQ", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Número Manifesto FEEDER", "NumeroManifestoFeeder", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Qtd. Container", "QuantidadeContainer", 1.75m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("PTAX", "Taxa", 1.75m, Models.Grid.Align.right, false, false, false, false);
            }

            grid.AdicionarCabecalho("Alíquota ICMS Interna", "AliquotaICMSInterna", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("% ICMS Partilha", "PercentualICMSPartilha", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS UF Origem", "ValorICMSUFOrigem", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS UF Destino", "ValorICMSUFDestino", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS FCP Fim", "ValorICMSFCPFim", 1.75m, Models.Grid.Align.right, true, false, false, false);
            grid.AdicionarCabecalho("Característica Transporte", "CaracteristicaTransporteCTe", 3m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Produto Predominante", "ProdutoPredominante", 3m, Models.Grid.Align.left, true, false, false, false, false);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Centro Resultado", "CentroResultado", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Proc. Importação", "ProcImportacao", 3m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CPF Motorista", "CpfMotorista", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Rota de Frete", "RotaFrete", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor total sem Tributo", "ValorSemTributo", 1.75m, Models.Grid.Align.right, false, false, false, false);

            grid.AdicionarCabecalho("Nº CT-e Substituto", "NumeroCTeSubstituto", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Substituto", "NumeroControleCTeSubstituto", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Anulação", "NumeroCTeAnulacao", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Anulação", "NumeroControleCTeAnulacao", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Complementar", "NumeroCTeComplementar", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Complementar", "NumeroControleCTeComplementar", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Manual Duplicado", "NumeroCTeDuplicado", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Manual Duplicado", "NumeroControleCTeDuplicado", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Original", "NumeroCTeOriginal", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Controle CT-e Original", "NumeroControleCTeOriginal", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CIOT", "NumeroCIOT", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Doc. Originário", "NumeroDocumentoOriginario", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Pagamento", "DataPagamentoFormatada", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Início Viagem", "DataInicioViagem", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº CT-e Terceiro Ocorrência", "NumeroCTeTerceiroOcorrencia", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Doc. Recebedor Ocorrência", "NumeroDocumentoRecebedor", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Pedido Cliente", "NumeroPedidoCliente", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Total Produto", "QuantidadeTotalProduto", 1.75m, Models.Grid.Align.right, false, false, false, false);
            grid.AdicionarCabecalho("Distância Carga Agrupada", "DistanciaCargaAgrupada", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veículo", "ModeloVeiculo", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Carroceria", "TipoCarroceria", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cta. Contábil", "ContaContabil", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Operador Resp. Cancelamento", "OperadorResponsavelCancelamento", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo Tração", "VeiculoTracao", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo Reboque", "VeiculoReboque", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("KM da Rota", "KMRota", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Confirmação Documentos", "DataConfirmacaoDocumento", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário que Solicitou", "UsuarioSolicitante", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Lacre(s)", "LacresCargaLacre", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Pallets (Pedido)", "PalletsPedido", 1.75m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Regra de ICMS", "RegraICMS", 3m, Models.Grid.Align.left, false, false, false, false, false);

            //Colunas da Subcontratação
            grid.AdicionarCabecalho("CT-e Subcontratação", "CTeSubcontratacao", 3m, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Série CT-e Subcontratação", "SerieCTeSubcontracao", 3m, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Chave CT-e Subcontratação", "ChaveCTeSubcontratacao", 3m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Observação CT-e Subcontratação", "ObservacaoCTeSubcontratacao", 3m, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor ICMS CT-e Subcontratação", "ValorICMSSubcontratacao", 3m, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Frete CT-e Subcontratação", "ValorFreteSubcontratacao", 3m, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor a Receber CT-e Subcontratação", "ValorReceberSubcontratacao", 3m, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Prestação CT-e Subcontratação", "ValorPrestacaoSubcontratacao", 1.75m, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Total Sem Imposto CT-e Subcontratação", "ValorTotalSemImpostoSubcontratacao", 3m, Models.Grid.Align.right, false, false, false, false, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho("CNPJ Transportador CT-e Subcontratação", "CNPJTransportadorSubcontratacaoFormatada", 1.75m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Razão Social Transp. CT-e Subcontratação", "RazaoSocialTransportadorSubcontratacao", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
                grid.AdicionarCabecalho("Nome Fantasia Transp. CT-e Subcontratação", "NomeFantasiaTransportadorSubcontratacao", 5.50m, Models.Grid.Align.left, true, false, false, false, false);
            }

            grid.AdicionarCabecalho("Status CT-e Subcontratado", "DescricaoStatusCTeSubcontratado", 1.75m, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}
