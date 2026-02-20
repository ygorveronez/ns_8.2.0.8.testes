using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Notificacao
{
    public sealed class NotificacaoMobile
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public NotificacaoMobile(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao ObterNotificacao(int codigoNotificacao)
        {
            Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorioNotificacaoUsuario = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(_unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario notificacaoUsuario = repositorioNotificacaoUsuario.BuscarPorCodigo(codigoNotificacao, auditavel: false) ?? throw new ServicoException("Notificação não encontrada");

            if (!notificacaoUsuario.DataLeitura.HasValue)
            {
                notificacaoUsuario.DataLeitura = DateTime.Now;

                repositorioNotificacaoUsuario.Atualizar(notificacaoUsuario);
            }

            return new Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.Notificacao()
            {
                Assunto = notificacaoUsuario.Notificacao.Assunto,
                CodigoCentroCarregamento = notificacaoUsuario.Notificacao.CentroCarregamento?.Codigo ?? 0,
                Data = notificacaoUsuario.Notificacao.Data.ToString("dd/MM/yyyy HH:mm"),
                DescricaoCentroCarregamento = notificacaoUsuario.Notificacao.CentroCarregamento?.Descricao ?? "",
                Mensagem = notificacaoUsuario.Notificacao.Mensagem
            };
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida> ObterNotificacoes(int codigoUsuario, bool somenteNaoLidas)
        {
            Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario repositorioNotificacaoUsuario = new Repositorio.Embarcador.Notificacoes.NotificacaoMobileUsuario(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Notificacoes.NotificacaoMobileUsuario> notificacoesUsuario = repositorioNotificacaoUsuario.ObterNotificacoesPorUsuario(codigoUsuario, somenteNaoLidas);

            return (
                from notificacaoUsuario in notificacoesUsuario
                select new Dominio.ObjetosDeValor.Embarcador.Mobile.Notificacao.NotificacaoResumida()
                {
                    Codigo = notificacaoUsuario.Codigo,
                    Assunto = notificacaoUsuario.Notificacao.Assunto,
                    Data = notificacaoUsuario.Notificacao.Data.ToString("dd/MM/yyyy HH:mm"),
                    Lida = notificacaoUsuario.DataLeitura.HasValue
                }
            ).ToList();
        }

        #endregion
    }
}
