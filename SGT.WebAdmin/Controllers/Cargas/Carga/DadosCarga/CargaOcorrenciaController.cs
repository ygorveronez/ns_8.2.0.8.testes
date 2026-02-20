using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.Carga.DadosCarga
{
    [CustomAuthorize(new string[] { "DownloadPlanilhaRateio", "ConsultarCargasPorPeriodo" , "ExportarDetalhesCargas",
        "CargaOcorrenciaSumarizado", "DetalhesCargaSumarizada", "ExportarDetalhesDocumentos", "ExecutaPesquisaDetalhes" }, "Ocorrencias/Ocorrencia", "Ocorrencias/AutorizacaoOcorrencia")]
    public class CargaOcorrenciaController : BaseController
    {
		#region Construtores

		public CargaOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> DownloadPlanilhaRateio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork, cancellationToken);
                
                // Busca Dados
                int.TryParse(Request.Params("Ocorrencia"), out int codigo);
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = await repCargaOcorrencia.BuscarPorCodigoAsync(codigo);

                if (ocorrencia == null) 
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (ocorrencia.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.SituacaoDaOcorrenciaDeveSerFinalizada);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NotaFiscal, "NotaFiscal", 1, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ContaSAP, "ContaSAP", 1, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CentroDeCusto, "CentroCusto", 1, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Valor, "Valor", 1, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NomePrestador, "NomePrestador", 1, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.CNPJ, "CNPJ", 1, Models.Grid.Align.center, true);

                dynamic lista = null;

                if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo)
                    lista = await GerarPlanilhaRateioPeriodoAsync(ocorrencia, unitOfWork, cancellationToken);
                else if (ocorrencia.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato)
                    lista = GerarPlanilhaRateioContratoAsync(ocorrencia, unitOfWork, cancellationToken);

                if (lista == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelBuscarOsDadosParaGerarArquivo);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", Localization.Resources.Cargas.Carga.Rateio + "-" + ocorrencia.NumeroOcorrencia.ToString() + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCargasPorPeriodoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridCargas();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = await ExecutaPesquisaCargasAsync(totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, cancellationToken);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarDetalhesCargasAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridCargasDocumentos();

                // Ordenacao da grid
                string propOrdenar = "Carga.Veiculo.Placa";
                string dirOrdena = "asc";

                // Busca Dados
                /* Codigo do veiculo DEVE permanecer 0
                 * A exportação da grid não sumarizada inclui os dados detalhados de cada veículo
                 * Seria o mesmo que a concatenação de todos relatórios detalhados
                 */
                int codigoVeiculo = 0;
                int totalRegistros = 0;
                var lista = await ExecutaPesquisaDetalhesAsync(totalRegistros, codigoVeiculo, propOrdenar, dirOrdena, 0, 0, unitOfWork, cancellation);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> CargaOcorrenciaSumarizadoAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = 
                    new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork, cancellation);

                int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);

                Models.Grid.Grid grid = GridCargas();

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "ModeloVeicular")
                    propOrdenacao = "Veiculo.ModeloVeicularCarga.CodigoIntegracao";
                if (propOrdenacao == "Veiculo")
                    propOrdenacao = "Veiculo.Placa";
                dynamic lista = null;
                int totalRegistros = 0;

                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> cargas = await repCargaOcorrenciaSumarizado
                    .ConsultarCargasOcorrenciaPorOcorrenciaAsync(codigoOcorrencia, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                totalRegistros = await repCargaOcorrenciaSumarizado
                    .ContarConsultaCargasOcorrenciaPorOcorrenciaAsync(codigoOcorrencia);

                lista = (from obj in cargas
                         select new
                         {
                             obj.Codigo,
                             CodigoVeiculo = obj.Veiculo.Codigo,
                             obj.QuantidadeDias,
                             Veiculo = obj.Veiculo.Placa,
                             ModeloVeicular = obj.Veiculo.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                             obj.QuantidadeDocumentos,
                             obj.ValorMercadoria,
                         }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> DetalhesCargaSumarizadaAsync(CancellationToken cancellation)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridCargasDocumentos();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int.TryParse(Request.Params("CodigoVeiculo"), out int codigoVeiculo);
                int totalRegistros = 0;
                var lista = await ExecutaPesquisaDetalhesAsync(totalRegistros, codigoVeiculo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, 
                    unitOfWork, cancellation);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> ExportarDetalhesDocumentosAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridCargasDocumentos();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int.TryParse(Request.Params("CodigoVeiculo"), out int codigoVeiculo);
                int totalRegistros = 0;
                var lista = await ExecutaPesquisaDetalhesAsync(totalRegistros, codigoVeiculo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, 
                    unitOfWork, cancellationToken);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesDocumentosAgrupadosAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = 
                    new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork, cancellationToken);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                // Manipula grids
                Models.Grid.Grid grid = GridCargasDocumentos();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);

                int.TryParse(Request.Params("ContratoFreteTransportador"), out int contratoFreteTransportador);
                int.TryParse(Request.Params("ModeloDocumento"), out int modeloDocumento);
                int.TryParse(Request.Params("Veiculo"), out int veiculo);

                string cnpjRemetente = Request.Params("CnpjRemetente") ?? string.Empty;
                string cnpjDestinatario = Request.Params("CnpjDestinatario") ?? string.Empty;
                string placa = (await repVeiculo.BuscarPorCodigoAsync(veiculo).ConfigureAwait(false))?.Placa ?? string.Empty;

                FiltrosPesquisa(out DateTime periodoInicial, out DateTime periodoFim, out int transportador, out int filial, out string proprietario, 
                    unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> documentos = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                // Se existe um codigo, significa que esta listando as cargas de uma ocorrencia ja lancada
                // Caso contrários, trás as cargas de acordo com o periodo informado
                if (codigoOcorrencia > 0)
                {
                    documentos = await repCargaOcorrenciaSumarizado
                        .ConsultarDocumentosCargasPorContratoEVeiculoDaOcorrenciaAsync(codigoOcorrencia, cnpjRemetente, cnpjDestinatario, modeloDocumento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    totalRegistros = await repCargaOcorrenciaSumarizado
                        .ContarConsultaDocumentosCargasPorContratoEVeiculoDaOcorrenciaAsync(codigoOcorrencia, cnpjRemetente, cnpjDestinatario, modeloDocumento);
                }
                else
                {
                    documentos = await repCargaOcorrenciaSumarizado
                        .ConsultarDocumentosCargasPorContratoEVeiculoAsync(periodoInicial, periodoFim, transportador, proprietario, placa, contratoFreteTransportador, cnpjRemetente, cnpjDestinatario, modeloDocumento, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    totalRegistros = await repCargaOcorrenciaSumarizado
                        .ContarConsultaDocumentosCargasPorContratoEVeiculoAsync(periodoInicial, periodoFim, transportador, proprietario, placa, contratoFreteTransportador, cnpjRemetente, cnpjDestinatario, modeloDocumento);
                }

                var lista = (from obj in documentos select LinhaDetalheFormatada(obj,
                                                                                 unitOfWork,
                                                                                 cancellationToken)).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> ConsultarDocumentosAgrupadosVeiculoAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = 
                    new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CnpjRemetente", false);
                grid.AdicionarCabecalho("CnpjDestinatario", false);
                grid.AdicionarCabecalho("ModeloDocumento", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Remetente, "Remetente", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destinatario, "Destinatario", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloDocumento, "DescricaoModeloDocumento", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.QuantidadeDeCargas, "QuantidadeCargas", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorMercadoria, "ValorMercadoria", 15, Models.Grid.Align.center, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;

                int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);
                int.TryParse(Request.Params("Veiculo"), out int veiculo);
                int.TryParse(Request.Params("ContratoFreteTransportador"), out int contrato);

                DateTime.TryParse(Request.Params("PeriodoInicio"), out DateTime periodoInicial);
                DateTime.TryParse(Request.Params("PeriodoFim"), out DateTime periodoFim);

                string placa = (await repVeiculo.BuscarPorCodigoAsync(veiculo))?.Placa ?? string.Empty;

                int transportador = 0;
                string proprietario = "";

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    Dominio.Entidades.Empresa empresaTerceiro = await repEmpresa.BuscarPorCNPJAsync(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                    transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                    proprietario = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                {
                    transportador = Usuario.Empresa.Codigo;
                    proprietario = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : string.Empty;
                }
                else
                {
                    int.TryParse(Request.Params("Transportador"), out transportador);
                    Dominio.Entidades.Empresa transportadoraSelecionada = await repEmpresa.BuscarPorCodigoAsync(transportador);
                    proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
                }

                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados> documentos = new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaDocumentosAgrupados>();

                // Se existe um codigo, significa que esta listando as cargas de uma ocorrencia ja lancada
                // Caso contrários, trás as cargas de acordo com o periodo informado
                if (codigoOcorrencia > 0)
                {
                    documentos = await repCargaOcorrenciaSumarizado.ConsultarDocumentosAgrupadosDaOcorrenciaAsync(codigoOcorrencia, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite); 
                    totalRegistros = await repCargaOcorrenciaSumarizado.ContarConsultaDocumentosAgrupadosDaOcorrenciaAsync(codigoOcorrencia);
                }
                else
                {
                    if (propOrdenar == "Remetente")
                        propOrdenar = "NomeRemetente";
                    else if (propOrdenar == "Destinatario")
                        propOrdenar = "NomeDestinatario";

                    documentos = await repCargaOcorrenciaSumarizado.ConsultarDocumentosAgrupadosAsync(periodoInicial, periodoFim, transportador, proprietario, placa, contrato, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    totalRegistros = await repCargaOcorrenciaSumarizado.ContarConsultaDocumentosAgrupadosAsync(periodoInicial, periodoFim, transportador, proprietario, placa, contrato);
                }

                var lista = (from obj in documentos
                         select new
                         {
                             obj.Codigo,
                             obj.CnpjRemetente,
                             obj.CnpjDestinatario,
                             Remetente = !string.IsNullOrWhiteSpace(obj.CodigoRemetente) ? obj.CodigoRemetente : obj.CnpjRemetente_Formatado + " - " + obj.NomeRemetente,
                             Destinatario = !string.IsNullOrWhiteSpace(obj.CodigoDestinatario) ? obj.CodigoDestinatario : obj.CnpjDestinatario_Formatado + " - " + obj.NomeDestinatario,
                             obj.ModeloDocumento,
                             obj.DescricaoModeloDocumento,
                             obj.QuantidadeCargas,
                             ValorMercadoria = obj.ValorMercadoria.ToString("n2"),
                         }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private async Task<dynamic> ExecutaPesquisaDetalhesAsync(int totalRegistros, int codigoVeiculo, string propOrdenar, string dirOrdena, int inicio, 
            int limite, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = 
                new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork, cancellationToken);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork, cancellationToken);

            int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);
            int.TryParse(Request.Params("TipoOcorrencia"), out int codigoTipoOcorrencia);

            FiltrosPesquisa(out DateTime periodoInicial, out DateTime periodoFim, out int transportador, out int filial, out string proprietario, 
                unitOfWork, cancellationToken);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = await repTipoDeOcorrenciaDeCTe.BuscarPorCodigoAsync(codigoTipoOcorrencia);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> documentos = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            // Se existe um codigo, significa que esta listando as cargas de uma ocorrencia ja lancada
            // Caso contrários, trás as cargas de acordo com o periodo informado
            if (codigoOcorrencia > 0)
            {
                documentos = await repCargaOcorrenciaSumarizado.ConsultarDocumentosCargasOcorrenciaPorPeriodoAsync(codigoOcorrencia, codigoVeiculo, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = await repCargaOcorrenciaSumarizado.ContarConsultaDocumentosCargasOcorrenciaPorPeriodoAsync(codigoOcorrencia, codigoVeiculo);
            }
            else
            {
                documentos = await repCargaOcorrenciaSumarizado.ConsultarDocumentosCargasPorPeriodoAsync(periodoInicial, periodoFim, transportador, filial, proprietario, codigoVeiculo, tipoOcorrencia.FiltrarCargasPeriodo, tipoOcorrencia.Codigo, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = await repCargaOcorrenciaSumarizado.ContarConsultaDocumentosCargasPorPeriodoAsync(periodoInicial, periodoFim, transportador, filial, proprietario, codigoVeiculo, tipoOcorrencia.FiltrarCargasPeriodo, tipoOcorrencia.Codigo);
            }

            var lista = (from obj in documentos select LinhaDetalheFormatada(obj, unitOfWork, cancellationToken)).ToList();

            return lista;
        }

        private async Task<dynamic> LinhaDetalheFormatada(Dominio.Entidades.Embarcador.Cargas.CargaCTe obj, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellation)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellation);

            double.TryParse(obj.CTe.Remetente.CPF_CNPJ_SemFormato, out double cnpjorigem);
            double.TryParse(obj.CTe.Destinatario.CPF_CNPJ_SemFormato, out double cnpjdestino);

            var origem = await repCliente.BuscarPorCPFCNPJAsync(cnpjorigem);
            var destino = await repCliente.BuscarPorCPFCNPJAsync(cnpjdestino);

            return new
            {
                Codigo = obj.Codigo,
                Carga = obj.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                DataEmissao = obj.CTe.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                Origem = origem.CodigoIntegracao ?? obj.CTe.Remetente.CPF_CNPJ_Formatado,
                Destino = destino.CodigoIntegracao ?? obj.CTe.Destinatario.CPF_CNPJ_Formatado,              
                Placa = obj.Carga?.Veiculo?.Placa ?? string.Empty,
                ModeloVeicular = obj.Carga?.Veiculo?.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                ValorMercadoria = obj.CTe.ValorTotalMercadoria
            };
        }

        private async Task<dynamic> ExecutaPesquisaCargasAsync(int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, 
            Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado repCargaOcorrenciaSumarizado = new 
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado(unitOfWork, cancellationToken);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork, cancellationToken);

            int.TryParse(Request.Params("Ocorrencia"), out int codigoOcorrencia);
            int.TryParse(Request.Params("TipoOcorrencia"), out int codigoTipoOcorrencia);
            DateTime.TryParse(Request.Params("PeriodoInicio"), out DateTime periodoInicial);
            DateTime.TryParse(Request.Params("PeriodoFim"), out DateTime periodoFim);

            int transportador = 0;
            string proprietario = "";
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = await repTipoOcorrencia.BuscarPorCodigoAsync(codigoTipoOcorrencia);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Dominio.Entidades.Empresa empresaTerceiro = await repEmpresa.BuscarPorCNPJAsync(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                proprietario = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                transportador = Usuario.Empresa.Codigo;
                proprietario = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : string.Empty;
            }
            else
            {
                int.TryParse(Request.Params("Empresa"), out transportador);
                Dominio.Entidades.Empresa transportadoraSelecionada = await repEmpresa.BuscarPorCodigoAsync(transportador);
                proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
            }

            int.TryParse(Request.Params("Filial"), out int filial);

            dynamic lista = null;

            // Se existe um codigo, significa que esta listando as cargas de uma ocorrencia ja lancada
            // Caso contrários, trás as cargas de acordo com o periodo informado
            if (codigoOcorrencia > 0)
            {
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaSumarizado> cargas = await 
                    repCargaOcorrenciaSumarizado.ConsultarCargasOcorrenciaPorOcorrenciaAsync(codigoOcorrencia, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = await 
                    repCargaOcorrenciaSumarizado.ContarConsultaCargasOcorrenciaPorOcorrenciaAsync(codigoOcorrencia);

                lista = (from obj in cargas
                         select new
                         {
                             obj.Codigo,
                             CodigoVeiculo = obj.Veiculo.Codigo,
                             obj.QuantidadeDias,
                             Veiculo = obj.Veiculo.Placa,
                             ModeloVeicular = obj.Veiculo.ModeloVeicularCarga?.CodigoIntegracao ?? string.Empty,
                             obj.QuantidadeDocumentos,
                             obj.ValorMercadoria,
                         }).ToList();
            }
            else
            {
                if (propOrdenar == "Codigo")
                    propOrdenar = "CodigoVeiculo";
                if (propOrdenar == "ValorMercadoria")
                    propOrdenar = "ValorNotas";

                List<dynamic> cargasAlteradas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("CargasComplementadas"));

                List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaOcorrenciaSumarizado> cargas = await repCargaOcorrenciaSumarizado
                    .ConsultarCargasOcorrenciaPorPeriodoAsync(periodoInicial, periodoFim, transportador, filial, proprietario, tipoOcorrencia.FiltrarCargasPeriodo, tipoOcorrencia.Codigo, propOrdenar, dirOrdena, inicio, limite);
                totalRegistros = await repCargaOcorrenciaSumarizado.ContarConsultaCargasOcorrenciaPorPeriodoAsync(periodoInicial, periodoFim, transportador, filial, proprietario, tipoOcorrencia.FiltrarCargasPeriodo, tipoOcorrencia.Codigo);

                lista = (from obj in cargas
                         select new
                         {
                             Codigo = obj.CodigoVeiculo,
                             obj.CodigoVeiculo,
                             QuantidadeDias = QuantidadeDiasJaInformados(obj.CodigoVeiculo, obj.QuantidadeDias, cargasAlteradas),
                             obj.Veiculo,
                             ModeloVeicular = obj.ModeloVeicular,
                             obj.QuantidadeDocumentos,
                             ValorMercadoria = obj.ValorNotas,
                         }).ToList();
            }

            return lista;
        }

        private int QuantidadeDiasJaInformados(int veiculo, int quantidadeDias, List<dynamic> cargasAlteradas)
        {
            foreach(var obj in cargasAlteradas)
            {
                if ((int)obj.Veiculo == veiculo) return (int)obj.Dias;
            }

            return quantidadeDias;
        }

        private Models.Grid.Grid GridCargas()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            Models.Grid.EditableCell editarDias = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt);

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoVeiculo", false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Veiculo", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.QuantidadeDeDocumentos, "QuantidadeDocumentos", 15, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.QuantidadeDeDias, "QuantidadeDias", 10, Models.Grid.Align.center, true).Editable(editarDias).Ord(false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorMercadoria, "ValorMercadoria", 15, Models.Grid.Align.center, true);

            return grid;
        }
        private Models.Grid.Grid GridCargasDocumentos()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DescricaoCarga, "Carga", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataEmissao, "DataEmissao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "Placa", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Origem, "Origem", 17, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Destino, "Destino", 17, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ValorMercadoria, "ValorMercadoria", 10, Models.Grid.Align.right, true);

            return grid;
        }

        private void FiltrosPesquisa(out DateTime periodoInicial, out DateTime periodoFim, out int transportador, out int filial, out string proprietario, 
            Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            DateTime.TryParse(Request.Params("PeriodoInicio"), out periodoInicial);
            DateTime.TryParse(Request.Params("PeriodoFim"), out periodoFim);

            transportador = 0;
            proprietario = "";

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Dominio.Entidades.Empresa empresaTerceiro = repEmpresa.BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                transportador = empresaTerceiro != null ? empresaTerceiro.Codigo : 0;
                proprietario = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : string.Empty;
            }
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                transportador = Usuario.Empresa.Codigo;
                proprietario = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : string.Empty;
            }
            else
            {
                int.TryParse(Request.Params("Transportador"), out transportador);
                Dominio.Entidades.Empresa transportadoraSelecionada = repEmpresa.BuscarPorCodigo(transportador);
                proprietario = transportadoraSelecionada?.CNPJ_SemFormato ?? string.Empty;
            }

            int.TryParse(Request.Params("Filial"), out filial);
        }

        private async Task<dynamic> GerarPlanilhaRateioPeriodoAsync(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellation)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork, cancellation);

            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia> documentosRateados = new List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia>();
            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia> rateioOcorrencia = await repCargaOcorrencia.BuscarDadosParaRateioAsync(ocorrencia.Codigo);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = (from c in ocorrencia.Cargas where c.CargaCTes != null select c).FirstOrDefault();

            if (carga == null)
                return null;

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in carga.CargaCTes
                                                                        where
                                                                            o.CargaCTeComplementoInfo != null &&
                                                                            (o.CargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || o.CargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)                                                                            
                                                                        select o.CargaCTeComplementoInfo.CTe).FirstOrDefault();

            if (cte == null)
                return null;

            decimal valorTotal = rateioOcorrencia.Sum(o => o.ValorMercadoria);
            decimal somaValorRateado = 0;

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia rateio in rateioOcorrencia)
            {
                decimal valorRateado = decimal.Round((rateio.ValorMercadoria / valorTotal) * ocorrencia.ValorOcorrencia, 2, MidpointRounding.ToEven);
                somaValorRateado += valorRateado;

                documentosRateados.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia()
                {
                    CentroCusto = rateio.CentroCusto ?? rateio.CNPJ,
                    ValorMercadoria = rateio.ValorMercadoria,
                    ContaSAP = "",
                    NotaFiscal = cte.Numero,
                    Valor = valorRateado,
                    CNPJ = cte.Empresa.CNPJ_SemFormato,
                    NomePrestador = cte.Empresa.RazaoSocial,
                });
            }

            decimal diferenca = ocorrencia.ValorOcorrencia - somaValorRateado;
            if (diferenca != 0)
                documentosRateados[0].Valor += diferenca;

            return documentosRateados.OrderBy(o => o.CentroCusto).ToList();
        }

        private async Task<dynamic> GerarPlanilhaRateioContratoAsync(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = await repCargaCTeComplementoInfo.BuscarCTesPorOcorrenciaAsync(ocorrencia.Codigo, "", "", 0, 0);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in cargaCTesComplementoInfo
                                                                        where
                                                                            (o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || o.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) 
                                                                        select o.CTe).FirstOrDefault();

            if (cte == null)
                return null;

            if (ocorrencia.ContratoFrete == null)
                return null;

            if (ocorrencia.ContratoFrete.Clientes == null || ocorrencia.ContratoFrete.Clientes.Count() == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia> documentosRateados = new List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia>();
            
            decimal valorTotal = ocorrencia.ValorOcorrencia;
            int totalClientes = ocorrencia.ContratoFrete.Clientes.Count();
            decimal somaValorRateado = 0;

            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente cliente in ocorrencia.ContratoFrete.Clientes)
            {
                decimal valorRateado = decimal.Round(valorTotal / totalClientes, 2, MidpointRounding.ToEven);
                somaValorRateado += valorRateado;

                documentosRateados.Add(new Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia()
                {
                    NotaFiscal = cte.Numero,
                    ContaSAP = "",
                    CentroCusto = cliente.Cliente.CodigoIntegracao ?? cliente.Cliente.CPF_CNPJ_Formatado,
                    Valor = valorRateado,
                    CNPJ = cte.Empresa.CNPJ_SemFormato,
                    NomePrestador = cte.Empresa.RazaoSocial,
                });
            }

            decimal diferenca = ocorrencia.ValorOcorrencia - somaValorRateado;
            if (diferenca != 0)
                documentosRateados[0].Valor += diferenca;

            return documentosRateados.OrderBy(o => o.CentroCusto).ToList();
        }
    }
}
