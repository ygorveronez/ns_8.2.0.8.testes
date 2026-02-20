using Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.CTe
{
    public class ModalRodoviario : ServicoBase
    {        

        public ModalRodoviario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario ConverterModalTransporteCTeParaModalRodoviario(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.ModalTransporte != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.WebService.Frota.Veiculo serVeiculo = new WebService.Frota.Veiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();
                modalRodoviario.CIOT = cte.CIOT;
                modalRodoviario.DataEntrega = cte.DataPrevistaEntrega.HasValue ? cte.DataPrevistaEntrega.Value.ToString("dd/MM/yyyy") : "";
                modalRodoviario.Lotacao = cte.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? true : false;

                if (cte.Veiculos != null)
                {
                    modalRodoviario.Veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
                    foreach (Dominio.Entidades.VeiculoCTE veiculoCTe in cte.Veiculos)
                    {
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(veiculoCTe.Veiculo.Codigo);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = serVeiculo.ConverterObjetoVeiculo(veiculo, unitOfWork);
                        modalRodoviario.Veiculos.Add(veiculoIntegracao);
                    }
                }

                if (cte.Motoristas != null)
                {
                    modalRodoviario.Motoristas = new List<Dominio.ObjetosDeValor.Motorista>();
                    foreach (Dominio.Entidades.MotoristaCTE motoristaCTe in cte.Motoristas)
                    {
                        Dominio.ObjetosDeValor.Motorista motorista = new Dominio.ObjetosDeValor.Motorista();
                        motorista.Nome = motoristaCTe.NomeMotorista;
                        motorista.CPF = motoristaCTe.CPFMotorista;
                        modalRodoviario.Motoristas.Add(motorista);
                    }
                }
                return modalRodoviario;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario ConverterModalTransporteCTeParaModalRodoviario(Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.Modal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal.Rodoviario)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.WebService.Frota.Veiculo serVeiculo = new WebService.Frota.Veiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();
                modalRodoviario.CIOT = "";
                modalRodoviario.DataEntrega = cte.DataEmissao.ToString("dd/MM/yyyy");
                modalRodoviario.Lotacao = true;

                return modalRodoviario;
            }
            else
            {
                return null;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario ConverterDynamicModalRodoviario(dynamic dynCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynCTe.Rodoviario != null)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Servicos.WebService.Frota.Veiculo serVeiculo = new WebService.Frota.Veiculo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario = new Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario();
                modalRodoviario.CIOT = (string)dynCTe.Rodoviario.CIOT;
                modalRodoviario.DataEntrega = (string)dynCTe.Rodoviario.PrevisaoEntrega;
                modalRodoviario.Lotacao = (bool)dynCTe.Rodoviario.IndicadorLotacao;

                if (dynCTe.Veiculos != null)
                {
                    modalRodoviario.Veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo>();
                    foreach (var dynVeiculo in dynCTe.Veiculos)
                    {
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo((int)dynVeiculo.CodigoVeiculo);
                        Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao = serVeiculo.ConverterObjetoVeiculo(veiculo, unitOfWork);
                        modalRodoviario.Veiculos.Add(veiculoIntegracao);
                    }
                }

                if (dynCTe.Motoristas != null)
                {
                    modalRodoviario.Motoristas = new List<Dominio.ObjetosDeValor.Motorista>();
                    foreach (var dynMotorista in dynCTe.Motoristas)
                    {
                        Dominio.ObjetosDeValor.Motorista motorista = new Dominio.ObjetosDeValor.Motorista();
                        motorista.Nome = (string)dynMotorista.Nome;
                        motorista.CPF = (string)dynMotorista.CPF;
                        modalRodoviario.Motoristas.Add(motorista);
                    }
                }
                return modalRodoviario;
            }
            else
                return null;
        }

        public void SalvarModalRodoviario(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalRodoviario != null)
            {
                cte.Lotacao = modalRodoviario.Lotacao ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                DateTime dataPrevistaEntrega = new DateTime();
                if (DateTime.TryParseExact(modalRodoviario.DataEntrega, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevistaEntrega))
                    cte.DataPrevistaEntrega = dataPrevistaEntrega;

                SalvarVeiculosCTe(ref cte, modalRodoviario.Veiculos, unitOfWork);
                SalvarMotoristasCTe(ref cte, modalRodoviario.Motoristas, unitOfWork);
            }
        }

        private void SalvarVeiculosCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo> veiculos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unitOfWork);

            if (cte.Codigo > 0)
                repVeiculoCTe.DeletarPorCTe(cte.Codigo);

            List<string> placas = (from obj in veiculos select obj.Placa).Distinct().ToList();

            foreach (string placa in placas)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placa);

                Dominio.Entidades.VeiculoCTE veiculoCTe = new Dominio.Entidades.VeiculoCTE();

                veiculoCTe.CTE = cte;
                veiculoCTe.Veiculo = veiculo;
                veiculoCTe.SetarDadosVeiculo(veiculo);

                repVeiculoCTe.Inserir(veiculoCTe);
            }
        }

        private void SalvarMotoristasCTe(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.ObjetosDeValor.Motorista> motoristas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.MotoristaCTE repMotoristaCTe = new Repositorio.MotoristaCTE(unitOfWork);

            if (cte.Codigo > 0)
                repMotoristaCTe.DeletarPorCTe(cte.Codigo);

            List<string> cpfAdicionado = new List<string>();

            foreach (Dominio.ObjetosDeValor.Motorista moto in motoristas)
            {
                if (!cpfAdicionado.Contains(moto.CPF))
                {
                    Dominio.Entidades.MotoristaCTE motorista = new Dominio.Entidades.MotoristaCTE();

                    motorista.CPFMotorista = moto.CPF;
                    motorista.CTE = cte;
                    motorista.NomeMotorista = moto.Nome;

                    repMotoristaCTe.Inserir(motorista);

                    cpfAdicionado.Add(moto.CPF);
                }
            }
        }

        public void SalvarModalRodoviarioPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, Dominio.ObjetosDeValor.Embarcador.CTe.ModalRodoviario modalRodoviario, Repositorio.UnitOfWork unitOfWork)
        {
            if (modalRodoviario != null)
            {
                Servicos.WebService.Frota.Veiculo serVeiculo = new WebService.Frota.Veiculo(unitOfWork);

                Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unitOfWork);
                preCTe.ModalTransporte = repModalTransporte.BuscarPorCodigo(1, false);
                preCTe.Lotacao = modalRodoviario.Lotacao ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                DateTime dataPrevistaEntrega = new DateTime();

                if (DateTime.TryParseExact(modalRodoviario.DataEntrega, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevistaEntrega))
                    preCTe.DataPrevistaEntrega = dataPrevistaEntrega;
                else
                    preCTe.DataPrevistaEntrega = preCTe.DataEmissao;

                SalvarVeiculosPreCTe(ref preCTe, modalRodoviario.Veiculos, unitOfWork);
                SalvarMotoristasPreCTe(ref preCTe, modalRodoviario.Motoristas, unitOfWork);

            }
        }

        private void SalvarVeiculosPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCTe, List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo> veiculos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.VeiculoPreCTE repVeiculoPreCTe = new Repositorio.VeiculoPreCTE(unitOfWork);

            if (preCTe.Codigo > 0)
                repVeiculoPreCTe.DeletarPorPreCTe(preCTe.Codigo);


            foreach (Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo veiculoIntegracao in veiculos)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(veiculoIntegracao.Placa);

                if (veiculo != null)
                {
                    Dominio.Entidades.VeiculoPreCTE veiculoPreCTe = new Dominio.Entidades.VeiculoPreCTE();
                    veiculoPreCTe.PreCTE = preCTe;
                    veiculoPreCTe.Veiculo = veiculo;
                    veiculoPreCTe.SetarDadosVeiculo(veiculo);
                    repVeiculoPreCTe.Inserir(veiculoPreCTe);
                }
            }
        }

        private void SalvarMotoristasPreCTe(ref Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte, List<Dominio.ObjetosDeValor.Motorista> motoristas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.MotoristaPreCTE repMotoristaPreCTe = new Repositorio.MotoristaPreCTE(unitOfWork);

            if (preCte.Codigo > 0)
                repMotoristaPreCTe.DeletarPorPreCTe(preCte.Codigo);

            foreach (Dominio.ObjetosDeValor.Motorista moto in motoristas)
            {
                Dominio.Entidades.MotoristaPreCTE motorista = new Dominio.Entidades.MotoristaPreCTE();

                motorista.CPFMotorista = moto.CPF;
                motorista.PreCTE = preCte;
                motorista.NomeMotorista = moto.Nome;
                repMotoristaPreCTe.Inserir(motorista);
            }
        }

    }
}
