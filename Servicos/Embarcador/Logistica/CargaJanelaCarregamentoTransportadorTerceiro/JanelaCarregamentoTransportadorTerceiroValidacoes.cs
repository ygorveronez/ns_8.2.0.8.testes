using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System.Globalization;

namespace Servicos.Embarcador.Logistica
{
    public class JanelaCarregamentoTransportadorTerceiroValidacoes
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento _configuracaoJanelaCarregamento;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public JanelaCarregamentoTransportadorTerceiroValidacoes(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware) : this(unitOfWork, tipoServicoMultisoftware, configuracaoEmbarcador: null) { }

        public JanelaCarregamentoTransportadorTerceiroValidacoes(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _unitOfWork = unitOfWork;
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void ValidarValorFrete(decimal valorFreteTransportador, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportador cargaJanelaCarregamentoTransportador, bool multiplasCargas)
        {
            if (valorFreteTransportador <= 0 && multiplasCargas)
                throw new ServicoException("Necessário informar valores.");
            else if (valorFreteTransportador <= 0)
                throw new ServicoException("Valor do frete deve ser maior que zero.");

            if (ObterConfiguracaoJanelaCarregamento().LiberarCargaParaCotacaoAoLiberarParaTransportadores)
                return;

            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.CargaJanelaCarregamentoTransportadorValorFrete janelaComMenorLance = new CargaJanelaCarregamentoTransportadorTerceiroConsulta(_unitOfWork).ObterCargaJanelaCarregamentoTransportadorTerceiroComMenorLance(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento, considerarCargasVinculadas: false);
            decimal menorLance = janelaComMenorLance?.ValorTotalFrete ?? cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Carga?.ValorFrete ?? 0m;

            if (menorLance <= 0)
                return;

            string campoMenorLance = janelaComMenorLance == null ? "valor original do frete da carga" : "valor do menor lance";

            if (valorFreteTransportador > menorLance)
                throw new ServicoException($"O valor informado não pode ser superior ao {campoMenorLance} ({menorLance.ToString("C2")}).");

            Dominio.Entidades.Embarcador.Logistica.LancesCarregamento configuracaoLance = new Repositorio.Embarcador.Logistica.LancesCarregamento(_unitOfWork).BuscarPorNumeroLance(cargaJanelaCarregamentoTransportador.CargaJanelaCarregamento.Rodada, centroCarregamento.Codigo);
            decimal percentualTolerancia = configuracaoLance != null ? configuracaoLance.PorcentagemLance : centroCarregamento?.PercentualMaximoDiferencaValorCotacao ?? 0m;

            if (percentualTolerancia <= 0)
                return;

            decimal percentualDiferenca = 100 - ((valorFreteTransportador * 100.0m) / menorLance);

            if (percentualDiferenca < percentualTolerancia)
                throw new ServicoException($"O valor informado deve ter uma diferença mínima de {percentualTolerancia.ToString("n2")}% do {campoMenorLance}. O valor do lance deve ser menor ou igual a {(menorLance - (menorLance * percentualTolerancia / 100.0m)).ToString("C2", new CultureInfo("pt-BR"))}");
        }

        #endregion Métodos Públicos


        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento ObterConfiguracaoJanelaCarregamento()
        {
            if (_configuracaoJanelaCarregamento == null)
                _configuracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoJanelaCarregamento;
        }

        #endregion Métodos Privados
    }
}
