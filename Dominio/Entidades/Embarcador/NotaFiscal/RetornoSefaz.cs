using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RETORNO_SEFAZ", EntityName = "RetornoSefaz", Name = "Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz", NameType = typeof(RetornoSefaz))]
    public class RetornoSefaz : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.RetornoSefaz>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RES_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetornoSefaz", Column = "RES_MENSAGEM_RETORNO_SEFAZ", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string MensagemRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AbreviacaoRetornoSefaz", Column = "RES_ABREVIACAO_RETORNO_SEFAZ", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string AbreviacaoRetornoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "RES_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        public virtual string DescricaoStatus
        {
            get { return Status ? "Ativo" : "Inativo"; }
        }

        public virtual string Descricao
        {
            get { return MensagemRetornoSefaz; }
        }

        public virtual bool Equals(RetornoSefaz other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
