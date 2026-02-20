using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Controllers.Cargas.CTeManual
{
    [CustomAuthorize("Cargas/CargaCTeManual")]
    public class CargaCTeManualController : BaseController
    {
		#region Construtores

		public CargaCTeManualController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConfirmarEnvioCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
                Servicos.Embarcador.Carga.RateioCTe serRateioCTe = new Servicos.Embarcador.Carga.RateioCTe(unitOfWork);
                Servicos.Embarcador.Carga.Frete serCargaFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork, TipoServicoMultisoftware);
                Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro serFreteSubcontratacaoTerceiro = new Servicos.Embarcador.Carga.FreteSubcontratacaoTerceiro(unitOfWork);
                Servicos.Embarcador.Carga.Documentos svcDocumentosCarga = new Servicos.Embarcador.Carga.Documentos(unitOfWork);
                Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
                Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                Servicos.Embarcador.Carga.CTeSubContratacao serCTeSubContratacao = new Servicos.Embarcador.Carga.CTeSubContratacao(unitOfWork);
                Servicos.Embarcador.Carga.CargaPedido serCargaPedido = new Servicos.Embarcador.Carga.CargaPedido(unitOfWork);
                Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

                unitOfWork.Start();

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool naoGerarMDFe;
                bool.TryParse(Request.Params("NaoGerarMDFe"), out naoGerarMDFe);

                if (codigoCarga > 0)
                {

                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesRemover = repCargaCTe.BuscarPorCargaCanceladosManualmente(codigoCarga);
                    string erro = string.Empty;

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCteRemove in cargaCTesRemover)
                    {
                        if (cargaCteRemove.CTe.Status == "C")
                        {
                            if (!svcDocumentosCarga.GerarMovimentoEmissaoCTe(out erro, cargaCteRemove, TipoServicoMultisoftware, unitOfWork, true))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, erro);
                            }

                            Servicos.Embarcador.Escrituracao.DocumentoEscrituracao.AdicionarDocumentoParaEscrituracao(cargaCteRemove, unitOfWork);
                            Servicos.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento.AdicionarDocumentoParaEscrituracao(cargaCteRemove, unitOfWork);

                            if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out erro, cargaCteRemove, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, erro);
                            }
                        }
                        else if (cargaCteRemove.CTe.Status == "I")
                        {
                            Servicos.Embarcador.Escrituracao.DocumentoEscrituracaoCancelamento.AdicionarDocumentoParaEscrituracao(cargaCteRemove, unitOfWork);
                        }

                        cargaCteRemove.NotasFiscais = null;
                        cargaCteRemove.Componentes = null;
                        cargaCteRemove.CIOTs = null;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCteRemove.CTe, null, "CT-e removido da Carga por confirmação de envio CT-e", unitOfWork);

                        repCargaCTe.Deletar(cargaCteRemove);
                    }

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo, false, false, true);

                    if (cargaCTes.Count > 0)
                    {
                        if (repCargaCTe.ContarPendentesPorCarga(carga.Codigo) == 0)
                        {
                            serRateioCTe.CriarDocumentosCargaCTe(cargaPedidos[0], cargaCTes, TipoServicoMultisoftware, unitOfWork, Auditado);
                            //serCargaFrete.CalcularFreteExtornandoComplementos(ref carga, unitOfWork);
                            serRateioCTe.AjustarFretePorCTes(cargaPedidos[0], cargaCTes, TipoServicoMultisoftware, unitOfWork);
                            serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacaoPorCTe(carga, cargaPedidos, cargaCTes, unitOfWork, configuracaoPedido);
                            serFreteSubcontratacaoTerceiro.CalcularFreteSubcontratacao(carga, carga.TipoFreteEscolhido, unitOfWork, false, TipoServicoMultisoftware, _conexao.StringConexao);

                            serRateioCTe.AlterarDadosCarga(cargaCTes, ref carga, unitOfWork, TipoServicoMultisoftware, Auditado);

                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos;
                            carga.DataInicioEmissaoDocumentos = DateTime.Now;
                            carga.DataEnvioUltimaNFe = DateTime.Now;
                            carga.DataRecebimentoUltimaNFe = DateTime.Now;
                            carga.PossuiPendencia = false;
                            carga.MotivoPendencia = "";
                            carga.NaoGerarMDFe = naoGerarMDFe;

                            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                            repCarga.Atualizar(carga, Auditado);

                            unitOfWork.CommitChanges();

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Confirmação de Envio de CT-e", unitOfWork);

                            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                            serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                            return new JsonpResult(true);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "Não é possível confirmar o envio pois existem CT-es pendentes.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Para confirmar a emissão dos CT-es é necessário que sejam emitidos CT-es para a carga.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A carga Informada não é válida. ");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o envio do(s) CT-e(s) para carga. ");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EmitirCTeManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                Servicos.Embarcador.Carga.ComponetesFrete serComponentesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
                Servicos.Embarcador.Seguro.Seguro svcSeguro = new Servicos.Embarcador.Seguro.Seguro(unitOfWork);
                Servicos.Embarcador.Carga.CTe svcCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));

                if (codigoCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao repApoliceSeguroAverbacao = new Repositorio.Embarcador.Seguros.ApoliceSeguroAverbacao(unitOfWork);
                    Repositorio.Embarcador.CTe.CTeRelacaoDocumento repCTeRelacaoDocumento = new Repositorio.Embarcador.CTe.CTeRelacaoDocumento(unitOfWork);
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                    Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                        return new JsonpResult(false, false, "Não é possível adicionar ou duplicar um CT-e a uma carga Bloqueada.");

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(carga.Codigo);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (carga.SituacaoCarga == SituacaoCarga.AgNFe || carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.CalculoFrete))
                    {
                        if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                            return new JsonpResult(false, true, "Não é possível adicionar um CT-e a uma carga que esteja na situação atual (" + carga.DescricaoSituacaoCarga + ").");
                        else if (carga.CargaTransbordo)
                            return new JsonpResult(false, true, "Não é possível adicionar um CT-e a uma carga de transbordo.");
                        else if (carga.TipoOperacao != null && carga.TipoOperacao.CTeEmitidoNoEmbarcador)
                            return new JsonpResult(false, true, "Não é possível adicionar um CT-e a uma carga de importação de conhecimento anterior.");
                        else if (repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo))
                            return new JsonpResult(false, true, "Não é possível adicionar um CT-e a uma carga de importação de conhecimento anterior.");
                        else if (repCargaPedido.ExisteNotaFiscalVinculada(carga.Codigo))
                            return new JsonpResult(false, true, "Não é possível adicionar um CT-e a uma carga contendo documentos vinculados.");
                        else if (repCargaPedido.ExisteCTeAnteriorVinculada(carga.Codigo))
                            return new JsonpResult(false, true, "Não é possível adicionar um CT-e a uma carga contendo documentos vinculados.");
                    }

                    if (Usuario.LimitarOperacaoPorEmpresa && carga.Empresa != null && !Usuario.Empresas.Contains(carga.Empresa))
                        return new JsonpResult(false, true, "Você não possui permissão para fazer operações nessa Empresa da carga.");

                    if (!carga.Pedidos.Any(o => o.ApoliceSeguroAverbacao.Any()))
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                        Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCarga(carga, cargaPedidos, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork);
                    }

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>() {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.total
                    };

                    Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);
                    cteIntegracao.NumeroOS = cargaPedido.Pedido?.NumeroOS ?? "";

                    if (!serCTe.EmitirCTeManualmente(cteIntegracao, codigoCarga, this.Usuario, Auditado, ConfiguracaoEmbarcador, out string msgRetorno, TipoServicoMultisoftware, unitOfWork, _conexao.StringConexao, false))
                        return new JsonpResult(false, true, msgRetorno);

                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "O CT-e Informado não é válido. ");
                }
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterInformacoesXMLNFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

            int codigoCarga = Request.GetIntParam("Codigo");

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                return new JsonpResult(false, "Não é possível adicionar arquivo CT-e, a carga está bloqueada");
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();
                if (files.Count <= 0)
                    return new JsonpResult(false, true, "Quantidade de arquivos inválida para a importação.");

                List<object> retorno = new List<object>();

                for (var i = 0; i < files.Count; i++)
                {
                    try
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

                        if (extensao == ".xml")
                        {
                            Servicos.NFe svcNFe = new Servicos.NFe(unidadeTrabalho);
                            object documento = svcNFe.ObterDocumentoPorXML(file.InputStream, this.Empresa.Codigo, null, unidadeTrabalho);

                            if (documento == null)
                                retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "Verifique se o XML é de uma NF-e autorizada." });
                            else
                                retorno.Add(new { Indice = i, Sucesso = true, Documento = documento });
                        }
                        else
                        {
                            retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "A extensão " + extensao + " é inválida. Somente a extensão XML é aceita." });
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "Não foi possível ler o XML. Verifique se o XML da NF-e é válido e está formatado corretamente." });
                    }
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do XML da(s) NF-e(s).");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }


        public async Task<IActionResult> ImportarCTeEmbarcador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);           
            try
            {
                Servicos.DTO.CustomFile file = HttpContext.GetFile();

                if (file == null)
                    return new JsonpResult(false, true, "Nehum arquivos informado.");
                
                try
                {
                    string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                    if (extensao == ".xml")
                    {
                        var objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);
                        if (objCTe != null)
                        {
                            object cteRetorno = ProcessarXMLCTe(objCTe, file.InputStream, unitOfWork);                       
                            if (cteRetorno is string)
                            {                                
                                return new JsonpResult(false, true, (string)cteRetorno);
                            }
                            else if (cteRetorno is Dominio.Entidades.ConhecimentoDeTransporteEletronico)
                            {                               
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)cteRetorno;
                                return new JsonpResult(new {
                                    cteConvertido.Codigo,
                                    cteConvertido.Chave,
                                    cteConvertido.Numero,
                                    GrupoPessoas = cteConvertido.TomadorPagador.GrupoPessoas.Descricao,
                                    Origem = cteConvertido.LocalidadeInicioPrestacao.Descricao + " - " + cteConvertido.LocalidadeInicioPrestacao.Estado.Sigla,
                                    Destino = cteConvertido.LocalidadeTerminoPrestacao.Descricao + " - " + cteConvertido.LocalidadeTerminoPrestacao.Estado.Sigla,
                                    ValorFrete = cteConvertido.ValorAReceber.ToString("n2"),
                                    Aliquota = cteConvertido.AliquotaICMS.ToString("n2"),
                                    NotasFiscais = cteConvertido.NumeroNotas,
                                    SituacaoCTe = cteConvertido.Status,
                                    Observacao = cteConvertido.ObservacoesGerais,
                                    NumeroModeloDocumentoFiscal = cteConvertido.ModeloDocumentoFiscal.Numero,
                                    CodigoEmpresa = cteConvertido.Empresa.Codigo,
                                    DataEmissao = cteConvertido.DataEmissao?.ToString("dd/MM/yyyy HH:mm:ss") ?? ""
                                });
                            }
                            else {
                                return new JsonpResult(false, true, "Conhecimento de transporte inválido.");
                            }                            
                        }
                        else {
                            return new JsonpResult(false, true, "Não foi possí­vel ler o XML. Verifique se o XML do CT-e é válido e está formatado corretamente.");
                        }
                    }
                    else
                    {
                        return new JsonpResult(false, true, "A extensão " + extensao + " é inválida. Somente a extensão XML é aceita.");                        
                    }

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);                    
                    return new JsonpResult(false, true, "Não foi possí­vel ler o XML. Verifique se o XML do CT-e é válido e está formatado corretamente.");

                }
                finally
                {
                    if (file != null)
                        file.InputStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do XML do(s) CT-e(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterInformacoesXMLCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, true, "Quantidade de arquivos inválida para a importação.");

                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.UnidadeDeMedida repUnidadeDeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, "Não é possível adicionar arquivo CT-e, a carga está bloqueada");
                List<object> retorno = new List<object>();

                Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                for (var i = 0; i < files.Count; i++)
                {
                    try
                    {
                        Servicos.DTO.CustomFile file = files[i];

                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();

                        if (extensao == ".xml")
                        {
                            object objCTe = MultiSoftware.CTe.Servicos.Leitura.Ler(file.InputStream);

                            Type tipoCTe = objCTe.GetType();

                            if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc) ||
                                tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc) ||
                                tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                            {
                                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);

                                decimal valorFrete = 0;
                                List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional> componentesConfigurados = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();
                                List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga> quantidadesCarga = new List<Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga>();

                                if (cte.ValorFrete != null && cte.ValorFrete.ComponentesAdicionais != null)
                                {
                                    foreach (Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componente in cte.ValorFrete.ComponentesAdicionais)
                                    {
                                        if (string.Compare(componente.Componente.Descricao, carga?.TipoOperacao?.DescricaoComponenteFreteEmbarcador ?? "", true) == 0)
                                            valorFrete += componente.ValorComponente;

                                        if (carga?.TipoOperacao != null)
                                        {
                                            foreach (Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes tipoOperacaoComponentes in carga.TipoOperacao.TipoOperacaoConfiguracoesComponentes)
                                            {
                                                if ((string.Compare(componente.Componente.Descricao, tipoOperacaoComponentes.OutraDescricaoCTe, true) == 0)
                                                    || (string.Compare(componente.Componente.Descricao, tipoOperacaoComponentes.ComponenteFrete.Descricao, true) == 0))
                                                {
                                                    componente.Componente.CodigoIntegracao = tipoOperacaoComponentes.ComponenteFrete.Codigo.ToString();
                                                    componentesConfigurados.Add(componente);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (cte.QuantidadesCarga != null)
                                {
                                    foreach (Dominio.ObjetosDeValor.Embarcador.CTe.QuantidadeCarga quantidadeCarga in cte.QuantidadesCarga)
                                    {
                                        Dominio.Entidades.UnidadeDeMedida unidadeMedida = repUnidadeDeMedida.BuscarPorCodigoUnidade(string.Format("{0:00}", (int)quantidadeCarga.Unidade));
                                        if (unidadeMedida != null)
                                        {
                                            quantidadeCarga.UnidadeMedida = new Dominio.ObjetosDeValor.Embarcador.CTe.UnidadeMedida()
                                            {
                                                Codigo = unidadeMedida.Codigo,
                                                Descricao = unidadeMedida.Descricao
                                            };

                                            quantidadesCarga.Add(quantidadeCarga);
                                        }
                                    }
                                }

                                cte.ValorFrete.FreteProprio = valorFrete;
                                cte.ValorFrete.ComponentesAdicionais = componentesConfigurados;
                                cte.QuantidadesCarga = quantidadesCarga;

                                if (cte == null)
                                    retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "Verifique se o XML é de um CT-e autorizado." });
                                else
                                    retorno.Add(new { Indice = i, Sucesso = true, Documento = cte });
                            }
                            else
                                retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "A versão do CT-e não é suportada. Verifique se o arquivo é válido." });
                        }
                        else
                            retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "A extensão " + extensao + " é inválida. Somente a extensão XML é aceita." });
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);

                        retorno.Add(new { Indice = i, Sucesso = false, Mensagem = "Não foi possível ler o XML. Verifique se o XML do CT-e é válido e está formatado corretamente." });
                    }
                }

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter as informações do XML do(s) CT-e(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VerificarSePossuiMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                bool possuiMDFe = false;

                if (carga.CargaCTes.Any(o => o.CTe.Status == "A" && o.CTe.LocalidadeInicioPrestacao.Estado.Sigla != o.CTe.LocalidadeTerminoPrestacao.Estado.Sigla))
                    possuiMDFe = true;

                return new JsonpResult(new { PossuiMDFe = possuiMDFe });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao verificar se a carga possui MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AnularGerencialCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                string justificativa = Request.GetStringParam("Justificativa");

                Servicos.Embarcador.CTe.CTe svcCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);
                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);
                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal = repCargaCTe.BuscarPorCargaECTe(codigoCarga, codigoCTe);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, false, "Não é possível anular CTe, carga bloqueada.");

                if (cargaCTeOriginal == null)
                    return new JsonpResult(false, true, "CT-e não encontrado.");

                if (cargaCTeOriginal.CTe.Status != "A" && cargaCTeOriginal.CTe.Status != "Z")
                    return new JsonpResult(false, true, $"A situação do CT-e ({cargaCTeOriginal.CTe.DescricaoStatus}) não permite a emissão de CT-e de anulação gerencial.");

                if (cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Encerrada)
                    return new JsonpResult(false, true, $"A situação da carga ({cargaCTeOriginal.Carga.SituacaoCarga.ObterDescricao()}) não permite a emissão de CT-e de anulação gerencial.");

                if (cargaCTeOriginal.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é permitido editar/gerar CT-es em uma carga de transbordo. Selecione a carga original do CT-e.");

                if (Usuario.LimitarOperacaoPorEmpresa && carga.Empresa != null && !Usuario.Empresas.Contains(carga.Empresa))
                    return new JsonpResult(false, true, "Você não possui permissão para fazer operações nessa Empresa da carga.");

                if (!Servicos.Embarcador.CTe.CTe.VerificarSeCTeEstaAptoParaCancelamento(out string mensagemErro, cargaCTeOriginal, unitOfWork, true, true))
                    return new JsonpResult(false, true, mensagemErro);

                unitOfWork.Start();

                cargaCTeOriginal.CTe.AnuladoGerencialmente = true;
                cargaCTeOriginal.CTe.Status = "Z";
                cargaCTeOriginal.CTe.DataRetornoSefaz = DateTime.Now;
                cargaCTeOriginal.CTe.DataAnulacao = DateTime.Now;
                cargaCTeOriginal.CTe.ObservacaoCancelamento = justificativa;
                cargaCTeOriginal.CTe.UsuarioEmissaoCTe = Usuario;

                repCTe.Atualizar(cargaCTeOriginal.CTe);

                if (!Servicos.Embarcador.Carga.Cancelamento.GerarMovimentoCancelamentoCTe(out string erro, cargaCTeOriginal, TipoServicoMultisoftware, unitOfWork, "", false, true))
                    throw new ControllerException("Não foi possível gerar a movimentação de anulação do CT-e.");

                if (!Servicos.Embarcador.Carga.Cancelamento.ReverterItensEmAbertoAposCancelamentoCTe(out erro, cargaCTeOriginal, TipoServicoMultisoftware, unitOfWork))
                    throw new ControllerException("Não foi possível reverter os itens em aberto na anulação do CT-e");

                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCTe(cargaCTeOriginal.CTe.Codigo);
                if (documentoFaturamento != null)
                {
                    documentoFaturamento.Situacao = SituacaoDocumentoFaturamento.Anulado;
                    documentoFaturamento.DataAnulacao = DateTime.Now.Date;

                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeOriginal, null, "CT-e anulado gerencialmente pela tela de CT-e Manual", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeOriginal.CTe, null, "CT-e anulado gerencialmente pela tela de CT-e Manual", unitOfWork);

                if (ConfiguracaoEmbarcador.DeixarCargaPendenteDeIntegracaoAposCTeManual)
                {
                    Servicos.Log.TratarErro($"Atualização de CAR_CARGA_INTEGRADA_EMBARCADOR na carga {carga.Codigo} para false pelo CargaCTeManualController > AnularGerencialCTe", "AtualizacaoCargaIntegradaEmbarcador");

                    carga.CargaIntegradaEmbarcador = false;
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou a carga para pendente de integração devido a anulação gerencial.", unitOfWork);
                }
                servicoCargaCTe.GerarIntegracoesCargaCTeManual(cargaCTeOriginal.CTe, false, cargaCTeOriginal.Carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { EmitidoComSucesso = true });
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao anular gerencialmente o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnularCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                double tomadorCTeSubstituto = Request.GetDoubleParam("TomadorCTeSubstituto");

                DateTime dataEventoDesacordo = Request.GetDateTimeParam("DataEventoDesacordo");

                decimal valorFreteSubstituicao = Request.GetDecimalParam("ValorCTeSubstituicao");

                string observacaoCTeAnulacao = Request.GetStringParam("ObservacaoCTeAnulacao");
                string observacaoCTeSubstituicao = Request.GetStringParam("ObservacaoCTeSubstituicao");

                if (valorFreteSubstituicao <= 0m)
                    return new JsonpResult(false, true, "O valor do frete para o CT-e de substituição deve ser maior que zero.");

                Servicos.Embarcador.CTe.CTe svcCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal = repCargaCTe.BuscarPorCargaECTe(codigoCarga, codigoCTe);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, false, "Não é possível anular CTe, carga bloqueada.");
                if (cargaCTeOriginal == null)
                    return new JsonpResult(false, true, "CT-e não encontrado.");

                if (valorFreteSubstituicao > cargaCTeOriginal.CTe.ValorAReceber)
                    return new JsonpResult(false, true, "O valor do frete para o CT-e de substituição deve ser menor que o valor do frete do CT-e original.");

                if (cargaCTeOriginal.CTe.Status != "A" && cargaCTeOriginal.CTe.Status != "Z")
                    return new JsonpResult(false, true, $"A situação do CT-e ({cargaCTeOriginal.CTe.DescricaoStatus}) não permite a emissão de CT-e de anulação.");

                if (cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    return new JsonpResult(false, true, $"A situação da carga ({cargaCTeOriginal.Carga.SituacaoCarga.ObterDescricao()}) não permite a emissão de CT-e de anulação.");

                if (cargaCTeOriginal.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é permitido editar/gerar CT-es em uma carga de transbordo. Selecione a carga original do CT-e.");

                if (Usuario.LimitarOperacaoPorEmpresa && carga.Empresa != null && !Usuario.Empresas.Contains(carga.Empresa))
                    return new JsonpResult(false, true, "Você não possui permissão para fazer operações nessa Empresa da carga.");

                if (!Servicos.Embarcador.CTe.CTe.GerarCTeAnulacao(out string mensagemErro, out Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, cargaCTeOriginal, codigoCarga, dataEventoDesacordo, valorFreteSubstituicao, observacaoCTeAnulacao, observacaoCTeSubstituicao, unitOfWork, Auditado, tomadorCTeSubstituto, ConfiguracaoEmbarcador, false, Request.GetListParam<dynamic>("ComponentesFrete")))
                {
                    if (controleGeracaoCTeAnulacao != null)
                        return new JsonpResult(new { EmitidoComSucesso = false });
                    else
                        return new JsonpResult(false, true, mensagemErro);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
                {
                    Carga = carga,
                    CTe = cargaCTeOriginal.CTe
                };

                Servicos.Embarcador.Escrituracao.CancelamentoPagamento.GerarCancelamentoPagamentoAutomatico(cargaCancelamento, unitOfWork, true);

                if (ConfiguracaoEmbarcador.DeixarCargaPendenteDeIntegracaoAposCTeManual)
                {
                    Servicos.Log.TratarErro($"Atualização de CAR_CARGA_INTEGRADA_EMBARCADOR na carga {carga.Codigo} para false pelo CargaCTeManualController > AnularCTe", "AtualizacaoCargaIntegradaEmbarcador");

                    carga.CargaIntegradaEmbarcador = false;
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou a carga para pendente de integração devido a anulação do CT-e.", unitOfWork);
                }

                cargaCTeOriginal.CTe.UsuarioEmissaoCTe = Usuario;
                repCargaCTe.Atualizar(cargaCTeOriginal);

                return new JsonpResult(new { EmitidoComSucesso = true });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o CT-e de anulação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnularCTeSemSubstituicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                DateTime dataEventoDesacordo = Request.GetDateTimeParam("DataEventoDesacordo");
                string observacaoCTeAnulacao = Request.GetStringParam("ObservacaoCTeAnulacao");

                Servicos.Embarcador.CTe.CTe svcCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal = repCargaCTe.BuscarPorCargaECTe(codigoCarga, codigoCTe);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, false, "Não é possível anular CTe, carga bloqueada.");

                if (cargaCTeOriginal == null)
                    return new JsonpResult(false, true, "CT-e não encontrado.");

                if (cargaCTeOriginal.CTe.Status != "A" && cargaCTeOriginal.CTe.Status != "Z")
                    return new JsonpResult(false, true, $"A situação do CT-e ({cargaCTeOriginal.CTe.DescricaoStatus}) não permite a emissão de CT-e de anulação.");

                if (cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    return new JsonpResult(false, true, $"A situação da carga ({cargaCTeOriginal.Carga.SituacaoCarga.ObterDescricao()}) não permite a emissão de CT-e de anulação.");

                if (cargaCTeOriginal.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é permitido editar/gerar CT-es em uma carga de transbordo. Selecione a carga original do CT-e.");

                if (Usuario.LimitarOperacaoPorEmpresa && carga.Empresa != null && !Usuario.Empresas.Contains(carga.Empresa))
                    return new JsonpResult(false, true, "Você não possui permissão para fazer operações nessa Empresa da carga.");

                if (!Servicos.Embarcador.CTe.CTe.GerarCTeAnulacao(out string mensagemErro, out Dominio.Entidades.Embarcador.CTe.ControleGeracaoCTeAnulacao controleGeracaoCTeAnulacao, cargaCTeOriginal, codigoCarga, dataEventoDesacordo, 0, observacaoCTeAnulacao, "", unitOfWork, Auditado, 0, ConfiguracaoEmbarcador, true))
                {
                    if (controleGeracaoCTeAnulacao != null)
                        return new JsonpResult(new { EmitidoComSucesso = false });
                    else
                        return new JsonpResult(false, true, mensagemErro);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
                {
                    Carga = carga,
                    CTe = cargaCTeOriginal.CTe
                };

                Servicos.Embarcador.Escrituracao.CancelamentoPagamento.GerarCancelamentoPagamentoAutomatico(cargaCancelamento, unitOfWork, true);

                if (ConfiguracaoEmbarcador.DeixarCargaPendenteDeIntegracaoAposCTeManual)
                {
                    Servicos.Log.TratarErro($"Atualização de CAR_CARGA_INTEGRADA_EMBARCADOR na carga {carga.Codigo} para false pelo CargaCTeManualController > AnularCTeSemSubstituicao", "AtualizacaoCargaIntegradaEmbarcador");

                    carga.CargaIntegradaEmbarcador = false;
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Alterou a carga para pendente de integração devido a anulação do CT-e.", unitOfWork);
                }

                cargaCTeOriginal.CTe.UsuarioEmissaoCTe = Usuario;
                repCargaCTe.Atualizar(cargaCTeOriginal);

                return new JsonpResult(new { EmitidoComSucesso = true });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o CT-e de anulação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularCTeEmbarcadorCarga()
        {

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repConhecimentoDeTransporteEletronico = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);           
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeTrabalho);

            
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/CargaCTeManual");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CTeManual_VincularCTeEmbarcador) || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    throw new CustomException("Você não possui permissões para executar esta ação.");
                
                int codCarga = int.Parse(Request.Params("Carga"));
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                if (carga == null) {
                    throw new CustomException($"Carga {codCarga} não localizada");
                }

                List<SituacaoCarga> status = new List<SituacaoCarga> { 
                    SituacaoCarga.NaLogistica, 
                    SituacaoCarga.Nova, 
                    SituacaoCarga.CalculoFrete, 
                    SituacaoCarga.AgNFe, 
                    SituacaoCarga.PendeciaDocumentos, 
                    SituacaoCarga.Anulada, 
                    SituacaoCarga.Cancelada                 
                };

                if (status.Contains(carga.SituacaoCarga)) {
                    throw new CustomException($"Essa operação não é permitida para situação atual da carga");
                }

                List<dynamic> CTesEmbarcador = Request.GetListParam<dynamic>("CTesEmbarcador");

                if (CTesEmbarcador != null && CTesEmbarcador.Count > 0)
                {
                    unidadeTrabalho.Start();
                    foreach (var CTeEmbarcador in CTesEmbarcador)
                    {
                        int codCTe;
                        int.TryParse((string)CTeEmbarcador.Codigo, out codCTe);
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repConhecimentoDeTransporteEletronico.BuscarPorCodigo(codCTe);
                        if (conhecimento == null) {
                            throw new CustomException($"CTe {codCTe} não localizado");
                        }

                        if ((conhecimento.Empresa?.Codigo ?? 0) != (carga.Empresa?.Codigo ?? 0)) {
                            throw new CustomException($"A empresa/filial da carga: {carga.CodigoCargaEmbarcador}, não é a mesma do CT-e: {conhecimento.Numero}.");
                        }



                        Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCTe(conhecimento.Codigo);
                        if (cargaCTe == null)
                        {
                            cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe();
                            cargaCTe.Carga = carga;
                            cargaCTe.CargaOrigem = carga;
                            cargaCTe.CTe = conhecimento;
                            cargaCTe.VinculoManual = true;
                            cargaCTe.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor.MultiCTe;
                            repCargaCTe.Inserir(cargaCTe);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, $"Vinculou o do CT-e do embarcador não {conhecimento.Descricao} á carga.", unidadeTrabalho);

                            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
                            serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unidadeTrabalho.StringConexao);
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracaoKMM = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
                        if(cargaIntegracaoKMM != null)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracaoKMM = repCargaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(cargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM).FirstOrDefault();

                            if (cargaCTeIntegracaoKMM == null)
                            {
                                new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unidadeTrabalho).AdicionarCargaCTeParaIntegracao(cargaCTe, cargaIntegracaoKMM.TipoIntegracao);
                            }

                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracaoKMM = repCargaDadosTransporteIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
                        if (cargaDadosTransporteIntegracaoKMM != null)
                        {
                            cargaDadosTransporteIntegracaoKMM.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaDadosTransporteIntegracaoKMM, null, "Reenviou Integração, vinculo CT-e embarcador.", unidadeTrabalho);
                            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracaoKMM);
                        }
                    }
                    unidadeTrabalho.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (CustomException ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar o vinculo do CTE do embarcador.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SubstituirCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                double tomadorCTeSubstituto = Request.GetDoubleParam("TomadorCTeSubstituto");

                DateTime dataEventoDesacordo = Request.GetDateTimeParam("DataEventoDesacordo");

                decimal valorFreteSubstituicao = Request.GetDecimalParam("ValorCTeSubstituicao");

                string observacaoCTeSubstituicao = Request.GetStringParam("ObservacaoCTeSubstituicao");

                if (valorFreteSubstituicao <= 0m)
                    return new JsonpResult(false, true, "O valor do frete para o CT-e de substituição deve ser maior que zero.");

                Servicos.Embarcador.CTe.CTe servicoCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal = repCargaCTe.BuscarPorCargaECTe(codigoCarga, codigoCTe);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga.CargaBloqueadaParaEdicaoIntegracao)
                    return new JsonpResult(false, false, "Não é possível substituir CTe, carga bloqueada.");

                if (cargaCTeOriginal == null)
                    return new JsonpResult(false, true, "CT-e não encontrado.");

                if (valorFreteSubstituicao > cargaCTeOriginal.CTe.ValorAReceber)
                    return new JsonpResult(false, true, "O valor do frete para o CT-e de substituição deve ser menor que o valor do frete do CT-e original.");

                if (cargaCTeOriginal.CTe.Status != "A" && cargaCTeOriginal.CTe.Status != "Z")
                    return new JsonpResult(false, true, $"A situação do CT-e ({cargaCTeOriginal.CTe.DescricaoStatus}) não permite a emissão de CT-e de substituição.");

                if (cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                    cargaCTeOriginal.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                    return new JsonpResult(false, true, $"A situação da carga ({cargaCTeOriginal.Carga.SituacaoCarga.ObterDescricao()}) não permite a emissão de CT-e de substituição.");

                if (cargaCTeOriginal.Carga.CargaTransbordo)
                    return new JsonpResult(false, true, "Não é permitido editar/gerar CT-es em uma carga de transbordo. Selecione a carga original do CT-e.");

                if (Usuario.LimitarOperacaoPorEmpresa && carga.Empresa != null && !Usuario.Empresas.Contains(carga.Empresa))
                    return new JsonpResult(false, true, "Você não possui permissão para fazer operações nessa Empresa da carga.");

                servicoCTe.GerarCTeSubstituicao(cargaCTeOriginal, carga, dataEventoDesacordo, valorFreteSubstituicao, observacaoCTeSubstituicao, unitOfWork, Auditado, tomadorCTeSubstituto, ConfiguracaoEmbarcador, Request.GetListParam<dynamic>("ComponentesFrete"));

                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaCancelamento
                {
                    Carga = carga,
                    CTe = cargaCTeOriginal.CTe
                };

                Servicos.Embarcador.Escrituracao.CancelamentoPagamento.GerarCancelamentoPagamentoAutomatico(cargaCancelamento, unitOfWork, true);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o CT-e de substituição.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RetornarDadosPadraoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeOriginal = repCargaCTe.BuscarPrimeirPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                    return new JsonpResult(false, false, "A carga está bloqueada e não pode ser editada.");

                if ((cargaCTeOriginal == null || cargaCTeOriginal.CTe == null) && carga == null)
                    return new JsonpResult(false, true, "Não foi encontrado nenhum CT-e para a carga informada, favor digite os campos manualmente.");

                var retorno = new
                {
                    TipoTomador = cargaCTeOriginal?.CTe?.TipoTomador ?? Dominio.Enumeradores.TipoTomador.Remetente,
                    CodigoEmpresa = cargaCTeOriginal?.CTe?.Empresa?.Codigo ?? carga?.Empresa?.Codigo ?? 0,
                    DescricaoEmpresa = cargaCTeOriginal?.CTe?.Empresa?.RazaoSocial ?? carga?.Empresa?.RazaoSocial ?? "",
                    CodigoSerie = cargaCTeOriginal?.CTe?.Serie?.Codigo ?? 0,
                    DescricaoSerie = cargaCTeOriginal?.CTe?.Serie?.Numero ?? 0,
                    CodigoLocalidadeEmissao = cargaCTeOriginal?.CTe?.LocalidadeEmissao?.Codigo ?? 0,
                    DescricaoLocalidadeEmissao = cargaCTeOriginal?.CTe?.LocalidadeEmissao?.Descricao ?? "",
                    TipoModal = cargaCTeOriginal?.CTe?.TipoModal ?? TipoModal.Rodoviario,
                    CodigoCFOP = cargaCTeOriginal?.CTe?.CFOP?.Codigo ?? 0,
                    DescricaoCFOP = cargaCTeOriginal?.CTe?.CFOP?.CodigoCFOP ?? 0,
                    CodigoPortoOrigem = cargaCTeOriginal?.CTe?.PortoOrigem?.Codigo ?? 0,
                    DescricaoPortoOrigem = cargaCTeOriginal?.CTe?.PortoOrigem?.Descricao ?? "",
                    CodigoPortoPassagemUm = cargaCTeOriginal?.CTe?.PortoPassagemUm?.Codigo ?? 0,
                    DescricaoPortoPassagemUm = cargaCTeOriginal?.CTe?.PortoPassagemUm?.Descricao ?? "",
                    CodigoPortoPassagemDois = cargaCTeOriginal?.CTe?.PortoPassagemDois?.Codigo ?? 0,
                    DescricaoPortoPassagemDois = cargaCTeOriginal?.CTe?.PortoPassagemDois?.Descricao ?? "",

                    CodigoPortoPassagemTres = cargaCTeOriginal?.CTe?.PortoPassagemTres?.Codigo ?? 0,
                    DescricaoPortoPassagemTres = cargaCTeOriginal?.CTe?.PortoPassagemTres?.Descricao ?? "",

                    CodigoPortoPassagemQuatro = cargaCTeOriginal?.CTe?.PortoPassagemQuatro?.Codigo ?? 0,
                    DescricaoPortoPassagemQuatro = cargaCTeOriginal?.CTe?.PortoPassagemQuatro?.Descricao ?? "",

                    CodigoPortoPassagemCinco = cargaCTeOriginal?.CTe?.PortoPassagemCinco?.Codigo ?? 0,
                    DescricaoPortoPassagemCinco = cargaCTeOriginal?.CTe?.PortoPassagemCinco?.Descricao ?? "",

                    CodigoPortoDestino = cargaCTeOriginal?.CTe?.PortoDestino?.Codigo ?? 0,
                    DescricaoPortoDestino = cargaCTeOriginal?.CTe?.PortoDestino?.Descricao ?? "",

                    CodigoTerminalOrigem = cargaCTeOriginal?.CTe?.TerminalOrigem?.Codigo ?? 0,
                    DescricaoTerminalOrigem = cargaCTeOriginal?.CTe?.TerminalOrigem?.Descricao ?? "",

                    CodigoTerminalDestino = cargaCTeOriginal?.CTe?.TerminalDestino?.Codigo ?? 0,
                    DescricaoTerminalDestino = cargaCTeOriginal?.CTe?.TerminalDestino?.Descricao ?? "",

                    CodigoViagem = cargaCTeOriginal?.CTe?.Viagem?.Codigo ?? 0,
                    DescricaoViagem = cargaCTeOriginal?.CTe?.Viagem?.Descricao ?? "",

                    NumeroBooking = cargaCTeOriginal?.CTe?.NumeroBooking,
                    DescricaoCarrier = cargaCTeOriginal?.CTe?.DescricaoCarrier,
                    TipoPropostaFeeder = cargaCTeOriginal?.CTe?.TipoPropostaFeeder,

                    ICMS = cargaCTeOriginal?.CTe?.CSTICMS ?? Dominio.Enumeradores.TipoICMS.ICMS_Normal_00,
                    AliquotaICMS = cargaCTeOriginal?.CTe?.AliquotaICMS ?? 0m,

                    ProdutoPredominante = cargaCTeOriginal?.CTe?.ProdutoPredominante,
                    Expedidor = cargaCTeOriginal != null && cargaCTeOriginal.CTe != null ? Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Expedidor, cargaCTeOriginal.CTe) : null,
                    Recebedor = cargaCTeOriginal != null && cargaCTeOriginal.CTe != null ? Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Recebedor, cargaCTeOriginal.CTe) : null,
                    Tomador = cargaCTeOriginal != null && cargaCTeOriginal.CTe != null ? Servicos.Embarcador.CTe.CTe.ObterDadosParticipanteCTe(Dominio.Enumeradores.TipoTomador.Outros, cargaCTeOriginal.CTe) : null,

                    IncluirICMSFrete = cargaCTeOriginal?.CTe?.IncluirICMSNoFrete,
                    PercentualICMSIncluir = cargaCTeOriginal?.CTe?.PercentualICMSIncluirNoFrete.ToString("n2") ?? "0,00",

                    Seguro = (cargaCTeOriginal != null && cargaCTeOriginal.CTe != null) ? ObterSegurosCTe(cargaCTeOriginal?.CTe) : null,

                    RNTRC = cargaCTeOriginal?.CTe?.Empresa?.RegistroANTT ?? carga?.Empresa?.RegistroANTT ?? ""
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o CT-e de anulação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarRegraICMSCTeManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.ICMS serICMS = new Servicos.Embarcador.Carga.ICMS(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoEmpresa = Request.GetIntParam("Empresa");
                string cnpjEmitente = Utilidades.String.OnlyNumbers(Request.GetStringParam("Emitente"));
                string cnpjDestinatario = Utilidades.String.OnlyNumbers(Request.GetStringParam("Destinatario"));
                string cnpjTomador = Utilidades.String.OnlyNumbers(Request.GetStringParam("Tomador"));
                decimal valorFrete = Request.GetDecimalParam("ValorFrete");

                double.TryParse(cnpjEmitente, out double codigoEmitente);
                double.TryParse(cnpjDestinatario, out double codigoDestinatario);
                double.TryParse(cnpjTomador, out double codigoTomador);

                bool incluirBase = false;
                decimal baseCalculo = valorFrete;
                decimal percentualIncluir = 100m;

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(codigoCarga);

                if (cargaPedido == null || cargaPedido.Carga == null)
                    return new JsonpResult(false, true, "Não foi encontrado nenhuma carga selecionada, favor digite os campos manualmente.");

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                if (empresa == null)
                    return new JsonpResult(false, true, "Empresa emitente não informada.");

                Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(codigoEmitente);
                if (emitente == null || codigoEmitente == 0)
                    return new JsonpResult(false, true, "Emitente não informado.");

                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(codigoDestinatario);
                if (destinatario == null)
                    return new JsonpResult(false, true, "Destinatário não informado.");

                Dominio.Entidades.Cliente tomador = repCliente.BuscarPorCPFCNPJ(codigoTomador);
                if (tomador == null)
                    return new JsonpResult(false, true, "Tomador não informado.");

                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = serICMS.BuscarRegraICMS(cargaPedido.Carga, cargaPedido, empresa, emitente, destinatario, tomador, emitente.Localidade, destinatario.Localidade, ref incluirBase, ref percentualIncluir, baseCalculo, null, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                if (regraICMS != null)
                {
                    var retorno = new
                    {
                        regraICMS.ObservacaoCTe,
                        regraICMS.SimplesNacional,
                        Aliquota = regraICMS.Aliquota.ToString("n2"),
                        AliquotaSimples = regraICMS.AliquotaSimples.ToString("n2"),
                        ValorICMS = regraICMS.ValorICMS.ToString("n2"),
                        ValorBaseCalculoICMS = regraICMS.ValorBaseCalculoICMS.ToString("n2"),
                        PercentualReducaoBC = regraICMS.PercentualReducaoBC.ToString("n2"),
                        PercentualInclusaoBC = regraICMS.PercentualInclusaoBC.ToString("n2"),
                        regraICMS.IncluirICMSBC,
                        regraICMS.DescontarICMSDoValorAReceber,
                        regraICMS.NaoImprimirImpostosDACTE,
                        regraICMS.NaoEnviarImpostoICMSNaEmissaoCte,
                        CST = Servicos.CTe.ObterEnumeradorICMS(regraICMS.CST),
                        CFOP = repCFOP.BuscarPorNumero(regraICMS.CFOP)?.Codigo ?? 0,
                        DescricaoCFOP = regraICMS.CFOP,
                        regraICMS.NaoReduzirRetencaoICMSDoValorDaPrestacao
                    };

                    return new JsonpResult(retorno, true, "Sucesso");
                }
                else
                {
                    return new JsonpResult(null, true, "Regra de ICMS não localizada");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o CT-e de anulação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoGeralIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi repConfiguracaoIntegracaoCTePagamentoLoggi = new Repositorio.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoKMM repConfiguracaoIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repConfiguracaoIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi configuracaoIntegracaoCTePagamentoLoggi = repConfiguracaoIntegracaoCTePagamentoLoggi.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoKMM = repConfiguracaoIntegracaoKMM.Buscar();
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGlobus configuracaoGlobus = repConfiguracaoIntegracaoGlobus.Buscar();

                var retorno = new
                {
                    UsaFluxoSubstituicaoFaseada = configuracaoCargaEmissaoDocumento?.UsaFluxoSubstituicaoFaseada ?? false,
                    IntegrarCTeSubstituto = configuracaoIntegracaoCTePagamentoLoggi?.IntegrarCTeSubstituto ?? false,
                    PossuiConfiguracaoIntegracao = (configuracaoKMM != null) ? true : (configuracaoGlobus != null) ? true : false,
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao buscar as configurações gerais de integração");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string EmitirCTe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Autorizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmCancelamento
                    && cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmInutilizacao)
                {

                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    cte.Status = "P";
                    repCTe.Atualizar(cte);

                    Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                    bool sucesso = svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo);
                    if (!sucesso)
                    {
                        mensagem = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                    }
                }
                else
                {
                    mensagem = "A atual situação do CT-e (" + cte.DescricaoStatus + ") não permite sua emissão.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }

        private object ObterSegurosCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            var retorno = (from obj in cte.Seguros
                           select new
                           {
                               Codigo = obj.Codigo,
                               CodigoResponsavel = obj.Tipo,
                               Responsavel = obj.DescricaoTipo,
                               Seguradora = obj.NomeSeguradora,
                               CNPJSeguradora = obj.CNPJSeguradora,
                               NumeroApolice = obj.NumeroApolice,
                               NumeroAverbacao = obj.NumeroAverbacao,
                               Valor = obj.Valor.ToString("n2")
                           }).ToList();

            return retorno;
        }

        private dynamic ProcessarXMLCTe(dynamic objCTe, Stream xml, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CTe.CTe serCte = new Servicos.Embarcador.CTe.CTe(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Type tipoCTe = objCTe.GetType();
            if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc) ||
                tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cte = serCte.ConverterProcCTeParaCTePorObjeto(objCTe);
                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    return "Não é possí­vel importar um CT-e de complemento.";

                Dominio.Entidades.Empresa empresa = null;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    empresa = repEmpresa.BuscarPorCNPJ(cte.Emitente.CNPJ);

                object conhecimento = repCTe.BuscarPorNumeroESerie(empresa.Codigo, int.Parse(objCTe.CTe.infCte.ide.nCT), int.Parse(objCTe.CTe.infCte.ide.serie), "57", objCTe.protCTe.infProt.tpAmb == MultiSoftware.CTe.v400.ConhecimentoDeTransporte.TAmb.Item1 ? Dominio.Enumeradores.TipoAmbiente.Producao : Dominio.Enumeradores.TipoAmbiente.Homologacao);

                
                if (conhecimento != null && conhecimento is Dominio.Entidades.ConhecimentoDeTransporteEletronico)
                    return conhecimento;

                if (string.IsNullOrWhiteSpace(cte.Xml))
                {
                    xml.Position = 0;
                    StreamReader reader = new StreamReader(xml);
                    cte.Xml = reader.ReadToEnd();
                }



                if (empresa != null)
                {
                    try
                    {
                        unitOfWork.Start();

                        if (tipoCTe == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
                            conhecimento = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)objCTe, xml, unitOfWork, "", "", false, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem);
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
                            conhecimento = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)objCTe, xml, unitOfWork, "", "", false, false, false, TipoServicoMultisoftware, false, ConfiguracaoEmbarcador.AdicionarOutroDocumentoQuandoCTeAnteriorNaoTem);
                        else if (tipoCTe == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
                            conhecimento = svcCTe.GerarCTeAnterior(empresa, (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)objCTe, xml, unitOfWork, "", "", false, false, false, TipoServicoMultisoftware);

                        if (conhecimento is string)
                        {
                            unitOfWork.Rollback();
                            return (string)conhecimento;
                        }
                        else if (conhecimento is Dominio.Entidades.ConhecimentoDeTransporteEletronico)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteConvertido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)conhecimento;
                            cteConvertido.CTeSemCarga = true;

                            repCTe.Atualizar(cteConvertido);

                            unitOfWork.CommitChanges();

                            return cteConvertido;
                        }
                        else
                        {
                            return "Conhecimento de transporte inválido.";
                        }
                    }
                    catch (Exception ex) {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
                else
                {
                    return "O CT-e informado não foi emitido por uma transportadora cadastrada.";
                }

            }
            else
            {
                return "A versão do CT-e não é compatível, por favor, verique com a multisoftware";
            }
        }

        #endregion
    }
}
