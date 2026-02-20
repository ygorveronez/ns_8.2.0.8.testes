using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Integracao
{
    public class ControleIntegracaoCargaEDIAlteracao : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao>
    {
        #region Construtores

        public ControleIntegracaoCargaEDIAlteracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos


        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao> BuscarPorCargas(List<int> codigos)
        {
            var integracaoAlteracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao>()
                .Where(o => codigos.Contains(o.Carga.Codigo))
                .ToList();

            return integracaoAlteracoes.Distinct().ToList();
        }
        public Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao BuscarPorCarga(int codigo)
        {
            var integracaoAlteracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao>()
                .Where(o => o.Carga.Codigo == codigo)
                .ToList();

            return integracaoAlteracoes.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao BuscarUltimaPorCargaEFilial(string carga, string filial)
        {
            var integracaoAlteracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao>()
                .Where(o => o.Carga.CodigoCargaEmbarcador == carga && (o.Carga.Filial.CodigoFilialEmbarcador == filial || o.Carga.Filial.OutrosCodigosIntegracao.Contains(filial)))
                ;

            return integracaoAlteracoes.OrderByDescending(o => o.ControleIntegracaoCargaEDI.Data).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao BuscarPorCodigo(int codigo)
        {
            var controleIntegracaoCargaEDIAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return controleIntegracaoCargaEDIAlteracao;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao> BuscarPorIntegracao(int codigo)
        {
            var integracaoAlteracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao>()
                .Where(o => o.ControleIntegracaoCargaEDI.Codigo == codigo)
                .ToList();

            return integracaoAlteracoes;
        }

        #endregion
    }
}
