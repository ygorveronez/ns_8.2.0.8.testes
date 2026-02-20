using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Servicos
{
    public class Avon
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public Avon(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void EnviarRetornoAvon(List<Dominio.Entidades.DocumentoManifestoAvon> documentos, Dominio.Entidades.Empresa empresa)
        {

            Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(_unitOfWork);

            //Servicos.Log.TratarErro("Carregando XMLs CTes para retorno Avon.");
            List<Dominio.Entidades.XMLCTe> xmls = repXMLCTe.BuscarPorCTe((from obj in documentos select obj.CTe.Codigo).ToArray(), Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

            //Servicos.Log.TratarErro("Enviando CTes para retorno Avon.");
            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = EnviarRetornoCTe(empresa.CNPJ, empresa.Configuracao.TokenIntegracaoAvon, xmls);
            Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(_unitOfWork);
            
            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200" && retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Document[0].Response != null)
            {
                for (int i = 0; i < retorno.CrossTalk_Body.Document.Length; i++)
                {
                    Dominio.ObjetosDeValor.CrossTalk.Document documentoCte = retorno.CrossTalk_Body.Document[i];

                    Dominio.Entidades.DocumentoManifestoAvon documentoAvon = (from obj in documentos where obj.CTe.Chave == documentoCte.AccessKey select obj).FirstOrDefault();
                    if (documentoCte.Response.InnerCode == "100")
                    {
                        documentoAvon.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.Finalizado;
                        documentoAvon.ProblemaIntegracao = "";
                    }
                    else
                    {
                        documentoAvon.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno;
                        documentoAvon.ProblemaIntegracao = documentoCte.Response.InnerCode + " - " + documentoCte.Response.Description;
                    }
                    documentoAvon.NumeroTentativas++;
                    repDocumento.Atualizar(documentoAvon);
                }
            }
            else
            {
                foreach (Dominio.Entidades.DocumentoManifestoAvon documento in documentos)
                {
                    documento.Status = Dominio.Enumeradores.StatusDocumentoManifestoAvon.FalhaNoRetorno;
                    documento.ProblemaIntegracao = retorno.CrossTalk_Header.ResponseCode + " - " + retorno.CrossTalk_Header.ResponseCodeMessage;
                    documento.NumeroTentativas++;
                    repDocumento.Atualizar(documento);
                }
            }
        }

        public List<Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica> ConsultarNFesParaEmissao(int codigoEmpresa, string numeroManifesto, string groupId)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || empresa.Configuracao == null || !empresa.Configuracao.UtilizaIntegracaoAvon || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoAvon))
                return null;

            string enterpriseId = empresa.CNPJ,
                   token = empresa.Configuracao.TokenIntegracaoAvon;

#if DEBUG
            enterpriseId = "82809088000666";
            token = "A2ED7133603A40A4BC6C15E909837604";
#endif

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = this.RequisitarInformacoesDoManifesto(ref svcAvon, enterpriseId, groupId, token, numeroManifesto);

            //Pega XML De Envio
            var xmlConsulta1 = inspector.LastRequestXML;
            Servicos.Log.TratarErro("AVON Retorno consulta1 minuta: " + xmlConsulta1);

            //Pega XML De Retorno
            var xmlRetorno1 = inspector.LastResponseXML;
            Servicos.Log.TratarErro("AVON Retorno retorno1 minuta: " + xmlRetorno1);

            if (retorno != null && retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "202")
            {
                retorno = this.ObterInformacoesDoManifesto(ref svcAvon, enterpriseId, token, retorno.CrossTalk_Header.GUID);

                //Pega XML De Envio
                var xmlConsulta2 = inspector.LastRequestXML;
                Servicos.Log.TratarErro("AVON Retorno consulta2 minuta: " + xmlConsulta2);

                //Pega XML De Retorno
                var xmlRetorno2 = inspector.LastResponseXML;
                Servicos.Log.TratarErro("AVON Retorno retorno2 minuta: " + xmlRetorno2);

                if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200" && retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Response != null && retorno.CrossTalk_Body.Response.Identification != null)
                {
                    retorno = this.ObterNotasFiscaisDoManifesto(ref svcAvon, enterpriseId, token, (from obj in retorno.CrossTalk_Body.Response.Identification where obj.Name.Equals("docNumber") || obj.Name.Equals("issueCode") select obj).ToArray());

                    //Pega XML De Envio
                    var xmlConsulta3 = inspector.LastRequestXML;
                    Servicos.Log.TratarErro("AVON Retorno consulta3 minuta: " + xmlConsulta3);

                    //Pega XML De Retorno
                    var xmlRetorno3 = inspector.LastResponseXML;
                    Servicos.Log.TratarErro("AVON Retorno retorno3 minuta: " + xmlRetorno3);

                    if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200" && retorno.CrossTalk_Body != null && retorno.CrossTalk_Body.Documents != null)
                    {
                        var notasFiscais = (from obj in retorno.CrossTalk_Body.Documents select new Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica(obj)).ToList();

                        return notasFiscais;
                    }
                }
            }

            return null;
        }

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico GerarCTePorNFe(int codigoEmpresa, Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica nfe, Dominio.Entidades.DocumentoManifestoAvon documento)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            Dominio.ObjetosDeValor.CTe.CTe cte = new Dominio.ObjetosDeValor.CTe.CTe();

            cte.CodigoIBGECidadeInicioPrestacao = nfe.Emitente.CodigoMunicipio;
            cte.CodigoIBGECidadeTerminoPrestacao = nfe.Destinatario.CodigoMunicipio;

            cte.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            cte.Destinatario = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = nfe.Destinatario.Bairro,
                CEP = nfe.Destinatario.CEP,
                Cidade = nfe.Destinatario.DescricaoMunicipio,
                CodigoAtividade = Atividade.ObterAtividade(empresa.Codigo, Utilidades.String.OnlyNumbers(nfe.Destinatario.CPFCNPJ).Length == 14 ? "J" : "F", _unitOfWork.StringConexao, 0, _unitOfWork).Codigo,
                CodigoIBGECidade = nfe.Destinatario.CodigoMunicipio,
                CodigoPais = nfe.Destinatario.CodigoPais.ToString(),
                Complemento = nfe.Destinatario.Complemento,
                CPFCNPJ = nfe.Destinatario.CPFCNPJ,
                Endereco = nfe.Destinatario.Logradouro,
                Exportacao = false,
                NomeFantasia = nfe.Destinatario.Nome,
                RazaoSocial = nfe.Destinatario.Nome,
                RGIE = nfe.Destinatario.IE,
                Numero = nfe.Destinatario.Numero.Length > 2 ? nfe.Destinatario.Numero : "S/N",
                Telefone1 = nfe.Destinatario.Fone
            };

            cte.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>()
            {
                new Dominio.ObjetosDeValor.CTe.Documento() {
                    ChaveNFE = nfe.Chave,
                    DataEmissao = nfe.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                    ModeloDocumentoFiscal = "55",
                    Peso = nfe.Peso,
                    Numero = nfe.Numero.ToString(),
                    Serie = nfe.Serie.ToString(),
                    Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe,
                    Volume = nfe.Quantidade,
                    Valor = nfe.Valor > 0m ? nfe.Valor: 0.01m
                }
            };

            cte.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
            {
                CNPJ = empresa.CNPJ,
                Atualizar = false
            };

            cte.ProdutoPredominante = empresa.Configuracao.ProdutoPredominante;
            cte.OutrasCaracteristicasDaCarga = empresa.Configuracao.OutrasCaracteristicas;

            cte.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>()
            {
                new Dominio.ObjetosDeValor.CTe.QuantidadeCarga(){
                        Descricao = "Kilograma",
                        Quantidade = nfe.Peso,
                        UnidadeMedida = "01"
                }
            };

            cte.Remetente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = nfe.Emitente.Bairro,
                CEP = nfe.Emitente.CEP,
                Cidade = nfe.Emitente.DescricaoMunicipio,
                CodigoAtividade = Atividade.ObterAtividade(empresa.Codigo, Utilidades.String.OnlyNumbers(nfe.Emitente.CPFCNPJ).Length == 14 ? "J" : "F", _unitOfWork.StringConexao, 0, _unitOfWork).Codigo,
                CodigoIBGECidade = nfe.Emitente.CodigoMunicipio,
                CodigoPais = nfe.Emitente.CodigoPais.ToString(),
                Complemento = nfe.Emitente.Complemento,
                CPFCNPJ = nfe.Emitente.CPFCNPJ,
                Endereco = nfe.Emitente.Logradouro,
                Exportacao = false,
                NomeFantasia = nfe.Emitente.Nome,
                RazaoSocial = nfe.Emitente.Nome,
                Numero = nfe.Emitente.Numero.Length > 2 ? nfe.Emitente.Numero : "S/N",
                RGIE = nfe.Emitente.IE,
                Telefone1 = nfe.Emitente.Fone
            };

            cte.Seguros = new List<Dominio.ObjetosDeValor.CTe.Seguro>()
            {
                new Dominio.ObjetosDeValor.CTe.Seguro(){
                        NomeSeguradora = "",
                        NumeroApolice = "",
                        NumeroAverbacao = "",
                        Tipo = Dominio.Enumeradores.TipoSeguro.Remetente,
                        Valor = 0m
                }
            };

            if (documento.ValorPedagio > 0)
            {
                cte.ComponentesDaPrestacao = new List<Dominio.ObjetosDeValor.CTe.ComponentePrestacao>(){
                    new Dominio.ObjetosDeValor.CTe.ComponentePrestacao()
                    {
                         Descricao = "Pedágio",
                         IncluiBaseCalculoICMS = true,
                         IncluiValorAReceber = true,
                         Valor = documento.ValorPedagio
                    }
                };
            }

            if (documento.Manifesto.Veiculo != null)
            {
                cte.Veiculos = new List<Dominio.ObjetosDeValor.CTe.Veiculo>()
                {
                     new Dominio.ObjetosDeValor.CTe.Veiculo(){
                          Placa = documento.Manifesto.Veiculo.Placa
                     }
                };
            }

            if (documento.Manifesto.Motorista != null)
            {
                cte.Motoristas = new List<Dominio.ObjetosDeValor.CTe.Motorista>()
                {
                    new Dominio.ObjetosDeValor.CTe.Motorista(){
                         CPF = documento.Manifesto.Motorista.CPF,
                         Nome = documento.Manifesto.Motorista.Nome
                    }
                };
            }

            if (cte.Motoristas != null && cte.Motoristas.Count > 0 && cte.Veiculos != null && cte.Veiculos.Count > 0)
                cte.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;


            cte.PercentualICMSIncluirNoFrete = 100;
            cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
            cte.TipoCTe = Dominio.Enumeradores.TipoCTE.Normal;
            cte.TipoImpressao = empresa.Configuracao.TipoImpressao;
            cte.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            cte.TipoServico = Dominio.Enumeradores.TipoServico.Normal;
            cte.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            cte.ValorAReceber = documento.ValorFrete;
            cte.ValorFrete = documento.ValorFrete;
            cte.ValorTotalMercadoria = nfe.Valor > 0m ? nfe.Valor : 0.01m;
            cte.ValorTotalPrestacaoServico = documento.ValorFrete;
            cte.ObservacoesGerais = "Manifesto Nº " + documento.Manifesto.Numero;

            Servicos.CTe svcCTe = new Servicos.CTe(_unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteIntegrado = svcCTe.GerarCTePorObjeto(cte, 0, _unitOfWork, "1", 1);

            return cteIntegrado;
        }

        public bool FinalizarNotaFiscal(string enterpriseId, string token, Dominio.ObjetosDeValor.CrossTalk.NotaFiscalEletronica nfe)
        {
            try
            {
#if DEBUG
                enterpriseId = "82809088000666";
                token = "A2ED7133603A40A4BC6C15E909837604";
#endif

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
                {
                    CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                    {
                        ProcessCode = "10004",
                        MessageType = "101",
                        ExchangePattern = "1",
                        EnterpriseId = enterpriseId,
                        Token = token,
                    },
                    CrossTalk_Body = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Body()
                    {
                        GroupId = "NFeSearchDocument",
                        Identification = new Dominio.ObjetosDeValor.CrossTalk.Field[] {
                            new Dominio.ObjetosDeValor.CrossTalk.Field(){
                                 Name = "accessKey",
                                 Value = nfe.Chave
                            }
                        }
                    }
                };

                string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

                ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request);

                string response = svcAvon.Send(request, "");

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

                if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200")
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        public bool FinalizarManifesto(string enterpriseId, string token, string numeroManifesto)
        {
            try
            {
#if DEBUG
                enterpriseId = "82809088000666";
                token = "A2ED7133603A40A4BC6C15E909837604";
#endif

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
                {
                    CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                    {
                        ProcessCode = "10004",
                        MessageType = "101",
                        ExchangePattern = "1",
                        EnterpriseId = enterpriseId,
                        Token = token
                    },
                    CrossTalk_Body = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Body()
                    {
                        GroupId = "ManifestSearchDocument",
                        Identification = new Dominio.ObjetosDeValor.CrossTalk.Field[] {
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Name = "docNumber", Value = numeroManifesto }
                        }
                    }
                };

                string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

                ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request);

                string response = svcAvon.Send(request, "");

                Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

                if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "200")
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        public Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message EnviarRetornoCTe(string enterpriseId, string token, List<Dominio.Entidades.XMLCTe> xmls)
        {
            string salvarLogRetornoAvon = "SIM";
            string tentativasRetornoAvon = "10";

#if DEBUG
            enterpriseId = "82809088000666";
            token = "A2ED7133603A40A4BC6C15E909837604";
#endif

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10005",
                    MessageType = "100",
                    ExchangePattern = "7",
                    EnterpriseId = enterpriseId,
                    Token = token,
                    ContentEncoding = "utf-8",
                    ContentType = "application/zip"
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request);

            if (!string.IsNullOrWhiteSpace(salvarLogRetornoAvon))
            {
                byte[] zip = CriarZip(xmls);

                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Servicos.FS.GetPath("C:\\LotesEnviadosAvon\\" + Guid.NewGuid().ToString() + ".zip"), zip);
            }

            string response = svcAvon.CompressedSend(request, CriarZip(xmls));

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "202")
            {
                mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
                {
                    CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                    {
                        ProcessCode = "10005",
                        MessageType = "100",
                        ExchangePattern = "8",
                        EnterpriseId = enterpriseId,
                        Token = token,
                        GUID = retorno.CrossTalk_Header.GUID
                    }
                };

                request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

                int i = 0;

                do
                {
                    i++;

                    response = svcAvon.CompressedSend(request, null);

                    if (!string.IsNullOrWhiteSpace(salvarLogRetornoAvon))
                        Servicos.Log.TratarErro("Retorno " + i.ToString() + ": " + response);

                    //Servicos.Log.TratarErro("Consultando tentativa " + i.ToString());
                    retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

                    if (retorno.CrossTalk_Header.ResponseCode != "202")
                    {
                        //Servicos.Log.TratarErro("Processou envio.");
                        return retorno;
                    }

                    //Servicos.Log.TratarErro("Envio em processamento na avon, aguardando proxima tentativa "+i.ToString());
                    System.Threading.Thread.Sleep(10000);

                } while (i <= int.Parse(tentativasRetornoAvon));
            }

            //Servicos.Log.TratarErro("Não processou envio.");
            return retorno;
        }

        public MemoryStream ObterArquivosRequisicaoFatura(Dominio.Entidades.FaturaAvon fatura, string enterpriseId, string token)
        {
            Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(_unitOfWork);

            Dominio.ObjetosDeValor.CrossTalk.Fatura arquivoFatura = new Dominio.ObjetosDeValor.CrossTalk.Fatura();

            arquivoFatura.CNPJAvon = repDocumento.ObterCNPJRemetente(fatura.Manifestos.First().Codigo);
            arquivoFatura.CNPJTransportador = fatura.Empresa.CNPJ;
            arquivoFatura.DataEmissao = fatura.DataEmissao;
            arquivoFatura.DataVencimento = fatura.DataVencimento;
            arquivoFatura.NumeroFatura = fatura.Numero.ToString();
            arquivoFatura.SerieFatura = fatura.Serie.ToString();
            arquivoFatura.ValorTotal = fatura.ValorTotal;
            arquivoFatura.CTes = repDocumento.ObterChaveDosCTes(fatura.Manifestos.Select(o => o.Codigo).ToArray());

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10006",
                    MessageType = "100",
                    ExchangePattern = "7",
                    EnterpriseId = enterpriseId,
                    Token = token
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);
            string arqFatura = arquivoFatura.ObterArquivo();

            byte[] arquivoRequest = System.Text.Encoding.Default.GetBytes(request);
            byte[] arquivoFaturas = System.Text.Encoding.Default.GetBytes(arqFatura);

            MemoryStream fZip = new MemoryStream();
            ZipOutputStream zipOStream = new ZipOutputStream(fZip);
            zipOStream.SetLevel(9);

            ZipEntry entry = new ZipEntry(string.Concat("Requisicao.xml"));
            entry.DateTime = DateTime.Now;
            zipOStream.PutNextEntry(entry);
            zipOStream.Write(arquivoRequest, 0, arquivoRequest.Length);
            zipOStream.CloseEntry();

            ZipEntry entryFat = new ZipEntry(string.Concat("Faturas.txt"));
            entryFat.DateTime = DateTime.Now;
            zipOStream.PutNextEntry(entryFat);
            zipOStream.Write(arquivoFaturas, 0, arquivoFaturas.Length);
            zipOStream.CloseEntry();

            zipOStream.IsStreamOwner = false;
            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message EnviarFatura(string enterpriseId, string token, Dominio.Entidades.FaturaAvon fatura)
        {
#if DEBUG
            enterpriseId = "82809088000666";
            token = "A2ED7133603A40A4BC6C15E909837604";
#endif

            Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(_unitOfWork);

            Dominio.ObjetosDeValor.CrossTalk.Fatura arquivoFatura = new Dominio.ObjetosDeValor.CrossTalk.Fatura();

            arquivoFatura.CNPJAvon = repDocumento.ObterCNPJRemetente(fatura.Manifestos.First().Codigo);
            arquivoFatura.CNPJTransportador = fatura.Empresa.CNPJ;
            arquivoFatura.DataEmissao = fatura.DataEmissao;
            arquivoFatura.DataVencimento = fatura.DataVencimento;
            arquivoFatura.NumeroFatura = fatura.Numero.ToString();
            arquivoFatura.SerieFatura = fatura.Serie.ToString();
            arquivoFatura.ValorTotal = fatura.ValorTotal;
            arquivoFatura.CTes = repDocumento.ObterChaveDosCTes(fatura.Manifestos.Select(o => o.Codigo).ToArray());

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10006",
                    MessageType = "100",
                    ExchangePattern = "7",
                    EnterpriseId = enterpriseId,
                    Token = token
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            //Servicos.Log.TratarErro("Request 1 - fatura " + fatura.Numero.ToString() + ": " + request);

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request);

            string response = svcAvon.Send(request, arquivoFatura.ObterArquivo());

            //Servicos.Log.TratarErro("Response 1 - fatura " + fatura.Numero.ToString() + ": " + response + " Arquivo Fatura: " + arquivoFatura.ObterArquivo());

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            if (retorno.CrossTalk_Header != null && retorno.CrossTalk_Header.ResponseCode == "202")
            {
                int countConsultas = 0;

                while (retorno.CrossTalk_Header.ResponseCode != "200" && countConsultas < 20)
                {
                    System.Threading.Thread.Sleep(2000);

                    countConsultas++;

                    mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
                    {
                        CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                        {
                            ProcessCode = "10006",
                            MessageType = "100",
                            ExchangePattern = "8",
                            EnterpriseId = enterpriseId,
                            Token = token,
                            GUID = retorno.CrossTalk_Header.GUID
                        }
                    };

                    request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

                    response = svcAvon.Send(request, "");

                    //Servicos.Log.TratarErro("Response 2 (pode haver vários) - fatura " + fatura.Numero.ToString() + ": " + response);

                    retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);
                }
            }

            return retorno;
        }

        public Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message ConsultarFatura(string enterpriseId, string token, Dominio.Entidades.FaturaAvon fatura)
        {
#if DEBUG
            enterpriseId = "82809088000666";
            token = "A2ED7133603A40A4BC6C15E909837604";
#endif

            Repositorio.DocumentoManifestoAvon repDocumento = new Repositorio.DocumentoManifestoAvon(_unitOfWork);

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10006",
                    MessageType = "103",
                    ExchangePattern = "1",
                    EnterpriseId = enterpriseId,
                    Token = token
                },
                CrossTalk_Body = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Body()
                {
                    Invoices = new Dominio.ObjetosDeValor.CrossTalk.Invoice[] {
                        new Dominio.ObjetosDeValor.CrossTalk.Invoice() {
                            Number = fatura.Numero.ToString(),
                            IssueDate = fatura.DataEmissao.ToString("yyyy-MM-dd"),
                            Series = fatura.Serie.ToString(),
                            TakerCnpj = repDocumento.ObterCNPJRemetente(fatura.Manifestos.First().Codigo)
                        }
                    }
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            ServicoAvon.RequestSoapClient svcAvon = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoAvon.RequestSoapClient, ServicoAvon.RequestSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Avon_Request);

            string response = svcAvon.Send(request, "");

            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message retorno = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);

            return retorno;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message RequisitarInformacoesDoManifesto(ref ServicoAvon.RequestSoapClient svcAvon, string enterpriseId, string groupId, string token, string numeroManifesto)
        {
            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10003",
                    MessageType = "103",
                    ExchangePattern = "7",
                    SourceId = "1",
                    EnterpriseId = enterpriseId,
                    Token = token
                },
                CrossTalk_Body = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Body()
                {
                    GroupId = groupId, //GroupId = "OpenedManifestSearchIdPrimary",
                    StandardFilter = new Dominio.ObjetosDeValor.CrossTalk.StandardFilter()
                    {
                        FilterName = groupId + "Filter", //"OpenedManifestSearchIdPrimaryFilter",
                        Fields = new Dominio.ObjetosDeValor.CrossTalk.Field[] {
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Connection= "0", Name = "nddManifestId", Value = "" },
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Connection= "0", Name = "docNumber", Value = numeroManifesto },
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Connection= "0", Name = "processId", Value = "" },
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Connection= "0", Name = "issueCode", Value = "" },
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Connection= "0", Name = "docDate", Value = "" },
                            new Dominio.ObjetosDeValor.CrossTalk.Field (){ Connection= "0", Name = "createDate", Value = "" }
                        }
                    }
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.Send(request, "");

            return Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);
        }

        private Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message ObterInformacoesDoManifesto(ref ServicoAvon.RequestSoapClient svcAvon, string enterpriseId, string token, string guid)
        {
            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10003",
                    MessageType = "103",
                    ExchangePattern = "8",
                    EnterpriseId = enterpriseId,
                    Token = token,
                    GUID = guid
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.Send(request, "");

            return Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);
        }

        private Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message ObterNotasFiscaisDoManifesto(ref ServicoAvon.RequestSoapClient svcAvon, string enterpriseId, string token, Dominio.ObjetosDeValor.CrossTalk.Field[] identification)
        {
            Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message mensagem = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message()
            {
                CrossTalk_Header = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Header()
                {
                    ProcessCode = "10004",
                    MessageType = "103",
                    ExchangePattern = "1",
                    EnterpriseId = enterpriseId,
                    Token = token,
                },
                CrossTalk_Body = new Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Body()
                {
                    GroupId = "ManifestSearchDocument",
                    Identification = identification
                }
            };

            string request = Servicos.XML.ConvertObjectToXMLString<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(mensagem);

            string response = svcAvon.Send(request, "");

            return Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.CrossTalk.CrossTalk_Message>(response);
        }

        private byte[] CriarZip(List<Dominio.Entidades.XMLCTe> xmls)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (Dominio.Entidades.XMLCTe xml in xmls)
                    {
                        var file = archive.CreateEntry(xml.CTe.Chave + ".xml");

                        using (var entryStream = file.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(xml.XML);
                        }
                    }
                }

                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream.ToArray();
            }
        }

        #endregion Métodos Privados
    }
}
