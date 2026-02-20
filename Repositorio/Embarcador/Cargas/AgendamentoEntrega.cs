using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class AgendamentoEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>
    {
        #region Construtores

        public AgendamentoEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public AgendamentoEntrega(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            var consultaAgendamentoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>();

            if (filtrosPesquisa.SenhaAgendamento > 0)
                consultaAgendamentoEntrega = consultaAgendamentoEntrega.Where(o => o.SenhaAgendamento == filtrosPesquisa.SenhaAgendamento);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Senha))
                consultaAgendamentoEntrega = consultaAgendamentoEntrega.Where(o => o.Senha == filtrosPesquisa.Senha);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Placa))
                consultaAgendamentoEntrega = consultaAgendamentoEntrega.Where(o => o.Placa.Contains(filtrosPesquisa.Placa));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0)
                consultaAgendamentoEntrega = consultaAgendamentoEntrega.Where(o => o.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjDestinatario);

            if (filtrosPesquisa.CpfCnpjRemetente > 0)
                consultaAgendamentoEntrega = consultaAgendamentoEntrega.Where(o => o.Remetente.CPF_CNPJ == filtrosPesquisa.CpfCnpjRemetente);

            if (filtrosPesquisa.Situacao.HasValue)
                consultaAgendamentoEntrega = consultaAgendamentoEntrega.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            return consultaAgendamentoEntrega;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega BuscarPorCarga(int codigoCarga)
        {
            var consultaAgendamentoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaAgendamentoEntrega.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega> BuscarPorCargas(List<int> codigosCarga)
        {
            var consultaAgendamentoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo));

            return consultaAgendamentoEntrega.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>> BuscarPorCargasAsync(List<int> codigosCarga)
        {
            var consultaAgendamentoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo));

            return consultaAgendamentoEntrega.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega> Consultar(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAgendamentoEntrega = Consultar(filtrosPesquisa);

            return ObterLista(consultaAgendamentoEntrega, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAgendamentoEntrega filtrosPesquisa)
        {
            var consultaAgendamentoEntrega = Consultar(filtrosPesquisa);

            return consultaAgendamentoEntrega.Count();
        }

        public int ObterProximaSenhaAgendamento()
        {
            var consultaAgendamentoEntrega = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega>();
            int? ultimaSenhaAgendamento = consultaAgendamentoEntrega.Max(o => (int?)o.SenhaAgendamento);

            return ultimaSenhaAgendamento.HasValue ? (ultimaSenhaAgendamento.Value + 1) : 1;
        }

        #endregion
    }
}
