using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Compras
{
    public class RequisicaoMercadoria : ServicoBase
    {
        public RequisicaoMercadoria(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public static void EtapaAprovacao(ref Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            // Instancia Repositorios
            List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> regras = Servicos.Embarcador.Compras.RequisicaoMercadoria.VerificarRegrasAutorizacao(requisicao, unitOfWork);

            bool possuiRegra = regras.Count() > 0;
            bool agAprovacao = true;

            if (possuiRegra)
            {
                requisicao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.AgAprovacao;

                agAprovacao = Servicos.Embarcador.Compras.RequisicaoMercadoria.CriarRegrasAutorizacao(regras, requisicao, requisicao.Usuario, tipoServicoMultisoftware, stringConexao, unitOfWork);

                if (!agAprovacao)
                    requisicao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.Aprovada;
            }
            else
            {
                requisicao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.SemRegra;
            }

            Servicos.Embarcador.Compras.RequisicaoMercadoria.RequisicaoAprovada(requisicao, unitOfWork, Auditado);

        }

        public static List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> VerificarRegrasAutorizacao(Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.RegrasRequisicaoMercadoria repRegraDescarte = new Repositorio.Embarcador.Compras.RegrasRequisicaoMercadoria(unitOfWork);
            List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> listaRegras = new List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>();
            List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> listaFiltrada = new List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria>();
            List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> alcadasCompativeis;

            // Filial
            alcadasCompativeis = repRegraDescarte.AlcadasPorFilial(requisicao.Filial.Codigo, DateTime.Today);
            listaRegras.AddRange(alcadasCompativeis);

            // Deposito
            alcadasCompativeis = repRegraDescarte.AlcadasPorMotivo(requisicao.MotivoCompra.Codigo, DateTime.Today);
            listaRegras.AddRange(alcadasCompativeis);

            listaRegras = listaRegras.Distinct().ToList();
            if (listaRegras.Count() > 0)
            {
                listaFiltrada.AddRange(listaRegras);
                foreach (Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria regra in listaRegras)
                {
                    if (regra.RegraPorFilial)
                    {
                        bool valido = false;
                        if (regra.AlcadasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Filial.Codigo == requisicao.Filial.Codigo))
                            valido = true;
                        else if (regra.AlcadasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Filial.Codigo == requisicao.Filial.Codigo))
                            valido = true;
                        else if (regra.AlcadasFilial.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Filial.Codigo != requisicao.Filial.Codigo))
                            valido = true;
                        else if (regra.AlcadasFilial.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Filial.Codigo != requisicao.Filial.Codigo))
                            valido = true;

                        if (!valido)
                        {
                            listaFiltrada.Remove(regra);
                            continue;
                        }
                    }

                    if (regra.RegraPorMotivo)
                    {
                        bool valido = false;
                        if (regra.AlcadasMotivo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Motivo.Codigo == requisicao.MotivoCompra.Codigo))
                            valido = true;
                        else if (regra.AlcadasMotivo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.IgualA && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Motivo.Codigo == requisicao.MotivoCompra.Codigo))
                            valido = true;
                        else if (regra.AlcadasMotivo.Any(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.Ou && o.Motivo.Codigo != requisicao.MotivoCompra.Codigo))
                            valido = true;
                        else if (regra.AlcadasMotivo.All(o => o.Condicao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao.DiferenteDe && o.Juncao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E && o.Motivo.Codigo != requisicao.MotivoCompra.Codigo))
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

        public static bool CriarRegrasAutorizacao(List<Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria> listaFiltrada, Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            bool possuiRegraPendente = false;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, null, tipoServicoMultisoftware, string.Empty);
            Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria repAprovacaoAlcadaRequisicaoMercadoria = new Repositorio.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria(unitOfWork);

            if (listaFiltrada == null || listaFiltrada.Count() == 0)
                throw new ArgumentException("Lista de Regras deve ser maior que 0");

            foreach (Dominio.Entidades.Embarcador.Compras.RegrasRequisicaoMercadoria regra in listaFiltrada)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    possuiRegraPendente = true;
                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria autorizacao = new Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria
                        {
                            RequisicaoMercadoria = requisicao,
                            Usuario = aprovador,
                            RegraRequisicaoMercadoria = regra,
                            DataCriacao = requisicao.DataAlteracao,
                        };
                        repAprovacaoAlcadaRequisicaoMercadoria.Inserir(autorizacao);

                        string titulo = Localization.Resources.Compras.RequisicaoMecadoria.RequisicaoMercadoria;
                        string nota = string.Empty;
                        nota = string.Format(Localization.Resources.Compras.RequisicaoMecadoria.UsuarioCriouRequisicao, usuario.Nome, requisicao.Numero);
                        serNotificacao.GerarNotificacaoEmail(aprovador, usuario, requisicao.Codigo, "Compras/AutorizacaoRequisicaoMercadoria", titulo, nota, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.cifra, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, tipoServicoMultisoftware, unitOfWork);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria autorizacao = new Dominio.Entidades.Embarcador.Compras.AprovacaoAlcadaRequisicaoMercadoria
                    {
                        RequisicaoMercadoria = requisicao,
                        Usuario = null,
                        RegraRequisicaoMercadoria = regra,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = "Alçada aprovada pela Regra " + regra.Descricao,
                        DataCriacao = requisicao.DataAlteracao,
                    };
                    repAprovacaoAlcadaRequisicaoMercadoria.Inserir(autorizacao);
                }
            }

            return possuiRegraPendente;
        }

        public static void RequisicaoAprovada(Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            if (requisicao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRequisicaoMercadoria.Aprovada)
                return;

            Repositorio.Embarcador.Compras.Mercadoria repMercadoria = new Repositorio.Embarcador.Compras.Mercadoria(unitOfWork);
            Repositorio.Embarcador.Compras.RequisicaoMercadoria repRequisicaoMercadoria = new Repositorio.Embarcador.Compras.RequisicaoMercadoria(unitOfWork);

            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

            List<Dominio.Entidades.Embarcador.Compras.Mercadoria> mercadorias = repMercadoria.BuscarPorRequisicao(requisicao.Codigo);
            bool transformarRequisicaoEmCompra = false;
            bool formaRequisicaoCompra = requisicao.MotivoCompra.Forma == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria.Compra;
            bool formaRequisicaoEstoque = requisicao.MotivoCompra.Forma == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaRequisicaoMercadoria.Estoque;

            foreach (Dominio.Entidades.Embarcador.Compras.Mercadoria mercadoria in mercadorias)
            {
                if (formaRequisicaoCompra)
                {
                    mercadoria.Saldo = mercadoria.Quantidade;
                    mercadoria.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Compra;
                    repMercadoria.Atualizar(mercadoria);
                }
                else if (formaRequisicaoEstoque)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = mercadoria.ProdutoEstoque;
                    servicoEstoque.MovimentarEstoque(estoque, mercadoria.Quantidade, Dominio.Enumeradores.TipoMovimento.Saida, "REQ", requisicao.Numero.ToString(), estoque.UltimoCusto, requisicao.Filial, DateTime.Now);
                }
                else
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = mercadoria.ProdutoEstoque;

                    decimal quantidadeFaltante = 0;
                    decimal quantidadeMovimento = 0;

                    if (estoque.Quantidade < 0)
                    {
                        quantidadeMovimento = 0;
                        quantidadeFaltante = mercadoria.Quantidade + (estoque.Quantidade * -1);
                    }
                    else if (estoque.Quantidade > 0)
                    {
                        if (estoque.Quantidade > mercadoria.Quantidade)
                        {
                            quantidadeMovimento = mercadoria.Quantidade;
                            quantidadeFaltante = 0;
                        }
                        else
                        {
                            quantidadeMovimento = estoque.Quantidade;
                            quantidadeFaltante = ((estoque.Quantidade - mercadoria.Quantidade) * -1);
                        }
                    }
                    else
                    {
                        quantidadeMovimento = 0;
                        quantidadeFaltante = mercadoria.Quantidade;
                    }

                    if (quantidadeMovimento > 0)
                        servicoEstoque.MovimentarEstoque(estoque, quantidadeMovimento, Dominio.Enumeradores.TipoMovimento.Saida, "REQ", requisicao.Numero.ToString(), estoque.UltimoCusto, requisicao.Filial, DateTime.Now);

                    if (quantidadeFaltante > 0)
                    {
                        transformarRequisicaoEmCompra = true;
                        mercadoria.Saldo = quantidadeFaltante;
                        mercadoria.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Compra;
                        repMercadoria.Atualizar(mercadoria);
                    }
                }
            }

            if (transformarRequisicaoEmCompra || formaRequisicaoCompra)
                requisicao.Modo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoRequisicaoMercadoria.Compra;

            requisicao.DataAprovacao = DateTime.Now;
            repRequisicaoMercadoria.Atualizar(requisicao);
        }

        #endregion
    }
}
