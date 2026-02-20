using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaSaidaCD
    {
        public static void CriarEtapaSaidaCD(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD repEtapaSaidaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD etapaSaidaCD = repEtapaSaidaCD.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaSaidaCD == null)
            {
                etapaSaidaCD = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD();
                etapaSaidaCD.DataInformada = DateTime.Now;
                etapaSaidaCD.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaSaidaCD.Observacao = "";
                repEtapaSaidaCD.Inserir(etapaSaidaCD);
            }
        }

        public static void LiberarEtapaSaidaCD(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD repEtapaSaidaCD = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaCD etapaSaidaCD = repEtapaSaidaCD.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaSaidaCD != null)
            {
                etapaSaidaCD.EtapaLiberada = true;
                repEtapaSaidaCD.Atualizar(etapaSaidaCD);
            }
        }
    }
}
