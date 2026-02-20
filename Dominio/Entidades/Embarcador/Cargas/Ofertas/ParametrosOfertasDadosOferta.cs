using System;

namespace Dominio.Entidades.Embarcador.Cargas.Ofertas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETROS_OFERTAS_DADOS_OFERTA", EntityName = "ParametrosOfertasDadosOferta", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.ParametrosOfertasDadosOferta", NameType = typeof(ParametrosOfertasDadosOferta))]
    public class ParametrosOfertasDadosOferta : EntidadeBase, Interfaces.Embarcador.Cargas.Ofertas.IRelacionamentoParametrosOfertas
    {
        public virtual string Descricao
        {
            get { return Raio.ToString(); }
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametrosOfertas", Column = "POF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametrosOfertas ParametrosOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDO_HORA_INICIO", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDO_HORA_TERMINO", TypeType = typeof(TimeSpan), NotNull = true)]
        public virtual TimeSpan HoraTermino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDO_RAIO", TypeType = typeof(int), NotNull = false)]
        public virtual int Raio { get; set; }

    }
}
