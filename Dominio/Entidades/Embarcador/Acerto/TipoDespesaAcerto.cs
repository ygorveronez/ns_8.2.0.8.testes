using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DESPESA",EntityName = "TipoDespesaAcerto", Name = "Dominio.Entidades.Embarcador.Acerto.TipoDespesaAcerto",NameType = typeof(TipoDespesaAcerto))]
    public class TipoDespesaAcerto : EntidadeBase , IEquatable<TipoDespesaAcerto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TID_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TID_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TID_OBSERVACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TID_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDeDespesa", Column = "TID_TIPO_DESPESA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa? TipoDeDespesa { get; set; }

        public virtual string DescricaoTipoDespesa
        {
            get
            {
                switch (this.TipoDeDespesa)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa.Geral:
                        return "Despesa Geral";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDespesa.Alimentacao:
                        return "Despesa Com Alimentação";
                    default:
                        return "";
                }
            }
        } 
        public virtual  bool Equals(TipoDespesaAcerto other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
