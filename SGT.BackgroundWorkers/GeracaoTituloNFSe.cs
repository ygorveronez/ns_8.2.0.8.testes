using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class GeracaoTituloNFSe : LongRunningProcessBase<GeracaoTituloNFSe>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            GerarTitulosNFSe(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
            ReverteTitulosNFSe(unitOfWork, _stringConexao, _tipoServicoMultisoftware);
        }

        private void GerarTitulosNFSe(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);

            Repositorio.Embarcador.CTe.CTeParcela repCTeParcela = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloIntegracao repositorioTituloIntegracao = new Repositorio.Embarcador.Financeiro.TituloIntegracao(unitOfWork);

            unitOfWork.Start();

            List<Dominio.Entidades.Embarcador.CTe.CTeParcela> parcelas = repCTeParcela.BuscarParcelasPendentes();

            for (int i = 0; i < parcelas.Count(); i++)
            {
                var parcelaNFSe = parcelas[i];

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarPorNFSe(parcelaNFSe.ConhecimentoDeTransporteEletronico.Codigo);

                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                titulo.Acrescimo = 0;
                titulo.DataEmissao = parcelaNFSe.DataEmissao;
                titulo.DataVencimento = parcelaNFSe.DataVencimento;
                titulo.DataProgramacaoPagamento = parcelaNFSe.DataVencimento;
                titulo.Desconto = 0;
                titulo.Historico = "GERADO A PARTIR DA NOTA FISCAL DE SERVIÇO DE NÚMERO " + parcelaNFSe.ConhecimentoDeTransporteEletronico.Numero + " E SÉRIE " + parcelaNFSe.ConhecimentoDeTransporteEletronico.Serie.Numero;
                titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(Double.Parse(parcelaNFSe.ConhecimentoDeTransporteEletronico.Remetente.CPF_CNPJ));
                titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                titulo.Sequencia = parcelaNFSe.Sequencia;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                titulo.DataAlteracao = DateTime.Now;
                titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                titulo.ValorOriginal = parcelaNFSe.Valor;
                titulo.ValorPago = 0;
                titulo.ValorPendente = parcelaNFSe.Valor;
                titulo.Empresa = parcelaNFSe.ConhecimentoDeTransporteEletronico.Empresa;
                titulo.ValorTituloOriginal = titulo.ValorOriginal;
                titulo.TipoDocumentoTituloOriginal = "NFS-e";
                titulo.NumeroDocumentoTituloOriginal = parcelaNFSe.ConhecimentoDeTransporteEletronico.Numero.ToString();
                titulo.CTeParcela = repCTeParcela.BuscarPorCodigo(parcelaNFSe.Codigo);
                titulo.Observacao = "NFS-e: " + parcelaNFSe.ConhecimentoDeTransporteEletronico.Numero.ToString();
                titulo.FormaTitulo = parcelaNFSe.Forma;

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    titulo.TipoAmbiente = parcelaNFSe.ConhecimentoDeTransporteEletronico.TipoAmbiente;

                if (parcelaNFSe.ConhecimentoDeTransporteEletronico.NaturezaNFSe != null)
                {
                    Dominio.Entidades.NaturezaDaOperacao natureza = repNaturezaDaOperacao.BuscarPorIdNFSe(parcelaNFSe.ConhecimentoDeTransporteEletronico.NaturezaNFSe.Codigo);
                    if (natureza != null && natureza.TipoMovimento != null)
                        titulo.TipoMovimento = natureza.TipoMovimento;
                }

                if (faturamentoClienteServico != null)
                {
                    titulo.Historico += " PELO FATURAMENTO MENSAL";
                    titulo.BoletoConfiguracao = faturamentoClienteServico.FaturamentoMensalCliente.BoletoConfiguracao;
                    if (titulo.BoletoConfiguracao != null)
                        titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
                    titulo.TipoMovimento = faturamentoClienteServico.FaturamentoMensalCliente.TipoMovimento;

                    titulo.Observacao += " " + faturamentoClienteServico.ObservacaoFatura;
                    if (faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto || faturamentoClienteServico.FaturamentoMensalCliente.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                    {
                        if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.Observacao))
                            titulo.Observacao += " " + faturamentoClienteServico.FaturamentoMensalCliente.Observacao;
                    }
                    if (faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto || faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.TipoObservacaoFaturamentoMensal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto)
                    {
                        if (!string.IsNullOrWhiteSpace(faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao))
                            titulo.Observacao += " " + faturamentoClienteServico.FaturamentoMensalCliente.FaturamentoMensalGrupo.Observacao;
                    }
                    titulo.Observacao = titulo.Observacao.Trim();

                    if (titulo.BoletoStatusTitulo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido)
                    {
                        Servicos.Embarcador.Financeiro.Titulo servTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork);
                        servTitulo.IntegrarEmitido(titulo, unitOfWork);
                    }
                }

                repTitulo.Inserir(titulo);

                if (faturamentoClienteServico != null)
                {
                    faturamentoClienteServico.Titulo = titulo;
                    repFaturamentoMensalClienteServico.Atualizar(faturamentoClienteServico);
                }

                if (titulo.TipoMovimento != null)
                    servProcessoMovimento.GerarMovimentacao(titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, titulo.Codigo.ToString(),
                        "TÍTULO GERADO A PARTIR DE UMA NOTA DE SERVIÇO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0,
                        null, null, titulo.Codigo);
            }

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();
        }

        private void ReverteTitulosNFSe(Repositorio.UnitOfWork unitOfWork, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento servProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(stringConexao);

            unitOfWork.Start();

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Embarcador.CTe.CTeParcela repCTeParcela = new Repositorio.Embarcador.CTe.CTeParcela(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repCTeParcela.BuscarTitulosParaCancelamento();
            for (int i = 0; i < titulos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = titulos[i];

                titulo.DataAlteracao = DateTime.Now;
                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                titulo.DataCancelamento = DateTime.Now.Date;

                repTitulo.Atualizar(titulo);

                if (titulo.TipoMovimento != null && titulo.TipoMovimento.PlanoDeContaCredito != null && titulo.TipoMovimento.PlanoDeContaDebito != null)
                    servProcessoMovimento.GerarMovimentacao(null, titulo.DataCancelamento.Value, titulo.Valor, titulo.Codigo.ToString(),
                        "REVERSÃO DO TÍTULO GERADO A PARTIR DE UMA NOTA DE SERVIÇO", unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros,
                        tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito, titulo.Codigo);
            }

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();
        }
    }
}