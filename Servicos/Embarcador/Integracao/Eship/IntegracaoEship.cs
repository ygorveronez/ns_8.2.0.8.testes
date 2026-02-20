using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Embarcador.Integracao.Eship
{
    public class IntegracaoEship
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoEship(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void MontarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEShip repositorioIntegracaoEShip = new Repositorio.Embarcador.Configuracoes.IntegracaoEShip(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracaoEShip = repositorioIntegracaoEShip.Buscar();

            cargaCargaIntegracao.DataIntegracao = DateTime.Now;
            cargaCargaIntegracao.NumeroTentativas++;

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string msgIntegrado = "";

            try
            {
                if (!configuracaoIntegracaoEShip.PossuiIntegracao)
                    throw new ServicoException("Integração com e-Ship não habilitada!");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoEShip?.URLComunicacao))
                    throw new ServicoException("Favor informar a URL de comunicação com a e-Ship!");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoEShip?.ApiToken))
                    throw new ServicoException("Favor informar token da api para integração!");

                string url = configuracaoIntegracaoEShip.URLComunicacao + "MontarCarga";
                HttpClient requisicao = CriarRequisicao(url);

                List<Dominio.Entidades.Cliente> remetentes = cargaCargaIntegracao.Carga.Pedidos.Select(o => o.Pedido.Remetente).Distinct().ToList();

                foreach (Dominio.Entidades.Cliente remetente in remetentes)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = cargaCargaIntegracao.Carga.Pedidos.Where(ped => ped.Pedido.Remetente.CPF_CNPJ == remetente.CPF_CNPJ).Select(ped => ped.Pedido).ToList();
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCarga objetoMontarCarga = ObterDadosMontarCarga(configuracaoIntegracaoEShip, cargaCargaIntegracao.Carga, cargaCargaIntegracao.Protocolo, remetente, pedidos);
                    jsonRequisicao = JsonConvert.SerializeObject(objetoMontarCarga, Formatting.Indented);

                    MultipartFormDataContent conteudoRequisicao = new MultipartFormDataContent
                    {
                       { new StringContent(jsonRequisicao), "request" }
                    };

                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.RetornoMontarCarga retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.RetornoMontarCarga>(jsonRetorno);

                        if (retorno.Erros != null)
                            throw new ServicoException($"Problema ao integrar com e-Ship: { string.Join(", ", retorno.Erros.Select(o => o.Erro.Codigo + " - " + o.Erro.Mensagem)) }");

                        msgIntegrado = $"Integrado com sucesso indentificador rota: {remetente.CPF_CNPJ}. { string.Join(", ", "idEmbarque: " + retorno.Corpo.ProtocoloDaCarga.Dados.IdEmbarque + " - " + "idOrdem: " + retorno.Corpo.ProtocoloDaCarga.Dados.IdOrdem) }";
                        servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json", msgIntegrado);
                    }
                    else
                        throw new ServicoException($"Problema ao integrar com e-Ship: {retornoRequisicao.StatusCode}");
                }

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaCargaIntegracao.ProblemaIntegracao = "Pedidos integrados com sucesso!";
                cargaCargaIntegracao.Protocolo = Guid.NewGuid().ToString();
            }
            catch (ServicoException ex)
            {
                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com e-Ship";
            }

            if (cargaCargaIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json");

            repCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void ControlePortaria(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoEShip repositorioIntegracaoEShip = new Repositorio.Embarcador.Configuracoes.IntegracaoEShip(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracaoEShip = repositorioIntegracaoEShip.Buscar();

            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;
            cargaDadosTransporteIntegracao.NumeroTentativas++;

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string msgIntegrado = "";

            try
            {
                if (!configuracaoIntegracaoEShip.PossuiIntegracao)
                    throw new ServicoException("Integração com e-Ship não habilitada!");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoEShip?.URLComunicacao))
                    throw new ServicoException("Favor informar a URL de comunicação com a e-Ship!");

                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoEShip?.ApiToken))
                    throw new ServicoException("Favor informar token da api para integração!");

                string url = configuracaoIntegracaoEShip.URLComunicacao + "ControlePortaria";
                HttpClient requisicao = CriarRequisicao(url);

                List<Dominio.Entidades.Cliente> remetentes = cargaDadosTransporteIntegracao.Carga.Pedidos.Select(o => o.Pedido.Remetente).Distinct().ToList();

                foreach (Dominio.Entidades.Cliente remetente in remetentes)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortaria objetoControlePortaria = ObterDadosControlePortaria(configuracaoIntegracaoEShip, cargaDadosTransporteIntegracao.Carga, cargaDadosTransporteIntegracao.Protocolo, remetente);
                    jsonRequisicao = JsonConvert.SerializeObject(objetoControlePortaria, Formatting.Indented);

                    MultipartFormDataContent conteudoRequisicao = new MultipartFormDataContent
                {
                       { new StringContent(jsonRequisicao), "request" }
                };

                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.RetornoMontarCarga retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.RetornoMontarCarga>(jsonRetorno);

                        if (retorno.Erros != null)
                            throw new ServicoException($"Problema ao integrar o identificador rota {remetente.CPF_CNPJ}: { string.Join(", ", retorno.Erros.Select(o => o.Erro.Codigo + " - " + o.Erro.Mensagem)) }");

                        msgIntegrado = $"Integrado com sucesso o idenitifcador rota: {remetente.CPF_CNPJ}. { string.Join(", ", "idEmbarque: " + retorno.Corpo.ProtocoloDaCarga.Dados.IdEmbarque + " - " + "idOrdem: " + retorno.Corpo.ProtocoloDaCarga.Dados.IdOrdem) }";
                        servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json", msgIntegrado);
                    }
                    else
                        throw new ServicoException($"Problema ao integrar com e-Ship: {retornoRequisicao.StatusCode}");
                }

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Pedidos integrados com sucesso!";
                cargaDadosTransporteIntegracao.Protocolo = Guid.NewGuid().ToString();
            }
            catch (ServicoException ex)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com e-Ship";
            }

            if (cargaDadosTransporteIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");

            repCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);

        }

        public void VerificarIntegracaoEShip(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga == null)
                return;

            AtualizarStatusIntegracaoDadosTransporte(carga);
            AtualizarStatusIntegracao(carga);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoEship));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return requisicao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCarga ObterDadosMontarCarga(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracaoEShip, Dominio.Entidades.Embarcador.Cargas.Carga carga, string protocolo, Dominio.Entidades.Cliente remetente, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCarga()
            {
                Autenticacao = ObterDadosAutenticacao(configuracaoIntegracaoEShip),
                Parametros = ObterDadosParametrosMontarCarga(carga, protocolo, remetente),
                Pedidos = ObterDadosPedido(pedidos)

            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortaria ObterDadosControlePortaria(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracaoEShip, Dominio.Entidades.Embarcador.Cargas.Carga carga, string protocolo, Dominio.Entidades.Cliente remetente)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortaria()
            {
                Autenticacao = ObterDadosAutenticacaoControlePortaria(configuracaoIntegracaoEShip),
                Parametros = ObterDadosParametrosControlePortaria(carga, protocolo, remetente)
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaAutenticacao ObterDadosAutenticacao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracao)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaAutenticacao()
            {
                ApiKey = configuracaoIntegracao.ApiToken
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortariaAutenticacao ObterDadosAutenticacaoControlePortaria(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoEship configuracaoIntegracao)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortariaAutenticacao()
            {
                ApiKey = configuracaoIntegracao.ApiToken
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaParametros ObterDadosParametrosMontarCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, string protocolo, Dominio.Entidades.Cliente remetente)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaParametros()
            {
                CodigoIntegracaoFilial = carga.Filial != null ? string.Join(", ", carga.Filial.OutrosCodigosIntegracao) ?? "" : "",
                CNPJTransportadoraCarga = !carga.FreteDeTerceiro ? carga.Empresa?.CNPJ_SemFormato ?? carga.CPFPrimeiroMotorista : "",
                CNPJRemetentePedido = remetente.CPF_CNPJ_SemFormato ?? "",
                DataCarregamentoCarga = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                NumeroDoca = !string.IsNullOrWhiteSpace(carga.NumeroDoca) ? carga.NumeroDoca : "0",
                CpfMotorista = carga.CPFPrimeiroMotorista ?? "",
                NomeMotorista = carga.NomePrimeiroMotorista ?? "",
                PlacaVeiculo = carga.PlacasVeiculos ?? "",
                ProtocoloDaCarga = carga.Protocolo.ToString() ?? "",
                RazaoSocialTransportadora = carga.Empresa?.RazaoSocial ?? "",
                TipoOperacao = "Carga",
                TipoAcao = !string.IsNullOrEmpty(protocolo) ? "Reprogramar" : "Programar"
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaObjeto> ObterDadosPedido(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaObjeto> listaPedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaObjeto>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                listaPedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.MontarCargaObjeto()
                {
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                });
            }

            return listaPedidos;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortariaParametros ObterDadosParametrosControlePortaria(Dominio.Entidades.Embarcador.Cargas.Carga carga, string protocolo, Dominio.Entidades.Cliente remetente)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Eship.ControlePortariaParametros()
            {
                CNPJRemetente = remetente.CPF_CNPJ_SemFormato ?? "",
                DataCarregamentoCarga = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                CNPJTransportadoraCarga = !carga.FreteDeTerceiro ? carga.Empresa?.CNPJ_SemFormato ?? carga.CPFPrimeiroMotorista : "",
                ProtocoloDaCarga = carga.Protocolo.ToString() ?? "",
                CodigoIntegracaoFilial = carga.Filial != null ? string.Join(", ", carga.Filial.OutrosCodigosIntegracao) ?? "" : "",
                TipoOperacao = "Carga",
                PlacaVeiculo = carga.PlacasVeiculos ?? "",
                ChavesNotasFiscais = "",
                NomeMotorista = carga.NomePrimeiroMotorista ?? "",
                CpfMotorista = carga.CPFPrimeiroMotorista ?? "",
                TipoAcao = !string.IsNullOrEmpty(protocolo) ? "Reprogramar" : "Programar"
            };
        }

        private void AtualizarStatusIntegracaoDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes repCargaDadosTransporte = new Repositorio.Embarcador.Cargas.CargaIntegracaoDadosTransportes(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaIntegracaoDadosTransporte = repCargaDadosTransporte.BuscarPorCargaETipoIntegracao(carga.Codigo, TipoIntegracao.Eship);

            if (cargaIntegracaoDadosTransporte == null)
                return;

            cargaIntegracaoDadosTransporte.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repCargaDadosTransporte.Atualizar(cargaIntegracaoDadosTransporte);
        }

        private void AtualizarStatusIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, TipoIntegracao.Eship);

            if (cargaIntegracao == null)
                return;

            cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        #endregion Métodos Privados

    }
}
