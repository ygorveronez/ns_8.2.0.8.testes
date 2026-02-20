using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Documentos
{
    public class CargaCIOTPedido
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string Numero { get; set; }

        public string Proprietario { get; set; }

        public string TipoPessoa { get; set; }

        public double CNPJProprietario { get; set; }

        public string Carga { get; set; }

        public string Pedido { get; set; }

        public string Empresa { get; set; }

        public string CNPJEmpresa { get; set; }

        public decimal PesoBruto { get; set; }

        public decimal ValorMercadoriaKG { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal ValorTarifaFrete { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal PercentualTolerancia { get; set; }

        public decimal PercentualToleranciaSuperior { get; set; }

        public decimal ValorAdiantamento { get; set; }

        public decimal ValorSeguro { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorIRRF { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorSENAT { get; set; }

        public decimal ValorSEST { get; set; }

        public decimal ValorOutrosDescontos { get; set; }

        public string MotoristaCBO { get; set; }

        public string MotoristaCPFCNPJ { get; set; }

        public DateTime MotoristaDataNascimento { get; set; }

        public string MotoristaNome { get; set; }

        public string MotoristaPisPasep { get; set; }

        public DateTime DataPagamentoAdiantamentoFrete { get; set; }

        public DateTime DataPagamentoSaldoFrete { get; set; }

        public decimal BaseCalculoINSS { get; set; }

        public decimal BaseCalculoIRRF { get; set; }

        public string MensagemCIOT { get; set; }

        public string ProtocoloAutorizacao { get; set; }

        public string VeiculoTracao { get; set; }

        public string VeiculosReboques { get; set; }

        private DateTime DataCarga { get; set; }

        public string Destinatario { get; set; }

        public string Destino { get; set; }


        public decimal AliquotaIRRF { get; set; }
        public decimal BaseCalculoIRRFSemAcumulo { get; set; }
        public decimal ValorPorDependente { get; set; }
        public int QuantidadeDependentes { get; set; }
        public decimal ValorTotalDependentes { get; set; }
        public decimal BaseCalculoIRRFSemDesconto { get; set; }
        public decimal ValorIRRFSemDesconto { get; set; }


        #endregion


        #region Propriedades com Regras

        public string PedidoFormatado
        {
            get { return !string.IsNullOrEmpty(Pedido) ? (Pedido.Contains("_") ? Pedido.Split('_')[1].ToString() : Pedido) : Pedido; }
        }

        public string DataCargaFormatada
        {
            get { return DataCarga != DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataPagamentoAdiantamentoFreteFormatada
        {
            get { return DataPagamentoAdiantamentoFrete != DateTime.MinValue ? DataPagamentoAdiantamentoFrete.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataPagamentoSaldoFreteFormatada
        {
            get { return DataPagamentoSaldoFrete != DateTime.MinValue ? DataPagamentoSaldoFrete.ToString("dd/MM/yyyy") : ""; }
        }

        public string EmpresaFormatado
        {
            get { return $"{this.Empresa} - {CNPJEmpresa.ObterCnpjFormatado()}"; }
        }

        public string MotoristaDataNascimentoFormatada
        {
            get { return MotoristaDataNascimento != DateTime.MinValue ? MotoristaDataNascimento.ToString("dd/MM/yyyy") : ""; }
        }

        public string MotoristaFormatado
        {
            get { return $"{this.MotoristaNome} - {MotoristaCPFCNPJ.ObterCpfOuCnpjFormatado()}"; }
        }

        public virtual string ProprietarioFormatado
        {
            get { return $"{Proprietario} - {(CNPJProprietario > 0d ? CNPJProprietario.ToString().ObterCpfOuCnpjFormatado(TipoPessoa) : "")}"; }
        }

        public decimal Saldo
        {
            get { return this.ValorFrete - (this.ValorINSS + this.ValorSEST + this.ValorSENAT + this.ValorIRRF + this.ValorAdiantamento + this.ValorOutrosDescontos); }
        }

        public decimal ValorFreteComImpostos
        {
            get { return this.ValorFrete + (this.ValorINSS + this.ValorSEST + this.ValorSENAT + this.ValorIRRF); }
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT Situacao { get; set; }

        public string DescricaoSituacao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOTHelper.ObterDescricao(this.Situacao); }
        }

        #endregion
    }
}
