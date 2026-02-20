using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Servicos.Embarcador.Integracao.GPA
{
    public class IntegracaoGPA
    {
        #region Propriedades Privadas

        private HttpWebRequest ClientRequest { get; set; }
        private string ClientRequestContent { get; set; }
        private string ClientResponseContent { get; set; }

        #endregion

        #region Construtores

        public static IntegracaoGPA GetInstance()
        {
            return new IntegracaoGPA();
        }

        #endregion

        #region Métodos Públicos

        public static bool EmitirNFSe(string endpoint, Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EmitirNfse dadosNFSe, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            try
            {
                erro = "";

                //Servicos.Log.TratarErro("Criando instância de comunicação GPA.", "IntegracaoGPA");
                IntegracaoGPA instancia = GetInstance();

                //Servicos.Log.TratarErro("Montando request endpoint.", "IntegracaoGPA");
                instancia.MontaRequest(endpoint, System.Net.WebRequestMethods.Http.Post);

                //Servicos.Log.TratarErro("Setando dados ao request.", "IntegracaoGPA");

                if (!instancia.SetDadosRequisicao(dadosNFSe))
                {
                    erro = "Erro ao montar dados para requisição de integração.";
                    return false;
                }

                try
                {
                    //Servicos.Log.TratarErro("Configurando certificado.", "IntegracaoGPA");
                    instancia.AdicionarCertificadoRequisicao(null, unitOfWork);

                    //Servicos.Log.TratarErro("Enviando request.", "IntegracaoGPA");
                    instancia.EnviarRequisicao();
                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro(e, "IntegracaoGPA");
                    erro = "Erro ao enviar a requisição para WebService.";
                    return false;
                }

                //Servicos.Log.TratarErro("Criando log de integração.", "IntegracaoGPA");
                instancia.CriarLogIntegracaoNFSe(unitOfWork);

                //Servicos.Log.TratarErro("Validando response:", "IntegracaoGPA");
                if (instancia.ClientResponseContent != "Sucesso")
                {
                    //Servicos.Log.TratarErro("Falha.", "IntegracaoGPA");
                    erro = "Requisição ao WebService retornou algum algum erro.";
                    return false;
                }
                //else
                //    Servicos.Log.TratarErro("Sucesso.", "IntegracaoGPA");

                return true;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoGPA");
                erro = "Erro genérico ao enviar requisição para WebService.";
                return false;
            }
        }

        public static void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoGPA || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoGPA))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a GPA.";
            }
            else
            {
                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                if (cargaIntegracao.Carga.CodigoCargaEmbarcador.Length > 7)
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Conforme definido pelo GPA cargas com mais de 7 digitos não são retornadas.";
                }
                else
                {

                    string urlWebService = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoGPA : configuracaoIntegracao.URLHomologacaoGPA;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCargaFilaOnline retornoCargaFilaOnline = ObterObjetoRequisicao(cargaIntegracao, unitOfWork);
                    IntegracaoGPA integracaoGPA = GetInstance();

                    integracaoGPA.MontaRequest(urlWebService, System.Net.WebRequestMethods.Http.Post);

                    if (!integracaoGPA.SetDadosRequisicao(retornoCargaFilaOnline))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Erro ao montar dados para requisição de integração.";
                    }
                    else
                    {
                        Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                        try
                        {
                            integracaoGPA.AdicionarCertificadoRequisicao(configuracaoIntegracao.APIKeyGPA, unitOfWork);
                            integracaoGPA.EnviarRequisicao();

                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                            integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unitOfWork);

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        }
                        catch (Exception excecao)
                        {
                            Servicos.Log.TratarErro(excecao, "IntegracaoGPA");
                            cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da GPA.";
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                            integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unitOfWork);

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            Servicos.Log.TratarErro(JsonConvert.SerializeObject(retornoCargaFilaOnline), "IntegracaoGPA");
                        }
                    }
                }
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarEncostaVeiculo(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            IntegracaoGPA instancia = GetInstance();

            Repositorio.Embarcador.Logistica.CentroCarregamentoDoca repCentroCarregamentoDoca = new Repositorio.Embarcador.Logistica.CentroCarregamentoDoca(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            mensagem = string.Empty;
            if (configuracaoIntegracao == null || !configuracaoIntegracao.PossuiIntegracaoGPA || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoGPA))
            {
                Servicos.Log.TratarErro("Não existe configuração de integração disponível.", "IntegrarEncostaVeiculoGPA");
                mensagem = "Não existe configuração de integração disponível";
            }
            else
            {
                if (carga.CodigoCargaEmbarcador.Length > 7)
                {
                    Servicos.Log.TratarErro("Conforme definido pelo GPA cargas com mais de 7 digitos não são retornadas: " + carga.CodigoCargaEmbarcador, "IntegrarEncostaVeiculoGPA");
                    mensagem = "Conforme definido pelo GPA cargas com mais de 7 digitos não são retornadas: " + carga.CodigoCargaEmbarcador;

                    return;
                }

                string urlWebService = configuracaoIntegracao.URLHomologacaoGPA;

                if (carga.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                    urlWebService = configuracaoIntegracao.URLProducaoGPA;

                string numeroDoca = !string.IsNullOrWhiteSpace(carga.NumeroDocaEncosta) ? carga.NumeroDocaEncosta : !string.IsNullOrWhiteSpace(carga.NumeroDoca) ? carga.NumeroDoca : string.Empty;
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDoca docaCentroCarregamento = repCentroCarregamentoDoca.BuscarPorFilialDescricao(carga.Filial.Codigo, numeroDoca);
                if (docaCentroCarregamento != null)
                    numeroDoca = docaCentroCarregamento.Numero.ToString();
                else
                {
                    Servicos.Log.TratarErro("Não localizada Doca " + numeroDoca + " para filial " + carga.Filial.Codigo.ToString(), "IntegrarEncostaVeiculoGPA");
                }

                string placaVeiculo = carga.Veiculo != null ? carga.Veiculo.Placa : string.Empty;
                if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Count > 0)
                    placaVeiculo = carga.VeiculosVinculados.FirstOrDefault().Placa;

                // Objeto de requisicao do GPA
                Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoEncostaVeiculoDoca retornoCargaFilaOnline = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoEncostaVeiculoDoca()
                {
                    tipoMensagem = "encostaVeiculoDoca",
                    Dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoEncostaVeiculoDocaDados()
                    {
                        CnpjFilial = carga.Filial?.CNPJ ?? string.Empty,
                        NumeroDaCarga = carga.CodigoCargaEmbarcador,
                        Placa = placaVeiculo,
                        NumeroDaDoca = numeroDoca
                    }
                };


                instancia.MontaRequest(urlWebService, System.Net.WebRequestMethods.Http.Post);
                if (!instancia.SetDadosRequisicao(retornoCargaFilaOnline))
                {
                    Servicos.Log.TratarErro("Erro ao montar dados para requisição de integração.", "IntegrarEncostaVeiculoGPA");
                    mensagem = "Não foi possível montar dados para requisição de integração";
                }
                else
                {
                    try
                    {
                        Servicos.Log.TratarErro(JsonConvert.SerializeObject(retornoCargaFilaOnline), "IntegrarEncostaVeiculoGPA");

                        instancia.AdicionarCertificadoRequisicao(configuracaoIntegracao.APIKeyGPA, unitOfWork);

                        instancia.EnviarRequisicao();

                        Servicos.Log.TratarErro("Integração retorno doca carga " + carga.CodigoCargaEmbarcador + " realizada com sucesso: " + JsonConvert.SerializeObject(retornoCargaFilaOnline), "IntegrarEncostaVeiculoGPA");
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Ocorreu uma falha ao comunicar com o Web Service da GPA (" + urlWebService + ") retorno doca " + carga.NumeroDoca + " da carga " + carga.CodigoCargaEmbarcador + ": " + ex, "IntegrarEncostaVeiculoGPA");

                        mensagem = "Falha ao comunicar com o serviço do GPA.";
                    }
                }
            }
        }

        public static void IntegrarMDFeCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoGPA || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoGPA))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a GPA.";
            }
            else
            {
                Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
                Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCodigoCarga(cargaIntegracao.Carga.Codigo);

                int numeroCarga = 0;
                int numeroUnidade = 0;
                if (integracaoCarga != null)
                {
                    int.TryParse(integracaoCarga.NumeroDaCarga, out numeroCarga);
                    int.TryParse(integracaoCarga.NumeroDaUnidade, out numeroUnidade);
                }
                else
                {
                    int.TryParse(cargaIntegracao.Carga.CodigoCargaEmbarcador, out numeroCarga);
                    int.TryParse(cargaIntegracao.Carga.Filial?.CodigoFilialEmbarcador, out numeroUnidade);
                }

                cargaIntegracao.NumeroTentativas += 1;
                cargaIntegracao.DataIntegracao = DateTime.Now;

                if (numeroCarga.ToString().Length > 7) //10023266
                {
                    cargaIntegracao.ProblemaIntegracao = "Conforme definido pelo GPA cargas com mais de 7 digitos não são retornadas.";
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {

                    string urlWebService = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoGPA : configuracaoIntegracao.URLHomologacaoGPA;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoMDFe retornoMDFe = ObterObjetoMDFeCIOT(cargaIntegracao, numeroCarga, numeroUnidade, unitOfWork);

                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(retornoMDFe, Formatting.Indented), "IntegrarMDFeCIOT");

                    IntegracaoGPA integracaoGPA = GetInstance();

                    integracaoGPA.MontaRequest(urlWebService, System.Net.WebRequestMethods.Http.Post);

                    if (!integracaoGPA.SetDadosRequisicao(retornoMDFe))
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = "Erro ao montar dados para requisição de integração.";
                    }
                    else
                    {
                        Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                        try
                        {
                            integracaoGPA.AdicionarCertificadoRequisicao(configuracaoIntegracao.APIKeyGPA, unitOfWork);
                            integracaoGPA.EnviarRequisicao();

                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                            repositorioCargaIntegracao.Atualizar(cargaIntegracao);

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = cargaIntegracao.ProblemaIntegracao;
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                            integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unitOfWork);

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        }
                        catch (Exception excecao)
                        {
                            Servicos.Log.TratarErro(excecao, "IntegrarMDFeCIOT");
                            cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da GPA.";
                            cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                            arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                            integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unitOfWork);

                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                            Servicos.Log.TratarErro(JsonConvert.SerializeObject(retornoMDFe), "IntegrarMDFeCIOT");
                        }
                    }
                }
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao, Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoGPA || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoGPA))
            {
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Configuração para integração com GPA inválida.";
                cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaIntegracao.NumeroTentativas++;
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                return;
            }

            Repositorio.IntegracaoCarga repIntegracaoCarga = new Repositorio.IntegracaoCarga(unitOfWork);
            Dominio.Entidades.IntegracaoCarga integracaoCarga = repIntegracaoCarga.BuscarPorCodigoCarga(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Codigo);

            int numeroCarga = 0;
            int numeroUnidade = 0;
            if (integracaoCarga != null)
            {
                int.TryParse(integracaoCarga.NumeroDaCarga, out numeroCarga);
                int.TryParse(integracaoCarga.NumeroDaUnidade, out numeroUnidade);
            }
            else
            {
                int.TryParse(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.CodigoCargaEmbarcador, out numeroCarga);
                int.TryParse(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga.Filial?.CodigoFilialEmbarcador, out numeroUnidade);
            }

            cargaCancelamentoCargaIntegracao.NumeroTentativas += 1;
            cargaCancelamentoCargaIntegracao.DataIntegracao = DateTime.Now;


            string urlWebService = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoGPA : configuracaoIntegracao.URLHomologacaoGPA;
            Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCancelamentoCarga retornoCancelamento = ObterObjetoCancelamentoCarga(cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga, numeroCarga, numeroUnidade, unitOfWork);

            Servicos.Log.TratarErro(JsonConvert.SerializeObject(retornoCancelamento, Formatting.Indented), "IntegrarCancelamentoCarga");

            IntegracaoGPA integracaoGPA = GetInstance();

            integracaoGPA.MontaRequest(urlWebService, System.Net.WebRequestMethods.Http.Post);

            if (!integracaoGPA.SetDadosRequisicao(retornoCancelamento))
            {
                cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Erro ao montar dados para requisição de integração.";
            }
            else
            {
                try
                {
                    integracaoGPA.AdicionarCertificadoRequisicao(configuracaoIntegracao.APIKeyGPA, unitOfWork);
                    integracaoGPA.EnviarRequisicao();

                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCancelamentoCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = cargaCancelamentoCargaIntegracao.ProblemaIntegracao;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                    integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCancelamentoCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao, "IntegrarCancelamentoCarga");
                    cargaCancelamentoCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da GPA.";
                    cargaCancelamentoCargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                    Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                    arquivoIntegracao.Data = cargaCancelamentoCargaIntegracao.DataIntegracao;
                    arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unitOfWork);

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                    cargaCancelamentoCargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    Servicos.Log.TratarErro(JsonConvert.SerializeObject(retornoCancelamento), "IntegrarCancelamentoCarga");
                }
            }
            repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        public static void IntegrarEscrituracaoCTe(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            if ((configuracaoIntegracao == null) || !configuracaoIntegracao.PossuiIntegracaoGPA || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLProducaoGPA))
            {
                cargaCTeIntegracao.ProblemaIntegracao = "Configuração para integração com o GPA inválida.";
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return;
            }
            string mensagemErro = string.Empty;
            try
            {
                string urlWebService = (empresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao) ? configuracaoIntegracao.URLProducaoGPA : configuracaoIntegracao.URLHomologacaoGPA;
                Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTe escriturarCTe = ObterObjetoEscriturarCTe(cargaCTeIntegracao, unidadeDeTrabalho);

                Servicos.Log.TratarErro(JsonConvert.SerializeObject(escriturarCTe, Formatting.Indented), "IntegrarGPAEscrituracaoCTe");

                if (escriturarCTe.Dados.cenario == "") //Somente retorna cargas do projeto TAC (que possuem CIOT) && tipoProprietarioVeiculo == "F") ou cargas de Subcontratação
                {
                    cargaCTeIntegracao.ProblemaIntegracao = "Conforme definição GPA somente cargas do processo TAC, Subcontratação e eCommerce tem seus CTEs escriturados.";
                    cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                    cargaCTeIntegracao.NumeroTentativas++;
                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    return;
                }

                IntegracaoGPA integracaoGPA = GetInstance();

                integracaoGPA.MontaRequest(urlWebService, System.Net.WebRequestMethods.Http.Post);

                if (!integracaoGPA.SetDadosRequisicao(escriturarCTe))
                {
                    cargaCTeIntegracao.ProblemaIntegracao = "Erro ao montar dados para requisição de integração.";
                    cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                    cargaCTeIntegracao.NumeroTentativas++;
                    cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    return;
                }
                else
                {
                    try
                    {
                        integracaoGPA.AdicionarCertificadoRequisicao(configuracaoIntegracao.APIKeyGPA, unidadeDeTrabalho);
                        integracaoGPA.EnviarRequisicao();

                        cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCTeIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = cargaCTeIntegracao.ProblemaIntegracao;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                        integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unidadeDeTrabalho);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao, "IntegrarMDFeCIOT");
                        cargaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao comunicar com o Web Service da GPA.";
                        cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaCTeIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                        integracaoGPA.CriarLogIntegracaoCarga(ref arquivoIntegracao, unidadeDeTrabalho);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                        Servicos.Log.TratarErro(JsonConvert.SerializeObject(escriturarCTe), "IntegrarGPAEscrituracaoCTe");
                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar escrituração o CT-e " + cargaCTeIntegracao.CargaCTe.CTe.Numero + " para GPA";
                cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            cargaCTeIntegracao.ProblemaIntegracao = mensagemErro;
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas++;
        }

        #endregion

        #region Metodos Privados

        private void MontaRequest(string endpoint, string metodo)
        {
            //string urlAPI = "https://" + "servicos-homolog.cbdnet.com.br:450" + "/tmsfiscal/multicte/callback";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest client = (HttpWebRequest)WebRequest.Create(endpoint);

            client.Method = metodo;
            client.ProtocolVersion = HttpVersion.Version10;
            client.ContentType = "application/json";
            client.KeepAlive = false;

            ClientRequest = client;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCargaFilaOnline ObterObjetoRequisicao(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCargaFilaOnlineDados dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCargaFilaOnlineDados()
            {
                protocoloCarga = cargaIntegracao.Carga.Codigo.ToString(),
                cnpjOrigem = cargaIntegracao.Carga.DadosSumarizados.ClientesRemetentes != null && cargaIntegracao.Carga.DadosSumarizados.ClientesRemetentes.Count > 0 ? cargaIntegracao.Carga.DadosSumarizados.ClientesRemetentes.FirstOrDefault().CPF_CNPJ_SemFormato : string.Empty,
                numeroDaRota = cargaIntegracao.Carga.CodigoCargaEmbarcador,
                cnpjTransportador = cargaIntegracao.Carga.Empresa != null ? cargaIntegracao.Carga.Empresa.CNPJ : string.Empty,
                cpfMotorista = cargaIntegracao.Carga.Motoristas != null && cargaIntegracao.Carga.Motoristas.Count > 0 ? cargaIntegracao.Carga.Motoristas.FirstOrDefault().CPF : string.Empty
            };

            Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoInformacao filaCarregamentoVeiculoInformacao = new Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema).ObterInformacoesPorCarga(cargaIntegracao.Carga.Codigo);

            if (filaCarregamentoVeiculoInformacao == null)
            {
                dados.dataHoraEntradaFila = DateTime.Now.AddMinutes(-35).ToString("dd/MM/yyyy HH:mm");
                dados.dataHoraSaidaFila = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                dados.placaCavalo = cargaIntegracao.Carga.Veiculo?.Placa ?? string.Empty;
                dados.placaCarreta = cargaIntegracao.Carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty;
                dados.posicaoEntradaNaFila = "1";
            }
            else
            {
                dados.dataHoraEntradaFila = filaCarregamentoVeiculoInformacao.DataEntrada.ToString("dd/MM/yyyy HH:mm");
                dados.dataHoraSaidaFila = filaCarregamentoVeiculoInformacao.DataCargaAceita.HasValue ? filaCarregamentoVeiculoInformacao.DataCargaAceita.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                dados.placaCavalo = filaCarregamentoVeiculoInformacao.Tracao?.Placa ?? string.Empty;
                dados.placaCarreta = filaCarregamentoVeiculoInformacao.Reboques?.FirstOrDefault()?.Placa ?? string.Empty;
                dados.posicaoEntradaNaFila = filaCarregamentoVeiculoInformacao.PosicaoEntrada.ToString();
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCargaFilaOnline()
            {
                tipoMensagem = "retornoCargaFilaOnline",
                Dados = dados
            };
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoMDFe ObterObjetoMDFeCIOT(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, int numeroCarga, int numeroUnidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> listaCargaMDFe = repCargaMDFe.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaIntegracao.Carga.Codigo);

            // Objeto de requisicao do GPA
            Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoMDFe retornoMDFe = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoMDFe()
            {
                tipoMensagem = "retornoMdfe",
                Dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoMDFeDados()
                {
                    numeroCarga = numeroCarga,
                    numeroUnidade = numeroUnidade
                }
            };

            if (cargaCIOT != null)
            {
                retornoMDFe.Dados.ciots = new List<string>();
                retornoMDFe.Dados.ciots.Add(cargaCIOT.CIOT.Numero);
            }

            if (listaCargaMDFe != null && listaCargaMDFe.Count > 0)
            {
                retornoMDFe.Dados.mdfes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.MDFe>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in listaCargaMDFe)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.MDFe mdfeRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.MDFe();
                    mdfeRetorno.numero = cargaMDFe.MDFe.Numero;
                    mdfeRetorno.serie = cargaMDFe.MDFe.Serie.Numero;
                    mdfeRetorno.xml = servicoMDFe.ObterStringXML(cargaMDFe.MDFe, Dominio.Enumeradores.TipoXMLMDFe.Autorizacao, unitOfWork);
                    retornoMDFe.Dados.mdfes.Add(mdfeRetorno);

                    //Ajustada configuração configuracaoEmbarcador.EnviarMDFeAutomaticamenteParaImpressao para enviar MDFes automaticamente para impressão
                    //Dominio.Entidades.IntegracaoMDFe integracaoMDFe = new Dominio.Entidades.IntegracaoMDFe();
                    //integracaoMDFe.Arquivo = "";
                    //integracaoMDFe.MDFe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais { Codigo = cargaMDFe.MDFe.Codigo };
                    //integracaoMDFe.NumeroDaUnidade = integracaoCarga.NumeroDaUnidade;
                    //integracaoMDFe.NumeroDaCarga = integracaoCarga.NumeroDaCarga;
                    //integracaoMDFe.Status = Dominio.Enumeradores.StatusIntegracao.Pendente;
                    //integracaoMDFe.Tipo = Dominio.Enumeradores.TipoIntegracaoMDFe.Emissao;
                    //integracaoMDFe.TipoArquivo = Dominio.Enumeradores.TipoArquivoIntegracao.CTe;
                    //repIntegracaoMDFe.Inserir(integracaoMDFe);
                }
            }

            return retornoMDFe;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCancelamentoCarga ObterObjetoCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, int numeroCarga, int numeroUnidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMDFe repCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);
            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repCargaCTe.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> listaCargaMDFe = repCargaMDFe.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(carga.Codigo);

            // Objeto de requisicao do GPA
            Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCancelamentoCarga retornoCancelamento = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCancelamentoCarga()
            {
                tipoMensagem = "retornoCancelamentoCarga",
                Dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.RetornoCancelamentoDados()
                {
                    numeroCarga = numeroCarga,
                    numeroUnidade = numeroUnidade
                }
            };

            if (cargaCIOT != null)
            {
                retornoCancelamento.Dados.ciots = new List<string>();
                retornoCancelamento.Dados.ciots.Add(cargaCIOT.CIOT.Numero);
            }

            if (listaCargaCTe != null && listaCargaCTe.Count > 0)
            {
                retornoCancelamento.Dados.ctes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.CTeCancelamento>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.CTeCancelamento cteRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.CTeCancelamento();
                    cteRetorno.numero = cargaCTe.CTe.Numero;
                    cteRetorno.status = cargaCTe.CTe.MensagemStatus?.CodigoDoErro ?? 100;
                    cteRetorno.descricaoStatus = cargaCTe.CTe.MensagemRetornoSefaz;
                    cteRetorno.xmlCancelamento = servicoCTe.ObterStringXMLCancelamento(cargaCTe.CTe, unitOfWork);
                    retornoCancelamento.Dados.ctes.Add(cteRetorno);
                }
            }

            if (listaCargaMDFe != null && listaCargaMDFe.Count > 0)
            {
                retornoCancelamento.Dados.mdfes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.MDFeCancelamento>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe cargaMDFe in listaCargaMDFe)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.MDFeCancelamento mdfeRetorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.MDFeCancelamento();
                    mdfeRetorno.numero = cargaMDFe.MDFe.Numero;
                    mdfeRetorno.serie = cargaMDFe.MDFe.Serie.Numero;
                    mdfeRetorno.xmlCancelamento = servicoMDFe.ObterStringXML(cargaMDFe.MDFe, Dominio.Enumeradores.TipoXMLMDFe.Cancelamento, unitOfWork);
                    retornoCancelamento.Dados.mdfes.Add(mdfeRetorno);
                }
            }

            return retornoCancelamento;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTe ObterObjetoEscriturarCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Repositorio.SubcontratacaoDocumentos repSubcontratacaoDocumentos = new Repositorio.SubcontratacaoDocumentos(unitOfWork);

            Dominio.Entidades.IntegracaoCTe integracaoCTe = repIntegracaoCTe.BuscarPrimeiroPorCTeETipo(cargaCTeIntegracao.CargaCTe.CTe.Codigo, Dominio.Enumeradores.TipoIntegracao.Emissao);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPorCarga(cargaCTeIntegracao.CargaCTe.Carga.Codigo);

            //Dominio.Entidades.InformacaoCargaCTE informacaoQuantidadeCTe = repInformacaoCargaCTe.BuscarPorCTeUnidade(cargaCTeIntegracao.CargaCTe.CTe.Codigo, "03");

            string cfopGPA = ObterCFOPGPA(cargaCTeIntegracao.CargaCTe.CTe);
            string cfopCTe = cargaCTeIntegracao.CargaCTe.CTe.CFOP.CodigoCFOP.ToString() + "18";
            string material = cargaCTeIntegracao.CargaCTe.CTe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "7640094" : "2529806";

            string cenario = "";
            if (repSubcontratacaoDocumentos.BuscarPorCTe(cargaCTeIntegracao.CargaCTe.CTe.Codigo) != null) //CTe de subcontratação
            {
                if (cargaCTeIntegracao.CargaCTe.CTe.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                {
                    cenario = "05";
                    material = "2506142";
                }
                else
                {
                    cenario = "01";
                    material = "1258248";
                }
            }
            else if (integracaoCTe?.CodigoTipoOperacao == "06") //Projeto eCommerce
            {
                if (cargaCIOT != null && cargaCIOT.CIOT != null)
                    cenario = "14"; //eCommerce com CIOT
                else
                    cenario = "05"; //eCommerce sem CIOT
            }
            else if (integracaoCTe?.CodigoTipoOperacao == "09") //Projeto Fullfilment 
            {
                cenario = "10";
                material = "1284223";
            }
            else if (cargaCIOT != null && cargaCIOT.CIOT != null)
                cenario = "10"; //Quando é projeto TAC envia 10

            if (cenario == string.Empty && cargaCTeIntegracao.CargaCTe.Carga.TipoOperacao != null)
            {
                if (cargaCTeIntegracao.CargaCTe.Carga.TipoOperacao.CodigoIntegracao.Contains("Escriturar_"))
                {
                    cenario = "14";
                    cfopGPA = "535318";
                    material = "2506142";
                }
            }


            string protocolo = string.Empty;
            if (integracaoCTe != null)
                protocolo = integracaoCTe.CodigoControleInternoCliente;
            else if (cargaCTeIntegracao.CargaCTe.CTe.TipoOperacao != null && !string.IsNullOrWhiteSpace(cargaCTeIntegracao.CargaCTe.CTe.TipoOperacao.ConfiguracaoEmissao?.PrefixoObservacaoSequencialCTe))
                protocolo = cargaCTeIntegracao.CargaCTe.CTe.TipoOperacao.ConfiguracaoEmissao.PrefixoObservacaoSequencialCTe + cargaCTeIntegracao.CargaCTe.CTe.SequencialOperacao.ToString().PadLeft(14, '0');

            Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTe escriturarCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTe()
            {
                tipoMensagem = "escriturarCTe",

                Dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTeDados()
                {
                    protocolo = protocolo,
                    cenario = cenario,
                    empresa = "",//cargaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ,
                    localNegocio = "",
                    centro = "",
                    idFiscal = cargaCTeIntegracao.CargaCTe.CTe.Empresa.CNPJ,
                    nfeNum = cargaCTeIntegracao.CargaCTe.CTe.Numero.ToString(),
                    serie = cargaCTeIntegracao.CargaCTe.CTe.Serie.Numero.ToString(),
                    material = material,
                    centroCusto = "7759",
                    doctoData = cargaCTeIntegracao.CargaCTe.CTe.DataEmissao.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.DataEmissao.Value.ToString("dd/MM/yyyy") : "", // Formato dd/MM/yyyy
                    doctoStatus = "1",
                    doctoNro = !string.IsNullOrWhiteSpace(cargaCTeIntegracao.CargaCTe.CTe.Chave) ? cargaCTeIntegracao.CargaCTe.CTe.Chave.Substring(34, 9) : "",
                    digitoChave = !string.IsNullOrWhiteSpace(cargaCTeIntegracao.CargaCTe.CTe.Chave) ? cargaCTeIntegracao.CargaCTe.CTe.Chave.Substring(43, 1) : "",
                    protocoloData = cargaCTeIntegracao.CargaCTe.CTe.DataAutorizacao.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.DataAutorizacao.Value.ToString("dd/MM/yyyy") : "", // Formato dd/MM/yyyy
                    protocoloHora = cargaCTeIntegracao.CargaCTe.CTe.DataAutorizacao.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.DataAutorizacao.Value.ToString("HH:mm:ss") : "", // Formato HH:mm:ss
                    protocoloLog = cargaCTeIntegracao.CargaCTe.CTe.Protocolo,
                    chaveDANFE = cargaCTeIntegracao.CargaCTe.CTe.Chave,
                    codVerifNFSe = "",
                    CPFCNPJ = cargaCTeIntegracao.CargaCTe.CTe.TomadorPagador?.CPF_CNPJ_SemFormato ?? cargaCTeIntegracao.CargaCTe.CTe.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                    CFOP = cfopGPA,
                    CFOP_NF = cfopCTe,
                    quantidade = 1,
                    unidadeMedida = "UN",
                    precoLiquido = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                    valorLiquido = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                    precoComImpostos = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                    valorComImpostos = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                    numeroRPS = ""
                }
            };

            escriturarCTe.Dados.taxas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTeDadosTaxas>();

            var taxaICMS = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTeDadosTaxas()
            {
                tipoImposto = cargaCTeIntegracao.CargaCTe.CTe.CST == "40" ? "ICMS1" : "ICMS",
                montanteBasico = cargaCTeIntegracao.CargaCTe.CTe.BaseCalculoICMS,
                taxaImposto = cargaCTeIntegracao.CargaCTe.CTe.AliquotaICMS,
                valorFiscal = cargaCTeIntegracao.CargaCTe.CTe.ValorICMS,
                impostoRetido = "",
                percBaseImposto = 100,
                impVlLiquido = "X"
            };
            escriturarCTe.Dados.taxas.Add(taxaICMS);

            var taxaPIS = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTeDadosTaxas()
            {
                tipoImposto = "PIS",
                montanteBasico = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                taxaImposto = cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaPIS.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaPIS.Value : 0,
                valorFiscal = cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaPIS.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber * (cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaPIS.Value / 100) : 0,
                impostoRetido = "",
                percBaseImposto = 100,
                impVlLiquido = "X"
            };
            escriturarCTe.Dados.taxas.Add(taxaPIS);

            var taxasCOFINS = new Dominio.ObjetosDeValor.Embarcador.Integracao.GPA.EscriturarCTeDadosTaxas()
            {
                tipoImposto = "COFI",
                montanteBasico = cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber,
                taxaImposto = cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaPIS.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaCOFINS.Value : 0,
                valorFiscal = cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaPIS.HasValue ? cargaCTeIntegracao.CargaCTe.CTe.ValorAReceber * (cargaCTeIntegracao.CargaCTe.CTe.Empresa.Configuracao.AliquotaCOFINS.Value / 100) : 0,
                impostoRetido = "",
                percBaseImposto = 100,
                impVlLiquido = "X"
            };
            escriturarCTe.Dados.taxas.Add(taxasCOFINS);

            return escriturarCTe;
        }

        private bool SetDadosRequisicao(dynamic dados)
        {
            StreamWriter dataWriter = null;
            string jsonPost = JsonConvert.SerializeObject(dados, Formatting.Indented);

            ClientRequestContent = jsonPost;

            try
            {
                dataWriter = new StreamWriter(ClientRequest.GetRequestStream());
                dataWriter.Write(jsonPost);
                //ClientRequest.ContentLength = jsonPost.Length;
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e, "IntegracaoGPA");
                Servicos.Log.TratarErro("Erro ao preparar dados de requisição", "IntegracaoGPA");
                return false;
            }
            finally
            {
                if (dataWriter != null)
                    dataWriter.Close();
            }

            return true;
        }

        private void AdicionarCertificadoRequisicao(string apiKey, Repositorio.UnitOfWork unitOfWork)
        {

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                ClientRequest.Headers.Add("x-api-key", apiKey);
                return;
            }

            // Buscar Certificado Configurado
            var certificado = new X509Certificate2(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas + @"\13969629000196\Certificado\MULTICTE_GPA.pfx", "Multi@2015");

            ClientRequest.ClientCertificates.Add(certificado);
            ClientRequest.PreAuthenticate = true;
        }

        private void EnviarRequisicao()
        {
            HttpWebResponse objResponse = (HttpWebResponse)ClientRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                ClientResponseContent = sr.ReadToEnd();
                sr.Close();
            }
        }

        private void CriarLogIntegracaoNFSe(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.IntegracaoGPA repIntegracaoGPA = new Repositorio.Embarcador.Integracao.IntegracaoGPA(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.IntegracaoGPA log = new Dominio.Entidades.Embarcador.Integracao.IntegracaoGPA()
            {
                Data = DateTime.Now,
                Requisicao = ClientRequestContent,
                Resposta = ClientResponseContent
            };

            repIntegracaoGPA.Inserir(log);
        }

        private void CriarLogIntegracaoCarga(ref Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(ClientRequestContent, "json", unitOfWork);
            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(ClientResponseContent, "json", unitOfWork);
        }

        private static string ObterCFOPGPA(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte == null)
                return string.Empty;

            return "810325"; //Chamado 9492 solicitado pelo Lopes para enviar sempre 810325
            //if (cte.Empresa.CNPJ.Contains("11666171")) //Quando é CTe do GPA
            //{
            //    if (cte.CFOP.CodigoCFOP == 5353)
            //        return "000810325";
            //    else if (cte.CFOP.CodigoCFOP == 6353)
            //        return "000235309";
            //    else
            //        return string.Empty;

            //}
            //else //Busca das observações
            //{
            //    //CNF 8927208 LOJA 2588 - PLACA CAVALO: - CARRETA: - MOTORISTA: / CFOP: 235314; 
            //    if (!string.IsNullOrWhiteSpace(cte.ObservacoesGerais) && cte.ObservacoesGerais.Contains("CFOP:"))
            //    {
            //        int posicao = cte.ObservacoesGerais.IndexOf("CFOP:");
            //        int posicaoFim = posicao + 5 + 7;
            //        if (posicao > -1 && posicaoFim > -1)
            //            return cte.ObservacoesGerais.Substring(posicao + 5, posicaoFim - (posicao + 5)).Replace(" ", "");
            //    }

            //}
            //return string.Empty;
        }

        #endregion
    }
}
