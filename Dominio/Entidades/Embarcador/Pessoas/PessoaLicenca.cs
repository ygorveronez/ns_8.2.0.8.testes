using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PESSOA_LICENCA", EntityName = "PessoaLicenca", Name = "Dominio.Entidades.Embarcador.Pessoas.MotoristaLicenca", NameType = typeof(PessoaLicenca))]
    public class PessoaLicenca : EntidadeBase, IEquatable<PessoaLicenca>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PLI_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PLI_NUMERO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "PLI_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "PLI_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [Obsolete("Migrado, utilizar a lista FormasAlerta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAlerta", Column = "PLI_FORMA_ALERTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FormaAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "PLI_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Licenca", Column = "LIC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Configuracoes.Licenca Licenca { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FormasAlerta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PESSOA_LICENCA_FORMA_ALERTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PLI_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "PLI_FORMA_ALERTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma> FormasAlerta { get; set; }

        public virtual bool Equals(PessoaLicenca other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
