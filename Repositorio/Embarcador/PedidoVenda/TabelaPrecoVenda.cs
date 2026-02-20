using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.PedidoVenda
{
    public class TabelaPrecoVenda : RepositorioBase<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda>
    {
        public TabelaPrecoVenda(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda> Consultar(int codigoEmpresa, string descricao, int codigoGrupoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (codigoGrupoProduto > 0)
                result = result.Where(obj => obj.GrupoProduto.Codigo == codigoGrupoProduto);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, int codigoGrupoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            if (codigoGrupoProduto > 0)
                result = result.Where(obj => obj.GrupoProduto.Codigo == codigoGrupoProduto);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        public decimal ValorProdutoPorTabelaPrecoPessoa(int codigoEmpresa, double pessoa, int codigoGrupoPessoa, int codigoGrupoProduto, decimal valorVenda)
        {
            var consultaTabela = ConsultarTabelaPorPessoa(codigoEmpresa, codigoGrupoProduto);
            consultaTabela = consultaTabela.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa && obj.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa);

            if (consultaTabela.Count() == 0)
            {
                consultaTabela = ConsultarTabelaPorPessoa(codigoEmpresa, codigoGrupoProduto);
                consultaTabela = consultaTabela.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa && obj.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa);
            }

            if (consultaTabela.Count() > 0)
                return consultaTabela.FirstOrDefault().Valor;
            else
                return valorVenda;
        }

        public int CodigoTabelaPrecoPessoa(int codigoEmpresa, double pessoa, int codigoGrupoPessoa, int codigoGrupoProduto)
        {
            var consultaTabela = ConsultarTabelaPorPessoa(codigoEmpresa, codigoGrupoProduto);
            consultaTabela = consultaTabela.Where(obj => obj.Pessoa.CPF_CNPJ == pessoa && obj.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa);

            if (consultaTabela.Count() == 0)
            {
                consultaTabela = ConsultarTabelaPorPessoa(codigoEmpresa, codigoGrupoProduto);
                consultaTabela = consultaTabela.Where(obj => obj.GrupoPessoas.Codigo == codigoGrupoPessoa && obj.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa);
            }

            if (consultaTabela.Count() > 0)
                return consultaTabela.FirstOrDefault().Codigo;
            else
                return 0;
        }

        private IQueryable<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda> ConsultarTabelaPorPessoa(int codigoEmpresa, int codigoGrupoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.PedidoVenda.TabelaPrecoVenda>();

            var result = from obj in query where obj.Status && obj.DataInicioVigencia.Date <= DateTime.Now.Date && obj.DataFimVigencia.Date >= DateTime.Now.Date select obj;

            result = result.Where(obj => obj.GrupoProduto.Codigo == codigoGrupoProduto);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result;
        }
    }
}
