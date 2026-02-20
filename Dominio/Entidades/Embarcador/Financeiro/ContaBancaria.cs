using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTA_BANCARIA", EntityName = "ContaBancaria", Name = "Dominio.Entidades.Embarcador.Financeiro.ContaBancaria", NameType = typeof(ContaBancaria))]
    public class ContaBancaria : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ContaBancaria>, Interfaces.Embarcador.Entidade.IEntidade
    {
        public ContaBancaria()
        {
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_PORTADOR_CONTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClientePortadorConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "COB_BANCO_AGENCIA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "COB_BANCO_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "COB_BANCO_NUMERO_CONTA", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "COB_BANCO_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePix", Column = "COB_TIPO_CHAVE_PIX", TypeType = typeof(TipoChavePix), NotNull = false)]
        public virtual TipoChavePix? TipoChavePix { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COB_CHAVE_PIX", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string ChavePix { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "COB_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroConta.ToString();
            }
        }

        public virtual bool Equals(ContaBancaria other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
