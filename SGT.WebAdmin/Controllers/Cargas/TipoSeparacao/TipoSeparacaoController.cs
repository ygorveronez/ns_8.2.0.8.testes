using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.TipoSeparacao
{
    [CustomAuthorize("Cargas/TipoSeparacao")]
    public class TipoSeparacaoController : BaseController
    {
		#region Construtores

		public TipoSeparacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoSeparacao repTipoSeparacao = new Repositorio.Embarcador.Cargas.TipoSeparacao(unitOfWork);
                string descricao = Request.Params("Descricao");
                string codigoTipoCargaEmbarcador = Request.Params("CodigoTipoSeparacaoEmbarcador");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoSeparacao.CodigoDeIntegracao, "CodigoTipoSeparacaoEmbarcador", 20, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Cargas.TipoSeparacao> tipoSeparacao = repTipoSeparacao.Consultar(descricao, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, codigoTipoCargaEmbarcador);
                grid.setarQuantidadeTotal(repTipoSeparacao.ContarConsulta(descricao, ativo, codigoTipoCargaEmbarcador));

                var retorno = (from obj in tipoSeparacao
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.CodigoTipoSeparacaoEmbarcador,
                                   obj.PadraoMontagemCarregamentoAuto,
                                   obj.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            bool novo = true;
            try
            {
                Repositorio.Embarcador.Cargas.TipoSeparacao repTipo = new Repositorio.Embarcador.Cargas.TipoSeparacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipo = new Dominio.Entidades.Embarcador.Cargas.TipoSeparacao();
                tipo.Ativo = bool.Parse(Request.Params("Ativo"));
                tipo.Descricao = Request.Params("Descricao");
                tipo.CodigoTipoSeparacaoEmbarcador = Request.Params("CodigoTipoSeparacaoEmbarcador");
                tipo.PadraoMontagemCarregamentoAuto = bool.Parse(Request.Params("PadraoMontagemCarregamentoAuto"));

                if (!string.IsNullOrEmpty(tipo.CodigoTipoSeparacaoEmbarcador))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoSeparacao existeTipo = repTipo.BuscarPorCodigoEmbarcador(tipo.CodigoTipoSeparacaoEmbarcador);
                    if (existeTipo != null)
                        novo = false;
                }
                else
                    tipo.CodigoTipoSeparacaoEmbarcador = Guid.NewGuid().ToString().Replace("-", "");

                if (tipo.PadraoMontagemCarregamentoAuto)
                    if (repTipo.BuscarPadrao() != null)
                        return new JsonpResult(false, true, "Já existe um tipo de separação padrão cadastrado.");

                if (novo)
                {
                    unitOfWork.Start();
                    repTipo.Inserir(tipo, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, true, "Já existe um tipo de separação cadastrado para o código informado.");
            }
            catch (Exception ex)
            {
                if (novo)
                    unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.TipoSeparacao repTipo = new Repositorio.Embarcador.Cargas.TipoSeparacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipo = repTipo.BuscarPorCodigo(codigo, true);

                tipo.Ativo = bool.Parse(Request.Params("Ativo"));
                tipo.Descricao = Request.Params("Descricao");
                tipo.CodigoTipoSeparacaoEmbarcador = Request.Params("CodigoTipoSeparacaoEmbarcador");
                tipo.PadraoMontagemCarregamentoAuto = bool.Parse(Request.Params("PadraoMontagemCarregamentoAuto"));

                bool atualizar = true;
                string compl = "cadastrado para o código informado.";

                if (!string.IsNullOrEmpty(tipo.CodigoTipoSeparacaoEmbarcador))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoSeparacao existeTipo = repTipo.BuscarPorCodigoEmbarcador(tipo.CodigoTipoSeparacaoEmbarcador);
                    if (existeTipo != null)
                        if (existeTipo.Codigo != tipo.Codigo)
                            atualizar = false;
                }

                if (tipo.PadraoMontagemCarregamentoAuto)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoSeparacao existeTipo = repTipo.BuscarPadrao();
                    if (existeTipo != null)
                    {
                        if (existeTipo.Codigo != tipo.Codigo)
                        {
                            atualizar = false;
                            compl = "padrão cadastrado.";
                        }
                    }
                }

                if (atualizar)
                {
                    Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = repTipo.Atualizar(tipo, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe um tipo de separação " + compl);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.TipoSeparacao repTipo = new Repositorio.Embarcador.Cargas.TipoSeparacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipo = repTipo.BuscarPorCodigo(codigo, false);

                var dynTipo = new
                {
                    tipo.Codigo,
                    tipo.Descricao,
                    CodigoTipoSeparacaoEmbarcador = tipo.CodigoTipoSeparacaoEmbarcador != null ? tipo.CodigoTipoSeparacaoEmbarcador : "",
                    tipo.PadraoMontagemCarregamentoAuto,
                    tipo.Ativo
                };

                return new JsonpResult(dynTipo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
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
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Cargas.TipoSeparacao repTipo = new Repositorio.Embarcador.Cargas.TipoSeparacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoSeparacao tipo = repTipo.BuscarPorCodigo(codigo, false);
                repTipo.Deletar(tipo, Auditado);
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

    }
}
