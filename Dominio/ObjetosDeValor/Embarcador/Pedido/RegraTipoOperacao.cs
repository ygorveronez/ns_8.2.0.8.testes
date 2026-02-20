using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class RegraTipoOperacao : IEquatable<RegraTipoOperacao>
    {
        public int CodigoTipoDocumeto { get; set; }
        public int CodigoRegra { get; set; }
        public List<string> CanaisEntrega { get; set; }
        public List<double> Destinatario { get; set; }
        public List<string> CanaisVenda { get; set; }
        public List<int> Filiais { get; set; }
        public List<double> Expedidores { get; set; }
        public List<double> Recebedores { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SimNao QuantidadeEtapas { get; set; }
        public int CodigoCategoria { get; set; }
        public bool CteGlobalizado { get; set; }
        public List<TipoModal> TipoModal { get; set; }
        public List<string> TiposOperacao { get; set; }

        public bool Equals(RegraTipoOperacao other)
        {
            bool tipoDocumentoIgual = this.CodigoTipoDocumeto == other.CodigoTipoDocumeto;
            bool existeFilial = this.Filiais.Count > 0 ? this.Filiais.Any(f => other.Filiais.Contains(f)) : true;
            bool existeCanaisEntrega = this.CanaisEntrega.Count > 0 ? this.CanaisEntrega.Any(c => other.CanaisEntrega.Contains(c)) : true;
            bool existeExpedidores = this.Expedidores.Count > 0 ? this.Expedidores.Any(e => other.Expedidores.Contains(e)) : true;
            bool existeRecebedores = this.Recebedores.Count > 0 ? this.Recebedores.Any(e => other.Recebedores.Contains(e)) : true;
            bool existeDestinatario = this.Destinatario.Count > 0 ? this.Destinatario.Any(e => other.Destinatario.Contains(e)) : true;
            bool quantidadeEtapaIgual = this.QuantidadeEtapas == other.QuantidadeEtapas;
            bool igualCategoria = this.CodigoCategoria > 0 ? this.CodigoCategoria == other.CodigoCategoria : true;
            bool cteGlobalizadoIgual = this.CteGlobalizado == other.CteGlobalizado;
            bool igualTipoModal = this.TipoModal.Count > 0 ? this.TipoModal.Any(t => other.TipoModal.Contains(t)) : true;
            bool existeTiposOperacao = this.TiposOperacao.Count > 0 ? this.TiposOperacao.Any(c => other.TiposOperacao.Contains(c)) : true;

            bool existeCanaisVenda = true;

            if (this.CanaisVenda.Count > 0)
            {
                if (other.CanaisVenda.Count > 1)
                    existeCanaisVenda = CompararListasIguais(this.CanaisVenda, other.CanaisVenda);
                else
                    existeCanaisVenda = this.CanaisVenda.Any(c => other.CanaisVenda.Contains(c));
            }

            bool matchRegraTipoOperacao = tipoDocumentoIgual && existeFilial && existeCanaisEntrega && existeCanaisVenda && existeExpedidores && quantidadeEtapaIgual && igualCategoria && cteGlobalizadoIgual && igualTipoModal && existeRecebedores && existeTiposOperacao;
            return matchRegraTipoOperacao;
        }

        public int EqualsExpecifica(RegraTipoOperacao other)
        {
            int quantidadeMacth = 0;

            if (this.CodigoTipoDocumeto == other.CodigoTipoDocumeto)
                quantidadeMacth++;

            if (this.QuantidadeEtapas.Equals(other.QuantidadeEtapas))
                quantidadeMacth++;

            if (this.CodigoCategoria == other.CodigoCategoria)
                quantidadeMacth++;

            if (this.CteGlobalizado == other.CteGlobalizado)
                quantidadeMacth++;

            if (this.TipoModal.Count > 0)
                if (this.TipoModal.Any(t => other.TipoModal.Contains(t)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.Filiais.Count > 0)
                if (this.Filiais.Any(e => other.Filiais.Contains(e)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.Recebedores.Count > 0)
                if (this.Recebedores.Any(e => other.Recebedores.Contains(e)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.Destinatario.Count > 0)
                if (this.Destinatario.Any(e => other.Destinatario.Contains(e)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.CanaisEntrega.Count > 0)
                if (this.CanaisEntrega.Any(c => other.CanaisEntrega.Contains(c)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.CanaisVenda.Count > 0)
                if (this.CanaisVenda.Count > 1 || other.CanaisVenda.Count > 1)
                {
                    bool contemEIgualQuantidade = false;
                    if (CompararListasIguais(this.CanaisVenda, other.CanaisVenda) && this.CanaisVenda.Count == other.CanaisVenda.Count)
                        contemEIgualQuantidade = true;

                    if (contemEIgualQuantidade)
                        quantidadeMacth++;
                    else quantidadeMacth--;
                }
                else if (this.CanaisVenda.Any(c => other.CanaisVenda.Contains(c)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.Expedidores.Count > 0)
                if (this.Expedidores.Any(e => other.Expedidores.Contains(e)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            if (this.TiposOperacao.Count > 0)
                if (this.TiposOperacao.Any(c => other.TiposOperacao.Contains(c)))
                    quantidadeMacth++;
                else quantidadeMacth--;

            return quantidadeMacth;
        }

        public bool CompararListasIguais(List<string> listaA, List<string> listaB)
        {
            foreach (var item in listaA)
                if (listaB.Where(itemB => item.Equals(itemB)).Count() == 0)
                    return false;

            return true;
        }

    }
}
