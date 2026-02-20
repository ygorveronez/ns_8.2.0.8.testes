using System;

namespace Servicos.Embarcador.Carga
{
    public class CargaDatas
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.Cargas.Carga _repositorioCarga;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        public CargaDatas(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoEmbarcador = configuracaoEmbarcador;
            _unitOfWork = unitOfWork;
            _repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }

        public void SalvarDataSalvamentoDadosTransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (!carga.DataPrimeiroSalvamentoDadosTransporte.HasValue)
            {
                carga.DataPrimeiroSalvamentoDadosTransporte = DateTime.Now;
            }

            carga.DataSalvamentoDadosTransporte = DateTime.Now;
            _repositorioCarga.Atualizar(carga);
        }
        
        public void SalvarDataConfirmacaoValorFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            carga.DataConfirmacaoValorFrete = DateTime.Now;
            _repositorioCarga.Atualizar(carga);
        }
    }
}
