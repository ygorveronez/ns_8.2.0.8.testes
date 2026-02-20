using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Servicos;
using Servicos.Embarcador.Integracao;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SGT.Italac.Thread
{
    public class IntegracaoItalac
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoItalac(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Globais 

        public void IntegrarLoteLiberacaoComercialPedido()
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(_unitOfWork);

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao> integracoes = repositorioLoteLiberacaoComercialPedidoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, TipoIntegracao.Italac);

            if (integracoes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracao in integracoes)
            {
                IntegrarLoteLiberacaoComercialPedidoPendenteIntegracao(integracao);
            }
        }

        public void IntegrarLoteLiberacaoComercialPedidoPendenteIntegracao(Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracaoPendente)
        {

            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoItalac repositorioIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoItalac(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;
            string mensagemErro = "Integrado com sucesso";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac configuracaoIntegracao = repositorioIntegracao.Buscar();

                if (configuracaoIntegracao == null)
                    throw new ServicoException("Configuração em REST da Italac não está completa, favor atualizar o cadastro da integração");

                string token = Login(configuracaoIntegracao);

                if (string.IsNullOrWhiteSpace(token))
                    throw new ServicoException("Não foi possivel gerar o Token.");

                HttpClient requisicao = CriarRequisicao(token);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas> requisicoes = PreencherRequisicao(integracaoPendente);

                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas pedido in requisicoes)
                {
                    jsonRequisicao = JsonConvert.SerializeObject(pedido);
                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                    HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLItalac + "/AOMS141/LiberaPedidoVendas", conteudoRequisicao).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                    dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                    if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    {
                        integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        integracaoPendente.LoteLiberacaoComercialPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteLiberacaoComercialPedido.Finalizado;
                        integracaoPendente.ProblemaIntegracao = (string)retornoIntegracao.mensagem;

                    }
                    else if (string.IsNullOrWhiteSpace((string)retornoIntegracao.mensagem))
                    {
                        mensagemErro = "Houve um erro interno no servidor requisitado.";
                        servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json", mensagemErro);
                        throw new ServicoException(mensagemErro);
                    }
                    else
                    {
                        mensagemErro = $"Pedido: {pedido.NumeroPedidoEmbarcador}, {(string)retornoIntegracao.mensagem}";
                        servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json", mensagemErro);
                        throw new ServicoException(mensagemErro);
                    }

                    servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json", mensagemErro);
                }
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.LoteLiberacaoComercialPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteLiberacaoComercialPedido.FalhaNaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoItalac");

                integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.LoteLiberacaoComercialPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteLiberacaoComercialPedido.FalhaNaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Italac.";
            }

            repositorioLoteLiberacaoComercialPedidoIntegracao.Atualizar(integracaoPendente);
        }

        #endregion

        #region Métodos Privados

        private string Login(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac configuracao)
        {
            HttpClient requisicao = CriarRequisicao();

            requisicao.DefaultRequestHeaders.Add("grant_type", "password");
            requisicao.DefaultRequestHeaders.Add("username", configuracao.UsuarioItalac);
            requisicao.DefaultRequestHeaders.Add("password", configuracao.SenhaItalac);

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync($"{configuracao.URLItalac}/api/oauth2/v1/token", null).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode == HttpStatusCode.Created)
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                return obj?.access_token;
            }
            else
            {
                return null;
            }

        }

        private HttpClient CriarRequisicao(string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoItalac));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(3);

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas> PreencherRequisicao(Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracaoPendente)
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado repositorioLoteLiberacaoComercialPedidoBloqueado = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);


            List<int> codigosPedidos = repositorioLoteLiberacaoComercialPedidoBloqueado.BuscarLoteBloqueadoPedidosPorCodigo(integracaoPendente.LoteLiberacaoComercialPedido.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repositorioPedido.BuscarPorCodigos(codigosPedidos);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas> pedidoBloqueados = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas>();


            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                pedidoBloqueados.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.LiberaPedidoVendas
                {
                    CnpjRemetentePedido = pedido.Remetente.CPF_CNPJ_SemFormato,
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                    CpfUsuario = integracaoPendente.LoteLiberacaoComercialPedido.Usuario?.CPF

                });
            }

            return pedidoBloqueados;
        }

        #endregion
    }
}