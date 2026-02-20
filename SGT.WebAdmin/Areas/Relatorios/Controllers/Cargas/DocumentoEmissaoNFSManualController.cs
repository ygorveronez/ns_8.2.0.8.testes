using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/DocumentoEmissaoNFSManual")]
    public class DocumentoEmissaoNFSManualController : BaseController
    {
		#region Construtores

		public DocumentoEmissaoNFSManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R125_DocumentoEmissaoNFSManual;
        private readonly decimal _tamanhoColumaExtraPequena = 1m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio,
                    TipoServicoMultisoftware, "Relatório de Documentos para Emissão de NFS Manual", "Cargas", "DocumentoEmissaoNFSManual.rpt",
                    Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Carga.CargaDocumentoEmissaoNFSManual svcCargaDocumentoEmissaoNFSManual = new Servicos.Embarcador.Relatorios.Carga.CargaDocumentoEmissaoNFSManual(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcCargaDocumentoEmissaoNFSManual.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa,
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario,
            string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaDocumentoEmissaoNFSManual> dataSourceCargaDocumentoEmissaoNFSManual = repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarRelatorioDocumentoEmissaoNFSManual(filtrosPesquisa, propriedades, parametrosConsulta);
               
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = await ObterParametrosRelatorioAsync(unitOfWork, filtrosPesquisa, cancellationToken);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Cargas/DocumentoEmissaoNFSManual", parametros, relatorioControleGeracao, relatorioTemporario, dataSourceCargaDocumentoEmissaoNFSManual, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtro =
                new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual()
            {
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigosFiliais = Request.GetListParam<int>("Filial").Count == 0 ?
                await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken) : Request.GetListParam<int>("Filial"),
                CodigosRecebedores = await ObterListaCnpjCpfRecebedorPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoLocalidadePrestacao = Request.GetIntParam("LocalidadePrestacao"),
                DataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetNullableDateTimeParam("DataEmissaoFinal"),
                DataEmissaoNFSInicial = Request.GetNullableDateTimeParam("DataEmissaoNFSInicial"),
                DataEmissaoNFSFinal = Request.GetNullableDateTimeParam("DataEmissaoNFSFinal"),
                EstadoLocalidadePrestacao = Request.GetStringParam("EstadoPrestacao"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                NumeroInicialNFS = Request.GetIntParam("NumeroNFSInicial"),
                NumeroFinalNFS = Request.GetIntParam("NumeroNFSFinal"),
                PossuiNFSGerada = Request.GetNullableBoolParam("PossuiNFSGerada"),
                SituacaoNFS = Request.GetListEnumParam<SituacaoLancamentoNFSManual>("SituacaoNFS"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtro.CodigoEmpresa = Usuario.Empresa.Codigo;

            return filtro;

        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Ocorrência", "NumeroOcorrencia", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número", "Numero", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "Serie", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Modelo Documento", "ModeloDocumento", _tamanhoColunaMedia, Models.Grid.Align.left, true, true, false, false, false);
            grid.AdicionarCabecalho("Descrição", "Descricao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Chave", "Chave", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Modelo", "ModeloDocumentoFiscal", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nº NFS", "NumeroNFS", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Série NFS", "SerieNFS", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Emissão NFS", "DataEmissaoNFSManual", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Situação NFS", "SituacaoNFSFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("NFS Gerada", "NFSGerada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetenteFormatado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "NomeRemetente", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJDestinatarioFormatado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "NomeDestinatario", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CPF/CNPJ Tomador", "CPFCNPJTomadorFormatado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "NomeTomador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Localidade Prestação", "LocalidadePrestacao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Estado Prestação", "EstadoPrestacao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);


            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("CPF/CNPJ Empresa/Filial", "CpfCnpjEmpresaFormatado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
                grid.AdicionarCabecalho("Empresa/Filial", "Empresa", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            }
            else
            {
                grid.AdicionarCabecalho("CPF/CNPJ Transportador", "CpfCnpjEmpresaFormatado", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
                grid.AdicionarCabecalho("Transportador", "Empresa", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Filial", "Filial", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            }

            grid.AdicionarCabecalho("Peso", "Peso", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Prestação", "ValorPrestacaoServico", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Base Cálculo ISS", "BaseCalculoISS", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Alíquota ISS", "AliquotaISS", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor ISS", "ValorISS", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("% Retenção ISS", "PercentualRetencaoISS", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor Retenção ISS", "ValorRetencaoISS", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Incluir ISS", "IncluirISSBaseCalculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Pedido Cliente", "NumeroPedidoCliente", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoasTomador", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor NF", "ValorNotaFiscal", _tamanhoColunaPequena, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("CEP Origem", "CEPOrigem", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CEP Destino", "CEPDestino", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Placa VeÃ­culo", "VeiculoCargaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private async Task<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>> ObterParametrosRelatorioAsync(Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaDocumentoEmissaoNFSManual filtrosPesquisa, CancellationToken cancellationToken)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork, cancellationToken);

            Dominio.Entidades.Cliente remetente = filtrosPesquisa.CpfCnpjRemetente > 0d ? await repositorioCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CpfCnpjRemetente) : null;
            Dominio.Entidades.Cliente destinatario = filtrosPesquisa.CpfCnpjDestinatario > 0d ? await repositorioCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CpfCnpjDestinatario) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CpfCnpjTomador > 0d ? await repositorioCliente.BuscarPorCPFCNPJAsync(filtrosPesquisa.CpfCnpjTomador) : null;
            Dominio.Entidades.Localidade localidadePrestacao = filtrosPesquisa.CodigoLocalidadePrestacao > 0 ? await repositorioLocalidade.BuscarPorCodigoAsync(filtrosPesquisa.CodigoLocalidadePrestacao) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? await repositorioEmpresa.BuscarPorCodigoAsync(filtrosPesquisa.CodigoEmpresa) : null;
            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFiliais.Count > 0 ? await repositorioFilial.BuscarPorCodigosAsync(filtrosPesquisa.CodigosFiliais) : null;
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas > 0 ? await repositorioGrupoPessoas.BuscarPorCodigoAsync(filtrosPesquisa.CodigoGrupoPessoas) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? await repTipoOperacao.BuscarPorCodigoAsync(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoInicial", filtrosPesquisa.DataEmissaoInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFinal", filtrosPesquisa.DataEmissaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoNFSInicial", filtrosPesquisa.DataEmissaoNFSInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoNFSFinal", filtrosPesquisa.DataEmissaoNFSFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PossuiNFSGerada", filtrosPesquisa.PossuiNFSGerada));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", tomador?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filiais != null ? string.Join(", ", filiais?.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalidadePrestacao", localidadePrestacao?.DescricaoCidadeEstado));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoPrestacao", filtrosPesquisa.EstadoLocalidadePrestacao != "0" ? filtrosPesquisa.EstadoLocalidadePrestacao : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroInicial", filtrosPesquisa.NumeroInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFinal", filtrosPesquisa.NumeroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNFSInicial", filtrosPesquisa.NumeroInicialNFS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroNFSFinal", filtrosPesquisa.NumeroFinalNFS));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", transportador?.Descricao));

            return parametros;
        }

        #endregion
    }
}
