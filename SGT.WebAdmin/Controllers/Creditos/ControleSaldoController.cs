using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Creditos
{
    [CustomAuthorize("Creditos/ControleSaldo", "SAC/AtendimentoCliente")]
    public class ControleSaldoController : BaseController
    {
		#region Construtores

		public ControleSaldoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarInformacoesSaldoCredito()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if(this.Usuario != null)
                {
                    Servicos.Embarcador.Credito.ControleSaldo serControleSaldo = new Servicos.Embarcador.Credito.ControleSaldo(unitOfWork);
                    return new JsonpResult(serControleSaldo.BuscarInformacoesSaldoCredito(this.Usuario, unitOfWork));
                }
                else
                {
                    return new JsonpResult(false, true, "Seu tempo de sessão expirou, por favor, faça novamente seu login.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o Saldo disponível.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSuperiores()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito repHierarquiaSolicitacaoCredito  = new Repositorio.Embarcador.Creditos.HierarquiaSolicitacaoCredito(unitOfWork);

                List<Dominio.Entidades.Embarcador.Creditos.HierarquiaSolicitacaoCredito> hierarquiasSolicitacaoCredito = repHierarquiaSolicitacaoCredito.BuscarPorRecebedor(this.Usuario.Codigo);

                dynamic dynCreditores = (from obj in hierarquiasSolicitacaoCredito
                                        select new
                                        {
                                            obj.Creditor.Codigo,
                                            obj.Creditor.Nome
                                        }).ToList();

                return new JsonpResult(dynCreditores);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar hierarquia.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
