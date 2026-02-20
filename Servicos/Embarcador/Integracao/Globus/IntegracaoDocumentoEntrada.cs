using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.DocumentoEntrada;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        public void IntegrarDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoEntradaIntegracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                documentoEntradaIntegracao.DataIntegracao = DateTime.Now;
                documentoEntradaIntegracao.NumeroTentativas++;

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(null, null, documentoEntradaIntegracao.DocumentoEntrada);
                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                var request = ConverterDocumentoEntrada(documentoEntradaIntegracao);

                var retWS = Transmitir(request, "api/LivroISSEntrada/Documento", token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);
                    if (!string.IsNullOrEmpty(retorno?.data?.idExterno))
                        documentoEntradaIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno;
                }

                documentoEntradaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                documentoEntradaIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                documentoEntradaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                documentoEntradaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(documentoEntradaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioDocumentoEntradaIntegracao.Atualizar(documentoEntradaIntegracao);
        }

        public void IntegrarCancelamentoDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoEntradaIntegracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {
                documentoEntradaIntegracao.DataIntegracao = DateTime.Now;
                documentoEntradaIntegracao.NumeroTentativas++;

                this.ValidarEnvioIntegracao(documentoEntradaIntegracao.DocumentoEntrada, TipoAcaoIntegracao.Cancelamento);

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(null, null, documentoEntradaIntegracao.DocumentoEntrada);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                var request = ConverterDocumentoEntrada(documentoEntradaIntegracao, true);

                var retWS = Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    documentoEntradaIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? "0";
                }

                documentoEntradaIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                documentoEntradaIntegracao.ProblemaIntegracao = documentoEntradaIntegracao.CodigoExternoRetornoIntegracao != null ? "Cancelamento integrado com sucesso!" : retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                documentoEntradaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                documentoEntradaIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(documentoEntradaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioDocumentoEntradaIntegracao.Atualizar(documentoEntradaIntegracao);
        }

        private object ConverterDocumentoEntrada(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoEntradaIntegracao, bool cancelamento = false)
        {
            var documentoEntrada = documentoEntradaIntegracao.DocumentoEntrada;
            var modeloDocumento = documentoEntrada.Modelo;
            string xmlString = "";

            if (modeloDocumento.Abreviacao.Equals("CT-e") || modeloDocumento.Abreviacao.Equals("NF-e"))
            {
                DocumentoEntradaEnvio documentoRetorno = new DocumentoEntradaEnvio();

                documentoRetorno.InscricaoEmpresa = documentoEntrada.Destinatario?.CNPJ.ToString() ?? "";
                documentoRetorno.Fase = cancelamento == true ? 2 : 1;
                documentoRetorno.Documento = documentoEntrada.Numero;
                documentoRetorno.Serie = documentoEntrada.Serie.ToInt() > 0 ? documentoEntrada.Serie.ToInt() : 0;
                documentoRetorno.Sistema = _configuracaoIntegracao.SistemaIntegrarComEscritaFiscal;
                documentoRetorno.Usuario = _configuracaoIntegracao.Usuario;
                documentoRetorno.CodigoModelo = documentoEntrada.Modelo.Abreviacao.Contains("CT-e") ? 57 : documentoEntrada.Modelo.Abreviacao.Contains("NF-e") ? 55 : 0;
                documentoRetorno.ConteudoXml = ObterConteudoXML(documentoEntrada);
                documentoRetorno.ChaveDeAcesso = documentoEntrada.Chave ?? "";
                documentoRetorno.DataEmissao = documentoEntrada.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                return documentoRetorno;

            }
            else if (modeloDocumento.Abreviacao.Equals("NFS-e") || modeloDocumento.Abreviacao.Equals("NFS"))
            {
                if (cancelamento)
                {
                    DocumentoCancelamentoEnvio documentoNF = new DocumentoCancelamentoEnvio();

                    documentoNF.CodigoDocumento = string.IsNullOrWhiteSpace(documentoEntradaIntegracao.CodigoExternoRetornoIntegracao) ? "0" : documentoEntradaIntegracao.CodigoExternoRetornoIntegracao;
                    documentoNF.Usuario = _configuracaoIntegracao.Usuario;
                    documentoNF.ExcluirDocumentoFiscal = false;

                    return documentoNF;
                }
                else
                {
                    DocumentoNFEnvio documentoNF = new DocumentoNFEnvio();

                    string abreviacao = modeloDocumento.Abreviacao.Replace("-", "").Replace("_", "");
                    if (abreviacao.Length > 3)
                        abreviacao = abreviacao.Substring(0, 3);

                    documentoNF.InscricaoEmpresa = documentoEntrada.Destinatario.CNPJ_SemFormato.ToString() ?? "";
                    documentoNF.InscricaoParticipante = documentoEntrada.Fornecedor.CPF_CNPJ_SemFormato.ToString() ?? "";
                    documentoNF.NumeroDocumento = (documentoEntrada.Numero.ToString() ?? "").PadLeft(10, '0');
                    documentoNF.Serie = documentoEntrada.Serie.ToString() ?? "";
                    documentoNF.CodigoTipoDocumento = abreviacao;
                    documentoNF.DataEmissao = documentoEntrada.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    documentoNF.DataEntradaSaida = documentoEntrada.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    documentoNF.DataVencimento = documentoEntrada.DataEmissao.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    documentoNF.Observacao = documentoEntrada.Observacao;
                    documentoNF.CodigoIbgeMunicipio = documentoEntrada.Destinatario?.Localidade?.CodigoIBGE ?? 0;
                    documentoNF.Usuario = _configuracaoIntegracao.Usuario;
                    documentoNF.IntegrarContabilidade = _configuracaoIntegracao.IntegrarComContabilidade;
                    documentoNF.IntegrarFinanceiro = _configuracaoIntegracao.IntegrarComContabilidade;
                    documentoNF.ISSRetido = false;
                    documentoNF.ISSBaseCalculo = 0;
                    documentoNF.ISSValor = 0;

                    documentoNF.Itens = new List<Item>();
                    foreach (var itemDocumentoEntrada in documentoEntrada.Itens)
                    {
                        documentoNF.Itens.Add(new Item
                        {
                            CodigoServico = documentoEntrada.Servico?.CodigoIntegracao ?? "",
                            ContaPlanoFinanceiro = documentoEntrada.TipoMovimento?.CodigoIntegracao ?? modeloDocumento.TipoMovimentoEmissao?.CodigoIntegracao ?? "",
                            ContaContabil = 1364, //temporario
                            CustoContabil = 1, //temporario
                            Valor = itemDocumentoEntrada.ValorTotal,
                            ValoresComplementares = new ValorComplementar
                            {
                                Grupo = null
                            }

                        });
                    }

                    return documentoNF;
                }
            }
            else
            {
                throw new Exception($"Não é permitida integração para o modelo {modeloDocumento.Abreviacao} - {modeloDocumento.Descricao} via documento de entrada.");
            }

        }

        private void ValidarEnvioIntegracao(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoIntegracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao repositorioDocumentoEntradaIntegracao = new Repositorio.Embarcador.Financeiro.DocumentoEntradaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaIntegracao documentoEntradaIntegracao = repositorioDocumentoEntradaIntegracao.BuscarIntegracaoPorCodigoDocumentoEntrada(documentoEntrada.Codigo);

            if (tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento)
            {
                if (documentoEntradaIntegracao.CodigoExternoRetornoIntegracao == null)
                    throw new Exception(@"Documento de entrada não foi integrado com sucesso, portanto não é possível cancelamento");
            }

        }

        private string ObterConteudoXML(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, bool cancelamento = false)
        {
            if (documentoEntrada.Modelo.Abreviacao.Contains("CT-e"))
            {
                string xmlCTe = ObterCTeXML(documentoEntrada, cancelamento);

                return xmlCTe ?? "O XML do CT-e não foi encontrado!";
            }
            else if (documentoEntrada.Modelo.Abreviacao.Contains("NF-e"))
            {
                string xmlNFe = ObterNFeXML(documentoEntrada);

                return xmlNFe ?? "O XML da NF-e não foi encontrado!";
            }

            throw new Exception("O tipo de documento não possui suporte para busca de XML!");
        }

        private string ObterCTeXML(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, bool cancelamento = false)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChaveNFe(documentoEntrada.Chave);
            string xmlString = "";

            if (cancelamento)
                xmlString = new Servicos.WebService.CTe.CTe(_unitOfWork).ObterRetornoXML(cte, _unitOfWork);
            else
                xmlString = new Servicos.WebService.CTe.CTe(_unitOfWork).ObterRetornoXMLAutorizacao(cte, _unitOfWork);

            return xmlString;
        }

        private string ObterNFeXML(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada)
        {
            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(_unitOfWork);

            string chave = Utilidades.String.OnlyNumbers(documentoEntrada.Chave);

            Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveETipoDocumento(documentoEntrada.Destinatario.Codigo,
                new TipoDocumentoDestinadoEmpresa[] { TipoDocumentoDestinadoEmpresa.NFeDestinada, TipoDocumentoDestinadoEmpresa.NFeTransporte }, chave);

            string caminho = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

            if (documentoDestinado != null)
                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "NFe", "nfeProc", documentoDestinado.Chave + ".xml");
            else
                return null;


            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
            {
                string conteudoXML = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho);

                return conteudoXML;
            }
            else
            {
                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.ObterDocumentosDestinadosEmpresa(documentoDestinado.Empresa.Codigo, _unitOfWork.StringConexao, null, out string msgErro, out string codigoStatusRetornoSefaz, documentoDestinado.NumeroSequencialUnico);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                {
                    string conteudoXML = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho);

                    return conteudoXML;
                }
                else
                {
                    return null;
                }

            }
        }
    }

}
