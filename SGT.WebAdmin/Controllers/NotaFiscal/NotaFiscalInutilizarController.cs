using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/NotaFiscalInutilizar")]
    public class NotaFiscalInutilizarController : BaseController
    {
		#region Construtores

		public NotaFiscalInutilizarController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar repNotaFiscalInutilizar = new Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar(unitOfWork);

                int numeroInicial, numeroFinal;

                string justificativa = Request.Params("Justificativa");
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Justificativa", "Justificativa", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo", "Modelo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Número Inicial", "NumeroInicial", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número Final", "NumeroFinal", 10, Models.Grid.Align.left, true);

                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar> listaNotaFiscalInutilizar = repNotaFiscalInutilizar.Consultar(justificativa, numeroInicial, numeroFinal, empresa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscalInutilizar.ContarConsulta(justificativa, numeroInicial, numeroFinal, empresa));
                var lista = (from p in listaNotaFiscalInutilizar
                            select new
                            {
                                p.Codigo,
                                p.Justificativa,
                                p.Modelo,
                                p.NumeroInicial,
                                p.NumeroFinal
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

                Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar repNotaFiscalInutilizar = new Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar notaFiscalInutilizar = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar();

                int empresa, empresaSerie, notaFiscal;
                int numeroInicial, numeroFinal;

                string justificativa = Request.Params("Justificativa");
                string modelo = Request.Params("Modelo");

                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("EmpresaSerie"), out empresaSerie);
                int.TryParse(Request.Params("NotaFiscal"), out notaFiscal);
                empresa = this.Usuario.Empresa.Codigo;

                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);

                if (notaFiscal > 0)
                    notaFiscalInutilizar.NotaFiscal = repNotaFiscal.BuscarPorCodigo(notaFiscal);
                else
                    notaFiscalInutilizar.NotaFiscal = null;
                if (empresaSerie > 0)
                    notaFiscalInutilizar.EmpresaSerie = repEmpresaSerie.BuscarPorCodigo(empresaSerie);
                else
                    notaFiscalInutilizar.EmpresaSerie = null;
                notaFiscalInutilizar.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                notaFiscalInutilizar.Justificativa = justificativa;
                notaFiscalInutilizar.Modelo = modelo;

                notaFiscalInutilizar.NumeroInicial = numeroInicial;
                notaFiscalInutilizar.NumeroFinal = numeroFinal;

                repNotaFiscalInutilizar.Inserir(notaFiscalInutilizar, Auditado);
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
                unitOfWork.Start();

                Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar repNotaFiscalInutilizar = new Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar notaFiscalInutilizar = repNotaFiscalInutilizar.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int empresa, empresaSerie, notaFiscal;
                int numeroInicial, numeroFinal;

                string justificativa = Request.Params("Justificativa");
                string modelo = Request.Params("Modelo");

                int.TryParse(Request.Params("Empresa"), out empresa);
                int.TryParse(Request.Params("EmpresaSerie"), out empresaSerie);
                int.TryParse(Request.Params("NotaFiscal"), out notaFiscal);
                empresa = this.Usuario.Empresa.Codigo;

                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);

                if (notaFiscal > 0)
                    notaFiscalInutilizar.NotaFiscal = repNotaFiscal.BuscarPorCodigo(notaFiscal);
                else
                    notaFiscalInutilizar.NotaFiscal = null;
                if (empresaSerie > 0)
                    notaFiscalInutilizar.EmpresaSerie = repEmpresaSerie.BuscarPorCodigo(empresaSerie);
                else
                    notaFiscalInutilizar.EmpresaSerie = null;
                notaFiscalInutilizar.Empresa = repEmpresa.BuscarPorCodigo(empresa);

                notaFiscalInutilizar.Justificativa = justificativa;
                notaFiscalInutilizar.Modelo = modelo;

                notaFiscalInutilizar.NumeroInicial = numeroInicial;
                notaFiscalInutilizar.NumeroFinal = numeroFinal;

                repNotaFiscalInutilizar.Atualizar(notaFiscalInutilizar, Auditado);
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar repNotaFiscalInutilizar = new Repositorio.Embarcador.NotaFiscal.NotaFiscalInutilizar(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalInutilizar notaFiscalInutilizar = repNotaFiscalInutilizar.BuscarPorCodigo(codigo);
                var dynProcessoMovimento = new
                {
                    notaFiscalInutilizar.Codigo,
                    notaFiscalInutilizar.Justificativa,
                    notaFiscalInutilizar.Modelo,
                    notaFiscalInutilizar.NumeroInicial,
                    notaFiscalInutilizar.NumeroFinal,
                    NotaFiscal = notaFiscalInutilizar.NotaFiscal != null ? new { Codigo = notaFiscalInutilizar.NotaFiscal.Codigo, Descricao = notaFiscalInutilizar.NotaFiscal.Numero } : null,
                    EmpresaSerie = notaFiscalInutilizar.EmpresaSerie != null ? new { Codigo = notaFiscalInutilizar.EmpresaSerie.Codigo, Descricao = notaFiscalInutilizar.EmpresaSerie.Numero } : null,
                    Empresa = notaFiscalInutilizar.Empresa != null ? new { Codigo = notaFiscalInutilizar.Empresa.Codigo, Descricao = notaFiscalInutilizar.Empresa.RazaoSocial } : null
                };
                return new JsonpResult(dynProcessoMovimento);
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

        #endregion
    }
}
