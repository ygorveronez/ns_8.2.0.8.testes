using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Compras
{
    public class FluxoCompra
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FluxoCompra(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public dynamic ObterDetalhesFluxoCompra(Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra)
        {
            if (fluxoCompra == null)
                return null;

            var retorno = new
            {
                fluxoCompra.Codigo,
                fluxoCompra.Numero,
                fluxoCompra.Situacao,
                fluxoCompra.EtapaAtual,
                CodigoCotacao = fluxoCompra.Cotacao?.Codigo ?? 0,
                fluxoCompra.VoltouParaEtapaAtual
            };

            return retorno;
        }
        public void RejeitarFluxoCompra(int codigoOrdemCompra, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.FluxoCompra> ListafluxoCompra = repFluxoCompra.BuscarPorOrdemCompra(codigoOrdemCompra);

            if (ListafluxoCompra.Count() > 0)
                foreach (var fluxoCompra in ListafluxoCompra)
                {
                    if (fluxoCompra != null)
                    {
                        fluxoCompra.Situacao = SituacaoFluxoCompra.Rejeitado;
                        repFluxoCompra.Atualizar(fluxoCompra);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, null, "Rejeitou o fluxo de compra", unitOfWork);
                    }
                }

        }

        #endregion
    }
}
