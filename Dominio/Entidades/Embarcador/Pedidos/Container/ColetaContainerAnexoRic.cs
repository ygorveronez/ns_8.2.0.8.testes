using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_CONTAINER_ANEXO_RIC", EntityName = "ColetaContainerAnexoRic", Name = "Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic", NameType = typeof(ColetaContainerAnexoRic))]
    public class ColetaContainerAnexoRic : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RIC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_DATACOLETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeColeta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_DESCRICAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string ContainerDescricao { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_TIPOCONTAINER", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string TipoContainer { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_TARACONTAINER", TypeType = typeof(int), NotNull = true)]
        public virtual int TaraContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_ARMADOR", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string ArmadorBooking { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_TRANSPORTADOR", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Transportadora { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_MOTORISTA", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Motorista { get; set; } = string.Empty;

        [NHibernate.Mapping.Attributes.Property(0, Column = "RIC_PLACA", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Placa { get; set; } = string.Empty;

        public virtual string Descricao
        {
            get
            {
                return "RIC do container " + ContainerDescricao;
            }
        }

        public virtual Dominio.ObjetosDeValor.OCR.ObjetoRicRetorno ConverterEmDTO()
        {
            return new Dominio.ObjetosDeValor.OCR.ObjetoRicRetorno()
            {
                ArmadorBooking = this.ArmadorBooking,
                CodigoContainer = this.Container.Codigo,
                Container = this.ContainerDescricao,
                DataDeColeta = this.DataDeColeta.HasValue ? this.DataDeColeta.Value.ToString("dd/MM/yyyy") : string.Empty,
                Motorista = this.Motorista,
                Placa = this.Placa,
                TaraContainer = this.TaraContainer,
                TipoContainer = this.TipoContainer,
                Transportadora = this.Transportadora
            };
        }

    }
}
