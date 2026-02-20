using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public sealed class Cheque : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Cheque>
    {
        #region Construtores

        public Cheque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.Cheque> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCheque filtrosPesquisa)
        {
            var consultaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Cheque>();

            if (filtrosPesquisa.CpfCnpjPessoa > 0d)
                consultaCheque = consultaCheque.Where(o => o.Pessoa.CPF_CNPJ == filtrosPesquisa.CpfCnpjPessoa);

            if (filtrosPesquisa.Status.HasValue)
                consultaCheque = consultaCheque.Where(o => o.Status == filtrosPesquisa.Status.Value);

            if (filtrosPesquisa.Tipo.HasValue)
                consultaCheque = consultaCheque.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            if (filtrosPesquisa.Tipos?.Count > 0)
                consultaCheque = consultaCheque.Where(o => filtrosPesquisa.Tipos.Contains(o.Tipo));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCheque))
                consultaCheque = consultaCheque.Where(o => o.NumeroCheque.Contains(filtrosPesquisa.NumeroCheque));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaCheque = consultaCheque.Where(o => o.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            return consultaCheque;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.Cheque BuscarPorCodigo(int codigo)
        {
            var consultaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Cheque>()
                .Where(o => o.Codigo == codigo);

            return consultaCheque.FirstOrDefault();
        }

        public bool ChequeDuplicado(int codigo, string numeroCheque, DateTime dataVencimento, int codigoBanco, int codigoEmpresa, double cnpjPessoa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheque tipo, string numeroAgencia, string numeroConta)
        {
            var consultaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Cheque>();
            consultaCheque = consultaCheque.Where(o => o.Codigo != codigo);

            if (!string.IsNullOrWhiteSpace(numeroCheque))
                consultaCheque = consultaCheque.Where(o => o.NumeroCheque == numeroCheque);

            if (!string.IsNullOrWhiteSpace(numeroAgencia))
                consultaCheque = consultaCheque.Where(o => o.NumeroAgencia == numeroAgencia);

            if (!string.IsNullOrWhiteSpace(numeroConta))
                consultaCheque = consultaCheque.Where(o => o.NumeroConta == numeroConta);

            /*if (dataVencimento > DateTime.MinValue)
                consultaCheque = consultaCheque.Where(o => o.DataVencimento.Date == dataVencimento.Date);

            if (codigoBanco > 0)
                consultaCheque = consultaCheque.Where(o => o.Banco.Codigo == codigoBanco);*/

            if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheque.Emitido && codigoEmpresa > 0)
                consultaCheque = consultaCheque.Where(o => o.Empresa.Codigo == codigoEmpresa);
            if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheque.Recebido && cnpjPessoa > 0)
                consultaCheque = consultaCheque.Where(o => o.Pessoa.CPF_CNPJ == cnpjPessoa);

            return consultaCheque.Any();
        }

        public int BuscarProximoNumero()
        {
            var consultaCheque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Cheque>();
            int? ultimoNumero = consultaCheque.Max(o => (int?)o.Numero);

            return ultimoNumero.HasValue ? (ultimoNumero.Value + 1) : 1;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Cheque> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCheque filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaCheque = Consultar(filtrosPesquisa);

            return ObterLista(consultaCheque, parametroConsulta);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Cheque> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCheque = new ConsultaCheque().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaCheque.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.Cheque)));

            return consultaCheque.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Cheque>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCheque filtrosPesquisa)
        {
            var consultaCheque = Consultar(filtrosPesquisa);

            return consultaCheque.Count();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaChequeRelatorio filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaCheque = new ConsultaCheque().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaCheque.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
