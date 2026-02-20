namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class MensagemAlertaFluxoGestaoPatio : Alertas.MensagemAlerta<Dominio.Entidades.Embarcador.GestaoPatio.MensagemAlertaFluxoGestaoPatio, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio>
    {
        #region Construtores

        public MensagemAlertaFluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public MensagemAlertaFluxoGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, auditado) { }

        #endregion Construtores
    }
}
