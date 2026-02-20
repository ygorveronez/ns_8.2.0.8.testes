using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas.Ofertas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PARAMETROS_OFERTAS_DADOS_OFERTA_DIA_SEMANA", EntityName = "ParametrosOfertasDadosOfertaDiaSemana", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Cargas.ParametrosOfertasDadosOfertaDiaSemana", NameType = typeof(ParametrosOfertasDadosOfertaDiaSemana))]
    public class ParametrosOfertasDadosOfertaDiaSemana : EntidadeBase
    {
        public virtual string Descricao
        {
            get { return DiaSemana.ObterDescricao(); }
        }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametrosOfertasDadosOferta", Column = "PDO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ParametrosOfertasDadosOferta ParametrosOfertasDadosOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PDS_DIA_SEMANA", TypeType = typeof(DiaSemana), NotNull = true)]
        public virtual DiaSemana DiaSemana { get; set; }
    }
}
