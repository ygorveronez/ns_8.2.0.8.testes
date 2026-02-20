using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize("Cargas/ResponsavelEntrega")]
    public class ResponsavelEntregaController : BaseController
    {
		#region Construtores

		public ResponsavelEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

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

        [AllowAuthenticate]
        public async Task<IActionResult> AtribuirResponsavel()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoResponsavel = Request.GetIntParam("ResponsavelEntrega");

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                var responsavelEntrega = repUsuario.BuscarPorCodigo(codigoResponsavel);

                if (responsavelEntrega == null)
                    return new JsonpResult(false, true, "Responsável deve ser informado.");

                var codigosCargas = ObterCodigoSelecionadas(unitOfWork);

                if (codigosCargas.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma carga selecionada.");


                if (codigosCargas.Count > 200)
                    return new JsonpResult(false, true, "Selecione um número menor de cargas para atribuir o responsável.");

                if (codigosCargas.Count() > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    var cargas = repCarga.BuscarPorCodigos(codigosCargas);

                    foreach(var carga in cargas)
                    {
                        carga.Initialize();
                        carga.ResponsavelEntrega = responsavelEntrega;
                        repCarga.Atualizar(carga, Auditado);
                    }
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter atribuir responsável.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverResponsavel()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var codigosCargas = ObterCodigoSelecionadas(unitOfWork);

                if (codigosCargas.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma carga selecionada.");


                if (codigosCargas.Count > 200)
                    return new JsonpResult(false, true, "Selecione um número menor de cargas para remover o responsável.");

                if (codigosCargas.Count() > 0)
                {

                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    var cargas = repCarga.BuscarPorCodigos(codigosCargas);

                    foreach (var carga in cargas)
                    {
                        carga.Initialize();
                        carga.ResponsavelEntrega = null;
                        repCarga.Atualizar(carga, Auditado);
                    }
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter atribuir responsável.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<int> ObterCodigoSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigosCarga = new List<int>();
            string itens = Request.Params("ItensSelecionados");

            if (string.IsNullOrEmpty(itens))
                return codigosCarga;

            var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)itens);

            foreach (var itemSelecionado in listaItensSelecionados)
                codigosCarga.Add((int)itemSelecionado.Codigo);

            return codigosCarga;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                DataInicial = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                CodigoResponsavelEntrega = Request.GetIntParam("ResponsavelEntrega"),
                SituacaoResponsavel = Request.GetEnumParam<SituacaoResponsavelEntrega>("SituacaoResponsavel")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };


                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Nº da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(propriedade: "TipoCarga", visivel: false);
                grid.AdicionarCabecalho(propriedade: "ModeloVeicular", visivel: false);
                grid.AdicionarCabecalho(propriedade: "NumeroCTes", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Transportador", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Filial", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Veiculo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Motorista", visivel: false);
                grid.AdicionarCabecalho(descricao: "Responsável Entrega", propriedade: "ResponsavelEntrega", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tipo Tomador", propriedade: "TiposTomador", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Remetente(s)", propriedade: "Remetentes", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Destinatário(s)", propriedade: "Destinatarios", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Container(es)", propriedade: "Containeres", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Data Carga", propriedade: "DataCriacaoCarga", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaEntrega filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                int totalRegistros = repCarga.ContarConsultarCargaEntrega(filtrosPesquisa, this.Usuario);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = totalRegistros > 0 ? repCarga.ConsultarCargasEntrega(filtrosPesquisa, parametrosConsulta, this.Usuario) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var lista = (
                    from carga in cargas
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Filial = carga.Filial?.Descricao,
                        ModeloVeicular = carga.ModeloVeicularCarga?.Descricao,
                        Motorista = carga.NomeMotoristas,
                        NumeroCTes = carga.NumerosCTes,
                        TipoCarga = carga.TipoDeCarga?.Descricao,
                        Transportador = carga.Empresa?.RazaoSocial,
                        Veiculo = carga.PlacasVeiculos,
                        ResponsavelEntrega = carga?.ResponsavelEntrega?.Descricao ?? string.Empty,
                        carga.TiposTomador,
                        Remetentes = carga.DadosSumarizados?.Remetentes ?? "",
                        Destinatarios = carga.DadosSumarizados?.Destinatarios ?? "",
                        carga.Containeres,
                        DataCriacaoCarga = carga.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                        carga.ModaisCarga
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "ModeloVeicular")
                return "ModeloVeicularCarga.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "PortoOrigem")
                return "PortoOrigem.Descricao";

            if (propriedadeOrdenar == "PortoDestino")
                return "PortoDestino.Descricao";

            if (propriedadeOrdenar == "Transportador")
                return "Empresa.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
