using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Pedido
{
    public class ImportacaoPedido
    {
        public static void GerarImportacaoPedidoPorFTP(string nomeArquivo, System.Data.DataTable dataTable, List<int> idsColunas, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Pedido.Pedido servicoPedido = new Servicos.Embarcador.Pedido.Pedido();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = servicoPedido.ConfiguracaoImportacaoPedido(unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

            Repositorio.Embarcador.Pedidos.ImportacaoPedido repositorioImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repositorioImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna repositorioImportacaoPedidoColuna = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna(unitOfWork);

            if (dataTable.Rows.Count == 0)
                return;

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = dataTable.Rows.Count,
                DataImportacao = DateTime.Now
            };

            repositorioImportacaoPedido.Inserir(importacaoPedido);

            int numeroColunas = idsColunas.Count;

            if (dataTable.Columns.Count < numeroColunas)
                numeroColunas = dataTable.Columns.Count;

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha linha = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha()
                {
                    ImportacaoPedido = importacaoPedido,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente
                };

                repositorioImportacaoPedidoLinha.Inserir(linha);

                for (int j = 0; j < numeroColunas; j++)
                {
                    if (idsColunas[j] == 0)
                        continue;

                    Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna coluna = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna()
                    {
                        Linha = linha,
                        NomeCampo = configuracoes.Where(obj => obj.Id == idsColunas[j]).Select(obj => obj.Propriedade).FirstOrDefault(),
                        Valor = (dataTable.Rows[i][j]).ToString()
                    };

                    repositorioImportacaoPedidoColuna.Inserir(coluna);
                }
            }

            unitOfWork.CommitChanges();
        }

        public static bool GerarImportacaoPedido(out Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno, string nomeArquivo, string dadosArquivo, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dadosArquivo);

            if (linhas.Count == 0)
            {
                retorno.MensagemAviso = "Nenhuma linha encontrada na planilha";
                return false;
            }

            Repositorio.Embarcador.Pedidos.ImportacaoPedido repImportacaoPedido = new Repositorio.Embarcador.Pedidos.ImportacaoPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha repImportacaoPedidoLinha = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinha(unitOfWork);
            Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna repImportacaoPedidoLinhaColuna = new Repositorio.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido importacaoPedido = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedido
            {
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente,
                Planilha = nomeArquivo,
                QuantidadeLinhas = linhas.Count,
                Usuario = usuario,
                DataImportacao = DateTime.Now
            };

            repImportacaoPedido.Inserir(importacaoPedido, auditado);

            for (int i = 0; i < importacaoPedido.QuantidadeLinhas; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha dadosLinhaArquivo = linhas[i];

                Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha linha = new Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinha()
                {
                    ImportacaoPedido = importacaoPedido,
                    Numero = i + 1,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido.Pendente
                };

                repImportacaoPedidoLinha.Inserir(linha);

                if (dadosLinhaArquivo.Colunas.Count > 0)
                    repImportacaoPedidoLinhaColuna.InsertSQL(linha, dadosLinhaArquivo.Colunas);
            }

            unitOfWork.CommitChanges();

            retorno.MensagemAviso = "Planilha adicionada com sucesso à fila de processamento.";
            retorno.Total = linhas.Count;
            retorno.Importados = linhas.Count;

            return true;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha ConverterParaImportacao(List<Dominio.Entidades.Embarcador.Pedidos.ImportacaoPedidoLinhaColuna> colunas)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha()
            {
                Colunas = colunas.Select(o => new Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna()
                {
                    NomeCampo = o.NomeCampo,
                    Valor = o.Valor
                }).ToList()
            };
        }
    }
}
