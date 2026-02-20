using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaAgAlocarVeiculo
    {
        public static void CriarEtapaAgAlocarVeiculo(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo repEtapaAgAlocarVeiculo = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo etapaAgAlocarVeiculo = repEtapaAgAlocarVeiculo.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaAgAlocarVeiculo == null)
            {
                etapaAgAlocarVeiculo = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo();
                etapaAgAlocarVeiculo.DataInformada = DateTime.Now;
                etapaAgAlocarVeiculo.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaAgAlocarVeiculo.Observacao = "";
                repEtapaAgAlocarVeiculo.Inserir(etapaAgAlocarVeiculo);
            }
        }

        public static void LiberarEtapaAgAlocarVeiculo(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo repEtapaAgAlocarVeiculo = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgAlocarVeiculo etapaAgAlocarVeiculo = repEtapaAgAlocarVeiculo.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaAgAlocarVeiculo != null)
            {
                etapaAgAlocarVeiculo.EtapaLiberada = true;
                repEtapaAgAlocarVeiculo.Atualizar(etapaAgAlocarVeiculo);
            }
        }
    }
}
