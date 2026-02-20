using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras
{
    [CustomAuthorize("Financeiros/DocumentoEntrada")]
    public class QualificaoFornecedorController : BaseController
    {
		#region Construtores

		public QualificaoFornecedorController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.QualificaoFornecedor repQualificaoFornecedor = new Repositorio.Embarcador.Compras.QualificaoFornecedor(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor qualificacao = repQualificaoFornecedor.BuscarPorOrdemCompra(codigo);

                // Valida
                if (qualificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    qualificacao.Codigo,
                    DataQualificacao = qualificacao.DataQualificacao?.ToString("dd/MM/yyyy") ?? string.Empty,
                    qualificacao.CriterioPrazoEntregaPontualidade,
                    qualificacao.CriterioCaracteristicaEspecificacoes,
                    qualificacao.CriterioQuantidadeRecebida,
                    qualificacao.CriterioIntegridadeFisica,
                    qualificacao.CriterioAtendimento,
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Qualificar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Compras.QualificaoFornecedor repQualificaoFornecedor = new Repositorio.Embarcador.Compras.QualificaoFornecedor(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor qualificacao = repQualificaoFornecedor.BuscarPorOrdemCompra(codigo);

                // Valida
                if (qualificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                qualificacao.Initialize();

                // Preenche entidade com dados
                PreencheEntidade(ref qualificacao, unitOfWork);

                // Persiste dados
                unitOfWork.Start();
                repQualificaoFornecedor.Atualizar(qualificacao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Compras.QualificaoFornecedor qualificacao, Repositorio.UnitOfWork unitOfWork)
        {
            int.TryParse(Request.Params("CriterioPrazoEntregaPontualidade"), out int criterioPrazoEntregaPontualidade);
            int.TryParse(Request.Params("CriterioCaracteristicaEspecificacoes"), out int criterioCaracteristicaEspecificacoes);
            int.TryParse(Request.Params("CriterioQuantidadeRecebida"), out int criterioQuantidadeRecebida);
            int.TryParse(Request.Params("CriterioIntegridadeFisica"), out int criterioIntegridadeFisica);
            int.TryParse(Request.Params("CriterioAtendimento"), out int criterioAtendimento);


            // Vincula dados
            qualificacao.Usuario = this.Usuario;
            qualificacao.DataQualificacao = DateTime.Now;
            qualificacao.CriterioPrazoEntregaPontualidade = criterioPrazoEntregaPontualidade;
            qualificacao.CriterioCaracteristicaEspecificacoes = criterioCaracteristicaEspecificacoes;
            qualificacao.CriterioQuantidadeRecebida = criterioQuantidadeRecebida;
            qualificacao.CriterioIntegridadeFisica = criterioIntegridadeFisica;
            qualificacao.CriterioAtendimento = criterioAtendimento;
        }
        #endregion
    }
}
