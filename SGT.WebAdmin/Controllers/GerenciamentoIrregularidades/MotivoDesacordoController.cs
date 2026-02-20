using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GerenciamentoIrregularidades
{
    [CustomAuthorize("GerenciamentoIrregularidades/MotivoDesacordo")]
    public class MotivoDesacordoController : BaseController
    {
		#region Construtores

		public MotivoDesacordoController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Substitui CTe", "SubstituiCTe", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Irregularidade", "Irregularidade", 40, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repositorioMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);
                int totalRegistro = repositorioMotivoDesacordo.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo> motivosDesacordo =
                    (totalRegistro > 0) ?
                    repositorioMotivoDesacordo.Consultar(filtrosPesquisa, parametrosConsulta) :
                    new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>();

                var motivoDesacordoRetornar = (
                    from motivoDesacordo in motivosDesacordo
                    select new
                    {
                        Codigo = motivoDesacordo.Codigo,
                        Descricao = motivoDesacordo.Descricao,
                        Observacao = motivoDesacordo.Observacao,
                        SubstituiCTe = motivoDesacordo.SubstituiCTe.ObterDescricao(),
                        Situacao = motivoDesacordo.Situacao ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao(),
                        Irregularidade = motivoDesacordo?.Irregularidade?.Descricao,
                    }
                ).ToList();

                grid.AdicionaRows(motivoDesacordoRetornar);
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
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repositorioMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo MotivoDesacordo = repositorioMotivoDesacordo.BuscarPorCodigo(codigo);

                if (MotivoDesacordo == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    MotivoDesacordo.Codigo,
                    MotivoDesacordo.Descricao,
                    MotivoDesacordo?.Observacao,
                    MotivoDesacordo.SubstituiCTe,
                    Situacao = MotivoDesacordo.Situacao,
                    Irregularidade = new { MotivoDesacordo?.Irregularidade?.Codigo, MotivoDesacordo?.Irregularidade?.Descricao },
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

                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repositorioMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo MotivoDesacordo = new Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo();

                PreencherIrregularidade(MotivoDesacordo, unitOfWork);

                if (repositorioMotivoDesacordo.ExisteDuplicidade(MotivoDesacordo) == true)
                {
                    throw new ControllerException("Já existe um registro com esses dados");
                }

                repositorioMotivoDesacordo.Inserir(MotivoDesacordo, Auditado);

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
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repositorioMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo MotivoDesacordo = repositorioMotivoDesacordo.BuscarPorCodigo(codigo);

                if (MotivoDesacordo == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherIrregularidade(MotivoDesacordo, unitOfWork);

                if (repositorioMotivoDesacordo.ExisteDuplicidade(MotivoDesacordo) == true)
                {
                    throw new ControllerException("Já existe um registro com esses dados");
                }

                repositorioMotivoDesacordo.Atualizar(MotivoDesacordo, Auditado);

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
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repositorioMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo MotivoDesacordo = repositorioMotivoDesacordo.BuscarPorCodigo(codigo);

                if (MotivoDesacordo == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositorioMotivoDesacordo.Deletar(MotivoDesacordo, Auditado);

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
                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo repositorioMotivoDesacordo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo(unitOfWork);
                int totalRegistro = repositorioMotivoDesacordo.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo> motivosDesacordo = (totalRegistro > 0) ? repositorioMotivoDesacordo.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo>();

                var motivosDesacordoRetornar = (
                    from desacordo in motivosDesacordo
                    select new
                    {
                        desacordo.Codigo,
                        desacordo.Descricao,
                        desacordo.Observacao,
                        SubstituiCTe = desacordo.SubstituiCTe.ObterDescricao(),
                        Irregularidade = desacordo?.Irregularidade?.Descricao,
                        Situacao = desacordo.Situacao ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao()
                    }
                ).ToList();

                // Seta valores na grid
                grid.AdicionaRows(motivosDesacordoRetornar);

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
            return prop;
        }

        private Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaMotivoDesacordo()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao"),
                SubstituiCTe = Request.GetBoolParam("SubstituiCTe"),
                CodigoIrregularidade = Request.GetIntParam("Irregularidade")
            };
        }

        private void PreencherIrregularidade(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoDesacordo MotivoDesacordo, Repositorio.UnitOfWork unitOfWork)
        {
            MotivoDesacordo.Descricao = Request.GetStringParam("Descricao");
            MotivoDesacordo.Observacao = Request.GetStringParam("Observacao");
            MotivoDesacordo.Situacao = Request.GetBoolParam("Situacao");
            MotivoDesacordo.SubstituiCTe = Request.GetBoolParam("SubstituiCTe");

            var irregularidadeRepo = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
            var Irregularidade = irregularidadeRepo.BuscarPorCodigo(Request.GetIntParam("Irregularidade"));
            if (Irregularidade == null)
                throw new ControllerException("É necessário vincular o Motivo a uma Irregularidade");
            MotivoDesacordo.Irregularidade = Irregularidade;
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
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Substitui CTe", "SubstituiCTe", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Irregularidade", "Irregularidade", 15, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}
