using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/Provisao")]
    public class ProvisaoController : BaseController
    {
        #region Construtores

        public ProvisaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork).BuscarPrimeiroRegistro();

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.AdicionaProvisao parametros = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.AdicionaProvisao()
                {
                    Carga = Request.GetIntParam("Carga"),
                    Filial = Request.GetIntParam("Filial"),
                    Ocorrencia = Request.GetIntParam("Ocorrencia"),
                    RegraEscrituracao = Request.GetIntParam("TipoOperacaoDocumento"),
                    TipoOperacao = Request.GetIntParam("TipoOperacao"),
                    Tomador = Request.GetDoubleParam("Tomador"),
                    DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                    DataFim = Request.GetNullableDateTimeParam("DataFim"),
                    TipoLocalPrestacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao>("TipoLocalPrestacao"),
                    CodigosTransportadores = Request.GetListParam<int>("Transportador"),
                    TipoProvisao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao>("TipoProvisao"),
                    GerarLoteIndividualPorNfe = configuracaoFinanceiro.GerarLoteProvisaoIndividualNfe ?? false
                };

                int provisaoCodigo = 0;

                int quantidadeProvisoesGeradas = 0;

                if (!parametros.GerarLoteIndividualPorNfe)
                {
                    provisaoCodigo = GerarProvisao(parametros, unitOfWork);
                    quantidadeProvisoesGeradas = 1;
                    await unitOfWork.CommitChangesAsync();
                }
                else
                {
                    List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentos = BuscarDocumentosSelecionados(unitOfWork, out string erro, parametros.GerarLoteIndividualPorNfe);

                    List<int> numerosDocumentosComProvisao = documentos.Where(documento => documento.Provisao != null).Select(documento => documento.NumeroDocumento).ToList();

                    if (numerosDocumentosComProvisao.Count != 0)
                        throw new ControllerException($"Já existe provisão para o(s) documento(s) {string.Join(",", (numerosDocumentosComProvisao))}");

                    if (!string.IsNullOrWhiteSpace(erro))
                        throw new ControllerException(erro);

                    foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentos)
                    {
                        parametros.DocumentosProvisoes = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>() {
                            documento
                        };
                        parametros.Tomador = documento.Tomador.CPF_CNPJ;
                        parametros.Carga = documento.Carga.Codigo;

                        if (documento.Empresa != null)
                            parametros.CodigosTransportadores = new List<int>()
                            {
                                documento.Empresa.Codigo
                            };

                        quantidadeProvisoesGeradas++;
                        provisaoCodigo = GerarProvisao(parametros, unitOfWork);

                        await unitOfWork.CommitChangesAsync();
                    }
                }

                return new JsonpResult(new
                {
                    Codigo = quantidadeProvisoesGeradas > 1 ? 0 : provisaoCodigo
                });
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao tipoProvisao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao>("TipoProvisao");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigo, true);

                // Valida
                if (provisao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Busca os documentos selecionados
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentos = BuscarDocumentosSelecionados(unitOfWork, out string erro);

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);

                if (documentos.Count() == 0)
                    return new JsonpResult(false, true, "Nenhum documento selecionado.");

                if (documentos.Any(obj => obj.Carga != null && (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)))
                    return new JsonpResult(false, true, "Enquanto as notas estavam sendo selecionadas uma carga das notas selecionadas foi cancelada, por favor, refaça o processo.");

                new Servicos.Embarcador.Integracao.IntegracaoProvisao(unitOfWork).Adicionar(provisao, adicionarIntegracoesEDI: false);
                Dominio.Entidades.Auditoria.HistoricoObjeto historicoProvisao = repProvisao.Atualizar(provisao, Auditado);

                FiltrarDocumentosAtualizarProvisao(provisao, documentos, historicoProvisao, unitOfWork);
                AtualizarSumarizadoresProvisao(ref provisao, documentos);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    provisao.Codigo
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigo);

                if (provisao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                bool possuiIntegracao = repositorioProvisaoIntegracao.ContarPorProvisao(provisao.Codigo) > 0;
                bool possuiIntegracaoEDI = repositorioProvisaoEDIIntegracao.ContarPorProvisao(provisao.Codigo) > 0;

                var retorno = new
                {
                    provisao.Codigo,
                    provisao.Situacao,
                    DataInicial = provisao.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = provisao.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    provisao.Numero,
                    provisao.TipoLocalPrestacao,
                    provisao.GerandoMovimentoFinanceiroProvisao,
                    provisao.MotivoRejeicaoFechamentoProvisao,
                    TipoOperacaoDocumento = provisao.RegraEscrituracao?.Codigo ?? 0,
                    Transportador = (
                        from transportador in provisao.Transportadores
                        select new
                        {
                            transportador.Codigo,
                            transportador.Descricao
                        }
                    ).ToList(),
                    Filial = new { Codigo = provisao.Filial?.Codigo ?? 0, Descricao = provisao.Filial?.Descricao ?? "" },
                    TipoOperacao = new { Codigo = provisao.TipoOperacao?.Codigo ?? 0, Descricao = provisao.TipoOperacao?.Descricao ?? "" },
                    Tomador = new { Codigo = provisao.Tomador?.Codigo ?? 0, Descricao = provisao.Tomador?.Nome ?? "" },
                    Carga = new { Codigo = provisao.Carga?.Codigo ?? 0, Descricao = provisao.Carga?.Descricao ?? "" },
                    Ocorrencia = new { Codigo = provisao.CargaOcorrencia?.Codigo ?? 0, Descricao = provisao.CargaOcorrencia?.Descricao ?? "" },
                    provisao.DescricaoSituacao,
                    Documentos = configuracaoTMS.ProvisionarDocumentosEmitidos ? (
                        from o in provisao.DocumentosProvisao
                        select new
                        {
                            DT_RowId = o.Codigo,
                            o.Codigo,
                            o.NumeroDocumento
                        }
                    ).ToList() : null,
                    Resumo = new
                    {
                        provisao.Codigo,
                        Transportador = provisao.DescricaoTransportadores,
                        Filial = provisao.Filial?.Descricao ?? string.Empty,
                        Numero = provisao.Numero.ToString(),
                        DataInicial = provisao.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFinal = provisao.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Carga = provisao.Carga?.Descricao ?? "",
                        Ocorrencia = provisao.CargaOcorrencia?.Descricao ?? "",
                        Situacao = provisao.DescricaoSituacao,
                        Tomador = provisao.Tomador?.Nome ?? "",
                        QuantidadeDocumentos = provisao.QuantidadeDocsProvisao.ToString(),
                        ValorProvisao = provisao.ValorProvisao.ToString("n2"),
                        ValorFrete = (from documentos in provisao.DocumentosProvisao select documentos.ValorProvisao).ToList().Sum().ToString("n2"),
                    },
                    PossuiIntegracao = possuiIntegracao,
                    PossuiIntegracaoEDI = possuiIntegracaoEDI,
                    TipoProvisao = provisao.TipoProvisao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao.Nenhum ?
                    (configuracaoTMS.ProvisionarDocumentosEmitidos ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao.ProvisaoPorCTe : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao.ProvisaoPorNotaFiscal) : provisao.TipoProvisao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTiposOperacaoDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Escrituracao.RegraEscrituracao> regras = repRegraEscrituracao.BuscarRegrasAtivas();

                var retorno = (from o in regras
                               select new
                               {
                                   o.Codigo,
                                   o.Descricao
                               }).ToList();

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDocumento(exportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDocumentos()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaDocumento(exportarPesquisa: true);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> Reabrir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigo);

                // Valida
                if (provisao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (provisao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.Finalizado)
                    return new JsonpResult(false, true, "Só é possível reabrir provisões finalizadas.");

                // Formata retorno
                unitOfWork.Start();
                provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmAlteracao;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, provisao, null, "Reabriu a provisão", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarEmLotes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

                List<int> codigosProvisoes = Request.GetListParam<int>("ListaProvisoesSelecionadas");

                if (codigosProvisoes?.Count == 0 && !selecionarTodos)
                    return new JsonpResult(false, true, "É necessário selecionar ao menos uma provisão");

                List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> provisoes = repositorioProvisao.ObterProvisoesAgIntegracao(filtrosPesquisa, selecionarTodos, codigosProvisoes);

                if (provisoes.Count == 0)
                    return new JsonpResult(false, true, "Registros não encontrados");

                Servicos.Embarcador.Integracao.IntegracaoProvisao servicoIntegracaoProvisao = new Servicos.Embarcador.Integracao.IntegracaoProvisao(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao in provisoes)
                    servicoIntegracaoProvisao.Adicionar(provisao);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reintegrar");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracoesProvisaoPorNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork);

                bool primeiroOuSegundoDiaUtil = servicoProvisao.EhPrimeiroOuSegundoDiaUtilMes();

                var retorno = new
                {
                    AgruparProvisoesPorNotaFiscalFechamentoMensal = configuracaoFinanceiro?.AgruparProvisoesPorNotaFiscalFechamentoMensal ?? false,
                    EhPrimeiroOuSegundoDiaUtil = primeiroOuSegundoDiaUtil,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarSumarizadoresProvisao(ref Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentos)
        {
            int quantidade = 0;
            decimal valor = 0;

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentos)
            {
                quantidade++;

                if (documento.CTe != null)
                {
                    decimal valorLiquido = documento.ObterValorLiquido();

                    if (documento.CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Debito)
                        valor -= valorLiquido;
                    else
                        valor += valorLiquido;
                }
            }

            provisao.ValorProvisao = valor;
            provisao.QuantidadeDocsProvisao = quantidade;
        }

        private List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> BuscarDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork, out string erro, bool loteIndividualProvisao = false, bool transportadorObrigatorio = true)
        {
            erro = string.Empty;

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && transportadorObrigatorio)
            {
                if (!loteIndividualProvisao && Request.GetListParam<int>("Transportador").Count == 0)
                {
                    erro = "Transportador é obrigatório.";
                    return null;
                }
            }

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> listaBusca = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
            bool todosSelecionados = Request.GetBoolParam("SelecionarTodos");

            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = "Codigo"
                    };
                    ExecutaPesquisaDocumento(ref listaBusca, ref totalRegistros, parametrosConsulta, exportarPesquisa: false, unitOfWork: unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                foreach (dynamic dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.Remove(new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (dynamic dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repDocumentoProvisao.BuscarPorCodigo((int)dynSelecionada.Codigo));
            }

            // Retorna lista
            return listaBusca;
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            List<dynamic> listaRetornar = new List<dynamic>();
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa = ObterFiltrosPesquisaDocumento(unitOfWork);

            totalRegistros = repositorioDocumentoProvisao.ContarConsulta(filtrosPesquisa);
            listaGrid = totalRegistros > 0 ? repositorioDocumentoProvisao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();

            if (listaGrid.Count > 0)
            {
                string nomeArquivoEDI = ObterNomeArquivoEDI(unitOfWork, filtrosPesquisa.CodigoProvisao, exportarPesquisa);

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in listaGrid)
                {

                    if (((documento.Tomador?.GrupoPessoas?.ProvisionarDocumentos ?? false) && (documento.Tomador?.GrupoPessoas?.GerarSomenteUmaProvisaoCadaCargaCompleta ?? false)) && documento.Carga?.DataInicioEmissaoDocumentos == DateTime.MinValue)
                        continue;

                    listaRetornar.Add(new
                    {
                        TipoDocumentoEmissao = documento.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.CTe,
                        SituacaoCTe = documento.CTe?.Status ?? "",
                        CodigoEmpresa = documento.Empresa?.Codigo ?? 0,
                        CodigoCTE = documento.CTe?.Codigo ?? 0,
                        documento.Codigo,
                        CodigoFilial = documento.Filial?.Codigo ?? 0,
                        CodigoTomador = documento.Tomador?.Codigo ?? 0,
                        CodigoTransportador = documento.Empresa?.Codigo ?? 0,
                        Transportador = documento.Empresa?.Descricao ?? "",
                        Documento = documento.NumeroDocumento.ToString() + " - " + documento.SerieDocumento.ToString(),
                        Tipo = documento.ModeloDocumentoFiscal?.Abreviacao ?? "",
                        Carga = documento.Carga?.CodigoCargaEmbarcador ?? "",
                        CodigoCarga = documento.Carga?.Codigo ?? 0,
                        FechamentoFrete = documento.FechamentoFrete?.Numero.ToString() ?? "",
                        Ocorrencia = documento.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? "",
                        DataEmissao = documento.DataEmissao.ToString("dd/MM/yyyy"),
                        Tomador = documento.Tomador?.Descricao ?? "",
                        GerarProvisaoSomenteParaCarga = documento?.Tomador?.GrupoPessoas?.GerarSomenteUmaProvisaoCadaCargaCompleta ?? false,
                        Filial = documento.Filial?.Descricao ?? "",
                        RegraEscrituracao = documento.RegraEscrituracao?.Descricao ?? "",
                        TipoDocumentoCreditoDebito = documento.CTe?.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebitoDescricao ?? "",
                        ValorICMS = documento.ObterValorICMS().ToString("n2"),
                        ValorPISCOFINS = documento.ObterValorPISCOFINS().ToString("n2"),
                        ValorProvisao = documento.ObterValorLiquido().ToString("n2"),
                        ValorFrete = documento.ValorProvisao.ToString("n2"),
                        ValorFreteSemICMS = (documento.ValorProvisao - documento.ObterValorICMS()).ToString("n2"),
                        NomeArquivoEDI = nomeArquivoEDI
                    });
                }
            }

            return listaRetornar;
        }

        private void FiltrarDocumentosAtualizarProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosSelecionados, Dominio.Entidades.Auditoria.HistoricoObjeto historicoProvisao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            List<int> codigosSelecionados = (from o in documentosSelecionados select o.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosRemover = repDocumentoProvisao.BuscarPorProvisaoNaoPresenteNaLista(provisao.Codigo, codigosSelecionados);
            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao docRemover in documentosRemover)
            {
                //docRemover.Initialize();
                docRemover.Provisao = null;
                docRemover.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao;
                repDocumentoProvisao.Atualizar(docRemover);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, docRemover, null, "Removeu documento da provisão " + provisao.Descricao, unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, provisao, null, "Removeu documento " + docRemover.Descricao + " da provisão", unitOfWork);
            }

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosSelecionados)
            {
                //documento.Initialize();
                if (documento.Provisao == null)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Adicionou documento á provisão " + provisao.Descricao, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, provisao, null, "Adicionou documento " + documento.Descricao + " na provisão", unitOfWork);
                }
                documento.Provisao = provisao;
                repDocumentoProvisao.Atualizar(documento);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoLocalidadePrestacao = Request.GetIntParam("LocalidadePrestacao"),
                CodigoOcorrencia = Request.GetIntParam("Ocorrencia"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Numero = Request.GetIntParam("Numero"),
                NumeroDocumento = Request.GetIntParam("NumeroDOC"),
                SituacaoProvisao = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.Todos),
                TipoProvisao = Request.GetEnumParam("TipoProvisao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao.Nenhum)
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
            else
                filtrosPesquisa.CodigoTransportador = Request.GetIntParam("Empresa");

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao ObterFiltrosPesquisaDocumento(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumentoProvisao()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoProvisao = Request.GetIntParam("Codigo"),
                CodigoOcorrencia = Request.GetIntParam("Ocorrencia"),
                CodigoRegraEscrituracao = Request.GetIntParam("TipoOperacaoDocumento"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                ConcatenarComDocumentosSemPrevisao = false,
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                ListaCodigoTransportador = Request.GetListParam<int>("Transportador"),
                SituacaoProvisaoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.AgProvisao,
                SomenteSemProvisao = true,
                TipoLocalPrestacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao>("TipoLocalPrestacao"),
                TipoEtapasDocumentoProvisao = Request.GetBoolParam("TipoEtapasDocumentoProvisao"),
                NaoPermitirProvisionarSemCalculoFrete = configuracaoFinanceiro.NaoPermitirProvisionarSemCalculoFrete ?? false
            };

            if (filtrosPesquisa.CodigoProvisao > 0)
            {
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repositorioProvisao.BuscarPorCodigo(filtrosPesquisa.CodigoProvisao);

                // Quando a provisao estiver em alteracao, exibe os documentos da provisao + disponiveis
                if (provisao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmAlteracao)
                {
                    filtrosPesquisa.ListaCodigoTransportador = (from transportador in provisao.Transportadores select transportador.Codigo).ToList();
                    filtrosPesquisa.ConcatenarComDocumentosSemPrevisao = true;
                }

                filtrosPesquisa.SomenteSemProvisao = false;
                filtrosPesquisa.SituacaoProvisaoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.Todos;
            }

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Inicio", "DataInicio", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Qnt. Docs", "QuantidadeDocumentos", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Provisão", "ValorProvisao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo Provisão", "TipoProvisao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nome Arquivo", "NomeArquivo", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaProvisao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);
                int totalRegistros = repositorioProvisao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> listaProvisao = totalRegistros > 0 ? repositorioProvisao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.Provisao>();

                List<int> codigosProvisao = listaProvisao.Select(o => o.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao> listaIntegracao = repositorioProvisaoEDIIntegracao.BuscarPorProvisao(codigosProvisao).ToList();


                var listaProvisaoRetornar = (
                    from provisao in listaProvisao
                    select new
                    {
                        provisao.Codigo,
                        Carga = provisao?.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        Numero = provisao.Numero.ToString(),
                        DataInicio = provisao.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                        DataFim = provisao.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                        Tomador = provisao.Tomador?.Descricao ?? "",
                        QuantidadeDocumentos = provisao.QuantidadeDocsProvisao,
                        ValorProvisao = provisao.ValorProvisao.ToString("n2"),
                        Transportador = provisao.DescricaoTransportadores,
                        Filial = provisao.Filial?.Descricao ?? "",
                        provisao.DescricaoSituacao,
                        TipoProvisao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisaoHelper.ObterDescricao(provisao.TipoProvisao),
                        NomeArquivo = string.Join(", ", listaIntegracao.Where(obj => obj.Provisao.Codigo == provisao.Codigo).Select(obj => obj.NomeArquivo)),
                    }
                ).ToList();

                grid.AdicionaRows(listaProvisaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaDocumento(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoTomador", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("GerarProvisaoSomenteParaCarga", false);
                grid.AdicionarCabecalho("Documento", "Documento", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 7, Models.Grid.Align.left, true);

                if (repFechamentoFrete.VerificarExistemFechamento())
                    grid.AdicionarCabecalho("Fechamento de Contrato", "FechamentoFrete", 8, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 18, Models.Grid.Align.left, true);

                if (configuracaoTMS.ProvisionarDocumentosEmitidos)
                {
                    grid.AdicionarCabecalho("Regra", "RegraEscrituracao", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Tipo Doc.", "TipoDocumentoCreditoDebito", 10, Models.Grid.Align.left, true);
                }
                else
                {
                    grid.AdicionarCabecalho("RegraEscrituracao", false);
                    grid.AdicionarCabecalho("TipoDocumentoCreditoDebito", false);
                }

                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 10, Models.Grid.Align.right, true);

                if (exportarPesquisa)
                {
                    grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", 10, Models.Grid.Align.right, true);
                    grid.AdicionarCabecalho("Frete sem ICMS", "ValorFreteSemICMS", 10, Models.Grid.Align.right, true);
                    grid.AdicionarCabecalho("Valor PIS/COFINS", "ValorPISCOFINS", 10, Models.Grid.Align.right, true);
                    grid.AdicionarCabecalho("Valor Provisão", "ValorProvisao", 10, Models.Grid.Align.right, true);
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);
                }

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> listaGrid = new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>();
                int totalRegistros = 0;
                dynamic lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterNomeArquivoEDI(Repositorio.UnitOfWork unitOfWork, int codigoProvisao, bool exportarPesquisa)
        {
            if (exportarPesquisa & (codigoProvisao > 0))
            {
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorio = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao integracao = repositorio.BuscarPorProvisao(codigoProvisao).FirstOrDefault();

                if (integracao != null)
                    return Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, true, unitOfWork);
            }

            return "";
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Tomador")
                return "Tomador.Nome";

            if (propriedadeOrdenar == "Transportador")
                return "Empresa.RazaoSocial";

            if (propriedadeOrdenar == "DataInicio")
                return "DataInicial";

            if (propriedadeOrdenar == "DataFim")
                return "DataFinal";

            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "QuantidadeDocumentos")
                return "QuantidadeDocsProvisao";

            if (propriedadeOrdenar == "DescricaoSituacao")
                return "Situacao";

            return propriedadeOrdenar;
        }

        private string ObterPropriedadeOrdenarDocumentos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                propriedadeOrdenar = "NumeroDocumento";
            else if (propriedadeOrdenar == "Carga")
                propriedadeOrdenar = "Carga.CodigoCargaEmbarcador";
            else if (propriedadeOrdenar == "FechamentoFrete")
                return "FechamentoFrete.Numero";
            else if (propriedadeOrdenar == "Ocorrencia")
                propriedadeOrdenar = "CargaOcorrencia.NumeroOcorrencia";
            else if (propriedadeOrdenar == "Tipo")
                propriedadeOrdenar = "ModeloDocumentoFiscal.Abreviacao";
            else if (propriedadeOrdenar == "Destinatario")
                propriedadeOrdenar = "Destinatario.Nome";
            else if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "ValorFrete")
                propriedadeOrdenar = "ValorProvisao";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";
            else if (propriedadeOrdenar == "TipoDocumentoCreditoDebito")
                propriedadeOrdenar = "CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito";

            return propriedadeOrdenar;
        }

        private void SetarDocumentosSelecionados(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, Dominio.ObjetosDeValor.Embarcador.Escrituracao.AdicionaProvisao parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);

            int totalRegistros = 0;
            decimal valorProvisao = 0;
            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);

            if (parametros.GerarLoteIndividualPorNfe && parametros.DocumentosProvisoes != null && parametros.DocumentosProvisoes.Count() == 1)
            {
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = parametros.DocumentosProvisoes.First();
                documentoProvisao.Provisao = provisao;
                documentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento;

                repDocumentoProvisao.Atualizar(documentoProvisao);

                provisao.ValorProvisao = documentoProvisao.ValorProvisao;
                provisao.QuantidadeDocsProvisao = 1;
            }
            else if (todosSelecionados)
            {
                List<int> codigosNaoSelecionadas = new List<int>();
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                foreach (dynamic dynNaoSelecionada in listaNaoSelecionadas)
                    codigosNaoSelecionadas.Add((int)dynNaoSelecionada.Codigo);

                provisao.QuantidadeDocsProvisao = repDocumentoProvisao.SetarDocumentosParaProvisao(provisao.Codigo, parametros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento, codigosNaoSelecionadas);

                if (parametros.GerarLoteIndividualPorNfe)
                    provisao.ValorProvisao = parametros.DocumentosProvisoes.FirstOrDefault().ValorProvisao;
                else
                    provisao.ValorProvisao = repDocumentoProvisao.ValorTotalPorProvisao(provisao.Codigo);
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (dynamic dynSelecionada in listaSelecionadas)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repDocumentoProvisao.BuscarPorCodigo((int)dynSelecionada.Codigo, false);
                    documentoProvisao.Provisao = provisao;
                    documentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisaoDocumento.EmFechamento;
                    repDocumentoProvisao.Atualizar(documentoProvisao);
                    totalRegistros++;
                    valorProvisao += documentoProvisao.ValorProvisao;
                }
                provisao.ValorProvisao = valorProvisao;
                provisao.QuantidadeDocsProvisao = totalRegistros;
            }

            provisao.GerandoMovimentoFinanceiroProvisao = true;
            repProvisao.Atualizar(provisao);
        }

        private int GerarProvisao(Dominio.ObjetosDeValor.Embarcador.Escrituracao.AdicionaProvisao parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.RegraEscrituracao repRegraEscrituracao = new Repositorio.Embarcador.Escrituracao.RegraEscrituracao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = new Dominio.Entidades.Embarcador.Escrituracao.Provisao();

            provisao.DataCriacao = DateTime.Now;
            provisao.Numero = Servicos.Embarcador.Escrituracao.ProvisaoSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            provisao.TipoLocalPrestacao = parametros.TipoLocalPrestacao;
            provisao.GeradoManualmente = true;

            if (parametros.DataInicio.HasValue)
                provisao.DataInicial = parametros.DataInicio.Value;

            if (parametros.DataFim.HasValue)
                provisao.DataFinal = parametros.DataFim.Value;

            if (parametros.Tomador > 0)
                provisao.Tomador = repCliente.BuscarPorCPFCNPJ(parametros.Tomador);

            if (parametros.Filial > 0)
                provisao.Filial = repFilial.BuscarPorCodigo(parametros.Filial);

            if (provisao.Tomador != null && (provisao.Tomador?.GrupoPessoas?.GerarSomenteUmaProvisaoCadaCargaCompleta ?? false))
            {
                if (parametros.Carga == 0)
                    throw new ControllerException("Tomador exige informar uma carga para gerar Provisão");

                provisao.Carga = repCarga.BuscarPorCodigo(parametros.Carga);

                if (provisao.Carga == null)
                    throw new ControllerException($"Carga Não encontrada pelo codigo {parametros.Carga}");

                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));

                if (listaSelecionadas == null || (listaNaoSelecionadas != null && listaNaoSelecionadas.Count > 0))
                    throw new ControllerException($"Precisa informar todos os documentos da carga para gerar provisão");

            }
            else if (parametros.Carga > 0)
                provisao.Carga = repCarga.BuscarPorCodigo(parametros.Carga);

            if (parametros.Ocorrencia > 0)
                provisao.CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(parametros.Ocorrencia);

            if (parametros.RegraEscrituracao > 0)
                provisao.RegraEscrituracao = repRegraEscrituracao.BuscarPorCodigo(parametros.RegraEscrituracao);

            if (parametros.TipoOperacao > 0)
                provisao.TipoOperacao = repTipoOperacao.BuscarPorCodigo(parametros.TipoOperacao);

            if (parametros.CodigosTransportadores.Count > 0)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                provisao.Transportadores = new List<Dominio.Entidades.Empresa>();

                foreach (int codigoTransportador in parametros.CodigosTransportadores)
                {
                    Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(codigoTransportador);

                    if (transportador != null)
                        provisao.Transportadores.Add(transportador);
                }
            }

            if (parametros.TipoProvisao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao.Nenhum)
                provisao.TipoProvisao = parametros.TipoProvisao;

            if (parametros.TipoProvisao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoProvisao.ProvisaoPorCTe)
            {
                provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.AgIntegracao;
                repProvisao.Inserir(provisao, Auditado);

                parametros.DocumentosProvisoes = BuscarDocumentosSelecionados(unitOfWork, out string erro);

                if (!string.IsNullOrWhiteSpace(erro))
                    throw new ControllerException(erro);

                if (parametros.DocumentosProvisoes.Count == 0)
                    throw new ControllerException("Nenhum documento selecionado.");

                if (parametros.DocumentosProvisoes.Exists(obj => obj.Carga != null && (obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || obj.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)))
                    throw new ControllerException("Enquanto as notas estavam sendo selecionadas uma carga das notas selecionadas foi cancelada, por favor, refaça o processo.");

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in parametros.DocumentosProvisoes)
                {
                    documento.Provisao = provisao;
                    repDocumentoProvisao.Atualizar(documento);

                }
                AtualizarSumarizadoresProvisao(ref provisao, parametros.DocumentosProvisoes);

                repProvisao.Atualizar(provisao);

                new Servicos.Embarcador.Integracao.IntegracaoProvisao(unitOfWork).Adicionar(provisao);
            }
            else
            {
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarSeExistePagamentoEmFechamento();
                if (pagamento != null)
                {
                    unitOfWork.Rollback();
                    throw new ControllerException("Não é possível iniciar a provisão enquanto o pagamento " + pagamento.Numero + " estiver em fechamento.");
                }

                if (!parametros.GerarLoteIndividualPorNfe)
                    parametros.DocumentosProvisoes = BuscarDocumentosSelecionados(unitOfWork, out string erro, transportadorObrigatorio: false);

                provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.EmFechamento;
                repProvisao.Inserir(provisao, Auditado);

                SetarDocumentosSelecionados(provisao, parametros, unitOfWork);
            }

            return provisao.Codigo;
        }

        #endregion
    }
}
