using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaProcessoFinalizado
    {
        public static void CriarEtapaProcessoFinalizado(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado repEtapaProcessoFinalizado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado etapaProcessoFinalizado = repEtapaProcessoFinalizado.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaProcessoFinalizado == null)
            {
                etapaProcessoFinalizado = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado();
                etapaProcessoFinalizado.DataInformada = DateTime.Now;
                etapaProcessoFinalizado.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaProcessoFinalizado.Observacao = "";
                repEtapaProcessoFinalizado.Inserir(etapaProcessoFinalizado);
            }
        }

        public static void LiberarEtapaProcessoFinalizado(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado repEtapaProcessoFinalizado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaProcessoFinalizado etapaProcessoFinalizado = repEtapaProcessoFinalizado.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaProcessoFinalizado != null)
            {
                etapaProcessoFinalizado.EtapaLiberada = true;
                repEtapaProcessoFinalizado.Atualizar(etapaProcessoFinalizado);
            }
        }
    }
}
