using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/TipoContainer")]
    public class TipoContainerController : BaseController
    {
		#region Construtores

		public TipoContainerController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);
          

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("PesoContainer", false);
                grid.AdicionarCabecalho("TaraContainer", false);
                grid.AdicionarCabecalho("MetroCubicoContainer", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoContainer.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoContainer.Status, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipo> tipoContainers = repTipoContainer.Consultar(descricao, codigoIntegracao, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoContainer.ContarConsulta(descricao, codigoIntegracao, status));

                var lista = (from p in tipoContainers
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.CodigoIntegracao,
                                 p.DescricaoStatus,
                                 PesoContainer = p.PesoMaximo.ToString("n2"),
                                 TaraContainer = p.Tara.ToString("n2"),
                                 MetroCubicoContainer = p.MetrosCubicos.ToString("n2"),
                                 p.TipoPes,
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

                Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = new Dominio.Entidades.Embarcador.Pedidos.ContainerTipo();

                PreencherTipoContainer(tipoContainer, unitOfWork);
                repTipoContainer.Inserir(tipoContainer, Auditado);
                SalvarTiposContainerAssociado(tipoContainer, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
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

                Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = repTipoContainer.BuscarPorCodigo(codigo, true);

                PreencherTipoContainer(tipoContainer, unitOfWork);
                repTipoContainer.Atualizar(tipoContainer, Auditado);
                SalvarTiposContainerAssociado(tipoContainer, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = repTipoContainer.BuscarPorCodigo(codigo, false);

                var dynTipoContainer = new
                {
                    tipoContainer.Codigo,
                    tipoContainer.Descricao,
                    tipoContainer.CodigoIntegracao,
                    tipoContainer.CodigoDocumento,
                    tipoContainer.FFE,
                    tipoContainer.TEU,
                    tipoContainer.TipoPes,
                    tipoContainer.Status,
                    PesoMaximo = tipoContainer.PesoMaximo.ToString("n2"),
                    Valor = tipoContainer.Valor.ToString("n2"),
                    PesoLiquido = tipoContainer.PesoLiquido.ToString("n2"),
                    Tara = tipoContainer.Tara.ToString("n2"),
                    MetrosCubicos = tipoContainer.MetrosCubicos.ToString("n2"),
                    TipoContainersAssociado = (from obj in tipoContainer.Tipos
                                               select new
                                               {
                                                   TIPOCONTAINER = new
                                                   {
                                                       obj.ContainerTipoVinculado.Codigo,
                                                       obj.ContainerTipoVinculado.Descricao,
                                                       obj.ContainerTipoVinculado.CodigoIntegracao
                                                   }
                                               }).ToList(),
                    tipoContainer.TipoMeioTransporteEDI
                };

                return new JsonpResult(dynTipoContainer);
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
                Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = repTipoContainer.BuscarPorCodigo(codigo, true);

                if (tipoContainer == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repTipoContainer.DeletarPorCodigo(tipoContainer.Codigo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoContainer, null, "Registro apagado", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Delete);

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

        private void PreencherTipoContainer(Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer, Repositorio.UnitOfWork unitOfWork)
        {
            bool.TryParse(Request.Params("Status"), out bool status);


            decimal.TryParse(Request.Params("Valor"), out decimal valor);
            decimal.TryParse(Request.Params("PesoMaximo"), out decimal pesoMaximo);

            string descricao = Request.Params("Descricao");

            string codigoIntegracao = Request.Params("CodigoIntegracao");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPes tipoPes = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPes>("TipoPes");
            tipoContainer.Descricao = descricao;
            tipoContainer.Integrado = false;
            tipoContainer.CodigoIntegracao = codigoIntegracao;
            tipoContainer.CodigoDocumento = Request.GetStringParam("CodigoDocumento");
            tipoContainer.FFE = Request.GetStringParam("FFE");
            tipoContainer.TEU = Request.GetStringParam("TEU");
            tipoContainer.Status = status;
            tipoContainer.Valor = valor;
            tipoContainer.TipoPes = tipoPes;
            tipoContainer.PesoMaximo = pesoMaximo;
            tipoContainer.PesoLiquido = Request.GetDecimalParam("PesoLiquido");
            tipoContainer.Tara = Request.GetDecimalParam("Tara");
            tipoContainer.MetrosCubicos = Request.GetDecimalParam("MetrosCubicos");
            tipoContainer.TipoMeioTransporteEDI = Request.GetStringParam("TipoMeioTransporteEDI");

        }

        private void SalvarTiposContainerAssociado(Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.ContainerTipoAssociado repTipoContainerAssociado = new Repositorio.Embarcador.Pedidos.ContainerTipoAssociado(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);

            dynamic tiposContainerAssociado = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoContainersAssociado"));
            if (tipoContainer.Tipos != null && tipoContainer.Tipos.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var tipoContainerAssociado in tiposContainerAssociado)
                    if (tipoContainerAssociado.TIPOCONTAINER.Codigo != null)
                        codigos.Add((int)tipoContainerAssociado.TIPOCONTAINER.Codigo);

                List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado> tipoContainerAssociadoDeletar = (from obj in tipoContainer.Tipos where !codigos.Contains(obj.ContainerTipoVinculado.Codigo) select obj).ToList();

                for (var i = 0; i < tipoContainerAssociadoDeletar.Count; i++)
                    repTipoContainerAssociado.Deletar(tipoContainerAssociadoDeletar[i]);
            }
            else
                tipoContainer.Tipos = new List<Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado>();

            foreach (var tipoContainerAssociado in tiposContainerAssociado)
            {
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado containerTipoAssociado = tipoContainerAssociado.TIPOCONTAINER.Codigo != null ? repTipoContainerAssociado.BuscarPorTipoContainerEAssociado((int)tipoContainerAssociado.TIPOCONTAINER.Codigo, tipoContainer.Codigo) : null;
                if (containerTipoAssociado == null)
                {
                    containerTipoAssociado = new Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado();

                    int.TryParse((string)tipoContainerAssociado.TIPOCONTAINER.Codigo, out int codigoTipoContainerAssociado);
                    containerTipoAssociado.ContainerTipoVinculado = repTipoContainer.BuscarPorCodigo(codigoTipoContainerAssociado, false);
                    containerTipoAssociado.ContainerTipo = tipoContainer;

                    repTipoContainerAssociado.Inserir(containerTipoAssociado);
                }
            }
        }

        #endregion
    }
}
