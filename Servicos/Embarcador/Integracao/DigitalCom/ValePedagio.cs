using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.DigitalCom
{
    public class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        private Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.ConfiguracaoIntegracao _configuracaoIntegracao;

        #endregion Atributos Globais

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ValePedagio(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion  Construtores

        #region Métodos Públicos

        public async Task GerarCompraValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Carga.ValePedagio.ValePedagio(_unitOfWork);

            ObterConfiguracaoIntegracao(cargaValePedagio.Carga);

            if (!ValidarConfiguracaoIntegracao(cargaValePedagio))
                return;

            await IntegrarValePedagio(cargaValePedagio);

            bool notificarTransportadorPorEmail = _configuracaoIntegracao?.IntegracaoDigitalComValePedagio?.NotificarTransportadorPorEmail ?? false;
            servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, notificarTransportadorPorEmail, tipoServicoMultisoftware);
        }

        public void SolicitarCancelamentoValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRetorno = "";
            string jsonRequisicao = "";
            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            try
            {
                ObterConfiguracaoIntegracao(cargaValePedagio.Carga);

                if (!ValidarConfiguracaoIntegracao(cargaValePedagio))
                    return;

                if (cargaValePedagio.CargaIntegracaoValePedagioCompra?.SituacaoValePedagio == SituacaoValePedagio.Cancelada)
                {
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                    cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso (Registro gerado para a volta com eixo suspenso)";

                    repositorioCargaValePedagio.Atualizar(cargaValePedagio);
                    return;
                }

                ObterToken();

                jsonRequisicao = $"[{(_configuracaoIntegracao.IntegracaoDigitalComValePedagio.EnviarNumeroCargaNoCampoDocumentoTransporte ? cargaValePedagio.Carga.CodigoCargaEmbarcador.ObterSomenteNumeros().ToLong() : cargaValePedagio.Carga.Codigo)}]";

                string url = $"{_configuracaoIntegracao.IntegracaoDigitalCom.EndpointDigitalCom}valePedagio/";
                WebResponse retornoRequest = CriarRequisicaoDelete(url, jsonRequisicao);

                if (((HttpWebResponse)retornoRequest).StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao conectar no WS DigitalCom: {((HttpWebResponse)retornoRequest).StatusCode}");

                using (System.IO.Stream streamDadosRetornoRequisicao = retornoRequest.GetResponseStream())
                {
                    System.IO.StreamReader leitorDadosRetornoRequisicao = new System.IO.StreamReader(streamDadosRetornoRequisicao);
                    jsonRetorno = leitorDadosRetornoRequisicao.ReadToEnd();

                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.DadosRetornoCancelamentoViagem> listaDadosRetornoRequisicao = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.DadosRetornoCancelamentoViagem>>(jsonRetorno);
                    Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.DadosRetornoCancelamentoViagem dadosRetornoRequisicao = listaDadosRetornoRequisicao.FirstOrDefault();

                    if (dadosRetornoRequisicao == null)
                        throw new ServicoException("Não foi possível obter o retorno da requisição.");

                    if (!dadosRetornoRequisicao.Status.ChaveMensagem.Contains("SUCESSO_CANCELAR_VIAGEM"))
                        throw new ServicoException($"{dadosRetornoRequisicao.Status.ChaveMensagem} - {dadosRetornoRequisicao.Status.Descricao}");

                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                    cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
                }

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
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
            finally
            {
                if (!string.IsNullOrWhiteSpace(jsonRequisicao) && !string.IsNullOrWhiteSpace(jsonRetorno))
                    servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

                repositorioCargaValePedagio.Atualizar(cargaValePedagio);
            }
        }

        private async Task IntegrarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;
            bool valePedagioLocalizado = false;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            try
            {
                ObterToken();

                DadosRetorno dadosRetorno = new DadosRetorno();

                if (cargaValePedagio.NumeroTentativas > 1)
                {
                    (DadosRetorno DadosRetorno, string jsonRequisicao, string jsonRetorno) possuiCompraPedagio = await PossuiCompraPedagio(cargaValePedagio.Carga);

                    dadosRetorno = possuiCompraPedagio.DadosRetorno;
                    cargaValePedagio.ProblemaIntegracao = dadosRetorno.Status.Descricao;

                    if (!string.IsNullOrWhiteSpace(possuiCompraPedagio.jsonRequisicao) && !string.IsNullOrWhiteSpace(possuiCompraPedagio.jsonRetorno))
                        servicoArquivoTransacao.Adicionar(cargaValePedagio, possuiCompraPedagio.jsonRequisicao, possuiCompraPedagio.jsonRetorno, "json");

                    valePedagioLocalizado = !dadosRetorno.Status.ChaveMensagem.Equals("VALE_PEDAGIO_NAO_LOCALIZADO");
                }

                if (!valePedagioLocalizado)
                {
                    RequisicaoValePedagio requisicaoValePedagio = ObterRequisicaoValePedagio(cargaValePedagio.Carga);

                    string url = $"{_configuracaoIntegracao.IntegracaoDigitalCom.EndpointDigitalCom}valePedagio";
                    HttpClient requisicao = CriarRequisicao(url);

                    jsonRequisicao = JsonConvert.SerializeObject(requisicaoValePedagio, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                    HttpResponseMessage retornoRequisicao = await requisicao.PostAsync(url, conteudoRequisicao);
                    jsonRetorno = await retornoRequisicao.Content.ReadAsStringAsync();

                    if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                        throw new ServicoException($"Falha ao conectar no WS Digital Com: {retornoRequisicao.StatusCode}");

                    RetornoValePedagio retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.RetornoValePedagio>(jsonRetorno);
                    dadosRetorno = retorno.DadosRetorno.FirstOrDefault();
                }

                bool rotaSemCusto = (dadosRetorno?.Status?.ChaveMensagem ?? string.Empty).Equals("SEM_PRACAS_PARA_ROTA");

                if (rotaSemCusto)
                {
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.RotaSemCusto;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaValePedagio.ProblemaIntegracao = "Não foram encontradas praças para a rota informada (Rota sem Custo).";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dadosRetorno?.Erro))
                        throw new ServicoException(dadosRetorno.Status.Descricao);

                    cargaValePedagio.IdCompraValePedagio = dadosRetorno?.Viagem?.FirstOrDefault()?.IdRecibo.ToString();
                    cargaValePedagio.NumeroValePedagio = dadosRetorno?.Viagem?.FirstOrDefault()?.NumeroViagem ?? string.Empty;
                    cargaValePedagio.CodigoEmissaoValePedagioANTT = dadosRetorno?.Viagem?.FirstOrDefault()?.IdVpoAntt ?? string.Empty;
                    cargaValePedagio.ValorValePedagio = dadosRetorno?.Viagem?.FirstOrDefault()?.TotalViagem ?? 0m;
                    cargaValePedagio.CnpjMeioPagamento = dadosRetorno?.CnpjMeioPagamento;
                    cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                    cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                    cargaValePedagio.ProblemaIntegracao = valePedagioLocalizado ? "Vale Pedágio Reemitido com Sucesso" : "Vale Pedágio Comprado com Sucesso";

                    SalvarReciboValePedagio(cargaValePedagio, dadosRetorno?.Viagem?.FirstOrDefault()?.Recibo);

                    if (dadosRetorno?.Viagem?.Count > 1)
                    {
                        cargaValePedagio.TipoPercursoVP = TipoRotaFrete.Ida;

                        Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagioRetorno = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio();
                        Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Viagem viagemRetorno = dadosRetorno.Viagem.LastOrDefault();

                        cargaValePedagioRetorno.Carga = cargaValePedagio.Carga;
                        cargaValePedagioRetorno.TipoRota = cargaValePedagio.TipoRota;
                        cargaValePedagioRetorno.TipoIntegracao = cargaValePedagio.TipoIntegracao;
                        cargaValePedagioRetorno.CargaIntegracaoValePedagioCompra = cargaValePedagio;

                        cargaValePedagioRetorno.IdCompraValePedagio = viagemRetorno?.IdRecibo.ToString();
                        cargaValePedagioRetorno.NumeroValePedagio = viagemRetorno?.NumeroViagem ?? string.Empty;
                        cargaValePedagioRetorno.CodigoEmissaoValePedagioANTT = viagemRetorno?.IdVpoAntt ?? string.Empty;
                        cargaValePedagioRetorno.ValorValePedagio = viagemRetorno?.TotalViagem ?? 0m;
                        cargaValePedagioRetorno.CnpjMeioPagamento = dadosRetorno.CnpjMeioPagamento;
                        cargaValePedagioRetorno.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                        cargaValePedagioRetorno.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
                        cargaValePedagioRetorno.TipoPercursoVP = TipoRotaFrete.Volta;
                        cargaValePedagioRetorno.ProblemaIntegracao = "Vale Pedágio Comprado com Sucesso (Registro gerado para a volta com eixo suspenso)";
                        cargaValePedagioRetorno.DataIntegracao = DateTime.Now;
                        cargaValePedagioRetorno.NumeroTentativas = 0;

                        repositorioCargaValePedagio.Inserir(cargaValePedagioRetorno);

                        SalvarReciboValePedagio(cargaValePedagioRetorno, viagemRetorno?.Recibo);
                        servicoArquivoTransacao.Adicionar(cargaValePedagioRetorno, jsonRequisicao, jsonRetorno, "json");
                    }
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
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar a compra do vale pedágio";
            }
            finally
            {
                if (!valePedagioLocalizado && !string.IsNullOrWhiteSpace(jsonRequisicao) && !string.IsNullOrWhiteSpace(jsonRetorno))
                    servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

                repositorioCargaValePedagio.Atualizar(cargaValePedagio);
            }
        }

        private async Task<(Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.DadosRetorno DadosRetorno, string jsonRequisicao, string jsonRetorno)> PossuiCompraPedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string jsonRequisicao = "";
            string jsonRetorno = "";
            string urlRecibo = $"{_configuracaoIntegracao.IntegracaoDigitalCom.EndpointDigitalCom}valePedagio/recibo";
            HttpClient requisicaoRecibo = CriarRequisicao(urlRecibo);
            long documentoTransporte = (_configuracaoIntegracao.IntegracaoDigitalComValePedagio?.EnviarNumeroCargaNoCampoDocumentoTransporte ?? false) ? carga.CodigoCargaEmbarcador.ObterSomenteNumeros().ToLong() : carga.Codigo;

            long[] body = new long[] { documentoTransporte };

            jsonRequisicao = JsonConvert.SerializeObject(body, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            StringContent conteudoRequisicaoRecibo = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");
            HttpResponseMessage retornoRequisicaoRecibo = await requisicaoRecibo.PostAsync(urlRecibo, conteudoRequisicaoRecibo);
            jsonRetorno = await retornoRequisicaoRecibo.Content.ReadAsStringAsync();

            if (retornoRequisicaoRecibo.StatusCode != HttpStatusCode.OK)
                throw new ServicoException($"Falha ao conectar no WS Digital Com: {retornoRequisicaoRecibo.StatusCode}");

            RetornoValePedagioRecibo retorno = JsonConvert.DeserializeObject<List<RetornoValePedagioRecibo>>(jsonRetorno)?.FirstOrDefault();

            return (
                new DadosRetorno
                {
                    DocumentoTransporte = retorno?.NumeroTransporte ?? 0,
                    CnpjMeioPagamento = retorno?.DadosRetorno?.CnpjMeioPagamento ?? string.Empty,
                    FlagPedagio = retorno?.DadosRetorno?.FlagPedagio ?? string.Empty,
                    Viagem = retorno?.DadosRetorno?.Viagem ?? new List<Viagem>(),
                    Status = retorno?.Status ?? new Status()
                },
                jsonRequisicao,
                jsonRetorno
            );
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.RequisicaoValePedagio ObterRequisicaoValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Rotas rotas = new Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Rotas();
            long documentoTransporte = (_configuracaoIntegracao.IntegracaoDigitalComValePedagio?.EnviarNumeroCargaNoCampoDocumentoTransporte ?? false) ? carga.CodigoCargaEmbarcador.ObterSomenteNumeros().ToLong() : carga.Codigo;

            Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Rota rota = new Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Rota()
            {
                DocumentoTransporte = documentoTransporte,
                CNPJ = carga.Filial?.CNPJ,
                CNPJTransportadora = _configuracaoIntegracao.IntegracaoDigitalCom.TipoObtencaoCNPJTransportadora == TipoObtencaoCNPJTransportadora.VeiculoTracao ? carga.Veiculo?.Empresa?.CNPJ : carga.Empresa?.CNPJ,
                InicioVigencia = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.ToString("yyyyMMdd") : string.Empty,
                FimVigencia = carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value.AddDays(15).ToString("yyyyMMdd") : string.Empty,
                NumeroEixos = ObterNumeroEixos(carga),
                Placa = carga.Veiculo?.Placa,
                CNPJCPFSubcontratado = carga.Veiculo?.Tipo == "T" ? carga.Veiculo?.Proprietario?.CPF_CNPJ_SemFormato : string.Empty,
                Posicoes = new Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicoes()
                {
                    Posicao = ObterPosicoes(carga)
                }
            };

            rotas.Rota = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Rota>() { rota };

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.RequisicaoValePedagio()
            {
                Rotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Rotas>() { rotas }
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicao> ObterPosicoes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Servicos.Embarcador.Localidades.Localidade servicoLocalidade = new Servicos.Embarcador.Localidades.Localidade(_unitOfWork);

            Dominio.Entidades.Localidade localidadeOrigem = null;
            Dominio.Entidades.Localidade localidadeDestino = null;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicao>();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = null;

            if (cargaRotaFrete != null)
                pontosRota = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            if (pontosRota == null || pontosRota.Count == 0)
                throw new ServicoException("Não foi possível definir os pontos da rota.");

            localidadeOrigem = servicoLocalidade.ObterLocalidade(pontosRota.FirstOrDefault());
            localidadeDestino = servicoLocalidade.ObterLocalidade(pontosRota.LastOrDefault());

            if (localidadeOrigem == null)
                throw new ServicoException("Não foi possível definir a localidade origem da rota.");

            if (localidadeDestino == null)
                throw new ServicoException("Não foi possível definir a localidade destino da rota.");

            posicoes.Add(PreencherPosicao(localidadeOrigem, eixoSuspenso: false));

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ultimaEntrega = pontosRota.LastOrDefault(x => x.TipoPontoPassagem == TipoPontoPassagem.Entrega);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem fronteiraBrasil = pontosRota.Find(x => x.TipoPontoPassagem == TipoPontoPassagem.Fronteira && x.Cliente.Localidade.Estado.Sigla != "EX");
            bool possuiPontoEntregaEmOutroPais = pontosRota.Exists(x => x.TipoPontoPassagem == TipoPontoPassagem.Entrega && servicoLocalidade.ObterLocalidade(x)?.Estado?.Sigla == "EX");

            if (possuiPontoEntregaEmOutroPais)
            {
                if (fronteiraBrasil == null)
                    throw new ServicoException("Possui entrega em outro país e não foi configurada a Fronteira do Brasil na rota, favor configurar!");

                if (localidadeDestino.Estado.Sigla == "EX")
                    localidadeDestino = servicoLocalidade.ObterLocalidade(fronteiraBrasil);
            }

            bool voltaEixoSuspenso = PossuiVoltaEixoSuspenso(carga, cargaRotaFrete.TipoUltimoPontoRoteirizacao);

            for (int i = 0; i < pontosRota.Count; i++)
            {
                if (i > 0 && i < pontosRota.Count - 1) //Não envia o primeiro e o ultimo ponto                
                {
                    Dominio.Entidades.Localidade localidade = servicoLocalidade.ObterLocalidade(pontosRota[i]);
                    Dominio.Entidades.Localidade localidadeAnterior = servicoLocalidade.ObterLocalidade(pontosRota[i - 1]);

                    if (localidade != null && (localidadeAnterior == null || localidade.Codigo != localidadeAnterior.Codigo) &&
                        localidade.Codigo != localidadeOrigem.Codigo && localidade.Codigo != localidadeDestino.Codigo &&
                        (!possuiPontoEntregaEmOutroPais || localidade.Estado.Sigla != "EX"))
                    {
                        bool eixoSuspenso = ((pontosRota[i].TipoPontoPassagem == TipoPontoPassagem.Retorno) || (pontosRota[i].Codigo == ultimaEntrega?.Codigo)) && voltaEixoSuspenso;
                        posicoes.Add(PreencherPosicao(localidade, eixoSuspenso));
                    }
                }
            }

            posicoes.Add(PreencherPosicao(localidadeDestino, eixoSuspenso: voltaEixoSuspenso));

            return posicoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicao PreencherPosicao(Dominio.Entidades.Localidade localidade, bool eixoSuspenso)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicao posicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom.Posicao()
            {
                Pais = localidade.Pais.Abreviacao,
                Estado = localidade.Estado.Descricao,
                Cidade = localidade.Descricao,
                EixoSuspenso = eixoSuspenso,
                CodigoIBGE = localidade.CodigoIBGE
            };

            return posicao;
        }

        private bool PossuiVoltaEixoSuspenso(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao)
        {
            bool eixosSuspensos = carga.TipoOperacao?.TipoCarregamento == RetornoCargaTipo.Vazio && tipoUltimoPontoRoteirizacao != TipoUltimoPontoRoteirizacao.PontoMaisDistante;

            int numeroEixos = 0;
            if (eixosSuspensos)
            {
                if (carga.Veiculo?.ModeloVeicularCarga != null)
                    numeroEixos += carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;

                if (carga.VeiculosVinculados?.Count > 0)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                    {
                        if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                            numeroEixos += reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return eixosSuspensos && numeroEixos > 0;
        }

        private int ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int numeroEixos = 0;
            if (carga.Veiculo?.ModeloVeicularCarga != null)
                numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;

            if (carga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;
                }
            }

            return numeroEixos;
        }

        public void SalvarReciboValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, string reciboBase64)
        {
            if (!string.IsNullOrEmpty(reciboBase64))
            {
                string nomeArquivo = Guid.NewGuid().ToString().Replace("-", "") + ".pdf";

                Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo repositorioAnexo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo cargaValePedagioAnexo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoAnexo()
                {
                    EntidadeAnexo = cargaValePedagio,
                    NomeArquivo = nomeArquivo,
                    GuidArquivo = nomeArquivo,
                    Descricao = ""
                };

                repositorioAnexo.Inserir(cargaValePedagioAnexo);

                byte[] imageBytes = Convert.FromBase64String(reciboBase64);
                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(Utilidades.IO.FileStorageService.Storage.Combine(ObterCaminhoArquivoRecibo(_unitOfWork), nomeArquivo), imageBytes);
            }
        }

        private string ObterCaminhoArquivoRecibo(Repositorio.UnitOfWork unitOfWork)
        {
            string diretorioArquivos = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Integracao");

            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(diretorioArquivos, TipoIntegracao.DigitalCom.ObterDescricao());

            return caminhoArquivo;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Configurações

        private bool ValidarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (_configuracaoIntegracao?.IntegracaoDigitalCom != null)
                return true;

            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Digital Com";
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
            requisicao.Timeout = TimeSpan.FromMinutes(3);
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuracaoIntegracao.IntegracaoDigitalCom.TokenDigitalCom);

            return requisicao;
        }

        private WebResponse CriarRequisicaoDelete(string url, string jsonRequisicao)
        {
            WebRequest requisicao = WebRequest.Create(url);
            byte[] byteArrayDadosRequisicao = Encoding.UTF8.GetBytes(jsonRequisicao);

            requisicao.Method = "DELETE";
            requisicao.ContentLength = byteArrayDadosRequisicao.Length;
            requisicao.ContentType = "application/json";
            requisicao.Headers.Add("Authorization", "Bearer " + _configuracaoIntegracao.IntegracaoDigitalCom.TokenDigitalCom);

            System.IO.Stream streamDadosRequisicao = requisicao.GetRequestStream();

            streamDadosRequisicao.Write(byteArrayDadosRequisicao, 0, byteArrayDadosRequisicao.Length);
            streamDadosRequisicao.Close();

            return requisicao.GetResponse();
        }

        private void ObterToken()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao repToken = new Repositorio.Embarcador.Cargas.TipoIntegracaoAutenticacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao token = repToken.BuscarPorTipo(TipoIntegracao.DigitalCom);

            if (!string.IsNullOrWhiteSpace(token?.Token) && token.DataExpiracao > DateTime.Now)
            {
                _configuracaoIntegracao.IntegracaoDigitalCom.TokenDigitalCom = token.Token;
                return;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string urlAutenticacao = !string.IsNullOrWhiteSpace(_configuracaoIntegracao.IntegracaoDigitalCom.UrlAutenticacaoDigitalCom) ? _configuracaoIntegracao.IntegracaoDigitalCom.UrlAutenticacaoDigitalCom : "https://apigateway.digitalcomm.com.br:8443/auth/oauth/v2/token";
            RestClient client = new RestClient(urlAutenticacao);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _configuracaoIntegracao.IntegracaoDigitalCom.UsuarioDigitalCom);
            request.AddParameter("client_secret", _configuracaoIntegracao.IntegracaoDigitalCom.SenhaDigitalCom);
            request.AddParameter("scope", "dclogg-internal");
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            if (string.IsNullOrWhiteSpace(retorno.access_token))
                throw new ServicoException("Não foi possível obter o Token");

            if (token == null)
                token = new Dominio.Entidades.Embarcador.Cargas.TipoIntegracaoAutenticacao();

            token.Tipo = TipoIntegracao.DigitalCom;
            token.Token = retorno.access_token;
            token.DataExpiracao = DateTime.Now.AddSeconds(retorno.expires_in - 60);

            if (token.Codigo > 0)
                repToken.Atualizar(token);
            else
                repToken.Inserir(token);

            _configuracaoIntegracao.IntegracaoDigitalCom.TokenDigitalCom = retorno.access_token;
        }

        private void ObterConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (_configuracaoIntegracao != null)
                return;

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            _configuracaoIntegracao = servicoValePedagio.ObterIntegracaoDigitalCom(carga, _tipoServicoMultisoftware);
        }

        #endregion Métodos Privados - Configurações
    }
}

