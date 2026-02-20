using System;

namespace Dominio.Entidades.Embarcador.Guias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GUIA_RECOLHIMENTO_ANEXO", EntityName = "GuiaRecolhimentoAnexo", Name = "Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo", NameType = typeof(GuiaRecolhimentoAnexo))]
    public class GuiaRecolhimentoAnexo : Anexo.Anexo<CTe.GuiaNacionalRecolhimentoTributoEstadual>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GuiaNacionalRecolhimentoTributoEstadual", Column = "GNR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CTe.GuiaNacionalRecolhimentoTributoEstadual EntidadeAnexo { get; set; }

        #endregion

        #region Propriedades Próprias

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAnexo", Column = "ANX_DATA_ANEXO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAnexo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAnexo", Column = "ANX_TIPO_ANEXO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoGuiaRecolhimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoGuiaRecolhimento? TipoAnexo { get; set; }

        #endregion
    }
}
