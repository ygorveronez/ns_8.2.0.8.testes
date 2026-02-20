using System;

namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_CONTATO", EntityName = "FuncionarioContato", Name = "Dominio.Entidades.Embarcador.Usuarios.FuncionarioContato", NameType = typeof(FuncionarioContato))]
    public class FuncionarioContato : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "CFO_NOME", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "CFO_EMAIL", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "CFO_TELEFONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParentesco", Column = "CFO_TIPO_PARENTESCO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoParentesco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoParentesco TipoParentesco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "CFO_CPF", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataNascimento", Column = "CFO_DATA_NASCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataNascimento { get; set; }

        public virtual string Descricao
        {
            get { return this.Nome; }
        }

        public virtual string CPF_Formatado
        {
            get { return !string.IsNullOrWhiteSpace(CPF) ? string.Format(@"{0:000\.000\.000\-00}", CPF.ToLong()) : string.Empty; }
        }
    }
}
