using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/ModalMensagemAviso")]
    public class ModalMensagemAvisoController : BaseController
    {
		#region Construtores

		public ModalMensagemAvisoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Buscar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.ModalMensagemAviso aviso = ObterMensagemTermoQuitacaoFinanceiro(unidadeTrabalho);

                if (aviso != null)
                    return new JsonpResult(aviso);
                else
                    return new JsonpResult(false);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter o modal de Mensagem de Aviso.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.ModalMensagemAviso ObterMensagemTermoQuitacaoFinanceiro(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador repAprovacaoTermo = new Repositorio.Embarcador.Financeiro.AprovacaoTermoQuitacaoFinanceiroTransportador(unitOfWork);

            if (repAprovacaoTermo.ExistePendenteMatrizTransportador(this.Empresa?.Codigo ?? 0))
            {
                return new Dominio.ObjetosDeValor.ModalMensagemAviso()
                {
                    Titulo = "Termo de Quitação Pendente",
                    Mensagem = @"<h3>A Matriz possui Termo de Quitação pendente de assinatura.</h3>
                                 <h3>Deve ser verificado, e prosseguir com a mesma.</h3>"
                };
            }

            return null;
        }

        #endregion Métodos Privados
    }
}
