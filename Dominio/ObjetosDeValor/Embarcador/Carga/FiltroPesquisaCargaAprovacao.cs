using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCargaAprovacao
    {
        public string CodigoCargaEmbarcador { get; set; }
        public int CodigoPortoDestino { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosFilialVenda { get; set; }
        public List<int> CodigosMotivoSolicitacaoFrete { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public int CodigoUsuario { get; set; }
        public double CpfCnpjTomador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public Enumeradores.SituacaoAlteracaoFreteCarga? SituacaoAlteracaoFrete { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
    }
}
