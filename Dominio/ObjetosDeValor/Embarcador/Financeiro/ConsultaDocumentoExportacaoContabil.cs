using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ConsultaDocumentoExportacaoContabil
    {
        public int Codigo { get; set; }
        public double CodigoTomador { get; set; }
        public string Tomador { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Empresa { get; set; }
        public string Documento { get; set; }
        public int CodigoModeloDocumento { get; set; }
        public string ModeloDocumento { get; set; }
        public string DataMovimento { get; set; }
        public decimal Valor { get; set; }
        public string NomeArquivoEDI { get; set; }
        public string Tipo
        {
            get
            {
                return TipoDocumentoExportacaoContabil.ObterDescricao();
            }
        }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao TipoDocumentoExportacaoContabil { get; set; }
    }
}
