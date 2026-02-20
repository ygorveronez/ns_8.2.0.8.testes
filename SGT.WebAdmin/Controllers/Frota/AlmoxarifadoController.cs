using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/Almoxarifado")]
    public class AlmoxarifadoController : BaseController
    {
		#region Construtores

		public AlmoxarifadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = new Dominio.Entidades.Embarcador.Frota.Almoxarifado();

                try
                {
                    PreencherAlmoxarifado(almoxarifado, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frota.Almoxarifado repositorio = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);

                repositorio.Inserir(almoxarifado, Auditado);

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
                Repositorio.Embarcador.Frota.Almoxarifado repositorio = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (almoxarifado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherAlmoxarifado(almoxarifado, unitOfWork);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(almoxarifado, Auditado);

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
                Repositorio.Embarcador.Frota.Almoxarifado repositorio = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = repositorio.BuscarPorCodigo(codigo);

                if (almoxarifado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    almoxarifado.Codigo,
                    almoxarifado.Descricao,
                    almoxarifado.Email,
                    Empresa = new { almoxarifado.Empresa.Codigo, almoxarifado.Empresa.Descricao },
                    FuncionarioResponsavel = new { Codigo = almoxarifado.FuncionarioResponsavel?.Codigo ?? 0, Descricao = almoxarifado.FuncionarioResponsavel?.Descricao ?? "" },
                    Status = almoxarifado.Ativo
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
                Repositorio.Embarcador.Frota.Almoxarifado repositorio = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (almoxarifado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(almoxarifado, Auditado);

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

        private void PreencherAlmoxarifado(Dominio.Entidades.Embarcador.Frota.Almoxarifado almoxarifado, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.GetStringParam("Descricao");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ControllerException("Descrição é obrigatória.");

            if (descricao.Length > 200)
                throw new ControllerException("Descrição não pode passar de 200 caracteres.");

            almoxarifado.Ativo = Request.GetBoolParam("Status");
            almoxarifado.Descricao = descricao;
            almoxarifado.Email = Request.GetStringParam("Email");
            almoxarifado.Empresa = ObterEmpresa(unitOfWork);
            almoxarifado.FuncionarioResponsavel = ObterFuncionarioResponsavel(unitOfWork);
        }

        private Dominio.Entidades.Empresa ObterEmpresa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoEmpresa = Request.GetIntParam("Empresa");
            Repositorio.Empresa repositorio = new Repositorio.Empresa(unitOfWork);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            return repositorio.BuscarPorCodigo(codigoEmpresa) ?? throw new ControllerException("Empresa não encontrada");
        }

        private Dominio.Entidades.Usuario ObterFuncionarioResponsavel(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFuncionario = Request.GetIntParam("FuncionarioResponsavel");
            Repositorio.Usuario repositorio = new Repositorio.Usuario(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoFuncionario) ?? throw new ControllerException("Funcionário responsável não encontrado");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAlmoxarifado filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaAlmoxarifado()
                {
                    CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Empresa"),
                    Descricao = Request.GetStringParam("Descricao"),
                    SituacaoAtivo = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Código","Codigo", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.Almoxarifado repositorio = new Repositorio.Embarcador.Frota.Almoxarifado(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.Almoxarifado> listaAlmoxarifado = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                var listaAlmoxarifadoRetornar = (
                    from almoxarifado in listaAlmoxarifado
                    select new
                    {
                        almoxarifado.Codigo,
                        almoxarifado.Descricao,
                        almoxarifado.DescricaoAtivo
                    }
                ).ToList();

                grid.AdicionaRows(listaAlmoxarifadoRetornar);
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
