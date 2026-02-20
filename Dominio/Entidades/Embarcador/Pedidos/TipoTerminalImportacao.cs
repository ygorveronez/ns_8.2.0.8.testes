using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_TERMINAL_IMPORTACAO", EntityName = "TipoTerminalImportacao", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao", NameType = typeof(TipoTerminalImportacao))]
    public class TipoTerminalImportacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.TipoTerminalImportacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TTI_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TTI_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTerminal", Column = "TTI_CODIGO_TERMINAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoTerminal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Terminal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Porto Porto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TTI_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDocumento", Column = "TTI_CODIGO_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoMercante", Column = "TTI_CODIGO_MERCANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoMercante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasEnvioDocumentacao", Column = "TTI_QUANTIDADE_DIAS_ENVIO_DOCUMENTACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasEnvioDocumentacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoObservacaoContribuinte", Column = "TTI_CODIGO_OBS_CONTRIBUINTE", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CodigoObservacaoContribuinte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integrado", Column = "TTI_INTEGRADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Integrado { get; set; }

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

        public virtual bool Equals(TipoTerminalImportacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
