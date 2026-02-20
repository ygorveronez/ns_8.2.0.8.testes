using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Cargas.MontagemCarga
{
    [CustomAuthorize("Cargas/RegrasAgrupamentoPedidos")]
    public class RegrasAgrupamentoPedidosController : BaseController
    {
		#region Construtores

		public RegrasAgrupamentoPedidosController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos repRegrasAgrupamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                int filial = 0;
                int.TryParse(Request.Params("Filial"), out filial);

                string codigoIntegracao = Request.Params("CodigoIntegracao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Filial, "Filial", 80, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.MontagemCargaMapa.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos> listaRegrasAgrupamentoPedidos = repRegrasAgrupamentoPedidos.Consultar(filial, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegrasAgrupamentoPedidos.ContarConsulta(filial, ativo));
                var lista = (from p in listaRegrasAgrupamentoPedidos
                             select new
                             {
                                 p.Codigo,
                                 Filial = p.Descricao,
                                 p.DescricaoAtivo
                             }).ToList();
                grid.AdicionaRows(lista);
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

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos repRegrasAgrupamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                int.TryParse(Request.Params("Filial"), out int filial);
                int.TryParse(Request.Params("RaioKMEntreCidades"), out int raioKMEntreCidades);
                int.TryParse(Request.Params("ToleranciaDiasDiferenca"), out int toleranciaDiasDiferenca);
                int.TryParse(Request.Params("NumeroMaximoEntregas"), out int numeroMaximoEntregas);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamentoPedidos = new Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos();
                regrasAgrupamentoPedidos.Ativo = bool.Parse(Request.Params("Ativo"));

                regrasAgrupamentoPedidos.RaioKMEntreCidades = raioKMEntreCidades;
                regrasAgrupamentoPedidos.ToleranciaDiasDiferenca = toleranciaDiasDiferenca;
                regrasAgrupamentoPedidos.NumeroMaximoEntregas = numeroMaximoEntregas;

                if (filial > 0)
                    regrasAgrupamentoPedidos.Filial = repFilial.BuscarPorCodigo(filial);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamentoPedidosExiste = repRegrasAgrupamentoPedidos.BuscarPorFilial(filial);
                if (regrasAgrupamentoPedidosExiste == null)
                {
                    repRegrasAgrupamentoPedidos.Inserir(regrasAgrupamentoPedidos, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.JaExisteUmaRegraDeAgrupamentoCadastradaParaEssaFilial);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos repRegrasAgrupamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                int.TryParse(Request.Params("EmpresaEmissora"), out int empresaEmissora);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamentoPedidos = repRegrasAgrupamentoPedidos.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int.TryParse(Request.Params("Filial"), out int filial);
                int.TryParse(Request.Params("RaioKMEntreCidades"), out int raioKMEntreCidades);
                int.TryParse(Request.Params("ToleranciaDiasDiferenca"), out int toleranciaDiasDiferenca);
                int.TryParse(Request.Params("NumeroMaximoEntregas"), out int numeroMaximoEntregas);
                regrasAgrupamentoPedidos.Ativo = bool.Parse(Request.Params("Ativo"));

                regrasAgrupamentoPedidos.RaioKMEntreCidades = raioKMEntreCidades;
                regrasAgrupamentoPedidos.ToleranciaDiasDiferenca = toleranciaDiasDiferenca;
                regrasAgrupamentoPedidos.NumeroMaximoEntregas = numeroMaximoEntregas;

                if (filial > 0)
                    regrasAgrupamentoPedidos.Filial = repFilial.BuscarPorCodigo(filial);

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamentoPedidosExiste = repRegrasAgrupamentoPedidos.BuscarPorFilial(filial);
                if (regrasAgrupamentoPedidosExiste == null || regrasAgrupamentoPedidosExiste.Codigo == regrasAgrupamentoPedidos.Codigo)
                {
                    repRegrasAgrupamentoPedidos.Atualizar(regrasAgrupamentoPedidos, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Cargas.MontagemCargaMapa.JaExisteUmaRegraDeAgrupamentoCadastradaParaEssaFilial);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos repRegrasAgrupamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamentoPedidos = repRegrasAgrupamentoPedidos.BuscarPorCodigo(codigo);
                var dynRegrasAgrupamentoPedidos = new
                {
                    regrasAgrupamentoPedidos.Codigo,
                    Filial = new { Codigo = regrasAgrupamentoPedidos.Filial?.Codigo ?? 0, Descricao = regrasAgrupamentoPedidos.Filial?.Descricao ?? "" },
                    regrasAgrupamentoPedidos.Ativo,
                    NumeroMaximoEntregas = regrasAgrupamentoPedidos.NumeroMaximoEntregas > 0 ? regrasAgrupamentoPedidos.NumeroMaximoEntregas.ToString() : "",
                    ToleranciaDiasDiferenca = regrasAgrupamentoPedidos.ToleranciaDiasDiferenca > 0 ? regrasAgrupamentoPedidos.ToleranciaDiasDiferenca.ToString() : "",
                    RaioKMEntreCidades = regrasAgrupamentoPedidos.RaioKMEntreCidades > 0 ? regrasAgrupamentoPedidos.RaioKMEntreCidades.ToString() : ""
                };
                return new JsonpResult(dynRegrasAgrupamentoPedidos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
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
                Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos repRegrasAgrupamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos(unitOfWork);

                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos regrasAgrupamentoPedidos = repRegrasAgrupamentoPedidos.BuscarPorCodigo(codigo);

                repRegrasAgrupamentoPedidos.Deletar(regrasAgrupamentoPedidos, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

    }
}
