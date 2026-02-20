using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class FiltroPesquisaPesagemAprovacao
    {
        public string CodigoCargaEmbarcador { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataLimite { get; set; }
        public Enumeradores.SituacaoPesagemCarga? SituacaoPesagemCarga { get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosModeloVeicular { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public int CodigoUsuario { get; set; }
    }
}
