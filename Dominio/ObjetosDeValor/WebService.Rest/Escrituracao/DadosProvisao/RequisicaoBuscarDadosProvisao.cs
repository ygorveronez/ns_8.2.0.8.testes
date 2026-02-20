
using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao
{
    public class RequisicaoBuscarDadosProvisao
    {
        public int ProtocoloIntegracaoCarga { get; set; }
        public DateTime? DataEmissaoProvisaoInicial { get; set; }
        public DateTime? DataEmissaoProvisaoFinal { get; set; }
        public DateTime? DataCriacaoCargaInicial { get; set; }
        public DateTime? DataCriacaoCargaFinal { get; set; }
        public int InicioRegistros { get; set; }
        public int LimiteRegistros { get; set; }
    }
}
