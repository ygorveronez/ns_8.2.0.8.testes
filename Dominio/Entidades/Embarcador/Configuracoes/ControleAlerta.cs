using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_ALERTA", EntityName = "ControleAlerta", Name = "Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta", NameType = typeof(ControleAlerta))]
    public class ControleAlerta : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Configuracoes.ControleAlerta>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDias", Column = "CAL_QUANTIDADE_DIAS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDias { get; set; }

        [Obsolete("Migrado, utilizar a lista FormasAlerta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "FormaAlerta", Column = "CAL_FORMAS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string FormaAlerta { get; set; }

        [Obsolete("Migrado, utilizar a lista TelasAlerta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "TelaAlerta", Column = "CAL_TELAS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string TelaAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasAlertaOsInterna", Column = "CAL_QUANTIDADE_DIAS_ALERTA_OS_INTERNA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasAlertaOsInterna { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDiasAlertaOsExterna", Column = "CAL_QUANTIDADE_DIAS_ALERTA_OS_EXTERNA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeDiasAlertaOsExterna { get; set; }

        [Obsolete("Migrado, utilizar a lista SituacoesOS")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CAL_SITUACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CAL_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "FormasAlerta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_ALERTA_FORMA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CAL_FORMA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaForma> FormasAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TelasAlerta", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_ALERTA_TELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CAL_TELA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.ControleAlertaTela> TelasAlerta { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "SituacoesOrdemServico", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONTROLE_ALERTA_SITUACAO_ORDEM_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Element(2, Column = "CAL_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota), NotNull = false)]
        public virtual ICollection<ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota> SituacoesOrdemServico { get; set; }

        public virtual string Descricao
        {
            get { return "Controle Alerta " + Codigo; }
        }

        public virtual bool Equals(ControleAlerta other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
