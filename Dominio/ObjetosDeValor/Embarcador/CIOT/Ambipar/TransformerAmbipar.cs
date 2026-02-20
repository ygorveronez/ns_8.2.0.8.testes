using System;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class TransformerAmbipar
    {
        private Dominio.Entidades.Embarcador.Documentos.CIOT ciot;
        private Dominio.Entidades.Embarcador.Cargas.CargaCIOT CargaCIOT;
        private Dominio.Entidades.Cliente remetente;
        private Dominio.Entidades.Cliente destinatario;
        private Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe;
        private Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro;



        Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFreteAmbipar retornoContratoFrete;
        Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa;
        public TransformerAmbipar(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa)
        {
            this.ciot = ciot;
        }

        public TransformerAmbipar(Dominio.Entidades.Embarcador.Cargas.CargaCIOT CargaCIOT, Dominio.Entidades.Cliente remetente, Dominio.Entidades.Cliente destinatario, Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe, Dominio.Entidades.Embarcador.Pessoas.ModalidadeTransportadoraPessoas modalidadeTerceiro)
        {
            this.modalidadeTerceiro = modalidadeTerceiro;
            this.CTe = CTe;
            this.remetente = remetente;
            this.destinatario = destinatario;
            this.CargaCIOT = CargaCIOT;
            this.ciot = CargaCIOT.CIOT;
        }

        public TransformerAmbipar(Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFreteAmbipar retornoContratoFrete)
        {
            this.retornoContratoFrete = retornoContratoFrete;
        }
        public string GetNumeroCiot()
        {
            return retornoContratoFrete.CIOT;
        }

        public string GetDigitoCiot()
        {
            return retornoContratoFrete.ampcId.ToString();
        }

        public string GetProtocoloAutorizacaoCiot()
        {
            return retornoContratoFrete._id;
        }

        public long? carretaID()
        {
            try
            {
                return long.Parse(CargaCIOT.CIOT.VeiculosVinculados.Where(x => x.Codigo != CargaCIOT.CIOT.Veiculo.Codigo)?.FirstOrDefault()?.CodigoIntegracao ?? "0");
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public ETipoEmissao TipoEmissaoID()
        {
            return Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ETipoEmissao.EmissaoComCIOTPadrao;
        }

        public ETipoMidiaValePedagio TipoMidiaValePedagio()
        {
            return Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ETipoMidiaValePedagio.ValePedagioAmbiparCartao;
        }

        public long EmbarcadorFilialID()
        {
            long ret = 0;
            long.TryParse((CargaCIOT?.Carga?.Filial?.CNPJ ?? "0"), out ret);
            return ret;
        }

        public long TransportadorID()
        {
            return (long)(ciot?.Transportador?.CPF_CNPJ ?? 0);
        }

        public long MotoristaID()
        {
            long ret = 0;
            long.TryParse((ciot?.Motorista?.CPF ?? "0"), out ret);
            return ret;
        }

        public long? CartaoID()
        {
            long ret = 0;
            long.TryParse((ciot?.Motorista?.NumeroCartao ?? "0"), out ret);
            return ret;
        }

        public long? CartaoIDTransportador()
        {
            return 0;
        }

        public long VeiculoID()
        {
            long ret = 0;
            long.TryParse((CargaCIOT?.Carga?.Veiculo?.CodigoIntegracao ?? "0"), out ret);
            return ret;
        }

        public long RoteiroID()
        {
            return 0;
        }

        public int TipoOperacaoID()
        {
            int ret = 0;
            int.TryParse((CargaCIOT?.Carga?.TipoOperacao?.CodigoIntegracao ?? "0"), out ret);
            return ret;
        }

        public long TipoMercadoriaID()
        {
            return 0;
        }

        public decimal Valor()
        {
            try
            {
                return ciot.CargaCIOT.Sum(o => (o.ContratoFrete?.ValorFreteSubcontratacao ?? 0m));
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public decimal ValorAdiantamento()
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT Motorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT Transportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador;
                if ((modalidadeTerceiro?.TipoFavorecidoCIOT ?? Transportador) == Motorista)
                    return ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAdiantamento ?? 0m);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal ValorAdiantamentoTransportadora()
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT Transportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador;
                if ((modalidadeTerceiro?.TipoFavorecidoCIOT ?? Transportador) == Transportador)
                    return ciot.CargaCIOT.Sum(o => o.ContratoFrete?.ValorAdiantamento ?? 0m);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal ValorCarga()
        {
            try
            {
                return ciot.CargaCIOT.Sum(x => x.ValorTotalMercadoria);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal PesoCarga()
        {
            try
            {
                return ciot.CargaCIOT.Sum(x => x.PesoBruto);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public DateTime DataPrevisaoEntrega()
        {
            try
            {
                return (DateTime)CargaCIOT.Carga.Pedidos.Max(x => x.Pedido.DataAgendamento);
            }
            catch (Exception)
            {
                return DateTime.UtcNow.AddDays(8);
            }
        }

        public bool RoteiroIdaVolta()
        {
            return false;
        }

        public int EixoSuspensoIda()
        {
            return 0;
        }

        public int EixoSuspensoVolta()
        {
            return 0;
        }

        public DateTime DataPrevistaSaida()
        {
            try
            {
                return (DateTime)CargaCIOT.Carga.Pedidos.Min(x => x.Pedido.DataAgendamento);
            }
            catch (Exception)
            {
                return DateTime.UtcNow.AddDays(8);
            }
        }

        public DateTime DataPrevistaPrestacaoContas()
        {
            try
            {
                return (DateTime)CargaCIOT.CIOT.DataVencimentoSaldo.Value;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public string CodigoViagem()
        {
            return (CargaCIOT?.Carga?.Codigo ?? 0).ToString();
        }

        public DateTime? DataPrestacaoContas()
        {
            return null;
        }

        public DateTime? DataQuitacao()
        {
            return null;
        }

        public DateTime? DataAgendamento()
        {
            return null;
        }

        public DateTime DataPrevistaPagamento()
        {
            try
            {
                return (DateTime)CargaCIOT.CIOT.DataVencimentoSaldo.Value;
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }

        public DateTime? DataPagamento()
        {
            return null;
        }

        public string CpfCnpjDestinatario()
        {
            return (destinatario?.CPF_CNPJ ?? 0).ToString();
        }

        public bool IgnorarPreConfiguracaoRoteiro()
        {
            return true;
        }

        public bool FreteContaMotorista()
        {
            return false;
        }

        public string TipoChavePix()
        {
            return "A";
        }

        public string TipoChavePixTransportador()
        {
            return "E";
        }

        public string ChavePix()
        {
            return "";
        }

        public string ChavePixTransportador()
        {
            return "";
        }

        public bool EmitirValePedagio()
        {
            return false;
        }

        public bool ValePedagioCartao()
        {
            return false;
        }

        public string TipoCreditoVPO()
        {
            return "C";
        }

        public bool FreteFracionado()
        {
            return false;
        }

        public decimal PercentualTransportador()
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT Transportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador;
                if ((modalidadeTerceiro?.TipoFavorecidoCIOT ?? Transportador) == Transportador)
                    return 100 - (modalidadeTerceiro?.PercentualAdiantamentoFretesTerceiro ?? 0);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public decimal PercentualMotorista()
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT Motorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Motorista;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT Transportador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFavorecidoCIOT.Transportador;
                if ((modalidadeTerceiro?.TipoFavorecidoCIOT ?? Transportador) == Motorista)
                    return 100 - (modalidadeTerceiro?.PercentualAdiantamentoFretesTerceiro ?? 0);
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public bool AprovarContrato()
        {
            return false;
        }

        public long TipoDocumentoID()
        {
            return 57;
        }

        public string TipoDocumento()
        {
            return "CT-e";
        }

        public string Numero()
        {
            return CTe.Numero.ToString();
        }

        public string Serie()
        {
            return CTe.Serie.ToString();
        }

        public int Ordem()
        {
            return 1;
        }

        public int EixosSuspenso()
        {
            return 0;
        }

        public bool Tolerancia()
        {
            return false;
        }

        public decimal PercentualTolerancia()
        {
            return 0.0m;
        }

        public decimal LimiteSuperior()
        {
            return 0.0m;
        }

        public decimal QuebraTolerancia()
        {
            return 0.0m;
        }

        public string TipoPeso()
        {
            return "C";
        }

        public bool TipoCobrancaQuebra()
        {
            return false;
        }

        public bool TipoCobrancaAvaria()
        {
            return false;
        }

        public int ContratoID()
        {
            try
            {
                return int.Parse(ciot.Numero);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public DateTime DataMovimentoFinanceiro()
        {
            return DateTime.Now;
        }

        public string TipoTransacao()
        {
            return "C";
        }

        public string DescricaoContratoLancamentoManua()
        {
            throw new NotImplementedException();
        }

        public string motivoCancelamento()
        {
            return !string.IsNullOrWhiteSpace(ciot.MotivoCancelamento) ? ciot.MotivoCancelamento : "CIOT gerado incorretamente.";
        }
    }
}
