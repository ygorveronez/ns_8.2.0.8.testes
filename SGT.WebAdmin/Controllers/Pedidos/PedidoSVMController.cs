using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/PedidoSVM", "Pessoas/Usuario")]
    public class PedidoSVMController : BaseController
    {
		#region Construtores

		public PedidoSVMController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

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
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                // Valida
                if (pedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    pedido.Codigo,
                    NumeroPedido = pedido.Numero,
                    DataColeta = pedido.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    PedidoViagemNavio = pedido.PedidoViagemNavio != null ? new { Codigo = pedido.PedidoViagemNavio != null ? pedido.PedidoViagemNavio.Codigo : 0, Descricao = pedido.PedidoViagemNavio != null ? pedido.PedidoViagemNavio.Descricao : "" } : null,
                    TerminalOrigem = pedido.TerminalOrigem != null ? new { Codigo = pedido.TerminalOrigem != null ? pedido.TerminalOrigem.Codigo : 0, Descricao = pedido.TerminalOrigem != null ? pedido.TerminalOrigem.Descricao : "" } : null,
                    TerminalDestino = pedido.TerminalDestino != null ? new { Codigo = pedido.TerminalDestino != null ? pedido.TerminalDestino.Codigo : 0, Descricao = pedido.TerminalDestino != null ? pedido.TerminalDestino.Descricao : "" } : null,
                    pedido.Observacao,
                    SomenteCargaPerigosa = pedido.PossuiCargaPerigosa
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Log.TratarErro("1 - Iniciou", "PedidoSVM");
                // Inicia transacao
                //unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Busca informacoes
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                List<string> listaCargasSemContainer = new List<string>();
                // Preenche entidade com dados
                string retorno = "", retornoBookings = "";
                if (!PreencheEntidade(out retorno, ref pedidos, unitOfWork, out listaCargasSemContainer))
                {
                    if (listaCargasSemContainer != null && listaCargasSemContainer.Count > 0)
                    {
                        foreach (var numeroCarga in listaCargasSemContainer)
                        {
                            retorno += "Carga: " + numeroCarga + " -> A carga não possui container cadastrado.<br/>";
                            serNotificacao.GerarNotificacao(this.Usuario, 0, "Pedidos/PedidoSVM", string.Format(Localization.Resources.Pedidos.PedidoSVM.CargaNaoPossuiContainer, numeroCarga), IconesNotificacao.agConfirmacao, TipoNotificacao.todas, TipoServicoMultisoftware, unitOfWork);
                        }
                    }
                    return new JsonpResult(false, true, retorno);
                }
                else if (!string.IsNullOrEmpty(retorno))
                {
                    retornoBookings = retorno;
                    retorno = "";
                }

                //unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno) && pedidos != null && pedidos.Count > 0)
                {
                    int codigoUsuario = this.Usuario.Codigo;
                    string stringConexao = _conexao.StringConexao;
                    AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware = TipoServicoMultisoftware;
                    List<int> codigosPedidos = pedidos.Select(o => o.Codigo).Distinct().ToList();

                    Task.Run(() => Servicos.Embarcador.Pedido.Pedido.CriarCargaSVM(codigosPedidos, stringConexao, tipoServicoMultisoftware, Cliente, codigoUsuario));
                }
                else
                {
                    unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    if (listaCargasSemContainer != null && listaCargasSemContainer.Count > 0)
                    {
                        foreach (var numeroCarga in listaCargasSemContainer)
                        {
                            retorno += "Carga: " + numeroCarga + " -> A carga não possui container cadastrado.<br/>";
                            serNotificacao.GerarNotificacao(this.Usuario, 0, "Pedidos/PedidoSVM", string.Format(Localization.Resources.Pedidos.PedidoSVM.CargaNaoPossuiContainer, numeroCarga), IconesNotificacao.agConfirmacao, TipoNotificacao.todas, TipoServicoMultisoftware, unitOfWork);
                        }
                    }
                    throw new ControllerException(retorno);
                }

                if (listaCargasSemContainer != null && listaCargasSemContainer.Count > 0)
                {
                    unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                    foreach (var numeroCarga in listaCargasSemContainer)
                    {
                        retornoBookings += "Carga: " + numeroCarga + " -> A carga não possui container cadastrado.<br/>";
                        serNotificacao.GerarNotificacao(this.Usuario, 0, "Pedidos/PedidoSVM", string.Format(Localization.Resources.Pedidos.PedidoSVM.CargaNaoPossuiContainer, numeroCarga), IconesNotificacao.agConfirmacao, TipoNotificacao.todas, TipoServicoMultisoftware, unitOfWork);
                    }
                }

                if (!string.IsNullOrWhiteSpace(retornoBookings))
                    return new JsonpResult(false, true, retornoBookings);
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "PedidoSVM");
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(false, true, "O pedido está vinculado à uma carga que não está na etapa 1-Carga, não sendo possível alterar os dados do mesmo.");
                //// Inicia transacao
                //unitOfWork.Start();

                //// Instancia repositorios
                //Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                //// Parametros
                //int.TryParse(Request.Params("Codigo"), out int codigo);

                //// Busca informacoes
                //Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                //// Valida
                //if (pedido == null)
                //    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                //// Preenche entidade com dados
                //PreencheEntidade(ref pedido, unitOfWork);

                //// Valida entidade
                //if (!ValidaEntidade(pedido, out string erro))
                //    return new JsonpResult(false, true, erro);

                //// Persiste dados
                //repPedido.Atualizar(pedido);
                //unitOfWork.CommitChanges();

                //// Retorna sucesso
                //return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(false, true, "O pedido já está vinculado à uma carga, não sendo possível realizar a exclusão do mesmo.");
                //// Inicia transacao
                //unitOfWork.Start();

                //// Instancia repositorios
                //Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                //// Parametros
                //int.TryParse(Request.Params("Codigo"), out int codigo);

                //// Busca informacoes
                //Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                //// Valida
                //if (pedido == null)
                //    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                //// Persiste dados
                //repPedido.Deletar(pedido, Auditado);
                //unitOfWork.CommitChanges();

                //// Retorna informacoes
                //return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
            grid.Prop("Numero").Nome("Número").Tamanho(10).Align(Models.Grid.Align.left);
            grid.Prop("PedidoViagemNavio").Nome("Viagem").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("TerminalOrigem").Nome("Ter. Origem").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("TerminalDestino").Nome("Ter. Destino").Tamanho(20).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("NumeroPedido"), out int numero);
            int.TryParse(Request.Params("PedidoViagemNavio"), out int codigoPedidoViagemNavio);
            int.TryParse(Request.Params("TerminalOrigem"), out int codigoTerminalOrigem);
            int.TryParse(Request.Params("TerminalDestino"), out int codigoTerminalDestino);

            // Consulta
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaGrid = repPedido.BuscarPedidosSVM(numero, codigoPedidoViagemNavio, codigoTerminalOrigem, codigoTerminalDestino, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPedido.ContarBuscarPedidosSVM(numero, codigoPedidoViagemNavio, codigoTerminalOrigem, codigoTerminalDestino);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Numero = obj.Numero.ToString("D"),
                            PedidoViagemNavio = obj.PedidoViagemNavio?.Descricao ?? "",
                            TerminalOrigem = obj.TerminalOrigem?.Descricao ?? "",
                            TerminalDestino = obj.TerminalDestino?.Descricao ?? ""
                        };
            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private bool PreencheEntidade(out string msgErro, ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork, out List<string> listaCargasSemContainer)
        {
            Servicos.Log.TratarErro("2 - Preenchendo", "PedidoSVM");
            msgErro = "";
            listaCargasSemContainer = new List<string>();
            Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTipoTerminalImportacao = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador reposotorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = reposotorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            string observacao = Request.Params("Observacao") ?? string.Empty;

            bool.TryParse(Request.Params("SomenteCargaPerigosa"), out bool somenteCargaPerigosa);

            int.TryParse(Request.Params("PedidoViagemNavio"), out int codigoPedidoViagemNavio);
            int.TryParse(Request.Params("TerminalOrigem"), out int codigoTerminalOrigem);
            int.TryParse(Request.Params("TerminalDestino"), out int codigoTerminalDestino);

            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemNavio);
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem = repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalOrigem);
            List<int> terminaisDestino = new List<int>();

            if (codigoTerminalDestino > 0)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalDestino);
                terminaisDestino.Add(terminalDestino.Codigo);
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminaisDestinoCTe = repCargaCTe.ConsultarTerminaisDestino(codigoPedidoViagemNavio, codigoTerminalOrigem, situacoesPermitidas);
                List<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao> terminaisDestinoTransbordo = repCargaCTe.ConsultarTerminalDestinoTransbordo(codigoPedidoViagemNavio, situacoesPermitidas);

                terminaisDestino.AddRange(terminaisDestinoCTe.Select(o => o.Codigo).ToList());
                terminaisDestino.AddRange(terminaisDestinoTransbordo.Select(o => o.Codigo).ToList());
            }

            if (terminaisDestino == null || terminaisDestino.Count == 0)
            {
                msgErro = "Não foi localizado nenhum terminal de destino para a geração da carga de SVM.";
                return false;
            }
            else
                terminaisDestino = terminaisDestino.Distinct().ToList();

            if (configuracaoTransportador.AtivarControleCarregamentoNavio)
            {
                if (pedidoViagemNavio.ConsumoPlugs > 0 && pedidoViagemNavio.Navio.CapacidadePlug > 0 && pedidoViagemNavio.ConsumoPlugs > pedidoViagemNavio.Navio.CapacidadePlug)
                    msgErro += "Capacidade de PLUGS do navio (" + pedidoViagemNavio.Navio.CapacidadePlug.ToString("n4") + ") excedida para esta viagem (" + pedidoViagemNavio.ConsumoPlugs.ToString("n4") + ").<br/>";
                if (pedidoViagemNavio.ConsumoTeus > 0 && pedidoViagemNavio.Navio.CapacidadeTeus > 0 && pedidoViagemNavio.ConsumoTeus > pedidoViagemNavio.Navio.CapacidadeTeus)
                    msgErro += "Capacidade de TEUS do navio (" + pedidoViagemNavio.Navio.CapacidadeTeus.ToString("n4") + ") excedida para esta viagem (" + pedidoViagemNavio.ConsumoTeus.ToString("n4") + ").<br/>";
                if (pedidoViagemNavio.ConsumoTons > 0 && pedidoViagemNavio.Navio.CapacidadeTons > 0 && pedidoViagemNavio.ConsumoTons > pedidoViagemNavio.Navio.CapacidadeTons)
                    msgErro += "Capacidade de TONS do navio (" + pedidoViagemNavio.Navio.CapacidadeTons.ToString("n4") + ") excedida para esta viagem (" + pedidoViagemNavio.ConsumoTons.ToString("n4") + ").<br/>";

                if (!string.IsNullOrWhiteSpace(msgErro))
                    return false;
            }

            Dominio.Entidades.Cliente remetente = null;
            Dominio.Entidades.Cliente destinatario = null;
            Dominio.Entidades.Cliente recebedor = null;
            Dominio.Entidades.Cliente expedidor = null;

            if (terminalOrigem != null && terminalOrigem.Empresa != null && terminalOrigem.Terminal != null)
            {
                remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(terminalOrigem.Empresa.CNPJ));
                expedidor = repCliente.BuscarPorCPFCNPJ(terminalOrigem.Terminal.CPF_CNPJ);
            }
            //foreach (var terminalDestino in terminaisDestino)
            for (int t = 0; t < terminaisDestino.Count; t++)
            {
                Servicos.Log.TratarErro("3 - For Terminais Destino", "PedidoSVM");
                Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino = repTipoTerminalImportacao.BuscarPorCodigo(terminaisDestino[t]);
                terminalOrigem = repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalOrigem);
                pedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemNavio);
                bool gerarUmaCargaSVMPorCargaMTLQuandoPortoDestino = terminalDestino?.Porto?.GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino ?? false;
                if (!gerarUmaCargaSVMPorCargaMTLQuandoPortoDestino)
                    gerarUmaCargaSVMPorCargaMTLQuandoPortoDestino = terminalOrigem?.Porto?.GerarUmaCargaSVMPorCargaMTLQuandoPortoDestino ?? false;

                if (terminalDestino != null && terminalDestino.Empresa != null && terminalDestino.Terminal != null)
                {
                    destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(terminalDestino.Empresa.CNPJ));
                    recebedor = repCliente.BuscarPorCPFCNPJ(terminalDestino.Terminal.CPF_CNPJ);
                }
                List<string> listaBookings = RetornaListaNumeroBooking(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, unitOfWork);
                List<string> listaBookingsCargasPendentes = RetornaListaNumeroBookingCargasPendentes(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.Nenhum, "", unitOfWork);
                if (listaBookingsCargasPendentes != null && listaBookingsCargasPendentes.Count > 0)
                {
                    listaBookings.AddRange(listaBookingsCargasPendentes);
                    listaBookings = listaBookings.Distinct().ToList();
                }
                listaCargasSemContainer = RetornaListaNumeroCargaSemContainer(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, unitOfWork);

                List<string> listaBookingsErros = new List<string>();
                int qtdBookings = 0;
                //foreach (var numeroBooking in listaBookings)
                for (int i = 0; i < listaBookings.Count; i++)
                {
                    Servicos.Log.TratarErro("4 - For booking count" + listaBookings.Count.ToString(), "PedidoSVM");

                    int contadorCargasSVM = 1;
                    List<int> codigosCargas = new List<int>();

                    if (gerarUmaCargaSVMPorCargaMTLQuandoPortoDestino)
                        codigosCargas.AddRange(RetornaListaCodigosCargasPendentes(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, listaBookings[i], unitOfWork));
                    else
                        codigosCargas.Add(0);

                    for (int j = 0; j < codigosCargas.Count; j++)
                    {
                        terminalOrigem = repTipoTerminalImportacao.BuscarPorCodigo(codigoTerminalOrigem);
                        terminalDestino = repTipoTerminalImportacao.BuscarPorCodigo(terminaisDestino[t]);
                        pedidoViagemNavio = repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemNavio);

                        if (terminalDestino != null && terminalDestino.Empresa != null && terminalDestino.Terminal != null)
                        {
                            destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(terminalDestino.Empresa.CNPJ));
                            recebedor = repCliente.BuscarPorCPFCNPJ(terminalDestino.Terminal.CPF_CNPJ);
                        }

                        string numeroBooking = listaBookings[i];
                        if (listaBookingsErros.Contains(numeroBooking))
                            continue;

                        if (ContemCargaPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, null, numeroBooking, unitOfWork))
                        {
                            listaBookingsErros.Add(numeroBooking);
                            msgErro += "BK: " + numeroBooking + " -> Existem cargas pendêntes de emissão para esta geração de SVM, favor verifique no relatório de cargas/conhecimento.<br/>";

                            continue;
                            //return false;
                        }

                        Servicos.Log.TratarErro("5 - ContemCargaPendentesAutorizacao " + numeroBooking, "PedidoSVM");

                        // Vincula dados
                        if (BookingPendenteDeEmissaoDeSVM(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork) && ContemDocumentosParaEmissao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork, codigosCargas[j]))
                        {
                            Servicos.Log.TratarErro("6 - BookingPendenteDeEmissaoDeSVM " + numeroBooking, "PedidoSVM");
                            if (ContemEntidadeSemCadastro(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork))
                            {
                                listaBookingsErros.Add(numeroBooking);
                                msgErro += "BK: " + numeroBooking + " -> Existe entidades (Remetente/Destinatário) sem seu código de integração informado, favor verifique no relatório de Pessoa.<br/>";
                                string entidades = RetornaDescricaoEntidadeSemCadastro(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork);
                                msgErro += entidades;

                                continue;
                            }
                            Servicos.Log.TratarErro("7 - ContemEntidadeSemCadastro " + numeroBooking, "PedidoSVM");
                            if (ContemConhecimentoPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork))
                            {
                                listaBookingsErros.Add(numeroBooking);
                                msgErro += "BK: " + numeroBooking + " -> Existem conhecimentos rejeitados e/ou em digitação para esta geração de SVM, favor verifique no relatório de cargas/conhecimento.<br/>";
                                string entidades = RetornarConhecimentoPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork);
                                msgErro += entidades;

                                continue;
                            }
                            Servicos.Log.TratarErro("8 - ContemConhecimentoPendentesAutorizacao " + numeroBooking, "PedidoSVM");
                            if (ContemCargaPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork))
                            {
                                listaBookingsErros.Add(numeroBooking);
                                msgErro += "BK: " + numeroBooking + " -> Existem cargas/conhecimentos pendêntes de emissão para esta geração de SVM, favor verifique no relatório de cargas/conhecimento.<br/>";
                                string entidades = RetornarConhecimentoPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, numeroBooking, unitOfWork);
                                msgErro += entidades;

                                continue;
                            }
                            Servicos.Log.TratarErro("9 - ContemCargaPendentesAutorizacao " + numeroBooking, "PedidoSVM");
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoMultimodal = RetornaPrimeiroPedidoMultimodal(numeroBooking, somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, unitOfWork);
                            unitOfWork.Start();
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = PreecherDadosPedido(somenteCargaPerigosa, numeroBooking, terminalOrigem, terminalDestino, remetente, destinatario, recebedor, expedidor, pedidoViagemNavio, observacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFracionada, unitOfWork, pedidoMultimodal?.PossuiCargaPerigosa ?? false, pedidoMultimodal?.IMOClasse ?? "", pedidoMultimodal?.IMOSequencia ?? "", pedidoMultimodal?.IMOUnidade ?? "", codigosCargas[j]);
                            PreencherCodigoCargaEmbarcador(unitOfWork, ref pedido, codigosCargas[j] > 0, contadorCargasSVM);
                            //if (!repPedido.ContemPedidoMesmoNumeroCarga(pedido.CodigoCargaEmbarcador, pedido.DataCadastro.Value))
                            //{
                            qtdBookings += 1;
                            if (!ValidaEntidade(pedido, out string erro))
                            {
                                msgErro = erro;
                                return false;
                            }
                            pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                            repPedido.Inserir(pedido, Auditado);

                            pedidos.Add(pedido);
                            //}
                            Servicos.Log.TratarErro("10 - Gerou Pedido" + numeroBooking, "PedidoSVM");
                            unitOfWork.CommitChanges();
                        }
                        else if (BookingPendenteDeEmissaoDeSVM(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork) && ContemDocumentosParaEmissao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork, codigosCargas[j]))
                        {
                            Servicos.Log.TratarErro("6 - BookingPendenteDeEmissaoDeSVM " + numeroBooking, "PedidoSVM");
                            if (ContemEntidadeSemCadastro(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork))
                            {
                                listaBookingsErros.Add(numeroBooking);
                                msgErro += "BK: " + numeroBooking + " -> Existe entidades (Remetente/Destinatário) sem seu código de integração informado, favor verifique no relatório de Pessoa.<br/>";
                                string entidades = RetornaDescricaoEntidadeSemCadastro(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork);
                                msgErro += entidades;

                                continue;
                            }
                            Servicos.Log.TratarErro("7 - ContemEntidadeSemCadastro " + numeroBooking, "PedidoSVM");
                            if (ContemConhecimentoPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork))
                            {
                                listaBookingsErros.Add(numeroBooking);
                                msgErro += "BK: " + numeroBooking + " -> Existem conhecimentos rejeitados e/ou em digitação para esta geração de SVM, favor verifique no relatório de cargas/conhecimento.<br/>";
                                string entidades = RetornarConhecimentoPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork);
                                msgErro += entidades;

                                continue;
                            }
                            Servicos.Log.TratarErro("8 - ContemConhecimentoPendentesAutorizacao " + numeroBooking, "PedidoSVM");
                            if (ContemCargaPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork))
                            {
                                listaBookingsErros.Add(numeroBooking);
                                msgErro += "BK: " + numeroBooking + " -> Existem cargas/conhecimentos pendêntes de emissão para esta geração de SVM, favor verifique no relatório de cargas/conhecimento.<br/>";
                                string entidades = RetornarConhecimentoPendentesAutorizacao(somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, numeroBooking, unitOfWork);
                                msgErro += entidades;

                                continue;
                            }
                            Servicos.Log.TratarErro("9 - ContemCargaPendentesAutorizacao " + numeroBooking, "PedidoSVM");
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoMultimodal = RetornaPrimeiroPedidoMultimodal(numeroBooking, somenteCargaPerigosa, terminalOrigem, terminalDestino, pedidoViagemNavio, unitOfWork);
                            unitOfWork.Start();
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = PreecherDadosPedido(somenteCargaPerigosa, numeroBooking, terminalOrigem, terminalDestino, remetente, destinatario, recebedor, expedidor, pedidoViagemNavio, observacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal.CargaFechada, unitOfWork, pedidoMultimodal?.PossuiCargaPerigosa ?? false, pedidoMultimodal?.IMOClasse ?? "", pedidoMultimodal?.IMOSequencia ?? "", pedidoMultimodal?.IMOUnidade ?? "", codigosCargas[j]);
                            PreencherCodigoCargaEmbarcador(unitOfWork, ref pedido, codigosCargas[j] > 0, contadorCargasSVM);

                            //if (!repPedido.ContemPedidoMesmoNumeroCarga(pedido.CodigoCargaEmbarcador, pedido.DataCadastro.Value))
                            //{
                            qtdBookings += 1;
                            if (!ValidaEntidade(pedido, out string erro))
                            {
                                msgErro = erro;
                                return false;
                            }
                            pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;
                            repPedido.Inserir(pedido, Auditado);

                            pedidos.Add(pedido);
                            //}
                            Servicos.Log.TratarErro("10 - Gerou Pedido" + numeroBooking, "PedidoSVM");
                            unitOfWork.CommitChanges();
                        }

                        if (codigosCargas[j] > 0)
                            contadorCargasSVM++;

                        Servicos.Log.TratarErro("11 - Flush", "PedidoSVM");
                        unitOfWork.FlushAndClear();
                    }
                }
            }
            if (pedidos != null && pedidos.Count > 0)
                return true;
            else
            {
                msgErro = "Não foi encontrado nenhum Multimodal disponível para a geração deste SVM. <br/>" + msgErro;
                return false;
            }
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, out string msgErro)
        {
            msgErro = "";

            if (pedido.Origem == null)
            {
                msgErro = "O terminal de origem " + (pedido.TerminalOrigem?.Descricao ?? "") + " não possui a terminal vinculado.";
                return false;
            }
            if (pedido.Empresa == null)
            {
                msgErro = "O terminal de origem " + (pedido.TerminalOrigem?.Descricao ?? "") + " não possui a empresa vinculada.";
                return false;
            }
            if (pedido.Remetente == null)
            {
                msgErro = "O terminal de origem " + (pedido.TerminalOrigem?.Descricao ?? "") + " não possui a empresa vinculada.";
                return false;
            }
            if (pedido.Expedidor == null)
            {
                msgErro = "O terminal de origem " + (pedido.TerminalOrigem?.Descricao ?? "") + " não possui a empresa vinculada.";
                return false;
            }
            if (pedido.Porto == null)
            {
                msgErro = "O terminal de origem " + (pedido.TerminalOrigem?.Descricao ?? "") + " não possui o porto vinculado.";
                return false;
            }
            if (pedido.TerminalOrigem == null)
            {
                msgErro = "O terminal de origem não foi informado.";
                return false;
            }
            if (pedido.PedidoViagemNavio == null)
            {
                msgErro = "A viagem/navio/diração não foi informada.";
                return false;
            }
            if (pedido.TerminalDestino != null)
            {
                if (pedido.Destino == null)
                {
                    msgErro = "O terminal de destino " + (pedido.TerminalDestino?.Descricao ?? "") + " não possui o terminal vinculado.";
                    return false;
                }
                if (pedido.Destinatario == null)
                {
                    msgErro = "O terminal de destino " + (pedido.TerminalDestino?.Descricao ?? "") + " não possui o terminal vinculado.";
                    return false;
                }
                if (pedido.PortoDestino == null)
                {
                    msgErro = "O terminal de destino " + (pedido.TerminalDestino?.Descricao ?? "") + " não possui o porto vinculado.";
                    return false;
                }
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

        private void PreencherCodigoCargaEmbarcador(Repositorio.UnitOfWork unitOfWork, ref Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, bool possuiCargaSVM, int contadorCargaSVM)
        {
            if (IsGerarCargaAutomaticamente(pedido) && pedido.GerarAutomaticamenteCargaDoPedido)
            {
                pedido.CodigoCargaEmbarcador = "SVM-" + pedido.NumeroBooking + (possuiCargaSVM ? "-" + contadorCargaSVM : "");
                pedido.AdicionadaManualmente = true;
            }
        }

        private bool IsGerarCargaAutomaticamente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return (pedido.PedidoIntegradoEmbarcador && !pedido.ColetaEmProdutorRural && !pedido.Cotacao);
        }

        private List<string> RetornaListaNumeroBooking(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
            Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, "", null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, "", null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);

            List<string> listaBookings = new List<string>();
            foreach (var cargaCTe in cargaCTes)
            {
                if (!string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroBooking))
                {
                    if (!listaBookings.Contains(cargaCTe.CTe.NumeroBooking))
                    {
                        listaBookings.Add(cargaCTe.CTe.NumeroBooking);
                    }
                }
            }

            foreach (var cargaCTe in cargaCTesTransbordo)
            {
                if (!string.IsNullOrWhiteSpace(cargaCTe.CTe.NumeroBooking))
                {
                    if (!listaBookings.Contains(cargaCTe.CTe.NumeroBooking))
                    {
                        listaBookings.Add(cargaCTe.CTe.NumeroBooking);
                    }
                }
            }
            return listaBookings;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido RetornaPrimeiroPedidoMultimodal(string numeroBooking, bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoMultiModal = repCargaCTe.ConsultarPedidoMultiModal(somenteCargaPerigosa, numeroBooking, null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            if (pedidoMultiModal != null)
                return pedidoMultiModal;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoMultiModalTransbordo = repCargaCTe.ConsultarPedidoMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            return pedidoMultiModalTransbordo;
        }

        private List<string> RetornaListaNumeroCargaSemContainer(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
         Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
         Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
         Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, "", null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, false, false);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, "", null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, false, false);

            List<string> listaCargas = new List<string>();
            foreach (var cargaCTe in cargaCTes)
            {
                if (!string.IsNullOrWhiteSpace(cargaCTe.Carga.CodigoCargaEmbarcador))
                {
                    if (!listaCargas.Contains(cargaCTe.Carga.CodigoCargaEmbarcador))
                    {
                        listaCargas.Add(cargaCTe.Carga.CodigoCargaEmbarcador);
                    }
                }
            }

            foreach (var cargaCTe in cargaCTesTransbordo)
            {
                if (!string.IsNullOrWhiteSpace(cargaCTe.Carga.CodigoCargaEmbarcador))
                {
                    if (!listaCargas.Contains(cargaCTe.Carga.CodigoCargaEmbarcador))
                    {
                        listaCargas.Add(cargaCTe.Carga.CodigoCargaEmbarcador);
                    }
                }
            }
            return listaCargas;
        }

        private string RetornarConhecimentoPendentesAutorizacao(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, true);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, true);

            List<string> entidades = new List<string>();
            if (cargaCTes != null && cargaCTes.Count > 0)
            {
                cargaCTes = cargaCTes.Where(o => o.CTe.Remetente.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                               || o.CTe.Remetente.Cliente.CodigoIntegracao == null || o.CTe.Destinatario.Cliente.CodigoIntegracao == null).ToList();
                foreach (var cargaCTe in cargaCTes)
                {
                    if (cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        string entidade = "Nº Controle (" + cargaCTe.CTe.NumeroControle + ") BK (" + cargaCTe.CTe.NumeroBooking + ") ainda não se encontra autorizado. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
            {
                cargaCTesTransbordo = cargaCTesTransbordo.Where(o => o.CTe.Remetente.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                               || o.CTe.Remetente.Cliente.CodigoIntegracao == null || o.CTe.Destinatario.Cliente.CodigoIntegracao == null).ToList();
                foreach (var cargaCTe in cargaCTesTransbordo)
                {
                    if (cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        string entidade = "Nº Controle (" + cargaCTe.CTe.NumeroControle + ") BK (" + cargaCTe.CTe.NumeroBooking + ") ainda não se encontra autorizado. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return entidades.ToString();
        }

        private bool ContemConhecimentoPendentesAutorizacao(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, true);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, true);

            return cargaCTes.Count > 0 || cargaCTesTransbordo.Count > 0;
        }

        private string RetornarCargaPendentesAutorizacao(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = repCargaCTe.ConsultarCargaMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaTransbordo = repCargaCTe.ConsultarCargaMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);

            List<string> entidades = new List<string>();
            if (carga != null && carga.Count > 0)
            {
                foreach (var cargaCTe in carga)
                {
                    if (cargaCTe.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        string entidade = "A carga de número Nº Controle (" + cargaCTe.CodigoCargaEmbarcador + ") ainda não se encontra autorizado. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (cargaTransbordo != null && cargaTransbordo.Count > 0)
            {
                foreach (var cargaCTe in cargaTransbordo)
                {
                    if (cargaCTe.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && cargaCTe.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada)
                    {
                        string entidade = "A carga de número Nº Controle (" + cargaCTe.CodigoCargaEmbarcador + ") ainda não se encontra autorizado. <br/>";
                        if (!entidades.Contains(entidade))
                            entidades.Add(entidade);
                    }
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return entidades.ToString();
        }

        private bool ContemCargaPendentesAutorizacao(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal? tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = repCargaCTe.ConsultarCargaMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaTransbordo = repCargaCTe.ConsultarCargaMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);

            return carga.Count > 0 || cargaTransbordo.Count > 0;
        }

        private List<string> RetornaListaNumeroBookingCargasPendentes(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = repCargaCTe.ConsultarCargaMultiModal(somenteCargaPerigosa, numeroBooking, null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaTransbordo = repCargaCTe.ConsultarCargaMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);

            List<string> listaBookings = new List<string>();
            foreach (var cargaCTe in carga)
            {
                if (cargaCTe.Pedidos != null && cargaCTe.Pedidos.Count > 0)
                {
                    foreach (var cargaPedido in cargaCTe.Pedidos)
                    {
                        if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroBooking))
                        {
                            if (!listaBookings.Contains(cargaPedido.Pedido.NumeroBooking))
                            {
                                listaBookings.Add(cargaPedido.Pedido.NumeroBooking);
                            }
                        }
                    }
                }
            }

            foreach (var cargaCTe in cargaTransbordo)
            {
                if (cargaCTe.Pedidos != null && cargaCTe.Pedidos.Count > 0)
                {
                    foreach (var cargaPedido in cargaCTe.Pedidos)
                    {
                        if (!string.IsNullOrWhiteSpace(cargaPedido.Pedido.NumeroBooking))
                        {
                            if (!listaBookings.Contains(cargaPedido.Pedido.NumeroBooking))
                            {
                                listaBookings.Add(cargaPedido.Pedido.NumeroBooking);
                            }
                        }
                    }
                }
            }

            return listaBookings;
        }

        private List<int> RetornaListaCodigosCargasPendentes(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> carga = repCargaCTe.ConsultarCargaMultiModal(somenteCargaPerigosa, numeroBooking, null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, true);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaTransbordo = repCargaCTe.ConsultarCargaMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, null, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0);

            List<int> listaBookings = new List<int>();
            foreach (var cargaCTe in carga)
                if (cargaCTe.Pedidos != null && cargaCTe.Pedidos.Count > 0)
                    foreach (var cargaPedido in cargaCTe.Pedidos)
                        if (cargaPedido.Carga.Codigo > 0)
                            if (!listaBookings.Contains(cargaPedido.Carga.Codigo))
                                listaBookings.Add(cargaPedido.Carga.Codigo);

            foreach (var cargaCTe in cargaTransbordo)
                if (cargaCTe.Pedidos != null && cargaCTe.Pedidos.Count > 0)
                    foreach (var cargaPedido in cargaCTe.Pedidos)
                        if (cargaPedido.Carga.Codigo > 0)
                            if (!listaBookings.Contains(cargaPedido.Carga.Codigo))
                                listaBookings.Add(cargaPedido.Carga.Codigo);

            return listaBookings;
        }

        private string RetornaDescricaoEntidadeSemCadastro(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            List<string> entidades = new List<string>();
            if (cargaCTes != null && cargaCTes.Count > 0)
            {
                cargaCTes = cargaCTes.Where(o => o.CTe.Remetente.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                               || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                               || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null
                                                               || o.CTe.Remetente.Cliente.CodigoIntegracao == null || o.CTe.Destinatario.Cliente.CodigoIntegracao == null).ToList();
                foreach (var cargaCTe in cargaCTes)
                {
                    string entidade = "";
                    if (string.IsNullOrWhiteSpace(cargaCTe.CTe.TomadorPagador.Cliente.CodigoIntegracao))
                        entidade = "Cliente (" + cargaCTe.CTe.TomadorPagador.Nome + ") com CNPJ (" + cargaCTe.CTe.TomadorPagador.CPF_CNPJ_Formatado + ") de IE (" + cargaCTe.CTe.TomadorPagador.IE_RG + ") do Nº Controle (" + cargaCTe.CTe.NumeroControle + ") não possuem cadastro. <br/>";
                    if (!string.IsNullOrWhiteSpace(entidade) && !entidades.Contains(entidade))
                        entidades.Add(entidade);

                    entidade = "";
                    if (string.IsNullOrWhiteSpace(cargaCTe.CTe.Remetente.Cliente.CodigoIntegracao))
                        entidade = "Cliente (" + cargaCTe.CTe.Remetente.Nome + ") com CNPJ (" + cargaCTe.CTe.Remetente.CPF_CNPJ_Formatado + ") de IE (" + cargaCTe.CTe.Remetente.IE_RG + ") do Nº Controle (" + cargaCTe.CTe.NumeroControle + ") não possuem cadastro. <br/>";
                    if (!string.IsNullOrWhiteSpace(entidade) && !entidades.Contains(entidade))
                        entidades.Add(entidade);

                    entidade = "";
                    if (string.IsNullOrWhiteSpace(cargaCTe.CTe.Destinatario.Cliente.CodigoIntegracao))
                        entidade = "Cliente (" + cargaCTe.CTe.Destinatario.Nome + ") com CNPJ (" + cargaCTe.CTe.Destinatario.CPF_CNPJ_Formatado + ") de IE (" + cargaCTe.CTe.Destinatario.IE_RG + ") do Nº Controle (" + cargaCTe.CTe.NumeroControle + ") não possuem cadastro. <br/>";
                    if (!string.IsNullOrWhiteSpace(entidade) && !entidades.Contains(entidade))
                        entidades.Add(entidade);
                }
            }
            if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
            {
                cargaCTesTransbordo = cargaCTesTransbordo.Where(o => o.CTe.Remetente.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                               || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                               || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null
                                                               || o.CTe.Remetente.Cliente.CodigoIntegracao == null || o.CTe.Destinatario.Cliente.CodigoIntegracao == null).ToList();
                foreach (var cargaCTe in cargaCTesTransbordo)
                {
                    string entidade = "";
                    if (string.IsNullOrWhiteSpace(cargaCTe.CTe.TomadorPagador.Cliente.CodigoIntegracao))
                        entidade = "Cliente (" + cargaCTe.CTe.TomadorPagador.Nome + ") com CNPJ (" + cargaCTe.CTe.TomadorPagador.CPF_CNPJ_Formatado + ") de IE (" + cargaCTe.CTe.TomadorPagador.IE_RG + ") do Nº Controle (" + cargaCTe.CTe.NumeroControle + ") não possuem cadastro. <br/>";
                    if (!string.IsNullOrWhiteSpace(entidade) && !entidades.Contains(entidade))
                        entidades.Add(entidade);

                    entidade = "";
                    if (string.IsNullOrWhiteSpace(cargaCTe.CTe.Remetente.Cliente.CodigoIntegracao))
                        entidade = "Cliente (" + cargaCTe.CTe.Remetente.Nome + ") com CNPJ (" + cargaCTe.CTe.Remetente.CPF_CNPJ_Formatado + ") de IE (" + cargaCTe.CTe.Remetente.IE_RG + ") do Nº Controle (" + cargaCTe.CTe.NumeroControle + ") não possuem cadastro. <br/>";
                    if (!string.IsNullOrWhiteSpace(entidade) && !entidades.Contains(entidade))
                        entidades.Add(entidade);

                    entidade = "";
                    if (string.IsNullOrWhiteSpace(cargaCTe.CTe.Destinatario.Cliente.CodigoIntegracao))
                        entidade = "Cliente (" + cargaCTe.CTe.Destinatario.Nome + ") com CNPJ (" + cargaCTe.CTe.Destinatario.CPF_CNPJ_Formatado + ") de IE (" + cargaCTe.CTe.Destinatario.IE_RG + ") do Nº Controle (" + cargaCTe.CTe.NumeroControle + ") não possuem cadastro. <br/>";
                    if (!string.IsNullOrWhiteSpace(entidade) && !entidades.Contains(entidade))
                        entidades.Add(entidade);
                }
            }
            if (entidades.Count > 0)
                return string.Join(" ", entidades);
            else
                return entidades.ToString();
        }

        private bool ContemEntidadeSemCadastro(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
           Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
           Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
           Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
           string numeroBooking,
           Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);

            if (cargaCTes != null && cargaCTes.Count > 0)
            {
                bool contemEntidadeSemCadastro = cargaCTes.Any(o => o.CTe.Remetente.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                                || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                                || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null
                                                                || o.CTe.Remetente.Cliente.CodigoIntegracao == null || o.CTe.Destinatario.Cliente.CodigoIntegracao == null);
                if (contemEntidadeSemCadastro)
                    return true;
            }
            if (cargaCTesTransbordo != null && cargaCTesTransbordo.Count > 0)
            {
                bool contemEntidadeSemCadastro = cargaCTesTransbordo.Any(o => o.CTe.Remetente.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                                || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == "" || o.CTe.Destinatario.Cliente.CodigoIntegracao == ""
                                                                || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null || o.CTe.TomadorPagador.Cliente.CodigoIntegracao == null
                                                                || o.CTe.Remetente.Cliente.CodigoIntegracao == null || o.CTe.Destinatario.Cliente.CodigoIntegracao == null);
                if (contemEntidadeSemCadastro)
                    return true;
            }

            return false;
        }

        private bool ContemDocumentosParaEmissao(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
            string numeroBooking,
            Repositorio.UnitOfWork unitOfWork, int codigoCargaSVM)
        {
            //return true;
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.ConsultarMultiModal(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, false, true, codigoCargaSVM);
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesTransbordo = repCargaCTe.ConsultarMultiModalTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas, false, true, codigoCargaSVM);

            return cargaCTes.Count > 0 || cargaCTesTransbordo.Count > 0;
        }

        private bool BookingPendenteDeEmissaoDeSVM(bool somenteCargaPerigosa, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
            string numeroBooking,
            Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                   {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos
                   };

            bool bookingPendenteEmissao = repCargaCTe.BookingPendenteEmissaoSVM(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);
            bool bookingPendenteEmissaoTransbordo = repCargaCTe.BookingPendenteEmissaoSVMTransbordo(somenteCargaPerigosa, numeroBooking, tipoPropostaMultimodal, pedidoViagemNavio?.Codigo ?? 0, terminalOrigem?.Codigo ?? 0, terminalDestino?.Codigo ?? 0, situacoesPermitidas);

            return !bookingPendenteEmissao || !bookingPendenteEmissaoTransbordo;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido PreecherDadosPedido(bool somenteCargaPerigosa, string numeroBooking, Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalOrigem,
            Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao terminalDestino,
            Dominio.Entidades.Cliente remetente,
            Dominio.Entidades.Cliente destinatario,
            Dominio.Entidades.Cliente recebedor,
            Dominio.Entidades.Cliente expedidor,
            Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio pedidoViagemNavio,
            string observacao,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal tipoPropostaMultimodal,
            Repositorio.UnitOfWork unitOfWork,
            bool possuiCargaPerigosa, string imoClasse, string imoSequencia, string imoUnidade, int codigoCargaSVM)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();

            pedido.Numero = repPedido.BuscarProximoNumero();

            pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
            pedido.CodigoCargaEmbarcador = "";
            pedido.NumeroPaletes = 0;
            pedido.NumeroBooking = numeroBooking;
            pedido.UltimaAtualizacao = DateTime.Now;
            pedido.Origem = terminalOrigem?.Terminal?.Localidade ?? null;
            pedido.Destino = terminalDestino?.Terminal?.Localidade ?? null;
            pedido.Destinatario = destinatario;
            pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.todos;
            pedido.PedidoRefaturamento = false;
            pedido.PedidoRefaturado = false;
            pedido.PesoTotalPaletes = (decimal)0;
            pedido.Recebedor = recebedor;
            pedido.Expedidor = expedidor;
            pedido.ValorTotalPaletes = (decimal)0;
            pedido.NumeroPedidoEmbarcador = pedido.Numero.ToString("D");
            pedido.Remetente = remetente;
            pedido.DataCarregamentoPedido = DateTime.Now;
            pedido.Requisitante = Dominio.ObjetosDeValor.Embarcador.Enumeradores.RequisitanteColeta.Destinatario;
            pedido.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
            pedido.PesoTotal = (decimal)0;
            pedido.ValorTotalNotasFiscais = (decimal)0;
            pedido.DataInicialColeta = DateTime.Now;
            pedido.Empresa = terminalOrigem?.Empresa ?? null;
            pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            pedido.Usuario = this.Usuario;
            pedido.Autor = this.Usuario;
            pedido.GerarAutomaticamenteCargaDoPedido = true;
            pedido.UsarOutroEnderecoOrigem = false;
            pedido.UsarOutroEnderecoDestino = false;
            pedido.EnderecoOrigem = null;
            pedido.EnderecoDestino = null;
            pedido.PedidoTotalmenteCarregado = true;
            pedido.GerarUmCTEPorNFe = false;
            pedido.PedidoSubContratado = false;
            pedido.ProdutoPredominante = "";
            pedido.UsarTipoPagamentoNF = false;
            pedido.ValorFreteNegociado = (decimal)0;
            pedido.Temperatura = "";
            pedido.TipoPessoa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa;
            pedido.TipoOperacao = null;
            pedido.CubagemTotal = (decimal)0;
            pedido.PedidoTransbordo = false;
            pedido.QtVolumes = 0;
            pedido.SaldoVolumesRestante = 0;
            pedido.NumeroNotaCliente = 0;
            pedido.NumeroPaletesFracionado = (decimal)0;
            pedido.AdicionadaManualmente = true;
            pedido.QtdEntregas = 0;
            pedido.NecessarioReentrega = false;
            pedido.Rastreado = false;
            pedido.GerenciamentoRisco = false;
            pedido.EscoltaArmada = false;
            pedido.EscoltaMunicipal = false;
            pedido.Seguro = false;
            pedido.Ajudante = false;
            pedido.QtdAjudantes = 0;
            pedido.ValorTotalCarga = (decimal)0;
            pedido.NumeroContainer = "";
            pedido.NumeroBL = "";
            pedido.NumeroNavio = "";
            pedido.EnderecoEntregaImportacao = "";
            pedido.BairroEntregaImportacao = "";
            pedido.CEPEntregaImportacao = "";
            pedido.ArmadorImportacao = "";
            pedido.EtapaPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Finalizada;
            pedido.PesoCubado = (decimal)0;
            pedido.DespachoTransitoAduaneiro = false;
            pedido.Cotacao = false;
            pedido.ValorFreteCotado = (decimal)0;
            pedido.SenhaAgendamento = "";
            pedido.SenhaAgendamentoCliente = "";
            pedido.ImprimirObservacaoCTe = true;
            pedido.PesoSaldoRestante = (decimal)0;
            pedido.PercentualAliquota = (decimal)0;
            pedido.BaseCalculoICMS = (decimal)0;
            pedido.ValorICMS = (decimal)0;
            pedido.ImpostoNegociado = false;
            pedido.ValorFreteAReceber = (decimal)0;
            pedido.IncluirBaseCalculoICMS = false;
            pedido.PercentualInclusaoBC = (decimal)0;
            pedido.PedidoRedespachoTotalmenteCarregado = false;
            pedido.ColetaEmProdutorRural = false;
            pedido.SituacaoPlanejamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPlanejamentoPedido.Pendente;
            pedido.TaraContainer = "";
            pedido.LacreContainerDois = "";
            pedido.LacreContainerTres = "";
            pedido.LacreContainerUm = "";
            pedido.DataCriacao = DateTime.Now;
            pedido.PossuiDescarga = false;
            pedido.QuantidadeNotasFiscais = 0;
            pedido.Protocolo = 0;
            pedido.QtdEscolta = 0;
            pedido.ValorFreteFilialEmissora = (decimal)0;
            pedido.DisponibilizarPedidoParaColeta = false;
            pedido.NumeroDTA = "";
            pedido.NumeroSequenciaPedido = 0;
            pedido.DataCadastro = DateTime.Now;
            pedido.PedidoViagemNavio = pedidoViagemNavio;
            pedido.Navio = pedidoViagemNavio?.Navio ?? null;
            pedido.TerminalOrigem = terminalOrigem;
            pedido.TerminalDestino = terminalDestino;
            pedido.Container = null;
            pedido.IDBAF = 0;
            pedido.ValorAdValorem = (decimal)0;
            pedido.ValorBAF = (decimal)0;
            pedido.NumeroOS = "";
            pedido.NumeroProposta = "";
            pedido.NecessitaAverbacaoAutomatica = false;
            pedido.PossuiCargaPerigosa = somenteCargaPerigosa;
            pedido.ContemCargaRefrigerada = false;
            pedido.ValidarDigitoVerificadorContainer = false;
            pedido.PedidoSVM = true;
            pedido.DirecaoViagemMultimodal = pedidoViagemNavio?.DirecaoViagemMultimodal ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal.Norte;
            pedido.NecessitaEnergiaContainerRefrigerado = false;
            pedido.Porto = terminalOrigem?.Porto ?? null;
            pedido.PortoDestino = terminalDestino?.Porto ?? null;
            pedido.Observacao = observacao;
            pedido.ObservacaoCTe = observacao;
            pedido.ResponsavelRedespacho = expedidor;
            pedido.PedidoIntegradoEmbarcador = true;
            pedido.PedidoDeSVMTerceiro = false;
            pedido.TipoPropostaMultimodal = tipoPropostaMultimodal;

            pedido.PossuiCargaPerigosa = possuiCargaPerigosa;
            pedido.IMOClasse = imoClasse;
            pedido.IMOSequencia = imoSequencia;
            pedido.IMOUnidade = imoUnidade;
            pedido.CargaSVM = codigoCargaSVM > 0 ? new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = codigoCargaSVM } : null;

            //faltou o transbordo

            return pedido;
        }

        #endregion
    }
}
