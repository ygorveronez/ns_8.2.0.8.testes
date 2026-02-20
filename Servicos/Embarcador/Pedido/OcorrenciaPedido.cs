using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Pedido
{
    public sealed class OcorrenciaPedido
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public OcorrenciaPedido(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Alternativa do método ProcessarOcorrenciaPedido para evitar uma grande quantidade de consultas desnecessárias em casos de grandes quantidades de pedidos.
        /// </summary>
        public void ProcessarOcorrenciaPedido(EventoColetaEntrega EventoGatilho, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrencia = null, bool gerarNotificacao = true)
        {
            if (pedidos == null)
                return;

            Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repositorioConfiguracaoOcorrenciaPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedido = repositorioConfiguracaoOcorrenciaPedido.BuscarRegraPorEvento(EventoGatilho);

            if (configuracaoOcorrenciaPedido == null)
                return;

            if (configOcorrencia == null)
                configOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();

            if (configuracaoOcorrencia.PermiteAdicionarMaisOcorrenciaMesmoEvento)
            {
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedidoPorTipoOperacao = configuracaoOcorrenciaPedido;

                    if (pedido.TipoOperacao != null)
                        configuracaoOcorrenciaPedidoPorTipoOperacao = repositorioConfiguracaoOcorrenciaPedido.BuscarRegraPorEventoETipoOperacaoDoPedidoEDaOcorrencia(pedido.TipoOperacao.Codigo, EventoGatilho);

                    if (configuracaoOcorrenciaPedidoPorTipoOperacao == null)
                        continue;

                    GerarOcorrenciaEntregaPedido(configuracaoOcorrenciaPedidoPorTipoOperacao, pedido, clienteMultisoftware, configuracaoEmbarcador, configOcorrencia, gerarNotificacao);
                }
            }
            else
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    GerarOcorrenciaEntregaPedido(configuracaoOcorrenciaPedido, pedido, clienteMultisoftware, configuracaoEmbarcador, configOcorrencia, gerarNotificacao);
        }

        public void ProcessarOcorrenciaPedido(EventoColetaEntrega EventoGatilho, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrencia = null, bool gerarNotificacao = true)
        {
            if (pedido == null)
                return;

            Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido repositorioConfiguracaoOcorrenciaPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repositorioConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repositorioConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configuracaoOcorrenciaPedido;

            if (configuracaoOcorrencia.PermiteAdicionarMaisOcorrenciaMesmoEvento && pedido.TipoOperacao != null)
                configuracaoOcorrenciaPedido = repositorioConfiguracaoOcorrenciaPedido.BuscarRegraPorEventoETipoOperacaoDoPedidoEDaOcorrencia(pedido.TipoOperacao.Codigo, EventoGatilho);
            else
                configuracaoOcorrenciaPedido = repositorioConfiguracaoOcorrenciaPedido.BuscarRegraPorEvento(EventoGatilho);

            if (configuracaoOcorrenciaPedido == null)
                return;

            if (configOcorrencia == null)
                configOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();

            GerarOcorrenciaEntregaPedido(configuracaoOcorrenciaPedido, pedido, clienteMultisoftware, configuracaoEmbarcador, configOcorrencia, gerarNotificacao);
        }

        #endregion

        #region Metodos Privados

        private bool ExisteEventoPedido(EventoColetaEntrega EventoColetaEntrega, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(_unitOfWork);

            return repositorioPedidoOcorrenciaColetaEntrega.ExistePorPedidoTipoEventoColetaEntrega(EventoColetaEntrega, pedido.Codigo);
        }

        private void GerarOcorrenciaEntregaPedido(Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoOcorrenciaPedido configOcorrenciaPedido, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configoOcorrenciaCoordenada, bool gerarNotificacao)
        {
            if (ExisteEventoPedido(configOcorrenciaPedido.EventoColetaEntrega, pedido))
                return;

            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(_unitOfWork);
            Dominio.Entidades.Cliente tomador = pedido.ObterTomador() ?? pedido.Remetente;

            Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega(tomador, pedido, null, configOcorrenciaPedido.TipoDeOcorrencia, configuracaoPortalCliente, DateTime.Now, configOcorrenciaPedido.EventoColetaEntrega, configuracaoEmbarcador, clienteMultisoftware, configoOcorrenciaCoordenada, _unitOfWork, gerarNotificacao);
        }

        #endregion 
    }
}
