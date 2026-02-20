using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Logistica
{
    public sealed class CargaJanelaCarregamentoConsulta
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public CargaJanelaCarregamentoConsulta(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmbarcador: null) { }

        public CargaJanelaCarregamentoConsulta(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
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

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento ObterCargaJanelaCarregamentoPorCarga(int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            return repositorioCargaJanelaCarregamento.BuscarPorCarga(codigoCarga);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterCargasJanelaCarregamentoPorCargas(List<int> codigosCargas)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);

            return repositorioCargaJanelaCarregamento.BuscarPorCargas(codigosCargas);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> ObterCargasJanelaCarregamentoComCargasAgrupadas(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamentoReferencia)
        {
            bool cargaJanelaCarregamentoPorCargaAgrupada = (cargaJanelaCarregamentoReferencia.Carga != null) && (cargaJanelaCarregamentoReferencia.Carga.CargaAgrupada || (cargaJanelaCarregamentoReferencia.Carga.CargaAgrupamento != null));

            if (!cargaJanelaCarregamentoPorCargaAgrupada)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>() { cargaJanelaCarregamentoReferencia };

            Dominio.Entidades.Embarcador.Cargas.Carga cargaReferencia = cargaJanelaCarregamentoReferencia.Carga.CargaAgrupamento ?? cargaJanelaCarregamentoReferencia.Carga;
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> cargasJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = cargaJanelaCarregamentoReferencia.Carga.CargaAgrupada ? cargaJanelaCarregamentoReferencia : repositorioCargaJanelaCarregamento.BuscarPorCarga(cargaReferencia.Codigo);

            if (cargaJanelaCarregamento != null)
                cargasJanelaCarregamento.Add(cargaJanelaCarregamento);

            cargasJanelaCarregamento.AddRange(repositorioCargaJanelaCarregamento.BuscarPorCargasOriginais(cargaReferencia.Codigo));

            return cargasJanelaCarregamento;
        }

        public Dominio.Entidades.Embarcador.Logistica.TempoCarregamento ObterConfiguracaoTempoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TimeSpan horaInicioCarregamento)
        {
            return ObterConfiguracaoTempoCarregamento(cargaJanelaCarregamento.CargaBase.TipoDeCarga, cargaJanelaCarregamento.CargaBase.ModeloVeicularCarga, cargaJanelaCarregamento.CentroCarregamento, horaInicioCarregamento);
        }

        public Dominio.Entidades.Embarcador.Logistica.TempoCarregamento ObterConfiguracaoTempoCarregamento(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, TimeSpan horaInicioCarregamento)
        {
            if (tipoCarga == null)
                return null;

            if (modeloVeicularCarga == null)
                return null;

            if (centroCarregamento == null)
                return null;

            return centroCarregamento.TemposCarregamento
                .Where(o =>
                    o.TipoCarga.Codigo == tipoCarga.Codigo &&
                    o.ModeloVeicular.Codigo == modeloVeicularCarga.Codigo &&
                    (!o.HoraInicio.HasValue || o.HoraInicio.Value <= horaInicioCarregamento) &&
                    (!o.HoraTermino.HasValue || o.HoraTermino.Value >= horaInicioCarregamento)
                )
                .FirstOrDefault();
        }

        public int ObterTempoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaBase cargaBase, TimeSpan horaInicioCarregamento)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(_unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorTipoCargaEFilial(cargaBase?.TipoDeCarga?.Codigo ?? 0, cargaBase?.Filial?.Codigo ?? 0, ativo: true);

            return ObterTempoCarregamento(cargaBase.TipoDeCarga, cargaBase.ModeloVeicularCarga, centroCarregamento, horaInicioCarregamento);
        }

        public int ObterTempoCarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento, TimeSpan horaInicioCarregamento)
        {
            return ObterTempoCarregamento(cargaJanelaCarregamento.CargaBase.TipoDeCarga, cargaJanelaCarregamento.CargaBase.ModeloVeicularCarga, cargaJanelaCarregamento.CentroCarregamento, horaInicioCarregamento);
        }

        public int ObterTempoCarregamento(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento, TimeSpan horaInicioCarregamento)
        {
            if (centroCarregamento?.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento > 0)
                return centroCarregamento.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento * 60;

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();

            if (configuracaoEmbarcador.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento > 0)
                return configuracaoEmbarcador.TempoPadraoTerminoCarregamentoParaValidarDisponibilidadeDescarregamento * 60;

            if (centroCarregamento?.LimiteCarregamentos == LimiteCarregamentosCentroCarregamento.QuantidadeDocas)
                return 0;

            Dominio.Entidades.Embarcador.Logistica.TempoCarregamento configuracaoTempoCarregamento = ObterConfiguracaoTempoCarregamento(tipoCarga, modeloVeicularCarga, centroCarregamento, horaInicioCarregamento);
            
            return configuracaoTempoCarregamento?.Tempo ?? 0;
        }

        #endregion Métodos Públicos
    }
}
