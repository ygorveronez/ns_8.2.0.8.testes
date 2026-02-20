using Dominio.Entidades;
using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using Exception = System.Exception;

namespace SGT.WebAdmin.Controllers.AtendimentoCRM
{
    [CustomAuthorize(new string[] { "PesquisaNotaFiscal" }, "Atendimentos/Atendimento")]
    public class AtendimentoCRMController : BaseController
    {
		#region Construtores

		public AtendimentoCRMController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        
        public async Task<IActionResult> PesquisaNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Models.Grid.Grid grid = GridPesquisa();
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoStage repPedidoStage = new Repositorio.Embarcador.Pedidos.PedidoStage(unitOfWork);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);
                int totalRegistros = 0;

                var codigoCarga = (Request.GetIntParam("Carga"));

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaEntidades = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);
                string tipoDeCarga = repositorioCarga.BuscaTipoDeCargaPorCarga(codigoCarga);
                var cargapedio = repPedidos.BuscarPorCarga(codigoCarga);
                var listaCodigo = cargapedio.Select(x => x.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.Stage> stagesCarga = repPedidoStage.BuscarPorListaPedidos(listaCodigo);

                string etapa = string.Join(", ", stagesCarga.Select(x => x.NumeroStage?.TrimStart('0')));
                string remessa = cargapedio.Select(X => X.NumeroPedidoEmbarcador).FirstOrDefault();
                var cteMaisRecente = listaEntidades.SelectMany(x => x.CTEs).OrderByDescending(a => a.DataAutorizacao).FirstOrDefault();
                ParticipanteCTe enderecoInfo = new ParticipanteCTe();

                if (cteMaisRecente != null)
                {
                    enderecoInfo = cteMaisRecente.TipoTomador == TipoTomador.Remetente ? cteMaisRecente.Remetente : cteMaisRecente.Destinatario;
                }

                var lista = (from obj in listaEntidades
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = obj.Numero,
                                 obj.Numero,
                                 obj.Serie,
                                 Emitente = obj.Emitente?.Descricao,
                                 DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                 Destinatario = obj.Destinatario?.NomeCNPJ ?? string.Empty,
                                 TipoDeCarga = tipoDeCarga,
                                 Etapa = etapa,
                                 Remessa = remessa,
                                 NFE = $"{obj.Numero} - {obj.Serie}",
                                 RazaoSocial = enderecoInfo?.Nome,
                                 Endereco = enderecoInfo?.Endereco ?? "",
                                 Cidade = enderecoInfo?.Localidade?.Descricao ?? "",
                                 Estado = enderecoInfo?.Localidade?.Estado?.Sigla ?? "",
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        #endregion

        #region Métodos Privados
        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal filtroPesquisaXMLNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscal()
            {
                NumeroNotaFiscal = Request.GetIntParam("Numero"),
                Serie = Request.GetStringParam("Serie"),
                CodigoEmitente = Request.GetDoubleParam("Emitente"),
                DataEmissao = Request.GetDateTimeParam("DataEmissao"),
                Chave = Request.GetStringParam("Chave"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoCargaEntrega = Request.GetIntParam("CargaEntrega"),
                CodigoCliente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : Request.GetIntParam("Cliente")
            };

            List<XMLNotaFiscal> listaGrid = repXMLNotaFiscal.Consultar(filtroPesquisaXMLNotaFiscal, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repXMLNotaFiscal.ContarConsulta(filtroPesquisaXMLNotaFiscal);

            return listaGrid;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Descricao");
            grid.Prop("Numero").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Numero).Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Serie").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Serie).Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Emitente").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Emitente).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("DataEmissao").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Destinatario").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Destinatario).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("TipoDeCarga").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.TipoDeCarga).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Etapa");
            grid.Prop("Remessa");
            grid.Prop("NFE");
            grid.Prop("RazaoSocial");
            grid.Prop("Endereco");
            grid.Prop("Cidade");
            grid.Prop("Estado");

            return grid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Emitente") propOrdenar = "Emitente";
        }
        #endregion
    }
}
