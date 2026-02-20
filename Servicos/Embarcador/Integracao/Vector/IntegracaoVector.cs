using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Vector
{
    public class IntegracaoVector
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVector _configuracaoIntegracaoVector;

        #endregion Atributos Privados

        #region Construtores

        public IntegracaoVector(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCargaCancelamentoRecebimentoViagemStatus(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            IntegrarRecebimentoViagemStatus(cargaCancelamentoCargaIntegracao, cargaCancelamentoCargaIntegracao.CargaCancelamento.Carga, out string jsonRequisicao, out string jsonRetorno, "VE");

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoCargaIntegracao);
        }

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            IntegrarRecebimentoViagemStatus(cargaCargaIntegracao, cargaCargaIntegracao.Carga, out string jsonRequisicao, out string jsonRetorno, "VF");

            servicoArquivoTransacao.Adicionar(cargaCargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaCargaIntegracao.Atualizar(cargaCargaIntegracao);
        }

        public void IntegrarRecebimentoViagem(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                ObterConfiguracaoIntegracaoVector();

                string token = ObterToken();
                HttpClient requisicao = CriarRequisicao(token);

                RecebimentoCarga corpoRequisicao = PreencherCorpoRequisicaoRecebimentoViagem(cargaDadosTransporteIntegracao.Carga);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracaoVector.URLIntegracao + "/api/integrationEtcdTransportation/ReceiveTrip", conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Vector.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RetornoVector retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RetornoVector>(jsonRetorno);

                if (!retornoIntegracao.Sucesso || !retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException(retornoIntegracao.Mensagem ?? "Problema ao tentar integrar com Vector.");

                //Desativado Temporariamente somente na 8.2 por causa desse -> BUG #99710 Vector | Onda 2 - APIs de integração e rateio do frete
                //cargaDadosTransporteIntegracao.Carga.ObrigatorioInformarValorFreteOperador = true;
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso!";

            }
            catch (ServicoException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoVector");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Problema ao tentar integrar com Vector.";
            }

            repositorioCarga.Atualizar(cargaDadosTransporteIntegracao.Carga);

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados Genéricos

        private HttpClient CriarRequisicao(string token = "")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoVector));

            requisicao.Timeout = TimeSpan.FromMinutes(5);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(token))
                requisicao.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            return requisicao;
        }

        private string ObterToken()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient(_configuracaoIntegracaoVector.URLIntegracao + "/connect/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _configuracaoIntegracaoVector.ClientID);
            request.AddParameter("client_secret", _configuracaoIntegracaoVector.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RetornoToken>(response.Content);

            return retorno.TokenAcesso;
        }

        private void ObterConfiguracaoIntegracaoVector()
        {
            if (_configuracaoIntegracaoVector != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVector repositorioIntegracaoVector = new Repositorio.Embarcador.Configuracoes.IntegracaoVector(_unitOfWork);
            _configuracaoIntegracaoVector = repositorioIntegracaoVector.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracaoVector == null || !_configuracaoIntegracaoVector.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Vector.");
        }

        private bool ClienteValido(List<Parada> paradas, Dominio.Entidades.Cliente cliente, TipoEntrega tipoEntrega)
        {
            if (cliente == null)
                return false;

            return !paradas.Exists(parada => parada.CPFCNPJ == cliente.CPF_CNPJ_SemFormato && parada.TipoEntrega == tipoEntrega);
        }

        private void IntegrarRecebimentoViagemStatus<T>(T integracao, Dominio.Entidades.Embarcador.Cargas.Carga carga, out string jsonRequisicao, out string jsonRetorno, string situacaoViagem) where T : Dominio.Entidades.Embarcador.Integracao.Integracao
        {
            jsonRequisicao = string.Empty;
            jsonRetorno = string.Empty;

            try
            {
                integracao.NumeroTentativas++;
                integracao.DataIntegracao = DateTime.Now;

                ObterConfiguracaoIntegracaoVector();

                string token = ObterToken();
                HttpClient requisicao = CriarRequisicao(token);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.RecebimentoCargaStatus corpoRequisicao = PreencherCorpoRequisicaoRecebimentoViagemStatus(carga, situacaoViagem);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(_configuracaoIntegracaoVector.URLIntegracao + "/api/IntegrationEtcdTransportation/ReceiveTripOfferStatus", conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Houve um erro interno no servidor requisitado Vector.");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RetornoVector retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RetornoVector>(jsonRetorno);

                if (!retornoIntegracao.Sucesso || !retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException(retornoIntegracao.Mensagem ?? "Problema ao tentar integrar com Vector.");

                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracao.ProblemaIntegracao = "Integração realizada com sucesso!";
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoVector");

                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Problema ao tentar integrar com Vector.";
            }
        }

        #endregion Métodos Privados Genéricos

        #region Métodos Privados Dados

        private RecebimentoCarga PreencherCorpoRequisicaoRecebimentoViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (string.IsNullOrWhiteSpace(carga.CodigoCargaEmbarcador))
                throw new ServicoException("Código da carga não encontrado ou não existe.");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga.Carga> cargas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga.Carga>
            {
                new Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCarga.Carga
                {
                    NumeroCarga = carga.CodigoCargaEmbarcador,
                    Protocolo = carga.Protocolo,
                    DataCricao = carga.DataCriacaoCarga,
                    ModeloVeicular = carga.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    ValorFrete = 1,
                    ValorTotalFrete = 1,
                    TipoCarga = "MATERIAIS DE CONSTRUÇÃO EM GERAL",
                    IdentificadorFormaDivulgacao = "0",
                    Paradas = ObterParadas(carga.Codigo),
                }
            };

            return new RecebimentoCarga
            {
                Cargas = cargas
            };
        }

        private List<Parada> ObterParadas(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repositorioCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido = repositorioCargaPedido.BuscarPorCarga(codigoCarga);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repositorioCargaPedidoProduto.BuscarPorCarga(codigoCarga);
            List<Parada> paradas = new List<Parada>();

            int ordemEntrega = 0;
            string identificadorExpedidor = string.Empty;
            TipoEntrega? ultimoTipoEntregaOrdenado = null;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedido.OrderBy(cargaPedido => cargaPedido.OrdemEntrega))
            {
                Dominio.Entidades.Cliente tomador = cargaPedido.ObterTomador();

                if (ClienteValido(paradas, cargaPedido.Pedido.Remetente, TipoEntrega.Remetente))
                    paradas.Add(ObterParada(cargaPedido.Pedido.Remetente, TipoEntrega.Remetente, ObterOrdemEntrega(ref ultimoTipoEntregaOrdenado, TipoEntrega.Remetente, ref ordemEntrega), cargaPedido.Pedido.EnderecoOrigem));

                if (ClienteValido(paradas, (cargaPedido.Expedidor ?? cargaPedido.Pedido.Remetente), TipoEntrega.ConsignatarioExpedidor))
                {
                    paradas.Add(ObterParada(cargaPedido.Expedidor ?? cargaPedido.Pedido.Remetente, TipoEntrega.ConsignatarioExpedidor, ObterOrdemEntrega(ref ultimoTipoEntregaOrdenado, TipoEntrega.ConsignatarioExpedidor, ref ordemEntrega)));
                    identificadorExpedidor = paradas.Last().IdentificadorParada;
                }

                if (ClienteValido(paradas, cargaPedido.Pedido.Destinatario, TipoEntrega.Destinatario))
                {
                    int quantidadeNotasFiscais = cargasPedido.Count(c => c.Pedido.Destinatario.CPF_CNPJ.Equals(cargaPedido.Pedido.Destinatario.CPF_CNPJ));
                    paradas.Add(ObterParada(cargaPedido.Pedido.Destinatario, TipoEntrega.Destinatario, ObterOrdemEntrega(ref ultimoTipoEntregaOrdenado, TipoEntrega.Destinatario, ref ordemEntrega), cargaPedido.Pedido.EnderecoDestino, cargaPedidoProdutos: null, identificadorExpedidor: string.Empty, quantidadeNotasFiscais));
                }

                Dominio.Entidades.Cliente recebedor = cargaPedido.Recebedor ?? cargaPedido.Pedido.Destinatario;

                if (ClienteValido(paradas, recebedor, TipoEntrega.ConsignatarioRecebedor))
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtos = cargaPedidoProdutos.FindAll(cargaPedidoProduto => recebedor.CPF_CNPJ.Equals(cargaPedidoProduto.CargaPedido.Recebedor?.CPF_CNPJ) || recebedor.CPF_CNPJ.Equals(cargaPedidoProduto.CargaPedido.Pedido.Destinatario?.CPF_CNPJ));
                    paradas.Add(ObterParada(recebedor, TipoEntrega.ConsignatarioRecebedor, ObterOrdemEntrega(ref ultimoTipoEntregaOrdenado, TipoEntrega.ConsignatarioRecebedor, ref ordemEntrega), null, produtos, identificadorExpedidor));
                }

                if (ClienteValido(paradas, tomador, TipoEntrega.TomadorServico))
                    paradas.Add(ObterParada(tomador, TipoEntrega.TomadorServico, ObterOrdemEntrega(ref ultimoTipoEntregaOrdenado, TipoEntrega.TomadorServico, ref ordemEntrega)));

                ultimoTipoEntregaOrdenado = null;
                identificadorExpedidor = string.Empty;
            }

            return paradas;
        }

        private int ObterOrdemEntrega(ref TipoEntrega? ultimoTipoEntregaOrdenado, TipoEntrega tipoEntregaAtual, ref int ordemEntrega)
        {
            if ((ultimoTipoEntregaOrdenado == TipoEntrega.Remetente || ultimoTipoEntregaOrdenado == TipoEntrega.ConsignatarioExpedidor) &&
                (tipoEntregaAtual == TipoEntrega.Remetente || tipoEntregaAtual == TipoEntrega.ConsignatarioExpedidor))
            {
                return ordemEntrega;
            }

            if ((ultimoTipoEntregaOrdenado == TipoEntrega.TomadorServico || ultimoTipoEntregaOrdenado == TipoEntrega.Destinatario || ultimoTipoEntregaOrdenado == TipoEntrega.ConsignatarioRecebedor) &&
                (tipoEntregaAtual == TipoEntrega.TomadorServico || tipoEntregaAtual == TipoEntrega.Destinatario || tipoEntregaAtual == TipoEntrega.ConsignatarioRecebedor))
            {
                return ordemEntrega;
            }

            ordemEntrega++;
            ultimoTipoEntregaOrdenado = tipoEntregaAtual;

            return ordemEntrega;
        }

        private List<Entrega> ObterEntregas(List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos)
        {
            List<Entrega> entregas = new List<Entrega>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                entregas.Add(new Entrega
                {
                    Descricao = cargaPedidoProduto.Produto.Descricao,
                    NCM = cargaPedidoProduto.Produto.CodigoNCM ?? "1",
                    QuantidadeTotal = cargaPedidoProduto.PesoTotal,
                    UnidadeMedida = cargaPedidoProduto.Produto.Unidade?.Sigla ?? "KG",
                    ValorTotalMercadoria = cargaPedidoProduto.ValorTotal
                });

            return entregas;
        }

        private Parada ObterParada(Dominio.Entidades.Cliente cliente, TipoEntrega tipoEntrega, int ordemEntrega, Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco endereco = null, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = null, string identificadorExpedidor = "", int quantidadeNotasFiscais = 0)
        {
            string longitude = endereco?.Localidade?.Longitude.ToString().Replace(",", ".") ?? cliente.Localidade.Longitude.ToString().Replace(",", ".");
            string latitude = endereco?.Localidade?.Latitude.ToString().Replace(",", ".") ?? cliente.Localidade.Latitude.ToString().Replace(",", ".");

            if (string.IsNullOrWhiteSpace(longitude) || string.IsNullOrWhiteSpace(latitude))
                throw new ServicoException($"Latitude e/ou Longitude do cliente {cliente.Nome} é inválida");

            List<Entrega> entregas = null;
            string identificadorParada = $"{(int)tipoEntrega}:{cliente.CodigoIntegracao ?? cliente.CPF_CNPJ_SemFormato}";

            if ((cargaPedidoProdutos?.Count > 0) && (tipoEntrega == TipoEntrega.ConsignatarioRecebedor))
                entregas = ObterEntregas(cargaPedidoProdutos);

            return new Parada
            {
                IdentificadorParada = identificadorParada,
                IdentificadorCarregamentoParada = identificadorExpedidor,
                LocalColeta = (tipoEntrega == TipoEntrega.ConsignatarioExpedidor),
                Nome = cliente.Nome ?? string.Empty,
                CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                Email = cliente.Email ?? string.Empty,
                InscricaoEstadual = cliente.IE_RG ?? string.Empty,
                Endereco = endereco?.Endereco ?? cliente.Endereco,
                Numero = endereco?.Numero ?? cliente.Numero,
                Complemento = endereco?.Complemento ?? cliente.Complemento,
                CEP = endereco?.CEP ?? cliente.CEP,
                Bairro = endereco?.Bairro ?? cliente.Bairro,
                CodigoIBGE = cliente.Localidade.CodigoIBGE,
                Estado = endereco?.Localidade?.Estado?.Sigla ?? cliente.Localidade.Estado.Sigla,
                Pais = endereco?.Localidade?.Pais?.Descricao ?? cliente.Localidade.Pais?.Descricao,
                Cidade = endereco?.Localidade?.Descricao ?? cliente.Localidade.Descricao,
                Longitude = longitude,
                Latitude = latitude,
                SequenciaEntrega = ordemEntrega,
                TipoEntrega = tipoEntrega,
                Entregas = entregas,
                NumeroNotasFiscais = quantidadeNotasFiscais
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.RecebimentoCargaStatus PreencherCorpoRequisicaoRecebimentoViagemStatus(Dominio.Entidades.Embarcador.Cargas.Carga carga, string situacaoViagem)
        {
            if (string.IsNullOrWhiteSpace(carga.CodigoCargaEmbarcador))
                throw new ServicoException("Código da carga não encontrado ou não existe.");

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.RecebimentoCargaStatus
            {
                Status = ObterTodosStatus(carga, situacaoViagem)
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.Status> ObterTodosStatus(Dominio.Entidades.Embarcador.Cargas.Carga carga, string situacaoViagem)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.Status> status = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.Status>();

            List<Parada> paradas = ObterParadas(carga.Codigo);

            List<string> identificadores = paradas.Select(parada => parada.IdentificadorParada).ToList();

            foreach (string identificador in identificadores)
                status.Add(ObterStatus(carga, identificador, situacaoViagem));

            return status;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.Status ObterStatus(Dominio.Entidades.Embarcador.Cargas.Carga carga, string identificador, string situacaoViagem)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Vector.RecebimentoCargaStatus.Status
            {
                IdentificadorParada = identificador,
                NumeroCarga = carga.CodigoCargaEmbarcador,
                Protocolo = carga.Protocolo,
                SituacaoViagem = situacaoViagem
            };
        }

        #endregion Métodos Privados Dados
    }
}
