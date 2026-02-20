using System.Collections.Generic;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaDescarregamentoConsulta
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaDescarregamentoConsulta(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaJanelaDescarregamentoConsulta(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> ObterCargasJanelaCarregamentoComCargasAgrupadas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamentoReferencia)
        {
            bool cargaJanelaDescarregamentoPorCargaAgrupada = (cargaJanelaDescarregamentoReferencia.Carga != null) && (cargaJanelaDescarregamentoReferencia.Carga.CargaAgrupada || (cargaJanelaDescarregamentoReferencia.Carga.CargaAgrupamento != null));

            if (!cargaJanelaDescarregamentoPorCargaAgrupada)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>() { cargaJanelaDescarregamentoReferencia };

            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = cargaJanelaDescarregamentoReferencia.Carga.CargaAgrupamento ?? cargaJanelaDescarregamentoReferencia.Carga;
            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento>();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = cargaJanelaDescarregamentoReferencia.Carga.CargaAgrupada ? cargaJanelaDescarregamentoReferencia : repositorioCargaJanelaDescarregamento.BuscarPorCarga(cargaReferencia.Codigo);

            if (cargaJanelaDescarregamento != null)
                cargasJanelaCarregamento.Add(cargaJanelaDescarregamento);

            cargasJanelaCarregamento.AddRange(repositorioCargaJanelaDescarregamento.BuscarPorCargasOriginais(cargaReferencia.Codigo));

            return cargasJanelaCarregamento;
        }

        #endregion Métodos Públicos
    }
}
