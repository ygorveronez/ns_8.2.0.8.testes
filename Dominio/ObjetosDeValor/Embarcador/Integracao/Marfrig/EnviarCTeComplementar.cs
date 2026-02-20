using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class EnviarCTeComplementar
    {
        public string ocorrenciaMultiEmbarcador { get; set; }
        public List<string> protocoloCTe { get; set; }
        public List<string> protocoloNFSe { get; set; }
        public string motivo { get; set; }
        public string problema { get; set; }
        public string dtOcorrencia { get; set; }
        public string horaOcorrencia { get; set; }
        public string transportador { get; set; }
        public string prestadorServico { get; set; }
        public string CNPJFilial { get; set; }
        public string userCriacao { get; set; }
        public string userBaixa { get; set; }
        public decimal quantidadeServicos { get; set; }
        public List<NotasFiscais> notasFiscais { get; set; }
    }
}

