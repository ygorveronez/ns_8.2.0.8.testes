namespace Dominio.ObjetosDeValor.Embarcador.Terceiros
{
    public class CalcularImpostosParametros
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCalcularImposto origemCalcularImposto { get; set; }

        public int codigoContratoFrete { get; set; }

        public int codigoPagamentoMotoristaTMS { get; set; }

        public double cpfCnpjTerceiro { get; set; }

        public int? codigoTipoTerceiro { get; set; }

        public decimal valorTotalParaCalculo { get; set; }
    }
}
