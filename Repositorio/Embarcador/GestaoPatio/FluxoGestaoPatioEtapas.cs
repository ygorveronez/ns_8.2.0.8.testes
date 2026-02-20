using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class FluxoGestaoPatioEtapas : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>
    {
        #region Construtores

        public FluxoGestaoPatioEtapas(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> BuscarParaGerarAlertaSla(int codigoFilial, List<EtapaFluxoGestaoPatio> etapas)
        {
            var consultaEtapas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>()
                .Where(o =>
                    o.DataAlertaTempoExcedido.HasValue == false &&
                    etapas.Contains(o.EtapaFluxoGestaoPatio) &&
                    o.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando &&
                    o.FluxoGestaoPatio.EtapaAtual == o.Ordem &&
                    o.FluxoGestaoPatio.Filial.Codigo == codigoFilial 
                );

            return consultaEtapas.ToList();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas> BuscarPorGestao(int gestao)
        {
            var listaFluxoGestaoPatioEtapas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>()
                .Where(o => o.FluxoGestaoPatio.Codigo == gestao)
                .OrderBy(o => o.Ordem)
                .ToList();

            return listaFluxoGestaoPatioEtapas;
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas BuscarPorGestaoEEtapa(int gestao, EtapaFluxoGestaoPatio etapa)
        {
            var consultaFluxoGestaoPatioEtapas = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioEtapas>()
               .Where(o => o.FluxoGestaoPatio.Codigo == gestao && o.EtapaFluxoGestaoPatio == etapa);

            return consultaFluxoGestaoPatioEtapas.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
