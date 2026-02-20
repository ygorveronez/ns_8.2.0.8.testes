using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.Vedacit
{
    public sealed class IntegracaoVedacit
    {
        #region Propriedades Privadas

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVedacit _configuracaoIntegracao;

        #endregion Propriedades Privadas

        #region Construtores

        public IntegracaoVedacit(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos - Integrações

        public void IntegracarCanhoto(Dominio.Entidades.Embarcador.Canhotos.CanhotoIntegracao canhotoIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Repositorio.Embarcador.Canhotos.CanhotoIntegracao repositorioCanhotoIntegracao = new Repositorio.Embarcador.Canhotos.CanhotoIntegracao(_unitOfWork);

            canhotoIntegracao.DataIntegracao = DateTime.Now;
            canhotoIntegracao.NumeroTentativas++;

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                ObterConfiguracaoIntegracaoCarga();

                HttpClient client = CriarRequisicao(_configuracaoIntegracao.Usuario, _configuracaoIntegracao.Senha);

                client.DefaultRequestHeaders.Add("sistema-origem", "TMS");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoRequest dados = ObterDadosRequisicaoIntegracaoCanhoto(canhotoIntegracao.Canhoto);

                jsonRequisicao = dados.ToJson(new JsonSerializerSettings(), Formatting.Indented);

                StringContent conteudo = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = client.PostAsync($"{_configuracaoIntegracao.URLIntegracao}/api/v1/cte/escrituracao", conteudo).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoErroResponse escrituracaoErroResponse = jsonRetorno.FromJson<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoErroResponse>();

                    throw new ServicoException(escrituracaoErroResponse.Erro?.Mensagem ?? "Problema ao tentar integrar com Vedacit.");
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoResponse escrituracaoResponse = jsonRetorno.FromJson<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoResponse>();

                if (!escrituracaoResponse.Status)
                    throw new ServicoException(escrituracaoResponse.Mensagem ?? "Problema ao tentar integrar com Vedacit.");

                canhotoIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (BaseException excecao)
            {
                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoVedacit");

                canhotoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                canhotoIntegracao.ProblemaIntegracao = "Problema ao tentar integrar com Vedacit.";
            }

            servicoArquivoTransacao.Adicionar(canhotoIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCanhotoIntegracao.Atualizar(canhotoIntegracao);
        }

        public void IntegrarCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                ObterConfiguracaoIntegracaoCarga();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CargaDadosTransporte cargaDadosTransporte = ObterCargaDadosTransporte(cargaDadosTransporteIntegracao.Carga);

                HttpClient client = CriarRequisicao(_configuracaoIntegracao.UsuarioIntegracaoCarga, _configuracaoIntegracao.SenhaIntegracaoCarga);

                client.DefaultRequestHeaders.Add("sistema-origem", "TMS");

                jsonRequisicao = cargaDadosTransporte.ToJson(new JsonSerializerSettings(), Formatting.Indented);

                StringContent conteudo = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = client.PostAsync($"{_configuracaoIntegracao.URLIntegracaoCarga}/api/v1/pedido/entrega", conteudo).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrWhiteSpace(jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.RetornoVedacit retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.RetornoVedacit>(jsonRetorno);

                    if (retornoIntegracao.Erro != null || !retornoRequisicao.IsSuccessStatusCode)
                        throw new ServicoException(retornoIntegracao.Erro?.Mensagem ?? "Problema ao tentar integrar com Vedacit.");
                }

                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integrado com Sucesso.";
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            catch (BaseException excecao)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "IntegracaoVedacit");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = excecao.Message;
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        #endregion Métodos Públicos - Integrações

        #region Métodos Privados

        private void ObterConfiguracaoIntegracaoCarga()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoVedacit repositorioConfiguracaoIntegracaoVedacit = new Repositorio.Embarcador.Configuracoes.IntegracaoVedacit(_unitOfWork);
            _configuracaoIntegracao = repositorioConfiguracaoIntegracaoVedacit.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracao == null || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Vedacit.");
        }

        private HttpClient CriarRequisicao(string usuario = "", string senha = "")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoVedacit));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(10);

            if (!string.IsNullOrWhiteSpace(usuario) && !string.IsNullOrWhiteSpace(senha))
                requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(usuario, senha);

            return requisicao;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Dados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoRequest ObterDadosRequisicaoIntegracaoCanhoto(Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto)
        {
            Repositorio.XMLCTe repositorioXMLCTe = new Repositorio.XMLCTe(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repositorioLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoRequest dados = new Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.Escrituracao.EscrituracaoRequest();

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCtePorCargaENotaFiscal(canhoto.Carga.Codigo, canhoto.XMLNotaFiscal.Codigo);

            if (cte == null)
            {
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repositorioLancamentoNFSManual.BuscarPorCargaNFS(canhoto.Carga.Codigo, canhoto.XMLNotaFiscal.Codigo);

                if (lancamentoNFSManual == null)
                    throw new ServicoException("Não foi possível encontrar a Nota Fiscal.");

                dados.TipoNFe = "NFS";
                dados.InputXML = lancamentoNFSManual.DadosNFS.XML;
                dados.Protocolo = cte.Codigo.ToString();
            }
            else
            {
                string xml = repositorioXMLCTe.BuscarXMLPorCodigoCTe(cte.Codigo);

                dados.TipoNFe = "CTE";
                dados.InputXML = xml;
                dados.Protocolo = cte.Codigo.ToString();
            }

            if (string.IsNullOrWhiteSpace(dados.InputXML))
                throw new ServicoException("Não foi possível encontrar o XML.");

            dados.CodigoAcao = "S";

            return dados;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CargaDadosTransporte ObterCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CargaDadosTransporte
            {
                Pedidos = ObterPedidosCargaDadosTransporte(carga)
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Pedido> ObterPedidosCargaDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoAdicional repositorioPedidoAdicionais = new Repositorio.Embarcador.Pedidos.PedidoAdicional(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> pedidosAdicionais = repositorioPedidoAdicionais.BuscarPorPedidos(cargaPedidos.Select(o => o.Pedido.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> listaPedidosProdutos = repositorioPedidoProduto.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Pedido> pedidosCargaDadosTransporte = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Pedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos = listaPedidosProdutos.FindAll(pedidoProduto => pedidoProduto.Pedido.Codigo == pedido.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional pedidoAdicionais = pedidosAdicionais.Find(pedidoAdicionais => pedidoAdicionais.Pedido.Codigo == pedido.Codigo);

                pedidosCargaDadosTransporte.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Pedido
                {
                    DataEntregaProgramada = pedido.DataPrevisaoSaida?.ToString("yyyy-MM-dd'T'HH:mm:ssZ") ?? string.Empty,
                    DataRetiradaProgramada = pedido.PrevisaoEntrega?.ToString("yyyy-MM-dd'T'HH:mm:ssZ") ?? string.Empty,
                    CodigoFrete = pedido.TipoPagamento.ObterTipoCondicaoPagamento()?.ObterDescricao() ?? string.Empty,
                    IdentificacaoPedido = pedido.Filial?.CodigoFilialEmbarcador ?? string.Empty,
                    TipoPedido = pedidoAdicionais?.GrupoFreteMaterial ?? string.Empty,
                    NumeroPedido = pedido.CodigoPedidoCliente ?? string.Empty,
                    DataSolicitacao = pedidoAdicionais?.DataCriacaoRemessa?.ToString("yyyy-MM-dd'T'HH:mm:ssZ") ?? string.Empty,
                    DataPedido = pedidoAdicionais.DataCriacaoVenda?.ToString("yyyy-MM-dd'T'HH:mm:ssZ") ?? string.Empty,
                    ProtocoloCarga = carga.Protocolo.ToString() ?? string.Empty,
                    ProtocoloRelease = pedido.Protocolo.ToString() ?? string.Empty,
                    NumeroCarga = carga.CodigoCargaEmbarcador ?? string.Empty,
                    Praca = pedido.ObservacaoEntrega ?? string.Empty,
                    Rota = carga.Rota?.Descricao ?? string.Empty,
                    Transportadora = carga.Empresa?.CodigoIntegracao ?? string.Empty,
                    NomeTransportadora = carga.Empresa?.RazaoSocial ?? string.Empty,
                    CodigoOperacao = carga.TipoOperacao?.CodigoIntegracao ?? string.Empty,
                    Itens = ObterItensPedidoCargaDadosTransporte(pedido, pedidoProdutos),
                    Cliente = ObterClientePedidoCargaDadosTransporte(pedido, cargaPedido.OrdemEntrega),
                    ReferenciaEnvio = ObterReferenciaEnvioPedidoCargaDadosTransporte(pedido)
                });
            }

            return pedidosCargaDadosTransporte;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Item> ObterItensPedidoCargaDadosTransporte(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoProduto> pedidoProdutos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Item> itensPedido = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Item>();

            if (pedidoProdutos == null || pedidoProdutos.Count == 0)
                return itensPedido;

            StringBuilder erros = new StringBuilder();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in pedidoProdutos)
            {
                if (string.IsNullOrWhiteSpace(pedidoProduto.CamposPersonalizados))
                    continue;

                try
                {
                    // Validamos se o json é válido
                    JToken.Parse(pedidoProduto.CamposPersonalizados);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CampoPersonalizado campoPersonalizado = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.CampoPersonalizado>(pedidoProduto.CamposPersonalizados);

                    itensPedido.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Item
                    {
                        CodigoProduto = pedidoProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                        ValorRelease = campoPersonalizado.Item.ValorRelease,
                        Release = pedido.NumeroPedidoEmbarcador ?? string.Empty,
                        Unidade = pedidoProduto.Produto.Unidade?.Sigla ?? string.Empty,
                        QuantidadePedidoTransporte = campoPersonalizado.Item.QuantidadePedidoTransporte ?? string.Empty,
                        Quantidade = pedidoProduto.Quantidade.ToString() ?? string.Empty,
                        Reservado = campoPersonalizado.Item.Reservado ?? string.Empty,
                        ValorPrecoUnidade = pedidoProduto.PrecoUnitario,
                        UnidadeMedidaDigitada = campoPersonalizado.Item.UnidadeMedidaDigitada ?? string.Empty,
                        UnidadeMedidaEntrada = campoPersonalizado.Item.UnidadeMedidaDigitada ?? string.Empty,
                        NumeroLinha = campoPersonalizado.Item.NumeroLinha
                    });
                }
                catch (JsonReaderException jsonExcecao)
                {
                    Log.TratarErro(jsonExcecao, "IntegracaoVedacit");

                    erros.Append($"JSON inválido para o produto: {pedidoProduto.Produto.CodigoProdutoEmbarcador}\n");
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao, "IntegracaoVedacit");

                    erros.Append($"Ocorreu um erro ao desserializar o produto: {pedidoProduto.Produto.CodigoProdutoEmbarcador}\n");
                }
            }

            if (erros.Length > 0)
                throw new ServicoException(erros.ToString());

            return itensPedido;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Cliente ObterClientePedidoCargaDadosTransporte(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, int ordemEntrega)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.Cliente
            {
                Cidade = pedido.Destino?.Descricao ?? string.Empty,
                IBGE = pedido.Destino?.CodigoIBGE.ToString() ?? string.Empty,
                UF = pedido.Destino?.Estado?.Sigla ?? string.Empty,
                Estado = pedido.Destino?.Estado?.Descricao ?? string.Empty,
                Endereco = pedido.Destinatario?.Endereco ?? string.Empty,
                NumeroReferenciaEntrega = pedido.Destinatario?.Numero ?? string.Empty,
                Complemento = pedido.Destinatario?.Complemento ?? string.Empty,
                CEP = pedido.Destinatario?.CEP ?? string.Empty,
                Bairro = pedido.Destinatario?.Bairro ?? string.Empty,
                LinhaEntrega = ordemEntrega,
                CNPJ = pedido.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                InscricaoEstadual = pedido.Destinatario?.IE_RG ?? "ISENTO",
                InscricaoMunicipal = pedido.Destinatario?.InscricaoMunicipal ?? string.Empty,
                Nome = pedido.Destinatario?.Nome ?? string.Empty,
                CodigoCadastro = pedido.Destinatario?.CodigoIntegracao ?? string.Empty,
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.ReferenciaEnvio ObterReferenciaEnvioPedidoCargaDadosTransporte(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Vedacit.CargaDadosTransporte.ReferenciaEnvio
            {
                Cidade = pedido.EnderecoDestino?.Localidade?.Descricao ?? string.Empty,
                IBGE = pedido.EnderecoDestino?.Localidade?.CodigoIBGE.ToString() ?? string.Empty,
                Estado = pedido.EnderecoDestino?.Localidade?.Estado?.Descricao ?? string.Empty,
                Endereco = pedido.EnderecoDestino?.Endereco ?? string.Empty,
                NumeroReferenciaEntrega = pedido.EnderecoDestino?.Numero ?? string.Empty,
                Complemento = pedido.EnderecoDestino?.Complemento ?? string.Empty,
                CEP = pedido.EnderecoDestino?.CEP ?? string.Empty,
                Bairro = pedido.EnderecoDestino?.Bairro ?? string.Empty,
                LinhaEntrega = Convert.ToDecimal(pedido.Destinatario?.CodigoIntegracao ?? ""),
                InscricaoMunicipal = pedido.Destinatario?.InscricaoMunicipal ?? string.Empty,
                CodigoCadastro = pedido.Destinatario?.CodigoIntegracao ?? string.Empty,
                UF = pedido.EnderecoDestino?.Localidade?.Estado.Sigla ?? string.Empty,
                InscricaoEstadual = pedido.Destinatario?.IE_RG ?? "ISENTO",
                Nome = pedido.Destinatario?.Nome ?? string.Empty,
                CNPJ = pedido.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
            };
        }

        #endregion Métodos Privados - Dados
    }
}
