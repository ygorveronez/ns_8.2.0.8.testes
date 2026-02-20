using Dominio.Entidades;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize("Pedidos/AtendimentoPedido")]
    public class AtendimentoPedidoClienteController : BaseController
    {
		#region Construtores

		public AtendimentoPedidoClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

            try
            {
                (List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos, int QuantidadeRegistros) retornoBusca = BuscarPedidos(unitOfWork);

                var data = (from obj in retornoBusca.Pedidos
                            select new
                            {
                                obj.Codigo,
                                NumeroPedido = obj.NumeroPedidoEmbarcador.Contains("_") ? obj.NumeroPedidoEmbarcador.Split('_')[1] : obj.NumeroPedidoEmbarcador,
                                CNPJCliente = obj.Destinatario?.CPF_CNPJ_Formatado,
                                Contato = obj.Destinatario?.Email,
                                DataEntrega = obj.DataAgendamento.HasValue ? obj.DataAgendamento.Value.ToString("dd/MM/yyyy") : "",
                                DataEmissao = obj.DataCadastro.HasValue ? obj.DataCadastro.Value.ToString("dd/MM/yyyy") : "",
                                Destino = obj.Destino?.Descricao,
                                Destinatario = obj.Destinatario?.Descricao,
                                DataCriacao = obj.DataCriacao?.ToString("dd/MM/yyyy"),
                                PossuiChatAtivo = obj.PossuiChatAtivo,
                                PossuiChat = repChatMobileMensagem.ExisteChatPorPedidoRemetente(obj.Codigo, Usuario.Codigo),
                            }).ToList();

                return new JsonpResult(new
                {
                    Pedidos = data
                }, retornoBusca.QuantidadeRegistros);
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
        public async Task<IActionResult> ObterHistoricoMensagemChatAtendimentoPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem> chatMobileMensagens = repChatMobileMensagem.BuscarPorPedido(codigoPedido);

                dynamic mensagens = new List<dynamic>();
                foreach (var chatMobileMensagem in chatMobileMensagens)
                {
                    mensagens.Add(Servicos.Embarcador.Chat.ChatMensagem.ObterMensagemMontada(chatMobileMensagem, ClienteAcesso.Cliente.Codigo));
                    if (!chatMobileMensagem.MensagemLida && chatMobileMensagem.Remetente.Codigo != this.Usuario.Codigo)
                        Servicos.Embarcador.Chat.ChatMensagem.NotificarMensagemRecebida(chatMobileMensagem, ClienteAcesso.Cliente.Codigo, unitOfWork);
                }

                repChatMobileMensagem.MarcarTodasComoLidasPorRemetentePedido(codigoPedido, this.Usuario.Codigo, DateTime.Now);

                return new JsonpResult(mensagens);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoBuscarHistoricoDoChat);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarMensagemChat()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");
                int motivo = Request.GetIntParam("Motivo");

                string mensagem = string.Empty;
				switch(motivo) 
                {
                    case 1: 
                        mensagem = "Solicitando atendimento de avaria";
                        break;
                    case 2: 
                        mensagem = "Solicitando atendimento de falta de produto";
                        break;
                    case 3: 
                        mensagem = "Solicitando atendimento de sobra de produto";
                        break;
                    default: 
                        mensagem = Request.GetStringParam("Mensagem");
                        break;
                }

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMensagemPromotor repChatMensagemPromotor = new Repositorio.Embarcador.Cargas.ChatMensagemPromotor(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repxmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

                List<Dominio.Entidades.Usuario> usuariosDestinatario = new List<Usuario>();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                if (pedido != null)
                {

                    Dominio.Entidades.Embarcador.Cargas.ChatMobileMensagem chatMobileMensagem = Servicos.Embarcador.Chat.ChatMensagem.EnviarMensagemChat(null, this.Usuario, DateTime.Now, usuariosDestinatario, mensagem, ClienteAcesso.Cliente.Codigo, unitOfWork, new Dominio.Entidades.Embarcador.Pedidos.Pedido() { Codigo = codigoPedido });
                    pedido.PossuiChatAtivo = true;
                    repPedido.Atualizar(pedido);

                    unitOfWork.CommitChanges();

                    servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

                    return new JsonpResult(Servicos.Embarcador.Chat.ChatMensagem.ObterMensagemMontada(chatMobileMensagem, ClienteAcesso.Cliente.Codigo));
                }
                else
                {
                    return new JsonpResult(true, false, "Pedido não encontrado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoEnviarMensagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPedidosChat()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.ChatMobileMensagem repChatMobileMensagem = new Repositorio.Embarcador.Cargas.ChatMobileMensagem(unitOfWork);

                try
                {
                    // Executa metodo de consutla
                    (List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, int totalRegistro) retornoBusca = BuscarPedidosComChat(unitOfWork);

                    // Converte os dados recebidos
                    var lista = RetornaDyn(retornoBusca.listaPedidos, unitOfWork);

                    return new JsonpResult(new
                    {
                        Pedidos = lista
                    }, retornoBusca.totalRegistro);
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
        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedido = Request.GetIntParam("Pedido");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ChatMensagemPromotor repChatMensagemPromotor = new Repositorio.Embarcador.Cargas.ChatMensagemPromotor(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repxmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                List<Dominio.Entidades.Usuario> usuariosDestinatario = new List<Usuario>();

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigoPedido);

                if (pedido != null)
                {
                    pedido.PossuiChatAtivo = false;
                    repPedido.Atualizar(pedido);

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true, pedido.Codigo);
                }
                else
                {
                    return new JsonpResult(true, false, "Pedido não encontrado");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.OcorreuUmaFalhaAoEnviarMensagem);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigosPedidos = (from obj in listaPedidos select obj.Codigo).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> notasFiscaisPedidos = ObterNotasFiscaisPorPedidos(codigosPedidos, unitOfWork);

            var data = (from obj in listaPedidos
                        select new
                        {
                            obj.Codigo,
                            NumeroPedido = obj.NumeroPedidoEmbarcador,
                            CNPJCliente = obj.Destinatario?.CPF_CNPJ_Formatado,
                            DataEntrega = obj.DataAgendamento.HasValue ? obj.DataAgendamento.Value.ToString("dd/MM/yyyy") : "",
                            DataEmissao = obj.DataCadastro.HasValue ? obj.DataCadastro.Value.ToString("dd/MM/yyyy") : "",
                            Destino = obj.Destino?.Descricao,
                            Destinatario = obj.Destinatario?.Descricao,
                            Remetente = obj.Remetente?.Descricao,
                            NotasFiscais = string.Join(", ", (from o in notasFiscaisPedidos where o.CodigoPedido == obj.Codigo select o.NumeroNota)),
                            DataCriacao = obj.DataCriacao?.ToString("dd/MM/yyyy"),
                            PossuiChatAtivo = obj.PossuiChatAtivo,
                        });

            return data.ToList();
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> ObterNotasFiscaisPorPedidos(List<int> codigosPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal> listaNotas = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoNotaFiscal>();
            if (codigosPedidos?.Count > 0)
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                listaNotas = repPedido.BuscarNumeroNotasFiscaisPorPedidos(codigosPedidos);
            }

            return listaNotas;
        }


        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("PossuiChatAtivo", false);
            grid.AdicionarCabecalho("Nº Pedido", "NumeroPedido", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("CNPJ Cliente", "CNPJCliente", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data Entrega", "DataEntrega", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatario", "Destinatario", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("DataCriacao", "DataCriacao", 6, Models.Grid.Align.right, true);

            return grid;
        }

        private (List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos, int QuantidadeRegistros) BuscarPedidos(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedidoCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedidoCliente()
            {
                codigoPedido = Request.GetIntParam("Codigo"),
                NotaFiscal = Request.GetIntParam("NotaFiscal"),
                NumeroPedidoEmbarcador = Request.GetStringParam("NumeroPedido"),
                CNPJDestinatario = Request.GetStringParam("CNPJDestinatario").ObterSomenteNumeros().ToDouble(),
                CodigoCliente = this.Usuario.ClienteFornecedor != null ? this.Usuario.ClienteFornecedor.CPF_CNPJ : 0d,
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial")
            };

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = ObterParametrosConsulta();

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            int quantidadeRegistros = repositorioPedido.ContarConsultaAtendimentoPedidoCliente(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = quantidadeRegistros > 0 ? repositorioPedido.ConsultarAtendimentoPedidoCliente(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            return (pedidos, quantidadeRegistros);
        }

        private (List<Dominio.Entidades.Embarcador.Pedidos.Pedido> Pedidos, int QuantidadeRegistros) BuscarPedidosComChat(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAtendimentoPedido()
            {
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                ApenasPedidosComChat = Request.GetBoolParam("PedidosComChat"),
                Remetente = Request.GetDoubleParam("Remetente"),
                NumeroNotaFiscal = Request.GetIntParam("NotaFiscal")
            };

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = ObterParametrosConsulta();

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            int quantidadeRegistros = repositorioPedido.ContarConsultaAtendimentoPedidoChat(filtrosPesquisa);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = quantidadeRegistros > 0 ? repositorioPedido.ConsultarAtendimentoPedidoChat(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            return (pedidos, quantidadeRegistros);
        }

        private Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta ObterParametrosConsulta()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                DirecaoOrdenar = "desc",
                InicioRegistros = Request.GetIntParam("inicio"),
                LimiteRegistros = Request.GetIntParam("limite"),
                PropriedadeOrdenar = "Codigo"
            };
        }

        #endregion
    }
}
