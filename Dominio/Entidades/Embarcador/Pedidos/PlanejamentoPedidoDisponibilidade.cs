using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_PLANEJAMENTO_PEDIDO_DISPONIBILIDADE", EntityName = "PlanejamentoPedidoDisponibilidade", Name = "Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade", NameType = typeof(PlanejamentoPedidoDisponibilidade))]
    public class PlanejamentoPedidoDisponibilidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PPD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PPD_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "PPD_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusVeiculo", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus), Column = "PPD_STATUS_VEICULO", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido StatusVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusMotorista", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus), Column = "PPD_STATUS_MOTORISTA", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido StatusMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusViagem", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus), Column = "PPD_STATUS_VIAGEM", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido StatusViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Localizacao", Column = "PPD_LOCALIZACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Localizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PPD_OBSERVACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFrota", Column = "PPD_NUMERO_FROTA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NumeroFrota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Disponivel", Column = "PPD_DISPONIVEL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Disponivel { get; set; }

        public virtual int NumeroFrotaNumero
        {
            get
            {
                int.TryParse(NumeroFrota, out int numero);
                return numero;
            }
        }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido Status
        {
            get
            {

                bool Indispoonivel = (StatusMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido.Indisponivel) ||
                                     (StatusViagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido.Indisponivel) ||
                                     (StatusVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido.Indisponivel);

                return Indispoonivel ? ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido.Indisponivel : ObjetosDeValor.Embarcador.Enumeradores.StatusDisponibilidadePlanejamentoPedido.Disponivel;
            }
        }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
