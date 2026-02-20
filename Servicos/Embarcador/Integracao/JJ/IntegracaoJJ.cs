using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.JJ
{
    public class IntegracaoJJ
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoJJ(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarChamado(Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao)
        {
            Repositorio.Embarcador.Chamados.ChamadoIntegracao repositorioChamadoIntegracao = new Repositorio.Embarcador.Chamados.ChamadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoJJ repositorioIntegracaoJJ = new Repositorio.Embarcador.Configuracoes.IntegracaoJJ(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ configuracaoIntegracao = repositorioIntegracaoJJ.Buscar();

            chamadoIntegracao.NumeroTentativas += 1;
            chamadoIntegracao.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                if (!(configuracaoIntegracao?.PossuiIntegracao ?? false))
                    throw new ServicoException("A integração com a JJ não está habilitada.");

                string url = $"{configuracaoIntegracao.URLIntegracaoAtendimento}/Nota/SalvaDevolucao";

                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.Chamado objetoChamado = ObterObjetoChamado(chamadoIntegracao, _unitOfWork);
                jsonRequisicao = JsonConvert.SerializeObject(objetoChamado, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.RetornoChamado retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.RetornoChamado>(jsonRetorno);

                    if (string.IsNullOrWhiteSpace(retorno.Protocolo))
                        throw new ServicoException(retorno.MensagemErro ?? "Falha não especificada no retorno padrão, verificar o histórico");

                    chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    chamadoIntegracao.ProblemaIntegracao = "Integrado com sucesso.";
                    chamadoIntegracao.ProtocoloDevolucao = retorno.Protocolo;
                    chamadoIntegracao.StatusDevolucao = retorno.Status;
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS JJ: {retornoRequisicao.StatusCode}");
            }
            catch (ServicoException excecao)
            {
                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                chamadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                chamadoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a JJ";
            }

            servicoArquivoTransacao.Adicionar(chamadoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioChamadoIntegracao.Atualizar(chamadoIntegracao);
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.Chamado ObterObjetoChamado(Dominio.Entidades.Embarcador.Chamados.ChamadoIntegracao chamadoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = chamadoIntegracao.Chamado;
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = chamadoIntegracao.CargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.Chamado()
            {
                Nota = xmlNotaFiscal.Numero.ToString(),
                Motivo = chamado.MotivoChamado.Descricao,
                InstituicaoGovernamental = (xmlNotaFiscal.Destinatario?.InstituicaoGovernamental ?? false) ? "t" : "f",
                TipoDevolucao = chamado.CargaEntrega.DevolucaoParcial ? 1 : 2,
                Justificativa = chamado.Observacao,
                Serie = xmlNotaFiscal.Serie,
                RealMotivo = chamado.RealMotivo?.Descricao ?? string.Empty,
                CNPJDestinatario = xmlNotaFiscal.Destinatario?.CPF_CNPJ_SemFormato ?? string.Empty,
                TipoCarga = chamado.Carga.TipoDeCarga?.ProdutoPredominante ?? string.Empty,
                Protocolo = chamadoIntegracao.ProtocoloDevolucao ?? string.Empty,
                Anexos = ObterObjetoChamadoAnexos(chamado, unitOfWork),
                Itens = ObterObjetoChamadoItens(chamadoIntegracao.CargaEntregaNotaFiscal)
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoAnexo> ObterObjetoChamadoAnexos(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoAnexo> listaAnexos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoAnexo>();

            foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo in chamado.Anexos)
            {
                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), chamadoAnexo.GuidArquivo + "." + chamadoAnexo.ExtensaoArquivo);
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                    continue;

                byte[] byteArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoArquivo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoAnexo objetoAnexo = new Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoAnexo()
                {
                    NomeArquivo = chamadoAnexo.NomeArquivoSemExtensao,
                    Extensao = chamadoAnexo.ExtensaoArquivo,
                    Base64 = Convert.ToBase64String(byteArquivo)
                };

                listaAnexos.Add(objetoAnexo);
            }

            return listaAnexos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoItem> ObterObjetoChamadoItens(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos;
            if (cargaEntregaNotaFiscal.CargaEntrega.DevolucaoParcial)
                cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarPorCargaEntregaENotaFiscalProdutosDevolvidos(cargaEntregaNotaFiscal.CargaEntrega.Codigo, cargaEntregaNotaFiscal.Codigo);
            else
                cargaEntregaProdutos = repositorioCargaEntregaProduto.BuscarPorCargaEntregaENotaFiscal(cargaEntregaNotaFiscal.CargaEntrega.Codigo, cargaEntregaNotaFiscal.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoItem> listaItens = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoItem>();
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto in cargaEntregaProdutos)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoItem objetoItem = new Dominio.ObjetosDeValor.Embarcador.Integracao.JJ.ChamadoItem()
                {
                    CodigoProduto = cargaEntregaProduto.Produto.CodigoProdutoEmbarcador,
                    Quantidade = (int)cargaEntregaProduto.QuantidadeDevolucao,
                    DataCritica = cargaEntregaProduto.DataCritica?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") ?? null,
                    Lote = cargaEntregaProduto.Lote
                };

                listaItens.Add(objetoItem);
            }

            return listaItens;
        }

        private HttpClient CriarRequisicao(string url, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoJJ configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoJJ));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

            return requisicao;
        }

        #endregion
    }
}
