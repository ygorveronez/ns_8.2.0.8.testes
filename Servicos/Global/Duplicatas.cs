using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class Duplicatas : ServicoBase
    {
        public Duplicatas(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public bool GeraDuplicatasPorDocumentoDeEntrada(int codigoEmpresa, int codigoDocumentoEntrada, int codigoUsuario, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                unidadeDeTrabalho.Start();

                Repositorio.DocumentoEntrada repDocumentoEntrada = new Repositorio.DocumentoEntrada(unidadeDeTrabalho);
                Repositorio.ParcelaDocumentoEntrada repParcelaDocumentoEntrada = new Repositorio.ParcelaDocumentoEntrada(unidadeDeTrabalho);
                Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);
                Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeDeTrabalho);

                Dominio.Entidades.DocumentoEntrada documentoEntrada = repDocumentoEntrada.BuscarPorCodigo(codigoEmpresa, codigoDocumentoEntrada);
                List<Dominio.Entidades.ParcelaDocumentoEntrada> parcelas = repParcelaDocumentoEntrada.BuscarPorDocumentoEntrada(codigoDocumentoEntrada);

                Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorDocumentoEntrada(documentoEntrada.Empresa.Codigo, documentoEntrada.Codigo);
                bool inserir = false;
                if (duplicata == null)
                {
                    duplicata = new Dominio.Entidades.Duplicata();
                    inserir = true;
                }
                else
                {
                    List<Dominio.Entidades.DuplicataParcelas> listaDuplicatasParcelas = repDuplicataParcelas.BuscarPorDuplicata(duplicata.Codigo);
                    foreach (Dominio.Entidades.DuplicataParcelas duplicatasParcelas in listaDuplicatasParcelas)
                        repDuplicataParcelas.Deletar(duplicatasParcelas);

                    this.DeletarMovimentoDoFinanceiro(duplicata, unidadeDeTrabalho);
                }

                duplicata.Numero = repDuplicata.BuscarUltimoNumero(documentoEntrada.Empresa.Codigo) + 1;
                duplicata.Acrescimo = 0;
                duplicata.DataDocumento = documentoEntrada.DataEntrada;
                duplicata.DataLancamento = documentoEntrada.DataEntrada;
                duplicata.Desconto = 0;
                duplicata.Documento = documentoEntrada.Numero.ToString();                
                duplicata.Empresa = documentoEntrada.Empresa;
                duplicata.Funcionario = repUsuario.BuscarPorCodigo(codigoUsuario);
                duplicata.Pessoa = documentoEntrada.Fornecedor;
                duplicata.PlanoDeConta = documentoEntrada.PlanoDeConta;
                duplicata.Status = "A";
                duplicata.Tipo = Dominio.Enumeradores.TipoDuplicata.APagar;
                duplicata.Valor = documentoEntrada.ValorTotal;
                duplicata.Veiculo1 = documentoEntrada.Veiculo;
                duplicata.Observacao = "Duplicata gerada a partir do documento de entrada Número " + documentoEntrada.Numero.ToString() + " Série " + documentoEntrada.Serie.ToString() + " Fornecedor " + documentoEntrada.Fornecedor.CPF_CNPJ_Formatado + " " + documentoEntrada.Fornecedor.Nome;
                duplicata.DocumentoEntrada = documentoEntrada;

                if (inserir)
                    repDuplicata.Inserir(duplicata);
                else
                    repDuplicata.Atualizar(duplicata);

                for (var i = 0; i < parcelas.Count; i++)
                {
                    Dominio.Entidades.DuplicataParcelas duplicataParcela = new Dominio.Entidades.DuplicataParcelas();
                    duplicataParcela.Duplicata = duplicata;
                    duplicataParcela.Parcela = i + 1;
                    duplicataParcela.DataVcto = parcelas[i].DataVencimento;
                    duplicataParcela.Valor = parcelas[i].Valor;
                    duplicataParcela.Status = Dominio.Enumeradores.StatusDuplicata.Pendente;
                    repDuplicataParcelas.Inserir(duplicataParcela);
                }

                this.GerarMovimentoDoFinanceiro(codigoEmpresa, duplicata, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Não foi possível gera duplicata do documento de entrada " + ex);

                unidadeDeTrabalho.Rollback();

                return false;
            }
        }

        public void DeletarDuplicataDocumentoEntrada(int codigoEmpresa, int codigoDocumentoEntrada, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);
            Repositorio.DuplicataParcelas repDuplicataParcelas = new Repositorio.DuplicataParcelas(unidadeDeTrabalho);

            Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorDocumentoEntrada(codigoEmpresa, codigoDocumentoEntrada);

            if (duplicata != null)
            {
                List<Dominio.Entidades.DuplicataParcelas> listaDuplicatasParcelas = repDuplicataParcelas.BuscarPorDuplicata(duplicata.Codigo);
                foreach (Dominio.Entidades.DuplicataParcelas duplicatasParcelas in listaDuplicatasParcelas)
                    repDuplicataParcelas.Deletar(duplicatasParcelas);

                this.DeletarMovimentoDoFinanceiro(duplicata, unidadeDeTrabalho);

                repDuplicata.Deletar(duplicata);
            }
        }

        public void GerarMovimentoDoFinanceiro(int codigoEmpresa, Dominio.Entidades.Duplicata duplicata, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);
            //Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorCodigo(codigoEmpresa, codigoDuplicata);

            if (duplicata != null)
            {
                if (duplicata.PlanoDeConta != null)
                {
                    Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                    Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorDuplicata(duplicata.Codigo);

                    if (movimento == null)
                        movimento = new Dominio.Entidades.MovimentoDoFinanceiro();

                    movimento.Data = duplicata.DataLancamento;
                    movimento.Documento = duplicata.Documento;
                    movimento.Empresa = duplicata.Empresa;
                    movimento.PlanoDeConta = duplicata.PlanoDeConta;
                    movimento.Valor = duplicata.Valor;
                    movimento.Pessoa = duplicata.Pessoa;
                    movimento.Veiculo = duplicata.Veiculo1;
                    movimento.Observacao = string.Concat("Ref. à duplicata nº " + duplicata.Numero.ToString() + ".");
                    movimento.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                    if (duplicata.Motorista != null)
                        movimento.Motorista = duplicata.Motorista;

                    if (movimento.Duplicata != null && movimento.Duplicata.Count() > 0)
                        movimento.Duplicata.Clear();
                    else if (movimento.Duplicata == null)
                        movimento.Duplicata = new List<Dominio.Entidades.Duplicata>();

                    movimento.Duplicata.Add(duplicata);

                    if (movimento.Codigo > 0)
                        repMovimento.Atualizar(movimento);
                    else
                        repMovimento.Inserir(movimento);

                    Repositorio.MovimentoDoFinanceiro repMovimentoDesconto = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoDesconto = repMovimentoDesconto.BuscarPorDuplicataDesconto(duplicata.Codigo);
                    if (duplicata.Desconto > 0)
                    {
                        if (movimentoDesconto == null)
                            movimentoDesconto = new Dominio.Entidades.MovimentoDoFinanceiro();

                        movimentoDesconto.Data = duplicata.DataLancamento;
                        movimentoDesconto.Documento = duplicata.Documento;
                        movimentoDesconto.Empresa = duplicata.Empresa;
                        movimentoDesconto.PlanoDeConta = duplicata.PlanoDeConta;
                        movimentoDesconto.Valor = duplicata.Desconto;
                        movimentoDesconto.Pessoa = duplicata.Pessoa;
                        movimentoDesconto.Veiculo = duplicata.Veiculo1;
                        movimentoDesconto.Observacao = string.Concat("Ref. ao desconto da duplicata nº " + duplicata.Numero.ToString() + ".");
                        movimentoDesconto.Tipo = Dominio.Enumeradores.TipoMovimento.Saida;
                        if (duplicata.Motorista != null)
                            movimentoDesconto.Motorista = duplicata.Motorista;

                        if (movimentoDesconto.DuplicataDesconto != null && movimentoDesconto.DuplicataDesconto.Count() > 0)
                            movimentoDesconto.DuplicataDesconto.Clear();
                        else if (movimentoDesconto.DuplicataDesconto == null)
                            movimentoDesconto.DuplicataDesconto = new List<Dominio.Entidades.Duplicata>();

                        movimentoDesconto.DuplicataDesconto.Add(duplicata);

                        if (movimentoDesconto.Codigo > 0)
                            repMovimentoDesconto.Atualizar(movimentoDesconto);
                        else
                            repMovimentoDesconto.Inserir(movimentoDesconto);
                    }
                    else if (movimentoDesconto != null)
                    {
                        repMovimentoDesconto.Deletar(movimentoDesconto);
                    }

                    Repositorio.MovimentoDoFinanceiro repMovimentoAcrescimo = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                    Dominio.Entidades.MovimentoDoFinanceiro movimentoAcrescimo = repMovimentoAcrescimo.BuscarPorDuplicataAcrescimo(duplicata.Codigo);
                    if (duplicata.Acrescimo > 0)
                    {
                        if (movimentoAcrescimo == null)
                            movimentoAcrescimo = new Dominio.Entidades.MovimentoDoFinanceiro();

                        movimentoAcrescimo.Data = duplicata.DataLancamento;
                        movimentoAcrescimo.Documento = duplicata.Documento;
                        movimentoAcrescimo.Empresa = duplicata.Empresa;
                        movimentoAcrescimo.PlanoDeConta = duplicata.PlanoDeConta;
                        movimentoAcrescimo.Valor = duplicata.Acrescimo;
                        movimentoAcrescimo.Pessoa = duplicata.Pessoa;
                        movimentoAcrescimo.Veiculo = duplicata.Veiculo1;
                        movimentoAcrescimo.Observacao = string.Concat("Ref. ao acrescimo da duplicata nº " + duplicata.Numero.ToString() + ".");
                        movimentoAcrescimo.Tipo = Dominio.Enumeradores.TipoMovimento.Entrada;
                        if (duplicata.Motorista != null)
                            movimentoAcrescimo.Motorista = duplicata.Motorista;

                        if (movimentoAcrescimo.DuplicataAcrescimo != null && movimentoAcrescimo.DuplicataAcrescimo.Count() > 0)
                            movimentoAcrescimo.DuplicataAcrescimo.Clear();
                        else if (movimentoAcrescimo.DuplicataAcrescimo == null)
                            movimentoAcrescimo.DuplicataAcrescimo = new List<Dominio.Entidades.Duplicata>();

                        movimentoAcrescimo.DuplicataAcrescimo.Add(duplicata);

                        if (movimentoAcrescimo.Codigo > 0)
                            repMovimentoAcrescimo.Atualizar(movimentoAcrescimo);
                        else
                            repMovimentoAcrescimo.Inserir(movimentoAcrescimo);
                    }
                    else if (movimentoAcrescimo != null)
                    {
                        repMovimentoAcrescimo.Deletar(movimentoAcrescimo);
                    }

                    //if (duplicata.Tipo == Dominio.Enumeradores.TipoDuplicata.AReceber)
                    //{
                    //    if (duplicata.Empresa.Configuracao != null && duplicata.Empresa.Configuracao.PlanoCTe != null)
                    //    {
                    //        Repositorio.MovimentoDoFinanceiro repMovimentoCTe = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                    //        Dominio.Entidades.MovimentoDoFinanceiro movimentoCTe = repMovimentoCTe.BuscarPorBaixaCTeDuplicata(duplicata.Codigo);

                    //        if (movimentoCTe == null)
                    //            movimentoCTe = new Dominio.Entidades.MovimentoDoFinanceiro();

                    //        movimentoCTe.Data = duplicata.DataLancamento;
                    //        movimentoCTe.Documento = duplicata.Documento;
                    //        movimentoCTe.Empresa = duplicata.Empresa;
                    //        movimentoCTe.PlanoDeConta = duplicata.Empresa.Configuracao.PlanoCTe;
                    //        movimentoCTe.Valor = duplicata.Valor;
                    //        movimentoCTe.Pessoa = duplicata.Pessoa;
                    //        movimentoCTe.Veiculo = duplicata.Veiculo1;
                    //        movimentoCTe.Observacao = string.Concat("Ref. à duplicata nº " + duplicata.Numero.ToString() + ".");
                    //        movimentoCTe.Tipo = Dominio.Enumeradores.TipoMovimento.Saida;
                    //        if (duplicata.Motorista != null)
                    //            movimentoCTe.Motorista = duplicata.Motorista;

                    //        if (movimentoCTe.DuplicataBaixaCte != null && movimentoCTe.DuplicataBaixaCte.Count() > 0)
                    //            movimentoCTe.DuplicataBaixaCte.Clear();
                    //        else if (movimentoCTe.DuplicataBaixaCte == null)
                    //            movimentoCTe.DuplicataBaixaCte = new List<Dominio.Entidades.Duplicata>();

                    //        movimentoCTe.DuplicataBaixaCte.Add(duplicata);

                    //        if (movimentoCTe.Codigo > 0)
                    //            repMovimentoCTe.Atualizar(movimentoCTe);
                    //        else
                    //            repMovimentoCTe.Inserir(movimentoCTe);
                    //    }
                    //}
                }
            }
        }

        public void DeletarMovimentoDoFinanceiro(Dominio.Entidades.Duplicata duplicata, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Duplicata repDuplicata = new Repositorio.Duplicata(unidadeDeTrabalho);
            //Dominio.Entidades.Duplicata duplicata = repDuplicata.BuscaPorCodigo(0, codigoDuplicata);

            if (duplicata != null)
            {
                Repositorio.MovimentoDoFinanceiro repMovimento = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                Dominio.Entidades.MovimentoDoFinanceiro movimento = repMovimento.BuscarPorDuplicata(duplicata.Codigo);

                if (movimento != null)
                    repMovimento.Deletar(movimento);

                Repositorio.MovimentoDoFinanceiro repMovimentoDesconto = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                Dominio.Entidades.MovimentoDoFinanceiro movimentoDesconto = repMovimentoDesconto.BuscarPorDuplicataDesconto(duplicata.Codigo);

                if (movimentoDesconto != null)
                    repMovimentoDesconto.Deletar(movimentoDesconto);

                Repositorio.MovimentoDoFinanceiro repMovimentoAcrescimo = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                Dominio.Entidades.MovimentoDoFinanceiro movimentoAcresimo = repMovimentoAcrescimo.BuscarPorDuplicataAcrescimo(duplicata.Codigo);

                if (movimentoAcresimo != null)
                    repMovimentoAcrescimo.Deletar(movimentoAcresimo);

                Repositorio.MovimentoDoFinanceiro repMovimentoBaixaCTe = new Repositorio.MovimentoDoFinanceiro(unidadeDeTrabalho);
                Dominio.Entidades.MovimentoDoFinanceiro movimentoBaixaCTe = repMovimentoBaixaCTe.BuscarPorBaixaCTeDuplicata(duplicata.Codigo);

                if (movimentoBaixaCTe != null)
                    repMovimentoBaixaCTe.Deletar(movimentoBaixaCTe);
            }
        }
    }
}
