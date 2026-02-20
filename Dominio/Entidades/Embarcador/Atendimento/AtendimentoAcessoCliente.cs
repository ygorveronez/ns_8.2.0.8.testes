using System;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO_ACESSO_CLIENTE", EntityName = "AtendimentoAcessoCliente", Name = "Dominio.Entidades.Embarcador.Atendimento.AtendimentoAcessoCliente", NameType = typeof(AtendimentoAcessoCliente))]
    public class AtendimentoAcessoCliente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoAcessoCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ID", Column = "AAC_ID", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ID { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "AAC_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAcessoRemoto", Column = "AAC_TIPO_ACESSO_REMOTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoRemoto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoRemoto TipoAcessoRemoto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_FILHO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa EmpresaFilho { get; set; }

        public virtual bool Equals(AtendimentoAcessoCliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
