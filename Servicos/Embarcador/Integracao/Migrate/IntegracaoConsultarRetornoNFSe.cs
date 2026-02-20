using Dominio.Entidades.Embarcador.BI;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Migrate
{
    public partial class IntegracaoMigrate
    {

        #region Métodos Públicos

        public bool ConsultarRetornoNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);

            try
            {
                if (cte.Status != "E")
                    return true;

                if (string.IsNullOrEmpty(cte.Empresa?.Configuracao?.TokenMigrate))
                    throw new ServicoException(@"Processo Abortado: Token não informado na configuração da empresa");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

                if (string.IsNullOrEmpty(configuracaoAmbiente.ChaveParceiroMigrate))
                    throw new ServicoException(@"Processo Abortado: Chave do Parceiro não configurado.");

                // Obter Request
                Servicos.ServicosMigrate.InvoiCy envio = this.obterConsultarRetornoNFSe(cte, true, true, configuracaoAmbiente.ChaveParceiroMigrate);

                // Transmitir
                string urlWebService = this.ObterUrlWebService(cte.TipoAmbiente);
                ServicosMigrate.recepcaoSoapPortClient servicoMigrate = ObterClientIntegrarNFSe(urlWebService);
                servicoMigrate.Endpoint.EndpointBehaviors.Add(inspector);
                Servicos.ServicosMigrate.ExecuteResponse retConsultarRetorno = servicoMigrate.ExecuteAsync(envio).Result;

                string xmlEnvioMigrate = inspector.LastRequestXML;
                string xmlRetornoEnvioMigrate = inspector.LastResponseXML;

                // Salvar XML Retorno Migrate
                this.SalvarArquivoXML(cte, false, null, xmlEnvioMigrate, Dominio.Enumeradores.TipoXMLCTe.EnvioConsultaIntegracao, cte.Status, unitOfWork);
                this.SalvarArquivoXML(cte, false, null, xmlRetornoEnvioMigrate, Dominio.Enumeradores.TipoXMLCTe.RetornoConsultaIntegracao, cte.Status, unitOfWork);

                // Processar Response
                this.processarRetornoConsultarNFSe(cte, retConsultarRetorno, unitOfWork);
                repCTe.Atualizar(cte);
            }
            catch (ServicoException excecao)
            {
                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }

                cte = repCTe.BuscarPorCodigo(cte.Codigo);
                cte.Status = "R";
                cte.MensagemRetornoSefaz = message;
                repCTe.Atualizar(cte);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                cte = repCTe.BuscarPorCodigo(cte.Codigo);
                cte.MensagemRetornoSefaz = "Processo Abortado: Ocorreu um erro ao integrar NFSe com a migrate";
                repCTe.Atualizar(cte);
            }

            return true;
        }

        public byte[] ObterDANFSE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

                if (string.IsNullOrEmpty(configuracaoAmbiente.ChaveParceiroMigrate))
                    throw new ServicoException(@"Processo Abortado: Chave do Parceiro não configurado.");

                // Obter Request
                Servicos.ServicosMigrate.InvoiCy envio = this.obterConsultarRetornoNFSe(cte, false, true, configuracaoAmbiente.ChaveParceiroMigrate);

                // Transmitir
                string urlWebService = this.ObterUrlWebService(cte.TipoAmbiente);
                ServicosMigrate.recepcaoSoapPortClient servicoMigrate = ObterClientIntegrarNFSe(urlWebService);
                servicoMigrate.Endpoint.EndpointBehaviors.Add(inspector);
                Servicos.ServicosMigrate.ExecuteResponse retConsultarRetorno = servicoMigrate.ExecuteAsync(envio).Result;

                var documento = this.ProcessaRetornoObterDocumentoConsulta(cte, retConsultarRetorno);

                if (documento != null)
                {
                    this.SalvarArquivoPDF(cte, documento.DocPDF, unitOfWork);
                    return Convert.FromBase64String(documento.DocPDF);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }

            return null;
        }

        public string ObterXMLNFSE(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

                if (string.IsNullOrEmpty(configuracaoAmbiente.ChaveParceiroMigrate))
                    throw new ServicoException(@"Processo Abortado: Chave do Parceiro não configurado.");

                // Obter Request
                Servicos.ServicosMigrate.InvoiCy envio = this.obterConsultarRetornoNFSe(cte, true, false, configuracaoAmbiente.ChaveParceiroMigrate);

                // Transmitir
                string urlWebService = this.ObterUrlWebService(cte.TipoAmbiente);
                ServicosMigrate.recepcaoSoapPortClient servicoMigrate = ObterClientIntegrarNFSe(urlWebService);
                servicoMigrate.Endpoint.EndpointBehaviors.Add(inspector);
                Servicos.ServicosMigrate.ExecuteResponse retConsultarRetorno = servicoMigrate.ExecuteAsync(envio).Result;

                var documento = this.ProcessaRetornoObterDocumentoConsulta(cte, retConsultarRetorno);

                if (documento != null)
                {
                    //this.SalvarArquivoXML(cte, false, documento.DocXML, Dominio.Enumeradores.TipoXMLCTe.Autorizacao, cte.Status, unitOfWork);
                    byte[] decodedData = System.Text.Encoding.Default.GetPreamble().Concat(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.Default, System.Convert.FromBase64String(documento.DocXML))).ToArray();
                    return Encoding.UTF8.GetString(decodedData);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }

            return null;
        }

        #endregion

        #region Métodos Privados

        private Servicos.ServicosMigrate.InvoiCy obterConsultarRetornoNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, bool pGerarXml = true, bool pGerarPDF = true, string chaveParceiro = "")
        {
            Servicos.ServicosMigrate.InvoiCy.DadosType listaDocumentos = new ServicosMigrate.InvoiCy.DadosType();

            string sb;
            sb = "<Consulta>";
            sb += "<ModeloDocumento>NFSe</ModeloDocumento>";
            sb += "<Versao>1.00</Versao>";
            sb += "<tpAmb>" + (cte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? 1 : 2) + "</tpAmb>";
            sb += "<CnpjEmissor>" + cte.Empresa.CNPJ + "</CnpjEmissor>";
            sb += "<NumeroInicial>" + cte.RPS.Numero + "</NumeroInicial>";
            sb += "<NumeroFinal>" + cte.RPS.Numero + "</NumeroFinal>";
            sb += "<Serie>" + cte.RPS.Serie + "</Serie>";
            sb += "<ChaveAcesso/>";
            sb += "<DataEmissaoInicial/>";
            sb += "<DataEmissaoFinal/>";
            sb += "</Consulta>";

            string param;
            param = "<ParametrosConsulta>";
            param += "<Situacao>S</Situacao>";
            param += "<XMLCompleto>" + (pGerarXml ? "S" : "N") + "</XMLCompleto>";
            param += "<XMLLink>N</XMLLink>";
            param += "<PDFBase64>" + (pGerarPDF ? "S" : "N") + "</PDFBase64>";
            param += "<PDFLink>N</PDFLink>";
            param += "<Eventos>N</Eventos>";
            param += "</ParametrosConsulta>";

            //Cria um objeto para guardar os dados do cabeçalho da conexão
            //Chama a função que gera a CK, e depois adiciona a mesma no cabeçalho.
            Servicos.ServicosMigrate.InvoiCyRecepcaoCabecalho cabecalho = new Servicos.ServicosMigrate.InvoiCyRecepcaoCabecalho()
            {
                EmpPK = chaveParceiro,
                EmpCK = this.GeraHashMD5(sb, cte.Empresa.Configuracao.TokenMigrate),
                EmpCO = ""
            };

            //Armazena os dados da requisição.
            Servicos.ServicosMigrate.InvoiCyRecepcaoDadosItem documento = new Servicos.ServicosMigrate.InvoiCyRecepcaoDadosItem();
            documento.Documento = sb;
            documento.Parametros = param;
            listaDocumentos.Add(documento);

            //Adiciona as informações da requisição
            Servicos.ServicosMigrate.InvoiCyRecepcaoInformacoes Info = new Servicos.ServicosMigrate.InvoiCyRecepcaoInformacoes()
            {
                Texto = ""
            };

            //Adiciona os dados na recepção
            Servicos.ServicosMigrate.InvoiCy retorno = new Servicos.ServicosMigrate.InvoiCy()
            {
                Cabecalho = cabecalho,
                Informacoes = Info,
                Dados = listaDocumentos
            };

            return retorno;
        }

        private void processarRetornoConsultarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Servicos.ServicosMigrate.ExecuteResponse retConsultarRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            foreach (var msgitem in retConsultarRetorno.Invoicyretorno.Mensagem)
            {
                if (msgitem.Codigo == 100 || msgitem.Codigo == 313) // 100 - Consulta efetuada com sucesso; 313 - já efetivado no sistema
                {
                    //Percorre documento atualizando situação
                    foreach (var documentos in msgitem.Documentos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe retWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe();
                        retWS = (Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe)DeSerialize<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe>("<RetornoMigrate>" + documentos.Documento + "</RetornoMigrate>");

                        foreach (var documento in retWS.Documento)
                        {
                            if (cte.Status == "E")
                            { 
                                if (documento.DocSitCodigo == "999") // 999 - Rejeitado
                                {
                                    throw new ServicoException($"Processo abortado: {documento.DocSitCodigo + "-" + documento.DocSitDescricao}");
                                }
                                else if (documento.DocSitCodigo == "104" || documento.DocSitCodigo == "105") // 104 - Em processamento, 105 - Pendente
                                {
                                    continue;
                                }
                                else if (documento.DocSitCodigo == "100") // 100 - Autorizado
                                {
                                    autorizarNFSe(cte, documento, unitOfWork);
                                }
                                else
                                {
                                    throw new ServicoException($"Processo abortado: {documento.DocSitCodigo + "-" + documento.DocSitDescricao}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    cte.MensagemRetornoSefaz = $"Processo abortado: {msgitem.Codigo.ToString() + " - " + msgitem.Descricao}";
                }
            }
        }

        private Documento ProcessaRetornoObterDocumentoConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Servicos.ServicosMigrate.ExecuteResponse retEnvio)
        {
            foreach (var msgitem in retEnvio.Invoicyretorno.Mensagem)
            {
                //Consulta efetuada com sucesso
                if (msgitem.Codigo == 100)
                {
                    //Percorre documento atualizando situação
                    foreach (var documentos in msgitem.Documentos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe retWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe();
                        retWS = (Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe)DeSerialize<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoConsultarNFSe>("<RetornoMigrate>" + documentos.Documento + "</RetornoMigrate>");

                        foreach (var documento in retWS.Documento)
                        {
                            if (cte.RPS.Numero == Convert.ToInt32(documento.DocNumero))
                                return documento;
                        }
                    }
                }
            }

            return null;
        }

        #endregion Métodos Privados

    }
}