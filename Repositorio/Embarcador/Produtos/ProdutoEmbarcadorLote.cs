using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class ProdutoEmbarcadorLote : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>
    {
        public ProdutoEmbarcadorLote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote BuscarPorCodigoBarras(string codigoBarras, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria tipoRecebimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            var result = from obj in query where obj.CodigoBarras == codigoBarras && obj.TipoRecebimentoMercadoria == tipoRecebimento select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote BuscarPorCodigoBarrasPosicao(string codigoBarras, int codigoPosicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            var result = from obj in query where obj.CodigoBarras.Contains(codigoBarras) && obj.DepositoPosicao.Codigo == codigoPosicao select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote BuscarPorLote(int codigoProduto, List<int> codigosNaoSelecionar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            var result = from obj in query where obj.ProdutoEmbarcador.Codigo == codigoProduto && obj.QuantidadeAtual > 0 select obj;
            if (codigosNaoSelecionar != null && codigosNaoSelecionar.Count > 0)
                result = result.Where(obj => !codigosNaoSelecionar.Contains(obj.Codigo));
            return result.OrderBy("DataVencimento").FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote BuscarPorLote(int codigoProduto, string numeroNota, decimal quantidade, double cnpjDestinatario, double cnpjRemetente, List<int> codigosNaoSelecionar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            var result = from obj in query where obj.ProdutoEmbarcador.Codigo == codigoProduto && obj.QuantidadeAtual > 0 && obj.Numero == numeroNota && obj.Destinatario.CPF_CNPJ == cnpjDestinatario && obj.Remetente.CPF_CNPJ == cnpjRemetente select obj;
            if (codigosNaoSelecionar != null && codigosNaoSelecionar.Count > 0)
                result = result.Where(obj => !codigosNaoSelecionar.Contains(obj.Codigo));
            return result.OrderBy("DataVencimento").FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote BuscarPorProduto(int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();
            var result = from obj in query where obj.ProdutoEmbarcador.Codigo == codigoProduto select obj;
            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> Consultar(string descricao, string codigoBarras, string numero, bool ativo, int codigoProduto, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();

            var result = from obj in query select obj;

            if (codigoProduto > 0)
                result = result.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProduto);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(obj => obj.Numero.Contains(numero));

            if (!string.IsNullOrWhiteSpace(codigoBarras))
                result = result.Where(obj => obj.CodigoBarras.Contains(codigoBarras));

            result = result.Where(obj => obj.ProdutoEmbarcador.Ativo == ativo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, string codigoBarras, string numero, bool ativo, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();

            var result = from obj in query select obj;

            if (codigoProduto > 0)
                result = result.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProduto);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numero))
                result = result.Where(obj => obj.Numero.Contains(numero));

            if (!string.IsNullOrWhiteSpace(codigoBarras))
                result = result.Where(obj => obj.CodigoBarras.Contains(codigoBarras));

            result = result.Where(obj => obj.ProdutoEmbarcador.Ativo == ativo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote> Consultar(int codigoDepositoPosicao, int codigoDepositoBloco, int codigoDepositoRua, int codigoDeposito, int codigoProdutoEmbarcador, int codigoProdutoEmbarcadorLote, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();

            var result = from obj in query where obj.QuantidadeAtual > 0 select obj;

            if (codigoProdutoEmbarcador > 0)
                result = result.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador);

            if (codigoProdutoEmbarcadorLote > 0)
                result = result.Where(obj => obj.Codigo == codigoProdutoEmbarcadorLote);

            if (codigoDepositoPosicao > 0)
                result = result.Where(obj => obj.DepositoPosicao.Codigo == codigoDepositoPosicao);

            if (codigoDepositoBloco > 0)
                result = result.Where(obj => obj.DepositoPosicao.Bloco.Codigo == codigoDepositoBloco);

            if (codigoDepositoRua > 0)
                result = result.Where(obj => obj.DepositoPosicao.Bloco.Rua.Codigo == codigoDepositoRua);

            if (codigoDeposito > 0)
                result = result.Where(obj => obj.DepositoPosicao.Bloco.Rua.Deposito.Codigo == codigoDeposito);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoDepositoPosicao, int codigoDepositoBloco, int codigoDepositoRua, int codigoDeposito, int codigoProdutoEmbarcador, int codigoProdutoEmbarcadorLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote>();

            var result = from obj in query where obj.QuantidadeAtual > 0 select obj;

            if (codigoProdutoEmbarcador > 0)
                result = result.Where(obj => obj.ProdutoEmbarcador.Codigo == codigoProdutoEmbarcador);

            if (codigoProdutoEmbarcadorLote > 0)
                result = result.Where(obj => obj.Codigo == codigoProdutoEmbarcadorLote);

            if (codigoDepositoPosicao > 0)
                result = result.Where(obj => obj.DepositoPosicao.Codigo == codigoDepositoPosicao);

            if (codigoDepositoBloco > 0)
                result = result.Where(obj => obj.DepositoPosicao.Bloco.Codigo == codigoDepositoBloco);

            if (codigoDepositoRua > 0)
                result = result.Where(obj => obj.DepositoPosicao.Bloco.Rua.Codigo == codigoDepositoRua);

            if (codigoDeposito > 0)
                result = result.Where(obj => obj.DepositoPosicao.Bloco.Rua.Deposito.Codigo == codigoDeposito);

            return result.Count();
        }

        #endregion
    }
}
