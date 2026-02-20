using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaPallets
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion Atributos

        #region Contrutores

        public CargaPallets(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaPallets(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracaoEmbarcador;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public void RatearPaletesModeloVeicularCargaEntreOsPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso)
                return;

            if (carga.ModeloVeicularCarga?.NumeroPaletes > 0)
                RatearPaletesEntreOsPedidos(carga.ModeloVeicularCarga.NumeroPaletes.Value, cargaPedidos);
        }
        
        public async Task RatearPaletesModeloVeicularCargaEntreOsPedidosAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (!configuracaoEmbarcador.RatearNumeroPalletsModeloVeiculoEntrePedidoPorPeso)
                return;

            if (carga.ModeloVeicularCarga?.NumeroPaletes > 0)
                RatearPaletesEntreOsPedidos(carga.ModeloVeicularCarga.NumeroPaletes.Value, cargaPedidos);
        }

        public void RatearPaletesEntreOsPedidos(int quantidadePaletesTotal, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            bool desconsiderarNotasRemessaPallets = repositorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever);
            List<int> cargaPedidosFiltrados = new List<int>();

            if (desconsiderarNotasRemessaPallets)
            {
                cargaPedidosFiltrados = repositorioPedidoXmlNotaFiscal.BuscarCodigosCargaPedidosSemPallet(cargaPedidos.Select(x => x.Codigo).ToList());

                // Ordenação adicionada para não dar problema no "for" abaixo
                cargaPedidos = cargaPedidos.OrderByDescending(x => !cargaPedidosFiltrados.Contains(x.Codigo)).ToList();
            }

            decimal pesoTotal = cargaPedidos.Sum(cargaPedido => cargaPedido.Peso);

            if (pesoTotal <= 0m)
                return;

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            int totalPaletesRateado = 0;

            for (int i = 0; i < cargaPedidos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[i];
                if (desconsiderarNotasRemessaPallets && !cargaPedidosFiltrados.Contains(cargaPedido.Codigo))
                {
                    cargaPedido.Pedido.NumeroPaletes = 0;
                    repositorioPedido.Atualizar(cargaPedido.Pedido);
                    repositorioCargaPedido.Atualizar(cargaPedido);

                    // Se esse fosse o último pedido, ele não aplicava o "resto" faltante dos pallets, por isso a ordenação
                    continue;
                }
                bool ultimoCargaPedido = i == (cargaPedidos.Count - 1);
                decimal percentualRateio = quantidadePaletesTotal / pesoTotal;
                int paletesRateado = (int)Math.Round((percentualRateio * cargaPedido.Peso), 0, MidpointRounding.ToEven);

                if (quantidadePaletesTotal >= (totalPaletesRateado + paletesRateado))
                    cargaPedido.Pedido.NumeroPaletes = paletesRateado;
                else if (quantidadePaletesTotal >= totalPaletesRateado)
                    cargaPedido.Pedido.NumeroPaletes = (quantidadePaletesTotal - totalPaletesRateado);
                else
                    cargaPedido.Pedido.NumeroPaletes = 0;

                totalPaletesRateado += cargaPedido.Pedido.NumeroPaletes;

                if (ultimoCargaPedido && (quantidadePaletesTotal > totalPaletesRateado))
                    cargaPedido.Pedido.NumeroPaletes += quantidadePaletesTotal - totalPaletesRateado;

                Servicos.Embarcador.Carga.Carga.CalcularValorDescargaPorCargaPedido(cargaPedido, configuracaoEmbarcador, _unitOfWork);

                repositorioPedido.Atualizar(cargaPedido.Pedido);
                repositorioCargaPedido.Atualizar(cargaPedido);
            }
        }

        #endregion Métodos Públicos
    }
}