using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios.Comissao
{
    public class FuncionarioComissaoTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>
    {
        public FuncionarioComissaoTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarNaoPresentesNaLista(int codigo, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();
            var result = from obj in query
                         where
                            obj.FuncionarioComissao.Codigo == codigo
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo BuscarPorFuncionarioComissaoETitulo(int funcionarioComissao, int titulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();
            var result = from obj in query
                         where
                            obj.FuncionarioComissao.Codigo == funcionarioComissao
                            && obj.Codigo == titulo
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo> BuscarPorFuncionarioComissao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();
            var result = from obj in query where obj.FuncionarioComissao.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo> ConsultarPorFuncionarioComissao(int funcionarioComissao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();

            var result = from obj in query where obj.FuncionarioComissao.Codigo == funcionarioComissao select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorFuncionarioComissao(int funcionarioComissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();

            var result = from obj in query where obj.FuncionarioComissao.Codigo == funcionarioComissao select obj;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarTitulosPorVendedorPessoa(int codigoFuncionario, DateTime dataInicial, DateTime dataFinal, int codigoEmpresa)
        {            
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            var queryPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Cliente>();
            var queryPessoaVendedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PessoaFuncionario>();

            var queryGrupoPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas>();
            var queryGrupoPessoaVendedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasFuncionario>();

            var queryTituloComissao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissaoTitulo>();

            var resultGrupoPessoaVendedor = from objVendedor in queryGrupoPessoaVendedor where objVendedor.Funcionario.Codigo == codigoFuncionario && objVendedor.DataInicioVigencia.Value.Date <= dataInicial.Date && (objVendedor.DataFimVigencia.Value.Date >= dataFinal.Date || !objVendedor.DataFimVigencia.HasValue) select objVendedor;
            var resultGrupoPessoa = from objPessoa in queryGrupoPessoa where resultGrupoPessoaVendedor.Any(v => v.GrupoPessoas == objPessoa) select objPessoa;

            var resultVendedor = from objVendedor in queryPessoaVendedor where objVendedor.Funcionario.Codigo == codigoFuncionario && objVendedor.DataInicioVigencia.Value.Date <= dataInicial.Date && (objVendedor.DataFimVigencia.Value.Date >= dataFinal.Date || !objVendedor.DataFimVigencia.HasValue) select objVendedor;
            var resultPessoa = from objPessoa in queryPessoa where resultVendedor.Select(v => v.Pessoa.CPF_CNPJ).Contains(objPessoa.CPF_CNPJ) select objPessoa;
            var resultTituloComissao = from objTituloComissao in queryTituloComissao
                                       where objTituloComissao.FuncionarioComissao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Cancelado &&
                                             objTituloComissao.FuncionarioComissao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao.Rejeitada
                                       select objTituloComissao.Titulo;

            var result = from obj in query
                         where
                               obj.TipoTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber &&
                               obj.StatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada &&
                               obj.DataLiquidacao.Value.Date >= dataInicial.Date &&
                               obj.DataLiquidacao.Value.Date <= dataFinal.Date
                         select obj;

            result = result.Where(obj => resultGrupoPessoa.Any(p => p.Clientes.Contains(obj.Pessoa)) || resultPessoa.Any(p => p == obj.Pessoa));
            //result = result.Where(obj => resultPessoa.Select(p => p.CPF_CNPJ).Contains(obj.Pessoa.CPF_CNPJ));
            result = result.Where(obj => !resultTituloComissao.Select(t => t.Codigo).Contains(obj.Codigo));

            if (codigoEmpresa > 0)
                result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Fetch(o => o.Pessoa).Fetch(o => o.FaturaParcela).ThenFetch(o => o.Fatura).ToList();
        }
    }
}
