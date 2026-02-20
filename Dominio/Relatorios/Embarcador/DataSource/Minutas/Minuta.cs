using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Minutas
{
    public class Minuta
    {
        public string GrupoPessoas { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string LocalidadeOrigem { get; set; }
        public string EstadoOrigem { get; set; }
        public string LocalidadeDestino { get; set; }
        public string EstadoDestino { get; set; }
        public string Transportador { get; set; }
        public string Veiculo { get; set; }
        public string PropriedadeVeiculo { get; set; }
        public string ProprietarioVeiculo { get; set; }
        public string Motorista { get; set; }
        public DateTime DataEmissao { get; set; }
        public string NumeroMinuta { get; set; }
        public int QuantidadeCTes { get; set; }
        public int QuantidadeCTesIntegrados { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorServico { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorMinuta { get; set; }
        public decimal AliquotaICMS { get; set; }
        public string NumeroCTes { get; set; }
        public decimal Peso { get; set; }
        public string NumeroFatura { get; set; }
        public string SituacaoIntegracaoFatura { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta TipoIntegradora { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga Status { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string SituacaoIntegracao
        {
            get
            {
                if (QuantidadeCTesIntegrados != QuantidadeCTes)
                    return "Não Integrado";
                else
                    return "Integrado";
            }
        }
        public string Carga { get; set; }
        public string DescricaoStatus
        {
            get { return this.Status.ObterDescricao(); }
        }
    }
}
