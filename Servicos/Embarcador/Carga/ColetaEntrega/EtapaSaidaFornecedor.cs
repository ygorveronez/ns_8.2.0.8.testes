using System;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaSaidaFornecedor
    {
        public static void CriarEtapaSaidaFornecedor(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor repEtapaSaidaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor etapaSaidaFornecedor = repEtapaSaidaFornecedor.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaSaidaFornecedor == null)
            {
                etapaSaidaFornecedor = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor();
                etapaSaidaFornecedor.DataInformada = DateTime.Now;
                etapaSaidaFornecedor.FluxoColetaEntrega = fluxoColetaEntrega;
                etapaSaidaFornecedor.Observacao = "";
                repEtapaSaidaFornecedor.Inserir(etapaSaidaFornecedor);
            }
        }

        public static void LiberarEtapaSaidaFornecedor(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor repEtapaSaidaFornecedor = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaSaidaFornecedor etapaSaidaFornecedor = repEtapaSaidaFornecedor.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaSaidaFornecedor != null)
            {
                etapaSaidaFornecedor.EtapaLiberada = true;
                repEtapaSaidaFornecedor.Atualizar(etapaSaidaFornecedor);
            }
        }
    }
}
