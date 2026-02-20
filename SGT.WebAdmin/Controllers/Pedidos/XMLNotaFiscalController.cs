using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/XMLNotaFiscal")]
    public class XMLNotaFiscalController : BaseController
    {
		#region Construtores

		public XMLNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaEntidades = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                string tipoDeCarga = repositorioCarga.BuscaTipoDeCargaPorCarga(Request.GetIntParam("Carga"));

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
                             }).ToList();

                // Seta valores na grid
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMontagemContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = GridPesquisaMontagemContainer();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                Repositorio.Embarcador.WMS.RecebimentoMercadoria repositorioRecebimentoMercadoria = new Repositorio.Embarcador.WMS.RecebimentoMercadoria(unitOfWork);

                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaEntidades = ExecutaPesquisaMontagemContainer(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);
                List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria> listaRecebimentoMercadoria = listaEntidades.Count > 0 ? repositorioRecebimentoMercadoria.BuscarPorXMLNotasFiscais(listaEntidades.Select(x => x.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria>();

                var lista = (from o in listaEntidades
                             select new
                             {
                                 o.Codigo,
                                 o.Numero,
                                 o.Chave,
                                 Emissor = o.Emitente?.Nome,
                                 CNPJ = o.Emitente?.CPF_CNPJ_Formatado,
                                 Quantidade = o.Volumes,
                                 Tipo = o.TipoDeCarga,
                                 ProdutoEmbarcador = o.Produto,
                                 MetroCubico = o.MetrosCubicos.ToString("n2"),
                                 PesoNota = o.Peso.ToString("n2"),
                                 DataRecebimentoWMS = (from r in listaRecebimentoMercadoria where r.XMLNotaFiscal.Codigo == o.Codigo && r.Recebimento != null select r.Recebimento.Data.ToString("dd/MM/yyyy HH:mm")).FirstOrDefault(),
                                 Serie = o.Serie,
                                 DataEmissao = o.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                 ValorNota = o.Valor.ToString("n2"),
                                 ValorProdutos = o.ValorTotalProdutos.ToString("n2"),
                                 Medidas = $"{o.Comprimento.ToString("n2")} x {o.Largura.ToString("n2")} x {o.Altura.ToString("n2")}"
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasComplemento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaNotasComplemento();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaEntidades = ExecutaPesquisaNotasComplemento(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (from obj in listaEntidades
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = obj.Numero,
                                 obj.Numero,
                                 obj.Serie,
                                 Emitente = obj.Emitente?.Descricao,
                                 Destinatario = obj.Destinatario?.Descricao,
                                 Valor = obj.Valor,
                                 Peso = obj.Peso,
                                 CTe = obj.CTEs?.FirstOrDefault()?.Codigo,
                                 DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy")
                             }).ToList();

                // Seta valores na grid
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaNotasFiscaisSaida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaNotasSaida();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida filtroPesquisa = OberFiltroPesquisaNotaFiscalSaida();

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaEntidades = ExecutaPesquisaNotasSaida(filtroPesquisa, ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (from obj in listaEntidades
                             select new
                             {
                                 obj.Codigo,
                                 obj.Numero,
                                 obj.Serie,
                                 DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                 Emitente = obj.Emitente?.NomeCNPJ ?? string.Empty,
                                 Destinatario = obj.Destinatario?.NomeCNPJ ?? string.Empty,
                                 CNPJEmitente = obj.Emitente?.CPF_CNPJ,
                                 Valor = obj.Valor.ToString("n2"),
                                 Chave = obj.Chave ?? string.Empty,
                             }).ToList();

                // Seta valores na grid
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

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao");
            grid.Prop("Numero").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Numero).Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Serie").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Serie).Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Emitente").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Emitente).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("DataEmissao").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("Destinatario").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Destinatario).Tamanho(15).Align(Models.Grid.Align.center);
            grid.Prop("TipoDeCarga").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.TipoDeCarga).Tamanho(15).Align(Models.Grid.Align.center);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaMontagemContainer()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Chave");
            grid.Prop("CNPJ");
            grid.Prop("Quantidade");
            grid.Prop("Tipo");
            grid.Prop("ProdutoEmbarcador");
            grid.Prop("ValorNota");
            grid.Prop("ValorProdutos");
            grid.Prop("PesoNota");
            grid.Prop("Medidas");
            grid.Prop("MetroCubico");
            grid.Prop("DataRecebimentoWMS");
            grid.Prop("Numero").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Numero).Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Serie").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Serie).Tamanho(10).Align(Models.Grid.Align.right);
            grid.Prop("Emissor").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Emitente).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("DataEmissao").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao).Tamanho(15).Align(Models.Grid.Align.center);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaNotasSaida()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("CNPJEmitente");
            grid.Prop("Numero").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Numero).Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Serie").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Serie).Tamanho(5).Align(Models.Grid.Align.center);
            grid.Prop("Chave").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Chave).Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("DataEmissao").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao).Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Emitente").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Remetente).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Destinatario).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Valor").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Valor).Tamanho(20).Align(Models.Grid.Align.left);

            return grid;
        }

        private Models.Grid.Grid GridPesquisaNotasComplemento()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Descricao");
            grid.Prop("CTe");
            grid.Prop("Numero").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Numero).Tamanho(10).Align(Models.Grid.Align.center);
            grid.Prop("Serie").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Serie).Tamanho(5).Align(Models.Grid.Align.center);
            grid.Prop("Emitente").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Emitente).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Destinatario).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Valor").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Valor).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("Peso").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.Peso).Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("DataEmissao").Nome(Localization.Resources.Consultas.NotaFiscalEletronica.DataEmissao).Tamanho(10).Align(Models.Grid.Align.center);

            return grid;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ExecutaPesquisaMontagemContainer(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            // Dados do filtro
            int numero = Request.GetIntParam("Numero");
            string serie = Request.GetStringParam("Serie");
            double emitente = Request.GetDoubleParam("Emitente");
            DateTime dataEmissao = Request.GetDateTimeParam("DataEmissao");
            string chave = Request.GetStringParam("Chave");
            int codigoCarga = Request.GetIntParam("Carga");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaGrid = repXMLNotaFiscal.ConsultarMontagemContainer(numero, serie, emitente, dataEmissao, chave, codigoCarga, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repXMLNotaFiscal.ContarConsultaMontagemContainer(numero, serie, emitente, dataEmissao, chave, codigoCarga);

            return listaGrid;
        }

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
                CodigoCliente = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? 0 : Request.GetDoubleParam("Cliente")
            };

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaGrid = repXMLNotaFiscal.Consultar(filtroPesquisaXMLNotaFiscal, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repXMLNotaFiscal.ContarConsulta(filtroPesquisaXMLNotaFiscal);

            return listaGrid;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ExecutaPesquisaNotasComplemento(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            // Dados do filtro
            int codigoCTe = Request.GetIntParam("CTe");
            double codigoDestinatario = Request.GetDoubleParam("Destinatario");

            // Consulta
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaGrid = repXMLNotaFiscal.ConsultarNotasComplementar(codigoCTe, codigoDestinatario, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repXMLNotaFiscal.ContarConsultaNotasComplementar(codigoCTe, codigoDestinatario);

            return listaGrid;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ExecutaPesquisaNotasSaida(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida filtroPesquisa, ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            // Dados do filtro

            // Consulta
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaGrid = repXMLNotaFiscal.ConsultarNotasSaida(filtroPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repXMLNotaFiscal.ContarConsultarNotasSaida(filtroPesquisa);

            return listaGrid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Emitente") propOrdenar = "Emitente.Nome";
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida OberFiltroPesquisaNotaFiscalSaida()
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaXMLNotaFiscalSaida()
            {
                TipoOperacaoNotaFiscal = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal>("TipoEmissao"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                Chave = Request.GetStringParam("Chave"),
                Serie = Request.GetStringParam("Serie"),
                CodigoRemetente = Request.GetDoubleParam("Remetente"),
                CodigoDestinatario = Request.GetDoubleParam("Destinatario")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtroPesquisa.Empresa = this.Empresa.Codigo;

            return filtroPesquisa;
        }
        #endregion
    }
}
