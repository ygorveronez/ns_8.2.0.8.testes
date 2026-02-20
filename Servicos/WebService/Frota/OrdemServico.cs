using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.WebService.Frota
{
    public class OrdemServico : ServicoBase
    {
        
        public OrdemServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico ConverterObjetoOrdemServico(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork)
        {
            if (ordemServico == null)
                return null;

            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repFechamentoProduto = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unitOfWork);

            Veiculo serWSVeiculo = new Veiculo(unitOfWork);
            Pessoas.Pessoa serPessoa = new Pessoas.Pessoa(unitOfWork);
            Empresa.Motorista serMotorista = new Empresa.Motorista(unitOfWork);
            Usuarios.Usuario serUsuario = new Usuarios.Usuario(unitOfWork);

            decimal valorTotalProdutosDocumento = repFechamentoProduto.BuscarValorDocumentoPorOrdemServico(ordemServico.Codigo);
            decimal valorOrcado = ordemServico.Orcamento?.ValorTotalOrcado ?? 0;

            Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico os = new Dominio.ObjetosDeValor.Embarcador.Frota.OrdemServico()
            {
                CentroResultado = null,
                Codigo = ordemServico.Codigo,
                CondicaoPagamento = ordemServico.CondicaoPagamento,
                DataFechamento = ordemServico.DataFechamento,
                DataProgramada = ordemServico.DataProgramada,
                DiferencaOrcadoRealizado = (valorOrcado - valorTotalProdutosDocumento),
                Equipamento = serWSVeiculo.ConverterObjetoEquipamento(ordemServico.Equipamento, unitOfWork),
                GrupoServico = ConverterObjetoGrupoServico(ordemServico.GrupoServico, unitOfWork),
                Horimetro = ordemServico.Horimetro,
                KMAtual = ordemServico.QuilometragemVeiculo,
                LocalManutencao = serPessoa.ConverterObjetoPessoa(ordemServico.LocalManutencao),
                Motorista = serMotorista.ConverterObjetoMotorista(ordemServico.Motorista),
                Numero = ordemServico.Numero,
                Observacao = ordemServico.Observacao,
                Operador = serUsuario.ConverterObjetoUsuario(ordemServico.Operador, unitOfWork),
                Produtos = ConverterObjetoProdutosFechamento(ordemServico, unitOfWork),
                Responsavel = serUsuario.ConverterObjetoUsuario(ordemServico.Responsavel, unitOfWork),
                ServicosManutencao = ConverterObjetoServicosManutencao(ordemServico, unitOfWork),
                TipoManutencao = ConverterObjetoTipoManutencao(ordemServico.TipoOrdemServico, unitOfWork),
                ValorOrcado = valorOrcado,
                ValorRealiazdo = valorTotalProdutosDocumento,
                Veiculo = serWSVeiculo.ConverterObjetoVeiculo(ordemServico.Veiculo, unitOfWork),
                DescricaoSituacao = ordemServico.DescricaoSituacao
            };

            return os;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.GrupoServico ConverterObjetoGrupoServico(Dominio.Entidades.Embarcador.Frota.GrupoServico grupoServico, Repositorio.UnitOfWork unitOfWork)
        {
            if (grupoServico == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frota.GrupoServico grupo = new Dominio.ObjetosDeValor.Embarcador.Frota.GrupoServico()
            {
                Codigo = grupoServico.Codigo,
                CodigoIntegracao = grupoServico.CodigoIntegracao,
                Descricao = grupoServico.Descricao
            };

            return grupo;
        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.TipoManutencao ConverterObjetoTipoManutencao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaTipo tipoManutencao, Repositorio.UnitOfWork unitOfWork)
        {
            if (tipoManutencao == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frota.TipoManutencao tipo = new Dominio.ObjetosDeValor.Embarcador.Frota.TipoManutencao()
            {
                Codigo = tipoManutencao.Codigo,
                CodigoIntegracao = tipoManutencao.CodigoIntegracao,
                Descricao = tipoManutencao.Descricao
            };

            return tipo;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Frota.ServicoManutencao> ConverterObjetoServicosManutencao(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork)
        {
            if (ordemServico == null)
                return null;

            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicos = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicos = repServicos.BuscarPorOrdemServico(ordemServico.Codigo);

            if (servicos == null || servicos.Count() == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Frota.ServicoManutencao> servs = new List<Dominio.ObjetosDeValor.Embarcador.Frota.ServicoManutencao>();

            foreach (var servico in servicos)
            {
                Dominio.ObjetosDeValor.Embarcador.Frota.ServicoManutencao s = new Dominio.ObjetosDeValor.Embarcador.Frota.ServicoManutencao()
                {
                    CustoEstimado = servico.CustoEstimado,
                    CustoMedio = servico.CustoMedio,
                    DataUltimaManutencao = servico.UltimaManutencao?.OrdemServico?.DataProgramada,
                    KMUltimaManutencao = servico.UltimaManutencao?.OrdemServico?.QuilometragemVeiculo,
                    TempoEstimado = servico.TempoEstimado,
                    Observacao = servico.Observacao,
                    Servico = ConverterObjetoServico(servico.Servico, unitOfWork)
                };
                servs.Add(s);
            }

            return servs;

        }

        public Dominio.ObjetosDeValor.Embarcador.Frota.Servico ConverterObjetoServico(Dominio.Entidades.Embarcador.Frota.ServicoVeiculoFrota servico, Repositorio.UnitOfWork unitOfWork)
        {
            if (servico == null)
                return null;

            Dominio.ObjetosDeValor.Embarcador.Frota.Servico serv = new Dominio.ObjetosDeValor.Embarcador.Frota.Servico()
            {
                Codigo = servico.Codigo,
                CodigoIntegracao = servico.CodigoIntegracao,
                Descricao = servico.Descricao
            };

            return serv;
        }


        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> ConverterObjetoProdutosFechamento(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico, Repositorio.UnitOfWork unitOfWork)
        {
            if (ordemServico == null)
                return null;

            Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto repProdutos = new Repositorio.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto> produtos = repProdutos.BuscarPorOrdemServico(ordemServico.Codigo);

            if (produtos == null || produtos.Count() == 0)
                return null;

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> prods = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
            foreach (var produto in produtos)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.Produto p = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
                {
                    DescricaoProduto = produto.Produto.Descricao,
                    Codigo = produto.Produto.Codigo,
                    CodigoProduto = produto.Produto.CodigoProduto,
                    UnidadeMedida = produto.Produto.UnidadeDeMedida.HasValue ? produto.Produto.UnidadeDeMedida.Value.ObterSigla() : "UNID",
                    Quantidade = produto.QuantidadeOrcada,
                    ValorUnitario = produto.ValorUnitario,
                    ValorTotal = produto.ValorUnitario * produto.QuantidadeOrcada,
                    FinalidadeProduto = ConverterObjetoFinalidadeProduto(produto.FinalidadeProduto, unitOfWork)
                };
                prods.Add(p);
            }
            return prods;
        }

        public Dominio.ObjetosDeValor.Embarcador.Pedido.FinalidadeProduto ConverterObjetoFinalidadeProduto(Dominio.Entidades.Embarcador.Frota.FinalidadeProdutoOrdemServico finalidade, Repositorio.UnitOfWork unitOfWork)
        {
            if (finalidade == null)
                return null;

            Servicos.WebService.Financeiro.TipoMovimento serTipoMovimento = new Financeiro.TipoMovimento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Pedido.FinalidadeProduto fin = new Dominio.ObjetosDeValor.Embarcador.Pedido.FinalidadeProduto()
            {
                CodigoIntegracao = finalidade.CodigoIntegracao,
                Descricao = finalidade.Descricao,
                PlanoContaCredito = serTipoMovimento.ConverterObjetoPlanoDeConta(finalidade.TipoMovimentoUso?.PlanoDeContaCredito, unitOfWork),
                PlanoContaDebito = serTipoMovimento.ConverterObjetoPlanoDeConta(finalidade.TipoMovimentoUso?.PlanoDeContaDebito, unitOfWork)
            };

            return fin;
        }
    }
}
