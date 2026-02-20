using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.ProdutoEmbarcador
{
    public class Lote
    {
        #region Métodos Públicos

        public static List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> VerificarRegrasAutorizacaoDescarte(Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.WMS.RegraDescarte repRegraDescarte = new Repositorio.Embarcador.WMS.RegraDescarte(unitOfWork);
            List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> listaRegras = new List<Dominio.Entidades.Embarcador.WMS.RegraDescarte>();
            List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> listaFiltrada = new List<Dominio.Entidades.Embarcador.WMS.RegraDescarte>();
            List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> alcadasCompativeis;

            // Produto Embarcador
            alcadasCompativeis = repRegraDescarte.AlcadasPorProdutoEmbarcador(descarte.Lote?.ProdutoEmbarcador.Codigo ?? 0, descarte.Data);
            listaRegras.AddRange(alcadasCompativeis);

            // Deposito
            alcadasCompativeis = repRegraDescarte.AlcadasPorDeposito(descarte.Lote?.DepositoPosicao.Bloco.Rua.Deposito.Codigo ?? 0, descarte.Data);
            listaRegras.AddRange(alcadasCompativeis);

            // Rua
            alcadasCompativeis = repRegraDescarte.AlcadasPorRua(descarte.Lote?.DepositoPosicao.Bloco.Rua.Codigo ?? 0, descarte.Data);
            listaRegras.AddRange(alcadasCompativeis);

            // Bloco
            alcadasCompativeis = repRegraDescarte.AlcadasPorBloco(descarte.Lote?.DepositoPosicao.Bloco.Codigo ?? 0, descarte.Data);
            listaRegras.AddRange(alcadasCompativeis);

            // Posicao
            alcadasCompativeis = repRegraDescarte.AlcadasPorPosicao(descarte.Lote?.DepositoPosicao.Codigo ?? 0, descarte.Data);
            listaRegras.AddRange(alcadasCompativeis);

            // Quantidade
            alcadasCompativeis = repRegraDescarte.AlcadasPorQuantidade(descarte.Quantidade, descarte.Data);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada = listaRegras;
                foreach (Dominio.Entidades.Embarcador.WMS.RegraDescarte regra in listaRegras)
                {
                    if (regra.RegraPorProdutoEmbarcador)
                    {
                        bool valido = false;
                        if (regra.AlcadasProdutoEmbarcador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.ProdutoEmbarcador.Codigo == descarte.Lote.ProdutoEmbarcador.Codigo))
                            valido = true;
                        else if (regra.AlcadasProdutoEmbarcador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.ProdutoEmbarcador.Codigo == descarte.Lote.ProdutoEmbarcador.Codigo))
                            valido = true;
                        else if (regra.AlcadasProdutoEmbarcador.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.ProdutoEmbarcador.Codigo != descarte.Lote.ProdutoEmbarcador.Codigo))
                            valido = true;
                        else if (regra.AlcadasProdutoEmbarcador.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.ProdutoEmbarcador.Codigo != descarte.Lote.ProdutoEmbarcador.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorDeposito)
                    {
                        bool valido = false;
                        if (regra.AlcadasDeposito.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Deposito.Codigo == descarte.Lote.DepositoPosicao.Bloco.Rua.Deposito.Codigo))
                            valido = true;
                        else if (regra.AlcadasDeposito.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Deposito.Codigo == descarte.Lote.DepositoPosicao.Bloco.Rua.Deposito.Codigo))
                            valido = true;
                        else if (regra.AlcadasDeposito.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Deposito.Codigo != descarte.Lote.DepositoPosicao.Bloco.Rua.Deposito.Codigo))
                            valido = true;
                        else if (regra.AlcadasDeposito.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Deposito.Codigo != descarte.Lote.DepositoPosicao.Bloco.Rua.Deposito.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorRua)
                    {
                        bool valido = false;
                        if (regra.AlcadasRua.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Rua.Codigo == descarte.Lote.DepositoPosicao.Bloco.Rua.Codigo))
                            valido = true;
                        else if (regra.AlcadasRua.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Rua.Codigo == descarte.Lote.DepositoPosicao.Bloco.Rua.Codigo))
                            valido = true;
                        else if (regra.AlcadasRua.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Rua.Codigo != descarte.Lote.DepositoPosicao.Bloco.Rua.Codigo))
                            valido = true;
                        else if (regra.AlcadasRua.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Rua.Codigo != descarte.Lote.DepositoPosicao.Bloco.Rua.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorBloco)
                    {
                        bool valido = false;
                        if (regra.AlcadasBloco.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Bloco.Codigo == descarte.Lote.DepositoPosicao.Bloco.Codigo))
                            valido = true;
                        else if (regra.AlcadasBloco.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Bloco.Codigo == descarte.Lote.DepositoPosicao.Bloco.Codigo))
                            valido = true;
                        else if (regra.AlcadasBloco.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Bloco.Codigo != descarte.Lote.DepositoPosicao.Bloco.Codigo))
                            valido = true;
                        else if (regra.AlcadasBloco.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Bloco.Codigo != descarte.Lote.DepositoPosicao.Bloco.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorPosicao)
                    {
                        bool valido = false;
                        if (regra.AlcadasPosicao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Posicao.Codigo == descarte.Lote.DepositoPosicao.Codigo))
                            valido = true;
                        else if (regra.AlcadasPosicao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Posicao.Codigo == descarte.Lote.DepositoPosicao.Codigo))
                            valido = true;
                        else if (regra.AlcadasPosicao.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Posicao.Codigo != descarte.Lote.DepositoPosicao.Codigo))
                            valido = true;
                        else if (regra.AlcadasPosicao.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Posicao.Codigo != descarte.Lote.DepositoPosicao.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorQuantidade)
                    {
                        bool valido = false;
                        if (regra.AlcadasQuantidade.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Quantidade == descarte.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Quantidade == descarte.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Quantidade != descarte.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Quantidade != descarte.Quantidade))
                            valido = true;
                        if (regra.AlcadasQuantidade.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && descarte.Quantidade >= o.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && descarte.Quantidade >= o.Quantidade))
                            valido = true;
                        if (regra.AlcadasQuantidade.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && descarte.Quantidade <= o.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorIgualQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && descarte.Quantidade <= o.Quantidade))
                            valido = true;
                        if (regra.AlcadasQuantidade.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && descarte.Quantidade > o.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MaiorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && descarte.Quantidade > o.Quantidade))
                            valido = true;
                        if (regra.AlcadasQuantidade.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && descarte.Quantidade < o.Quantidade))
                            valido = true;
                        else if (regra.AlcadasQuantidade.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.MenorQue && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && descarte.Quantidade < o.Quantidade))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                }
            }

            return listaFiltrada;
        }

        /// <summary>
        /// Cria o vinculo das regras com os aprovadores
        /// </summary>
        /// <returns>Retorna verdadeiro quando existe alguma regra para algum aprovador e falso para quando é aprovada automática</returns>
        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.WMS.RegraDescarte> listaFiltrada, Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto repAprovacaoAlcadaDescarteLoteProduto = new Repositorio.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.WMS.RegraDescarte regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto autorizacao = new Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto
                        {
                            Descarte = descarte,
                            Usuario = aprovador,
                            RegraDescarte = regra,
                        };
                        repAprovacaoAlcadaDescarteLoteProduto.Inserir(autorizacao);

                        string titulo = Localization.Resources.Produtos.Lote.DescarteLote;
                        string nota = string.Format(Localization.Resources.Produtos.Lote.UsuarioSolicitouLiberacaoDescarteQuantia, usuario.Nome, descarte.Quantidade.ToString("n2"), descarte.Lote.ProdutoEmbarcador.Descricao);
                        serNotificacao.GerarNotificacaoEmail(aprovador, usuario, descarte.Codigo, "WMS/DescarteLoteProduto", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto autorizacao = new Dominio.Entidades.Embarcador.WMS.AprovacaoAlcadaDescarteLoteProduto
                    {
                        Descarte = descarte,
                        Usuario = null,
                        RegraDescarte = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao
                    };
                    repAprovacaoAlcadaDescarteLoteProduto.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        public static void DescarteLoteAprovado(Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (descarte.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Finalizado)
                return;

            Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote repProdutoEmbarcadorLote = new Repositorio.Embarcador.Produtos.ProdutoEmbarcadorLote(unitOfWork);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

            descarte.Lote.QuantidadeAtual -= descarte.Quantidade;
            if (descarte.Lote.QuantidadeAtual < 0)
                descarte.Lote.QuantidadeAtual = 0;
            repProdutoEmbarcadorLote.Atualizar(descarte.Lote);

            if (descarte.Produto != null)
                servicoEstoque.MovimentarEstoque(out string erro, descarte.Produto, descarte.Quantidade, Dominio.Enumeradores.TipoMovimento.Saida, "SAID", "DESCARTE " + descarte.Motivo, descarte.Produto.CustoMedio, null, DateTime.Now, tipoServicoMultisoftware);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, descarte.Lote, null, "Descartou " + descarte.Quantidade.ToString("n3") + " do lote.", unitOfWork);
        }

        public static bool ValidaDescarteLote(Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador descarte, Repositorio.UnitOfWork unitOfWork)
        {
            if (descarte.Lote != null)
                return true;// descarte.Lote.QuantidadeAtual >= descarte.Quantidade;
            else
                return true;
        }

        #endregion
    }
}
