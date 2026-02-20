using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Integracao
{
    public class ControleIntegracaoCargaEDI
    {
        public static void AtualizarSituacaoCargaControleEDI(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI situacaoCargaIntegracaoEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            if (carga.CargaAgrupada)
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasOrigem = repCarga.BuscarCargasOriginais(carga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaorigem in cargasOrigem)
                    SetarSituacaoCargaControleEDI(cargaorigem, situacaoCargaIntegracaoEDI, unitOfWork);
            }
            else if (carga.CargaDeVinculo)
            {
                List<int> cargas = repCarga.BuscarCargasVinculadas(carga.Codigo);
                SetarSituacaoCargaControleEDI(cargas, situacaoCargaIntegracaoEDI, unitOfWork);
            }
            else
                SetarSituacaoCargaControleEDI(carga, situacaoCargaIntegracaoEDI, unitOfWork);
        }

        private static void SetarSituacaoCargaControleEDI(List<int> cargas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI situacaoCargaIntegracaoEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao> controleIntegracaoCargaEDIAlteracao = repControleIntegracaoCargaEDIAlteracao.BuscarPorCargas(cargas);
            foreach (Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao integracaoCargaEDIAlteracao in controleIntegracaoCargaEDIAlteracao)
                AtualizarSituacaoCargaControleEDI(integracaoCargaEDIAlteracao, situacaoCargaIntegracaoEDI, unitOfWork);
            
                
        }

        private static void SetarSituacaoCargaControleEDI(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI situacaoCargaIntegracaoEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleIntegracaoCargaEDIAlteracao = repControleIntegracaoCargaEDIAlteracao.BuscarPorCarga(carga.Codigo);
            if (controleIntegracaoCargaEDIAlteracao != null)
                AtualizarSituacaoCargaControleEDI(controleIntegracaoCargaEDIAlteracao, situacaoCargaIntegracaoEDI, unitOfWork);
        }

        private static void AtualizarSituacaoCargaControleEDI(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleIntegracaoCargaEDIAlteracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI situacaoCargaIntegracaoEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);

            if (controleIntegracaoCargaEDIAlteracao != null)
            {
                controleIntegracaoCargaEDIAlteracao.ControleIntegracaoCargaEDI.SituacaoCarga = situacaoCargaIntegracaoEDI;
                controleIntegracaoCargaEDIAlteracao.ControleIntegracaoCargaEDI.DataAtualizacaoSituacaoCarga = DateTime.Now;
                repControleIntegracaoCargaEDI.Atualizar(controleIntegracaoCargaEDIAlteracao.ControleIntegracaoCargaEDI);
            }
        }
    }
}
