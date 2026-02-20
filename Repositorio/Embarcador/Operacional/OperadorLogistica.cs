using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Operacional
{
    public class OperadorLogistica : RepositorioBase<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica>
    {
        public OperadorLogistica(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OperadorLogistica(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        public Dominio.Entidades.Embarcador.Operacional.OperadorLogistica BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Operacional.OperadorLogistica BuscarPorUsuario(int codigoUsuario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica>();
            var result = from obj in query where obj.Usuario.Codigo == codigoUsuario select obj;
            return result
                .FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica> BuscarPorUsuarioAsync(int codigoUsuario)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica>()
                .Where(x => x.Usuario.Codigo == codigoUsuario).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Operacional.OperadorLogistica BuscarOperadorLogisticaPorNivelEscalation(EscalationList escalationList)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica>();
            var result = query.Where(o => o.Usuario.NivelEscalationList == escalationList);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Usuario> ConsultarOperador(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string nome, string status, Dominio.Enumeradores.TipoAcesso tipoAcesso, bool apenasPodeAdicionarComplemento, int inicio, int limite, string propOrdenacao, string dirOrdenacao)
        {
            var result = criarQueryConsulta(situacaoColaborador, nome, status, tipoAcesso, apenasPodeAdicionarComplemento);
            var rUsuario = result.Select(obj => obj.Usuario);
            return rUsuario.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string nome, string status, Dominio.Enumeradores.TipoAcesso tipoAcesso, bool apenasPodeAdicionarComplemento, string tipo = "")
        {
            var result = criarQueryConsulta(situacaoColaborador, nome, status, tipoAcesso, apenasPodeAdicionarComplemento);
            var rUsuario = result.Select(obj => obj.Usuario);
            return rUsuario.Count();
        }
        private IQueryable<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica> criarQueryConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador situacaoColaborador, string nome, string status, Dominio.Enumeradores.TipoAcesso tipoAcesso, bool apenasPodeAdicionarComplemento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Operacional.OperadorLogistica>();

            var result = query.Where(obj => obj.Usuario.TipoAcesso == tipoAcesso && obj.Ativo == true);

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Usuario.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(obj => obj.Usuario.Status == status);

            if (apenasPodeAdicionarComplemento)
                result = result.Where(obj => obj.PermiteAdicionarComplementosDeFrete);

            if (situacaoColaborador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoColaborador.Trabalhando)
                result = result.Where(obj => obj.Usuario.SituacaoColaborador.Value == situacaoColaborador || obj.Usuario.SituacaoColaborador == null);

            return result;

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Operacional.ConfiguracaoOperadores> ConsultarRelatorioConfiguracaoOperadores(Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaQuantidade = new ConsultaConfiguracaoOperadores().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaQuantidade.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Operacional.ConfiguracaoOperadores)));

            return consultaQuantidade.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Operacional.ConfiguracaoOperadores>();
        }

        public int ContarConsultaRelatorioConfiguracaoOperadores(Dominio.ObjetosDeValor.Embarcador.Operacional.FiltroPesquisaRelatorioConfiguracaoOperadores filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaQuantidade = new ConsultaConfiguracaoOperadores().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaQuantidade.SetTimeout(600).UniqueResult<int>();
        }

        public int DeletarOperadorLogisticoVinculados(int codigoUsuario)
        {
            var sql = $@"DELETE T_OPERADOR_LOGISTICA WHERE FUN_CODIGO = ${codigoUsuario}";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }
    }
}
