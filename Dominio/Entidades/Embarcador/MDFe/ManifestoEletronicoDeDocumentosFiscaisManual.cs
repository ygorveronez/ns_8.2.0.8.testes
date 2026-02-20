using System;

namespace Dominio.Entidades.Embarcador.MDFe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_MANUAL", EntityName = "ManifestoEletronicoDeDocumentosFiscaisManual", Name = "Dominio.Entidades.Embarcador.MDFe.ManifestoEletronicoDeDocumentosFiscaisManual", NameType = typeof(ManifestoEletronicoDeDocumentosFiscaisManual))]
    public class ManifestoEletronicoDeDocumentosFiscaisManual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "MDM_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "MDM_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_CARREGAMENTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_DESCARREGAMENTO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDescarregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInformacaoManual", Column = "MDF_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInformacaoManual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoOperador", Column = "MDM_OBSERVACAO_OPERADOR", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string ObservacaoOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MDFeInformado", Column = "MDM_MDFE_INFORMADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool MDFeInformado { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString();
            }
        }
    }
}
