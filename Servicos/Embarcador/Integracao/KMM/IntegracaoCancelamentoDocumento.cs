using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.KMM;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas;
using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Enumeradores;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using Repositorio.Embarcador.Cargas;

namespace Servicos.Embarcador.Integracao.KMM
{
    public partial class IntegracaoKMM
    {
        #region Métodos Públicos

        public void IntegrarCancelamentoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao repositorioCargaCancelamentoCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {
                cargaCancelamentoCargaCTeIntegracao.DataIntegracao = DateTime.Now;
                cargaCancelamentoCargaCTeIntegracao.NumeroTentativas++;

                if (!ValidarIntegracaoEnvioCargaCTe(cargaCancelamentoCargaCTeIntegracao, ref mensagemErro))
                    throw new Exception("Erro ao integrar envio do documento: " + mensagemErro);

                if (cargaCancelamentoCargaCTeIntegracao.CargaCTe.CTe.Status == "C") 
                {
                    Hashtable parameters = ConverterCancelamentoCargaCTe(cargaCancelamentoCargaCTeIntegracao.CargaCTe);

                    Hashtable request = new Hashtable
                    {
                        { "module", "M1076" },
                        { "operation", "cancelarDocumento" },
                        { "parameters", parameters }
                    };

                    var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                    cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                    cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;
                }
                else
                {
                    cargaCancelamentoCargaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                    if (cargaCancelamentoCargaCTeIntegracao.CargaCTe.CTe.Status == "Z")
                        cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = "Autorização do documento integrado com sucesso, o documento encontra-se com status de anulado gerencialmente, por esse motivo não foi realizado integração do cancelamento";
                    else
                        cargaCancelamentoCargaCTeIntegracao.ProblemaIntegracao = "Autorização do documento integrado com sucesso, o documento encontra-se com status diferente de cancelado, por esse motivo não foi realizado integração do cancelamento";

                }             
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
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracaoArquivo>(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {

                ocorrenciaCTeCancelamentoIntegracao.DataIntegracao = DateTime.Now;
                ocorrenciaCTeCancelamentoIntegracao.NumeroTentativas++;

                if (!ValidarIntegracaoEnvioCargaCTeOcorrencia(ocorrenciaCTeCancelamentoIntegracao, ref mensagemErro))
                    throw new Exception("Erro ao integrar envio do documento: " + mensagemErro);

                if (ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe.CTe.Status == "C")
                {
                    Hashtable parameters = ConverterCancelamentoCargaCTe(ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe);

                    Hashtable request = new Hashtable
                    {
                        { "module", "M1076" },
                        { "operation", "cancelarDocumento" },
                        { "parameters", parameters }
                    };

                    var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                    ocorrenciaCTeCancelamentoIntegracao.SituacaoIntegracao = retWS.SituacaoIntegracao;
                    ocorrenciaCTeCancelamentoIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                    jsonRequisicao = retWS.jsonRequisicao;
                    jsonRetorno = retWS.jsonRetorno;
                }
                else
                {
                    ocorrenciaCTeCancelamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                    if (ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe.CTe.Status == "Z")
                        ocorrenciaCTeCancelamentoIntegracao.ProblemaIntegracao = "Autorização do documento integrado com sucesso, o documento encontra-se com status de anulado gerencialmente, por esse motivo não foi realizado integração do cancelamento";
                    else
                        ocorrenciaCTeCancelamentoIntegracao.ProblemaIntegracao = "Autorização do documento integrado com sucesso, o documento encontra-se com status diferente de cancelado, por esse motivo não foi realizado integração do cancelamento";

                }

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

        public void IntegrarCancelamentoNFSeManual(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe nfseManualCancelamentoIntegracaoCTe)
        {
            Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repositorioNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

            string jsonRequisicao = "";
            string jsonRetorno = "";
            string mensagemErro = "";
            try
            {

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(nfseManualCancelamentoIntegracaoCTe.LancamentoNFSManual?.Codigo ?? 0);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigoCTe(lancamentoNFSManual?.CTe?.Codigo ?? 0)?.FirstOrDefault();

                if (cargaCTe == null || lancamentoNFSManual == null)
                {
                    throw new Exception("CTe não possui carga vinculada");
                }

                nfseManualCancelamentoIntegracaoCTe.DataIntegracao = DateTime.Now;
                nfseManualCancelamentoIntegracaoCTe.NumeroTentativas++;

                if (!ValidarIntegracaoEnvioNFSeManual(lancamentoNFSManual, ref mensagemErro))
                    throw new Exception("Erro ao integrar envio do documento: " + mensagemErro);

                Hashtable parameters = ConverterCancelamentoCargaCTe(cargaCTe, nfseManualCancelamentoIntegracaoCTe.LancamentoNFSManual);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "cancelarDocumento" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

                nfseManualCancelamentoIntegracaoCTe.SituacaoIntegracao = retWS.SituacaoIntegracao;
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

        public void IntegrarCancelamentoCargaCTeManual(Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repositorioCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();

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


                Hashtable parameters = ConverterCancelamentoCargaCTe(cargaCTe);

                Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "cancelarDocumento" },
                    { "parameters", parameters }
                };

                var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

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
            Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(_unitOfWork);
            Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado repCargaCTeAgrupado = new Repositorio.Embarcador.Cargas.CTeAgrupado.CargaCTeAgrupado(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(_unitOfWork);

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.Buscar();
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

                    SituacaoIntegracao situacaoIntegracaoKMM = BuscarSituacaoIntegracaoCargaKMM(cargaCTe?.Carga);
                    if (situacaoIntegracaoKMM == SituacaoIntegracao.ProblemaIntegracao)
                    {
                        cargaCTeAgrupadoIntegracao.NumeroTentativas++;
                        throw new Exception("Problema na integração da carga");
                    }
                    else if (situacaoIntegracaoKMM == SituacaoIntegracao.AgIntegracao)
                        return;

                    #endregion

                    cargaCTeAgrupadoIntegracao.DataIntegracao = DateTime.Now;
                    cargaCTeAgrupadoIntegracao.NumeroTentativas++;


                    Hashtable parameters = ConverterCancelamentoCargaCTe(cargaCTe);

                    Hashtable request = new Hashtable
                {
                    { "module", "M1076" },
                    { "operation", "cancelarDocumento" },
                    { "parameters", parameters }
                };

                    var retWS = this.Transmitir(configuracaoIntegracaoKMM, request);

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
        #endregion Métodos Públicos

        #region Métodos Privados

        private bool ValidarIntegracaoEnvioNFSeManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, ref string mensagemErro)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPorLancamentoNFSManualETipoIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);

            if (nfsManualCTeIntegracao == null)
            {
                AdicionarNFSManualCTeParaIntegracao(nfsManualCTeIntegracao.LancamentoNFSManual, nfsManualCTeIntegracao.TipoIntegracao);
                nfsManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPorLancamentoNFSManualETipoIntegracao(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);
            }

            if (nfsManualCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
            {
                this.IntegrarNFSeManual(nfsManualCTeIntegracao);

                nfsManualCTeIntegracao = repNFSManualCTeIntegracao.BuscarPorCodigo(nfsManualCTeIntegracao.Codigo);

                if (nfsManualCTeIntegracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                {
                    mensagemErro = nfsManualCTeIntegracao.ProblemaIntegracao;
                    return false;
                }

            }

            if (lancamentoNFSManual.CTe.Status.Equals("Z"))
                mensagemErro = "Não é possível efetuar a integração do cancelamento do documento, pois o mesmo se encontra anulado gerancialmente";
            
            return true;
        }

        private bool ValidarIntegracaoEnvioCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaCTeIntegracao cargaCancelamentoCargaCTeIntegracao, ref string mensagemErro)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao cargaCTeIntegracao = repCargaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM).FirstOrDefault();

            if (cargaCTeIntegracao == null)
            {
                AdicionarCargaCTeParaIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe, cargaCancelamentoCargaCTeIntegracao.TipoIntegracao);
                cargaCTeIntegracao = repCargaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(cargaCancelamentoCargaCTeIntegracao.CargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM).FirstOrDefault();
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

            return true;
        }

        private bool ValidarIntegracaoEnvioCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao ocorrenciaCTeCancelamentoIntegracao, ref string mensagemErro)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao repositorioOcorrenciaCTeCancelamentoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeCancelamentoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao = repOcorrenciaCTeIntegracao.BuscarPorCargaCTeETipoIntegracao(ocorrenciaCTeCancelamentoIntegracao.OcorrenciaCTeIntegracao.CargaCTe.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM);

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

