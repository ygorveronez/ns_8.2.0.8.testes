using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using Repositorio;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class ControleEntrega
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public ControleEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Classes Privadas
        private class ObjetoGeracaoControleEntrega
        {
            //PROPRIEDADE USADA APENAS QUANDO GERA ENTREGAS POR NOTA.
            public Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal PedidoXmlNotaFiscal { get; set; }

            public Dominio.Entidades.Cliente Cliente { get; set; }

            public Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

            //PROPRIEDADE USADA APENAS QUANDO GERA ENTREGAS SEM CLIENTE, APENAS PARA LOCALIDADE.
            public Dominio.Entidades.Localidade Localidade { get; set; }

            public int Ordem { get; set; }

            public int Distancia { get; set; }

            public int TempoBalsa { get; set; }

            public bool Coleta { get; set; }

            public bool Reentrega { get; set; }

            public bool ColetaEquipamento { get; set; }

            public bool Fronteira { get; set; }

            public bool PostoFiscal { get; set; }

            public bool Parqueamento { get; set; }
            public int CodigoCargaEntregaOriginal { get; set; }
            public int CodigoNota { get; set; }

        }
        #endregion

        #region Métodos Públicos

        public static void TrocarCarga(Dominio.Entidades.Embarcador.Cargas.Carga cargaAtual, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> preCargasEntrega = repCargaEntrega.BuscarPorCarga(cargaAtual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> preCargasEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(cargaAtual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga = repCargaPedido.BuscarPorCarga(cargaNova.Codigo);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalCarga = repPedidoXMLNotaFiscal.BuscarPorCarga(cargaNova.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(cargaNova.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargasEntregaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido>();
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in preCargasEntrega)
            {
                cargaEntrega.Carga = cargaNova;
                repCargaEntrega.Atualizar(cargaEntrega);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> pedidosEntregaPedido = (from obj in preCargasEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
                //quando troca pela pre carga olha se existe pelo numero do pedido
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedidoPreCarga in pedidosEntregaPedido)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (from obj in cargaPedidosCarga where obj.Pedido.NumeroPedidoEmbarcador == cargaEntregaPedidoPreCarga.CargaPedido.Pedido.NumeroPedidoEmbarcador select obj).FirstOrDefault();
                    if (cargaPedido != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido();
                        cargaEntregaPedido.CargaEntrega = cargaEntrega;
                        cargaEntregaPedido.CargaPedido = cargaPedido;
                        cargasEntregaPedidos.Add(cargaEntregaPedido);
                    }
                }

            }
            repCargaEntregaPedido.ExcluirPorCarga(cargaNova.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargasEntregaPedidos)
            {
                repCargaEntregaPedido.Inserir(cargaEntregaPedido);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntregaNotas(cargaEntregaPedido.CargaPedido, pedidosXmlNotaFiscalCarga, cargaEntregaPedido.CargaEntrega, unitOfWork);
            }

            if (xmlNotaFiscalProdutos.Count > 0)
            {
                repCargaEntregaProduto.ExcluirTodosPorCarga(cargaNova.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = (from obj in cargasEntregaPedidos select obj.CargaEntrega).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosCargaPedido = (from obj in cargasEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidosCargaPedido)
                        pedidoXMLNotaFiscais.AddRange(Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterNotasFiscalsColetaEntrega(cargaEntregaPedido.CargaPedido, pedidosXmlNotaFiscalCarga, cargaEntrega));

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarProdutosColetaEntregaPorNota(cargaEntrega, pedidoXMLNotaFiscais, xmlNotaFiscalProdutos, unitOfWork);
                }
            }
        }

        public static int ObterParametroRaioEntrega(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork);

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioEntrega);

            return parametro?.Raio ?? 0;

        }

        public static bool ValidarEntregaRaio(Dominio.Entidades.Cliente cliente, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, int raioPadrao)
        {
            if ((cliente == null) || (cliente.Latitude == string.Empty) || (cliente.Longitude == string.Empty))
                return true;

            return Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(cliente, wayPoint.Latitude, wayPoint.Longitude, raioPadrao);
        }

        public static bool ValidarInicioViagemRaio(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Cliente remetente = ObterRemetente(codigoCarga, unitOfWork);

            if (remetente == null)
                return true;

            int raioPadrao = 0;
            if (remetente.RaioEmMetros == 0)
                raioPadrao = ObterParametroRaioInicioViagem(configuracaoEmbarcador, unitOfWork);

            int? raio = remetente.RaioEmMetros != null && remetente.RaioEmMetros > 0 ? remetente.RaioEmMetros : raioPadrao;


            return Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.ValidarEmRaioOuAreaCliente(remetente, wayPoint.Latitude, wayPoint.Longitude, (int)raio);

        }

        public static int ObterRaioEntrega(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            if (cliente == null)
                return 0;


            int raioPadrao = 0;
            if (cliente.RaioEmMetros == 0)
                raioPadrao = ObterParametroRaioEntrega(unitOfWork);

            int? raio = cliente.RaioEmMetros != null && cliente.RaioEmMetros > 0 ? cliente.RaioEmMetros : raioPadrao;

            return raio ?? 0;

        }

        public static void AtualizarInicioViagem(int codigoCarga, DateTime data, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            if (carga == null)
                return;

            carga.DataInicioViagem = data;

            DefinirPrevisaoCargaEntrega(carga, (configControleEntrega?.RecalcularPrevisaoAoIniciarViagem ?? false), true, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

            OcorrenciaEntrega.GerarEventosColetaEntregaCargaAtualizacaoInicioFimViagem(carga, EventoColetaEntrega.IniciaViagem, data, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, auditoria?.OrigemAuditado ?? OrigemAuditado.Sistema, configControleEntrega, unitOfWork);

            repositorioCarga.Atualizar(carga);
        }

        public static bool IniciarViagem(int codigoCarga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega origemSituacaoEntrega, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoria, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repopsitorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.ColetaContainer repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repopsitorioCarga.BuscarPorCodigoFetch(codigoCarga);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            Servicos.Embarcador.Carga.AlertaCarga.InicioViagem alertaInicioViagem = new Servicos.Embarcador.Carga.AlertaCarga.InicioViagem(unitOfWork, unitOfWork.StringConexao);
            Servicos.Embarcador.Pedido.ColetaContainer servColetaContainer = new Pedido.ColetaContainer(unitOfWork);
            Servicos.Embarcador.Carga.MensagemAlertaCarga servicoMensagemAlerta = new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork);
            Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();

            if (carga == null)
                return false;

            if (carga.DataInicioViagem != null)
                return false;

            if (configuracaoEmbarcador.GerarCargaTrajeto)
                Embarcador.Carga.Carga.AtualizarInicioTrajetoCarga(carga, unitOfWork);

            carga.DataInicioViagem = data;
            carga.OrigemSituacao = origemSituacaoEntrega;
            carga.InicioDeViagemNoRaio = false;

            if (wayPoint != null)
            {
                carga.LatitudeInicioViagem = Convert.ToDecimal(wayPoint.Latitude);
                carga.LongitudeInicioViagem = Convert.ToDecimal(wayPoint.Longitude);
                carga.InicioDeViagemNoRaio = ValidarInicioViagemRaio(codigoCarga, wayPoint, configuracaoEmbarcador, unitOfWork);
            }

            if (origemSituacaoEntrega == OrigemSituacaoEntrega.App)
                auditoria.OrigemAuditado = OrigemAuditado.WebServiceMobile;

            if (carga.Redespacho != null)//#44095 caso carga de redespacho verificar se possui coleta container e alterar status para deslocamento carregado.
            {
                Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repositorioColetaContainer.BuscarPorCargaAtual(carga.Codigo);
                if (coletaContainer != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                    parametrosColetaContainer.Status = StatusColetaContainer.EmDeslocamentoCarregado;
                    parametrosColetaContainer.DataAtualizacao = DateTime.Now;
                    parametrosColetaContainer.coletaContainer = coletaContainer;
                    parametrosColetaContainer.OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema;
                    parametrosColetaContainer.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.IniciarViagemRedespacho;

                    servColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                }
            }
            else
            {
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.ContainerTipo containerTipo = retiradaContainer?.ContainerTipo ?? carga.ModeloVeicularCarga?.ContainerTipo ?? carga.Carregamento?.ModeloVeicularCarga?.ContainerTipo;

                if (containerTipo != null && retiradaContainer?.ColetaContainer?.Container != null)
                {
                    servColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro()
                    {
                        coletaContainer = retiradaContainer.ColetaContainer,
                        DataAtualizacao = DateTime.Now,
                        Status = StatusColetaContainer.EmDeslocamentoCarregado,
                        OrigemMonimentacaoContainer = OrigemMovimentacaoContainer.Sistema,
                        InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.AoIniciarViagem

                    });
                }
            }

            DefinirPrevisaoCargaEntrega(carga, (configControleEntrega?.RecalcularPrevisaoAoIniciarViagem ?? false), true, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware, origemSituacaoEntrega);
            repopsitorioCarga.Atualizar(carga);
            OcorrenciaEntrega.GerarOcorrenciaCarga(carga, data, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem, carga.LatitudeInicioViagem, carga.LongitudeInicioViagem, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, origemSituacaoEntrega, configControleEntrega, unitOfWork, auditoria);
            alertaInicioViagem.ProcessarEvento(carga);

            SetarConfiguracaoPedidoOcorrenciaPorCargaEntrega(codigoCarga, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);

            if (configuracaoEmbarcador.PossuiMonitoramento)
                Servicos.Embarcador.Monitoramento.Monitoramento.IniciarMonitoramento(carga, data, configuracaoEmbarcador, auditoria, unitOfWork);

            if (configMonitoramento?.AtualizarStatusViagemMonitoramentoAoIniciarViagem ?? false)
            {
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(carga.Codigo);
                if (monitoramento != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.IdentificaStatusViagemRegistraHistorico(unitOfWork, monitoramento, null, true);
                    if (statusViagem != null)
                        monitoramento.StatusViagem = statusViagem;

                    repMonitoramento.Atualizar(monitoramento);
                }

                //#75532 - Tratativa Automatica para alertas com gatilho de Inicio de Viagem:
                TratarEventoAutomaticamente(carga, TratativaAutomaticaMonitoramentoEvento.InicioViagem, unitOfWork);
            }

            if (configuracaoEmbarcador?.UtilizaAppTrizy ?? false)
                servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(carga.Codigo, data, (double?)carga.LatitudeInicioViagem, (double?)carga.LongitudeInicioViagem, EventoRelevanteMonitoramento.IniciarViagem, unitOfWork);

            return true;
        }

        public static bool FinalizarViagem(int codigoCarga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega, Repositorio.UnitOfWork unitOfWork, bool finalizandoEntregaTransbordo = false)
        {
            return FinalizarViagem(codigoCarga, data, auditado, string.Empty, null, Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, tipoServicoMultisoftware, clienteMultisoftware, OrigemSituacaoEntrega, unitOfWork, finalizandoEntregaTransbordo);
        }

        //Chamado pelo Monitoramento
        public static bool FinalizarViagem(int codigoCarga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string mensagemAuditoria, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            return FinalizarViagem(codigoCarga, data, auditado, mensagemAuditoria, null, Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp, tipoServicoMultisoftware, clienteMultisoftware, OrigemSituacaoEntrega, unitOfWork);
        }

        public static bool FinalizarViagem(int codigoCarga, DateTime data, DateTime dataInicioViagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string mensagemAuditoria, Dominio.Entidades.Usuario responsavelFinalizacaoManual, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.DataTerminoEntrega> listaDataTerminoEntregas = null)
        {
            return FinalizarViagem(codigoCarga, data, auditado, mensagemAuditoria, responsavelFinalizacaoManual, Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, tipoServicoMultisoftware, clienteMultisoftware, OrigemSituacaoEntrega, unitOfWork, false, listaDataTerminoEntregas, dataInicioViagem);
        }

        public static void IniciarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, DateTime data, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemSituacaoEntrega origem, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            IniciarEntrega(cargaEntrega, wayPoint, data, data, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, origem, configuracaoControleEntrega, unitOfWork);
        }

        public static void FinalizarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataEntrega, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos, int motivoRetificacao, string justificativaEntregaForaRaio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, OrigemSituacaoEntrega origemSituacaoEntrega, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, bool finalizandoEntregaTransbordo, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro)
        {
            configuracaoControleEntrega ??= new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).ObterConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
            {
                cargaEntrega = cargaEntrega,
                dataInicioEntrega = dataEntrega,
                dataTerminoEntrega = dataEntrega,
                dataConfirmacao = dataEntrega,
                dataSaidaRaio = null,
                wayPoint = wayPoint,
                wayPointDescarga = null,
                pedidos = pedidos,
                motivoRetificacao = motivoRetificacao,
                justificativaEntregaForaRaio = justificativaEntregaForaRaio,
                motivoFalhaGTA = 0,
                tipoServicoMultisoftware = tipoServicoMultisoftware,
                sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                dadosRecebedor = null,
                OrigemSituacaoEntrega = origemSituacaoEntrega,
                clienteMultisoftware = clienteMultisoftware,
                auditado = auditado,
                FinalizandoEntregaTransbordo = finalizandoEntregaTransbordo,

                configuracaoEmbarcador = configuracao,
                configuracaoControleEntrega = configuracaoControleEntrega,
                tipoOperacaoParametro = tipoOperacaoParametro ?? new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0),
                TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega?.TornarFinalizacaoDeEntregasAssincrona ?? false
            };

            FinalizarEntrega(parametros, unitOfWork);
        }

        public static void FinalizarEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona repositorioCargaEntregaFinalizacaoAssincrona = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona(unitOfWork);

            if (parametros.cargaEntrega == null)
                throw new ServicoException("Coleta/Entrega não encontrada");

            parametros.retiradaContainer ??= repositorioRetiradaContainer.BuscarPorCarga(parametros.Container);
            parametros.auditado ??= new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = TipoAuditado.Sistema, OrigemAuditado = OrigemAuditado.Sistema };

            if (parametros.ExecutarValidacoes)
                ExecutarValidacoesFinalizacaoEntrega(parametros, unitOfWork);

            if (parametros.TornarFinalizacaoDeEntregasAssincrona)
            {
                parametros.dataConfirmacao = (parametros.dataConfirmacao != null && parametros.dataConfirmacao != DateTime.MinValue) ? parametros.dataConfirmacao : DateTime.Now;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona cargaEntregaFinalizacaoAssincrona = repositorioCargaEntregaFinalizacaoAssincrona.BuscarPorCargaEntrega(parametros.cargaEntrega.Codigo) ?? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona();

                cargaEntregaFinalizacaoAssincrona.ParametrosFinalizacao = Newtonsoft.Json.JsonConvert.SerializeObject(ConverterParametrosFinalizacaoAssincrona(parametros));
                cargaEntregaFinalizacaoAssincrona.SituacaoProcessamento = SituacaoProcessamentoIntegracao.AguardandoProcessamento;
                cargaEntregaFinalizacaoAssincrona.NumeroTentativas = 0;

                if (cargaEntregaFinalizacaoAssincrona.Codigo == 0)
                {
                    cargaEntregaFinalizacaoAssincrona.DataInclusao = DateTime.Now;
                    cargaEntregaFinalizacaoAssincrona.DetalhesProcessamento = string.Empty;
                    repositorioCargaEntregaFinalizacaoAssincrona.Inserir(cargaEntregaFinalizacaoAssincrona);
                }
                else
                    repositorioCargaEntregaFinalizacaoAssincrona.Atualizar(cargaEntregaFinalizacaoAssincrona);

                parametros.cargaEntrega.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmFinalizacao;
                parametros.cargaEntrega.OrigemSituacao = parametros.OrigemSituacaoEntrega;
                parametros.cargaEntrega.CargaEntregaFinalizacaoAssincrona = cargaEntregaFinalizacaoAssincrona;
                parametros.dataConfirmacaoEntrega = DateTime.Now;
                parametros.OrdemRealizada = ObterQuantidadeEntregaRealizada(parametros.cargaEntrega.Carga.Codigo, unitOfWork);
                repCargaEntrega.Atualizar(parametros.cargaEntrega);
                Servicos.Auditoria.Auditoria.Auditar(parametros.auditado, parametros.cargaEntrega, Localization.Resources.Cargas.ControleEntrega.SituacaoAlteradaParaFinalizacaoAssincrona, unitOfWork);
            }
            else
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);
                Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal repMotivoFalhaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoFalhaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaMDFe repMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork);

                Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Servicos.Embarcador.Pedido.ColetaContainer servicoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega servicoOcorrenciaEntrega = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(unitOfWork, parametros.tipoServicoMultisoftware);

                if (parametros.cargaEntrega.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova ||
                parametros.cargaEntrega.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    AdicionarHandlingUnitIds(parametros.handlingUnitIds, parametros.cargaEntrega.Carga, unitOfWork);

                AdicionarChavesNFeNaCargaEntrega(parametros.chavesNFe, parametros.cargaEntrega, parametros.tipoServicoMultisoftware, parametros.configuracaoEmbarcador, parametros.auditado, unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> listaNotasCargaEntrega = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(parametros.cargaEntrega.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscaisCargaEntrega = listaNotasCargaEntrega != null && listaNotasCargaEntrega.Count > 0 ? listaNotasCargaEntrega.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList() : new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                AtualizarStatusNotaFiscal(listaNotasFiscaisCargaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotaFiscalSituacaoGatilho.ConfirmacaoEntregaNota, parametros.auditado, unitOfWork);

                if (!parametros.cargaEntrega.Carga.DataInicioViagem.HasValue && !parametros.cargaEntrega.ColetaEquipamento && PermiteIniciarFinalizarViagem(parametros.cargaEntrega, parametros.tipoServicoMultisoftware, parametros.configuracaoControleEntrega, parametros.configuracaoTipoOperacaoControleEntrega, false))
                {
                    Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, parametros.configuracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada && parametros.cargaEntrega.Carga.CargaAgrupamento != null ? parametros.cargaEntrega.Carga.CargaAgrupamento : parametros.cargaEntrega.Carga);
                    if (clienteOrigem?.CPF_CNPJ != parametros.cargaEntrega.Cliente?.CPF_CNPJ)
                    {
                        DateTime dataInicioViagem = Servicos.Embarcador.Monitoramento.Monitoramento.ObterMenorDataDePosicaoDoMonitoramento(unitOfWork, parametros.cargaEntrega.Carga, parametros.dataInicioEntrega, parametros.configuracaoEmbarcador);

                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(parametros.cargaEntrega.Carga.Codigo, dataInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, parametros.wayPoint, parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, parametros.clienteMultisoftware, parametros.auditado, unitOfWork))
                            Servicos.Auditoria.Auditoria.Auditar(parametros.auditado, parametros.cargaEntrega.Carga, Localization.Resources.Cargas.ControleEntrega.InicioViagemInformadoAutomaticamente, unitOfWork);
                    }
                }

                if (parametros.dataSaidaRaio != null)
                    parametros.cargaEntrega.DataSaidaRaio = parametros.dataSaidaRaio;

                if (parametros.cargaEntrega.DataSaidaRaio != null && parametros.cargaEntrega.DataEntradaRaio == null)
                    parametros.cargaEntrega.DataEntradaRaio = parametros.cargaEntrega.DataSaidaRaio;

                if (!parametros.cargaEntrega.DataInicio.HasValue)
                {
                    if (parametros.dataInicioEntrega != null)
                        parametros.cargaEntrega.DataInicio = parametros.dataInicioEntrega;
                    else
                    {
                        if (parametros.cargaEntrega.DataEntradaRaio.HasValue)
                            parametros.cargaEntrega.DataInicio = parametros.cargaEntrega.DataEntradaRaio.Value;
                        else
                            parametros.cargaEntrega.DataInicio = parametros.dataTerminoEntrega;
                    }
                }

                if (parametros.wayPoint != null)
                {
                    parametros.cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(parametros.wayPoint.Latitude);
                    parametros.cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(parametros.wayPoint.Longitude);
                }
                else
                {
                    parametros.cargaEntrega.LatitudeFinalizada = null;
                    parametros.cargaEntrega.LongitudeFinalizada = null;
                }

                // Descarga é o mesmo que a posição Finalizada. Isso pode ser removido no futuro, caso o app seja refatorado para não mandar mais o waypointDescarga
                if (parametros.wayPointDescarga != null)
                {
                    parametros.cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(parametros.wayPointDescarga.Latitude);
                    parametros.cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(parametros.wayPointDescarga.Longitude);
                }

                //Confirmação de chegada
                if (parametros.wayPointConfirmacaoChegada != null)
                {
                    parametros.cargaEntrega.LatitudeConfirmacaoChegada = Convert.ToDecimal(parametros.wayPointConfirmacaoChegada.Latitude);
                    parametros.cargaEntrega.LongitudeConfirmacaoChegada = Convert.ToDecimal(parametros.wayPointConfirmacaoChegada.Longitude);
                }

                parametros.cargaEntrega.DistanciaAteDestino = null;
                parametros.cargaEntrega.Observacao = parametros.observacao ?? "";

                if (!parametros.cargaEntrega.DataFim.HasValue || parametros.cargaEntrega.DataRejeitado.HasValue) //entrega pode ter sido rejeitada... (deve substituir data confirmacao mesmo se ja existe)
                    parametros.cargaEntrega.DataFim = parametros.dataTerminoEntrega;
                repositorioPedido.DefinirDataEntregaPorCargaEntrega(parametros.cargaEntrega.Codigo, parametros.dataTerminoEntrega);

                parametros.cargaEntrega.DataConfirmacao = (parametros.dataConfirmacao != null && parametros.dataConfirmacao != DateTime.MinValue) ? parametros.dataConfirmacao : DateTime.Now;

                if (!parametros.cargaEntrega.DataConfirmacaoEntregaOriginal.HasValue)
                    parametros.cargaEntrega.DataConfirmacaoEntregaOriginal = (parametros.dataConfirmacao != null && parametros.dataConfirmacao != DateTime.MinValue) ? parametros.dataConfirmacao : DateTime.Now;

                parametros.cargaEntrega.DataRejeitado = null;
                parametros.cargaEntrega.PermitirEntregarMaisTarde = false;
                parametros.cargaEntrega.JustificativaEntregaForaRaio = Utilidades.String.Left(parametros.justificativaEntregaForaRaio, 300);
                parametros.cargaEntrega.MotoristaACaminho = false;

                //#60188 - Tratamento para manter a OrdemRealizada, mesmo quando reprocessar finalização assíncrona.
                if (parametros.OrdemRealizada > 0)
                    parametros.cargaEntrega.OrdemRealizada = parametros.OrdemRealizada;
                else if (SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(parametros.cargaEntrega.Situacao))
                    parametros.cargaEntrega.OrdemRealizada = ObterQuantidadeEntregaRealizada(parametros.cargaEntrega.Carga.Codigo, unitOfWork);

                parametros.cargaEntrega.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue;

                if (parametros.dataConfirmacaoEntrega != null)
                    parametros.cargaEntrega.DataConfirmacaoEntrega = parametros.dataConfirmacaoEntrega;
                else
                    parametros.cargaEntrega.DataConfirmacaoEntrega = DateTime.Now;

                parametros.cargaEntrega.OrigemSituacao = parametros.OrigemSituacaoEntrega;
                if (parametros.OrigemSituacaoFimViagem != null)
                {
                    parametros.cargaEntrega.OrigemSituacaoFimViagem = parametros.OrigemSituacaoFimViagem;
                }

                parametros.cargaEntrega.ResponsavelFinalizacaoManual = parametros.auditado?.Usuario != null ? parametros.auditado.Usuario : parametros.motorista;

                parametros.cargaEntrega.StatusPrazoEntrega = ObterEntregaEmJanela(parametros.cargaEntrega, parametros.dataTerminoEntrega, unitOfWork);
                parametros.cargaEntrega.EntregueNoRaio = ValidarEntregaRaio(parametros.cargaEntrega.Cliente, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = parametros.cargaEntrega.LatitudeFinalizada != null ? Convert.ToDouble(parametros.cargaEntrega.LatitudeFinalizada) : 0, Longitude = parametros.cargaEntrega.LongitudeFinalizada != null ? Convert.ToDouble(parametros.cargaEntrega.LongitudeFinalizada) : 0 }, parametros.configuracaoEmbarcador.RaioPadrao);
                parametros.cargaEntrega.AvaliacaoColetaEntrega = parametros.avaliacaoColetaEntrega;

                if (parametros.motivoRetificacao > 0)
                    parametros.cargaEntrega.MotivoRetificacaoColeta = repMotivoRetificacaoColeta.BuscarPorCodigo(parametros.motivoRetificacao);

                if (parametros.motivoFalhaGTA > 0)
                    parametros.cargaEntrega.MotivoFalhaGTA = repMotivoFalhaGTA.BuscarPorCodigo(parametros.motivoFalhaGTA, false);

                if (parametros.motivoFalhaNotaFiscal > 0)
                    parametros.cargaEntrega.MotivoFalhaNotaFiscal = repMotivoFalhaNotaFiscal.BuscarPorCodigo(parametros.motivoFalhaNotaFiscal);

                // Dados Recebedor
                if (parametros.dadosRecebedor != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor dadosRecebedorEntity = buildDadosRecebedor(parametros.dadosRecebedor, parametros.cargaEntrega, unitOfWork);
                    parametros.cargaEntrega.DadosRecebedor = dadosRecebedorEntity;
                }

                //validação para quando o veiculo ja tenha entrado e saido da entrega, porem a confirmacao do motorista acontece posteriormente; #60100
                if (parametros.cargaEntrega.DataConfirmacao.HasValue && parametros.cargaEntrega.DataEntradaRaio.HasValue && parametros.cargaEntrega.DataSaidaRaio.HasValue && parametros.cargaEntrega.DataEntradaRaio.Value > parametros.cargaEntrega.DataSaidaRaio.Value)
                    parametros.cargaEntrega.DataSaidaRaio = parametros.cargaEntrega.DataConfirmacao;

                DateTime? dataRealizada = ObterDataPrazoParada(parametros.cargaEntrega, parametros.cargaEntrega.Carga.TipoOperacao?.ConfiguracaoControleEntrega?.DataRealizacaoDoEvento ?? TipoDataCalculoParadaNoPrazo.DataConfirmacao);
                DateTime? dataPrevisao = ObterDataPrazoParada(parametros.cargaEntrega, parametros.cargaEntrega.Carga.TipoOperacao?.ConfiguracaoControleEntrega?.DataPrevistaDoEvento ?? TipoDataCalculoParadaNoPrazo.DataPrevista);
                if (dataRealizada.HasValue && dataPrevisao.HasValue)
                    parametros.cargaEntrega.RealizadaNoPrazo = dataRealizada <= dataPrevisao;

                repCargaEntrega.Atualizar(parametros.cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(parametros.cargaEntrega, repCargaEntrega, unitOfWork);

                //Movido para antes de finalizar o monitoramento (finalizarViagem), pois é necessário do monitoramentoVeiculo em aberto.
                if (parametros.configuracaoEmbarcador?.UtilizaAppTrizy ?? false)
                    servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(parametros.cargaEntrega.Carga.Codigo, parametros.cargaEntrega.DataConfirmacao ?? DateTime.Now, (double?)parametros.cargaEntrega.LatitudeFinalizada, (double?)parametros.cargaEntrega.LongitudeFinalizada, EventoRelevanteMonitoramento.ConfirmarEntrega, unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordoCarga = repTransbordo.BuscarPorCargaGerada(parametros.cargaEntrega.Carga?.Codigo ?? 0);
                if (transbordoCarga != null && transbordoCarga.Carga != null && transbordoCarga.Carga.SituacaoCarga != SituacaoCarga.Cancelada && transbordoCarga.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                {
                    //devemos buscar a entrega transbordada da entrega de transbordo que esta sendo finalizada e finalizar;
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregaTransbordada = repCargaEntrega.BuscarCargaeCliente(transbordoCarga.Carga.Codigo, parametros.cargaEntrega.Cliente?.CPF_CNPJ ?? 0);

                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entregaTransbordada in cargaEntregaTransbordada)
                    {
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(entregaTransbordada, (parametros.dataConfirmacao.HasValue && parametros.dataConfirmacao != DateTime.MinValue) ? parametros.dataConfirmacao.Value : DateTime.Now, null, null, 0, "", parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, parametros.auditado, parametros.OrigemSituacaoEntrega, parametros.clienteMultisoftware, unitOfWork, true, parametros.configuracaoControleEntrega, parametros.tipoOperacaoParametro);

                        if (parametros.auditado != null)
                            Servicos.Auditoria.Auditoria.Auditar(parametros.auditado, entregaTransbordada, null, "Finalizou entrega pela entrega de transbordo", unitOfWork);
                    }
                }

                DefinirPrevisaoCargaEntrega(parametros.cargaEntrega.Carga, parametros.configuracaoEmbarcador, unitOfWork, parametros.tipoServicoMultisoftware, parametros.configuracaoControleEntrega);

                // Criar automaticamente o Lote de Comprovante de Entregas
                if ((parametros.cargaEntrega.Carga?.TipoOperacao?.GerarLoteEntregaAutomaticamente ?? false) && !parametros.cargaEntrega.Coleta)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.LoteComprovanteEntrega.CriarAutomaticamenteLoteByCargaEntrega(parametros.cargaEntrega, unitOfWork);
                }

                DateTime dataEventoConfirmacao = parametros.cargaEntrega.Carga?.TipoOperacao?.RealizarBaixaEntradaNoRaio == true
                    ? parametros.cargaEntrega.DataEntradaRaio ?? parametros.cargaEntrega.DataConfirmacao.Value
                    : parametros.cargaEntrega.DataConfirmacao.Value;

                bool validarApenasEntregas = (parametros.tipoOperacaoParametro?.ConfiguracaoControleEntrega?.ConfirmarColetasQuandoTodasAsEntregasDaCargaForemConcluidas ?? false);
                bool naoPossuiEntregasPendentes = NaoPossuiEntregasPendentes(parametros.cargaEntrega.Carga, unitOfWork, validarApenasEntregas);

                if (repConfiguracaoOcorrenciaEntrega.ExisteConfiguracaoOcorrenciaPorEvento(EventoColetaEntrega.UltimaConfirmacao))
                {
                    EventoColetaEntrega eventoAExecutar = naoPossuiEntregasPendentes ? EventoColetaEntrega.UltimaConfirmacao : EventoColetaEntrega.Confirma;
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(parametros.cargaEntrega, dataEventoConfirmacao, eventoAExecutar, parametros.cargaEntrega.LatitudeFinalizada, parametros.cargaEntrega.LongitudeFinalizada, parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, parametros.clienteMultisoftware, parametros.OrigemSituacaoEntrega, parametros.configuracaoControleEntrega, parametros.auditado);
                }
                else
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(parametros.cargaEntrega, dataEventoConfirmacao, EventoColetaEntrega.Confirma, parametros.cargaEntrega.LatitudeFinalizada, parametros.cargaEntrega.LongitudeFinalizada, parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, parametros.clienteMultisoftware, parametros.OrigemSituacaoEntrega, parametros.configuracaoControleEntrega, parametros.auditado);

                if (repositorioXMLNotaFiscal.ExisteNotasFiscaisPorEntrega(parametros.cargaEntrega.Codigo, null, SituacaoNotaFiscal.AgReentrega))//Remover finalizando a tarefa #55267
                    Log.TratarErro($"Alterando situação para Entregue da cargaEntrega {parametros.cargaEntrega.Codigo}", "FinalizarEntrega");


                if (parametros.cargaEntrega.Coleta)
                    InformarProdutosColetados(parametros.cargaEntrega, parametros.pedidos, parametros.auditado, unitOfWork);
                else
                {
                    if (parametros.tipoOperacaoParametro?.ConfiguracaoControleEntrega?.AlterarSituacaoEntregaNFeParaDevolvidaAoConfirmarEntrega ?? false)
                        ProcessarSituacaoEntregaXMLNotaFiscal(parametros.cargaEntrega, SituacaoNotaFiscal.Devolvida, unitOfWork);
                    else
                        ProcessarSituacaoEntregaXMLNotaFiscal(parametros.cargaEntrega, SituacaoNotaFiscal.Entregue, unitOfWork);
                }

                if ((parametros.configuracaoControleEntrega?.EncerrarMDFeAutomaticamenteAoFinalizarEntregas ?? false))
                    EncerrarMdfeComEntregaFinalizada(parametros, validarApenasEntregas, repMDFe, unitOfWork);

                if (naoPossuiEntregasPendentes)
                {
                    if (!parametros.FinalizandoViagem && PermiteIniciarFinalizarViagem(parametros.cargaEntrega, parametros.tipoServicoMultisoftware, parametros.configuracaoControleEntrega, parametros.configuracaoTipoOperacaoControleEntrega, true, parametros.auditado, unitOfWork))
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaCargaEntrega = repCargaEntrega.BuscarUltimaCargaEntregaRealizada(parametros.cargaEntrega.Carga.Codigo);
                        DateTime dataEventoFimViagem = ultimaCargaEntrega != null && ultimaCargaEntrega.DataConfirmacao.HasValue && ultimaCargaEntrega.DataConfirmacao.Value >= dataEventoConfirmacao ? ultimaCargaEntrega.DataConfirmacao.Value : dataEventoConfirmacao;

                        if (FinalizarViagem(parametros.cargaEntrega.Carga.Codigo, dataEventoFimViagem, parametros.auditado, string.Empty, null, parametros.sistemaOrigem, parametros.tipoServicoMultisoftware, parametros.clienteMultisoftware, parametros.OrigemSituacaoEntrega, unitOfWork, parametros.FinalizandoEntregaTransbordo))
                            Servicos.Auditoria.Auditoria.Auditar(parametros.auditado, parametros.cargaEntrega.Carga, Localization.Resources.Cargas.ControleEntrega.FimViagemInformadoAutomaticamente, unitOfWork);

                        // Notifica para outros motoristas da carga que uma ação foi realizada
                        if (parametros.motorista != null)
                        {
                            Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                            serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(parametros.cargaEntrega.Carga, null, parametros.motorista, TipoEventoAlteracaoCargaPorOutroMotorista.FimViagem);
                        }
                    }
                    else
                        Log.TratarErro($"Finalizando entrega {parametros.cargaEntrega.Codigo} pelo processo de finalização da viagem.", "FinalizarEntrega");
                }
                else
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega confOcorrencia = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.RecalculoPrevisao)?.FirstOrDefault();
                    if (confOcorrencia != null)
                    {
                        List<SituacaoEntrega> situacoes = new List<SituacaoEntrega>
                        {
                            SituacaoEntrega.Revertida,
                            SituacaoEntrega.NaoEntregue,
                            SituacaoEntrega.EmCliente,
                        };

                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasPendentes = repCargaEntrega.BuscarPorCargaeSituacao(parametros.cargaEntrega.Carga.Codigo, situacoes);

                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in entregasPendentes)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenada = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();
                            configOcorrenciaCoordenada.Latitude = parametros.cargaEntrega.LatitudeFinalizada;
                            configOcorrenciaCoordenada.Longitude = parametros.cargaEntrega.LongitudeFinalizada;
                            configOcorrenciaCoordenada.DataExecucao = DateTime.Now;
                            configOcorrenciaCoordenada.DataPosicao = parametros.dataConfirmacao;
                            configOcorrenciaCoordenada.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
                            configOcorrenciaCoordenada.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
                            configOcorrenciaCoordenada.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";

                            Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaEntregaRecalculoPrevisao(cargaEntrega, DateTime.Now, confOcorrencia.TipoDeOcorrencia, parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, null, configOcorrenciaCoordenada, parametros.OrigemSituacaoEntrega, parametros.configuracaoControleEntrega, unitOfWork, parametros.auditado);
                        }
                    }
                }


                RealizouTodasColetas(parametros.cargaEntrega.Carga, parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, servicoHubCarga, serCarga, unitOfWork);

                Servicos.Embarcador.Carga.AlertaCarga.ConfirmacaoColetaEntrega alertaConfirmacaoColetaEntrega = new Servicos.Embarcador.Carga.AlertaCarga.ConfirmacaoColetaEntrega(unitOfWork, unitOfWork.StringConexao);
                alertaConfirmacaoColetaEntrega.ProcessarEvento(parametros.cargaEntrega);

                //#75532 - Tratativa Automatica para alertas com gatilho de Confirmação da Entrega:
                TratarEventoAutomaticamente(parametros.cargaEntrega.Carga, TratativaAutomaticaMonitoramentoEvento.ConfirmacaoEntrega, unitOfWork);

                if (!parametros.cargaEntrega.Coleta)
                {
                    Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, parametros.auditado);
                    servicoMovimentacaoPallet.InformarMudancaResponsavel(parametros.cargaEntrega);
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMovimentacaoContainer OrigemMovimentacaoContainer = OrigemMovimentacaoContainer.UsuarioInterno;
                if (parametros.tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    OrigemMovimentacaoContainer = OrigemMovimentacaoContainer.PortalTransportador;

                //carga PARA ENTREGA do container da carga de Coleta;
                if ((parametros.cargaEntrega.Carga.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false) && !parametros.cargaEntrega.Coleta && parametros.tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer;
                    if (parametros.cargaEntrega.Carga?.TipoOperacao?.OperacaoTransferenciaContainer ?? false)
                        coletaContainer = repColetaContainer.BuscarPorCargaAtual(parametros.cargaEntrega.Carga.Codigo);
                    else
                        coletaContainer = repColetaContainer.BuscarPorCargaDeColeta(parametros.cargaEntrega.Carga.Codigo);

                    if (coletaContainer?.Container != null)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                        parametrosColetaContainer.LocalAtual = parametros.cargaEntrega.Cliente;
                        parametrosColetaContainer.Status = StatusColetaContainer.EmAreaEsperaVazio;
                        parametrosColetaContainer.DataAtualizacao = DateTime.Now;
                        parametrosColetaContainer.coletaContainer = coletaContainer;
                        parametrosColetaContainer.Usuario = parametros.auditado?.Usuario;
                        parametrosColetaContainer.AreaEsperaVazio = parametros.cargaEntrega.Cliente;
                        parametrosColetaContainer.OrigemMonimentacaoContainer = OrigemMovimentacaoContainer;
                        parametrosColetaContainer.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.ConfirmarEntregaContainer;

                        servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                    }
                }
                else if (!parametros.cargaEntrega.Coleta)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer> coletaContainers = new List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainer>();

                    if (parametros.cargaEntrega.Carga.CargaAgrupada)
                    {
                        coletaContainers = repColetaContainer.BuscarPorCargaAgrupadaAtual(parametros.cargaEntrega.Carga.Codigo);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorCargaAtual(parametros.cargaEntrega.Carga.Codigo);
                        if (coletaContainer != null)
                            coletaContainers.Add(coletaContainer);
                    }
                    //carga PARA ENTREGA de container em area de redex OU cliente

                    Dominio.Entidades.Cliente cliente = parametros.cargaEntrega.Cliente;
                    if (parametros.ClienteAreaRedex > 0)
                        cliente = repCliente.BuscarPorCPFCNPJ(parametros.ClienteAreaRedex);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer in coletaContainers)
                    {
                        if (coletaContainer?.Container != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                            if (cliente.AreaRedex)
                            {
                                parametrosColetaContainer.Status = StatusColetaContainer.EmAreaEsperaCarregado;
                                GerarCargaRedespachoAreaRedex(parametros.cargaEntrega.Carga, parametros.cargaEntrega, parametros.cargaEntrega.Cliente, cliente, coletaContainer, parametros.tipoServicoMultisoftware, parametros.configuracaoEmbarcador, unitOfWork);
                            }
                            else
                            {
                                parametrosColetaContainer.LocalEmbarque = cliente;
                                parametrosColetaContainer.DataEmbarque = DateTime.Now;
                                parametrosColetaContainer.Status = StatusColetaContainer.Porto;
                            }

                            parametrosColetaContainer.LocalAtual = cliente;
                            parametrosColetaContainer.DataAtualizacao = DateTime.Now;
                            parametrosColetaContainer.coletaContainer = coletaContainer;
                            parametrosColetaContainer.Usuario = parametros.auditado?.Usuario;
                            parametrosColetaContainer.OrigemMonimentacaoContainer = OrigemMovimentacaoContainer;
                            parametrosColetaContainer.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.ConfirmarEntregaContainer;

                            servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);
                        }
                    }
                }

                if (parametros.cargaEntrega.Coleta && parametros.cargaEntrega.ColetaEquipamento && parametros.Container > 0)
                {
                    //carga PARA COLETA de container
                    Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer = repColetaContainer.BuscarPorLocalAtualEStatusEContainer(parametros.cargaEntrega.Cliente?.CPF_CNPJ ?? 0, StatusColetaContainer.EmAreaEsperaVazio, parametros.Container);
                    Dominio.Entidades.Embarcador.Pedidos.Container container = repContainer.BuscarPorCodigo(parametros.Container);

                    if (coletaContainer == null)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainerValidacao = repColetaContainer.BuscarPorContainerNaoEmbarcadoENaoCancelado(parametros.Container);
                        if (coletaContainerValidacao != null)
                            throw new ServicoException("Não é possível finalizar a coleta pois o container " + container.Numero + " está vinculado a carga " + coletaContainerValidacao.CargaAtual.CodigoCargaEmbarcador + " com status " + coletaContainerValidacao.Status.ObterDescricao() + " regularize o status do container para Em Área Espera Vazio e tente novamente");
                    }

                    if (coletaContainer == null)
                    {
                        coletaContainer = new Dominio.Entidades.Embarcador.Pedidos.ColetaContainer();
                        coletaContainer.Container = container;
                        coletaContainer.DataColeta = parametros.DataColetaContainer.HasValue ? parametros.DataColetaContainer : DateTime.Now;
                        coletaContainer.Filial = parametros.cargaEntrega.Carga.Filial;
                        coletaContainer.CargaDeColeta = parametros.cargaEntrega.Carga;
                        coletaContainer.CargaAtual = parametros.cargaEntrega.Carga;
                        coletaContainer.LocalColeta = parametros.cargaEntrega.Cliente;
                        coletaContainer.LocalAtual = parametros.cargaEntrega.Cliente;
                        Dominio.Entidades.Cliente ClienteArmador = parametros.cargaEntrega.Cliente;

                        if (ClienteArmador != null && ClienteArmador.Armador)
                        {
                            Repositorio.Embarcador.Pessoas.PessoaArmador repPessoaArmador = new Repositorio.Embarcador.Pessoas.PessoaArmador(unitOfWork);
                            Dominio.Entidades.Embarcador.Pessoas.PessoaArmador pessoaArmador = repPessoaArmador.BuscarPorPessoaETipoContainer(ClienteArmador.CPF_CNPJ, container.ContainerTipo?.Codigo ?? 0, DateTime.Now);
                            if (pessoaArmador == null)
                                pessoaArmador = repPessoaArmador.BuscarPorPessoaETipoContainer(ClienteArmador.CPF_CNPJ, container.ContainerTipo?.Codigo ?? 0);

                            coletaContainer.FreeTime = pessoaArmador?.DiasFreetime ?? 0;
                            coletaContainer.ValorDiaria = pessoaArmador?.ValorDariaAposFreetime ?? 0;
                        }

                        repColetaContainer.Inserir(coletaContainer);
                    }
                    else
                    {
                        if (coletaContainer.LocalColeta == null)
                            coletaContainer.LocalColeta = parametros.cargaEntrega.Cliente;

                        coletaContainer.LocalAtual = parametros.cargaEntrega.Cliente;
                        coletaContainer.CargaAtual = parametros.cargaEntrega.Carga;

                        repColetaContainer.Atualizar(coletaContainer);
                    }

                    servicoColetaContainer.ValidarTipoOperacaoCargaDuplicado(coletaContainer);

                    Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro parametrosColetaContainer = new Dominio.ObjetosDeValor.Embarcador.Pedido.AtualizarColetaContainerParametro();
                    parametrosColetaContainer.Status = StatusColetaContainer.EmDeslocamentoCarregamento;
                    parametrosColetaContainer.DataAtualizacao = DateTime.Now;
                    parametrosColetaContainer.coletaContainer = coletaContainer;
                    parametrosColetaContainer.Usuario = parametros.auditado?.Usuario;
                    parametrosColetaContainer.OrigemMonimentacaoContainer = OrigemMovimentacaoContainer;
                    parametrosColetaContainer.InformacaoOrigemMonimentacaoContainer = InformacaoOrigemMovimentacaoContainer.ConfirmarColetaContainer;

                    servicoColetaContainer.AtualizarSituacaoColetaContainerEGerarHistorico(parametrosColetaContainer);

                    parametros.retiradaContainer.ColetaContainer = coletaContainer;
                    repositorioRetiradaContainer.Atualizar(parametros.retiradaContainer);

                    if (container != null && container.ClienteArmador == null)
                    {
                        container.ClienteArmador = parametros.cargaEntrega.Cliente;
                        repContainer.Atualizar(container, parametros.auditado);
                    }

                    new Servicos.Embarcador.Carga.MensagemAlertaCarga(unitOfWork).Confirmar(parametros.cargaEntrega.Carga, TipoMensagemAlerta.CargaSemInformacaoContainer);//remover mensagem
                }

                if (parametros.auditado != null)
                    Auditoria.Auditoria.Auditar(parametros.auditado, parametros.cargaEntrega, null, !string.IsNullOrEmpty(parametros.auditado.Texto) ? parametros.auditado.Texto : Localization.Resources.Cargas.ControleEntrega.SolicitouConfirmacao, unitOfWork);

                if (parametros.configuracaoEmbarcador.LiberarPagamentoAoConfirmarEntrega)
                    LiberarPagamentoPorEntrega(parametros.cargaEntrega, false, parametros.configuracaoEmbarcador, unitOfWork);

                if (parametros.tipoOperacaoParametro?.ConfiguracaoEmissao?.LiberarDocumentosEmitidosQuandoEntregaForConfirmada ?? false)
                    LiberacaoDocumentosBloqueadosConfirmarEntrega(parametros.cargaEntrega, unitOfWork);

                EnviarOcorrenciaParaVendedor(unitOfWork, parametros.cargaEntrega);

                if (repositorioXMLNotaFiscal.ExisteNotasFiscaisPorEntrega(parametros.cargaEntrega.Codigo, null, SituacaoNotaFiscal.AgReentrega))//Remover finalizando a tarefa #55267
                    Log.TratarErro($"Não atualizou situação das notas fiscais para Entregue da cargaEntrega {parametros.cargaEntrega.Codigo}", "FinalizarEntrega");

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaEntrega = repCargaEntregaPedido.BuscarPedidosPorEntrega(parametros.cargaEntrega.Codigo);
                if (pedidosCargaEntrega != null && pedidosCargaEntrega.Count > 0)
                {
                    Servicos.Embarcador.Logistica.AgendamentoEntregaPedido svcAgendamentoPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, parametros.configuracaoEmbarcador, parametros.tipoServicoMultisoftware, parametros.auditado, parametros.auditado?.Usuario);
                    svcAgendamentoPedido.GerarIntegracaoDriveIn(unitOfWork, pedidosCargaEntrega, parametros.clienteMultisoftware, GatilhoIntegracaoMondelezDrivin.ConfirmacaoEntrega);
                }

                new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, parametros.configuracaoQualidadeEntrega).ProcessarRegrasDeQualidadeDeEntrega(parametros.cargaEntrega);

                if ((parametros.cargaEntrega?.Carga?.TipoOperacao?.ConfiguracaoTipoOperacaoChamado?.FinalizarAutomaticamenteAtendimentoNfeEntregue ?? false))
                {
                    List<int> numerosNotasFiscais = null;
                    Servicos.Embarcador.Chamado.Chamado servicoChamado = new Servicos.Embarcador.Chamado.Chamado(unitOfWork);

                    if (parametros.cargaEntrega.NotasFiscais != null && parametros.cargaEntrega.NotasFiscais.Count > 0)
                        numerosNotasFiscais = parametros.cargaEntrega.NotasFiscais.Select(nf => nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).ToList();

                    servicoChamado.FinalizarAtendimentosEmAberto(numerosNotasFiscais, parametros.cargaEntrega, parametros.auditado, "Entrega Confirmada", unitOfWork);
                }


            }

        }

        private static void EncerrarMdfeComEntregaFinalizada(FinalizarEntregaParametros parametros, bool validarApenasEntregas, Repositorio.Embarcador.Cargas.CargaMDFe repMDFe, UnitOfWork unitOfWork)
        {

            try
            {
                string uf = parametros.cargaEntrega.Cliente.Localidade.Estado.Sigla ?? parametros.cargaEntrega.Localidade.Estado.Sigla;
                int codigoLocalidade = parametros.cargaEntrega?.Cliente?.Localidade.Codigo ?? parametros.cargaEntrega.Localidade.Codigo;

                if (string.IsNullOrEmpty(uf))
                {
                    Log.TratarErro($"Não foi possivel encerrar MDFe ao finalizar entrega {parametros.cargaEntrega.Codigo} -- UF não encontrada", "FinalizarEntrega");
                    return;
                }

                bool naoPossuiEntregasPendentesMesmoEstado = NaoPossuiEntregasPendentes(parametros.cargaEntrega.Carga, unitOfWork, validarApenasEntregas, uf);

                if (!naoPossuiEntregasPendentesMesmoEstado)
                {
                    Log.TratarErro($"Não foi possivel encerrar MDFe ao finalizar entrega {parametros.cargaEntrega.Codigo} -- Existe entrega pendente no mesmo estado", "FinalizarEntrega");
                    return;
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFe> mdfes = repMDFe.BuscarPorCargaEEstadoDestino(parametros.cargaEntrega.Carga.Codigo, uf);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFe mdfe in mdfes)
                {
                    try
                    {
                        string erro = new Servicos.Embarcador.Carga.MDFe(unitOfWork).EncerrarMDFe(mdfe.MDFe.Codigo, parametros.cargaEntrega.Carga.Codigo, codigoLocalidade, DateTime.Now, string.Empty, parametros.auditado.Usuario, parametros.tipoServicoMultisoftware, unitOfWork, parametros.auditado);

                        if (!string.IsNullOrEmpty(erro))
                            Log.TratarErro($"Erro ao encerrar MDFe {mdfe.MDFe.Codigo} ao finalizar entrega {parametros.cargaEntrega.Codigo} -- {erro}", "FinalizarEntrega");
                    }
                    catch (Exception e)
                    {
                        Log.TratarErro($"Erro ao encerrar MDFe {mdfe.MDFe.Codigo} ao finalizar entrega {parametros.cargaEntrega.Codigo} -- {e.Message}", "FinalizarEntrega");
                        continue;
                    }

                }
            }
            catch (Exception e)
            {
                Log.TratarErro($"Erro ao Encerrar Mdfe. {e.Message} --- {e.StackTrace}");
            }
        }

        public static void AtualizarDataAgendamentoPorPedido(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Repositorio.UnitOfWork unitOfWork, string senhaEntregaAgendamento = "")
        {
            if (cargasEntrega == null || cargasEntrega.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTms = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                if (!pedido.DataAgendamento.HasValue || pedido.Destinatario == null)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargasEntrega.Where(obj => obj.Pedidos.Any(p => p.CargaPedido.Pedido.Codigo == pedido.Codigo) && obj.Cliente != null && obj.Cliente.CPF_CNPJ == pedido.Destinatario.CPF_CNPJ).FirstOrDefault();

                if (cargaEntrega == null)
                    continue;

                cargaEntrega.DataAgendamento = pedido.DataAgendamento;
                if (!string.IsNullOrEmpty(senhaEntregaAgendamento))
                    cargaEntrega.SenhaEntrega = senhaEntregaAgendamento;
                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork, configControleEntrega);
            }

            if (configControleEntrega?.CalcularDataAgendamentoAutomaticamenteDataFaturamento ?? false)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = cargasEntrega.Select(x => x.Carga).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntregaColeta = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracaoTms);
                    servicoPrevisaoControleEntregaColeta.CalcularDataAgendamentoColetaAoFecharCargaAutomatico(carga, unitOfWork);
                }
            }

        }

        public static void AtualizarDataPrevisaoEntregaPorEntregaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            if (cargaEntrega != null && (!cargaEntrega.Coleta))
            {
                cargaEntrega.DataPrevista = pedido.PrevisaoEntrega;
                repositorioCargaEntrega.Atualizar(cargaEntrega);
            }
        }

        public static void AtualizarFinalizacaoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, double latitude, double longitude, Repositorio.UnitOfWork unitOfWork)
        {
            if (!cargaEntrega.DataFim.HasValue || (data >= cargaEntrega.DataFim))
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            cargaEntrega.Initialize();
            cargaEntrega.DataFim = data;
            cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(latitude);
            cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(longitude);

            repositorioCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(CriarAuditoria(), cargaEntrega, cargaEntrega.GetChanges(), Localization.Resources.Cargas.ControleEntrega.AlterouFimEntregaDevidoPosicao, unitOfWork);
            repositorioPedido.DefinirDataEntregaPorCargaEntrega(cargaEntrega.Codigo, data);
            AtualizarInicioViagem(cargaEntrega.Carga, data, unitOfWork);
        }

        public static void AtualizarInicioEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, double latitude, double longitude, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega.DataInicio.HasValue && data < cargaEntrega.DataInicio)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = CriarAuditoria();

                cargaEntrega.Initialize();
                cargaEntrega.DataInicio = data;
                cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(latitude);
                cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(longitude);

                repCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Auditoria.Auditoria.Auditar(CriarAuditoria(), cargaEntrega, cargaEntrega.GetChanges(), Localization.Resources.Cargas.ControleEntrega.AlterouInicioEntregaDevidoPosicao, unitOfWork);
                AtualizarInicioViagem(cargaEntrega.Carga, data, unitOfWork);
            }
        }

        public static void AtualizarEntradaNoRaio(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = null, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento janelaDescarregamento = null)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            cargaEntrega.DataEntradaRaio = data;
            cargaEntrega.DataLimitePermanenciaRaio = CalcularPermanenciaMaxima(cargaEntrega, unitOfWork, configuracao, janelaDescarregamento);
            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);


            //AtualizarCargaPelaCargaEntrega(cargaEntrega, unitOfWork);
        }

        public static void AtualizarSaidaDoRaio(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            if (cargaEntrega.DataEntradaRaio == null || cargaEntrega.DataEntradaRaio > data)
                cargaEntrega.DataEntradaRaio = data;

            cargaEntrega.DataSaidaRaio = data;
            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
            Servicos.Auditoria.Auditoria.Auditar(CriarAuditoria(), cargaEntrega, cargaEntrega.GetChanges(), Localization.Resources.Cargas.ControleEntrega.AlterouInicioEntregaDevidoPosicao, unitOfWork);
        }

        public static void AtualizarInicioViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataInicioViagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (carga.DataInicioViagem.HasValue && dataInicioViagem < carga.DataInicioViagem)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                carga.Initialize();
                carga.DataInicioViagem = dataInicioViagem;
                repCarga.Atualizar(carga);
                Servicos.Auditoria.Auditoria.Auditar(CriarAuditoria(), carga, carga.GetChanges(), Localization.Resources.Cargas.ControleEntrega.AlterouInicioViagemDeividoPosicao, unitOfWork);
            }
        }

        public static void AtualizarCoordenadaClientePelaEntrega(int codigoCargaEntrega, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

            if (cargaEntrega.Cliente == null)
                return;

            if (cargaEntrega.ClienteOutroEndereco != null)
            {
                Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

                cargaEntrega.ClienteOutroEndereco.Initialize();

                cargaEntrega.ClienteOutroEndereco.Latitude = cargaEntrega.LatitudeFinalizada.ToString();
                cargaEntrega.ClienteOutroEndereco.Longitude = cargaEntrega.LongitudeFinalizada.ToString();
                cargaEntrega.ClienteOutroEndereco.Cliente.DataUltimaAtualizacao = DateTime.Now;
                cargaEntrega.ClienteOutroEndereco.Cliente.Integrado = false;

                repClienteOutroEndereco.Atualizar(cargaEntrega.ClienteOutroEndereco);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(auditado, cargaEntrega.ClienteOutroEndereco.Cliente, cargaEntrega.ClienteOutroEndereco.GetChanges(), $"Geolocalização(lat/long) do endereço do cliente alterada via controle entrega ao atualizar coordenadas do cliente de acordo com o Local da Entrega.", unitOfWork);
            }
            else
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                cargaEntrega.Cliente.Initialize();

                cargaEntrega.Cliente.Latitude = cargaEntrega.LatitudeFinalizada.ToString();
                cargaEntrega.Cliente.Longitude = cargaEntrega.LongitudeFinalizada.ToString();
                cargaEntrega.Cliente.CoordenadaConferida = true;
                cargaEntrega.Cliente.DataUltimaAtualizacao = DateTime.Now;
                cargaEntrega.Cliente.Integrado = false;

                repCliente.Atualizar(cargaEntrega.Cliente);

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(auditado, cargaEntrega.Cliente, cargaEntrega.Cliente.GetChanges(), $"Geolocalização(lat/long) do endereço do cliente alterada via controle entrega ao atualizar coordenadas do cliente de acordo com o Local da Entrega.", unitOfWork);

            }


        }

        public static void RejeitarEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, out Dominio.Entidades.Embarcador.Chamados.Chamado chamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tipoCausadorOcorrencia = null)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);
            Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repConfiguracaoTipoOperacaoControle = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = parametros.configuracao ?? repConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(parametros.codigoCargaEntrega);
            Dominio.Entidades.TipoDeOcorrenciaDeCTe motivoRejeicao = repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(parametros.codigoMotivo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repConfiguracaoTipoOperacaoControle.BuscarPorTipoOperacao(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

            chamado = null;

            if (cargaEntrega == null)
                return;

            if (parametros.motivoRetificacao > 0)
                cargaEntrega.MotivoRetificacaoColeta = repMotivoRetificacaoColeta.BuscarPorCodigo(parametros.motivoRetificacao);

            if (parametros.motivoFalhaGTA > 0)
                cargaEntrega.MotivoFalhaGTA = repMotivoFalhaGTA.BuscarPorCodigo(parametros.motivoFalhaGTA, false);

            cargaEntrega.MotivoRejeicao = motivoRejeicao;
            cargaEntrega.PermitirEntregarMaisTarde = parametros.permitirEntregarMaisTarde;
            cargaEntrega.Observacao = parametros.observacao;
            cargaEntrega.MotoristaACaminho = false;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            bool devolucaoPorPeso = cargaEntrega.Carga.TipoOperacao?.DevolucaoProdutosPorPeso ?? false;

            bool possuiChamadoAberto = false;
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = motivoRejeicao?.MotivoChamado ?? motivoRejeicao?.MotivoChamadoGeracaoAutomatica;
            if (motivoChamado != null && !parametros.apenasRegistrar)
            {
                if (parametros.usuario == null)
                    parametros.usuario = cargaEntrega.Carga.Motoristas.Count > 0 ? cargaEntrega.Carga.Motoristas?.FirstOrDefault() : (configuracaoOcorrencia.UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI ? configuracaoOcorrencia.UsuarioPadraoParaGeracaoOcorrenciaPorEDI : null);

                if (!configuracaoOcorrencia.UtilizaUsuarioPadraoParaGeracaoOcorrenciaPorEDI && parametros.usuario == null)
                    throw new ServicoException("Não é possível gerar um chamado de uma carga sem motorista.");

                Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado();

                objChamado.AtendimentoRegistradoPeloMotorista = parametros.atendimentoRegistradoPeloMotorista;
                objChamado.DataRegistroMotorista = parametros.data;
                objChamado.Empresa = cargaEntrega.Carga.Empresa;
                objChamado.Cliente = cargaEntrega.Cliente;
                objChamado.MotivoChamado = motivoChamado;
                objChamado.QuantidadeImagens = parametros.quantidadeImagens;
                objChamado.Valor = parametros.valorChamado;

                if (cargaEntrega.Carga.CargaAgrupada && !configuracao.GerarOcorrenciaParaCargaAgrupada)
                    objChamado.Carga = cargaEntregaPedidos.FirstOrDefault()?.CargaPedido.CargaOrigem ?? cargaEntrega.Carga;
                else
                    objChamado.Carga = cargaEntrega.Carga;

                if (tipoServicoMultisoftware == TipoServicoMultisoftware.Fornecedor && objChamado.Pedido == null)
                    objChamado.Pedido = cargaEntregaPedidos.FirstOrDefault()?.CargaPedido?.Pedido;

                objChamado.Observacao = motivoRejeicao.Descricao;

                if (!string.IsNullOrEmpty(parametros.observacao))
                    objChamado.Observacao += ". " + parametros.observacao;

                objChamado.CargaEntrega = cargaEntrega;

                if (parametros.notasFiscais?.Any() ?? false)
                {
                    List<int> listaNotasSelecionadas = parametros.notasFiscais.Select(x => x.Codigo).ToList();
                    objChamado.NotasFiscais = cargaEntregaNotasFiscais.Where(x => listaNotasSelecionadas.Contains(x.Codigo)).Select(x => x.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
                }

                possuiChamadoAberto = repChamado.ContemChamadoMesmaCargaEntregaMotivoSituacao(objChamado.Carga.Codigo, objChamado.CargaEntrega.Codigo, objChamado.MotivoChamado.Codigo, SituacaoChamado.Aberto);
                if (!possuiChamadoAberto)
                {
                    chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objChamado, parametros.usuario, parametros.tipoServicoMultisoftware, auditado, unitOfWork);
                    cargaEntrega.ChamadoEmAberto = true;
                    cargaEntrega.Situacao = SituacaoEntrega.AgAtendimento;
                    possuiChamadoAberto = true;
                }
            }
            else
            {
                if (!(motivoRejeicao?.NaoAlterarSituacaoColetaEntrega ?? false))
                    cargaEntrega.Situacao = parametros.devolucaoParcial ? SituacaoEntrega.Entregue : SituacaoEntrega.Rejeitado;

                if (!parametros.devolucaoParcial)
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.LiberarPagamentoPorEntrega(cargaEntrega, true, configuracao, unitOfWork);
            }

            cargaEntrega.OrigemSituacao = parametros.OrigemSituacaoEntrega;

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscaisCargaEntrega = cargaEntregaNotasFiscais.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();

            AtualizarStatusNotaFiscal(listaNotasFiscaisCargaEntrega, NotaFiscalSituacaoGatilho.RejeicaoEntregaNota, auditado, unitOfWork);

            cargaEntrega.DevolucaoParcial = parametros.devolucaoParcial;

            if (motivoChamado?.TipoMotivoAtendimento != TipoMotivoAtendimento.Atendimento)
            {
                if (parametros.notasFiscais?.Count > 0)
                    servicoControleEntrega.SalvarDevolucaoCargaEntrega(cargaEntrega, parametros.notasFiscais, parametros.produtos, motivoChamado, chamado, configuracao, auditado, cargaEntrega.Situacao, cargaEntrega.DevolucaoParcial, false, null, tipoServicoMultisoftware, null, true);
                else
                    ProcessarSituacaoEntregaXMLNotaFiscal(cargaEntrega, SituacaoNotaFiscal.Devolvida, parametros.situacoesNotasFiscaisNaoAtualizar, unitOfWork);
            }

            cargaEntrega.DataRejeitado = parametros.data;

            //Confirmação de chegada
            if (parametros.wayPointConfirmacaoChegada != null)
            {
                cargaEntrega.LatitudeConfirmacaoChegada = Convert.ToDecimal(parametros.wayPointConfirmacaoChegada.Latitude);
                cargaEntrega.LongitudeConfirmacaoChegada = Convert.ToDecimal(parametros.wayPointConfirmacaoChegada.Longitude);
            }

            // Carga e descarga
            if (parametros.dataInicioCarregamento != null && parametros.dataInicioCarregamento != DateTime.MinValue)
                cargaEntrega.DataInicio = parametros.dataInicioCarregamento;

            if (parametros.dataTerminoCarregamento != null && parametros.dataTerminoCarregamento != DateTime.MinValue)
            {
                if (cargaEntrega.Situacao != SituacaoEntrega.Entregue)
                    cargaEntrega.DataFim = parametros.dataTerminoCarregamento;
            }

            if (parametros.dataInicioDescarga != null && parametros.dataInicioDescarga != DateTime.MinValue)
                cargaEntrega.DataInicio = parametros.dataInicioDescarga;

            if (parametros.dataTerminoDescarga != null && parametros.dataTerminoDescarga != DateTime.MinValue)
            {
                if (cargaEntrega.Situacao != SituacaoEntrega.Entregue)
                    cargaEntrega.DataFim = parametros.dataTerminoDescarga;
            }

            // Dados Recebedor
            if (parametros.dadosRecebedor != null)
                cargaEntrega.DadosRecebedor = buildDadosRecebedor(parametros.dadosRecebedor, cargaEntrega, unitOfWork);

            cargaEntrega.StatusPrazoEntrega = ObterEntregaEmJanela(cargaEntrega, parametros.data, unitOfWork);

            if (SituacaoEntregaHelper.ObterSituacaoEntregaEmAberto(cargaEntrega.Situacao))
                cargaEntrega.OrdemRealizada = ObterQuantidadeEntregaRealizada(cargaEntrega.Carga.Codigo, unitOfWork);

            if (parametros.wayPoint != null)
            {
                cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(parametros.wayPoint.Latitude);
                cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(parametros.wayPoint.Longitude);
            }

            // Descarga é o mesmo que a posição Finalizada. Isso pode ser removido no futuro, caso o app seja refatorado para não mandar mais o waypointDescarga
            if (parametros.wayPointDescarga != null)
            {
                cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(parametros.wayPointDescarga.Latitude);
                cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(parametros.wayPointDescarga.Longitude);
            }

            DefinirPrevisaoCargaEntrega(cargaEntrega.Carga, configuracao, unitOfWork, tipoServicoMultisoftware, configuracaoControleEntrega);
            RemoverProdutosColetados(cargaEntrega, unitOfWork);

            if (auditado != null)
                Auditoria.Auditoria.Auditar(auditado, cargaEntrega, null, Localization.Resources.Cargas.ControleEntrega.SolicitouRejeicao, unitOfWork);

            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

            if (!possuiChamadoAberto || (motivoRejeicao != null && motivoRejeicao.SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias))
            {
                if (motivoRejeicao == null && tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                    throw new ServicoException("Não foi localizado nenhum tipo de ocorrência com o código informado.");

                bool existeOcorrenciaEntrega = repOcorrenciaEntrega.ExisteOcorrenciaPorCargaEntregaETipoOcorrencia(cargaEntrega.Codigo, parametros.data, motivoRejeicao.Codigo);

                if (!existeOcorrenciaEntrega)
                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaRejeicao(cargaEntrega, parametros.data, motivoRejeicao, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, string.Empty, 0m, configuracao, parametros.tipoServicoMultisoftware, parametros.clienteMultisoftware, parametros.OrigemSituacaoEntrega, unitOfWork, auditado, null, null, EventoColetaEntrega.Todos, cargaEntregaNotasFiscais, parametros.imagens, tiposCausadoresOcorrencia: tipoCausadorOcorrencia);
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosColetaRejetaida = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosEntregaRejetada = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosTransporteRejetada = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
            {
                if (cargaEntrega.Coleta)
                {
                    if (cargaEntregaPedido.CargaPedido.Pedido.Remetente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ)
                        pedidosColetaRejetaida.Add(cargaEntregaPedido.CargaPedido.Pedido);
                }
                else
                {
                    if (cargaEntregaPedido.CargaPedido.Pedido.Destinatario.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ)
                        pedidosEntregaRejetada.Add(cargaEntregaPedido.CargaPedido.Pedido);
                    else
                        pedidosTransporteRejetada.Add(cargaEntregaPedido.CargaPedido.Pedido);
                }
            }

            if (pedidosColetaRejetaida.Count > 0)
                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosColetaRejetaida select obj.Codigo).ToList(), SituacaoAcompanhamentoPedido.ColetaRejeitada);

            if (pedidosEntregaRejetada.Count > 0)
            {
                SituacaoAcompanhamentoPedido situacaoAcompanhamentoPedido = repXmlNotaFiscal.ExisteNotasFiscaisPorEntrega(cargaEntrega.Codigo, situacoesNotasFiscaisDesconsiderar: null, SituacaoNotaFiscal.DevolvidaParcial) ? SituacaoAcompanhamentoPedido.EntregaParcial : SituacaoAcompanhamentoPedido.EntregaRejeitada;
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRejeitado in pedidosEntregaRejetada)
                    pedidoRejeitado.SituacaoAcompanhamentoPedido = situacaoAcompanhamentoPedido;

                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosEntregaRejetada select obj.Codigo).ToList(), situacaoAcompanhamentoPedido);

                if (situacaoAcompanhamentoPedido == SituacaoAcompanhamentoPedido.EntregaParcial)
                    repPedido.DefinirDataEntregaPorCargaEntrega(cargaEntrega.Codigo, cargaEntrega.DataFim);
            }

            if (pedidosTransporteRejetada.Count > 0)
                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosTransporteRejetada select obj.Codigo).ToList(), SituacaoAcompanhamentoPedido.ProblemaNoTransporte);

            if (!cargaEntrega.ChamadoEmAberto)
            {
                if (NaoPossuiEntregasPendentes(cargaEntrega.Carga, unitOfWork) && PermiteIniciarFinalizarViagem(cargaEntrega, tipoServicoMultisoftware, configuracaoControleEntrega, configuracaoTipoOperacaoControleEntrega, true))
                {
                    if (FinalizarViagem(cargaEntrega.Carga.Codigo, parametros.data, auditado, parametros.tipoServicoMultisoftware, parametros.clienteMultisoftware, parametros.OrigemSituacaoEntrega, unitOfWork))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditadoAutomatico = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                        {
                            TipoAuditado = TipoAuditado.Sistema,
                            OrigemAuditado = OrigemAuditado.Sistema
                        };

                        Servicos.Auditoria.Auditoria.Auditar(auditadoAutomatico, cargaEntrega.Carga, Localization.Resources.Cargas.ControleEntrega.FimViagemInformadoAutomaticamenteAoRejeitar, unitOfWork);
                    }
                }

                RealizouTodasColetas(cargaEntrega.Carga, configuracao, tipoServicoMultisoftware, servicoHubCarga, serCarga, unitOfWork);
            }

            EnviarOcorrenciaParaVendedor(unitOfWork, cargaEntrega, parametros.produtos);

            if (repCargaEntrega.RejeitouTodasColetasPorCarga(cargaEntrega.Carga.Codigo, cargaEntrega.Codigo) && (cargaEntrega.Carga.TipoOperacao?.ConfiguracaoCarga?.GerarCargaRetornoRejeitarTodasColetas ?? false))
                serCarga.GerarCargaEspelho(cargaEntrega.Carga, unitOfWork, tipoServicoMultisoftware, configuracao, auditado, true);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCargaEntrega = repCargaEntregaPedido.BuscarPedidosPorEntrega(cargaEntrega.Codigo);
            if (pedidosCargaEntrega != null && pedidosCargaEntrega.Count > 0)
            {
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido svcAgendamentoPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, configuracao, tipoServicoMultisoftware, auditado, parametros.usuario);
                svcAgendamentoPedido.GerarIntegracaoDriveIn(unitOfWork, pedidosCargaEntrega, parametros.clienteMultisoftware, GatilhoIntegracaoMondelezDrivin.RejeicaoEntrega, motivoRejeicao);
            }
        }

        public static void LiberarPagamentoPorEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, bool cancelarCanhoto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.GerarPagamentoBloqueado)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                Repositorio.PreConhecimentoDeTransporteEletronico repPreCTe = new Repositorio.PreConhecimentoDeTransporteEletronico(unitOfWork);

                Servicos.Embarcador.Canhotos.Canhoto serCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);


                List<int> cargaPedidos = repCargaEntregaPedido.BuscarCodigosCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);
                if (cargaPedidos.Count > 0)
                {
                    List<int> ctes = repCargaPedidoXMLNotaFiscalCTe.BuscarCodigosSemFilialEmissoraCTePorCargaPedidos(cargaPedidos);
                    if (ctes.Count > 0)
                        repDocumentoFaturamento.LiberarPagamentosPorCTes(ctes, DateTime.Now);

                    List<int> preCtes = repCargaPedidoXMLNotaFiscalCTe.BuscarCodigosPreCTesSemFilialEmissoraCTePorCargaPedidos(cargaPedidos);
                    if (preCtes.Count > 0)
                        repPreCTe.LiberarPagamentosPorPreCTes(preCtes, DateTime.Now);
                }

                if (cancelarCanhoto)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntrega = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotaFiscais)
                    {
                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = (from obj in canhotos
                                                                                 where
                                                            (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)
                                                            || (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso
                                                            && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo)
                                                            )
                                                                                 select obj).FirstOrDefault();

                        if (canhoto != null && !canhotosEntrega.Contains(canhoto))
                            canhotosEntrega.Add(canhoto);
                    }

                    foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosEntrega)
                    {
                        canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Cancelado;
                        canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Cancelada;
                        serCanhoto.GerarHistoricoCanhoto(canhoto, null, "Cancelado por devolução na entrega.", unitOfWork);
                    }
                }
            }
        }

        public static void LiberacaoDocumentosBloqueadosConfirmarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repositorioDocumentoFaturamento = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);

            List<int> cargaPedidos = repCargaEntregaPedido.BuscarCodigosCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);

            if (cargaPedidos.Count == 0)
                return;

            List<int> ctes = repCargaPedidoXMLNotaFiscalCTe.BuscarCodigosSemFilialEmissoraCTePorCargaPedidos(cargaPedidos);

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentosEscrituracao = repositorioDocumentoFaturamento.BuscarPorCtesComSituacaoBloqueado(ctes);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao documento in documentosEscrituracao)
            {
                documento.Situacao = SituacaoEscrituracaoDocumento.AgEscrituracao;

                repositorioDocumentoFaturamento.Atualizar(documento);
            }
        }

        public static void GerarEntregasControleQualidade(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem, bool excluir, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            GerarCargaEntrega(carga, cargasPedido, cargaPedidosXMLs, cargaRotaFrete, pontosPassagem, excluir, configuracao, unitOfWork, tipoServicoMultisoftware);

            carga.EntregasGeradasPeloControleQualidade = true;
            carga.NumeroTentativaGeracaoEntregasControleQualidade += 1;
            repCarga.Atualizar(carga);
        }

        /// <summary>
        /// Devido ao método: GerarCargaEntrega não conter uma responsabilidade única, esta sobrescrita busca os parâmetros necessários não informados para executar o método: GerarCargaEntregaParaCarga somente se o cenário estiver apto para a execução do mesmo
        /// </summary>
        public static void GerarCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, bool excluir, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = null;
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = null;
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagens = null;

            if (ValidarGeracaoCargaEntrega(carga))
            {
                cargaPedidosXMLs = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork).BuscarPorCarga(carga.Codigo);
                cargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork).BuscarPorCarga(carga.Codigo);
                pontosPassagens = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork).BuscarPorCarga(carga.Codigo);
            }

            GerarCargaEntrega(carga, cargasPedido, cargaPedidosXMLs, cargaRotaFrete, pontosPassagens, excluir, configuracao, unitOfWork, tipoServicoMultisoftware);
        }

        public static void GerarCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem, bool excluir, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracao, null, "Geração de controle de entrega", unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregas = null;
            if (ValidarGeracaoCargaEntrega(carga))
                cargasEntregas = GerarCargaEntregaParaCarga(carga, cargasPedido, cargaPedidosXMLs, pontosPassagem, true, excluir, configuracao, tipoServicoMultisoftware, unitOfWork);

            DefinirPrevisaoCargaEntrega(carga, cargaRotaFrete, cargasEntregas, false, false, configuracao, unitOfWork, tipoServicoMultisoftware, true, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

            Servicos.Embarcador.Integracao.Diaria.DiariaMotorista servicoDiariaMotorista = new Servicos.Embarcador.Integracao.Diaria.DiariaMotorista(unitOfWork);
            servicoDiariaMotorista.GerarPagamentoMotoristaEmbarcador(carga, configuracao);
        }
        public static async Task GerarCargaEntregaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem, bool excluir, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            await Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciarAsync(carga, configuracao, null, "Geração de controle de entrega", unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregas = null;
            if (ValidarGeracaoCargaEntrega(carga))
                cargasEntregas = GerarCargaEntregaParaCarga(carga, cargasPedido, cargaPedidosXMLs, pontosPassagem, true, excluir, configuracao, tipoServicoMultisoftware, unitOfWork);

            DefinirPrevisaoCargaEntrega(carga, cargaRotaFrete, cargasEntregas, false, false, configuracao, unitOfWork, tipoServicoMultisoftware, true, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

            Servicos.Embarcador.Integracao.Diaria.DiariaMotorista servicoDiariaMotorista = new Servicos.Embarcador.Integracao.Diaria.DiariaMotorista(unitOfWork);
            servicoDiariaMotorista.GerarPagamentoMotoristaEmbarcador(carga, configuracao);
        }

        public static void GerarControleEntregaSemRota(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, bool excluir, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (((configuracao.ExigirCargaRoteirizada || (carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) && !(carga.TipoOperacao?.NaoExigeRotaRoteirizada ?? false)) || !(carga.TipoOperacao?.ConfiguracaoControleEntrega?.GerarControleEntregaSemRota ?? false)) return;

            GerarCargaEntregaParaCarga(carga, cargasPedido, cargaPedidosXMLs, null, false, excluir, configuracao, tipoServicoMultisoftware, unitOfWork);
        }

        public static void GerarProdutosColetaEntregaPorNota(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals = (from obj in pedidoXMLNotaFiscais select obj.XMLNotaFiscal).Distinct().ToList();
            bool multiplicar = cargaEntrega.Carga.TipoOperacao?.ConfiguracaoCarga?.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ?? false;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xMLNotaFiscals)
            {
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = (from obj in xmlNotaFiscalProdutos where obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj.Produto).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador in produtosEmbarcador)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto();
                    cargaEntregaProduto.CargaEntrega = cargaEntrega;
                    cargaEntregaProduto.XMLNotaFiscal = xmlNotaFiscal;
                    cargaEntregaProduto.Produto = produtoEmbarcador;

                    int quantidadeCaixa = produtoEmbarcador.QuantidadeCaixa;
                    if (quantidadeCaixa == 0 || !multiplicar)
                        quantidadeCaixa = 1;

                    cargaEntregaProduto.Quantidade = quantidadeCaixa * (from obj in xmlNotaFiscalProdutos where obj.Produto.Codigo == produtoEmbarcador.Codigo && obj.XMLNotaFiscal.Codigo == xmlNotaFiscal.Codigo select obj.QuantidadeUtilizar).Sum();

                    cargaEntregaProduto.PesoUnitario = produtoEmbarcador.PesoUnitario;
                    repCargaEntregaProduto.Inserir(cargaEntregaProduto);
                }
            }
        }

        public static List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> ObterJanelaDescarregamento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataPrevista, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega == null)
                return null;

            Repositorio.Embarcador.Logistica.CentroDescarregamento repcentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repcentroDescarregamento.BuscarPorDestinatario(cargaEntrega?.Cliente?.Codigo ?? 0);


            if (centroDescarregamento == null)
                return null;

            int diaSemana = (int)dataPrevista.DayOfWeek + 1;

            Dominio.Entidades.Cliente remetente = ObterRemetente(cargaEntrega.Carga, unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodoComTipoEntregaEDestinatario = (from obj in centroDescarregamento.PeriodosDescarregamento
                                                                                                                      where
                                                                                                                         (int)obj.Dia == diaSemana &&
                                                                                                                         obj.TiposDeCarga.Any(o => o?.TipoDeCarga?.Codigo == cargaEntrega?.Carga?.TipoDeCarga?.Codigo) &&
                                                                                                                         obj.Remetentes.Any(o => o?.Remetente?.Codigo == remetente?.Codigo)
                                                                                                                      select obj).ToList();
            if (periodoComTipoEntregaEDestinatario.Count > 0)
                return periodoComTipoEntregaEDestinatario;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodoComRemetente = (from objRemetente in centroDescarregamento.PeriodosDescarregamento
                                                                                                       where
                                                                                                          (int)objRemetente.Dia == diaSemana &&
                                                                                                          objRemetente.TiposDeCarga.Count == 0 &&
                                                                                                          objRemetente.Remetentes.Any(o => o?.Remetente?.Codigo == remetente?.Codigo)
                                                                                                       select objRemetente).ToList();
            if (periodoComRemetente.Count > 0)
                return periodoComRemetente;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodoComTipoDeCarga = (from objTipoCarga in centroDescarregamento.PeriodosDescarregamento
                                                                                                         where
                                                                                                           (int)objTipoCarga.Dia == diaSemana &&
                                                                                                           objTipoCarga.TiposDeCarga.Any(o => o?.TipoDeCarga?.Codigo == cargaEntrega?.Carga?.TipoDeCarga?.Codigo) &&
                                                                                                           objTipoCarga.Remetentes.Count == 0
                                                                                                         select objTipoCarga).ToList();

            if (periodoComTipoDeCarga.Count > 0)
                return periodoComTipoDeCarga;

            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodo = (from objPeriodo in centroDescarregamento.PeriodosDescarregamento
                                                                                           where
                                                                                              (int)objPeriodo.Dia == diaSemana &&
                                                                                              objPeriodo.TiposDeCarga.Count == 0 &&
                                                                                              objPeriodo.Remetentes.Count == 0
                                                                                           select objPeriodo).ToList();
            if (periodo.Count > 0)
                return periodo;

            return null;
        }

        public static Dominio.Entidades.Cliente ObterRemetente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            return ObterRemetente(carga.Codigo, unitOfWork);
        }

        public static void SalvarImagem(Stream imagem, Repositorio.UnitOfWork unitOfWork, out string tokenImagem, bool arquivoPDF = false)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            ArmazenarArquivoFisico(imagem, caminho, out tokenImagem, arquivoPDF);
        }

        public static void SalvarImagemNotaFiscal(Stream imagem, Repositorio.UnitOfWork unitOfWork, out string guid)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "NotasFiscais" });

            ArmazenarArquivoFisico(imagem, caminho, out guid);
        }

        public static void SalvarImagemAssinatura(Stream imagem, Repositorio.UnitOfWork unitOfWork, out string guid)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "Assinatura" });

            ArmazenarArquivoFisico(imagem, caminho, out guid);
        }

        public static void SalvarImagemRecebedor(Stream imagem, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "CargaColetaEntrega", "FotoRecebedor" });
            ArmazenarArquivoFisico(imagem, caminho, out string guid);

            if (cargaEntrega.DadosRecebedor == null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor dadosRecebedorEntity = buildDadosRecebedor(new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor
                {
                    GuidFoto = guid,
                }, cargaEntrega, unitOfWork);
                cargaEntrega.DadosRecebedor = dadosRecebedorEntity;
            }
            else
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor repDadosRecebedor = new Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor(unitOfWork);
                cargaEntrega.DadosRecebedor.GuidFoto = guid;
                repDadosRecebedor.Atualizar(cargaEntrega.DadosRecebedor);
            }
        }

        public static void ArmazenarArquivoFisico(Stream imagem, string caminho, out string guid, bool arquivoPDF = false)
        {
            if (string.IsNullOrWhiteSpace(caminho))
            {
                string mensagemRetorno = "Local para armazenamento do arquivo não está configurado! Favor entrar em contato com o suporte.";
                Log.TratarErro(mensagemRetorno);
                throw new ServicoException(mensagemRetorno);
            }

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string extensao = ".jpg";

            if (arquivoPDF)
                extensao = ".pdf";

            guid = Guid.NewGuid().ToString().Replace("-", "");

            try
            {
                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guid + extensao);
                string fileLocationMiniatura = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guid + $"-miniatura{extensao}");

                using (Stream fs = Utilidades.IO.FileStorageService.Storage.Create(fileLocation))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }

                using (Stream fs = Utilidades.IO.FileStorageService.Storage.Create(fileLocationMiniatura))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }

                if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation) || !Utilidades.IO.FileStorageService.Storage.Exists(fileLocationMiniatura))
                {
                    string mensagemRetorno = $"Arquivo enviado não foi armazenado!";
                    Log.TratarErro(mensagemRetorno);
                    throw new ServicoException(mensagemRetorno);
                }
            }
            catch (Exception ex)
            {
                string mensagemRetorno = "Arquivo enviado não foi armazenado! Favor entrar em contato com o suporte.";
                Log.TratarErro(ex.Message);
                throw new ServicoException(mensagemRetorno);
            }
        }

        public static bool SalvarGTAColetaEntrega(int codigoCargaEntrega, string codigoBarras, string numeroNF, string serie, string uf, int quantidade, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = "";
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
            Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(uf);

            if (cargaEntrega == null)
            {
                mensagemErro = "Carga entrega não encontrada";
                return false;
            }

            if (estado == null)
            {
                mensagemErro = "Estado não encontrada";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal cargaEntregaGTA = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal
            {
                CargaEntrega = cargaEntrega,
                CodigoBarras = codigoBarras,
                NumeroNotaFiscal = numeroNF,
                Serie = serie,
                Estado = estado,
                Quantidade = quantidade,
            };

            repCargaEntregaGuiaTransporteAnimal.Inserir(cargaEntregaGTA);

            return true;
        }

        public static bool SalvarAssinaturaProdutorColetaEntrega(int codigoClienteMultisoftware, int codigoCargaEntrega, string tokenImagem, DateTime data, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = "";
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

            if (cargaEntrega == null)
            {
                mensagemErro = "Carga entrega não encontrada";
                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel cargaEntregaAssinatura = repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            if (cargaEntregaAssinatura == null)
            {
                cargaEntregaAssinatura = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel
                {
                    CargaEntrega = cargaEntrega,
                    GuidArquivo = "",
                    DataEnvioAssinatura = data
                };

                repCargaEntregaAssinaturaResponsavel.Inserir(cargaEntregaAssinatura);
            }

            string extensao = ".jpg";
            string nomeArquivo = "AS_" + codigoCargaEntrega + "_" + codigoClienteMultisoftware + extensao;

            cargaEntregaAssinatura.GuidArquivo = tokenImagem;
            cargaEntregaAssinatura.NomeArquivo = nomeArquivo;
            cargaEntregaAssinatura.DataEnvioAssinatura = data;

            repCargaEntregaAssinaturaResponsavel.Atualizar(cargaEntregaAssinatura);

            return true;
        }

        public static bool SalvarImagemNotaFiscalColetaEntrega(int codigoClienteMultisoftware, int codigoCargaEntrega, string tokenImagem, DateTime data, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = "";
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal repCargaEntregaFotoNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

            if (cargaEntrega == null)
            {
                mensagemErro = "Carga entrega não encontrada";
                return false;
            }

            string extensao = ".jpg";
            string nomeArquivo = "NF_" + codigoCargaEntrega + "_" + codigoClienteMultisoftware + extensao;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal cargaEntregaFotoNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal
            {
                GuidArquivo = tokenImagem,
                NomeArquivo = nomeArquivo,
                CargaEntrega = cargaEntrega,
                DataEnvioImagem = data
            };

            repCargaEntregaFotoNotaFiscal.Inserir(cargaEntregaFotoNotaFiscal);

            return true;
        }

        public static string SalvarImagemNotaDevolucao(Stream imagem, Repositorio.UnitOfWork unitOfWork, out string tokenImagem)
        {
            tokenImagem = "";
            string retorno = "";

            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = imagem.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);

            ms.Position = 0;

            string extensao = ".jpg";
            if (extensao.Equals(".jpg") || extensao.Equals(".jpeg"))
            {
                string token = Guid.NewGuid().ToString().Replace("-", "");
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "NotasDevolucao" });

                using (System.Drawing.Image t = System.Drawing.Image.FromStream(ms))
                {
                    tokenImagem = token;
                    string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem + extensao);
                    string fileLocationMiniatura = Utilidades.IO.FileStorageService.Storage.Combine(caminho, tokenImagem + $"-miniatura{extensao}");

                    Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocation, t);
                    Utilidades.IO.FileStorageService.Storage.SaveImage(fileLocationMiniatura, t);
                }
            }
            else
            {
                retorno = "A extensão do arquivo é inválida.";
            }

            return retorno;
        }

        public static string SalvarImagemEntrega(int codigoClienteMultisoftware, int codigoCargaEntrega, string tokenImagem, Repositorio.UnitOfWork unitOfWork, DateTime data, double? latitude, double? longitude, OrigemSituacaoEntrega origemSituacaoEntrega, bool adicionarNoChamado = false, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, string extensao = ".jpg")
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = adicionarNoChamado ? repChamado.BuscarPorCargaEntrega(codigoCargaEntrega) : null;

            if (cargaEntrega == null)
                return "Carga entrega não encontrada";

            string nomeArquivo = "EN_" + codigoCargaEntrega + "_" + codigoClienteMultisoftware + extensao;
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto
            {
                GuidArquivo = tokenImagem,
                NomeArquivo = nomeArquivo,
                CargaEntrega = cargaEntrega,
                Latitude = Convert.ToDecimal(latitude),
                Longitude = Convert.ToDecimal(longitude),
                DataEnvioImagem = data != DateTime.MinValue ? data : DateTime.Now,
            };


            repCargaEntregaFoto.Inserir(cargaEntregaFoto, auditado);
            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, string.Format(Localization.Resources.Cargas.ControleEntrega.EnviouAnexoXNaDataX, nomeArquivo, data.ToDateTimeString()), unitOfWork);

            string caminhoEntrega = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" }), tokenImagem + extensao);
            string caminhoEntregaBackup = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedidoBackup" }), tokenImagem + extensao);

            //Imagens estão excluindo do server, backup será para validar se a mesma ficou armazenada, referente tarefa #31513
            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoEntrega))
            {
                Utilidades.IO.FileStorageService.Storage.Copy(caminhoEntrega, caminhoEntregaBackup);
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoEntregaBackup))
                    Log.TratarErro("Imagem não foi salva para o backup do Controle de Entrega código " + codigoCargaEntrega);
            }
            else
                Log.TratarErro("Imagem não foi salva para o Controle de Entrega código " + codigoCargaEntrega);

            if (chamado != null)
            {
                string caminhoChamado = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), tokenImagem + extensao);

                Utilidades.IO.FileStorageService.Storage.Copy(caminhoEntrega, caminhoChamado);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
                {
                    Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo()
                    {
                        Chamado = chamado,
                        Descricao = string.Empty,
                        GuidArquivo = tokenImagem,
                        NomeArquivo = nomeArquivo
                    };

                    repChamadoAnexo.Inserir(chamadoAnexo, auditado);
                    if (auditado != null)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, string.Format(Localization.Resources.Cargas.ControleEntrega.EnviouAnexoXNaDataX, nomeArquivo, data.ToDateTimeString()), unitOfWork);
                }
                else
                    Log.TratarErro("Imagem não foi salva para o Atendimento " + chamado.Numero);
            }

            if (cargaEntrega.Coleta)
            {
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repositorioConfiguracaoTipoOperacaoControleEntrega = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repositorioConfiguracaoTipoOperacaoControleEntrega.BuscarPorTipoOperacao(cargaEntrega.Carga?.TipoOperacao?.Codigo ?? 0);

                if (configuracaoTipoOperacaoControleEntrega?.DisponibilizarFotoDaColetaNaTelaDeAprovacaoDeNotaFiscal ?? false)
                {
                    Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                    if (!repositorioXMLNotaFiscal.ExistePorCargaEntrega(cargaEntrega.Codigo))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAdicionar gestaoDadosColetaDadosNFeAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAdicionar()
                        {
                            CodigoCargaEntrega = cargaEntrega.Codigo,
                            GuidArquivo = cargaEntregaFoto.GuidArquivo,
                            Latitude = cargaEntregaFoto.Latitude,
                            Longitude = cargaEntregaFoto.Longitude,
                            Origem = origemSituacaoEntrega == OrigemSituacaoEntrega.App ? OrigemGestaoDadosColeta.Motorista : OrigemGestaoDadosColeta.Embarcador,
                            OrigemFoto = OrigemFotoDadosNFEGestaoDadosColeta.ControleDeEntrega
                        };

                        if (gestaoDadosColetaDadosNFeAdicionar.Latitude == 0m)
                        {
                            gestaoDadosColetaDadosNFeAdicionar.Latitude = cargaEntrega.LatitudeFinalizada;
                            gestaoDadosColetaDadosNFeAdicionar.Longitude = cargaEntrega.LongitudeFinalizada;
                        }

                        new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, auditado).Adicionar(gestaoDadosColetaDadosNFeAdicionar);
                    }
                }
            }

            return string.Empty;
        }

        public string ObterBase64ImagemEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto foto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            string extensao = Path.GetExtension(foto.NomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, foto.GuidArquivo + "-miniatura" + extensao);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                return string.Empty;

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        public string ObterBase64ImagemOcorrencia(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo anexo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), $"{anexo.GuidArquivo}.{anexo.ExtensaoArquivo}");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                return string.Empty;

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        public static bool BuscarExistePorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            return new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork).BuscarExistePorCarga(carga.Codigo);
        }

        public static bool NaoPossuiEntregasPendentes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, bool validarApenasEntregas = false, string uf = "")
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> listaSituacoesPendentes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>
            {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento
            };

            int entregas = repCargaEntrega.ContarEntregasPendentes(carga.Codigo, listaSituacoesPendentes, validarApenasEntregas, uf);
            return entregas == 0;
        }

        public static bool UltimaEntregaPendente(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> listaSituacoesPendentes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>
            {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento
            };

            int entregas = repCargaEntrega.ContarEntregasPendentes(carga.Codigo, listaSituacoesPendentes);
            return entregas == 1;
        }

        public static bool NaoPossuiColetasPendentes(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>
            {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento
            };

            return !repCargaEntrega.BuscarExisteColetaPorCargaeSituacao(carga.Codigo, situacoes);
        }

        public static Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega AdicionarEntrega(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, Dominio.Entidades.Cliente destinatario, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            int ordemNovaColeta = repCargaEntrega.BuscarProximaOrdemPorCarga(carga.Codigo);

            ObjetoGeracaoControleEntrega cliente = new ObjetoGeracaoControleEntrega()
            {
                Cliente = destinatario,
                Ordem = ordemNovaColeta,
                Reentrega = cargaPedido.ReentregaSolicitada,
                Distancia = 0,
            };

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntrega.BuscarCargaPedidosPorReentregaClienteECarga(carga.Codigo, destinatario.CPF_CNPJ, cliente.Reentrega);
            cargaPedidos.Add(cargaPedido);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = AdicionarColetaEntrega(cliente, cargaPedidos, pedidoXMLNotaFiscals, DateTime.Now, carga, ordemNovaColeta, true, cargaPedidoProdutos, xmlNotaFiscalProdutos, checkList: null, unitOfWork: unitOfWork, configuracao);

            return cargaEntrega;
        }

        public static void NotificarCargaEntregaAdicionada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            NotificarAcaoCargaEntrega(cargaEntrega, clienteMultisoftware.Codigo, MobileHubs.EntregaAdicionada, unitOfWork);
        }

        public static void NotificarCargaEntregaAlterada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            NotificarAcaoCargaEntrega(cargaEntrega, clienteMultisoftware.Codigo, MobileHubs.EntregaAlterada, unitOfWork);
        }

        public static void NotificarCargaEntregaRemovida(int codigoCargaEntrega, int codigoCarga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            NotificarAcaoCargaEntrega(codigoCargaEntrega, codigoCarga, clienteMultisoftware.Codigo, MobileHubs.EntregaExcluida, unitOfWork);
        }

        public static void AdicionarNovaColeta(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Cliente remetente, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.CargaLocaisPrestacao serCargaLocaisPrestacao = new Servicos.Embarcador.Carga.CargaLocaisPrestacao(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);
            Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregasPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            cargaEntregasPedidos = cargaEntregasPedidos.OrderBy(obj => obj.CargaEntrega.Ordem).ToList();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosNaoRealizados = (from obj in cargaEntregasPedidos where obj.CargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue select obj).ToList();

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos.FirstOrDefault();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido.Clonar();
            if (configuracao.NumeroCargaSequencialUnico)
                pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo();
            else
                pedido.NumeroSequenciaPedido = repPedido.ObterProximoCodigo(carga.Filial);

            pedido.NumeroPedidoEmbarcador = pedido.NumeroSequenciaPedido.ToString();
            pedido.Remetente = remetente;
            pedido.SituacaoAcompanhamentoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.AgColeta;

            if (cargaEntregaPedidosNaoRealizados.Count > 0)
            {
                pedido.OrdemColetaProgramada = cargaEntregaPedidosNaoRealizados.FirstOrDefault().CargaPedido.Pedido.OrdemColetaProgramada;
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidosNaoRealizados)
                {
                    cargaEntregaPedido.CargaPedido.Pedido.OrdemColetaProgramada++;
                    repPedido.Atualizar(cargaEntregaPedido.CargaPedido.Pedido);
                }
            }
            else
                pedido.OrdemColetaProgramada = cargaEntregasPedidos.LastOrDefault().CargaPedido.Pedido.OrdemColetaProgramada + 1;

            Utilidades.Object.DefinirListasGenericasComoNulas(pedido);

            repPedido.Inserir(pedido);

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto in cargaPedido.Pedido.Produtos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProdutoDuplicado = pedidoProduto.Clonar();
                pedidoProdutoDuplicado.Pedido = pedido;
                Utilidades.Object.DefinirListasGenericasComoNulas(pedidoProdutoDuplicado);
                repPedidoProduto.Inserir(pedidoProdutoDuplicado);
            }

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoDuplicado = cargaPedido.Clonar();
            Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoDuplicado);
            cargaPedidoDuplicado.Pedido = pedido;
            cargaPedidoDuplicado.Origem = remetente.Localidade;
            repCargaPedido.Inserir(cargaPedidoDuplicado);
            cargaPedidos.Add(cargaPedidoDuplicado);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedido.Produtos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProdutoDuplicado = cargaPedidoProduto.Clonar();
                cargaPedidoProdutoDuplicado.CargaPedido = cargaPedidoDuplicado;
                cargaPedidoProdutoDuplicado.Quantidade = 0;
                cargaPedidoProdutoDuplicado.Temperatura = 0;
                Utilidades.Object.DefinirListasGenericasComoNulas(cargaPedidoProdutoDuplicado);
                repCargaPedidoProduto.Inserir(cargaPedidoProdutoDuplicado);
            }

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            cargaEntregas = cargaEntregas.OrderBy(obj => obj.Ordem).ToList();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaColetaNaoRealizada = (from obj in cargaEntregas where obj.Coleta && obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue orderby obj.Ordem select obj).LastOrDefault();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaColeta = (from obj in cargaEntregas where obj.Coleta orderby obj.Ordem select obj).LastOrDefault();

            int ordemNovaColeta = 0;
            bool encontrouOrdem = false;
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
            {
                if (encontrouOrdem)
                {
                    cargaEntrega.Ordem++;
                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);
                }
                else
                {
                    if (ultimaColetaNaoRealizada != null && ultimaColetaNaoRealizada.Codigo == cargaEntrega.Codigo)
                    {
                        ordemNovaColeta = cargaEntrega.Ordem;
                        cargaEntrega.Ordem++;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);
                        encontrouOrdem = true;
                    }
                    else if (ultimaColeta.Codigo == cargaEntrega.Codigo)
                    {
                        ordemNovaColeta = ultimaColeta.Ordem + 1;
                        encontrouOrdem = true;
                    }
                }
            }

            ObjetoGeracaoControleEntrega cliente = new ObjetoGeracaoControleEntrega()
            {
                Cliente = remetente,
                Ordem = ordemNovaColeta,
                Distancia = 0,
                Coleta = true,
                ColetaEquipamento = false
            };

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList = repCheckListTipo.BuscarPrimeiroPorCargaPedidoProduto(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(carga.Codigo);
            AdicionarColetaEntrega(cliente, cargaPedidos, pedidoXMLNotaFiscals, DateTime.Now, carga, ordemNovaColeta, true, cargaPedidoProdutos, xmlNotaFiscalProdutos, checkList, unitOfWork, configuracao);
            serRota.DeletarPercursoDestinosCarga(carga, unitOfWork);
            serCargaLocaisPrestacao.VerificarEAjustarLocaisPrestacao(carga, cargaPedidos, unitOfWork, tipoServicoMultisoftware, configuracaoPedido);
            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);

            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, string.Format(Localization.Resources.Cargas.ControleEntrega.AdicionouColetaXNaCarga, pedido.NumeroPedidoEmbarcador), unitOfWork);
        }

        public static void GerarAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, decimal? latitude, decimal? longitude, DateTime dataAlerta, string observacao, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repCargaEntrega.BuscarPorCargaNaoRealizada(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAtual = cargasEntrega.OrderBy(o => o.Ordem).FirstOrDefault();

            if (cargaEntregaAtual == null)
                cargaEntregaAtual = repCargaEntrega.BuscarUltimaCargaEntrega(carga.Codigo);

            if (cargaEntregaAtual == null)
                return;

            //Sem latitude insere na primeira em aberto
            if (latitude == null || longitude == null)
            {
                Servicos.Embarcador.Logistica.AlertaMonitor.InserirAlertaEntrega(tipoAlerta, monitoramentoEvento, latitude, longitude, dataAlerta, observacao, cargaEntregaAtual, unitOfWork);
                return;
            }

            //Procura a entrega mais próxima
            double latitudeAlerta = Convert.ToDouble(latitude ?? 0);
            double longitudeAlerta = Convert.ToDouble(longitude ?? 0);

            double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(cargaEntregaAtual.Cliente?.Latitude?.ToDouble() ?? 0, cargaEntregaAtual.Cliente?.Latitude?.ToDouble() ?? 0, latitudeAlerta, longitudeAlerta);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
            {

                double novaDistancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaMetros(cargaEntregaAtual.Cliente?.Latitude?.ToDouble() ?? 0, cargaEntregaAtual.Cliente?.Latitude?.ToDouble() ?? 0, latitudeAlerta, longitudeAlerta);

                if (novaDistancia < distancia)
                {
                    cargaEntregaAtual = cargaEntrega;
                    distancia = novaDistancia;
                }
            }

            Servicos.Embarcador.Logistica.AlertaMonitor.InserirAlertaEntrega(tipoAlerta, monitoramentoEvento, latitude, longitude, dataAlerta, observacao, cargaEntregaAtual, unitOfWork);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> ObterDocumentosMobile(DateTime data, int codigoMotorista, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> documentosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(codigoMotorista);
            if (motorista != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaPorMotorista(motorista.Codigo, data);
                else
                    cargas = repCarga.BuscarCargaPorClienteDestino(motorista.Cliente.CPF_CNPJ, data);


                for (int i = 0; i < cargas.Count; i++)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> retornoLista = ObterDocumentosPorCarga(cargas[i], clienteMultisoftware, motorista.Cliente, unitOfWork);
                    for (int k = 0; k < retornoLista.Count; k++)
                    {
                        documentosMob.Add(retornoLista[k]);
                    }
                }
            }
            return documentosMob;
        }

        public static List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> ObterNotasFiscalsColetaEntrega(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidoXMLNotasFiscais, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado ||
                cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual)
            {
                pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo));
            }
            else
            {
                if (cargaEntrega.Coleta)
                {
                    if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor ||
                        cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo));
                    else
                    {
                        pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida));
                        pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada));
                    }
                }
                else
                {
                    if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor ||
                        cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo));
                    else
                    {
                        pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.XMLNotaFiscal.Destinatario.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida));
                        pedidoXMLNotaFiscais.AddRange(cargaPedidoXMLNotasFiscais.Where(obj => obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ && obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada));
                    }
                }
            }
            return pedidoXMLNotaFiscais;
        }

        public static void VincularNotasNaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXmlNotaFiscalCarga, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntregaPedidos.Count == 0)
                return;

            Log.TratarErro($"Iniciou VincularNotasNaCargaEntrega da carga {carga.Codigo}", "ConfirmarEnvioDosDocumentos");

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaSemFetch(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
                GerarCargaEntregaNotas(cargaEntregaPedido.CargaPedido, pedidosXmlNotaFiscalCarga, cargaEntregaPedido.CargaEntrega, unitOfWork, cargaEntregaNotaFiscais);

            if (xmlNotaFiscalProdutos.Count > 0)
            {
                repCargaEntregaProduto.ExcluirTodosPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = (from obj in cargaEntregaPedidos select obj.CargaEntrega).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosCargaPedido = (from obj in cargaEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidosCargaPedido)
                        pedidoXMLNotaFiscais.AddRange(ObterNotasFiscalsColetaEntrega(cargaEntregaPedido.CargaPedido, pedidosXmlNotaFiscalCarga, cargaEntrega));

                    GerarProdutosColetaEntregaPorNota(cargaEntrega, pedidoXMLNotaFiscais, xmlNotaFiscalProdutos, unitOfWork);
                }
            }

            Log.TratarErro($"Finalizou VincularNotasNaCargaEntrega da carga {carga.Codigo}", "ConfirmarEnvioDosDocumentos");
        }

        public static void GerarCargaEntregaNotas(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidoXMLNotasFiscais, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscais = null)
        {
            if (cargaPedido == null)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = ObterNotasFiscalsColetaEntrega(cargaPedido, cargaPedidoXMLNotasFiscais, cargaEntrega);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> listaCargaEntregaNotaFiscal = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> listaCargaEntregaNotaFiscalDeletar = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscaisJaInseridas = cargaEntregaNotaFiscais?.Count > 0 ? cargaEntregaNotaFiscais.Where(o => o.CargaEntrega.Codigo == cargaEntrega.Codigo).ToList() : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            HashSet<int> codigosPedidoXMLNotaFiscais = new HashSet<int>(pedidoXMLNotaFiscais.Select(o => o.Codigo));
            if (cargaEntregaNotaFiscaisJaInseridas.Count > 0)
            {
                HashSet<int> codigosNotasFiscais = new HashSet<int>(cargaEntregaNotaFiscaisJaInseridas.Select(o => o.PedidoXMLNotaFiscal.Codigo));
                pedidoXMLNotaFiscais = pedidoXMLNotaFiscais.Where(entrega => !codigosNotasFiscais.Contains(entrega.Codigo)).ToList();
            }

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal in pedidoXMLNotaFiscais)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal
                {
                    CargaEntrega = cargaEntrega,
                    PedidoXMLNotaFiscal = pedidoXMLNotaFiscal
                };

                listaCargaEntregaNotaFiscal.Add(cargaEntregaNotaFiscal);
            }

            if (cargaEntregaNotaFiscaisJaInseridas.Count > 0)
                listaCargaEntregaNotaFiscalDeletar.AddRange(cargaEntregaNotaFiscaisJaInseridas.Where(entrega => !codigosPedidoXMLNotaFiscais.Contains(entrega.PedidoXMLNotaFiscal.Codigo)).ToList());

            if (listaCargaEntregaNotaFiscalDeletar.Count > 0)
                repositorioCargaEntregaNotaFiscal.DeletarSQLListaCargaEntregaPedido(listaCargaEntregaNotaFiscalDeletar.Select(x => x.Codigo).ToList());

            repositorioCargaEntregaNotaFiscal.InsertSQLListaCargaEntregaPedido(listaCargaEntregaNotaFiscal, cargaEntrega).GetAwaiter().GetResult();
        }


        public static void AtualizarPrevisaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            AtualizarPrevisaoCargaEntrega(carga, configuracao, unitOfWork, DateTime.MinValue, tipoServicoMultisoftware);
        }

        public static void AtualizarPrevisaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, DateTime dataTerminoCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (!configuracao.PermitirAtualizarPrevisaoControleEntrega)
                return;

            if (carga.TipoOperacao?.NaoGerarControleColetaEntrega ?? false)
                return;

            if (dataTerminoCarregamento != DateTime.MinValue)
                carga.DataInicioViagemPrevista = dataTerminoCarregamento;

            DefinirPrevisaoCargaEntrega(carga, true, true, configuracao, unitOfWork, tipoServicoMultisoftware, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);
        }

        public static void DefinirPrevisaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            bool atualizarDataReprogramada = carga.DataInicioViagem != null;
            DefinirPrevisaoCargaEntrega(carga, false, atualizarDataReprogramada, configuracao, unitOfWork, tipoServicoMultisoftware, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, configuracaoControleEntrega);
        }

        public static bool CargaPossuiControleEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            if (carga == null)
                return false;

            return repCargaEntrega.CargaPossuiControleEntrega(carga.Codigo);
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> ObterAtendimentosCargaMobile(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> paradasAtendimento = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>();

            if (carga == null)
                return paradasAtendimento;

            List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarAnalisesAtendimentosPorCargaComEntrega(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosCarga = repChamado.BuscarAtendimentosPorCargaComEntrega(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada parada = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada()
                {
                    Codigo = cargaEntrega.Codigo,
                    Atendimentos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>()
                };

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = (from obj in chamadosCarga where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                    parada.Atendimentos.Add(ObterAtendimento(chamado, chamadoAnalises));

                paradasAtendimento.Add(parada);
            }

            return paradasAtendimento;
        }

        public static List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga> ObterControleEntregaCargaMobile(DateTime data, int codigoMotorista, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga> documentosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(codigoMotorista);
            if (motorista != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = null;
                if (motorista.Cliente == null)
                    cargas = repCarga.BuscarCargaPorMotorista(codigoMotorista, 2, !configuracao.OrdenarCargasMobileCrescente, false, configuracao.ExibirEntregaAntesEtapaTransporte, configuracao.HorasCargaExibidaNoApp, null, null, null);
                else
                    cargas = repCarga.BuscarCargaPorClienteDestino(motorista.Cliente.CPF_CNPJ, 2);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga cargaMob = ConverterCargaControleEntrega(carga, unitOfWork);
                    documentosMob.Add(cargaMob);
                }
            }
            return documentosMob;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga ConverterCargaControleEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            return ConverterCargaControleEntrega(carga, filtrarCodigoEntrega: 0, unitOfWork: unitOfWork);
        }

        public static Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga ConverterCargaControleEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, int filtrarCodigoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Servicos.Embarcador.Mobile.Cargas.Carga();
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.WebService.Filial.Filial serFilial = new WebService.Filial.Filial(unitOfWork);
            Servicos.WebService.Frota.Veiculo serWSVeiculo = new Servicos.WebService.Frota.Veiculo(unitOfWork);

            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhotos = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();

            Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            Servicos.WebService.Empresa.Motorista serMotorista = new WebService.Empresa.Motorista();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoMobile configuracaoMobile = carga.TipoOperacao?.ConfiguracaoMobile;
            Dominio.Entidades.Cliente remetente = ObterRemetente(carga.Codigo, unitOfWork);
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarAtendimentosPorCargaComEntrega(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarAnalisesAtendimentosPorCargaComEntrega(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCarga(carga.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga cargaMob = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga
            {
                Codigo = carga.Codigo,

                NumeroCargaEmbarcador = carga.CodigoCargaEmbarcador,
                DataCarregamentoCarga = carga.DataCarregamentoCarga,

                Configuracoes = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoCarga
                {
                    ObrigarFotoNaDevolucao = configuracaoEmbarcador.ObrigarFotoNaDevolucao,
                    ObrigarFotoNaEntrega = configuracaoEmbarcador.ObrigarFotoNaEntrega,
                    ExigeSenhaClienteRecebimento = carga.TipoOperacao?.ExigeSenhaConfirmacaoEntrega ?? false,
                    NumeroTentativasSenhaClientePermitidas = carga.TipoOperacao?.NumeroTentativaSenhaConfirmacaoEntrega ?? 0,
                    PermiteQRCodeMobile = configuracaoEmbarcador.PermiteQRCodeMobile,
                    PermiteImpressaoMobile = carga.TipoOperacao?.PermiteImpressaoMobile ?? false,
                    ExibirCalculadoraMobile = carga.TipoOperacao?.ExibirCalculadoraMobile ?? false,
                    PermiteRetificarMobile = carga.TipoOperacao?.PermiteRetificarMobile ?? false,
                    ServerChatURL = configuracaoEmbarcador.ServerChatURL,
                    NaoPermiteRejeitarEntrega = carga.TipoOperacao?.NaoPermiteRejeitarEntrega ?? false,
                    HabilitarControleFluxoNFeDevolucaoChamado = configuracaoEmbarcador.HabilitarControleFluxoNFeDevolucaoChamado,
                    ObrigarJustificativaSolicitacoesForaAreaCliente = configuracaoEmbarcador.JustificarEntregaForaDoRaio || configuracaoMobile.SolicitarJustificativaRegistroForaRaio,
                    ObrigarFotoCanhoto = configuracaoMobile?.ObrigarFotoCanhoto ?? false,
                    ObrigarAssinaturaEntrega = configuracaoMobile?.ObrigarAssinaturaEntrega ?? false,
                    ObrigarDadosRecebedor = configuracaoMobile?.ObrigarDadosRecebedor ?? false,
                    ForcarPreenchimentoSequencialMobile = configuracaoMobile?.ForcarPreenchimentoSequencial ?? false,
                    PermiteFotos = configuracaoMobile?.PermiteFotosEntrega ?? false,
                    PermiteFotosColeta = configuracaoMobile?.PermiteFotosColeta ?? false,
                    PermiteConfirmarChegadaEntrega = configuracaoMobile?.PermiteConfirmarChegadaEntrega ?? false,
                    PermiteConfirmarChegadaColeta = configuracaoMobile?.PermiteConfirmarChegadaColeta ?? false,
                    ControlarTempoColeta = configuracaoMobile?.ControlarTempoColeta ?? false,
                    NaoUtilizarProdutosNaColeta = configuracaoMobile?.NaoUtilizarProdutosNaColeta ?? false,
                    PermitirVisualisarProgramacaoAntesViagem = configuracaoMobile?.PermitirVisualizarProgramacaoAntesViagem ?? false,
                    QuantidadeMinimasFotos = configuracaoMobile?.QuantidadeMinimasFotosEntrega ?? 0,
                    QuantidadeMinimasFotosColeta = configuracaoMobile?.QuantidadeMinimasFotosColeta ?? 0,
                    PermiteEventos = configuracaoMobile?.PermiteEventos ?? false,
                    PermiteChat = configuracaoMobile?.PermiteChat ?? false,
                    PermiteSAC = configuracaoMobile?.PermiteSAC ?? false,
                    ObrigarAssinaturaProdutor = configuracaoMobile?.ObrigarAssinaturaProdutor ?? false,
                    PermiteConfirmarEntrega = configuracaoMobile?.PermiteConfirmarEntrega ?? true,
                    BloquearRastreamento = configuracaoMobile?.BloquearRastreamento ?? false,
                    PermiteCanhotoModoManual = configuracaoMobile?.PermiteCanhotoModoManual ?? false,
                    PermiteEntregaParcial = configuracaoMobile?.PermiteEntregaParcial ?? false,
                    ControlarTempoEntrega = configuracaoMobile?.ControlarTempoEntrega ?? false,
                    ExibirRelatorio = configuracaoMobile?.ExibirRelatorio ?? false,
                    NaoRetornarColetas = configuracaoMobile?.NaoRetornarColetas ?? false,
                    DevolucaoProdutosPorPeso = carga?.TipoOperacao?.DevolucaoProdutosPorPeso ?? false,
                    ObrigarHandlingUnit = repCargaIntegracao.ExistePorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre),
                    ValidarCapacidadeMaximaNoApp = carga?.Veiculo?.ModeloVeicularCarga?.ValidarCapacidadeMaximaNoApp ?? false,
                    ObrigarChavesNfe = carga?.TipoOperacao?.ConfiguracaoMobile?.PermitirEscanearChavesNfe ?? false,
                    SolicitarReconhecimentoFacialDoRecebedor = carga?.TipoOperacao?.ConfiguracaoMobile?.SolicitarReconhecimentoFacialDoRecebedor ?? false,
                    PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte = carga?.TipoOperacao?.PermiteEnviarNotasComplementaresAposEmissaoDocumentosTransporte ?? false,
                },

                Filial = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
                {
                    CPFCNPJ = carga.Filial?.CNPJ ?? string.Empty,
                    RazaoSocial = carga.Filial?.Descricao ?? string.Empty,
                },
                ViagemIniciada = carga.DataInicioViagem.HasValue,
                Origens = carga?.DadosSumarizados?.Origens ?? string.Empty,
                Destinos = carga?.DadosSumarizados?.Destinos ?? string.Empty,
                DataSaida = carga.DataFinalizacaoEmissao ?? DateTime.Now,
                DataCarga = carga.DataCriacaoCarga,
                TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Pedido.TipoOperacao
                {
                    Codigo = carga?.TipoOperacao?.Codigo ?? 0,
                    Descricao = carga?.TipoOperacao?.Descricao ?? string.Empty
                },
                TipoCarga = new Dominio.ObjetosDeValor.Embarcador.Pedido.TipoCarga
                {
                    Codigo = carga?.TipoDeCarga?.Codigo ?? 0,
                    Descricao = carga?.TipoDeCarga?.Descricao ?? string.Empty
                },
                Peso = carga?.DadosSumarizados?.PesoTotal ?? 0,
                Pallets = 0, // verificar
                QuantidadeNotas = 0, //verificar
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaColetaEntega.AguarandoAceita, //Verificar
                Tempertura = 0, //Verificar
                Polilinha = cargaRotaFrete?.PolilinhaRota ?? string.Empty,
                Veiculo = serWSVeiculo.ConverterObjetoConjuntoVeiculos(carga.Veiculo, carga.VeiculosVinculados.ToList(), unitOfWork),
                Paradas = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>(),
                Notas = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota>(),
                Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(),
                Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>()
            };

            foreach (Dominio.Entidades.Usuario motorista in carga.Motoristas.ToList())
                cargaMob.Motoristas.Add(serMotorista.ConverterObjetoMotorista(motorista));

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repCargaEntregaProduto.BuscarPorCargaPaginado(carga.Codigo, 0, 1000); //DECATHLON - Feito retornar  1000 pois ao retornar muitos produtos APP Mobile não funciona
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> paradas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            if (configuracaoMobile.NaoRetornarColetas)
                paradas = repCargaEntrega.BuscarEntregasPorCarga(carga.Codigo);
            else
                paradas = repCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (filtrarCodigoEntrega > 0)
                paradas = paradas.Where(o => o.Codigo == filtrarCodigoEntrega).ToList();

            cargaMob.Configuracoes.AtualizarCargaAutomaticamente = paradas.Any(o => o.PossuiNotaCobertura);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPaginado(carga.Codigo, 0, 1000); //DECATHLON - Feito retornar  1000 pois ao retornar muitos produtos APP Mobile não funciona
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade = repCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in paradas)
            {
                cargaMob.Paradas.Add(ObterParada(cargaEntrega, cargaEntregaNotasFiscais, cargaEntregaPedidos, cargaEntregaProdutos, cargaPedidoProdutos, cargaCargaPedidoProdutoDivisaoCapacidade, chamados, chamadoAnalises, canhotos, configuracaoEmbarcador, unitOfWork));
            }

            return cargaMob;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada ObterParada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade, List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosCarga, List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalisesCarga, List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
            Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhotos = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList servicoCheckList = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscaisParada = (from obj in cargaEntregaNotasFiscais where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaPedidosParada = (from obj in cargaEntregaPedidos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
            Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoBase = cargaPedidosParada.FirstOrDefault()?.CargaPedido?.Pedido;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutosParada = (from obj in cargaEntregaProdutos where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = (from obj in chamadosCarga where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();

            TipoCheckList tipoChecklist = cargaEntrega.Coleta ? TipoCheckList.Coleta : TipoCheckList.Entrega;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta> perguntasChecklist = repCargaEntregaCheckListPergunta.BuscarPerguntasOrdenadasPorCargaEntrega(cargaEntrega.Codigo, tipoChecklist);

            TimeSpan? tempoProgramadaColeta = (pedidoBase?.DataPrevisaoSaida - pedidoBase?.DataInicialColeta);

            Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada parada = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada
            {
                Codigo = cargaEntrega.Codigo,
                DataInicioEntrega = cargaEntrega.DataInicio,
                Data = cargaEntrega.DataFim,
                DataConfirmacao = cargaEntrega.DataConfirmacao,

                // Carregamento e descarga. Depois da refatoração, eles compartilham os dados com início e final. Pode ser removido no futuro caso o app esteja tratando essa nova forma
                DataInicioCarregamento = cargaEntrega.DataInicio,
                DataTerminoCarregamento = cargaEntrega.DataFim,
                DataInicioDescarga = cargaEntrega.DataInicio,
                DataTerminoDescarga = cargaEntrega.DataFim,

                EntradaPropriedade = cargaEntrega.DataEntradaRaio,
                SaidaPropriedade = cargaEntrega.DataSaidaRaio,
                DataProgramadaColeta = pedidoBase?.DataInicialColeta,
                DataProgramadaDescarga = pedidoBase?.PrevisaoEntrega,
                TempoProgramadoColeta = tempoProgramadaColeta.HasValue ? $"{(int)tempoProgramadaColeta.Value.TotalHours}:{tempoProgramadaColeta.Value.Minutes}" : string.Empty,
                Ordem = cargaEntrega.Ordem,
                PossuiReentrega = cargaEntrega.Reentrega,
                DevolucaoParcial = cargaEntrega.DevolucaoParcial,
                DiferencaDevolucao = cargaEntrega.NotificarDiferencaDevolucao,
                CheckList = perguntasChecklist.Count() > 0 ? servicoCheckList.ObterObjetoMobileCheckList(perguntasChecklist) : null,
                Cliente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa
                {
                    CPFCNPJ = cargaEntrega.Cliente?.CPF_CNPJ.ToString(),
                    RazaoSocial = cargaEntrega.Cliente?.Descricao,
                    SenhaConfirmacaoColetaEntrega = cargaEntrega.Cliente?.SenhaConfirmacaoColetaEntrega,
                    NomeFantasia = cargaEntrega.Cliente?.NomeFantasia,
                    CodigoIntegracao = cargaEntrega.Cliente?.CodigoIntegracao,
                    RaioEmMetros = (cargaEntrega.Cliente != null && cargaEntrega.Cliente.RaioEmMetros.HasValue && cargaEntrega.Cliente.RaioEmMetros.Value > 0) ? cargaEntrega.Cliente.RaioEmMetros.Value : configuracao.RaioPadrao,
                    Endereco = getEnderecoCliente(cargaEntrega)
                },
                Endereco = cargaEntrega.ClienteOutroEndereco?.EnderecoCompletoCidadeeEstado ?? cargaEntrega.Cliente?.EnderecoCompletoCidadeeEstado ?? string.Empty,
                Senha = (from obj in cargaPedidosParada select obj.CargaPedido).FirstOrDefault()?.Pedido?.SenhaAgendamentoCliente ?? "",
                Situacao = cargaEntrega.SituacaoParaMobile,
                Peso = (from obj in cargaPedidosParada select obj.CargaPedido.Peso).Sum(),
                Coleta = cargaEntrega.Coleta,
                Tipo = cargaEntrega.TipoCargaEntrega,
                ColetaAdicional = cargaEntrega.ColetaAdicional,
                Pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido>(),
                MotivoDevolucao = (!cargaEntrega.Coleta && cargaEntrega.MotivoRejeicao != null) ? new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao()
                {
                    Codigo = cargaEntrega.MotivoRejeicao.Codigo,
                    Motivo = cargaEntrega.MotivoRejeicao.Descricao
                } : null,
                MotivoRejeicaoColeta = (cargaEntrega.Coleta && cargaEntrega.MotivoRejeicao != null) ? new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta()
                {
                    Codigo = cargaEntrega.MotivoRejeicao.Codigo,
                    Descricao = cargaEntrega.MotivoRejeicao.Descricao
                } : null,
                MotivoRetificacaoColeta = cargaEntrega.MotivoRetificacaoColeta != null ? new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta()
                {
                    Codigo = cargaEntrega.MotivoRetificacaoColeta.Codigo,
                    Descricao = cargaEntrega.MotivoRetificacaoColeta.Descricao
                } : null,
                Notas = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota>(),
                Canhotos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos.Canhoto>(),
                Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(),
                Atendimentos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento>(),
                JanelaDescarga = getJanelaDescarga(cargaEntrega, unitOfWork),
                NotasFiscais = getNotasFiscais(cargaEntrega, unitOfWork),
                DadosRecebedor = getDadosRecebedor(cargaEntrega),
                DataConfirmacaoChegada = cargaEntrega.DataEntradaRaio,
                ObservacoesPedidos = string.Join(" / ", (from o in cargaPedidosParada where !string.IsNullOrEmpty(o.CargaPedido.Pedido.Observacao) select o.CargaPedido.Pedido.Observacao)),
            };

            foreach (Dominio.Entidades.Embarcador.Chamados.Chamado chamado in chamados)
                parada.Atendimentos.Add(ObterAtendimento(chamado, chamadoAnalisesCarga));

            int numeroCasasDecimais = configuracao?.NumeroCasasDecimaisQuantidadeProduto ?? 2;

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto in cargaEntregaProdutosParada)
            {

                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto();
                produto.Protocolo = cargaEntregaProduto.Codigo;
                produto.Codigo = cargaEntregaProduto.Produto.CodigoProdutoEmbarcador;
                produto.Descricao = cargaEntregaProduto.Produto.Descricao;
                produto.ObrigatorioGuiaTransporteAnimal = cargaEntregaProduto.Produto.ObrigatorioGuiaTransporteAnimal;
                produto.ObrigatorioNFProdutor = cargaEntregaProduto.Produto.ObrigatorioNFProdutor;
                produto.Quantidade = Math.Round(cargaEntregaProduto.Quantidade, numeroCasasDecimais, MidpointRounding.ToEven);
                produto.QuantidadePlanejada = Math.Round(cargaEntregaProduto.QuantidadePlanejada, numeroCasasDecimais, MidpointRounding.ToEven);
                produto.QuantidadeDevolucao = Math.Round(cargaEntregaProduto.QuantidadeDevolucao, numeroCasasDecimais, MidpointRounding.ToEven);
                produto.Lote = cargaEntregaProduto.Lote;
                produto.DataCritica = cargaEntregaProduto.DataCritica;
                produto.UnidadeDeMedida = new Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida()
                {
                    Codigo = cargaEntregaProduto.Produto.Unidade?.Codigo ?? 0,
                    Descricao = cargaEntregaProduto.Produto.Unidade?.Descricao ?? "",
                    UnidadeMedida = cargaEntregaProduto.Produto.Unidade?.UnidadeMedida ?? Dominio.Enumeradores.UnidadeMedida.UN
                };

                if (cargaEntregaProduto.XMLNotaFiscal != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota nota = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota()
                    {
                        Codigo = cargaEntregaProduto.XMLNotaFiscal.Codigo,
                        Chave = cargaEntregaProduto.XMLNotaFiscal.Chave,
                        NumeroNota = cargaEntregaProduto.XMLNotaFiscal.Numero.ToString(),
                        Serie = cargaEntregaProduto.XMLNotaFiscal.Serie,
                        Peso = cargaEntregaProduto.XMLNotaFiscal.Peso,
                        Valor = cargaEntregaProduto.XMLNotaFiscal.Valor,
                        DigitalizacaoCanhotoInteiro = cargaEntregaProduto?.XMLNotaFiscal?.Emitente?.DigitalizacaoCanhotoInteiro ?? false,
                    };
                    produto.nota = nota;
                }

                produto.PesoUnitario = cargaEntregaProduto.Produto.PesoUnitario;

                parada.Produtos.Add(produto);
            }

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosEntrega = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            if (!cargaEntrega.Coleta)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscaisParada)
                {
                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota nota = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota()
                    {
                        Codigo = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo,
                        Chave = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                        NumeroNota = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero.ToString(),
                        Serie = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie,
                        Peso = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso,
                        Valor = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor,
                    };
                    parada.Notas.Add(nota);

                    Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = (from obj in canhotos
                                                                             where
                                                        (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo)
                                                        || (obj.TipoCanhoto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto.Avulso
                                                        && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo)
                                                        )
                                                                             select obj).FirstOrDefault();

                    if (canhoto != null && !canhotosEntrega.Contains(canhoto))
                        canhotosEntrega.Add(canhoto);
                }

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotosEntrega)
                    parada.Canhotos.Add(serCanhotos.ConverterCanhoto(canhoto, null, unitOfWork));
            }


            for (int i = 0; i < cargaPedidosParada.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaPedidoParada = cargaPedidosParada[i];
                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido pedidoParada = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido();
                pedidoParada.Codigo = cargaPedidoParada.CargaPedido.Codigo;
                pedidoParada.NumeroPedido = cargaPedidoParada.CargaPedido.Pedido.NumeroPedidoEmbarcador;
                pedidoParada.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> produtosPedido = (from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == cargaPedidoParada.CargaPedido.Codigo select obj).ToList();

                for (int j = 0; j < produtosPedido.Count; j++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto produtoPedido = produtosPedido[j];
                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto
                    {
                        Protocolo = produtoPedido.Codigo,
                        ObrigatorioGuiaTransporteAnimal = produtoPedido.Produto.ObrigatorioGuiaTransporteAnimal,
                        ObrigatorioNFProdutor = produtoPedido.Produto.ObrigatorioNFProdutor,
                        Codigo = produtoPedido.Produto.CodigoProdutoEmbarcador,
                        Descricao = produtoPedido.Produto.Descricao,
                        Observacao = produtoPedido.Produto.Observacao,
                        Quantidade = produtoPedido.Quantidade,
                        QuantidadeCaixa = produtoPedido.QuantidadeCaixa,
                        QuantidadePorCaixaRealizada = produtoPedido.QuantidadePorCaixaRealizada,
                        QuantidadeCaixasVazias = produtoPedido.QuantidadeCaixasVazias,
                        QuantidadeCaixasVaziasRealizada = produtoPedido.QuantidadeCaixasVaziasRealizada,
                        QuantidadePlanejada = produtoPedido.QuantidadePlanejada,
                        Temperatura = produtoPedido.Temperatura,
                        InformarDadosColeta = produtoPedido.Produto.PossuiIntegracaoColetaMobile,
                        InformarTemperatura = produtoPedido.Produto.ObrigatorioInformarTemperatura,
                        UnidadeDeMedida = new Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida()
                        {
                            Codigo = produtoPedido.Produto.Unidade?.Codigo ?? 0,
                            Descricao = produtoPedido.Produto.Unidade?.Descricao ?? "",
                            UnidadeMedida = produtoPedido.Produto.Unidade?.UnidadeMedida ?? Dominio.Enumeradores.UnidadeMedida.UN
                        },
                        ProdutoDivisoesCapacidade = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade>(),
                        Imuno = produtoPedido.ImunoPlanejado,
                        ImunoRealizado = produtoPedido.ImunoRealizado
                    };

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> pedidoProdutosDivisaoCapacidade = (from obj in cargaCargaPedidoProdutoDivisaoCapacidade where obj.CargaPedidoProduto.Codigo == produtoPedido.Codigo select obj).ToList();
                    for (int d = 0; d < pedidoProdutosDivisaoCapacidade.Count; d++)
                    {
                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade pedidoProdutoDivisaoCapacidade = pedidoProdutosDivisaoCapacidade[d];
                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade modeloVeicularCargaDivisaoCapacidade = pedidoProdutoDivisaoCapacidade.ModeloVeicularCargaDivisaoCapacidade;

                        Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade produtoDivisaoCapacidade = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade
                        {
                            Quantidade = pedidoProdutoDivisaoCapacidade.Quantidade,
                            QuantidadePlanejada = pedidoProdutoDivisaoCapacidade.QuantidadePlanejada,
                            DivisaoCapacidadeModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.DivisaoCapacidadeModeloVeicular()
                            {
                                Codigo = modeloVeicularCargaDivisaoCapacidade.Codigo,
                                Capacidade = modeloVeicularCargaDivisaoCapacidade.Quantidade,
                                Descricao = modeloVeicularCargaDivisaoCapacidade.Descricao,
                                Piso = modeloVeicularCargaDivisaoCapacidade.Piso,
                                Coluna = modeloVeicularCargaDivisaoCapacidade.Coluna,
                                UnidadeDeMedida = new Dominio.ObjetosDeValor.Embarcador.Carga.UnidadeDeMedida()
                                {
                                    Codigo = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Codigo ?? 0,
                                    UnidadeMedida = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.UnidadeMedida ?? Dominio.Enumeradores.UnidadeMedida.UN,
                                    Descricao = modeloVeicularCargaDivisaoCapacidade.UnidadeMedida?.Descricao ?? ""
                                }
                            }
                        };

                        produto.ProdutoDivisoesCapacidade.Add(produtoDivisaoCapacidade);
                    }

                    pedidoParada.Produtos.Add(produto);
                }
                parada.Pedidos.Add(pedidoParada);
            }

            return parada;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco getEnderecoCliente(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (cargaEntrega.ClienteOutroEndereco != null)
            {
                // Localidade de entrega. Por algum motivo, se chama "ClienteOutroEndereco", mas é a localidade da entrega
                return new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
                {
                    Latitude = ConverterCoordenada(cargaEntrega.ClienteOutroEndereco?.Latitude),
                    Longitude = ConverterCoordenada(cargaEntrega.ClienteOutroEndereco?.Longitude),
                    Logradouro = cargaEntrega.ClienteOutroEndereco?.Endereco,
                    Bairro = cargaEntrega.ClienteOutroEndereco?.Bairro ?? string.Empty,
                    Numero = cargaEntrega.ClienteOutroEndereco?.Numero ?? string.Empty,
                    Telefone = cargaEntrega.Cliente?.Telefone1 ?? string.Empty,
                    Telefone2 = cargaEntrega.Cliente?.Telefone2 ?? string.Empty,
                    CodigoTelefonicoPais = cargaEntrega.Cliente?.Localidade?.Pais?.CodigoTelefonico.ToString() ?? string.Empty,
                    Cidade = new Dominio.ObjetosDeValor.Localidade
                    {
                        Codigo = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Codigo ?? 0,
                        SiglaUF = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Estado?.Sigla,
                        Descricao = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Descricao,
                        IBGE = cargaEntrega?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0,
                        Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais
                        {
                            CodigoPais = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Pais?.Codigo ?? 0,
                            NomePais = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Pais?.Descricao ?? string.Empty,
                            SiglaPais = cargaEntrega?.ClienteOutroEndereco?.Localidade?.Pais?.Abreviacao ?? string.Empty,
                        }
                    }

                };
            }

            // Endereço da localidade normal do cliente
            return new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco
            {
                Latitude = ConverterCoordenada(cargaEntrega.Cliente?.Latitude),
                Longitude = ConverterCoordenada(cargaEntrega.Cliente?.Longitude),
                Logradouro = cargaEntrega.Cliente?.Endereco,
                Bairro = cargaEntrega.Cliente?.Bairro ?? string.Empty,
                Numero = cargaEntrega.Cliente?.Numero ?? string.Empty,
                Telefone = cargaEntrega.Cliente?.Telefone1 ?? string.Empty,
                Telefone2 = cargaEntrega.Cliente?.Telefone2 ?? string.Empty,
                CodigoTelefonicoPais = cargaEntrega.Cliente?.Localidade?.Pais?.CodigoTelefonico.ToString() ?? string.Empty,
                Cidade = new Dominio.ObjetosDeValor.Localidade
                {
                    Codigo = cargaEntrega?.Cliente?.Localidade?.Codigo ?? 0,
                    SiglaUF = cargaEntrega?.Cliente?.Localidade?.Estado?.Sigla,
                    Descricao = cargaEntrega?.Cliente?.Localidade?.Descricao,
                    IBGE = cargaEntrega?.Cliente?.Localidade?.CodigoIBGE ?? 0,
                    Pais = new Dominio.ObjetosDeValor.Embarcador.Localidade.Pais
                    {
                        CodigoPais = cargaEntrega?.Cliente?.Localidade?.Pais?.Codigo ?? 0,
                        NomePais = cargaEntrega?.Cliente?.Localidade?.Pais?.Descricao ?? string.Empty,
                        SiglaPais = cargaEntrega?.Cliente?.Localidade?.Pais?.Abreviacao ?? string.Empty,
                    }
                }

            };
        }

        public static string SalvarFotoGTA(int codigoClienteMultisoftware, int codigoCargaEntrega, string tokenImagem, Repositorio.UnitOfWork unitOfWork, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA repCargaEntregaFotoGTA = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

            string extensao = ".jpg";

            if (cargaEntrega == null)
                return "Carga entrega não encontrada";

            string nomeArquivo = "GTA_" + codigoCargaEntrega + "_" + codigoClienteMultisoftware + extensao;
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA cargaEntregaFotoGTA = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoGTA
            {
                GuidArquivo = tokenImagem,
                NomeArquivo = nomeArquivo,
                CargaEntrega = cargaEntrega,
                DataEnvioImagem = data != DateTime.MinValue ? data : DateTime.Now,
            };


            repCargaEntregaFotoGTA.Inserir(cargaEntregaFotoGTA, auditado);
            if (auditado != null)
                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, string.Format(Localization.Resources.Cargas.ControleEntrega.EnviouAnexoXNaDataX, nomeArquivo, data.ToDateTimeString()), unitOfWork);

            string caminhoEntrega = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" }), tokenImagem + extensao);
            string caminhoEntregaBackup = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedidoBackup" }), tokenImagem + extensao);

            //Imagens estão excluindo do server, backup será para validar se a mesma ficou armazenada, referente tarefa #31513
            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoEntrega))
            {
                Utilidades.IO.FileStorageService.Storage.Copy(caminhoEntrega, caminhoEntregaBackup);
                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoEntregaBackup))
                    Log.TratarErro("Imagem não foi salva para o backup do Controle de Entrega código " + codigoCargaEntrega);
            }
            else
                Log.TratarErro("Imagem não foi salva para o Controle de Entrega código " + codigoCargaEntrega);

            return string.Empty;
        }

        private static string getJanelaDescarga(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime dataEntrega = cargaEntrega.DataReprogramada ?? cargaEntrega.DataPrevista ?? DateTime.Now;
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> janelasDescarga = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterJanelaDescarregamento(cargaEntrega, dataEntrega, unitOfWork);
            return janelasDescarga != null ? string.Join(", ", (from janelaDescarga in janelasDescarga select janelaDescarga?.HoraInicio.ToString(@"hh\:mm") + " - " + janelaDescarga?.HoraTermino.ToString(@"hh\:mm"))) : string.Empty;
        }

        private static string getNotasFiscais(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscals = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            return String.Join(", ", (from notas in cargaEntregaNotaFiscals select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero));
        }

        private static Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor getDadosRecebedor(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            if (cargaEntrega.DadosRecebedor == null)
            {
                return null;
            }

            Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor retorno = new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor();
            retorno.Nome = cargaEntrega.DadosRecebedor?.Nome ?? "";
            retorno.CPF = cargaEntrega.DadosRecebedor?.CPF ?? "";
            retorno.DataEntrega = cargaEntrega.DadosRecebedor?.DataEntrega.ToString("dd/MM/yyyy HH:mm");
            return retorno;
        }

        public static void SincronizarEntregaOrigem(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = null)
        {
            if (cargaEntrega.CargaEntregaOrigem <= 0)
                return;

            if (configControleEntrega == null)
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            }
            if (configControleEntrega == null || !configControleEntrega.ConsiderarCargaOrigemParaEntregasTransbordadas)
                return;

            // Atenção esta regra considera que todas as cargas agrupadas agrupam CargaEntrega por cliente em se mudando isso teremos que criar uma tabela para corelacionar as origens 
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada> lstVinculoEntreCargaEntregaComCargaEntregaTransbordada = repTransbordo.ConsultarVinculoEntreCargaEntregaComCargaEntregaTransbordada(cargaEntrega.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregasOrigem = repCargaEntrega.BuscarPorCodigos(lstVinculoEntreCargaEntregaComCargaEntregaTransbordada.Where(x => x.CodigoCliente == cargaEntrega.Cliente?.CPF_CNPJ).Select(x => x.CodigoCargaEntregaOrigem).ToList());

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaOrigem in cargasEntregasOrigem)
            {
                cargaEntregaOrigem.DataEntradaRaio = cargaEntrega.DataEntradaRaio;
                cargaEntregaOrigem.DataLimitePermanenciaRaio = cargaEntrega.DataLimitePermanenciaRaio;
                cargaEntregaOrigem.DataAgendamento = cargaEntrega.DataAgendamento;
                cargaEntregaOrigem.DataFim = cargaEntrega.DataFim;
                cargaEntregaOrigem.LatitudeFinalizada = cargaEntrega.LatitudeFinalizada;
                cargaEntregaOrigem.LongitudeFinalizada = cargaEntrega.LongitudeFinalizada;
                cargaEntregaOrigem.DataPrevista = cargaEntrega.DataPrevista;
                cargaEntregaOrigem.DataFimPrevista = cargaEntrega.DataFimPrevista;
                cargaEntregaOrigem.DataReprogramada = cargaEntrega.DataReprogramada;
                cargaEntregaOrigem.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino;
                cargaEntregaOrigem.OrigemCriacaoDataAgendamentoCargaEntrega = cargaEntrega.OrigemCriacaoDataAgendamentoCargaEntrega;
                cargaEntregaOrigem.DataAvaliacao = cargaEntrega.DataAvaliacao;
                cargaEntregaOrigem.AvaliacaoGeral = cargaEntrega.AvaliacaoGeral;
                cargaEntregaOrigem.ObservacaoAvaliacao = cargaEntrega.ObservacaoAvaliacao;
                cargaEntregaOrigem.Ordem = cargaEntrega.Ordem;
                cargaEntregaOrigem.FinalizadaManualmente = cargaEntrega.FinalizadaManualmente;
                cargaEntregaOrigem.Observacao = cargaEntrega.Observacao;
                cargaEntregaOrigem.DataInicio = cargaEntrega.DataInicio;
                cargaEntregaOrigem.DataConfirmacao = cargaEntrega.DataConfirmacao;
                cargaEntregaOrigem.DadosRecebedor = cargaEntrega.DadosRecebedor;
                cargaEntregaOrigem.SituacaoOnTime = cargaEntrega.SituacaoOnTime;
                cargaEntregaOrigem.Situacao = cargaEntrega.Situacao;
                cargaEntregaOrigem.JustificativaOnTime = cargaEntrega.JustificativaOnTime;
                cargaEntregaOrigem.DataAgendamentoEntregaTransportador = cargaEntrega.DataAgendamentoEntregaTransportador;
                cargaEntregaOrigem.DataSaidaRaio = cargaEntrega.DataSaidaRaio;
                //cargaEntregaOrigem.TratativaDevolucao = cargaEntrega.TratativaDevolucao;
                cargaEntregaOrigem.ChamadoEmAberto = cargaEntrega.ChamadoEmAberto;
                cargaEntregaOrigem.MotivoRejeicao = cargaEntrega.MotivoRejeicao;
                cargaEntregaOrigem.MotoristaACaminho = cargaEntrega.MotoristaACaminho;

                repCargaEntrega.Atualizar(cargaEntregaOrigem);
                //cargaEntrega.ResponsavelFinalizacaoManual 
                //cargaEntrega.Carga.DataAtualizacaoCarga = DateTime.Now;
            }
        }

        public static DateTime? CalcularPermanenciaMaxima(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = null, Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento janela = null)
        {
            if (!cargaEntrega.DataEntradaRaio.HasValue)
                return null;

            if (cargaEntrega.DataSaidaRaio.HasValue)
                return cargaEntrega.DataLimitePermanenciaRaio;

            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
            janela ??= repCargaJanelaDescarregamento.BuscarPorCarga(cargaEntrega.Carga.Codigo);

            int tempoPadraoDeEntrega = janela?.CentroDescarregamento?.TempoPadraoDeEntrega ?? 0;

            if (tempoPadraoDeEntrega <= 0)
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                configuracao ??= repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                tempoPadraoDeEntrega = configuracao?.TempoPadraoDeEntrega ?? 0;
            }

            if (tempoPadraoDeEntrega <= 0)
                return null;

            return cargaEntrega.DataEntradaRaio.Value.AddMinutes(tempoPadraoDeEntrega);
        }

        public static void SalvarPrevisaoCargaEntrega(List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, bool atualizarDataPrevisaoEntrega, bool atualizarDataPrevisaoEntregaReprogramada, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool inicial = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega Origem = OrigemSituacaoEntrega.UsuarioMultiEmbarcador, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregasCalculoPrevisao = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = null, Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTrizy configIntegracaoTrizy = null)
        {
            if (previsoesCargaEntrega == null || previsoesCargaEntrega.Count == 0)
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoTrizy repConfiguracaoTrizy = new Repositorio.Embarcador.Configuracoes.IntegracaoTrizy(unitOfWork);

            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, configuracao);
            Servicos.Embarcador.Integracao.Diaria.DiariaMotorista servicoDiariaMotorista = new Servicos.Embarcador.Integracao.Diaria.DiariaMotorista(unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);

            if (configIntegracaoTrizy == null)
                configIntegracaoTrizy = repConfiguracaoTrizy.BuscarPrimeiroRegistro();

            if (configuracaoControleEntrega == null)
                configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

            if (configuracaoOcorrenciaEntregasCalculoPrevisao == null)
                configuracaoOcorrenciaEntregasCalculoPrevisao = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.CalculoPrevisao);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };

            DateTime? dataFimViagemPrevista = null;
            int total = cargasEntrega.Count;
            bool dataEntregaPrevistaAtualizada = false;

            for (int i = 0; i < total; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargasEntrega[i];

                cargaEntrega.Initialize();

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidosPorEntrega = (from o in cargaEntregaPedidos where o.CargaEntrega.Codigo == cargaEntrega.Codigo select o).ToList();
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoBase = cargaEntregaPedidosPorEntrega.FirstOrDefault()?.CargaPedido;
                bool utilizarDataCarregamentoPedidoBase = (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS && cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Coleta && cargaPedidoBase != null && cargaPedidoBase.Pedido.DataCarregamentoPedido.HasValue);
                bool utilizarPrevisaoEntregaCargaPedidoBase = (cargaPedidoBase?.PrevisaoEntrega.HasValue ?? false);
                bool utilizarPrevisaoEntregaPedidoBase = false;

                if (configuracaoControleEntrega.UtilizarPrevisaoEntregaPedidoComoDataPrevista)
                {
                    if (cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Coleta && (cargaPedidoBase?.Pedido.DataCarregamentoPedido.HasValue ?? false))
                        utilizarPrevisaoEntregaPedidoBase = true;
                    else if ((cargaPedidoBase?.Pedido.PrevisaoEntrega.HasValue ?? false))
                        utilizarPrevisaoEntregaPedidoBase = true;
                }

                DateTime? dataPrevistaAnterior = cargaEntrega.DataPrevista;

                if (utilizarDataCarregamentoPedidoBase)
                {
                    cargaEntrega.DataPrevista = cargaPedidoBase.Pedido.DataCarregamentoPedido.Value;
                    cargaEntrega.DataFimPrevista = cargaPedidoBase.Pedido.DataCarregamentoPedido.Value;
                }
                else if (utilizarPrevisaoEntregaCargaPedidoBase)
                {
                    cargaEntrega.DataPrevista = cargaPedidoBase.PrevisaoEntrega;
                    cargaEntrega.DataFimPrevista = cargaPedidoBase.PrevisaoEntrega;
                }
                else if (utilizarPrevisaoEntregaPedidoBase)
                {
                    if (cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Coleta)
                    {
                        cargaEntrega.DataPrevista = cargaPedidoBase.Pedido.DataCarregamentoPedido;
                        cargaEntrega.DataFimPrevista = cargaPedidoBase.Pedido.DataCarregamentoPedido;
                    }
                    else
                    {
                        cargaEntrega.DataPrevista = cargaPedidoBase.Pedido.PrevisaoEntrega;
                        cargaEntrega.DataFimPrevista = cargaPedidoBase.Pedido.PrevisaoEntrega;
                    }
                }
                else
                {
                    if (!cargaEntrega.DataPrevista.HasValue || atualizarDataPrevisaoEntrega)
                        cargaEntrega.DataPrevista = previsoesCargaEntrega[i].DataInicioEntregaPrevista;

                    if (!cargaEntrega.DataFimPrevista.HasValue || atualizarDataPrevisaoEntrega)
                        cargaEntrega.DataFimPrevista = previsoesCargaEntrega[i].DataFimEntregaPrevista;
                }

                if (cargaEntrega.DataConfirmacao == null && atualizarDataPrevisaoEntregaReprogramada && ValidarAtualizaDataEntregaReprogramada(cargaEntrega, carga))
                    cargaEntrega.DataReprogramada = previsoesCargaEntrega[i].DataInicioEntregaPrevista;

                if (!configuracao.PossuiMonitoramento && carga.Rota?.TempoDeViagemEmMinutos > 0 && carga.DataInicioViagem.HasValue && cargaEntrega.DataConfirmacao == null && ValidarAtualizaDataEntregaReprogramada(cargaEntrega, carga)) //#48671 SIM PEDIRAM PRA FAZER ISSO
                {
                    cargaEntrega.DataReprogramada = carga.DataInicioViagem.Value.AddMinutes(carga.Rota.TempoDeViagemEmMinutos);
                }

                if (previsoesCargaEntrega[i].DistanciaAteDestino > 0)
                    cargaEntrega.DistanciaAteDestino = previsoesCargaEntrega[i].DistanciaAteDestino;

                if (configuracaoControleEntrega?.UtilizarLeadTimeDaTabelaDeFreteParaCalculoDaPrevisaoDeEntrega ?? false)
                    cargaEntrega.DataPrevista = previsoesCargaEntrega[i].DataInicioEntregaPrevista;

                if (previsoesCargaEntrega[i].DataPrevisaoEntregaTransportador.HasValue)
                    cargaEntrega.DataPrevisaoEntregaTransportador = previsoesCargaEntrega[i].DataPrevisaoEntregaTransportador;

                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork, configuracaoControleEntrega);
                if (atualizarDataPrevisaoEntrega && configuracao.PermitirAtualizarPrevisaoEntregaPedidoControleEntrega)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidosPorEntrega)
                    {
                        cargaEntregaPedido.CargaPedido.Pedido.PrevisaoEntrega = cargaEntrega.DataPrevista;
                        repositorioPedido.Atualizar(cargaEntregaPedido.CargaPedido.Pedido);
                    }
                }

                if (dataFimViagemPrevista == null || previsoesCargaEntrega[i].DataFimEntregaPrevista > dataFimViagemPrevista)
                    dataFimViagemPrevista = previsoesCargaEntrega[i].DataFimEntregaPrevista;

                if (dataPrevistaAnterior.HasValue && dataPrevistaAnterior != cargaEntrega.DataPrevista)
                {
                    dataEntregaPrevistaAtualizada = true;

                    cargaEntrega.SetExternalChange(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        De = dataPrevistaAnterior.ToDateString(),
                        Para = cargaEntrega.DataPrevista.ToDateString(),
                        Propriedade = $"Data Entrega"
                    });

                    repositorioCargaEntrega.Atualizar(cargaEntrega, auditado);
                }

                if ((atualizarDataPrevisaoEntrega || inicial) && configuracaoOcorrenciaEntregasCalculoPrevisao?.Count > 0)
                {
                    //se tem configuracao de ocorrencia de evento calculo previsao vamos gerar evento e integrar se existir configuracao
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntregaPedido = null;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega;

                    if (cargaEntrega.Coleta)
                        tipoAplicacaoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta;
                    else
                        tipoAplicacaoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega;

                    configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregasCalculoPrevisao where obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();

                    if (configuracaoOcorrenciaEntregaPedido != null)
                    {
                        //Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = repositorioPosicaoAtual.BuscarPorVeiculo(carga?.Veiculo?.Codigo ?? 0);

                        if (cargaEntrega.Situacao != SituacaoEntrega.Entregue && cargaEntrega.Situacao != SituacaoEntrega.Reentergue)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento()
                            {
                                Carga = cargaEntrega.Carga,
                                CargaEntrega = cargaEntrega,
                                DataOcorrencia = DateTime.Now,
                                DataPosicao = DateTime.Now,
                                DataPrevisaoRecalculada = cargaEntrega.DataPrevista,
                                EventoColetaEntrega = EventoColetaEntrega.CalculoPrevisao,
                                TipoDeOcorrencia = configuracaoOcorrenciaEntregaPedido.TipoDeOcorrencia,
                                Latitude = null,//(decimal)posicaoAtual?.Latitude,
                                Longitude = null,//(decimal)posicaoAtual?.Longitude,
                                Origem = Origem,
                            };

                            servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, true);
                        }
                    }
                }

                if (configuracao.UtilizaAppTrizy && (configIntegracaoTrizy?.EnviarPatchAtualizacoesEntrega ?? false) && !string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy) && !string.IsNullOrWhiteSpace(cargaEntrega.IdTrizy))
                    Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AtualzarEntrega(cargaEntrega, unitOfWork.StringConexao, configuracao);
            }

            if ((cargasEntrega.Where(o => !o.Coleta).Count() == 1) && configuracaoControleEntrega.UtilizarMaiorDataColetaPrevistaComoDataPrevistaParaEntregaUnica)
            {
                DateTime? maiorDataColetaPrevista = cargasEntrega.Where(o => o.Coleta).Max(o => o.DataPrevista);

                if (maiorDataColetaPrevista.HasValue)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega entregaUnica = cargasEntrega.Where(o => !o.Coleta).FirstOrDefault();

                    entregaUnica.DataPrevista = maiorDataColetaPrevista;

                    repositorioCargaEntrega.Atualizar(entregaUnica);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(entregaUnica, repositorioCargaEntrega, unitOfWork, configuracaoControleEntrega);
                }
            }

            carga.DataInicioViagemPrevista = ObterDataInicioViagemPrevista(carga, cargasEntrega, configuracao, unitOfWork);

            if (atualizarDataPrevisaoEntrega)
                carga.DataInicioViagemReprogramada = carga.DataInicioViagem;

            if (carga.DataFimViagemPrevista == null)
                carga.DataFimViagemPrevista = dataFimViagemPrevista;

            if (atualizarDataPrevisaoEntregaReprogramada && dataFimViagemPrevista.HasValue)
                carga.DataFimViagemReprogramada = dataFimViagemPrevista;

            if ((carga.TipoOperacao?.ConfiguracaoCarga?.PermitirAlterarDataRetornoCDCarga ?? false) && carga.SituacaoCarga.IsSituacaoCargaNaoFaturada() && !carga.DataRetornoCD.HasValue)
            {
                DateTime? dataPrevisaoRetorno = servicoDiariaMotorista.ObterDataPrevisaoRetorno(carga);
                if (dataPrevisaoRetorno.HasValue)
                    carga.DataRetornoCD = dataPrevisaoRetorno;
            }

            if (dataEntregaPrevistaAtualizada)
                servicoFluxoGestaoPatio.AtualizarDataPrevisaoInicioEtapas(cargasEntrega);
        }

        public static void AtualizarTendenciasEntrega(Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendencias, Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga ConfigAlertaAtrasoDescarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            DataBaseAlerta? dataBase = ConfigAlertaAtrasoDescarga.DataBaseAlerta;
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> coletasEmTempoOuAdiantadas = repCargaEntrega.BuscarEntregasControleTendenciaAtraso(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Adiantado, true, ConfigAlertaAtrasoDescarga);
            if (coletasEmTempoOuAdiantadas != null && coletasEmTempoOuAdiantadas.Count() > 0)
            {
                //atualizar status;
                repCargaEntrega.AtualizarStatusTendenciasEntregas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Adiantado, coletasEmTempoOuAdiantadas);

                //notificar
                GerarAlertaNotificacaoTendencia(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Adiantado, coletasEmTempoOuAdiantadas, true, dataBase, unitOfWork);
            }

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> coletasNoHorario = repCargaEntrega.BuscarEntregasControleTendenciaAtraso(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nohorario, true, ConfigAlertaAtrasoDescarga);
            if (coletasNoHorario != null && coletasNoHorario.Count() > 0)
            {
                //atualizar status;
                repCargaEntrega.AtualizarStatusTendenciasEntregas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nohorario, coletasNoHorario);

                GerarAlertaNotificacaoTendencia(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nohorario, coletasNoHorario, true, dataBase, unitOfWork);
            }

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> coletasAtrasadas = repCargaEntrega.BuscarEntregasControleTendenciaAtraso(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Atrasado, true, ConfigAlertaAtrasoDescarga);
            if (coletasAtrasadas != null && coletasAtrasadas.Count() > 0)
            {
                //atualizar status;
                repCargaEntrega.AtualizarStatusTendenciasEntregas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Atrasado, coletasAtrasadas);

                GerarAlertaNotificacaoTendencia(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Atrasado, coletasAtrasadas, true, dataBase, unitOfWork);
            }

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> entregasEmTempoOuAdiantadas = repCargaEntrega.BuscarEntregasControleTendenciaAtraso(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Adiantado, false, ConfigAlertaAtrasoDescarga);
            if (entregasEmTempoOuAdiantadas != null && entregasEmTempoOuAdiantadas.Count() > 0)
            {
                //atualizar status;
                repCargaEntrega.AtualizarStatusTendenciasEntregas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Adiantado, entregasEmTempoOuAdiantadas);

                GerarAlertaNotificacaoTendencia(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Adiantado, entregasEmTempoOuAdiantadas, false, dataBase, unitOfWork);
            }

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> entregasHorario = repCargaEntrega.BuscarEntregasControleTendenciaAtraso(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nohorario, false, ConfigAlertaAtrasoDescarga);
            if (entregasHorario != null && entregasHorario.Count() > 0)
            {
                //atualizar status;
                repCargaEntrega.AtualizarStatusTendenciasEntregas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nohorario, entregasHorario);

                GerarAlertaNotificacaoTendencia(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Nohorario, entregasHorario, false, dataBase, unitOfWork);
            }

            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> entregasAtrasadas = repCargaEntrega.BuscarEntregasControleTendenciaAtraso(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Atrasado, false, ConfigAlertaAtrasoDescarga);
            if (entregasAtrasadas != null && entregasAtrasadas.Count() > 0)
            {
                //atualizar status;
                repCargaEntrega.AtualizarStatusTendenciasEntregas(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Atrasado, entregasAtrasadas);

                GerarAlertaNotificacaoTendencia(configuracaoTempoTendencias, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntrega.Atrasado, entregasAtrasadas, false, dataBase, unitOfWork);
            }
        }

        public decimal? ObterQuantidadePlanejada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga)
        {
            if (cargaEntregaPedidos.Count == 0)
                return null;

            decimal? quantidadePlanejada = null;

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProduto = (from obj in listaCargaPedidoProdutoCarga where obj.CargaPedido.Codigo == cargaEntregaPedido.CargaPedido.Codigo select obj).ToList();

                if (!listaCargaPedidoProduto.Any(o => o.Produto.PossuiIntegracaoColetaMobile))
                    continue;

                quantidadePlanejada = (quantidadePlanejada ?? 0) + (from produto in listaCargaPedidoProduto select produto.QuantidadePlanejada).Sum();
            }

            return quantidadePlanejada;
        }

        public decimal? ObterQuantidadeTotal(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoCarga)
        {
            if (cargaEntregaPedidos.Count == 0)
                return null;

            decimal? quantidadeTotal = null;

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProduto = (from obj in listaCargaPedidoProdutoCarga where obj.CargaPedido.Codigo == cargaEntregaPedido.CargaPedido.Codigo select obj).ToList();

                if (!listaCargaPedidoProduto.Any(o => o.Produto.PossuiIntegracaoColetaMobile))
                    continue;

                quantidadeTotal = (quantidadeTotal ?? 0) + (from produto in listaCargaPedidoProduto select produto.Quantidade).Sum();
            }

            return quantidadeTotal;
        }

        public byte[] ObterPdfRecebimentoProduto(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            byte[] pdf = ReportRequest.WithType(ReportType.RecebimentoProduto)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoCargaEntrega", cargaEntrega.Codigo)
                .CallReport()
                .GetContentFile();

            if (pdf == null)
                throw new ServicoException("Não foi possível gerar a relação de Recebimento Produto.");

            return pdf;
        }

        public void SalvarDevolucaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notaFiscais, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos, Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, SituacaoEntrega situacaoEntrega, bool devolucaoParcial, bool finalizandoAnaliseDevolucao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (situacaoEntrega == SituacaoEntrega.Revertida)
            {
                Chamado.Chamado.ReverterQuantidadeDevolucao(chamado, _unitOfWork);

                return;
            }

            Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega = null;
            bool deveAtualizarSituacaoNota = true;

            SalvarDevolucaoCargaEntrega(cargaEntrega, notaFiscais, produtos, motivoChamado, chamado, configuracao, auditado, situacaoEntrega, devolucaoParcial, finalizandoAnaliseDevolucao, configuracaoChamado, tipoServicoMultisoftware, motivoDevolucaoEntrega, deveAtualizarSituacaoNota);
        }

        public void SalvarDevolucaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notaFiscais, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos, Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, SituacaoEntrega situacaoEntrega, bool devolucaoParcial, bool finalizandoAnaliseDevolucao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDevolucaoEntrega, bool deveAtualizarSituacaoNota)
        {
            if (cargaEntrega == null)
                return;

            bool devolucaoParcialSemMotivo = motivoChamado == null && devolucaoParcial && situacaoEntrega == SituacaoEntrega.Entregue;
            TipoMotivoAtendimento tipoMotivoAtendimento = motivoChamado != null ? motivoChamado.TipoMotivoAtendimento : devolucaoParcialSemMotivo ? TipoMotivoAtendimento.Devolucao : TipoMotivoAtendimento.Atendimento;
            if (tipoMotivoAtendimento != TipoMotivoAtendimento.Devolucao && !(devolucaoParcial && situacaoEntrega == SituacaoEntrega.Reentergue) &&
                !(tipoMotivoAtendimento == TipoMotivoAtendimento.ReentregarMesmaCarga && (situacaoEntrega == SituacaoEntrega.NaoEntregue || situacaoEntrega == SituacaoEntrega.Rejeitado)))
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado repCargaEntregaNotaFiscalChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado repConfiguracaoChamado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoChamado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado> cargaEntregaNotaFiscalChamados = repCargaEntregaNotaFiscalChamado.BuscarPorCodigoChamado(chamado?.Codigo ?? 0);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            if (configuracaoChamado == null)
                configuracaoChamado = repConfiguracaoChamado.BuscarConfiguracaoPadrao();
            bool devolucaoPorPeso = cargaEntrega.Carga.TipoOperacao?.DevolucaoProdutosPorPeso ?? false;

            bool possuiNotaParcialInformada = false;
            if (notaFiscais?.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal notaFiscal in notaFiscais)
                {
                    if (devolucaoParcial && (notaFiscal.DevolucaoParcial || notaFiscal.DevolucaoTotal))
                        possuiNotaParcialInformada = true;

                    if (devolucaoParcial && !notaFiscal.DevolucaoParcial && !notaFiscal.DevolucaoTotal && !devolucaoParcialSemMotivo)//Se nenhuma opção marcada quando parcial, não altera a nota
                        continue;

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = cargaEntregaNotasFiscais.Where(o => o.Codigo == notaFiscal.Codigo).FirstOrDefault();
                    if (cargaEntregaNotaFiscal == null)
                        continue;

                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal;

                    if (chamado != null)
                        SalvarHistoricoSituacaoNotaFiscalChamado(chamado, xmlNotaFiscal, _unitOfWork);

                    if (deveAtualizarSituacaoNota)
                    {
                        if (!devolucaoParcial || notaFiscal.DevolucaoTotal)
                        {
                            if (situacaoEntrega == SituacaoEntrega.Reentergue)
                                xmlNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.AgReentrega;
                            else
                                xmlNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.Devolvida;
                        }
                        else if (notaFiscal.DevolucaoParcial)
                        {
                            xmlNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.DevolvidaParcial;

                            if (situacaoEntrega == SituacaoEntrega.Reentergue)
                                xmlNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.AgReentrega;
                        }
                        else if (situacaoEntrega == SituacaoEntrega.Entregue)
                            xmlNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.Entregue;
                        else
                            xmlNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.AgEntrega;

                    }

                    if (xmlNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida || xmlNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial)
                        xmlNotaFiscal.UltimaSituacaoEntregaDevolucao = xmlNotaFiscal.SituacaoEntregaNotaFiscal;

                    repXmlNotaFiscal.Atualizar(xmlNotaFiscal);

                    if (chamado != null)
                    {
                        cargaEntregaNotaFiscal.Initialize();
                        cargaEntregaNotaFiscal.Chamado = chamado;
                        cargaEntregaNotaFiscal.SituacaoEntregaNotaFiscalChamado = xmlNotaFiscal.SituacaoEntregaNotaFiscal;
                        cargaEntregaNotaFiscal.MotivoDaDevolucao = devolucaoParcial ? notaFiscal.MotivoDevolucaoEntrega : chamado.MotivoDaDevolucao;

                        if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe && devolucaoParcial)
                        {
                            if (deveAtualizarSituacaoNota)
                                cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal = SituacaoNotaFiscal.DevolvidaParcial;

                            cargaEntregaNotaFiscal.CargaEntrega.DevolucaoParcial = true;
                        }

                        repCargaEntregaNotaFiscal.Atualizar(cargaEntregaNotaFiscal);

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado cargaEntregaNotaFiscalChamado = cargaEntregaNotaFiscalChamados.Find(cnfc => cnfc.CargaEntregaNotaFiscal.Codigo == cargaEntregaNotaFiscal.Codigo) ?? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado();
                        cargaEntregaNotaFiscalChamado.CargaEntregaNotaFiscal = cargaEntregaNotaFiscal;
                        cargaEntregaNotaFiscalChamado.Chamado = chamado;
                        cargaEntregaNotaFiscalChamado.SituacaoEntregaNotaFiscalChamado = xmlNotaFiscal.SituacaoEntregaNotaFiscal;
                        cargaEntregaNotaFiscalChamado.MotivoDaDevolucao = devolucaoParcial ? notaFiscal.MotivoDevolucaoEntrega : chamado.MotivoDaDevolucao;
                        cargaEntregaNotaFiscalChamado.DevolucaoParcial = notaFiscal.DevolucaoParcial;
                        cargaEntregaNotaFiscalChamado.DevolucaoTotal = notaFiscal.DevolucaoTotal;
                        if (cargaEntregaNotaFiscalChamado.Codigo > 0)
                            repCargaEntregaNotaFiscalChamado.Atualizar(cargaEntregaNotaFiscalChamado);
                        else
                        {
                            repCargaEntregaNotaFiscalChamado.Inserir(cargaEntregaNotaFiscalChamado);
                            cargaEntregaNotaFiscalChamados.Add(cargaEntregaNotaFiscalChamado);
                        }

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = cargaEntregaNotaFiscal.GetChanges();//Para ter histórico caso mudem entre os atendimentos.
                        if (alteracoes.Count > 0 && auditado != null)
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaNotaFiscal, alteracoes, $"Definiu/alterou o atendimento na nota fiscal {xmlNotaFiscal.Numero}", _unitOfWork);

                        if (chamado.XMLNotasFiscais == null)
                            chamado.XMLNotasFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

                        //#23288
                        if (!chamado.XMLNotasFiscais.Contains(xmlNotaFiscal))
                        {
                            chamado.XMLNotasFiscais.Add(xmlNotaFiscal);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, $"Vinculou a nota {xmlNotaFiscal.Numero} ao chamado (Devolução).", _unitOfWork);
                        }
                    }
                    SalvarDevolucaoCargaEntregaProduto(notaFiscal.Produtos, devolucaoParcial && notaFiscal.DevolucaoParcial, devolucaoPorPeso, configuracaoChamado, notaFiscal.Codigo, null, chamado);
                }

                //#23288
                if (chamado?.XMLNotasFiscais != null && chamado?.XMLNotasFiscais.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nf in chamado.XMLNotasFiscais.ToList())
                    {
                        if (!notaFiscais.Exists(nota => nota.Numero == nf.Numero))
                        {
                            chamado.XMLNotasFiscais.Remove(nf);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, $"Removeu a nota {nf.Numero} do chamado (Devolução).", _unitOfWork);
                        }
                    }
                }

            }

            if (finalizandoAnaliseDevolucao && configuracaoChamado.ObrigatorioInformarNotaNaDevolucaoParcialChamado && situacaoEntrega != SituacaoEntrega.Revertida && devolucaoParcial && !possuiNotaParcialInformada)
                throw new ServicoException("É obrigatório informar qual nota está sendo devolvida parcialmente!");

            SalvarDevolucaoCargaEntregaProduto(produtos, devolucaoParcial, devolucaoPorPeso, configuracaoChamado, 0, null, chamado);

            if (finalizandoAnaliseDevolucao &&
               ((cargaEntrega.MotivoRejeicao != null && cargaEntrega.MotivoRejeicao.GerarPedidoDevolucaoAutomaticamente) || (chamado.MotivoChamado.TipoOcorrencia?.GerarPedidoDevolucaoAutomaticamente ?? false)) &&
               (situacaoEntrega == SituacaoEntrega.Rejeitado || situacaoEntrega == SituacaoEntrega.Reentergue || possuiNotaParcialInformada))
                Servicos.Embarcador.Pedido.Pedido.RegistrarPedidoDevolucao(cargaEntrega, DateTime.Now, cargaEntrega.MotivoRejeicao ?? chamado.MotivoChamado.TipoOcorrencia, notaFiscais.Select(nf => nf.Codigo).ToList(), configuracao, _unitOfWork);
        }

        public void EnviarNFesParaAnaliseNaoConformidade(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<string> chavesNFe, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (cargaEntrega == null)
                throw new ServicoException("Entrega não encontrada");

            if ((chavesNFe?.Count ?? 0) == 0)
                throw new ServicoException("Nenhuma chave de nfe enviada");

            foreach (string chave in chavesNFe)
            {
                if (!Utilidades.Validate.ValidarChaveNFe(chave))
                    throw new ServicoException($"Chave {chave} informada é inválida!");
            }

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();

            AdicionarChavesNFeNaCargaEntrega(chavesNFe, cargaEntrega, tipoServicoMultisoftware, configuracao, auditado, _unitOfWork);
        }

        public void SalvarProdutosNotasFiscaisAnaliseDevolucao(List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos, bool devolucaoParcial, bool devolucaoPorPeso, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, int codigoNotaFiscal, Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null)
        {
            SalvarDevolucaoCargaEntregaProduto(produtos, devolucaoParcial, devolucaoPorPeso, configuracaoChamado, codigoNotaFiscal, auditado, chamado);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> ObterAlertasDasCargas(List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta)
        {
            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
            Repositorio.Embarcador.Logistica.AlertaTratativa repAlertaTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativa(unitOfWork);

            List<int> codigoCargas = (from carga in cargas select carga.Codigo).ToList();

            if (tiposAlerta == null || tiposAlerta.Count == 0)
                tiposAlerta = ObterTiposAlertaControleEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = new List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa> tratativas = new List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativa>();

            if (tiposAlerta.Count > 0)
            {
                alertas = repAlertaMonitor.BuscarAlertasPorCargaETipoDeAlerta(codigoCargas, tiposAlerta);

                tratativas = repAlertaTratativa.BuscarPorCargasETiposAlertas(codigoCargas, tiposAlerta);
            }

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta> listaAlertas = (from alerta in alertas
                                                                                                select new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntregaAlerta
                                                                                                {
                                                                                                    CodigoAlerta = alerta.Codigo,
                                                                                                    Carga = alerta.Carga.Codigo,
                                                                                                    TipoAlerta = alerta.TipoAlerta,
                                                                                                    Descricao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(alerta.TipoAlerta),
                                                                                                    Imagem = ObterImagemAlerta(alerta.TipoAlerta),
                                                                                                    Data = alerta.Data.ToString("dd/MM/yyyy HH:mm"),
                                                                                                    DataFim = alerta?.DataFim != null ? alerta.DataFim?.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                                                                                    Latitude = alerta?.Latitude ?? 0,
                                                                                                    Longitude = alerta?.Longitude ?? 0,
                                                                                                    CodigoEntrega = alerta?.CargaEntrega?.Codigo ?? 0,
                                                                                                    Observacao = (from tratativa in tratativas where tratativa.AlertaMonitor.Codigo == alerta.Codigo select tratativa.Observacao)?.FirstOrDefault() ?? alerta?.Observacao ?? "",
                                                                                                    Tratativa = (from tratativa in tratativas where tratativa.AlertaMonitor.Codigo == alerta.Codigo && tratativa.AlertaTratativaAcao != null select tratativa.AlertaTratativaAcao.Codigo)?.FirstOrDefault() ?? 0,
                                                                                                    ObservacaoMotorista = alerta?.Observacao ?? "",
                                                                                                    ValorAlerta = alerta.AlertaDescricao,
                                                                                                    ImagemStatus = alerta.Status.ObterImagemStatus(),
                                                                                                    Status = alerta.Status,
                                                                                                    ExibirNoControleEntrega = alerta.MonitoramentoEvento?.ExibirControleEntrega ?? false
                                                                                                }).ToList();

            return listaAlertas;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosDaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosRetorno = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            // Pega todos os canhotos da cargaEntrega
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> todosCanhotos = repCanhoto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            //caso a entrega nao tenha notas vamos buscar pela carga;
            if (cargaEntregaNotasFiscais == null || cargaEntregaNotasFiscais.Count() <= 0)
                cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaFetchSimples(cargaEntrega.Carga.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotoFiltrados = (
                    from obj in todosCanhotos
                    where (
                        (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) ||
                        (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo))
                    )
                    select obj
                ).ToList();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotoFiltrados)
                {
                    if (canhoto != null && !canhotosRetorno.Contains(canhoto))
                        canhotosRetorno.Add(canhoto);
                }
            }

            return canhotosRetorno;
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosDaCargaEntregas(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosRetorno = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            // Pega todos os canhotos das cargasEntregas

            List<int> codigosCargaEntregaCarga = cargasEntrega.Select(c => c.Carga.Codigo).ToList();
            List<int> codigosCargaEntrega = cargasEntrega.Select(c => c.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> todosCanhotos = repCanhoto.BuscarPorCargasAsync(codigosCargaEntregaCarga).Result;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntregas(codigosCargaEntrega);

            //caso a entrega nao tenha notas vamos buscar pela carga;
            if (cargaEntregaNotasFiscais == null || cargaEntregaNotasFiscais.Count() <= 0)
                cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargasFetchSimplesAsync(codigosCargaEntregaCarga).Result;

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotoFiltrados = (
                    from obj in todosCanhotos
                    where (
                        (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) ||
                        (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo))
                    )
                    select obj
                ).ToList();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotoFiltrados)
                {
                    if (canhoto != null && !canhotosRetorno.Contains(canhoto))
                        canhotosRetorno.Add(canhoto);
                }
            }

            return canhotosRetorno;
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal>> ConverterDevolucaoNotasFiscais(dynamic itensDevolver)
        {
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(_unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotaFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal>();

            foreach (dynamic notaFiscal in itensDevolver.NotasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal()
                {
                    Codigo = ((string)notaFiscal.Codigo).ToInt(),
                    Numero = ((string)notaFiscal.Numero).ToInt(),
                    DevolucaoParcial = ((string)notaFiscal.DevolucaoParcial).ToBool(),
                    DevolucaoTotal = ((string)notaFiscal.DevolucaoTotal).ToBool(),
                    MotivoDevolucaoEntrega = await repositorioMotivoDevolucaoEntrega.BuscarPorCodigoAsync(((string)notaFiscal.MotivoDevolucaoEntrega).ToInt(), false),
                };

                List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> cargaEntregaNotaFiscalProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>();

                foreach (dynamic produto in notaFiscal.Produtos)
                {
                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto cargaEntregaNotaFiscalProduto = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                    {
                        Protocolo = ((string)produto.Codigo).ToInt(),
                        QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal(),
                        Lote = ((string)produto.Lote),
                        DataCritica = ((string)produto.DataCritica).ToNullableDateTime(),
                        ValorDevolucao = ((string)produto.ValorDevolucao).ToDecimal(),
                        NFDevolucao = ((string)produto.NFDevolucao).ToInt(),
                        CodigoMotivoDaDevolucao = ((string)produto.CodigoMotivoDaDevolucao).ToInt(),
                    };

                    cargaEntregaNotaFiscalProdutos.Add(cargaEntregaNotaFiscalProduto);
                }

                if (cargaEntregaNotaFiscalProdutos.Count > 0)
                {
                    cargaEntregaNotaFiscal.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>();
                    cargaEntregaNotaFiscal.Produtos.AddRange(cargaEntregaNotaFiscalProdutos);
                }

                cargaEntregaNotaFiscais.Add(cargaEntregaNotaFiscal);
            }

            return cargaEntregaNotaFiscais;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> ConverterDevolucaoProdutos(dynamic itensDevolver)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> cargaEntregaNotaFiscalProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>();

            foreach (dynamic produto in itensDevolver.Produtos)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto cargaEntregaNotaFiscalProduto = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                {
                    Protocolo = ((string)produto.Codigo).ToInt(),
                    QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal(),
                    ValorDevolucao = ((string)produto.ValorDevolucao).ToDecimal(),
                    NFDevolucao = ((string)produto.NFDevolucao).ToInt(),
                    CodigoMotivoDaDevolucao = ((string)produto.CodigoMotivoDaDevolucao).ToInt(),
                };

                cargaEntregaNotaFiscalProdutos.Add(cargaEntregaNotaFiscalProduto);
            }

            return cargaEntregaNotaFiscalProdutos;
        }

        public dynamic ObterNotasFiscais(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados, bool modalRejeicaoControleEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado repCargaEntregaNotaFiscalChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao repNotaItemDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado> cargaEntregaNotaFiscalChamados = repCargaEntregaNotaFiscalChamado.BuscarPorCodigoChamado(chamado?.Codigo ?? 0);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();


            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCarga(cargaEntrega.Carga.Codigo);
            else
                cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntregaComLimiteDeRegistros(cargaEntrega.Codigo);

            List<dynamic> notasFiscaisRetornar = new List<dynamic>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {
                //#23823 - Retorna apenas notas vinculadas ao Atendimento, exceto quando a ação vem da rejeição da modal de controle de entrega.
                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !modalRejeicaoControleEntrega && chamado != null && chamado.XMLNotasFiscais?.Count > 0 && !chamado.XMLNotasFiscais.Any(notaDoChamado => notaDoChamado.Numero == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero))
                    continue;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado cargaEntregaNotaFiscalChamado = cargaEntregaNotaFiscalChamados.Find(cnfc => cnfc.CargaEntregaNotaFiscal.Codigo == cargaEntregaNotaFiscal.Codigo);

                List<dynamic> notaFiscalProdutosRetornar = new List<dynamic>();
                List<dynamic> notaFiscalDevolucaoRetornar = new List<dynamic>();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaNotaFiscalProdutos = (
                    from produto in cargaEntregaProdutos
                    where produto.XMLNotaFiscal != null && produto.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo
                    select produto
                ).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaNotaFiscalProduto in cargaEntregaNotaFiscalProdutos)
                {
                    cargaEntregaProdutos.Remove(cargaEntregaNotaFiscalProduto);
                    notaFiscalProdutosRetornar.Add(ObterProduto(cargaEntregaNotaFiscalProduto, configuracaoEmbarcador, configuracaoChamado, unitOfWork, cargaEntregaProdutoChamados.Where(p => p.XMLNotaFiscal != null && p.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo).ToList()));
                }

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao> itensDevolucao = repNotaItemDevolucao.BuscarPorCargaEntregaNotaFiscal(cargaEntregaNotaFiscal.Codigo);

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao itemDevolucao in itensDevolucao)
                    notaFiscalDevolucaoRetornar.Add(ObterDevolucao(itemDevolucao));

                SituacaoNotaFiscal situacaoNotaFiscal = cargaEntregaNotaFiscal.SituacaoEntregaNotaFiscalChamado ?? cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal;
                bool devolucaoParcial = false;

                devolucaoParcial = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS
                                        ? cargaEntregaNotaFiscal?.SituacaoEntregaNotaFiscalChamado == SituacaoNotaFiscal.DevolvidaParcial
                                        : cargaEntregaNotaFiscalChamado?.DevolucaoParcial ?? situacaoNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial;


                notasFiscaisRetornar.Add(new
                {
                    Dados = new
                    {
                        cargaEntregaNotaFiscal.Codigo,
                        cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero,
                        Serie = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Serie,
                        DataEmissao = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.DataEmissao.ToDateTimeString(),
                        Volume = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Volumes.ToString(),
                        Valor = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Valor.ToString(),
                        Peso = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Peso.ToString(),
                        SituacaoNotaFiscal = situacaoNotaFiscal,
                        Filial = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Emitente.Descricao,
                        Chave = cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Chave,
                        DevolucaoParcial = devolucaoParcial,
                        DevolucaoTotal = cargaEntregaNotaFiscalChamado?.DevolucaoTotal ?? situacaoNotaFiscal == SituacaoNotaFiscal.Devolvida || situacaoNotaFiscal == SituacaoNotaFiscal.AgReentrega,
                        MotivoDaDevolucao = new
                        {
                            Codigo = cargaEntregaNotaFiscalChamado?.MotivoDaDevolucao?.Codigo ?? 0,
                            Descricao = cargaEntregaNotaFiscalChamado?.MotivoDaDevolucao?.Descricao ?? string.Empty
                        }
                    },
                    Produtos = notaFiscalProdutosRetornar,
                    ItensDevolucao = notaFiscalDevolucaoRetornar
                });
            }

            return notasFiscaisRetornar;
        }

        public dynamic ObterProdutos(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados)
        {
            return (
                from produto in cargaEntregaProdutos
                select ObterProduto(produto, configuracaoEmbarcador, configuracaoChamado, unitOfWork, cargaEntregaProdutoChamados)
            ).ToList();
        }

        public dynamic ObterItensDevolucao(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao> itensDevolucao)
        {
            if (itensDevolucao == null || itensDevolucao.Count == 0)
                return new List<dynamic>();
            return (
                from itemDevolucao in itensDevolucao
                select ObterDevolucao(itemDevolucao)
            ).ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static bool ValidarGeracaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return !(carga.TipoOperacao?.NaoGerarControleColetaEntrega ?? false) && (carga.SituacaoCarga.IsSituacaoCargaEmitida() || !(carga.TipoOperacao?.GerarControleColetaEntregaAposEmissaoDocumentos ?? false));
        }

        private void SalvarDevolucaoCargaEntregaProduto(List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos, bool devolucaoParcial, bool devolucaoPorPeso, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, int codigoNotaFiscal = 0, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null)
        {
            if (produtos == null || produtos.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repositorioCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repositorioCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = codigoNotaFiscal > 0 ? repositorioCargaEntregaProduto.BuscarPorCodigos(produtos.Select(o => o.Protocolo), codigoNotaFiscal) : repositorioCargaEntregaProduto.BuscarPorCodigos(produtos.Select(o => o.Protocolo).ToList(), false);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados = repositorioCargaEntregaProdutoChamado.BuscarPorChamado(chamado?.Codigo ?? 0);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = codigoNotaFiscal > 0 ? repositorioCargaEntregaNotaFiscal.BuscarPorCodigo(codigoNotaFiscal) : null;

            foreach (Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto produto in produtos)
            {
                Dominio.Entidades.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega motivoDaDevolucaoPorProduto = repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(produto.CodigoMotivoDaDevolucao);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto = cargaEntregaProdutos.Find(o => o.Codigo == produto.Protocolo);
                if (cargaEntregaProduto == null)
                    continue;

                decimal quantidadeJaDevolvida = cargaEntregaProduto.QuantidadeDevolucao;
                decimal quantidadeDevolucao = produto.QuantidadeDevolucao;
                decimal valorDevolucao = produto.ValorDevolucao;
                int NFDevolucao = produto.NFDevolucao;
                string lote = produto.Lote;
                DateTime? dataCritica = produto.DataCritica;

                if (chamado != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado cargaEntregaProdutoChamado = cargaEntregaProdutoChamados
                        .Find(o =>
                            ((o.XMLNotaFiscal == null && cargaEntregaProduto.XMLNotaFiscal == null) ||
                            (o.XMLNotaFiscal != null && cargaEntregaProduto.XMLNotaFiscal != null && o.XMLNotaFiscal.Codigo == cargaEntregaProduto.XMLNotaFiscal.Codigo)) &&
                            o.Produto.Codigo == cargaEntregaProduto.Produto.Codigo)
                        ?? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado();

                    quantidadeJaDevolvida -= cargaEntregaProdutoChamado.QuantidadeDevolucao;

                    cargaEntregaProdutoChamado.CargaEntrega = cargaEntregaProduto.CargaEntrega;
                    cargaEntregaProdutoChamado.Produto = cargaEntregaProduto.Produto;
                    cargaEntregaProdutoChamado.XMLNotaFiscal = cargaEntregaProduto.XMLNotaFiscal;
                    cargaEntregaProdutoChamado.Chamado = chamado;
                    cargaEntregaProdutoChamado.PesoUnitario = cargaEntregaProduto.PesoUnitario;
                    cargaEntregaProdutoChamado.Quantidade = cargaEntregaProduto.Quantidade;
                    cargaEntregaProdutoChamado.QuantidadePlanejada = cargaEntregaProduto.QuantidadePlanejada;
                    cargaEntregaProdutoChamado.QuantidadeConferencia = cargaEntregaProduto.QuantidadeConferencia;
                    cargaEntregaProdutoChamado.ObservacaoProdutoDevolucao = cargaEntregaProduto.ObservacaoProdutoDevolucao;

                    cargaEntregaProdutoChamado.QuantidadeDevolucao = quantidadeDevolucao;
                    cargaEntregaProdutoChamado.ValorDevolucao = valorDevolucao;
                    cargaEntregaProdutoChamado.NFDevolucao = NFDevolucao;
                    cargaEntregaProdutoChamado.Lote = lote;
                    cargaEntregaProdutoChamado.DataCritica = dataCritica;
                    cargaEntregaProdutoChamado.MotivoDaDevolucao = motivoDaDevolucaoPorProduto;

                    if (cargaEntregaProdutoChamado.Codigo > 0)
                        repositorioCargaEntregaProdutoChamado.Atualizar(cargaEntregaProdutoChamado);
                    else
                        repositorioCargaEntregaProdutoChamado.Inserir(cargaEntregaProdutoChamado);

                    quantidadeDevolucao += quantidadeJaDevolvida;
                }

                if (auditado != null) cargaEntregaProduto.Initialize();

                if (devolucaoParcial)
                {
                    cargaEntregaProduto.QuantidadeDevolucao = quantidadeDevolucao;

                    if (!(configuracaoChamado?.CalcularValorDasDevolucoes ?? false))
                    {
                        cargaEntregaProduto.ValorDevolucao = valorDevolucao;
                        cargaEntregaProduto.NFDevolucao = NFDevolucao;
                    }
                }
                else
                    cargaEntregaProduto.QuantidadeDevolucao = devolucaoPorPeso ? (cargaEntregaProduto.Quantidade * cargaEntregaProduto.PesoUnitario) : cargaEntregaProduto.Quantidade;

                cargaEntregaProduto.Lote = lote;
                cargaEntregaProduto.DataCritica = dataCritica;

                repositorioCargaEntregaProduto.Atualizar(cargaEntregaProduto);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = cargaEntregaProduto.GetChanges();
                if (alteracoes?.Count > 0 && auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntregaNotaFiscal, alteracoes, $"Alterou o produto {cargaEntregaProduto.Produto.Descricao}", _unitOfWork);
            }
        }

        private static DateTime? ObteraDataBaseCargaLoger(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);

            DateTime dataretornar = DateTime.MinValue;

            if (carga.DataInicioViagem.HasValue && carga.DataInicioViagem.Value != DateTime.MinValue)
                dataretornar = carga.DataInicioViagem.Value;
            else if (carga.DataLoger.HasValue && carga.DataLoger.Value != DateTime.MinValue)
                dataretornar = carga.DataLoger.Value;
            else if (carga.DataRealFaturamento.HasValue && carga.DataRealFaturamento.Value != DateTime.MinValue)
                dataretornar = carga.DataRealFaturamento.Value;
            else if (carga.DataAgendamentoCarga.HasValue && carga.DataAgendamentoCarga.Value != DateTime.MinValue)
                dataretornar = carga.DataAgendamentoCarga.Value;
            else if (carga.DataInicioViagemPrevista.HasValue && carga.DataInicioViagemPrevista.Value != DateTime.MinValue)
                dataretornar = carga.DataInicioViagemPrevista.Value;

            dataretornar = servicoPrevisaoControleEntrega.ObterDataConsiderandoFinalSemanaFeriados(dataretornar, aplicarHorarioJornada: true, carga);

            dataretornar = servicoPrevisaoControleEntrega.ObterDataInicialAjustadaParaHorarioUtil(dataretornar);

            return dataretornar;
        }

        private static void GerarCargaRedespachoAreaRedex(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Cliente cliente, Dominio.Entidades.Cliente Expedidor, Dominio.Entidades.Embarcador.Pedidos.ColetaContainer coletaContainer, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacaoPadraoRedespacho = cliente.TipoOperacaoPadraoRedespachoAreaRedex != null ? cliente.TipoOperacaoPadraoRedespachoAreaRedex : carga.TipoOperacao;

            List<Dominio.Entidades.Embarcador.Cargas.Redespacho> redespachogerado = repositorioRedespacho.BuscarAtivasPorCargaOrigem(carga.Codigo);
            if (redespachogerado != null && redespachogerado.Count > 0)
                return;

            Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = new Dominio.Entidades.Embarcador.Cargas.Redespacho()
            {
                DataRedespacho = DateTime.Now,
                Carga = carga,
                NumeroRedespacho = repositorioRedespacho.BuscarProximoCodigo(),
                TipoOperacao = TipoOperacaoPadraoRedespacho,
                Expedidor = Expedidor
            };

            repositorioRedespacho.Inserir(redespacho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga CargaGerada = CargaDistribuidor.GerarCargaProximoTrecho(carga, TipoOperacaoPadraoRedespacho, 0m, true, Expedidor, cargaPedidos, null, configuracaoEmbarcador, false, redespacho, null, tipoServicoMultisoftware, unitOfWork, redespachoContainer: true);

            if (CargaGerada == null)
                return;

            Repositorio.Embarcador.Pedidos.ColetaContainer repColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(unitOfWork);
            Pedido.ColetaContainer servicoColetaContainer = new Pedido.ColetaContainer(unitOfWork);

            Auditoria.Auditoria.Auditar(CriarAuditoria(), CargaGerada, Localization.Resources.Cargas.ControleEntrega.CargaCriadaViaGestaoContainers, unitOfWork);

            if (coletaContainer.Container != null)
            {
                //CRIAR UMA RETIRADA CONTAINER PARA ESSA CARGA (deve servir apenas para informação na etapa 1 da carga)..
                Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(CargaGerada.Codigo);

                if (retiradaContainer == null)
                    retiradaContainer = new Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer()
                    {
                        Carga = CargaGerada
                    };

                retiradaContainer.Container = coletaContainer.Container;
                retiradaContainer.ContainerTipo = coletaContainer.Container.ContainerTipo;
                retiradaContainer.Local = Expedidor;
                retiradaContainer.ColetaContainer = coletaContainer;

                if (retiradaContainer.Codigo == 0)
                    repositorioRetiradaContainer.Inserir(retiradaContainer);
                else
                    repositorioRetiradaContainer.Atualizar(retiradaContainer);
            }

            redespacho.CargaGerada = CargaGerada;
            coletaContainer.CargaAtual = CargaGerada;

            repositorioRedespacho.Atualizar(redespacho);
            repColetaContainer.Atualizar(coletaContainer);
        }

        private static void GerarAlertaNotificacaoTendencia(AcompanhamentoEntregaTempoConfiguracao configuracaoTempoTendencias, TendenciaEntrega tendencia, IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> CargasEntregasGerarAlerta, bool coleta, DataBaseAlerta? dataBaseAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.AlertaCarga.CargaAlertaTendenciaEntregaColeta AlertaTendenciaEntregaColeta = new Servicos.Embarcador.Carga.AlertaCarga.CargaAlertaTendenciaEntregaColeta(unitOfWork, unitOfWork.StringConexao);

            switch (tendencia)
            {
                case TendenciaEntrega.Adiantado:

                    GerarEventoAlertaMonitoramento(CargasEntregasGerarAlerta, 0, coleta, tendencia, TipoAlerta.AlertaTendenciaEntregaAdiantada, dataBaseAlerta, unitOfWork);
                    TratativaAutomaticaPorTendencia(TratativaAutomaticaMonitoramentoEvento.TendenciaDeEntregaAdiantada, CargasEntregasGerarAlerta, unitOfWork);
                    break;

                case TendenciaEntrega.Atrasado:

                    GerarEventoAlertaMonitoramento(CargasEntregasGerarAlerta, 0, coleta, tendencia, TipoAlerta.AlertaTendenciaEntregaAtrasada, dataBaseAlerta, unitOfWork);
                    break;
                default:
                    break;
            }
        }

        private static void TratativaAutomaticaPorTendencia(TratativaAutomaticaMonitoramentoEvento tratativaMonitoramento, IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> CargasEntregasGerarAlerta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<int> codigosCarga = CargasEntregasGerarAlerta.Select(c => c.CodCarga).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarTodasPorCodigo(codigosCarga);


            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                TratarEventoAutomaticamente(carga, tratativaMonitoramento, unitOfWork);
            }
        }


        private static void GerarEventoAlertaMonitoramento(IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega> CargasEntregasGerarAlerta, int codigoVeiculo, bool coleta, TendenciaEntrega tendenciaEntrega, TipoAlerta tipoAlerta, DataBaseAlerta? dataBaseAlerta, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento = repMonitoramentoEvento.BuscarAtivo(tipoAlerta);

            if (evento == null || evento?.Gatilho == null)
                return;

            if (evento.Gatilho.DataBase is not MonitoramentoEventoData.DataAgendamentodeEntrega and not MonitoramentoEventoData.PrevisaoEntrega)
                return;

            foreach (Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RetornoConsultaTendenciasEntrega cargaAlertar in CargasEntregasGerarAlerta)
            {
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarUltimoPorCarga(cargaAlertar.CodCarga);

                if (ValidarGeracaoAlertaMonitoramento(evento, monitoramento, unitOfWork))
                {

                    string dataPrevistaOuAgendamento = dataBaseAlerta == DataBaseAlerta.PrevisaoEntrega ? $" Data prevista: {cargaAlertar.DataEntregaPrevista.ToString("dd/MM/yyyy HH:mm:ss")}" : $" Data Agendamento: {cargaAlertar.DataAgendamentoEntrega.ToString("dd/MM/yyyy HH:mm:ss")}";

                    string texto = $"Carga " + cargaAlertar.CodCargaEmbarcador + (coleta ? " Coleta: " + cargaAlertar.CPF_CNPJ : " Entrega: " + cargaAlertar.CPF_CNPJ) + " Tendência: " + Dominio.ObjetosDeValor.Embarcador.Enumeradores.TendenciaEntregaHelper.ObterDescricao(tendenciaEntrega) + dataPrevistaOuAgendamento + " Data reprogramada: " + cargaAlertar.DataEntregaReprogramada.ToString("dd/MM/yyyy HH:mm:ss");

                    // Cria o alerta para a carga se não existir algum
                    GerarEvento(tipoAlerta, texto, cargaAlertar.CodCarga, codigoVeiculo, cargaAlertar.CodCargaEntrega, DateTime.Now, unitOfWork);
                }
            }

        }

        private static bool ValidarGeracaoAlertaMonitoramento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            bool deveProcessar = true;

            switch (evento.QuandoProcessar)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoReceberPosicao:
                    deveProcessar = true;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoCriarMonitoramento:
                    deveProcessar = monitoramento != null;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarMonitoramento:
                    deveProcessar = monitoramento != null && monitoramento.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento.AoIniciarViagem:
                    deveProcessar = monitoramento != null &&
                                    monitoramento.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Aguardando &&
                                    monitoramento.Carga != null &&
                                    monitoramento.Carga.DataInicioViagem != null;
                    break;
            }

            if (!deveProcessar) return false;

            // Verifica o status de viagem do monitoramento
            if (!VerificaStatusViagemMonitoramento(evento, monitoramento, unitOfWork)) return false;

            // Verifica o status de viagem do monitoramento
            if (!VerificaTipoDeCargaMonitoramento(evento, monitoramento)) return false;

            // Verifica o status de viagem do monitoramento
            if (!VerificaTipoDeOperacaoMonitoramento(evento, monitoramento)) return false;


            return true;
        }

        private static bool VerificaStatusViagemMonitoramento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            if (
                monitoramento != null &&
                evento.VerificarStatusViagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.Todos &&
                evento.VerificarStatusViagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.NaoVerificar
            )
            {

                var monitoramentoEventoStatusViagem = (
                    from statusViagem in evento.StatusViagem
                    where statusViagem.MonitoramentoEvento.Ativo
                    select new { statusViagem.MonitoramentoStatusViagem }
                ).ToList();
                int total = monitoramentoEventoStatusViagem.Count;
                if (total > 0)
                {

                    if (evento.VerificarStatusViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.EstarComStatusViagem ||
                        evento.VerificarStatusViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.NaoEstarComStatusViagem
                    )
                    {
                        switch (evento.VerificarStatusViagem)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.EstarComStatusViagem:
                                if (monitoramento.StatusViagem != null)
                                {
                                    for (int i = 0; i < total; i++)
                                    {
                                        if (monitoramento.StatusViagem.TipoRegra == monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra)
                                        {
                                            return true;
                                        }
                                    }
                                }
                                break;

                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.NaoEstarComStatusViagem:
                                bool naoEstaComStatusViagem = true;
                                if (monitoramento.StatusViagem != null)
                                {
                                    for (int i = 0; i < total; i++)
                                    {
                                        if (monitoramento.StatusViagem.TipoRegra == monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra)
                                        {
                                            naoEstaComStatusViagem = false;
                                            break;
                                        }
                                    }
                                }
                                return naoEstaComStatusViagem;
                        }
                    }
                    else
                    {
                        if (monitoramento.StatusViagem != null)
                        {
                            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicosStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramento);
                            int totalHistorico = historicosStatusViagem.Count;

                            switch (evento.VerificarStatusViagem)
                            {
                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.HaverPeloMenosUmStatusViagem:
                                    for (int i = 0; i < total; i++)
                                    {
                                        for (int j = 0; j < totalHistorico; j++)
                                        {
                                            if (monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra == historicosStatusViagem[j].StatusViagem.TipoRegra)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    break;

                                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem.HaverTodosStatusViagem:
                                    for (int i = 0; i < total; i++)
                                    {
                                        bool haStatusViagem = false;
                                        for (int j = 0; j < totalHistorico; j++)
                                        {
                                            if (monitoramentoEventoStatusViagem[i].MonitoramentoStatusViagem.TipoRegra == historicosStatusViagem[j].StatusViagem.TipoRegra)
                                            {
                                                haStatusViagem = true;
                                                break;
                                            }
                                        }
                                        if (!haStatusViagem) return false;
                                    }
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }

            return true;
        }

        private static bool VerificaTipoDeCargaMonitoramento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (
                monitoramento != null &&
                monitoramento.Carga != null &&
                monitoramento.Carga.TipoDeCarga != null &&
                evento.VerificarTipoDeCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.Todos &&
                evento.VerificarTipoDeCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.NaoVerificar
            )
            {

                var monitoramentoEventoTipoDeCarga = (
                    from obj in evento.TipoDeCarga
                    where obj.MonitoramentoEvento.Ativo
                    select new { obj.TipoDeCarga }
                ).ToList();
                int total = monitoramentoEventoTipoDeCarga.Count;
                if (total > 0)
                {
                    switch (evento.VerificarTipoDeCarga)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.Algum:
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoDeCarga.Codigo == monitoramentoEventoTipoDeCarga[i].TipoDeCarga.Codigo)
                                {
                                    return true;
                                }
                            }
                            return false;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga.Nenhum:
                            bool naoEstaComStatusViagem = true;
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoDeCarga.Codigo == monitoramentoEventoTipoDeCarga[i].TipoDeCarga.Codigo)
                                {
                                    naoEstaComStatusViagem = false;
                                    break;
                                }
                            }
                            return naoEstaComStatusViagem;
                    }
                }
            }

            return true;
        }

        private static bool VerificaTipoDeOperacaoMonitoramento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (
                monitoramento != null &&
                monitoramento.Carga != null &&
                monitoramento.Carga.TipoOperacao != null &&
                evento.VerificarTipoDeOperacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.Todos &&
                evento.VerificarTipoDeOperacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.NaoVerificar
            )
            {

                var monitoramentoEventoTipoDeOperacao = (
                    from obj in evento.TipoDeOperacao
                    where obj.MonitoramentoEvento.Ativo
                    select new { obj.TipoDeOperacao }
                ).ToList();
                int total = monitoramentoEventoTipoDeOperacao.Count;
                if (total > 0)
                {
                    switch (evento.VerificarTipoDeOperacao)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.Algum:
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoOperacao.Codigo == monitoramentoEventoTipoDeOperacao[i].TipoDeOperacao.Codigo)
                                {
                                    return true;
                                }
                            }
                            return false;

                        case Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao.Nenhum:
                            bool naoEstaComStatusViagem = true;
                            for (int i = 0; i < total; i++)
                            {
                                if (monitoramento.Carga.TipoOperacao.Codigo == monitoramentoEventoTipoDeOperacao[i].TipoDeOperacao.Codigo)
                                {
                                    naoEstaComStatusViagem = false;
                                    break;
                                }
                            }
                            return naoEstaComStatusViagem;
                    }
                }
            }

            return true;
        }


        private static void GerarEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, string descricaoAlerta, int codigoCarga, int codigoVeiculo, int codigoEntrega, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento = repMonitoramentoEvento.BuscarAtivo(tipoAlerta);

            if (evento != null && evento.Gatilho != null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoEntrega);
                if (cargaEntrega != null && cargaEntrega.Fronteira)
                    return;

                Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista = repCargaMotorista.BuscarPorCarga(cargaEntrega.Carga.Codigo).FirstOrDefault();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>
                {
                    tipoAlerta
                };

                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarUltimoAlertaCargaETipoDeAlerta(codigoCarga, tiposAlerta);

                if (alertas == null || alertas.Count == 0 || (alertas[0].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado && alertas[0].Data.AddMinutes(evento.Gatilho.Tempo) < data))
                {
                    Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor()
                    {
                        TipoAlerta = tipoAlerta,
                        MonitoramentoEvento = evento,
                        Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto,
                        DataCadastro = DateTime.Now,
                        Data = data,
                        Veiculo = cargaEntrega.Carga.Veiculo,
                        Carga = new Dominio.Entidades.Embarcador.Cargas.Carga { Codigo = codigoCarga },
                        CargaEntrega = cargaEntrega,
                        AlertaDescricao = descricaoAlerta.Length > 50 ? descricaoAlerta.Substring(0, 50) : descricaoAlerta,
                        Motorista = cargaMotorista?.Motorista ?? null,
                    };

                    Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                    repAlerta.Inserir(alerta);

                    CriarAlertaAcompanhamentoCarga(alerta, evento, unitOfWork);
                }
            }
        }

        private static void CriarAlertaAcompanhamentoCarga(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento MonitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            if (MonitoramentoEvento.GerarAlertaAcompanhamentoCarga)
            {
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga repAlertaAcompanhamentoCarga = new Repositorio.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga alertaAcompanhamentoCarga = new Dominio.Entidades.Embarcador.TorreControle.AlertasAcompanhamentoCarga
                {
                    Carga = alerta.Carga,
                    AlertaMonitor = alerta,
                    CargaEvento = null,
                    AlertaTratado = false,
                    DataCadastro = DateTime.Now,
                    DataEvento = alerta.Data,
                    CargaEntrega = alerta.CargaEntrega
                };

                repAlertaAcompanhamentoCarga.Inserir(alertaAcompanhamentoCarga);
                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(alerta.Carga);
            }
        }

        private static bool ValidarAtualizaDataEntregaReprogramada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (cargaEntrega != null && carga.TipoOperacao != null)
            {
                if (cargaEntrega.DataEntradaRaio.HasValue && carga.TipoOperacao.NaoAtualizarDataReprogramadaAposEntradaRaio)
                {
                    //nao pode mais atualizar a data entrega reprogramada;
                    return false;
                }
                else
                    return true;
            }
            else
                return true;
        }

        private static void AtualizarCargaPelaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            cargaEntrega.Carga.DataAtualizacaoCarga = DateTime.Now;

            repositorioCarga.Atualizar(cargaEntrega.Carga);
        }

        private static void NotificarAcaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, int codigoClienteMultisoftware, MobileHubs acao, Repositorio.UnitOfWork unitOfWork)
        {
            NotificarAcaoCargaEntrega(cargaEntrega.Codigo, cargaEntrega.Carga.Codigo, codigoClienteMultisoftware, acao, unitOfWork);
        }

        private static void NotificarAcaoCargaEntrega(int codigoCargaEntrega, int codigoCarga, int codigoClienteMultisoftware, MobileHubs acao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            bool possuiNotaCobertura = repCargaEntrega.PossuiNotaCoberturaPorCarga(codigoCarga);

            if (!possuiNotaCobertura)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(codigoCarga);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMotorista cargaMotorista in cargaMotoristas)
            {
                dynamic conteudo = new
                {
                    Carga = codigoCarga,
                    Entrega = codigoCargaEntrega,
                    Acao = acao
                };

                Dominio.MSMQ.Notification notification = new Dominio.MSMQ.Notification(
                    conteudo,
                    codigoClienteMultisoftware,
                    cargaMotorista.Motorista.CodigoMobile,
                    Dominio.MSMQ.MSMQQueue.SGTMobile,
                    Dominio.SignalR.Hubs.Mobile,
                    SignalR.Mobile.GetHub(acao)
                );

                MSMQ.MSMQ.SendPrivateMessage(notification);

                // Mandar notificação para o novo app MTrack através do OneSignal
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                Notificacao.NotificacaoMTrack serNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);
                serNotificacaoMTrack.NotificarMudancaCargaEntrega(cargaEntrega, cargaMotorista.Motorista, acao);
            }
        }

        private static int ObterParametroRaioInicioViagem(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            int raio = 0;
            Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork);

            repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho parametro = repMonitoramentoEvento.BuscarParametorPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMonitoramentoEvento.InicioDeViagem);

            raio = parametro?.Raio ?? 0;

            if (raio == 0)
                raio = configuracaoEmbarcador.RaioPadrao;

            return raio;
        }

        private static void IniciarEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint, DateTime dataInicioEntrega, DateTime? dataEntradaRaio, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemSituacaoEntrega origem, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            if (cargaEntrega == null)
                return;

            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega servicoOcorrenciaEntrega = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = TipoAuditado.Sistema,
                OrigemAuditado = OrigemAuditado.Sistema
            };

            if (!cargaEntrega.Carga.DataInicioViagem.HasValue)
            {
                Dominio.Entidades.Cliente clienteOrigem = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarClienteOrigemDaCargaPeloPedido(unitOfWork, cargaEntrega.Carga);
                if (clienteOrigem != null && clienteOrigem.CPF_CNPJ != cargaEntrega.Cliente?.CPF_CNPJ)
                {
                    DateTime dataInicioViagem = Servicos.Embarcador.Monitoramento.Monitoramento.ObterMenorDataDePosicaoDoMonitoramento(unitOfWork, cargaEntrega.Carga, dataInicioEntrega, configuracaoEmbarcador);

                    if (IniciarViagem(cargaEntrega.Carga.Codigo, dataInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, wayPoint, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, auditado, unitOfWork))
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega.Carga, Localization.Resources.Cargas.ControleEntrega.InicioViagemInformadoAutomaticamenteIniciarEntrega, unitOfWork);
                }
            }

            if (wayPoint != null)
            {
                cargaEntrega.LatitudeFinalizada = Convert.ToDecimal(wayPoint.Latitude);
                cargaEntrega.LongitudeFinalizada = Convert.ToDecimal(wayPoint.Longitude);
            }

            if (!cargaEntrega.DataInicio.HasValue)
                cargaEntrega.DataInicio = dataInicioEntrega;

            if (cargaEntrega.DataFim.HasValue && cargaEntrega.DataFim < dataInicioEntrega)
            {
                cargaEntrega.DataFim = null;
                repositorioPedido.DefinirDataEntregaPorCargaEntrega(cargaEntrega.Codigo, dataEntrega: null);
            }

            cargaEntrega.DistanciaAteDestino = null;
            cargaEntrega.Situacao = SituacaoEntrega.EmCliente;

            //DefinirPrevisaoCargaEntrega(cargaEntrega.Carga, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware, configuracaoControleEntrega);

            //caso a empresa utiliza app trizzy a chegada no alvo se da pelo metodo InformarChegada do WS.
            if (!configuracaoEmbarcador.UtilizaAppTrizy)
            {
                bool existeConfiguracaoUltimaChegada = repositorioConfiguracaoOcorrenciaEntrega.ExisteConfiguracaoOcorrenciaPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.UltimaChegadaNoAlvo);

                if (existeConfiguracaoUltimaChegada)
                {
                    bool ultimaChegada = UltimaEntregaPendente(cargaEntrega.Carga, unitOfWork);
                    EventoColetaEntrega eventoASerExecutado = ultimaChegada ? EventoColetaEntrega.UltimaChegadaNoAlvo : EventoColetaEntrega.ChegadaNoAlvo;
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, dataInicioEntrega, eventoASerExecutado, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, origem, configuracaoControleEntrega, auditado);
                }
                else
                    servicoOcorrenciaEntrega.GerarOcorrenciaEntrega(cargaEntrega, dataInicioEntrega, EventoColetaEntrega.ChegadaNoAlvo, cargaEntrega.LatitudeFinalizada, cargaEntrega.LongitudeFinalizada, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, origem, configuracaoControleEntrega, auditado);
            }

            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
            string mensagemHistorico = cargaEntrega?.Coleta == true ? "Inicio de Coleta" : "Fim de Coleta";

            Pedido.Pedido.GerarPedidoHistorico(cargaEntrega, mensagemHistorico, unitOfWork);
        }

        private static int ObterQuantidadeEntregaRealizada(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            return repCarga.BuscarQuantidadeEntregaRalizada(codigoCarga);
        }

        private static bool FinalizarViagem(int codigoCarga, DateTime data, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string mensagemAuditoria, Dominio.Entidades.Usuario responsavelFinalizacaoManual, Dominio.ObjetosDeValor.Enumerador.OrigemAuditado sistemaOrigem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega OrigemSituacaoEntrega, Repositorio.UnitOfWork unitOfWork, bool finalizandoEntregaTransbordo = false, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.DataTerminoEntrega> listaDataTerminoEntregas = null, DateTime? dataInicioViagem = null)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

            if (carga == null)
                return false;

            if (sistemaOrigem != Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCarga = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>();
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.ProntoTransporte);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                if (carga.TipoFreteEscolhido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Cliente && !situacoesCarga.Contains(carga.SituacaoCarga) && !carga.AgImportacaoCTe && (!carga.TipoOperacao?.ConfiguracaoMobile?.ExibirEntregaAntesEtapaTransporte ?? false))
                    throw new ServicoException($"Não é possível finalizar a viagem pois a carga ainda não está em transporte. Carga: {codigoCarga}");
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador repPreAgrupamentoAgrupador = new Repositorio.Embarcador.Cargas.PreAgrupamentoCargaAgrupador(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
            Repositorio.Embarcador.Pedidos.RetiradaContainer repositorioRetiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repConfiguracaoTipoOperacaoControle = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoOrtec = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ortec);
            Dominio.Entidades.Embarcador.Cargas.PreAgrupamentoCargaAgrupador cargaPreAgrupamentoAgrupador = null;
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repConfiguracaoTipoOperacaoControle.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe &&
                ((configuracaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false) || (configuracaoTipoOperacaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false)) &&
                SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida().Contains(carga.SituacaoCarga))
                throw new ServicoException($"A carga {carga.CodigoCargaEmbarcador} está em uma situação {carga.SituacaoCarga.ObterDescricao()} que não pode ser finalizada pelo transportador, por favor siga com as etapas da carga e tente novamente");

            if (tipoIntegracaoOrtec != null)
                cargaPreAgrupamentoAgrupador = repPreAgrupamentoAgrupador.BuscarPorCodigoCargaESituacao(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreAgrupamentoCarga.Carregado);

            if (cargaPreAgrupamentoAgrupador != null)
            {
                Dominio.Entidades.Embarcador.Configuracoes.Integracao integracao = repIntegracao.Buscar();
                if (integracao != null && integracao.IntegrarEntregaOrtec)
                    //possui integracao ortec e carga ja esta em pre agrupamento; assim, vamos gerar um registro de integracao
                    Servicos.Embarcador.Carga.ControleEntrega.EntregaIntegracao.GerarNovaIntegracaoEntrega(carga, cargaPreAgrupamentoAgrupador.CodigoAgrupamento, tipoIntegracaoOrtec, tipoServicoMultisoftware, unitOfWork);
            }

            if (configuracaoEmbarcador.GerarCargaTrajeto)
                Embarcador.Carga.Carga.AtualizarFimTrajetoCarga(carga, unitOfWork);

            List<SituacaoEntrega> situacoes = new List<SituacaoEntrega>
            {
                SituacaoEntrega.Revertida,
                SituacaoEntrega.NaoEntregue,
                SituacaoEntrega.EmCliente,
            };

            if (dataInicioViagem.HasValue && dataInicioViagem.Value != DateTime.MinValue)
                carga.DataInicioViagem = dataInicioViagem;

            if (OrigemSituacaoEntrega != null)
                carga.OrigemSituacaoFimViagem = OrigemSituacaoEntrega;

            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscaisCarga = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

            bool obrigaCanhoto = carga.TipoOperacao?.ConfiguracaoMobile?.ObrigarFotoCanhoto ?? false;
            if (obrigaCanhoto)
            {
                canhotos = repCanhoto.BuscarPorCarga(carga.Codigo);
                cargaEntregaNotasFiscaisCarga = repCargaEntregaNotaFiscal.BuscarPorCarga(carga.Codigo);
            }

            bool todasEntregasFinalizadas = true;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repCargaEntrega.BuscarPorCargaeSituacao(codigoCarga, situacoes);
            if (cargasEntrega.Count > 0)
                Log.TratarErro($"Cargas entrega pendentes {string.Join(", ", cargasEntrega.Select(o => o.Codigo))} da carga {codigoCarga}", "FinalizarEntrega");

            Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer retiradaContainer = repositorioRetiradaContainer.BuscarPorCarga(codigoCarga);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregasPedido = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            bool existeConfiguracaoFimViagem = repositorioConfiguracaoOcorrenciaEntrega.ExisteConfiguracaoOcorrenciaPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.FimViagem);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
            {
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
                cargaEntrega.FinalizadaManualmente = (responsavelFinalizacaoManual != null);
                cargaEntrega.ResponsavelFinalizacaoManual = responsavelFinalizacaoManual;
                DateTime dataConfirmacaoEntrega = listaDataTerminoEntregas?.Find(entrega => entrega.CodigoEntrega == cargaEntrega.Codigo)?.DataFimEntregaFormatada ?? data;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                {
                    cargaEntrega = cargaEntrega,
                    dataInicioEntrega = !cargaEntrega.DataInicio.HasValue ? data : cargaEntrega.DataInicio.Value,
                    dataTerminoEntrega = data,
                    dataConfirmacao = dataConfirmacaoEntrega,
                    wayPoint = null,
                    wayPointDescarga = null,
                    pedidos = null,
                    motivoRetificacao = 0,
                    justificativaEntregaForaRaio = "",
                    motivoFalhaGTA = 0,
                    configuracaoEmbarcador = configuracaoEmbarcador,
                    tipoServicoMultisoftware = tipoServicoMultisoftware,
                    sistemaOrigem = sistemaOrigem,
                    dadosRecebedor = null,
                    Container = retiradaContainer != null && retiradaContainer.Container != null ? retiradaContainer.Container.Codigo : 0,
                    OrigemSituacaoEntrega = OrigemSituacaoEntrega,
                    OrigemSituacaoFimViagem = OrigemSituacaoEntrega,
                    FinalizandoEntregaTransbordo = false,
                    FinalizandoViagem = true,
                    configuracaoControleEntrega = configuracaoControleEntrega,
                    tipoOperacaoParametro = tipoOperacaoParametro,
                    TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                };

                bool podeFinalizar = true;
                if (obrigaCanhoto && !cargaEntrega.Coleta)
                {
                    Log.TratarErro($"Verifica se pode finalizar {cargaEntrega.Codigo}", "FinalizarEntrega");
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = (from obj in cargaEntregaNotasFiscaisCarga where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj).ToList();
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNota in cargaEntregaNotasFiscais)
                    {
                        Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = (from obj in canhotos where obj.XMLNotaFiscal != null && obj.XMLNotaFiscal.Codigo == cargaEntregaNota.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo select obj).FirstOrDefault();
                        if (canhoto != null && canhoto.SituacaoDigitalizacaoCanhoto != SituacaoDigitalizacaoCanhoto.Digitalizado && canhoto.SituacaoCanhoto != SituacaoCanhoto.RecebidoFisicamente && canhoto.SituacaoCanhoto != SituacaoCanhoto.Justificado)
                        {
                            Log.TratarErro($"Canhoto {canhoto.Numero} pendente, não vai finalizar {cargaEntrega.Codigo}", "FinalizarEntrega");
                            podeFinalizar = false;
                            break;
                        }
                    }
                }

                if (podeFinalizar)
                {
                    Log.TratarErro($"Inicia finalizar {cargaEntrega.Codigo}", "FinalizarEntrega");
                    FinalizarEntrega(parametros, unitOfWork);
                    Log.TratarErro($"Finalizou {cargaEntrega.Codigo}", "FinalizarEntrega");

                    if (auditado != null)
                    {
                        Log.TratarErro($"Audita finalizar {cargaEntrega.Codigo}", "FinalizarEntrega");
                        string mensagemAuditoriaEntrega = responsavelFinalizacaoManual != null ? string.Format(Localization.Resources.Cargas.ControleEntrega.OperadorXConfirmouManualmenteEntregaFinalizarViagem, responsavelFinalizacaoManual.Descricao) : Localization.Resources.Cargas.ControleEntrega.EntregaConfirmadaFinalizarViagem;
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, mensagemAuditoriaEntrega, unitOfWork);
                    }
                }
                else
                    todasEntregasFinalizadas = false;

            }

            if (todasEntregasFinalizadas)
                carga.DataFimViagem = data;

            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            DefinirPrevisaoCargaEntrega(carga, cargaRotaFrete, cargasEntrega, false, false, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware, false, OrigemSituacaoEntrega);

            if (existeConfiguracaoFimViagem)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaEntrega = repCargaEntrega.BuscarUltimaCargaEntregaRealizada(codigoCarga);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = ultimaEntrega.MotivoRejeicao;

                if (ultimaEntrega.DataConfirmacao.HasValue || ultimaEntrega.DataRejeitado.HasValue)
                {
                    DateTime dataUltimaEntrega = ultimaEntrega.DataConfirmacao.HasValue ? ultimaEntrega.DataConfirmacao.Value : ultimaEntrega.DataRejeitado.Value;

                    bool existeOcorrenciaEntrega = repOcorrenciaEntrega.ExisteOcorrenciaPorCargaEntregaETipoOcorrencia(ultimaEntrega.Codigo, dataUltimaEntrega, tipoDeOcorrencia?.Codigo);

                    if (!existeOcorrenciaEntrega)
                        OcorrenciaEntrega.GerarOcorrenciaCarga(carga, dataUltimaEntrega, EventoColetaEntrega.FimViagem, null, null, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, OrigemSituacaoEntrega, configuracaoControleEntrega, unitOfWork, auditado);
                }
            }

            if (!carga.DataInicioViagem.HasValue)
                if (IniciarViagem(carga.Codigo, data, OrigemSituacaoEntrega, null, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, auditado, unitOfWork))
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Viagem iniciada automaticamente durante a finalização, pois não havia registro prévio de início de viagem.", unitOfWork);

            repCarga.Atualizar(carga);

            Log.TratarErro($"Todas entregas finalizadas {todasEntregasFinalizadas} da carga {codigoCarga}", "FinalizarEntrega");

            if (!todasEntregasFinalizadas)
                return false;

            bool gerarRetornoAutomatico = (carga.TipoOperacao?.ConfiguracaoCarga?.GerarRetornoAutomaticoMomento ?? GerarRetornoAutomaticoMomento.Nenhum) == GerarRetornoAutomaticoMomento.ConfirmacaoViagem;
            if (gerarRetornoAutomatico)
            {
                Servicos.Embarcador.Carga.Retornos.RetornoCarga serRetornos = new Servicos.Embarcador.Carga.Retornos.RetornoCarga(unitOfWork);

                serRetornos.GerarCargaRetorno(carga, carga.TipoOperacao?.TipoRetornoCarga?.Codigo ?? 0, 0, carga.Veiculo?.Codigo ?? 0, carga.VeiculosVinculados?.FirstOrDefault()?.Codigo ?? 0, 0, false, null);
            }

            // Encerramento do monitoramento
            Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(carga, data, configuracaoEmbarcador, auditado, ((!string.IsNullOrWhiteSpace(mensagemAuditoria)) ? mensagemAuditoria : "Viagem finalizada"), unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoAoFimDaViagem);

            //Alertas acompanhamento carga;
            Servicos.Embarcador.Carga.AlertaCarga.FimViagem alertaFimViagem = new Servicos.Embarcador.Carga.AlertaCarga.FimViagem(unitOfWork, unitOfWork.StringConexao);
            alertaFimViagem.ProcessarEvento(carga);

            if (!string.IsNullOrWhiteSpace(carga.IDIdentificacaoTrizzy) && (configuracaoEmbarcador?.UtilizaAppTrizy ?? false))
                Servicos.Embarcador.Integracao.Trizy.IntegracaoTrizy.AtualizarViagem(carga, "COMPLETED", unitOfWork, null, true, null, false, null, null, string.Empty, string.Empty, false, string.Empty);

            //Servicos.Embarcador.Pedido.Pedido.GerarPedidoHistorico(carga, "Fim de Viagem", unitOfWork);

            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            //serCarga.GerarRegistroEncerramentoCarga(ref carga, "Pedidos finalizados", null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null, unitOfWork);

            if (!configuracaoEmbarcador.NaoEncerrarViagemAoEncerrarControleEntrega)
            {
                string retorno = serCarga.SolicitarEncerramentoCarga(carga.Codigo, "Fim de viagem, Pedidos finalizados", "", tipoServicoMultisoftware, unitOfWork, null);

                if (!string.IsNullOrEmpty(retorno))
                {
                    Log.TratarErro(retorno);
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, retorno, unitOfWork);
                }
            }

            if ((carga.TipoOperacao?.ConfiguracaoCarga?.GerarCargaEspelhoAoConfirmarEntrega ?? false) && !finalizandoEntregaTransbordo && !repCarga.ExisteCargaEspelhoPorCarga(carga.Codigo))
            {
                Servicos.Log.TratarErro($"1 - Carga: {carga.CodigoCargaEmbarcador} Antiga: {carga.Codigo}", "CargaEspelhoGeracao");
                serCarga.GerarCargaEspelho(carga, unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador, auditado);
            }

            try
            {
                new Servicos.Embarcador.GestaoPatio.EmailCheckList(unitOfWork, auditado).EnviarEmailCheckListSetoresTipoOperacao(carga.Codigo, carga.TipoOperacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha EmailCheckList " + ex, "OcorrenciaEstadia");
            }

            //VALIAR TIPO OPERACAO E REDESPACHO
            if (carga.TipoOperacao?.ConfiguracaoMontagemCarga?.DisponibilizarPedidosMontagemAoFinalizarTransporte ?? false)
                serCarga.MostrarPedidosNaMontagemCarga(carga, (from obj in cargaPedidos select obj.Pedido).ToList(), unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Transbordo transbordoCarga = repTransbordo.BuscarPorCargaGerada(carga.Codigo);
            if (transbordoCarga != null && transbordoCarga.Carga != null && transbordoCarga.Carga.SituacaoCarga != SituacaoCarga.Cancelada && transbordoCarga.Carga.SituacaoCarga != SituacaoCarga.Anulada)
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(transbordoCarga.Carga.Codigo, data, auditado, tipoServicoMultisoftware, clienteMultisoftware, OrigemSituacaoEntrega, unitOfWork, true);


            if (carga.DataFimViagem != null || carga.DataFimViagem != DateTime.MinValue)
                AvancarEtapaGestaoDevolucao(carga, auditado, clienteMultisoftware, unitOfWork);

            ValidarIntegracaoCanhotoFimViagem(carga, unitOfWork);

            return true;
        }

        private static void RealizouTodasColetas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Servicos.Embarcador.Hubs.Carga servicoHubCarga, Servicos.Embarcador.Carga.Carga servicoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Log.TratarErro($"Entrou no RealizouTodasColetas da carga {carga.Codigo}", "FinalizarEntrega");

            if (NaoPossuiColetasPendentes(carga, unitOfWork))
            {
                Log.TratarErro($"Nenhuma coleta pendente no RealizouTodasColetas da carga {carga.Codigo}", "FinalizarEntrega");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                if (!configuracaoEmbarcador.NaoEncerrarViagemAoEncerrarControleEntrega && carga.TipoOperacao != null && carga.TipoOperacao.FretePorContadoCliente && carga.TipoOperacao.GerarControleColeta
                    && (carga.SituacaoCarga == SituacaoCarga.Nova || carga.SituacaoCarga == SituacaoCarga.AgNFe || carga.SituacaoCarga == SituacaoCarga.CalculoFrete))
                {
                    //todo: regra fixa para coleta da aurora pensar em algo se necessário.
                    carga.SituacaoCarga = SituacaoCarga.AgIntegracao;
                    carga = servicoCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
                    carga.GerandoIntegracoes = true;
                    repCarga.Atualizar(carga);
                    Log.TratarErro($"Executou atualização no RealizouTodasColetas da carga {carga.Codigo}", "FinalizarEntrega");
                }
            }

            Log.TratarErro($"Saiu do RealizouTodasColetas da carga {carga.Codigo}", "FinalizarEntrega");
        }

        private static void AdicionarHandlingUnitIds(List<string> handlingUnitIds, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            if (!repCargaIntegracao.ExistePorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MercadoLivre))
                return;

            Servicos.Embarcador.Integracao.MercadoLivre.IntegracaoMercadoLivre svcIntegracaoMercadoLivre = new Integracao.MercadoLivre.IntegracaoMercadoLivre(unitOfWork);

            svcIntegracaoMercadoLivre.AdicionarHandlingUnitParaConsulta(handlingUnitIds, 0, null, carga, unitOfWork);
        }

        private static Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor buildDadosRecebedor(Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.DadosRecebedor dadosRecebedor, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor repDadosRecebedor = new Repositorio.Embarcador.Cargas.ControleEntrega.DadosRecebedor(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor dadosRecebedorEntity = cargaEntrega.DadosRecebedor ?? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor();

            DateTime.TryParseExact(dadosRecebedor.DataEntrega, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime DataEntrega);

            if (DataEntrega == DateTime.MinValue)
            {
                DataEntrega = DateTime.Now;
            }

            dadosRecebedorEntity.Nome = dadosRecebedor.Nome ?? "";
            dadosRecebedorEntity.CPF = dadosRecebedor.CPF?.Replace(".", "").Replace("-", "") ?? "";
            dadosRecebedorEntity.DataEntrega = DataEntrega;
            dadosRecebedorEntity.PercentualCompatibilidadeFoto = dadosRecebedor.PercentualCompatibilidadeFoto;

            if (!string.IsNullOrEmpty(dadosRecebedor.GuidFoto))
            {
                dadosRecebedorEntity.GuidFoto = dadosRecebedor.GuidFoto;
            }

            if (cargaEntrega.DadosRecebedor != null)
            {
                repDadosRecebedor.Atualizar(dadosRecebedorEntity);
            }
            else
            {
                repDadosRecebedor.Inserir(dadosRecebedorEntity);
            }

            return dadosRecebedorEntity;
        }

        private static string ConverterCoordenada(string coordenada)
        {
            if (string.IsNullOrWhiteSpace(coordenada))
                return string.Empty;
            return Utilidades.String.RemoveSpecialCharactersLatitudeLongitude(coordenada.Replace(",", ""));
        }

        private static void RemoverProdutosColetados(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            bool atualizarSumarizados = false;
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido in cargaEntregaPedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(cargaEntregaPedido.CargaPedido.Codigo);

                if (cargaPedidoProdutos.Any(obj => obj.Produto.PossuiIntegracaoColetaMobile))
                {
                    repCargaPedidoProdutoDivisaoCapacidade.ExcluirTodosPorCargaPedido(cargaEntregaPedido.CargaPedido.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto in cargaPedidoProdutos)
                    {
                        if (cargaPedidoProduto.Quantidade > 0 || cargaPedidoProduto.Temperatura != 0)
                        {
                            cargaEntrega.Carga.DadosSumarizados.PesoTotal -= cargaPedidoProduto.Quantidade;
                            atualizarSumarizados = true;
                            cargaPedidoProduto.Quantidade = 0;
                            cargaPedidoProduto.Temperatura = 0;
                            repCargaPedidoProduto.Atualizar(cargaPedidoProduto);
                        }
                    }
                }
            }
            if (atualizarSumarizados)
                repCargaDadosSumarizados.Atualizar(cargaEntrega.Carga.DadosSumarizados);
        }

        private static void ExcluirCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            repCarga.DeletarEntregas(carga.Codigo);
        }

        private static void InformarProdutosColetados(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidos == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaDadosSumarizados repCargaDadosSumarizados = new Repositorio.Embarcador.Cargas.CargaDadosSumarizados(unitOfWork);
            bool atualizarSumarizados = false;

            foreach (Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido pedido in pedidos)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCargaPedido(pedido.Codigo);

                if (pedido.Produtos == null || !cargaPedidoProdutos.Any(obj => obj.Produto.PossuiIntegracaoColetaMobile))
                    continue;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> listaDivisoesCapacidade = repCargaPedidoProdutoDivisaoCapacidade.BuscarPorCargasPedidoProduto(cargaPedidoProdutos.Select(obj => obj.Codigo).ToList());

                foreach (Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto produto in pedido.Produtos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto cargaPedidoProduto = (from obj in cargaPedidoProdutos where obj.Produto.CodigoProdutoEmbarcador == produto.Codigo select obj).FirstOrDefault();

                    if (cargaPedidoProduto == null)
                        continue;

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> divisoesPedidoProduto = (from o in listaDivisoesCapacidade where o.CargaPedidoProduto.Codigo == cargaPedidoProduto.Codigo select o).ToList();

                    List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> valoresAlterados = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();
                    cargaPedidoProduto.Initialize();

                    try
                    {
                        cargaPedidoProduto.Quantidade = Convert.ToDecimal(produto.Quantidade);
                    }
                    catch (FormatException)
                    {
                        cargaPedidoProduto.Quantidade = 0;
                    }

                    cargaPedidoProduto.QuantidadePorCaixaRealizada = produto.QuantidadePorCaixaRealizada;
                    cargaPedidoProduto.QuantidadeCaixasVaziasRealizada = produto.QuantidadeCaixasVaziasRealizada;
                    cargaPedidoProduto.ImunoRealizado = produto.ImunoRealizado;
                    cargaPedidoProduto.Temperatura = produto.Temperatura;
                    cargaPedidoProduto.JustificativaTemperatura = null;

                    if (produto.motivoTemperatura > 0)
                        cargaPedidoProduto.JustificativaTemperatura = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura() { Codigo = produto.motivoTemperatura };

                    repCargaPedidoProduto.Atualizar(cargaPedidoProduto);

                    Auditoria.Auditoria.Auditar(auditado, cargaPedidoProduto, cargaPedidoProduto.GetChanges(), Localization.Resources.Cargas.ControleEntrega.SalvouProduto, unitOfWork);

                    cargaEntrega.Carga.DadosSumarizados.PesoTotal += cargaPedidoProduto.Quantidade;
                    atualizarSumarizados = true;

                    if (produto.ProdutoDivisoesCapacidade != null)
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ProdutoDivisaoCapacidade produtoDivisaoCapacidade in produto.ProdutoDivisoesCapacidade)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade cargaPedidoProdutoDivisaoCapacidade = (from obj in divisoesPedidoProduto where obj.ModeloVeicularCargaDivisaoCapacidade.Codigo == produtoDivisaoCapacidade.DivisaoCapacidadeModeloVeicular.Codigo select obj).FirstOrDefault();

                            if (cargaPedidoProdutoDivisaoCapacidade == null)
                                cargaPedidoProdutoDivisaoCapacidade = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade();
                            else
                                divisoesPedidoProduto.Remove(cargaPedidoProdutoDivisaoCapacidade);

                            cargaPedidoProdutoDivisaoCapacidade.Initialize();

                            cargaPedidoProdutoDivisaoCapacidade.CargaPedidoProduto = cargaPedidoProduto;
                            cargaPedidoProdutoDivisaoCapacidade.Quantidade = produtoDivisaoCapacidade.Quantidade;
                            cargaPedidoProdutoDivisaoCapacidade.QuantidadePlanejada = produtoDivisaoCapacidade.QuantidadePlanejada;
                            cargaPedidoProdutoDivisaoCapacidade.ModeloVeicularCargaDivisaoCapacidade = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaDivisaoCapacidade()
                            {
                                Codigo = produtoDivisaoCapacidade.DivisaoCapacidadeModeloVeicular.Codigo

                            };

                            foreach (Dominio.Entidades.Auditoria.HistoricoPropriedade alteracao in cargaPedidoProdutoDivisaoCapacidade.GetChanges())
                                valoresAlterados.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                                {
                                    De = alteracao.De,
                                    Para = alteracao.Para,
                                    Propriedade = $"Divisão {cargaPedidoProdutoDivisaoCapacidade.Codigo} - {alteracao.Propriedade}"
                                });

                            repCargaPedidoProdutoDivisaoCapacidade.Inserir(cargaPedidoProdutoDivisaoCapacidade);
                        }
                    }

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade divisaoExcluir in divisoesPedidoProduto)
                        repCargaPedidoProdutoDivisaoCapacidade.Deletar(divisaoExcluir);

                    cargaPedidoProduto.SetExternalChanges(valoresAlterados);
                    cargaPedidoProduto.SetChanges();

                    Auditoria.Auditoria.Auditar(auditado, cargaPedidoProduto, cargaPedidoProduto.GetChanges(), Localization.Resources.Cargas.ControleEntrega.SalvouDivisoesCapacidade, unitOfWork);
                }
            }
            if (atualizarSumarizados)
                repCargaDadosSumarizados.Atualizar(cargaEntrega.Carga.DadosSumarizados);
        }

        private static Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega AdicionarColetaEntrega(ObjetoGeracaoControleEntrega cliente, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, DateTime dataCriacaoEntrega, Dominio.Entidades.Embarcador.Cargas.Carga carga, int ordem, bool coletaExtra, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;
            bool possuiPedidosReentrega = false;

            if (cliente.Reentrega)
            {
                cargaEntrega = repCargaEntrega.BuscarReentregaPorClienteECarga(carga.Codigo, cliente.Cliente.CPF_CNPJ);
                if (cargaEntrega == null)
                    cargaEntrega = repCargaEntrega.BuscarReentregaPorClienteECarga(carga.Codigo, cliente.Cliente.CPF_CNPJ, false);

                possuiPedidosReentrega = true;
            }

            if (cargaEntrega == null)
            {
                cargaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega()
                {
                    Cliente = cliente.Cliente == null ? cliente.ClienteOutroEndereco?.Cliente : cliente.Cliente,
                    ClienteOutroEndereco = cliente.ClienteOutroEndereco,
                    Localidade = cliente.Localidade,
                    DataCriacao = dataCriacaoEntrega,
                    Carga = carga,
                    Situacao = SituacaoEntrega.NaoEntregue,
                    Ordem = ordem,
                    Distancia = cliente.Distancia,
                    TempoExtraEntrega = cliente.TempoBalsa,
                    Coleta = cliente.Coleta,
                    Fronteira = cliente.Fronteira,
                    Parqueamento = cliente.Parqueamento,
                    PostoFiscal = cliente.PostoFiscal,
                    ColetaAdicional = coletaExtra,
                    ColetaEquipamento = cliente.ColetaEquipamento,
                    PossuiNotaCobertura = cargasPedido.Any(obj => !string.IsNullOrWhiteSpace(obj.Pedido.NumeroControle)),
                    DataAgendamento = cargasPedido.FirstOrDefault()?.Pedido?.DataAgendamento,
                    InicioJanela = cargasPedido.FirstOrDefault()?.InicioJanelaDescarga,
                    FimJanela = cargasPedido.FirstOrDefault()?.FimJanelaDescarga,
                    ResponsavelAgendamento = cargasPedido.FirstOrDefault()?.Pedido?.Usuario,
                    OrigemCriacaoDataAgendamentoCargaEntrega = (cargasPedido.FirstOrDefault()?.Pedido?.OrigemCriacaoDataAgendamentoPedido.HasValue ?? false) ? cargasPedido.FirstOrDefault()?.Pedido?.OrigemCriacaoDataAgendamentoPedido.Value : null,
                    DistanciaAteDestino = cliente.Distancia,
                    CargaEntregaOrigem = cliente.CodigoCargaEntregaOriginal == null ? 0 : cliente.CodigoCargaEntregaOriginal,
                    HoraLimiteDescarga = cliente.Cliente?.ClienteDescargas?.FirstOrDefault()?.HoraLimiteDescarga ?? string.Empty,
                };
                repCargaEntrega.Inserir(cargaEntrega);
            }

            List<int> cargaEntregaPedidoExistentes = repositorioCargaEntregaPedido.BuscarCodigosCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);
            cargasPedido = cargasPedido.Where(obj => !cargaEntregaPedidoExistentes.Contains(obj.Codigo)).ToList();

            if (cargaEntrega.ColetaEquipamento)
                return cargaEntrega;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoPorCliente = GerarCargaEntregaPedido(cargasPedido, cargaEntrega, cargaPedidosXMLs, unitOfWork, configuracao);
            if (possuiPedidosReentrega)
                cargaEntrega.PossuiPedidosReentrega = true;
            else
                cargaEntrega.Reentrega = cargasPedidoPorCliente.Any(obj => obj.ReentregaSolicitada);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutoPedido = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto>();
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidoPorCliente)
                cargaPedidoProdutoPedido.AddRange((from obj in cargaPedidoProdutos where obj.CargaPedido.Codigo == cargaPedido.Codigo select obj).ToList());

            if (xmlNotaFiscalProdutos.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscais = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidoPorCliente)
                    pedidoXMLNotaFiscais.AddRange(ObterNotasFiscalsColetaEntrega(cargaPedido, cargaPedidosXMLs, cargaEntrega));

                GerarProdutosColetaEntregaPorNota(cargaEntrega, pedidoXMLNotaFiscais, xmlNotaFiscalProdutos, unitOfWork);
            }
            else
                GerarProdutosColetaEntrega(cargaEntrega, cargaPedidoProdutoPedido, cliente.PedidoXmlNotaFiscal?.XMLNotaFiscal, unitOfWork);

            // Checklist da coleta
            if (cargaEntrega.Coleta && (cargaEntrega.Carga?.TipoOperacao?.CheckListColeta != null || checkList != null) && !cargaEntrega.Fronteira && !cargaEntrega.Parqueamento)
                GerarCheckListColetaEntrega(cargaEntrega.Carga?.TipoOperacao?.CheckListColeta ?? checkList, cargaEntrega, TipoCheckList.Coleta, unitOfWork);

            // Checklist da entrega
            if (!cargaEntrega.Coleta && cargaEntrega.Carga?.TipoOperacao?.CheckListEntrega != null && !cargaEntrega.Fronteira && !cargaEntrega.Parqueamento)
                GerarCheckListColetaEntrega(cargaEntrega.Carga?.TipoOperacao?.CheckListEntrega, cargaEntrega, TipoCheckList.Entrega, unitOfWork);

            // Checklist de desembarque
            if (!cargaEntrega.Coleta && cargaEntrega.Carga?.TipoOperacao?.CheckListDesembarque != null && !cargaEntrega.Fronteira && !cargaEntrega.Parqueamento)
                GerarCheckListColetaEntrega(cargaEntrega.Carga?.TipoOperacao?.CheckListDesembarque, cargaEntrega, TipoCheckList.Desembarque, unitOfWork);

            if (cargaEntrega.Reentrega)
                cargaEntrega.PesoPedidosReentrega = cargasPedidoPorCliente.Sum(cp => cp.Peso);

            bool existeCargaOrigemDiferenteACarga = cargasPedido.Any(cp => cp.CargaControleEntrega != null && cp.Carga.Codigo != cp.CargaControleEntrega.Codigo);

            if (existeCargaOrigemDiferenteACarga)
                cargaEntrega.CargaOrigem = cargasPedido.Where(cp => cp.CargaControleEntrega != null && cp.Carga.Codigo != cp.CargaControleEntrega.Codigo).Select(cp => cp.CargaControleEntrega).FirstOrDefault();

            repCargaEntrega.Atualizar(cargaEntrega);
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
            return cargaEntrega;
        }

        public static void GerarCheckListColetaEntrega(Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, TipoCheckList tipoCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta repCargaEntregaCheckListPergunta = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa repCargaEntregaCheckListAlternativa = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa(unitOfWork);
            GestaoPatio.CheckList servicoChecklist = new GestaoPatio.CheckList(unitOfWork, CriarAuditoria());

            bool GerarCheckList = true;
            if (checkList.Clientes != null && checkList.Clientes.Count > 0)
            {
                GerarCheckList = false; //deve gerar se cliente esta na lista;
                if (checkList.Clientes.Contains(cargaEntrega.Cliente))
                    GerarCheckList = true;
            }

            if (GerarCheckList)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList cargaEntregaCheckList = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList
                {
                    CargaEntrega = cargaEntrega,
                    Descricao = checkList.Descricao,
                    TipoCheckList = tipoCheckList,
                    CheckListTipo = checkList,
                };

                repCargaEntregaCheckList.Inserir(cargaEntregaCheckList);

                List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> perguntas = checkList.Perguntas.ToList();

                servicoChecklist.ObterPerguntasFiltradas(cargaEntrega.Carga, perguntas);

                foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes pergunta in perguntas)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta cargaEntregaCheckListPergunta = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListPergunta
                    {
                        CodigoIntegracao = pergunta.CodigoIntegracao,
                        CargaEntregaCheckList = cargaEntregaCheckList,
                        Descricao = servicoChecklist.ObterDescricaoPergunta(pergunta, cargaEntrega.Carga),
                        Assunto = pergunta.ObterAssunto(),
                        Tipo = pergunta.Tipo,
                        Ordem = pergunta.Ordem,
                        Obrigatorio = pergunta.Obrigatorio,
                        TipoData = pergunta.TipoData,
                        TipoHora = pergunta.TipoHora,
                        PermiteNaoAplica = pergunta.PermiteNaoAplica,
                    };
                    repCargaEntregaCheckListPergunta.Inserir(cargaEntregaCheckListPergunta);

                    List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa> alternativas = pergunta.Alternativas.ToList();
                    foreach (Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa alternativa in alternativas)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa cargaEntregaCheckListAlternativa = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa
                        {
                            CodigoIntegracao = alternativa.CodigoIntegracao,
                            CargaEntregaCheckListPergunta = cargaEntregaCheckListPergunta,
                            Descricao = alternativa.Descricao,
                            Ordem = alternativa.Ordem,
                            Valor = alternativa.Valor,
                        };
                        repCargaEntregaCheckListAlternativa.Inserir(cargaEntregaCheckListAlternativa);
                    }
                }
            }
        }

        public static void GerarProdutosColetaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);

            bool multiplicar = cargaEntrega.Carga.TipoOperacao?.ConfiguracaoCarga?.MultiplicarQuantidadeProdutoPorCaixaPelaQuantidadeCaixa ?? false;
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador> produtosEmbarcador = (from obj in cargaPedidoProdutos select obj.Produto).Distinct().ToList();

            repCargaEntregaProduto.InsertSQLListaProdutos(produtosEmbarcador, cargaPedidoProdutos, cargaEntrega, xmlNotaFiscal, multiplicar);
        }

        private static Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPrazoEntrega ObterEntregaEmJanela(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataPrevista, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosJanela = ObterJanelaDescarregamento(cargaEntrega, dataPrevista, unitOfWork);

            if ((periodosJanela == null) || (periodosJanela.Count == 0))
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPrazoEntrega.NoPrazo;

            foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoJanela in periodosJanela)
            {

                DateTime dataInicio = dataPrevista.Date.Add(periodoJanela.HoraInicio);
                DateTime dataFim = dataPrevista.Date.Add(periodoJanela.HoraTermino);
                if (periodoJanela.HoraInicio > periodoJanela.HoraTermino)
                    dataFim.AddDays(1);

                if (dataPrevista < dataInicio)
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPrazoEntrega.Antecipado;

                if (dataPrevista > dataFim)
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPrazoEntrega.Atrasado;

            }

            return Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPrazoEntrega.NoPrazo;
        }

        private static Dominio.Entidades.Cliente ObterRemetente(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(codigoCarga);

            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete?.Codigo ?? 0);

            return pontosPassagem?.OrderBy(o => o.Ordem)?.FirstOrDefault()?.Cliente;
        }

        private static List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> GerarCargaEntregaPedido(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLCargas, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (configuracao.Pais == TipoPais.Brasil)
                cargasPedido = cargasPedido.Where(obj => !obj.PedidoSemNFe || obj.PedidoPallet || obj.ReentregaSolicitada || (obj.Carga.TipoOperacao?.FretePorContadoCliente ?? false) || (obj.Pedido.CanalEntrega?.LiberarPedidoSemNFeAutomaticamente ?? false)).ToList();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoPorCliente = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            cargasPedidoPorCliente.AddRange(cargasPedido.Where(o => (
                (
                    o.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado
                    || o.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual
                    || o.TipoRateio == TipoEmissaoCTeDocumentos.NaoInformado
                    || cargaPedidosXMLCargas.Count() == 0
                    || (
                        cargaEntrega.Coleta
                        && (
                            o.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidor
                            || o.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor
                        )
                        && o.Expedidor != null
                    )
                    || (
                        !cargaEntrega.Coleta
                        && (
                            o.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor
                            || o.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor
                        )
                        && o.Recebedor != null
                    )
                )
                && cargaEntrega.Cliente != null
                && cargaEntrega.Cliente?.CPF_CNPJ == (cargaEntrega.Coleta ? o?.ClienteColeta?.CPF_CNPJ : o?.ClienteEntrega?.CPF_CNPJ)
            )).ToList());

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosRateioPorNota = cargasPedido.Where(o => (
                o.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalAgrupada
                || o.TipoRateio == TipoEmissaoCTeDocumentos.EmitePorNotaFiscalIndividual
                || (
                    cargaEntrega.Coleta
                    && (
                        (
                            o.TipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComExpedidor
                            && o.TipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComExpedidorERecebedor
                        )
                        || o.Expedidor == null
                    )
                )
                || (
                    !cargaEntrega.Coleta
                    && (
                        (
                            o.TipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComRecebedor
                            && o.TipoEmissaoCTeParticipantes != TipoEmissaoCTeParticipantes.ComExpedidorERecebedor
                        )
                        || o.Recebedor == null
                    )
                )
            )).ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidosRateioPorNota)
            {
                if (cargasPedidoPorCliente.Any(o => o.Codigo == cargaPedido.Codigo))
                    continue;

                if (cargaEntrega.Coleta)
                {
                    if (cargaPedidosXMLCargas.Any(o => o.CargaPedido.Codigo == cargaPedido.Codigo &&
                                                       ((o.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada &&
                                                         o.XMLNotaFiscal.Destinatario.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ) ||
                                                        (o.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida &&
                                                         o.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ))))
                        cargasPedidoPorCliente.Add(cargaPedido);
                }
                else
                {
                    if (cargaPedidosXMLCargas.Any(o => o.CargaPedido.Codigo == cargaPedido.Codigo &&
                                                       ((o.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada &&
                                                         o.XMLNotaFiscal.Emitente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ) ||
                                                        (o.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida &&
                                                         o.XMLNotaFiscal.Destinatario.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ))))
                        cargasPedidoPorCliente.Add(cargaPedido);
                    else if (cargaPedido.PedidoSemNFe && cargaPedido.ObterDestinatario()?.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ) //caso o pedido nao tem nota, mas é para o mesmo destinatario vamos add o pedido a entrega
                        cargasPedidoPorCliente.Add(cargaPedido);
                }
            }

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosColetaAgendada = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            if (cargaEntrega.ClienteOutroEndereco != null)
            {
                if (cargaEntrega.Coleta)
                    cargasPedidoPorCliente = (from obj in cargasPedidoPorCliente where obj.Pedido.UsarOutroEnderecoOrigem && obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco?.Codigo == cargaEntrega.ClienteOutroEndereco.Codigo select obj).ToList();
                else
                    cargasPedidoPorCliente = (from obj in cargasPedidoPorCliente where obj.Pedido.UsarOutroEnderecoDestino && obj.Pedido.EnderecoDestino?.ClienteOutroEndereco?.Codigo == cargaEntrega.ClienteOutroEndereco.Codigo select obj).ToList();
            }
            else
            {
                if (cargaEntrega.Coleta)
                    cargasPedidoPorCliente = (from obj in cargasPedidoPorCliente where !obj.Pedido.UsarOutroEnderecoOrigem || obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco == null || obj.Expedidor != null || obj.Pedido.EnderecoOrigem?.ClienteOutroEndereco.Cliente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ select obj).ToList();
                else
                    cargasPedidoPorCliente = (from obj in cargasPedidoPorCliente where !obj.Pedido.UsarOutroEnderecoDestino || obj.Pedido.EnderecoDestino?.ClienteOutroEndereco == null || obj.Recebedor != null || obj.Pedido.EnderecoDestino?.ClienteOutroEndereco.Cliente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ select obj).ToList();
                //Adicionado pois na rota frete não tem clientes outros endereços e quando compra vale pedagio e a rota-frete é roteirizada não localizava os pedidos para geração do controle de entrega (obj.Pedido.EnderecoDestino?.ClienteOutroEndereco.Cliente.CPF_CNPJ == cargaEntrega.Cliente.CPF_CNPJ)
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargasPedidoPorCliente)
            {
                //vamos criar um insert para inserir todos os registros de uma vez;
                //Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido
                //{
                //    CargaPedido = cargaPedido,
                //    CargaEntrega = cargaEntrega
                //};
                //repCargaEntregaPedido.Inserir(cargaEntregaPedido);

                if (cargaEntrega.Coleta && cargaPedido.Pedido.Remetente.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ)
                    pedidosColetaAgendada.Add(cargaPedido.Pedido);

                GerarCargaEntregaNotas(cargaPedido, cargaPedidosXMLCargas, cargaEntrega, unitOfWork);
            }

            Log.TratarErro($"Roteirizacao |{cargaEntrega.Carga.Codigo}| ({cargaEntrega.Codigo}) - Gerando CargaEntregaPedido: ({(cargasPedidoPorCliente.Count > 0 ? string.Join(", ", cargasPedidoPorCliente.Select(x => x.Codigo).ToList()) : "Sem pedidos por cliente")})", "GeracaoControleEntrega");

            repCargaEntregaPedido.InsertSQLListaCargaPedido(cargasPedidoPorCliente, cargaEntrega);

            if (pedidosColetaAgendada.Count > 0)
                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosColetaAgendada select obj.Codigo).ToList(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.ColetaAgendada);

            return cargasPedidoPorCliente;
        }

        private static List<ObjetoGeracaoControleEntrega> ObterListaDeClientesSemOrdem(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            bool retornarColetas = carga.TipoOperacao?.GerarControleColeta ?? false;

            int ordem = 0;

            List<ObjetoGeracaoControleEntrega> retorno = new List<ObjetoGeracaoControleEntrega>();
            if (retornarColetas)
            {
                List<Dominio.Entidades.Cliente> clientes = (from obj in cargaPedidos where obj.Expedidor == null select obj.Pedido.Remetente).ToList();
                clientes.AddRange((from obj in cargaPedidos where obj.Expedidor != null select obj.Expedidor).ToList());
                clientes = clientes.Distinct().ToList();

                foreach (Dominio.Entidades.Cliente cliente in clientes)
                {
                    retorno.Add(new ObjetoGeracaoControleEntrega()
                    {
                        Cliente = cliente,
                        ClienteOutroEndereco = null,
                        Ordem = ordem,
                        Distancia = 0,
                        Coleta = true,
                        ColetaEquipamento = false
                    });
                    ordem++;
                }
            }

            List<Dominio.Entidades.Cliente> destinatario = (from obj in cargaPedidos where obj.Recebedor == null select obj.Pedido.Destinatario).ToList();
            destinatario.AddRange((from obj in cargaPedidos where obj.Recebedor != null select obj.Recebedor).ToList());
            destinatario = destinatario.Distinct().ToList();

            foreach (Dominio.Entidades.Cliente cliente in destinatario)
            {
                retorno.Add(new ObjetoGeracaoControleEntrega()
                {
                    Cliente = cliente,
                    ClienteOutroEndereco = null,
                    Ordem = ordem,
                    Distancia = 0,
                    Coleta = false,
                    ColetaEquipamento = false
                });
                ordem++;
            }


            return retorno;
        }

        private static List<ObjetoGeracaoControleEntrega> ObterListaDeClientesOrdenados(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem)
        {
            bool retornarColetas = carga.TipoOperacao?.GerarControleColeta ?? false;
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagemFiltrados = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();

            if (retornarColetas)
            {
                pontosDePassagemFiltrados = (
                    from o in pontosDePassagem
                    where (o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega
                            || o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta
                            || o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira
                            || o.TipoPontoPassagem == TipoPontoPassagem.PostoFiscal
                            || o.LocalDeParqueamento == true)
                    orderby o.Ordem
                    select o
                ).ToList();

                if (pontosDePassagemFiltrados.Count > 2)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem primeiroPonto = pontosDePassagemFiltrados.First();
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ultimoPonto = pontosDePassagemFiltrados.Last();
                    bool mesmoPontoPassagem = (primeiroPonto.Cliente != null) && (ultimoPonto.Cliente != null) && (primeiroPonto.Cliente.CPF_CNPJ == ultimoPonto.Cliente.CPF_CNPJ);

                    if (mesmoPontoPassagem && (ultimoPonto.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta))
                        pontosDePassagemFiltrados.Remove(ultimoPonto);
                }
            }
            else
            {
                pontosDePassagemFiltrados = (
                    from o in pontosDePassagem
                    where (o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega
                            || o.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira)
                    orderby o.Ordem
                    select o
                ).ToList();
            }

            return (
                from obj in pontosDePassagemFiltrados
                where obj.Cliente != null || obj.ClienteOutroEndereco?.Cliente != null || obj.Localidade != null
                select new ObjetoGeracaoControleEntrega()
                {
                    Cliente = obj.Cliente,
                    ClienteOutroEndereco = obj.ClienteOutroEndereco,
                    Localidade = obj.Localidade,
                    Ordem = obj.Ordem,
                    Distancia = obj.Distancia + (from bal in pontosDePassagem
                                                 where bal.TipoPontoPassagem == TipoPontoPassagem.Balsa
                                                    && bal.Ordem < obj.Ordem
                                                    && bal.Ordem > ((from t in pontosDePassagemFiltrados
                                                                     orderby t.Ordem descending
                                                                     where t.Ordem < obj.Ordem
                                                                     select t.Ordem)?.FirstOrDefault() ?? 0)
                                                 select bal.Distancia).Sum(),
                    TempoBalsa = (from bal in pontosDePassagem
                                  orderby bal.Ordem descending
                                  where bal.TipoPontoPassagem == TipoPontoPassagem.Balsa
                                     && bal.Ordem < obj.Ordem
                                     && bal.Ordem > ((from t in pontosDePassagemFiltrados
                                                      orderby t.Ordem descending
                                                      where t.Ordem < obj.Ordem
                                                      select t.Ordem)?.FirstOrDefault() ?? 0)
                                  select bal.Tempo)?.FirstOrDefault() ?? 0,
                    Coleta = obj.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta,
                    Fronteira = obj.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira,
                    PostoFiscal = obj.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal,
                    Parqueamento = obj.LocalDeParqueamento,
                    ColetaEquipamento = obj.ColetaEquipamento
                }
            ).ToList();
        }

        private static void DefinirPrevisaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, bool atualizarDataPrevisaoEntrega, bool atualizarDataPrevisaoEntregaReprogramada, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega Origem, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = null)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            DefinirPrevisaoCargaEntrega(carga, cargaRotaFrete, cargasEntrega, atualizarDataPrevisaoEntrega, atualizarDataPrevisaoEntregaReprogramada, configuracao, unitOfWork, tipoServicoMultisoftware, false, Origem, configuracaoControleEntrega);
        }

        private static void DefinirPrevisaoCargaEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, bool atualizarDataPrevisaoEntrega, bool atualizarDataPrevisaoEntregaReprogramada, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool inicial = false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega Origem = OrigemSituacaoEntrega.UsuarioMultiEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = null)
        {
            if (carga.DataFimViagem != null || cargasEntrega == null) return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            configuracaoControleEntrega ??= repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repositorioCargaEntregaPedido.BuscarPorCargaEntregas((from o in cargasEntrega select o.Codigo).ToList());

            if (carga.DataInicioViagem.HasValue == false && (carga.DataLoger.HasValue || carga.DataRealFaturamento.HasValue || carga.DataAgendamentoCarga.HasValue || (carga.DataInicioViagemPrevista.HasValue && !(configuracaoControleEntrega?.UtilizarPrevisaoEntregaPedidoComoDataPrevista ?? false))))
            {
                DateTime? dataBaseCalculo = ObteraDataBaseCargaLoger(carga, configuracao, unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega parametrosconfiguracao = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ParametrosConfiguracaoCalculoPrevisaoEntrega();
                parametrosconfiguracao.DataBase = dataBaseCalculo;
                parametrosconfiguracao.ArmazenarComposicoesPrevisoes = true;

                Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao, parametrosconfiguracao);
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = servicoPrevisaoControleEntrega.CalcularPrevisoesEntregas(carga, cargaRotaFrete, cargasEntrega, cargaEntregaPedidos);

                // Salva as previsões de entregas calculadas nas entregas da carga e na própria carga
                SalvarPrevisaoCargaEntrega(previsoesCargaEntrega, cargaEntregaPedidos, carga, cargasEntrega, atualizarDataPrevisaoEntrega, atualizarDataPrevisaoEntregaReprogramada, configuracao, unitOfWork, tipoServicoMultisoftware, inicial, Origem, null, configuracaoControleEntrega);
            }
            else
            {
                Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);
                // Calcula as datas de entrega (mantém os mesmos índices dos vetores)
                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = servicoPrevisaoControleEntrega.CalcularPrevisoesEntregas(carga, cargaRotaFrete, cargasEntrega, cargaEntregaPedidos);

                // Salva as previsões de entregas calculadas nas entregas da carga e na própria carga
                SalvarPrevisaoCargaEntrega(previsoesCargaEntrega, cargaEntregaPedidos, carga, cargasEntrega, atualizarDataPrevisaoEntrega, atualizarDataPrevisaoEntregaReprogramada, configuracao, unitOfWork, tipoServicoMultisoftware, inicial, Origem, null, configuracaoControleEntrega);
            }

            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntregaColeta = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);
            if (configuracaoControleEntrega?.CalcularDataAgendamentoAutomaticamenteDataFaturamento ?? false)
                servicoPrevisaoControleEntregaColeta.CalcularDataAgendamentoColetaAoFecharCargaAutomatico(carga, unitOfWork);

        }

        private static List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> ObterDocumentosPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Cliente clienteDestino, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento> documentosMob = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in carga.Entregas)
                documentosMob.Add(ConverterDocumento(cargaEntrega, clienteMultisoftware, unitOfWork));


            documentosMob = documentosMob.OrderBy(obj => obj.OrdemEntrega).ToList();

            for (int i = 0; i < documentosMob.Count; i++)
                documentosMob[i].OrdemEntrega = i + 1;

            return documentosMob;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento ConverterDocumento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Servicos.Embarcador.Mobile.Cargas.Carga();
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);
            Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);
            Servicos.WebService.Filial.Filial serFilial = new WebService.Filial.Filial(unitOfWork);


            Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento documentooMob = new Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento
            {
                CodigoIntegracao = cargaEntrega.Codigo,
                Carga = serCarga.ConverterCarga(cargaEntrega.Carga, clienteMultisoftware, false, unitOfWork),
                Destinatario = serPessoa.ConverterObjetoPessoa(cargaEntrega.Cliente),
                Emitente = null,
                Numero = cargaEntrega.Codigo,
                Serie = 1,
                Coleta = cargaEntrega.Coleta,
                OrdemEntrega = cargaEntrega.Ordem,
                NumeroNF = String.Join("", (from notas in cargaEntrega?.NotasFiscais select notas?.PedidoXMLNotaFiscal?.XMLNotaFiscal?.Numero)),
                NumeroPedido = String.Join("", (from pedidos in cargaEntrega?.Pedidos select pedidos.CargaPedido?.Pedido?.NumeroPedidoEmbarcador)),
            };

            return documentooMob;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado CriarAuditoria()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceRastreadora,
            };
        }

        private static Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento ObterAtendimento(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises)
        {
            List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalisesAtendimento = (from obj in chamadoAnalises where obj.Chamado.Codigo == chamado.Codigo select obj).ToList();
            Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento atendimento = new Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Atendimento()
            {
                Codigo = chamado.Codigo,
                Numero = chamado.Numero,
                Descricao = chamado.Descricao,
                ProtocoloCarga = chamado.Carga?.Codigo ?? 0,
                Tipo = chamado.MotivoChamado.TipoMotivoAtendimento,
                DescricaoCarga = chamado.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                CodigoMotivo = chamado.MotivoChamado?.Codigo ?? 0,
                DescricaoMotivo = chamado.MotivoChamado?.Descricao ?? string.Empty,
                TipoCliente = chamado.TipoCliente,
                CNPJCliente = chamado.Destinatario?.CPF_CNPJ ?? 0,
                DescricaoCliente = chamado.Destinatario?.Descricao ?? string.Empty,
                Situacao = chamado.Situacao,
                DescricaoSituacao = chamado.DescricaoSituacao,
                DataCriacao = chamado.DataCriacao.ToString(),
                DataRetencaoInicio = chamado.DataRetencaoInicio.HasValue ? chamado.DataRetencaoInicio.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                DataRetencaoFim = chamado.DataRetencaoFim.HasValue ? chamado.DataRetencaoFim.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                DataReentrega = chamado.DataReentrega.HasValue ? chamado.DataReentrega.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                TempoRetencao = chamado.TempoRetencao,
                RetencaoBau = chamado.RetencaoBau,
                Observacao = chamado.Observacao,
                Analises = String.Join(" / ", (from obj in chamadoAnalisesAtendimento select obj.Observacao).ToList()),
                HistoricoAnalises = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Analise>()
            };

            foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analise in chamadoAnalisesAtendimento)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Analise historio = new Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Analise();
                historio.Analista = new Dominio.ObjetosDeValor.Embarcador.Mobile.Atendimentos.Analista() { Nome = analise.Autor.Nome };
                historio.Codigo = analise.Codigo;
                historio.DataCriacao = analise.DataCriacao.ToString("dd/MM/yyyy HH:mm:ss");
                historio.DataRetorno = analise.DataRetorno?.ToString("dd/MM/yyyy HH:mm:ss") ?? "";
                historio.Observacao = analise.Observacao;
                atendimento.HistoricoAnalises.Add(historio);
            }

            return atendimento;
        }

        private static void VincularCargaEntregaComCargaEntregaorigem(List<ObjetoGeracaoControleEntrega> objetoGeracaoControleEntrega, IList<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada> lstVinculoEntreCargaEntregaComCargaEntregaTransbordada)
        {
            foreach (ObjetoGeracaoControleEntrega objetoGeracao in objetoGeracaoControleEntrega)
            {
                Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada VinculoCargaEntregaTransbordo = lstVinculoEntreCargaEntregaComCargaEntregaTransbordada.Where(x => x.CodigoCliente == objetoGeracao.Cliente.CPF_CNPJ).FirstOrDefault();
                if (VinculoCargaEntregaTransbordo != null)
                    objetoGeracao.CodigoCargaEntregaOriginal = VinculoCargaEntregaTransbordo.CodigoCargaEntregaOrigem;
            }
        }

        private static List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> GerarCargaEntregaParaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedido, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem, bool ordenarClientes, bool excluir, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.TransferirEntregaParametros transferirEntregaParametros = null;
            if (excluir)
            {
                transferirEntregaParametros = ObterParametrosTransferirEntrega(carga, unitOfWork);
                if (transferirEntregaParametros != null)
                    ExcluirCargaEntrega(carga, unitOfWork);
                else
                    excluir = false;
            }

            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repositorioConfiguracaoTipoOperacaoControleEntrega = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada> lstVinculoEntreCargaEntregaComCargaEntregaTransbordada = configuracaoControleEntrega.ConsiderarCargaOrigemParaEntregasTransbordadas ? repTransbordo.ConsultarVinculoEntreCargaEntregaComCargaEntregaTransbordada(carga.Codigo) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada>();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(carga.Codigo);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList = repCheckListTipo.BuscarPrimeiroPorCargaPedidoProduto(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutos = repXMLNotaFiscalProduto.BuscarPorNotaFiscais(carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = null;


            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repositorioConfiguracaoTipoOperacaoControleEntrega.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

            DateTime dataCriacaoEntrega = DateTime.Now;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            bool possuiRecebedor = cargasPedido.Any(o => (o.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComRecebedor || o.TipoEmissaoCTeParticipantes == TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && o.Recebedor != null);
            if ((carga.TipoOperacao?.GerarEntregaPorNotaFiscalCarga ?? false) && !possuiRecebedor)
            {
                cargaEntregas = GerarCargaEntregaPorNotaFiscalCarga(carga, cargasPedido, cargaPedidoProdutos, cargaPedidosXMLs, xmlNotaFiscalProdutos, checkList, configuracao, unitOfWork, lstVinculoEntreCargaEntregaComCargaEntregaTransbordada);
            }
            else if (carga.CargaTransbordo && tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
            {
                cargaEntregas = GerarCargaEntregaPorTransbordo(carga, cargasPedido, cargaPedidoProdutos, cargaPedidosXMLs, xmlNotaFiscalProdutos, configuracao, unitOfWork, lstVinculoEntreCargaEntregaComCargaEntregaTransbordada, pontosPassagem, ordenarClientes);
            }
            else
            {
                List<ObjetoGeracaoControleEntrega> clientesOrdenados = ordenarClientes ? ObterListaDeClientesOrdenados(carga, pontosPassagem) : ObterListaDeClientesSemOrdem(carga, cargasPedido);
                if ((carga.Redespacho?.Carga?.OrdemRoteirizacaoDefinida ?? false) && cargasPedido.Count() == carga.Redespacho?.Carga?.Pedidos.Count())
                {
                    List<ObjetoGeracaoControleEntrega> clientesOrdenadosCargaAnterior = (
                       from obj in carga.Redespacho.Carga.Entregas
                       where obj.Cliente != null && clientesOrdenados.Any(c => c.Cliente?.Codigo == obj.Cliente?.Codigo)
                       select new ObjetoGeracaoControleEntrega()
                       {
                           Cliente = obj.Cliente,
                           ClienteOutroEndereco = obj.ClienteOutroEndereco,
                           Ordem = obj.Ordem,
                           Distancia = obj.Distancia,
                           Coleta = obj.Coleta,
                           ColetaEquipamento = obj.ColetaEquipamento,
                           Fronteira = obj.Fronteira,
                           Parqueamento = obj.Parqueamento,
                       }
                   ).ToList();

                    if (clientesOrdenadosCargaAnterior != null && clientesOrdenadosCargaAnterior.Count > 0)
                    {
                        List<Dominio.Entidades.Cliente> clientesDiferentesCargaAnterior = clientesOrdenadosCargaAnterior.Select(x => x.Cliente).Distinct().ToList();
                        List<Dominio.Entidades.Cliente> clientesDiferentesCarga = clientesOrdenados.Select(x => x.Cliente).Distinct().ToList();

                        if (clientesDiferentesCargaAnterior?.Count >= clientesDiferentesCarga.Count)
                            clientesOrdenados = clientesOrdenadosCargaAnterior;
                    }
                }

                VincularCargaEntregaComCargaEntregaorigem(clientesOrdenados, lstVinculoEntreCargaEntregaComCargaEntregaTransbordada);

                int ordem = 0;
                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - clientesOrdenados.Cliente: ({string.Join(", ", clientesOrdenados.Count > 0 ? string.Join(", ", clientesOrdenados.Select(x => x.Cliente?.CPF_CNPJ).Distinct().ToList()) : ("Sem cliente"))})", "GeracaoControleEntrega");
                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - clientesOrdenados.ClienteOutroEndereco: ({string.Join(", ", clientesOrdenados.Count > 0 ? string.Join(", ", clientesOrdenados.Select(x => x.ClienteOutroEndereco?.Cliente?.CPF_CNPJ).Distinct().ToList()) : ("Sem clienteOutroEndereco"))})", "GeracaoControleEntrega");
                Log.TratarErro($"Roteirizacao |{carga.Codigo}| - clientesOrdenados.Localidade: ({string.Join(", ", clientesOrdenados.Count > 0 ? string.Join(", ", clientesOrdenados.Select(x => x.Localidade?.Codigo).Distinct().ToList()) : ("Sem localidade"))})", "GeracaoControleEntrega");

                foreach (ObjetoGeracaoControleEntrega cliente in clientesOrdenados)
                {
                    cargaEntregas.Add(AdicionarColetaEntrega(cliente, cargasPedido, cargaPedidosXMLs, dataCriacaoEntrega, carga, ordem, false, cargaPedidoProdutos, xmlNotaFiscalProdutos, checkList, unitOfWork, configuracao));
                    ordem++;
                }
            }

            if (configuracaoTipoOperacaoControleEntrega?.OrdenarColetasPorDataCarregamento ?? false)
            {
                // Filtra cargas com coleta ativa
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> coletas = cargaEntregas.Where(x => x.Coleta).ToList();

                if (coletas != null && coletas.Count > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos =
                        repositorioCargaEntregaPedido.BuscarPorCargaEntregas(coletas.Select(o => o.Codigo).ToList());

                    if (cargaEntregaPedidos != null && cargaEntregaPedidos.Count > 0)
                    {
                        coletas = coletas
                            .OrderBy(coleta =>
                            {
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido pedidoAssociado = cargaEntregaPedidos
                                    .Where(p => p.CargaEntrega.Codigo == coleta.Codigo)
                                    .OrderBy(p => p.CargaPedido?.Pedido?.DataInicialColeta)
                                    .FirstOrDefault();

                                return pedidoAssociado?.CargaPedido?.Pedido?.DataInicialColeta;
                            })
                            .ToList();

                        for (int i = 0; i < coletas.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega coleta = cargaEntregas.FirstOrDefault(x => x.Codigo == coletas[i].Codigo);
                            if (coleta != null)
                            {
                                coleta.Ordem = i;
                                repCargaEntrega.Atualizar(coleta);

                            }
                        }

                    }
                }
            }

            if ((carga.TipoOperacao?.ObrigatorioVincularContainerCarga ?? false) && tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
            {
                Servicos.Embarcador.Pedido.ColetaContainer servivoColetaContainer = new Servicos.Embarcador.Pedido.ColetaContainer(unitOfWork);
                Servicos.Embarcador.Pedido.ConferenciaContainer servivoConferenciaContainer = new Servicos.Embarcador.Pedido.ConferenciaContainer(unitOfWork);

                servivoColetaContainer.GerarSolicitacaoColetaContainer(cargaEntregas, cargasPedido);
                servivoConferenciaContainer.Adicionar(carga);
            }

            if (excluir)
            {
                transferirEntregaParametros.CargaEntregasNovas = cargaEntregas;
                TransferirDadosDasEntregasAntigas(transferirEntregaParametros, carga, transbordo, unitOfWork);
            }
            else if (carga.CargaTransbordo)
            {
                transbordo = repositorioTransbordo.BuscarPorCargaGerada(carga.Codigo);

                if (transbordo != null)
                {
                    transferirEntregaParametros = ObterParametrosTransferirEntrega(transbordo.Carga, unitOfWork);

                    if (transferirEntregaParametros != null)
                    {
                        transferirEntregaParametros.CargaEntregasNovas = cargaEntregas;

                        TransferirDadosDasEntregasAntigas(transferirEntregaParametros, carga, transbordo, unitOfWork);
                    }

                }
            }

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            if (carga.TipoOperacao?.InicioViagemPorCargaGerada ?? false)
                IniciarViagem(carga.Codigo, carga.DataCriacaoCarga, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracao, tipoServicoMultisoftware, null, auditado, unitOfWork);

            return cargaEntregas;
        }

        private static List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> GerarCargaEntregaPorNotaFiscalCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutosCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutosCarga, Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, IList<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada> lstVinculoEntreCargaEntregaComCargaEntregaTransbordada)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            List<ObjetoGeracaoControleEntrega> objetosGeracao = ObterObjetoGeracaoControleEntregaListaNotasFiscais(cargaPedidosXMLs);

            VincularCargaEntregaComCargaEntregaorigem(objetosGeracao, lstVinculoEntreCargaEntregaComCargaEntregaTransbordada);

            int ordem = 0;
            foreach (ObjetoGeracaoControleEntrega objetoGeracao in objetosGeracao)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedidoNota = (from obj in cargasPedidoCarga where obj.Codigo == objetoGeracao.PedidoXmlNotaFiscal.CargaPedido.Codigo select obj).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> listaCargaPedidoProdutoXmlNota = (from obj in cargaPedidoProdutosCarga where obj.CargaPedido.Codigo == objetoGeracao.PedidoXmlNotaFiscal.CargaPedido.Codigo select obj).ToList();

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> listaXmNotaFiscalProdutoCarga = (from obj in xmlNotaFiscalProdutosCarga where obj.Pedido != null && obj.Pedido.Codigo == objetoGeracao.PedidoXmlNotaFiscal.CargaPedido.Pedido?.Codigo select obj).ToList();
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> listaPedidoXmlNota = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
                if (carga.TipoOperacao?.VincularApenasUmaNotaPorEntrega ?? false)
                    listaPedidoXmlNota = (from obj in cargaPedidosXMLs where obj.XMLNotaFiscal.Codigo == objetoGeracao.CodigoNota select obj).ToList();
                else
                    listaPedidoXmlNota = (from obj in cargaPedidosXMLs where obj.CargaPedido.Codigo == objetoGeracao.PedidoXmlNotaFiscal.CargaPedido.Codigo select obj).ToList();

                cargaEntregas.Add(AdicionarColetaEntrega(objetoGeracao, listaCargaPedidoNota, listaPedidoXmlNota, DateTime.Now, carga, ordem, false, listaCargaPedidoProdutoXmlNota, listaXmNotaFiscalProdutoCarga, checkList, unitOfWork, configuracao));
                ordem++;
            }

            return cargaEntregas;
        }

        private static List<ObjetoGeracaoControleEntrega> ObterObjetoGeracaoControleEntregaListaNotasFiscais(List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasfiscais)
        {
            int ordem = 0;
            List<ObjetoGeracaoControleEntrega> retorno = new List<ObjetoGeracaoControleEntrega>();
            notasfiscais = notasfiscais.Distinct().ToList();
            notasfiscais = notasfiscais.OrderBy(x => x.OrdemEntrega).ToList();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal notaFiscal in notasfiscais)
            {
                retorno.Add(new ObjetoGeracaoControleEntrega()
                {
                    PedidoXmlNotaFiscal = notaFiscal,
                    CodigoNota = notaFiscal.XMLNotaFiscal.Codigo,
                    Cliente = notaFiscal.XMLNotaFiscal.Destinatario,
                    ClienteOutroEndereco = notaFiscal.CargaPedido.Pedido.EnderecoDestino?.ClienteOutroEndereco,
                    Ordem = ordem,
                    Distancia = 0,
                    Coleta = false,
                    ColetaEquipamento = false
                });
                ordem++;
            }

            return retorno;
        }

        private static List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> GerarCargaEntregaPorTransbordo(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargasPedidoCarga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutosCarga, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> xmlNotaFiscalProdutosCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, IList<Dominio.ObjetosDeValor.Embarcador.Carga.VinculoEntreCargaEntregaComCargaEntregaTransbordada> lstVinculoEntreCargaEntregaComCargaEntregaTransbordada, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem, bool ordenarClientes)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidosDocumentosCTe = repCargaPedidoDocumentoCTe.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlNotasFicaisCarga = repXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

            List<ObjetoGeracaoControleEntrega> objetosGeracao = ordenarClientes ? ObterListaDeClientesOrdenados(carga, pontosDePassagem) : ObterObjetoGeracaoControleEntregaTransbordo(carga, xmlNotasFicaisCarga, cargaPedidosDocumentosCTe, unitOfWork);

            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListTipo checkList = repCheckListTipo.BuscarPrimeiroPorCargaPedidoProduto(carga.Codigo);

            VincularCargaEntregaComCargaEntregaorigem(objetosGeracao, lstVinculoEntreCargaEntregaComCargaEntregaTransbordada);

            int ordem = 0;
            foreach (ObjetoGeracaoControleEntrega objetoGeracao in objetosGeracao)
            {
                cargaEntregas.Add(AdicionarColetaEntrega(objetoGeracao, cargasPedidoCarga, cargaPedidosXMLs, DateTime.Now, carga, ordem, false, cargaPedidoProdutosCarga, xmlNotaFiscalProdutosCarga, checkList, unitOfWork, configuracao));
                ordem++;
            }

            return cargaEntregas;
        }

        private static List<ObjetoGeracaoControleEntrega> ObterObjetoGeracaoControleEntregaTransbordo(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasfiscais, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> documentosCTe, Repositorio.UnitOfWork unitOfWork)
        {
            bool retornarColetas = carga.TipoOperacao?.GerarControleColeta ?? false;
            int ordem = 0;
            List<ObjetoGeracaoControleEntrega> retorno = new List<ObjetoGeracaoControleEntrega>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notafiscal in notasfiscais)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from obj in documentosCTe where obj.CTe.XMLNotaFiscais.Contains(notafiscal) select obj.CTe).FirstOrDefault();
                if (cte?.Recebedor == null)
                    destinatarios.Add(notafiscal.Destinatario);
                else
                {
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cte.Recebedor.CPF_CNPJ.ToDouble());
                    destinatarios.Add(cliente);
                }

                if (retornarColetas)
                {
                    if (cte?.Expedidor == null)
                        remetentes.Add(notafiscal.Emitente);
                    else
                    {
                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cte.Expedidor.CPF_CNPJ.ToDouble());
                        remetentes.Add(cliente);
                    }
                }
            }

            if (retornarColetas)
            {
                remetentes.Distinct().ToList();
                foreach (Dominio.Entidades.Cliente remetente in remetentes)
                {
                    retorno.Add(new ObjetoGeracaoControleEntrega()
                    {
                        Cliente = remetente,
                        ClienteOutroEndereco = null,
                        Ordem = ordem,
                        Distancia = 0,
                        Coleta = true,
                        ColetaEquipamento = false
                    });
                    ordem++;
                }
            }

            destinatarios = destinatarios.Distinct().ToList();
            foreach (Dominio.Entidades.Cliente destinatario in destinatarios)
            {
                retorno.Add(new ObjetoGeracaoControleEntrega()
                {
                    Cliente = destinatario,
                    ClienteOutroEndereco = null,
                    Ordem = ordem,
                    Distancia = 0,
                    Coleta = false,
                    ColetaEquipamento = false
                });
                ordem++;
            }

            return retorno;
        }

        private static Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.TransferirEntregaParametros ObterParametrosTransferirEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.TransferirEntregaParametros transferirEntregaParametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.TransferirEntregaParametros();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            transferirEntregaParametros.CargaEntregasAntigas = repCargaEntrega.BuscarPorCarga(carga.Codigo);

            if (transferirEntregaParametros.CargaEntregasAntigas.Count == 0)
                return null;

            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
            transferirEntregaParametros.AlertasMonitor = repAlertaMonitor.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento repositorioPerdaSinalMonitoramento = new Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento(unitOfWork);
            transferirEntregaParametros.PerdasSinalMonitoramento = repositorioPerdaSinalMonitoramento.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(unitOfWork);
            transferirEntregaParametros.CargaEntregasAssinaturaResponsavel = repCargaEntregaAssinaturaResponsavel.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
            transferirEntregaParametros.CargaEntregasCheckList = repCargaEntregaCheckList.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
            transferirEntregaParametros.CargaEntregasFoto = repCargaEntregaFoto.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal repCargaEntregaFotoNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal(unitOfWork);
            transferirEntregaParametros.CargaEntregasFotoNotaFiscal = repCargaEntregaFotoNotaFiscal.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unitOfWork);
            transferirEntregaParametros.CargaEntregasGuiaTransporteAnimal = repCargaEntregaGuiaTransporteAnimal.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
            transferirEntregaParametros.CargaEntregasNFeDevolucao = repCargaEntregaNFeDevolucao.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            transferirEntregaParametros.CargaOcorrencias = repCargaOcorrencia.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado repCargaEntregaNotaFiscalChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado(unitOfWork);
            transferirEntregaParametros.CargaEntregaNotaFiscalChamado = repCargaEntregaNotaFiscalChamado.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(unitOfWork);
            transferirEntregaParametros.CargaEntregaProdutoChamado = repCargaEntregaProdutoChamado.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            transferirEntregaParametros.Chamados = repChamado.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
            transferirEntregaParametros.ChamadosAnalise = repChamadoAnalise.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
            transferirEntregaParametros.ChamadosOcorrencias = repChamadoOcorrencia.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.ChamadoAnexo repositorioChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);
            transferirEntregaParametros.ChamadosAnexos = repositorioChamadoAnexo.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.ChamadoData repositorioChamadoData = new Repositorio.Embarcador.Chamados.ChamadoData(unitOfWork);
            transferirEntregaParametros.ChamadosDatas = repositorioChamadoData.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(unitOfWork);
            transferirEntregaParametros.ChamadosInformacoesFechamento = repositorioChamadoInformacaoFechamento.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Chamados.NivelAtendimento repositorioChamadoNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork);
            transferirEntregaParametros.ChamadosNiveisAtendimento = repositorioChamadoNivelAtendimento.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            transferirEntregaParametros.OcorrenciasColetaEntregas = repOcorrenciaColetaEntrega.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
            transferirEntregaParametros.PermanenciasClientes = repPermanenciaCliente.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente repositorioMonitoramentoHistoricoStatusViagemPermanenciaCliente = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente(unitOfWork);
            transferirEntregaParametros.MonitoramentoHistoricoStatusViagemPermanenciaCliente = repositorioMonitoramentoHistoricoStatusViagemPermanenciaCliente.BuscarPorCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
            transferirEntregaParametros.PermanenciasSubareas = repPermanenciaSubarea.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea repositorioMonitoramentoHistoricoStatusViagemPermanenciaSubArea = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea(unitOfWork);
            transferirEntregaParametros.MonitoramentoHistoricoStatusViagemPermanenciaSubArea = repositorioMonitoramentoHistoricoStatusViagemPermanenciaSubArea.BuscarPorCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repcargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
            transferirEntregaParametros.CargaEventos = repcargaEvento.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repIntegracaoSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(unitOfWork);
            transferirEntregaParametros.IntegracaoSuperApp = repIntegracaoSuperApp.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(unitOfWork);
            transferirEntregaParametros.gestaoDadosColetas = repGestaoDadosColeta.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repGestaoDadosColetaDadosNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(unitOfWork);
            transferirEntregaParametros.GestaoDadosColetaDadosNFe = repGestaoDadosColetaDadosNFe.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte repGestaoDadosColetaDadosTransporte = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte(unitOfWork);
            transferirEntregaParametros.GestaoDadosColetaDadosTransporte = repGestaoDadosColetaDadosTransporte.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao repGestaoDadosColetaIntegracao = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao(unitOfWork);
            transferirEntregaParametros.GestaoDadosColetaIntegracao = repGestaoDadosColetaIntegracao.BuscarPorCargaEntregaCodigoCarga(carga.Codigo);

            Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet repGestaoDevolucaoNFeTransferenciaPallet = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet(unitOfWork);
            transferirEntregaParametros.GestaoDevolucaoNFeTransferenciaPallet = repGestaoDevolucaoNFeTransferenciaPallet.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
            transferirEntregaParametros.ControleNotaDevolucao = repControleNotaDevolucao.BuscarPorCarga(carga.Codigo);

            Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repMonitoramentoNotificacoesApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(unitOfWork);
            transferirEntregaParametros.MonitoramentoNotificacoesApp = repMonitoramentoNotificacoesApp.BuscarPorCargaEntregaCodigoCarga(carga.Codigo).GetAwaiter().GetResult();

            return transferirEntregaParametros;
        }

        private static void TransferirDadosDasEntregasAntigas(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.TransferirEntregaParametros parametros, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Repositorio.UnitOfWork unitOfWork)
        {
            List<int> codigoEntregasJaPassou = new List<int>();

            int total = parametros.CargaEntregasNovas.Count;
            for (int i = 0; i < total; i++)
            {
                int totalCargaEntregasAntigas = parametros.CargaEntregasAntigas.Count;
                for (int j = 0; j < totalCargaEntregasAntigas; j++)
                {
                    if (
                        parametros.CargaEntregasNovas[i].Cliente != null && parametros.CargaEntregasAntigas[j].Cliente != null &&
                        parametros.CargaEntregasNovas[i].Cliente.CPF_CNPJ == parametros.CargaEntregasAntigas[j].Cliente.CPF_CNPJ && parametros.CargaEntregasNovas[i].Coleta == parametros.CargaEntregasAntigas[j].Coleta
                    )
                    {
                        if (!codigoEntregasJaPassou.Contains(parametros.CargaEntregasAntigas[j].Codigo))
                        {
                            if (!carga.CargaTransbordo)
                                TransferirDadosDaEntregaAntigaParaNova(parametros.CargaEntregasAntigas[j], parametros.CargaEntregasNovas[i], parametros, unitOfWork);
                            else
                                TransferirDadosDaEntregaAntigaParaNovaTransbordo(parametros.CargaEntregasAntigas[j], parametros.CargaEntregasNovas[i], parametros, transbordo, unitOfWork);

                            codigoEntregasJaPassou.Add(parametros.CargaEntregasAntigas[j].Codigo);
                            break;
                        }
                    }
                }
            }
        }

        private static void TransferirDadosDaEntregaAntigaParaNovaTransbordo(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAntiga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaNova, TransferirEntregaParametros parametros, Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, UnitOfWork unitOfWork)
        {

            if (transbordo != null)
            {

                bool ehColeta = cargaEntregaNova.Coleta;

                // Se for coleta e não deve clonar coleta, não segue
                if (ehColeta && !transbordo.ClonaLancamentoColetas)
                    return;

                // Se for entrega e não deve clonar entrega, não segue
                if (!ehColeta && !transbordo.ClonaLancamentoEntregas)
                    return;

            }


            cargaEntregaNova.Situacao = cargaEntregaAntiga.Situacao;
            cargaEntregaNova.ClienteOutroEndereco = cargaEntregaAntiga.ClienteOutroEndereco;
            cargaEntregaNova.Localidade = cargaEntregaAntiga.Localidade;
            cargaEntregaNova.DataPrevisaoEntregaAjustada = cargaEntregaAntiga.DataPrevisaoEntregaAjustada;
            cargaEntregaNova.DataPrevista = cargaEntregaAntiga.DataPrevista;
            cargaEntregaNova.DataFimPrevista = cargaEntregaAntiga.DataFimPrevista;
            cargaEntregaNova.DataConfirmacao = cargaEntregaAntiga.DataConfirmacao;
            cargaEntregaNova.DataConfirmacaoApp = cargaEntregaAntiga.DataConfirmacaoApp;
            cargaEntregaNova.ResponsavelFinalizacaoManual = cargaEntregaAntiga.ResponsavelFinalizacaoManual;
            cargaEntregaNova.DataAgendamento = cargaEntregaAntiga.DataAgendamento;
            cargaEntregaNova.DataAgendamentoEntregaTransportador = cargaEntregaAntiga.DataAgendamentoEntregaTransportador;
            cargaEntregaNova.DataReentregaEmMesmaCarga = cargaEntregaAntiga.DataReentregaEmMesmaCarga;
            cargaEntregaNova.DataRejeitado = cargaEntregaAntiga.DataRejeitado;
            cargaEntregaNova.DataReprogramada = cargaEntregaAntiga.DataReprogramada;
            cargaEntregaNova.Tendencia = cargaEntregaAntiga.Tendencia;
            cargaEntregaNova.QuantidadePacotesColetados = cargaEntregaAntiga.QuantidadePacotesColetados;
            cargaEntregaNova.Coleta = cargaEntregaAntiga.Coleta;
            cargaEntregaNova.Observacao = cargaEntregaAntiga.Observacao;
            cargaEntregaNova.MotivoRejeicao = cargaEntregaAntiga.MotivoRejeicao;
            cargaEntregaNova.MotivoRetificacaoColeta = cargaEntregaAntiga.MotivoRetificacaoColeta;
            cargaEntregaNova.OrigemSituacao = cargaEntregaAntiga.OrigemSituacao;
            cargaEntregaNova.OrigemSituacaoFimViagem = cargaEntregaAntiga.OrigemSituacaoFimViagem;
            cargaEntregaNova.CargaEntregaFinalizacaoAssincrona = cargaEntregaAntiga.CargaEntregaFinalizacaoAssincrona;
            cargaEntregaNova.IdTrizy = cargaEntregaAntiga.IdTrizy;
            cargaEntregaNova.DataEntradaRaio = cargaEntregaAntiga.DataEntradaRaio;
            cargaEntregaNova.DataSaidaRaio = cargaEntregaAntiga.DataSaidaRaio;
            cargaEntregaNova.DistanciaAteDestino = cargaEntregaAntiga.DistanciaAteDestino;
            cargaEntregaNova.PossuiPedidosReentrega = cargaEntregaAntiga.PossuiPedidosReentrega;
            cargaEntregaNova.OrdemRealizada = cargaEntregaAntiga.OrdemRealizada;
            cargaEntregaNova.LatitudeFinalizada = cargaEntregaAntiga.LatitudeFinalizada;
            cargaEntregaNova.LongitudeFinalizada = cargaEntregaAntiga.LongitudeFinalizada;
            cargaEntregaNova.Fronteira = cargaEntregaAntiga.Fronteira;
            cargaEntregaNova.Parqueamento = cargaEntregaAntiga.Parqueamento;
            cargaEntregaNova.CargaEntregaQualidade = cargaEntregaAntiga.CargaEntregaQualidade;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            repCargaEntrega.Atualizar(cargaEntregaNova);

            Servicos.Auditoria.Auditoria.TrocarAuditoria(cargaEntregaAntiga, cargaEntregaNova, unitOfWork);

            unitOfWork.Flush();
        }

        private static void TransferirDadosDaEntregaAntigaParaNova(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAntiga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaNova, Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.TransferirEntregaParametros parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            cargaEntregaNova.Fronteira = cargaEntregaAntiga.Fronteira;
            cargaEntregaNova.Parqueamento = cargaEntregaAntiga.Parqueamento;
            cargaEntregaNova.Situacao = cargaEntregaAntiga.Situacao;
            cargaEntregaNova.DataInicio = cargaEntregaAntiga.DataInicio;
            cargaEntregaNova.DataFim = cargaEntregaAntiga.DataFim;
            cargaEntregaNova.DataConfirmacao = cargaEntregaAntiga.DataConfirmacao;
            cargaEntregaNova.LatitudeFinalizada = cargaEntregaAntiga.LatitudeFinalizada;
            cargaEntregaNova.LongitudeFinalizada = cargaEntregaAntiga.LongitudeFinalizada;
            cargaEntregaNova.OrdemRealizada = cargaEntregaAntiga.OrdemRealizada;
            cargaEntregaNova.DataEntradaRaio = cargaEntregaAntiga.DataEntradaRaio;
            cargaEntregaNova.DataSaidaRaio = cargaEntregaAntiga.DataSaidaRaio;
            cargaEntregaNova.DataPrevista = cargaEntregaAntiga.DataPrevista;
            //cargaEntregaNova.DataInicioEntregaReprogramada = cargaEntregaAntiga.DataInicioEntregaReprogramada;
            cargaEntregaNova.DataReprogramada = cargaEntregaAntiga.DataReprogramada;
            cargaEntregaNova.DistanciaAteDestino = cargaEntregaAntiga.DistanciaAteDestino;
            cargaEntregaNova.PossuiPedidosReentrega = cargaEntregaAntiga.PossuiPedidosReentrega;
            cargaEntregaNova.IdTrizy = cargaEntregaAntiga.IdTrizy;
            cargaEntregaNova.CargaEntregaFinalizacaoAssincrona = cargaEntregaAntiga.CargaEntregaFinalizacaoAssincrona;
            cargaEntregaNova.CargaEntregaQualidade = cargaEntregaAntiga.CargaEntregaQualidade;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            repCargaEntrega.Atualizar(cargaEntregaNova);

            Servicos.Auditoria.Auditoria.TrocarAuditoria(cargaEntregaAntiga, cargaEntregaNova, unitOfWork);

            unitOfWork.Flush();

            int total = parametros.AlertasMonitor.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento repositorioPerdaSinalMonitoramento = new Repositorio.Embarcador.Logistica.PerdaSinalMonitoramento(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.AlertasMonitor[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaMonitorAntigo = parametros.AlertasMonitor[i];
                        Dominio.Entidades.Embarcador.Logistica.AlertaMonitor novo = alertaMonitorAntigo.Clonar<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repAlertaMonitor.Inserir(novo);

                        //refazendo a perda sinal monitoramento
                        for (int j = 0; j < parametros.PerdasSinalMonitoramento.Count; j++)
                        {
                            if (parametros.PerdasSinalMonitoramento[j].AlertaMonitor.Codigo == alertaMonitorAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento perdaSinalMonitoramentoNovo = parametros.PerdasSinalMonitoramento[j].Clonar<Dominio.Entidades.Embarcador.Logistica.PerdaSinalMonitoramento>();
                                perdaSinalMonitoramentoNovo.AlertaMonitor = novo;
                                repositorioPerdaSinalMonitoramento.Inserir(perdaSinalMonitoramentoNovo);
                            }
                        }
                    }
                }
            }

            total = parametros.CargaEntregasAssinaturaResponsavel.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel repCargaEntregaAssinaturaResponsavel = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregasAssinaturaResponsavel[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel novo = parametros.CargaEntregasAssinaturaResponsavel[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAssinaturaResponsavel>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaEntregaAssinaturaResponsavel.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregasCheckList.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregasCheckList[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList novo = parametros.CargaEntregasCheckList[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaEntregaCheckList.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregasFoto.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregasFoto[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto novo = parametros.CargaEntregasFoto[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaEntregaFoto.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregasFotoNotaFiscal.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal repCargaEntregaFotoNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregasFotoNotaFiscal[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal novo = parametros.CargaEntregasFotoNotaFiscal[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFotoNotaFiscal>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaEntregaFotoNotaFiscal.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregasGuiaTransporteAnimal.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal repCargaEntregaGuiaTransporteAnimal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregasGuiaTransporteAnimal[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal novo = parametros.CargaEntregasGuiaTransporteAnimal[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaGuiaTransporteAnimal>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaEntregaGuiaTransporteAnimal.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaOcorrencias.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaOcorrencias[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia novo = parametros.CargaOcorrencias[i].Clonar<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaOcorrencia.Inserir(novo);
                    }
                }
            }

            Dictionary<int, Dominio.Entidades.Embarcador.Chamados.Chamado> cargaEntregaChamados = new();
            total = parametros.Chamados.Count;
            if (total > 0)
            {
                for (int i = 0; i < total; i++)
                {
                    if (parametros.Chamados[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Chamados.Chamado chamadoAntigo = parametros.Chamados[i];
                        Dominio.Entidades.Embarcador.Chamados.Chamado novo = chamadoAntigo.Clonar<Dominio.Entidades.Embarcador.Chamados.Chamado>();
                        novo.Analistas = chamadoAntigo.Analistas.ToList();
                        novo.Anexos = chamadoAntigo.Anexos.ToList();
                        novo.PerfisAcesso = chamadoAntigo.PerfisAcesso.ToList();
                        novo.Datas = chamadoAntigo.Datas.ToList();
                        novo.RegrasAnalise = chamadoAntigo.RegrasAnalise.ToList();
                        novo.XMLNotasFiscais = chamadoAntigo.XMLNotasFiscais.ToList();
                        novo.CargaEntrega = cargaEntregaNova;
                        repChamado.Inserir(novo);
                        cargaEntregaChamados[chamadoAntigo.Codigo] = novo;

                        Servicos.Auditoria.Auditoria.TrocarAuditoria(chamadoAntigo, novo, unitOfWork);

                        //refazendo o chamado analise
                        Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                        for (int j = 0; j < parametros.ChamadosAnalise.Count; j++)
                        {
                            if (parametros.ChamadosAnalise[j].Chamado?.Codigo == chamadoAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise analiseNovo = parametros.ChamadosAnalise[j].Clonar<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise>();
                                analiseNovo.Chamado = novo;
                                repChamadoAnalise.Inserir(analiseNovo);
                            }
                        }

                        //refazendo o chamado ocorrência
                        Repositorio.Embarcador.Chamados.ChamadoOcorrencia repChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);
                        for (int j = 0; j < parametros.ChamadosOcorrencias.Count; j++)
                        {
                            if (parametros.ChamadosOcorrencias[j].Chamado.Codigo == chamadoAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrenciaNovo = parametros.ChamadosOcorrencias[j].Clonar<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();
                                chamadoOcorrenciaNovo.Chamado = novo;
                                repChamadoOcorrencia.Inserir(chamadoOcorrenciaNovo);
                            }
                        }

                        //refazendo o chamado anexo
                        Repositorio.Embarcador.Chamados.ChamadoAnexo repositorioChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);
                        for (int j = 0; j < parametros.ChamadosAnexos.Count; j++)
                        {
                            if (parametros.ChamadosAnexos[j].Chamado.Codigo == chamadoAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexoNovo = parametros.ChamadosAnexos[j].Clonar<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();
                                chamadoAnexoNovo.Chamado = novo;
                                repositorioChamadoAnexo.Inserir(chamadoAnexoNovo);
                            }
                        }

                        //refazendo o chamado data
                        Repositorio.Embarcador.Chamados.ChamadoData repositorioChamadoData = new Repositorio.Embarcador.Chamados.ChamadoData(unitOfWork);
                        for (int j = 0; j < parametros.ChamadosDatas.Count; j++)
                        {
                            if (parametros.ChamadosDatas[j].Chamado.Codigo == chamadoAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Chamados.ChamadoData chamadoDataNovo = parametros.ChamadosDatas[j].Clonar<Dominio.Entidades.Embarcador.Chamados.ChamadoData>();
                                chamadoDataNovo.Chamado = novo;
                                repositorioChamadoData.Inserir(chamadoDataNovo);
                            }
                        }

                        //refazendo o chamado informação fechamento
                        Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento repositorioChamadoInformacaoFechamento = new Repositorio.Embarcador.Chamados.ChamadoInformacaoFechamento(unitOfWork);
                        for (int j = 0; j < parametros.ChamadosInformacoesFechamento.Count; j++)
                        {
                            if (parametros.ChamadosInformacoesFechamento[j].Chamado.Codigo == chamadoAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento chamadoInformacaoFechamentoNovo = parametros.ChamadosInformacoesFechamento[j].Clonar<Dominio.Entidades.Embarcador.Chamados.ChamadoInformacaoFechamento>();
                                chamadoInformacaoFechamentoNovo.Chamado = novo;
                                repositorioChamadoInformacaoFechamento.Inserir(chamadoInformacaoFechamentoNovo);
                            }
                        }

                        //refazendo o chamado nível atendimento
                        Repositorio.Embarcador.Chamados.NivelAtendimento repositorioNivelAtendimento = new Repositorio.Embarcador.Chamados.NivelAtendimento(unitOfWork);
                        for (int j = 0; j < parametros.ChamadosNiveisAtendimento.Count; j++)
                        {
                            if (parametros.ChamadosNiveisAtendimento[j].Chamado.Codigo == chamadoAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Chamados.NivelAtendimento chamadoNivelAtendimentoNovo = parametros.ChamadosNiveisAtendimento[j].Clonar<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
                                chamadoNivelAtendimentoNovo.Chamado = novo;
                                repositorioNivelAtendimento.Inserir(chamadoNivelAtendimentoNovo);
                            }
                        }
                    }
                }
            }

            total = parametros.MonitoramentoNotificacoesApp.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp repMonitoramentoNotificacoesApp = new Repositorio.Embarcador.TorreControle.MonitoramentoNotificacoesApp(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.MonitoramentoNotificacoesApp[i].Chamado.CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp monitoramentoNotificacaoAppAntigo = parametros.MonitoramentoNotificacoesApp[i];
                        Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp monitoramentoNotificacaoAppNovo = monitoramentoNotificacaoAppAntigo.Clonar<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>();
                        Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCargaEntrega(cargaEntregaNova.Codigo);

                        monitoramentoNotificacaoAppNovo.Chamado = chamado;
                        repMonitoramentoNotificacoesApp.Inserir(monitoramentoNotificacaoAppNovo);
                    }
                }
            }

            total = parametros.gestaoDadosColetas.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.gestaoDadosColetas[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoEntregaAntigo = parametros.gestaoDadosColetas[i];
                        Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta novo = gestaoEntregaAntigo.Clonar<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repGestaoDadosColeta.Inserir(novo);

                        //refazendo o GestaoDadosColetaDadosNFe
                        Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repGestaoDadosColetaDadosNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(unitOfWork);
                        for (int j = 0; j < parametros.GestaoDadosColetaDadosNFe.Count; j++)
                        {
                            if (parametros.GestaoDadosColetaDadosNFe[j].GestaoDadosColeta?.Codigo == gestaoEntregaAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFeNovo = parametros.GestaoDadosColetaDadosNFe[j].Clonar<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe>();
                                gestaoDadosColetaDadosNFeNovo.GestaoDadosColeta = novo;
                                repGestaoDadosColetaDadosNFe.Inserir(gestaoDadosColetaDadosNFeNovo);
                            }
                        }

                        //refazendo o GestaoDadosColetaDadosTransporte
                        Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte repGestaoDadosColetaDadosTransporte = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte(unitOfWork);
                        for (int j = 0; j < parametros.GestaoDadosColetaDadosTransporte.Count; j++)
                        {
                            if (parametros.GestaoDadosColetaDadosTransporte[j].GestaoDadosColeta.Codigo == gestaoEntregaAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporteNovo = parametros.GestaoDadosColetaDadosTransporte[j].Clonar<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte>();
                                gestaoDadosColetaDadosTransporteNovo.GestaoDadosColeta = novo;
                                repGestaoDadosColetaDadosTransporte.Inserir(gestaoDadosColetaDadosTransporteNovo);
                            }
                        }

                        //refazendo o chamado anexo
                        Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao repGestaoDadosColetaIntegracao = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao(unitOfWork);
                        for (int j = 0; j < parametros.GestaoDadosColetaIntegracao.Count; j++)
                        {
                            if (parametros.GestaoDadosColetaIntegracao[j].GestaoDadosColeta.Codigo == gestaoEntregaAntigo.Codigo)
                            {
                                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao gestaoDadosColetaIntegracaoNovo = parametros.GestaoDadosColetaIntegracao[j].Clonar<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao>();
                                gestaoDadosColetaIntegracaoNovo.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracaoArquivo>();
                                gestaoDadosColetaIntegracaoNovo.GestaoDadosColeta = novo;

                                repGestaoDadosColetaIntegracao.Inserir(gestaoDadosColetaIntegracaoNovo);
                            }
                        }
                    }
                }
            }

            total = parametros.OcorrenciasColetaEntregas.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.OcorrenciasColetaEntregas[i].CargaEntrega?.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega novo = parametros.OcorrenciasColetaEntregas[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repOcorrenciaColetaEntrega.Inserir(novo);
                    }
                }
            }

            total = parametros.IntegracaoSuperApp.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.SuperApp.IntegracaoSuperApp repIntegracaoSuperApp = new Repositorio.Embarcador.SuperApp.IntegracaoSuperApp(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.IntegracaoSuperApp[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp novo = parametros.IntegracaoSuperApp[i].Clonar<Dominio.Entidades.Embarcador.SuperApp.IntegracaoSuperApp>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repIntegracaoSuperApp.Inserir(novo);
                    }
                }
            }

            Dictionary<int, Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes = new();
            total = parametros.PermanenciasClientes.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.PermanenciasClientes[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaClienteAntigo = parametros.PermanenciasClientes[i];
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente novo = permanenciaClienteAntigo.Clonar<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repPermanenciaCliente.Inserir(novo);
                        permanenciasClientes[permanenciaClienteAntigo.Codigo] = novo;
                    }
                }
            }

            total = parametros.MonitoramentoHistoricoStatusViagemPermanenciaCliente.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente repositorioMonitoramentoHistoricoStatusViagemPermanenciaCliente = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.MonitoramentoHistoricoStatusViagemPermanenciaCliente[i].PermanenciaCliente.CargaEntrega.Codigo == cargaEntregaAntiga.Codigo && permanenciasClientes.ContainsKey(parametros.MonitoramentoHistoricoStatusViagemPermanenciaCliente[i].PermanenciaCliente.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente novo = parametros.MonitoramentoHistoricoStatusViagemPermanenciaCliente[i].Clonar<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaCliente>();
                        novo.PermanenciaCliente = permanenciasClientes[parametros.MonitoramentoHistoricoStatusViagemPermanenciaCliente[i].PermanenciaCliente.Codigo];
                        repositorioMonitoramentoHistoricoStatusViagemPermanenciaCliente.Inserir(novo);
                    }
                }
            }

            Dictionary<int, Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubareas = new();
            total = parametros.PermanenciasSubareas.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.PermanenciasSubareas[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubareaAntigo = parametros.PermanenciasSubareas[i];
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea novo = permanenciaSubareaAntigo.Clonar<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea>();
                        novo.CargaEntrega = cargaEntregaNova;
                        repPermanenciaSubarea.Inserir(novo);
                        permanenciasSubareas[permanenciaSubareaAntigo.Codigo] = novo;
                    }
                }
            }

            total = parametros.MonitoramentoHistoricoStatusViagemPermanenciaSubArea.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea repositorioMonitoramentoHistoricoStatusViagemPermanenciaSubArea = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.MonitoramentoHistoricoStatusViagemPermanenciaSubArea[i].PermanenciaSubarea.CargaEntrega.Codigo == cargaEntregaAntiga.Codigo && permanenciasSubareas.ContainsKey(parametros.MonitoramentoHistoricoStatusViagemPermanenciaSubArea[i].PermanenciaSubarea.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea novo = parametros.MonitoramentoHistoricoStatusViagemPermanenciaSubArea[i].Clonar<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagemPermanenciaSubArea>();
                        novo.PermanenciaSubarea = permanenciasSubareas[parametros.MonitoramentoHistoricoStatusViagemPermanenciaSubArea[i].PermanenciaSubarea.Codigo];
                        repositorioMonitoramentoHistoricoStatusViagemPermanenciaSubArea.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEventos.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEventos[i].CargaEntrega != null && parametros.CargaEventos[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento novo = parametros.CargaEventos[i].Clonar<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento>();
                        novo.Chamado = null; //chamado pode nao existir;
                        novo.CargaEntrega = cargaEntregaNova;
                        repCargaEvento.Inserir(novo);
                    }
                }
            }

            total = parametros.GestaoDevolucaoNFeTransferenciaPallet.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet repGestaoDevolucaoNFeTransferenciaPallet = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.GestaoDevolucaoNFeTransferenciaPallet[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet novo = parametros.GestaoDevolucaoNFeTransferenciaPallet[i].Clonar<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNFeTransferenciaPallet>();
                        repGestaoDevolucaoNFeTransferenciaPallet.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregaNotaFiscalChamado.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotas = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntregaNova.Codigo);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado repCargaEntregaNotaFiscalChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregaNotaFiscalChamado[i].CargaEntregaNotaFiscal.CargaEntrega.Codigo == cargaEntregaAntiga.Codigo && cargaEntregaChamados.ContainsKey(parametros.CargaEntregaNotaFiscalChamado[i].Chamado.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = cargaEntregaNotas.Find(n => n.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo == parametros.CargaEntregaNotaFiscalChamado[i].CargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo);

                        if (cargaEntregaNotaFiscal == null)
                            continue;

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado novo = parametros.CargaEntregaNotaFiscalChamado[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalChamado>();
                        novo.CargaEntregaNotaFiscal = cargaEntregaNotaFiscal;
                        novo.Chamado = cargaEntregaChamados[parametros.CargaEntregaNotaFiscalChamado[i].Chamado.Codigo];
                        repCargaEntregaNotaFiscalChamado.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregaProdutoChamado.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado repCargaEntregaProdutoChamado = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregaProdutoChamado[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo && cargaEntregaChamados.ContainsKey(parametros.CargaEntregaProdutoChamado[i].Chamado.Codigo))
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado novo = parametros.CargaEntregaProdutoChamado[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado>();
                        novo.CargaEntrega = cargaEntregaNova;
                        novo.Chamado = cargaEntregaChamados[parametros.CargaEntregaProdutoChamado[i].Chamado.Codigo];
                        repCargaEntregaProdutoChamado.Inserir(novo);
                    }
                }
            }

            total = parametros.CargaEntregasNFeDevolucao.Count;
            if (total > 0)
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    if (parametros.CargaEntregasNFeDevolucao[i].CargaEntrega.Codigo == cargaEntregaAntiga.Codigo)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao novo = parametros.CargaEntregasNFeDevolucao[i].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao>();
                        novo.CargaEntrega = cargaEntregaNova;
                        novo.Chamado = null;
                        repCargaEntregaNFeDevolucao.Inserir(novo);

                        //refazendo o Controle Nota Devolução
                        Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao repControleNotaDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao(unitOfWork);
                        for (int j = 0; j < parametros.ControleNotaDevolucao.Count; j++)
                        {
                            if (parametros.ControleNotaDevolucao[j].CargaEntregaNFeDevolucao.CargaEntrega.Codigo == cargaEntregaAntiga.Codigo && cargaEntregaChamados.ContainsKey(parametros.ControleNotaDevolucao[j].Chamado.Codigo))
                            {
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao novoControleNotaDevolucao = parametros.ControleNotaDevolucao[j].Clonar<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao>();
                                novoControleNotaDevolucao.CargaEntregaNFeDevolucao = novo;
                                novoControleNotaDevolucao.Chamado = cargaEntregaChamados[parametros.ControleNotaDevolucao[j].Chamado.Codigo];
                                repControleNotaDevolucao.Inserir(novoControleNotaDevolucao);
                            }
                        }
                    }
                }
            }
        }

        public static void GerarOcorrenciaEstadia(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.RotaFrete repRota = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.RotaFreteFronteira repRotaFreteFronteira = new Repositorio.RotaFreteFronteira(unitOfWork);

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Dominio.Entidades.RotaFrete rota = repRota.BuscarPorCodigo(carga.Rota?.Codigo ?? 0);
            if (rota == null)
                return;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloCobrancaEstadiaTracking modelo = carga.TipoOperacao.ModeloCobrancaEstadiaTracking.HasValue ? carga.TipoOperacao.ModeloCobrancaEstadiaTracking.Value : ModeloCobrancaEstadiaTracking.PorEtapa;
            int tempoMinimo = carga.TipoOperacao?.TempoMinimoCobrancaEstadia ?? 0;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            if (cargaEntregas == null && cargaEntregas.Count < 0)
                return;

            double totalHorasAcumulada = 0;
            double totalFreeTimeAcumulada = 0;
            Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido parqueamento = rota.PontoPassagemPreDefinido?.Where(p => p.LocalDeParqueamento).FirstOrDefault();
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
            {
                if (cargaEntrega.DataConfirmacao.HasValue && cargaEntrega.DataInicio.HasValue)
                {
                    DateTime dataInicial = cargaEntrega.DataInicio.Value;
                    DateTime dataFinal = cargaEntrega.DataConfirmacao.Value;
                    if ((carga.TipoOperacao?.ConfiguracaoFreeTime?.ConsiderarDatasDePrevisaoDoPedidoParaEstadia ?? false) && cargaEntrega.DataPrevista.HasValue && cargaEntrega.DataPrevista.Value > cargaEntrega.DataInicio.Value && cargaEntrega.TipoCargaEntrega != TipoCargaEntrega.Fronteira)
                        dataInicial = cargaEntrega.DataPrevista.Value;

                    double horasEstadia = (cargaEntrega.DataConfirmacao.Value - dataInicial).TotalHours;
                    if (horasEstadia <= 0)
                        continue;

                    TimeSpan horasFreetime = new TimeSpan();
                    if (cargaEntrega.Parqueamento && ((parqueamento?.TempoEstimadoPermanencia ?? 0) > 0))
                    {
                        horasFreetime = TimeSpan.FromMinutes(parqueamento.TempoEstimadoPermanencia);
                    }
                    else if (cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Coleta && rota.TempoCarregamentoTicks > 0)
                    {
                        horasFreetime = rota.TempoCarregamento;
                        if (horasFreetime.Ticks <= 0)
                            continue;
                    }
                    else if (cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Entrega && rota.TempoDescargaTicks > 0)
                    {
                        horasFreetime = rota.TempoDescarga;
                        if (horasFreetime.Ticks <= 0)
                            continue;
                    }
                    else if (cargaEntrega.TipoCargaEntrega == TipoCargaEntrega.Fronteira && cargaEntrega.Cliente != null)
                    {
                        Dominio.Entidades.RotaFreteFronteira fronteira = repRotaFreteFronteira.BuscarPorRotaFreteECliente(rota.Codigo, cargaEntrega.Cliente.CPF_CNPJ);
                        if (fronteira != null && fronteira.TempoMedioPermanenciaFronteira > 0)
                            horasFreetime = TimeSpan.FromHours(fronteira.TempoMedioPermanenciaFronteira / 60);
                        else
                            continue;
                    }
                    else
                        continue;

                    if (modelo != ModeloCobrancaEstadiaTracking.PorViagem)
                    {
                        double diferencaHoras = horasEstadia - horasFreetime.TotalHours;
                        if (diferencaHoras > 0 && diferencaHoras > tempoMinimo)
                        {
                            if (modelo == ModeloCobrancaEstadiaTracking.PorEtapa)
                                GerarOcorrencia(diferencaHoras, carga, cargaEntrega, dataInicial, unitOfWork, tipoServicoMultisoftware, auditado, modelo, horasFreetime.TotalHours, dataFinal, cargaEntrega.TipoCargaEntrega, clienteMultisoftware);
                            else if (modelo == ModeloCobrancaEstadiaTracking.PorEtapaAcumulada)
                            {
                                totalFreeTimeAcumulada += horasFreetime.TotalHours;
                                totalHorasAcumulada += diferencaHoras;
                            }
                        }
                    }
                    else
                    {
                        totalFreeTimeAcumulada += horasFreetime.TotalHours;
                        totalHorasAcumulada += horasEstadia;
                    }
                }
            }
            if (modelo == ModeloCobrancaEstadiaTracking.PorEtapaAcumulada && totalHorasAcumulada > 0)
            {
                if (totalHorasAcumulada > 0 && totalHorasAcumulada > tempoMinimo)
                {
                    GerarOcorrencia(totalHorasAcumulada, carga, null, carga.DataCriacaoCarga, unitOfWork, tipoServicoMultisoftware, auditado, modelo, totalFreeTimeAcumulada, carga.DataCriacaoCarga.AddHours(totalHorasAcumulada), null, clienteMultisoftware);
                }
            }
            else if (modelo == ModeloCobrancaEstadiaTracking.PorViagem && totalHorasAcumulada > totalFreeTimeAcumulada)
            {
                double diferencaHoras = totalHorasAcumulada - totalFreeTimeAcumulada;
                if (diferencaHoras > 0 && diferencaHoras > tempoMinimo)
                {
                    GerarOcorrencia(diferencaHoras, carga, null, carga.DataCriacaoCarga, unitOfWork, tipoServicoMultisoftware, auditado, modelo, totalFreeTimeAcumulada, carga.DataCriacaoCarga.AddHours(totalHorasAcumulada), null, clienteMultisoftware);
                }
            }
        }

        private static void GerarOcorrencia(double horaEstadia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataInicial, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloCobrancaEstadiaTracking modeloEstadia, double horasFreetime, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaEntrega? tipoCargaEntrega, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            try
            {
                Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia repImportarOcorrencia = new Repositorio.Embarcador.Ocorrencias.ImportarOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros repCargaOcorrenciaParametros = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaParametros(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();

                PreencherEntidade(ref ocorrencia, horaEstadia, carga, cargaEntrega, dataInicial, unitOfWork, tipoServicoMultisoftware, modeloEstadia, horasFreetime, dataFinal, tipoCargaEntrega);

                string msgRetorno = string.Empty;
                if (ocorrencia.TipoOcorrencia == null)
                    msgRetorno += "É obrigatório selecionar o tipo de ocorrência. ";

                if (ocorrencia.ValorOcorrencia > 10000000m)
                    msgRetorno += ("O valor da ocorrência não pode ser maior que R$ 10.000.000,00. ");

                if (ocorrencia.TipoOcorrencia.ExigirInformarObservacao && string.IsNullOrWhiteSpace(ocorrencia.Observacao))
                    msgRetorno += ("É obrigatório informar uma observação. ");

                if (ocorrencia.TipoOcorrencia.OcorrenciaExclusivaParaIntegracao)
                    msgRetorno += ("Tipo de Ocorrência é excluisiva para integração. ");

                if (ocorrencia.OrigemOcorrenciaPorPeriodo && ((!ocorrencia.PeriodoInicio.HasValue || !ocorrencia.PeriodoFim.HasValue) || ocorrencia.PeriodoInicio.Value > ocorrencia.PeriodoFim.Value))
                    msgRetorno += "Período selecionado é inválido. ";

                if (string.IsNullOrWhiteSpace(msgRetorno))
                {
                    ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia.OrigemOcorrencia;
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = null;
                    if (cargaEntrega != null)
                        cargaCTEs = repCargaEntregaNotaFiscal.BuscarCargaCTePorCargaEntrega(cargaEntrega.Codigo);

                    if (cargaCTEs == null || cargaCTEs.Count == 0)
                        cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, true);

                    if (cargaCTEs == null || cargaCTEs.Count == 0)
                        msgRetorno = "Nenhum CT-e localizado. ";

                    if (cargaCTEs.Any(obj => obj.CargaCTeFilialEmissora != null))
                        ocorrencia.EmiteComplementoFilialEmissora = true;

                    ocorrencia.IncluirICMSFrete = cargaCTEs.Count > 0 ? (cargaCTEs.FirstOrDefault().CTe.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false) : false;

                    string codigoCFOPIntegracao = string.Empty;
                    if (ocorrencia != null && ocorrencia.TipoOcorrencia != null && cargaCTEs != null && cargaCTEs.Count > 0)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (from o in cargaCTEs where o.CTe != null && o.CargaCTeComplementoInfo == null select o.CTe).FirstOrDefault();

                        if (cte != null && cte.LocalidadeInicioPrestacao.Estado.Sigla == cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                        {
                            if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento))
                                codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadualIsento;
                            else
                                codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;

                        }
                        else if (cte != null)
                        {
                            if (cte.ValorICMS == 0 && !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento))
                                codigoCFOPIntegracao = ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadualIsento;
                            else
                                codigoCFOPIntegracao = !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPInterestadual : !string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual) ? ocorrencia.TipoOcorrencia.CodigoIntegracaoCFOPEstadual : string.Empty;
                        }

                    }

                    if (!string.IsNullOrWhiteSpace(codigoCFOPIntegracao))
                        ocorrencia.CFOP = codigoCFOPIntegracao;

                    if (ocorrencia.TipoOcorrencia.DataOcorrenciaIgualDataCTeComplementado)
                        ocorrencia.DataOcorrencia = cargaCTEs.FirstOrDefault()?.CTe.DataEmissao ?? ocorrencia.DataOcorrencia;

                    srvOcorrencia.SetaEmitenteOcorrencia(ref ocorrencia, unitOfWork, tipoServicoMultisoftware, carga.Operador);

                    string msgRetornoOcorrencia = Servicos.Embarcador.Ocorrencia.Ocorrencia.ValidarDadosOcorrencia(ocorrencia, null, tipoServicoMultisoftware, cargaCTEs, unitOfWork.StringConexao, unitOfWork).Result;

                    if (!string.IsNullOrWhiteSpace(msgRetornoOcorrencia))
                        msgRetorno += msgRetornoOcorrencia + " ";

                    if (string.IsNullOrWhiteSpace(msgRetorno))
                    {
                        repOcorrencia.Inserir(ocorrencia);

                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros cargaOcorrencaParametros = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaParametros
                        {
                            CargaOcorrencia = ocorrencia,
                            ParametroOcorrencia = repParametroOcorrencia.BuscarPorTipo(TipoParametroOcorrencia.Periodo),
                            DataInicio = dataInicial,
                            DataFim = dataInicial.AddHours(horaEstadia),
                            TotalHoras = (decimal)horaEstadia
                        };
                        repCargaOcorrenciaParametros.Inserir(cargaOcorrencaParametros);

                        ocorrencia.ValorOcorrencia = CalcularValorOcorrencia(ocorrencia, horaEstadia, dataInicial, carga, cargaEntrega, unitOfWork, tipoServicoMultisoftware, out string msgRetornoCalculo);
                        if (ocorrencia.ValorOcorrencia <= 0m)
                            ocorrencia.ValorOcorrencia = 0.01m;

                        if (!string.IsNullOrWhiteSpace(msgRetornoCalculo))
                            Servicos.Auditoria.Auditoria.Auditar(auditado, ocorrencia, msgRetornoCalculo, unitOfWork);

                        Servicos.Embarcador.Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(ocorrencia, cargaCTEs, unitOfWork);

                        if (ocorrencia.TipoOcorrencia.OcorrenciaPorQuantidade)
                            ocorrencia.ValorOcorrencia = ocorrencia.Quantidade * ocorrencia.TipoOcorrencia.Valor;

                        string mensagemRetorno = string.Empty;
                        if (!srvOcorrencia.FluxoGeralOcorrencia(ref ocorrencia, cargaCTEs, null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, carga.Operador, configuracao, clienteMultisoftware, "", true))
                            msgRetorno += mensagemRetorno + " ";

                        ocorrencia.ValorOriginalOcorrencia = ocorrencia.ValorOcorrencia;

                        repOcorrencia.Atualizar(ocorrencia);
                    }
                }
                if (!string.IsNullOrWhiteSpace(msgRetorno))
                    Log.TratarErro(msgRetorno, "OcorrenciaEstadia");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "OcorrenciaEstadia");
            }
        }

        private static void PreencherEntidade(ref Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, double totalHoraEstadia, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataInicial, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModeloCobrancaEstadiaTracking modeloEstadia, double horasFreetime, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCargaEntrega? tipoCargaEntrega)
        {
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = null;
            if (cargaEntrega != null)
                cargaCTEs = repCargaEntregaNotaFiscal.BuscarCargaCTePorCargaEntrega(cargaEntrega.Codigo);

            if (cargaCTEs == null)
                cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, true);

            int codigoEmpresa = cargaCTEs?.FirstOrDefault()?.CTe?.Empresa?.Codigo ?? 0;

            TimeSpan tsEstadia = TimeSpan.FromHours(totalHoraEstadia);
            TimeSpan tsFreetime = TimeSpan.FromHours(horasFreetime);

            ocorrencia.CTeTerceiro = null;
            ocorrencia.Veiculo = null;
            ocorrencia.Carga = carga;
            ocorrencia.Responsavel = null;
            ocorrencia.Quantidade = 0;
            ocorrencia.ComponenteFrete = repComponenteFrete.BuscarPorCodigo(carga.TipoOperacao.ComponenteFrete?.Codigo ?? 0);
            ocorrencia.TipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(carga.TipoOperacao.TipoOcorrencia?.Codigo ?? 0);
            ocorrencia.PercentualAcresciomoValor = ocorrencia.TipoOcorrencia.PercentualAcrescimo;
            ocorrencia.Tomador = null;
            ocorrencia.DataOcorrencia = DateTime.Now;
            ocorrencia.DataEvento = DateTime.Now;
            ocorrencia.PeriodoInicio = dataInicial;
            ocorrencia.PeriodoFim = dataInicial.AddHours(totalHoraEstadia);
            ocorrencia.ParametroDataInicial = dataInicial;
            ocorrencia.ParametroDataFim = dataInicial.AddHours(totalHoraEstadia);
            if (modeloEstadia == ModeloCobrancaEstadiaTracking.PorEtapa)
            {
                ocorrencia.Observacao = "Geração automática pela estadia/diária pelo modelo de cálculo " + modeloEstadia.Descricao() + "." +
                    " Etapa: " + (tipoCargaEntrega.HasValue ? tipoCargaEntrega.Value.ObterDescricao() : "Coleta") + "." +
                    " Inicio: " + dataInicial.ToString("dd/MM/yyyy HH:mm") + "." +
                    " Término: " + dataFinal.ToString("dd/MM/yyyy HH:mm") + "." +
                    " Total de Horas: " + tsEstadia.TotalHours.ToString() + "." +
                    " Freetime: " + tsFreetime.TotalHours.ToString();
            }
            else
                ocorrencia.Observacao = "Geração automática pela estadia/diária pelo modelo de cálculo " + modeloEstadia.Descricao() + ". Total de Horas Acumulada: " + (tsEstadia.TotalHours + tsFreetime.TotalHours).ToString() + " Total de Freetime Acumulado: " + tsFreetime.TotalHours.ToString();

            ocorrencia.NumeroOcorrenciaCliente = "";
            ocorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            ocorrencia.DataAlteracao = DateTime.Now;
            ocorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
            ocorrencia.DataInicialEstadia = dataInicial;
            ocorrencia.DataFinalEstadia = dataFinal;
            if (modeloEstadia == ModeloCobrancaEstadiaTracking.PorEtapa)
                ocorrencia.HorasEstadia = totalHoraEstadia;
            else
                ocorrencia.HorasEstadia = totalHoraEstadia + horasFreetime;
            ocorrencia.HorasFreetime = horasFreetime;
            ocorrencia.OcorrenciaDeEstadia = true;
            ocorrencia.ModeloCobrancaEstadiaTracking = modeloEstadia;
            ocorrencia.TipoCargaEntrega = tipoCargaEntrega;

            ocorrencia.ObservacaoCTe = "Realizado pelo modelo " + modeloEstadia.Descricao() + " e calculado a partir das horas " + tsEstadia.TotalHours.ToString();

            ocorrencia.Usuario = carga.Operador;
            ocorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada;
            ocorrencia.ValorICMS = 0m;
            ocorrencia.BaseCalculoICMS = 0m;
            ocorrencia.AliquotaICMS = 0m;
            ocorrencia.DTNatura = null;
            ocorrencia.ComplementoValorFreteCarga = ocorrencia.TipoOcorrencia?.OcorrenciaComplementoValorFreteCarga ?? false;
            ocorrencia.NomeRecebedor = "";
            ocorrencia.TipoDocumentoRecebedor = "";
            ocorrencia.NumeroDocumentoRecebedor = "";
            ocorrencia.NotificarDebitosAtivos = false;
            ocorrencia.CTeEmitidoNoEmbarcador = false;
            ocorrencia.ContratoFrete = null;
            ocorrencia.Filial = null;
            ocorrencia.NaoGerarDocumento = ocorrencia.TipoOcorrencia?.NaoGerarDocumento ?? false;
            ocorrencia.Cargas = srvOcorrencia.BuscarCargasDoPeriodoSelecionado(ocorrencia, unitOfWork, tipoServicoMultisoftware, carga.Operador, codigoEmpresa, 0);
            ocorrencia.OrigemOcorrencia = ocorrencia.TipoOcorrencia.OrigemOcorrencia;

            if (ocorrencia.TipoOcorrencia.TomadorTipoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia.Outros && ocorrencia.Tomador == null && ocorrencia.TipoOcorrencia.OutroTomador != null)
            {
                ocorrencia.Responsavel = Dominio.Enumeradores.TipoTomador.Outros;
                ocorrencia.Tomador = ocorrencia.TipoOcorrencia.OutroTomador;
            }

            ocorrencia.ModeloDocumentoFiscal = null;// cargaCTEs?.FirstOrDefault()?.CTe?.ModeloDocumentoFiscal ?? null;            
            ocorrencia.ValorOcorrencia = 0m;
            ocorrencia.AliquotaICMS = 0m;
            ocorrencia.ValorOcorrenciaOriginal = 0m;
            if (ocorrencia.ComponenteFrete != null && ocorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                ocorrencia.ValorOcorrenciaLiquida = ocorrencia.ValorOcorrencia;

            bool destacarComponenteTabelaFrete = Servicos.Embarcador.Carga.Frete.DestacarComponenteTabelaFrete(carga.TabelaFrete, ocorrencia.ComponenteFrete);
            if (ocorrencia.ComponenteFrete != null && (destacarComponenteTabelaFrete ? carga.TabelaFrete.DescontarComponenteFreteLiquido : ocorrencia.ComponenteFrete.DescontarComponenteFreteLiquido))
                ocorrencia.ValorOcorrenciaLiquida = ocorrencia.ValorOcorrencia;
        }

        private static decimal CalcularValorOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, double totalHoraEstadia, DateTime dataInicial, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string msgRetorno)
        {
            try
            {
                msgRetorno = string.Empty;
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();
                Servicos.Embarcador.Carga.Ocorrencia servicoOcorrenciaCalculoFrete = new Servicos.Embarcador.Carga.Ocorrencia();
                Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoGatilhoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia repParametroOcorrencia = new Repositorio.Embarcador.Ocorrencias.ParametroOcorrencia(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                LocalFreeTime? localFreeTime = null;
                if (ocorrencia.TipoCargaEntrega.HasValue)
                {
                    if (ocorrencia.TipoCargaEntrega == TipoCargaEntrega.Coleta)
                        localFreeTime = LocalFreeTime.Coleta;
                    else if (ocorrencia.TipoCargaEntrega == TipoCargaEntrega.Fronteira)
                        localFreeTime = LocalFreeTime.Fronteira;
                    else
                        localFreeTime = LocalFreeTime.Entrega;
                }

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia parametrosCalcularValorOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.ParametroCalcularValorOcorrencia()
                {
                    ApenasReboque = false,
                    CodigoCarga = carga.Codigo,
                    CodigoParametroBooleano = 0,
                    CodigoParametroInteiro = 0,
                    CodigoParametroPeriodo = repParametroOcorrencia.BuscarPorTipo(TipoParametroOcorrencia.Periodo)?.Codigo ?? 0,
                    CodigoTipoOcorrencia = ocorrencia.TipoOcorrencia?.Codigo ?? 0,
                    DataFim = ocorrencia.ParametroDataFim,
                    DataInicio = ocorrencia.ParametroDataInicial,
                    ParametroData = ocorrencia.ParametroDataInicial,
                    Minutos = 0,
                    HorasSemFranquia = ocorrencia.TipoOcorrencia?.HorasSemFranquia ?? 0,
                    KmInformado = 0,
                    PermiteInformarValor = false,
                    ValorOcorrencia = 0m,
                    LocalFreeTime = localFreeTime
                };

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoDeOcorrencia.BuscarPorCodigo(parametrosCalcularValorOcorrencia.CodigoTipoOcorrencia);

                Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilho = repGatilhoGeracaoAutomaticaOcorrencia.BuscarPorTipoOcorrencia(parametrosCalcularValorOcorrencia.CodigoTipoOcorrencia);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs = null;
                if (cargaEntrega != null)
                    cargaCTEs = repCargaEntregaNotaFiscal.BuscarCargaCTePorCargaEntrega(cargaEntrega.Codigo);

                if (cargaCTEs == null || cargaCTEs?.Count == 0)
                    cargaCTEs = repCargaCTe.BuscarPorCarga(carga.Codigo, true);

                if (gatilho != null && carga != null)
                {
                    parametrosCalcularValorOcorrencia.DeducaoHoras = servicoGatilhoOcorrencia.ObterHorasDeducaoPorGatilho(carga, gatilho);

                    if (!gatilho.GerarAutomaticamente)
                    {
                        (DateTime? DataInicio, DateTime? DataFim) dados = servicoGatilhoOcorrencia.ObterDataInicioEFimGatilho(parametrosCalcularValorOcorrencia.ParametroData, parametrosCalcularValorOcorrencia.DataInicio, parametrosCalcularValorOcorrencia.DataFim);

                        if (dados.DataInicio.HasValue && dados.DataFim.HasValue)
                        {
                            parametrosCalcularValorOcorrencia.DataInicio = dados.DataInicio.Value;
                            parametrosCalcularValorOcorrencia.DataFim = dados.DataFim.Value;
                        }
                    }
                }

                parametrosCalcularValorOcorrencia.ListaCargaCTe = cargaCTEs;
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.CalculoFreteOcorrencia calculoFreteOcorrencia = servicoOcorrenciaCalculoFrete.CalcularValorOcorrencia(parametrosCalcularValorOcorrencia, unitOfWork, configuracaoTMS, tipoServicoMultisoftware);

                return Math.Round(calculoFreteOcorrencia.ValorOcorrencia, 2, MidpointRounding.ToEven);
            }
            catch (ServicoException excecao)
            {
                msgRetorno = excecao.Message;
                Log.TratarErro(excecao.Message, "OcorrenciaEstadia");
                return 0.01m;
            }
            catch (Exception excecao)
            {
                msgRetorno = Localization.Resources.Cargas.ControleEntrega.OcorreuFalhaCalcularValorOcorrencia;
                Servicos.Log.TratarErro(excecao, "OcorrenciaEstadia");
                return 0.01m;
            }
        }

        private static void EnviarOcorrenciaParaVendedor(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> produtos = null)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            int total = cargaEntregaPedidos.Count();
            if (total > 0)
            {
                List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens = new List<Dominio.ObjetosDeValor.Email.Mensagem>();
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedidos[i].CargaPedido.Pedido;
                    Dominio.Entidades.Usuario vendedor = pedido.FuncionarioVendedor;
                    if (vendedor != null && !string.IsNullOrWhiteSpace(vendedor.Email) && vendedor.NotificarOcorrenciaEntrega)
                    {

                        DateTime data = ((cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado) ? cargaEntrega.DataRejeitado : cargaEntrega.DataFim) ?? DateTime.Now;
                        string situacaoEntrega = cargaEntrega.MotivoRejeicao != null ? "Rejeitado" : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaHelper.ObterDescricao(cargaEntrega.Situacao);

                        string assunto = $@"[Evento de Entrega] Pedido {pedido.NumeroPedidoEmbarcador} para {pedido.Destinatario.Descricao}";

                        List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscais = repNotaFiscal.BuscarPorCargaPedido(cargaEntregaPedidos[i].CargaPedido.Codigo);
                        string numerosNotasFiscais = string.Join(", ", (from obj in notasFiscais select obj.XMLNotaFiscal.Numero + "/" + obj.XMLNotaFiscal.Serie).ToList().ToArray());

                        List<KeyValuePair<string, string>> linhas = new List<KeyValuePair<string, string>>();
                        linhas.Add(new KeyValuePair<string, string>("Entrega", situacaoEntrega));
                        linhas.Add(new KeyValuePair<string, string>("Nº Carga", cargaEntrega.Carga.CodigoCargaEmbarcador));
                        linhas.Add(new KeyValuePair<string, string>("Nº Pedido", pedido.NumeroPedidoEmbarcador));
                        linhas.Add(new KeyValuePair<string, string>("N° Notas", numerosNotasFiscais));
                        linhas.Add(new KeyValuePair<string, string>("Data e Hora", data.ToString("dd/MM/yyyy HH:mm:ss")));
                        linhas.Add(new KeyValuePair<string, string>("Cliente", pedido.Destinatario.Nome));
                        linhas.Add(new KeyValuePair<string, string>("CPF/CNPJ", pedido.Destinatario.CPF_CNPJ_Formatado));
                        linhas.Add(new KeyValuePair<string, string>("Telefone", pedido.Destinatario.Telefone1 + " / " + pedido.Destinatario.Telefone2));
                        if (cargaEntrega.MotivoRejeicao != null)
                        {
                            linhas.Add(new KeyValuePair<string, string>("Rejeição", cargaEntrega.TipoDevolucao.ObterDescricao()));
                            linhas.Add(new KeyValuePair<string, string>("Motivo", cargaEntrega.MotivoRejeicao.Descricao ?? "Sem motivo"));
                            int totalProdutos = produtos?.Count() ?? 0;
                            for (int j = 0; j < totalProdutos; j++)
                            {
                                linhas.Add(new KeyValuePair<string, string>("Produto " + (j + 1), produtos[j].Descricao + ", quantidade " + produtos[j].QuantidadeDevolucao));
                            }
                        }

                        mensagens.Add(new Dominio.ObjetosDeValor.Email.Mensagem
                        {
                            Destinatarios = new List<string> { vendedor.Email },
                            Assunto = assunto,
                            Corpo = Servicos.Email.TemplateCorpoEmail(situacaoEntrega, linhas, "Houve uma ocorrência na entrega do pedido:", null, "Controle de Entrega", "E-mail enviado automaticamente.")
                        });
                    }
                }

                Servicos.Email.EnviarMensagensAsync(mensagens, unitOfWork);
            }

        }

        private static void AtualizarStatusNotaFiscal(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscais, Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotaFiscalSituacaoGatilho gatilho, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaNotasFiscais?.Count == 0)
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao repositorioNotaFiscalSituacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscalSituacao(unitOfWork);

            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalSituacao notaFiscalSituacao = repositorioNotaFiscalSituacao.BuscarPorGatilho(gatilho);

            if (notaFiscalSituacao == null)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in listaNotasFiscais)
            {
                notaFiscal.DataNotaFiscalSituacao = DateTime.Now;
                notaFiscal.NotaFiscalSituacao = notaFiscalSituacao;
                repositorioXmlNotaFiscal.Atualizar(notaFiscal);
            }
        }

        private static void AdicionarChavesNFeNaCargaEntrega(List<string> listaChavesNfe, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            if (listaChavesNfe == null || listaChavesNfe.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChavesNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> chaves = repCargaEntregaChavesNfe.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            List<string> novasChaves = new List<string>();
            foreach (string chaveNfe in listaChavesNfe.Distinct())
            {
                if (!chaves.Any(o => o.ChaveNfe == chaveNfe))
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe cargaEntregaChaveNfe = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe()
                    {
                        CargaEntrega = cargaEntrega,
                        ChaveNfe = chaveNfe,
                        DataCriacao = DateTime.Now
                    };

                    repCargaEntregaChavesNfe.Inserir(cargaEntregaChaveNfe);
                    novasChaves.Add(chaveNfe);
                }
            }

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals = repXMLnotaFiscal.BuscarPorChaves(novasChaves);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);

            if (xMLNotaFiscals.Count > 0)
            {
                Servicos.Embarcador.Pedido.NotaFiscal serNotaFiscal = new Pedido.NotaFiscal(unitOfWork);

                for (int i = 0; i < cargaPedidos.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                    if (cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Remetente != null)
                    {
                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotasFiscaisPedido = (from obj in xMLNotaFiscals where obj.Destinatario.CPF_CNPJ == cargaPedido.Pedido.Destinatario.CPF_CNPJ && obj.Emitente.CPF_CNPJ == cargaPedido.Pedido.Remetente.CPF_CNPJ select obj).ToList();
                        if (xmlsNotasFiscaisPedido.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xmlsNotasFiscaisPedido)
                                serNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracao, false, out bool alteradoTipoDeCarga, auditado);

                            if (repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever))
                            {
                                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repositorioCargaPedidParcialxml = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);

                                bool enviouTodos = true;

                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial> cargaPedidoXMLNotaFiscalParcials = repositorioCargaPedidParcialxml.BuscarPorCargaPedido(cargaPedido.Codigo);
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial in cargaPedidoXMLNotaFiscalParcials)
                                {
                                    Servicos.Log.TratarErro($"Validando CargaPedidoParcial: - {(cargaPedidoXMLNotaFiscalParcial.Codigo)} ");

                                    if (cargaPedidoXMLNotaFiscalParcial.XMLNotaFiscal == null)
                                    {
                                        enviouTodos = false;
                                        break;
                                    }
                                }

                                if (enviouTodos)
                                {
                                    Servicos.Log.TratarErro("Finalizando envio das notas fiscais");
                                    cargaPedido.SituacaoEmissao = SituacaoNF.NFEnviada;
                                    repositorioCargaPedido.Atualizar(cargaPedido);

                                    decimal pesoNaNFs = repositorioPedidoXMLNotaFiscal.BuscarPesoPorCarga(cargaPedido.Carga.Codigo);
                                    int volumes = repositorioPedidoXMLNotaFiscal.BuscarVolumesPorCarga(cargaPedido.Carga.Codigo);
                                    string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, pesoNaNFs, volumes, null, null, null, null, null, configuracao, tipoServicoMultisoftware, auditado, null, unitOfWork);
                                    if (!string.IsNullOrWhiteSpace(retornoFinalizacao))
                                        Servicos.Log.TratarErro("Problemas ao finalizar envio das notas. Carga: " + cargaPedido.Carga.CodigoCargaEmbarcador + " erro: " + retornoFinalizacao);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FinalizarEnvioChavesNFeControleEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool finalizarEnvioDocumentosCarga, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChavesNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLnotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal serNotaFiscal = new Pedido.NotaFiscal(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> chaves = repCargaEntregaChavesNfe.BuscarPorCargaEntrega(cargaEntrega.Codigo);
            List<string> listaChaves = chaves.Select(obj => obj.ChaveNfe).ToList();

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xMLNotaFiscals = repXMLnotaFiscal.BuscarPorChaves(listaChaves);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega.Codigo);

            if (xMLNotaFiscals.Count > 0)
            {
                for (int i = 0; i < cargaPedidos.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];

                    if (cargaPedido.Pedido.Destinatario != null && cargaPedido.Pedido.Remetente != null)
                    {
                        //List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> xmlsNotasFiscaisPedido = (from obj in xMLNotaFiscals where obj.Destinatario.CPF_CNPJ == cargaPedido.Pedido.Destinatario.CPF_CNPJ && obj.Emitente.CPF_CNPJ == cargaPedido.Pedido.Remetente.CPF_CNPJ select obj).ToList();
                        //if (xmlsNotasFiscaisPedido.Count > 0)
                        //{

                        //de momento vamos inserir todas as notas em todos pedidos....
                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal in xMLNotaFiscals)
                        {
                            serNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracao, false, out bool alteradoTipoDeCarga, auditado);
                        }
                        //}
                    }

                    if (finalizarEnvioDocumentosCarga && cargaPedido != null)
                    {
                        string retornoFinalizacao = Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref cargaPedido, 0, 0, null, null, null, null, null, configuracao, tipoServicoMultisoftware, auditado, null, unitOfWork);

                        if (string.IsNullOrWhiteSpace(retornoFinalizacao))
                        {
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido, "Integrou notas fiscais", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, cargaPedido.Carga, "Integrou notas fiscais", unitOfWork);
                        }
                    }
                }
            }
        }

        private static void ProcessarSituacaoEntregaXMLNotaFiscal(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, SituacaoNotaFiscal novaSituacaoNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            ProcessarSituacaoEntregaXMLNotaFiscal(cargaEntrega, novaSituacaoNotaFiscal, situacaoNotasFiscaisNaoAtualizar: null, unitOfWork);
        }

        public static void ProcessarSituacaoEntregaXMLNotaFiscal(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, SituacaoNotaFiscal novaSituacaoNotaFiscal, List<SituacaoNotaFiscal> situacaoNotasFiscaisNaoAtualizar, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega == null)
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            repositorioXmlNotaFiscal.AtualizarSituacaoNotasFiscaisPorEntrega(cargaEntrega.Codigo, situacaoNotasFiscaisNaoAtualizar, novaSituacaoNotaFiscal);
        }

        public static void SetarConfiguracaoPedidoOcorrenciaPorCargaEntrega(int codigoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(codigoCarga);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            Servicos.Embarcador.Pedido.OcorrenciaPedido servicoOcorrenciaPedido = new Pedido.OcorrenciaPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarEntregaPorCargaPedido(cargaPedido.Codigo);

                if (cargaEntrega != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configPedidoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();
                    configPedidoOcorrencia.Latitude = carga.LatitudeInicioViagem;
                    configPedidoOcorrencia.Longitude = carga.LongitudeInicioViagem;
                    configPedidoOcorrencia.DataExecucao = DateTime.Now;
                    configPedidoOcorrencia.DataPosicao = DateTime.Now;
                    configPedidoOcorrencia.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
                    configPedidoOcorrencia.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
                    configPedidoOcorrencia.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";
                    configPedidoOcorrencia.OrigemSituacaoEntrega = cargaEntrega.Carga.OrigemSituacao;

                    servicoOcorrenciaPedido.ProcessarOcorrenciaPedido(EventoColetaEntrega.IniciaViagem, cargaPedido.Pedido, configuracaoEmbarcador, clienteMultisoftware, configPedidoOcorrencia);
                }
            }
        }

        private static DateTime? ObterDataInicioViagemPrevista(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repositorioCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repositorioCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            if (carga.DataInicioViagemPrevista.HasValue)
                return carga.DataInicioViagemPrevista.Value;

            if (carga.DadosSumarizados != null && carga.DadosSumarizados.DataPrevisaoInicioViagem.HasValue)
                return carga.DadosSumarizados.DataPrevisaoInicioViagem.Value;

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repositorioCargaRotaFrete.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosRota = cargaRotaFrete != null ? repositorioCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo) : new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();

            if (pontosRota.Count > 1 && cargasEntrega?.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargasEntrega.FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem primeiroPontoAposOrigem = pontosRota[1];

                DateTime? dataPrevista = cargaEntrega.DataPrevista;
                if (dataPrevista.HasValue && primeiroPontoAposOrigem.Tempo > 0)
                    return dataPrevista.Value.AddMinutes(-primeiroPontoAposOrigem.Tempo);//Desconta o tempo de deslocamento até a primeira coleta/entrega
            }

            return null;//carga.DataPrevisaoTerminoCarga ?? carga.DataCriacaoCarga;
        }

        private static void TratarEventoAutomaticamente(Dominio.Entidades.Embarcador.Cargas.Carga carga, TratativaAutomaticaMonitoramentoEvento tipoTratativa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Monitoramento.MonitoramentoEventoTratativaAutomatica serMonitoramentoEventoTratativaAutomatica = new Servicos.Embarcador.Monitoramento.MonitoramentoEventoTratativaAutomatica(unitOfWork);
            serMonitoramentoEventoTratativaAutomatica.TratarEventoAutomaticamente(carga, tipoTratativa);
        }

        public void SetarCanhotosComoPendente(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditoriaBase, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosDaCargaEntrega(cargaEntrega, unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
            {
                if (canhoto.SituacaoCanhoto == SituacaoCanhoto.Cancelado)
                {
                    canhoto.SituacaoCanhoto = SituacaoCanhoto.Pendente;
                    canhoto.SituacaoDigitalizacaoCanhoto = SituacaoDigitalizacaoCanhoto.PendenteDigitalizacao;
                    repCanhoto.Atualizar(canhoto);
                    Servicos.Auditoria.Auditoria.Auditar(auditoriaBase, canhoto, string.Format("Canhoto estava na situação Cancelado e voltou automaticamente para a situação Pendente porque a entrega/coleta {0} da carga {1} foi confirmada manualmente", cargaEntrega.Ordem, cargaEntrega.Carga.CodigoCargaEmbarcador), unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(auditoriaBase, cargaEntrega, string.Format("O canhoto {0} estava na situação Cancelado e voltou automaticamente para a situação Pendente porque a entrega/coleta foi confirmada manualmente", canhoto.Numero), unitOfWork);
                }
            }

        }

        private static List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosDaCargaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotosRetorno = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);

            // Pega todos os canhotos da cargaEntrega
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> todosCanhotos = repCanhoto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal in cargaEntregaNotasFiscais)
            {
                List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotoFiltrados = (
                    from obj in todosCanhotos
                    where (
                        (obj.TipoCanhoto == TipoCanhoto.NFe && obj.XMLNotaFiscal.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.XMLNotaFiscal.Codigo) ||
                        (obj.TipoCanhoto == TipoCanhoto.Avulso && obj.CanhotoAvulso != null && obj.CanhotoAvulso.PedidosXMLNotasFiscais.Any(nf => nf.Codigo == cargaEntregaNotaFiscal.PedidoXMLNotaFiscal.Codigo))
                    )
                    select obj
                ).ToList();

                foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotoFiltrados)
                {
                    if (canhoto != null && !canhotosRetorno.Contains(canhoto))
                        canhotosRetorno.Add(canhoto);
                }
            }

            return canhotosRetorno;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> ObterTiposAlertaControleEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaMonitoramentoEvento = repMonitoramentoEvento.BuscarMonitoramentoControleEntrega();

            return (from mon in listaMonitoramentoEvento select mon.TipoAlerta).ToList();

        }

        private string ObterImagemAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            switch (tipoAlerta)
            {
                case TipoAlerta.AlertaTendenciaEntregaAdiantada:
                    return "Content/TorreControle/Icones/alertas/tendencia-adiantamento.svg";
                case TipoAlerta.AlertaTendenciaEntregaAtrasada:
                    return "Content/TorreControle/Icones/alertas/tendencia-atraso.svg";
                case TipoAlerta.AtrasoNaDescarga:
                case TipoAlerta.AtrasoNaEntrega:
                case TipoAlerta.AtrasoNaLiberacao:
                case TipoAlerta.AtrasoNoCarregamento:
                case TipoAlerta.ForaDoPrazo:
                case TipoAlerta.PossivelAtrasoNaOrigem:
                    return "Content/TorreControle/Icones/alertas/espera.svg";
                case TipoAlerta.AusenciaDeInicioDeViagem:
                case TipoAlerta.InicioViagemSemDocumentacao:
                    return "Content/TorreControle/Icones/alertas/inicio-viagem-problema.svg";
                case TipoAlerta.ChegadaNoRaio:
                case TipoAlerta.ChegadaNoRaioEntrega:
                    return "Content/TorreControle/Icones/alertas/chegada-raio-entrega.svg";
                case TipoAlerta.ConcentracaoDeVeiculosNoRaio:
                case TipoAlerta.PermanenciaNoRaio:
                case TipoAlerta.PermanenciaNoRaioEntrega:
                    return "Content/TorreControle/Icones/alertas/permanencia-raio.svg";
                case TipoAlerta.DesvioDeRota:
                    return "Content/TorreControle/Icones/alertas/desvio-rota.svg";
                case TipoAlerta.DirecaoContinuaExcessiva:
                case TipoAlerta.DirecaoSemDescanso:
                    return "Content/TorreControle/Icones/alertas/direcao-problema.svg";
                case TipoAlerta.ParadaEmAreaDeRisco:
                case TipoAlerta.ParadaExcessiva:
                case TipoAlerta.ParadaNaoProgramada:
                    return "Content/TorreControle/Icones/alertas/parada-excessiva.svg";
                case TipoAlerta.PerdaDeSinal:
                case TipoAlerta.SemSinal:
                    return "Content/TorreControle/Icones/alertas/sem-sinal.svg";
                case TipoAlerta.PermanenciaNoPontoApoio:
                    return "Content/TorreControle/Icones/alertas/permanencia-ponto-apoio.svg";
                case TipoAlerta.SensorTemperaturaComProblema:
                case TipoAlerta.TemperaturaForaDaFaixa:
                    return "Content/TorreControle/Icones/alertas/temperatura.svg";
                case TipoAlerta.VelocidadeExcedida:
                    return "Content/TorreControle/Icones/alertas/velocidade.svg";
                default:
                    return "Content/TorreControle/AcompanhamentoCarga/assets/icons/default.png";
            }
        }

        private static void ExecutarValidacoesFinalizacaoEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = parametros.configuracaoControleEntrega ?? repositorioConfiguracaoControleEntrega.ObterConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = parametros.configuracaoEmbarcador ?? repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

            if (configuracaoControleEntrega?.PermitirBloqueioFinalizacaoEntrega ?? false)
            {
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamadosEmAbertoOuTratativa = repositorioChamado.BuscarAbertosOuEmTratativaPorCargaEntrega(parametros.cargaEntrega.Codigo).Result;

                if (chamadosEmAbertoOuTratativa.Any())
                {
                    List<string> detalhesChamados = chamadosEmAbertoOuTratativa
                        .Select(c => $"- Chamado #{c.Codigo} | Situação: {c.Situacao.ObterDescricao()}")
                        .ToList();

                    string mensagem = "A finalização da entrega está bloqueada pois existem atendimentos em aberto:\n" +
                                      string.Join("\n", detalhesChamados);

                    throw new ServicoException(mensagem, CodigoExcecao.RegistroIgnorado);
                }
            }


            if (parametros.tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe && configuracaoControleEntrega.NaoPermitirConfirmacaoEntregaPortalTransportadorSemDigitalizacaoCanhotos && repCargaEntregaNotaFiscal.ExisteCanhotosPendentesPorCargaEntrega(parametros.cargaEntrega.Codigo, parametros.cargaEntrega.Carga.Codigo))
                throw new ServicoException("Configuração não permite confirmar entrega com canhotos pendentes de digitalização.");

            if (parametros.cargaEntrega.Situacao == SituacaoEntrega.Rejeitado && (parametros.tipoOperacaoParametro?.NaoPermitirFinalizarEntregaRejeitada ?? false))
                throw new ServicoException("Para a operação da carga não é permitido confirmar uma entrega previamente rejeitada.");

            if (!parametros.cargaEntrega.Coleta && (parametros.tipoOperacaoParametro?.ObrigatorioVincularContainerCarga ?? false) && parametros.tipoServicoMultisoftware != TipoServicoMultisoftware.MultiTMS)
            {
                if (repCargaEntrega.ExisteColetaNaoEntregueNaOrigemPorCarga(parametros.cargaEntrega.Carga.Codigo))
                    throw new ServicoException("Não é possível finalizar a entrega pois a coleta do container ainda não foi confirmada");
            }

            if (parametros.cargaEntrega.Coleta && parametros.cargaEntrega.ColetaEquipamento && parametros.Container == 0 && parametros.retiradaContainer != null)
            {
                if (parametros.sistemaOrigem == Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp)
                {
                    string mensagem = string.Format(Localization.Resources.Cargas.ControleEntrega.TentativaConfirmacaoColetaEquipamentoX, parametros.cargaEntrega.Cliente?.NomeCNPJ ?? "");
                    Servicos.Auditoria.Auditoria.Auditar(parametros.auditado, parametros.cargaEntrega.Carga, mensagem, unitOfWork);
                    throw new ServicoException(mensagem);
                }
                throw new ServicoException("Não é possível finalizar a coleta pois o container não foi informado");
            }

            if (parametros.sistemaOrigem != Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.GerenciadorApp)
            {
                if (parametros.cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.Cancelada || parametros.cargaEntrega.Carga.SituacaoCarga == SituacaoCarga.Anulada)
                    throw new ServicoException("A carga está cancelada/anulada, não sendo possível realizar a operação.", Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado);

                if (parametros.tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS &&
                    !parametros.cargaEntrega.Coleta && !parametros.cargaEntrega.Fronteira && !parametros.cargaEntrega.Parqueamento &&
                    parametros.cargaEntrega.Carga.SituacaoCarga != SituacaoCarga.EmTransporte &&
                    parametros.cargaEntrega.Carga.SituacaoCarga != SituacaoCarga.Encerrada &&
                    parametros.cargaEntrega.Carga.SituacaoCarga != SituacaoCarga.AgIntegracao)
                    throw new ServicoException($"A situação da carga ({parametros.cargaEntrega.Carga.SituacaoCarga.ObterDescricao()}) não permite a confirmação da entrega.");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoesCarga = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>();
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgImpressaoDocumentos);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.ProntoTransporte);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.LiberadoPagamento);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao);
                situacoesCarga.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe);

                if (!parametros.cargaEntrega.Coleta && parametros.cargaEntrega.Carga.TipoFreteEscolhido != TipoFreteEscolhido.Cliente &&
                    !situacoesCarga.Contains(parametros.cargaEntrega.Carga.SituacaoCarga) && !parametros.cargaEntrega.Carga.AgImportacaoCTe &&
                    !(parametros.tipoOperacaoParametro?.ConfiguracaoMobile?.ExibirEntregaAntesEtapaTransporte ?? false) && !configuracaoEmbarcador.ExibirEntregaAntesEtapaTransporte)
                    throw new ServicoException("Não é possível finalizar a entrega pois a carga ainda não está em transporte.");
            }
        }

        public static Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaAssincronaParametros ConverterParametrosFinalizacaoAssincrona(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros)
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaAssincronaParametros parametrosFinalizarEntregaAssincrona = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaAssincronaParametros();
            parametrosFinalizarEntregaAssincrona.cargaEntrega = parametros.cargaEntrega.Codigo;
            parametrosFinalizarEntregaAssincrona.configuracaoEmbarcador = parametros.configuracaoEmbarcador?.Codigo ?? 0;
            parametrosFinalizarEntregaAssincrona.clienteMultisoftware = parametros.clienteMultisoftware?.Codigo ?? 0;
            parametrosFinalizarEntregaAssincrona.motorista = parametros.motorista?.Codigo ?? 0;
            parametrosFinalizarEntregaAssincrona.configuracaoControleEntrega = parametros.configuracaoControleEntrega?.Codigo ?? 0;
            parametrosFinalizarEntregaAssincrona.tipoOperacaoParametro = parametros.tipoOperacaoParametro?.Codigo ?? 0;
            parametrosFinalizarEntregaAssincrona.retiradaContainer = parametros.retiradaContainer?.Codigo ?? 0;
            parametrosFinalizarEntregaAssincrona.auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.AuditadoAssincrono()
            {
                Empresa = parametros.auditado?.Empresa?.Codigo ?? 0,
                Usuario = parametros.auditado?.Usuario?.Codigo ?? 0,
                Integradora = parametros.auditado?.Integradora?.Codigo ?? 0,
                IP = parametros.auditado?.IP ?? string.Empty,
                Texto = parametros.auditado?.Texto ?? string.Empty,
                TipoAuditado = parametros.auditado?.TipoAuditado ?? TipoAuditado.Sistema,
                OrigemAuditado = parametros.auditado?.OrigemAuditado ?? OrigemAuditado.Sistema
            };

            parametrosFinalizarEntregaAssincrona.tipoServicoMultisoftware = parametros.tipoServicoMultisoftware;
            parametrosFinalizarEntregaAssincrona.sistemaOrigem = parametros.sistemaOrigem;
            parametrosFinalizarEntregaAssincrona.OrigemSituacaoEntrega = parametros.OrigemSituacaoEntrega;
            parametrosFinalizarEntregaAssincrona.OrigemSituacaoFimViagem = parametros.OrigemSituacaoFimViagem;

            parametrosFinalizarEntregaAssincrona.dataInicioEntrega = parametros.dataInicioEntrega;
            parametrosFinalizarEntregaAssincrona.dataTerminoEntrega = parametros.dataTerminoEntrega;
            parametrosFinalizarEntregaAssincrona.dataConfirmacao = parametros.dataConfirmacao;
            parametrosFinalizarEntregaAssincrona.dataConfirmacaoChegada = parametros.dataConfirmacaoChegada;
            parametrosFinalizarEntregaAssincrona.wayPointConfirmacaoChegada = parametros.wayPointConfirmacaoChegada;
            parametrosFinalizarEntregaAssincrona.dataSaidaRaio = parametros.dataSaidaRaio;
            parametrosFinalizarEntregaAssincrona.DataColetaContainer = parametros.DataColetaContainer;
            parametrosFinalizarEntregaAssincrona.finalizandoViagem = parametros.FinalizandoViagem;
            parametrosFinalizarEntregaAssincrona.dataConfirmacaoEntrega = parametros.dataConfirmacaoEntrega;

            parametrosFinalizarEntregaAssincrona.wayPoint = parametros.wayPoint;
            parametrosFinalizarEntregaAssincrona.wayPointDescarga = parametros.wayPointDescarga;

            parametrosFinalizarEntregaAssincrona.pedidos = parametros.pedidos;
            parametrosFinalizarEntregaAssincrona.dadosRecebedor = parametros.dadosRecebedor;
            parametrosFinalizarEntregaAssincrona.chavesNFe = parametros.chavesNFe;

            parametrosFinalizarEntregaAssincrona.motivoRetificacao = parametros.motivoRetificacao;
            parametrosFinalizarEntregaAssincrona.motivoFalhaNotaFiscal = parametros.motivoFalhaNotaFiscal;
            parametrosFinalizarEntregaAssincrona.justificativaEntregaForaRaio = parametros.justificativaEntregaForaRaio;
            parametrosFinalizarEntregaAssincrona.motivoFalhaGTA = parametros.motivoFalhaGTA;
            parametrosFinalizarEntregaAssincrona.OrdemRealizada = parametros.OrdemRealizada;

            parametrosFinalizarEntregaAssincrona.handlingUnitIds = parametros.handlingUnitIds;
            parametrosFinalizarEntregaAssincrona.ClienteAreaRedex = parametros.ClienteAreaRedex;
            parametrosFinalizarEntregaAssincrona.Container = parametros.Container;
            parametrosFinalizarEntregaAssincrona.avaliacaoColetaEntrega = parametros.avaliacaoColetaEntrega;
            parametrosFinalizarEntregaAssincrona.observacao = parametros.observacao;

            parametrosFinalizarEntregaAssincrona.ExecutarValidacoes = parametros.ExecutarValidacoes;
            return parametrosFinalizarEntregaAssincrona;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros ObterParametrosFinalizacaoAssincrona(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaAssincronaParametros parametros, Repositorio.UnitOfWork _unitOfWork)
        {
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametrosFinalizarEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros();
            parametrosFinalizarEntrega.cargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork).BuscarPorCodigo(parametros.cargaEntrega);
            parametrosFinalizarEntrega.configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarPorCodigo(parametros.configuracaoEmbarcador, false);
            parametrosFinalizarEntrega.clienteMultisoftware = new AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente() { Codigo = parametros.clienteMultisoftware };
            parametrosFinalizarEntrega.motorista = repositorioUsuario.BuscarPorCodigo(parametros.motorista);
            parametrosFinalizarEntrega.configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork).BuscarPorCodigo(parametros.configuracaoControleEntrega, false);
            parametrosFinalizarEntrega.tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork).BuscarPorCodigoFetch(parametros.tipoOperacaoParametro);
            parametrosFinalizarEntrega.retiradaContainer = new Repositorio.Embarcador.Pedidos.RetiradaContainer(_unitOfWork).BuscarPorCodigo(parametros.retiradaContainer, false);
            parametrosFinalizarEntrega.auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                Empresa = new Repositorio.Empresa(_unitOfWork).BuscarPorCodigo(parametros.auditado?.Empresa ?? 0),
                Usuario = repositorioUsuario.BuscarPorCodigo(parametros.auditado?.Usuario ?? 0),
                Integradora = new Repositorio.WebService.Integradora(_unitOfWork).BuscarPorCodigo(parametros.auditado?.Integradora ?? 0),
                IP = parametros.auditado?.IP ?? string.Empty,
                Texto = parametros.auditado?.Texto ?? string.Empty,
                TipoAuditado = parametros.auditado?.TipoAuditado ?? TipoAuditado.Sistema,
                OrigemAuditado = parametros.auditado?.OrigemAuditado ?? OrigemAuditado.Sistema
            };

            parametrosFinalizarEntrega.ExecutarValidacoes = parametros.ExecutarValidacoes;

            parametrosFinalizarEntrega.tipoServicoMultisoftware = parametros.tipoServicoMultisoftware;
            parametrosFinalizarEntrega.sistemaOrigem = parametros.sistemaOrigem;
            parametrosFinalizarEntrega.OrigemSituacaoEntrega = parametros.OrigemSituacaoEntrega;
            parametrosFinalizarEntrega.OrigemSituacaoFimViagem = parametros.OrigemSituacaoFimViagem;

            parametrosFinalizarEntrega.dataInicioEntrega = parametros.dataInicioEntrega;
            parametrosFinalizarEntrega.dataTerminoEntrega = parametros.dataTerminoEntrega;
            parametrosFinalizarEntrega.dataConfirmacao = parametros.dataConfirmacao;
            parametrosFinalizarEntrega.dataConfirmacaoChegada = parametros.dataConfirmacaoChegada;
            parametrosFinalizarEntrega.wayPointConfirmacaoChegada = parametros.wayPointConfirmacaoChegada;
            parametrosFinalizarEntrega.dataSaidaRaio = parametros.dataSaidaRaio;
            parametrosFinalizarEntrega.DataColetaContainer = parametros.DataColetaContainer;
            parametrosFinalizarEntrega.FinalizandoViagem = parametros.finalizandoViagem;
            parametrosFinalizarEntrega.dataConfirmacaoEntrega = parametros.dataConfirmacaoEntrega;

            parametrosFinalizarEntrega.wayPoint = parametros.wayPoint;
            parametrosFinalizarEntrega.wayPointDescarga = parametros.wayPointDescarga;

            parametrosFinalizarEntrega.pedidos = parametros.pedidos;
            parametrosFinalizarEntrega.dadosRecebedor = parametros.dadosRecebedor;
            parametrosFinalizarEntrega.chavesNFe = parametros.chavesNFe;

            parametrosFinalizarEntrega.motivoRetificacao = parametros.motivoRetificacao;
            parametrosFinalizarEntrega.motivoFalhaNotaFiscal = parametros.motivoFalhaNotaFiscal;
            parametrosFinalizarEntrega.justificativaEntregaForaRaio = parametros.justificativaEntregaForaRaio;
            parametrosFinalizarEntrega.motivoFalhaGTA = parametros.motivoFalhaGTA;
            parametrosFinalizarEntrega.OrdemRealizada = parametros.OrdemRealizada;

            parametrosFinalizarEntrega.handlingUnitIds = parametros.handlingUnitIds;
            parametrosFinalizarEntrega.ClienteAreaRedex = parametros.ClienteAreaRedex;
            parametrosFinalizarEntrega.Container = parametros.Container;
            parametrosFinalizarEntrega.avaliacaoColetaEntrega = parametros.avaliacaoColetaEntrega;
            parametrosFinalizarEntrega.observacao = parametros.observacao;
            return parametrosFinalizarEntrega;
        }

        private static void AvancarEtapaGestaoDevolucao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(unitOfWork);
            Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao = repositorioGestaoDevolucao.BuscarPorCodigoCarga(carga.Codigo);
            if (gestaoDevolucao != null)
            {
                Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new GestaoDevolucao.GestaoDevolucao(unitOfWork, auditado, clienteMultisoftware);
                servicoGestaoDevolucao.AvancarEtapaGestaoDevolucao(gestaoDevolucao, "Avançado ao finalizar viagem");
            }
        }

        private static void ValidarIntegracaoCanhotoFimViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Servicos.Embarcador.Canhotos.Canhoto servicoCanhoto = new Servicos.Embarcador.Canhotos.Canhoto(unitOfWork);

            List<TipoIntegracao> tiposIntegracaoGeracaoCteCanhoto = new List<TipoIntegracao>();
            tiposIntegracaoGeracaoCteCanhoto.Add(TipoIntegracao.Cebrace);
            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tiposIntegracaoGeracaoCteCanhoto);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repositorioCargaCte.BuscarCTePorCarga(carga.Codigo);

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    if (cte == null || cte.XMLNotaFiscais == null)
                        continue;

                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in cte.XMLNotaFiscais)
                        servicoCanhoto.GerarRegistroIntegracao(cte, notaFiscal.Canhoto, tipoIntegracao, TipoRegistroIntegracaoCTeCanhoto.Imagem).GetAwaiter().GetResult();
                }
            }

        }

        private static bool PermiteIniciarFinalizarViagem(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega, bool finalizarViagem, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Repositorio.UnitOfWork unitOfWork = null)
        {
            bool permite = true;
            string origem = "";

            if (finalizarViagem)
                origem = "Finalizar Viagem";
            else
                origem = "Iniciar Viagem";

            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe &&
                ((configuracaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false) || (configuracaoTipoOperacaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false))
                && SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida().Contains(cargaEntrega.Carga.SituacaoCarga))
            {
                Log.TratarErro($@"A atual situação da carga ({cargaEntrega.Carga.DescricaoSituacaoCarga}) não permite {origem} pelo Transportador. Carga: {cargaEntrega.Carga.CodigoCargaEmbarcador}", "FinalizarEntrega");

                if (auditado != null)
                    Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, $"A atual situação da carga ({cargaEntrega.Carga.DescricaoSituacaoCarga}) não permite {origem} pelo transportador.", unitOfWork);

                permite = false;
            }

            return permite;
        }

        private static DateTime? ObterDataPrazoParada(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDataCalculoParadaNoPrazo tipoDataCalculoParadaNoPrazo)
        {
            DateTime? data = null;
            switch (tipoDataCalculoParadaNoPrazo)
            {
                case TipoDataCalculoParadaNoPrazo.DataEntradaNoRaio:
                    data = cargaEntrega.DataEntradaRaio;
                    break;
                case TipoDataCalculoParadaNoPrazo.DataInicio:
                    data = cargaEntrega.DataInicio;
                    break;
                case TipoDataCalculoParadaNoPrazo.DataConfirmacao:
                    data = cargaEntrega.DataConfirmacao;
                    break;
                case TipoDataCalculoParadaNoPrazo.DataPrevista:
                    data = cargaEntrega.DataPrevista;
                    break;
                case TipoDataCalculoParadaNoPrazo.DataAgendamento:
                    data = cargaEntrega.DataAgendamento;
                    break;
            }
            return data;
        }

        private void SalvarHistoricoSituacaoNotaFiscalChamado(Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Chamados.HistoricoNotaFiscalChamado repositorioHistoricoNotaFiscalChamado = new Repositorio.Embarcador.Chamados.HistoricoNotaFiscalChamado(unitOfWork);
            Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado historicoNotaFiscalChamado = repositorioHistoricoNotaFiscalChamado.BuscarPrimeiroRegistroNotaPorCodigoEChamado(chamado.Codigo, xmlNotaFiscal.Codigo);

            //Se já salvou a situação da nota ao abrir o chamado, não vamos salvar novamente
            if (historicoNotaFiscalChamado != null)
                return;

            Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado historicoNotaFiscalChamadoSalvar = new Dominio.Entidades.Embarcador.Chamados.HistoricoNotaFiscalChamado()
            {
                Chamado = chamado,
                SituacaoNotaFiscal = xmlNotaFiscal.SituacaoEntregaNotaFiscal,
                XMLNotaFiscal = xmlNotaFiscal
            };

            repositorioHistoricoNotaFiscalChamado.Inserir(historicoNotaFiscalChamadoSalvar);
        }
        private dynamic ObterProduto(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto cargaEntregaProduto, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoChamado configuracaoChamado, Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado> cargaEntregaProdutoChamados)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            string corFonte = string.Empty;
            if (cargaEntregaProduto.QuantidadeDevolucao >= cargaEntregaProduto.Quantidade)
                corFonte = "#ff0000";
            else if (cargaEntregaProduto.QuantidadeDevolucao > 0m)
                corFonte = "#e08506";

            decimal valorDevolucaoCalculado = 0;
            if (cargaEntregaProduto.QuantidadeDevolucao > 0 && configuracaoChamado.CalcularValorDasDevolucoes)
                valorDevolucaoCalculado = cargaEntregaProduto.QuantidadeDevolucao * repositorioCargaEntregaPedido.ObterPrecoUnitarioPedidoProdutoPorCargaEntrega(cargaEntregaProduto.CargaEntrega.Codigo, cargaEntregaProduto.Produto.Codigo);

            decimal quantidadeDevolucao = 0;
            decimal valorDevolucao = (configuracaoChamado?.CalcularValorDasDevolucoes ?? false) ? valorDevolucaoCalculado : 0;
            decimal NFDevolucao = 0;
            int codigoMotivoDaDevolucao = 0;
            string descricaoMotivoDaDevolucao = string.Empty;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProdutoChamado cargaEntregaProdutoChamado = cargaEntregaProdutoChamados?.Find(p => p.Produto.Codigo == cargaEntregaProduto.Produto.Codigo);
            if (cargaEntregaProdutoChamado != null)
            {
                quantidadeDevolucao = cargaEntregaProdutoChamado.QuantidadeDevolucao;
                valorDevolucao = cargaEntregaProdutoChamado.ValorDevolucao;
                NFDevolucao = cargaEntregaProdutoChamado.NFDevolucao;

                if (cargaEntregaProdutoChamado.MotivoDaDevolucao != null)
                {
                    codigoMotivoDaDevolucao = cargaEntregaProdutoChamado.MotivoDaDevolucao.Codigo;
                    descricaoMotivoDaDevolucao = cargaEntregaProdutoChamado.MotivoDaDevolucao.Descricao;
                }
            }

            if (cargaEntregaProduto.ValorDevolucao > 0 && valorDevolucao == 0)
                valorDevolucao = cargaEntregaProduto.ValorDevolucao;
            if (cargaEntregaProduto.NFDevolucao > 0 && NFDevolucao == 0)
                NFDevolucao = cargaEntregaProduto.NFDevolucao;

            return new
            {
                cargaEntregaProduto.Codigo,
                cargaEntregaProduto.Produto.Descricao,
                CodigoProduto = cargaEntregaProduto.Produto.CodigoProdutoEmbarcador,
                PesoTotal = (cargaEntregaProduto.Quantidade * cargaEntregaProduto.PesoUnitario).ToString($"n{configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto}"),
                Quantidade = cargaEntregaProduto.Quantidade.ToString($"n{configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto}"),
                QuantidadeDevolvida = (Math.Max(cargaEntregaProduto.QuantidadeDevolucao - quantidadeDevolucao, 0)).ToString($"n{configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto}"),
                QuantidadeDevolucao = quantidadeDevolucao.ToString($"n{configuracaoEmbarcador.NumeroCasasDecimaisQuantidadeProduto}"),
                Observacao = cargaEntregaProduto.ObservacaoProdutoDevolucao ?? string.Empty,
                Lote = cargaEntregaProduto?.Lote ?? string.Empty,
                DataCritica = cargaEntregaProduto.DataCritica.HasValue ? cargaEntregaProduto.DataCritica.Value.ToString("dd/MM/yyyy") : string.Empty,
                ValorDevolucao = valorDevolucao.ToString("n2"),
                NFDevolucao,
                DT_FontColor = corFonte,
                CodigoMotivoDaDevolucao = codigoMotivoDaDevolucao,
                MotivoDaDevolucao = descricaoMotivoDaDevolucao,
                ValorTotal = cargaEntregaProduto.Produto.ValorTotal
            };
        }

        private dynamic ObterDevolucao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscalItemDevolucao itemDevolucao)
        {
            return new
            {
                Codigo = itemDevolucao.Codigo,
                CodigoNotaFiscal = itemDevolucao.CargaEntregaNotaFiscal.Codigo,
                QuantidadeDevolucao = itemDevolucao.QuantidadeDevolucao,
                QuantidadeDevolvida = 0,
                ValorDevolucao = itemDevolucao.ValorDevolucao,
                NFDevolucao = itemDevolucao.NFDevolucao,
            };
        }


        #endregion Métodos Privados
    }
}
