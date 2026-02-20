using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Financeiro
{
    public class RetornoProvisaoDT
    {
        public  int ProtocoloIntegracaoCarga { get; set; }
        public string MensagemRetornoCarga { get; set; }
        public List<StageRetornoProvisao> Stages { get; set; }
    }
}
