using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.NFS.PreNFSe
{
    public class PreNFSe
    {
        #region Propriedades

        public string CodigoIdentificacao { get; set; }

        public string NumeroCarga { get; set; }

        public string NumeroEtapa { get; set; }

        public DateTime DataEmissao { get; set; }

        public decimal AliquotaIss { get; set; }

        public decimal BaseCalculoIss { get; set; }

        public decimal PercentualRetencaoIss { get; set; }

        public decimal ValorIss { get; set; }

        public decimal ValorIssRetido { get; set; }

        public decimal ValorPrestacaoServico { get; set; }

        public decimal ValorReceber { get; set; }

        public Enumeradores.TipoTomador TipoTomador { private get; set; }

        public Localidade LocalidadeEmissao { get; set; }

        public Localidade LocalidadePrestacao { get; set; }

        public Transportador Transportador { get; set; }

        public Participante Remetente { get; set; }

        public Participante Destinatario { get; set; }

        public Participante Expedidor { get; set; }

        public Participante Recebedor { get; set; }

        public Participante Outros { get; set; }

        public List<NotaFiscal> NotasFiscais { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public Participante Tomador
        {
            get
            {
                switch (TipoTomador)
                {
                    case Enumeradores.TipoTomador.Remetente: return Remetente;
                    case Enumeradores.TipoTomador.Destinatario: return Destinatario;
                    case Enumeradores.TipoTomador.Expedidor: return Expedidor;
                    case Enumeradores.TipoTomador.Recebedor: return Recebedor;
                    case Enumeradores.TipoTomador.Outros: return Outros;
                    default: return null;
                }
            }
        }

        #endregion Propriedades com Regras
    }
}
