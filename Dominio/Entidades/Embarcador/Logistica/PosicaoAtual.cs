using System;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_POSICAO_ATUAL", EntityName = "PosicaoAtual", Name = "Dominio.Entidades.Embarcador.Logistica.PosicaoAtual", NameType = typeof(PosicaoAtual))]
    public class PosicaoAtual : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "POA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual Int64 Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "POA_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Latitude", Column = "POA_LATITUDE", TypeType = typeof(double), NotNull = true)]
        public virtual double Latitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Longitude", Column = "POA_LONGITUDE", TypeType = typeof(double), NotNull = true)]
        public virtual double Longitude { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Velocidade", Column = "POA_VELOCIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Velocidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ignicao", Column = "POA_IGNICAO", TypeType = typeof(int), NotNull = true)]
        public virtual int Ignicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDEquipamento", Column = "POA_ID_EQUIPAMENTO", TypeType = typeof(string), Length = 30, NotNull = true)]
        public virtual string IDEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "POA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVeiculo", Column = "POA_DATA_VEICULO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "POA_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "POA_TEMPERATURA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal? Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelBateria", Column = "POA_NIVEL_BATERIA", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal NivelBateria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NivelSinalGPS", Column = "POA_NIVEL_SINAL_GPS", TypeType = typeof(decimal), Scale = 2, Precision = 10, NotNull = false)]
        public virtual decimal NivelSinalGPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "POA_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Posicao", Column = "POS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.Posicao Posicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SensorTemperatura", Column = "POA_SENSOR_TEMPERATURA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? SensorTemperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmAlvo", Column = "POA_EM_ALVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmAlvo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmLocal", Column = "POA_EM_LOCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? EmLocal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "ClienteAlvo", Class = "Cliente", Column = "CLI_CGCCPF_ALVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente ClienteAlvo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "SubareaAlvo", Class = "SubareaCliente", Column = "SAC_CODIGO_ALVO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.SubareaCliente SubareaAlvo { get; set; }

        public virtual string DescricaoPosicao { get { return $"{Latitude}; {Longitude}"; } }

        public virtual string IgnicaoDescricao
        {
            get
            {
                if (Ignicao == 1)
                    return "Ligado";
                else
                    return "Desligado";
            }
        }

        public virtual bool Equals(PosicaoAtual other)
        {
            if (other == null)
                return false;

            return other.Latitude == Latitude && other.Longitude == Longitude;
        }

    }
}
