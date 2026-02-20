using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/DocumentoDestinadoEmpresa")]
    public class DocumentoDestinadoEmpresaController : BaseController
    {
        #region Construtores

        public DocumentoDestinadoEmpresaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = GridPesquisa();

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unidadeDeTrabalho, "DocumentoDestinadoEmpresa/Pesquisa", "grid-documento-destino-empresa");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracoes = repIntegracao.BuscarPorCodigoDocumentoDestinadoEmpresa(documentosDestinados.Select(o => o.Codigo).ToList());

                if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                {
                    List<(int codigoIntegracao, long codigoDocumento)> codigoDOcumentos = integracoes.Select(x => (x.Codigo, x.DocumentoDestinadoEmpresa.Codigo)).ToList();
                    documentosDestinados = documentosDestinados.Where(x => codigoDOcumentos.Any(a => a.codigoDocumento == x.Codigo) && DocumentoValidoRetornar(x, integracoes, filtrosPesquisa.SituacaoIntegracao.Value)).ToList();
                }

                grid.setarQuantidadeTotal(repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa));

                var retorno = (from obj in documentosDestinados
                               select new
                               {
                                   obj.Codigo,
                                   TipoDocumento = obj.DescricaoTipoDocumento,
                                   CodigoTipoDocumento = (int)obj.TipoDocumento,
                                   TipoOperacao = obj.DescricaoTipoOperacao,
                                   obj.Numero,
                                   obj.Serie,
                                   DataAutorizacao = obj.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   DataIntegracao = obj.DataIntegracao?.ToString("dd/MM/yyyy HH:mm"),
                                   obj.Chave,
                                   Emitente = obj.CPFCNPJEmitente + " - " + obj.NomeEmitente,
                                   Destinatario = obj.Empresa.CNPJ + " - " + obj.Empresa.NomeFantasia,
                                   Valor = obj.Valor.ToString("n2"),
                                   Cancelado = obj.Cancelado ? "Cancelado" : "Autorizado",
                                   NotasDoCTe = obj.NumeroNotasCTe,
                                   SituacaoManifestacaoDestinatario = obj.DescricaoSituacaoManifestacaoDestinatario,
                                   DocumentoCTe = obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeOSDestinadoTomador,
                                   DocumentoMDFe = obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFeDestinado ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.MDFECancelado ||
                                       obj.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe,
                                   DT_RowClass = obj.DocumentoEntrada != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Info(IntensidadeCor._100) : null,
                                   CodigoDocumentoEntrada = obj.DocumentoEntrada?.Codigo ?? 0,
                                   FornecedorDocumentoEntrada = obj.DocumentoEntrada?.Fornecedor?.Descricao ?? "",
                                   FornecedorOrdemCompra = obj.DocumentoEntrada?.OrdemCompra?.Fornecedor?.Descricao ?? "",
                                   SituacaoIntegracao = integracoes.Count > 0 ? ObterDescricaoIntegracao(integracoes, obj.Codigo) : "Sem Integração",
                                   DocumentoCTeNaoComplementarPossuiOcorrencia = repCargaOcorrenciaDocumento.ExisteOcorrenciaPorChaveCTeNaoComplementar(obj?.Chave ?? string.Empty)
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarStatusSefazPorChave(Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtroPesquisa)
        {
            filtroPesquisa.Chave = filtroPesquisa.Chave.ObterSomenteNumeros();
            if (filtroPesquisa.Chave.Length != 44)
                return new JsonpResult(false, "A chave do documento deve ser informada com 44 caracteres numéricos.");

            if (filtroPesquisa.Chave.Substring(6, 14) == Usuario.CNPJEmbarcador)
                return new JsonpResult(false, "Só é permitido a consulta de documentos destinados.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarNFePorChave(filtroPesquisa.Chave);

                if (documentoDestinado == null && filtroPesquisa.CodigoEmpresa == 0)
                    return new JsonpResult(false, "Documento destinado não encontrado no repositório, para consultar essa chave na SEFAZ, deverá informar o campo empresa.");

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa cancelamento = repDocumentoDestinadoEmpresa.BuscarCancelamentoPorChave(filtroPesquisa.Chave);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();

                int codEmpresaDestinatario = documentoDestinado != null ? repEmpresa.BuscarCodigoPorCNPJ(documentoDestinado.Empresa.CNPJ_SemFormato) : filtroPesquisa.CodigoEmpresa;
                List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();
                if (documentoDestinado?.Cancelado == true)
                {
                    var dataCanc = "";
                    if (cancelamento?.DataAutorizacao != null)
                        dataCanc = "<br/>data cancelamento: " + cancelamento.DataAutorizacao?.ToString("dd/MM/yyyy HH:mm");
                    return new JsonpResult(true, "O status do documento é: " + "CANCELADO" + dataCanc + "<br/><b>Origem:</b> Banco de dados.");
                }

                Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoConsultaStatusSefaz retorno = Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentoDestinadoEmpresaPorChave(filtroPesquisa.Chave, cnpjsNaoImportar, codEmpresaDestinatario, _conexao.StringConexao);

                return new JsonpResult(retorno.HouveConsula, retorno.Mensagem + (retorno.HouveConsula ? " <br/><b>Origem:</b> Sefaz." : ""));

            }
            catch (ServicoException ex)
            {
                return new JsonpResult(false, ex.Message + " <br/><b>Origem:</b> Sefaz.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar o status do documento na sefaz.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaParaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = GridPesquisaParaCarga();

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa));

                var retorno = (from obj in documentosDestinados
                               select new
                               {
                                   obj.Codigo,
                                   TipoOperacao = obj.DescricaoTipoOperacao,
                                   obj.Numero,
                                   obj.Serie,
                                   DataAutorizacao = obj.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   DataIntegracao = obj.DataIntegracao?.ToString("dd/MM/yyyy HH:mm"),
                                   obj.Chave,
                                   Emitente = obj.CPFCNPJEmitente_Formatado + " - " + obj.NomeEmitente,
                                   Destinatario = obj.Empresa.CNPJ_Formatado + " - " + obj.Empresa.NomeFantasia,
                                   Valor = obj.Valor.ToString("n2")
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = GridPesquisa();

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                if (repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa) > 5000)
                    return new JsonpResult(false, true, "Exportação não permitida para mais de 5000 registros.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                var retorno = (from obj in documentosDestinados
                               select new
                               {
                                   obj.Codigo,
                                   TipoDocumento = obj.DescricaoTipoDocumento,
                                   TipoOperacao = obj.DescricaoTipoOperacao,
                                   obj.Numero,
                                   obj.Serie,
                                   DataEmissao = obj.DataEmissao != null && obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   DataAutorizacao = obj.DataAutorizacao != null && obj.DataAutorizacao.HasValue ? obj.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   DataIntegracao = obj.DataIntegracao != null && obj.DataIntegracao.HasValue ? obj.DataIntegracao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   obj.Chave,
                                   NotasDoCTe = obj.NumeroNotasCTe,
                                   Emitente = obj.CPFCNPJEmitente + " - " + obj.NomeEmitente,
                                   Destinatario = obj.Empresa != null ? obj.Empresa.CNPJ + " - " + obj.Empresa.NomeFantasia : string.Empty,
                                   Valor = obj.Valor.ToString("n2"),
                                   Cancelado = obj.Cancelado ? "Sim" : "Não",
                                   SituacaoManifestacaoDestinatario = obj.DescricaoSituacaoManifestacaoDestinatario,
                                   SituacaoIntegracao = string.Empty
                               }).ToList();

                grid.AdicionaRows(retorno);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAuditoriaOrdemServico()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocumentoEntrada;
                int.TryParse(Request.Params("Codigo"), out codigoDocumentoEntrada);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Produto", "Produto", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Qtd. NF", "QuantidadeNF", 11, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Qtd. Ordem", "QuantidadeOrdem", 11, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CheckQuantidade", false);
                grid.AdicionarCabecalho("Valor NF", "ValorNota", 11, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor Ordem", "ValorOrdem", 11, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CheckValor", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Compras.OrdemCompra _repositorioNotaEntradaOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeDeTrabalho);

                Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioNotaEntradaOrdemCompra()
                {
                    CodigoNota = codigoDocumentoEntrada
                };
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoAgrupar = "",
                    DirecaoOrdenar = "",
                    InicioRegistros = grid.inicio,
                    LimiteRegistros = grid.limite,
                    PropriedadeAgrupar = "",
                    PropriedadeOrdenar = "Produto"
                };

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                IList<Dominio.Relatorios.Embarcador.DataSource.Compras.NotaEntradaOrdemCompra> lista = _repositorioNotaEntradaOrdemCompra.ConsultarRelatorioNotaEntradaOrdemCompra(filtrosPesquisa, agrupamentos, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);

                grid.setarQuantidadeTotal(_repositorioNotaEntradaOrdemCompra.ContarConsultaRelatorioNotaEntradaOrdemCompra(filtrosPesquisa, agrupamentos));

                var retorno = (from obj in lista
                               select new
                               {
                                   Codigo = 1,
                                   obj.Produto,
                                   QuantidadeNF = obj.QuantidadeNF.ToString("n4"),
                                   QuantidadeOrdem = obj.QuantidadeOrdem.ToString("n4"),
                                   CheckQuantidade = "fal fa-ban",
                                   ValorNota = obj.ValorNota.ToString("n2"),
                                   ValorOrdem = obj.ValorOrdem.ToString("n2"),
                                   CheckValor = "fal fa-ban",
                                   DT_RowClass = (obj.QuantidadeNF != obj.QuantidadeOrdem || obj.ValorNota != obj.ValorOrdem) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(IntensidadeCor._100) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Sucess(IntensidadeCor._100)
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaManifestacaoDestinatario()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocumentoDestinado;
                int.TryParse(Request.Params("Codigo"), out codigoDocumentoDestinado);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno SEFAZ", "Retorno", 20, Models.Grid.Align.left, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinadoEmpresa = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigoDocumentoDestinado);

                grid.setarQuantidadeTotal(documentoDestinadoEmpresa.Manifestacoes.Count());

                var retorno = (from obj in documentoDestinadoEmpresa.Manifestacoes
                               select new
                               {
                                   obj.Codigo,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                   obj.DescricaoTipo,
                                   obj.Protocolo,
                                   obj.DescricaoSituacao,
                                   Retorno = obj.CodigoStatusResposta + " - " + obj.DescricaoStatusResposta
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarDocumentosDestinados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = repGrupoPessoas.BuscarQueNaoImportaDocumentoDestinadoTransporte();

                List<string> cnpjsNaoImportar = (from obj in grupoPessoas select obj.Clientes.Select(o => o.CPF_CNPJ_SemFormato)).SelectMany(o => o).ToList();

                List<int> codigosEmpresas = new List<int>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigosEmpresas.Add(this.Empresa?.Codigo ?? 0);
                else
                    codigosEmpresas = repEmpresa.BuscarCodigosEmpresasAtivasDocumentosDestinados();

                foreach (int codigoEmpresa in codigosEmpresas)
                {
#if DEBUG
                    if (codigoEmpresa == 3 || codigoEmpresa == 22667)
#endif
                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(codigoEmpresa, _conexao.StringConexao, cnpjsNaoImportar, out string msgErro, out string codigoStatusRetornoSefaz);

                    unidadeTrabalho = null;
                    unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar os documentos destinados.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirDesacordoCTeLote()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<long> codigosTitulos = JsonConvert.DeserializeObject<List<long>>(Request.Params("ListaDocumentos"));
                int codigoMotivo = Request.GetIntParam("Motivo");
                string justificativa = Request.GetStringParam("Justificativa");

                Repositorio.Embarcador.Documentos.EventoDesacordoServico repEventoDesacordoServico = new Repositorio.Embarcador.Documentos.EventoDesacordoServico(unidadeDeTrabalho);
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unidadeDeTrabalho);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivoDesacordo = codigoMotivo > 0 ? repMotivoDesacordo.BuscarPorCodigo(codigoMotivo) : null;

                if (repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa) > 500)
                    return new JsonpResult(false, true, "Existem muitos documentos para os filtros selecionados. O máximo de documentos permitidos é de 500 por vez.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.ObterDocumentosDestinados(filtrosPesquisa, selecionarTodos, codigosTitulos);

                if (repMotivoDesacordo.ExisteCadastrado() && motivoDesacordo == null)
                    return new JsonpResult(false, true, "Não é permitido emitir um Desacordo sem um Motivo.");

                if (documentosDestinados.Any(o => o.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador && o.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload))
                    return new JsonpResult(false, true, "Evento de desacordo permitido apenas quando tipo for Destinado Tomador.");

                unidadeDeTrabalho.Start();
                foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado in documentosDestinados)
                {
                    Dominio.Entidades.Estado estadoEmissorDocumento = repEstado.BuscarPorIBGE(int.Parse(documentoDestinado.Chave.Substring(0, 2)));
                    string url = "";
                    string mensagemRetorno = "";

                    if (estadoEmissorDocumento == null)
                        return new JsonpResult(false, false, "Estado do emissor do documento não encontrado.");

                    if (documentoDestinado.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                    {
                        if (estadoEmissorDocumento.SefazCTe == null)
                            return new JsonpResult(false, false, "Estado do emissor do documento sem Sefaz configurado.");
                        url = estadoEmissorDocumento.SefazCTe.UrlRecepcaoEvento;
                    }
                    else
                    {
                        if (estadoEmissorDocumento.SefazCTeHomologacao == null)
                            return new JsonpResult(false, false, "Estado do emissor do documento sem Sefaz Homologação configurado.");
                        url = estadoEmissorDocumento.SefazCTeHomologacao.UrlRecepcaoEvento;
                    }

                    if (string.IsNullOrWhiteSpace(url))
                        return new JsonpResult(false, false, "Sefaz do emissor do documento sem URL de Recepção de Evento configurado.");

                    Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo retornoEnvioAprovacao = Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EnviarParaAprovacao(documentoDestinado, motivoDesacordo, unidadeDeTrabalho, Auditado);

                    if (!string.IsNullOrWhiteSpace(retornoEnvioAprovacao.mensagem))
                        return new JsonpResult(retornoEnvioAprovacao.status ?? false, true, retornoEnvioAprovacao.mensagem);

                    bool desacordoRealizado = false;

                    string versaoCte = ObterVersaoCte(documentoDestinado.Chave, documentoDestinado.CPFCNPJDestinatario, unidadeDeTrabalho);

                    desacordoRealizado = Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EmitirDesacordoServicoVersao4(documentoDestinado.Empresa.Codigo, documentoDestinado.Chave, justificativa, url, ref mensagemRetorno, unidadeDeTrabalho);

                    if (desacordoRealizado)
                    {
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.DescordoServico;
                        repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Emitiu desacordo em lote.", unidadeDeTrabalho);
                    }
                }
                unidadeDeTrabalho.Rollback();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao emitir o desacordo em lote.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirDesacordoCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                VerificarPermissaoPersonalizada();

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario tipoManifestacao;
                Enum.TryParse(Request.Params("TipoManifestacao"), out tipoManifestacao);

                string justificativa = Request.Params("Justificativa");

                int codigoMotivo = Request.GetIntParam("Motivo");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo motivoDesacordo = codigoMotivo > 0 ? repMotivoDesacordo.BuscarPorCodigo(codigoMotivo) : null;

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (documentoDestinado.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador && documentoDestinado.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.AutorizadoDownload)
                    return new JsonpResult(false, true, "Evento de desacordo permitido apenas quando tipo for Destinado Tomador.");

                if (repMotivoDesacordo.ExisteCadastrado() && motivoDesacordo == null)
                    return new JsonpResult(false, true, "Não é permitido emitir um Desacordo sem um Motivo.");

                Dominio.Entidades.Estado estadoEmissorDocumento = repEstado.BuscarPorIBGE(int.Parse(documentoDestinado.Chave.Substring(0, 2)));
                string url = "";
                string mensagemRetorno = "";

                if (estadoEmissorDocumento == null)
                    return new JsonpResult(false, false, "Estado do emissor do documento não encontrado.");

                if (documentoDestinado.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (estadoEmissorDocumento.SefazCTe == null)
                        return new JsonpResult(false, false, "Estado do emissor do documento sem Sefaz configurado.");
                    url = estadoEmissorDocumento.SefazCTe.UrlRecepcaoEvento;
                }
                else
                {
                    if (estadoEmissorDocumento.SefazCTeHomologacao == null)
                        return new JsonpResult(false, false, "Estado do emissor do documento sem Sefaz Homologação configurado.");
                    url = estadoEmissorDocumento.SefazCTeHomologacao.UrlRecepcaoEvento;
                }

                if (string.IsNullOrWhiteSpace(url))
                    return new JsonpResult(false, false, "Sefaz do emissor do documento sem URL de Recepção de Evento configurado.");

                Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoEmitirDesacordo retornoEnvioAprovacao = Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EnviarParaAprovacao(documentoDestinado, motivoDesacordo, unidadeDeTrabalho, Auditado);

                if (!string.IsNullOrWhiteSpace(retornoEnvioAprovacao.mensagem))
                    return new JsonpResult(retornoEnvioAprovacao.status ?? false, true, retornoEnvioAprovacao.mensagem);

                bool desacordoRealizado = false;

                string versaoCte = ObterVersaoCte(documentoDestinado.Chave, documentoDestinado.CPFCNPJDestinatario, unidadeDeTrabalho);

                desacordoRealizado = Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EmitirDesacordoServicoVersao4(documentoDestinado.Empresa.Codigo, documentoDestinado.Chave, justificativa, url, ref mensagemRetorno, unidadeDeTrabalho);

                if (desacordoRealizado)
                {
                    unidadeDeTrabalho.Start();
                    documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.DescordoServico;
                    repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Emitiu desacordo.", unidadeDeTrabalho);
                    unidadeDeTrabalho.CommitChanges();
                }
                else
                {
                    return new JsonpResult(false, false, mensagemRetorno);
                }

                return new JsonpResult(true);
            }

            catch (ControllerException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o desacordo.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirManifestacaoDestinatarioLote()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<long> codigosTitulos = JsonConvert.DeserializeObject<List<long>>(Request.Params("ListaDocumentos"));

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario tipoManifestacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario>("TipoManifestacao");

                string justificativa = Request.GetStringParam("Justificativa");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                if (repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa) > 500)
                    return new JsonpResult(false, true, "Existem muitos documentos para os filtros selecionados. O máximo de documentos permitidos é de 500 por vez.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.ObterDocumentosDestinados(filtrosPesquisa, selecionarTodos, codigosTitulos);

                if (documentosDestinados.Any(o => o.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada))
                    return new JsonpResult(false, true, "Somente é permitida a manifestação para o tipo de documento NF-e Destinada.");

                foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado in documentosDestinados)
                {
                    if (!Servicos.Embarcador.Documentos.ManifestacaoDestinatario.EmitirManifestacaoDestinatario(out string erro, documentoDestinado, tipoManifestacao, justificativa, unidadeDeTrabalho, Auditado))
                        return new JsonpResult(false, true, erro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirManifestacaoDestinatario()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario tipoManifestacao;
                Enum.TryParse(Request.Params("TipoManifestacao"), out tipoManifestacao);

                string justificativa = Request.Params("Justificativa");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (documentoDestinado.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada)
                    return new JsonpResult(false, true, "Somente é permitida a manifestação para o tipo de documento NF-e Destinada.");

                //Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario manifestacao = Servicos.Embarcador.Documentos.ManifestacaoDestinatario.EmitirManifestacaoDestinatario(documentoDestinado.Empresa.Codigo, documentoDestinado.Chave, tipoManifestacao, justificativa, unidadeDeTrabalho);

                //documentoDestinado.Manifestacoes.Add(manifestacao);

                //if (manifestacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Autorizado)
                //{
                //    switch (manifestacao.Tipo)
                //    {
                //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao:
                //            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.CienciaOperacao;
                //            break;
                //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.ConfirmadaOperacao:
                //            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.ConfirmadaOperacao;
                //            break;
                //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.Desconhecida:
                //            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.Desconhecida;
                //            break;
                //        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.OperacaoNaoRealizada:
                //            documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.OperacaoNaoRealizada;
                //            break;
                //    }
                //}

                //repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                //Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Emitiu Manisfestação destinatário.", unidadeDeTrabalho);

                if (!Servicos.Embarcador.Documentos.ManifestacaoDestinatario.EmitirManifestacaoDestinatario(out string erro, documentoDestinado, tipoManifestacao, justificativa, unidadeDeTrabalho, Auditado))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> GerarDocumentoEntradaLote()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<long> codigosTitulos = JsonConvert.DeserializeObject<List<long>>(Request.Params("ListaDocumentos"));

                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfiguracaoDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configDocumentDestinado = repConfiguracaoDocumentoDestinado.BuscarConfiguracaoPadrao();

                if (repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa) > 500)
                    return new JsonpResult(false, true, "Existem muitos documentos para os filtros selecionados. O máximo de documentos permitidos é de 500 por vez.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.ObterDocumentosDestinados(filtrosPesquisa, selecionarTodos, codigosTitulos);

                if (documentosDestinados.Any(o => o.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada))
                    return new JsonpResult(false, true, "Somente é permitida a utilização de documentos do tipo NF-e Destinada.");

                if (documentosDestinados.Any(o => o.TipoOperacao != TipoOperacaoNFe.Saida) && configDocumentDestinado.BloquearLancamentoDocumentosTipoEntrada)
                    return new JsonpResult(false, true, "Somente é permitido a geração de documentos de entrada para documentos do tipo saída.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);
                string msgRetornoErro = string.Empty;

                //foreach (Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado in documentosDestinados)
                for (int i = 0; i < documentosDestinados.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(documentosDestinados[i].Codigo, false);

                    if (documentoDestinado.DocumentoEntrada != null)
                        continue;

                    if (documentoDestinado.Cancelado)
                        continue;

                    if (repDocumentoEntradaTMS.BuscarPorChave(documentoDestinado.Chave) != null)
                        return new JsonpResult(false, true, "O documento de entrada " + documentoDestinado.Chave + " já foi gerado.");

                    string caminhoXML = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                    {
                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico, null, null, documentoDestinado.Chave);

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                            continue;
                    }

                    object nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML)));

                    unidadeDeTrabalho.Start();
                    string erro = string.Empty;
                    bool possuiPermissaoGravarValorDiferente = this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = svcDocumentoEntrada.GerarDocumentoEntradaPorNFe(nfe, Empresa, unidadeDeTrabalho, TipoServicoMultisoftware, this.Usuario, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada, ConfiguracaoEmbarcador.DataEntradaDocumentoEntrada, ConfiguracaoEmbarcador.NaoControlarKMLancadoNoDocumentoEntrada, possuiPermissaoGravarValorDiferente, out erro, ConfiguracaoEmbarcador.LancarDocumentoEntradaAbertoSeKMEstiverErrado);
                    documentoEntrada.DocumentoImportadoXML = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoXML);

                    if (!string.IsNullOrWhiteSpace(erro))
                        msgRetornoErro += " " + erro;

                    if (documentoEntrada != null && documentoEntrada.Codigo > 0)
                    {
                        documentoEntrada = repDocumentoEntradaTMS.BuscarPorCodigo(documentoEntrada.Codigo);
                        documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(documentosDestinados[i].Codigo, false);
                        documentoDestinado.DocumentoEntrada = documentoEntrada;

                        repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Gerou Documento de Entrada em lote.", unidadeDeTrabalho);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, null, "Gerado pelos documentos destinados.", unidadeDeTrabalho);

                        if (documentoEntrada.DocumentoFinalizadoAutomaticamente)
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, null, "Finalizado automaticamente.", unidadeDeTrabalho);

                        unidadeDeTrabalho.CommitChanges();
                    }
                    else
                    {
                        unidadeDeTrabalho.Rollback();
                    }
                }
                if (!string.IsNullOrWhiteSpace(msgRetornoErro))
                    return new JsonpResult(false, true, "A(s) nota(s) a seguir está(ão) com sua(s) situação(ões) Em Aberto: " + msgRetornoErro);
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar os documentos de entrada.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> GerarDocumentoEntrada()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Financeiros/DocumentoEntrada");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfiguracaoDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configDocumentDestinado = repConfiguracaoDocumentoDestinado.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (documentoDestinado.DocumentoEntrada != null)
                    return new JsonpResult(false, true, "O documento de entrada já foi gerado.");

                if (documentoDestinado.Cancelado)
                    return new JsonpResult(false, true, "O documento foi cancelado pelo emitente, não sendo possível gerar um documento de entrada.");

                if (documentoDestinado.TipoDocumento != TipoDocumentoDestinadoEmpresa.NFeDestinada)
                    return new JsonpResult(false, true, "Somente é permitida a utilização de documentos do tipo NF-e Destinada.");

                if (documentoDestinado.TipoOperacao != TipoOperacaoNFe.Saida && configDocumentDestinado.BloquearLancamentoDocumentosTipoEntrada)
                    return new JsonpResult(false, true, "Somente é permitido gerar documento de entrada para documentos do tipo saída.");

                if (repDocumentoEntradaTMS.BuscarPorChave(documentoDestinado.Chave) != null)
                    return new JsonpResult(false, true, "O documento de entrada já foi gerado.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                Servicos.Embarcador.Financeiro.DocumentoEntrada svcDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unidadeDeTrabalho);

                string caminhoXML = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                {
                    Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico, null, TipoServicoMultisoftware, documentoDestinado.Chave);

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                        return new JsonpResult(false, true, "Não foi possível realizar o download do xml da SEFAZ. Dica: Verifique se o documento possui manifestação ou aguarde alguns minutos e tente novamente.");
                }

                object nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(new System.IO.MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML)));

                if (nfe == null)
                    return new JsonpResult(false, true, "Não foi possível ler o documento de entrada.");

                unidadeDeTrabalho.Start();

                string msgRetornoErro = string.Empty;
                bool possuiPermissaoGravarValorDiferente = this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoEntrada_AutorizarPrecoCombustivelDiferenteFornecedor) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;
                string erro = "";
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = svcDocumentoEntrada.GerarDocumentoEntradaPorNFe(nfe, Empresa, unidadeDeTrabalho, TipoServicoMultisoftware, this.Usuario, Auditado, ConfiguracaoEmbarcador.DataCompetenciaDocumentoEntrada, ConfiguracaoEmbarcador.DataEntradaDocumentoEntrada, ConfiguracaoEmbarcador.NaoControlarKMLancadoNoDocumentoEntrada, possuiPermissaoGravarValorDiferente, out erro, ConfiguracaoEmbarcador.LancarDocumentoEntradaAbertoSeKMEstiverErrado);
                documentoEntrada.DocumentoImportadoXML = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoXML);

                if (!string.IsNullOrWhiteSpace(erro))
                    msgRetornoErro += " " + erro;

                if (documentoEntrada != null)
                {
                    documentoDestinado.DocumentoEntrada = documentoEntrada;

                    repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Gerou Documento de Entrada.", unidadeDeTrabalho);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, null, "Gerado pelos documentos destinados.", unidadeDeTrabalho);

                    if (documentoEntrada.DocumentoFinalizadoAutomaticamente)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, null, "Finalizado automaticamente.", unidadeDeTrabalho);

                    unidadeDeTrabalho.CommitChanges();

                    if (!string.IsNullOrWhiteSpace(msgRetornoErro))
                        return new JsonpResult(false, true, "A nota está com sua situação Em Aberto: " + msgRetornoErro);
                    else
                        return new JsonpResult(true);
                }
                else
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, "Não foi possível gerar o documento de entrada. " + erro);
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar os documentos de entrada.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> VincularDocumentoEntrada()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoDocumentoEntrada = Request.GetIntParam("CodigoDocumentoEntrada");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (documentoDestinado.DocumentoEntrada != null)
                    return new JsonpResult(false, true, "O documento de entrada já foi vinculado.");

                if (documentoDestinado.Cancelado)
                    return new JsonpResult(false, true, "O documento foi cancelado pelo emitente, não sendo possível vincular um documento de entrada.");

                if (documentoDestinado.TipoDocumento != TipoDocumentoDestinadoEmpresa.NFeDestinada)
                    return new JsonpResult(false, true, "Somente é permitida a utilização de documentos do tipo NF-e Destinada.");

                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada = repDocumentoEntradaTMS.BuscarPorCodigo(codigoDocumentoEntrada);

                if (documentoEntrada == null)
                    return new JsonpResult(false, true, "Documento de entrada não encontrado.");

                if (documentoEntrada.Numero != documentoDestinado.Numero || documentoEntrada.Serie.ToInt() != documentoDestinado.Serie || documentoEntrada.Chave != documentoDestinado.Chave)
                    return new JsonpResult(false, true, "Documento de entrada possui Número/Série/Chave diferente do Documento Destinado, não sendo possível vincular.");

                unidadeDeTrabalho.Start();

                documentoDestinado.DocumentoEntrada = documentoEntrada;

                repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Vinculou manualmente o Documento de Entrada.", unidadeDeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoEntrada, null, "Vinculou manualmente com o Documento Destinado.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular os documentos de entrada.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarXMLCTesDestinados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string url = string.Empty;
                int codigoEmpresa;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa;

                if (empresa == null)
                    return new JsonpResult(false, true, "Empresa Pai não configurada");

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (empresa.Localidade.Estado.SefazCTe == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz configurado.");
                    url = empresa.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (empresa.Localidade.Estado.SefazCTeHomologacao == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz Homologação configurado.");
                    url = empresa.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe;
                }

                string mensagemRetorno = string.Empty;

                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ProcessarXMLDocumentosDestinadosCTeEmpresa(empresa.Codigo, _conexao.StringConexao, ref mensagemRetorno, null, TipoServicoMultisoftware, unitOfWork))
                    return new JsonpResult(false, true, mensagemRetorno);
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarCTesDestinados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string url = string.Empty;
                int codigoEmpresa;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

