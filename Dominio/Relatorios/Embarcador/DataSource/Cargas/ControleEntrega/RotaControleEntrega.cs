using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega
{
    public class RotaControleEntrega
    {
        #region Atributos
        public int Codigo { get; set; }
        public string DescricaoFilial { get; set; }
        public string NumeroCarga { get; set; }
        public string VeiculoPlaca { get; set; }
        public string Transportador { get; set; }
        public decimal PesoEntregue { get; set; }
        public decimal PesoDevolvido { get; set; }
        public int EntregasRealizadas { get; set; }
        public int Reentregas { get; set; }
        public decimal KMRealizados { get; set; }
        public int CodigoCliente { get; set; }
        public string RazaoSocial { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string ApontamentoNoRaio { get; set; }
        public int SequenciaRealizada { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Reversao { get; set; }
        private DateTime InicioRealizado { get; set; }
        private DateTime FimRealizado { get; set; }
        private DateTime ChegadaRealizada { get; set; }
        private DateTime InicioDescarga { get; set; }
        private DateTime SaidaRealizada { get; set; }
        private SituacaoCarga Status { get; set; }
        #endregion

        #region Propriedades Formatadas
        public string StatusFormatada
        {
            get
            {
                return Status.ObterDescricao();
            }
        }
        public string InicioRealizadoFormatada
        {
            get { return InicioRealizado != DateTime.MinValue ? InicioRealizado.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string FimRealizadoFormatada
        {
            get { return FimRealizado != DateTime.MinValue ? FimRealizado.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string ChegadaRealizadaFormatada
        {
            get { return ChegadaRealizada != DateTime.MinValue ? ChegadaRealizada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string InicioDescargaFormatada
        {
            get { return InicioDescarga != DateTime.MinValue ? InicioDescarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string SaidaRealizadaFormatada
        {
            get { return SaidaRealizada != DateTime.MinValue ? SaidaRealizada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        #endregion

    }
}
