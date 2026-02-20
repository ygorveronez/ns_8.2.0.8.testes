using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec;

namespace Servicos.Embarcador.Integracao.Runtec
{
    public partial class IntegracaoRuntec
    {
        #region Métodos Globais

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec configRuntec = new Repositorio.Embarcador.Configuracoes.IntegracaoRuntec(unitOfWork).Buscar();
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);

            if (configRuntec == null || !configRuntec.PossuiIntegracao)
            {
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Falha ao buscar configuração da integração com a Runtec";
                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                try
                {
                    object objetoEnvio = null;
                    bool ocorrenciaEntrega = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.EntregaRealizada ? true : false;
                    string urlWebService = configRuntec.URL;
                    if (urlWebService.EndsWith("/"))
                        urlWebService += (ocorrenciaEntrega ? "nfe/entrega" : "nfe/ocorrencia");
                    else
                        urlWebService += (ocorrenciaEntrega ? "/nfe/entrega" : "/nfe/ocorrencia");

                    if (ocorrenciaEntrega)
                        objetoEnvio = GerarEntrega(ocorrenciaCTeIntegracao, unitOfWork);
                    else
                        objetoEnvio = GerarOcorrencia(ocorrenciaCTeIntegracao, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.retornoWebService retWS = Transmitir(configRuntec, urlWebService, objetoEnvio);

                    if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    {
                        ocorrenciaCTeIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        ocorrenciaCTeIntegracao.PendenteRetorno = true;
                        ocorrenciaCTeIntegracao.Protocolo = "";
                    }
                    else
                    {
                        ocorrenciaCTeIntegracao.ProblemaIntegracao = retWS.ProblemaIntegracao;
                        ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo();

                    ocorrenciaCTeIntegracao.NumeroTentativas += 1;
                    ocorrenciaCTeIntegracao.DataIntegracao = DateTime.Now;

                    arquivoIntegracao.Data = ocorrenciaCTeIntegracao.DataIntegracao;
                    arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
                    arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retWS.jsonRequisicao, "json", unitOfWork);
                    arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retWS.jsonRetorno, "json", unitOfWork);
                    arquivoIntegracao.Mensagem = retWS.ProblemaIntegracao;

                    repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);
                    ocorrenciaCTeIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);

                    ocorrenciaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Runtec";
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        #endregion

        #region Métodos Privados

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeocorrencia GerarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeocorrencia retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeocorrencia();

            retorno.embarcador = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty;

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlnotasfiscais = null;
            if (ocorrenciaCTeIntegracao.CargaOcorrencia.UtilizarSelecaoPorNotasFiscaisCTe)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = repCargaOcorrenciaDocumento.BuscarPorCargaCTeEOcorrencia(ocorrenciaCTeIntegracao.CargaCTe.Codigo, ocorrenciaCTeIntegracao.CargaOcorrencia.Codigo);

                xmlnotasfiscais = cargaOcorrenciaDocumento.XMLNotaFiscais?.ToList();
            }
            else
            {
                xmlnotasfiscais = ocorrenciaCTeIntegracao.CargaCTe?.CTe?.XMLNotaFiscais?.ToList();
            }

            if (xmlnotasfiscais != null)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in xmlnotasfiscais)
                {
                    nfeocorrencianota nota = new nfeocorrencianota();

                    int codigoOcorrencia = 0;
                    if (!string.IsNullOrEmpty(ocorrenciaCTeIntegracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao))
                        int.TryParse(ocorrenciaCTeIntegracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao, out codigoOcorrencia);

                    nota.motivo_ocorrencia = codigoOcorrencia;
                    nota.data_problema = ocorrenciaCTeIntegracao?.CargaOcorrencia?.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss") ?? "0000-00-00T00:00:00";
                    nota.motorista = ocorrenciaCTeIntegracao?.CargaOcorrencia?.Carga?.Motoristas?.FirstOrDefault().CPF ?? string.Empty;
                    nota.veiculo = ocorrenciaCTeIntegracao?.CargaOcorrencia?.Carga?.Veiculo?.Placa ?? string.Empty;
                    nota.placa = ocorrenciaCTeIntegracao?.CargaOcorrencia?.Carga?.Veiculo?.Placa ?? string.Empty;
                    nota.data_chegada = null;
                    nota.chave = XMLNota.Chave;
                    nota.transportadora = ocorrenciaCTeIntegracao.CargaCTe.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty;
                    nota.latitude = Convert.ToDecimal(XMLNota?.Destinatario?.Latitude ?? "0.0");
                    nota.longitude = Convert.ToDecimal(XMLNota?.Destinatario?.Longitude ?? "0.0");

                    retorno.notas.Add(nota);
                }
            }

            return retorno;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeentrega GerarEntrega(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeentrega retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.nfeentrega();

            retorno.embarcador = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in ocorrenciaCTeIntegracao.CargaCTe?.CTe.XMLNotaFiscais ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>())
            {
                nfeentreganota nota = new nfeentreganota();

                nota.data_entrega = ocorrenciaCTeIntegracao?.CargaOcorrencia?.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss") ?? "0000-00-00T00:00:00";
                nota.recebedor_nome = "0";
                nota.recebedor_rg = "0";
                nota.chave = XMLNota.Chave;
                nota.transportadora = ocorrenciaCTeIntegracao.CargaCTe.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty;
                nota.latitude = Convert.ToDecimal(XMLNota?.Destinatario?.Latitude ?? "0.0");
                nota.longitude = Convert.ToDecimal(XMLNota?.Destinatario?.Longitude ?? "0.0");

                retorno.notas.Add(nota);
            }

            return retorno;
        }

        #endregion
    }
}
