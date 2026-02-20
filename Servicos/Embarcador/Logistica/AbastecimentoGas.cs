using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public class AbastecimentoGas
    {
        #region Atributos

        private Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public AbastecimentoGas(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Método Públicos

        public void EnviarEmailAbastecimentoGas()
        {
            Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas repositorioFilialSuprimento = new Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> filialSuprimentosLimiteSolicitacaoNotificar = ObterSuprimentosParaNotificarComLimiteSolicitacao();
            List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> filialSuprimentosLimiteGerenteNotificar = ObterSuprimentosParaNotificarComLimiteGerente();
            List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> filialSuprimentosBloqueioNotificar = ObterSuprimentosParaNotificarComBloqueio();

            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            (string Assunto, string Body) composicaoEmail;

            List<string> emails = new List<string>();

            foreach (Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas filialSuprimento in filialSuprimentosLimiteGerenteNotificar)
            {
                composicaoEmail = ObterEmailAbastecimentoGas(filialSuprimento.Cliente, filialSuprimento.SuprimentoDeGas.TipoCargaPadrao);
                emails = !string.IsNullOrWhiteSpace(filialSuprimento.SuprimentoDeGas.NotificarPorEmailGerente) ? filialSuprimento.SuprimentoDeGas.NotificarPorEmailGerente.Split(';').ToList() : new List<string>();

                if (emails.Count > 0)
                {
                    servicoEmail.EnviarEmail(null, null, null, null, null, null, composicaoEmail.Assunto, composicaoEmail.Body, null, null, null, false, "", 587, _unitOfWork, 0, true, emails);
                    filialSuprimento.SuprimentoDeGas.DataUltimaNotificacaoGerente = DateTime.Now.Date;
                    repositorioFilialSuprimento.Atualizar(filialSuprimento);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas filialSuprimento in filialSuprimentosLimiteSolicitacaoNotificar)
            {
                composicaoEmail = ObterEmailAbastecimentoGas(filialSuprimento.Cliente, filialSuprimento.SuprimentoDeGas.TipoCargaPadrao);
                emails = !string.IsNullOrWhiteSpace(filialSuprimento.SuprimentoDeGas.NotificarPorEmailLimite) ? filialSuprimento.SuprimentoDeGas.NotificarPorEmailLimite.Split(';').ToList() : new List<string>();

                if (emails.Count > 0)
                {
                    servicoEmail.EnviarEmail(null, null, null, null, null, null, composicaoEmail.Assunto, composicaoEmail.Body, null, null, null, false, "", 587, _unitOfWork, 0, true, emails);
                    filialSuprimento.SuprimentoDeGas.DataUltimaNotificacaoLimite = DateTime.Now.Date;
                    repositorioFilialSuprimento.Atualizar(filialSuprimento);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas filialSuprimento in filialSuprimentosBloqueioNotificar)
            {
                composicaoEmail = ObterEmailAbastecimentoGas(filialSuprimento.Cliente, filialSuprimento.SuprimentoDeGas.TipoCargaPadrao);
                emails = !string.IsNullOrWhiteSpace(filialSuprimento.SuprimentoDeGas.NotificarPorEmailBloqueio) ? filialSuprimento.SuprimentoDeGas.NotificarPorEmailBloqueio.Split(';').ToList() : new List<string>();

                if (emails.Count > 0)
                {
                    servicoEmail.EnviarEmail(null, null, null, null, null, null, composicaoEmail.Assunto, composicaoEmail.Body, null, null, null, false, "", 587, _unitOfWork, 0, true, emails);
                    filialSuprimento.SuprimentoDeGas.DataUltimaNotificacaoBloqueio = DateTime.Now.Date;
                    repositorioFilialSuprimento.Atualizar(filialSuprimento);
                }
            }
        }

        #endregion

        #region Método Privados

        private (string Assunto, string Body) ObterEmailAbastecimentoGas(Dominio.Entidades.Cliente cliente, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga)
        {
            (string Assunto, string Body) composicao;
            composicao.Assunto = $"Solicitação de Abastecimento com horário diário excedido para o cliente {cliente?.Descricao}";
            composicao.Body = $"O cliente {cliente?.Descricao} não recebeu nenhuma solicitação de abastecimento de gás no dia atual ({DateTime.Now.ToString("dd/MM/yyyy")}) para o tipo de carga {tipoCarga?.Descricao}.";

            return composicao;
        }

        private List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> ObterSuprimentosParaNotificarComLimiteGerente()
        {
            Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas repositorio = new Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas(_unitOfWork);

            return repositorio.BuscarPorHoraLimiteSolicitacaoGerenteEsgotada();
        }

        private List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> ObterSuprimentosParaNotificarComLimiteSolicitacao()
        {
            Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas repositorio = new Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas(_unitOfWork);

            return repositorio.BuscarPorHoraLimiteSolicitacaoEsgotada();
        }

        private List<Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas> ObterSuprimentosParaNotificarComBloqueio()
        {
            Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas repositorio = new Repositorio.Embarcador.Filiais.FilialSuprimentoDeGas(_unitOfWork);

            return repositorio.BuscarPorHoraLimiteSolicitacaoBloqueioEsgotada();
        }

        #endregion
    }
}
