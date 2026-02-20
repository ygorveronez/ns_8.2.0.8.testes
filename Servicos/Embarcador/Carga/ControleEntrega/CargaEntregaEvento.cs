using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class CargaEntregaEvento
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaEntregaEvento(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento GerarCargaEntregaEvento(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, bool forcarEventoDuplicado = false)
        {
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = objetoEvento.TipoDeOcorrencia;
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = objetoEvento.CargaEntrega;

            if (tipoDeOcorrencia.GerarEventoIntegracaoCargaEntregaPorCarga)
                cargaEntrega = null;
            else if (cargaEntrega == null)
                return null;

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento repositorioCargaEntregaEvento = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(_unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventoIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);

            if (!forcarEventoDuplicado && !configuracaoControleEntrega.PermitirEnvioNovasOcorrenciasComMesmoCadastroTipoOcorrencia && repositorioCargaEntregaEvento.ExistePorTipoOcorrenciaCargaEntrega(tipoDeOcorrencia.Codigo, objetoEvento.Carga?.Codigo ?? 0, cargaEntrega?.Codigo ?? 0))
                return null;

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repOcorrenciaTipoIntegracao.BuscarIntegracaoPorTipoOcorrencia(tipoDeOcorrencia.Codigo);
            if (tiposIntegracao.Count == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipoIntegracaoAutorizados = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>()
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LoggiEventosEntrega,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.YPEEventosEntrega,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buntech,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CassolEventosEntrega,
            };

            tiposIntegracao = tiposIntegracao.Where(o => tipoIntegracaoAutorizados.Contains(o.Tipo)).ToList();

            if (tiposIntegracao.Count == 0)
                return null;

            objetoEvento.GerarIntegracao = true;

            //ARCELORMITTAL - Aqui vamos validar a geracao do cargaEntregaEvendo e Integracao do mesmo dependendo da carga (primeiro ou segundo trecho)
            if (tiposIntegracao.Exists(ti => ti.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ArcelorMittal))
                ValidarGeracaoIntegracaoEventoCarga(objetoEvento);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento()
            {
                DataOcorrencia = objetoEvento.DataOcorrencia,
                TipoDeOcorrencia = tipoDeOcorrencia,
                Carga = objetoEvento.Carga,
                CargaEntrega = cargaEntrega,
                EventoColetaEntrega = objetoEvento.EventoColetaEntrega,
                Latitude = objetoEvento.Latitude,
                Longitude = objetoEvento.Longitude,
                DataPosicao = objetoEvento.DataPosicao,
                DataPrevisaoRecalculada = objetoEvento.DataPrevisaoRecalculada,
                Origem = objetoEvento.Origem
            };

            repositorioCargaEntregaEvento.Inserir(cargaEntregaEvento);

            if (objetoEvento.GerarIntegracao)
                servicoCargaEntregaEventoIntegracao.GerarIntegracoes(cargaEntregaEvento, tiposIntegracao);

            if (objetoEvento.EventoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem
                && tipoDeOcorrencia.EssaOcorrenciaGeraOutraOcorrenciaIntegracao
                && tipoDeOcorrencia.TipoOcorrenciaIntegracao != null)
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrenciaIntegracao = tipoDeOcorrencia.TipoOcorrenciaIntegracao;

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento cargaEntregaEventoIntegracao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento()
                {
                    DataOcorrencia = objetoEvento.DataOcorrencia,
                    TipoDeOcorrencia = tipoOcorrenciaIntegracao,
                    Carga = objetoEvento.Carga,
                    CargaEntrega = cargaEntrega,
                    EventoColetaEntrega = objetoEvento.EventoColetaEntrega,
                    Latitude = objetoEvento.Latitude,
                    Longitude = objetoEvento.Longitude,
                    DataPosicao = objetoEvento.DataPosicao,
                    DataPrevisaoRecalculada = objetoEvento.DataPrevisaoRecalculada,
                    Origem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente
                };

                repositorioCargaEntregaEvento.Inserir(cargaEntregaEventoIntegracao);

                servicoCargaEntregaEventoIntegracao.GerarIntegracoes(cargaEntregaEventoIntegracao, tiposIntegracao);
            }

            return cargaEntregaEvento;
        }

        public void ProcessarEventoEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento repositorioCargaEntregaEvento = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaEvento(_unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao servicoCargaEntregaEventoIntegracao = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEventoIntegracao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEncaixados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (cargaPedidos.Any(obj => obj.PedidoEncaixado))
                cargaPedidosEncaixados = repCargaPedido.BuscarPorPedidos((from o in cargaPedidos where o.PedidoEncaixado select o.Pedido.Codigo).ToList());

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaEvento.CargaEntrega;

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega;
                Dominio.Entidades.Cliente clientePedido;
                if (cargaEntrega.Coleta)
                {
                    tipoAplicacaoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta;
                    clientePedido = pedido.Remetente;
                }
                else
                {
                    clientePedido = pedido.Destinatario;
                    tipoAplicacaoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega;
                }

                bool clienteAlvo = cargaEntrega.Cliente == clientePedido;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntregaPedido = null;
                if (cargaEntrega.Reentrega)
                {
                    configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == clienteAlvo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega && obj.Reentrega == true select obj).FirstOrDefault();

                    if (configuracaoOcorrenciaEntregaPedido == null) // caso nao exista de reentrega continua como estava..
                        configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == clienteAlvo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();
                }
                else
                    configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == clienteAlvo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();

                if (configuracaoOcorrenciaEntregaPedido == null && cargaEntregaEvento.EventoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem)
                    configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();

                //aqui buscar a configuracaoOcorrenciaEntregaPedido caso estiver o tipo operacao marcado e validar com a operacao da CARGA origem.
                if (cargaEntrega.Carga?.TipoOperacao?.ConfiguracaoControleEntrega?.GerarEventoColetaEntregaUnicoParaTodosTrechos ?? false && cargaEntregaEvento.EventoColetaEntrega.HasValue)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntregaPedidoecc = null;
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregasEcc = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(cargaEntregaEvento.EventoColetaEntrega.Value);

                    configuracaoOcorrenciaEntregaPedidoecc = (from obj in configuracaoOcorrenciaEntregasEcc where obj.AlvoDoPedido == clienteAlvo && obj.TipoOperacao?.Codigo == cargaEntrega.Carga.TipoOperacao?.Codigo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();

                    if (configuracaoOcorrenciaEntregaPedidoecc != null)
                        configuracaoOcorrenciaEntregaPedido = configuracaoOcorrenciaEntregaPedidoecc;
                }

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                if (cargaEntrega.CargaOrigem != null)
                    carga = cargaEntrega.CargaOrigem;
                else
                    carga = cargaEntrega.Carga;

                if (cargaPedido.PedidoEncaixado)
                    carga = (from obj in cargaPedidosEncaixados where obj.Pedido.Codigo == pedido.Codigo && obj.ObterTomador().CPF_CNPJ == pedido.ObterTomador().Codigo select obj).FirstOrDefault()?.Carga ?? null;

                if (configuracaoOcorrenciaEntregaPedido != null)
                {
                    cargaEntregaEvento.Carga = carga;
                    cargaEntregaEvento.TipoDeOcorrencia = configuracaoOcorrenciaEntregaPedido.TipoDeOcorrencia;

                    GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega);

                    //evento de inicio de viagem de uma carga com origem diferente, precisamos buscar as demais cargas desse pedido e iniciar viagem tambem das demais cargas origens.
                    if (cargaEntregaEvento.EventoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem
                        && cargaEntregaEvento.CargaEntrega != null && cargaEntregaEvento.CargaEntrega.Carga.Codigo != cargaEntregaEvento.Carga.Codigo)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasFilho = repCargaPedido.BuscarCargasDeEntregasRedespachoPorPedido(pedido.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaFilho in cargasFilho)
                        {
                            if (cargaFilho.Codigo == cargaEntrega.CargaOrigem?.Codigo || cargaFilho.Codigo == cargaEntrega.Carga.Codigo)
                                continue;

                            cargaEntregaEvento.Carga = cargaFilho;
                            cargaEntregaEvento.TipoDeOcorrencia = configuracaoOcorrenciaEntregaPedido.TipoDeOcorrencia;
                            GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, false);
                        }
                    }

                    if (cargaEntregaEvento.EventoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.FimViagem
                        && cargaEntregaEvento.CargaEntrega != null && cargaEntregaEvento.CargaEntrega.Carga.Codigo != cargaEntregaEvento.Carga.Codigo)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasFilho = repCargaPedido.BuscarCargasDeEntregasRedespachoPorPedido(pedido.Codigo);

                        foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaFilho in cargasFilho)
                        {
                            if (cargaFilho.Codigo == cargaEntrega.CargaOrigem?.Codigo || cargaFilho.Codigo == cargaEntrega.Carga.Codigo)
                                continue;

                            cargaEntregaEvento.Carga = cargaFilho;
                            cargaEntregaEvento.TipoDeOcorrencia = configuracaoOcorrenciaEntregaPedido.TipoDeOcorrencia;
                            GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, false);
                        }
                    }
                }
            }
        }

        #endregion

        #region Metodos privados

        private void ValidarGeracaoIntegracaoEventoCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento)
        {
            Repositorio.Embarcador.Cargas.Redespacho repositorioCargaRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(_unitOfWork);

            if (!objetoEvento.Carga?.TipoOperacao?.ConfiguracaoControleEntrega?.GerarEventoColetaEntregaUnicoParaTodosTrechos ?? false)
                return;

            Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = repositorioCargaRedespacho.BuscarPorCarga(objetoEvento.Carga.Codigo);
            if (redespacho == null)
                redespacho = repositorioCargaRedespacho.BuscarPorCargaGerada(objetoEvento.Carga.Codigo);

            switch (objetoEvento.EventoColetaEntrega)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem:
                    ValidarInicioViagemCarga(objetoEvento, redespacho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Confirma:
                    ValidarConfirmacaoEntregaCarga(objetoEvento, redespacho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.UltimaConfirmacao:
                    ValidarConfirmacaoUltimaEntregaCarga(objetoEvento, redespacho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.FimViagem:
                    ValidarFimViagemCarga(objetoEvento, redespacho);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.ChegadaNoAlvo:
                    ValidarChegadaAlvo(objetoEvento, redespacho);
                    break;
                default:
                    // code block
                    break;
            }

        }

        private void ValidarChegadaAlvo(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento, Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho)
        {
            if (redespacho == null)
                return;

            if (redespacho.CargaGerada?.Codigo == objetoEvento.Carga?.Codigo)
                objetoEvento.Carga = redespacho.CargaGerada;
        }

        private void ValidarInicioViagemCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento, Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho)
        {
            if (redespacho == null)
                return;

            if (redespacho.CargaGerada?.Codigo == objetoEvento.Carga?.Codigo)
                //carga filho não gera integracao inicio viagem.
                objetoEvento.GerarIntegracao = false;
            else
            {
                //validar se carga é ORIGEM de alguma carga de redespacho
                if (objetoEvento.CargaEntrega?.CargaOrigem?.Codigo == redespacho.CargaGerada?.Codigo)
                    objetoEvento.GerarIntegracao = false;
            }
        }

        private void ValidarConfirmacaoEntregaCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento, Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho)
        {
            if (redespacho == null)
                return;

            if (redespacho.CargaGerada?.Codigo == objetoEvento.Carga?.Codigo)
            {
                //carga filho gera integracao CONFIRMA ENTREGA PARA A CARGA PAI.
                objetoEvento.Carga = redespacho.CargaGerada;
            }
            else
            {
                //validar se carga é ORIGEM de alguma carga de redespacho, se sim é pra essa carga q temos q enviar o evento
                if (objetoEvento.CargaEntrega?.CargaOrigem?.Codigo == redespacho.CargaGerada?.Codigo)
                    objetoEvento.Carga = objetoEvento.CargaEntrega.CargaOrigem;
            }
        }

        private void ValidarConfirmacaoUltimaEntregaCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento, Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho)
        {


            if (redespacho == null)
                return;

            if (redespacho.CargaGerada?.Codigo == objetoEvento.Carga?.Codigo)
            {
                //na confirmacao da ultima entrega para uma carga filho, devemos validar se todas as entregas da pai ja foram entregues; se sim gera.
                objetoEvento.Carga = redespacho.CargaGerada;
            }
        }

        private void ValidarFimViagemCarga(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento objetoEvento, Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho)
        {
            //PRECISAMOS VALIDAR SE ENTREGA DESTA CARGA POSSUI CARGA ORIGEM.
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasFilho = repositorioCargaEntrega.BuscarCargaFilhoOrigemEntregaPorCargaOrigem(objetoEvento.Carga?.Codigo ?? 0);
            if (cargasFilho?.Count() > 0)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> pedidosOrigem = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaFilho in cargasFilho) //validar se os pedidos de todas carga cargasFilho foram entregues.
                    pedidosOrigem.AddRange(repCargaPedido.BuscarPorCarga(cargaFilho.Codigo));

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = pedidosOrigem.Select(x => x.Pedido).Distinct().ToList();

                //buscar dentre os pedidos quais ja foram entregues 
                if (repCargaEntregaPedido.ExistePedidoAindaNaoEntregue(pedidos.Select(x => x.Codigo).ToList()))
                {
                    objetoEvento.GerarIntegracao = false;
                    return;
                }
            }


            if (redespacho == null)
                return;

            if (redespacho.Carga?.Codigo == objetoEvento.Carga?.Codigo)
            {   //carga pai. nao gera integracao fim viagem.
                objetoEvento.GerarIntegracao = false;
            }
            else if (redespacho.CargaGerada?.Codigo == objetoEvento.Carga?.Codigo)
            {
                //carga filho nao gera evento fim de viagem, mas tem q validar se todas foram finalizadas, se positivo gera fim de viagem para pai.
                objetoEvento.GerarIntegracao = false;

                if (!PossuiPedidosAbertos(redespacho.Carga))
                {
                    objetoEvento.GerarIntegracao = true;
                    objetoEvento.Carga = objetoEvento.Carga;
                }
            }
        }


        private bool PossuiPedidosAbertos(Dominio.Entidades.Embarcador.Cargas.Carga cargaOriginal)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.Redespacho repositorioRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> listaSituacoesPendentes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega>
            {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Revertida,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.AgAtendimento
            };

            //pegar todas as cargas de redespacho com a carga pai.
            List<Dominio.Entidades.Embarcador.Cargas.Redespacho> listaRedespachoGerado = repositorioRedespacho.BuscarAtivasPorCargaOrigem(cargaOriginal.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho in listaRedespachoGerado)
            {
                //percorrer cada uma das cargas geradas do redespacho e buscar as carga entrega de cada uma delas.
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregasFilho = repositorioCargaEntrega.BuscarPorCarga(redespacho.CargaGerada.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaFilho in cargasEntregasFilho)
                {
                    if (listaSituacoesPendentes.Contains(cargaEntregaFilho.Situacao))
                        return true;
                }

                //se todas as carga entrega das filhos estao finalizadas; passar para o proximo passo.. validar pedidos de cada carga filho (gerada redespacho) se pedido totalmentecarregado = false
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosFilho = repositorioPedido.BuscarPorCarga(redespacho.CargaGerada.Codigo);
                if (pedidosFilho.Any(obj => !obj.PedidoTotalmenteCarregado))
                    return true;

                if (cargasEntregasFilho.Count <= 0)
                {
                    //carga filho(redespacho) nao possui entregas, precisamos buscar as entregas onde essa carga é a carga origem nas entregas e ver se estao confirmadas
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntregasFilhoRedespacho = repositorioCargaEntrega.BuscarPorCargaOrigem(redespacho.CargaGerada.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaFilhoRedespacho in cargasEntregasFilhoRedespacho)
                    {
                        if (listaSituacoesPendentes.Contains(cargaEntregaFilhoRedespacho.Situacao))
                            return true;
                    }
                }
            }

            return false;

        }
        #endregion

    }
}
