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
using System.Text;

namespace Servicos.Embarcador.Integracao.Extratta
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio _integracaoExtratta;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void GerarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            _integracaoExtratta = servicoValePedagio.ObterIntegracaoExtratta(carga, tipoServicoMultisoftware);
            if (!ValidarConfiguracaoIntegracao(cargaValePedagio))
                return;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPrimeiroPorCarga(carga.Codigo);
            if (pedido == null || pedido.Remetente == null || pedido.Destinatario == null)
            {
                cargaValePedagio.ProblemaIntegracao = "Não foi possível identificar cliente de origem/destino para realizar integração na Extratta.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);
            }

            string codigoIdentificadorRota = cargaValePedagio.RotaTemporaria;//Se já retornou a rota, não buscar novamente
            if (_integracaoExtratta.TipoRota == TipoRotaExtratta.RotaDinamica && string.IsNullOrWhiteSpace(codigoIdentificadorRota))
            {
                codigoIdentificadorRota = ConsultarCustoPedagioRota(cargaValePedagio);
                if (string.IsNullOrWhiteSpace(codigoIdentificadorRota))
                    return;
            }

            //Efetua cadastros necessários, como no CIOT
            Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade = Servicos.Embarcador.CIOT.CIOT.ObterModalidadeTransportador(carga.Terceiro, _unitOfWork);

            if (!IntegrarCliente(cargaValePedagio, pedido.Remetente, true) ||
                !IntegrarCliente(cargaValePedagio, pedido.Destinatario, false) ||
                !IntegrarMotorista(cargaValePedagio, modalidade) ||
                !IntegrarVeiculos(cargaValePedagio, modalidade))
                return;

            //Compra o vale pedágio
            IntegrarValePedagioViagemAvulsa(cargaValePedagio, pedido);
        }

        public void SolicitarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            _integracaoExtratta = servicoValePedagio.ObterIntegracaoExtratta(cargaValePedagio.Carga, tipoServicoMultisoftware);
            if (!ValidarConfiguracaoIntegracao(cargaValePedagio))
                return;

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CancelamentoViagem objetoCancelamentoViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.CancelamentoViagem()
                {
                    CNPJAplicacao = _integracaoExtratta.CNPJAplicacao,
                    Token = _integracaoExtratta.Token,
                    DataEvento = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    CodigoViagem = cargaValePedagio.IdCompraValePedagio.ToInt()
                };

                string url = $"{_integracaoExtratta.URL}/Viagem/CancelarViagem";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(objetoCancelamentoViagem, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Extratta: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoCancelamentoViagem retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoCancelamentoViagem>(jsonRetorno);
                if (!retorno.Sucesso)
                    throw new ServicoException(retorno.Mensagem);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cancelamento do vale pedágio";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public byte[] ObterReciboPdf(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ref string mensagemRetorno)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtrattaValePedagio integracaoExtratta = servicoValePedagio.ObterIntegracaoExtratta(cargaValePedagio.Carga, tipoServicoMultisoftware);
            if (integracaoExtratta == null)
                return null;

            byte[] arquivoRetorno = null;

            try
            {
                //Exemplo teste: "https://api.extratta.com.br:2110/ViagemAts/ReciboVPO/1"
                string url = $"{integracaoExtratta.URL}/ViagemAts/ReciboVPO/{cargaValePedagio.IdCompraValePedagio}";

                HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));
                HttpResponseMessage retornoRequisicao = requisicao.GetAsync(url).Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Retorno obter recibo: { (int)retornoRequisicao.StatusCode }");

                if (retornoRequisicao.Content.Headers.ContentType.MediaType == "application/json")
                {
                    string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(retornoRequisicao.Content.ReadAsStringAsync().Result);
                    throw new ServicoException((string)retorno.message);
                }
                else
                    arquivoRetorno = retornoRequisicao.Content.ReadAsByteArrayAsync().Result;
            }
            catch (ServicoException excecao)
            {
                mensagemRetorno = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemRetorno = "Falha ObterReciboPdf Extratta";
            }

            return arquivoRetorno;
        }

        #endregion

        #region Métodos Privados

        private string ConsultarCustoPedagioRota(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string codigoIdentificadorRota = "";

            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRota objetoConsultaCustoPedagioRota = ObterConsultaCustoPedagioRota(cargaValePedagio);

                string url = $"{_integracaoExtratta.URL}/Viagem/ConsultarCustoPedagioRota";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(objetoConsultaCustoPedagioRota, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Extratta: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.RetornoConsultaCustoPedagioRota retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.RetornoConsultaCustoPedagioRota>(jsonRetorno);
                if (retorno.Status == 0)
                    throw new ServicoException(retorno.Mensagem);

                if (retorno.CustoTotal > 0)
                {
                    codigoIdentificadorRota = retorno.IdentificadorHistorico;
                    cargaValePedagio.RotaTemporaria = codigoIdentificadorRota;
                    cargaValePedagio.ProblemaIntegracao = "Consulta de Custo de Pedágios na Rota efetuada com sucesso";
                }
                else
                {
                    cargaValePedagio.ProblemaIntegracao = "Rota sem custo de Vale Pedágio na Extratta";
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                }
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao consultar o custo da rota";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            if (string.IsNullOrWhiteSpace(codigoIdentificadorRota))
                cargaValePedagio.NumeroTentativas++;

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return codigoIdentificadorRota;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRota ObterConsultaCustoPedagioRota(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRota request = new Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRota()
            {
                CNPJAplicacao = _integracaoExtratta.CNPJAplicacao,
                Token = _integracaoExtratta.Token,
                CNPJEmpresa = cargaValePedagio.Carga.Empresa.CNPJ,
                TipoVeiculo = 3,//Caminhão
                QtdEixos = ObterNumeroEixos(cargaValePedagio.Carga),
                ExibirDetalhes = 1,//True
                Localizacoes = ObterLocalizacoes(cargaValePedagio)
            };

            return request;
        }

        private int ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool eixosSuspensos = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio;

            int numeroEixos = 0;
            if (carga.Veiculo.ModeloVeicularCarga != null)
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

            return numeroEixos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRotaLocalizacao> ObterLocalizacoes(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            Dominio.Entidades.Localidade localidadeOrigem, localidadeDestino;
            List<Dominio.Entidades.Localidade> pontosPassagem;
            servicoLocalidade.ObterLocalidadesValePedagioCarga(cargaValePedagio.Carga, out localidadeOrigem, out localidadeDestino, out pontosPassagem, _unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRotaLocalizacao> localizacoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRotaLocalizacao>();

            localizacoes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRotaLocalizacao() { IbgeCidade = localidadeOrigem.CodigoIBGE });

            foreach (Dominio.Entidades.Localidade localidade in pontosPassagem)
                localizacoes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRotaLocalizacao() { IbgeCidade = localidade.CodigoIBGE });

            localizacoes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta.ConsultaCustoPedagioRotaLocalizacao() { IbgeCidade = localidadeDestino.CodigoIBGE });

            return localizacoes;
        }

        private void IntegrarValePedagioViagemAvulsa(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.IntegrarViagem integrarViagem = ObterIntegrarViagem(cargaValePedagio.Carga, pedido, cargaValePedagio);

                string url = $"{_integracaoExtratta.URL}/Viagem/Integrar";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(integrarViagem, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Extratta: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoIntegrarViagem retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoIntegrarViagem>(jsonRetorno);
                if (!retorno.Sucesso)
                    throw new ServicoException(retorno.Mensagem);

                cargaValePedagio.NumeroValePedagio = "";
                cargaValePedagio.IdCompraValePedagio = retorno.Objeto.CodigoViagem.ToString();

                if (retorno.Objeto.Pedagio.Status != 0)
                    throw new ServicoException(retorno.Objeto.Pedagio.Mensagem ?? retorno.Mensagem);

                cargaValePedagio.NumeroValePedagio = retorno.Objeto.Pedagio.ProtocoloProcessamento;
                cargaValePedagio.ValorValePedagio = retorno.Objeto.Pedagio.Valor ?? 0m;
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do vale pedágio";
            }

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.IntegrarViagem ObterIntegrarViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.IntegrarViagem integrarViagem = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.IntegrarViagem()
            {
                Token = _integracaoExtratta.Token,
                IdViagem = cargaValePedagio.IdCompraValePedagio.ToNullableInt(),
                CNPJAplicacao = _integracaoExtratta.CNPJAplicacao,
                CNPJEmpresa = carga.Empresa.CNPJ,
                CPFCNPJClienteOrigem = pedido.Remetente.CPF_CNPJ_SemFormato,
                CPFCNPJClienteDestino = pedido.Destinatario.CPF_CNPJ_SemFormato,
                CPFMotorista = carga.Motoristas.FirstOrDefault().CPF,
                Placa = carga.Veiculo.Placa,
                StatusViagem = 1,
                Carretas = carga.VeiculosVinculados?.Count > 0 ? carga.VeiculosVinculados.Select(o => o.Placa).ToList() : null,
                HabilitarDeclaracaoCiot = false,
                NumeroControle = $"PedagioAvulso{carga.Codigo}",
                ForcarCiotNaoEquiparado = false,
                Pedagio = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.Pedagio
                {
                    Fornecedor = (int)_integracaoExtratta.FornecedorParceiro,
                    IdentificadorHistorico = cargaValePedagio.RotaTemporaria,
                    IdRotaModelo = cargaValePedagio.CodigoIntegracaoValePedagio
                }
            };

            return integrarViagem;
        }

        private bool IntegrarCliente(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Cliente cliente, bool origem)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;
            string tipoCliente = origem ? "origem" : "destino";

            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ClienteIntegrar objetoCliente = ObterCliente(cliente, cargaValePedagio.Carga.Empresa);

                string url = $"{_integracaoExtratta.URL}/Cliente/Integrar";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(objetoCliente, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Extratta: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta>(jsonRetorno);
                if (!retorno.Sucesso)
                    throw new ServicoException(retorno.Mensagem);

                cargaValePedagio.ProblemaIntegracao = $"Cliente {tipoCliente} integrado com sucesso.";
                sucesso = true;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Retorno cliente {tipoCliente}: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Ocorreu uma falha ao integrar o cliente {tipoCliente}";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            if (!sucesso)
                cargaValePedagio.NumeroTentativas++;

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ClienteIntegrar ObterCliente(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ClienteIntegrar clienteIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.ClienteIntegrar()
            {
                CNPJAplicacao = _integracaoExtratta.CNPJAplicacao,
                Token = _integracaoExtratta.Token,
                CNPJEmpresa = empresa.CNPJ,
                IdCliente = null,
                BACENPais = cliente.Localidade.Pais?.Codigo ?? 0,
                IBGEEstado = cliente.Localidade.Estado.CodigoIBGE,
                IBGECidade = cliente.Localidade.CodigoIBGE,
                RazaoSocial = cliente.Nome,
                NomeFantasia = !string.IsNullOrWhiteSpace(cliente.NomeFantasia) ? cliente.NomeFantasia : cliente.Nome,
                TipoPessoa = cliente.Tipo == "F" ? 1 : 2,
                CNPJCPF = cliente.CPF_CNPJ_SemFormato,
                RG = cliente.Tipo == "F" ? cliente.IE_RG : string.Empty,
                OrgaoExpedidorRG = cliente.Tipo == "F" ? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRGHelper.ObterDescricao(cliente.OrgaoEmissorRG ?? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum) : string.Empty,
                IE = cliente.Tipo == "J" ? (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cliente.IE_RG)) ? Utilidades.String.OnlyNumbers(cliente.IE_RG).ToInt() : 0) : 0,
                Celular = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cliente.Celular)) ? Utilidades.String.OnlyNumbers(cliente.Celular).ToInt() : 0,
                Email = cliente.Email,
                CEP = cliente.CEP,
                Endereco = cliente.Endereco,
                Complemento = cliente.Complemento,
                Numero = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cliente.Numero)) ? Utilidades.String.OnlyNumbers(cliente.Numero).ToInt() : 0,
                Bairro = cliente.Bairro
            };

            return clienteIntegrar;
        }

        private bool IntegrarMotorista(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.Entidades.Usuario motorista = cargaValePedagio.Carga.Motoristas.FirstOrDefault();
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar objetoMotorista = ObterMotorista(motorista, cargaValePedagio.Carga.Empresa, modalidade);

                string url = $"{_integracaoExtratta.URL}/Motorista/Integrar";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(objetoMotorista, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Extratta: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoSucessoMensagemExtratta>(jsonRetorno);
                if (!retorno.Sucesso)
                    throw new ServicoException(retorno.Mensagem);

                cargaValePedagio.ProblemaIntegracao = "Motorista integrado com sucesso.";
                sucesso = true;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Retorno motorista: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao integrar o motorista";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            if (!sucesso)
                cargaValePedagio.NumeroTentativas++;

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar ObterMotorista(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar motoristaIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.MotoristaIntegrar()
            {
                CNPJAplicacao = _integracaoExtratta.CNPJAplicacao,
                Token = _integracaoExtratta.Token,
                CNPJEmpresa = empresa.CNPJ,
                Nome = motorista.Nome,
                RG = motorista.RG,
                RGOrgaoExpedidor = Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRGHelper.ObterDescricao(motorista.OrgaoEmissorRG ?? Dominio.ObjetosDeValor.Enumerador.OrgaoEmissorRG.Nenhum),
                CPF = motorista.CPF,
                Sexo = motorista.Sexo == Dominio.ObjetosDeValor.Enumerador.Sexo.Feminino ? "F" : "M",
                CNH = !string.IsNullOrWhiteSpace(motorista.NumeroHabilitacao) ? motorista.NumeroHabilitacao.PadLeft(11, '0') : string.Empty,
                CNHCategoria = !string.IsNullOrWhiteSpace(motorista.Categoria) ? motorista.Categoria : "C",
                ValidadeCNH = motorista.DataVencimentoHabilitacao.HasValue ? motorista.DataVencimentoHabilitacao.Value.ToString("yyyy-MM-dd") : string.Empty,
                Celular = motorista.Celular,
                TipoContrato = ObterTipoContrato(modalidade),
                Email = motorista.Email,
                CEP = motorista.CEP,
                Endereco = motorista.Endereco,
                Complemento = motorista.Complemento,
                Numero = motorista.NumeroEndereco,
                Bairro = motorista.Bairro,
                IBGECidade = motorista.Localidade?.CodigoIBGE ?? 0,
                IBGEEstado = motorista.Localidade?.Estado?.CodigoIBGE ?? 0,
                BACENPais = motorista.Localidade?.Pais?.Codigo ?? 0
            };

            return motoristaIntegrar;
        }

        private int ObterTipoContrato(Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            return modalidade?.TipoTransportador == TipoProprietarioVeiculo.TACAgregado ? 3 : 4; //Tipo do contrato: 1 = Frota; 2 = Cooperado; 3 = Agregado; 4 = Terceiro;
        }

        private bool IntegrarVeiculos(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            if (!IntegrarVeiculo(cargaValePedagio, cargaValePedagio.Carga.Veiculo, cargaValePedagio.Carga.Empresa, modalidade))
                return false;

            foreach (Dominio.Entidades.Veiculo veiculo in cargaValePedagio.Carga.VeiculosVinculados)
            {
                if (!IntegrarVeiculo(cargaValePedagio, veiculo, cargaValePedagio.Carga.Empresa, modalidade))
                    return false;
            }

            return true;
        }

        private bool IntegrarVeiculo(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            cargaValePedagio.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar objetoVeiculo = ObterVeiculo(veiculo, empresa, modalidade);

                string url = $"{_integracaoExtratta.URL}/Veiculo/Integrar";
                HttpClient requisicao = CriarRequisicao(url);

                jsonRequisicao = JsonConvert.SerializeObject(objetoVeiculo, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS Extratta: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoVeiculo retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.RetornoVeiculo>(jsonRetorno);
                if (!retorno.Sucesso)
                    throw new ServicoException(retorno.Mensagem);

                cargaValePedagio.ProblemaIntegracao = $"Veículo placa {veiculo.Placa} integrado com sucesso.";
                sucesso = true;
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Retorno veículo {veiculo.Placa}: {excecao.Message}";
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.ProblemaIntegracao = $"Ocorreu uma falha ao integrar o veículo {veiculo.Placa}";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            if (!sucesso)
                cargaValePedagio.NumeroTentativas++;

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar ObterVeiculo(Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidade)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar veiculoIntegrar = new Dominio.ObjetosDeValor.Embarcador.CIOT.Extratta.VeiculoIntegrar()
            {
                CNPJAplicacao = _integracaoExtratta.CNPJAplicacao,
                Token = _integracaoExtratta.Token,
                CNPJEmpresa = empresa.CNPJ,
                Placa = veiculo.Placa,
                Chassi = veiculo.Chassi,
                Renavam = !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(veiculo.Renavam)) ? Utilidades.String.OnlyNumbers(veiculo.Renavam).ToInt() : 0,
                AnoFabricacao = veiculo.AnoFabricacao,
                AnoModelo = veiculo.AnoModelo,
                Marca = veiculo.Marca?.Descricao ?? string.Empty,
                Modelo = veiculo.Modelo?.Descricao ?? string.Empty,
                ComTracao = veiculo.TipoVeiculo == "0" ? true : false,
                TipoContrato = ObterTipoContrato(modalidade),
                QuantidadeEixos = veiculo.ModeloVeicularCarga?.NumeroEixos ?? 0,
            };

            return veiculoIntegrar;
        }

        #endregion

        #region Métodos Privados - Configurações

        private bool ValidarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (_integracaoExtratta != null)
                return true;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Extratta.";
            cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;
            repositorioCargaValePedagio.Atualizar(cargaValePedagio);

            return false;
        }

        private HttpClient CriarRequisicao(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("CNPJAplicacao", _integracaoExtratta.CNPJAplicacao);
            requisicao.DefaultRequestHeaders.Add("Token", _integracaoExtratta.Token);
            requisicao.DefaultRequestHeaders.Add("CNPJEmpresa", _integracaoExtratta.CNPJAplicacao);

            return requisicao;
        }

        #endregion
    }
}
