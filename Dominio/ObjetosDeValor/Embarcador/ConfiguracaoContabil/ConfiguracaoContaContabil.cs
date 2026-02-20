using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil
{
    public class ConfiguracaoContaContabil
    {
        public int Codigo { get; set; }
        public int CodigoEmpresa { get; set; }
        public double CodigoRemetente { get; set; }
        public double CodigoDestinatario { get; set; }
        public double CodigoTomador { get; set; }
        public int CodigoCategoriaRemetente { get; set; }
        public int CodigoCategoriaDestinatario { get; set; }
        public int CodigoCategoriaTomador { get; set; }
        public int CodigoGrupoRemetente { get; set; }
        public int CodigoGrupoDestinatario { get; set; }
        public int CodigoGrupoTomador { get; set; }
        public int CodigoGrupoProduto { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoModeloDocumentoFiscal { get; set; }
        public int CodigoTipoOcorrencia { get; set; }
        public int CodigoRotaFrete { get; set; }
        public bool Ativo { get; set; }
        public int CodigoTipoDT { get; set; }
        public string Descricao { get; set; }

        public List<ConfiguracaoContaContabilContabilizacao> ConfiguracaoContaContabilContabilizacoes { get; set; }
        public List<ConfiguracaoContaContabilEscrituracao> ConfiguracaoContaContabilEscrituracoes { get; set; }
        public List<ConfiguracaoContaContabilProvisao> ConfiguracaoContaContabilProvisoes { get; set; }  
    }
}
