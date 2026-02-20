namespace Dominio.ObjetosDeValor.Embarcador.Mobile.Canhotos
{
    public class Canhoto
    {
        public int CodigoIntegracao { get; set; }
        public int Numero { get; set; }
        public string DataEmissao { get; set; }
        public string DataEnvioCanhoto { get; set; }
        public string MotivoRejeicaoDigitalizacao { get; set; }
        public string Descricao { get; set; }
        public string Identificacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga Carga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Empresa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Filial.Filial Filial { get; set; }
        public virtual decimal Peso { get; set; }
        public virtual decimal Valor { get; set; }
        public string Observacao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCanhoto TipoCanhoto { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto SituacaoCanhoto { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto SituacaoDigitalizacaoCanhoto { get; set; }
        public string DataEntregaNotaCliente { get; set; }
        public bool DigitalizacaoCanhotoInteiro { get; set; }
    }
}
