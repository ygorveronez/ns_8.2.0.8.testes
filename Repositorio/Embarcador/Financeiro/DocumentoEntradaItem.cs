using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Financeiro
{
    public class DocumentoEntradaItem : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>
    {
        public DocumentoEntradaItem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.Codigo == codigo select obj;

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> BuscarPorDocumentoEntrada(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> BuscarPorDocumentoEntradaEOrdemServico(int[] codigoDocumentoEntrada, int codigoOrdemServico)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where codigoDocumentoEntrada.Contains(obj.DocumentoEntrada.Codigo) && (obj.OrdemServico.Codigo == codigoOrdemServico || obj.OrdensServico.Any(o => o.OrdemServico.Codigo == codigoOrdemServico)) select obj;

            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> BuscarPorOrdemCompraEProduto(int codigoOrdemCompra, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.OrdemCompra.Codigo == codigoOrdemCompra && obj.Produto.Codigo == codigoProduto select obj;

            return query.ToList();
        }
        public List<decimal> BuscarAliquotasICMS(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.AliquotaICMS > 0 select obj;

            return query.Select(obj => obj.AliquotaICMS).Distinct().ToList();
        }

        public decimal BaseICMSAliquota(int codigoDocumentoEntrada, decimal aliquotaICMS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.AliquotaICMS == aliquotaICMS select obj;

            if (query.Count() > 0)
                return query.Select(obj => obj.BaseCalculoICMS).Sum();
            else
                return 0;
        }

        public decimal ValorICMSAliquota(int codigoDocumentoEntrada, decimal aliquotaICMS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.AliquotaICMS == aliquotaICMS select obj;

            if (query.Count() > 0)
                return query.Select(obj => obj.ValorICMS).Sum();
            else
                return 0;
        }

        public decimal ValorMercadoriasComST(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.ValorICMSST > 0 select obj;

            if (query.Count() > 0)
                return query.Select(obj => obj.ValorTotal).Sum();
            else
                return 0;
        }

        public decimal ValorOutrasICMS(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && (obj.CSTICMS == "040" || obj.CSTICMS == "40") select obj;

            if (query.Count() > 0)
                return query.Select(obj => obj.ValorTotal).Sum();
            else
                return 0;
        }

        public decimal ValorBaseICMS(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            if (query.Count() > 0)
                return query.Select(obj => obj.BaseCalculoICMS).Sum();
            else
                return 0;
        }

        public decimal ValorTotalItens(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            if (query.Count() > 0)
                return query.Select(obj => obj.ValorTotal).Sum();
            else
                return 0;
        }

        public bool RealizarRateioDespesaVeiculo(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();
            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.CFOP != null && obj.CFOP.RealizarRateioDespesaVeiculo select obj;
            return query.Any();
        }

        public decimal ValorRateioDespesaVeiculo(int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();
            query = from obj in query where obj.DocumentoEntrada.Codigo == codigoDocumentoEntrada && obj.CFOP != null && obj.CFOP.RealizarRateioDespesaVeiculo select obj;
            if (query.Count() > 0)
                return query.Select(obj => obj.ValorCustoTotal).Sum();
            else
                return 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> BuscarPorDocumentosEntrada(List<int> codigosDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();
            var result = from obj in query where codigosDocumentoEntrada.Contains(obj.DocumentoEntrada.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> BuscarPorOrdemCompra(int codigoOrdemCompra, int codigoDocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();
            var result = from obj in query
                         where obj.DocumentoEntrada.OrdemCompra.Codigo == codigoOrdemCompra &&
                                obj.DocumentoEntrada.Codigo != codigoDocumentoEntrada &&
                                obj.DocumentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado
                         select obj;
            return result.ToList();
        }

        #endregion
    }
}
