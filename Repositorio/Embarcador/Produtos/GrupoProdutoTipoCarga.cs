using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Produtos
{
    public class GrupoProdutoTipoCarga: RepositorioBase<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>
    {
        public GrupoProdutoTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga> ConsultarPorGrupoProduto(int grupoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.GrupoProduto.Codigo == grupoProduto);
            return result.OrderBy(obj => obj.Posicao).ToList();
        } 
        
        public Task<List<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>> ConsultarPorGrupoProdutoAsync(int grupoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.GrupoProduto.Codigo == grupoProduto);
            return result.OrderBy(obj => obj.Posicao).ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga ConsultarPorGrupoProdutoTipoCarga(int grupoProduto, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.TipoDeCarga.Codigo == codigoTipoCarga && obj.GrupoProduto.Codigo == grupoProduto);
            return result.FirstOrDefault();
        }
        
        public Task<bool> ExisteConsultarPorGrupoProdutoTipoCargaAsync(int grupoProduto, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTipoCarga>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.TipoDeCarga.Codigo == codigoTipoCarga && obj.GrupoProduto.Codigo == grupoProduto);
            return result.AnyAsync();
        }

    }
}
