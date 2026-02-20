using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Configuracoes
{
    public class ConfiguracaoArquivo
    {
        public virtual int Codigo { get; set; }

        public virtual string CaminhoRelatorios { get; set; }

        public virtual string CaminhoTempArquivosImportacao { get; set; }

        public virtual string CaminhoCanhotos { get; set; }

        public virtual string CaminhoCanhotosAvulsos { get; set; }

        public virtual string CaminhoXMLNotaFiscalComprovanteEntrega { get; set; }

        public virtual string CaminhoArquivosIntegracao { get; set; }

        public virtual string CaminhoRelatoriosEmbarcador { get; set; }

        public virtual string CaminhoLogoEmbarcador { get; set; }

        public virtual string CaminhoDocumentosFiscaisEmbarcador { get; set; }

        public virtual string Anexos { get; set; }

        public virtual string CaminhoGeradorRelatorios { get; set; }

        public virtual string CaminhoArquivosEmpresas { get; set; }

        public virtual string CaminhoRelatoriosCrystal { get; set; }

        public virtual string CaminhoRetornoXMLIntegrador { get; set; }

        public virtual string CaminhoArquivos { get; set; }

        public virtual string CaminhoArquivosIntegracaoEDI { get; set; }

        public virtual string CaminhoArquivosImportacaoBoleto { get; set; }

        public virtual string CaminhoOcorrencias { get; set; }

        public virtual string CaminhoOcorrenciasMobiles { get; set; }

        public virtual string CaminhoArquivosImportacaoXMLNotaFiscal { get; set; }


        public virtual string CaminhoDestinoXML { get; set; }

        public virtual string CaminhoCanhotosAntigos { get; set; }

        public virtual string CaminhoRaiz { get; set; }

        public virtual string CaminhoGuia { get; set; }

        public virtual string CaminhoDanfeSMS { get; set; }
        public virtual string CaminhoRaizFTP { get; set; }

        public virtual string CaminhoDocumentosINPUT { get; set; }

        public virtual string CaminhoDocumentosOUTPUT { get; set; }

    }

}
