using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorCliente : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente>
    {
        public ContratoFreteTransportadorCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<double> BuscarCNPJsNaoPesentesNaLista(int contrato, List<double> CNPJsClientes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !CNPJsClientes.Contains(obj.Cliente.CPF_CNPJ)
                         select obj.Cliente.CPF_CNPJ;

            return result.ToList();
        }

        public List<double> BuscarCpfCnpjClientesPorContrato(int codigoContrato)
        {
            var consultaContratoCliente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente>()
                .Where(contratoCliente => contratoCliente.ContratoFrete.Codigo == codigoContrato);

            return consultaContratoCliente
                .Select(contratoCliente => contratoCliente.Cliente.CPF_CNPJ)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente BuscarPorContratoECliente(int contrato, double cliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.Cliente.CPF_CNPJ == cliente
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportadorCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente> result = Consultar(filtrosPesquisa);

            result = result
                .Fetch(o => o.Cliente);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportadorCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportadorCliente filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente>();
            var result = from obj in query where obj.ContratoFrete.Codigo == filtrosPesquisa.CodigoContratoFreteTransportador select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Nome))
                result = result.Where(obj => obj.Cliente.Nome.Contains(filtrosPesquisa.Nome));

            if (filtrosPesquisa.CnpjCpf > 0)
                result = result.Where(o => o.Cliente.CPF_CNPJ == filtrosPesquisa.CnpjCpf);

            return result;
        }

        #endregion
    }
}

