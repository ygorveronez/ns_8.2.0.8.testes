using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Frota
{
    public class OrdemServicoVeiculoManutencao
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public OrdemServicoVeiculoManutencao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void VeiculoIndisponivelParaTransporte(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<int> codigosVeiculos = null)
        {
            VeiculoIndisponivelParaTransporte(new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>() { pedido });
        }

        public void VeiculoIndisponivelParaTransporte(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, List<int> codigosVeiculos = null)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            if (!configuracaoVeiculo.NaoPermitirUtilizarVeiculoEmManutencao)
                return;

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoRetornado in pedidos)
            {
                if (codigosVeiculos != null && codigosVeiculos.Count > 0)
                {
                    if (ValidarSeVeiculoIndisponivelParaTransporteDevidoManutencaoOrdemServico(pedidoRetornado, codigosVeiculos))
                        throw new ServicoException("Processo cancelado! O veículo/reboque selecionado encontra-se em manutenção.");
                }
                else if (ValidarSeVeiculoIndisponivelParaTransporteDevidoManutencaoOrdemServico(pedidoRetornado))
                    throw new ServicoException("Processo cancelado! O veículo/reboque selecionado encontra-se em manutenção.");

                if (pedidoRetornado.DataPrevisaoSaida == null)
                    if (ValidarSeVeiculoIndisponivelParaTransporteDevidoEstarEmManutencao(pedidoRetornado, codigosVeiculos))
                        throw new ServicoException("Processo cancelado! Status do veículo/reboque: Em Manutenção.");
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool ValidarSeVeiculoIndisponivelParaTransporteDevidoManutencaoOrdemServico(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            List<int> codigosVeiculos = ObterVeiculosPedido(pedido);

            if (codigosVeiculos.Count == 0)
                return false;

            List<SituacaoOrdemServicoFrota> listaStatusOS = new List<SituacaoOrdemServicoFrota>() {
                SituacaoOrdemServicoFrota.EmManutencao, SituacaoOrdemServicoFrota.EmDigitacao, SituacaoOrdemServicoFrota.AgAutorizacao,
                SituacaoOrdemServicoFrota.Rejeitada, SituacaoOrdemServicoFrota.DivergenciaOrcadoRealizado, SituacaoOrdemServicoFrota.SemRegraAprovacao,
                SituacaoOrdemServicoFrota.AguardandoAprovacao, SituacaoOrdemServicoFrota.AprovacaoRejeitada
            };

            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordensServico = repOrdemServico.BuscarPorVeiculos(codigosVeiculos, listaStatusOS);

            if (ordensServico != null && ordensServico.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota os in ordensServico)
                {
                    if (pedido.DataPrevisaoSaida < os.DataLimiteExecucao)
                        return true;
                }
            }
            return false;
        }

        private bool ValidarSeVeiculoIndisponivelParaTransporteDevidoManutencaoOrdemServico(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<int> listaCodigosVeiculos)
        {
            if (listaCodigosVeiculos.Count == 0)
                return false;

            List<SituacaoOrdemServicoFrota> listaStatusOS = new List<SituacaoOrdemServicoFrota>() {
                SituacaoOrdemServicoFrota.EmManutencao, SituacaoOrdemServicoFrota.EmDigitacao, SituacaoOrdemServicoFrota.AgAutorizacao,
                SituacaoOrdemServicoFrota.Rejeitada, SituacaoOrdemServicoFrota.DivergenciaOrcadoRealizado, SituacaoOrdemServicoFrota.SemRegraAprovacao,
                SituacaoOrdemServicoFrota.AguardandoAprovacao, SituacaoOrdemServicoFrota.AprovacaoRejeitada
            };

            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordensServico = repOrdemServico.BuscarPorVeiculos(listaCodigosVeiculos, listaStatusOS);

            if (ordensServico != null && ordensServico.Count > 0)
            {
                foreach (Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota os in ordensServico)
                {
                    if (pedido.DataPrevisaoSaida < os.DataLimiteExecucao)
                        return true;
                }
            }
            return false;
        }

        private bool ValidarSeVeiculoIndisponivelParaTransporteDevidoEstarEmManutencao(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, List<int> listaCodigosVeiculos)
        {
            List<int> codigosVeiculos;

            if (listaCodigosVeiculos != null && listaCodigosVeiculos.Count > 0)
                codigosVeiculos = listaCodigosVeiculos;
            else
                codigosVeiculos = ObterVeiculosPedido(pedido);

            if (codigosVeiculos.Count == 0)
                return false;

            Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(_unitOfWork);

            foreach (int codigosVeiculo in codigosVeiculos)
            {
                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacaoVeiculo = repSituacaoVeiculo.BuscarUltimoPorVeiculo(codigosVeiculo);
                if (situacaoVeiculo != null && (situacaoVeiculo.Situacao == SituacaoVeiculo.EmManutencao || situacaoVeiculo.Situacao == SituacaoVeiculo.EmViagem))
                    return true;
            }

            return false;
        }

        private List<int> ObterVeiculosPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            List<int> codigosVeiculos = new List<int>();

            if (pedido.VeiculoTracao != null)
                codigosVeiculos.Add(pedido.VeiculoTracao.Codigo);

            if (pedido.Veiculos != null && pedido.Veiculos.Count > 0)
                codigosVeiculos.AddRange(pedido.Veiculos.Select(o => o.Codigo).ToList());

            return codigosVeiculos;
        }

        #endregion Métodos Privados

    }
}
