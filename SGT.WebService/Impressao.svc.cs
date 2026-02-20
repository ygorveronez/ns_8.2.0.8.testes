using CoreWCF;
using System.Text;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Impressao(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IImpressao
    {
        #region Métodos Globais

        public Retorno<string> EnviarArquivoXMLNFe(Stream arquivo)
        {
            ValidarToken();
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

                Retorno<string> retorno = new Retorno<string>();
                string nomeArquivo = Guid.NewGuid().ToString();

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao, string.Concat(nomeArquivo, ".xml"));

                using (StreamReader reader = new StreamReader(arquivo))
                    Utilidades.IO.FileStorageService.Storage.WriteAllText(caminho, RemoveTroublesomeCharacters(reader.ReadToEnd()), Encoding.UTF8);

                arquivo.Close();
                arquivo.Dispose();

                retorno.Status = true;
                retorno.DataRetorno = DateTime.Now.Date.ToString("dd/MM/yyyy HH:mm:ss");
                retorno.Objeto = nomeArquivo;

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<string>() { Mensagem = "Ocorreu uma falha ao salvar o arquivo.", Status = false, CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica };
            }
        }

        public Retorno<bool> SolicitarImpressaoNFe(string tokenXML, int? protocoloCarga, int? protocoloPedido)
        {
            ValidarToken();

            protocoloCarga ??= 0;
            protocoloPedido ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<bool> retorno = new Retorno<bool>();
            List<string> caminhosXMLTemp = new List<string>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe repCargaImpressaoNFe = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoNFe(unitOfWork);

                if (protocoloCarga >= 0 && !string.IsNullOrWhiteSpace(tokenXML))
                {
                    //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocoloCarga, protocoloPedido);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido((int)protocoloCarga, (int)protocoloPedido);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

                    if (cargaPedido != null)
                    {

                        string path = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracao;
                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(path, string.Concat(tokenXML, ".xml"));

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        {
                            using (System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminho)))
                            {
                                Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

                                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.DadosNFe nfXml = null;

                                try
                                {
                                    nfXml = svcNFe.ObterDocumentoPorXML(reader.BaseStream, unitOfWork);
                                }
                                catch (Exception)
                                {
                                    retorno.Mensagem = "O xml enviado não é de uma nota fiscal autorizada.";
                                    retorno.Status = false;
                                    Servicos.Log.TratarErro("O xml enviado não é de uma nota fiscal autorizada, avulsa.");
                                }

                                if (nfXml != null)
                                {
                                    reader.BaseStream.Position = 0;

                                    Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe cargaImpressaoNFe = new Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoNFe
                                    {
                                        CargaPedido = cargaPedido,
                                        SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Solicitado,
                                        XML = reader.ReadToEnd(),
                                        Chave = nfXml.Chave,
                                        Numero = int.Parse(nfXml.Numero),
                                        Serie = nfXml.Serie
                                    };

                                    repCargaImpressaoNFe.Inserir(cargaImpressaoNFe);

                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, "Solicitou impressão de NF-e", unitOfWork);

                                    retorno.Status = true;
                                }
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem += "O Token informado não existe mais físicamente no servidor, por favor, envie o XML da nota novamente e receba um novo token";
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Carga/Pedido não encontrada";
                    }
                }
                else
                {
                    if (protocoloCarga == 0)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Protocolo da carga Inválido.";
                    }

                    if (string.IsNullOrWhiteSpace(tokenXML))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Token do XML inválido.";
                    }
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar vincular xml com a carga";
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SolicitarImpressaoBoleto(Dominio.ObjetosDeValor.WebService.Impressao.Boleto boleto, int? protocoloCarga, int? protocoloPedido)
        {
            Servicos.Log.TratarErro("SolicitarImpressaoBoleto: " + (boleto != null ? Newtonsoft.Json.JsonConvert.SerializeObject(boleto) : string.Empty), "Request");

            ValidarToken();

            protocoloCarga ??= 0;
            protocoloPedido ??= 0;

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (boleto != null && protocoloCarga > 0) //&& protocoloPedido > 0) Adicionar validação do pedido após entrar em produção
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto repCargaImpressaoBoleto = new Repositorio.Embarcador.Cargas.Impressao.CargaImpressaoBoleto(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                    if (protocoloPedido > 0)
                    {
                        //cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocoloCarga, protocoloPedido);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido((int)protocoloCarga, (int)protocoloPedido);
                        cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;
                    }
                    else
                    {
                        //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorCarga(protocoloCarga);
                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidos = repCargaPedido.BuscarPorProtocoloCarga((int)protocoloCarga);
                        if (cargasPedidos != null && cargasPedidos.Count > 0)
                            cargaPedido = cargasPedidos.FirstOrDefault();
                    }


                    if (cargaPedido == null)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Carga/Pedido não encontrado";
                        retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        return retorno;
                    }

                    Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto cargaImpressaoBoleto = new Dominio.Entidades.Embarcador.Cargas.Impressao.CargaImpressaoBoleto();
                    cargaImpressaoBoleto.SituacaoImpressao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao.Pendente;
                    cargaImpressaoBoleto.CargaPedido = cargaPedido;

                    cargaImpressaoBoleto.Aceite = boleto.Aceite;
                    cargaImpressaoBoleto.Agencia = boleto.Agencia;
                    cargaImpressaoBoleto.Carteira = boleto.Carteira;
                    cargaImpressaoBoleto.CedenteBairro = boleto.CedenteBairro;
                    cargaImpressaoBoleto.CedenteCEP = boleto.CedenteCEP;
                    cargaImpressaoBoleto.CedenteCidade = boleto.CedenteCidade;
                    cargaImpressaoBoleto.CedenteCNPJ = boleto.CedenteCNPJ;
                    cargaImpressaoBoleto.CedenteCodigo = boleto.CedendeCodigo;
                    cargaImpressaoBoleto.CedenteComplemento = boleto.CedenteComplemento;
                    cargaImpressaoBoleto.CedenteEstado = boleto.CedenteEstado;
                    cargaImpressaoBoleto.CedenteIE = boleto.CedenteIE;
                    cargaImpressaoBoleto.CedenteNome = boleto.CedenteNome;
                    cargaImpressaoBoleto.CedenteNumero = boleto.CedenteNumero;
                    cargaImpressaoBoleto.CedenteRua = boleto.CedenteRua;
                    cargaImpressaoBoleto.CodigoBanco = boleto.CodigoBanco;
                    cargaImpressaoBoleto.CodigoBarras = boleto.CodigoBarras;
                    cargaImpressaoBoleto.DataDocumento = boleto.DataDocumento;
                    cargaImpressaoBoleto.DataProcessamento = boleto.DataProcessamento;
                    cargaImpressaoBoleto.DataVencimento = boleto.DataVencimento;
                    cargaImpressaoBoleto.EspecieDocumento = boleto.EspecieDocumento;
                    cargaImpressaoBoleto.EspecieMoeda = boleto.EspecieMoeda;
                    cargaImpressaoBoleto.Instrucoes = boleto.Instrucoes;
                    cargaImpressaoBoleto.InstrucoesAdicional = boleto.InstucoesAdicionais;
                    cargaImpressaoBoleto.LinhaDigitavel = boleto.LinhaDigitavel;
                    cargaImpressaoBoleto.LocalPagamento = boleto.LocalPagamento;
                    cargaImpressaoBoleto.NomeBanco = boleto.NomeBanco;
                    cargaImpressaoBoleto.NossoNumero = boleto.NossoNumero;
                    cargaImpressaoBoleto.NumeroDocumento = boleto.NumeroDocumento;
                    cargaImpressaoBoleto.Quantidade = boleto.Quantidade;
                    cargaImpressaoBoleto.SacadoBairro = boleto.SacadoBairro;
                    cargaImpressaoBoleto.SacadoCEP = boleto.SacadoCEP;
                    cargaImpressaoBoleto.SacadoCidade = boleto.SacadoCidade;
                    cargaImpressaoBoleto.SacadoCNPJ = boleto.SacadoCNPJ;
                    cargaImpressaoBoleto.SacadoComplemento = boleto.SacadoComplemento;
                    cargaImpressaoBoleto.SacadoEstado = boleto.SacadoEstado;
                    cargaImpressaoBoleto.SacadoIE = boleto.SacadoIE;
                    cargaImpressaoBoleto.SacadoNome = boleto.SacadoNome;
                    cargaImpressaoBoleto.SacadoNumero = boleto.SacadoNumero;
                    cargaImpressaoBoleto.SacadoRua = boleto.SacadoRua;
                    cargaImpressaoBoleto.UsoBanco = boleto.UsoBanco;
                    cargaImpressaoBoleto.ValorCobrado = boleto.ValorCobrado;
                    cargaImpressaoBoleto.ValorDescontoAcrescimo = boleto.ValorDescontoAcrescimo;
                    cargaImpressaoBoleto.ValorDocumento = boleto.ValorDocumeno;
                    cargaImpressaoBoleto.DigitoBanco = boleto.DigitoBanco;
                    cargaImpressaoBoleto.CIP = boleto.CIP;

                    repCargaImpressaoBoleto.Inserir(cargaImpressaoBoleto);

                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido, "Solicitou impressão de boleto", unitOfWork);
                }
                else
                {
                    if (protocoloCarga == 0)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Protocolo da carga Inválido.";
                    }

                    if (protocoloPedido == 0)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Protocolo do pedido Inválido.";
                    }

                    if (boleto == null)
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem += "Dados do boleto Inválidos.";
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao solicitar impressão de boleto";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        #endregion

        #region Métodos Privados

        private static string RemoveTroublesomeCharacters(string inString)
        {
            if (inString == null) return null;

            StringBuilder newString = new StringBuilder();
            char ch;

            for (int i = 0; i < inString.Length; i++)
            {

                ch = inString[i];
                // remove any characters outside the valid UTF-8 range as well as all control characters
                // except tabs and new lines
                if ((ch < 0x00FD && ch > 0x001F) || ch == '\t' || ch == '\n' || ch == '\r')
                {
                    newString.Append(ch);
                }
            }
            return newString.ToString();

        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceImpressao;
        }

        #endregion
    }
}
