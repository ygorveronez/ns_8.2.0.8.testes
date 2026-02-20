using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public sealed class FichaCliente : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.FichaCliente>
    {
        #region Construtores

        public FichaCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.FichaCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaFichaCliente filtrosPesquisa)
        {
            var consultaFichaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FichaCliente>();

            if (filtrosPesquisa.CpfCnpjCliente > 0d)
                consultaFichaCliente = consultaFichaCliente.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjCliente);

            return consultaFichaCliente;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.FichaCliente BuscarPorCpfCnpjCliente(double cpfcnpj)
        {
            var consultaFichaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FichaCliente>()
                .Where(o => o.Cliente.CPF_CNPJ == cpfcnpj);

            return consultaFichaCliente.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.FichaCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaFichaCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaFichaCliente = Consultar(filtrosPesquisa);

            return ObterLista(consultaFichaCliente, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaFichaCliente filtrosPesquisa)
        {
            var consultaFichaCliente = Consultar(filtrosPesquisa);

            return consultaFichaCliente.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.FichaCliente BuscarPorCodigo(int codigo)
        {
            var consultaFichaCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.FichaCliente>()
                .Where(obj => obj.Codigo == codigo);

            return consultaFichaCliente.FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
