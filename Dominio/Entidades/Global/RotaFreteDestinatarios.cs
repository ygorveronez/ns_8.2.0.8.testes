using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_DESTINATARIO_ORDEM", EntityName = "RotaFreteDestinatarios", Name = "Dominio.Entidades.RotaFreteDestinatarios", NameType = typeof(RotaFreteDestinatarios))]
    public class RotaFreteDestinatarios : EntidadeBase, IEquatable<RotaFreteDestinatarios>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFD_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ClienteOutroEndereco", Column = "COE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco ClienteOutroEndereco { get; set; }

        public virtual bool Equals(RotaFreteDestinatarios other)
        {
            if (Cliente.CPF_CNPJ == other.Cliente.CPF_CNPJ && Ordem == other.Ordem)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashCliente = Cliente == null ? 0 : Cliente.CPF_CNPJ.GetHashCode();
            int hashOrdem = Ordem.GetHashCode();

            return hashCliente ^ hashOrdem;
        }
    }
}
