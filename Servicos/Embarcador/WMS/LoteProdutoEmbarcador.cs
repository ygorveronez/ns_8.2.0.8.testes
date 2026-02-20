namespace Servicos.Embarcador.WMS
{
    public class LoteProdutoEmbarcador : ServicoBase
    {
        public LoteProdutoEmbarcador() : base() { }        
        public LoteProdutoEmbarcador(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.Embarcador.WMS.DepositoPosicao BuscarPosicaoPadrao(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto, decimal quantidade, decimal peso, decimal metroCubico, decimal qtdPalet, Repositorio.UnitOfWork unitOfWork)
        {
            if (produto == null)
                return null;
            else
            {
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao = null;
                Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                posicao = repDepositoPosicao.BuscarPorProduto(produto.Codigo, peso, metroCubico, qtdPalet);
                if (posicao != null)
                    return posicao;

                posicao = repDepositoPosicao.BuscarProximaPosicao(peso, metroCubico, qtdPalet);
                return posicao;
            }
        }

        public dynamic ObterDetalhesLoteProduto(Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote lote)
        {
            var dynLote = new
            {
                lote.Codigo,
                CodigoProdutoEmbarcador = lote.ProdutoEmbarcador.Codigo,
                CodigoArmazenamento = lote.DepositoPosicao.Codigo,
                Descricao = lote.Descricao,
                CodigoBarras = !string.IsNullOrEmpty(lote.CodigoBarras) ? lote.CodigoBarras.Length > 32 ? lote.CodigoBarras.Replace("|", " ") : lote.CodigoBarras : "",
                NumeroLote = lote.Numero,
                DataVencimento = lote.DataVencimento != null && lote.DataVencimento.HasValue ? lote.DataVencimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                QuantidadeAtual = lote.QuantidadeAtual.ToString("n3"),
                Peso = (lote.Peso).ToString("n3"),//(lote.Peso * lote.QuantidadeAtual).ToString("n3"),
                QuantidadePalet = (lote.QuantidadePalet).ToString("n3"),//(lote.QuantidadePalet * lote.QuantidadeAtual).ToString("n3"),
                Armazenamento = lote.DepositoPosicao.Abreviacao,
                Cubagem = (lote.MetroCubico).ToString("n3")//(lote.MetroCubico * lote.QuantidadeAtual).ToString("n3")
            };

            return dynLote;
        }

        public dynamic ObterDetalhesPosicao(Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao)
        {
            var dynLote = new
            {
                posicao.Codigo,
                Armazenamento = posicao.Abreviacao,
                PesoMaximo = posicao.PesoMaximo.ToString("n3"),
                PesoAtual = posicao.PesoAtual.ToString("n3"),
                QuantidadePaletMaximo = posicao.QuantidadePaletes.ToString("n3"),
                QuantidadePaletAtual = posicao.QuantidadePaletesAtual.ToString("n3"),
                MetroCubicoMaximo = posicao.MetroCubicoMaximo.ToString("n3"),
                MetroCubicoAtual = posicao.MetroCubicoAtual.ToString("n3")
            };

            return dynLote;
        }

        public static void BaixarNoEstoqueSeparacao(Dominio.Entidades.Embarcador.WMS.Separacao separacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (separacao.SituacaoSelecaoSeparacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSelecaoSeparacao.Finalizada)
                return;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.WMS.SeparacaoProdutoEmbarcador produto in separacao.Produtos)
            {
                produto.ProdutoEmbarcadorLote.QuantidadeAtual -= produto.QuantidadeSeparada;
                repProdutoEmbarcadorLote.Atualizar(produto.ProdutoEmbarcadorLote);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, produto.ProdutoEmbarcadorLote, null, "Finalizou a Separação e baixou " + produto.QuantidadeSeparada.ToString("n3") + " do estoque.", unitOfWork);
            }
        }

        public static void GerarExpedicao(Dominio.Entidades.Embarcador.WMS.Separacao separacao, Repositorio.UnitOfWork unitOfWork)
        {
            GestaoPatio.Expedicao servicoExpedicao = new GestaoPatio.Expedicao(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.WMS.SelecaoCarga selecaoCarga in separacao.Selecao.Cargas)
                servicoExpedicao.Adicionar(selecaoCarga.Carga);
        }
    }
}
