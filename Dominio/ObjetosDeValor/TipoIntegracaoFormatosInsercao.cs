using Amazon.Runtime.Internal.Transform;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class TipoIntegracaoFormatosInsercao
    {
        public Dictionary<string, object> Campos { get; set; }

        //formatos padrão por categoria
        private static Dictionary<CategoriaIntegracao, Dictionary<string, object>> formatosPorCategoria = new()
        {
            {
                CategoriaIntegracao.Rastreamento, new Dictionary<string, object> { { "Grupo", 2 } }
            },
            {
                CategoriaIntegracao.GerenciadoraRisco, new Dictionary<string, object> { { "TipoEnvio", 1 } }
            },
            {
                CategoriaIntegracao.Outros, new Dictionary<string, object> { { "TipoEnvio", 0 } } 
            }
        };

        public static TipoIntegracaoFormatosInsercao ObterFormato(TipoIntegracao tipo)
        {
            //verifica se tem um formato especifico 
            switch (tipo)
            {
                case TipoIntegracao.CTASmart:
                case TipoIntegracao.MicDta:
                case TipoIntegracao.DBTrans:
                case TipoIntegracao.Magalu:
                case TipoIntegracao.FTP:
                case TipoIntegracao.SAP:
                case TipoIntegracao.SAP_V9:
                case TipoIntegracao.SIC:
                case TipoIntegracao.SAP_AV:
                case TipoIntegracao.SAP_ESTORNO_FATURA:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 1 } } };
                case TipoIntegracao.KMM:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 1 } } };
                case TipoIntegracao.Natura:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 2 } } };
                case TipoIntegracao.MercadoLivre:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "ControlePorLote", 1 } } };
                case TipoIntegracao.Trizy:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "Grupo", 3 } } };
                case TipoIntegracao.NOX:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 1 } } };
                case TipoIntegracao.Raster:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 0 } } };
                case TipoIntegracao.Sefaz:
                    return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 1 }, { "s", 10 }, { "Grupo", 2 } } };
            }

            // se nao tem formato especifico entao utiliza o da categoria 
            var categoria = tipo.ObterCategoria();
            if (formatosPorCategoria.TryGetValue(categoria, out var camposPadrao))
            {
                return new TipoIntegracaoFormatosInsercao { Campos = camposPadrao };
            }

            // retorna um formato padrao se nao encontrar
            return new TipoIntegracaoFormatosInsercao { Campos = new Dictionary<string, object> { { "TipoEnvio", 0 } } };

        }
    }
}