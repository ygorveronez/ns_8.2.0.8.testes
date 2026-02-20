using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Pedido
{
    public class LoteLiberacaoComercialPedido
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public LoteLiberacaoComercialPedido(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void AdicionarIntegracao(Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido loteLiberacaoComercialPedido)
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reptipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = reptipoIntegracao.BuscarPorTipo(TipoIntegracao.Italac);

            if (tipoIntegracao != null && loteLiberacaoComercialPedido != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracao = new Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao()
                {
                    DataIntegracao = DateTime.Now,
                    LoteLiberacaoComercialPedido = loteLiberacaoComercialPedido,
                    ProblemaIntegracao = "",
                    SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                    TipoIntegracao = tipoIntegracao
                };

                repositorioLoteLiberacaoComercialPedidoIntegracao.Inserir(integracao);
            }
            else
            {
                throw new ControllerException("Tipo de integração não configurado");
            }

        }

        public void IntegrarLoteLiberacaoComercialPedido()
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao repositorioLoteLiberacaoComercialPedidoIntegracao = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao(_unitOfWork);
            
            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao> integracoes = repositorioLoteLiberacaoComercialPedidoIntegracao.BuscarPendentesIntegracaoPorTipo(numeroTentativas, minutosACadaTentativa, TipoIntegracao.Italac);

            if (integracoes.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoIntegracao integracao in integracoes)
            {
                Integracao.Italac.IntegracaoItalac serItalac = new Integracao.Italac.IntegracaoItalac(_unitOfWork);
                serItalac.IntegrarLoteLiberacaoComercialPedidoPendenteIntegracao(integracao);
            }
        }

        #endregion
    }
}
