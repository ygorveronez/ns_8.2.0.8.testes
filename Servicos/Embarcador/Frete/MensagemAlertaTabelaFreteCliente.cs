namespace Servicos.Embarcador.Frete
{
    public sealed class MensagemAlertaTabelaFreteCliente : Alertas.MensagemAlerta<Dominio.Entidades.Embarcador.Frete.MensagemAlertaTabelaFreteCliente, Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>
    {
        #region Construtores

        public MensagemAlertaTabelaFreteCliente(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public MensagemAlertaTabelaFreteCliente(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, auditado) { }

        #endregion Construtores
    }
}
