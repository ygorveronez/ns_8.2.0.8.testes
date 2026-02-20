using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ExportacaoContabilizacao
    {
        public int? CodigoCTe { get; set; }
        public int? CodigoContratoFrete { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoExportacaoContabil TipoDocumento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao TipoMovimentoExportacao { get; set; }
        public string Numero { get; set; }
        public int? SerieCTe { get; set; }
        public DateTime DataEmissao { get; set; }
        public Dominio.Enumeradores.TipoDocumento? TipoDocumentoEmissao { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataMovimento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito DebitoCredito { get; set; }
        public string CNPJEmpresa { get; set; }
        public string CodigoIntegracaoEmpresa { get; set; }
        public double CPFCNPJTomador { get; set; }
        public string TipoTomador { get; set; }
        public string CPFCNPJTomadorSemFormato
        {
            get
            {
                if (this.TipoTomador == "E")
                    return "00000000000000";
                else
                    return TipoTomador == "J" ? string.Format(@"{0:00000000000000}", CPFCNPJTomador) : string.Format(@"{0:00000000000}", CPFCNPJTomador);
            }
        }
        public string CodigoIntegracaoTomador { get; set; }
        public string CodigoContaContabil { get; set; }
        public string CodigoContaContabilCadastro { get; set; }
        public string CodigoCentroResultado { get; set; }
        public string CodigoCentroResultadoCadastro { get; set; }
        public string CodigoCentroResultadoEmpresa { get; set; }
        public bool TomadorFazParteGrupoEconomico { get; set; }

    }
}
