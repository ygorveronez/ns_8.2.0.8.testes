using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class PreDocumento
    {
        public string ModeloDocumento { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> ListaNotas { get; set; }

        public Localidade Origem { get; set; }

        public Localidade Destino { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Recebedor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor ValorFrete { get; set; }
        
        public string XML { get; set; }
    }
}
