using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class PosicaoContasReceber
    {
        public string Filial { get; set; }
        public int NumeroCTe { get; set; }
        public int CodigoCTe { get; set; }
        public int SerieCTe { get; set; }
        public string Tomador { get; set; }
        public double CNPJTomador { get; set; }
        public int CodigoGrupo { get; set; }
        public string DescricaoGrupo { get; set; }
        public string CidadeTomador { get; set; }
        public string CNPJRemetente { get; set; }
        public string Remetente { get; set; }
        public string CNPJDestinatario { get; set; }
        public string Destinatario { get; set; }
        public decimal ValorReceber { get; set; }
        public string ProprioTerceiro { get; set; }
        public string Frotas { get; set; }
        public string Placas { get; set; }
        public string Motoristas { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public string Notas { get; set; }
        public string Origem { get; set; }
        public string UFOrigem { get; set; }
        public string Destino { get; set; }
        public string UFDestino { get; set; }
        public long NumeroDTMinuta { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataEmissaoCarga { get; set; }
        public int CodigoFatura { get; set; }
        public int NumeroFatura { get; set; }
        public int NumeroPreFatura { get; set; }
        public int CodigoGrupoFatura { get; set; }
        public string GrupoFatura { get; set; }
        public string ClienteFatura { get; set; }
        public DateTime DataEmissaoFatura { get; set; }
        public string ClienteTitulo { get; set; }
        public string Modelo { get; set; }
        public string ComponentesFrete { get; set; }
        public DateTime DataBaseBaixa { get; set; }
        public DateTime DataMovimento { get; set; }
        public decimal ValorTitulo { get; set; }
        public decimal ValorPendenteTitulo { get; set; }
        public DateTime DataVencimentoTitulo { get; set; }
        public DateTime DataEmissaoTitulo { get; set; }
        public string StatusTitulo { get; set; }
        public int CodigoStatus { get; set; }
    }
}
