using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Entidades.Embarcador.Escrituracao;
using Dominio.Entidades.Embarcador.Financeiro;
using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Camil;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Camil
{
    public class IntegracaoCamil
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.Filiais.Filial _repositorioFilial;
        private readonly Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal _repositorioPedidoXMLNotaFiscal;
        private readonly Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador _repositorioCondicaoPagamentoTransportador;
        private readonly Repositorio.Embarcador.Canhotos.Canhoto _repositorioCanhoto;
        private readonly Repositorio.Embarcador.Escrituracao.PagamentoIntegracao _repositorioPagamentoIntegracao;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCamil _configuracaoIntegracao;

        #endregion

        #region Construtores

        public IntegracaoCamil(Repositorio.UnitOfWork unitOfWork) : base()
        {
            _unitOfWork = unitOfWork;
            _repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            _repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            _repositorioCondicaoPagamentoTransportador = new Repositorio.Embarcador.Transportadores.CondicaoPagamentoTransportador(_unitOfWork);
            _repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);
            _repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);
        }

        #endregion

        #region Métodos de Integração do Documento de Cancelamento de Pagamento

        public void IntegrarCancelamentoPagamentoCargaCTeIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repositorioCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaCancelamentoCargaCTeIntegracao.NumeroTentativas++;
            cargaCancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao>();
            List<string> erros = null;

            try
            {
                informacaoEnvio.ProtocoloIntegracao = cargaCancelamentoCargaCTeIntegracao.Codigo;

                CancelamentoContabilizacao cancelamentoContabilizacao = ObterCancelamentoContabilizacaoPorCancelamentoCargaCTe(cargaCancelamentoCargaCTeIntegracao.CargaCTe, informacaoEnvio.ProtocoloIntegracao);
                informacaoEnvio.Contabilizacao = new List<CancelamentoContabilizacao>() {
                    cancelamentoContabilizacao
                };

                erros = ValidarCancelamentoContabilizacao(cancelamentoContabilizacao);

                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/contabil/cancelContabilizacao");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros?.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaCTeIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioCancelamentoCargaCTeIntegracao.Atualizar(cargaCancelamentoCargaCTeIntegracao);
        }

        public void IntegrarCancelamentoPagamento(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao cancelamentoPagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao repositorioCancelamentoPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cancelamentoPagamentoIntegracao.NumeroTentativas++;
            cancelamentoPagamentoIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao>();
            List<string> erros = null;

            try
            {
                informacaoEnvio.ProtocoloIntegracao = cancelamentoPagamentoIntegracao.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao cancelamentoContabilizacao = ObterCancelamentoContabilizacaoPorDocumentoFaturamento(cancelamentoPagamentoIntegracao.DocumentoFaturamento, informacaoEnvio.ProtocoloIntegracao);
                informacaoEnvio.Contabilizacao = new List<CancelamentoContabilizacao>(){
                    cancelamentoContabilizacao
                };

                erros = ValidarCancelamentoContabilizacao(cancelamentoContabilizacao);

                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/contabil/cancelContabilizacao");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                cancelamentoPagamentoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                cancelamentoPagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cancelamentoPagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoPagamentoIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros?.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(cancelamentoPagamentoIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioCancelamentoPagamentoIntegracao.Atualizar(cancelamentoPagamentoIntegracao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao ObterCancelamentoContabilizacaoPorCancelamentoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, long protocoloIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao cancelamentoContabilizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao();
            cancelamentoContabilizacao.ProtocoloIntegracao = protocoloIntegracao;
            cancelamentoContabilizacao.CodigoEstabelecimento = cargaCTe.CTe?.Tomador?.Cliente?.CodigoIntegracao;

            cancelamentoContabilizacao.CodigoFornecedor = cargaCTe.CTe.Empresa?.CodigoIntegracao;
            cancelamentoContabilizacao.NaturezaOperacao = (cargaCTe.CTe?.CFOP?.CodigoCFOP ?? 0).ToString();
            cancelamentoContabilizacao.NumeroDocumento = (cargaCTe.CTe?.Numero ?? 0).ToString();
            cancelamentoContabilizacao.SerieDocumento = (cargaCTe.CTe?.Serie?.Numero ?? 0).ToString();

            cancelamentoContabilizacao.LimpaProtocolo = true;

            cancelamentoContabilizacao.CodigoTipoOperacao = cargaCTe.Carga?.TipoOperacao?.CodigoIntegracao;
            cancelamentoContabilizacao.CodigoCancelOcorrencia = cargaCTe.CargaCTeComplementoInfo?.CargaOcorrencia?.NumeroOcorrencia.ToString();

            return cancelamentoContabilizacao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao ObterCancelamentoContabilizacaoPorDocumentoFaturamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, long protocoloIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao cancelamentoContabilizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao();
            cancelamentoContabilizacao.ProtocoloIntegracao = protocoloIntegracao;
            cancelamentoContabilizacao.CodigoEstabelecimento = documentoFaturamento.Tomador.CodigoIntegracao;

            if (documentoFaturamento.CTe != null)
            {
                cancelamentoContabilizacao.CodigoFornecedor = documentoFaturamento.CTe.Empresa?.CodigoIntegracao;
                cancelamentoContabilizacao.NaturezaOperacao = (documentoFaturamento.CTe?.CFOP?.CodigoCFOP ?? 0).ToString();
                cancelamentoContabilizacao.NumeroDocumento = (documentoFaturamento.CTe?.Numero ?? 0).ToString();
                cancelamentoContabilizacao.SerieDocumento = (documentoFaturamento.CTe?.Serie?.Numero ?? 0).ToString();
            }
            else if (documentoFaturamento.LancamentoNFSManual != null)
            {
                cancelamentoContabilizacao.CodigoFornecedor = documentoFaturamento.LancamentoNFSManual.DadosNFS?.Serie?.Empresa?.CodigoIntegracao;
                cancelamentoContabilizacao.NaturezaOperacao = (documentoFaturamento.LancamentoNFSManual?.CTe?.CFOP?.CodigoCFOP ?? 0).ToString();
                cancelamentoContabilizacao.NumeroDocumento = (documentoFaturamento.LancamentoNFSManual?.DadosNFS?.Numero ?? 0).ToString();
                cancelamentoContabilizacao.SerieDocumento = (documentoFaturamento.LancamentoNFSManual?.DadosNFS?.Serie?.Numero ?? 0).ToString();
            }

            cancelamentoContabilizacao.CodigoTipoOperacao = documentoFaturamento.TipoOperacao?.CodigoIntegracao;
            cancelamentoContabilizacao.CodigoCancelOcorrencia = documentoFaturamento.CargaOcorrenciaPagamento?.NumeroOcorrencia.ToString();

            cancelamentoContabilizacao.LimpaProtocolo = false;

            return cancelamentoContabilizacao;
        }

        private List<string> ValidarCancelamentoContabilizacao(Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.CancelamentoContabilizacao cancelamento)
        {
            List<string> erros = new List<string>();
            string numeroDocumento = cancelamento.NumeroDocumento ?? "";

            if (string.IsNullOrEmpty(cancelamento.NumeroDocumento))
                erros.Add($"Número do documento não encontrado para o documento");

            if (string.IsNullOrEmpty(cancelamento.CodigoEstabelecimento))
                erros.Add($"Código de integração do estabelecimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(cancelamento.CodigoFornecedor))
                erros.Add($"Código de integração do fornecedor não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(cancelamento.NaturezaOperacao))
                erros.Add($"Código de integração da natureza da operação não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(cancelamento.SerieDocumento))
                erros.Add($"Série não encontrado para o documento {numeroDocumento}");

            return erros;
        }

        #endregion

        #region Método de Integração da Ocorrência de Carga

        public void IntegrarCargaOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            ocorrenciaCTeIntegracao.NumeroTentativas++;
            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito>();
            List<string> erros = new List<string>();

            try
            {
                informacaoEnvio.ProtocoloIntegracao = ocorrenciaCTeIntegracao.Codigo;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarPorOcorrencia(ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito notaDebito = ObterNotaDebitoPorOcorrenciaCTeIntegracao(ocorrenciaCTeIntegracao, cargaCTesComplementoInfo, informacaoEnvio.ProtocoloIntegracao);

                informacaoEnvio.NotasDebito = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito>()
                {
                    notaDebito
                };
                erros = ValidarNotaDebito(notaDebito);

                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/notaDebito/insertNotaDebito");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito ObterNotaDebitoPorOcorrenciaCTeIntegracao(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo, long protocoloIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito notaDebito = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito();
            notaDebito.ProtocoloIntegracao = protocoloIntegracao;
            notaDebito.CodigoEstabelecimento = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.Tomador?.Cliente?.CodigoIntegracao;
            notaDebito.NumeroTitulo = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.Numero.ToString();
            notaDebito.SerieTitulo = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.Serie?.Numero.ToString();
            notaDebito.EspecieTitulo = "CTe";
            notaDebito.CodigoFornecedor = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.Empresa?.CodigoIntegracao;
            notaDebito.ValorTotalTitulo = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia;
            notaDebito.ValorTotalTitulo = ocorrenciaCTeIntegracao.CargaOcorrencia.ValorOcorrencia;
            notaDebito.DataTransacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            notaDebito.DataEmissao = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.DataEmissao?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
            notaDebito.DataVencimento = notaDebito.DataEmissao;
            notaDebito.Observacao = ocorrenciaCTeIntegracao.CargaOcorrencia.Observacao;
            notaDebito.Documento = new List<Documento>();

            if (cargaCTesComplementoInfo != null)
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo complementoInfo in cargaCTesComplementoInfo)
                {
                    Documento documento = ObterDocumentoPorCargaCTeComplementoInfo(ocorrenciaCTeIntegracao.CargaOcorrencia?.Carga.Filial, complementoInfo);

                    if (documento == null)
                        continue;

                    notaDebito.Documento.Add(documento);
                }

            return notaDebito;
        }

        private List<string> ValidarNotaDebito(Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaDebito notaDebito)
        {
            List<string> erros = new List<string>();
            string numeroDocumento = notaDebito.NumeroTitulo ?? "";

            if (string.IsNullOrEmpty(notaDebito.NumeroTitulo))
                erros.Add($"Número do Título não encontrado para o documento");

            if (string.IsNullOrEmpty(notaDebito.CodigoEstabelecimento))
                erros.Add($"Código de integração do estabelecimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(notaDebito.EspecieTitulo))
                erros.Add($"Espécie do título não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(notaDebito.CodigoFornecedor))
                erros.Add($"Código de integração do fornecedor não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(notaDebito.DataTransacao))
                erros.Add($"Data de transação não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(notaDebito.DataEmissao))
                erros.Add($"Data de emissão não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(notaDebito.DataVencimento))
                erros.Add($"Data de vencimento não encontrada para o documento {numeroDocumento}");

            return erros;
        }

        private Documento ObterDocumentoPorCargaCTeComplementoInfo(Dominio.Entidades.Embarcador.Filiais.Filial filial, CargaCTeComplementoInfo complemento)
        {
            Documento documento = null;

            if (complemento.CTeComplementado != null)
                documento = new Documento()
                {
                    CodigoFornecedorDocumento = complemento.CTeComplementado.Empresa?.CodigoIntegracao ?? "",
                    NumeroDocumento = (complemento.CTeComplementado?.Numero ?? 0).ToString(),
                    NaturezaOperacaoDocumento = complemento.CTeComplementado?.NaturezaDaOperacao?.CodigoIntegracao,
                    SerieDocumento = (complemento.CTeComplementado?.Serie?.Numero ?? 0).ToString(),
                    CodigoEstabelecimentoDocumento = complemento.CTeComplementado?.Tomador?.CodigoIntegracao
                };
            else if (complemento.CargaDocumentoParaEmissaoNFSManualComplementado != null
                && complemento.CargaDocumentoParaEmissaoNFSManualComplementado.LancamentoNFSManual != null)
            {
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = complemento.CargaDocumentoParaEmissaoNFSManualComplementado.LancamentoNFSManual;

                documento = new Documento()
                {
                    CodigoFornecedorDocumento = lancamentoNFSManual.DadosNFS?.Serie?.Empresa?.CodigoIntegracao,
                    NumeroDocumento = (lancamentoNFSManual.DadosNFS?.Numero ?? 0).ToString(),
                    NaturezaOperacaoDocumento = lancamentoNFSManual.CTe?.NaturezaNFSe?.Descricao,
                    SerieDocumento = (lancamentoNFSManual.DadosNFS?.Serie?.Numero ?? 0).ToString(),
                    CodigoEstabelecimentoDocumento = lancamentoNFSManual.Tomador?.CodigoIntegracao
                };
            }

            if (documento == null)
                return null;

            return documento;
        }

        #endregion

        #region Métodos de Integração do Documento de Cancelamento de Provisão

        public void IntegrarCancelamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao cancelamentoProvisaoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cancelamentoProvisaoIntegracao.NumeroTentativas++;
            cancelamentoProvisaoIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno>();
            List<string> erros = new List<string>();

            try
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repositorioDocumentoProvisao.BuscarPorCancelamento(cancelamentoProvisaoIntegracao.CancelamentoProvisao.Codigo);

                informacaoEnvio.Estorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno>();
                informacaoEnvio.ProtocoloIntegracao = cancelamentoProvisaoIntegracao.Codigo;

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisao)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno estorno = ObterEstornoPorDocumento(documento, informacaoEnvio.ProtocoloIntegracao);
                    erros.AddRange(ValidarEstorno(estorno));

                    informacaoEnvio.Estorno.Add(estorno);
                }

                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/provisao/insertEstorno");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                cancelamentoProvisaoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                cancelamentoProvisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                cancelamentoProvisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cancelamentoProvisaoIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(cancelamentoProvisaoIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioCancelamentoProvisaoIntegracao.Atualizar(cancelamentoProvisaoIntegracao);
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno ObterEstornoPorDocumento(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, long protocoloIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno estorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno();
            estorno.ProtocoloIntegracao = protocoloIntegracao;
            estorno.CodigoEstabelecimento = documentoProvisao.Provisao?.Tomador?.CodigoIntegracao;
            estorno.NumeroTitulo = (documentoProvisao.Provisao?.Numero ?? 0).ToString();

            estorno.CodigoTipoOperacao = documentoProvisao.Carga?.TipoOperacao?.CodigoIntegracao;

            if (documentoProvisao.Empresa != null)
                estorno.CodigoFornecedor = documentoProvisao.Empresa.CodigoIntegracao ?? "";

            return estorno;
        }

        private List<string> ValidarEstorno(Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Estorno estorno)
        {
            List<string> erros = new List<string>();
            string numeroDocumento = estorno.NumeroTitulo ?? "";

            if (string.IsNullOrEmpty(estorno.NumeroTitulo))
                erros.Add($"Número do Título não encontrado para o documento");

            if (string.IsNullOrEmpty(estorno.CodigoEstabelecimento))
                erros.Add($"Código de integração do estabelecimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(estorno.CodigoFornecedor))
                erros.Add($"Código de integração do fornecedor não encontrado para o documento {numeroDocumento}");

            return erros;
        }

        #endregion

        #region Métodos de Integração do Documento de Provisão

        public async Task IntegrarProvisaoAsync(Dominio.Entidades.Embarcador.Escrituracao.ProvisaoIntegracao provisaoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao>();
            List<string> erros = new List<string>();

            provisaoIntegracao.NumeroTentativas++;
            provisaoIntegracao.DataIntegracao = DateTime.Now;
            informacaoEnvio.ProtocoloIntegracao = provisaoIntegracao.Codigo;

            try
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repositorioDocumentoProvisao.BuscarPorProvisao(provisaoIntegracao.Provisao.Codigo);
                Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao primeiroDocumentoProvisao = documentosProvisao
                    .OrderBy(documento => documento.DataEmissao)
                    .FirstOrDefault();

                if (primeiroDocumentoProvisao == null)
                    throw new ServicoException("Nenhum documento encontrado na provisão");

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = provisaoIntegracao.Provisao;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao provisaoCamil = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao();

                provisaoCamil.ProtocoloIntegracao = informacaoEnvio.ProtocoloIntegracao;
                provisaoCamil.NumeroTitulo = provisao.Numero.ToString();
                provisaoCamil.CodigoEstabelecimento = provisao.Tomador?.CodigoIntegracao ?? string.Empty;
                provisaoCamil.NaturezaOperacao = ObterNaturezaOperacaoPorDocumentoProvisao(primeiroDocumentoProvisao);
                provisaoCamil.CodigoFornecedor = primeiroDocumentoProvisao.Empresa?.CodigoIntegracao;
                provisaoCamil.DataEmissao = provisao.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss");
                provisaoCamil.DataVencimento = ObterDataComUltimoDiaDoMes(DateTime.Now).ToString("dd/MM/yyyy HH:mm:ss");
                provisaoCamil.TipoOperacao = provisao.Carga.TipoOperacao?.CodigoIntegracao ?? string.Empty;
                if (provisaoIntegracao.Provisao.GeradoManualmente)
                    provisaoCamil.DataTransacao = ObterDataTransacao().ToString("dd/MM/yyyy HH:mm:ss");
                else
                    provisaoCamil.DataTransacao = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                provisaoCamil.NotaFiscal = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal>();
                provisaoCamil.Imposto = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto>();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisao)
                {
                    await AdicionarNotasFisicaisProvisaoPorDocumento(provisaoCamil, documento);
                }

                erros = ValidarProvisao(provisaoCamil);
                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                informacaoEnvio.Provisoes = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao>()
                {
                    provisaoCamil
                };

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/provisao/insertProvisao");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                provisaoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                provisaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                provisaoIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(provisaoIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioProvisaoIntegracao.Atualizar(provisaoIntegracao);
        }

        private async Task AdicionarNotasFisicaisProvisaoPorDocumento(Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao provisao, DocumentoProvisao documento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal> notasFiscais = null;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto> impostos = null;

            if (documento.LancamentoNFSManual != null)
            {
                impostos = ObterImpostosPorLancamentoNFSManual(documento.LancamentoNFSManual);
                notasFiscais = ObterNotasFiscaisPorLancamentoNFSManual(documento.LancamentoNFSManual);
            }
            else if (documento.XMLNotaFiscal != null)
            {
                impostos = ObterImpostosPorXMLNotaFiscal(documento.XMLNotaFiscal, documento.Carga?.Codigo ?? 0);

                List<string> cnpjs = new() { documento.XMLNotaFiscal.Emitente.CPF_CNPJ.ToString() };
                List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = _repositorioFilial.BuscarPorCNPJSAsync(cnpjs).GetAwaiter().GetResult();

                Dominio.Entidades.Embarcador.Filiais.Filial filial = documento.Carga.Filial;

                notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal>
                    {
                        await ObterNotasFiscaisPorXMLNotaFiscal(documento.XMLNotaFiscal, filiais, filial)
                    };
            }

            if (notasFiscais != null)
                provisao.NotaFiscal.AddRange(notasFiscais);

            if (impostos != null)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoAdicionar in impostos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoExistente = provisao.Imposto.Find(impostoProvisao =>
                        impostoProvisao.CodigoImposto == impostoAdicionar.CodigoImposto
                        && impostoProvisao.AliquotaImposto == impostoAdicionar.AliquotaImposto);

                    if (impostoExistente != null)
                    {
                        impostoExistente.ValorImposto += impostoAdicionar.ValorImposto;
                        impostoExistente.ValorBaseImposto += impostoAdicionar.ValorBaseImposto;
                    }
                    else
                        provisao.Imposto.Add(impostoAdicionar);
                }
            }

            provisao.ValorTotalTitulo += documento.ValorProvisao;
        }

        private List<string> ValidarProvisao(Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Provisao provisao)
        {
            List<string> erros = new List<string>();
            string numeroDocumento = provisao.NumeroTitulo ?? "";

            if (string.IsNullOrEmpty(provisao.NumeroTitulo))
                erros.Add($"Número do Título não encontrado para o documento");

            if (string.IsNullOrEmpty(provisao.CodigoEstabelecimento))
                erros.Add($"Código de integração do estabelecimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(provisao.CodigoFornecedor))
                erros.Add($"Código de integração do fornecedor não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(provisao.DataTransacao))
                erros.Add($"Data de transação não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(provisao.DataEmissao))
                erros.Add($"Data de emissão não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(provisao.DataVencimento))
                erros.Add($"Data de vencimento não encontrada para o documento {numeroDocumento}");

            return erros;
        }

        #endregion

        #region Métodos de Integração do Documento de Escritação

        public async Task IntegrarContabilizacao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            pagamentoIntegracao.NumeroTentativas++;
            pagamentoIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao>();
            List<string> erros = new List<string>();

            try
            {
                informacaoEnvio.Contabilizacao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao>();
                informacaoEnvio.ProtocoloIntegracao = pagamentoIntegracao.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao contabilizacao = await ObterContabilizacao(pagamentoIntegracao);
                erros = ValidarContabilizacao(contabilizacao);

                informacaoEnvio.Contabilizacao.Add(contabilizacao);

                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/contabil/insertContabilizacao");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                pagamentoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(pagamentoIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            await repositorioPagamentoIntegracao.AtualizarAsync(pagamentoIntegracao);
        }

        public void IntegrarDesbloqueio(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            pagamentoIntegracao.NumeroTentativas++;
            pagamentoIntegracao.DataIntegracao = DateTime.Now;

            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<DesbloqueioTitulo> informacaoEnvio = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DadosRequisicaoCamil<DesbloqueioTitulo>();
            List<string> erros = new List<string>();

            try
            {
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamentos = repositorioDocumentoFaturamento.BuscarPorPagamento(pagamentoIntegracao.Pagamento.Codigo, pagamentoIntegracao.Pagamento.LotePagamentoLiberado);
                informacaoEnvio.Desbloqueio = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DesbloqueioTitulo>();
                informacaoEnvio.ProtocoloIntegracao = pagamentoIntegracao.Codigo;

                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in documentosFaturamentos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DesbloqueioTitulo desbloqueioTitulo = ObterDesbloqueioTituloPorDocumentoFaturamento(documentoFaturamento, informacaoEnvio.ProtocoloIntegracao);

                    string mensagemFalha = "";
                    DateTime? dataDigitalizacaoCanhoto = ObterDataDigitalizacaoCanhotoPorDocumentoFaturamento(documentoFaturamento);
                    desbloqueioTitulo.DataVencimento = CalcularDataVencimento(documentoFaturamento.Empresa?.Codigo, documentoFaturamento.Carga, dataDigitalizacaoCanhoto, out mensagemFalha)?.ToString("dd/MM/yyyy 00:00:00") ?? "";

                    if (string.IsNullOrEmpty(mensagemFalha))
                        erros.AddRange(ValidarDesbloqueioTitulo(desbloqueioTitulo));
                    else
                        erros.Add(mensagemFalha);

                    informacaoEnvio.Desbloqueio.Add(desbloqueioTitulo);
                }

                if (erros.Count > 0)
                    throw new ServicoException(erros.FirstOrDefault());

                respostaHttp = ExecutarRequisicao(informacaoEnvio, "/v1/contabil/insertDesbloqueioTitulo");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRespostaCamil respostaCamil = JsonConvert.DeserializeObject<DadosRespostaCamil>(respostaHttp.conteudoResposta);
                    if (!bool.TryParse(respostaCamil.Data?.ToString() ?? "", out bool sucesso) || !sucesso)
                        throw new ServicoException($"Resposta API Camil: {respostaCamil.Data?.ToString()}");
                }
                else
                    throw new ServicoException($"Problema ao obter a resposta da API Camil.");

                pagamentoIntegracao.ProblemaIntegracao = "Sucesso ao comunicar com API Camil.";
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgRetorno;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                pagamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                pagamentoIntegracao.ProblemaIntegracao = ex.Message;
            }

            string conteudoRequisicao = erros.Count > 0 ? ObterErrosValidacao(informacaoEnvio, erros) : respostaHttp.conteudoRequisicao;

            servicoArquivoTransacao.Adicionar(pagamentoIntegracao, conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioPagamentoIntegracao.Atualizar(pagamentoIntegracao);
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao> ObterContabilizacao(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao contabilizacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao();
            string mensagemFalha = string.Empty;
            contabilizacao.ProtocoloIntegracao = pagamentoIntegracao.Codigo;
            DocumentoFaturamento documentoFaturamento = pagamentoIntegracao.DocumentoFaturamento;
            Pagamento pagamento = pagamentoIntegracao.Pagamento;

            contabilizacao.CodigoEstabelecimento = documentoFaturamento.CTe.Tomador.Cliente.CodigoIntegracao;
            contabilizacao.CodigoFornecedor = documentoFaturamento.Empresa.CodigoIntegracao;
            contabilizacao.TituloIntermediario = false;

            if (documentoFaturamento.CTe != null)
            {
                contabilizacao.NumeroDocumento = documentoFaturamento.CTe.Numero.ToString();
                contabilizacao.SerieDocumento = (documentoFaturamento.CTe.Serie?.Numero ?? 0).ToString();
                contabilizacao.DataEmissao = documentoFaturamento.CTe.DataEmissao?.ToString("dd/MM/yyyy HH:mm:ss");
                contabilizacao.ValorDocumento = documentoFaturamento.CTe.ValorAReceber;
                contabilizacao.TipoTributacaoICMS = ObterTipoTributacaoICMS(documentoFaturamento.CTe.CST ?? "");
                contabilizacao.Imposto = ObterImpostosPorDocumentoFaturamento(documentoFaturamento);
                contabilizacao.NotaFiscal = await ObterNotasFiscaisPorCTe(documentoFaturamento);
                contabilizacao.Documento = ObterDocumentosPorDocumentoFaturamentoEPagamento(documentoFaturamento, pagamento);
                contabilizacao.NaturezaOperacao = ObterNaturezaOperacaoPorDocumentoFaturamento(documentoFaturamento);
                contabilizacao.CodigoChaveAcesso = documentoFaturamento.CTe.ChaveAcesso;
                contabilizacao.TipoOperacao = documentoFaturamento.CargaPagamento.TipoOperacao?.CodigoIntegracao ?? string.Empty;
                contabilizacao.CodigoOcorrencia = documentoFaturamento.CargaOcorrenciaPagamento?.NumeroOcorrencia ?? 0;

                if (documentoFaturamento.CTe.LocalidadeInicioPrestacao != null)
                {
                    Localidade localidadeInicio = documentoFaturamento.CTe.LocalidadeInicioPrestacao;
                    contabilizacao.CodigoIBGEOrigem = localidadeInicio.CodigoIBGE.ToString();
                    contabilizacao.UFOrigem = localidadeInicio.Estado?.Sigla.ToString() ?? "";
                }

                if (documentoFaturamento.CTe.LocalidadeTerminoPrestacao != null)
                {
                    Localidade localidadeDestino = documentoFaturamento.CTe.LocalidadeTerminoPrestacao;
                    contabilizacao.CodigoIBGEDestino = localidadeDestino.CodigoIBGE.ToString();
                    contabilizacao.UFDestino = localidadeDestino.Estado?.Sigla.ToString() ?? "";
                }

                if (documentoFaturamento.CTe.XMLs != null)
                {
                    XMLCTe cteXML = documentoFaturamento.CTe.XMLs.FirstOrDefault(xml => xml.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao);

                    if (cteXML != null && !string.IsNullOrEmpty(cteXML.XML))
                        contabilizacao.XMLCTE = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cteXML.XML));
                }
            }

            if (documentoFaturamento.CTe?.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && documentoFaturamento.CargaOcorrenciaPagamento != null)
            {
                contabilizacao.DataVencimento = CalcularDataVencimento(documentoFaturamento.Empresa.Codigo, documentoFaturamento.CargaOcorrenciaPagamento.Carga, documentoFaturamento.CargaOcorrenciaPagamento.DataAprovacao, out mensagemFalha)?.ToString("dd/MM/yyyy 00:00:00") ?? "";
                if (!string.IsNullOrEmpty(mensagemFalha))
                    throw new ServicoException(mensagemFalha);
            }
            else
                contabilizacao.DataVencimento = contabilizacao.DataEmissao;

            return contabilizacao;
        }

        private static List<string> ValidarContabilizacao(Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Contabilizacao contabilizacao)
        {
            List<string> erros = new List<string>();
            string numeroDocumento = contabilizacao.NumeroDocumento ?? "";

            if (string.IsNullOrEmpty(contabilizacao.NumeroDocumento))
                erros.Add($"Número do Documento não encontrado para o documento");

            if (string.IsNullOrEmpty(contabilizacao.CodigoEstabelecimento))
                erros.Add($"Código de integração do estabelecimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(contabilizacao.CodigoFornecedor))
                erros.Add($"Código de integração do fornecedor não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(contabilizacao.NaturezaOperacao))
                erros.Add($"Código de integração da natureza da operação não encontrada para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(contabilizacao.SerieDocumento))
                erros.Add($"Série do documento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(contabilizacao.DataEmissao))
                erros.Add($"Data de emissão do documento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(contabilizacao.DataVencimento))
                erros.Add($"Data de vencimento do documento não encontrado para o documento {numeroDocumento}");

            return erros;
        }

        #endregion

        #region Métodos de Integração do Documento de Faturamento  

        public async Task IntegrarPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);

            bool contabilizacao = true;
            List<PagamentoIntegracao> integracoes = repositorioPagamentoIntegracao.BuscarPorPagamento(pagamentoIntegracao.Pagamento.Codigo).OrderBy(obj => obj.Codigo).ToList();

            if (integracoes.Count > 1 && integracoes.Select(obj => obj.Codigo).FirstOrDefault() != pagamentoIntegracao.Codigo)
                contabilizacao = false;

            if (contabilizacao)
                await IntegrarContabilizacao(pagamentoIntegracao);
            else
                IntegrarDesbloqueio(pagamentoIntegracao);
        }

        private DateTime? ObterDataDigitalizacaoCanhotoPorDocumentoFaturamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            DateTime? dataDigitalizacaoCanhoto = _repositorioCanhoto.ObterUltimaDataDigitalizacaoCanhotoPorCTe(documentoFaturamento.CTe.Codigo);

            if (dataDigitalizacaoCanhoto == DateTime.MinValue && documentoFaturamento.CTe?.XMLNotaFiscais != null)
            {
                ICollection<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = documentoFaturamento.CTe.XMLNotaFiscais;
                int codigoNotaFiscal = notasFiscais.FirstOrDefault()?.Codigo ?? 0;

                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = Servicos.Embarcador.Escrituracao.Provisao.ObterCanhotosPorDocumentoProvisao(documentoFaturamento.Carga?.Codigo, codigoNotaFiscal, _repositorioCanhoto);

                if (canhotos != null)
                {
                    canhotos = canhotos
                        .Where(canhoto => canhoto.SituacaoDigitalizacaoCanhoto == SituacaoDigitalizacaoCanhoto.Digitalizado && canhoto.DataDigitalizacao != null)
                        .ToList();

                    dataDigitalizacaoCanhoto = canhotos.OrderByDescending(canhoto => canhoto.DataDigitalizacao)
                        .FirstOrDefault()?.DataDigitalizacao;
                }
            }

            return dataDigitalizacaoCanhoto;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DesbloqueioTitulo ObterDesbloqueioTituloPorDocumentoFaturamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, long protocoloIntegracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DesbloqueioTitulo desbloqueioTitulo = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.DesbloqueioTitulo();
            desbloqueioTitulo.ProtocoloIntegracao = protocoloIntegracao;
            desbloqueioTitulo.CodigoEstabelecimento = documentoFaturamento.Tomador.CodigoIntegracao;
            desbloqueioTitulo.CodigoFornecedor = documentoFaturamento.Empresa?.CodigoIntegracao;
            desbloqueioTitulo.CodigoTipoOperacao = documentoFaturamento.TipoOperacao?.CodigoIntegracao;

            if (documentoFaturamento.CTe != null)
            {
                desbloqueioTitulo.NumeroTitulo = (documentoFaturamento.CTe?.Numero ?? 0).ToString();
                desbloqueioTitulo.SerieTitulo = (documentoFaturamento.CTe?.Serie?.Numero ?? 0).ToString();
            }
            else if (documentoFaturamento.LancamentoNFSManual != null)
            {
                desbloqueioTitulo.NumeroTitulo = (documentoFaturamento.LancamentoNFSManual?.DadosNFS?.Numero ?? 0).ToString();
                desbloqueioTitulo.SerieTitulo = (documentoFaturamento.LancamentoNFSManual?.DadosNFS?.Serie?.Numero ?? 0).ToString();
            }

            return desbloqueioTitulo;
        }

        private static List<string> ValidarDesbloqueioTitulo(DesbloqueioTitulo desbloqueioTitulo)
        {
            List<string> erros = new List<string>();
            string numeroDocumento = desbloqueioTitulo.NumeroTitulo ?? "";

            if (string.IsNullOrEmpty(desbloqueioTitulo.NumeroTitulo))
                erros.Add($"Número do Título não encontrado para o documento");

            if (string.IsNullOrEmpty(desbloqueioTitulo.CodigoEstabelecimento))
                erros.Add($"Código de integração do estabelecimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(desbloqueioTitulo.DataVencimento))
                erros.Add($"Data de Vencimento não encontrado para o documento {numeroDocumento}");

            if (string.IsNullOrEmpty(desbloqueioTitulo.CodigoFornecedor))
                erros.Add($"Código de integração do fornecedor não encontrado para o documento {numeroDocumento}");

            return erros;
        }

        #endregion

        #region Métodos Privados de Requisição

        private static Localidade ObterLocalidadeOrigemNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal)
        {
            return notaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ?
                notaFiscal.Emitente?.Localidade
                : notaFiscal.Destinatario?.Localidade;
        }

        private static Localidade ObterLocalidadeDestinoNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal)
        {
            return notaFiscal.TipoOperacaoNotaFiscal == TipoOperacaoNotaFiscal.Saida ?
                notaFiscal.Destinatario?.Localidade
                : notaFiscal.Emitente?.Localidade;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal> ObterNotasFiscaisPorLancamentoNFSManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal>();
            if (lancamentoNFSManual != null)
                notasFiscais.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal()
                {
                    CodigoEstabelecimentoNota = lancamentoNFSManual.Tomador.CodigoIntegracao,
                    NumeroNotaFiscal = lancamentoNFSManual.DadosNFS?.Numero.ToString(),
                    SerieNotaFiscal = lancamentoNFSManual.DadosNFS?.Serie?.Numero.ToString()
                });

            return notasFiscais;
        }

        private async Task<List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal>> ObterNotasFiscaisPorCTe(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            ConhecimentoDeTransporteEletronico cte = documentoFaturamento.CTe;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = documentoFaturamento.CargaPagamento.Filial;

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal>();
            if (cte == null || cte.XMLNotaFiscais == null)
                return notasFiscais;

            List<string> cnpjs = cte.XMLNotaFiscais.Select(nf => nf.Emitente.CPF_CNPJ.ToString()).Distinct().ToList();

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = await _repositorioFilial.BuscarPorCNPJSAsync(cnpjs);

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in cte.XMLNotaFiscais)
            {
                notasFiscais.Add(await ObterNotasFiscaisPorXMLNotaFiscal(notaFiscal, filiais, filial));
            }

            return notasFiscais;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal> ObterNotasFiscaisPorXMLNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, List<Dominio.Entidades.Embarcador.Filiais.Filial> filials, Dominio.Entidades.Embarcador.Filiais.Filial filialparam)
        {
            if (notaFiscal == null)
                return new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal();

            Dominio.Entidades.Embarcador.Filiais.Filial filial = filials.Where(filial => filial.CNPJ == notaFiscal.Emitente.CPF_CNPJ.ToString()).FirstOrDefault() ?? filialparam;

            if (filial == null)
                throw new ServicoException($"Não foi possível obter a filial emitente da NF {notaFiscal.Numero.ToString()}");


            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.NotaFiscal()
            {
                CodigoEstabelecimentoNota = filial.CodigoFilialEmbarcador ?? "",
                NumeroNotaFiscal = notaFiscal.Numero.ToString(),
                SerieNotaFiscal = notaFiscal.Serie,
                Chave = notaFiscal.Chave
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto> ObterImpostosPorDocumentoFaturamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = documentoFaturamento.CTe;

            if (cte == null)
                return new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto>();

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoCTe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.ICMS,
                AliquotaImposto = cte.AliquotaICMS,
                ValorBaseImposto = cte.BaseCalculoICMS,
                ValorImposto = cte.ValorICMS,
                DescricaoImposto = "ICMS",
                CST = cte.CSTICMS.ObterDescricao(),
            };
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoNFSe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.ISS,
                AliquotaImposto = cte.AliquotaISS,
                ValorBaseImposto = cte.BaseCalculoISS,
                ValorImposto = cte.ValorISSRetido,
                DescricaoImposto = "ISS"
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoCBS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.CBS,
                AliquotaImposto = cte.AliquotaCBS,
                ValorBaseImposto = cte.BaseCalculoIBSCBS,
                ValorImposto = cte.ValorCBS,
                DescricaoImposto = "CBS",
                CST = cte?.OutrasAliquotas?.CST,
                ClassTrib = cte?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                PerctReducao = cte.PercentualReducaoCBS
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoIBSMunicipal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.IBSMunicipal,
                AliquotaImposto = cte.AliquotaIBSMunicipal,
                ValorBaseImposto = cte.BaseCalculoIBSCBS,
                ValorImposto = cte.ValorIBSMunicipal,
                DescricaoImposto = "IBS Municipal",
                CST = cte?.OutrasAliquotas?.CST,
                ClassTrib = cte?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                PerctReducao = cte.PercentualReducaoIBSMunicipal
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoIBSEstadual = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.IBSEstadual,
                AliquotaImposto = cte.AliquotaIBSEstadual,
                ValorBaseImposto = cte.BaseCalculoIBSCBS,
                ValorImposto = cte.ValorIBSEstadual,
                DescricaoImposto = "IBS  Estadual",
                CST = cte?.OutrasAliquotas?.CST,
                ClassTrib = cte?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                PerctReducao = cte.PercentualReducaoIBSEstadual
            };

            if (cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && cte.PercentualISSRetido == 0)
            {
                impostoNFSe.AliquotaImposto = 0;
                impostoNFSe.ValorBaseImposto = 0;
                impostoNFSe.ValorImposto = 0;
            }

            return new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto>()
            {
                impostoCTe,
                impostoNFSe,
                impostoCBS,
                impostoIBSMunicipal,
                impostoIBSEstadual
            };
        }

        private string ObterNaturezaOperacaoPorDocumentoFaturamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            return ObterNaturezaOperacaoPorDocumento(documentoFaturamento.CTe, documentoFaturamento.LancamentoNFSManual);
        }

        private string ObterNaturezaOperacaoPorDocumentoProvisao(DocumentoProvisao primeiroDocumentoProvisao)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = primeiroDocumentoProvisao.XMLNotaFiscal;

            if (notaFiscal == null)
                throw new ServicoException("Não foi possível identificar a NF-e da provisão. Fluxo de Provisão por CT-e Terceiro não implementado.");

            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPrimeiroPorCargaEXMLNotaFiscal(primeiroDocumentoProvisao.Carga.Codigo, notaFiscal.Codigo);

            if (cargaCTe != null && (cargaCTe.CTe != null || cargaCTe.LancamentoNFSManual != null))
                return ObterNaturezaOperacaoPorDocumento(cargaCTe.CTe, cargaCTe.LancamentoNFSManual);
            else
            {
                Localidade origem = ObterLocalidadeOrigemNotaFiscal(notaFiscal);
                Localidade destino = ObterLocalidadeDestinoNotaFiscal(notaFiscal);

                Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(_unitOfWork);
                Dominio.Entidades.Aliquota aliquota = svcICMS.ObterAliquota(primeiroDocumentoProvisao.Carga.Empresa.Localidade.Estado, origem.Estado, destino.Estado, notaFiscal.Emitente.Atividade, notaFiscal.Destinatario.Atividade, _unitOfWork);

                return aliquota?.CFOP?.CodigoCFOP.ToString() ?? string.Empty;
            }
        }

        private string ObterNaturezaOperacaoPorDocumento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            if (lancamentoNFSManual != null)
                return lancamentoNFSManual.DadosNFS?.ServicoNFSe?.Numero ?? string.Empty;

            if (cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                return ObterNaturezaOperacaoNFSe(cte);
            else
                return (cte.CFOP?.CodigoCFOP ?? 0).ToString();
        }

        private string ObterNaturezaOperacaoNFSe(ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.NFSeItem repositorioItemNFSe = new Repositorio.NFSeItem(_unitOfWork);

            Dominio.Entidades.NFSeItem itemNFSe = repositorioItemNFSe.BuscarPrimeiroPorCTe(cte.Codigo);

            if (itemNFSe == null)
                return ObterTransportadorConfiguracaoNFSePorCTe(cte)?.ServicoNFSe.Numero ?? string.Empty;

            return itemNFSe.Servico.Numero ?? string.Empty;
        }

        private Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe ObterTransportadorConfiguracaoNFSePorCTe(ConhecimentoDeTransporteEletronico cte)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repConfigNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(_unitOfWork);

            Localidade localidadeTomador = cte.Tomador?.Localidade ?? null;

            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repConfigNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo, cte.LocalidadeInicioPrestacao?.Codigo ?? 0, localidadeTomador?.Estado?.Sigla ?? "", 0, localidadeTomador?.Codigo ?? 0);

            if (transportadorConfiguracaoNFSe == null)
                transportadorConfiguracaoNFSe = repConfigNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo, cte.LocalidadeInicioPrestacao?.Codigo ?? 0, "", 0, 0);

            if (transportadorConfiguracaoNFSe == null && cte.LocalidadeInicioPrestacao != null)
                transportadorConfiguracaoNFSe = repConfigNFSe.BuscarPorEmpresaELocalidade(cte.Empresa.Codigo, 0, "", 0, 0);

            return transportadorConfiguracaoNFSe;
        }

        private List<Documento> ObterDocumentosPorDocumentoFaturamentoEPagamento(Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento)
        {

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repDocumentoProvisao.BuscarPorDocumentoFaturamentoEPagamento(documentoFaturamento.Codigo, pagamento.Codigo);

            List<Documento> documentos = new List<Documento>();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisao)
            {
                documentos.Add(new Documento
                {
                    CodigoEstabelecimentoDocumento = documentoProvisao.Tomador?.CodigoIntegracao ?? string.Empty,
                    CodigoFornecedorDocumento = documentoProvisao.Empresa?.CodigoIntegracao,
                    NaturezaOperacaoDocumento = ObterNaturezaOperacaoPorDocumentoProvisao(documentoProvisao).ToString(),
                    NumeroDocumento = documentoProvisao.Provisao?.Numero.ToString() ?? string.Empty,
                    SerieDocumento = ""
                });
            }

            return documentos;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto> ObterImpostosPorXMLNotaFiscal(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, int codigoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto> impostos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto>();
            if (xmlNotaFiscal == null)
                return impostos;

            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = _repositorioPedidoXMLNotaFiscal.BuscarPorNotaFiscalECarga(xmlNotaFiscal.Codigo, codigoCarga);
            CargaPedido cargaPedido = pedidoXMLNotaFiscal.CargaPedido;

            if (cargaPedido == null)
                return impostos;

            if (cargaPedido.ValorISS > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.ISS,
                    AliquotaImposto = cargaPedido.PercentualAliquotaISS,
                    ValorBaseImposto = cargaPedido.BaseCalculoISS,
                    ValorImposto = cargaPedido.ValorISS,
                    DescricaoImposto = "ISS"
                });

            if (cargaPedido.ValorICMSIncluso > 0 || cargaPedido.ValorICMS > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = cargaPedido.CST == "60" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.ICMSST : Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.ICMS,
                    AliquotaImposto = cargaPedido.PercentualAliquota,
                    ValorBaseImposto = cargaPedido.BaseCalculoICMS,
                    ValorImposto = cargaPedido.ValorICMSIncluso > 0 ? cargaPedido.ValorICMSIncluso : cargaPedido.ValorICMS,
                    DescricaoImposto = cargaPedido.CST == "60" ? "ICMS ST" : "ICMS",
                    CST = cargaPedido.CST
                });

            if (cargaPedido.ValorPis > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.PIS,
                    AliquotaImposto = cargaPedido.AliquotaPis,
                    ValorImposto = cargaPedido.ValorPis,
                    DescricaoImposto = "PIS",
                });

            if (cargaPedido.ValorCofins > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.Cofins,
                    AliquotaImposto = cargaPedido.AliquotaCofins,
                    ValorImposto = cargaPedido.ValorCofins,
                    DescricaoImposto = "COFINS"
                });
            if (cargaPedido.ValorCBS > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.CBS,
                    AliquotaImposto = cargaPedido.AliquotaCBS,
                    ValorImposto = cargaPedido.ValorCBS,
                    DescricaoImposto = "CBS",
                    CST = cargaPedido?.OutrasAliquotas?.CST,
                    ClassTrib = cargaPedido?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                    PerctReducao = cargaPedido.PercentualReducaoCBS
                });

            if (cargaPedido.ValorIBSMunicipal > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.IBSMunicipal,
                    AliquotaImposto = cargaPedido.AliquotaIBSMunicipal,
                    ValorImposto = cargaPedido.ValorIBSMunicipal,
                    DescricaoImposto = "IBS Municipal",
                    CST = cargaPedido?.OutrasAliquotas?.CST,
                    ClassTrib = cargaPedido?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                    PerctReducao = cargaPedido.PercentualReducaoIBSMunicipal
                });

            if (cargaPedido.ValorIBSEstadual > 0)
                impostos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto()
                {
                    CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.IBSEstadual,
                    AliquotaImposto = cargaPedido.AliquotaIBSEstadual,
                    ValorImposto = cargaPedido.ValorIBSEstadual,
                    DescricaoImposto = "IBS Estadual",
                    CST = cargaPedido?.OutrasAliquotas?.CST,
                    ClassTrib = cargaPedido?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                    PerctReducao = cargaPedido.PercentualReducaoIBSEstadual
                });

            return impostos;
        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto> ObterImpostosPorLancamentoNFSManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto> impostos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto>();

            if (lancamentoNFSManual == null)
                return impostos;

            // ISS
            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoISS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.ISS,
                AliquotaImposto = lancamentoNFSManual.DadosNFS?.AliquotaISS ?? 0,
                ValorBaseImposto = lancamentoNFSManual.DadosNFS?.ValorBaseCalculo ?? 0,
                ValorImposto = lancamentoNFSManual.DadosNFS?.ValorISS ?? 0,
                DescricaoImposto = "ISS"
            };

            if (lancamentoNFSManual.DadosNFS.PercentualRetencao == 0)
            {
                impostoISS.AliquotaImposto = 0;
                impostoISS.ValorBaseImposto = 0;
                impostoISS.ValorImposto = 0;
            }

            impostos.Add(impostoISS);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoCBS = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.CBS,
                AliquotaImposto = lancamentoNFSManual.DadosNFS?.AliquotaCBS ?? 0,
                ValorBaseImposto = lancamentoNFSManual.DadosNFS?.ValorBaseCalculo ?? 0,
                ValorImposto = lancamentoNFSManual.DadosNFS?.ValorCBS ?? 0,
                DescricaoImposto = "CBS",
                CST = lancamentoNFSManual?.CTe?.OutrasAliquotas?.CST,
                ClassTrib = lancamentoNFSManual?.CTe?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                PerctReducao = lancamentoNFSManual?.CTe?.PercentualReducaoCBS ?? 0
            };

            impostos.Add(impostoCBS);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoIBSMunicipal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.IBSMunicipal,
                AliquotaImposto = lancamentoNFSManual.DadosNFS?.AliquotaIBSMunicipal ?? 0,
                ValorBaseImposto = lancamentoNFSManual.DadosNFS?.ValorBaseCalculo ?? 0,
                ValorImposto = lancamentoNFSManual.DadosNFS?.ValorIBSMunicipal ?? 0,
                DescricaoImposto = "IBS Municipal",
                CST = lancamentoNFSManual?.CTe?.OutrasAliquotas?.CST,
                ClassTrib = lancamentoNFSManual?.CTe?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                PerctReducao = lancamentoNFSManual?.CTe?.PercentualReducaoIBSMunicipal ?? 0
            };

            impostos.Add(impostoIBSMunicipal);

            Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto impostoIBSEstadual = new Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.Imposto
            {
                CodigoImposto = Dominio.ObjetosDeValor.Embarcador.Integracao.Camil.TipoImposto.IBSEstadual,
                AliquotaImposto = lancamentoNFSManual.DadosNFS?.AliquotaIBSEstadual ?? 0,
                ValorBaseImposto = lancamentoNFSManual.DadosNFS?.ValorBaseCalculo ?? 0,
                ValorImposto = lancamentoNFSManual.DadosNFS?.ValorIBSEstadual ?? 0,
                DescricaoImposto = "IBS Estadual",
                CST = lancamentoNFSManual?.CTe?.OutrasAliquotas?.CST,
                ClassTrib = lancamentoNFSManual?.CTe?.OutrasAliquotas?.CodigoClassificacaoTributaria,
                PerctReducao = lancamentoNFSManual?.CTe?.PercentualReducaoIBSEstadual ?? 0
            };

            impostos.Add(impostoIBSEstadual);

            return impostos;
        }

        private static TipoTributacaoICMS ObterTipoTributacaoICMS(string CST)
        {
            switch (CST)
            {
                case "40":
                    return TipoTributacaoICMS.Isento;
                case "10":
                    return TipoTributacaoICMS.SubstituicaoTributaria;
                case "51":
                    return TipoTributacaoICMS.Diferido;
                case "90":
                case "91":
                    return TipoTributacaoICMS.Outros;
                default:
                    return TipoTributacaoICMS.Tributado;
            }
        }

        private void CarregarConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoCamil repositorioIntegracaoCamil = new Repositorio.Embarcador.Configuracoes.IntegracaoCamil(_unitOfWork);
            _configuracaoIntegracao = repositorioIntegracaoCamil.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracao == null || !_configuracaoIntegracao.PossuiIntegracao)
                throw new ServicoException("Integração com a Camil não foi configurada");

            if (string.IsNullOrEmpty(_configuracaoIntegracao.URL))
                throw new ServicoException("URL da integração com a Camil não foi configurada");

            if (string.IsNullOrEmpty(_configuracaoIntegracao.ApiKey))
                throw new ServicoException("API Key da integração com a Camil não foi configurada");
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(DadosRequisicaoCamil<T> dadosRequisicaoCamil, string endpointAcao)
        {
            HttpClient client = ObterHttpClient();

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicaoCamil, Formatting.Indented, jsonSerializerSettings);

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(string.Concat(_configuracaoIntegracao.URL.TrimEnd('/'), endpointAcao), content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private static HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        private static DateTime ObterDataComUltimoDiaDoMes(DateTime dataAtual)
        {
            DateTime ultimoDiaDoMes = new DateTime(dataAtual.Year, dataAtual.Month, DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month));
            DateTime ultimoDiaComHora = ultimoDiaDoMes.AddHours(23).AddMinutes(59).AddSeconds(59);

            return ultimoDiaComHora;
        }

        private DateTime ObterDataTransacao()
        {
            Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(_unitOfWork);

            DateTime primeiroDiaUtil = servicoProvisao.ObterDiaUtil(DateTime.Now.FirstDayOfMonth());

            DateTime segundoDiaUtil = servicoProvisao.ObterDiaUtil(primeiroDiaUtil.AddDays(1));

            return segundoDiaUtil.Day >= DateTime.Now.Day ? ObterDataComUltimoDiaDoMes(DateTime.Now.AddMonths(-1)) : DateTime.Now;
        }

        private DateTime? CalcularDataVencimento(int? codigoEmpresa, Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime? dataEmissao, out string mensagemFalha)
        {
            if (dataEmissao == null || dataEmissao == DateTime.MinValue)
            {
                mensagemFalha = "Data de emissão do documento não localizada";
                return null;
            }

            if (codigoEmpresa == null)
            {
                mensagemFalha = "Transportador não localizado";
                return null;
            }

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamento = ObterCondicoesPagamentoPorEmpresa(codigoEmpresa.Value);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento condicaoPagamento = ObterCondicaoPagamentoFiltrada(condicoesPagamento, dataEmissao.Value, carga?.TipoDeCarga?.Codigo, carga?.TipoOperacao?.Codigo);

            if (condicaoPagamento == null)
            {
                mensagemFalha = "Condicação de Pagamento não encontrada para o cálculo da data de vencimento";
                return null;
            }

            if (!condicaoPagamento.DiaSemana.HasValue)
            {
                mensagemFalha = "Dia da Semana não configurado para o cálculo da data de vencimento";
                return null;
            }

            if (!condicaoPagamento.DiasDePrazoPagamento.HasValue)
            {
                mensagemFalha = "Dia de Prazo do Pagamento não configurado para o cálculo da data de vencimento";
                return null;
            }

            DateTime dataVencimento = dataEmissao.Value;
            DayOfWeek diaSemanaCondicaoPagamento = ObterDayOfWeek(condicaoPagamento.DiaSemana.Value);

            if (condicaoPagamento.VencimentoForaMes)
                dataVencimento = dataVencimento.AddMonths(1).AddDays(-dataVencimento.Day + 1);

            dataVencimento = dataVencimento.AddDays(condicaoPagamento.DiasDePrazoPagamento.Value);

            if (dataVencimento.DayOfWeek != diaSemanaCondicaoPagamento)
                while (dataVencimento.DayOfWeek != diaSemanaCondicaoPagamento)
                    dataVencimento = dataVencimento.AddDays(-1);

            if (condicaoPagamento.ConsiderarDiaUtilVencimento ?? false)
            {
                Configuracoes.Feriado servicoFeriado = new Configuracoes.Feriado(_unitOfWork);

                while (true)
                {
                    if (dataVencimento.DayOfWeek == DayOfWeek.Saturday || dataVencimento.DayOfWeek == DayOfWeek.Sunday)
                        dataVencimento = dataVencimento.AddDays(1);
                    else if (servicoFeriado.VerificarSePossuiFeriado(dataVencimento))
                        dataVencimento = dataVencimento.AddDays(1);
                    else
                        break;
                }
            }

            mensagemFalha = "";
            return dataVencimento;
        }

        private static DayOfWeek ObterDayOfWeek(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana)
        {
            return (DayOfWeek)((int)diaSemana - 1);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> ObterCondicoesPagamentoPorEmpresa(int codigoEmpresa)
        {
            return _repositorioCondicaoPagamentoTransportador.BuscarObjetoPorEmpresa(codigoEmpresa);
        }

        private static Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento ObterCondicaoPagamentoFiltrada(List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.CondicaoPagamento> condicoesPagamento, DateTime dataDocumento, int? tipoCarga, int? tipoOperacao)
        {
            if (!condicoesPagamento.Any())
                return null;

            int diaDocumento = dataDocumento.Day;

            if (tipoCarga.HasValue)
                condicoesPagamento = (from condicao in condicoesPagamento where !condicao.CodigoTipoCarga.HasValue || condicao.CodigoTipoCarga.Value == tipoCarga.Value select condicao).ToList();
            else
                condicoesPagamento = (from condicao in condicoesPagamento where !condicao.CodigoTipoCarga.HasValue select condicao).ToList();

            if (tipoOperacao.HasValue)
                condicoesPagamento = (from condicao in condicoesPagamento where !condicao.CodigoTipoOperacao.HasValue || condicao.CodigoTipoOperacao.Value == tipoOperacao.Value select condicao).ToList();
            else
                condicoesPagamento = (from condicao in condicoesPagamento where !condicao.CodigoTipoOperacao.HasValue select condicao).ToList();

            condicoesPagamento = (
                from condicao in condicoesPagamento
                where condicao.TipoPrazoPagamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPrazoPagamento.DataLiberacaoDocumento
                select condicao
            ).ToList();

            return condicoesPagamento.FirstOrDefault();
        }

        private static string ObterErrosValidacao<T>(DadosRequisicaoCamil<T> informacaoEnvio, List<string> erros)
        {
            return JsonConvert.SerializeObject(new
            {
                DadosEnvioCamil = informacaoEnvio,
                ErrosValidacaoMulti = erros
            });
        }

        private HttpClient ObterHttpClient()
        {
            CarregarConfiguracaoIntegracao();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoCamil));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", _configuracaoIntegracao.ApiKey);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            return client;
        }

        #endregion
    }
}