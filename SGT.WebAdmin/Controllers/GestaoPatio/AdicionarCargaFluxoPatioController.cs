using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/AdicionarCargaFluxoPatio")]
    public class AdicionarCargaFluxoPatioController : BaseController
    {
		#region Construtores

		public AdicionarCargaFluxoPatioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga fluxoCaixaInformarCarga = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FluxoCaixaInformarCarga()
                {
                    CodigoCarga = Request.GetIntParam("Codigo"),
                    CodigoFilaCarregamento = Request.GetIntParam("FilaCarregamento"),
                    CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                    CodigoMotivoSelecaoMotoristaForaOrdem = Request.GetIntParam("MotivoSelecaoMotoristaForaOrdem"),
                    DataCarregamento = DateTime.Now,
                    Doca = Request.GetStringParam("Doca"),
                    Lacre = Request.GetStringParam("Lacre"),
                    PrimeiroNaFila = Request.GetBoolParam("PrimeiroNaFila")
                };

                Servicos.Embarcador.GestaoPatio.FluxoPatioInformarCarga servicoFluxoPatioInformarCarga = new Servicos.Embarcador.GestaoPatio.FluxoPatioInformarCarga(unitOfWork, Auditado);
                servicoFluxoPatioInformarCarga.AdicionarCarga(fluxoCaixaInformarCarga, TipoServicoMultisoftware, Cliente);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a carga no fluxo de pátio.");
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
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Cargas.CargaLacre repositorioCargaLacre = new Repositorio.Embarcador.Cargas.CargaLacre(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaLacre> lacres = repositorioCargaLacre.BuscarPorCarga(carga.Codigo);

                return new JsonpResult(new
                {
                    carga.Codigo,
                    carga.CodigoCargaEmbarcador,
                    DataCarregamento = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    Doca = carga.NumeroDoca,
                    Filial = new { carga.Filial.Codigo, carga.Filial.Descricao },
                    Lacre = string.Join(", ", (from o in lacres select o.Numero)),
                    ModeloVeicular = new { Codigo = carga.ModeloVeicularCarga?.Codigo ?? 0, Descricao = carga.ModeloVeicularCarga?.Descricao ?? "" }
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CpfCnpjDestinatario = Request.GetListParam<double>("Destinatario"),
                CpfCnpjRemetente = Request.GetListParam<double>("Remetente"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número da Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data de Carregamento", "DataCarregamento", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Filial", "Filial", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicularCarga", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoCarga", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaAdicionarFluxoPatio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterProriedadeOrdenar);
                Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
                int totalRegistros = repositorioCargaJanelaCarregamento.ContarConsultaPorCargaAdicionarFluxoPatio(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = (totalRegistros > 0) ? repositorioCargaJanelaCarregamento.ConsultarPorCargaAdicionarFluxoPatio(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();

                var listaCargaRetornar = (
                    from cargaJanelaCarregamento in listaCargaJanelaCarregamento
                    select new
                    {
                        cargaJanelaCarregamento.Carga.Codigo,
                        cargaJanelaCarregamento.Carga.CodigoCargaEmbarcador,
                        DataCarregamento = cargaJanelaCarregamento.Carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Filial = cargaJanelaCarregamento.Carga.Filial?.Descricao ?? "",
                        Destinatario = cargaJanelaCarregamento.Carga.DadosSumarizados?.DestinatariosReais ?? "",
                        ModeloVeicularCarga = cargaJanelaCarregamento.Carga.ModeloVeicularCarga?.Descricao ?? "",
                        Veiculo = cargaJanelaCarregamento.Carga.RetornarPlacas ?? "",
                        TipoCarga = cargaJanelaCarregamento.Carga.TipoDeCarga?.Descricao ?? "",
                        TipoOperacao = cargaJanelaCarregamento.Carga.TipoOperacao?.Descricao ?? "",
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterProriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Codigo")
                return "Carga.Codigo";

            if (propriedadeOrdenar == "CodigoCargaEmbarcador")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "DataCarregamento")
                return "Carga.DataCarregamentoCarga";

            if (propriedadeOrdenar == "Filial")
                return "Carga.Filial.Descricao";

            if (propriedadeOrdenar == "ModeloVeicularCarga")
                return "Carga.ModeloVeicularCarga.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "Carga.TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "TipoOperacao")
                return "Carga.TipoOperacao.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
