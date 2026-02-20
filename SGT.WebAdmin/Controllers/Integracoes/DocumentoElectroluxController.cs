using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/DocumentoElectrolux")]
    public class DocumentoElectroluxController : BaseController
    {
		#region Construtores

		public DocumentoElectroluxController(Conexao conexao) : base(conexao) { }

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

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os documentos da Electrolux.");
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
                    return Arquivo(bArquivo, grid.extensaoCSV, grid.tituloExportacao + "." + grid.extensaoCSV);
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
        
        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarDocumentoElectrolux()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            try
            {
                long numeroIdentificacaoServico;
                int codigoEmpresa;
                long.TryParse(Request.Params("NumeroIdentificacaoServico"), out numeroIdentificacaoServico);

                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                if(string.IsNullOrEmpty(empresa?.Configuracao?.IdentificadorTransportadorElectrolux))
                    return new JsonpResult(false, true, "Não foi definido o Identificador Transportador da empresa para efetuar a integração");

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Pendente svcElectrolux = new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Pendente(Usuario, unidadeDeTrabalho, empresa.Configuracao.IdentificadorTransportadorElectrolux,dataInicial,dataFinal);
                var obj = svcElectrolux.ConsultarPendentes(empresa);

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

        [AllowAuthenticate]
        public async Task<IActionResult> VincularCargaAoDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDT = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeDeTrabalho);

                int codigoDocumentoTransporte, codigoCarga;
                int.TryParse(Request.Params("DocumentoTransporte"), out codigoDocumentoTransporte);
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDT.BuscarPorCodigo(codigoDocumentoTransporte);

                Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe svcElectrolux = new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe(Usuario, unidadeDeTrabalho);
                
                string mensagem = string.Empty;

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                if (cargaPedido == null)
                    return new JsonpResult(false, true, "Pedido não encontrado.");

                unidadeDeTrabalho.Start();

                if (!svcElectrolux.VincularCargaAoDT(Usuario, new List<int>() { codigoDocumentoTransporte }, cargaPedido.Codigo, TipoServicoMultisoftware, unidadeDeTrabalho, out mensagem))
                {
                    unidadeDeTrabalho.Rollback();
                    return new JsonpResult(false, true, mensagem);
                }


                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Vinculou o DT " + documentoTransporte.Descricao + ".", unidadeDeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoTransporte, null, "Vinculou à Carga " + carga.Descricao + ".", unidadeDeTrabalho);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int.TryParse(Request.Params("DocumentoTransporte"), out int codigoDocumentoTransporte);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDocumentoTransporte.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documentoTransporte.Cargas.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                    return new JsonpResult(false, true, "O documento de transporte já está vinculado à carga " + documentoTransporte.Cargas.First().Carga.CodigoCargaEmbarcador + ", não sendo possível atualizar os dados.");

                unitOfWork.Start();
                Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe svcElectrolux = new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe(Usuario, unitOfWork);

                if (!svcElectrolux.GerarCargaPorDT(documentoTransporte, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Usuario, unitOfWork, out int codigoCarga, out string msgErro))
                    return new JsonpResult(false, true, msgErro);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(codigoCarga);

                if (!svcElectrolux.VincularCargaAoDT(Usuario, new List<int>() { documentoTransporte.Codigo }, cargaPedido.Codigo, TipoServicoMultisoftware, unitOfWork, out msgErro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, msgErro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoTransporte, null, "Gerou a carga " + carga.Descricao + " com o DT.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Gerou a carga pelo DT " + documentoTransporte.Descricao + ".", unitOfWork: unitOfWork);

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

        [AllowAuthenticate]
        public async Task<IActionResult> AtualizarDocumentoElectrolux()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocumentoTransporte;
                int.TryParse(Request.Params("DocumentoTransporte"), out codigoDocumentoTransporte);

                Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDocumentoTransporte.BuscarPorCodigo(codigoDocumentoTransporte);

                if (documentoTransporte.Cargas.Any(o => o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && o.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada))
                    return new JsonpResult(false, true, "O documento de transporte já está vinculado à carga " + documentoTransporte.Cargas.First().Carga.CodigoCargaEmbarcador + ", não sendo possível atualizar os dados.");

                Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe svcElectrolux = new Servicos.Embarcador.Integracao.Electrolux.IntegracaoElectroluxNOTFIS_Detalhe(Usuario, unidadeDeTrabalho, documentoTransporte.Empresa.Configuracao?.IdentificadorTransportadorElectrolux ?? "", documentoTransporte.NumeroNotfis);
                await svcElectrolux.ConsultarNOTFISAsync(documentoTransporte.Empresa, documentoTransporte);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documentoTransporte, null, "Atualizou o DT.", unidadeDeTrabalho);

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
        
        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracaoGeral()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Consulta", "Tipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Parâmetros", "Parametro", 20, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog> integracoes = repIntegracao.Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux.NotfisPendente, true, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repIntegracao.ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux.NotfisPendente, true));

                var retorno = (from obj in integracoes
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Retorno,
                                   Protocolo = obj.RetornoIdentificadorDocumento ?? "",
                                   Tipo = obj.Tipo.ObterDescricao(),
                                   Usuario = obj.Usuario?.Nome,
                                   Parametro = obj.ParametroDataFinal.HasValue && obj.ParametroDataInicial.HasValue ? obj.ParametroDataInicial.Value.ToString("dd/MM/yyyy") +" à " + obj.ParametroDataFinal.Value.ToString("dd/MM/yyyy") : "" 
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
        public async Task<IActionResult> ConsultarHistoricoIntegracaoDocumentoElectrolux()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte documentoTransporte = repDocumentoTransporte.BuscarPorCodigo(codigo);

                if (documentoTransporte == null)
                    return new JsonpResult(false, true, "Documento de transporte não encontrado.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Protocolo", "Protocolo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 20, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(documentoTransporte.Integracoes.Count());

                var retorno = (from obj in documentoTransporte.Integracoes.OrderByDescending(o => o.DataConsulta).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.DataConsulta.ToString("dd/MM/yyyy HH:mm:ss"),
                                   Protocolo = obj.RetornoIdentificadorDocumento,
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
        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoDocumentoTransporte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = 0;
                long.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxConsultaLog(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxConsultaLog integracao = repIntegracao.BuscarPorCodigo(codigo,false);

                if (integracao == null)
                    return new JsonpResult(true, false, "Integração não encontrada.");

                if (integracao.ArquivoRequisicao == null && integracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta DT " + integracao.Codigo + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
        }
        
        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarIntegracoesRetorno()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repIntegracaoElectrolux = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte integracaoElectrolux = repIntegracaoElectrolux.BuscarPorCodigo(codigo);

                if (integracaoElectrolux == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Retorno", 55, Models.Grid.Align.left, false);

                grid.setarQuantidadeTotal(integracaoElectrolux.Integracoes.Count());

                var retorno = (from obj in integracaoElectrolux.Integracoes.OrderByDescending(o => o.DataConsulta).Skip(grid.inicio).Take(grid.limite)
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDTElectrolux = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte DTElectrolux = repDTElectrolux.BuscarPorCodigo(codigo);

                if (DTElectrolux == null)
                    return new JsonpResult(false, true, "DT Electrolux não encontrado.");

                var retorno = new
                {
                    DTElectrolux.Codigo,
                    NotasFiscais = (from obj in DTElectrolux.NotasFiscais
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
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar os detalhes do DT da Electrolux.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
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
            grid.AdicionarCabecalho("Cargas", "Cargas", 10, Models.Grid.Align.left, false);
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

            string numeroDocumentoTransporte = Request.Params("NumeroDocumentoTransporte");

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            bool.TryParse(Request.Params("SemCarga"), out bool semCarga);

            int.TryParse(Request.Params("NumeroNotaFiscal"), out int numeroNotaFiscal);

            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte repDocumentoTransporte = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoElectrolux repCargaIntegracaoElectrolux = new Repositorio.Embarcador.Cargas.CargaIntegracaoElectrolux(unitOfWork);

            Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal repDocumentoTransporteNotaFiscal = new Repositorio.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporteNotaFiscal(unitOfWork);

            count = repDocumentoTransporte.ContarConsulta(numeroDocumentoTransporte, numeroNotaFiscal, dataInicial, dataFinal, semCarga);

            if (exportacao && count > 1000)
            {
                mensagemErro = "Exportação não permitida para mais de 1000 registros.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Integracao.IntegracaoElectroluxDocumentoTransporte> documentosTransporte = repDocumentoTransporte.Consultar(numeroDocumentoTransporte, numeroNotaFiscal, dataInicial, dataFinal, semCarga, propOrdenar, dirOrdenar, inicio, limite);

            lista = documentosTransporte.Select(obj => new
            {
                obj.Codigo,
                Numero = obj.NumeroNotfis,
                Empresa = obj.Empresa?.CNPJ_Formatado + " - " + obj.Empresa?.RazaoSocial,
                Recebedor = obj.Recebedor?.Descricao,
                Data = obj.Data.ToString("dd/MM/yyyy"),
                NotasFiscais = string.Join(", ", repDocumentoTransporteNotaFiscal.BuscarNumerosPorDT(obj.Codigo)),
                Cargas = string.Join(", ", repCargaIntegracaoElectrolux.BuscarNumerosCargasPorDT(obj.Codigo)),
                obj.ValorFrete,
                Status = obj.Status ? "Sucesso" : "Problemas",
                obj.Observacao
            }).ToList();

            return true;
        }
        /*
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
        */
        #endregion
    }
}
