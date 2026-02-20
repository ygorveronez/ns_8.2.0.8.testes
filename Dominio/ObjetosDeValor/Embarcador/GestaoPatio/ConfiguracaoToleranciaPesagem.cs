using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class ConfiguracaoToleranciaPesagem
    {
        public virtual int Codigo { get; set; }
        public virtual string Descricao { get; set; }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem TipoRegra { get; set; }

        public virtual decimal ToleranciaPesoSuperior { get; set; }

        public virtual decimal ToleranciaPesoInferior { get; set; }

        public virtual decimal PercentualToleranciaPesoSuperior { get; set; }

        public virtual decimal PercentualToleranciaPesoInferior { get; set; }

        public List<BuscaMultiplosConfiguracaoToleranciaPesagem> CodigosFiliais { get; set; }

        public List<BuscaMultiplosConfiguracaoToleranciaPesagem> CodigosModeloVeicular { get; set; }

        public List<BuscaMultiplosConfiguracaoToleranciaPesagem> CodigosTipoCarga { get; set; }

        public List<BuscaMultiplosConfiguracaoToleranciaPesagem> CodigosTipoOperacao { get; set; }
    }
}
