using System;
using System.Collections.Generic;
using System.IO;

namespace Servicos.Embarcador.Integracao.PH
{
    public class IntegracaoPH
    {
        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoPH || string.IsNullOrWhiteSpace(configuracaoIntegracao.UsuarioPH) || string.IsNullOrWhiteSpace(configuracaoIntegracao.SenhaPH))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a PH.";
                repCargaIntegracao.Atualizar(cargaIntegracao);
                return;
            }

            bool todosConhecimentosEnviados = true;
            string msgErro = "";
            string arquivosRetornos = "";
            string arquivosEnvios = "";

            List<string> naoGerar = new List<string>();

            try
            {
                MultiSoftware.EFD.PH ph = new MultiSoftware.EFD.PH(cargaIntegracao.Carga.Empresa, DateTime.Now.Date, DateTime.Now.Date, cargaIntegracao.Carga.Codigo, 0, 0, "011", naoGerar, unitOfWork);
                StreamReader reader = new StreamReader(ph.GerarPH());
                arquivosEnvios = reader.ReadToEnd();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "ArquivoPH");
                todosConhecimentosEnviados = false;
                arquivosRetornos = "Falha na geração do arquivo";
            }

            ServicoPHContabil.GravarRequest svcGravarPH = new ServicoPHContabil.GravarRequest();
            svcGravarPH.Senha = configuracaoIntegracao.SenhaPH;
            svcGravarPH.Usuario = configuracaoIntegracao.UsuarioPH;
            svcGravarPH.Xml = "<![CDATA[<?xml version='1.0' encoding='ISO-8859-1'?>" +
                              "<Mensagem>" +
                                    "<Cabecalho>" +
                                        "<identificador>AutorizacaoCarga_" + cargaIntegracao.Carga.Codigo.ToString("D") + "</identificador>" +
                                        "<operacao>DOCUMENTO</operacao>" +
                                        "<cpf_cnpj_origem>" + cargaIntegracao.Carga.Empresa.CNPJ_SemFormato + "</cpf_cnpj_origem>" +
                                        "<cpf_cnpj_destino>" + configuracaoIntegracao.CNPJContadorPH + "</cpf_cnpj_destino>" +
                                        "<software_origem>MultiTMS</software_origem>" +
                                        "<software_destino>EFPH</software_destino>" +
                                    "</Cabecalho>" +
                                    "<Corpo>" +
                                        "<dtini>" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "</dtini>" +
                                        "<dtfim>" + DateTime.Now.Date.ToString("dd/MM/yyyy") + "</dtfim>" +
                                        "<conteudo>" +
                                            arquivosEnvios +
                                        "</conteudo>" +
                                    "</Corpo>" +
                                "</Mensagem>]]>";

            try
            {
                ServicoPHContabil.MetodosPHClient metodosPHClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoPHContabil.MetodosPHClient, ServicoPHContabil.IMetodosPH>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.PHContabil_MetodosPH);
                string msgRetorno = metodosPHClient.Gravar(svcGravarPH.Usuario, svcGravarPH.Senha, svcGravarPH.Xml);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "ArquivoPH");
                todosConhecimentosEnviados = false;
                arquivosRetornos = "Falha no envio do arquivo";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosEnvios, "txt", unitOfWork);
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;

            if (todosConhecimentosEnviados)
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaIntegracao.ProblemaIntegracao = "Sucesso";
                arquivoIntegracao.Mensagem = "Sucesso";
                cargaIntegracao.Protocolo = "";
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "txt", unitOfWork);
            }
            else
            {
                arquivosRetornos += " " + msgErro;
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Falha ao processar. " + msgErro;
                arquivoIntegracao.Mensagem = "Falha ao processar. " + msgErro;
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(arquivosRetornos, "txt", unitOfWork);
            }

            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

    }
}
