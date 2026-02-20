using System.Collections.Generic;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class EntregaIntegracao
    {
        #region Métodos Privados


        #endregion


        #region Metodos Publicos

        public static void GerarNovaIntegracaoEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, int CodigoAgrupador, Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repCargaEntregaIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao cargaEntregaIntegracao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao();
            cargaEntregaIntegracao.Carga = carga;
            cargaEntregaIntegracao.TipoIntegracao = tipoIntegracao;
            cargaEntregaIntegracao.CodigoAgrupador = CodigoAgrupador;
            cargaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

            repCargaEntregaIntegracao.Inserir(cargaEntregaIntegracao);

        }


        public static void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao repCargaEntregaIntegracao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao> IntegracoesEntrega = repCargaEntregaIntegracao.BuscarIntegracaoPendente(3, 5, 20, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao.Individual);

            for (int i = 0; i < IntegracoesEntrega.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaIntegracao integracaoPendente = IntegracoesEntrega[i];
                if (integracaoPendente.TipoIntegracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Ortec)
                {
                    new Integracao.Ortec.IntegracaoOrtec(unitOfWork).EnviarIntegracaoEntregaCarga(integracaoPendente, tipoServicoMultisoftware);
                }
                else
                {
                    integracaoPendente.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    integracaoPendente.ProblemaIntegracao = "Tipo de integração não implementada";
                    repCargaEntregaIntegracao.Atualizar(integracaoPendente);
                }
            }

        }

        #endregion



    }
}
