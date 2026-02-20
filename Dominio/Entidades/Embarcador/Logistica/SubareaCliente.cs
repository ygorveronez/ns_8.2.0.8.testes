using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SUBAREA_CLIENTE", EntityName = "SubareaCliente", Name = "Dominio.Entidades.Embarcador.Logistica.SubareaCliente", NameType = typeof(SubareaCliente))]
    public class SubareaCliente : EntidadeBase
    {
        public SubareaCliente()
        {
            DataCadastro = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SAC_DESCRICAO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "SAC_AREA", Type = "StringClob", NotNull = true)]
        public virtual string Area { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "SAC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "SAC_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "TipoSubarea", Class = "TipoSubareaCliente", Column = "TSA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.TipoSubareaCliente TipoSubarea { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Cliente", Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTag", Column = "SAC_CODIGO_TAG", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoTag { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AcoesFluxoPatio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SUBAREA_CLIENTE_ACOES_FLUXO_DE_PATIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "SAC_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "SubareaClienteAcoesFluxoDePatio")]
        public virtual IList<SubareaClienteAcoesFluxoDePatio> AcoesFluxoPatio { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
    }
}
