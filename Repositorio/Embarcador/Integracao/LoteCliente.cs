using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Integracao
{
    public class LoteCliente : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente>
    {
        #region Construtores

        public LoteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public int BuscarUltimoNumero()
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente>();

            return query.Max(o => (int?)o.Numero) ?? 0;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> Consultar(int numero, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente? situacao, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> query = ObterQueryConsulta(numero, dataInicial, dataFinal, situacao);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(int numero, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> query = ObterQueryConsulta(numero, dataInicial, dataFinal, situacao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> BuscarLotesAgIntegracao(int quantidadeRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente>();

            query = query.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente.AgIntegracao && !o.GerouIntegracoes);

            return query.OrderByDescending(o => o.Codigo).Take(quantidadeRegistros).ToList();
        }

        public List<Dominio.Entidades.Cliente> ConsultarClientes(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Cliente> query = ObterQueryConsultaClientes(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar))
                query = query.OrderBy(parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar);

            if (parametrosConsulta.InicioRegistros > 0 || parametrosConsulta.LimiteRegistros > 0)
                query = query.Skip(parametrosConsulta.InicioRegistros).Take(parametrosConsulta.LimiteRegistros);

            return query.ToList();
        }

        public int ContarConsultaClientes(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Cliente> query = ObterQueryConsultaClientes(filtrosPesquisa);

            return query.Count();
        }

        public List<Dominio.Entidades.Cliente> BuscarClientesPorLote(int codigoLoteCliente)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente>();

            query = query.Where(o => o.Codigo == codigoLoteCliente);

            return query.SelectMany(o => o.Clientes).ToList();
        }

        #endregion

        #region Metodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> ObterQueryConsulta(int numero, DateTime dataInicial, DateTime dataFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLoteCliente? situacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente>();

            if (dataInicial != DateTime.MinValue)
                query = query.Where(o => o.DataInicial >= dataInicial);

            if (dataFinal != DateTime.MinValue)
                query = query.Where(o => o.DataFinal <= dataFinal);

            if (situacao.HasValue)
                query = query.Where(o => o.Situacao == situacao);

            if (numero > 0)
                query = query.Where(o => o.Numero == numero);

            return query;
        }

        public IQueryable<Dominio.Entidades.Cliente> ObterQueryConsultaClientes(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Cliente> query = SessionNHiBernate.Query<Dominio.Entidades.Cliente>();

            if (filtrosPesquisa.CodigoLote > 0)
            {
                IQueryable<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> queryLoteCliente = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente>();
                var resultQueryLoteCliente = from obj in queryLoteCliente select obj;

                query = query.Where(o => resultQueryLoteCliente.Where(l => l.Clientes.Any(c => c.CPF_CNPJ == o.CPF_CNPJ) && l.Codigo == filtrosPesquisa.CodigoLote).Any());
            }
            else
            {
                if (filtrosPesquisa.DataInicio.HasValue)
                    query = query.Where(o => o.DataCadastro.Value.Date >= filtrosPesquisa.DataInicio);

                if (filtrosPesquisa.DataLimite.HasValue)
                    query = query.Where(o => o.DataCadastro.Value.Date <= filtrosPesquisa.DataLimite);

                if  (!string.IsNullOrWhiteSpace(filtrosPesquisa.RazaoSocial))
                    query = query.Where(o => o.Nome.Contains(filtrosPesquisa.RazaoSocial));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CNPJ))
                    query = query.Where(o => o.CPF_CNPJ == Double.Parse(filtrosPesquisa.CNPJ));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.InscricaoEstadual))
                    query = query.Where(o => o.IE_RG.Contains(filtrosPesquisa.InscricaoEstadual));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Endereco))
                    query = query.Where(o => o.Endereco.Contains(filtrosPesquisa.Endereco));

                if (filtrosPesquisa.Localidade > 0)
                    query = query.Where(o => o.Localidade.Codigo == filtrosPesquisa.Localidade);
                
                if (filtrosPesquisa.SelecionarTodos.HasValue)
                {
                    if (filtrosPesquisa.SelecionarTodos == true)
                        query = query.Where(o => !filtrosPesquisa.CodigosSelecionados.Contains(o.CPF_CNPJ));
                    else
                        query = query.Where(o => filtrosPesquisa.CodigosSelecionados.Contains(o.CPF_CNPJ));
                }
            }

            return query;
        }

        #endregion
    }
}
