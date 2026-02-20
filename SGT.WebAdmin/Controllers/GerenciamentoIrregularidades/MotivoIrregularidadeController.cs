using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciamentoIrregularidades
{
    [CustomAuthorize("GerenciamentoIrregularidades/MotivosIrregularidades")]
    public class MotivoIrregularidadeController : BaseController
    {
		#region Construtores

		public MotivoIrregularidadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);
                int totalRegistro = repositorioMotivoIrregularidade.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade> motivosIrregularidades =
                    (totalRegistro > 0) ?
                    repositorioMotivoIrregularidade.Consultar(filtrosPesquisa, parametrosConsulta) :
                    new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>();

                var motivosIrregularidadesRetornar = (
                    from motivoIrregularidade in motivosIrregularidades
                    select new
                    {
                        Codigo = motivoIrregularidade.Codigo,
                        Descricao = motivoIrregularidade.Descricao,
                        Observacao = motivoIrregularidade.Observacao,
                        Situacao = motivoIrregularidade.Ativa ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao(),
                        Irregularidade = motivoIrregularidade.Irregularidade.Descricao,
                    }
                ).ToList();

                grid.AdicionaRows(motivosIrregularidadesRetornar);
                grid.setarQuantidadeTotal(totalRegistro);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade = repositorioMotivoIrregularidade.BuscarPorCodigo(codigo);

                if (MotivoIrregularidade == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    MotivoIrregularidade.Codigo,
                    MotivoIrregularidade.Descricao,
                    MotivoIrregularidade.Observacao,
                    MotivoIrregularidade.TipoMotivo,
                    Situacao = MotivoIrregularidade.Ativa,
                    Irregularidade = new { MotivoIrregularidade.Irregularidade.Codigo, MotivoIrregularidade.Irregularidade.Descricao },
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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
                unitOfWork.Start();

                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade = new Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade();

                PreencherIrregularidade(MotivoIrregularidade, unitOfWork);

                repositorioMotivoIrregularidade.Inserir(MotivoIrregularidade, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade = repositorioMotivoIrregularidade.BuscarPorCodigo(codigo);

                if (MotivoIrregularidade == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherIrregularidade(MotivoIrregularidade, unitOfWork);

                repositorioMotivoIrregularidade.Atualizar(MotivoIrregularidade, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade = repositorioMotivoIrregularidade.BuscarPorCodigo(codigo);

                if (MotivoIrregularidade == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioMotivoIrregularidade.Deletar(MotivoIrregularidade, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repositorioMotivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);
                int totalRegistro = repositorioMotivoIrregularidade.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade> motivosIrregularidades = (totalRegistro > 0) ? repositorioMotivoIrregularidade.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade>();

                var motivosIrregularidadesRetornar = (
                    from motivoIrregularidade in motivosIrregularidades
                    select new
                    {
                        motivoIrregularidade.Codigo,
                        motivoIrregularidade.Descricao,
                        motivoIrregularidade.Observacao,
                        Irregularidade = motivoIrregularidade.Irregularidade.Descricao,
                        Situacao = motivoIrregularidade.Ativa ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao()
                    }
                ).ToList();

                // Seta valores na grid
                grid.AdicionaRows(motivosIrregularidadesRetornar);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string prop)
        {
            return prop == "Situacao" ? "Ativa" : prop;
        }

        private Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoIrregularidade()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao"),
                CodigoIrregularidade = Request.GetIntParam("Irregularidade")
            };
        }

        private void PreencherIrregularidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade MotivoIrregularidade, Repositorio.UnitOfWork unitOfWork)
        {
            MotivoIrregularidade.Descricao = Request.GetStringParam("Descricao");
            MotivoIrregularidade.Observacao = Request.GetStringParam("Observacao");
            MotivoIrregularidade.Ativa = Request.GetBoolParam("Situacao");
            MotivoIrregularidade.TipoMotivo = Request.GetEnumParam<TipoMotivoIrregularidade>("TipoMotivo");

            var irregularidadeRepo = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
            var Irregularidade = irregularidadeRepo.BuscarPorCodigo(Request.GetIntParam("Irregularidade"));
            if (Irregularidade == null)
                throw new ControllerException("É necessário vincular a Irregularidade a um Motivo");
            MotivoIrregularidade.Irregularidade = Irregularidade;
        }


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Irregularidade", "Irregularidade", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
