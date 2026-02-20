using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/LocalArmazenamentoHistorico")]
    public class LocalArmazenamentoHistoricoController : BaseController
    {
        #region Construtores

        public LocalArmazenamentoHistoricoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSaldoTanqueHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoLocalArmazenamento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Produtos.LocalArmazenamentoHistorico repLocalArmazenamentoHistorico = new Repositorio.Embarcador.Produtos.LocalArmazenamentoHistorico(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoHistorico localArmazenamentoHistorico = repLocalArmazenamentoHistorico.BuscarUltimoHistoricoPorLocalArmazenamento(codigoLocalArmazenamento);

                if (localArmazenamentoHistorico == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o saldo para o combustível.");


                var dynLocalArmazenamentoHistorico = new
                {
                    localArmazenamentoHistorico.Codigo,
                    localArmazenamentoHistorico.Data,
                    localArmazenamentoHistorico.SaldoAnterior,
                    localArmazenamentoHistorico.SaldoAtual

                };

                return new JsonpResult(dynLocalArmazenamentoHistorico, true,"Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o saldo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        #endregion

        #region Métodos Privados
       
        #endregion
    }
}