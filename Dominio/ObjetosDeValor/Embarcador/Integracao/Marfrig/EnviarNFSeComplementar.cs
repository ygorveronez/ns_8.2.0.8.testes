using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class EnviarNFSeComplementar
    {
        public string transportador { get; set; }
        public string numeroDocumento { get; set; }
        public string dataEmissao { get; set; }
        public string cnpjFilial { get; set; }
        public string cnpjRemetente { get; set; }
        public decimal valorDocumento { get; set; }
        public decimal aliquotaISS { get; set; }
        public decimal iSSBaseCalculo { get; set; }
        public decimal percentualImpRetido { get; set; }
        public decimal valorBaseCalculoISS { get; set; }
        public decimal valorISS { get; set; }
        public decimal valorRetencaoISS { get; set; }
        public decimal quantidadeDocumentos { get; set; }
        public decimal quantidadeVolumes { get; set; }
        public decimal valorCarga { get; set; }
        public int tipoTributacao { get; set; }
        public string codigoVerificacao { get; set; }
        public List<NotasFiscaisND> notasFiscais { get; set; }
        public Ocorrencia ocorrencia { get; set; }
    }
}

