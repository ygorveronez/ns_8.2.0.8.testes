using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Carrefour
{
    public class IntegracaoCarrefour : IntegracaoBase
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao _configuracaoIntegracao;
        private readonly UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoCarrefour(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void IntegrarProvisao(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoProvisao();

            if (!ValidaConfiguracaoIntegracao(configuracaoIntegracao, ref provisaoIntegracao))
                return;

            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string xmlRequisicao = string.Empty;
            string xmlRetorno = string.Empty;

            provisaoIntegracao.DataIntegracao = DateTime.Now;
            provisaoIntegracao.NumeroTentativas++;

            try
            {
                Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao repositorioProvisaoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoEDIIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.ProvisaoEDIIntegracao provisaoEDIIntegracao = repositorioProvisaoEDIIntegracao.BuscarUltimoPorProvisao(provisaoIntegracao.Provisao.Codigo);

                if (provisaoEDIIntegracao == null)
                    throw new ServicoException("Não foi possível gerar o GZIP (Não foram provisionados CTes)");

                System.IO.MemoryStream edi = IntegracaoEDI.GerarEDI(provisaoEDIIntegracao, _unitOfWork);

                if (edi == null)
                    throw new ServicoException("Não foi possível gerar o GZIP");

                string nomeArquivo = IntegracaoEDI.ObterNomeArquivoEDI(provisaoEDIIntegracao, incrementarSequencia: false, _unitOfWork);
                string mensagemErro = "";

                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Request objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Request
                {
                    MensagemOcorrencia = Convert.ToBase64String(edi.ToArray()),
                    NomeArquivo = nomeArquivo,
                    TipoArquivo = "DFE"
                };

                xmlRequisicao = PostData(XML.ConvertObjectToXMLString(objetoRequest));
                HttpResponseMessage retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, xmlRequisicao, contentType: "application/xml");
                xmlRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Response objetoResponse = ConverteResposta<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Response>(xmlRetorno);

                ProcessaRetornoIntegracao(retornoRequisicao, objetoResponse, ref provisaoIntegracao, out mensagemErro);

                provisaoIntegracao.ProblemaIntegracao = string.IsNullOrWhiteSpace(mensagemErro) ? "Integrado com sucesso" : mensagemErro;

                servicoArquivoTransacao.Adicionar(provisaoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }
            catch (ServicoException excecao)
            {
                provisaoIntegracao.ProblemaIntegracao = excecao.Message;
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                provisaoIntegracao.ProblemaIntegracao = $"Falha ao enviar a Provisão {provisaoIntegracao.Provisao.Descricao} para o WS do Carrefour";
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(provisaoIntegracao, xmlRequisicao, xmlRetorno, "xml");
            }

            repositorioProvisaoIntegracao.Atualizar(provisaoIntegracao);
        }

        // NÃO APAGAR ESSA INTEGRACAO
        public void IntegrarCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, byte[] gz)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoOcorrencia();
            string mensagemErro = "";

            if (!ValidaConfiguracaoIntegracao(configuracaoIntegracao, ref integracao))
                return;

            if (gz == null)
            {
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao gerar o GZIP do XML.";
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return;
            }

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = integracao.CargaCTe.CargaCTeComplementoInfo.CTe;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Request objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Request
                {
                    ChaveCTe = cte.Chave,
                    XmlCTe = Convert.ToBase64String(gz),
                    CnpjOrigem = cte.Remetente.CPF_CNPJ_SemFormato,
                    CnpjDestino = cte.Destinatario.CPF_CNPJ_SemFormato,
                };

                string postData = PostData(XML.ConvertObjectToXMLString(objetoRequest));
                HttpResponseMessage response = EnviarRequisicao(configuracaoIntegracao, postData, contentType: "application/xml");
                string xmlRetorno = response.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Response objetoResponse = ConverteResposta<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Response>(xmlRetorno);

                ProcessaRetornoIntegracao(response, objetoResponse, ref integracao, out mensagemErro);
                servicoArquivoTransacao.Adicionar(integracao, postData, xmlRetorno, "xml", mensagemErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar o complemento " + integracao.Descricao + " para o WS do Carrefour";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            integracao.ProblemaIntegracao = mensagemErro;
            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;
        }

        // NÃO APAGAR ESSA INTEGRACAO
        public void IntegrarCTeOcorrenciaNotaDebito(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, byte[] gz, string nomeArquivo)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoProvisao();
            string mensagemErro = "";

            if (!ValidaConfiguracaoIntegracao(configuracaoIntegracao, ref integracao))
                return;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Request objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Request
                {
                    MensagemOcorrencia = Convert.ToBase64String(gz),
                    NomeArquivo = nomeArquivo// + ".gz"
                };
                string postData = PostData(XML.ConvertObjectToXMLString(objetoRequest));
                HttpResponseMessage response = EnviarRequisicao(configuracaoIntegracao, postData, contentType: "application/xml");
                string xmlRetorno = response.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Response objetoResponse = ConverteResposta<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Response>(xmlRetorno);

                ProcessaRetornoIntegracao(response, objetoResponse, ref integracao, out mensagemErro);
                servicoArquivoTransacao.Adicionar(integracao, postData, xmlRetorno, "xml", mensagemErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar a Ocorrência " + integracao.CargaOcorrencia.Descricao + " para o WS do Carrefour";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            integracao.ProblemaIntegracao = mensagemErro;
            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;
        }

        public void IntegrarCTeFechamentoFrete(Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao, byte[] gz)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo repositorioArquivo = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo>(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoFechamentoFrete();
            string mensagemErro = "";

            if (!ValidaConfiguracaoIntegracao(configuracaoIntegracao, ref integracao))
                return;

            if (gz == null)
            {
                integracao.ProblemaIntegracao = "Ocorreu uma falha ao gerar o GZIP do XML.";
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return;
            }

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = integracao.Complemento.CTe;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Request objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Request
                {
                    ChaveCTe = cte.Chave,
                    XmlCTe = Convert.ToBase64String(gz),
                    CnpjOrigem = cte.Remetente.CPF_CNPJ_SemFormato,
                    CnpjDestino = cte.Destinatario.CPF_CNPJ_SemFormato,
                };
                string postData = PostData(XML.ConvertObjectToXMLString(objetoRequest));
                HttpResponseMessage response = EnviarRequisicao(configuracaoIntegracao, postData, contentType: "application/xml");
                string xmlRetorno = response.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Response objetoResponse = ConverteResposta<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Ocorrencia.Response>(xmlRetorno);

                ProcessaRetornoIntegracao(response, objetoResponse, ref integracao, out mensagemErro);
                servicoArquivoTransacao.Adicionar(integracao, postData, xmlRetorno, "xml", mensagemErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar o complemento " + integracao.Descricao + " para o WS do Carrefour";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            integracao.ProblemaIntegracao = mensagemErro;
            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;
        }

        public void IntegrarCTeFechamentoFreteNotaDebito(Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracao integracao, byte[] gz, string nomeArquivo)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo repositorioArquivo = new Repositorio.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCTeIntegracaoArquivo>(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoFechamentoFreteNotaDebito();
            string mensagemErro = "";

            if (!ValidaConfiguracaoIntegracao(configuracaoIntegracao, ref integracao))
                return;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Request objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Request
                {
                    MensagemOcorrencia = Convert.ToBase64String(gz),
                    NomeArquivo = nomeArquivo,
                    TipoArquivo = "GEN"
                };
                string postData = PostData(XML.ConvertObjectToXMLString(objetoRequest));
                HttpResponseMessage response = EnviarRequisicao(configuracaoIntegracao, postData, contentType: "application/xml");
                string xmlRetorno = response.Content.ReadAsStringAsync().Result;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Response objetoResponse = ConverteResposta<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.Provisao.Response>(xmlRetorno);

                ProcessaRetornoIntegracao(response, objetoResponse, ref integracao, out mensagemErro);
                servicoArquivoTransacao.Adicionar(integracao, postData, xmlRetorno, "xml", mensagemErro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Falha ao enviar a complemento " + integracao.Complemento.CTe + " para o WS do Carrefour";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            integracao.ProblemaIntegracao = mensagemErro;
            integracao.DataIntegracao = DateTime.Now;
            integracao.NumeroTentativas++;
        }

        public void IntegrarCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;
            cargaCancelamentoIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoIntegracaoCancelamentoCarga();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Url) || string.IsNullOrWhiteSpace(configuracaoIntegracao.IbmClienteId))
                    throw new ServicoException("Configuração para integração com o Carrefour inválida.");

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = ObterCargas(cargaCancelamentoIntegracao.CargaCancelamento.Carga);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.CancelamentoCarga.CancelamentoCargaFinalizado objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.CancelamentoCarga.CancelamentoCargaFinalizado
                    {
                        CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                        ProtocoloCarga = carga.Protocolo.ToString()
                    };

                    jsonRequisicao = JsonConvert.SerializeObject(objetoRequest, Formatting.Indented);
                    HttpResponseMessage retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao, contentType: "application/json");
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.RetornoRequisicao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.RetornoRequisicao>(jsonRetorno);

                    if (!retorno.ResultadoOperacaoSucesso)
                        throw new ServicoException($"Falha na integração: {retorno.MensagemErro}", errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao);

                    cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaCancelamentoIntegracao.ProblemaIntegracao = "Integrado com sucesso";

                    servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
                }
            }
            catch (ServicoException excecao)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = excecao.Message;

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.FalhaAoRealizarIntegracao)
                    servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com o Carrefour";

                servicoArquivoTransacao.Adicionar(cargaCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaCancelamentoIntegracao.Atualizar(cargaCancelamentoIntegracao);
        }

        public void IntegrarCarga<TEntidade>(TEntidade cargaIntegracao, Dominio.Entidades.Embarcador.Cargas.Carga cargaPrincipal, bool aguardaRecebimento)
            where TEntidade : Dominio.Entidades.Embarcador.Integracao.Integracao, Dominio.Interfaces.Embarcador.Integracao.IIntegracaoComArquivo<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>
        {
            Repositorio.RepositorioBase<TEntidade> repositorioCargaIntegracao = new Repositorio.RepositorioBase<TEntidade>(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            cargaIntegracao.DataIntegracao = DateTime.Now;
            cargaIntegracao.NumeroTentativas++;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoIntegracaoCarga();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Url) || string.IsNullOrWhiteSpace(configuracaoIntegracao.IbmClienteId))
                    throw new ServicoException("Configuração para integração com o Carrefour inválida.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(_unitOfWork);

                Servicos.WebServiceCarrefour.Conversores.CTe.CTe conversorCTe = new WebServiceCarrefour.Conversores.CTe.CTe(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                if (cargaPrincipal.CargaAgrupada)
                    cargas = repCarga.BuscarCargasOriginais(cargaPrincipal.Codigo);
                else
                    cargas.Add(cargaPrincipal);


                bool problemaIntegracao = false;
                string motivoProblemaIntegracao = "";

                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(TipoIntegracao.Carrefour);


                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    if (integracaoGrupoPessoas.Count > 0)
                    {
                        if (carga.GrupoPessoaPrincipal != null && !integracaoGrupoPessoas.Where(o => o.Habilitado).Any(o => o.GrupoPessoas.Codigo == carga.GrupoPessoaPrincipal.Codigo))
                            continue;
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCargaOrigem(carga.Codigo);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTe integracaoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTe()
                    {
                        NumeroCarga = carga.CodigoCargaEmbarcador,
                        TipoOperacao = carga.TipoOperacao?.CodigoIntegracao,
                        AguardaRecebimento = aguardaRecebimento,
                        Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTePedido>(),
                        ProtocoloCarga = carga.Protocolo,
                        NumeroTotalCTes = 0
                    };

                    bool possuiCTe = false;
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTePedido integracaoCTePedido = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTePedido()
                        {
                            CTes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTeDadosCTe>(),
                            ProtocoloPedido = cargaPedido.Pedido.Protocolo
                        };

                        List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTePorPedido = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCTesPorCargaPedido(cargaPedido.Codigo, cargaPedido.CargaOrigem.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargasCTePorPedido)
                        {
                            possuiCTe = true;
                            string xml = conversorCTe.ObterRetornoXMLPorStatus(cargaCTe.CTe, cargaCTe.CTe.Status);
                            string xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));

                            integracaoCTePedido.CTes.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.IntegracaoCTe.IntegracaoCTeDadosCTe()
                            {
                                Chave = cargaCTe.CTe.Chave,
                                CpfCnpjDestinatario = cargaCTe.CTe.Destinatario?.CPF_CNPJ ?? "",
                                LiberacaoParaPagamentoAutomatico = cargaCTe.CTe.Empresa?.LiberacaoParaPagamentoAutomatico ?? false,
                                TipoTomador = cargaCTe.CTe.TipoTomador.ToString(),
                                Xml = xmlBase64
                            });

                            integracaoCTe.NumeroTotalCTes++;
                        }

                        integracaoCTe.Pedidos.Add(integracaoCTePedido);
                    }

                    if (possuiCTe)
                    {
                        jsonRequisicao = JsonConvert.SerializeObject(integracaoCTe, Formatting.None);
                        HttpResponseMessage retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao, contentType: "application/json");
                        jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.RetornoRequisicao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.RetornoRequisicao>(jsonRetorno);

                        string mensagem = "Sucesso ao integrar o romaneio " + carga.CodigoCargaEmbarcador;
                        if (!retorno.ResultadoOperacaoSucesso)
                        {
                            problemaIntegracao = true;
                            motivoProblemaIntegracao = retorno.MensagemErro;
                            mensagem = $"(Romaneio {carga.CodigoCargaEmbarcador}). Falha na integração: {motivoProblemaIntegracao}";
                        }

                        servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json", mensagem);
                    }
                }


                if (!problemaIntegracao)
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    cargaIntegracao.ProblemaIntegracao = "Integrado com sucesso";
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = $"Falha na integração: {motivoProblemaIntegracao}";
                }


            }
            catch (ServicoException excecao)
            {
                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com o Carrefour";

                servicoArquivoTransacao.Adicionar(cargaIntegracao, jsonRequisicao, jsonRetorno, "json");
            }

            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public bool PossuiIntegracaoCancelamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (carga.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                return false;

            if (!PossuiIntegracaoCarrefour())
                return false;

            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipo(carga.Codigo, TipoIntegracao.Carrefour);
            if (cargaIntegracao == null)
                return false;

            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            bool pendenciaIntegracaoProgramada = UtilizaIntegracaoProgramada(carga) && !repIntegracaoEnvioProgramado.ExisteAutorizadoPorCargaETipo(carga.Codigo, TipoIntegracao.Carrefour);
            if (pendenciaIntegracaoProgramada)
                return false;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoRequisicao = ObterConfiguracaoIntegracaoCancelamentoCarga();
            bool possuiConfiguracao = !string.IsNullOrWhiteSpace(configuracaoRequisicao.Url) && !string.IsNullOrWhiteSpace(configuracaoRequisicao.IbmClienteId);

            if (!possuiConfiguracao)
                return false;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            return !repositorioCargaPedido.ExistePedidoQueGerouCargaAutomaticamente(carga.Codigo);
        }

        public bool PossuiIntegracaoCarga()
        {
            if (!PossuiIntegracaoCarrefour())
                return false;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoRequisicao = ObterConfiguracaoIntegracaoCarga();

            return !string.IsNullOrWhiteSpace(configuracaoRequisicao.Url) && !string.IsNullOrWhiteSpace(configuracaoRequisicao.IbmClienteId);
        }

        public bool UtilizaIntegracaoProgramada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return carga.TipoOperacao?.ConfiguracaoIntegracao?.PossuiTempoEnvioIntegracaoDocumentosCarga ?? false;
        }

        public void ValidarPermissaoCancelarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaValidarCancelamento, string motivoCancelamento)
        {
            try
            {
                if (cargaValidarCancelamento.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                    return;

                if (!PossuiIntegracaoCarrefour())
                    return;

                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(_unitOfWork);
                Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas repConfiguracaoIntegracaoGrupoPessoas = new Repositorio.Embarcador.Configuracoes.IntegracaoGrupoPessoas(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipo(cargaValidarCancelamento.Codigo, TipoIntegracao.Carrefour);

                if (cargaIntegracao == null)
                    return;
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaCargaIntegracao = repCargaCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaValidarCancelamento.Codigo, TipoIntegracao.Carrefour);
                    bool gerouIntegracaoProgramada = repIntegracaoEnvioProgramado.ExisteAutorizadoPorCargaETipo(cargaValidarCancelamento.Codigo, TipoIntegracao.Carrefour);
                    if (cargaCargaIntegracao?.SituacaoIntegracao != SituacaoIntegracao.Integrado && !gerouIntegracaoProgramada)
                        return;
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao = ObterConfiguracaoValidacaoCancelamentoCarga();

                if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Url) || string.IsNullOrWhiteSpace(configuracaoIntegracao.IbmClienteId))
                    return;

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

                if (repositorioCargaPedido.ExistePedidoQueGerouCargaAutomaticamente(cargaValidarCancelamento.Codigo))
                    return;

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = ObterCargas(cargaValidarCancelamento);
                List<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGrupoPessoa> integracaoGrupoPessoas = repConfiguracaoIntegracaoGrupoPessoas.BuscarPorTipoIntegracao(TipoIntegracao.Carrefour);
                List<int> grupos = (from obj in cargas where obj.GrupoPessoaPrincipal != null select obj.GrupoPessoaPrincipal.Codigo).Distinct().ToList();
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    if (grupos.Count() == 1 || (carga.GrupoPessoaPrincipal != null && integracaoGrupoPessoas.Any(obj => obj.GrupoPessoas.Codigo == carga.GrupoPessoaPrincipal.Codigo)))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.CancelamentoCarga.CancelamentoCarga objetoRequest = new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.CancelamentoCarga.CancelamentoCarga
                        {
                            CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                            MotivoCancelamento = motivoCancelamento,
                            ProtocoloCarga = carga.Protocolo.ToString()
                        };

                        string jsonRequisicao = JsonConvert.SerializeObject(objetoRequest, Formatting.Indented);
                        HttpResponseMessage retornoRequisicao = EnviarRequisicao(configuracaoIntegracao, jsonRequisicao, contentType: "application/json");
                        string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.RetornoRequisicao retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.RetornoRequisicao>(jsonRetorno);

                        if (!retorno.ResultadoOperacaoSucesso)
                            throw new ServicoException($"Cancelamento não autorizado: {retorno.MensagemErro}");
                    }
                }
            }
            catch (ServicoException)
            {
                throw;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                throw new ServicoException("Ocorreu uma falha ao validar a permissão para cancelar a carga.");
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool TestarDisponibilidade()
        {
            SecurityProtocolType protocoloAnterior = ServicePointManager.SecurityProtocol;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracao = ObterConfiguracaoIntegracaoCarga();
                bool configuracaoInvalida = (string.IsNullOrWhiteSpace(configuracao.Url) || string.IsNullOrWhiteSpace(configuracao.IbmClienteId));

                if (configuracaoInvalida)
                    return true;

                HttpResponseMessage response = EnviarRequisicao(configuracao, dadosEnviar: "", contentType: "application/json");

                return response.StatusCode == HttpStatusCode.BadRequest;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                return false;
            }
            finally
            {
                ServicePointManager.SecurityProtocol = protocoloAnterior;
            }
        }

        #endregion

        #region Métodos Privados

        private T ConverteResposta<T>(string response)
        {
            try
            {
                return Servicos.XML.ConvertXMLStringToObject<T>(response);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private HttpResponseMessage EnviarRequisicao(Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao, string dadosEnviar, string contentType)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCarrefour));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            requisicao.DefaultRequestHeaders.Add(configuracaoIntegracao.HeaderIbmClienteId, configuracaoIntegracao.IbmClienteId);
            requisicao.DefaultRequestHeaders.Add("User-Agent", "multi-embarcador-ms");

            StringContent conteudoRequisicao = new StringContent(dadosEnviar, Encoding.UTF8, contentType);
            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.Url, conteudoRequisicao).Result;

            return retornoRequisicao;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> ObterCargas(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

            if (carga.CargaAgrupada)
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                cargas.AddRange(repositorioCarga.BuscarCargasOriginais(carga.Codigo));
            }
            else
                cargas.Add(carga);

            return cargas;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.Integracao ObterConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao == null)
                _configuracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork).Buscar();

            return _configuracaoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoIntegracaoCancelamentoCarga()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "x-api-key",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefour ?? "",
                Url = configuracaoIntegracao?.URLCarrefourCancelamentoCarga ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoIntegracaoCarga()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "x-api-key",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefour ?? "",
                Url = configuracaoIntegracao?.URLCarrefourCarga ?? ""
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoFechamentoFrete()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "x-api-key",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefourProvisao ?? string.Empty,
                Url = configuracaoIntegracao?.URLCarrefourProvisao ?? string.Empty
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoFechamentoFreteNotaDebito()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "x-api-key",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefourProvisao ?? string.Empty,
                Url = configuracaoIntegracao?.URLCarrefourOcorrencia ?? string.Empty
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoOcorrencia()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "X-IBM-Client-Id",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefourProvisao ?? string.Empty,
                Url = configuracaoIntegracao?.URLCarrefourOcorrencia ?? string.Empty
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoProvisao()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "X-IBM-Client-Id",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefourProvisao ?? string.Empty,
                Url = configuracaoIntegracao?.URLCarrefourProvisao ?? string.Empty
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao ObterConfiguracaoValidacaoCancelamentoCarga()
        {
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = ObterConfiguracaoIntegracao();

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao()
            {
                HeaderIbmClienteId = "x-api-key",
                IbmClienteId = configuracaoIntegracao?.TokenCarrefour ?? "",
                Url = configuracaoIntegracao?.URLCarrefourValidarCancelamentoCarga ?? ""
            };
        }

        private bool PossuiIntegracaoCarrefour()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Carrefour);
        }

        private string PostData(string xml)
        {
            return (new StringBuilder())
                   .Append("<?xml version='1.0' encoding='UTF-8'?>")
                   .Append(xml.Replace("\r\n", ""))
                   .ToString()
                   .Replace("q1", "ns0");
        }

        private void ProcessaRetornoIntegracao<T>(HttpResponseMessage response, dynamic objetoResponse, ref T integracao, out string mensagemErro) where T : Dominio.Entidades.Embarcador.Integracao.Integracao
        {
            mensagemErro = "";
            if (response.StatusCode == HttpStatusCode.OK || objetoResponse != null && objetoResponse.ResultadoOperacao == "10")
            {
                if (objetoResponse.ResultadoOperacao == "0" || objetoResponse.ResultadoOperacao == "10")
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                }
                else
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    mensagemErro = objetoResponse.MensagemErro ?? "Erro não retornado pela Carrefour";
                }
            }
            else
            {
                if (objetoResponse != null)
                    mensagemErro = "Erro WS: " + (objetoResponse.MensagemErro ?? "");
                else
                    mensagemErro = "Falha ao comunicar com o WS do Carrefour";
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                if (mensagemErro.Length > 300)
                    mensagemErro = mensagemErro.Substring(0, 299);
            }
        }

        private bool ValidaConfiguracaoIntegracao<T>(Dominio.ObjetosDeValor.Embarcador.Integracao.Carrefour.ConfiguracaoRequisicao configuracaoIntegracao, ref T integracao) where T : Dominio.Entidades.Embarcador.Integracao.Integracao
        {
            if (string.IsNullOrWhiteSpace(configuracaoIntegracao.Url) || string.IsNullOrWhiteSpace(configuracaoIntegracao.IbmClienteId))
            {
                integracao.ProblemaIntegracao = "Configuração para integração com Carrefour inválida.";
                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas++;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                return false;
            }

            return true;
        }

        #endregion
    }
}
