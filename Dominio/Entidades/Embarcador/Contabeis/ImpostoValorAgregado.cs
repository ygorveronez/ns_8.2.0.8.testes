using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Contabeis
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_IMPOSTO_SOBRE_VALOR_AGREGADO", EntityName = "ImpostoValorAgregado", Name = "Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado", NameType = typeof(ImpostoValorAgregado))]
    public class ImpostoValorAgregado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IVA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IVA_CODIGO_IVA", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string CodigoIVA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IVA_ICMS_MAIOR_QUE_ZERO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImpostoMaiorQueZero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IVA_DESTINATARIO_EXTERIOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DestinatarioExterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IVA_PERMITIR_INFORMAR_MANUALMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PermitirInformarManualmente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsoMaterial", Column = "IVA_USO_MATERIAL", TypeType = typeof(UsoMaterial), NotNull = false)]
        public virtual UsoMaterial UsoMaterial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        public virtual string Descricao
        {
            get { return CodigoIVA; }
        }
    }
}
