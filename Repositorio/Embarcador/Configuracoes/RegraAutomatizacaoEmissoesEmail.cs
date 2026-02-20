using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class RegraAutomatizacaoEmissoesEmail : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail>
    {
        #region Construtores

        public RegraAutomatizacaoEmissoesEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaRegraAutomatizacaoEmissoesEmail filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaRegraAutomatizacaoEmissoesEmail filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail BuscarPorRegraAutomatizacaoEmissoesEmail(List<double> cnpjcpfRemetente, List<double> cnpjcpfDestinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail>();

            query = from obj in query where (obj.Remetentes.Any(o => cnpjcpfRemetente.Contains(o.Remetente.CPF_CNPJ)) || !obj.Remetentes.Any()) && (obj.Destinatarios.Any(o => cnpjcpfDestinatario.Contains(o.Destinatario.CPF_CNPJ)) || !obj.Destinatarios.Any()) select obj;

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail BuscarPorRegraAutomatizacaoEmissoesEmail(string emailDestino, double cnpjcpfRemetente, double cnpjcpfDestinatario)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail>();
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente> queryRemetente = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailRemetente>();
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario> queryDestinatario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmailDestinatario>();

            query = query.Where(o => o.EmailDestino == emailDestino);
            query = query.Where(o => !queryRemetente.Where(rem => rem.RegraAutomatizacaoEmissoesEmail == o).Any() || queryRemetente.Where(rem => rem.RegraAutomatizacaoEmissoesEmail == o && rem.Remetente.CPF_CNPJ == cnpjcpfRemetente).Any());
            query = query.Where(o => !queryDestinatario.Where(dest => dest.RegraAutomatizacaoEmissoesEmail == o).Any() || queryDestinatario.Where(dest => dest.RegraAutomatizacaoEmissoesEmail == o && dest.Destinatario.CPF_CNPJ == cnpjcpfDestinatario).Any());

            return query.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail> Consultar(Dominio.ObjetosDeValor.Embarcador.Configuracoes.FiltroPesquisaRegraAutomatizacaoEmissoesEmail filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EmailDestino))
                result = result.Where(obj => obj.EmailDestino.Contains(filtrosPesquisa.EmailDestino));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.CodigoRemetente > 0)
                result = result.Where(obj => obj.Remetentes.Any(a => a.Remetente.CPF_CNPJ == filtrosPesquisa.CodigoRemetente));

            if (filtrosPesquisa.CodigoDestinatario > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoTipoOperacao);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Ativo);

            return result;
        }

        #endregion
    }
}
