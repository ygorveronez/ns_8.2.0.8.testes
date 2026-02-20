using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Compras
{
    public class FluxoCompraTratativa : RepositorioBase<Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa>
    {
        public FluxoCompraTratativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public void InserirPorEntidade(Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa fluxoCompraTratativa)
        {
            Inserir(fluxoCompraTratativa);
        }

        public Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut?.FirstOrDefault();
        }

        public void AtualizarPorEntidade(Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa fluxoCompraTratativa)
        {
            Atualizar(fluxoCompraTratativa);
        }

        public void DeletarPorEntidade(Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa fluxoCompraTratativa)
        {
            Deletar(fluxoCompraTratativa);
        }

        public List<Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa> BuscarPorEntidadeCodigo(int codigoOrdemCompra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraTratativa>();
            var resut = from obj in query where obj.OrdemCompra.Codigo == codigoOrdemCompra select obj;
            return resut.ToList();
        }


        #endregion

    }
}
