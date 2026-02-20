using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pessoas
{
    public class ClienteBuscaAutomatica : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica>
    {
        public ClienteBuscaAutomatica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ClienteBuscaAutomatica(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public async Task<List<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica>> ConsultarAsync(FiltroPesquisaClienteBuscaAutomatica filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica> consultaBuscaAutomatica = Consultar(filtrosPesquisa);

            return await ObterListaAsync(consultaBuscaAutomatica, parametrosConsulta);
        }

        public async Task<int> ContarConsultaAsync(FiltroPesquisaClienteBuscaAutomatica filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica> clienteBuscaAutomatica = Consultar(filtrosPesquisa);

            return await clienteBuscaAutomatica.CountAsync();
        }

        public async Task<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica> BuscarPorParametrosAsync(double codigoRemetente, double codigoDestinatario, int codigoLocalidadeOrigem, int codigoFilial, TipoParticipante tipoParticipante, SituacaoAtivoPesquisa situacaoAtivoPesquisa)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica>()
                .Where(c =>
                    c.Situacao == situacaoAtivoPesquisa
                    && c.TipoParticipante == tipoParticipante
                    && c.Remetente.CPF_CNPJ == codigoRemetente
                    && c.Destinatario.CPF_CNPJ == codigoDestinatario
                    && c.Origem.Codigo == codigoLocalidadeOrigem
                    && c.Filial.Codigo == codigoFilial
                )
                .OrderBy(c => c.Codigo)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Task<IList<Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica>> BuscarParaOrdenarPorCompatibilidade(CancellationToken cancellationToken)
        {
            string sql = $@"SELECT 
                            CBA_CODIGO as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.Codigo)},
                            CBA_TIPO_PARTICIPANTE as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.TipoParticipante)},
                            CAST(CLI_CGCCPF_REMETENTE AS float(53)) as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.CPFCNPJRemetente)},
                            CAST(CLI_CGCCPF_DESTINATARIO AS float(53)) as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.CPFCNPJDestinatario)},
                            LOC_CODIGO_ORIGEM as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.CodigoOrigem)},
                            FIL_CODIGO as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.CodigoFilial)},
                            CLI_CGCCPF as {nameof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica.CPFCNPJCliente)}
                            FROM T_CLIENTE_BUSCA_AUTOMATICA 
                            WHERE CBA_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo}";

            ISQLQuery query = SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica)));
            return query.ListAsync<Dominio.ObjetosDeValor.Embarcador.ClienteBuscaAutomatica.ParametrosClienteBuscaAutomatica>(cancellationToken);
        }

        #endregion

        #region Métodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica> Consultar(FiltroPesquisaClienteBuscaAutomatica filtros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ClienteBuscaAutomatica>();

            if (filtros.Situacao != SituacaoAtivoPesquisa.Todos)
                query = query.Where(x => x.Situacao == filtros.Situacao);

            if (filtros.CodigoFilial > 0)
                query = query.Where(x => x.Filial != null && x.Filial.Codigo == filtros.CodigoFilial);

            if (filtros.CodigoCliente > 0)
                query = query.Where(x => x.Cliente != null && x.Cliente.CPF_CNPJ == filtros.CodigoCliente);

            if (filtros.CodigoRemetente > 0)
                query = query.Where(x => x.Remetente != null && x.Remetente.CPF_CNPJ == filtros.CodigoRemetente);

            if (filtros.CodigoDestinatario > 0)
                query = query.Where(x => x.Destinatario != null && x.Destinatario.CPF_CNPJ == filtros.CodigoDestinatario);

            if (filtros.CodigoLocalidadeOrigem > 0)
                query = query.Where(x => x.Origem != null && x.Origem.Codigo == filtros.CodigoLocalidadeOrigem);

            return query;
        }


        #endregion
    }
}
