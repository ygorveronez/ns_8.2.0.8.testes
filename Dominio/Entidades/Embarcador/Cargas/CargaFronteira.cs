using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    /*
     * Entidade N:M entre Carga de Fronteiras (Que são Clientes, mas com a flag Fronteira ativada)
     */
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_FRONTEIRA", EntityName = "CargaFronteira", Name = "Dominio.Entidades.Embarcador.Cargas.CargaFronteira", NameType = typeof(CargaFronteira))]
    public class CargaFronteira : EntidadeBase, IEquatable<CargaFronteira>
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Fronteira { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.Codigo.ToString();
            }
        }

        public virtual bool Equals(CargaFronteira other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Fronteira.CPF_CNPJ.GetHashCode();
        }

        public virtual Dominio.Entidades.Embarcador.Cargas.CargaFronteira Clonar()
        {
            return (Dominio.Entidades.Embarcador.Cargas.CargaFronteira)this.MemberwiseClone();
        }
    }
}
