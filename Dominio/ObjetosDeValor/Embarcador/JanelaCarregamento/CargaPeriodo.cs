using System;

namespace Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento
{
    public class CargaPeriodo
    {
        public int Codigo { get; set; }

        public int? CodigoCarga { get; set; }

        public double? CPFCNPJRemetente { get; set; }

        public int? CodigoGrupoPessoaRemetente { get; set; }

        public int? CodigoGrupoProdutoDominante { get; set; }

        public int? TipoOperacao { get; set; }

        public int? Transportador { get; set; }

        public double? Destinatario { get; set; }

        public int? QuantidadeSku { get; set; }

        public int? ModeloVeicularCarga { get; set; }

        public int? TipoCarga { get; set; }

        public int? TipoOperacaoEncaixe { get; set; }

        public bool? Encaixe { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public int ObterTipoOperacao => (Encaixe.HasValue && Encaixe.Value ? TipoOperacaoEncaixe : TipoOperacao) ?? 0;
    }
}
