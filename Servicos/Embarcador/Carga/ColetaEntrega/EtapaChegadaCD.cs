using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaChegadaCD
    {
        public static void CriarEtapaChegadaCD(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD repEtapaChegadaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD etapaChegadaCD = repEtapaChegadaCD.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaChegadaCD == null)
            {
                etapaChegadaCD = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD();
                etapaChegadaCD.DataInformada = DateTime.Now;
                etapaChegadaCD.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaChegadaCD.Observacao = "";
                repEtapaChegadaCD.Inserir(etapaChegadaCD);
            }
        }

        public static void LiberarEtapaChegadaCD(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD repEtapaChegadaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaCD etapaChegadaCD = repEtapaChegadaCD.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaChegadaCD != null)
            {
                etapaChegadaCD.EtapaLiberada = true;
                repEtapaChegadaCD.Atualizar(etapaChegadaCD);
            }
        }
    }
}
