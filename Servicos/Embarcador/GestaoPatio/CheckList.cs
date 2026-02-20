using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class CheckList : CheckListBase
    {
        #region Construtores

        public CheckList(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, cliente: null) { }

        public CheckList(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(unitOfWork, auditado, cliente, EtapaFluxoGestaoPatio.CheckList) { }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override void AtualizarDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            if (preSetTempoEtapa.DataFimCheckListPrevista.HasValue)
                fluxoGestaoPatio.DataFimCheckListPrevista = preSetTempoEtapa.DataFimCheckListPrevista.Value;
        }

        public override void DefinirDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime dataPrevista, Dominio.ObjetosDeValor.Embarcador.GestaoPatio.PreSetTempoEtapa preSetTempoEtapa)
        {
            fluxoGestaoPatio.DataFimCheckListPrevista = preSetTempoEtapa.DataFimCheckListPrevista ?? dataPrevista;
        }

        public override bool DefinirTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, DateTime data, decimal tempoEtapaAnterior)
        {
            if (fluxoGestaoPatio.DataFimCheckList.HasValue)
                return false;

            fluxoGestaoPatio.TempoAgChecklist = tempoEtapaAnterior;
            fluxoGestaoPatio.DataFimCheckList = data;

            return true;
        }

        public override DateTime? ObterData(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimCheckList;
        }

        public override DateTime? ObterDataPrevista(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return fluxoGestaoPatio.DataFimCheckListPrevista;
        }

        public override void RemoverTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            fluxoGestaoPatio.TempoAgChecklist = 0;
            fluxoGestaoPatio.DataFimCheckList = null;

            Repositorio.Embarcador.GestaoPatio.CheckListCarga repositorioChecklist = new Repositorio.Embarcador.GestaoPatio.CheckListCarga(_unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.CheckListCarga checklist = ObterCheckList(fluxoGestaoPatio);

            if (checklist != null)
            {
                checklist.Situacao = SituacaoCheckList.Aberto;
                repositorioChecklist.Atualizar(checklist);
            }
        }

        public override void ReprogramarTempo(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, TimeSpan tempoReprogramar)
        {
            if (fluxoGestaoPatio.DataFimCheckListPrevista.HasValue)
                fluxoGestaoPatio.DataFimCheckListReprogramada = fluxoGestaoPatio.DataFimCheckListPrevista.Value.Add(tempoReprogramar);
        }

        #endregion
    }
}