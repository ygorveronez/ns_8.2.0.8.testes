namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoServicoCarga
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
        SVMTerceiro = 7,
        /// <summary>
        /// Quando a carga é do tipo Feeder
        /// </summary>
        Feeder = 8,
        NaoInformado = 99
    }

    public static class TipoServicoCargaHelper
    {
        public static string ObterDescricao(this TipoServicoCarga tipo)
        {
            switch (tipo)
            {
                case TipoServicoCarga.Normal: return "Normal";
                case TipoServicoCarga.SubContratada: return "Sub Contratada";
                case TipoServicoCarga.NormalESubContratada: return "Normal e Sub Contratada";
                case TipoServicoCarga.Redespacho: return "Redespacho";
                case TipoServicoCarga.RedespachoIntermediario: return "Redesp. Intermediário";
                case TipoServicoCarga.SVMProprio: return "SVM Próprio";
                case TipoServicoCarga.SVMTerceiro: return "SVM Terceiro";
                case TipoServicoCarga.Feeder: return "Feeder";
                case TipoServicoCarga.NaoInformado: return "Não informado";
                default: return string.Empty;
            }
        }
    }
}
