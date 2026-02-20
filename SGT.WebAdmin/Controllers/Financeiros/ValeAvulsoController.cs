using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ValeAvulso")]
    public class ValeAvulsoController : BaseController
    {
		#region Construtores

		public ValeAvulsoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso = new Dominio.Entidades.Embarcador.Financeiro.ValeAvulso();

                unitOfWork.Start();

                PreencherEntidade(valeAvulso, unitOfWork);

                if (valeAvulso.TipoDocumento == TipoDocumentoValeAvulso.Todos)
                    return new JsonpResult(false, "Favor informar o campo 'Tipo de Documento!' ");

                valeAvulso.NumeroVale = repValeAvulso.BuscarProximoNumero();
                valeAvulso.Numero = string.Format("0" + valeAvulso.NumeroVale);

                repValeAvulso.Inserir(valeAvulso, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso = repValeAvulso.BuscarPorCodigo(codigo, true);

                if (valeAvulso == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (valeAvulso.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValeAvulso.Aberto)
                    return new JsonpResult(false, true, "O status do Vale não permite edição.");

                unitOfWork.Start();

                PreencherEntidade(valeAvulso, unitOfWork);

                repValeAvulso.Atualizar(valeAvulso, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso = repValeAvulso.BuscarPorCodigo(codigo);

                if (valeAvulso == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var dynValeAvulso = new
                {
                    valeAvulso.Codigo,
                    valeAvulso.Numero,
                    RecebidoDe = new { valeAvulso.Empresa.Codigo, valeAvulso.Empresa.Descricao },
                    Pessoa = new { valeAvulso.Pessoa.Codigo, valeAvulso.Pessoa.Descricao },
                    valeAvulso.Situacao,
                    valeAvulso.Valor,
                    Data = valeAvulso.Data.ToString("dd/MM/yyyy"),
                    valeAvulso.Correspondente,
                    valeAvulso.TipoDocumento
                };
                return new JsonpResult(dynValeAvulso);
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso = repValeAvulso.BuscarPorCodigo(codigo);

                if (valeAvulso == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repValeAvulso.Deletar(valeAvulso, Auditado);

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

        public async Task<IActionResult> DownloadValeAvulso()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso = repValeAvulso.BuscarPorCodigo(codigo);

                if (valeAvulso == null)
                    return new JsonpResult(false, true, "Vale Avulso não encontrado.");

                byte[] relatorio = Servicos.Embarcador.Financeiro.ValeAvulso.GerarPdfValeAvulso(codigo, unitOfWork);

                return Arquivo(relatorio, "application/pdf", "Vale Avulso.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o Vale Avulso.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.ValeAvulso valeAvulso, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repPessoa = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int codigoEmpresa = Request.GetIntParam("RecebidoDe");
            double codigoPessoa = Request.GetDoubleParam("Pessoa");

            valeAvulso.Numero = Request.Params("Numero");
            valeAvulso.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            valeAvulso.Pessoa = repPessoa.BuscarPorCPFCNPJ(codigoPessoa);
            valeAvulso.Situacao = Request.GetEnumParam<SituacaoValeAvulso>("Situacao");
            valeAvulso.Valor = Request.GetDecimalParam("Valor");
            valeAvulso.Data = Request.GetDateTimeParam("Data");
            valeAvulso.TipoDocumento = Request.GetEnumParam<TipoDocumentoValeAvulso>("TipoDocumento");
            valeAvulso.Correspondente = Request.GetStringParam("Correspondente");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string numero = Request.Params("Numero");
                double codigoPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out codigoPessoa);
                SituacaoValeAvulso situacao = Request.GetEnumParam<SituacaoValeAvulso>("Situacao");
                TipoDocumentoValeAvulso tipoDocumento = Request.GetEnumParam<TipoDocumentoValeAvulso>("TipoDocumento");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Recebido", "RecebidoDe", 70, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo Doc.", "TipoDocumento", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 65, Models.Grid.Align.center, true);


                Repositorio.Embarcador.Financeiro.ValeAvulso repValeAvulso = new Repositorio.Embarcador.Financeiro.ValeAvulso(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.ValeAvulso> listaValeAvulso = repValeAvulso.Consultar(numero, codigoPessoa, situacao, tipoDocumento, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repValeAvulso.ContarConsulta(numero, codigoPessoa, situacao, tipoDocumento);

                var lista = (from p in listaValeAvulso
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 RecebidoDe = p.Empresa.Descricao,
                                 Situacao = p.Situacao.ObterDescricao(),
                                 p.Valor,
                                 TipoDocumento = p.TipoDocumento.ObterDescricao(),                                 
                                 Pessoa = p.Pessoa.Descricao
                             }).ToList();

                grid.AdicionaRows(lista);
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

        #endregion
    }
}
