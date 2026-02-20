using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Globus;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Globus
{
    public partial class IntegracaoGlobus
    {
        #region Métodos Públicos

        public void IntegrarCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repositorioCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                cargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeIntegracao.NumeroTentativas++;

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTeIntegracao.CargaCTe, null, null);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                Hashtable request = ConverterCargaCTe(cargaCTeIntegracao.CargaCTe, null, null);

                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);
                    cargaCTeIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? retorno?.CodigoDocumento?.ToString() ?? "0";
                }

                cargaCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                cargaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                cargaCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(cargaCTeIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaCTeIntegracao.Atualizar(cargaCTeIntegracao);
        }

        public void IntegrarCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCTeIntegracao.NumeroTentativas++;

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(ocorrenciaCTeIntegracao.CargaCTe, null, null);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                Hashtable request = ConverterCargaCTe(ocorrenciaCTeIntegracao.CargaCTe, null, null);

                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<retornoWebServiceSuccess>(retWS.jsonRetorno);
                    if (!string.IsNullOrEmpty(retorno?.data?.idExterno))
                        ocorrenciaCTeIntegracao.CodigoExternoRetornoIntegracao = retorno.data.idExterno;
                    else if (retorno?.CodigoDocumento.HasValue == true)
                        ocorrenciaCTeIntegracao.CodigoExternoRetornoIntegracao = retorno.CodigoDocumento.ToString();
                    else
                        ocorrenciaCTeIntegracao.CodigoExternoRetornoIntegracao = "0";
                }

                ocorrenciaCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                ocorrenciaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                {
                    message = message.Substring(0, 300);
                }
                ocorrenciaCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(ocorrenciaCTeIntegracao, jsonRequisicao, jsonRetorno, "json");

            repositorioOcorrenciaCTeIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        public void IntegrarCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repositorioCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {

                int codigoCargaCTe = repositorioCargaCTe.BuscarCodigoPorCte(cargaCTeManualIntegracao.CTe?.Codigo ?? 0, cargaCTeManualIntegracao.Carga?.Codigo ?? 0);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                #region Validar Situacao da integração da carga

                if (cargaCTe == null)
                {
                    cargaCTeManualIntegracao.NumeroTentativas++;
                    throw new Exception("CTe não possui carga vinculada");
                }

                #endregion

                cargaCTeManualIntegracao.DataIntegracao = DateTime.Now;
                cargaCTeManualIntegracao.NumeroTentativas++;

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTe, null, null);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                Hashtable request = ConverterCargaCTe(cargaCTe, null, null);

                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                    cargaCTeManualIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? retorno?.CodigoDocumento?.ToString() ?? "0";
                }

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

        public void IntegrarCargaCTeAgrupado(Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao cargaCTeAgrupadoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao repositorioCargaCTeAgrupadoIntegracao = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupadoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado cargaCTeAgrupado = repCargaCTeAgrupado.BuscarPorCodigo(cargaCTeAgrupadoIntegracao.CargaCTeAgrupado?.Codigo ?? 0, false);

                if (cargaCTeAgrupado == null || cargaCTeAgrupado.CTes == null || cargaCTeAgrupado.CTes.Count == 0)
                    throw new Exception("Carga CT-e Agrupado não posssui CTE");

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

                    cargaCTeAgrupadoIntegracao.DataIntegracao = DateTime.Now;
                    cargaCTeAgrupadoIntegracao.NumeroTentativas++;

                    var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTe, null, null);

                    string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                    Hashtable request = ConverterCargaCTe(cargaCTe, null, null);

                    var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                    if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);

                        cargaCTeAgrupadoIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? retorno?.CodigoDocumento?.ToString() ?? "0";
                    }

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

        public void IntegrarNFSEManual(Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfseManualCTeIntegracao)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoArquivo>(_unitOfWork);
            var repositorioNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            var repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
            var repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            string jsonRequisicao = "", jsonRetorno = "";

            try
            {
                var lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(nfseManualCTeIntegracao.LancamentoNFSManual?.Codigo ?? 0);
                var cargaCTe = repCargaCTe.BuscarPorCodigoCTe(lancamentoNFSManual?.CTe?.Codigo ?? 0)?.FirstOrDefault();

                #region Validar Situacao da integração da carga
                if (cargaCTe == null || lancamentoNFSManual == null)
                {
                    nfseManualCTeIntegracao.NumeroTentativas++;
                    throw new Exception("CTe não possui carga vinculada");
                }
                #endregion

                nfseManualCTeIntegracao.DataIntegracao = DateTime.Now;
                nfseManualCTeIntegracao.NumeroTentativas++;

                var configuracaoIntegracao = ObterConfiguracaoComunicacao(cargaCTe, null, null);

                string token = ObterToken(configuracaoIntegracao.EndPointToken, configuracaoIntegracao.URLWebService, configuracaoIntegracao.ShortCode, configuracaoIntegracao.Token, true);

                Hashtable request = ConverterCargaCTe(cargaCTe, null, null);

                var retWS = this.Transmitir(request, configuracaoIntegracao.EndPoint, token, configuracaoIntegracao.URLWebService);

                if (retWS != null && retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado && !String.IsNullOrEmpty(retWS.jsonRetorno))
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Globus.retornoWebServiceSuccess>(retWS.jsonRetorno);
                    nfseManualCTeIntegracao.CodigoExternoRetornoIntegracao = retorno?.data?.idExterno ?? retorno?.CodigoDocumento?.ToString() ?? "0";
                }

                nfseManualCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                nfseManualCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                jsonRequisicao = retWS.jsonRequisicao;
                jsonRetorno = retWS.jsonRetorno;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                nfseManualCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                String message = excecao.Message;
                if (message.Length > 300)
                    message = message.Substring(0, 300);
                nfseManualCTeIntegracao.ProblemaIntegracao = message;
            }

            servicoArquivoTransacao.Adicionar(nfseManualCTeIntegracao, jsonRequisicao, jsonRetorno, "json");
            repositorioNFSManualCTeIntegracao.Atualizar(nfseManualCTeIntegracao);
        }

        #endregion

        #region Métodos Privados
        public SituacaoIntegracao BuscarSituacaoIntegracaoCargaGlobus(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            return repositorioCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus)?.SituacaoIntegracao ?? SituacaoIntegracao.ProblemaIntegracao;
        }

        #endregion
    }
}
