using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/Booking")]
    public class PedidoDadosTransporteMaritimoController : BaseController
    {
		#region Construtores

		public PedidoDadosTransporteMaritimoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Pedido", "NumeroPedidoEmbarcador", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("N° Booking", "NumeroBooking", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("N° EXP", "NumeroExp", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Cliente", "Cliente", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioPedidoDadosTransporteMaritimo.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> listaPedidoDadosTransporte = totalRegistros > 0 ? repositorioPedidoDadosTransporteMaritimo.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>();

                grid.AdicionaRows((
                    from o in listaPedidoDadosTransporte
                    select new
                    {
                        o.Codigo,
                        DescricaoStatus = o.DescricaoStatus,
                        o.Status,
                        NumeroPedidoEmbarcador = o.Pedido.NumeroPedidoEmbarcador,
                        NumeroBooking = o.NumeroBooking,
                        NumeroExp = o.NumeroEXP,
                        Cliente = o.Pedido.Destinatario.Descricao,
                        Destino = o.Pedido.Recebedor?.Localidade?.DescricaoCidadeEstado ?? ""
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pedido", "NumeroPedidoEmbarcador", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("N° Booking", "NumeroBooking", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("N° EXP", "NumeroExp", 8, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Cliente", "Cliente", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioPedidoDadosTransporteMaritimo.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> listaPedidoDadosTransporte = totalRegistros > 0 ? repositorioPedidoDadosTransporteMaritimo.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo>();

                grid.AdicionaRows((
                    from o in listaPedidoDadosTransporte
                    select new
                    {
                        o.Codigo,
                        DescricaoStatus = o.DescricaoStatus,
                        o.Status,
                        NumeroPedidoEmbarcador = o.Pedido.NumeroPedidoEmbarcador,
                        NumeroBooking = o.Pedido.NumeroBooking,
                        NumeroExp = o.Pedido.NumeroEXP,
                        Cliente = o.Pedido.Destinatario.Descricao,
                        Destino = o.Pedido.Recebedor?.Localidade?.DescricaoCidadeEstado ?? ""
                    }).ToList()
                );

                grid.setarQuantidadeTotal(totalRegistros);


                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");

                return Arquivo(bArquivo, "application/csv", "PedidoDadosTransporteMaritimo.csv");

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

        //NAÕ DEVE ADICIONAR BOOKING
        //public async Task<IActionResult> Adicionar()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

        //    try
        //    {

        //        unitOfWork.Start();

        //        Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
        //        Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo servIntegracaoPedidoDadosTransporteMaritimo = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork);

        //        Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = new Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo();
        //        dadosTransporteMaritimo.Status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo>("Situacao");
        //        preencherEntidade(dadosTransporteMaritimo, unitOfWork);
        //        PreencherPlanejamentoControle(dadosTransporteMaritimo, unitOfWork);
        //        PreencherTransporte(dadosTransporteMaritimo, unitOfWork);

        //        repositorioPedidoDadosTransporteMaritimo.Inserir(dadosTransporteMaritimo);

        //        servIntegracaoPedidoDadosTransporteMaritimo.AdicionarPedidoDadosTransporteMaritimoIntegracao(dadosTransporteMaritimo);

        //        DesativarOutrosBookingPedido(dadosTransporteMaritimo.Codigo, dadosTransporteMaritimo.Pedido.Codigo, unitOfWork); //garantindo que ficara apenas um para o pedido.

        //        unitOfWork.CommitChanges();

        //        return new JsonpResult(true);

        //    }
        //    catch (BaseException excecao)
        //    {
        //        unitOfWork.Rollback();
        //        return new JsonpResult(false, true, excecao.Message);
        //    }
        //    catch (Exception excecao)
        //    {
        //        Servicos.Log.TratarErro(excecao);
        //        unitOfWork.Rollback();
        //        return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo servIntegracaoPedidoDadosTransporteMaritimo = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = repositorioPedidoDadosTransporteMaritimo.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (dadosTransporteMaritimo == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimoClonado = dadosTransporteMaritimo.Clonar();

                dadosTransporteMaritimoClonado.Initialize();
                preencherEntidade(dadosTransporteMaritimoClonado, unitOfWork);
                PreencherPlanejamentoControle(dadosTransporteMaritimoClonado, unitOfWork);
                PreencherTransporteEOutros(dadosTransporteMaritimoClonado, unitOfWork);

                if (dadosTransporteMaritimoClonado.IsChanged())
                {
                    dadosTransporteMaritimoClonado.CodigoOriginal = dadosTransporteMaritimo.Codigo;
                    dadosTransporteMaritimoClonado.BookingTemporario = true;

                    repositorioPedidoDadosTransporteMaritimo.Inserir(dadosTransporteMaritimoClonado);
                    servIntegracaoPedidoDadosTransporteMaritimo.AdicionarPedidoDadosTransporteMaritimoIntegracao(dadosTransporteMaritimoClonado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarBooking()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo servIntegracaoPedidoDadosTransporteMaritimo = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = repositorioPedidoDadosTransporteMaritimo.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (dadosTransporteMaritimo == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                dadosTransporteMaritimo.Initialize();

                string justificativa = Request.GetStringParam("Justificativa");
                dadosTransporteMaritimo.JustificativaCancelamento = justificativa;
                dadosTransporteMaritimo.Status = StatusControleMaritimo.Cancelado;

                repositorioPedidoDadosTransporteMaritimo.Atualizar(dadosTransporteMaritimo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repPedidoDadosTransporte = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao repPedidoDadosTransporteIntegracao = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimoIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = repPedidoDadosTransporte.BuscarPorCodigo(codigo, true);

                if (dadosTransporteMaritimo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                dynamic dynPedido = new
                {
                    Codigo = dadosTransporteMaritimo.Codigo,
                    NumeroExp = !string.IsNullOrEmpty(dadosTransporteMaritimo.NumeroEXP) ? dadosTransporteMaritimo.NumeroEXP : dadosTransporteMaritimo.Pedido?.NumeroEXP ?? "",
                    Situacao = dadosTransporteMaritimo.Status,
                    TipoCarga = new { Codigo = dadosTransporteMaritimo.TipoDeCarga?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.TipoDeCarga?.Descricao ?? string.Empty },
                    dadosTransporteMaritimo.CodigoContratoFOB,
                    dadosTransporteMaritimo.DescricaoEspecie,
                    Pedido = new { Codigo = dadosTransporteMaritimo.Pedido?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Pedido?.NumeroPedidoEmbarcador ?? string.Empty },
                    Filial = new { Codigo = dadosTransporteMaritimo.Filial?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Filial?.Descricao ?? string.Empty },
                    dadosTransporteMaritimo.CodigoArmador,
                    dadosTransporteMaritimo.CodigoEspecie,
                    dadosTransporteMaritimo.CodigoNCM,
                    dadosTransporteMaritimo.Incoterm,
                    Importador = new { Codigo = dadosTransporteMaritimo.Importador?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Importador?.Descricao ?? "" },

                    PlanejamentoControle = new
                    {
                        Halal = dadosTransporteMaritimo.Halal,
                        dadosTransporteMaritimo.StatusEXP,
                        DataPrevisaoEstufagem = dadosTransporteMaritimo.DataPrevisaoEstufagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        Remetente = new { Codigo = dadosTransporteMaritimo.Remetente?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Remetente?.Descricao ?? "" },
                        dadosTransporteMaritimo.Observacao,
                    },

                    Transporte = new
                    {
                        NumeroBooking = !string.IsNullOrEmpty(dadosTransporteMaritimo.NumeroBooking) ? dadosTransporteMaritimo.NumeroBooking : dadosTransporteMaritimo.Pedido?.NumeroBooking ?? "",
                        dadosTransporteMaritimo.CodigoIdentificacaoCarga,
                        CodigoCarga = !string.IsNullOrEmpty(dadosTransporteMaritimo.CodigoCargaEmbarcador) ? dadosTransporteMaritimo.CodigoCargaEmbarcador : dadosTransporteMaritimo.Pedido?.CodigoCargaEmbarcador ?? "",
                        dadosTransporteMaritimo.ProtocoloCarga,
                        dadosTransporteMaritimo.MetragemCarga,
                        dadosTransporteMaritimo.Transbordo,
                        dadosTransporteMaritimo.MensagemTransbordo,
                        Despachante = new { Codigo = dadosTransporteMaritimo.Despachante?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Despachante?.Descricao ?? string.Empty },
                        DataBooking = dadosTransporteMaritimo.DataBooking?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataDeadLineCarga = dadosTransporteMaritimo.DataDeadLineCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataDeadLineDraf = dadosTransporteMaritimo.DataDeadLineDraf?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETAOrigem = dadosTransporteMaritimo.DataETAOrigem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETAOrigemFinal = dadosTransporteMaritimo.DataETAOrigemFinal?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETASegundoDestino = dadosTransporteMaritimo.DataETASegundoDestino?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETATransbordo = dadosTransporteMaritimo.DataETATransbordo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETS = dadosTransporteMaritimo.DataETS?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETSTransbordo = dadosTransporteMaritimo.DataETSTransbordo?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        MoedaCapatazia = new { Codigo = dadosTransporteMaritimo.MoedaCapatazia?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.MoedaCapatazia?.Simbolo ?? "" },
                        dadosTransporteMaritimo.ValorCapatazia,
                        dadosTransporteMaritimo.ValorFrete,
                        dadosTransporteMaritimo.FretePrepaid,
                        dadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo,
                        dadosTransporteMaritimo.DescricaoPortoCarregamentoTransbordo,
                        PortoDestino = new { Codigo = dadosTransporteMaritimo.PortoDestino?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.PortoDestino?.Descricao ?? "" },
                        SiglaPaisPortoDestinoTransbordo = dadosTransporteMaritimo.PortoDestino?.Localidade?.Pais?.Abreviacao ?? "",
                        DescricaoPaisPortoDestino = dadosTransporteMaritimo.PortoDestino?.Localidade?.Pais?.Descricao ?? "",
                        PortoOrigem = new { Codigo = dadosTransporteMaritimo.PortoOrigem?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.PortoOrigem?.Descricao ?? "" },
                        SiglaPaisPortoCarregamento = dadosTransporteMaritimo.PortoOrigem?.Localidade?.Pais?.Abreviacao ?? "",
                        DescricaoPaisPortoCarregamento = dadosTransporteMaritimo.PortoOrigem?.Localidade?.Pais?.Descricao ?? "",
                        NavioTransbordo = new { Codigo = dadosTransporteMaritimo.NavioTransbordo?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.NavioTransbordo?.Descricao ?? "" },
                        Navio = new { Codigo = dadosTransporteMaritimo.Navio?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Navio?.Descricao ?? "" },
                        dadosTransporteMaritimo.ModoTransporte,
                        Tipotransporte = dadosTransporteMaritimo.TipoDeTransporte,
                        dadosTransporteMaritimo.NumeroLacre,
                        dadosTransporteMaritimo.NumeroViagem,
                        dadosTransporteMaritimo.NumeroViagemTransbordo,
                        TerminalContainer = new { Codigo = dadosTransporteMaritimo.LocalTerminalContainer?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.LocalTerminalContainer?.Descricao ?? "" },
                        TerminalOrigem = new { Codigo = dadosTransporteMaritimo.LocalTerminalOrigem?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.LocalTerminalOrigem?.Descricao ?? "" },
                        dadosTransporteMaritimo.Temperatura,
                        dadosTransporteMaritimo.TipoInLand,
                        Armador = new { Codigo = dadosTransporteMaritimo.Armador?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Armador?.NomeCNPJ ?? string.Empty },
                        Container = new { Codigo = dadosTransporteMaritimo.Container?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.Container?.Descricao ?? string.Empty },
                        TipoContainer = new { Codigo = dadosTransporteMaritimo.TipoContainer?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.TipoContainer?.Descricao ?? "" },
                        ViaTransporte = new { Codigo = dadosTransporteMaritimo.ViaTransporte?.Codigo ?? 0, Descricao = dadosTransporteMaritimo.ViaTransporte?.Descricao ?? "" },
                        dadosTransporteMaritimo.TipoProbe,
                        dadosTransporteMaritimo.TipoEnvio,
                        dadosTransporteMaritimo.CargaPaletizada,
                        PossuiGenset = dadosTransporteMaritimo.PossuiGenset,
                        JustificativaCancelamento = dadosTransporteMaritimo.JustificativaCancelamento,
                        ETSPermiteEditar = validarDataETSPermiteEditar(dadosTransporteMaritimo.DataETS),
                        RegistroAguardandoRetorno = repPedidoDadosTransporteIntegracao.ExisteDadosTransporteMaritimoCodigoOriginalAguardandoRetorno(dadosTransporteMaritimo.Codigo),
                    },
                    Outros = new
                    {
                        DataDeadLinePedido = dadosTransporteMaritimo.DataDeadLinePedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        SegundaDataDeadLineCarga = dadosTransporteMaritimo.SegundaDataDeadLineCarga?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataReserva = dadosTransporteMaritimo.DataReserva?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        SegundaDataDeadLineDraf = dadosTransporteMaritimo.SegundaDataDeadLineDraf?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETASegundaOrigem = dadosTransporteMaritimo.DataETASegundaOrigem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataRetiradaContainerDestino = dadosTransporteMaritimo.DataRetiradaContainerDestino?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataETADestinoFinal = dadosTransporteMaritimo.DataETADestinoFinal?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        dadosTransporteMaritimo.CodigoRota,
                        DataETADestino = dadosTransporteMaritimo.DataETADestino?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataDepositoContainer = dadosTransporteMaritimo.DataDepositoContainer?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataRetiradaContainer = dadosTransporteMaritimo.DataRetiradaContainer?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataRetiradaVazio = dadosTransporteMaritimo.DataRetiradaVazio?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataCarregamentoPedido = dadosTransporteMaritimo.DataCarregamentoPedido?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataPrevisaoEntrega = dadosTransporteMaritimo.DataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        DataConhecimento = dadosTransporteMaritimo.DataConhecimento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        NumeroBL = !string.IsNullOrEmpty(dadosTransporteMaritimo.NumeroBL) ? dadosTransporteMaritimo.NumeroBL : dadosTransporteMaritimo.Pedido?.NumeroBL ?? "",
                    }
                };


                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DuplicarBookingCancelado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo servIntegracaoPedidoDadosTransporteMaritimo = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoPedidoDadosTransporteMaritimo(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo dadosTransporteMaritimo = new Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo();
                dadosTransporteMaritimo.Status = StatusControleMaritimo.Ativo;

                preencherEntidade(dadosTransporteMaritimo, unitOfWork);
                PreencherPlanejamentoControle(dadosTransporteMaritimo, unitOfWork);
                PreencherTransporteEOutros(dadosTransporteMaritimo, unitOfWork);

                repositorioPedidoDadosTransporteMaritimo.Inserir(dadosTransporteMaritimo);

                servIntegracaoPedidoDadosTransporteMaritimo.AdicionarPedidoDadosTransporteMaritimoIntegracao(dadosTransporteMaritimo);

                DesativarOutrosBookingPedido(dadosTransporteMaritimo.Codigo, dadosTransporteMaritimo.Pedido.Codigo, unitOfWork); //garantindo que ficara apenas um para o pedido.

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ObterTipoContainerPorModeloVeicular()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                dynamic dynPedido = new
                {
                    TipoContainer = new { Codigo = pedido.ModeloVeicularCarga?.ContainerTipo?.Codigo ?? 0, Descricao = pedido.ModeloVeicularCarga?.ContainerTipo?.Descricao ?? "" },
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por tipo de container.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos - Importações

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Pedido.PedidoDadosTransporteMaritimo servicoPedidoDadostransporteMaritimo = new Servicos.Embarcador.Pedido.PedidoDadosTransporteMaritimo(unitOfWork, configuracaoTMS, TipoServicoMultisoftware);
                Dominio.Entidades.Embarcador.Operacional.OperadorLogistica operadorLogistica = ObterOperadorLogistica(unitOfWork);

                string dados = Request.GetStringParam("Dados");
                (string Nome, string Guid) arquivoGerador = ValueTuple.Create(Request.GetStringParam("Nome") ?? string.Empty, Request.GetStringParam("ArquivoSalvoComo") ?? string.Empty);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = servicoPedidoDadostransporteMaritimo.ImportarBooking(dados, arquivoGerador, Usuario, operadorLogistica, Cliente, Auditado, _conexao.AdminStringConexao);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Pedido.PedidoDadosTransporteMaritimo servicoPedidoDadostransporteMaritimo = new Servicos.Embarcador.Pedido.PedidoDadosTransporteMaritimo(unitOfWork, configuracaoTMS, TipoServicoMultisoftware);
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoPedidoDadostransporteMaritimo.ConfiguracaoImportacaoBooking();

                return new JsonpResult(configuracoes.ToList());
            }
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaPedidoDadosTransporteMaritimo()
            {
                NumeroCargaEmbarcador = Request.GetStringParam("Carga"),
                numeroBooking = Request.GetStringParam("NumeroBooking"),
                numeroEXP = Request.GetStringParam("NumeroEXP"),
                status = Request.GetNullableEnumParam<StatusControleMaritimo>("Situacao"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFim = Request.GetNullableDateTimeParam("DataFinal"),
                Filial = Request.GetIntParam("Filial"),
                Origem = Request.GetIntParam("Origem"),
                Destino = Request.GetIntParam("Destino")
            };

            return filtrosPesquisa;
        }

        private void preencherEntidade(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo, Repositorio.UnitOfWork unitOfWork)
        {
            int CodPedido = Request.GetIntParam("Pedido");
            if (CodPedido > 0)
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                pedidoDadosTransporteMaritimo.Pedido = repositorioPedido.BuscarPorCodigo(CodPedido);
            }

            int codigoFilial = Request.GetIntParam("Filial");
            if (codigoFilial > 0)
            {
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                pedidoDadosTransporteMaritimo.Filial = repositorioFilial.BuscarPorCodigo(codigoFilial);
            }

            double codArmador = Request.GetDoubleParam("Armador");
            if (codArmador > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                pedidoDadosTransporteMaritimo.Armador = repCliente.BuscarPorCPFCNPJ(codArmador);
            }

            double codImportador = Request.GetDoubleParam("Importador");
            if (codImportador > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                pedidoDadosTransporteMaritimo.Importador = repCliente.BuscarPorCPFCNPJ(codImportador);
            }

            int tipoCarga = Request.GetIntParam("TipoDeCarga");
            if (tipoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                pedidoDadosTransporteMaritimo.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(tipoCarga, false);
            }


            pedidoDadosTransporteMaritimo.NumeroEXP = Request.GetStringParam("NumeroEXP");
            pedidoDadosTransporteMaritimo.CodigoEspecie = Request.GetStringParam("CodigoEspecie");
            pedidoDadosTransporteMaritimo.DescricaoEspecie = Request.GetStringParam("DescricaoEspecie");
            pedidoDadosTransporteMaritimo.CodigoNCM = Request.GetStringParam("CodigoNCM");
            pedidoDadosTransporteMaritimo.Incoterm = Request.GetStringParam("Incoterm");
            pedidoDadosTransporteMaritimo.DescricaoIdentificacaoCarga = Request.GetStringParam("DescricaoIdentificacaoCarga");
            pedidoDadosTransporteMaritimo.CodigoContratoFOB = Request.GetStringParam("CodigoContratoFOB");

        }

        private void DesativarOutrosBookingPedido(int codigoBookingAtivo, int codigoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo repositorioPedidoDadosTransporteMaritimo = new Repositorio.Embarcador.Pedidos.PedidoDadosTransporteMaritimo(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo> DadosTransporteMaritimoPedido = repositorioPedidoDadosTransporteMaritimo.BuscarTodosPorPedido(codigoPedido);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo bookingDesativar in DadosTransporteMaritimoPedido)
            {
                if (bookingDesativar.Codigo != codigoBookingAtivo)
                {
                    bookingDesativar.Status = StatusControleMaritimo.Cancelado;
                    bookingDesativar.JustificativaCancelamento = "Booking cancelado ao ativar novo booking para o pedido";
                    repositorioPedidoDadosTransporteMaritimo.Atualizar(bookingDesativar);
                }
            }
        }

        private bool validarDataETSPermiteEditar(DateTime? dataEts)
        {
            if (!dataEts.HasValue)
                return true; //SE VAZIO, PERMITE EDITAR;

            // Validar: Se data atual for menor que o dia 10 do mês subsequente a data do campo PERMITE EDITAR;
            // Validar: Se data atual for maior ou igual que o dia 10 do mês subsequente a data do campo NÃO PERMITE EDITAR;
            DateTime dataSubSequente = dataEts.Value.AddMonths(1);
            DateTime dia10MesSubsequente = new DateTime(dataSubSequente.Year, dataSubSequente.Month, 10);

            if (DateTime.Now < dia10MesSubsequente)
                return true;
            else
                return false;
        }

        private void PreencherPlanejamentoControle(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic dynPlanejamentoControle = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PlanejamentoControle"));

            if (dynPlanejamentoControle != null)
            {
                pedidoDadosTransporteMaritimo.Halal = ((string)dynPlanejamentoControle.Halal).ToBool();
                pedidoDadosTransporteMaritimo.StatusEXP = ((string)dynPlanejamentoControle.StatusEXP).ToEnum<StatusEXP>();
                pedidoDadosTransporteMaritimo.DataPrevisaoEstufagem = ((string)dynPlanejamentoControle.DataPrevisaoEstufagem).ToNullableDateTime();
                double codigoRemetente = ((string)dynPlanejamentoControle.Remetente).ToDouble();
                pedidoDadosTransporteMaritimo.Remetente = codigoRemetente > 0 ? repCliente.BuscarPorCPFCNPJ(codigoRemetente) : null;
                pedidoDadosTransporteMaritimo.Observacao = ((string)dynPlanejamentoControle.Observacao);
            }
        }

        private void PreencherTransporteEOutros(Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pedidos.Navio repNavio = new Repositorio.Embarcador.Pedidos.Navio(unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(unitOfWork);
            Repositorio.Embarcador.Cargas.ViaTransporte repViaTransporte = new Repositorio.Embarcador.Cargas.ViaTransporte(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Moedas.Moeda repMoeda = new Repositorio.Embarcador.Moedas.Moeda(unitOfWork);

            dynamic dynPlanejamentoControle = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Transporte"));

            if (dynPlanejamentoControle != null)
            {
                pedidoDadosTransporteMaritimo.NumeroBooking = ((string)dynPlanejamentoControle.NumeroBooking);
                pedidoDadosTransporteMaritimo.CodigoIdentificacaoCarga = ((string)dynPlanejamentoControle.CodigoIdentificacaoCarga);
                pedidoDadosTransporteMaritimo.CodigoCargaEmbarcador = ((string)dynPlanejamentoControle.CodigoCarga);
                pedidoDadosTransporteMaritimo.ProtocoloCarga = ((string)dynPlanejamentoControle.ProtocoloCarga);
                pedidoDadosTransporteMaritimo.MetragemCarga = ((string)dynPlanejamentoControle.MetragemCarga);
                pedidoDadosTransporteMaritimo.Transbordo = ((string)dynPlanejamentoControle.Transbordo);
                pedidoDadosTransporteMaritimo.MensagemTransbordo = ((string)dynPlanejamentoControle.MensagemTransbordo);
                pedidoDadosTransporteMaritimo.CodigoRota = ((string)dynPlanejamentoControle.CodigoRota);
                pedidoDadosTransporteMaritimo.Transbordo = ((string)dynPlanejamentoControle.Transbordo);
                pedidoDadosTransporteMaritimo.DataBooking = ((string)dynPlanejamentoControle.DataBooking).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataDeadLineCarga = ((string)dynPlanejamentoControle.DataDeadLineCarga).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataDeadLineDraf = ((string)dynPlanejamentoControle.DataDeadLineDraf).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETADestino = ((string)dynPlanejamentoControle.DataETADestino).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETADestinoFinal = ((string)dynPlanejamentoControle.DataETADestinoFinal).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETAOrigem = ((string)dynPlanejamentoControle.DataETAOrigem).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETAOrigemFinal = ((string)dynPlanejamentoControle.DataETAOrigemFinal).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETASegundoDestino = ((string)dynPlanejamentoControle.DataETASegundoDestino).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETATransbordo = ((string)dynPlanejamentoControle.DataETATransbordo).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataDepositoContainer = ((string)dynPlanejamentoControle.DataDepositoContainer).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataRetiradaContainer = ((string)dynPlanejamentoControle.DataRetiradaContainer).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataRetiradaVazio = ((string)dynPlanejamentoControle.DataRetiradaVazio).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETS = ((string)dynPlanejamentoControle.DataETS).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETSTransbordo = ((string)dynPlanejamentoControle.DataETSTransbordo).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataCarregamentoPedido = ((string)dynPlanejamentoControle.DataCarregamentoPedido).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataPrevisaoEntrega = ((string)dynPlanejamentoControle.DataPrevisaoEntrega).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataConhecimento = ((string)dynPlanejamentoControle.DataConhecimento).ToNullableDateTime();
                int codigoMoeda = ((string)dynPlanejamentoControle.MoedaCapatazia).ToInt();
                pedidoDadosTransporteMaritimo.MoedaCapatazia = codigoMoeda > 0 ? repMoeda.BuscarPorCodigo(codigoMoeda) : null;
                pedidoDadosTransporteMaritimo.ValorCapatazia = ((string)dynPlanejamentoControle.ValorCapatazia).ToDecimal();
                pedidoDadosTransporteMaritimo.ValorFrete = ((string)dynPlanejamentoControle.ValorFrete).ToDecimal();
                pedidoDadosTransporteMaritimo.FretePrepaid = ((string)dynPlanejamentoControle.FretePrepaid).ToEnum<FretePrepaid>();
                pedidoDadosTransporteMaritimo.CodigoPortoCarregamentoTransbordo = ((string)dynPlanejamentoControle.CodigoPortoCarregamentoTransbordo);
                pedidoDadosTransporteMaritimo.DescricaoPortoCarregamentoTransbordo = ((string)dynPlanejamentoControle.DescricaoPortoCarregamentoTransbordo);
                double codigoPortoDestino = ((string)dynPlanejamentoControle.PortoDestino).ToDouble();
                pedidoDadosTransporteMaritimo.PortoDestino = codigoPortoDestino > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPortoDestino) : null;
                double codigoPortoOrigem = ((string)dynPlanejamentoControle.PortoOrigem).ToDouble();
                pedidoDadosTransporteMaritimo.PortoOrigem = codigoPortoOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPortoOrigem) : null;
                int codigoNavioTransbordo = ((string)dynPlanejamentoControle.NavioTransbordo).ToInt();
                pedidoDadosTransporteMaritimo.NavioTransbordo = codigoNavioTransbordo > 0 ? repNavio.BuscarPorCodigo(codigoNavioTransbordo, false) : null;
                int codigoNavio = ((string)dynPlanejamentoControle.Navio).ToInt();
                pedidoDadosTransporteMaritimo.Navio = codigoNavio > 0 ? repNavio.BuscarPorCodigo(codigoNavio, false) : null;
                pedidoDadosTransporteMaritimo.ModoTransporte = ((string)dynPlanejamentoControle.ModoTransporte);
                pedidoDadosTransporteMaritimo.TipoDeTransporte = ((string)dynPlanejamentoControle.TipoTransporte).ToEnum<TipoTransporteDadosMaritimos>();
                pedidoDadosTransporteMaritimo.NumeroBL = ((string)dynPlanejamentoControle.NumeroBL);
                pedidoDadosTransporteMaritimo.NumeroLacre = ((string)dynPlanejamentoControle.NumeroLacre);
                pedidoDadosTransporteMaritimo.NumeroViagem = ((string)dynPlanejamentoControle.NumeroViagem);
                pedidoDadosTransporteMaritimo.NumeroViagemTransbordo = ((string)dynPlanejamentoControle.NumeroViagemTransbordo);
                double codigoLocalTerminalContainer = ((string)dynPlanejamentoControle.TerminalContainer).ToDouble();
                pedidoDadosTransporteMaritimo.LocalTerminalContainer = codigoLocalTerminalContainer > 0 ? repCliente.BuscarPorCPFCNPJ(codigoLocalTerminalContainer) : null;
                double codigoLocalTerminalOrigem = ((string)dynPlanejamentoControle.TerminalOrigem).ToDouble();
                pedidoDadosTransporteMaritimo.LocalTerminalOrigem = codigoLocalTerminalOrigem > 0 ? repCliente.BuscarPorCPFCNPJ(codigoLocalTerminalOrigem) : null;
                pedidoDadosTransporteMaritimo.Temperatura = ((string)dynPlanejamentoControle.Temperatura);
                pedidoDadosTransporteMaritimo.TipoInLand = ((string)dynPlanejamentoControle.TipoInLand).ToEnum<TipoInland>();
                double codigoArmador = ((string)dynPlanejamentoControle.Armador).ToDouble();
                pedidoDadosTransporteMaritimo.Armador = codigoArmador > 0 ? repCliente.BuscarPorCPFCNPJ(codigoArmador) : null;
                int codigoContainer = ((string)dynPlanejamentoControle.Container).ToInt();
                pedidoDadosTransporteMaritimo.Container = codigoContainer > 0 ? repContainer.BuscarPorCodigo(codigoContainer, false) : null;
                int codigoTipoContainer = ((string)dynPlanejamentoControle.TipoContainer).ToInt();
                pedidoDadosTransporteMaritimo.TipoContainer = codigoTipoContainer > 0 ? repTipoContainer.BuscarPorCodigo(codigoTipoContainer, false) : null;
                int codigoViaTransporte = ((string)dynPlanejamentoControle.ViaTransporte).ToInt();
                pedidoDadosTransporteMaritimo.ViaTransporte = codigoViaTransporte > 0 ? repViaTransporte.BuscarPorCodigo(codigoViaTransporte, false) : null;
                pedidoDadosTransporteMaritimo.TipoProbe = ((string)dynPlanejamentoControle.TipoProbe).ToEnum<TipoProbe>();
                pedidoDadosTransporteMaritimo.TipoEnvio = ((string)dynPlanejamentoControle.TipoEnvio).ToEnum<TipoEnvioTransporteMaritimo>();
                pedidoDadosTransporteMaritimo.CargaPaletizada = ((string)dynPlanejamentoControle.CargaPaletizada).ToBool();
                pedidoDadosTransporteMaritimo.PossuiGenset = ((string)dynPlanejamentoControle.PossuiGenset).ToBool();
                pedidoDadosTransporteMaritimo.JustificativaCancelamento = ((string)dynPlanejamentoControle.JustificativaCancelamento);
                double codigoDespachante = ((string)dynPlanejamentoControle.Despachante).ToDouble();
                if (codigoDespachante > 0)
                    pedidoDadosTransporteMaritimo.Despachante = repCliente.BuscarPorCPFCNPJ(codigoDespachante);

            }


            dynamic dynOutros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Outros"));

            if (dynOutros != null)
            {
                pedidoDadosTransporteMaritimo.DataRetiradaContainerDestino = ((string)dynOutros.DataRetiradaContainerDestino).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataDeadLinePedido = ((string)dynOutros.DataDeadLinePedido).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.SegundaDataDeadLineCarga = ((string)dynOutros.SegundaDataDeadLineCarga).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.SegundaDataDeadLineDraf = ((string)dynOutros.SegundaDataDeadLineDraf).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataReserva = ((string)dynOutros.DataReserva).ToNullableDateTime();
                pedidoDadosTransporteMaritimo.DataETASegundaOrigem = ((string)dynOutros.DataETASegundaOrigem).ToNullableDateTime();
            }

        }
        #endregion
    }
}
