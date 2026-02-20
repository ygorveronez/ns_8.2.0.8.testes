using System;

namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILIAL_SUPRIMENTO_DE_GAS", EntityName = "FilialSuprimentoDeGas", Name = "Dominio.Entidades.Embarcador.Filiais.FilialSuprimentoDeGas", NameType = typeof(FilialSuprimentoDeGas))]
    public class FilialSuprimentoDeGas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FSG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
                    
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SuprimentoDeGas", Column = "SDG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual SuprimentoDeGas SuprimentoDeGas { get; set; }
    
        [Obsolete("Passou a ser por Cliente", true)]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }
    }
}
