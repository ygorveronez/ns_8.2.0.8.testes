using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public class FiltroPesquisaSolicitacaoToken
    {
        public int NumeroProtocolo { get; set; }
        public int Prioridade { get; set; }
        public int Usuario { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataInicioVigencia { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public SituacaoAutorizacaoToken Situacao { get; set; }
        public EtapaAutorizacaoToken EtapaAutorizacao { get; set; }
        public int CodigoEmpresa { get; set; }

    }
}
