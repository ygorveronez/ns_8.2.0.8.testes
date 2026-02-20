using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PLANEJAMENTO_PEDIDO_DISPONIBILIDADE_MOTORISTA", EntityName = "PlanejamentoPedidoDisponibilidadeMotorista", Name = "Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista", NameType = typeof(PlanejamentoPedidoDisponibilidadeMotorista))]
    public class PlanejamentoPedidoDisponibilidadeMotorista : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PDM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PDM_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "PDM_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PDM_OBSERVACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PDM_NOME", TypeType = typeof(string), Length = 80, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Disponivel", Column = "PDM_DISPONIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Disponivel { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
