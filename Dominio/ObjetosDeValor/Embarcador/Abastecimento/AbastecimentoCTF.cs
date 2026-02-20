using System.Collections.Generic;

namespace Servicos.Embarcador.Abastecimento
{
    public class ABASTECIMENTOS
    {
        public List<ABASTECIMENTOSRow> ABASTECIMENTOSRow { get; set; }
    }

    public class ABASTECIMENTOSRow
    {
        public string INDICE { get; set; }
        public string NUMABAST { get; set; }
        public string VEICODIGO { get; set; }
        public string TPREG { get; set; }
        public string BOMBA { get; set; }
        public string REDE { get; set; }
        public string POSTO { get; set; }
        public string FROTA { get; set; }
        public string C { get; set; }
        public string UVE { get; set; }
        public string PLACA { get; set; }
        public string MOTORISTA { get; set; }
        public string KM { get; set; }
        public string QTD { get; set; }
        public string PU { get; set; }
        public string PUBRAD { get; set; }
        public string TOTAL { get; set; }
        public string DATA_ABASTECIMENTO { get; set; }
        public string DATA_DEB { get; set; }
        public string DATA_CRED { get; set; }
        public string ABTARQUIVO { get; set; }
        public string DIST_PERC { get; set; }
        public string COMB_TOTAL { get; set; }
        public string S { get; set; }
        public string CD_ABAT_ANTR_VEIC { get; set; }
        public string POSTO_FANTASIA { get; set; }
        public string POSTO_CIDADE { get; set; }
        public string CGC { get; set; }
        public string DAT_ABASTECIMENTO_INICIO { get; set; }
        public string DAT_ABASTECIMENTO_FIM { get; set; }
        public string COD_MAX { get; set; }
    }
}
