using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/TipoDetalhe")]
    public class TipoDetalheController : BaseController
    {
		#region Construtores

		public TipoDetalheController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

                string codigo = Request.Params("Codigo");
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoEmbarcador");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe tipoTipo;
                Enum.TryParse(Request.Params("Tipo"), out tipoTipo);

                bool exibirTipoPaleteCliente = Request.GetBoolParam("ExibirTipoPaleteCliente");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Detalhe", "Tipo", 20, Models.Grid.Align.left, true);
                if (exibirTipoPaleteCliente)
                {
                    grid.AdicionarCabecalho("Tipo Palete Cliente", "TipoPaleteCliente", 20, Models.Grid.Align.left);
                    grid.AdicionarCabecalho("Peso", "Valor", 10, Models.Grid.Align.left, false);
                }
                else
                {
                    grid.AdicionarCabecalho("TipoPaleteCliente", false);
                    grid.AdicionarCabecalho("Valor", false);
                }

                List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> listaTipoDetalhe = repositorioTipoDetalhe.Pesquisa(codigoIntegracao, descricao, tipoTipo);
                grid.setarQuantidadeTotal(listaTipoDetalhe.Count);
                var lista = (from p in listaTipoDetalhe
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalheHelper.ObterDescricao(p.Tipo),
                                 TipoPaleteCliente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPaleteClienteHelper.ObterDescricao(p?.TipoPaleteCliente ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente.NaoDefinido),
                                 Valor = p?.Valor ?? 0
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigo(codigo, false);

                // Valida
                if (tipoDetalhe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    tipoDetalhe.Codigo,
                    tipoDetalhe.CodigoIntegracao,
                    tipoDetalhe.Descricao,
                    tipoDetalhe.Tipo,
                    TipoPaleteCliente = tipoDetalhe.TipoPaleteCliente ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente.NaoDefinido,
                    Valor = tipoDetalhe?.Valor ?? 0
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = new Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe();

                // Preenche entidade com dados
                PreencheEntidade(ref tipoDetalhe, unitOfWork);

                if (!string.IsNullOrWhiteSpace(tipoDetalhe.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalheExistente = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(tipoDetalhe.CodigoIntegracao, tipoDetalhe.Tipo);
                    if (tipoDetalheExistente != null)
                        return new JsonpResult(false, true, "Já existe um tipo de detalhe com o código de integração " + tipoDetalhe.CodigoIntegracao + ".");
                }

                // Valida entidade
                string erro;
                if (!ValidaEntidade(tipoDetalhe, out erro, repositorioTipoDetalhe))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repositorioTipoDetalhe.Inserir(tipoDetalhe, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigo(codigo, true);

                // Valida
                if (tipoDetalhe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref tipoDetalhe, unitOfWork);
                if (!string.IsNullOrWhiteSpace(tipoDetalhe.CodigoIntegracao))
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalheExistente = repositorioTipoDetalhe.BuscarPorCodigoIntegracao(tipoDetalhe.CodigoIntegracao, tipoDetalhe.Tipo);
                    if (tipoDetalheExistente != null && tipoDetalheExistente.Codigo != tipoDetalhe.Codigo)
                        return new JsonpResult(false, true, "Já existe um Tipo de Detalhe cadastrado com o código de integração " + tipoDetalhe.CodigoIntegracao + ".");

                }
                // Valida entidade
                string erro;
                if (!ValidaEntidade(tipoDetalhe, out erro, repositorioTipoDetalhe))
                    return new JsonpResult(false, true, erro);

                // Inicia transacao
                unitOfWork.Start();

                // Persiste dados
                repositorioTipoDetalhe.Atualizar(tipoDetalhe, Auditado);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe = new Repositorio.Embarcador.Pedidos.TipoDetalhe(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe = repositorioTipoDetalhe.BuscarPorCodigo(codigo, true);

                // Valida
                if (tipoDetalhe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repositorioTipoDetalhe.Deletar(tipoDetalhe, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */
            tipoDetalhe.Codigo = Request.GetIntParam("Codigo");
            tipoDetalhe.Descricao = Request.GetStringParam("Descricao");
            tipoDetalhe.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoDetalhe.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe>("Tipo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe.Todos);
            tipoDetalhe.TipoPaleteCliente = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente>("TipoPaleteCliente", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente.NaoDefinido);
            tipoDetalhe.Valor = Request.GetDecimalParam("Valor");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe tipoDetalhe, out string msgErro, Repositorio.Embarcador.Pedidos.TipoDetalhe repositorioTipoDetalhe)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (string.IsNullOrWhiteSpace(tipoDetalhe.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (tipoDetalhe.Descricao.Length > 80)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(tipoDetalhe.CodigoIntegracao))
            {
                msgErro = "Código de integração é obrigatória.";
                return false;
            }

            if (tipoDetalhe.CodigoIntegracao.Length > 50)
            {
                msgErro = "Código de integração não pode passar de 50 caracteres.";
                return false;
            }

            if (tipoDetalhe.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe.TipoPallet)
            {
                if (tipoDetalhe.TipoPaleteCliente == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPaleteCliente.NaoDefinido)
                {
                    msgErro = "Para o Tipo de Detalhe Palete, é obrigatório informar o Tipo Palete Cliente.";
                    return false;
                }
                List<Dominio.Entidades.Embarcador.Pedidos.TipoDetalhe> outros = repositorioTipoDetalhe.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTipoDetalhe.TipoPallet);
                if (outros.Any(x => x.TipoPaleteCliente == tipoDetalhe.TipoPaleteCliente && x.Codigo != tipoDetalhe.Codigo))
                {
                    msgErro = "É permitido apenas 1 Tipo de Detalhe para cada Tipo Palete Cliente.";
                    return false;
                }
            }
            else
            {
                tipoDetalhe.TipoPaleteCliente = null;
                tipoDetalhe.Valor = null;
            }

            return true;
        }


        #endregion
    }
}
