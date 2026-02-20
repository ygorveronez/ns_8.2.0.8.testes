using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoDevolucao
{
    public class ParametrosGestaoDevolucao
    {
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao OrigemRecebimento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeracaoGestaoDevolucao Geracao { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalOrigem { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> NotasFiscaisDeOrigem { get; set; }

        public Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalDevolucao { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> NotasFiscaisDeDevolucao { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Produtos> Produtos { get; set; }

        public Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        public Dominio.Entidades.Empresa Transportador { get; set; }

        public Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        public bool PosEntrega { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotasGestaoDevolucao TipoNotas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoDevolucao TipoFluxoGestaoDevolucao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGestaoDevolucao SituacaoDevolucao { get; set; }

    }

    public class GestaoDevolucaoNotaFiscal
    {
        public long CodigoGestaoDevolucao { get; set; }
        public int CodigoNotaFiscal { get; set; }
        public string ChaveNotaFiscal { get; set; }
    }
}

