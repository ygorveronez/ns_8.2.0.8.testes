using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Compras
{
    public class FluxoCompraTratativa
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public FluxoCompraTratativa() { }

        public FluxoCompraTratativa(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa> BuscarPorFluxoCompra(int codigoFluxoCompra)
        {
            return new Repositorio.Embarcador.Compras.FluxoCompraTratativa(_unitOfWork).BuscarPorEntidadeCodigo(codigoFluxoCompra);
        }

        public long Inserir(Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa ordemCompraTratativa)
        {
            var ordemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(_unitOfWork).BuscarPorCodigo(ordemCompraTratativa.OrdemCompra.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTratativaFluxoCompra situacaoTratativa = ordemCompraTratativa.OrdemCompra.SituacaoTratativa;

            ordemCompraTratativa.OrdemCompra = ordemCompra;
            ordemCompraTratativa.OrdemCompra.SituacaoTratativa = situacaoTratativa;

            return new Repositorio.Embarcador.Compras.FluxoCompraTratativa(_unitOfWork).Inserir(ordemCompraTratativa);
        }

        public void Atualizar(Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa ordemCompraTratativa)
        {
            new Repositorio.Embarcador.Compras.FluxoCompraTratativa(_unitOfWork).Atualizar(ordemCompraTratativa);
        }

        public void DeletarPorCodigo(int codigo)
        {
            var obj = new Repositorio.Embarcador.Compras.FluxoCompraTratativa(_unitOfWork).BuscarPorCodigo(codigo);
            new Repositorio.Embarcador.Compras.FluxoCompraTratativa(_unitOfWork).Deletar(obj);
        }

        public Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa ObterStatusTratativa(int codigoOrdemCompraTratativa)
        {
            List<Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa> listaOrdemCompraTratativa = new Repositorio.Embarcador.Compras.FluxoCompraTratativa(_unitOfWork).BuscarPorEntidadeCodigo(codigoOrdemCompraTratativa);
            return listaOrdemCompraTratativa.FirstOrDefault();
        }

        #endregion
    }
}
