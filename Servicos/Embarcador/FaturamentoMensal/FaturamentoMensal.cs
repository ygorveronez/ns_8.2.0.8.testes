using System;

namespace Servicos.Embarcador.FaturamentoMensal
{
    public class FaturamentoMensal : ServicoBase
    {
        public FaturamentoMensal(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public DateTime? UltimaDataVencimento(int codigoFaturamentoCliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamento = repFaturamentoMensalCliente.BuscarPorCodigo(codigoFaturamentoCliente);
            if (faturamento != null)
            {
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(codigoFaturamentoCliente);
                if (faturamentoClienteServico != null)
                {
                    DateTime diaVencimentoFatura = faturamentoClienteServico.DataVencimento.Value;
                    return diaVencimentoFatura;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;

        }

        public DateTime? ProximaDataVencimento(int codigoFaturamentoCliente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamento = repFaturamentoMensalCliente.BuscarPorCodigo(codigoFaturamentoCliente);
            if (faturamento != null)
            {
                int diaFatura = faturamento.DiaFatura;
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(codigoFaturamentoCliente);
                if (faturamentoClienteServico != null)
                {
                    string strDiaVencimentoFatura = diaFatura.ToString() + "/" + faturamentoClienteServico.DataVencimento.Value.Month + "/" + faturamentoClienteServico.DataVencimento.Value.Year;                    
                    DateTime diaVencimentoFatura;
                    DateTime.TryParse(strDiaVencimentoFatura, out diaVencimentoFatura);
                    diaVencimentoFatura = diaVencimentoFatura.AddMonths(1);
                    while (diaVencimentoFatura < DateTime.Now.Date)
                    {
                        diaVencimentoFatura = diaVencimentoFatura.AddMonths(1);
                    }
                    return diaVencimentoFatura;
                }
                else
                {
                    DateTime diaVencimentoFatura;
                    string strDiaVencimentoFatura = diaFatura.ToString() + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    DateTime.TryParse(strDiaVencimentoFatura, out diaVencimentoFatura);
                    if (diaVencimentoFatura == DateTime.MinValue)
                    {
                        while (diaVencimentoFatura == DateTime.MinValue)
                        {
                            strDiaVencimentoFatura = (diaFatura - 1).ToString() + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                            DateTime.TryParse(strDiaVencimentoFatura, out diaVencimentoFatura);
                        }
                    }
                    if (diaVencimentoFatura > DateTime.MinValue)
                    {
                        while (diaVencimentoFatura < DateTime.Now.Date)
                        {
                            diaVencimentoFatura = diaVencimentoFatura.AddMonths(1);
                        }
                        return diaVencimentoFatura;
                    }
                    else
                        return null;
                }
            }
            else
                return null;

        }

        public DateTime? ProximaDataVencimento(int diaFatura)
        {
            if (diaFatura > 0)
            {
                DateTime diaVencimentoFatura;
                string strDiaVencimentoFatura = diaFatura.ToString() + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                DateTime.TryParse(strDiaVencimentoFatura, out diaVencimentoFatura);
                if (diaVencimentoFatura == DateTime.MinValue)
                {
                    while (diaVencimentoFatura == DateTime.MinValue)
                    {
                        strDiaVencimentoFatura = (diaFatura - 1).ToString() + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                        DateTime.TryParse(strDiaVencimentoFatura, out diaVencimentoFatura);
                    }
                }
                if (diaVencimentoFatura > DateTime.MinValue)
                {
                    while (diaVencimentoFatura < DateTime.Now.Date)
                    {
                        diaVencimentoFatura = diaVencimentoFatura.AddMonths(1);
                    }
                    return diaVencimentoFatura;
                }
                else
                    return null;
            }
            else
                return DateTime.Now.Date;
        }

        public decimal ValorTotalFaturamentoCliente(int codigoFaturamentoCliente, DateTime? dataVencimento, Repositorio.UnitOfWork unidadeDeTrabalho, double cnpjCliente = 0)
        {
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente repFaturamentoMensalCliente = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalCliente(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico repFaturamentoMensalServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalServico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unidadeDeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
            Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente faturamento = repFaturamentoMensalCliente.BuscarPorCodigo(codigoFaturamentoCliente);
            decimal valorPlanoEmissao = 0;
            if (codigoFaturamentoCliente == 0 && cnpjCliente > 0)
            {
                string cnpjEmpresa = cnpjCliente.ToString().PadLeft(14, '0');
                Dominio.Entidades.Empresa empresaCliente = repEmpresa.BuscarPorCNPJ(cnpjEmpresa);
                int codigoEmpresaCliente = 0;
                if (empresaCliente != null)
                    codigoEmpresaCliente = empresaCliente.Codigo;
                int qtdTitulo = repTitulo.QuantidadeTitulosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));
                int qtdBoleto = repTitulo.QuantidadeBoletosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));
                int qtdNFSe = repCTe.QuantidadeNotaServico(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));
                int qtdNotaFiscal = repNotaFiscal.QuantidadeNotaFiscal(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(codigoFaturamentoCliente);
                Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor planoEmissao = repPlanoEmissaoNFe.BuscarPlanoEmissao(qtdTitulo, qtdBoleto, qtdNotaFiscal, qtdNFSe, empresaCliente.EmpresaPai?.Codigo);
                if (planoEmissao != null)
                {
                    if (planoEmissao.PlanoEmissaoNFe.ValorAdesao > 0 && faturamentoClienteServico == null)
                        valorPlanoEmissao = planoEmissao.PlanoEmissaoNFe.ValorAdesao + planoEmissao.Valor;
                    else
                        valorPlanoEmissao = planoEmissao.Valor;
                }
                else
                    valorPlanoEmissao = 0;
                return valorPlanoEmissao;
            }
            else if (faturamento != null)
            {
                if (faturamento.FaturamentoMensalGrupo.FaturamentoAutomatico)
                {
                    Dominio.Entidades.Empresa empresaCliente = repEmpresa.BuscarPorCNPJ(faturamento.Pessoa.CPF_CNPJ_SemFormato);
                    int codigoEmpresaCliente = 0;
                    if (empresaCliente != null)
                        codigoEmpresaCliente = empresaCliente.Codigo;
                    int qtdTitulo = repTitulo.QuantidadeTitulosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));
                    int qtdBoleto = repTitulo.QuantidadeBoletosReceber(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));
                    int qtdNFSe = repCTe.QuantidadeNotaServico(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));
                    int qtdNotaFiscal = repNotaFiscal.QuantidadeNotaFiscal(codigoEmpresaCliente, Dominio.Enumeradores.TipoAmbiente.Producao, dataVencimento.Value.AddMonths(-1));

                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(codigoFaturamentoCliente);
                    Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor planoEmissao = repPlanoEmissaoNFe.BuscarPlanoEmissao(qtdTitulo, qtdBoleto, qtdNotaFiscal, qtdNFSe, empresaCliente?.EmpresaPai?.Codigo);
                    if (planoEmissao != null)
                    {
                        if (planoEmissao.PlanoEmissaoNFe.ValorAdesao > 0 && faturamentoClienteServico == null)
                            valorPlanoEmissao = planoEmissao.PlanoEmissaoNFe.ValorAdesao + planoEmissao.Valor;
                        else
                            valorPlanoEmissao = planoEmissao.Valor;
                    }
                    else
                        valorPlanoEmissao = 0;
                }

                if (valorPlanoEmissao == 0)
                {
                    decimal valorServicoPrincipal = 0;
                    if ((faturamento.DataLancamento == null || !faturamento.DataLancamento.HasValue) && (faturamento.DataLancamentoAte == null || !faturamento.DataLancamentoAte.HasValue))
                        valorServicoPrincipal = faturamento.ValorServicoPrincipal;
                    else if ((faturamento.DataLancamento.HasValue && faturamento.DataLancamento.Value.Date <= DateTime.Now.Date) && (faturamento.DataLancamentoAte.HasValue && faturamento.DataLancamentoAte.Value.Date >= DateTime.Now.Date))
                        valorServicoPrincipal = faturamento.ValorServicoPrincipal;
                    else if ((faturamento.DataLancamento.HasValue && faturamento.DataLancamento.Value.Date <= DateTime.Now.Date) && (faturamento.DataLancamentoAte == null || !faturamento.DataLancamentoAte.HasValue))
                        valorServicoPrincipal = faturamento.ValorServicoPrincipal;
                    else if ((faturamento.DataLancamento == null || !faturamento.DataLancamento.HasValue) && (faturamento.DataLancamentoAte.HasValue && faturamento.DataLancamentoAte.Value.Date >= DateTime.Now.Date))
                        valorServicoPrincipal = faturamento.ValorServicoPrincipal;

                    decimal valorServicosExtra = repFaturamentoMensalServico.ValorServicoExtra(codigoFaturamentoCliente, dataVencimento);
                    if (faturamento.ValorAdesao > 0)
                    {
                        Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico faturamentoClienteServico = repFaturamentoMensalClienteServico.BuscarUltimoFaturamentoCliente(codigoFaturamentoCliente);
                        if (faturamentoClienteServico == null)
                            return faturamento.ValorAdesao + valorServicoPrincipal + valorServicosExtra;
                    }
                    return valorServicoPrincipal + valorServicosExtra;
                }
                else
                {
                    decimal valorServicosExtra = repFaturamentoMensalServico.ValorServicoExtra(codigoFaturamentoCliente, dataVencimento);
                    return valorPlanoEmissao + valorServicosExtra;
                }

            }
            else
                return 0;

        }
    }
}
