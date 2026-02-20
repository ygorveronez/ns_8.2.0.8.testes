using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ROTA_FRETE_FRONTEIRA", EntityName = "RotaFreteFronteira", Name = "Dominio.Entidades.RotaFreteFronteira", NameType = typeof(RotaFreteFronteira))]
    public class RotaFreteFronteira : EntidadeBase, IEquatable<RotaFreteFronteira>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.RotaFrete RotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFF_ORDEM", TypeType = typeof(int), NotNull = false)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoMedioPermanenciaFronteira", Column = "RFF_TEMPO_MEDIO_PERMANENCIA_FRONTEIRA", TypeType = typeof(long), NotNull = false)]
        public virtual long TempoMedioPermanenciaFronteira { get; set; }

        public virtual bool Equals(RotaFreteFronteira other)
        {
            if (Cliente.CPF_CNPJ == other.Cliente.CPF_CNPJ && Ordem == other.Ordem)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            int hashCliente = Cliente.CPF_CNPJ == null ? 0 : Cliente.CPF_CNPJ.GetHashCode();
            int hashOrdem = Ordem == null ? 0 : Ordem.GetHashCode();

            return hashCliente ^ hashOrdem;
        }
    }
}
