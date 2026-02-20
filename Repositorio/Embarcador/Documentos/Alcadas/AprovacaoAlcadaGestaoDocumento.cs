namespace Repositorio.Embarcador.Documentos.Alcadas
{
    public sealed class AprovacaoAlcadaGestaoDocumento : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Documentos.Alcadas.AprovacaoAlcadaGestaoDocumento,
        Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento,
        Dominio.Entidades.Embarcador.Documentos.GestaoDocumento
    >
    {
        #region Construtores

        public AprovacaoAlcadaGestaoDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion
    }
}