#if DEBUG
                codigoEmpresa = 15;
#endif

                //Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa;

                if (empresa == null)
                    return new JsonpResult(false, true, "Empresa/Transportador não informado");

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (empresa.Localidade.Estado.SefazCTe == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz configurado.");
                    url = empresa.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (empresa.Localidade.Estado.SefazCTeHomologacao == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz Homologação configurado.");
                    url = empresa.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe;
                }

                string mensagemRetorno = string.Empty;

                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(empresa.Codigo, url, _conexao.StringConexao, 0, ref mensagemRetorno, out string codigoStatusRetornoSefaz, null, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, mensagemRetorno);
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

            }
        }

        public async Task<IActionResult> ReintegrarDocumentosDestinados()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado servDestinados = new Servicos.Embarcador.Integracao.Unilever.DocumentoDestinado(unitOfWork);
                servDestinados.ProcessarIntegracoesPendentesDestinados();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao integrar os documentos.");

            }
            return new JsonpResult(true);
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ObterCartaCorrecoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string chave = Request.GetStringParam("Chave");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data de emissão", "Data", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Nº Protocolo", "Protocolo", 20, Models.Grid.Align.left);

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> cartaCorecoes = new List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa>();

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repositorioDocumentoDestinado = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento = repositorioDocumentoDestinado.BuscarPorChaveETipoDocumento(new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada }, chave);
                if (documento != null)
                {
                    cartaCorecoes = repositorioDocumentoDestinado.BuscarDocumentosPorChaveETipoDocumento(documento.Chave, new Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa[] { Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CCe });
                }

                grid.AdicionaRows(
                    from c in cartaCorecoes.OrderByDescending(x => x.DataEmissao)
                    select new
                    {
                        c.Codigo,
                        Data = c.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                        c.Protocolo
                    }
                    );
                grid.setarQuantidadeTotal(cartaCorecoes.Count);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico de integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoIntegracaoSAP()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                TipoDocumentoDestinadoEmpresa tipoDocumentoDestinado = Request.GetEnumParam<TipoDocumentoDestinadoEmpresa>("TipoDocumento");
                Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracao = repositorioDocumentoDestinadoIntegracao.BuscarPorCodigoETipoDocumentoDestinado(codigo, tipoDocumentoDestinado);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left);

                var retorno = new List<Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoHistoricoIntegracaoSAP>();

                List<(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo, TipoIntegracao)> historico = new List<(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo, TipoIntegracao)>();

                historico.Add((new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo()
                {
                    Codigo = integracao.Codigo,
                    Mensagem = integracao?.ProblemaIntegracao ?? "",
                    Data = integracao?.DataIntegracao ?? DateTime.MinValue,
                }, integracao.TipoIntegracao.Tipo));


                foreach (var (arquivo, tipo) in historico)
                    retorno.Add(new Dominio.ObjetosDeValor.Embarcador.Documentos.RetornoHistoricoIntegracaoSAP
                    {
                        Codigo = arquivo?.Codigo ?? 0,
                        Data = arquivo?.Data != null && arquivo?.Data != DateTime.MinValue ? arquivo?.Data.ToString("dd/MM/yyyy HH:mm:ss") : "",
                        DescricaoTipo = tipo.ObterDescricao() ?? "",
                        Mensagem = arquivo?.Mensagem ?? ""
                    });

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(1);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico de integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoSAP()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repositorioDocumentoDestinadoIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao integracao = repositorioDocumentoDestinadoIntegracao.BuscarPorCodigo(codigo, false);

                if (integracao == null)
                    return new JsonpResult(false, "Histórico de integração não encontrada.");

                var lista = new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>();

                foreach (var arquivo in integracao.ArquivosTransacao)
                {
                    lista.Add(arquivo.ArquivoRequisicao);
                    lista.Add(arquivo.ArquivoResposta);
                }

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(lista);
                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {integracao.DocumentoDestinadoEmpresa.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos da integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        public async Task<IActionResult> ReintegrarDocumentoDestinado(int codigo)
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao repIntegracao = new Repositorio.Embarcador.Documentos.DocumentoDestinadoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracoes = repIntegracao.ObterIntegracoesPorDocumento(codigo);
                if (integracoes == null)
                    return new JsonpResult(false, "Registro não encontrado");

                foreach (var i in integracoes)
                {
                    i.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    i.ProblemaIntegracao = "";
                    repIntegracao.Atualizar(i);
                }

                return new JsonpResult(true, "Registro enviado para reprocessamento.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }


        public async Task<IActionResult> ReConsultarCTesDestinados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string url = string.Empty;
                int codigoEmpresa = 15;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                //Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa;

                if (empresa == null)
                    return new JsonpResult(false, true, "Empresa/Transportador não informado");

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (empresa.Localidade.Estado.SefazCTe == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz configurado.");
                    url = empresa.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (empresa.Localidade.Estado.SefazCTeHomologacao == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz Homologação configurado.");
                    url = empresa.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe;
                }

                string mensagemRetorno = string.Empty;

                List<int> listaNSU = new List<int>() { 1830817, 1830818 };
                for (var i = 0; i < listaNSU.Count; i++)
                {
                    if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(empresa.Codigo, url, _conexao.StringConexao, listaNSU[i], ref mensagemRetorno, out string codigoStatusRetornoSefaz, null, TipoServicoMultisoftware))
                        Servicos.Log.TratarErro(listaNSU.ToString() + " " + mensagemRetorno);

                }
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

            }
        }

        public async Task<IActionResult> ConsultarMDFesDestinados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                string url = string.Empty;
                int codigoEmpresa;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                //Dominio.Entidades.Empresa empresa = repEmpresa.BuscarEmpresaPai();
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa;

                if (empresa == null)
                    return new JsonpResult(false, true, "Empresa/Transportador não informado");

                if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (empresa.Localidade.Estado.SefazMDFe == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz configurado.");
                    url = empresa.Localidade.Estado.SefazMDFe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (empresa.Localidade.Estado.SefazMDFeHomologacao == null)
                        return new JsonpResult(false, true, "Estado sem Sefaz Homologação configurado.");
                    url = empresa.Localidade.Estado.SefazMDFeHomologacao.UrlDistribuicaoDFe;
                }

                string mensagemRetorno = string.Empty;

                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosMDFeEmpresa(empresa.Codigo, url, _conexao.StringConexao, 0, ref mensagemRetorno, null, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, mensagemRetorno);
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

            }
        }

        public async Task<IActionResult> DownloadXMLMDFe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                if (documentoDestinado.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.EncerramentoMDFe)
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "MDFe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + "_procEncMDFe.xml");
                else if (documentoDestinado.TipoDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CancelamentoMDFe)
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "MDFe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + "_procCancMDFe.xml");
                else
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "MDFe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + ".xml");

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                else
                    return new JsonpResult(false, true, "XML não disponível para download.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXMLCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", documentoDestinado.Empresa.CNPJ, documentoDestinado.Chave + ".xml");

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                else
                    return new JsonpResult(false, true, "XML não disponível para download.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadXMLNFe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                if (documentoDestinado.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFSeDestinada)
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFSe", documentoDestinado.Chave + "_NFSe.xml");
                else
                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                else
                {
                    Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico, null, TipoServicoMultisoftware, documentoDestinado.Chave);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho), "text/xml", System.IO.Path.GetFileName(caminho));
                    else
                        return new JsonpResult(false, true, "Não foi possível realizar o download do XML da NF-e. Dica: Verifique se o documento destinado possui manifestação e, se possui, aguarde, pois a SEFAZ pode demorar até 24h da data de manifestação para liberar o documento para download.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDANFENFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);
                string chave = Request.Params("Chave");

                try
                {
                    string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                    string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", chave + ".pdf");
                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", chave + ".pdf");
                    else
                    {
                        Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                        Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);
                        if (documentoDestinado == null)
                            return new JsonpResult(false, false, "Documento não encontrado.");

                        if (documentoDestinado.TipoDocumento == TipoDocumentoDestinadoEmpresa.NFSeDestinada)
                            return new JsonpResult(false, false, "DANFSE de NFS-e Destinada deve ser gerada no site da prefeitura da cidade em que foi emitida.");


                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        {
                            Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico);

                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                                return new JsonpResult(false, "Não foi possível realizar o download do DANFE da NF-e. Dica: Verifique se o documento destinado possui manifestação e, se possui, aguarde, pois a SEFAZ pode demorar até 24h da data de manifestação para liberar o documento para download.");
                        }

                        var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                        var retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unitOfWork);

                        if (retorno == "")
                            return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", chave + ".pdf");
                        else
                            return new JsonpResult(false, retorno);
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao realizar o download PDF.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadLoteXMLNFe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                List<TipoDocumentoDestinadoEmpresa> tipoDocumentosPermitidos = new List<TipoDocumentoDestinadoEmpresa>() { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte };
                if (!filtrosPesquisa.TiposDocumento.Any(o => tipoDocumentosPermitidos.Contains(o)) || filtrosPesquisa.TiposDocumento.Where(o => !tipoDocumentosPermitidos.Contains(o)).ToList().Count > 0)
                    return new JsonpResult(false, true, "Download múltiplo de XMLs permitido apenas com o filtro Tipos Documentos igual a NF-e Destinada ou NF-e para Transporte");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                int qtdPesquisa = repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa);

                if (qtdPesquisa == 0)
                    return new JsonpResult(false, true, "Os filtros realizados não retornaram nenhuma NF-e! Favor verificar.");
                else if (qtdPesquisa > 500)
                    return new JsonpResult(false, true, "Quantidade de NF-es para geração de lote inválida (" + qtdPesquisa + "). É permitido o download de um lote com o máximo de 500 NF-es.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.Consultar(filtrosPesquisa, "Codigo", "asc", 0, 500);

                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa svcDocumentoDestinadoEmpresa = new Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                return Arquivo(svcDocumentoDestinadoEmpresa.ObterLoteDeXMLNFe(documentosDestinados, filtrosPesquisa.CodigoEmpresa, unidadeDeTrabalho), "application/zip", "LoteXMLNFeDocumentosDestinados.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadLoteXMLCTe()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa filtrosPesquisa = ObterFiltrosPesquisa();

                List<TipoDocumentoDestinadoEmpresa> tipoDocumentosPermitidos = new List<TipoDocumentoDestinadoEmpresa>() {
                    TipoDocumentoDestinadoEmpresa.CTeDestinadoDestinatario, TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente, TipoDocumentoDestinadoEmpresa.CTeDestinadoExpedidor, TipoDocumentoDestinadoEmpresa.CTeDestinadoRecebedor,
                    TipoDocumentoDestinadoEmpresa.CTeDestinadoRemetente,TipoDocumentoDestinadoEmpresa.CTeDestinadoTerceiro, TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador, TipoDocumentoDestinadoEmpresa.AutorizadoDownload
                };
                if (!filtrosPesquisa.TiposDocumento.Any(o => tipoDocumentosPermitidos.Contains(o)) || filtrosPesquisa.TiposDocumento.Where(o => !tipoDocumentosPermitidos.Contains(o)).ToList().Count > 0)
                    return new JsonpResult(false, true, "Download múltiplo de XMLs permitido apenas com o filtro Tipos Documentos igual a CT-e Destinatário, Remetente, etc...");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                int qtdPesquisa = repDocumentoDestinadoEmpresa.ContarConsulta(filtrosPesquisa);

                if (qtdPesquisa == 0)
                    return new JsonpResult(false, true, "Os filtros realizados não retornaram nenhum CT-e! Favor verificar.");
                else if (qtdPesquisa > 500)
                    return new JsonpResult(false, true, "Quantidade de CT-es para geração de lote inválida (" + qtdPesquisa + "). É permitido o download de um lote com o máximo de 500 CT-es.");

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentosDestinados = repDocumentoDestinadoEmpresa.Consultar(filtrosPesquisa, "Codigo", "asc", 0, 500);

                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa svcDocumentoDestinadoEmpresa = new Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);
                return Arquivo(svcDocumentoDestinadoEmpresa.ObterLoteDeXMLCTe(documentosDestinados, filtrosPesquisa.CodigoEmpresa, unidadeDeTrabalho), "application/zip", "LoteXMLCTeDocumentosDestinados.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SolicitarTXTFTP()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo;
                long.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo, true);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (!documentoDestinado.GerouArquivoIntegracao)
                    return new JsonpResult(false, true, "Arquivo já está aguardando integração.");

                documentoDestinado.GerouArquivoIntegracao = false;
                repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado, Auditado);

                try
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Solicitou nova geração de arquivo TXT de integração via FTP.", unidadeDeTrabalho);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problemas ao auditar solicitação de arquivo TXT de integração via FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                }

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o TXT ao FTP.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EnviarXMLParaFTP()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo;
                long.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo, true);

                if (documentoDestinado == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                if (!documentoDestinado.EnviouXMLImputIntegracao)
                    return new JsonpResult(false, true, "Documento já está aguardando envio, favor aguarde o processamento.");

                documentoDestinado.EnviouXMLImputIntegracao = false;
                documentoDestinado.TentativasEnvioImputIntegracao = 0;
                repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado, Auditado);

                try
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Solicitou o envio do XML via imput para o FTP de forma manual.", unidadeDeTrabalho);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Problemas ao auditar solicitação de envio do XML via imput ao FTP: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                }

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o imput do XML ao FTP.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> DownloadTXTDocumento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                long codigo;
                long.TryParse(Request.Params("Codigo"), out codigo);

                try
                {
                    Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeTrabalho);
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo, true);
                    if (documentoDestinado == null)
                        return new JsonpResult(false, true, "Documento não encontrado.");

                    MemoryStream arquivoINPUT = new MemoryStream();
                    string siglaEmpresa = documentoDestinado.Empresa?.CodigoEmpresa ?? "";
                    string nomeArquivo = siglaEmpresa + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_1.txt";
                    StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);

                    x.WriteLine(documentoDestinado.Chave + ";" +
                            documentoDestinado.DataEmissao.Value.ToString("dd/MM/yyyy hh:MM") + ";" +
                            documentoDestinado.Valor.ToString("n2") + ";" +
                            documentoDestinado.CPFCNPJEmitente + ";" +
                            documentoDestinado.Protocolo + ";" +
                            (documentoDestinado.Cancelado ? "Cancelado" : "Autorizada"));
                    x.Flush();

                    try
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoDestinado, null, "Baixou arquivo TXT de integração manualmente.", unidadeTrabalho);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Problemas ao auditar baixa de arquivo TXT de integração manualmente: " + ex.ToString(), "IntegracaoDocumentosDestinados");
                    }

                    return Arquivo(arquivoINPUT.ToArray(), "text/plain", nomeArquivo);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do TXT.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeTrabalho.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do TXT.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoGeral()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repConfiguracaoDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configDocumentDestinado = repConfiguracaoDocumentoDestinado.BuscarConfiguracaoPadrao();

                var retorno = new
                {
                    BloquearLancamentoDocumentosTipoEntrada = configDocumentDestinado.BloquearLancamentoDocumentosTipoEntrada
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar configuração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarHistoricoIrregularidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioControleDocumento = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicoIrregularidades = repositorioControleDocumento.BuscarPorControleDocumento(codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("Data Irregularidade", "DataIrregularidade", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Portifólio", "Porfolio", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Irregularidade", "Irregularidade", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Responsável pela irregularidade", "Responsavel", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);

                var retorno = (from obj in historicoIrregularidades
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   DataIrregularidade = obj.DataIrregularidade.HasValue ? obj.DataIrregularidade.Value.ToString("dd/MM/yyyy") : "",
                                   Porfolio = obj?.Porfolio?.Descricao ?? "",
                                   Irregularidade = obj?.Irregularidade?.Descricao ?? "",
                                   Responsavel = obj.ServicoResponsavel.ObterDescricao(),
                                   Situacao = obj.SituacaoIrregularidade.ObterDescricao(),
                                   Observacao = obj?.Observacao ?? ""
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(historicoIrregularidades.Count);
                return new JsonpResult(grid);

            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar histórico de irregularidade.");
            }
        }
        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CodigoTipoDocumento", false);
            grid.AdicionarCabecalho("Tipo", "TipoDocumento", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Número", "Numero", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Autorização", "DataAutorizacao", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integração", "DataIntegracao", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Chave", "Chave", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Notas CTe", "NotasDoCTe", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Emitente", "Emitente", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatário/Transportador", "Destinatario", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação do Documento", "Cancelado", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação MD-e", "SituacaoManifestacaoDestinatario", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("DocumentoCTe", false);
            grid.AdicionarCabecalho("DocumentoMDFe", false);
            grid.AdicionarCabecalho("CodigoDocumentoEntrada", false);
            grid.AdicionarCabecalho("FornecedorDocumentoEntrada", false);
            grid.AdicionarCabecalho("FornecedorOrdemCompra", false);
            grid.AdicionarCabecalho("DocumentoCTeNaoComplementarPossuiOcorrencia", false);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaParaCarga()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Autorização", "DataAutorizacao", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Integração", "DataIntegracao", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Chave", "Chave", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Emitente", "Emitente", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Destinatário/Transportador", "Destinatario", 12, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.left, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaDocumentoDestinadoEmpresa()
            {
                Chave = Request.GetStringParam("Chave"),
                CpfCnpjFornecedor = Request.GetStringParam("CPFCNPJFornecedor"),
                NomeFornecedor = Request.GetStringParam("NomeFornecedor"),
                NumeroDe = Request.GetIntParam("NumeroDe"),
                NumeroAte = Request.GetIntParam("NumeroAte"),
                Serie = Request.GetIntParam("Serie"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Empresa"),
                TiposDocumento = Request.GetListEnumParam<TipoDocumentoDestinadoEmpresa>("TipoDocumento"),
                SituacaoManifestacaoDestinatario = Request.GetListEnumParam<SituacaoManifestacaoDestinatario>("SituacaoManifestacaoDestinatario"),
                DataAutorizacaoInicial = Request.GetDateTimeParam("DataAutorizacaoInicial"),
                DataAutorizacaoFinal = Request.GetDateTimeParam("DataAutorizacaoFinal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                TipoOperacao = Request.GetNullableEnumParam<TipoOperacaoNFe>("TipoOperacao"),
                Cancelado = Request.GetNullableBoolParam("Cancelado"),
                PossuiDocumentoEntrada = Request.GetNullableBoolParam("PossuiDocumentoEntrada"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao")
            };
        }

        private void VerificarPermissaoPersonalizada()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/DocumentoDestinadoEmpresa");

            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.DocumentoDestinado_PermiteEmitirDesacordo))
            {
                throw new ControllerException("Você não possui permissão para Emitir Desacordos");
            }
        }

        private string ObterDescricaoIntegracao(List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracoes, long Codigo)
        {

            List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracaoDestinados = integracoes.Where(o => o.DocumentoDestinadoEmpresa.Codigo == Codigo).ToList();

            if (!integracaoDestinados.Any(x => x.SituacaoIntegracao == SituacaoIntegracao.Integrado))
                return integracaoDestinados.FirstOrDefault()?.SituacaoIntegracao.ObterDescricao();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoes = integracaoDestinados.Select(o => o.TipoIntegracao.Tipo).Distinct().ToList();

            bool todasIntegradas = true;

            foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo in tiposIntegracoes)
                todasIntegradas = integracaoDestinados.Any(x => x.TipoIntegracao.Tipo == tipo && x.SituacaoIntegracao == SituacaoIntegracao.Integrado);

            if (!todasIntegradas)
                return SituacaoIntegracao.ProblemaIntegracao.ObterDescricao();

            return SituacaoIntegracao.Integrado.ObterDescricao();

        }

        private string ObterVersaoCte(string chave, string cnpj, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            string versao = "3.00";

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCte.BuscarPorChave(chave);
            if (cte != null)
                return cte.Versao;

            //se nao encontrar o t_cte de um cte destinado, tenta pegar a versao pelo arquivo xml
            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", cnpj, chave + ".xml");

            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
            {
                XDocument doc = null;

                using (TextReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho)))
                {
                    doc = XDocument.Load(reader);
                }

                XNamespace ns = doc.Root.Name.Namespace;
                versao = (from ele in doc.Descendants(ns + "infCte") select ele.Attribute("versao").Value).FirstOrDefault();
            }

            return versao;
        }

        private bool DocumentoValidoRetornar(Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documento, List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracoes, SituacaoIntegracao situacaoIntegracao)
        {
            List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoIntegracao> integracoesDocumentoAtual = integracoes.Where(x => x.DocumentoDestinadoEmpresa.Codigo == documento.Codigo).ToList();

            if (integracoesDocumentoAtual.Count == 0)
                return false;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoes = integracoesDocumentoAtual.Select(o => o.TipoIntegracao.Tipo).Distinct().ToList();
            bool retornar = false;

            if (situacaoIntegracao == SituacaoIntegracao.Integrado)
            {
                foreach (var tipo in tiposIntegracoes)
                    retornar = integracoesDocumentoAtual.Any(x => x.TipoIntegracao.Tipo == tipo && x.SituacaoIntegracao == SituacaoIntegracao.Integrado);

                return retornar;
            }

            if (situacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
            {
                foreach (var tipo in tiposIntegracoes)
                    retornar = integracoesDocumentoAtual.Any(x => x.TipoIntegracao.Tipo == tipo && x.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao && !integracoesDocumentoAtual.Any(a => a.TipoIntegracao.Tipo == tipo && a.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao));

                return retornar;
            }

            if (situacaoIntegracao == SituacaoIntegracao.AgIntegracao)
            {
                foreach (var tipo in tiposIntegracoes)
                    retornar = integracoesDocumentoAtual.Any(x => x.TipoIntegracao.Tipo == tipo && x.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao &&
                    !integracoesDocumentoAtual.Any(a => a.TipoIntegracao.Tipo == tipo && (a.SituacaoIntegracao == SituacaoIntegracao.Integrado || a.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)));

                return retornar;
            }

            return retornar;
        }
        #endregion
    }
}
