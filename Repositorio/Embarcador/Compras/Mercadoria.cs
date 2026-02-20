using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class Mercadoria : RepositorioBase<Dominio.Entidades.Embarcador.Compras.Mercadoria>
    {
        public Mercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.Mercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Compras.Mercadoria BuscarPorModoCompra(int codigoReq, int codigoProduto, int codigoEmpresa, decimal quantidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();
            var result = from obj in query
                         where obj.RequisicaoMercadoria.Codigo == codigoReq && obj.ProdutoEstoque.Produto.Codigo == codigoProduto
       && obj.ProdutoEstoque.Empresa.Codigo == codigoEmpresa
       && obj.Modo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Compra
       && obj.Saldo > 0
                         select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarNaoPesentesNaLista(int codigo, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();
            var result = from obj in query
                         where
                            obj.RequisicaoMercadoria.Codigo == codigo
                            && !codigos.Contains(obj.Codigo)
                         select obj.Codigo;

            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Compras.Mercadoria BuscarPorRequisicaoEMercadoria(int requisicao, int mercadoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();
            var result = from obj in query
                         where
                            obj.RequisicaoMercadoria.Codigo == requisicao
                            && obj.Codigo == mercadoria
                         select obj;

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Compras.Mercadoria> BuscarPorRequisicao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();
            var result = from obj in query where obj.RequisicaoMercadoria.Codigo == codigo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.Mercadoria> BuscarPorRequisicaoCompra(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria modo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();
            var result = from obj in query where codigos.Contains(obj.RequisicaoMercadoria.Codigo) && obj.Modo == modo && obj.Saldo > 0 select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Compras.Mercadoria> ConsultarPorRequisicao(int requisicao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();

            var result = from obj in query where obj.RequisicaoMercadoria.Codigo == requisicao select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaPorRequisicao(int requisicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.Mercadoria>();

            var result = from obj in query where obj.RequisicaoMercadoria.Codigo == requisicao select obj;

            return result.Count();
        }
    }
}
