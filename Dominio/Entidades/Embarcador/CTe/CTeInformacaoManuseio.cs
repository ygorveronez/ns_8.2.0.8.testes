using System;

namespace Dominio.Entidades.Embarcador.CTe
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_INFORMACAO_MANUSEIO", EntityName = "CTeInformacaoManuseio", Name = "Dominio.Entidades.Embarcador.CTe.CTeInformacaoManuseio", NameType = typeof(CTeInformacaoManuseio))]
    public class CTeInformacaoManuseio : EntidadeBase, IEquatable<CTeInformacaoManuseio>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InformacaoManuseio", Column = "CIM_INFORMACAO_MANUSEIO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.InformacaoManuseio), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.InformacaoManuseio InformacaoManuseio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        public virtual bool Equals(CTeInformacaoManuseio other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
