using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/DocumentoTransporteNatura")]
    public class DocumentoTransporteNaturaController : BaseController
    {
		#region Construtores

		public DocumentoTransporteNaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        public static bool ThreadExecutada = false;
        public static bool Sucesso = false;
        public static Dominio.Entidades.LayoutEDI LayoutEDI = null;
        public static Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis NOTFIS = null;
        public static Stream ArquivoEDI = null;

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridPesquisa();

                if (!ExecutarPesquisa(out string mensagemErro, out dynamic lista, out int count, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite, false, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);

                grid.setarQuantidadeTotal(count);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte.");
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
                Models.Grid.Grid grid = GridPesquisa();

                if (!ExecutarPesquisa(out string mensagemErro, out dynamic lista, out int count, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, 0, 0, true, unitOfWork))
                    return new JsonpResult(false, true, mensagemErro);

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> ConsultarDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long numeroDocumentoTransporte;
                long.TryParse(Request.Params("NumeroDocumentoTransporte"), out numeroDocumentoTransporte);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura(unidadeDeTrabalho);
                svcNatura.ConsultarDT(Usuario, numeroDocumentoTransporte, dataInicial, dataFinal, unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> VincularCargaAoDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);

                int codigoDocumentoTransporte, codigoCarga;
                int.TryParse(Request.Params("DocumentoTransporte"), out codigoDocumentoTransporte);
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura(unidadeDeTrabalho);
                string mensagem = string.Empty;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                unidadeDeTrabalho.Start();

                if (!svcNatura.VincularCargaAoDT(Usuario, new List<int>() { codigoDocumentoTransporte }, cargaPedido.Codigo, TipoServicoMultisoftware, unidadeDeTrabalho, out mensagem))
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }

                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDTNatura.BuscarPorCodigo(codigoDocumentoTransporte);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Vinculou o DT " + dtNatura.Descricao + ".", unidadeDeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, dtNatura, null, "Vinculou à Carga " + carga.Descricao + ".", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao vincular o documento de transporte à carga.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> GerarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura(unitOfWork);

                int.TryParse(Request.Params("DocumentoTransporte"), out int codigoDocumentoTransporte);

                Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporteNatura = repDocumentoTransporteNatura.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documentoTransporteNatura.Cargas.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                    return new JsonpResult(false, true, "O documento de transporte já está vinculado à carga " + documentoTransporteNatura.Cargas.First().Carga.CodigoCargaEmbarcador + ", não sendo possível atualizar os dados.");

                unitOfWork.Start();

                if (!Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura.GerarCargaPorDT(documentoTransporteNatura, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Usuario, unitOfWork, out int codigoCarga, out string msgErro))
                    return new JsonpResult(false, true, msgErro);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(codigoCarga);

                if (!svcNatura.VincularCargaAoDT(Usuario, new List<int>() { documentoTransporteNatura.Codigo }, cargaPedido.Codigo, TipoServicoMultisoftware, unitOfWork, out msgErro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, msgErro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoTransporteNatura, null, "Gerou a carga " + carga.Descricao + " com o DT.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Gerou a carga pelo DT " + documentoTransporteNatura.Descricao + ".", unitOfWork: unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    Carga = carga.CodigoCargaEmbarcador
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocumentoTransporte;
                int.TryParse(Request.Params("DocumentoTransporte"), out codigoDocumentoTransporte);

                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Integracao.DTNatura documentoTransporteNatura = repDocumentoTransporteNatura.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documentoTransporteNatura.Cargas.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                    return new JsonpResult(false, true, "O documento de transporte já está vinculado à carga " + documentoTransporteNatura.Cargas.First().Carga.CodigoCargaEmbarcador + ", não sendo possível atualizar os dados.");

                Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura svcNatura = new Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura(unidadeDeTrabalho);
                svcNatura.ConsultarDT(Usuario, documentoTransporteNatura.Numero, DateTime.MinValue, DateTime.MinValue, unidadeDeTrabalho);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoTransporteNatura, null, "Atualizou o DT.", unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoGeral()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracaoNatura = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura> integracoes = repIntegracaoNatura.Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.DT, true, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repIntegracaoNatura.ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura.DT, true));

                var retorno = (from obj in integracoes
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Protocolo,
                                   obj.Retorno,
                                   Usuario = obj.Usuario?.Nome
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

        public async Task<IActionResult> ConsultarHistoricoIntegracaoDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDocumentoTransporteNatura.BuscarPorCodigo(codigo);

                if (dtNatura == null)
                    return new JsonpResult(false, true, "Documento de transporte não encontrado.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(dtNatura.Integracoes.Count());

                var retorno = (from obj in dtNatura.Integracoes.OrderByDescending(o => o.DataConsulta).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Protocolo,
                                   obj.Retorno,
                                   Usuario = obj.Usuario?.Nome
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura integracao = repIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Integração não encontrada.");

                if (integracao.ArquivoRequisicao == null && integracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta DT " + integracao.Protocolo + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
        }

        public async Task<IActionResult> ConsultarIntegracoesRetorno()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoNatura repIntegracaoNatura = new Repositorio.Embarcador.Integracao.IntegracaoNatura(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoNatura integracaoNatura = repIntegracaoNatura.BuscarPorCodigo(codigo);

                if (integracaoNatura == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 55, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(integracaoNatura.SubIntegracoes.Count());

                var retorno = (from obj in integracaoNatura.SubIntegracoes.OrderByDescending(o => o.DataConsulta).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Retorno
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDTNatura.BuscarPorCodigo(codigo);

                if (dtNatura == null)
                    return new JsonpResult(false, true, "DT Natura não encontrado.");

                var retorno = new
                {
                    dtNatura.Codigo,
                    NotasFiscais = (from obj in dtNatura.NotasFiscais
                                    select new
                                    {
                                        obj.Codigo,
                                        obj.Serie,
                                        obj.Numero,
                                        Chave = obj.Chave ?? string.Empty
                                    }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os detalhes do DT da Natura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDTNatura = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura = repDTNatura.BuscarPorCodigo(codigo);

                if (dtNatura == null)
                    return new JsonpResult(false, true, "DT Natura não encontrado.");

                //if (dtNatura.Cargas.Count > 0)
                //    return new JsonpResult(false, true, "O DT já está vinculado a uma carga, não sendo possível alterar as informações dele.");

                string erro = string.Empty;

                unidadeTrabalho.Start();

                if (!SalvarNotasFiscais(out erro, dtNatura, unidadeTrabalho))
                {
                    unidadeTrabalho.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, dtNatura, null, "Salvou o DT.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os detalhes do DT da Natura.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImportarNOTFIS()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork unidadeTrabalhoAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(_conexao.AdminStringConexao);

            try
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeTrabalho);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);

                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                string id = Request.Params("ID");

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorTipoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Natura, true);

                if (grupoPessoas == null)
                    return new JsonpResult(false, true, "Não existe um grupo de pessoas configurado para integrar com a Natura, não sendo possível importar o arquivo.");

                if (empresa == null)
                    return new JsonpResult(false, true, "Empresa/Filial não encontrada.");

                Dominio.Entidades.LayoutEDI layoutEDI = grupoPessoas.LayoutsEDI.Where(o => o.LayoutEDI != null && o.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO).Select(o => o.LayoutEDI).FirstOrDefault();

                if (layoutEDI == null)
                    return new JsonpResult(false, true, "Não existe um NOTFIS configurado no grupo de pessoas.");

                Servicos.DTO.CustomFile file = HttpContext.GetFile();

                ThreadExecutada = false;
                Sucesso = false;
                LayoutEDI = layoutEDI;
                ArquivoEDI = file.InputStream;

                int executionCount = 0;

                Thread thread = new Thread(GerarNOTFIS, 12582912); //maxStackSize = 12mb

                thread.Start();

                while (!ThreadExecutada)
                {
                    executionCount++;

                    if (executionCount == 20)
                    {
                        thread.Abort();
                        return new JsonpResult(new { ID = id, Sucesso = false, Mensagem = "Ocorreu uma falha ao ler o NOTFIS. Tempo de execução muito longo." });
                    }

                    System.Threading.Thread.Sleep(500);

                    if (ThreadExecutada)
                    {
                        if (Sucesso)
                        {
                            if (!Servicos.Embarcador.Integracao.Natura.IntegracaoDTNatura.GerarDT(out string erro, out List<Dominio.Entidades.Embarcador.Integracao.DTNatura> dtsNatura, null, Usuario, empresa, NOTFIS, unidadeTrabalho, unidadeTrabalhoAdmin, _conexao.StringConexao, _conexao.AdminStringConexao, Auditado, TipoServicoMultisoftware))
                            {
                                unidadeTrabalho.Rollback();
                                return new JsonpResult(new { ID = id, Sucesso = false, Mensagem = erro });
                            }
                        }
                        else
                        {
                            return new JsonpResult(new { ID = id, Sucesso = false, Mensagem = "Ocorreu uma falha ao ler o NOTFIS." });
                        }
                    }
                }

                return new JsonpResult(new { ID = id, Sucesso = true });
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao importar o arquivo.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
                unidadeTrabalhoAdmin.Dispose();
            }
        }

        public static void GerarNOTFIS()
        {
            try
            {
                Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(null, LayoutEDI, ArquivoEDI, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));

                NOTFIS = serLeituraEDI.GerarNotasFis();

                Sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            ThreadExecutada = true;
        }

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Empresa", "Empresa", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Recebedor", "Recebedor", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Status", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false);

            return grid;
        }

        private bool ExecutarPesquisa(out string mensagemErro, out dynamic lista, out int count, string propOrdenar, string dirOrdenar, int inicio, int limite, bool exportacao, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;
            lista = null;
            count = 0;

            long.TryParse(Request.Params("NumeroDocumentoTransporte"), out long numeroDocumentoTransporte);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            bool.TryParse(Request.Params("SemCarga"), out bool semCarga);

            int.TryParse(Request.Params("NumeroNotaFiscal"), out int numeroNotaFiscal);

            Repositorio.Embarcador.Integracao.DocumentoTransporteNatura repDocumentoTransporte = new Repositorio.Embarcador.Integracao.DocumentoTransporteNatura(unitOfWork);
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unitOfWork);

            count = repDocumentoTransporte.ContarConsulta(numeroDocumentoTransporte, numeroNotaFiscal, dataInicial, dataFinal, semCarga);

            if (exportacao && count > 1000)
            {
                mensagemErro = "Exportação não permitida para mais de 1000 registros.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Integracao.DTNatura> documentosTransporte = repDocumentoTransporte.Consultar(numeroDocumentoTransporte, numeroNotaFiscal, dataInicial, dataFinal, semCarga, propOrdenar, dirOrdenar, inicio, limite);

            lista = documentosTransporte.Select(obj => new
            {
                obj.Codigo,
                obj.Numero,
                Empresa = obj.Empresa?.CNPJ_Formatado + " - " + obj.Empresa?.RazaoSocial,
                Recebedor = obj.Recebedor?.Descricao,
                Data = obj.Data.ToString("dd/MM/yyyy"),
                NotasFiscais = string.Join(", ", repNotaFiscalDTNatura.BuscarNumerosPorDT(obj.Codigo)),
                obj.ValorFrete,
                Status = obj.Status ? "Sucesso" : "Problemas",
                obj.Observacao
            }).ToList();

            return true;
        }

        private bool SalvarNotasFiscais(out string erro, Dominio.Entidades.Embarcador.Integracao.DTNatura dtNatura, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Integracao.NotaFiscalDTNatura repNotaFiscalDTNatura = new Repositorio.Embarcador.Integracao.NotaFiscalDTNatura(unidadeTrabalho);

            dynamic notasFiscais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("NotasFiscais"));

            if (dtNatura.NotasFiscais != null && dtNatura.NotasFiscais.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var notaFiscal in notasFiscais)
                    if (notaFiscal.Codigo != null)
                        codigos.Add((int)notaFiscal.Codigo);

                List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura> notaFiscalDeletar = (from obj in dtNatura.NotasFiscais where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < notaFiscalDeletar.Count; i++)
                    repNotaFiscalDTNatura.Deletar(notaFiscalDeletar[i]);
            }
            else
            {
                dtNatura.NotasFiscais = new List<Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura>();
            }

            foreach (var notaFiscal in notasFiscais)
            {
                Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura notaFiscalAdd = notaFiscal.Codigo != null ? repNotaFiscalDTNatura.BuscarPorCodigo((int)notaFiscal.Codigo) : null;

                if (notaFiscalAdd == null)
                    notaFiscalAdd = new Dominio.Entidades.Embarcador.Integracao.NotaFiscalDTNatura();

                notaFiscalAdd.DocumentoTransporte = dtNatura;
                notaFiscalAdd.Numero = (int)notaFiscal.Numero;
                notaFiscalAdd.Serie = (int)notaFiscal.Serie;

                DateTime dataEmissao;
                if (!DateTime.TryParseExact((string)notaFiscal.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    dataEmissao = DateTime.Now;

                notaFiscalAdd.DataEmissao = dataEmissao;

                string chave = (string)notaFiscal.Chave;

                if (string.IsNullOrWhiteSpace(chave) || chave.Trim().Length < 44)
                {
                    erro = "A chave de acesso informada é inválida: " + chave + ".";
                    return false;
                }

                notaFiscalAdd.Chave = chave;

                if (notaFiscalAdd.Codigo > 0)
                    repNotaFiscalDTNatura.Atualizar(notaFiscalAdd);
                else
                {
                    notaFiscalAdd.InseridaManualmente = true;
                    repNotaFiscalDTNatura.Inserir(notaFiscalAdd);
                }
            }

            erro = string.Empty;
            return true;
        }

        #endregion
    }
}
