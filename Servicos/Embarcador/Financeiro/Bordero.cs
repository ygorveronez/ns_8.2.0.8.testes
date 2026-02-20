using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace Servicos.Embarcador.Financeiro
{
    public class Bordero
    {
        #region Métodos Públicos

        public static object ObterDetalhesBordero(Dominio.Entidades.Embarcador.Financeiro.Bordero bordero)
        {
            return new
            {
                bordero.Codigo,
                bordero.Numero,
                DataEmissao = bordero.DataEmissao.ToString("dd/MM/yyyy"),
                DataVencimento = bordero.DataVencimento.ToString("dd/MM/yyyy"),
                ValorACobrar = bordero.ValorACobrar.ToString("n2"),
                ValorTotalACobrar = bordero.ValorTotalACobrar.ToString("n2"),
                ValorTotalAcrescimo = bordero.ValorTotalAcrescimo.ToString("n2"),
                ValorTotalDesconto = bordero.ValorTotalDesconto.ToString("n2"),
                bordero.Situacao,
                bordero.DescricaoSituacao,
                bordero.Observacao,
                bordero.ImprimirObservacao,
                bordero.NumeroConta,
                bordero.DigitoAgencia,
                bordero.Agencia,
                bordero.TipoConta,
                Tomador = new
                {
                    Codigo = bordero.Tomador?.CPF_CNPJ_SemFormato ?? string.Empty,
                    Descricao = bordero.Tomador != null ? bordero.Tomador.Nome + " (" + bordero.Tomador.CPF_CNPJ_Formatado + ")" : string.Empty
                },
                Banco = new
                {
                    Codigo = bordero.Banco?.Codigo,
                    Descricao = bordero.Banco?.Descricao
                },
                GrupoPessoas = new
                {
                    Codigo = bordero.GrupoPessoas?.Codigo,
                    Descricao = bordero.GrupoPessoas?.Descricao
                },
                Pessoa = new
                {
                    Codigo = bordero.Cliente?.CPF_CNPJ_SemFormato,
                    Descricao = bordero.Cliente?.Nome
                },
                Empresa = new
                {
                    bordero.Empresa?.Codigo,
                    Descricao = bordero.Empresa?.RazaoSocial
                },
                bordero.TipoPessoa
            };
        }

        public static void AtualizarTotaisBordero(ref Dominio.Entidades.Embarcador.Financeiro.Bordero bordero, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unidadeTrabalho);

            bordero.ValorACobrar = repBorderoTitulo.ObterTotalACobrarLiquido(bordero.Codigo);
            bordero.ValorTotalACobrar = repBorderoTitulo.ObterTotalACobrar(bordero.Codigo);
            bordero.ValorTotalAcrescimo = repBorderoTitulo.ObterTotalAcrescimo(bordero.Codigo);
            bordero.ValorTotalDesconto = repBorderoTitulo.ObterTotalDesconto(bordero.Codigo);

            repBordero.Atualizar(bordero);
        }

        public static void AtualizarTotaisBorderoTitulo(ref Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unidadeTrabalho);

            borderoTitulo.ValorACobrar = repBorderoTituloDocumento.ObterTotalACobrarLiquido(borderoTitulo.Codigo);
            borderoTitulo.ValorTotalACobrar = repBorderoTituloDocumento.ObterTotalACobrar(borderoTitulo.Codigo);
            borderoTitulo.ValorTotalAcrescimo = repBorderoTituloDocumento.ObterTotalAcrescimo(borderoTitulo.Codigo);
            borderoTitulo.ValorTotalDesconto = repBorderoTituloDocumento.ObterTotalDesconto(borderoTitulo.Codigo);

            repBorderoTitulo.Atualizar(borderoTitulo);
        }

        public static bool AdicionarTituloAoBordero(out string erro, Dominio.Entidades.Embarcador.Financeiro.Bordero bordero, Dominio.Entidades.Embarcador.Financeiro.Titulo titulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            if (bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
            {
                erro = "A situação atual do borderô não permite que sejam adicionados títulos ao mesmo.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.TituloDocumento> tituloDocumentos = repTituloDocumento.BuscarPorTitulo(titulo.Codigo);

            if (tituloDocumentos.Count <= 0)
            {
                erro = "O título " + titulo.Codigo.ToString() + " não possui documentos vinculados.";
                return false;
            }

            if (titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
            {
                erro = "A situação do título " + titulo.Codigo.ToString() + " (" + titulo.DescricaoSituacao + ") não permite que o mesmo seja adicionado ao borderô.";
                return false;
            }

            if (repBorderoTitulo.ExistePorTitulo(titulo.Codigo))
            {
                erro = "O título " + titulo.Codigo.ToString() + " já está vinculado à um borderô.";
                return false;
            }

            Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = new Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo()
            {
                Bordero = bordero,
                Titulo = titulo
            };

            repBorderoTitulo.Inserir(borderoTitulo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.TituloDocumento tituloDocumento in tituloDocumentos)
            {
                Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento = new Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento()
                {
                    BorderoTitulo = borderoTitulo,
                    TituloDocumento = tituloDocumento,
                    ValorACobrar = tituloDocumento.ValorPendente,
                    ValorTotalACobrar = tituloDocumento.ValorPendente
                };

                repBorderoTituloDocumento.Inserir(borderoTituloDocumento);
            }

            AtualizarTotaisBorderoTitulo(ref borderoTitulo, unitOfWork);

            titulo.Bordero = bordero;

            repTitulo.Atualizar(titulo);

            erro = string.Empty;
            return true;
        }

        public static bool RemoverTituloDoBordero(out string erro, int codigoBorderoTitulo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = repBorderoTitulo.BuscarPorCodigo(codigoBorderoTitulo);

            if (borderoTitulo == null)
            {
                erro = "Título não encontrado.";
                return false;
            }

            if (borderoTitulo.Bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
            {
                erro = "A situação atual do borderô não permite que sejam removidos títulos do mesmo.";
                return false;
            }

            borderoTitulo.Titulo.Bordero = null;

            repTitulo.Atualizar(borderoTitulo.Titulo);

            List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> borderoTituloDocumentos = repBorderoTituloDocumento.BuscarPorBorderoTitulo(borderoTitulo.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento in borderoTituloDocumentos)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto> borderoTituloDocumentoAcrescimosDescontos = repBorderoTituloDocumentoAcrescimoDesconto.BuscarPorBorderoTituloDocumento(borderoTituloDocumento.Codigo);

                foreach (Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto borderoTituloDocumentoAcrescimoDesconto in borderoTituloDocumentoAcrescimosDescontos)
                    repBorderoTituloDocumentoAcrescimoDesconto.Deletar(borderoTituloDocumentoAcrescimoDesconto);

                repBorderoTituloDocumento.Deletar(borderoTituloDocumento);
            }

            repBorderoTitulo.Deletar(borderoTitulo);

            erro = string.Empty;
            return true;
        }

        public static byte[] GerarImpressaoBordero(Dominio.Entidades.Embarcador.Financeiro.Bordero bordero, Repositorio.UnitOfWork unidadeTrabalho)
        {
            return ReportRequest.WithType(ReportType.Bordero)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("CodigoBordero", bordero.Codigo.ToString())
                .CallReport()
                .GetContentFile();
        }

        public static bool Liquidar(out string erro, Dominio.Entidades.Usuario usuario, Dominio.Entidades.Embarcador.Financeiro.Bordero bordero, DateTime dataBase, DateTime dataBaixa, Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (bordero == null)
            {
                erro = "Borderô não encontrado.";
                return false;
            }

            if (bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Finalizado)
            {
                erro = "O borderô deve estar finalizado para realizar a liquidação do mesmo.";
                return false;
            }

            Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> titulosBordero = repBorderoTitulo.BuscarPorBordero(bordero.Codigo);

            decimal valorPendente = titulosBordero.Sum(o => o.Titulo.ValorPendente) - bordero.ValorACobrar;

            if (valorPendente > 0m)
            {
                erro = "Não é permitido fazer negociação de títulos no borderô, para esta funcionalidade utilize a tela de baixa de títulos a receber.";
                return false;
            }

            if (titulosBordero.Any(o => o.Titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto))
            {
                erro = "Existem títulos quitados ou cancelados no borderô, não sendo possível realizar a liquidação.";
                return false;
            }

            List<int> codigosTitulosDataEmissaoMaior = titulosBordero.Where(o => o.Titulo.DataEmissao.Value.Date > dataBaixa.Date).Select(o => o.Titulo.Codigo).ToList();

            if (codigosTitulosDataEmissaoMaior.Count > 0)
            {
                erro = "A data de emissão dos títulos (" + string.Join(", ", codigosTitulosDataEmissaoMaior) + ") não pode ser maior que a data da baixa.";
                return false;
            }

            List<int> codigosTitulosSemTipoMovimento = titulosBordero.Where(o => o.Titulo.TipoMovimento == null).Select(o => o.Titulo.Codigo).ToList();

            if (codigosTitulosSemTipoMovimento.Count > 0)
            {
                erro = "Existem títulos (" + string.Join(", ", codigosTitulosSemTipoMovimento) + ") sem tipo de movimentação vinculado.";
                return false;
            }

            unitOfWork.Start();

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa
            {
                Bordero = bordero,
                DataBaixa = dataBaixa,
                DataBase = dataBase,
                DataOperacao = DateTime.Now,
                Numero = 1,
                Observacao = (bordero.GrupoPessoas?.Descricao ?? bordero.Cliente?.Nome) + " - BORDERÔ Nº " + bordero.Numero.ToString() + " (" + dataBaixa.ToString("dd/MM/yyyy") + ")",
                SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada,
                Sequencia = 1,
                Valor = bordero.ValorACobrar,
                ValorTotalAPagar = bordero.ValorTotalACobrar,
                Usuario = usuario,
                DataEmissao = bordero.DataEmissao,
                GrupoPessoas = bordero.GrupoPessoas,
                Pessoa = bordero.Cliente,
                ValorAcrescimo = bordero.ValorTotalAcrescimo,
                ValorDesconto = bordero.ValorTotalDesconto,
                TipoPagamentoRecebimento = tipoPagamento
            };

            repTituloBaixa.Inserir(tituloBaixa);

            for (int i = 0; i < titulosBordero.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo = titulosBordero[i];
                List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento> borderoTituloDocumentos = repBorderoTituloDocumento.BuscarPorBorderoTitulo(borderoTitulo.Codigo);

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado
                {
                    Titulo = borderoTitulo.Titulo,
                    TituloBaixa = tituloBaixa,
                    ValorPago = borderoTitulo.ValorACobrar,
                    ValorTotalAPagar = borderoTitulo.ValorTotalACobrar,
                    ValorAcrescimo = borderoTitulo.ValorTotalAcrescimo,
                    ValorDesconto = borderoTitulo.ValorTotalDesconto,
                    DataBase = dataBase,
                    DataBaixa = dataBaixa
                };

                repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado);

                foreach (Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumento borderoTituloDocumento in borderoTituloDocumentos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento tituloBaixaAgrupadoDocumento = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento
                    {
                        TituloBaixaAgrupado = tituloBaixaAgrupado,
                        TituloDocumento = borderoTituloDocumento.TituloDocumento,
                        ValorAcrescimo = borderoTituloDocumento.ValorTotalAcrescimo,
                        ValorDesconto = borderoTituloDocumento.ValorTotalDesconto,
                        ValorPago = borderoTituloDocumento.ValorACobrar,
                        ValorTotalAPagar = borderoTituloDocumento.ValorTotalACobrar,
                        DataAplicacaoDesconto = DateTime.Now,
                        Usuario = usuario
                    };

                    repTituloBaixaAgrupadoDocumento.Inserir(tituloBaixaAgrupadoDocumento);

                    List<Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto> acrescimosDescontos = repBorderoTituloDocumentoAcrescimoDesconto.BuscarPorBorderoTituloDocumento(borderoTituloDocumento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto acrescimoDesconto in acrescimosDescontos)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto tituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto
                        {
                            Justificativa = acrescimoDesconto.Justificativa,
                            Observacao = acrescimoDesconto.Observacao,
                            TipoJustificativa = acrescimoDesconto.TipoJustificativa,
                            TipoMovimentoReversao = acrescimoDesconto.TipoMovimentoReversao,
                            TipoMovimentoUso = acrescimoDesconto.TipoMovimentoUso,
                            TituloBaixaAgrupadoDocumento = tituloBaixaAgrupadoDocumento,
                            Valor = acrescimoDesconto.Valor
                        };

                        repTituloBaixaAgrupadoDocumentoAcrescimoDesconto.Inserir(tituloBaixaAgrupadoDocumentoAcrescimoDesconto);
                    }
                }

                int countDocumentosBaixados = 0;

                if (!BaixaTituloReceber.BaixarTitulo(out erro, tituloBaixa, tituloBaixaAgrupado, null, null, unitOfWork, tipoServicoMultisoftware, bordero, false, 0, ref countDocumentosBaixados))
                {
                    unitOfWork.Rollback();
                    return false;
                }
            }

            bordero.TipoPagamentoRecebimento = tipoPagamento;
            bordero.DataBaixa = dataBaixa;
            bordero.DataBase = dataBase;
            bordero.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Quitado;

            repBordero.Atualizar(bordero);

            unitOfWork.CommitChanges();

            erro = string.Empty;
            return true;
        }

        public static bool Cancelar(out string erro, Dominio.Entidades.Embarcador.Financeiro.Bordero bordero, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (bordero == null)
            {
                erro = "Borderô não encontrado.";
                return false;
            }

            if (bordero.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Cancelado)
            {
                erro = "A situação do borderô não permite que ele seja cancelado.";
                return false;
            }

            Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumento repBorderoTituloDocumento = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto repBorderoTituloDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.BorderoTituloDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento repTituloBaixaAgrupadoDocumento = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto repTituloBaixaAgrupadoDocumentoAcrescimoDesconto = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupadoDocumentoAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

            Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = repTituloBaixa.BuscarPorBordero(bordero.Codigo);

            List<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> borderoTitulos = repBorderoTitulo.BuscarPorBordero(bordero.Codigo);

            unitOfWork.Start();

            if (tituloBaixa != null)
            {
                if (!Servicos.Embarcador.Financeiro.BaixaTituloReceber.CancelarBaixa(out erro, tituloBaixa, unitOfWork, tipoServicoMultisoftware))
                    return false;
            }

            foreach (Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo borderoTitulo in borderoTitulos)
            {
                borderoTitulo.Titulo.Bordero = null;

                repTitulo.Atualizar(borderoTitulo.Titulo);
            }

            bordero.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.Cancelado;

            repBordero.Atualizar(bordero);

            unitOfWork.CommitChanges();

            erro = string.Empty;
            return true;
        }

        #endregion
    }
}