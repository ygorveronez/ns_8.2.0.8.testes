using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.VLI
{
    public class RetornoIntegracaoRastreio
    {
        public Data Data { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class Carregamento
    {
        public string RazaoSocialCorrentista { get; set; }
        public string CNPJCorrentista { get; set; }
        public string RazaoSocialRemetente { get; set; }
        public string CNPJRemetente { get; set; }
        public string RazaoSocialDestinatario { get; set; }
        public string CNPJDestinatario { get; set; }
        public string Produto { get; set; }
        public double VolumeVagao { get; set; }
        public string SiglaOrigem { get; set; }
        public string NomeOrigem { get; set; }
        public string EstadoOrigem { get; set; }
        public string CidadeOrigem { get; set; }
        public string SiglaDestino { get; set; }
        public string NomeDestino { get; set; }
        public string EstadoDestino { get; set; }
        public string CidadeDestino { get; set; }
        public DateTime DataDespacho { get; set; }
        public List<NotasFiscais> NotasFiscais { get; set; }
    }

    public class Data
    {
        public List<ListaRastreio> ListaRastreio { get; set; }
    }

    public class ListaRastreio
    {
        public string Trem { get; set; }
        public int CodigoVagao { get; set; }
        public string TipoVagao { get; set; }
        public string Lotacao { get; set; }
        public string Status { get; set; }
        public List<Carregamento> Carregamentos { get; set; }
        public string SiglaEstacaoAtual { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime? DataHoraUltimaAtualizacao { get; set; }
        public DateTime? DataHoraUltimoEvento { get; set; }
        public DateTime? PrevisaoCarregamento { get; set; }
        public DateTime? PrevisaoDescarregamento { get; set; }
        public DateTime? DataHoraChegadaTrem { get; set; }
        public DateTime? DataHoraPartidaTrem { get; set; }
        public DateTime? DataHoraPrevisaoChegadaTrem { get; set; }
        public DateTime? DataHoraEntradaLinha { get; set; }
        public DateTime? DataHoraInicioCarregamento { get; set; }
        public DateTime? DataHoraFimCarregamento { get; set; }
    }

    public class NotasFiscais
    {
        public string ChaveNotaFiscal { get; set; }
        public double PesoNotaFiscal { get; set; }
        public double PesoRateio { get; set; }
    }

}