            return true;
        }

        private SituacaoIntegracao BuscarSituacaoIntegracaoCancelamentoCargaKMM(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);
            return repositorioCargaIntegracao.BuscarPorCargaETipoIntegracao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM)?.SituacaoIntegracao ?? SituacaoIntegracao.ProblemaIntegracao;
        }

        private Hashtable ConverterCancelamentoCargaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = null)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            if (cargaCTe.CTe == null)
            {
                throw new Exception("A Carga CTe não possui CTe vinculado");
            }

            var carga = cargaCTe.Carga;
            var cte = cargaCTe.CTe;

            Hashtable documentoHash = new Hashtable();
            documentoHash.Add("tipo", cte.ModeloDocumentoFiscal?.TipoDocumentoEmissao.ObterDescricao() ?? "");
            documentoHash.Add("protocolo", cte.Codigo.ToString());

            string xmlString = "";
            if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("CT-e"))
            {
                documentoHash.Add("chave", cte.Chave.ToString() ?? "");

                xmlString = new Servicos.WebService.CTe.CTe(_unitOfWork).ObterRetornoXML(cargaCTe.CTe, _unitOfWork);
            }
            else if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS-e") || cte.ModeloDocumentoFiscal.Abreviacao.Equals("NFS"))
            {
                documentoHash.Add("chave", cte.Codigo.ToString());

                xmlString = this.ObterXMLDocumento(cte, lancamentoNFSManual?.DadosNFS, true);
            }
            else if (cte.ModeloDocumentoFiscal.Abreviacao.Equals("ND"))
            {
                documentoHash.Add("chave", cte.Codigo.ToString());

                xmlString = this.ObterXMLDocumento(cte, null, true);
            }
            else
            {
                throw new Exception("Não é permitido integração pra esse modelo de documento.");
            }

            documentoHash.Add("xml", Regex.Replace(xmlString, @"^\s*<\?xml.*?\?>", "", RegexOptions.IgnoreCase));


            return documentoHash;
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

        private Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao AdicionarNFSManualCTeParaIntegracao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao nfsManualCTeIntegracao = new Dominio.Entidades.Embarcador.NFS.NFSManualCTeIntegracao();
            nfsManualCTeIntegracao.LancamentoNFSManual = lancamentoNFSManual;
            nfsManualCTeIntegracao.DataIntegracao = DateTime.Now;
            nfsManualCTeIntegracao.NumeroTentativas = 0;
            nfsManualCTeIntegracao.ProblemaIntegracao = "";
            nfsManualCTeIntegracao.TipoIntegracao = tipoIntegracao;
            nfsManualCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            repNFSManualCTeIntegracao.Inserir(nfsManualCTeIntegracao);

            return nfsManualCTeIntegracao;
        }
        #endregion Métodos Privados
    }
}