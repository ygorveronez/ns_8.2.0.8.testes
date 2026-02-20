using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdminMultisoftware.Repositorio;

namespace SGT.BackgroundWorkers
{
    public class GeracaoTituloCTe : LongRunningProcessBase<GeracaoTituloCTe>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            GerarTitulosCTe(unitOfWork);
            ReverteTitulosCTe(unitOfWork);
        }

        private void GerarTitulosCTe(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Financeiro.ProcessoMovimento svcProcessoMovimento = new Servicos.Embarcador.Financeiro.ProcessoMovimento(unitOfWork.StringConexao);

            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.ParcelaCobrancaCTe repParcelaCobrancaCTe = new Repositorio.ParcelaCobrancaCTe(unitOfWork);
            Repositorio.Embarcador.Transportadores.TransportadorFilial repTransportadorFilial = new Repositorio.Embarcador.Transportadores.TransportadorFilial(unitOfWork);

            List<int> codigosCTes = repCTe.BuscarCTesParaGeracaoTitulosAutomatico();

            unitOfWork.Start();

            for (int i = 0; i < codigosCTes.Count(); i++)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigosCTes[i]);
                if (cte == null)
                    continue;
                string tomador = cte.ObterParticipante(cte.TipoTomador) != null ? cte.ObterParticipante(cte.TipoTomador).CPF_CNPJ : string.Empty;
                if (string.IsNullOrWhiteSpace(tomador))
                    continue;

                List<Dominio.Entidades.ParcelaCobrancaCTe> parcelas = repParcelaCobrancaCTe.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);
                if (parcelas == null || parcelas.Count == 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                    titulo.Acrescimo = 0;
                    titulo.DataEmissao = cte.DataEmissao;
                    titulo.DataVencimento = cte.DataEmissao;
                    titulo.DataProgramacaoPagamento = cte.DataEmissao;
                    titulo.Desconto = 0;
                    titulo.Historico = "GERADO AUTOMATICAMENTE DO CTE DE NÚMERO " + cte.Numero + " E SÉRIE " + cte.Serie.Numero;
                    titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(Double.Parse(tomador));
                    titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                    titulo.Sequencia = 1;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                    titulo.ValorOriginal = cte.ValorAReceber;
                    titulo.ValorPago = 0;
                    titulo.ValorPendente = cte.ValorAReceber;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "CT-e";
                    titulo.NumeroDocumentoTituloOriginal = cte.Numero.ToString();
                    titulo.ConhecimentoDeTransporteEletronico = cte;
                    titulo.Observacao = "CT-e Nº: " + cte.Numero;
                    titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                    titulo.TipoAmbiente = cte.TipoAmbiente;
                    titulo.Usuario = null;
                    titulo.DataLancamento = DateTime.Now;

                    Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial transportadorFilial = repTransportadorFilial.BuscarMatrizPorTransportadorFilial(cte.Empresa.CNPJ);
                    if (transportadorFilial != null && transportadorFilial.Empresa != null)
                        titulo.Empresa = transportadorFilial.Empresa;
                    else
                        titulo.Empresa = cte.Empresa;

                    if (titulo.Empresa != null && titulo.Empresa.TipoMovimento != null)
                    {
                        titulo.TipoMovimento = titulo.Empresa.TipoMovimento;
                    }

                    repTitulo.Inserir(titulo);

                    if (titulo.TipoMovimento != null)
                        svcProcessoMovimento.GerarMovimentacao(out string msgRetorno, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, cte.Numero.ToString(), titulo.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao.Value);
                }
                else
                {
                    foreach (var parcela in parcelas)
                    {
                        Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();
                        titulo.Acrescimo = 0;
                        titulo.DataEmissao = cte.DataEmissao;
                        titulo.DataVencimento = parcela.DataVencimento;
                        titulo.DataProgramacaoPagamento = parcela.DataVencimento;
                        titulo.Desconto = 0;
                        titulo.Historico = "GERADO AUTOMATICAMENTE DO CTE DE NÚMERO " + cte.Numero + " E SÉRIE " + cte.Serie.Numero;
                        titulo.Pessoa = repCliente.BuscarPorCPFCNPJ(Double.Parse(tomador));
                        titulo.GrupoPessoas = titulo.Pessoa.GrupoPessoas;
                        titulo.Sequencia = parcela.Numero;
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                        titulo.ValorOriginal = parcela.Valor;
                        titulo.ValorPago = 0;
                        titulo.ValorPendente = parcela.Valor;
                        titulo.ValorTituloOriginal = parcela.Valor;
                        titulo.TipoDocumentoTituloOriginal = "CT-e";
                        titulo.NumeroDocumentoTituloOriginal = cte.Numero.ToString();
                        titulo.ConhecimentoDeTransporteEletronico = cte;
                        titulo.Observacao = "CT-e Nº: " + cte.Numero;
                        titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros;
                        titulo.TipoAmbiente = cte.TipoAmbiente;

                        Dominio.Entidades.Embarcador.Transportadores.TransportadorFilial transportadorFilial = repTransportadorFilial.BuscarMatrizPorTransportadorFilial(cte.Empresa.CNPJ);
                        if (transportadorFilial != null && transportadorFilial.Empresa != null)
                            titulo.Empresa = transportadorFilial.Empresa;
                        else
                            titulo.Empresa = cte.Empresa;

                        if (titulo.Empresa != null && titulo.Empresa.TipoMovimento != null)
                        {
                            titulo.TipoMovimento = titulo.Empresa.TipoMovimento;
                        }

                        repTitulo.Inserir(titulo);

                        if (titulo.TipoMovimento != null)
                            svcProcessoMovimento.GerarMovimentacao(out string msgRetorno, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, cte.Numero.ToString(), titulo.Observacao, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao.Value);

                    }
                }
            }

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();
        }

        private void ReverteTitulosCTe(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repCTe.BuscarTitulosDeCTesParaCancelamento();

            unitOfWork.Start();

            for (int i = 0; i < titulos.Count; i++)
            {
                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = titulos[i];

                titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                titulo.DataCancelamento = DateTime.Now.Date;
                titulo.DataAlteracao = DateTime.Now;

                repTitulo.Atualizar(titulo);
            }

            unitOfWork.CommitChanges();

            unitOfWork.FlushAndClear();
        }
    }
}