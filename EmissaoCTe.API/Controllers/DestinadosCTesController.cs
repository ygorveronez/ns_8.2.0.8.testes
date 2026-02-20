using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DestinadosCTesController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                //Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EmitirDesacordoServico(this.EmpresaUsuario.Codigo, "35180317161395000107570010000000121000000127", "TESTE DE DESACORDO DO SERVICO", unitOfWork);

                DateTime.TryParse(Request.Params["DataInicial"], out DateTime dataInicial);
                DateTime.TryParse(Request.Params["DataFinal"], out DateTime dataFinal);

                DateTime.TryParse(Request.Params["DataInicialAutorizacao"], out DateTime dataInicialAutorizacao);
                DateTime.TryParse(Request.Params["DataFinalAutorizacao"], out DateTime dataFinalAutorizacao);

                string status = Request.Params["Status"];
                string cnpjEmissor = Utilidades.String.OnlyNumbers(Request.Params["CnpjEmissor"]);
                string nomeEmissor = Request.Params["NomeEmissor"];
                string chave = Request.Params["Chave"];
                string placa = Request.Params["Placa"];

                string cnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CnpjRemetente"]);
                string nomeRemetente = Request.Params["NomeRemetente"];

                string cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["CnpjTomador"]);
                string nomeTomador = Request.Params["NomeTomador"];

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);
                int.TryParse(Request.Params["Serie"], out int serie);

                bool raizCNPJ = false;
                bool.TryParse(Request.Params["RaizCNPJ"], out raizCNPJ);

                if (raizCNPJ)
                {
                    if (!string.IsNullOrWhiteSpace(cnpjEmissor) && cnpjEmissor.Length > 8)
                        cnpjEmissor = cnpjEmissor.Substring(0, 8);
                    if (!string.IsNullOrWhiteSpace(cnpjRemetente) && cnpjRemetente.Length > 8)
                        cnpjRemetente = cnpjRemetente.Substring(0, 8);
                    if (!string.IsNullOrWhiteSpace(cnpjTomador) && cnpjTomador.Length > 8)
                        cnpjTomador = cnpjTomador.Substring(0, 8);
                }

                bool? cancelado = null;
                if (status == "1")
                    cancelado = true;
                else if (status == "0")
                    cancelado = false;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa? tipoDocumentoDestinado = null;

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa> documentos = repDocumentoDestinadoEmpresa.ConsultarMultiCTe(this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe, tipoDocumentoDestinado, dataInicialAutorizacao, dataFinalAutorizacao, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, cnpjEmissor, nomeEmissor, placa, chave, cancelado, null, cnpjRemetente, nomeRemetente, cnpjTomador, nomeTomador, string.Empty, false, inicioRegistros, 50);
                int count = repDocumentoDestinadoEmpresa.ContarMultiCTe(this.EmpresaUsuario.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloDocumentoDestinado.CTe, tipoDocumentoDestinado, dataInicialAutorizacao, dataFinalAutorizacao, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, cnpjEmissor, nomeEmissor, placa, chave, cancelado, null, cnpjRemetente, nomeRemetente, cnpjTomador, nomeTomador, false, string.Empty);

                var lista = from obj in documentos
                            select new
                            {
                                obj.Codigo,
                                Emissor = obj.CPFCNPJEmitente_Formatado + " " + obj.NomeEmitente,
                                obj.Numero,
                                obj.Serie,
                                Data = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                Tipo = obj.DescricaoTipoDocumento,
                                Status = obj.Cancelado ? "Cancelado" : "Autorizado",
                                Evento = obj.SituacaoManifestacaoDestinatario == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.DescordoServico ? "Desacordo Serviço Autorizado" : ""
                            };

                return Json(lista, true, null, new string[] { "Código", "Emissor|30", "Número|10", "Serie|10", "Data Emissão|10", "Tipo Destinado|10", "Status|10", "Evento|10" }, count);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTesDestinados()
        {
            try
            {
                string url = string.Empty;

                if (this.EmpresaUsuario.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (this.EmpresaUsuario.Localidade.Estado.SefazCTe == null)
                        return Json<bool>(false, false, "Estado sem Sefaz configurado.");
                    url = this.EmpresaUsuario.Localidade.Estado.SefazCTe.UrlDistribuicaoDFe;
                }
                else
                {
                    if (this.EmpresaUsuario.Localidade.Estado.SefazCTeHomologacao == null)
                        return Json<bool>(false, false, "Estado sem Sefaz Homologação configurado.");
                    url = this.EmpresaUsuario.Localidade.Estado.SefazCTeHomologacao.UrlDistribuicaoDFe;
                }

                string mensagemRetorno = string.Empty;
                if (!Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosCTeEmpresa(this.EmpresaUsuario.Codigo, url, Conexao.StringConexao, 0, ref mensagemRetorno, out string codigoStatusRetornoSefaz, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                    return Json<bool>(false, false, mensagemRetorno);
                else
                    return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os dados.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarEventoDesacordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["Codigo"], out int codigo);
                string justificativa = Request.Params["Justificativa"];
                string url = string.Empty;

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Repositorio.Embarcador.Documentos.EventoDesacordoServico repEventoDesacordoServico = new Repositorio.Embarcador.Documentos.EventoDesacordoServico(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return Json<bool>(false, false, "Documento destinado não encontrado.");

                if (documentoDestinado.Cancelado)
                    return Json<bool>(false, false, "Documento destinado esta cancelado.");

                if (string.IsNullOrWhiteSpace(documentoDestinado.Chave))
                    return Json<bool>(false, false, "Documento destinado esta sem chave eletrônica para lançamento do Evento de Desacordo.");

                if (documentoDestinado.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoTomador && documentoDestinado.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.CTeDestinadoEmitente)
                    return Json<bool>(false, false, "Evento de desacordo permitido apenas quando tipo for Destinado Tomador.");

                Dominio.Entidades.Estado estadoEmissorDocumento = repEstado.BuscarPorIBGE(int.Parse(documentoDestinado.Chave.Substring(0, 2)));

                if (estadoEmissorDocumento == null)
                    return Json<bool>(false, false, "Estado do emissor do documento não encontrado.");

                if (this.EmpresaUsuario.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                {
                    if (estadoEmissorDocumento.SefazCTe == null)
                        return Json<bool>(false, false, "Estado do emissor do documento sem Sefaz configurado.");
                    url = estadoEmissorDocumento.SefazCTe.UrlRecepcaoEvento;
                }
                else
                {
                    if (estadoEmissorDocumento.SefazCTeHomologacao == null)
                        return Json<bool>(false, false, "Estado do emissor do documento sem Sefaz Homologação configurado.");
                    url = estadoEmissorDocumento.SefazCTeHomologacao.UrlRecepcaoEvento;
                }

                if (string.IsNullOrWhiteSpace(url))
                    return Json<bool>(false, false, "Sefaz do emissor do documento sem URL de Recepção de Evento configurado.");


                Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico eventoDesacordo = repEventoDesacordoServico.BuscarPorChaveStatus(documentoDestinado.Chave, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Autorizado);

                if (eventoDesacordo != null)
                    return Json<bool>(false, false, "Já existe um evento de desacordo lançado para este documento.");

                var mensagemRetorno = string.Empty;
                if (!Servicos.Embarcador.Documentos.DesacordoPrestacaoServicoCTe.EmitirDesacordoServico(this.EmpresaUsuario.Codigo, documentoDestinado.Chave, justificativa, url, ref mensagemRetorno, unitOfWork))
                    return Json<bool>(false, false, mensagemRetorno);
                else
                {
                    documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.DescordoServico;
                    repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Não foi possível enviar Evento de Desacordo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return Json<bool>(false, false, "Documento destinado não encontrado.");

                string caminho = System.Configuration.ConfigurationManager.AppSettings.Get("CaminhoDocumentosFiscaisEmbarcador");

                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", this.EmpresaUsuario.CNPJ, documentoDestinado.Chave + ".xml");

                if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                {
                    byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
                    return Arquivo(data, "text/xml", string.Concat(documentoDestinado.Chave, ".xml"));
                }
                else
                    return Json<bool>(false, false, "XML não disponível para download.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return Json<bool>(false, false, "Documento destinado não encontrado.");

                if (!documentoDestinado.Cancelado)
                    return Json<bool>(false, false, "Documento destinado não esta cancelado.");

                string caminho = System.Configuration.ConfigurationManager.AppSettings.Get("CaminhoDocumentosFiscaisEmbarcador");

                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "CTe", this.EmpresaUsuario.CNPJ, documentoDestinado.Chave + "_procCancCTe.xml");

                if (Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                {
                    byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
                    return Arquivo(data, "text/xml", string.Concat(documentoDestinado.Chave, "_procCancCTe.xml"));
                }
                else
                    return Json<bool>(false, false, "XML Cancelamento não disponível para download.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML Cancelamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLEventoDesacordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);
                Repositorio.Embarcador.Documentos.EventoDesacordoServico repEventoDesacordoServico = new Repositorio.Embarcador.Documentos.EventoDesacordoServico(unitOfWork);

                Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorCodigo(codigo);

                if (documentoDestinado == null)
                    return Json<bool>(false, false, "Documento destinado não encontrado.");

                if (documentoDestinado.Cancelado)
                    return Json<bool>(false, false, "Documento destinado esta cancelado.");

                Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico eventoDesacordo = repEventoDesacordoServico.BuscarPorChaveStatus(documentoDestinado.Chave, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico.Autorizado);

                if (eventoDesacordo == null)
                    return Json<bool>(false, false, "Não existe evento de desacordo autorizado.");

                if (string.IsNullOrWhiteSpace(eventoDesacordo.XML))
                    return Json<bool>(false, false, "Não existe XML salvo para o evento de desacordo.");

                byte[] data = System.Text.Encoding.Default.GetBytes(eventoDesacordo.XML);

                return Arquivo(data, "text/xml", string.Concat(eventoDesacordo.ChaveCTe, "_procPrestDesacordo.xml"));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML de Evento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}