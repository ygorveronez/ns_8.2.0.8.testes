using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao.Totvs
{
    public class Carga
    {
        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string mensagem = "")
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContrato = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Movimento svsMovimento = new Movimento();
            Pessoa svsPessoa = new Pessoa();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe = repCargaCTe.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato = repContrato.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);
            Dominio.Entidades.Embarcador.Documentos.CIOT ciot = repCIOT.BuscarPorCarga(cargaCargaIntegracao.Carga.Codigo);

            cargaCargaIntegracao.NumeroTentativas += 1;
            cargaCargaIntegracao.DataIntegracao = DateTime.Now;

            if (configuracaoIntegracao == null || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoTotvs))
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a TOTVS";
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);

                return;
            }

            bool situacaoIntegracao = true;
            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            try
            {
                bool integradoTerceiro = true;
                Dominio.Entidades.Cliente terceiro = null;
                bool integradoMovimento = false;
                string numeroContrato = ciot?.Numero ?? (contrato?.NumeroContrato.ToString("D") ?? "");
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = null;

                if (cargaCargaIntegracao.Carga.Terceiro != null)
                {
                    terceiro = repCliente.BuscarPorCPFCNPJ(cargaCargaIntegracao.Carga.Terceiro.CPF_CNPJ);
                    xmlRequest = string.Empty;
                    xmlResponse = string.Empty;

                    if (terceiro.Tipo == "F")
                        integradoTerceiro = svsPessoa.IntegrarAutonomo(terceiro, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                    else
                        integradoTerceiro = svsPessoa.IntegrarPessoaJuridica(terceiro, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);                    

                    if (integradoTerceiro)
                    {
                        arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = "1º - Integração de PJ/Autonomo";
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                        cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                }

                if (integradoTerceiro)
                {
                    foreach (var cargaCTe in cargasCTe)
                    {
                        bool integrouPessoa = false;
                        if (cargaCTe.CTe.Destinatario != null && cargaCTe.CTe.Destinatario.Cliente != null)
                        {
                            xmlRequest = string.Empty;
                            xmlResponse = string.Empty;
                            if (cargaCTe.CTe.Destinatario.Cliente.Tipo == "F")
                                integrouPessoa = svsPessoa.IntegrarAutonomo(cargaCTe.CTe.Destinatario.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            else
                                integrouPessoa = svsPessoa.IntegrarPessoaJuridica(cargaCTe.CTe.Destinatario.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            if (integrouPessoa)
                            {
                                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                                arquivoIntegracao.Mensagem = "Integração Destinatário";
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            }
                        }
                        if (cargaCTe.CTe.Remetente != null && cargaCTe.CTe.Remetente.Cliente != null)
                        {
                            xmlRequest = string.Empty;
                            xmlResponse = string.Empty;
                            if (cargaCTe.CTe.Remetente.Cliente.Tipo == "F")
                                integrouPessoa = svsPessoa.IntegrarAutonomo(cargaCTe.CTe.Remetente.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            else
                                integrouPessoa = svsPessoa.IntegrarPessoaJuridica(cargaCTe.CTe.Remetente.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            if (integrouPessoa)
                            {
                                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                                arquivoIntegracao.Mensagem = "Integração Remetente";
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            }
                        }
                        if (cargaCTe.CTe.Recebedor != null && cargaCTe.CTe.Recebedor.Cliente != null)
                        {
                            xmlRequest = string.Empty;
                            xmlResponse = string.Empty;
                            if (cargaCTe.CTe.Recebedor.Cliente.Tipo == "F")
                                integrouPessoa = svsPessoa.IntegrarAutonomo(cargaCTe.CTe.Recebedor.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            else
                                integrouPessoa = svsPessoa.IntegrarPessoaJuridica(cargaCTe.CTe.Recebedor.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            if (integrouPessoa)
                            {
                                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                                arquivoIntegracao.Mensagem = "Integração Recebedor";
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            }
                        }
                        if (cargaCTe.CTe.Expedidor != null && cargaCTe.CTe.Expedidor.Cliente != null)
                        {
                            xmlRequest = string.Empty;
                            xmlResponse = string.Empty;
                            if (cargaCTe.CTe.Expedidor.Cliente.Tipo == "F")
                                integrouPessoa = svsPessoa.IntegrarAutonomo(cargaCTe.CTe.Expedidor.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            else
                                integrouPessoa = svsPessoa.IntegrarPessoaJuridica(cargaCTe.CTe.Expedidor.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            if (integrouPessoa)
                            {
                                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                                arquivoIntegracao.Mensagem = "Integração Expedidor";
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            }
                        }
                        if (cargaCTe.CTe.OutrosTomador != null && cargaCTe.CTe.OutrosTomador.Cliente != null)
                        {
                            xmlRequest = string.Empty;
                            xmlResponse = string.Empty;
                            if (cargaCTe.CTe.OutrosTomador.Cliente.Tipo == "F")
                                integrouPessoa = svsPessoa.IntegrarAutonomo(cargaCTe.CTe.OutrosTomador.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            else
                                integrouPessoa = svsPessoa.IntegrarPessoaJuridica(cargaCTe.CTe.OutrosTomador.Cliente, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            if (integrouPessoa)
                            {
                                arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                                arquivoIntegracao.Mensagem = "Integração Outro Tomador";
                                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            }
                        }

                        xmlRequest = string.Empty;
                        xmlResponse = string.Empty;

                        terceiro = repCliente.BuscarPorCPFCNPJ(cargaCTe.CTe.Tomador.Cliente.CPF_CNPJ);
                        if (cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "57" || cargaCTe.CTe.ModeloDocumentoFiscal.Numero == "39")
                        {
                            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                integradoMovimento = svsMovimento.IntegrarCTe(cargaCTe.CTe, terceiro, cargaCTe.Carga.TipoOperacao, cargaCTe.Carga.Veiculo, numeroContrato, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);
                            else
                                integradoMovimento = svsMovimento.IntegrarNFs(cargaCTe.CTe, terceiro, cargaCTe.Carga.TipoOperacao, cargaCTe.Carga.Veiculo, configuracaoIntegracao.URLIntegracaoTotvs, configuracaoIntegracao.UsuarioTotvs, configuracaoIntegracao.SenhaTotvs, configuracaoIntegracao.ContextoTotvs, out xmlRequest, out xmlResponse, unitOfWork);

                            arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = "2 º Integração de Movimento CT-e/NFSe";
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);
                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                            cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                            if (!integradoMovimento)
                            {
                                mensagemErro = xmlResponse;
                                situacaoIntegracao = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(xmlResponse))
                        mensagemErro = "Falha na integração com a TOTVS.";
                    else
                        mensagemErro = "Retorno TOTVS: " + xmlResponse;

                    situacaoIntegracao = false;

                    arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = mensagemErro;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                    cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                    repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoTOTVS");
                Servicos.Log.TratarErro("Request: " + xmlRequest, "IntegracaoTOTVS");
                Servicos.Log.TratarErro("Response: " + xmlResponse, "IntegracaoTOTVS");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da TOTVS.";
                situacaoIntegracao = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                arquivoIntegracao.Data = cargaCargaIntegracao.DataIntegracao;
                arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "txt", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                cargaCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
                return;
            }

            if (!situacaoIntegracao)
            {
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = mensagemErro;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
            else
            {
                cargaCargaIntegracao.ProblemaIntegracao = string.Empty;
                cargaCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                repCargaIntegracao.Atualizar(cargaCargaIntegracao);
            }
        }

    }
}
