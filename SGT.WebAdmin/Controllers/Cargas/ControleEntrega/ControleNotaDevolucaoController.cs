using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize("Cargas/ControleNotaDevolucao")]
    public class ControleNotaDevolucaoController : BaseController
    {
		#region Construtores

		public ControleNotaDevolucaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaControleNotaDevolucao filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CargaEntregaNFeDevolucao", false);
                grid.AdicionarCabecalho("PossuiImagem", false);
                grid.AdicionarCabecalho("Status", false);

                grid.AdicionarCabecalho("NF-e Origem", "NotaOrigem", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Chave", "ChaveNFe", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Atendimento", "Chamado", 10, Models.Grid.Align.left, false);

                if (filtrosPesquisa.Status == StatusControleNotaDevolucao.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao> dados = repControleNotaDevolucao.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repControleNotaDevolucao.ContarConsulta(filtrosPesquisa));

                var lista = (from p in dados
                             select new
                             {
                                 p.Codigo,
                                 p.ChaveNFe,
                                 CargaEntregaNFeDevolucao = p.CargaEntregaNFeDevolucao?.Codigo ?? 0,
                                 PossuiImagem = !string.IsNullOrEmpty(p.CargaEntregaNFeDevolucao?.GuidArquivo),
                                 NotaOrigem = p.CargaEntregaNFeDevolucao?.XMLNotaFiscal != null ? (p.CargaEntregaNFeDevolucao.XMLNotaFiscal.Numero.ToString() + " - " + p.CargaEntregaNFeDevolucao.XMLNotaFiscal.Serie) : "",
                                 Destinatario = p.XMLNotaFiscal?.Destinatario?.Descricao,
                                 DataEmissao = p.XMLNotaFiscal?.DataEmissao.ToString("dd/MM/yyyy") ?? string.Empty,
                                 Carga = p.Chamado.Carga.CodigoCargaEmbarcador,
                                 Chamado = p.Chamado.Numero,
                                 DescricaoStatus = p.Status.ObterDescricao(),
                                 p.Status
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao notaDevolucao = repControleNotaDevolucao.BuscarPorCodigo(codigo);

                if (notaDevolucao == null)
                    return new JsonpResult(false, "Nota de devolução não encontrada.");

                var dynControleNotaDevolucao = new
                {
                    notaDevolucao.Codigo,
                    Chave = notaDevolucao.ChaveNFe,
                    notaDevolucao.Motivo,
                    notaDevolucao.Status,
                    CodigoChamado = notaDevolucao.Chamado.Codigo,
                    Atendimento = notaDevolucao.Chamado.Numero,
                    Carga = notaDevolucao.Chamado.Carga.CodigoCargaEmbarcador,
                    Numero = notaDevolucao.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                    notaDevolucao.XMLNotaFiscal?.Serie,
                    Destinatario = notaDevolucao.XMLNotaFiscal?.Destinatario.Descricao ?? string.Empty,
                    ObservacaoMotorista = notaDevolucao.CargaEntregaNFeDevolucao?.ObservacaoMotorista ?? "",
                    PossuiImagem = !string.IsNullOrEmpty(notaDevolucao.CargaEntregaNFeDevolucao?.GuidArquivo),
                    NotaOrigem = notaDevolucao.CargaEntregaNFeDevolucao?.XMLNotaFiscal != null ? (notaDevolucao.CargaEntregaNFeDevolucao.XMLNotaFiscal.Numero.ToString() + " - " + notaDevolucao.CargaEntregaNFeDevolucao.XMLNotaFiscal.Serie) : "",
                };

                return new JsonpResult(dynControleNotaDevolucao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarNotaDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao notaDevolucao = repControleNotaDevolucao.BuscarPorCodigo(codigo, true);

                if (notaDevolucao == null)
                    return new JsonpResult(false, true, "Nota de devolução não encontrada.");

                if (notaDevolucao.Status == StatusControleNotaDevolucao.Conferido)
                    return new JsonpResult(false, true, "Nota de devolução já está confirmada.");

                notaDevolucao.Status = StatusControleNotaDevolucao.Conferido;
                repControleNotaDevolucao.Atualizar(notaDevolucao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarNotaDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao notaDevolucao = repControleNotaDevolucao.BuscarPorCodigo(codigo, true);

                if (notaDevolucao == null)
                    return new JsonpResult(false, true, "Nota de devolução não encontrada.");

                if (notaDevolucao.Status == StatusControleNotaDevolucao.Rejeitado)
                    return new JsonpResult(false, true, "Nota de devolução já está rejeitada.");

                notaDevolucao.Status = StatusControleNotaDevolucao.Rejeitado;
                notaDevolucao.Motivo = Request.GetStringParam("Motivo");
                repControleNotaDevolucao.Atualizar(notaDevolucao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadDANFENFeDestinados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string chave = Request.GetStringParam("Chave");

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(chave) || !Utilidades.Validate.ValidarChaveNFe(chave))
                    return new JsonpResult(false, true, "A chave informada está inválida.");

                string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoDANFE = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", chave + ".pdf");
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoDANFE))
                    return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", chave + ".pdf");
                else
                {
                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(
                        new TipoDocumentoDestinadoEmpresa[] { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte }, chave);

                    if (documentoDestinado == null)
                        return new JsonpResult(false, true, "Documento Destinado não disponível.");

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    {
                        Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _conexao.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico);

                        if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                            return new JsonpResult(false, "Não foi possível realizar o download do XML da NF-e da SEFAZ. Dica: Verifique se o documento destinado possui manifestação.");
                    }

                    Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    string retorno = z.GerarDANFENFeDocumentoDestinados(caminho, caminhoDANFE, unidadeDeTrabalho);

                    if (string.IsNullOrWhiteSpace(retorno))
                        return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoDANFE), "application/x-pkcs12", chave + ".pdf");
                    else
                        return new JsonpResult(false, retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarChaveNotaDevolucao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string chave = Request.GetStringParam("Chave");

                Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao notaDevolucao = repControleNotaDevolucao.BuscarPorCodigo(codigo, true);

                if (notaDevolucao == null)
                    return new JsonpResult(false, true, "Nota de devolução não encontrada.");

                if (notaDevolucao.Status != StatusControleNotaDevolucao.AgNotaFiscal)
                    return new JsonpResult(false, true, "Nota de devolução não está aguardando uma chave.");

                if (Utilidades.Validate.ValidarChaveNFe(notaDevolucao.ChaveNFe))
                    return new JsonpResult(false, true, "Chave da NF-e já foi informada anteriormente.");

                if (!Utilidades.Validate.ValidarChaveNFe(chave))
                    return new JsonpResult(false, true, "Chave da NF-e informada é inválida.");

                notaDevolucao.Status = StatusControleNotaDevolucao.ComChaveNotaFiscal;
                notaDevolucao.ChaveNFe = chave;
                repControleNotaDevolucao.Atualizar(notaDevolucao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao informar chave.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaControleNotaDevolucao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaControleNotaDevolucao()
            {
                Chave = Request.GetStringParam("Chave"),
                Status = Request.GetEnumParam<StatusControleNotaDevolucao>("Status"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroChamado = Request.GetIntParam("NumeroChamado"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                CnpjCpfDestinatario = Request.GetDoubleParam("Destinatario")
            };
        }

        #endregion
    }
}
