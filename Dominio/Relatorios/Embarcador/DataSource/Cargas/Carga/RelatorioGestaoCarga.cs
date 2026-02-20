using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public sealed class RelatorioGestaoCarga
    {
        public string Status { get; set; }
        public string Tipo { get; set; }
        public string Grupo { get; set; }
        public string Filial { get; set; }
        public string ProgVei { get; set; }
        public string Remetente { get; set; }
        public string Origem { get; set; }
        public string PaisOrigem { get; set; }
        public string FronteiraOrigem { get; set; }
        public string FronteiraDestino { get; set; }
        public string Cavalo { get; set; }
        public string Carretas { get; set; }
        public DateTime ChegadaOrigem { get; set; }
        public DateTime SaidaOrigem { get; set; }
        public int TempoOrigem { get; set; }
        public DateTime ChegadaFronteiraOrigem { get; set; }
        public DateTime SaidaFronteiraOrigem { get; set; }
        public int TempoFronteiraOrigem { get; set; }
        public DateTime ChegadaFronteiraDestino { get; set; }
        public DateTime SaidaFronteiraDestino { get; set; }
        public int TempoFronteiraDestino { get; set; }
        public DateTime ChegadaDestino { get; set; }
        public DateTime SaidaDestino { get; set; }
        public int TempoDestino { get; set; }
        public int TempoViagem { get; set; }
        public decimal VlrDiaria { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public int NumeroPedido { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public string PaisDestino { get; set; }
        public DateTime PrevisaoEmbarque { get; set; }
        public decimal ValorFrete { get; set; }
        public string Produto { get; set; }
        public string Temperatura { get; set; }
        public string Motoristas { get; set; }
        public DateTime DataUltimaPosicao { get; set; }
        public string LocalUltimaPosicao { get; set; }
        public string DescricaoLocalUltimaPosicao { get; set; }
        public DateTime DataUltimaOcorrencia { get; set; }
        public string DescricaoUltimaOcorrencia { get; set; }
        public int NumeroNF { get; set; }
        public int NumeroCTe { get; set; }


        public TimeSpan TempoViagemHoras
        {
            get
            {
                return TimeSpan.FromHours((double)this.TempoViagem);
            }
        }

        public string TempoViagemFormatado
        {
            get
            {
                return $"{(int)TempoViagemHoras.TotalHours:d3}:{TempoViagemHoras:mm}";
            }
        }

        public TimeSpan TempoDestinoHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.TempoDestino);
            }
        }

        public string TempoDestinoFormatado
        {
            get
            {
                return $"{(int)TempoDestinoHoras.TotalHours:d3}:{TempoDestinoHoras:mm}";
            }
        }

        public TimeSpan TempoFronteiraDestinoHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.TempoFronteiraDestino);
            }
        }

        public string TempoFronteiraDestinoFormatado
        {
            get
            {
                return $"{(int)TempoFronteiraDestinoHoras.TotalHours:d3}:{TempoFronteiraDestinoHoras:mm}";
            }
        }

        public TimeSpan TempoFronteiraOrigemHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.TempoFronteiraOrigem);
            }
        }

        public string TempoFronteiraOrigemFormatado
        {
            get
            {
                return $"{(int)TempoFronteiraOrigemHoras.TotalHours:d3}:{TempoFronteiraOrigemHoras:mm}";
            }
        }

        public TimeSpan TempoOrigemHoras
        {
            get
            {
                return TimeSpan.FromMinutes((double)this.TempoOrigem);
            }
        }

        public string TempoOrigemFormatado
        {
            get
            {
                return $"{(int)TempoOrigemHoras.TotalHours:d3}:{TempoOrigemHoras:mm}";
            }
        }


        public string DataUltimaOcorrenciaFormatado
        {
            get { return DataUltimaOcorrencia == DateTime.MinValue ? "" : DataUltimaOcorrencia.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataUltimaPosicaoFormatado
        {
            get { return DataUltimaPosicao == DateTime.MinValue ? "" : DataUltimaPosicao.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string SaidaDestinoFormatado
        {
            get { return SaidaDestino == DateTime.MinValue ? "" : SaidaDestino.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string ChegadaDestinoFormatado
        {
            get { return ChegadaDestino == DateTime.MinValue ? "" : ChegadaDestino.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string SaidaFronteiraDestinoFormatado
        {
            get { return SaidaFronteiraDestino == DateTime.MinValue ? "" : SaidaFronteiraDestino.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string ChegadaFronteiraDestinoFormatado
        {
            get { return ChegadaFronteiraDestino == DateTime.MinValue ? "" : ChegadaFronteiraDestino.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string SaidaFronteiraOrigemFormatado
        {
            get { return SaidaFronteiraOrigem == DateTime.MinValue ? "" : SaidaFronteiraOrigem.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string ChegadaFronteiraOrigemFormatado
        {
            get { return ChegadaFronteiraOrigem == DateTime.MinValue ? "" : ChegadaFronteiraOrigem.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string SaidaOrigemFormatado
        {
            get { return SaidaOrigem == DateTime.MinValue ? "" : SaidaOrigem.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string ChegadaOrigemFormatado
        {
            get { return ChegadaOrigem == DateTime.MinValue ? "" : ChegadaOrigem.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string PrevisaoEmbarqueFormatado
        {
            get { return PrevisaoEmbarque == DateTime.MinValue ? "" : PrevisaoEmbarque.ToString("dd/MM/yyyy HH:mm"); }
        }
        
    }
}
