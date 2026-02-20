using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/AlteracaoFormaPagamento")]
    public class AlteracaoFormaPagamentoController : BaseController
    {
		#region Construtores

		public AlteracaoFormaPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarFormaPagamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            try
            {
                int.TryParse(Request.Params("TipoPagamentoRecebimento"), out int codigoTipoPagamento);
                List<int> codigosGrupoPessoa = Request.GetListParam<int>("ListaGrupoPessoa");
                
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(codigoTipoPagamento);
                var gruposPessoas = repGrupoPessoa.BuscarPorCodigos(codigosGrupoPessoa);

                if (tipoPagamentoRecebimento == null)
                {
                    return new JsonpResult(false, "Forma de pagamento não enviada.");
                }

                if(gruposPessoas.Count == 0)
                {
                    return new JsonpResult(false, "Nenhum grupo selecionado.");
                }

                foreach(var grupo in gruposPessoas)
                {
                    grupo.Initialize();
                    grupo.FormaPagamento = tipoPagamentoRecebimento;
                    repGrupoPessoa.Atualizar(grupo, Auditado);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a forma de pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
