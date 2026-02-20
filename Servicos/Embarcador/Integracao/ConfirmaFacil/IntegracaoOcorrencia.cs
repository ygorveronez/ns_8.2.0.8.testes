using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil;
using Dominio.Excecoes.Embarcador;
using System.Web;

namespace Servicos.Embarcador.Integracao.ConfirmaFacil
{
    public partial class IntegracaoConfirmaFacil
    {
        #region Métodos Globais

        public void IntegrarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil configConfirmaFacil = new Repositorio.Embarcador.Configuracoes.IntegracaoConfirmaFacil(unitOfWork).Buscar();
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repOcorrenciaIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);

            if (configConfirmaFacil == null || !configConfirmaFacil.PossuiIntegracao)
            {
                ocorrenciaCTeIntegracao.ProblemaIntegracao = "Falha ao buscar configuração da integração com a Confirma Fácil";
                ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
            }
            else
            {
                try
                {
                    object objetoEnvio = null;
                    //bool ocorrenciaEntrega = ocorrenciaCTeIntegracao.CargaOcorrencia.TipoOcorrencia.EntregaRealizada ? true : false;

                    objetoEnvio = GerarOcorrencia(ocorrenciaCTeIntegracao, unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.retornoWebService retWS = Transmitir(configConfirmaFacil, configConfirmaFacil.URL, objetoEnvio);

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

                    ocorrenciaCTeIntegracao.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Confirma Fácil";
                    ocorrenciaCTeIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
            }

            repOcorrenciaIntegracao.Atualizar(ocorrenciaCTeIntegracao);
        }

        #endregion

        #region Métodos Privados

        private static List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia> GerarOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao ocorrenciaCTeIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            List < Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia> retorno = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia>();

            if (ocorrenciaCTeIntegracao.CargaCTe?.CTe.XMLNotaFiscais.Count == 0)
                throw new ServicoException("Não possui nota fiscal para integrar!");

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNota in ocorrenciaCTeIntegracao.CargaCTe?.CTe.XMLNotaFiscais ?? new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>())
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia nota = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.nfeocorrencia();

                int codigoOcorrencia = 0;
                if (!string.IsNullOrEmpty(ocorrenciaCTeIntegracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao))
                    int.TryParse(ocorrenciaCTeIntegracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao, out codigoOcorrencia);

                var embarque = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Embarque();
                embarque.numero = XMLNota.Numero.ToString();
                embarque.serie = XMLNota.Serie.ToString();
                nota.embarque = embarque;

                var embarcador = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Embarcador();
                embarcador.cnpj = ocorrenciaCTeIntegracao.CargaCTe.CTe?.Remetente?.CPF_CNPJ_SemFormato ?? string.Empty;
                nota.embarcador = embarcador;

                var transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Transportadora();
                transportadora.cnpj = ocorrenciaCTeIntegracao.CargaCTe.Carga?.Empresa?.CNPJ_SemFormato ?? string.Empty;
                transportadora.nome = ocorrenciaCTeIntegracao.CargaCTe.Carga?.Empresa?.RazaoSocial ?? string.Empty;
                nota.transportadora = transportadora;

                var ocorrencia = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.Ocorrencia();
                ocorrencia.tipoEntrega = codigoOcorrencia.ToString();
                ocorrencia.dtOcorrencia = ocorrenciaCTeIntegracao?.CargaOcorrencia?.DataEvento?.ToString("dd-MM-yyyy") ?? "00-00-0000";
                ocorrencia.hrOcorrencia = ocorrenciaCTeIntegracao?.CargaOcorrencia?.DataEvento?.ToString("HH:mm:ss") ?? "00:00:00";

                //Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                //Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCPF(ocorrenciaCTeIntegracao.CargaCTe.Carga?.CPFPrimeiroMotorista);

                //if (motorista != null)
                //{
                //    List<string> fotos = new List<string>();
                //    string url = "https://" + HttpContext.Current.Request.Url.Host + "/Link/BuscarFoto/?";
                //    string key = Servicos.Criptografia.Criptografar(motorista.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3");
                //    fotos.Add(url + key);
                //    ocorrencia.fotos = fotos.ToArray() ?? null;
                //}

                nota.ocorrencia = ocorrencia;

                retorno.Add(nota);
            }

            return retorno;
        }

        #endregion
    }
}