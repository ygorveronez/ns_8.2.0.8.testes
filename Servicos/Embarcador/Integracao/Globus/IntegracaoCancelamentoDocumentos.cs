using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        #region Métodos Pùblicos
        public void IntegrarCancelamentoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repositorioCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {
                cargaCancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaCTeIntegracao.NumeroTentativas++;

                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
                var cargaCteIntegracao = repositorioCargaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe.Codigo, TipoIntegracao.Globus).FirstOrDefault();
                int codigoDocumento = int.TryParse(cargaCteIntegracao?.CodigoExternoRetornoIntegracao, out var resultado) ? resultado : 0;
                if (cargaCteIntegracao == null || codigoDocumento == 0)
                    throw new Exception("Não foi localizada a integração desse documento com o Globus!");

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCancelamentoCargaCTeIntegracao.CargaCTe, null, null, true);

                if (!ValidarIntegracaoEnvioCargaCTe(cargaCancelamentoCargaCTeIntegracao, ref mensagemErro))
                    throw new Exception("Erro ao integrar envio do documento: " + mensagemErro);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);
                Hashtable request = ConverterCargaCTe(cargaCancelamentoCargaCTeIntegracao.CargaCTe, null, null, true, codigoDocumento);
                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService, configuracaoIntegracao.Method);

                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaCancelamentoCargaCTeIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCancelamentoCargaCTeIntegracao.Atualizar(cargaCancelamentoCargaCTeIntegracao);
        }

        public void IntegrarCancelamentoCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao ocorrenciaCTeCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repositorioOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo>(_unitOfWork);
        
            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {
                ocorrenciaCTeCancelamentoIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCTeCancelamentoIntegracao.NumeroTentativas++;

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
                var ocorrenciaCteIntegracao = repositorioOcorrenciaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe.Codigo, TipoIntegracao.Globus);
                int codigoDocumento = int.TryParse(ocorrenciaCteIntegracao?.CodigoExternoRetornoIntegracao, out var resultado) ? resultado : 0;
                if (ocorrenciaCteIntegracao == null || codigoDocumento == 0)
                    throw new Exception("Não foi localizada a integração desse documento com o Globus!");

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe, null, null, true);

                if (!ValidarIntegracaoEnvioCargaCTeOcorrencia(ocorrenciaCTeCancelamentoIntegracao, ref mensagemErro))
                    throw new Exception("Erro ao integrar envio do documento: " + mensagemErro);

                Hashtable request = ConverterCargaCTe(ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe, null, null, true, codigoDocumento);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);
                
                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService, configuracaoIntegracao.Method);
                
                ocorrenciaCTeCancelamentoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                ocorrenciaCTeCancelamentoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                ocorrenciaCTeCancelamentoIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeCancelamentoIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioOcorrenciaCTeCancelamentoIntegracao.Atualizar(ocorrenciaCTeCancelamentoIntegracao);
        }

        public void IntegrarCancelamentoCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repositorioCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                cargaCTeManualIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeManualIntegracao.NumeroTentativas++;

                int codigoCargaCTe = repositorioCargaCTe.BuscarCodigoPorCte(cargaCTeManualIntegracao.CTe?.Codigo ?? 0, cargaCTeManualIntegracao.Carga?.Codigo ?? 0);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                var cargaCteIntegracao = repositorioCargaCTeManualIntegracao.BuscarPorCargaECTeTipo(cargaCTe?.Carga?.Codigo ?? 0, cargaCTe?.CTe?.Codigo ?? 0, TipoIntegracao.Globus);

                int codigoDocumento = int.TryParse(cargaCteIntegracao?.CodigoExternoRetornoIntegracao, out var resultado) ? resultado : 0;

                if (cargaCteIntegracao == null || codigoDocumento == 0)
                    throw new Exception("Não foi localizada a integração desse documento com o Globus!");


                var retWS = new retornoWebService();
                var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTe, null, null);

                Hashtable request = ConverterCargaCTe(cargaCTe, null, null, true, codigoDocumento);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                cargaCTeManualIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaCTeManualIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeManualIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCTeManualIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaCTeManualIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCTeManualIntegracao.Atualizar(cargaCTeManualIntegracao);
        }

        public void IntegrarCancelamentoCargaCTeAgrupado(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao cargaCTeAgrupadoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repositorioCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                cargaCTeAgrupadoIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeAgrupadoIntegracao.NumeroTentativas++;


                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupadoIntegracao.CargaCTeAgrupado?.Codigo ?? 0, false);

                if (cargaCTeAgrupado == null || cargaCTeAgrupado.CTes == null || cargaCTeAgrupado.CTes.Count == 0)
                    throw new Exception("Carga CT-e Agrupado não posssui CTE");

                var cargaCteAgrupadoIntegracao = repositorioCargaCTeAgrupadoIntegracao.BuscarPorCargaCTeAgrupadoETipo(cargaCTeAgrupado.Codigo, TipoIntegracao.Globus, TipoAcaoIntegracao.Autorizacao).FirstOrDefault();
                int codigoDocumento = int.TryParse(cargaCteAgrupadoIntegracao?.CodigoExternoRetornoIntegracao, out var resultado) ? resultado : 0;
                if (cargaCteAgrupadoIntegracao == null || codigoDocumento == 0)
                    throw new Exception("Não foi localizada a integração desse documento com o Globus!");

                foreach (var cte in cargaCTeAgrupado.CTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigoCTe(cte.Codigo)?.FirstOrDefault();

                    #region Validar Situacao da integração da carga

                    if (cargaCTe == null)
                    {
                        cargaCTeAgrupadoIntegracao.NumeroTentativas++;
                        throw new Exception("CTe não possui carga vinculada");
                    }

                    #endregion

                    var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTe, null, null);

                    Hashtable request = ConverterCargaCTe(cargaCTe, null, null, true, codigoDocumento);

                    string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                    var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                    cargaCTeAgrupadoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                    cargaCTeAgrupadoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;
                }

                servicoArquivoTransacao.Adicionar(cargaCTeAgrupadoIntegracao, jsonRequisicao, jsonRetorno, "json");

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeAgrupadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCTeAgrupadoIntegracao.ProblemaIntegracao = message;
            }

            repositorioCargaCTeAgrupadoIntegracao.Atualizar(cargaCTeAgrupadoIntegracao);
        }

        public void IntegrarCancelamentoNFSeManual(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfseManualCancelamentoIntegracaoCTe)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repositorioNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoGlobus repositorioIntegracaoGlobus = new Repositorio.Embarcador.Configuracoes.IntegracaoGlobus(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(nfseManualCancelamentoIntegracaoCTe.LancamentoNFSManual?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPrimeiroPorLancamentoNFSManual(nfseManualCancelamentoIntegracaoCTe.LancamentoNFSManual.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigoCTe(lancamentoNFSManual?.CTe?.Codigo ?? 0)?.FirstOrDefault();
            
            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {
                if (cargaCTe == null || lancamentoNFSManual == null)
                {
                    throw new Exception("O código da NFS não foi encontrado!");
                }

                nfseManualCancelamentoIntegracaoCTe.DataIntegracao = DateTime.Now;
                nfseManualCancelamentoIntegracaoCTe.NumeroTentativas++;

                this.ValidarEnvioIntegracao(nfseManualCancelamentoIntegracaoCTe.LancamentoNFSManual, TipoAcaoIntegracao.Cancelamento);

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTe, null, null, true);
                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);
                string codigoIntegracaoDocumentoGlobus = nfsManualCTeIntegracao.CodigoExternoRetornoIntegracao ?? "0";

                Hashtable request = new Hashtable
                {
                    { "CodigoDocumento", codigoIntegracaoDocumentoGlobus },
                    { "Usuario", _configuracaoIntegracao.Usuario },
                    { "ExcluirDocumentoFiscal", false }
                };

                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService, enumTipoWS.DELETE);

                if (retWS != null && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    var retorno = JsonConvert.DeserializeObject<retornoWebServiceError>(retWS.jsonRetorno);
                    if (retorno.success == true || retWS.ProblemaIntegracao.Contains("Documento cancelado"))
                        nfseManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    else
                        nfseManualCancelamentoIntegracaoCTe.SituacaoIntegracao = retWS.SituacaoIntegracao;
                }

                nfseManualCancelamentoIntegracaoCTe.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                nfseManualCancelamentoIntegracaoCTe.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                nfseManualCancelamentoIntegracaoCTe.ProblemaIntegracao = message;
            }

            AdicionarArquivoTransacaoIntegracaoNFSManual(nfseManualCancelamentoIntegracaoCTe, jsonRequisicao, jsonRetorno, _unitOfWork);

            repositorioNFSManualCancelamentoIntegracaoCTe.Atualizar(nfseManualCancelamentoIntegracaoCTe);
        }

        #endregion

        #region Métodos Privados
        private bool ValidarIntegracaoEnvioCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao, ref string mensagemErro)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = repCargaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus).FirstOrDefault();

            if (cargaCTeIntegracao == null)
            {
                AdicionarCargaCTeParaIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe, cargaCancelamentoCargaCTeIntegracao.TipoIntegracao);
                cargaCTeIntegracao = repCargaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus).FirstOrDefault();
            }

            if (cargaCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
            {
                this.IntegrarCargaCTe(cargaCTeIntegracao);

                cargaCTeIntegracao = repCargaCTeIntegracao.BuscarPorCodigo(cargaCTeIntegracao.Codigo);

                if (cargaCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                {
                    mensagemErro = cargaCTeIntegracao.ProblemaIntegracao;
                    return false;
                }

            }

            if (cargaCancelamentoCargaCTeIntegracao.CargaCTe.CTe.Status.Equals("Z"))
                mensagemErro = "Não é possível efetuar o cancelamento do documento, pois o mesmo se encontra anulado gerancialmente";

            return true;
        }

        private bool ValidarIntegracaoEnvioCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao ocorrenciaCTeCancelamentoIntegracao, ref string mensagemErro)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = repOcorrenciaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus);

            if (ocorrenciaCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
            {
                this.IntegrarCargaCTeOcorrencia(ocorrenciaCTeIntegracao);

                ocorrenciaCTeIntegracao = repOcorrenciaCTeIntegracao.BuscarPorCodigo(ocorrenciaCTeIntegracao.Codigo);

                if (ocorrenciaCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                {
                    mensagemErro = ocorrenciaCTeIntegracao.ProblemaIntegracao;
                    return false;
                }

            }

            if (ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe.CTe.Status.Equals("Z"))
                mensagemErro = "Não é possível efeturar o cancelamento do documento, pois o mesmo se encontra anulado gerancialmente";

            return true;
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao AdicionarCargaCTeParaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao();
            cargaCTeIntegracao.CargaCTe = cargaCte;
            cargaCTeIntegracao.DataIntegracao = DateTime.Now;
            cargaCTeIntegracao.NumeroTentativas = 0;
            cargaCTeIntegracao.ProblemaIntegracao = "";
            cargaCTeIntegracao.TipoIntegracao = tipoIntegracao;
            cargaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            repCargaCTeIntegracao.Inserir(cargaCTeIntegracao);

            return cargaCTeIntegracao;
        }

        private Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao AdicionarCargaCTeOcorrenciaParaIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao();
            ocorrenciaCTeIntegracao.CargaCTe = cargaCte;
            ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
            ocorrenciaCTeIntegracao.NumeroTentativas = 0;
            ocorrenciaCTeIntegracao.ProblemaIntegracao = "";
            ocorrenciaCTeIntegracao.TipoIntegracao = tipoIntegracao;
            ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            repOcorrenciaCTeIntegracao.Inserir(ocorrenciaCTeIntegracao);

            return ocorrenciaCTeIntegracao;
        }

        private static void AdicionarArquivoTransacaoIntegracaoNFSManual(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfsManualCancelamentoIntegracaoCTe, string jsonRequisicao, string jsonRetorno, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, nfsManualCancelamentoIntegracaoCTe.DataIntegracao, nfsManualCancelamentoIntegracaoCTe.ProblemaIntegracao, unitOfWork);

            if (arquivoIntegracao == null)
                return;

            if (nfsManualCancelamentoIntegracaoCTe.ArquivosTransacao == null)
                nfsManualCancelamentoIntegracaoCTe.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>();

            nfsManualCancelamentoIntegracaoCTe.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private static Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo AdicionarArquivoTransacao(string jsonRequisicao, string jsonRetorno, DateTime data, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo repositorio = new Repositorio.Embarcador.NFS.NFSManualIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", unitOfWork),
                Data = data,
                Mensagem = mensagem,
                Tipo = TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
            };

            repositorio.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        private void ValidarEnvioIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao tipoAcaoIntegracao)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPorLancamentoNFSManualETipoIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus);

            if (tipoAcaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoIntegracao.Cancelamento)
            {
                if (nfsManualCTeIntegracao.CodigoExternoRetornoIntegracao == null)
                    throw new Exception(@"NFS Manual não foi integrada com sucesso, portanto não é possível cancelamento");
            }

        }
        #endregion
    }
}
