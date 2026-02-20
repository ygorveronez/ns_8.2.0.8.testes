using System;

namespace Dominio.Entidades.Embarcador.GestaoPallet
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RECEBIMENTO_VALE_PALLET", EntityName = "RecebimentoValePallet", Name = "Dominio.Entidades.Embarcador.GestaoPallet.RecebimentoValePallet", NameType = typeof(RecebimentoValePallet))]
    public class RecebimentoValePallet : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        public RecebimentoValePallet()
        {
            DataCriacao = DateTime.Now;
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RVP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "RVP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroValePallet", Column = "RVP_NUMERO_VALE_PALLET", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroValePallet { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "RVP_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataVencimento { get; set; }

        public virtual string Descricao
        {
            get { return $"Vale Pallet n° {NumeroValePallet}"; }
        }
    }
}