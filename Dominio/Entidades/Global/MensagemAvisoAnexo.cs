

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MENSAGEM_AVISO_ANEXO", EntityName = "MensagemAvisoAnexo", Name = "Dominio.Entidades.MensagemAvisoAnexo", NameType = typeof(MensagemAvisoAnexo))]
    public class MensagemAvisoAnexo : Dominio.Entidades.Embarcador.Anexo.Anexo<MensagemAviso>
    {
        #region Propriedades Sobrescritas
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MensagemAviso", Column = "MAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override MensagemAviso EntidadeAnexo { get; set; }
        #endregion
    }
}

