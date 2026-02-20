using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteClienteIntegrarAlteracao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao>
    {
        #region Construtores

        public TabelaFreteClienteIntegrarAlteracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao BuscarPorAlteracao(int codigoTabelaFreteIntegrarAlteracao, int codigoTabelaFreteCliente)
        {
            var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao>()
                .Where(o => o.TabelaFreteIntegrarAlteracao.Codigo == codigoTabelaFreteIntegrarAlteracao && o.TabelaFreteCliente.Codigo == codigoTabelaFreteCliente);

            return consultaTabelaFreteClienteAlteracao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> BuscarPorAlteracao(int codigoTabelaFreteIntegrarAlteracao)
        {
            var consultaTabelaFreteClienteAlteracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegrarAlteracao>()
                .Where(o => o.TabelaFreteIntegrarAlteracao.Codigo == codigoTabelaFreteIntegrarAlteracao);

            return consultaTabelaFreteClienteAlteracao.Select(t => t.TabelaFreteCliente).ToList();
        }
        #endregion Métodos Públicos
    }
}
