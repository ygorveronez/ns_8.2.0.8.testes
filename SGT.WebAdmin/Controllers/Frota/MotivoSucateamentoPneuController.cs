using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/MotivoSucateamentoPneu")]
    public class MotivoSucateamentoPneuController : BaseController
    {
		#region Construtores

		public MotivoSucateamentoPneuController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu motivoSucateamentoPneu = new Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu();

                try
                {
                    PreencherMotivoSucateamentoPneu(motivoSucateamentoPneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repositorio = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(unitOfWork);

                repositorio.Inserir(motivoSucateamentoPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repositorio = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu motivoSucateamentoPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoSucateamentoPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherMotivoSucateamentoPneu(motivoSucateamentoPneu, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(motivoSucateamentoPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repositorio = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu motivoSucateamentoPneu = repositorio.BuscarPorCodigo(codigo);

                if (motivoSucateamentoPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    motivoSucateamentoPneu.Codigo,
                    motivoSucateamentoPneu.Descricao,
                    Status = motivoSucateamentoPneu.Ativo,
                    TipoMovimento = new { Codigo = motivoSucateamentoPneu.TipoMovimento != null ? motivoSucateamentoPneu.TipoMovimento.Codigo : 0, Descricao = motivoSucateamentoPneu.TipoMovimento != null ? motivoSucateamentoPneu.TipoMovimento.Descricao : "" }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repositorio = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu motivoSucateamentoPneu = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (motivoSucateamentoPneu == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(motivoSucateamentoPneu, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherMotivoSucateamentoPneu(Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu motivoSucateamentoPneu, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

            
            string descricao = Request.GetStringParam("Descricao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            int codigoTipoMovimento = Request.GetIntParam("TipoMovimento");

            motivoSucateamentoPneu.Descricao = descricao;
            motivoSucateamentoPneu.Ativo = Request.GetBoolParam("Status");
            motivoSucateamentoPneu.TipoMovimento = codigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                motivoSucateamentoPneu.Empresa = this.Usuario.Empresa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMotivoSucateamentoPneu filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMotivoSucateamentoPneu()
                {
                    Descricao = Request.GetStringParam("Descricao"),
                    SituacaoAtivo = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.MotivoSucateamentoPneu repositorio = new Repositorio.Embarcador.Frota.MotivoSucateamentoPneu(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.MotivoSucateamentoPneu> listaMotivoSucateamentoPneu = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaMotivoSucateamentoPneuRetornar = (
                    from motivoSucateamentoPneu in listaMotivoSucateamentoPneu
                    select new
                    {
                        motivoSucateamentoPneu.Codigo,
                        motivoSucateamentoPneu.Descricao,
                        motivoSucateamentoPneu.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaMotivoSucateamentoPneuRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
