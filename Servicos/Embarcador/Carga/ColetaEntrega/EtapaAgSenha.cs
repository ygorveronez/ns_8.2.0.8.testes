using System;
using System.Linq;

namespace Servicos.Embarcador.Carga.ColetaEntrega
{
    public static class EtapaAgSenha
    {
        public static bool CriarEtapaAgSenha(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha repEtapaSenha = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha etapaSenha = repEtapaSenha.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);
            
            string senhaInformacao = (from o in repCargaPedido.BuscarPorCarga(fluxoColetaEntrega.Carga.Codigo) where o.Pedido != null && !string.IsNullOrWhiteSpace(o.Pedido.SenhaAgendamento) select o.Pedido.SenhaAgendamento).FirstOrDefault();
            bool possuiSenha = false;
            if (etapaSenha == null)
            {
                etapaSenha = new Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha
                {
                    DataInformada = DateTime.Now,
                    FluxoColetaEntrega = fluxoColetaEntrega,
                    Senha = senhaInformacao,
                    EtapaLiberada = true,
                    Observacao = ""
                };
                repEtapaSenha.Inserir(etapaSenha);

                possuiSenha = !string.IsNullOrWhiteSpace(senhaInformacao);

                if (possuiSenha)
                    Servicos.Embarcador.Carga.ColetaEntrega.FluxoColetaEntrega.ReplicarSenha(fluxoColetaEntrega, senhaInformacao, unitOfWork);
            }

            return possuiSenha;
        }

        public static void LiberarEtapaAgSenha(Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.FluxoColetaEntrega fluxoColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha repEtapaAgSenha = new Repositorio.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ColetaEntrega.EtapaAgSenha etapaAgSenha = repEtapaAgSenha.BuscarPorFluxoColetaEntrega(fluxoColetaEntrega.Codigo);

            if (etapaAgSenha != null)
            {
                etapaAgSenha.EtapaLiberada = true;
                repEtapaAgSenha.Atualizar(etapaAgSenha);
            }
        }
    }
}
    