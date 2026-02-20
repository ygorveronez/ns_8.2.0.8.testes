using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALERTA", EntityName = "Alerta", Name = "Dominio.Entidades.Embarcador.Configuracoes.Alerta", NameType = typeof(Alerta))]
    public class Alerta : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Configuracoes.Alerta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ALE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEntidade", Column = "ALE_CODIGO_ENTIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoEntidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALE_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALE_TELA_ALERTA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela TelaAlerta { get; set; }

        [Obsolete("Migrado, utilizar a lista FormasAlerta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAlerta", Column = "ALE_FORMAS_ALERTA", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FormaAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ocultar", Column = "ALE_OCULTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ocultar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ALE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FormasAlerta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALERTA_FORMA_ALERTA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ALE_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "ALE_FORMA_ALERTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma> FormasAlerta { get; set; }

        public virtual bool Equals(Alerta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
