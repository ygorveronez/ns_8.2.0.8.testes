using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    [CustomAuthorize("DocumentosEmissao/ConsultaDocumentos", "Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class ConsultaDocumentosController : BaseController
    {
		#region Construtores

		public ConsultaDocumentosController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chaveOuNumero = Request.GetStringParam("ChaveOuNumero");
                ClassificacaoNFe classificacaoNFe = Request.GetEnumParam<ClassificacaoNFe>("ClassificacaoNFe");
                string msgAlerta = "";
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = ObterCargaPedido(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDestinado = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                TipoContratacaoCarga tipoContratacaoAnterior = cargaPedido.TipoContratacaoCarga;
                Servicos.Embarcador.Documentos.ConsultaDocumento servicoConsultaDocumento = new Servicos.Embarcador.Documentos.ConsultaDocumento(unitOfWork, TipoServicoMultisoftware, Auditado);

                servicoConsultaDocumento.Consultar(cargaPedido, chaveOuNumero, out msgAlerta, ConfiguracaoEmbarcador.PermitirAutalizarNotaFiscalCarga, classificacaoNFe, TipoServicoMultisoftware);

                if (tipoContratacaoAnterior != cargaPedido.TipoContratacaoCarga || (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.CriarNovoPedidoAoVincularNotaFiscalComDiferentesParticipantes))
                {
                    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                    serHubCarga.InformarCargaAtualizada(cargaPedido.Carga.Codigo, TipoAcaoCarga.Alterada, unitOfWork.StringConexao);

                    Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    return new JsonpResult(servicoCarga.ObterDetalhesDaCarga(cargaPedido.Carga, TipoServicoMultisoftware, unitOfWork));
                }
                else
                {
                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    {
                        chaveOuNumero = chaveOuNumero.ObterSomenteNumeros();
                        if (!string.IsNullOrWhiteSpace(chaveOuNumero) && chaveOuNumero.Length == 44)
                        {
                            if (repDestinado.VerificarNotaCanceladaEmitente(chaveOuNumero))
                                return new JsonpResult(true, true, "A nota informada se encontra cancelada pelos documentos destinados da SEFAZ.");
                            else if (repDestinado.VerificarNotaCartaCorrecaoEmitente(chaveOuNumero))
                                return new JsonpResult(true, true, "A nota informada contem uma carta de correção informada pelos documentos destinados da SEFAZ.");
                            else if (!string.IsNullOrWhiteSpace(msgAlerta))
                                return new JsonpResult(true, true, msgAlerta);
                            else
                                return new JsonpResult(true);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(msgAlerta))
                                return new JsonpResult(true, true, msgAlerta);
                            else
                                return new JsonpResult(true);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(msgAlerta))
                            return new JsonpResult(true, true, msgAlerta);
                        else
                            return new JsonpResult(true);
                    }
                }
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
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

        public async Task<IActionResult> ConsultarMultiplasChaves()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = ObterCargaPedido(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDestinado = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                TipoContratacaoCarga tipoContratacaoAnterior = cargaPedido.TipoContratacaoCarga;
                Servicos.Embarcador.Documentos.ConsultaDocumento servicoConsultaDocumento = new Servicos.Embarcador.Documentos.ConsultaDocumento(unitOfWork, TipoServicoMultisoftware, Auditado);
                List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoConsultaChave> retornoChaves = new List<Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoConsultaChave>();

                dynamic chaves = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Chaves"));
                if (chaves != null && chaves.Count > 0)
                {
                    int quantidadeNotasAdicionadas = 0;
                    foreach (dynamic chaveOuNumeroDigitada in chaves)
                    {
                        string msgAlerta = "";
                        Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoConsultaChave retornoChave = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoConsultaChave();
                        retornoChave.Codigo = Guid.NewGuid().ToString().Replace("-", "");
                        retornoChave.Chave = (string)chaveOuNumeroDigitada.Chave.Chave;
                        retornoChave.Numero = (string)chaveOuNumeroDigitada.Chave.Numero;
                        retornoChave.ClassificacaoNFe = ((string)chaveOuNumeroDigitada.Chave.ClassificacaoNFe).ToEnum<ClassificacaoNFe>();
                        retornoChave.MensagemRetorno = "Nota adicionada com sucesso.";

                        string chaveOuNumero = !string.IsNullOrWhiteSpace(retornoChave.Chave) ? retornoChave.Chave : retornoChave.Numero;
                        if (string.IsNullOrWhiteSpace(retornoChave.Numero))
                            retornoChave.Numero = Utilidades.Chave.ObterNumero(chaveOuNumero).ToString();

                        try
                        {
                            servicoConsultaDocumento.Consultar(cargaPedido, chaveOuNumero, out msgAlerta, ConfiguracaoEmbarcador.PermitirAutalizarNotaFiscalCarga, retornoChave.ClassificacaoNFe, TipoServicoMultisoftware);
                        }
                        catch (ServicoException excecao)
                        {
                            retornoChave.MensagemRetorno = excecao.Message;
                        }

                        if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && string.IsNullOrWhiteSpace(retornoChave.MensagemRetorno))
                        {
                            chaveOuNumero = chaveOuNumero.ObterSomenteNumeros();
                            if (!string.IsNullOrWhiteSpace(chaveOuNumero) && chaveOuNumero.Length == 44)
                            {
                                if (repDestinado.VerificarNotaCanceladaEmitente(chaveOuNumero))
                                    retornoChave.MensagemRetorno = "A nota informada se encontra cancelada pelos documentos destinados da SEFAZ.";
                                else if (repDestinado.VerificarNotaCartaCorrecaoEmitente(chaveOuNumero))
                                    retornoChave.MensagemRetorno = "A nota informada contem uma carta de correção informada pelos documentos destinados da SEFAZ.";
                                else if (!string.IsNullOrWhiteSpace(msgAlerta))
                                    retornoChave.MensagemRetorno = msgAlerta;
                            }
                        }
                        retornoChaves.Add(retornoChave);

                        if (retornoChave.MensagemRetorno.Equals("Nota adicionada com sucesso."))
                            quantidadeNotasAdicionadas++;
                    }
                    return new JsonpResult(new
                    {
                        Chaves = retornoChaves,
                        QuantidadeImportada = quantidadeNotasAdicionadas,
                        QuantidadeTotal = chaves.Count
                    });
                }
                else
                    return new JsonpResult(false, true, "Nenhuma chave/número informada para realizar o envio.");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
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

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Cargas.CargaPedido ObterCargaPedido(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCargaPedido = Request.GetIntParam("CargaPedido");
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            return repositorioCargaPedido.BuscarPorCodigo(codigoCargaPedido) ?? throw new ControllerException("Carga pedido não encontrada");
        }

        #endregion
    }
}
