using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class Abastecimento
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string Documento { get; set; }
        public string Produto { get; set; }
        private double CpfCnpjFornecedor { get; set; }
        private string TipoFornecedor { get; set; }
        public string Fornecedor { get; set; }
        public string Veiculo { get; set; }
        public string Categoria { get; set; }
        private DateTime Data { get; set; }
        public decimal KmAnterior { get; set; }
        public decimal Km { get; set; }
        public decimal Litros { get; set; }
        public int CapacidadeTanque { get; set; }
        public string Status { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }
        private int NumeroAcerto { get; set; }
        public string TipoPropriedade { get; set; }
        public string Proprietario { get; set; }
        public string Segmento { get; set; }
        public decimal KMTotal { get; set; }
        public decimal Media { get; set; }
        public string NumeroFrota { get; set; }
        public string Motorista { get; set; }
        public string Equipamento { get; set; }
        public int Horimetro { get; set; }
        public decimal MediaPadrao { get; set; }
        public int HorimetroAnterior { get; set; }
        public int HorimetroTotal { get; set; }
        public decimal MediaHorimetro { get; set; }
        public string MotivoInconsistencia { get; set; }
        private TipoRecebimentoAbastecimento TipoRecebimento { get; set; }
        public decimal KmOriginal { get; set; }
        public int HorimetroOriginal { get; set; }
        public string GrupoPessoa { get; set; }
        public string CentroResultado { get; set; }
        public string DataSeparada { get; set; }
        public string HoraSeparada { get; set; }
        public string FantasiaFornecedor { get; set; }
        public string UFFornecedor { get; set; }
        public string ModeloVeicularCarga { get; set; }
        public string MotoristaAnterior { get; set; }
        public decimal KMAnteriorAlteracao { get; set; }
        private DateTime DataAnterior { get; set; }
        public string Pais { get; set; }
        public MoedaCotacaoBancoCentral Moeda { get; set; }
        public DateTime DataBaseCRT { get; set; }
        public decimal ValorMoeda { get; set; }
        public decimal ValorOriginalMoeda { get; set; }
        public string LocalidadeFornecedor { get; set; }
        public string Observacao { get; set; }
        public string LocalArmazenamento { get; set; }
        public string PlacaReboque { get; set; }
        public string ModeloVeicularTracao { get; set; }
        public string ModeloVeicularReboque { get; set; }
        public string SegmentoTracao { get; set; }
        public string SegmentoReboque { get; set; }
        public string ValorLitroTicketLog { get; set; }
        public string ValorTotalTicketLog { get; set; }
        public int Requisicao { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DescricaoTipoRecebimento
        {
            get { return TipoRecebimento.ObterDescricao(); }
        }

        public virtual string CpfCnpjFornecedorFormatado
        {
            get
            {
                return CpfCnpjFornecedor > 0 ? TipoFornecedor.Equals("E") ? "00.000.000/0000-00" :
                    TipoFornecedor.Equals("J") ? string.Format(@"{0:00\.000\.000\/0000\-00}", CpfCnpjFornecedor) : string.Format(@"{0:000\.000\.000\-00}", CpfCnpjFornecedor) : string.Empty;
            }
        }

        public string DataAnteriorFormatada
        {
            get { return DataAnterior != DateTime.MinValue ? DataAnterior.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string NumeroAcertoFormatado
        {
            get { return NumeroAcerto > 0 ? NumeroAcerto.ToString("n0") : string.Empty; }
        }

        public string DataBaseCRTFormatada  
        {
            get { return DataBaseCRT != DateTime.MinValue ? DataBaseCRT.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string ValorMoedaFormatado 
        {
            get { return ValorMoeda.ToString("n10"); } 
        }

        public string MoedaDescricao 
        {
            get { return Moeda.ObterDescricao(); }
        }

        #endregion
    }
}
