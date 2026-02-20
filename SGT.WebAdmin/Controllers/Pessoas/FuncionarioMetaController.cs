using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/FuncionarioMeta")]
    public class FuncionarioMetaController : BaseController
    {
		#region Construtores

		public FuncionarioMetaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Funcionario"), out int codigoFuncionario);

                DateTime.TryParse(Request.Params("DataVigencia"), out DateTime dataVigencia);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Funcionário", "Funcionario", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Vigência", "DataVigencia", 20, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);
                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta> funcionarioMetas = repFuncionarioMeta.Consultar(codigoEmpresa, codigoFuncionario, dataVigencia, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFuncionarioMeta.ContarConsulta(codigoEmpresa, codigoFuncionario, dataVigencia));

                var lista = (from p in funcionarioMetas
                             select new
                             {
                                 p.Codigo,
                                 Funcionario = p.Funcionario != null ? p.Funcionario.Nome : string.Empty,
                                 DataVigencia = p.DataVigencia.ToString("dd/MM/yyyy")
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta = new Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta();

                PreencherFuncionarioMeta(funcionarioMeta, unitOfWork);
                repFuncionarioMeta.Inserir(funcionarioMeta, Auditado);

                SalvarTabelaMetas(funcionarioMeta, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta = repFuncionarioMeta.BuscarPorCodigo(codigo, true);

                PreencherFuncionarioMeta(funcionarioMeta, unitOfWork);
                repFuncionarioMeta.Atualizar(funcionarioMeta, Auditado);

                SalvarTabelaMetas(funcionarioMeta, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta = repFuncionarioMeta.BuscarPorCodigo(codigo);

                var dynFuncionarioMeta = new
                {
                    funcionarioMeta.Codigo,
                    DataVigencia = funcionarioMeta.DataVigencia.ToString("dd/MM/yyyy"),
                    Funcionario = funcionarioMeta.Funcionario != null ? new { funcionarioMeta.Funcionario.Codigo, funcionarioMeta.Funcionario.Descricao } : null,
                    Metas = (from obj in funcionarioMeta.Metas
                             select new
                             {
                                 obj.Codigo,
                                 Mes = obj.Mes.ToString("n0"),
                                 Ano = obj.Ano.ToString("n0"),
                                 Valor = obj.Valor.ToString("n2"),
                                 Percentual = obj.Percentual.ToString("n2")
                             }).ToList()
                };

                return new JsonpResult(dynFuncionarioMeta);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta = repFuncionarioMeta.BuscarPorCodigo(codigo, true);

                if (funcionarioMeta == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repFuncionarioMeta.Deletar(funcionarioMeta, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherFuncionarioMeta(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta repFuncionarioMeta = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMeta(unitOfWork);

            int codigoEmpresa = this.Usuario.Empresa.Codigo;
            int.TryParse(Request.Params("Funcionario"), out int codigoFuncionario);

            DateTime.TryParse(Request.Params("DataVigencia"), out DateTime dataVigencia);

            funcionarioMeta.DataVigencia = dataVigencia;
            if (codigoFuncionario > 0)
                funcionarioMeta.Funcionario = repFuncionario.BuscarPorCodigo(codigoFuncionario);
            else
                funcionarioMeta.Funcionario = null;
            if (funcionarioMeta.Codigo == 0)
                funcionarioMeta.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
        }

        private void SalvarTabelaMetas(Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMeta funcionarioMeta, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMetaValor repFuncionarioMetaValor = new Repositorio.Embarcador.Usuarios.Comissao.FuncionarioMetaValor(unidadeTrabalho);

            dynamic metas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Metas"));
            if (funcionarioMeta.Metas != null && funcionarioMeta.Metas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var meta in metas)
                    if (meta.Codigo != null)
                        codigos.Add((int)meta.Codigo);

                List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaValor> funcionarioMetaValorDeletar = (from obj in funcionarioMeta.Metas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < funcionarioMetaValorDeletar.Count; i++)
                    repFuncionarioMetaValor.Deletar(funcionarioMetaValorDeletar[i], Auditado);
            }
            else
                funcionarioMeta.Metas = new List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaValor>();

            foreach (var meta in metas)
            {
                Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaValor funcionarioMetaValor = meta.Codigo != null ? repFuncionarioMetaValor.BuscarPorCodigo((int)meta.Codigo, true) : null;
                if (funcionarioMetaValor == null)
                    funcionarioMetaValor = new Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioMetaValor();

                string stringAno = (string)meta.Ano;
                int.TryParse((string)meta.Mes, out int mes);
                int.TryParse(stringAno.Replace(".", ""), out int ano);

                funcionarioMetaValor.Mes = mes;
                funcionarioMetaValor.Ano = ano;
                funcionarioMetaValor.Valor = Utilidades.Decimal.Converter((string)meta.Valor);
                funcionarioMetaValor.Percentual = Utilidades.Decimal.Converter((string)meta.Percentual);
                funcionarioMetaValor.FuncionarioMeta = funcionarioMeta;

                if (funcionarioMetaValor.Codigo > 0)
                    repFuncionarioMetaValor.Atualizar(funcionarioMetaValor, Auditado);
                else
                    repFuncionarioMetaValor.Inserir(funcionarioMetaValor, Auditado);
            }
        }

        #endregion
    }
}
