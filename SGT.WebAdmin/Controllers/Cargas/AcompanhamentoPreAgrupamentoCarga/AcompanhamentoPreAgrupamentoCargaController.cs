using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Text.RegularExpressions;

namespace SGT.WebAdmin.Controllers.Cargas.AcompanhamentoPreAgrupamentoCarga
{
    [CustomAuthorize("Cargas/AcompanhamentoPreAgrupamentoCarga")]
    public class AcompanhamentoPreAgrupamentoCargaController : BaseController
    {
        #region Construtores

        public AcompanhamentoPreAgrupamentoCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> DownloadArquivoIntegracao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);
                string nomeArquivoIntegracao = servicoIntegracaoOrtec.ObterNomeArquivoIntegracaoPorPreAgrupamentoCarga(codigo);
                byte[] arquivoBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivoIntegracao);

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, contentType: "text/xml", fileDownloadName: Path.GetFileName(nomeArquivoIntegracao));
                else
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o arquivo de integração.");
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer download do arquivo de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repositorio = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador preAgrupamentoCargaAgrupador = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (preAgrupamentoCargaAgrupador == null)
                    return new JsonpResult(false, true, "Pré agrupamento de carga não encontrado.");

                if (preAgrupamentoCargaAgrupador.Situacao != SituacaoPreAgrupamentoCarga.ProblemaCarregamento)
                    return new JsonpResult(false, true, "A atual situação do agrupamento não permite essa operação.");

                unitOfWork.Start();
                if (preAgrupamentoCargaAgrupador.PossuiPreCargas)
                {
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoCargasPararEncaixe;
                    preAgrupamentoCargaAgrupador.PossuiPreCargas = false;
                }
                else if (preAgrupamentoCargaAgrupador.IsTodosAgrupamentosPossuemCarga(preAgrupamentoCargaAgrupador.PossuiPreCargas))
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoCarregamento;
                else
                    preAgrupamentoCargaAgrupador.Situacao = SituacaoPreAgrupamentoCarga.AguardandoRedespacho;

                repositorio.Atualizar(preAgrupamentoCargaAgrupador);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, preAgrupamentoCargaAgrupador, "Reenviou para processamento.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar processamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPreAgrupamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPreAgrupamentoCarga = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);

                servicoIntegracaoOrtec.ExcluirPorPreAgrupamentoCarga(codigoPreAgrupamentoCarga, Auditado);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o agrupamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhes()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorio = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga preAgrupamentoCarga = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (preAgrupamentoCarga == null)
                    return new JsonpResult(false, true, "Pré agrupamento de carga não encontrado.");

                Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal repositorioPreAgrupamentoNotaFiscal = new Repositorio.Embarcador.Cargas.PreAgrupamentoNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoNotaFiscal> notasFiscais = repositorioPreAgrupamentoNotaFiscal.BuscarPorPreAgrupamentoCarga(preAgrupamentoCarga.Codigo);

                var detalhes = new
                {
                    preAgrupamentoCarga.Codigo,
                    CodigoAgrupador = preAgrupamentoCarga.Agrupador.Codigo,
                    preAgrupamentoCarga.Agrupador.CodigoAgrupamento,
                    preAgrupamentoCarga.CodigoViagem,
                    Carga = preAgrupamentoCarga.Carga?.CodigoCargaEmbarcador ?? "",
                    CnpjEmitente = string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjEmitente.ObterSomenteNumeros()) ? preAgrupamentoCarga.CnpjEmitente : preAgrupamentoCarga.CnpjEmitente.ObterSomenteNumeros().ObterCnpjFormatado(),
                    Situacao = preAgrupamentoCarga.Agrupador.Situacao.ObterDescricao(),
                    EnumSituacao = preAgrupamentoCarga.Agrupador.Situacao,
                    preAgrupamentoCarga.Agrupador.Pendencia,
                    preAgrupamentoCarga.TipoViagem,
                    NotasFiscais = (
                        from notaFiscal in notasFiscais
                        select new
                        {
                            notaFiscal.CnpjDestinatario,
                            notaFiscal.CnpjEmitente,
                            notaFiscal.NumeroNota,
                            notaFiscal.SerieNota
                        }
                    ).ToList()
                };

                return new JsonpResult(detalhes);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes do pré agrupamento de carga.");
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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPreAgrupamentoCarga ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Emitente"));
            Dominio.Entidades.Cliente recebedor = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Recebedor"));
            Dominio.Entidades.Cliente expedidor = repCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Expedidor"));
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPreAgrupamentoCarga()
            {
                CodigoAgrupamento = Request.GetIntParam("Agrupamento"),
                CodigoViagem = Request.GetStringParam("Viagem"),
                NumeroNota = Request.GetStringParam("NumeroNota"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataPrevisaoEntregaInicial = Request.GetNullableDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetNullableDateTimeParam("DataPrevisaoEntregaFinal"),
                Situacao = Request.GetNullableEnumParam<SituacaoPreAgrupamentoCarga>("Situacao"),
                Emitente = emitente?.CPF_CNPJ_SemFormato ?? "",
                Recebedor = recebedor?.CPF_CNPJ_SemFormato ?? "",
                Expedidor = expedidor?.CPF_CNPJ_SemFormato ?? ""
            };
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
                grid.AdicionarCabecalho("Código Agrupamento", "CodigoAgrupamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emitente", "CnpjEmitente", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Placa", "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Expedidor", "CnpjExpedidor", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Recebedor", "CnpjRecebedor", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo Viagem", "TipoViagem", 7, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Código Viagem", "CodigoViagem", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número Nota", "NumeroNota", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Integração", "DataIntegracao", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Cargas/AcompanhamentoPreAgrupamentoCarga", "grid-pesquisa-pre-agrupamento-carga");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPreAgrupamentoCarga filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.PreAgrupamentoCarga repositorio = new Repositorio.Embarcador.Cargas.PreAgrupamentoCarga(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaPreAgrupamentoCarga(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga> listaPreAgrupamentoCarga = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCarga>();

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                List<double> cnpjsEmitente = (from obj in listaPreAgrupamentoCarga where String.IsNullOrWhiteSpace(obj.CnpjEmitente) != true && Regex.IsMatch(obj.CnpjEmitente, @"^\d+$") select double.Parse(obj.CnpjEmitente)).Distinct().ToList();
                List<Dominio.Entidades.Cliente> emitentes = repCliente.BuscarPorCPFCNPJs(cnpjsEmitente);

                List<double> cnpjsExpedidor = (from obj in listaPreAgrupamentoCarga where String.IsNullOrWhiteSpace(obj.CnpjExpedidor) != true && Regex.IsMatch(obj.CnpjExpedidor, @"^\d+$") select double.Parse(obj.CnpjExpedidor)).Distinct().ToList();
                List<Dominio.Entidades.Cliente> expedidores = repCliente.BuscarPorCPFCNPJs(cnpjsExpedidor);

                List<double> cnpjsRecebedor = (from obj in listaPreAgrupamentoCarga where String.IsNullOrWhiteSpace(obj.CnpjRecebedor) != true && Regex.IsMatch(obj.CnpjRecebedor, @"^\d+$") select double.Parse(obj.CnpjRecebedor)).Distinct().ToList();
                List<Dominio.Entidades.Cliente> recebedores = repCliente.BuscarPorCPFCNPJs(cnpjsRecebedor);



                var listaPreAgrupamentoCargaRetornar = (
                    from preAgrupamentoCarga in listaPreAgrupamentoCarga
                    select new
                    {
                        preAgrupamentoCarga.Codigo,
                        preAgrupamentoCarga.Agrupador.CodigoAgrupamento,
                        DataIntegracao = preAgrupamentoCarga.Agrupador.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss"),
                        preAgrupamentoCarga.CodigoViagem,
                        CnpjEmitente = !string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjEmitente?.ObterSomenteNumeros()) ? ((from obj in emitentes where obj.CPF_CNPJ_SemFormato == preAgrupamentoCarga.CnpjEmitente select obj).FirstOrDefault()?.Descricao ?? "") : (preAgrupamentoCarga.CnpjEmitente?.ObterSomenteNumeros().ObterCnpjFormatado() ?? ""),
                        CnpjExpedidor = !string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjExpedidor?.ObterSomenteNumeros()) ? ((from obj in expedidores where obj.CPF_CNPJ_SemFormato == preAgrupamentoCarga.CnpjExpedidor select obj).FirstOrDefault()?.Descricao ?? "") : (preAgrupamentoCarga.CnpjExpedidor?.ObterSomenteNumeros().ObterCnpjFormatado() ?? ""),
                        CnpjRecebedor = !string.IsNullOrWhiteSpace(preAgrupamentoCarga.CnpjRecebedor?.ObterSomenteNumeros()) ? ((from obj in recebedores where obj.CPF_CNPJ_SemFormato == preAgrupamentoCarga.CnpjRecebedor select obj).FirstOrDefault()?.Descricao ?? "") : (preAgrupamentoCarga.CnpjRecebedor?.ObterSomenteNumeros().ObterCnpjFormatado() ?? ""),
                        Placa = preAgrupamentoCarga.Agrupador.Veiculo?.Placa ?? "",
                        Transportador = preAgrupamentoCarga.Agrupador.Empresa?.Descricao ?? "",
                        preAgrupamentoCarga.NumeroNota,
                        Situacao = preAgrupamentoCarga.Agrupador.Situacao.ObterDescricao(),
                        preAgrupamentoCarga.TipoViagem,
                        DT_RowColor = preAgrupamentoCarga.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaPreAgrupamentoCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
