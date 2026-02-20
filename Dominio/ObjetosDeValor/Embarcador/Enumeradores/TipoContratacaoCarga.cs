namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoContratacaoCarga
    {
        /// <summary>
        /// Quando a operação fica entre o Embarcador e Transportador.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Quando a carga é subcontratada de outro transportador.
        /// </summary>
        SubContratada = 2,
        /// <summary>
        /// Quando a carga é mesclada, uma parte subcontratada e outra parte emissão normal.
        /// </summary>
        NormalESubContratada = 3,
        /// <summary>
        /// Quando a carga é redespachada para outro transportador.
        /// </summary>
        Redespacho = 4,
        /// <summary>
        /// Quando a carga está em um redespacho intermediario de outro transportador.
        /// </summary>
        RedespachoIntermediario = 5,
        /// <summary>
        /// Quando a carga é SVM Próprio
        /// </summary>
        SVMProprio = 6,
        /// <summary>
        /// Quando a carga é SVM de Terceiro
        /// </summary>
        SVMTerceiro = 7
    }
}
