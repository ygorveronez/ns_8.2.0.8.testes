using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    public class ResponseObterAtendimentoOcorrencia
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public bool DevolucaoParcial { get; set; }
        public string Observacao { get; set; }
        public SituacaoChamado Situacao { get; set; }
        public long? Data { get; set; }
        public string TratativaDevolucao { get; set; }
        public List<ImagemAnexoAtendimento> ImagemAnexoAtendimento { get; set; }
        public int CodigoCargaEntrega { get; set; }
    }
}
