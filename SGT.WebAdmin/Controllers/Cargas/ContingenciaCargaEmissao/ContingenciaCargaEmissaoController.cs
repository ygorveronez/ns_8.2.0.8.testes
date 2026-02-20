using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using static QRCoder.PayloadGenerator;

namespace SGT.WebAdmin.Controllers.Cargas
{
    [CustomAuthorize("Cargas/ContingenciaCargaEmissao")]
    public class ContingenciaCargaEmissaoController : BaseController
    {
		#region Construtores

		public ContingenciaCargaEmissaoController(Conexao conexao) : base(conexao) { }

		#endregion


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
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                {
                    return Arquivo(arquivoBinario, "application/octet-stream", string.Concat(grid.tituloExportacao, ".", grid.extensaoCSV));
                }

				return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> AlterarEmissaoContingencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            try
            {
                IList<int> codigosSelecionados = ObterCodigosSelecionados(unitOfWork);
                bool contingencia = Request.GetBoolParam("Contingencia");

                unitOfWork.Start();
                if (codigosSelecionados.Count > 0)
                {
                    int take = 500;
                    int start = 0;
                    while (start < codigosSelecionados.Count)
                    {
                        List<int> codigosProcessar = codigosSelecionados.Skip(start).Take(take).ToList();
                        repCarga.AtualizarContingenciaEmissao(codigosProcessar, contingencia, Usuario.Codigo);

                        start += take;
                    }

                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao processar a liberação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao()
            {
                SituacaoCarga = Request.GetListEnumParam<SituacaoCarga>("SituacaoCarga"),
                DataCriacaoInicial = Request.GetDateTimeParam("DataCriacaoInicial"),
                DataCriacaoFinal = Request.GetDateTimeParam("DataCriacaoFinal"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CargasEmContingencia = Request.GetBoolParam("CargasEmContingencia"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("ContingenciaEmissao", false);
                grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Filial", "Filial", 9, Models.Grid.Align.left, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ContingenciaCargaEmissao/Pesquisa", "grid-contingencia-carga-emissao");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repCarga.ContarConsultaCargasContingenciaEmissao(filtrosPesquisa);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.CargaContingenciaEmissao> vloCargas = (totalRegistros > 0) ? repCarga.ConsultarCargasContingenciaEmissao(filtrosPesquisa,  parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaContingenciaEmissao>();

                var lista = (from obj in vloCargas
                             select FormatarConsulta(obj)).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Codigo")
                return "Carga.CAR_CODIGO";
            if (propriedadeOrdenar == "DataCriacao")
                return "Carga.CAR_DATA_CRIACAO";
            return propriedadeOrdenar;
        }

        private IList<int> ObterCodigosSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            IList<int> retorno = new List<int>();
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (selecionarTodos)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaContingenciaCargaEmissao filtrosPesquisa = ObterFiltrosPesquisa();
                var codigosNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                retorno = repCarga.ConsultarCodigosCargasContingenciaEmissao(filtrosPesquisa, null);

                foreach (var itemNaoSelecionado in codigosNaoSelecionados)
                    retorno.Remove((int)itemNaoSelecionado.Codigo);
            }
            else
            {
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                retorno = new List<int>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    retorno.Add((int)itemSelecionado.Codigo);
            }

            return retorno;
        }

        private dynamic FormatarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.CargaContingenciaEmissao obj)
        {
            return new {
                obj.Codigo,
                obj.ContingenciaEmissao,
                obj.NumeroCarga,
                DataCriacao = obj.DataCriacao?.ToString() ?? "",
                Situacao = obj.Situacao.ObterDescricao(),
                obj.TipoOperacao,
                Transportador = $"({FormatarCNPJ(obj.CnpjTransportador)}) - {obj.RazaoTransportador}",
                obj.Filial
            };
        }

        public string FormatarCNPJ(string cnpj) => (!string.IsNullOrWhiteSpace(cnpj) && cnpj.Length > 11) ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(cnpj)) : string.Empty;

        #endregion Métodos Privados
    }
}
