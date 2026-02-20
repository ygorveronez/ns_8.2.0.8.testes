using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar;
using Dominio.ObjetosDeValor.Relatorios;
using Newtonsoft.Json;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.SemParar
{
    public class ValePedagio
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial Autenticar(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool registrarErro = true)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Piracanjuba.IntegracaoPiracanjuba servicoIntegracaoPiracanjuba = new Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware);

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial();
            bool possuiIntegracaoPiracanjuba = IsPossuiIntegracaoPiracanjuba(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, tipoServicoMultisoftware);

                if (integracaoSemParar == null)
                {
                    credencial.Autenticado = false;
                    credencial.Retorno = "Sem parar não está configurado, por favor, entre em contato com a Multisoftware";

                    if (registrarErro)
                    {
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.ProblemaIntegracao = "Sem parar não está configurado, por favor, entre em contato com a Multisoftware";
                        cargaValePedagio.NumeroTentativas++;
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        if (possuiIntegracaoPiracanjuba)
                            servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
                    }
                }
                else
                {

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                    string cnpj = integracaoSemParar.CNPJ;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (string.IsNullOrWhiteSpace(cnpj))
                            cnpj = cargaValePedagio.Carga.Empresa.CNPJ;
                    }

                    SemPararValePedagio.Identificador identifador = valePedagioClient.autenticarUsuario(cnpj, integracaoSemParar.Usuario, integracaoSemParar.Senha);
                    if (identifador.status == 0)
                    {
                        credencial.Autenticado = true;
                        credencial.Retorno = "Autenticado com sucesso";
                        credencial.Sessao = identifador.sessao;
                    }
                    else
                    {
                        credencial.Autenticado = false;
                        credencial.Retorno = ObterMensagemRetorno(identifador.status);

                        if (registrarErro)
                        {
                            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo
                            {
                                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork),
                                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork),
                                Data = DateTime.Now,
                                Mensagem = credencial.Retorno,
                                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                            };

                            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);

                            cargaValePedagio.DataIntegracao = DateTime.Now;
                            cargaValePedagio.ProblemaIntegracao = credencial.Retorno;
                            cargaValePedagio.NumeroTentativas++;
                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            if (cargaValePedagio.SituacaoValePedagio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.EmCancelamento)
                                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada;

                            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

                            repCargaValePedagio.Atualizar(cargaValePedagio);

                            if (possuiIntegracaoPiracanjuba)
                                servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                credencial.Autenticado = false;
                credencial.Retorno = "O WS da sem parar não está disponivel no momento.";
                if (registrarErro)
                {
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.ProblemaIntegracao = "O WS da sem parar não está disponivel no momento.";
                    cargaValePedagio.NumeroTentativas++;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    if (possuiIntegracaoPiracanjuba)
                        servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);

                }
            }

            return credencial;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial Autenticar(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool registrarErro = true)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial();

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaConsultaValePedagio.Carga, tipoServicoMultisoftware);

                if (integracaoSemParar == null)
                {
                    credencial.Autenticado = false;
                    credencial.Retorno = "Sem parar não está configurado, por favor, entre em contato com a Multisoftware";

                    if (registrarErro)
                    {
                        cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                        cargaConsultaValePedagio.ProblemaIntegracao = "Sem parar não está configurado, por favor, entre em contato com a Multisoftware";
                        cargaConsultaValePedagio.NumeroTentativas++;
                        cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        repCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);
                        //setar carga como possui pendencia

                        cargaConsultaValePedagio.Carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio. Sem parar não está configurado, por favor, entre em contato com a Multisoftware";
                        cargaConsultaValePedagio.Carga.PossuiPendencia = true;
                        cargaConsultaValePedagio.Carga.ProblemaIntegracaoValePedagio = true;
                        repCarga.Atualizar(cargaConsultaValePedagio.Carga);
                    }
                }
                else
                {

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                    string cnpj = integracaoSemParar.CNPJ;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (string.IsNullOrWhiteSpace(cnpj))
                            cnpj = cargaConsultaValePedagio.Carga.Empresa.CNPJ;
                    }

                    SemPararValePedagio.Identificador identifador = valePedagioClient.autenticarUsuario(cnpj, integracaoSemParar.Usuario, integracaoSemParar.Senha);
                    if (identifador.status == 0)
                    {
                        credencial.Autenticado = true;
                        credencial.Retorno = "Autenticado com sucesso";
                        credencial.Sessao = identifador.sessao;
                    }
                    else
                    {
                        credencial.Autenticado = false;
                        credencial.Retorno = ObterMensagemRetorno(identifador.status);

                        if (registrarErro)
                        {
                            Servicos.Log.TratarErro(credencial.Retorno);
                            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                            cargaConsultaValePedagio.ProblemaIntegracao = credencial.Retorno;
                            cargaConsultaValePedagio.NumeroTentativas++;
                            cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                            cargaValePedagioIntegracaoArquivo.Mensagem = credencial.Retorno;
                            cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                            cargaConsultaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
                            repCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);

                            cargaConsultaValePedagio.Carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio.";
                            cargaConsultaValePedagio.Carga.PossuiPendencia = true;
                            cargaConsultaValePedagio.Carga.ProblemaIntegracaoValePedagio = true;
                            repCarga.Atualizar(cargaConsultaValePedagio.Carga);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                credencial.Autenticado = false;
                credencial.Retorno = "O WS da sem parar não está disponivel no momento.";
                if (registrarErro)
                {
                    cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                    cargaConsultaValePedagio.ProblemaIntegracao = "O WS da sem parar não está disponivel no momento.";
                    cargaConsultaValePedagio.NumeroTentativas++;
                    cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    repCargaConsultaValePedagio.Atualizar(cargaConsultaValePedagio);


                    cargaConsultaValePedagio.Carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio. O WS da sem parar não está disponivel no momento.";
                    cargaConsultaValePedagio.Carga.PossuiPendencia = true;
                    cargaConsultaValePedagio.Carga.ProblemaIntegracaoValePedagio = true;
                    repCarga.Atualizar(cargaConsultaValePedagio.Carga);
                }
            }

            return credencial;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial Autenticar(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, out string xmlRequest, out string xmlResponse)
        {
            xmlRequest = string.Empty;
            xmlResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial();
            try
            {
                Frota.ValePedagio svcValePedagio = new Frota.ValePedagio(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = svcValePedagio.ObterIntegracaoSemPararParaAutenticacao(tipoServicoMultisoftware);
                if (integracaoSemParar == null)
                {
                    credencial.Autenticado = false;
                    credencial.Retorno = "Sem Parar não está configurado, por favor, entre em contato com a Multisoftware";
                }
                else
                {

                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                    SemPararValePedagio.Identificador identifador = valePedagioClient.autenticarUsuario(integracaoSemParar.CNPJ, integracaoSemParar.Usuario, integracaoSemParar.Senha);

                    xmlRequest = inspector.LastRequestXML;
                    xmlResponse = inspector.LastResponseXML;
                    //Servicos.Log.TratarErro(xmlRequest, "IntegracaoSemParar");
                    //Servicos.Log.TratarErro(xmlResponse, "IntegracaoSemParar");

                    if (identifador.status == 0)
                    {
                        credencial.Autenticado = true;
                        credencial.Retorno = "Autenticado com sucesso";
                        credencial.Sessao = identifador.sessao;
                    }
                    else
                    {
                        credencial.Autenticado = false;
                        credencial.Retorno = ObterMensagemRetorno(identifador.status);

                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                credencial.Autenticado = false;
                credencial.Retorno = "O WS da sem parar não está disponivel no momento. " + ex.Message;
            }
            erro = credencial.Retorno;
            return credencial;
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.RetornoConsultaSituacaoVeiculo consultarSituacaoVeiculoSemParar(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao consultaCargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.RetornoConsultaSituacaoVeiculo retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.RetornoConsultaSituacaoVeiculo();
            retorno.Data = DateTime.Now;

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            SemPararValePedagio.Veiculo[] retornoStatusVeiculo = valePedagioClient.obterStatusVeiculo(carga.Veiculo?.Placa ?? "", credencial.Sessao);
            if (retornoStatusVeiculo != null && retornoStatusVeiculo.Length > 0 && retornoStatusVeiculo[0].status != 0)
            {
                retorno.Erro = ObterMensagemRetorno(retornoStatusVeiculo[0].status);
                retorno.status = retornoStatusVeiculo[0].status;
                retorno.JsonRequest = inspector.LastRequestXML;
                retorno.JsonResponse = inspector.LastResponseXML;
            }

            return retorno;
        }

        public decimal ConsultaValorPedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao consultaCargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repConsultaValorPedagioIntegrecao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
            Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

            Servicos.Embarcador.Integracao.SemParar.PracasPedagio serPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();

            Dominio.Entidades.Embarcador.Cargas.Carga Carga = consultaCargaValePedagio.Carga;

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.Buscar();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> ListaCargaRotaFretePontosPasssagem = null;
            List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagemPreDefinidos = null;

            if (consultaCargaValePedagio.RotaFrete != null)
                pontosPassagemPreDefinidos = repRotaFretePontosPassagem.BuscarPorRotaFrete(consultaCargaValePedagio.RotaFrete.Codigo);

            if (pontosPassagemPreDefinidos == null || pontosPassagemPreDefinidos.Count == 0)
                ListaCargaRotaFretePontosPasssagem = repCargaRotaFretePontosPassagem.BuscarPorCarga(carga.Codigo);

            string[] concessionariasComDescontos = null;
            string[] percentualDescontoConcessionarias = null;
            string placaPadraoConsultaValorPedagio = "";

            bool consultarPeloCustoDaRota = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().ConsultarPeloCustoDaRota.HasValue ? Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().ConsultarPeloCustoDaRota.Value : false;

            if (consultarPeloCustoDaRota)
            {
                if (!string.IsNullOrEmpty(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().ConcessionariasComDescontos))
                    concessionariasComDescontos = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().ConcessionariasComDescontos.Split('|');
                if (!string.IsNullOrEmpty(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PercentualDescontoConcessionarias))
                    percentualDescontoConcessionarias = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PercentualDescontoConcessionarias.Split('|');
                if (!string.IsNullOrEmpty(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PlacaPadraoConsultaValorPedagio))
                    placaPadraoConsultaValorPedagio = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().PlacaPadraoConsultaValorPedagio;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosdaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();

            if (pontosPassagemPreDefinidos != null && pontosPassagemPreDefinidos.Count > 0)
            {
                foreach (Dominio.Entidades.RotaFretePontosPassagem pontospassagem in pontosPassagemPreDefinidos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota ponto = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();
                    ponto.codigo = pontospassagem.Codigo;
                    ponto.descricao = pontospassagem.Descricao;
                    ponto.distancia = pontospassagem.Distancia;
                    ponto.lat = (double)pontospassagem.Latitude;
                    ponto.lng = (double)pontospassagem.Longitude;
                    ponto.tempo = pontospassagem.Tempo;
                    ponto.tipoponto = pontospassagem.TipoPontoPassagem;
                    ponto.codigo_cliente = pontospassagem.Cliente != null ? pontospassagem.Cliente.Codigo : 0;

                    pontosdaRota.Add(ponto);
                }
            }

            if (pontosdaRota.Count == 0 && ListaCargaRotaFretePontosPasssagem != null && ListaCargaRotaFretePontosPasssagem.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargarotaPontoPassagem in ListaCargaRotaFretePontosPasssagem)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota ponto = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();
                    ponto.codigo = cargarotaPontoPassagem.Codigo;
                    ponto.descricao = cargarotaPontoPassagem.Descricao;
                    ponto.distancia = cargarotaPontoPassagem.Distancia;
                    ponto.lat = (double)cargarotaPontoPassagem.Latitude;
                    ponto.lng = (double)cargarotaPontoPassagem.Longitude;
                    ponto.tempo = cargarotaPontoPassagem.Tempo;
                    ponto.tipoponto = cargarotaPontoPassagem.TipoPontoPassagem;
                    ponto.codigo_cliente = cargarotaPontoPassagem.Cliente != null ? cargarotaPontoPassagem.Cliente.Codigo : 0;

                    pontosdaRota.Add(ponto);
                }
            }

            if (pontosdaRota.Count > 0)
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                string erro = "";

                string pontosDaRotaSerializado = JsonConvert.SerializeObject(pontosdaRota);
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();

                string requestPracas = "";
                string responsePracas = "";

                if ((carga.Rota?.ApenasObterPracasPedagio ?? false) || (!string.IsNullOrEmpty(carga.Rota?.PolilinhaRota) && configuracaoRoteirizacao.SempreUtilizarRotaParaBuscarPracasPedagio))
                    pracasPedagio = serPracasPedagio.ObterPracasPedagioPorPolilinha(credencial, carga.Rota.PolilinhaRota, integracaoSemParar?.DistanciaMinimaQuadrante ?? 0, out erro, unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida);
                else
                    pracasPedagio = serPracasPedagio.ObterPracasPedagioIda(credencial, pontosDaRotaSerializado, out erro, unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out requestPracas, out responsePracas);

                if (ValidarUltimoTipoRoterizado(pontosPassagemPreDefinidos, ListaCargaRotaFretePontosPasssagem))
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioRetorno = serPracasPedagio.ObterPracasPedagioVolta(credencial, pontosDaRotaSerializado, out erro, unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out requestPracas, out responsePracas);
                    pracasPedagio.AddRange(pracasPedagioRetorno);
                }

                if (!string.IsNullOrEmpty(erro))
                {
                    consultaCargaValePedagio.ProblemaIntegracao = erro;
                    consultaCargaValePedagio.NumeroTentativas++;
                    consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    //setar carga como possui pendencia
                    carga.MotivoPendencia = erro;
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoValePedagio = true;
                    repCarga.Atualizar(carga);

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                    cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(requestPracas, "xml", unitOfWork);
                    cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(responsePracas, "xml", unitOfWork);
                    cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                    cargaValePedagioIntegracaoArquivo.Mensagem = erro;

                    cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                    consultaCargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

                    return 0;
                }

                //vamos criar uma rota temporaria no sem parar e depois pesquisar essa rota o valor;
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> PracasdePedagio = pracasPedagio.Distinct().ToList();
                List<int> listpracas = PracasdePedagio.Select(obj => int.Parse(obj.CodigoIntegracao)).ToList();
                string mensagem = "";

                //cadastrando rota temporaria no sem parar;
                string rotaTempararia = Guid.NewGuid().ToString().Replace("-", "");
                Servicos.SemPararValePedagio.InfoRota infoRota = valePedagioClient.cadastrarRotaTemporaria(listpracas.ToArray(), rotaTempararia, credencial.Sessao);

                if (infoRota.status == 0)
                {
                    try
                    {
                        Servicos.SemPararValePedagio.Rota rota = new SemPararValePedagio.Rota
                        {
                            nome = rotaTempararia,
                            codigo = infoRota.id,
                            pracas = infoRota.pracas
                        };

                        Servicos.SemPararValePedagio.CategoriaVeiculo categoria = new SemPararValePedagio.CategoriaVeiculo { eixos = consultaCargaValePedagio.QuantidadeEixos, tipoRodagem = 1 };
                        Servicos.SemPararValePedagio.TipoVeiculo tipoVeiculo = new SemPararValePedagio.TipoVeiculo { codigo = 4 /*Caminhão*/ };

                        //consultando valores da rota recentemente cadastrada;
                        Servicos.SemPararValePedagio.ValoresPracasRota valorespracas = null;
                        Servicos.SemPararValePedagio.CustoRota custoRota = null;

                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && consultarPeloCustoDaRota)
                            custoRota = valePedagioClient.obterCustoRota(rotaTempararia, !string.IsNullOrWhiteSpace(placaPadraoConsultaValorPedagio) ? placaPadraoConsultaValorPedagio : Carga.Veiculo?.Placa ?? "", consultaCargaValePedagio.QuantidadeEixos, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1), credencial.Sessao);

                        if (custoRota == null || !custoRota.valor.HasValue || custoRota.valor.Value == 0)
                            valorespracas = valePedagioClient.obterValoresPracasRota(rota, categoria, tipoVeiculo, credencial.Sessao);

                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                        cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                        cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                        cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                        if (custoRota != null && custoRota.valor.HasValue && custoRota.valor.Value > 0)
                            cargaValePedagioIntegracaoArquivo.Mensagem = custoRota.status != 0 ? ObterMensagemRetorno(custoRota.status) : "Integrado com sucesso";
                        else
                            cargaValePedagioIntegracaoArquivo.Mensagem = valorespracas.status != 0 ? ObterMensagemRetorno(valorespracas.status) : "Integrado com sucesso";

                        cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                        consultaCargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

                        decimal valorRecebido = 0;
                        if ((custoRota != null && custoRota.status == 0 && custoRota.valor.Value > 0) || (valorespracas != null && valorespracas.status == 0))
                        {
                            if (custoRota != null && custoRota.valor.HasValue && custoRota.valor.Value > 0)
                                valorRecebido = custoRota.valor.Value;
                            else
                            {
                                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && concessionariasComDescontos != null && concessionariasComDescontos.Count() > 0 && percentualDescontoConcessionarias != null && percentualDescontoConcessionarias.Count() > 0)
                                {
                                    foreach (var praca in valorespracas.rota.pracas)
                                    {
                                        string nomeConcessionaria = praca.nomeConcessionaria.ToUpper();
                                        if (concessionariasComDescontos.Any(c => c == nomeConcessionaria))
                                        {
                                            int posicao = Array.IndexOf(concessionariasComDescontos, nomeConcessionaria);
                                            if (posicao >= 0)
                                            {
                                                decimal percentualDesconto = percentualDescontoConcessionarias[posicao].ToDecimal();
                                                if (percentualDesconto > 0)
                                                    valorRecebido += RoundUpValue((praca.tarifa.Value) / (1 - (percentualDesconto / 100)), 2);// Math.Round((praca.tarifa.Value) / (1 - (percentualDesconto / 100)), 2, MidpointRounding.ToEven);
                                                else
                                                    valorRecebido += praca.tarifa.Value;
                                            }
                                            else
                                                valorRecebido += praca.tarifa.Value;
                                        }
                                        else
                                            valorRecebido += praca.tarifa.Value;
                                    }

                                }
                                else
                                    valorRecebido = (decimal)valorespracas.rota.pracas.Sum(obj => obj.tarifa);
                            }
                        }
                        else if (valorespracas.status == 808)
                        {
                            //tratamento para quando o usuario tenta consultar uma rota sem praca de pedagio, ou seja nao tem valor pedagio.
                            consultaCargaValePedagio.ProblemaIntegracao = "Rota sem praças de vale pedágio";
                            consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            consultaCargaValePedagio.DataIntegracao = DateTime.Now;
                            consultaCargaValePedagio.ValorValePedagio = 0;
                            repConsultaValorPedagioIntegrecao.Atualizar(consultaCargaValePedagio);
                            carga.ProblemaIntegracaoValePedagio = false;

                            return 0;
                        }
                        else
                        {
                            if (custoRota != null && custoRota.valor.HasValue && custoRota.valor.Value > 0)
                                mensagem = ObterMensagemRetorno(custoRota.status);
                            else
                                mensagem = ObterMensagemRetorno(valorespracas.status);
                            consultaCargaValePedagio.ProblemaIntegracao = mensagem;
                            consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            //setar carga como possui pendencia
                            carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio. " + mensagem;
                            carga.PossuiPendencia = true;
                            carga.ProblemaIntegracaoValePedagio = true;
                            repCarga.Atualizar(carga);
                        }

                        consultaCargaValePedagio.DataIntegracao = DateTime.Now;
                        consultaCargaValePedagio.NumeroTentativas++;
                        repConsultaValorPedagioIntegrecao.Atualizar(consultaCargaValePedagio);

                        return valorRecebido;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "IntegracaoSemParar");
                        mensagem = "Falha na consulta dos valores do pedágio da rota com o Sem Parar";

                        consultaCargaValePedagio.ProblemaIntegracao = mensagem;
                        consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        //setar carga como possui pendencia
                        carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio. " + mensagem;
                        carga.PossuiPendencia = true;
                        carga.ProblemaIntegracaoValePedagio = true;
                        repCarga.Atualizar(carga);

                        consultaCargaValePedagio.DataIntegracao = DateTime.Now;
                        consultaCargaValePedagio.NumeroTentativas++;
                        repConsultaValorPedagioIntegrecao.Atualizar(consultaCargaValePedagio);
                        return 0;
                    }
                }
                else if (infoRota.status == 808)
                {
                    var jsonRequest = JsonConvert.SerializeObject(infoRota, Formatting.Indented, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });

                    Servicos.Log.TratarErro("CARGA: " + Carga.CodigoCargaEmbarcador + " com retorno status 808 " + jsonRequest.ToString());

                    //tratamento para quando o usuario tenta cadatrar uma rota sem praca de pedagio, ou seja nao tem valor pedagio.
                    consultaCargaValePedagio.ProblemaIntegracao = "Rota sem praças de vale pedágio";
                    consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    consultaCargaValePedagio.DataIntegracao = DateTime.Now;
                    consultaCargaValePedagio.NumeroTentativas++;
                    consultaCargaValePedagio.ValorValePedagio = 0;
                    repConsultaValorPedagioIntegrecao.Atualizar(consultaCargaValePedagio);
                    carga.ProblemaIntegracaoValePedagio = false;

                    return 0;
                }
                else
                {
                    mensagem = ObterMensagemRetorno(infoRota.status);

                    Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                    cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                    cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                    cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                    cargaValePedagioIntegracaoArquivo.Mensagem = mensagem;
                    cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                    consultaCargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

                    consultaCargaValePedagio.ProblemaIntegracao = mensagem;
                    consultaCargaValePedagio.NumeroTentativas++;
                    consultaCargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    //setar carga como possui pendencia
                    carga.MotivoPendencia = "Falha ao Integrar consulta valor Vale Pedágio. " + mensagem;
                    carga.PossuiPendencia = true;
                    carga.ProblemaIntegracaoValePedagio = true;
                    repCarga.Atualizar(carga);

                    return 0;
                }
            }


            //ver o q passa no else; se nao tem RotaFretePontosPassagem
            else
            {
                //Carga.IntegrandoValePedagio = false;
                //repCarga.Atualizar(Carga);
                return 0;
            }

        }

        public void GerarRotaTemporariaValePedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Piracanjuba.IntegracaoPiracanjuba servicoIntegracaoPiracanjuba = new Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware);
            Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Carga.ValePedagio.ValePedagio(unitOfWork);

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(unitOfWork);

            bool possuiIntegracaoPiracanjuba = IsPossuiIntegracaoPiracanjuba(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, tipoServicoMultisoftware);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioRota> rotas = repCargaValePedagioRota.BuscarPorCargaValePedagio(cargaValePedagio.Codigo);

            string mensagem = "";

            string rotaTempararia = Guid.NewGuid().ToString().Replace("-", "");

            if (rotas.Count > 0 && rotas.FirstOrDefault().CodigosPracaSemParar != null)
            {
                List<int> pracas = new List<int>();
                string[] splitPracas = rotas.FirstOrDefault().CodigosPracaSemParar.Split(',');
                for (int i = 0; i < splitPracas.Length; i++)
                {
                    int praca = 0;
                    int.TryParse(splitPracas[i], out praca);
                    if (praca > 0)
                        pracas.Add(praca);
                }

                SemPararValePedagio.InfoRota viagem = valePedagioClient.cadastrarRotaTemporaria(pracas.ToArray(), rotaTempararia, credencial.Sessao);

                if (viagem.status == 0)
                {
                    cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaGerada;
                    cargaValePedagio.RotaTemporaria = rotaTempararia;
                    mensagem = "Rota Gerada";
                }
                else
                {
                    mensagem = ObterMensagemRetorno(viagem.status);
                    cargaValePedagio.ProblemaIntegracao = mensagem;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (possuiIntegracaoPiracanjuba)
                        servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
                }

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                cargaValePedagioIntegracaoArquivo.Mensagem = mensagem;
                cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

            }
            else
            {
                if (integracaoSemParar?.BuscarPracasNaGeracaoDaCarga ?? false)
                {
                    string erro = string.Empty;
                    string xmlRequest = string.Empty;
                    string xmlResponse = string.Empty;
                    List<int> pracas = BuscarPracas(credencial, carga, unitOfWork, out erro, out xmlRequest, out xmlResponse);

                    if (!string.IsNullOrWhiteSpace(xmlRequest) || !string.IsNullOrWhiteSpace(xmlResponse))
                    {
                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                        cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                        cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);
                        cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                        cargaValePedagioIntegracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(erro) ? "Buscar Praças: " + erro : "Buscar Praças";
                        cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                        cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
                    }

                    if (pracas == null)
                    {
                        mensagem = !string.IsNullOrWhiteSpace(erro) ? "Buscar Praças: " + erro : "Sem parar não retornou praças de vale pedágio";
                        cargaValePedagio.ProblemaIntegracao = mensagem;
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        if (possuiIntegracaoPiracanjuba)
                            servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
                    }
                    else
                    {
                        if (pracas.Count == 0)
                        {
                            mensagem = "Rota sem praças de vale pedágio";
                            cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaSemCusto;
                            cargaValePedagio.ProblemaIntegracao = mensagem;
                            cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        }
                        else
                        {
                            SemPararValePedagio.InfoRota viagem = valePedagioClient.cadastrarRotaTemporaria(pracas.ToArray(), rotaTempararia, credencial.Sessao);

                            if (viagem.status == 0)
                            {
                                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaGerada;
                                cargaValePedagio.RotaTemporaria = rotaTempararia;
                                mensagem = "Rota Gerada";
                            }
                            else
                            {
                                mensagem = ObterMensagemRetorno(viagem.status);
                                cargaValePedagio.ProblemaIntegracao = mensagem;
                                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                                if (possuiIntegracaoPiracanjuba)
                                    servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
                            }
                        }

                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
                        cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
                        cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
                        cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
                        cargaValePedagioIntegracaoArquivo.Mensagem = mensagem;
                        cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
                        cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
                    }

                }
                else
                {
                    mensagem = "Não existe rota para geração do vale pedágio";
                    cargaValePedagio.ProblemaIntegracao = mensagem;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    if (possuiIntegracaoPiracanjuba)
                        servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
                }
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoSemParar?.NotificarTransportadorPorEmail ?? false, tipoServicoMultisoftware);
        }

        public void GerarCompraValePedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Carga.ValePedagio.ValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, tipoServicoMultisoftware);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Piracanjuba.IntegracaoPiracanjuba servicoIntegracaoPiracanjuba = new Piracanjuba.IntegracaoPiracanjuba(unitOfWork, tipoServicoMultisoftware);
            bool possuiIntegracaoPiracanjuba = IsPossuiIntegracaoPiracanjuba(unitOfWork);

            ConsultarTransportador(credencial, cargaValePedagio, integracaoSemParar, unitOfWork);

            bool eixosSuspensos = false;
            if (carga.TipoOperacao != null && carga.Rota != null)
            {
                if (carga.Rota.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida && carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    eixosSuspensos = true;
            }

            if (cargaValePedagio.CompraComEixosSuspensos)
                eixosSuspensos = true;

            int numeroEixos = 0;
            if (!integracaoSemParar.UtilizarModeoVeicularCarga)
            {
                if (carga.Veiculo?.ModeloVeicularCarga != null)
                {
                    numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;
                    if (eixosSuspensos)
                        numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                }

                if (carga.VeiculosVinculados != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                    {
                        if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                        {
                            numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                            if (eixosSuspensos)
                                numeroEixos -= reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                        }
                    }
                }
            }
            else
            {
                if (carga.ModeloVeicularCarga != null)
                {
                    numeroEixos = carga.ModeloVeicularCarga.NumeroEixos ?? 0;
                    if (eixosSuspensos)
                        numeroEixos -= carga.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                }
            }

            cargaValePedagio.QuantidadeEixos = numeroEixos;
            cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;

            int diasPrazo = integracaoSemParar.DiasPrazo > 0 ? integracaoSemParar.DiasPrazo : 5;

            string descricaoRota = cargaValePedagio.RotaTemporaria;
            if (cargaValePedagio.TipoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar.RotaFixa || cargaValePedagio.RotaFrete != null)
            {
                if (cargaValePedagio.RotaFrete != null)
                    cargaValePedagio.CodigoIntegracaoValePedagio = !cargaValePedagio.CompraComEixosSuspensos ? cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagio : !string.IsNullOrWhiteSpace(cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagioRetorno) ? cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagioRetorno : cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagio;

                if (string.IsNullOrWhiteSpace(cargaValePedagio.CodigoIntegracaoValePedagio))
                    cargaValePedagio.CodigoIntegracaoValePedagio = !cargaValePedagio.CompraComEixosSuspensos ? carga.Rota?.CodigoIntegracaoValePedagio : !string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagioRetorno) ? carga.Rota?.CodigoIntegracaoValePedagioRetorno : carga.Rota?.CodigoIntegracaoValePedagio;

                descricaoRota = cargaValePedagio.CodigoIntegracaoValePedagio;
            }

            //primeiro vamos consultar a situacao do veiculo na semparar. se status diferente de zero. problemas com o veiculo. (nao efetuará a compra)
            string mensagem = "";
            SemPararValePedagio.Viagem viagem = null;

            bool integrarComObservacao = (!string.IsNullOrWhiteSpace(integracaoSemParar.Observacao1) || !string.IsNullOrWhiteSpace(integracaoSemParar.Observacao2) || !string.IsNullOrWhiteSpace(integracaoSemParar.Observacao3) || !string.IsNullOrWhiteSpace(integracaoSemParar.Observacao4) || !string.IsNullOrWhiteSpace(integracaoSemParar.Observacao5) || !string.IsNullOrWhiteSpace(integracaoSemParar.Observacao6));

            if (!integrarComObservacao)
                viagem = valePedagioClient.comprarViagem(descricaoRota, carga.Veiculo?.Placa ?? "", numeroEixos, DateTime.Now, DateTime.Now.AddDays(diasPrazo), "", "", "", credencial.Sessao);
            else
            {
                SetarObservacoesValePedagio(ref cargaValePedagio, integracaoSemParar, unitOfWork);

                SemPararValePedagio.Observacoes observacoes = new SemPararValePedagio.Observacoes();
                if (!string.IsNullOrWhiteSpace(cargaValePedagio.Observacao1))
                    observacoes.observacao1 = Utilidades.String.Left(cargaValePedagio.Observacao1, 40);
                if (!string.IsNullOrWhiteSpace(cargaValePedagio.Observacao2))
                    observacoes.observacao2 = Utilidades.String.Left(cargaValePedagio.Observacao2, 40);
                if (!string.IsNullOrWhiteSpace(cargaValePedagio.Observacao3))
                    observacoes.observacao3 = Utilidades.String.Left(cargaValePedagio.Observacao3, 40);
                if (!string.IsNullOrWhiteSpace(cargaValePedagio.Observacao4))
                    observacoes.observacao4 = Utilidades.String.Left(cargaValePedagio.Observacao4, 40);
                if (!string.IsNullOrWhiteSpace(cargaValePedagio.Observacao5))
                    observacoes.observacao5 = Utilidades.String.Left(cargaValePedagio.Observacao5, 40);
                if (!string.IsNullOrWhiteSpace(cargaValePedagio.Observacao6))
                    observacoes.observacao6 = Utilidades.String.Left(cargaValePedagio.Observacao6, 40);

                viagem = valePedagioClient.comprarViagemComObservacoes(descricaoRota, carga.Veiculo?.Placa ?? "", numeroEixos, DateTime.Now, DateTime.Now.AddDays(diasPrazo), observacoes, credencial.Sessao);
            }

            if (viagem.status == 0)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                cargaValePedagio.NumeroValePedagio = viagem.numero.ToString();
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada;
                mensagem = "Ag Recibo";
            }
            else
            {
                mensagem = ObterMensagemRetorno(viagem.status);

                cargaValePedagio.ProblemaIntegracao = mensagem;
                if (mensagem.Contains("Prazo inválido") && cargaValePedagio.NumeroTentativas < 4)
                {
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Pendete;
                }
                else
                {
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Pendete;
                }

                if (possuiIntegracaoPiracanjuba)
                    servicoIntegracaoPiracanjuba.IntegrarFalhaDeValePedagio(cargaValePedagio);
            }


            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
            cargaValePedagioIntegracaoArquivo.Mensagem = mensagem;
            cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoSemParar.NotificarTransportadorPorEmail, tipoServicoMultisoftware);
        }

        public void ConsultarIdVpo(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, tipoServicoMultisoftware);

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                jsonRequisicao = $"{integracaoSemParar.UrlIntegracaoRest}/v1/integracao-antt/vpo/numero-viagem/{cargaValePedagio.NumeroValePedagio}";
                System.Net.Http.HttpClient requisicao = CriarRequisicao(integracaoSemParar.UrlIntegracaoRest, credencial);

                System.Net.Http.HttpResponseMessage retornoRequisicao = requisicao.GetAsync(jsonRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    RetornoErroValePedagio retornoErro = JsonConvert.DeserializeObject<RetornoErroValePedagio>(jsonRetorno);

                    throw new ServicoException(retornoErro.Erro?.Descricao ?? $"Erro ao consultar ID VPO. StatusCode: {(int)retornoRequisicao.StatusCode}");
                }

                if (!retornoRequisicao.IsSuccessStatusCode || retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Erro ao consultar ID VPO. StatusCode: {(int)retornoRequisicao.StatusCode}");

                RetornoConsultaIdVpo idsVpos = JsonConvert.DeserializeObject<RetornoConsultaIdVpo>(jsonRetorno);

                cargaValePedagio.ProblemaIntegracao = "Consulta do ID VPO realizada com sucesso!";
                cargaValePedagio.CodigoEmissaoValePedagioANTT = idsVpos.Dados.CodigosVpos?.FirstOrDefault() ?? string.Empty;
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a consulta do ID VPO do vale pedágio";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public void ConsultarTransportador(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = "Sem Retorno";

            try
            {
                Dominio.Entidades.Veiculo veiculo = cargaValePedagio.Carga.Veiculo;

                dynamic requisicaoConsultarTransportador = new
                {
                    placa = veiculo.Placa,
                    cpfCnpjTransportador = (veiculo.Tipo == "T" && veiculo.Proprietario != null) ? veiculo.Proprietario.CPF_CNPJ_SemFormato : cargaValePedagio.Carga.Empresa?.CNPJ_SemFormato
                };

                jsonRequisicao = JsonConvert.SerializeObject(requisicaoConsultarTransportador, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                System.Net.Http.StringContent conteudoRequisicao = new System.Net.Http.StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                string urlBase = $"{integracaoSemParar.UrlIntegracaoRest}/v1/integracao-rntrc-embarcador";
                System.Net.Http.HttpClient requisicao = CriarRequisicao(urlBase, credencial);

                System.Net.Http.HttpResponseMessage retornoRequisicao = requisicao.PostAsync(urlBase, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a consulta de rntrc-embarcador do vale pedágio";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public void SolicitarCancelamentoValePedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            int status = valePedagioClient.cancelarViagem(long.Parse(cargaValePedagio.NumeroValePedagio), credencial.Sessao);
            unitOfWork.Start();
            string mensagem = "";
            if (status == 0)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada;
                mensagem = "Vale Pedágio Cancelado com Sucesso";
            }
            else
            {
                mensagem = ObterMensagemRetorno(status);
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada;
            }

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();
            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unitOfWork);
            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unitOfWork);
            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
            cargaValePedagioIntegracaoArquivo.Mensagem = mensagem;
            cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ProblemaIntegracao = mensagem;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            //cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);
            unitOfWork.CommitChanges();
        }

        public byte[] GerarImpressaoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            return ReportRequest.WithType(ReportType.ValePedagioSemparar)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoCargaValePedagio", cargaValePedagio.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public SemPararValePedagio.Recibo ObterReciboViagem(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, out string request, out string response, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            SemPararValePedagio.Recibo recibo = valePedagioClient.obterReciboViagem(!string.IsNullOrWhiteSpace(cargaValePedagio.NumeroValePedagio) ? long.Parse(cargaValePedagio.NumeroValePedagio) : 0, credencial.Sessao);

            request = inspector.LastRequestXML;
            response = inspector.LastResponseXML;

            return recibo;
        }

        public void ObterReciboCompraValePedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Carga.ValePedagio.ValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaValePedagio repCargaValePedagioParaMDFe = new Repositorio.Embarcador.Cargas.CargaValePedagio(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemParar(cargaValePedagio.Carga, tipoServicoMultisoftware);

            SemPararValePedagio.Recibo recibo = ObterReciboViagem(credencial, cargaValePedagio, out string request, out string response, unitOfWork);

            unitOfWork.Start();

            string mensagem = "";
            if (recibo.status == 2 || recibo.status == 0)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaValePedagio.ValorValePedagio = recibo.total.HasValue ? recibo.total.Value : 0;
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada;
                mensagem = "1 - Recibo obtido com Sucesso";
            }
            else if (recibo.status == 7 || recibo.status == 15)
            {
                if (recibo.status == 15)
                    mensagem = $"15 - {ObterMensagemRetorno(recibo.status)}";
                else
                    mensagem = "7 - Compra recusada";
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Recusada;
                cargaValePedagio.ProblemaIntegracao = mensagem;
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                mensagem = "Ag retorno sem parar";
            }

            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(request, "xml", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(response, "xml", unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };
            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
            cargaValePedagio.ProblemaIntegracao = mensagem;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            //cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);

            if (cargaValePedagio.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado)
            {
                if (integracaoSemParar.FornecedorValePedagio != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaValePedagio cargaValePedagioParaMDFe = repCargaValePedagioParaMDFe.BuscarPorCodigo(cargaValePedagio.Carga.Codigo);

                    if (cargaValePedagioParaMDFe == null)
                        cargaValePedagioParaMDFe = new Dominio.Entidades.Embarcador.Cargas.CargaValePedagio();

                    cargaValePedagioParaMDFe.Carga = cargaValePedagio.Carga;
                    cargaValePedagioParaMDFe.Fornecedor = integracaoSemParar.FornecedorValePedagio;
                    cargaValePedagioParaMDFe.NumeroComprovante = cargaValePedagio.NumeroValePedagio;
                    cargaValePedagioParaMDFe.Valor = cargaValePedagio.ValorValePedagio;
                    cargaValePedagioParaMDFe.Responsavel = cargaValePedagio.Carga.Pedidos.FirstOrDefault().Pedido.ObterTomador();
                    cargaValePedagioParaMDFe.QuantidadeEixos = cargaValePedagio.QuantidadeEixos;
                    cargaValePedagioParaMDFe.CargaIntegracaoValePedagio = cargaValePedagio;
                    cargaValePedagioParaMDFe.TipoCompra = cargaValePedagio.TipoCompra;

                    if (cargaValePedagioParaMDFe.Responsavel == null)
                        cargaValePedagioParaMDFe.Responsavel = cargaValePedagio.Carga.Pedidos.FirstOrDefault().Pedido.Remetente;

                    if (cargaValePedagioParaMDFe.Codigo > 0)
                        repCargaValePedagioParaMDFe.Atualizar(cargaValePedagioParaMDFe);
                    else
                        repCargaValePedagioParaMDFe.Inserir(cargaValePedagioParaMDFe);
                }

                Servicos.Embarcador.Terceiros.ContratoFrete.AdicionarValePedagioContratoFrete(cargaValePedagio, unitOfWork, tipoServicoMultisoftware);
            }

            unitOfWork.CommitChanges();

            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoSemParar.NotificarTransportadorPorEmail, tipoServicoMultisoftware);
        }

        public static string ObterMensagemRetorno(int status)
        {
            string mensagem = "";

            if (status == 1)
                mensagem = "CNPJ, login ou senha inválidos";
            else if (status == 3)
                mensagem = "Sessão expirada ou inválida (Timeout da sessão ultrapassado ou código de sessão inválido)";
            else if (status == 4)
                mensagem = "Veículo não disponível (Veículo não encontrado no sistema ou com restrições)";
            else if (status == 5)
                mensagem = "Placa inválida";
            else if (status == 7)
                mensagem = "Veículo com múltiplos Tags (Veículo possui mais de um Tag ativo)";
            else if (status == 8)
                mensagem = "Viagem não encontrada (Numero de Vale Pedágio não encontrado no sistema)";
            else if (status == 9)
                mensagem = "Usuário sem permissão a este serviço (Usuário precisa estar cadastrado para acessar a funcionalidade)";
            else if (status == 10)
                mensagem = "Prazo inválido (Data vazia ou valor de início maior que final)";
            else if (status == 11)
                mensagem = "Prazo máximo extrapolado (Prazo máximo para extratos é de 90 dias.Para vigência de ValePedágio, 15 dias.)";
            else if (status == 12)
                mensagem = "Rota Inválida (Rota não encontrada no sistema para este embarcador)";
            else if (status == 13)
                mensagem = "Número de eixos inválido (Numero deve ser entre 2 a 10.E nos serviços de obter valores praças deve ser entre 2 a 15.)";
            else if (status == 14)
                mensagem = "Saldo insuficiente (Crédito disponível na conta menor que o valor da viagem a ser comprada.)";
            else if (status == 15)
                mensagem = "Recibo não disponível (O recibo do vale pedágio só é retornado caso a viagem não tenha sido cancelada ou encerrada.)";
            else if (status == 49)
                mensagem = "Viagem não pode ser cancelada (Viagem confirmada (impressa) a mais de 3 hrs ou com alguma passagem já reconhecida não pode mais ser cancelada.)";
            else if (status == 51)
                mensagem = "Praça(s) inválida(s) (Para reemissão (transferência) de viagem. Lista de praças deve ter o formato 99 - 99 - 99 -...)";
            else if (status == 52)
                mensagem = "Não foi encontrado o transportador (Transportador não foi encontrado no sistema)";
            else if (status == 53)
                mensagem = "Viagem não pode ser reemitida (Viagem expirada ou viagem que ainda não tenha sido confirmada(impressa) não pode ser reemitida.)";
            else if (status == 54)
                mensagem = "Viagem parcialmente reemitida (Houve alguma(s) praça(s) de pedágio selecionada(s) que não pode(puderam) ser reemitida(s))";
            else if (status == 55)
                mensagem = "Viagem não pode ser nula (Valor da viagem deve ser maior que R$ 0,00)";
            else if (status == 58)
                mensagem = "Nome de rota já existente (Usuário tentou cadastrar uma rota com um nome já existente)";
            else if (status == 59)
                mensagem = "Rota inexistente (Usuário tentou pesquisar uma rota que não existe)";
            else if (status == 62)
                mensagem = "Mais de um resultado encontrado... (Era esperado no máximo um valor de retorno, mas foi encontrado mais de um.)";
            else if (status == 66)
                mensagem = "Praça não encontrada (Usuário tentou cadastrar uma rota e uma ou mais praças não foram encontradas ou não são atendidas.)";
            else if (status == 77)
                mensagem = "Tipo de rodagem indisponível (Tipos de rodagem existentes são: 0 – Simples 1 – Dupla)";
            else if (status == 79)
                mensagem = "Praça incompatível com a tecnologia do equipamento do veículo (Usuário tentou cadastrar uma ou mais praças cujo equipamento do veículo não é compatível com o da praça.)";
            else if (status == 83)
                mensagem = "Compra de viagem não processada (Usuário tentou efetuar a compra de uma viagem que por algum motivo não ocorreu (ausência / insuficiência de saldo, parâmetros inválidos, etc..))";
            else if (status == 84)
                mensagem = "Pontos de Parada Inválido, alguns dos pontos de paradas estão com código inválido. (Esse status será utilizado tanto com código do IBGE quanto lat / long)";
            else if (status == 85)
                mensagem = "Pontos não possui ligação (Utilizado quando algum dos pontos não possui ligação com os outros)";
            else if (status == 411)
                mensagem = "Preenchimento do parâmetro observacao1 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido)";
            else if (status == 412)
                mensagem = "Preenchimento do parâmetro observacao2 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 413)
                mensagem = "Preenchimento do parâmetro observacao3 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 414)
                mensagem = "Preenchimento do parâmetro observacao4 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 415)
                mensagem = "Preenchimento do parâmetro observacao5 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 416)
                mensagem = "Preenchimento do parâmetro observacao6 obrigatório no serviço comprarViagemComObservacoes (Quando o parâmetro observação está configurado como obrigatório e não é preenchido.)";
            else if (status == 421)
                mensagem = "Texto informado no parâmetro observacao1 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 422)
                mensagem = "Texto informado no parâmetro observacao2 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 423)
                mensagem = "Texto informado no parâmetro observacao3 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 424)
                mensagem = "Texto informado no parâmetro observacao4 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 425)
                mensagem = "Texto informado no parâmetro observacao5 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 426)
                mensagem = "Texto informado no parâmetro observacao6 deverá ser único. (Quando texto enviado como observação já foi utilizado anteriormente em outra viagem com vigência igual ou superior a data em que o parâmetro foi configurado como único.)";
            else if (status == 431)
                mensagem = "Texto informado no parâmetro observacao1 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 432)
                mensagem = "Texto informado no parâmetro observacao2 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 433)
                mensagem = "Texto informado no parâmetro observacao3 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 434)
                mensagem = "Texto informado no parâmetro observacao4 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 435)
                mensagem = "Texto informado no parâmetro observacao5 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 436)
                mensagem = "Texto informado no parâmetro observacao6 deverá ser numérico. (Quando o texto enviado como observação contém caracteres que não sejam numéricos.)";
            else if (status == 451)
                mensagem = "Texto informado no parâmetro observacao1 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 452)
                mensagem = "Texto informado no parâmetro observacao2 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 453)
                mensagem = "Texto informado no parâmetro observacao3 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 454)
                mensagem = "Texto informado no parâmetro observacao4 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 455)
                mensagem = "Texto informado no parâmetro observacao5 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 456)
                mensagem = "Texto informado no parâmetro observacao6 excede o limite de 40 caracteres. (Quando o texto enviado como observação contém mais do que 40 caracteres.)";
            else if (status == 461)
                mensagem = "Parâmetro observacao1 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 462)
                mensagem = "Parâmetro observacao2 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 463)
                mensagem = "Parâmetro observacao3 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 464)
                mensagem = "Parâmetro observacao4 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 465)
                mensagem = "Parâmetro observacao5 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 466)
                mensagem = "Parâmetro observacao6 não está configurado no site. (Quando o parâmetro enviado não está configurado no site.)";
            else if (status == 808)
                mensagem = "Cadastrar rotas sem praça (Quando o usuário tenta cadastrar uma rota que não tem praças de pedágio ou as mesmas não são atendidas.)";
            else if (status == 999)
                mensagem = "Erro no serviço (Erro diverso que não foi previsto) - Retorno do Sem Parar";
            else
                mensagem = "Não foi possível efetur a comprar no Sem Parar (Codigo de Retorno Sem Parar " + status.ToString() + ")";

            return mensagem.Length <= 400 ? mensagem : mensagem.Substring(0, 399);
        }

        public string CaminhoLogoValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao", "LogoSemParar");

            return caminho;
        }

        public static string ConsultarRegistrarTarifasPracasPedagio(Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string erro, xmlRequest, xmlResponse;
            int totalPracasPedagioSemParar = 0, totalTotalTarifasPracasPedagio = 0, totalTotalTarifasPracasPedagioAtualizadas = 0;

            Servicos.Embarcador.Integracao.SemParar.ValePedagio serValepedagioSemParar = new Servicos.Embarcador.Integracao.SemParar.ValePedagio();
            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = serValepedagioSemParar.Autenticar(unitOfWork, tipoServicoMultisoftware, out erro, out xmlRequest, out xmlResponse);
            AdicionarArquivosIntegracao("Autenticar " + erro, xmlRequest, xmlResponse, pracaPedagioTarifaIntegracao, unitOfWork);

            if (credencial.Autenticado)
            {
                Servicos.Embarcador.Integracao.SemParar.PracasPedagio svcPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();

                // Consulta os modelos veiculares de carga
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesCarga = repModeloVeicularCarga.BuscarTodosAtivos();

                // Extrai as quantidades de eixos distintos para otimizar as requisições
                List<int> quantidadesEixos = (from modeloVeicularCarga in modelosVeicularesCarga where modeloVeicularCarga.NumeroEixos > 1 select modeloVeicularCarga.NumeroEixos.Value).Distinct().ToList();
                quantidadesEixos.Sort();
                int total = quantidadesEixos.Count;
                if (total > 0)
                {

                    // Consulta todas as praças de pedágio já cadatradas
                    Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                    Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = repPracaPedagio.BuscarTodosAtivas();

                    // Consulta as praças do Sem Parar
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagioSemParar = svcPracasPedagio.ListaPracasPedagio(credencial, out erro, out xmlRequest, out xmlResponse, unitOfWork);
                    AdicionarArquivosIntegracao("ListaPracasPedagio " + erro, xmlRequest, xmlResponse, pracaPedagioTarifaIntegracao, unitOfWork);

                    totalPracasPedagioSemParar = pracasPedagioSemParar.Count();
                    for (int i = 0; i < totalPracasPedagioSemParar; i++)
                    {
                        // Se a praça de pedágio ainda não está cadastrada, cadastra
                        Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = (from obj in pracasPedagio where obj.CodigoIntegracao == pracasPedagioSemParar[i].Id.ToString() select obj).FirstOrDefault();
                        if (pracaPedagio == null)
                        {
                            pracaPedagio = new Dominio.Entidades.Embarcador.Logistica.PracaPedagio
                            {
                                CodigoIntegracao = pracasPedagioSemParar[i].Id.ToString(),
                                Descricao = pracasPedagioSemParar[i].Praca,
                                Concessionaria = pracasPedagioSemParar[i].Concessionaria,
                                Rodovia = pracasPedagioSemParar[i].Rodovia,
                                KM = pracasPedagioSemParar[i].KM,
                                Ativo = true
                            };
                            repPracaPedagio.Inserir(pracaPedagio);
                        }
                        else
                        {
                            pracaPedagio.Descricao = pracasPedagioSemParar[i].Praca;
                            pracaPedagio.Concessionaria = pracasPedagioSemParar[i].Concessionaria;
                            pracaPedagio.Rodovia = pracasPedagioSemParar[i].Rodovia;
                            pracaPedagio.KM = pracasPedagioSemParar[i].KM;
                            repPracaPedagio.Atualizar(pracaPedagio);
                        }
                    }
                    unitOfWork.FlushAndClear();

                    // Reconsulta todas as praças de pedágio cadastradas
                    pracasPedagio = repPracaPedagio.BuscarTodosAtivas();

                    // Faz uma consulta dos valores para cada quantidade de eixo
                    for (int i = 0; i < total; i++)
                    {

                        // Conusulta o valor da tarifa para a quantidade de eixos
                        List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.TarifaPracaPedagio> tarifasPracasPedagio = svcPracasPedagio.ObterValoresPracas(credencial, quantidadesEixos[i], out erro, out xmlRequest, out xmlResponse, unitOfWork);
                        AdicionarArquivosIntegracao("ObterValoresPracas " + quantidadesEixos[i] + " eixos " + erro, xmlRequest, xmlResponse, pracaPedagioTarifaIntegracao, unitOfWork);

                        int totalTarifasPracasPedagio = tarifasPracasPedagio.Count;
                        totalTotalTarifasPracasPedagio += totalTarifasPracasPedagio;
                        DateTime dataAtual = DateTime.Now;
                        if (totalTarifasPracasPedagio > 0)
                        {

                            // Identifica os modelos que possuem a quantidade de eixos
                            List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga> modelosVeicularesCargaEixos = (from modeloVeicularCarga in modelosVeicularesCarga where modeloVeicularCarga.NumeroEixos == quantidadesEixos[i] select modeloVeicularCarga).ToList();
                            int totalModelosVeicularesCargaEixos = modelosVeicularesCargaEixos.Count();

                            // Atualiza/insere as tarifas para todos os modelos veiculadas para as praças
                            for (int j = 0; j < totalTarifasPracasPedagio; j++)
                            {
                                // Identifica a praça de pedágio pelo nome da praça
                                Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = (from obj in pracasPedagio where obj.Descricao == tarifasPracasPedagio[j].NomePraca select obj).FirstOrDefault();
                                if (pracaPedagio != null)
                                {
                                    for (int k = 0; k < totalModelosVeicularesCargaEixos; k++)
                                    {
                                        Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa pracaPedagioTarifa = repPracaPedagioTarifa.BuscarPorPracaPedagioModeloVeicularCarga(pracaPedagio.Codigo, modelosVeicularesCargaEixos[k].Codigo);
                                        if (pracaPedagioTarifa == null)
                                        {
                                            pracaPedagioTarifa = new Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa
                                            {
                                                Data = dataAtual,
                                                ModeloVeicularCarga = modelosVeicularesCargaEixos[k],
                                                PracaPedagio = pracaPedagio,
                                                Tarifa = tarifasPracasPedagio[j].Tarifa
                                            };
                                            repPracaPedagioTarifa.Inserir(pracaPedagioTarifa);
                                        }
                                        else
                                        {
                                            pracaPedagioTarifa.Data = dataAtual;
                                            pracaPedagioTarifa.Tarifa = tarifasPracasPedagio[j].Tarifa;
                                            repPracaPedagioTarifa.Atualizar(pracaPedagioTarifa);
                                        }
                                        totalTotalTarifasPracasPedagioAtualizadas++;
                                    }
                                    unitOfWork.FlushAndClear();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception((!string.IsNullOrWhiteSpace(erro)) ? erro : "Falha na integração");
            }
            return $"{totalPracasPedagioSemParar} praças consultadas, {totalTotalTarifasPracasPedagio} tarifas consultadas, {totalTotalTarifasPracasPedagioAtualizadas} tarifas de modelo veicular atualizadas.";
        }

        public void EnviarEmailTransportadorConsultaValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar, Repositorio.UnitOfWork unidadeTrabalho)
        {
            if (!(integracaoSemParar?.NotificarTransportadorPorEmail ?? false))
                return;

            if (cargaConsultaValePedagio.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                return;

            StringBuilder mensagem = new StringBuilder()
                .AppendLine($"Olá ({cargaConsultaValePedagio.Carga.Empresa.CNPJ_Formatado}) {cargaConsultaValePedagio.Carga.Empresa.Descricao},")
                .AppendLine()
                .AppendLine($"A carga {cargaConsultaValePedagio.Carga.CodigoCargaEmbarcador} possuí veículo alocado com possíveis problemas no SemParar.")
                .AppendLine($"Resposta SemParar: {cargaConsultaValePedagio.ProblemaIntegracao}.");

            Servicos.Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Servicos.Embarcador.Notificacao.NotificacaoEmpresa(unidadeTrabalho);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
            {
                AssuntoEmail = $"Retorno de Consulta de Vale Pedágio: Veículo {cargaConsultaValePedagio.Carga.Veiculo?.Placa} - {cargaConsultaValePedagio.Carga.Empresa.NomeCNPJ}",
                Empresa = cargaConsultaValePedagio.Carga.Empresa,
                Mensagem = mensagem.ToString(),
                NotificarSomenteEmailPrincipal = true
            };

            servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmpresa);
        }

        public void ConsultarExtratoValePedagio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao repositorioExtratoCreditoValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao(unitOfWork);

            Frota.ValePedagio svcValePedagio = new Frota.ValePedagio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = svcValePedagio.ObterIntegracaoSemPararParaAutenticacao(tipoServicoMultisoftware);

            if (integracaoSemParar == null || !integracaoSemParar.ConsultarExtrato)
                return;

            string erro, xmlRequest, xmlResponse;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.RetornoObterExtratoCredito> retornoExtratosCreditos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.RetornoObterExtratoCredito>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = Autenticar(unitOfWork, tipoServicoMultisoftware, out erro, out xmlRequest, out xmlResponse);
            Servicos.Log.TratarErro($"Atenticação: \n {xmlRequest} \n {xmlResponse} \n {erro}", "ConsultaExtratoValePedagio");

            xmlRequest = string.Empty;
            xmlResponse = string.Empty;

            if (credencial.Autenticado)
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                InspectorBehavior inspector = new InspectorBehavior();

                string url = @"https://app.viafacil.com.br/vpextrato/ValePedagio";
#if DEBUG
                url = @"https://apphom.viafacil.com.br/wsvp/ValePedagio";
#endif

                System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                SemPararValePedagio.ValePedagioClient valePedagioClient = new SemPararValePedagio.ValePedagioClient(binding, endpointAddress);
                valePedagioClient.Endpoint.EndpointBehaviors.Add(inspector);

                DateTime dataAtual = DateTime.Now;
                SemPararValePedagio.ExtratoCredito[] extratosCredito = valePedagioClient.obterExtratoCreditos(dataAtual.AddDays(-integracaoSemParar.QuantidadeDiasConsultarExtrato), dataAtual, credencial.Sessao);

                xmlRequest = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;

                Servicos.Log.TratarErro($"ObterExtratoCreditos: \n {xmlRequest} \n {xmlResponse} ", "ConsultaExtratoValePedagio");

                int total = extratosCredito?.Length ?? 0;
                for (int i = 0; i < total; i++)
                {
                    if (extratosCredito[i].status == 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao extratoCreditoValePedagioIntegracao = repositorioExtratoCreditoValePedagio.BuscarExtratoExistente(extratosCredito[i].numeroViagem, extratosCredito[i].dataCompra, extratosCredito[i].dataOperacao, extratosCredito[i].acao, extratosCredito[i].nomePraca, extratosCredito[i].placa);

                        if (extratoCreditoValePedagioIntegracao != null)
                        {
                            if (extratoCreditoValePedagioIntegracao.DataPassagem != extratosCredito[i].dataPassagem || extratoCreditoValePedagioIntegracao.Fatura != extratosCredito[i].fatura || extratoCreditoValePedagioIntegracao.DataFatura != extratosCredito[i].dataFatura)
                            {
                                extratoCreditoValePedagioIntegracao.SituacaoProcessamentoExtratoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.AguardandoProcessamento;
                                extratoCreditoValePedagioIntegracao.DataPassagem = extratosCredito[i].dataPassagem;
                                extratoCreditoValePedagioIntegracao.Fatura = extratosCredito[i].fatura;
                                extratoCreditoValePedagioIntegracao.DataFatura = extratosCredito[i].dataFatura;
                                extratoCreditoValePedagioIntegracao.DataAtualizacao = DateTime.Now;
                            }
                        }
                        else
                        {
                            extratoCreditoValePedagioIntegracao = new Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao()
                            {
                                Numero = extratosCredito[i].numero,
                                DataOperacao = extratosCredito[i].dataOperacao,
                                DataCompra = extratosCredito[i].dataCompra,
                                Acao = extratosCredito[i].acao,
                                Descricao = extratosCredito[i].descricao,
                                ValorOperacao = extratosCredito[i].valorOperacao,
                                NumeroViagem = extratosCredito[i].numeroViagem,
                                DataInicioVigencia = extratosCredito[i].dataInicioVigencia,
                                DataFimVigencia = extratosCredito[i].dataFimVigencia,
                                DataPassagem = extratosCredito[i].dataPassagem,
                                CNPJCPFTransp = extratosCredito[i].cnpjCpfTransp,
                                NomeTransp = extratosCredito[i].nomeTransp,
                                Tag = extratosCredito[i].tag,
                                Placa = extratosCredito[i].placa,
                                NomeRota = extratosCredito[i].nomeRota,
                                ItemFinanceiro1 = extratosCredito[i].itemFinanceiro1,
                                ItemFinanceiro2 = extratosCredito[i].itemFinanceiro2,
                                ItemFinanceiro3 = extratosCredito[i].itemFinanceiro3,
                                SaldoDia = extratosCredito[i].saldoDia,
                                NomePraca = extratosCredito[i].nomePraca,
                                NomeRodovia = extratosCredito[i].nomeRodovia,
                                NomeConcessionaria = extratosCredito[i].nomeConcessionaria,
                                Fatura = extratosCredito[i].fatura,
                                DataFatura = extratosCredito[i].dataFatura,
                                TipoVP = extratosCredito[i].tipoVP,
                                Status = extratosCredito[i].status,
                                SituacaoProcessamentoExtratoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.AguardandoProcessamento,
                                DataCriacao = DateTime.Now,
                            };
                        }

                        if (extratoCreditoValePedagioIntegracao.Codigo > 0)
                            repositorioExtratoCreditoValePedagio.Atualizar(extratoCreditoValePedagioIntegracao);
                        else
                            repositorioExtratoCreditoValePedagio.Inserir(extratoCreditoValePedagioIntegracao);
                    }
                    else
                        Servicos.Log.TratarErro($"ObterExtratoCreditos: \n Retornou código de status diferente de 0 ({extratosCredito[i].status})", "ConsultaExtratoValePedagio");
                }
            }

        }

        public void ProcessarExtratosCreditoValePedagio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao repositorioExtratoCreditoValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio repositorioExtratoValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio(unitOfWork);

            Frota.ValePedagio svcValePedagio = new Frota.ValePedagio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = svcValePedagio.ObterIntegracaoSemPararParaAutenticacao(tipoServicoMultisoftware);

            if (integracaoSemParar == null || !integracaoSemParar.ConsultarExtrato)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao> extratos = repositorioExtratoCreditoValePedagio.BuscarExtratosPendentesProcessamento();

            foreach (Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao extrato in extratos)
            {
                Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio extratoValePedagio = repositorioExtratoValePedagio.BuscarPorNumeroViagem(extrato.NumeroViagem);

                if (extratoValePedagio != null)
                {

                    if (extrato.Acao.ToUpper() == "CREDITO")
                        extratoValePedagio.TemCredito = true;

                    if (string.IsNullOrWhiteSpace(extratoValePedagio.Fatura) && !string.IsNullOrWhiteSpace(extrato.Fatura))
                        extratoValePedagio.Fatura = extrato.Fatura;

                    if ((!extratoValePedagio.DataFatura.HasValue || extratoValePedagio.DataFatura.Value == DateTime.MinValue) && extrato.DataFatura.HasValue)
                        extratoValePedagio.DataFatura = extrato.DataFatura;

                    if (extratoValePedagio.Extratos == null || extratoValePedagio.Extratos.Count == 0 || !extratoValePedagio.Extratos.Any(o => o.Codigo == extrato.Codigo))
                    {
                        if (extratoValePedagio.Extratos == null)
                            extratoValePedagio.Extratos = new List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao>() { extrato };
                        else
                            extratoValePedagio.Extratos.Add(extrato);
                    }

                    extratoValePedagio.SituacaoExtrato = RetornarSituacaoExtratoValePedagio(extratoValePedagio);
                }
                else
                {
                    extratoValePedagio = new Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio()
                    {
                        NumeroViagem = extrato.NumeroViagem,
                        Fatura = !string.IsNullOrWhiteSpace(extrato.Fatura) ? extrato.Fatura : string.Empty,
                        DataFatura = extrato.DataFatura,
                        TemCredito = extrato.Acao.ToUpper() == "CREDITO",
                        SituacaoExtrato = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio.SemValePedagio,
                        DataCriacao = DateTime.Now,
                        SituacaoProcessamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.AguardandoProcessamento,
                        Extratos = new List<Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratoCreditoValePedagioIntegracao>() { extrato }
                    };
                }

                extrato.SituacaoProcessamentoExtratoCredito = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.Processado;
                repositorioExtratoCreditoValePedagio.Atualizar(extrato);

                if (extratoValePedagio.Codigo > 0)
                    repositorioExtratoValePedagio.Atualizar(extratoValePedagio);
                else
                    repositorioExtratoValePedagio.Inserir(extratoValePedagio);
            }
        }

        public void ProcessarExtratoValePedagioPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<int> idsValePedagio = null)
        {
            Frota.ValePedagio svcValePedagio = new Frota.ValePedagio(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = svcValePedagio.ObterIntegracaoSemPararParaAutenticacao(tipoServicoMultisoftware);

            if (integracaoSemParar == null || !integracaoSemParar.ConsultarExtrato)
                return;

            Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio repositorioExtratoValePedagio = new Repositorio.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracao = repositorioTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);
            if (integracao == null)
                return;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> listaValePedagio;

            if (idsValePedagio == null)
                listaValePedagio = repositorioCargaIntegracaoValePedagio.BuscarValePedagioPorTipoIntegracao(integracao.Codigo, 100);
            else
                listaValePedagio = repositorioCargaIntegracaoValePedagio.BuscarPorCodigos(idsValePedagio);


            foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio in listaValePedagio)
            {
                Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio extratoValePedagio = repositorioExtratoValePedagio.BuscarPorNumeroViagem(valePedagio.NumeroValePedagio.ObterSomenteNumeros().ToLong());

                if (extratoValePedagio != null)
                {
                    extratoValePedagio.ValePedagio = valePedagio;
                    extratoValePedagio.SituacaoProcessamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.Processado;
                    extratoValePedagio.DataAtualizacao = DateTime.Now;
                    extratoValePedagio.DataProcessamento = DateTime.Now;

                    extratoValePedagio.SituacaoExtrato = RetornarSituacaoExtratoValePedagio(extratoValePedagio);
                }
                else
                {
                    extratoValePedagio = new Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio()
                    {
                        NumeroViagem = valePedagio.NumeroValePedagio.ObterSomenteNumeros().ToLong(),
                        ValePedagio = valePedagio,
                        SituacaoExtrato = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio.SemExtrato,
                        DataCriacao = DateTime.Now,
                        SituacaoProcessamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio.Processado,
                        DataProcessamento = DateTime.Now
                    };
                }

                if (extratoValePedagio.Codigo > 0)
                    repositorioExtratoValePedagio.Atualizar(extratoValePedagio);
                else
                    repositorioExtratoValePedagio.Inserir(extratoValePedagio);
            }
        }

        #endregion

        #region Métodos Privados

        private void SetarObservacoesValePedagio(ref Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

            cargaValePedagio.Observacao1 = AjustarObservacao(integracaoSemParar.Observacao1, cargaValePedagio.Carga, unitOfWork).Left(200);
            cargaValePedagio.Observacao2 = AjustarObservacao(integracaoSemParar.Observacao2, cargaValePedagio.Carga, unitOfWork).Left(200);
            cargaValePedagio.Observacao3 = AjustarObservacao(integracaoSemParar.Observacao3, cargaValePedagio.Carga, unitOfWork).Left(200);
            cargaValePedagio.Observacao4 = AjustarObservacao(integracaoSemParar.Observacao4, cargaValePedagio.Carga, unitOfWork).Left(200);
            cargaValePedagio.Observacao5 = AjustarObservacao(integracaoSemParar.Observacao5, cargaValePedagio.Carga, unitOfWork).Left(200);
            cargaValePedagio.Observacao6 = AjustarObservacao(integracaoSemParar.Observacao6, cargaValePedagio.Carga, unitOfWork).Left(200);

            repCargaIntegracaoValePedagio.Atualizar(cargaValePedagio);
        }

        private string AjustarObservacao(string observacao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            if (string.IsNullOrWhiteSpace(observacao))
                return string.Empty;

            int numeroNotaFiscal = 0;
            int numeroCTe = 0;
            string notas = observacao.Contains("#NumeroNotaFiscal") ? string.Join(" / ", repPedidoXMLNotaFiscal.ObterNumerosNotasPorCarga(carga.Codigo)) : string.Empty;

            if (observacao.Contains("#NumeroCTe") || observacao.Contains("#NumeroNFe"))
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(carga.Codigo);
                if (cargaCTes != null && cargaCTes.Count > 0)
                {
                    numeroNotaFiscal = cargaCTes.FirstOrDefault().NotasFiscais?.FirstOrDefault().PedidoXMLNotaFiscal?.XMLNotaFiscal.Numero ?? 0;
                    numeroCTe = cargaCTes.FirstOrDefault().CTe?.Numero ?? 0;
                }
            }

            return observacao.Replace("#NumeroCarga", carga.CodigoCargaEmbarcador).
                              Replace("#NumeroCTe", numeroCTe > 0 ? numeroCTe.ToString() : string.Empty).
                              Replace("#NumeroNFe", numeroNotaFiscal > 0 ? numeroNotaFiscal.ToString() : string.Empty).
                              Replace("#NumeroNotaFiscal", notas).
                              Replace("#CodigoFilial", carga.Filial?.CodigoFilialEmbarcador ?? string.Empty).
                              Replace("#NomeTransportadora", carga.Empresa?.RazaoSocial ?? string.Empty).
                              Replace("#TipoOperacao", carga.TipoOperacao?.Descricao ?? string.Empty);
        }

        private List<int> BuscarPracas(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, out string erro, out string xmlRequest, out string xmlResponse)
        {
            erro = string.Empty;
            xmlResponse = string.Empty;
            xmlRequest = string.Empty;
            try
            {

                Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

                Servicos.Embarcador.Integracao.SemParar.PracasPedagio serPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar IntegracaoSemParar = repIntegracaoSemParar.Buscar();
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

                string polilinha = cargaRotaFrete?.PolilinhaRota;
                if (credencial.Autenticado)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemParar = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio>();

                    if (IntegracaoSemParar?.TipoBuscarPracasNaGeracaoDaCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBuscarPracasNaGeracaoDaCarga.Polilinhas)
                    {
                        //Buscar por pontos                                                
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                        if (IntegracaoSemParar.TipoPontoSemParar == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoSemParar.PolilinhaComQuadrante)
                            listaPontos = serPracasPedagio.ObterPontosPracaDePedagioPorPolilinha(polilinha, IntegracaoSemParar?.DistanciaMinimaQuadrante ?? 0);
                        else
                            listaPontos = Servicos.Embarcador.Logistica.Polilinha.ObterPontosPolilinha(polilinha, IntegracaoSemParar?.DistanciaMinimaQuadrante ?? 0);

                        pracasPedagiosSemParar = serPracasPedagio.ObterPracasPedagioPorPontos(credencial, listaPontos, out erro, unitOfWork, IntegracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out xmlRequest, out xmlResponse, null);
                    }
                    else
                    {
                        //Buscar por IBGE
                        List<int> codigosIBGE = new List<int>();
                        if (cargaRotaFrete != null)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> origens = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta);
                            foreach (var origem in origens)
                            {
                                if (origem.ClienteOutroEndereco != null)
                                    codigosIBGE.Add(origem.ClienteOutroEndereco.Localidade?.CodigoIBGE ?? 0);
                                else
                                    codigosIBGE.Add(origem.Cliente?.Localidade?.CodigoIBGE ?? 0);
                            }

                            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> destinos = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega);
                            foreach (var destino in destinos)
                            {
                                if (destino.ClienteOutroEndereco != null)
                                    codigosIBGE.Add(destino.ClienteOutroEndereco.Localidade?.CodigoIBGE ?? 0);
                                else
                                    codigosIBGE.Add(destino.Cliente?.Localidade?.CodigoIBGE ?? 0);
                            }

                            //var remetentes = carga.DadosSumarizados.ClientesRemetentes;
                            //foreach (var remetente in remetentes)
                            //    codigosIBGE.Add(remetente.Localidade.CodigoIBGE);

                            //var destinatarios = carga.DadosSumarizados.ClientesDestinatarios;
                            //foreach (var destinatario in destinatarios)
                            //    codigosIBGE.Add(destinatario.Localidade.CodigoIBGE);
                        }
                        pracasPedagiosSemParar = serPracasPedagio.ObterPracasPedagio(credencial, codigosIBGE, out erro, out xmlRequest, out xmlResponse, unitOfWork, IntegracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida);
                    }

                    if (string.IsNullOrWhiteSpace(erro))
                    {
                        List<int> pracas = new List<int>();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio praca in pracasPedagiosSemParar)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = repPracaPedagio.BuscarPorCodigoIntegracao(praca.Id.ToString());
                            if (pracaPedagio == null)
                            {
                                pracaPedagio = new Dominio.Entidades.Embarcador.Logistica.PracaPedagio();
                                pracaPedagio.Ativo = true;
                                pracaPedagio.CodigoIntegracao = praca.Id.ToString();
                                pracaPedagio.Concessionaria = praca.Concessionaria;
                                pracaPedagio.Descricao = praca.Praca;
                                pracaPedagio.KM = praca.KM;
                                pracaPedagio.Observacao = "";
                                pracaPedagio.Rodovia = praca.Rodovia;
                                repPracaPedagio.Inserir(pracaPedagio);
                            }
                            pracasPedagio.Add(pracaPedagio);

                            int.TryParse(praca.Id.ToString(), out int codigoPraca);
                            if (codigoPraca > 0)
                                pracas.Add(codigoPraca);
                        }
                        return pracas;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    erro = "Não foi possível autenticar no SemParar";
                    return null;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return null;
            }

        }

        private bool IsPossuiIntegracaoPiracanjuba(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            return repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Piracanjuba) != null;
        }

        private static void AdicionarArquivosIntegracao(string descricao, string request, string response, Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            arquivoIntegracao.Data = DateTime.Now;
            arquivoIntegracao.Mensagem = descricao;
            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request, "xml", unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response, "xml", unitOfWork);
            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
            pracaPedagioTarifaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
            repPracaPedagioTarifaIntegracao.Atualizar(pracaPedagioTarifaIntegracao);

        }

        private decimal RoundUpValue(decimal value, int decimalpoint)
        {
            var result = Math.Round(value, decimalpoint);
            if (result < value)
            {
                result += (decimal)Math.Pow(10, -decimalpoint);
            }
            return result;
        }

        private bool ValidarUltimoTipoRoterizado(List<Dominio.Entidades.RotaFretePontosPassagem> rotaFretePontosPassagem, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> cargaRotaFretePontosPassagem)
        {
            if (rotaFretePontosPassagem != null && (rotaFretePontosPassagem[0].RotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || rotaFretePontosPassagem[0].RotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando))
                return true;

            if (cargaRotaFretePontosPassagem != null && (cargaRotaFretePontosPassagem[0].CargaRotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem || cargaRotaFretePontosPassagem[0].CargaRotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando))
                return true;

            return false;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio RetornarSituacaoExtratoValePedagio(Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio.ExtratosValePedagio extratoValePedagio)
        {
            if (extratoValePedagio.ValePedagio != null && (extratoValePedagio.Extratos?.Count ?? 0) > 0)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio.EmExtrato;
            else if (extratoValePedagio.ValePedagio == null && (extratoValePedagio.Extratos?.Count ?? 0) > 0)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio.SemValePedagio;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio.SemExtrato;
        }

        private System.Net.Http.HttpClient CriarRequisicao(string urlBase, Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            System.Net.Http.HttpClient requisicao = new System.Net.Http.HttpClient();

            requisicao.BaseAddress = new Uri(urlBase);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("sessao", credencial.Sessao.ToString());

            requisicao.Timeout = TimeSpan.FromMinutes(3);

            return requisicao;
        }
        #endregion
    }
}
