using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Containers
{
    [CustomAuthorize("Containers/ContainerRedex")]
    public class ContainerRedexController : BaseController
    {
		#region Construtores

		public ContainerRedexController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Publicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisa(unitOfWork, false);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisa(unitOfWork, true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Pedidos.TipoOperacao.OcorreuUmaFalhaAoExportar);
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> InformarEmbarqueContainer()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("Codigo");
                DateTime dataEmbarque = Request.GetDateTimeParam("DataEmbarque");
                double localEmbarque = Request.GetDoubleParam("LocalEmbarque");

                Servicos.Embarcador.Pedido.ColetaContainer servColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainerCargaEntrega repColetaContainerCargaEntrega = new Repositorio.Embarcador.Pedidos.ColetaContainerCargaEntrega(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer OrigemMovimentacaoContainer = OrigemMovimentacaoContainer.UsuarioInterno;

                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorCodigo(codigo);

                if (coletaContainer?.Container != null && dataEmbarque != DateTime.MinValue)
                {
                    unitOfWork.Start();

                    Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repColetaContainerCargaEntrega.BuscarCargaEntregaPorColetaContainer(coletaContainer.Codigo);

                    Dominio.Entidades.Cliente LocalEmbarque = cargaEntrega?.Cliente;

                    if (localEmbarque > 0)
                        LocalEmbarque = repositorioCliente.BuscarPorCPFCNPJ(localEmbarque);

                    if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        OrigemMovimentacaoContainer = OrigemMovimentacaoContainer.PortalTransportador;

                    servColetaContainer.InformarEmbarqueContainer(dataEmbarque, LocalEmbarque, coletaContainer, this.Usuario, OrigemMovimentacaoContainer);

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Informou embarque container: " + coletaContainer.Container.Codigo + " Data Embarque:" + dataEmbarque.ToString("dd/MM/yyyy"), unitOfWork);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true, "Embarque do container informado com sucesso.");
                }
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao informar embarque do container. verifique a Data Embarque ");

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar embarque do container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion


        #region Métodos Privados


        private IActionResult ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {

            Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtrosPesquisa = ObterFiltrosPesquisa();
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", false);
            grid.AdicionarCabecalho("Container", "NumeroContainer", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Status", "SituacaoContainer", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Area Atual", "ClienteAtual", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Movimentacao", "DataMovimentacao", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Coleta", "DataColeta", 6, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Dias Free Time", "DiasFreeTime", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Dias de Posse", "DiasPosse", 8, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Excedeu Free Time", "ExcedeuFreeTime", 10, Models.Grid.Align.left, false);

            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

            int totalRegistros = repColetaContainer.ContarContainersPesquisa(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer> coletaContainerEmPosseEmbarcador = (totalRegistros > 0) ? repColetaContainer.BuscarContainersPesquisa(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Container.ControleContainer>();

            var lista = new Object();
            lista = (from coletacontainer in coletaContainerEmPosseEmbarcador
                     select new
                     {
                         coletacontainer.Codigo,
                         Carga = coletacontainer.CargaAtual > 0 ? coletacontainer.CargaAtual : coletacontainer.Carga,
                         coletacontainer.NumeroContainer,
                         DataColeta = coletacontainer.DataColeta != DateTime.MinValue ? coletacontainer.DataColeta.ToString("dd/MM/yyyy") : "",
                         SituacaoContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainerHelper.ObterDescricao(coletacontainer.Status),
                         DataMovimentacao = coletacontainer.DataUltimaMovimentacao.ToString("dd/MM/yyyy"),
                         ClienteAtual = coletacontainer.LocalAtual > 0 ? coletacontainer.ClienteLocalAtual + "(" + coletacontainer.LocalAtual + ")" : "",
                         DiasPosse = coletacontainer.DiasEmPosse,
                         ExcedeuFreeTime = coletacontainer.DiasExcesso > 0 ? "Sim" : "Não",
                         DiasFreeTime = coletacontainer.FreeTime
                     }).ToList();




            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
                return new JsonpResult(grid);
        }

        private Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer ObterFiltrosPesquisa()
        {

            Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaControleContainer()
            {
                DataFinalColeta = Request.GetDateTimeParam("DataFinal"),
                DataInicialColeta = Request.GetDateTimeParam("DataInicial"),
                DiasPosse = Request.GetIntParam("DiasPosse"),
                AreaDeRedex = Request.GetDoubleParam("AreaRedex"),
                SomenteExcedidos = Request.GetBoolParam("SomenteExcedentes"),
                StatusContainer = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusColetaContainer.EmAreaEsperaCarregado
            };

            return filtrosPesquisa;
        }

        #endregion

    }
}
