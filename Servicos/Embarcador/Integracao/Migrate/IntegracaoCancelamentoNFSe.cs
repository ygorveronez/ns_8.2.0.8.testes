using Dominio.Entidades.Embarcador.BI;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor;
using Dominio.ObjetosDeValor.WebService.NFSe;
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
        public bool CancelarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Integracao.InspectorBehavior inspector = new Models.Integracao.InspectorBehavior(true);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);

            try
            {
                if (!(cte.Status == "K" || cte.Status == "A"))
                    return true;

                if (string.IsNullOrEmpty(cte.Empresa?.Configuracao?.TokenMigrate))
                    throw new ServicoException(@"Processo Abortado: Token não informado na configuração da empresa");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();

                if (string.IsNullOrEmpty(configuracaoAmbiente.ChaveParceiroMigrate))
                    throw new ServicoException(@"Processo Abortado: Chave do Parceiro não configurado.");

                // Obter Request
                Servicos.ServicosMigrate.InvoiCy envio = this.obterCancelarNFSe(cte, configuracaoAmbiente.ChaveParceiroMigrate);

                // Transmitir
                string urlWebService = this.ObterUrlWebService(cte.TipoAmbiente);
                ServicosMigrate.recepcaoSoapPortClient servicoMigrate = ObterClientIntegrarNFSe(urlWebService);
                servicoMigrate.Endpoint.EndpointBehaviors.Add(inspector);
                Servicos.ServicosMigrate.ExecuteResponse retEnvio = servicoMigrate.ExecuteAsync(envio).Result;

                // TODO: Criar estrutura para guardar os XMLs de comunicação com a migrate e disponibilizar através de uma opção "Download XML Migrate" na tela de cargas etapa 4.
                string xmlEnvioMigrate = inspector.LastRequestXML;
                string xmlRetornoEnvioMigrate = inspector.LastResponseXML;

                // Processar Response
                this.processarRetornoCancelarNFSe(cte, retEnvio, unitOfWork);
            }
            catch (ServicoException excecao)
            {
                String message = excecao.Message;
                if (message.Length > 300)
                    message = message.Substring(0, 300);

                cte.Status = "A";
                cte.MensagemRetornoSefaz = message;
                repCTe.Atualizar(cte);
            }
            catch (Exception excecao)
            {
                cte.Status = "A";
                cte.MensagemRetornoSefaz = "Processo Abortado: Ocorreu um erro ao integrar NFSe com a migrate";
                repCTe.Atualizar(cte);

                Servicos.Log.TratarErro(excecao);
            }

            return true;
        }
        #endregion

        #region Métodos Privados

        private Servicos.ServicosMigrate.InvoiCy obterCancelarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string chaveParceiro)
        {
            Servicos.ServicosMigrate.InvoiCy.DadosType listaDocumentos = new ServicosMigrate.InvoiCy.DadosType();

            string sb;
            sb = "<EnvioEvento>";
            sb += "<ModeloDocumento>NFSe</ModeloDocumento>";
            sb += "<Versao>1.00</Versao>";
            sb += "<Evento>";
            sb += "<CNPJ>" + cte.Empresa.CNPJ + "</CNPJ>";
            sb += "<NFSeNumero>" + cte.Numero + "</NFSeNumero>";
            sb += "<RPSNumero>" + cte.RPS.Numero + "</RPSNumero>";
            sb += "<RPSSerie>" + cte.RPS.Serie + "</RPSSerie>";
            sb += "<EveTp>110111</EveTp>";
            sb += "<tpAmb>" + (cte.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? 1 : 2) + "</tpAmb>";
            sb += "</Evento>";
            sb += "</EnvioEvento>";

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
            documento.Parametros = "";
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

        private void processarRetornoCancelarNFSe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Servicos.ServicosMigrate.ExecuteResponse retEnvio, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unitOfWork);

            foreach (var msgitem in retEnvio.Invoicyretorno.Mensagem)
            {
                if (msgitem.Codigo == 100)
                {
                    //Percorre documento atualizando situação
                    foreach (var documentos in msgitem.Documentos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe retWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe();
                        retWS = (Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe)DeSerialize<Dominio.ObjetosDeValor.Embarcador.Integracao.Migrate.RetornoNFSe>("<RetornoMigrate>" + documentos.Documento + "</RetornoMigrate>");

                        foreach (var documento in retWS.Documento)
                        {
                            if (cte.RPS.Numero == Convert.ToInt32(documento.DocNumero))
                            {
                                if (documento.Situacao.SitCodigo == "101") // Cancelado
                                {
                                    unitOfWork.Start();

                                    try
                                    {
                                        DateTime dataCancelamento = DateTime.Now;

                                        cte.Status = "C";
                                        cte.DataCancelamento = DateTime.Now;//verificar retorno migrate
                                        cte.MensagemRetornoSefaz = documento.Situacao.SitCodigo + "-" + documento.Situacao.SitDescricao;
                                        cte.Cancelado = "S";
                                        cte.DataRetornoSefaz = dataCancelamento;
                                        cte.DataCancelamento = dataCancelamento;

                                        cte.RPS.Status = "C";
                                        cte.RPS.ProtocoloCancelamento = null;
                                        cte.RPS.DataProtocolo = dataCancelamento;

                                        repCTe.Atualizar(cte);
                                        repRPSNFSe.Atualizar(cte.RPS);

                                        serCTe.SalvarMovimentoDoFinanceiro(cte, cte.Empresa.Codigo, unitOfWork);

                                        serCTe.AjustarAverbacoesParaCancelamento(cte.Codigo, unitOfWork);

                                        // TODO: Salvar XML Cancelamento
                                        //SalvarArquivoXML(cte, unitOfWork, true, "COLOCAR O XML", Dominio.Enumeradores.TipoXMLCTe.Cancelamento, cte.Status);

                                        unitOfWork.CommitChanges();
                                    }
                                    catch
                                    {
                                        unitOfWork.Rollback();
                                        throw;
                                    }
                                }
                                else
                                {
                                    cte.MensagemRetornoSefaz = $"Processo abortado: {documento.Situacao.SitCodigo + " - " + documento.Situacao.SitDescricao}";
                                    repCTe.Atualizar(cte);
                                }
                            }
                        }
                    }
                }
                else
                {
                    cte.MensagemRetornoSefaz = $"Processo abortado: {msgitem.Codigo.ToString() + " - " + msgitem.Descricao}";
                    repCTe.Atualizar(cte);
                }
            }
        }
        #endregion Métodos Privados

    }
}