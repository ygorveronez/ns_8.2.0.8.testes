using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.NovoApp.ColetaEntrega
{
    /// <summary>
    /// Entidade que representa uma nota devolvida no novo App.
    /// </summary>
    public class NotaRejeicao
    {
        public int codigo { get; set; }
        public bool devolucaoTotal { get; set; }
        public bool devolucaoParcial { get; set; }
        public List<ProdutoRejeicao> produtos { get; set; }

        /// <summary>
        /// Esse método existe para compatibilidade com serviços antigos. Com sorte, algum dia podemos deletar isso e usar
        /// apenas essa entidade.
        /// </summary>
        public Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal ConverterParaProdutoMobileAntigo()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal
            {
                Codigo = this.codigo,
                DevolucaoParcial = this.devolucaoParcial,
                DevolucaoTotal = this.devolucaoTotal,
                Produtos = (from o in this.produtos select o.ConverterParaProdutoMobileAntigo()).ToList(),
            };
        }

    }

}
