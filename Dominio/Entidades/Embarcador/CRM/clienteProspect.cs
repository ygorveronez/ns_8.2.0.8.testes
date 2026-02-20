using System;

namespace Dominio.Entidades.Embarcador.CRM
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CLIENTE_PROSPECT", EntityName = "ClienteProspect", Name = "Dominio.Entidades.Embarcador.CRM.ClienteProspect", NameType = typeof(ClienteProspect))]
    public class ClienteProspect : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPR_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPR_CNPJ", TypeType = typeof(string), Length = 20, NotNull = true)]
        public virtual string CNPJ { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Cidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPR_TIPO_CONTATO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento TipoContato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPR_CONTATO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Contato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPR_EMAIL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CPR_TELEFONE", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Telefone { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Nome + (this.Cidade != null ? " (" + this.Cidade.DescricaoCidadeEstado + ")" : string.Empty);
            }
        }

        public virtual string CNPJ_Formatado
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.CNPJ) ? String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJ.ObterSomenteNumeros())) : string.Empty;
            }
        }

        public virtual string CNPJ_SemFormato
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.CNPJ) ? String.Format(@"{0:00000000000000}", long.Parse(this.CNPJ)) : string.Empty;
            }
        }
    }
}
