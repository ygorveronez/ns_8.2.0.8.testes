using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class RetornoAbastecimentoAngellira
    {
        public int Codigo { get; set; }
        public DateTime DataConsulta { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Retorno { get; set; }
        public string Placa { get; set; }
        public string Frota { get; set; }
        public string PlacaRetorno { get; set; }
        public string CondutorRetorno { get; set; }
        public DateTime DataRetorno { get; set; }
        public string CordenadaRetorno { get; set; }
        public string LatitudeRetorno { get; set; }
        public string LontitudeRetorno { get; set; }
        public int Odometro { get; set; }
        public int CodigoAbastecimento { get; set; }
        public string SituacaoAbastecimento { get; set; }
        public string NomeMotorista { get; set; }
        public string CPFMotorista { get; set; }
        public string TipoPosto { get; set; }
        public double CNPJCPFPosto { get; set; }
        public string NomePosto { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        public string DescricaoSituacaoIntegracao
        {
            get { return SituacaoIntegracao.ObterDescricao(); }
        }

        public virtual string CpfCnpjFornecedorFormatado
        {
            get
            {
                return CNPJCPFPosto > 0 ? TipoPosto.Equals("E") ? "00.000.000/0000-00" :
                    TipoPosto.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPFPosto) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPFPosto) : string.Empty;
            }
        }

        public string DataConsultaFormatada
        {
            get { return DataConsulta != DateTime.MinValue ? DataConsulta.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataInicialFormatada
        {
            get { return DataInicial != DateTime.MinValue ? DataInicial.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFinalFormatada
        {
            get { return DataFinal != DateTime.MinValue ? DataFinal.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetornoFormatada
        {
            get { return DataRetorno != DateTime.MinValue ? DataRetorno.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
    }
}
