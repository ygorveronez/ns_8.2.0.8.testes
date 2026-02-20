using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Email
{
    public class EmailDocumentacaoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga>
    {
        public EmailDocumentacaoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailDocumentacaoCarga filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailDocumentacaoCarga filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        public Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga BuscarPorPessoaTipoOperacao(double cnpjCpfPessoa, int codigoTipoOperacao, bool enviarCte = false, bool enviarMdfe = false, bool enviarContratoFrete = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga>()
                .Where(obj => obj.Pessoa.CPF_CNPJ == cnpjCpfPessoa && (obj.TiposOperacao.Any(t => t.Codigo == codigoTipoOperacao) || obj.TiposOperacao.Count == 0));

            if (enviarCte)
                query = query.Where(o => o.EnviarCTe);

            if (enviarMdfe)
                query = query.Where(o => o.EnviarMDFe);

            if (enviarContratoFrete)
                query = query.Where(o => o.EnviarContratoFrete);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga BuscarPorPessoaTipoOperacaoEnvioContrato(double cnpjCpfPessoa, int codigoTipoOperacao)
        {
            return BuscarPorPessoaTipoOperacao(cnpjCpfPessoa, codigoTipoOperacao, false, false, true);
        }

        public Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga BuscarPorPessoaTipoOperacaoEnvioCTe(double cnpjCpfPessoa, int codigoTipoOperacao)
        {
            return BuscarPorPessoaTipoOperacao(cnpjCpfPessoa, codigoTipoOperacao, true, false, false);
        }

        public Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga BuscarPorPessoaTipoOperacaoEnvioMDFe(double cnpjCpfPessoa, int codigoTipoOperacao)
        {
            return BuscarPorPessoaTipoOperacao(cnpjCpfPessoa, codigoTipoOperacao, false, true, false);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga> Consultar(Dominio.ObjetosDeValor.Embarcador.Email.FiltroPesquisaEmailDocumentacaoCarga filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Email.EmailDocumentacaoCarga>();
            consulta = from obj in consulta select obj;

            if (filtrosPesquisa.CodigoPessoa > 0)
                consulta = consulta.Where(o => o.Pessoa.CPF_CNPJ.Equals(filtrosPesquisa.CodigoPessoa));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Emails))
                consulta = consulta.Where(o => o.Emails.Contains(filtrosPesquisa.Emails));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                consulta = consulta.Where(o => o.TiposOperacao.Any(obj => obj.Codigo == filtrosPesquisa.CodigoTipoOperacao));

            return consulta;
        }

        #endregion
    }
}
