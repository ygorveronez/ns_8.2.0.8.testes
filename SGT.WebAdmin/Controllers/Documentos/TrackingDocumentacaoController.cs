using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/TrackingDocumentacao")]
    public class TrackingDocumentacaoController : BaseController
    {
		#region Construtores

		public TrackingDocumentacaoController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaRegistro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro repTrackingRegistro = new Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro(unitOfWork);
                Repositorio.Embarcador.Documentos.TrackingDocumentacao repTracking = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);

                int codigoTracking = Request.GetIntParam("Codigo");

                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemNavio");
                int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
                int codigoPortoDestino = Request.GetIntParam("PortoDestino");

                DateTime dataGeracao = DateTime.Now;
                if (codigoTracking > 0)
                {
                    Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao tracking = repTracking.BuscarPorCodigo(codigoTracking);
                    dataGeracao = tracking.DataGeracao.HasValue ? tracking.DataGeracao.Value : DateTime.Now;
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao = Request.GetEnumParam("TipoTrackingDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao.Cabotagem);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO = Request.GetEnumParam("TipoIMO", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO.ApenasIMO);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao situacaoTrackingDocumentacao = Request.GetEnumParam("SituacaoTrackingDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao.SemRegistros);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("VVD", "VVD", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porto de Origem", "PortoOrigem", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Porto de Destino", "PortoDestino", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("IMO", "IMO", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data e Hora da Geração", "DataGeracao", 11, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Operador da Carga", "OperadorCarga", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("TipoMultimodal", false);
                grid.AdicionarCabecalho("CodigoVVD", false);
                grid.AdicionarCabecalho("CodigoPortoOrigem", false);
                grid.AdicionarCabecalho("CodigoPortoDestino", false);
                grid.AdicionarCabecalho("CargaPerigosa", false);
                grid.AdicionarCabecalho("CodigoOperadorCarga", false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.RegistrosTrackingDocumentacao> listaDocumentacao = repTrackingRegistro.BuscarRegistrosTrackingDocumentacao(false, codigoTracking, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, dataGeracao, situacaoTrackingDocumentacao, tipoTrackingDocumentacao, tipoIMO, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTrackingRegistro.ContarBuscarRegistrosTrackingDocumentacao(true, codigoTracking, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, dataGeracao, situacaoTrackingDocumentacao, tipoTrackingDocumentacao, tipoIMO, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite));

                var lista = (from p in listaDocumentacao
                             select new
                             {
                                 p.Codigo,
                                 p.TipoMultimodal,
                                 p.Tipo,
                                 p.CodigoVVD,
                                 p.VVD,
                                 p.CodigoPortoOrigem,
                                 p.PortoOrigem,
                                 p.CodigoPortoDestino,
                                 p.PortoDestino,
                                 p.CargaPerigosa,
                                 p.IMO,
                                 p.DataGeracao,
                                 p.CodigoOperadorCarga,
                                 p.OperadorCarga
                             }).ToList();

                grid.AdicionaRows(lista);
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
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Documentos.TrackingDocumentacao repTrackingDocumentacao = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao trackingDocumentacao = repTrackingDocumentacao.BuscarPorCodigo(codigo);

                // Valida
                if (trackingDocumentacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    trackingDocumentacao.Codigo,
                    trackingDocumentacao.Numero,
                    trackingDocumentacao.IntegracaoPendente,
                    PedidoViagemNavio = trackingDocumentacao.PedidoViagemNavio != null ? new { trackingDocumentacao.PedidoViagemNavio.Codigo, trackingDocumentacao.PedidoViagemNavio.Descricao } : null,
                    PortoOrigem = trackingDocumentacao.PortoOrigem != null ? new { trackingDocumentacao.PortoOrigem.Codigo, trackingDocumentacao.PortoOrigem.Descricao } : null,
                    PortoDestino = trackingDocumentacao.PortoDestino != null ? new { trackingDocumentacao.PortoDestino.Codigo, trackingDocumentacao.PortoDestino.Descricao } : null,
                    trackingDocumentacao.TipoTrackingDocumentacao,
                    trackingDocumentacao.TipoIMO,
                    trackingDocumentacao.SituacaoTrackingDocumentacao
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> GerarTracking()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Documentos.TrackingDocumentacao repTrackingDocumentacao = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);


                int codigoTracking = Request.GetIntParam("Codigo");
                // Busca informacoes
                Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao trackingDocumentacao = null;
                if (codigoTracking > 0)
                    trackingDocumentacao = repTrackingDocumentacao.BuscarPorCodigo(codigoTracking, true);
                else
                    trackingDocumentacao = new Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao();

                // Preenche entidade com dados
                bool contemDocumentacao = true;
                PreencheEntidade(ref trackingDocumentacao, (codigoTracking == 0), unitOfWork, ref contemDocumentacao);

                // Valida entidade
                if (!ValidaEntidade(trackingDocumentacao, out string erro, unitOfWork, contemDocumentacao))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                if (codigoTracking <= 0)
                {
                    trackingDocumentacao = repTrackingDocumentacao.BuscarPorCodigo(trackingDocumentacao.Codigo, true);
                    trackingDocumentacao.IntegracaoPendente = true;
                }

                repTrackingDocumentacao.Atualizar(trackingDocumentacao, Auditado);
                //else
                //    repTrackingDocumentacao.Inserir(trackingDocumentacao, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("TipoTrackingDocumentacao").Nome("Tipo").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("PedidoViagemNavio").Nome("VVD").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("PortoOrigem").Nome("Porto Origem").Tamanho(12).Align(Models.Grid.Align.left);
            grid.Prop("DataGeracao").Nome("Data Geração").Tamanho(10).Align(Models.Grid.Align.center);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Documentos.TrackingDocumentacao repTrackingDocumentacao = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);

            // Dados do filtro
            int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemNavio");
            int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
            int codigoPortoDestino = Request.GetIntParam("PortoDestino");

            DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");
            DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao tipoTrackingDocumentacao = Request.GetEnumParam("TipoTrackingDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao.Cabotagem);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO tipoIMO = Request.GetEnumParam("TipoIMO", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO.ApenasIMO);

            // Consulta
            List<Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao> listaGrid = repTrackingDocumentacao.Consultar(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, dataInicial, dataFinal, tipoTrackingDocumentacao, tipoIMO, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTrackingDocumentacao.ContarConsulta(codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, dataInicial, dataFinal, tipoTrackingDocumentacao, tipoIMO);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            TipoTrackingDocumentacao = obj.DescricaoTipoTrackingDocumentacao,
                            PedidoViagemNavio = obj.PedidoViagemNavio?.Descricao ?? "",
                            PortoOrigem = obj.PortoOrigem?.Descricao ?? "",
                            DataGeracao = obj.DataGeracao.Value.ToString("dd/MM/yyyy HH:mm")
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao trackingDocumentacao, bool inserindo, Repositorio.UnitOfWork unitOfWork, ref bool contemDocumentacao)
        {

            Repositorio.Embarcador.Documentos.TrackingDocumentacao repTrackingDocumentacao = new Repositorio.Embarcador.Documentos.TrackingDocumentacao(unitOfWork);
            Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro repTrackingRegistro = new Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro(unitOfWork);
            Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga repTrackingCarga = new Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Pedidos.Porto repPorto = new Repositorio.Embarcador.Pedidos.Porto(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemNavio");
            int codigoPortoOrigem = Request.GetIntParam("PortoOrigem");
            int codigoPortoDestino = Request.GetIntParam("PortoDestino");
            int codigoTracking = Request.GetIntParam("Codigo");

            // Vincula dados
            if (inserindo)
            {
                contemDocumentacao = false;
                trackingDocumentacao.Numero = repTrackingDocumentacao.BuscarProximoNumero();
                trackingDocumentacao.IntegracaoPendente = false;
                trackingDocumentacao.DataGeracao = DateTime.Now;
                trackingDocumentacao.Usuario = this.Usuario;
            }
            else
                contemDocumentacao = true;
            trackingDocumentacao.TipoTrackingDocumentacao = Request.GetEnumParam("TipoTrackingDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTrackingDocumentacao.Cabotagem); ;
            trackingDocumentacao.TipoIMO = Request.GetEnumParam("TipoIMO", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIMO.ApenasIMO); ;
            trackingDocumentacao.SituacaoTrackingDocumentacao = Request.GetEnumParam("SituacaoTrackingDocumentacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTrackingDocumentacao.SemRegistros); ;
            trackingDocumentacao.PedidoViagemNavio = codigoPedidoViagemDirecao > 0 ? repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemDirecao) : null;
            trackingDocumentacao.PortoOrigem = codigoPortoOrigem > 0 ? repPorto.BuscarPorCodigo(codigoPortoOrigem) : null;
            trackingDocumentacao.PortoDestino = codigoPortoDestino > 0 ? repPorto.BuscarPorCodigo(codigoPortoDestino) : null;

            if (inserindo)
            {
                repTrackingDocumentacao.Inserir(trackingDocumentacao);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.RegistrosTrackingDocumentacao> listaDocumentacao = repTrackingRegistro.BuscarRegistrosTrackingDocumentacao(true, codigoTracking, codigoPedidoViagemDirecao, codigoPortoOrigem, codigoPortoDestino, trackingDocumentacao.DataGeracao.Value, trackingDocumentacao.SituacaoTrackingDocumentacao, trackingDocumentacao.TipoTrackingDocumentacao, trackingDocumentacao.TipoIMO, "", "", 0, 0);
                foreach (var documentacao in listaDocumentacao)
                {
                    Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro documentacaoRegistro = new Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistro()
                    {
                        CargaIMO = documentacao.CargaPerigosa,
                        DataGeracao = trackingDocumentacao.DataGeracao,
                        OperadorCarga = repUsuario.BuscarPorCodigo(documentacao.CodigoOperadorCarga),
                        PedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(documentacao.CodigoVVD),
                        PortoDestino = repPorto.BuscarPorCodigo(documentacao.CodigoPortoDestino),
                        PortoOrigem = repPorto.BuscarPorCodigo(documentacao.CodigoPortoOrigem),
                        TipoTrackingDocumentacao = trackingDocumentacao.TipoTrackingDocumentacao,
                        TrackingDocumentacao = trackingDocumentacao
                    };
                    repTrackingRegistro.Inserir(documentacaoRegistro);
                    List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repTrackingCarga.BuscarCargas(documentacao.CargaPerigosa, documentacao.CodigoOperadorCarga, documentacao.CodigoVVD, documentacao.CodigoPortoDestino, documentacao.CodigoPortoOrigem, trackingDocumentacao.TipoTrackingDocumentacao, trackingDocumentacao.SituacaoTrackingDocumentacao);
                    if (cargas != null && cargas.Count > 0)
                    {
                        contemDocumentacao = true;
                        foreach (var carga in cargas)
                        {
                            Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga registroCarga = new Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacaoRegistroCarga()
                            {
                                Carga = carga,
                                TrackingDocumentacaoRegistro = documentacaoRegistro
                            };
                            repTrackingCarga.Inserir(registroCarga);
                        }
                    }
                }
            }
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Documentos.TrackingDocumentacao trackingDocumentacao, out string msgErro, Repositorio.UnitOfWork unitOfWork, bool contemDocumentacao)
        {
            Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro repDocumentacaoRegistro = new Repositorio.Embarcador.Documentos.TrackingDocumentacaoRegistro(unitOfWork);
            msgErro = "";

            if (trackingDocumentacao.PedidoViagemNavio == null)
            {
                msgErro = "VVD é obrigatório.";
                return false;
            }

            if (!contemDocumentacao)
            {
                msgErro = "É obrigatório que tenha ao menus uma documentação antes de salvar.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
        }
        #endregion
    }
}
