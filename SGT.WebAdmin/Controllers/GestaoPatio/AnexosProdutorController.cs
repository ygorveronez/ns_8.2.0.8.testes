using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/AnexosProdutor")]
    public class AnexosProdutorController : BaseController
    {
		#region Construtores

		public AnexosProdutorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio()
            {
                Situacao = Request.GetNullableEnumParam<SituacaoEtapaFluxoGestaoPatio>("Situacao"),
                NumeroCarga = Request.GetStringParam("Carga")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFluxoGestaoPatio", false);
                grid.AdicionarCabecalho("VisibilidadeFechamentoPesagem", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fim de Descarregamento", "FimDescarregamento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Etapa Fluxo Patio", "EtapaFluxoPatioDescricao", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaFluxoGestaoPatio filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                filtrosPesquisa.CpfCnpjRemetente = new List<double>();

                if (this.Usuario.ClienteFornecedor?.CPF_CNPJ_SemFormato != null)
                    filtrosPesquisa.CpfCnpjRemetente.Add(this.Usuario.ClienteFornecedor.CPF_CNPJ_SemFormato.ToDouble());

                int totalRegistros = repFluxoGestaoPatio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio> fluxosPatio = totalRegistros > 0 ? repFluxoGestaoPatio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>();

                var listaFluxos = (
                    from fluxo in fluxosPatio
                    where fluxo.Carga != null
                    select new
                    {
                        fluxo.Carga.Codigo,
                        CodigoFluxoGestaoPatio = fluxo.Codigo,
                        fluxo.Carga.CodigoCargaEmbarcador,
                        EtapaFluxoPatioDescricao = fluxo.DataFinalizacaoFluxo.HasValue ? "Finalizado" : servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(fluxo)?.Descricao ?? "",
                        Transportador = fluxo.Carga.Empresa?.Descricao ?? "",
                        Veiculo = fluxo.Carga.Veiculo?.Descricao ?? "",
                        FimDescarregamento = fluxo.DataFimCarregamento.HasValue ? fluxo.DataFimCarregamento.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        VisibilidadeFechamentoPesagem = ObterVisibilidadeFechamentoPesagem(fluxo)
                    }
                ).ToList();

                grid.AdicionaRows(listaFluxos);
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
            if (propriedadeOrdenar == "EtapaFluxoPatio")
                return "EtapaFluxoGestaoPatioAtual";

            return propriedadeOrdenar;
        }

        private bool ObterVisibilidadeFechamentoPesagem(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio)
        {
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas etapaFechamentoPesagem = fluxoPatio.GetEtapas().Where(etapa => etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.Expedicao).FirstOrDefault() ?? fluxoPatio.GetEtapas().Where(etapa => etapa.EtapaFluxoGestaoPatio == EtapaFluxoGestaoPatio.DeslocamentoPatio).FirstOrDefault();

            if (etapaFechamentoPesagem == null)
                return false;

            return fluxoPatio.EtapaAtual >= etapaFechamentoPesagem.Ordem;
        }

        #endregion
    }
}
