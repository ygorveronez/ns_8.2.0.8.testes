using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Configuracoes
{
    [CustomAuthorize("Configuracoes/ArquivoImportacaoNotaFiscal")]
    public class ArquivoImportacaoNotaFiscalController : BaseController
    {
		#region Construtores

		public ArquivoImportacaoNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 90, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unitOfWork);

                List<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal> listaArquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.Consultar(descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repArquivoImportacaoNotaFiscal.ContarConsulta(descricao, ativo));

                grid.AdicionaRows((from p in listaArquivoImportacaoNotaFiscal
                                   select new
                                   {
                                       p.Codigo,
                                       p.Descricao
                                   }).ToList());

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

            try
            {
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                string descricao = Request.Params("Descricao");

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacaoNotaFiscal = new Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal();

                arquivoImportacaoNotaFiscal.Descricao = descricao;
                arquivoImportacaoNotaFiscal.Ativo = ativo;

                repArquivoImportacaoNotaFiscal.Inserir(arquivoImportacaoNotaFiscal, Auditado);

                SalvarListaColuna(arquivoImportacaoNotaFiscal, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                string descricao = Request.Params("Descricao");

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.BuscarPorCodigo(codigo, true);

                arquivoImportacaoNotaFiscal.Descricao = descricao;
                arquivoImportacaoNotaFiscal.Ativo = ativo;

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repArquivoImportacaoNotaFiscal.Atualizar(arquivoImportacaoNotaFiscal, Auditado);

                SalvarListaColuna(arquivoImportacaoNotaFiscal, unitOfWork, historico);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.BuscarPorCodigo(codigo);

                var retorno = new
                {
                    arquivoImportacaoNotaFiscal.Codigo,
                    arquivoImportacaoNotaFiscal.Ativo,
                    arquivoImportacaoNotaFiscal.Descricao,
                    ListaColunas = (from obj in arquivoImportacaoNotaFiscal.Campos
                                    orderby obj.Posicao
                                    select new
                                    {
                                        Codigo = obj.Codigo,
                                        obj.Propriedade,
                                        obj.TipoPropriedade,
                                        obj.Posicao
                                    }).ToList()
                };

                return new JsonpResult(retorno);
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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal repArquivoImportacaoNotaFiscal = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacaoNotaFiscal = repArquivoImportacaoNotaFiscal.BuscarPorCodigo(codigo);

                repArquivoImportacaoNotaFiscal.Deletar(arquivoImportacaoNotaFiscal, Auditado);

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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarListaColuna(Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscal arquivoImportacaoNotaFiscal, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Auditoria.HistoricoObjeto historico = null)
        {
            Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo repArquivoImportacaoNotaFiscalCampo = new Repositorio.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo> listaCampo = repArquivoImportacaoNotaFiscalCampo.BuscarPorArquivo(arquivoImportacaoNotaFiscal.Codigo);
            for (int i = 0; i < listaCampo.Count(); i++)
                repArquivoImportacaoNotaFiscalCampo.Deletar(listaCampo[i], Auditado, historico);

            dynamic listaColuna = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaColunas"));

            foreach (var coluna in listaColuna)
            {
                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo campo = new Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo();
                campo.Arquivo = arquivoImportacaoNotaFiscal;
                campo.TipoPropriedade = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo)int.Parse((string)coluna.TipoPropriedade);
                campo.Posicao = (int)coluna.Posicao;
                campo.Propriedade = (string)coluna.Propriedade;

                repArquivoImportacaoNotaFiscalCampo.Inserir(campo, Auditado, historico);
            }
        }

        #endregion
    }
}
