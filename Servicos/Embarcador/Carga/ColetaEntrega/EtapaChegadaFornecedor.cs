using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaChegadaFornecedor
    {
        public static void CriarEtapaChegadaFornecedor(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor repEtapaChegadaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor etapaChegadaFornecedor = repEtapaChegadaFornecedor.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaChegadaFornecedor == null)
            {
                etapaChegadaFornecedor = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor();
                etapaChegadaFornecedor.DataInformada = DateTime.Now;
                etapaChegadaFornecedor.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaChegadaFornecedor.Observacao = "";
                repEtapaChegadaFornecedor.Inserir(etapaChegadaFornecedor);
            }
        }


        public static void LiberarEtapaChegadaFornecedor(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor repEtapaChegadaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaChegadaFornecedor etapaChegadaFornecedor = repEtapaChegadaFornecedor.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaChegadaFornecedor != null)
            {
                etapaChegadaFornecedor.EtapaLiberada = true;
                repEtapaChegadaFornecedor.Atualizar(etapaChegadaFornecedor);
            }
        }
    }
}
