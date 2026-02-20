using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Integracao
{
    public class IntegradoraIntegracaoRetorno
    {
        #region Métodos Públicos

        public static void InformarIntegracao(Dominio.Entidades.WebService.Integradora integradora, bool sucesso, string mensagem, string identificador, string request, string response, string tipoArquvo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = SituacaoIntegracao.Integrado, string Hash = "")
        {
            if (integradora != null)
            {
                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integradoraIntegracaoRetorno = new Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno
                {
                    Integradora = integradora,
                    Sucesso = sucesso,
                    Mensagem = mensagem,
                    NumeroIdentificacao = identificador,
                    Data = DateTime.Now,
                    ArquivoRequisicao = !string.IsNullOrWhiteSpace(request) ? ArquivoIntegracao.SalvarArquivoIntegracao(request, tipoArquvo, unitOfWork) : null,
                    ArquivoResposta = !string.IsNullOrWhiteSpace(response) ? ArquivoIntegracao.SalvarArquivoIntegracao(response, tipoArquvo, unitOfWork) : null,
                    Carga = carga,
                    Situacao = situacao,
                    HashJson = Hash,
                    Tentativas = 0
                };

                repIntegradoraIntegracaoRetorno.Inserir(integradoraIntegracaoRetorno);
            }

            if (!sucesso)
                NotificarPorEmail(carga, integradora?.Descricao ?? string.Empty, identificador, mensagem, unitOfWork);
        }

        public static void AtualizarIntegracao(Dominio.Entidades.WebService.IntegradoraIntegracaoRetorno integradoraIntegracaoRetorno, bool sucesso, string mensagem, string identificador, string response, string tipoArquvo, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga = null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao = SituacaoIntegracao.Integrado)
        {
            if (integradoraIntegracaoRetorno != null)
            {
                Repositorio.WebService.IntegradoraIntegracaoRetorno repIntegradoraIntegracaoRetorno = new Repositorio.WebService.IntegradoraIntegracaoRetorno(unitOfWork);

                integradoraIntegracaoRetorno.Sucesso = sucesso;
                integradoraIntegracaoRetorno.Mensagem = mensagem;
                integradoraIntegracaoRetorno.NumeroIdentificacao = identificador;
                integradoraIntegracaoRetorno.Data = DateTime.Now;
                integradoraIntegracaoRetorno.ArquivoResposta = !string.IsNullOrWhiteSpace(response) ? ArquivoIntegracao.SalvarArquivoIntegracao(response, tipoArquvo, unitOfWork) : null;
                integradoraIntegracaoRetorno.Carga = carga;
                integradoraIntegracaoRetorno.Situacao = situacao;

                repIntegradoraIntegracaoRetorno.Atualizar(integradoraIntegracaoRetorno);
            }

            if (!sucesso)
                NotificarPorEmail(carga, integradoraIntegracaoRetorno.Integradora?.Descricao ?? string.Empty, identificador, mensagem, unitOfWork);
        }



        private static void NotificarPorEmail(Dominio.Entidades.Embarcador.Cargas.Carga carga, string integradora, string identificador, string mensagem, Repositorio.UnitOfWork unidadeTrabalho)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(carga?.Filial?.Email ?? string.Empty))
                {
                    List<string> listaEmails = new List<string>();
                    listaEmails.AddRange(carga.Filial?.Email.Split(';').ToList());
                    listaEmails = listaEmails.Distinct().ToList();
                    if (listaEmails.Count > 0)
                    {
                        Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
                        Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
                        if (configEmail != null)
                        {
                            string mensagemErro = string.Empty;

                            string assunto = "Rejeição integração " + integradora;
                            if (carga != null)
                                assunto += " carga " + carga.CodigoCargaEmbarcador;
                            else
                                assunto += " identificador " + identificador;

                            string mensagemEmail = assunto;
                            mensagemEmail += "<br/>";
                            mensagemEmail += "<br/>";
                            mensagemEmail += "Integração " + integradora + " retornou: ";
                            mensagemEmail += "<br/>";
                            mensagemEmail += mensagem;
                            mensagemEmail += "<br/>";
                            mensagemEmail += "<br/>";
                            mensagemEmail += "<br/>";
                            mensagemEmail += " *** E-mail automático, favor não responder ***";

                            bool sucesso = Servicos.Email.EnviarEmail(configEmail.Email, configEmail.Email, configEmail.Senha, null, listaEmails.ToArray(), null, assunto, mensagemEmail, configEmail.Smtp, out mensagemErro, configEmail.DisplayEmail, null, "", configEmail.RequerAutenticacaoSmtp, "", configEmail.PortaSmtp, unidadeTrabalho);

                            if (!sucesso)
                                throw new Exception("Email de notificação " + integradora + " não enviado: " + mensagemErro);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        #endregion

    }
}
