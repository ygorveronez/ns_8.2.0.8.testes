using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class RegraEntradaDocumento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>
    {
        public RegraEntradaDocumento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento> BuscarAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>();
            query = query.Where(obj => obj.Ativo == true);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento BuscarRegraEntradaNCMCompleto(int codigoEmpresa, double codigoFornecedor, string ncm)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.Pessoa.CPF_CNPJ == codigoFornecedor || obj.Fornecedores.Any(o => o.Pessoa.CPF_CNPJ == codigoFornecedor)) && obj.Ativo select obj;

            if (!string.IsNullOrWhiteSpace(ncm))
            {
                if (ncm.Length == 8)
                    result = result.Where(obj => obj.NCMs.Any(o => o.NCM.Contains(ncm)));
                else
                    result = result.Where(obj => obj.NCMs.Any(o => o.NCM.StartsWith(ncm)));
            }
            else
                result = result.Where(obj => !obj.NCMs.Any());

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento BuscarRegraEntrada(int codigoEmpresa, double codigoFornecedor, string ncm)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && (obj.Pessoa.CPF_CNPJ == codigoFornecedor || obj.Fornecedores.Any(o => o.Pessoa.CPF_CNPJ == codigoFornecedor)) && obj.Ativo select obj;

            if (!string.IsNullOrWhiteSpace(ncm))
            {
                if (ncm.Length == 8)
                    result = result.Where(obj => obj.NCMs.Any(o => o.NCM.StartsWith(ncm.Substring(0, 4))));
                else
                    result = result.Where(obj => obj.NCMs.Any(o => o.NCM.StartsWith(ncm)));
            }
            else
                result = result.Where(obj => !obj.NCMs.Any());

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string ncm, int codigoEmpresa, double codigoPessoa, int codigoNaturezaDaOperacao, int codigoCFOPDentro, int codigoCFOPFora, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoPessoa > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == codigoPessoa || obj.Fornecedores.Any(o => o.Pessoa.CPF_CNPJ == codigoPessoa));
            if (codigoNaturezaDaOperacao > 0)
                result = result.Where(obj => obj.NaturezaOperacao.Codigo == codigoNaturezaDaOperacao);
            if (codigoCFOPDentro > 0)
                result = result.Where(obj => obj.CFOPDentro.Codigo == codigoCFOPDentro);
            if (codigoCFOPFora > 0)
                result = result.Where(obj => obj.CFOPFora.Codigo == codigoCFOPFora);
            if (!String.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (!string.IsNullOrWhiteSpace(ncm))
                result = result.Where(obj => (from p in obj.NCMs where p.NCM.Contains(ncm) select new { p.Codigo }).Count() > 0);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string ncm, int codigoEmpresa, double codigoPessoa, int codigoNaturezaDaOperacao, int codigoCFOPDentro, int codigoCFOPFora, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoPessoa > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == codigoPessoa || obj.Fornecedores.Any(o => o.Pessoa.CPF_CNPJ == codigoPessoa));
            if (codigoNaturezaDaOperacao > 0)
                result = result.Where(obj => obj.NaturezaOperacao.Codigo == codigoNaturezaDaOperacao);
            if (codigoCFOPDentro > 0)
                result = result.Where(obj => obj.CFOPDentro.Codigo == codigoCFOPDentro);
            if (codigoCFOPFora > 0)
                result = result.Where(obj => obj.CFOPFora.Codigo == codigoCFOPFora);
            if (!String.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (!string.IsNullOrWhiteSpace(ncm))
                result = result.Where(obj => (from p in obj.NCMs where p.NCM.Contains(ncm) select new { p.Codigo }).Count() > 0);

            return result.Count();
        }

        #endregion
    }
}
