using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IndicadorIntegracaoCTe")]
    public class IndicadorIntegracaoCTeController : BaseController
    {
		#region Construtores

		public IndicadorIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaCTeAguardandoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaCTeAguardandoIntegracao(exportarRegistros: true);
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaGraficoIntegracaoAutomaticaCTe()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaGraficoIntegracaoAutomaticaCTe();
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
        public async Task<IActionResult> PesquisaCTeAguardandoIntegracao()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaCTeAguardandoIntegracao(exportarRegistros: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaGraficoIntegracaoAutomaticaCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork);
                dynamic dadosGrafico = repositorioIndicadorIntegracaoCTe.ConsultaGraficoIntegracaoAutomatica(filtrosPesquisa);

                if (dadosGrafico == null)
                    return new JsonpResult(false, "Nenhum registro encontrado.");

                return new JsonpResult(dadosGrafico);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do gráfico de integração automática de CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataEmissaoInicio = Request.GetNullableDateTimeParam("DataEmissaoInicio"),
                DataEmissaoLimite = Request.GetNullableDateTimeParam("DataEmissaoLimite"),
                DataIntegracaoInicio = Request.GetNullableDateTimeParam("DataIntegracaoInicio"),
                DataIntegracaoLimite = Request.GetNullableDateTimeParam("DataIntegracaoLimite"),
                SomenteIntegrado = true
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe ObterFiltrosPesquisaAguardandoIntegracao()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataEmissaoInicio = Request.GetNullableDateTimeParam("DataEmissaoInicio"),
                DataEmissaoLimite = Request.GetNullableDateTimeParam("DataEmissaoLimite"),
                SomenteIntegrado = false
            };
        }

        private Models.Grid.Grid ObterGridPesquisaCTeAguardandoIntegracao(bool exportarRegistros)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras = ObterIntegradoras(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número CT-e", propriedade: "NumeroCTe", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Série CT-e", propriedade: "SerieCTe", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: exportarRegistros);
                grid.AdicionarCabecalho(descricao: "Data Emissão CT-e", propriedade: "DataEmissaoCTe", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Chave CT-e", propriedade: "ChaveCTe", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: exportarRegistros).UtilizarFormatoTexto(true);
                grid.AdicionarCabecalho(descricao: "Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 10, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false, visible: exportarRegistros);

                foreach (KeyValuePair<int, Dominio.Entidades.WebService.Integradora> integradora in integradoras.OrderBy(o => o.Key))
                    grid.AdicionarCabecalho(descricao: $"Integrado {integradora.Value.Descricao}", propriedade: $"Integradora{integradora.Key}", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: exportarRegistros);

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisaAguardandoIntegracao();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork);
                int totalRegistros = repositorioIndicadorIntegracaoCTe.ContarConsulta(filtrosPesquisa);
                IList<Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe> listaCTeAguardandoIntegracao = (totalRegistros > 0) ? repositorioIndicadorIntegracaoCTe.Consultar(filtrosPesquisa, parametrosConsulta) :  new List<Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe>();

                dynamic listaCargaCTeRetornar = (
                    from o in listaCTeAguardandoIntegracao
                    select ObterIndicadorAguardandoIntegracao(o, integradoras)
                ).ToList();

                grid.AdicionaRows(listaCargaCTeRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridPesquisaGraficoIntegracaoAutomaticaCTe()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras = ObterIntegradoras(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>(),
                    tituloExportacao = "Integração Automática de CT-e"
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número CT-e", propriedade: "NumeroCTe", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Série CT-e", propriedade: "SerieCTe", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Data Emissão CT-e", propriedade: "DataEmissaoCTe", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Chave CT-e", propriedade: "ChaveCTe", tamanho: 10, alinhamento: Models.Grid.Align.center).UtilizarFormatoTexto(true);
                grid.AdicionarCabecalho(descricao: "Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 10, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 10, alinhamento: Models.Grid.Align.left);

                foreach (KeyValuePair<int, Dominio.Entidades.WebService.Integradora> integradora in integradoras.OrderBy(o => o.Key))
                    grid.AdicionarCabecalho(descricao: $"Data Integração {integradora.Value.Descricao}", propriedade: $"Integradora{integradora.Key}", tamanho: 10, alinhamento: Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIndicadorIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    PropriedadeOrdenar = ""
                };
                Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe repositorioIndicadorIntegracaoCTe = new Repositorio.Embarcador.Integracao.IndicadorIntegracaoCTe(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe> listaIntegracaoAutomaticaCTe = repositorioIndicadorIntegracaoCTe.Consultar(filtrosPesquisa, parametrosConsulta);

                dynamic listaIntegracaoAutomaticaCTeRetornar = (
                    from o in listaIntegracaoAutomaticaCTe
                    select ObterIndicadorIntegracaoAutomatica(o, integradoras)
                ).ToList();

                grid.AdicionaRows(listaIntegracaoAutomaticaCTeRetornar);
                grid.setarQuantidadeTotal(listaIntegracaoAutomaticaCTe.Count());

                return grid;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe ObterIndicadorAguardandoIntegracao(Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe indicadorIntegracaoCTe, Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras)
        {
            string[] indicadorIntegradoras = indicadorIntegracaoCTe.Integradoras.Split(',');
            Dictionary<int, string> indicadorIntegradorasDadosFormatados = new Dictionary<int, string>();

            foreach (string indicadorIntegradora in indicadorIntegradoras)
            {
                string[] indicadorIntegradoraDados = indicadorIntegradora.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                indicadorIntegradorasDadosFormatados.Add(indicadorIntegradoraDados[0].ToInt(), (indicadorIntegradoraDados.Count() > 1) ? "Sim" : "Não");
            }

            foreach (KeyValuePair<int, Dominio.Entidades.WebService.Integradora> integradora in integradoras.OrderBy(o => o.Key))
            {
                string valor = string.Empty;

                if (!indicadorIntegradorasDadosFormatados.TryGetValue(integradora.Value.Codigo, out valor))
                    valor = "Não";

                indicadorIntegracaoCTe.GetType().GetProperty($"Integradora{integradora.Key}")?.SetValue(indicadorIntegracaoCTe, valor);
            }

            return indicadorIntegracaoCTe;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe ObterIndicadorIntegracaoAutomatica(Dominio.ObjetosDeValor.Embarcador.Integracao.IndicadorIntegracaoCTe indicadorIntegracaoCTe, Dictionary<int, Dominio.Entidades.WebService.Integradora> integradoras)
        {
            string[] indicadorIntegradoras = indicadorIntegracaoCTe.Integradoras.Split(',');
            Dictionary<int, string> indicadorIntegradorasDadosFormatados = new Dictionary<int, string>();

            foreach (string indicadorIntegradora in indicadorIntegradoras)
            {
                string[] indicadorIntegradoraDados = indicadorIntegradora.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                indicadorIntegradorasDadosFormatados.Add(indicadorIntegradoraDados[0].ToInt(), (indicadorIntegradoraDados.Count() > 1) ? indicadorIntegradoraDados[1] : "");
            }

            foreach (KeyValuePair<int, Dominio.Entidades.WebService.Integradora> integradora in integradoras.OrderBy(o => o.Key))
            {
                string valor = string.Empty;

                if (!indicadorIntegradorasDadosFormatados.TryGetValue(integradora.Value.Codigo, out valor))
                    valor = "";

                indicadorIntegracaoCTe.GetType().GetProperty($"Integradora{integradora.Key}")?.SetValue(indicadorIntegracaoCTe, valor);
            }

            return indicadorIntegracaoCTe;
        }

        private Dictionary<int, Dominio.Entidades.WebService.Integradora> ObterIntegradoras(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);
            List<Dominio.Entidades.WebService.Integradora> integradoras = repositorioIntegradora.BuscarPorIndicarIntegracao();
            Dictionary<int, Dominio.Entidades.WebService.Integradora> integradorasRetornar = new Dictionary<int, Dominio.Entidades.WebService.Integradora>();

            for (int i = 0; i < integradoras.Count; i++)
                integradorasRetornar.Add(i + 1, integradoras[i]);

            return integradorasRetornar;
        }

        #endregion
    }
}
