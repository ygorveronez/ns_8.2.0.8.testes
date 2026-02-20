using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/TipoTerminalImportacao")]
    public class TipoTerminalImportacaoController : BaseController
    {
		#region Construtores

		public TipoTerminalImportacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                string descricao = Request.Params("Descricao");
                string porto = Request.Params("Porto");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoTerminalImportacao.Porto, "Porto", 35, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoPorto", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Empresa", false);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("Localidade", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> listaTipoTerminalImportacao = repTipoTerminalImportacao.Consultar(porto, descricao, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoTerminalImportacao.ContarConsulta(porto, descricao, ativo));

                var retorno = (from obj in listaTipoTerminalImportacao
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   Porto = obj.Porto?.Descricao ?? "",
                                   obj.DescricaoAtivo,
                                   CodigoPorto = obj.Porto?.Codigo ?? 0,
                                   CodigoEmpresa = obj.Empresa?.Codigo ?? 0,
                                   Empresa = obj.Empresa?.Descricao ?? "",
                                   CodigoLocalidade = obj.Porto?.Localidade?.Codigo ?? 0,
                                   Localidade = obj.Porto?.Localidade?.Descricao ?? ""
                               }).ToList();
                grid.AdicionaRows(retorno);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao tipoTerminalImportacao = new Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao();

                PreencherTipoTerminalImportacao(tipoTerminalImportacao, unidadeDeTrabalho);
                repTipoTerminalImportacao.Inserir(tipoTerminalImportacao, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao tipoTerminalImportacao = repTipoTerminalImportacao.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                PreencherTipoTerminalImportacao(tipoTerminalImportacao, unidadeDeTrabalho);
                repTipoTerminalImportacao.Atualizar(tipoTerminalImportacao, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao tipoTerminalImportacao = repTipoTerminalImportacao.BuscarPorCodigo(codigo);
                var retorno = new
                {
                    tipoTerminalImportacao.Ativo,
                    tipoTerminalImportacao.Codigo,
                    tipoTerminalImportacao.Descricao,
                    tipoTerminalImportacao.CodigoTerminal,
                    tipoTerminalImportacao.CodigoIntegracao,
                    tipoTerminalImportacao.CodigoDocumento,
                    tipoTerminalImportacao.CodigoMercante,
                    tipoTerminalImportacao.CodigoObservacaoContribuinte,
                    tipoTerminalImportacao.QuantidadeDiasEnvioDocumentacao,
                    Porto = tipoTerminalImportacao.Porto != null ? new { tipoTerminalImportacao.Porto.Codigo, tipoTerminalImportacao.Porto.Descricao } : null,
                    Terminal = tipoTerminalImportacao.Terminal != null ? new { tipoTerminalImportacao.Terminal.Codigo, tipoTerminalImportacao.Terminal.Descricao } : null,
                    Empresa = tipoTerminalImportacao.Empresa != null ? new { tipoTerminalImportacao.Empresa.Codigo, tipoTerminalImportacao.Empresa.Descricao } : null
                };

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao tipoTerminalImportacao = repTipoTerminalImportacao.BuscarPorCodigo(codigo);

                repTipoTerminalImportacao.Deletar(tipoTerminalImportacao, Auditado);

                unidadeDeTrabalho.Dispose();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Dispose();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);

                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
        }

        #endregion


        #region Métodos Privados

        private void PreencherTipoTerminalImportacao(Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao tipoTerminalImportacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);

            double.TryParse(Request.Params("Terminal"), out double terminal);
            int.TryParse(Request.Params("Porto"), out int porto);
            int.TryParse(Request.Params("Empresa"), out int empresa);

            tipoTerminalImportacao.Ativo = bool.Parse(Request.Params("Ativo"));
            tipoTerminalImportacao.Descricao = Request.Params("Descricao");
            tipoTerminalImportacao.Integrado = false;
            tipoTerminalImportacao.CodigoIntegracao = Request.Params("CodigoIntegracao");
            tipoTerminalImportacao.CodigoDocumento = Request.Params("CodigoDocumento");
            tipoTerminalImportacao.CodigoTerminal = Request.Params("CodigoTerminal");
            tipoTerminalImportacao.CodigoMercante = Request.Params("CodigoMercante");
            tipoTerminalImportacao.CodigoObservacaoContribuinte = Request.Params("CodigoObservacaoContribuinte");
            tipoTerminalImportacao.QuantidadeDiasEnvioDocumentacao = Request.GetIntParam("QuantidadeDiasEnvioDocumentacao");
            tipoTerminalImportacao.Terminal = terminal > 0 ? repCliente.BuscarPorCPFCNPJ(terminal) : null;
            tipoTerminalImportacao.Porto = porto > 0 ? repPorto.BuscarPorCodigo(porto) : null;
            tipoTerminalImportacao.Empresa = empresa > 0 ? repEmpresa.BuscarPorCodigo(empresa) : null;
        }

        #endregion
    }
}
