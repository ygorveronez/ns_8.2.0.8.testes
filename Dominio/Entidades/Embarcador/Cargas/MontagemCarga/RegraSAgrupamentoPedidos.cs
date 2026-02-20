using System;

namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AGRUPAMENTO_PEDIDO", EntityName = "RegrasAgrupamentoPedidos", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos", NameType = typeof(RegrasAgrupamentoPedidos))]
    public class RegrasAgrupamentoPedidos : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.RegrasAgrupamentoPedidos>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RaioKMEntreCidades", Column = "CAR_RAIO_KM_ENTRE_CIDADES", TypeType = typeof(int), NotNull = false)]
        public virtual int RaioKMEntreCidades { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ToleranciaDiasDiferenca", Column = "CAR_TOLERANCIA_DIAS_DIFERENCAS", TypeType = typeof(int), NotNull = false)]
        public virtual int ToleranciaDiasDiferenca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroMaximoEntregas", Column = "CAR_DIAS_DIFERENCAS_ENTREGAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroMaximoEntregas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FIL_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Filial?.Descricao ?? "Global";
            }
        }

        public virtual string DescricaoAtivo
        {
            get
            {
                if (this.Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(RegrasAgrupamentoPedidos other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
