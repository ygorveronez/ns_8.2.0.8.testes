using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaVeiculoAlocado
    {
        public static void CriarEtapaVeiculoAlocado(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado repEtapaVeiculoAlocado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado etapaVeiculoAlocado = repEtapaVeiculoAlocado.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaVeiculoAlocado == null)
            {
                etapaVeiculoAlocado = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado();
                etapaVeiculoAlocado.DataInformada = DateTime.Now;
                etapaVeiculoAlocado.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaVeiculoAlocado.Observacao = "";
                repEtapaVeiculoAlocado.Inserir(etapaVeiculoAlocado);
            }
        }

        public static void LiberarEtapaVeiculoAlocado(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado repEtapaVeiculoAlocado = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaVeiculoAlocado etapaVeiculoAlocado = repEtapaVeiculoAlocado.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaVeiculoAlocado != null)
            {
                etapaVeiculoAlocado.EtapaLiberada = true;
                repEtapaVeiculoAlocado.Atualizar(etapaVeiculoAlocado);
            }
        }
    }
}
