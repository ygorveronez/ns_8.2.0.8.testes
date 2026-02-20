using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DADOS_RECEBEDOR", EntityName = "DadosRecebedor", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.DadosRecebedor", NameType = typeof(DadosRecebedor))]
    public class DadosRecebedor : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DRE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "DRE_NOME", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPF", Column = "DRE_CPF", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "DRE_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCompatibilidadeFoto", Column = "DRE_PERCENTUAL_COMPATIBILIDADE_FOTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? PercentualCompatibilidadeFoto { get; set; }

        // Guarda o nome do arquivo que contém a foto do recebedor
        [NHibernate.Mapping.Attributes.Property(0, Column = "DRE_GUID_FOTO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidFoto { get; set; }


        public virtual bool TemFotoRecebedor
        {
            get
            {
                return GuidFoto != null;
            }
        }

    }
}
