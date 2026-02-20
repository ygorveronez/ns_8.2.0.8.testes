using System;

namespace Servicos.Embarcador.Frete
{
    public class BonificacaoTransportador
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public BonificacaoTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador ObterBonificacao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return ObterBonificacao(carga, carga.Empresa);
        }

        public Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador ObterBonificacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Empresa empresa)
        {
            if ((empresa == null) || (carga.Filial == null) || (carga.TipoDeCarga == null))
                return null;

            DateTime data = carga.DataCarregamentoCarga ?? carga.DataCriacaoCarga;
            Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador bonificacaoTransportador = repositorioBonificacaoTransportador.BuscarBonificacaoPorCarga(empresa.Codigo, carga.TipoDeCarga.Codigo, carga.Filial.Codigo, data);

            return bonificacaoTransportador;
        }

        #endregion Métodos Públicos
    }
}
