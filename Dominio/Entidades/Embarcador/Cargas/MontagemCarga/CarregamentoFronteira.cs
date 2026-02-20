using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    /*
     * Entidade N:M entre carregamentos de Fronteiras (Que são Clientes, mas com a flag Fronteira ativada)
     */
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARREGAMENTO_FRONTEIRA", EntityName = "CarregamentoFronteira", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira", NameType = typeof(CarregamentoFronteira))]
    public class CarregamentoFronteira : EntidadeBase, IEquatable<CarregamentoFronteira>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carregamento", Column = "CRG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento Carregamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fronteira { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(CarregamentoFronteira other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
