using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao
{
    public sealed class ControleIntegracao
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;
        
        #endregion

        #region Construtores

        public ControleIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados 

        private void enviarEmailIndisponibilidadeWebService(List<string> tiposIntegracaoIndisponiveis, string emails)
        {
            string[] listaEmails = emails.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string assunto = "Web Service Indisponível";
            System.Text.StringBuilder mensagem = new System.Text.StringBuilder();

            mensagem.Append("Olá, ").AppendLine().AppendLine();

            if (tiposIntegracaoIndisponiveis.Count > 1)
                mensagem.Append($"Os seguintes Web Services estão indisponíveis:").AppendLine();
            else
                mensagem.Append($"O seguinte Web Service está indisponível:").AppendLine();

            mensagem.Append($"{string.Join(", ", tiposIntegracaoIndisponiveis) }.").AppendLine().AppendLine();
            mensagem.Append("E-mail enviado automaticamente. Por favor, não responda.");

            foreach (string email in listaEmails)
            {
                if (!Servicos.Email.EnviarEmailAutenticado(email.Trim(), assunto, mensagem.ToString(), _unitOfWork, out string mensagemErro, ""))
                {
                    Log.TratarErro($"Falha ao enviar e-mail de indisponibilidade de web service para {email.Trim()}: {mensagemErro}");
                }
            }
        }

        private bool IsRealizarTesteDisponibilidade(Dominio.Entidades.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade configuracaoTesteDisponibilidade)
        {
            if (!configuracaoTesteDisponibilidade.DataUltimoTeste.HasValue)
                return true;

            return (configuracaoTesteDisponibilidade.DataUltimoTeste.Value.AddMinutes(configuracaoTesteDisponibilidade.TempoAguardarExecutarTeste) < DateTime.Now);
        }

        private List<TipoIntegracao> ObterTiposIntegracao()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return repositorioTipoIntegracao.BuscarTipos();
        }

        #endregion

        #region Métodos Públicos

        public void TestarDisponibilidadeIntegracoes()
        {
            Repositorio.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade repositorio = new Repositorio.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade(_unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ConfiguracaoTesteDisponibilidade configuracaoTesteDisponibilidade = repositorio.ObterConfiguracao();

            if ((configuracaoTesteDisponibilidade != null) && IsRealizarTesteDisponibilidade(configuracaoTesteDisponibilidade))
            {
                List<TipoIntegracao> tiposIntegracao = ObterTiposIntegracao();
                List<string> tiposIntegracaoIndisponiveis = new List<string>();

                try
                {
                    IntegracaoBase integracaoSistema = new Sistema.IntegracaoSistema(_unitOfWork);
                    bool integracaoSistemaDisponivel = integracaoSistema.TestarDisponibilidade();

                    if (!integracaoSistemaDisponivel)
                        tiposIntegracaoIndisponiveis.Add("Multisoftware");

                    foreach (TipoIntegracao tipoIntegracao in tiposIntegracao)
                    {
                        IntegracaoBase integracao = IntegracaoBase.CriarIntegracaoTesteDisponibilidade(tipoIntegracao, _unitOfWork);
                        bool integracaoDisponivel = integracao?.TestarDisponibilidade() ?? true;

                        if (!integracaoDisponivel)
                            tiposIntegracaoIndisponiveis.Add(tipoIntegracao.ObterDescricao());
                    }

                    configuracaoTesteDisponibilidade.DataUltimoTeste = DateTime.Now;

                    repositorio.Atualizar(configuracaoTesteDisponibilidade);

                    if (tiposIntegracaoIndisponiveis.Count > 0)
                        enviarEmailIndisponibilidadeWebService(tiposIntegracaoIndisponiveis, configuracaoTesteDisponibilidade.Email);
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);
                }
            }
        }

        #endregion
    }
}
