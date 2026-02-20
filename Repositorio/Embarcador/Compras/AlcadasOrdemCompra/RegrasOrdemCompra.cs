using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras.AlcadasOrdemCompra
{
    public class RegrasOrdemCompra : RepositorioBase<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>
    {
        public RegrasOrdemCompra(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> ConsultarRegras(int codigoEmpresa, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao);

            if (inicioRegistros > 0 && maximoRegistros > inicioRegistros)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result
                    .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                    .ToList();
        }

        public int ContarConsultaRegras(int codigoEmpresa, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var result = _ConsultarRegras(codigoEmpresa, dataInicio, dataFim, aprovador, descricao);

            return result.Count();
        }

        #endregion

        #region Alçadas de Aprovação

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorFornecedor(double fornecedor, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Fornecedor.CPF_CNPJ == fornecedor) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Fornecedor.CPF_CNPJ != fornecedor)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorFornecedor == true && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorOperador(int operador, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Operador.Codigo == operador) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Operador.Codigo != operador)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorOperador == true && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorSetorOperador(int setorOperador, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Setor.Codigo == setorOperador) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Setor.Codigo != setorOperador)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorSetorOperador == true && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorProduto(int produto, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Produto.Codigo == produto) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Produto.Codigo != produto)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorProduto == true && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorValor(decimal valor, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.Valor == valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.Valor != valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && obj.Valor <= valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && obj.Valor < valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && obj.Valor >= valor ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && obj.Valor > valor)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorValor == true && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorGrupoProduto(int codigoGrupoProduto, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.GrupoProdutoTMS.Codigo == codigoGrupoProduto) ||
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.GrupoProdutoTMS.Codigo != codigoGrupoProduto)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorGrupoProduto && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> AlcadasPorPercentualDiferencaValorCustoProduto(decimal percentual, DateTime data, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto>();
            var result = from obj in query
                         where
                            (obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && obj.PercentualDiferencaValorCustoProduto == percentual ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && obj.PercentualDiferencaValorCustoProduto != percentual ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && obj.PercentualDiferencaValorCustoProduto <= percentual ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && obj.PercentualDiferencaValorCustoProduto < percentual ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && obj.PercentualDiferencaValorCustoProduto >= percentual ||
                             obj.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && obj.PercentualDiferencaValorCustoProduto > percentual)
                         select obj.RegrasOrdemCompra;

            result = result.Where(o => o.RegraPorPercentualDiferencaValorCustoProduto && (o.Vigencia >= data || o.Vigencia == null));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra> _ConsultarRegras(int codigoEmpresa, DateTime? dataInicio, DateTime? dataFim, Dominio.Entidades.Usuario aprovador, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.RegrasOrdemCompra>();
            var result = from obj in query select obj;

            if (dataInicio.HasValue && dataFim.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value && o.Vigencia < dataFim.Value.AddDays(1));
            else if (dataInicio.HasValue)
                result = result.Where(o => o.Vigencia >= dataInicio.Value);
            else if (dataFim.HasValue)
                result = result.Where(o => o.Vigencia < dataFim.Value.AddDays(1));

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (aprovador != null)
                result = result.Where(o => o.Aprovadores.Contains(aprovador));

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        #endregion
    }
}
