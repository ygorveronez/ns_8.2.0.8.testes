using System;
using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOTORISTA_DADO_BANCARIO", EntityName = "MotoristaDadoBancario", Name = "Dominio.Entidades.Embarcador.Transportadores.MotoristaDadoBancario", NameType = typeof(MotoristaDadoBancario))]
    public class MotoristaDadoBancario : EntidadeBase, IEquatable<MotoristaDadoBancario>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "MDB_AGENCIA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DigitoAgencia", Column = "MDB_DIGITO_AGENCIA", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string DigitoAgencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroConta", Column = "MDB_NUMERO_CONTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaBanco", Column = "MDB_TIPO_CONTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco TipoContaBanco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoConta", Column = "MDB_OBSERVACAO_CONTA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ObservacaoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Banco", Column = "BCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Banco Banco { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoChavePix", Column = "MDB_TIPO_CHAVE_PIX", TypeType = typeof(TipoChavePix), NotNull = false)]
        public virtual TipoChavePix? TipoChavePix { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MDB_CHAVE_PIX", TypeType = typeof(string), Length = 36, NotNull = false)]
        public virtual string ChavePix { get; set; }

        public virtual string Descricao
        {
            get { return Banco.Descricao + " - Número Conta: " + NumeroConta; }
        }

        public virtual bool Equals(MotoristaDadoBancario other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
