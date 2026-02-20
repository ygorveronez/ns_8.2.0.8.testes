using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Configuracoes
{
    public class AcordoFaturamentoCliente : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente>
    {
        public AcordoFaturamentoCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public bool ContemAcordoFaturamentoCliente(double cnpjCpfPessoa, int codigoGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente>();
            //query = query.Where(o => o.Status == true);

            if (cnpjCpfPessoa > 0)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cnpjCpfPessoa);
            if (codigoGrupoPessoa > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoa);

            return query.Any();
        }

        public bool ContemAcordoFaturamentoClienteDuplicado(double cnpjCpfPessoa, int codigoGrupoPessoa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente>();

            query = query.Where(o => o.Codigo != codigo && o.Status == true);

            if (cnpjCpfPessoa > 0)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cnpjCpfPessoa);
            if (codigoGrupoPessoa > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoa);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente BuscarAcordoCliente(double cnpjCpfPessoa, int codigoGrupoPessoa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente>();
            query = query.Where(o => o.Status == true);

            if (cnpjCpfPessoa > 0)
                query = query.Where(o => o.Pessoa.CPF_CNPJ == cnpjCpfPessoa);
            if (codigoGrupoPessoa > 0)
                query = query.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoa);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente> Consultar(double cnpjCpfPessoa, int codigoGrupoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(cnpjCpfPessoa, codigoGrupoPessoa, tipoPessoa, status);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(double cnpjCpfPessoa, int codigoGrupoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = Consultar(cnpjCpfPessoa, codigoGrupoPessoa, tipoPessoa, status);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente> Consultar(double cnpjCpfPessoa, int codigoGrupoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.AcordoFaturamentoCliente>();

            var result = from obj in query select obj;

            if (tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa)
            {
                if (cnpjCpfPessoa > 0)
                    result = result.Where(o => o.Pessoa.CPF_CNPJ == cnpjCpfPessoa);
            }
            else if (tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa)
            {
                if (codigoGrupoPessoa > 0)
                    result = result.Where(o => o.GrupoPessoas.Codigo == codigoGrupoPessoa);
            }

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            return result;
        }

        #endregion
    }
}
