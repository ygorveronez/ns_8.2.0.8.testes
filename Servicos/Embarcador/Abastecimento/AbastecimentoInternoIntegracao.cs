using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Abastecimento
{
    public class AbastecimentoInternoIntegracao
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public AbastecimentoInternoIntegracao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        public void VerificarIntegracoesAbastecimentoAutomatizado(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tipos = new List<TipoIntegracao> { TipoIntegracao.Conecttec };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repositorioTipoIntegracao.BuscarPorTipos(tipos);

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in tiposIntegracao)
            {
                GerarRegistroIntegracao(tipoIntegracao, liberacaoAbastecimentoAutomatizado, unitOfWork);
            }
        }

        #endregion

        #region Métodos privados

        private void GerarRegistroIntegracao(Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao, Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado liberacaoAbastecimentoAutomatizado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(unitOfWork);
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracaoo = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao = new Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao();

            liberacaoAbastecimentoAutomatizadoIntegracao.Initialize();
            liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado = liberacaoAbastecimentoAutomatizado;
            liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
            liberacaoAbastecimentoAutomatizadoIntegracao.DataIntegracao = DateTime.Now;
            liberacaoAbastecimentoAutomatizadoIntegracao.NumeroTentativas = 0;
            liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = "";
            liberacaoAbastecimentoAutomatizadoIntegracao.TipoIntegracao = tipoIntegracao;
            repLiberacaoAbastecimentoAutomatizadoIntegracaoo.Inserir(liberacaoAbastecimentoAutomatizadoIntegracao);


            liberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.PendenteReserva;
            repLiberacaoAbastecimentoAutomatizado.Atualizar(liberacaoAbastecimentoAutomatizado);

        }
        public void ProcessarIntegracaoPendenteReservaAbastecimento(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            switch (liberacaoAbastecimentoAutomatizadoIntegracao?.TipoIntegracao.Tipo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Conecttec:
                    new Servicos.Embarcador.Integracao.Conecttec.IntegracaoConecttec(unitOfWork).IntegrarReservaAbastecimento(liberacaoAbastecimentoAutomatizadoIntegracao);
                    break;

            }
        }

        public void ProcessarIntegracaoPendenteAutorizacaoAbastecimento(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            switch (liberacaoAbastecimentoAutomatizadoIntegracao?.TipoIntegracao.Tipo ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Conecttec:
                    new Servicos.Embarcador.Integracao.Conecttec.IntegracaoConecttec(unitOfWork).IntegrarAutorizacaoAbastecimento(liberacaoAbastecimentoAutomatizadoIntegracao);
                    break;

            }
        }

        public void CancelarAbastecimentoOcioso(Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao liberacaoAbastecimentoAutomatizadoIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizadoIntegracao = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
            liberacaoAbastecimentoAutomatizadoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            liberacaoAbastecimentoAutomatizadoIntegracao.ProblemaIntegracao = "TIMEOUT: Tempo de 2 minutos expirado";
            liberacaoAbastecimentoAutomatizadoIntegracao.LiberacaoAbastecimentoAutomatizado.SituacaoAbastecimento = SituacaoIntegracaoAbastecimento.ProblemaAbastecimento;

            repLiberacaoAbastecimentoAutomatizadoIntegracao.Atualizar(liberacaoAbastecimentoAutomatizadoIntegracao);
        }
        #endregion
    }
}
