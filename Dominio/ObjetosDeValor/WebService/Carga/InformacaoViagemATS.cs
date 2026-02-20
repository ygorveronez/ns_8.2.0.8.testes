using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class InformacaoViagemATS
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public DadosViagemATS DadosViagem { get; set; }
    }

    public class DadosViagemATS
    {
        public string Cliente { get; set; }
        public string CodigoExterno { get; set; }
        public string Condutor { get; set; }
        public DateTime DataHoraCadastro { get; set; }
        public DateTime DataHoraCadastroSolicitacaoMonitoramento { get; set; }
        public DateTime DataHoraEstimadaChegada { get; set; }
        public DateTime DataHoraFim { get; set; }
        public DateTime DataHoraPrevisaoFimViagem { get; set; }
        public DateTime DataHoraPrevisaoInicioViagem { get; set; }
        public double DistanciaPercorridaRota { get; set; }
        public double DistanciaProximoPonto { get; set; }
        public double DistanciaRestanteRota { get; set; }
        public string Frota { get; set; }
        public string Operacao { get; set; }
        public int PercentualProgresso { get; set; }
        public string Placa { get; set; }
        public string PlanoViagem { get; set; }
        public string PrimeiraCarreta { get; set; }
        public string Produto { get; set; }
        public string Proprietario { get; set; }
        public double RIPA { get; set; }
        public string RotaViagem { get; set; }
        public string SegundaCarreta { get; set; }
        public string StatusViagem { get; set; }
        public string TerceiraCarreta { get; set; }
        public double TotalKMCarregado { get; set; }
        public double TotalKMVazio { get; set; }
        public int TotalMinutosCarregado { get; set; }
        public int TotalMinutosVazio { get; set; }
        public string UsuarioCadastro { get; set; }
        public string UsuarioFinalizacao { get; set; }
    }

}
