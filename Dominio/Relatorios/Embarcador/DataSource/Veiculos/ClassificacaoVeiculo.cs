namespace Dominio.Relatorios.Embarcador.DataSource.Veiculos
{
    public class ClassificacaoVeiculo
    {
        public virtual int Codigo { get; set; }
        public virtual string Placa { get; set; }
        public virtual string RENAVAM { get; set; }
        public virtual decimal CapacidadeKG { get; set; }
        public virtual decimal CapacidadeM3 { get; set; }
        public virtual decimal Tara { get; set; }
        public virtual string Estado { get; set; }
        public virtual string TipoVeiculo { get; set; }
        public virtual string TipoRodado { get; set; }
        public virtual string TipoCarroceria { get; set; }
        public virtual string TipoPropriedade { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual string ModeloVeiculo { get; set; }
        public virtual decimal PercentualAdicionalFrete { get; set; }
        public virtual string ModeloCarroceria { get; set; }
        public virtual string Transportador { get; set; }
        public virtual string CNPJTransportador { get; set; }
        public virtual string DescricaoAtivo
        {
            get
            {
                if (Ativo)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }
        public virtual string DescricaoTipoVeiculo
        {
            get
            {
                switch (TipoVeiculo)
                {
                    case "0":
                        return "Tração";
                    case "1":
                        return "Reboque";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoTipoRodado
        {
            get
            {
                switch (TipoRodado)
                {
                    case "00":
                        return "Não Aplicado";
                    case "01":
                        return "Truck";
                    case "02":
                        return "Toco";
                    case "03":
                        return "Cavalo";
                    case "04":
                        return "Van";
                    case "05":
                        return "Utilitário";
                    case "06":
                        return "Outros";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoTipoCarroceria
        {
            get
            {
                switch (TipoCarroceria)
                {
                    case "00":
                        return "Não Aplicado";
                    case "01":
                        return "Aberta";
                    case "02":
                        return "Fechada/Baú";
                    case "03":
                        return "Granel";
                    case "04":
                        return "Porta Container";
                    case "05":
                        return "Sider";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoTipoPropriedade
        {
            get
            {
                switch (TipoPropriedade)
                {
                    case "P":
                        return "Próprio";
                    case "T":
                        return "Terceiro";
                    default:
                        return "";
                }
            }
        }
    }
}
