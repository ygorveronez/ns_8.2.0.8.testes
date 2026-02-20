using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao
{
    public class LoteProvisao
    {
        #region atributos
        public int Provisao { get; set; }
        public string? TipoProvisao { get; set; }
        public string? SituacaoPagamento { get; set; }
        public string? SituacaoProvisao { get; set; }
        public bool CancelamentoProvisao { get; set; }
        public string? SituacaoCancelamentoProvisao { get; set; }
        public List<DocumentoProvisao> Documentos { get; set; }
        #endregion 
    }
}
