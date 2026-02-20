using Dominio.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Documentos
{
    public class SerieDocumentos
    {
        #region Propriedades 
        private string CNPJEmpresa { get; set; }
        private TipoSerie TipoSerie { get; set; }
        private string StatusTransportador { get; set; }
        private string StatusSerie { get; set; }
        public int Codigo { get; set; }
        public int CodigoSerie { get; set; }
        public string RazaoSocialEmpresa { get; set; }
        public int NumeroSerie { get; set; }
        public string ModeloDocumentoFiscal { get; set; }
        #endregion


        #region Propriedades com Regras
        public string CNPJEmpresaFormatado
        {
            get
            {
                return CNPJEmpresa != null ? CNPJEmpresa.ObterCpfOuCnpjFormatado() : string.Empty;
            }
        }
        public string TipoSerieFormatada
        {
            get
            {
                return TipoSerieHelper.ObterDescricao(TipoSerie);
            }
        }
        public string StatusTransportadorFormatado
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(StatusTransportador)) ? StatusTransportador == "A" ? "Ativo" : "Inativo" : string.Empty;
            }
        }

        public string StatusSerieFormatado
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(StatusSerie)) ? StatusSerie == "A" ? "Ativo" : "Inativo" : string.Empty;
            }
        }
        #endregion

    }
}