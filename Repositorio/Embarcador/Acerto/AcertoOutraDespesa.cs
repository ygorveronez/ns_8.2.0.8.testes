using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoOutraDespesa : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>
    {
        public AcertoOutraDespesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> BuscarPorAcerto(int codigoAcerto)
        {
            IQueryable<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();

            query = query.Where(o => o.AcertoViagem.Codigo == codigoAcerto);

            return query.ToList();
        }

        public decimal BuscarValorMoedaestrangeira(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral[] moedas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && (obj.MoedaCotacaoBancoCentral == null || moedas.Contains(obj.MoedaCotacaoBancoCentral.Value)));
            else
                query = query.Where(obj => obj.AcertoViagem.Codigo == codigo && obj.MoedaCotacaoBancoCentral != null && moedas.Contains(obj.MoedaCotacaoBancoCentral.Value));

            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == false select obj;
            query = query.Where(a => resultModalidade.Any(c => c.ModalidadePessoas.Cliente == a.Pessoa));

            if (moedas.Any(c => c == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.Real))
                return query.Sum(o => (decimal?)o.Valor) ?? 0m;
            else
                return query.Sum(o => (decimal?)o.ValorOriginalMoedaEstrangeira) ?? 0m;
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> BuscarPorCodigoAcertoVeiculo(int codigo, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Fatura.Justificativa> BuscarJustificativas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fatura.Justificativa>();
            var result = from obj in query select obj;

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var resultoAcerto = from obj in queryAcerto where obj.AcertoViagem.Codigo == codigo select obj;

            result = result.Where(obj => resultoAcerto.Select(a => a.Justificativa).Contains(obj));

            return result.ToList();
        }

        public List<Dominio.Entidades.Produto> BuscarProdutos(int codigo, int codigoJustificativa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();
            var result = from obj in query select obj;

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var resultoAcerto = from obj in queryAcerto where obj.AcertoViagem.Codigo == codigo && obj.Justificativa.Codigo == codigoJustificativa select obj;

            result = result.Where(obj => resultoAcerto.Select(a => a.Produto).Contains(obj));

            return result.ToList();
        }

        public Dominio.Entidades.Produto ProdutoPrincipalOutrasDespesas(int codigoAcerto, DateTime? dataDespesa, int codigoJustificativa, bool pagoPorFatura)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();

            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == pagoPorFatura select obj;
            var result = resultModalidade.Join(query, vei => vei.ModalidadePessoas.Cliente.CPF_CNPJ, emp => emp.Pessoa.CPF_CNPJ, (vei, emp) => emp);

            result = result.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Justificativa.Codigo == codigoJustificativa && obj.Produto != null);
            if (dataDespesa.HasValue)
                result = result.Where(obj => obj.Data == dataDespesa);

            return result.Select(c => c.Produto).FirstOrDefault() ?? null;
        }

        public decimal ValorOutrasDespesas(int codigoAcerto, DateTime? dataDespesa, int codigoJustificativa, bool pagoPorFatura, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();

            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == pagoPorFatura select obj;
            var result = resultModalidade.Join(query, vei => vei.ModalidadePessoas.Cliente.CPF_CNPJ, emp => emp.Pessoa.CPF_CNPJ, (vei, emp) => emp);

            result = result.Where(obj => obj.AcertoViagem.Codigo == codigoAcerto && obj.Justificativa.Codigo == codigoJustificativa);
            if (dataDespesa.HasValue)
                result = result.Where(obj => obj.Data == dataDespesa);
            if (codigoProduto > 0)
                result = result.Where(obj => obj.Produto.Codigo == codigoProduto);

            return result.Sum(o => (decimal?)o.Valor * ((decimal?)o.Quantidade ?? 1)) ?? 0m;

            //if (result.Count() > 0)
            //    return result.Sum(obj => obj.Valor);
            //else
            //    return 0;
        }

        public List<DateTime> BuscarDatasJustificativas(int codigo)
        {
            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var resultoAcerto = from obj in queryAcerto where obj.AcertoViagem.Codigo == codigo select obj;

            return resultoAcerto.Select(obj => obj.Data).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> BuscarPorVeiculoAcerto(int codigoAcerto, int codigoVeiculo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).Distinct().ToList();
        }

        public int ContarBuscarPorVeiculoAcerto(int codigoAcerto, int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigoAcerto && obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.Count();
        }

        public decimal ReceitaDespesaOutraDespesa(int codigoAcerto, bool pagoPorFatura)
        {
            var queryDespesa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == pagoPorFatura select obj;

            var resultDespesa = resultModalidade.Join(queryDespesa, vei => vei.ModalidadePessoas.Cliente.CPF_CNPJ, emp => emp.Pessoa.CPF_CNPJ, (vei, emp) => emp);

            resultDespesa = from obj in resultDespesa where obj.AcertoViagem.Codigo == codigoAcerto select obj;
            if (!pagoPorFatura)
                resultDespesa = resultDespesa.Where(o => o.DespesaPagaPeloAdiantamento == null || o.DespesaPagaPeloAdiantamento == false);

            return resultDespesa.Sum(o => (decimal?)o.Valor * ((decimal?)o.Quantidade ?? 1)) ?? 0m;

            //if (resultDespesa.Count() > 0)
            //    return (from obj in resultDespesa select obj.Valor).Sum();
            //else
            //    return 0;
        }

        public decimal ReceitaDespesaOutraDespesaVeiculo(int codigoAcerto, bool pagoPorFatura, int codigoVeiculo)
        {
            var queryDespesa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();
            var queryModalidadeFornecedor = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas>();
            var resultModalidade = from obj in queryModalidadeFornecedor where obj.PagoPorFatura == pagoPorFatura select obj;

            var resultDespesa = resultModalidade.Join(queryDespesa, vei => vei.ModalidadePessoas.Cliente.CPF_CNPJ, emp => emp.Pessoa.CPF_CNPJ, (vei, emp) => emp);

            resultDespesa = from obj in resultDespesa where obj.Veiculo.Codigo == codigoVeiculo && obj.AcertoViagem.Codigo == codigoAcerto select obj;

            return resultDespesa.Sum(o => (decimal?)o.Valor) ?? 0m;

            //if (resultDespesa.Count() > 0)
            //    return (from obj in resultDespesa select obj.Valor).Sum();
            //else
            //    return 0;
        }

        public bool ContemPorChamado(int codigoChamado)
        {
            IQueryable<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOutraDespesa>();

            query = query.Where(o => o.Chamado.Codigo == codigoChamado);

            return query.Any();
        }

        #endregion
    }
}