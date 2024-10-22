using BoletoNetCore.WebAPI.Models;
using System.Net.NetworkInformation;

namespace BoletoNetCore.WebAPI.Extensions
{
    public class GerarBoletoBancos
    {
        readonly IBanco _banco;

        public GerarBoletoBancos(IBanco banco)
        {
            _banco = banco;
        }

        public string RetornarHtmlBoleto(DadosBoleto dadosBoleto)
        {
            // 1º Beneficiarios = Quem recebe o pagamento
            Beneficiario beneficiario = new Beneficiario()
            {
                CPFCNPJ = dadosBoleto.BeneficiarioResponse.CPFCNPJ,
                Nome = dadosBoleto.BeneficiarioResponse.Nome,
                Codigo = dadosBoleto.BeneficiarioResponse.Codigo,   
                ContaBancaria = new ContaBancaria()
                {
                    Agencia = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.Agencia,
                    Conta = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.Conta,
                    CarteiraPadrao = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.CarteiraPadrao,
                    TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                    TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                    TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa,
                    DigitoConta = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.DigitoConta,
                    DigitoAgencia = dadosBoleto.BeneficiarioResponse.ContaBancariaResponse.DigitoAgencia
                }
            };

            _banco.Beneficiario = beneficiario;
            _banco.FormataBeneficiario();

            var boleto = GerarBoleto(_banco, dadosBoleto);
            boleto.ValidarDados();

			var boletoBancario = new BoletoBancario();
            boletoBancario.Boleto = boleto;

            return boletoBancario.MontaHtmlEmbedded();
        }

        public static Boleto GerarBoleto(IBanco iBanco, DadosBoleto dadosBoleto)
        {
            try
            {
                var boleto = new Boleto(iBanco)
                {
                    Pagador = GerarPagador(dadosBoleto),
                    DataEmissao = dadosBoleto.DataEmissao,
                    DataProcessamento = dadosBoleto.DataProcessamento,
                    DataVencimento = dadosBoleto.DataVencimento,
                    ValorTitulo = dadosBoleto.ValorTitulo,
                    NossoNumero = dadosBoleto.NossoNumero,
                    NumeroDocumento = dadosBoleto.NumeroDocumento,
                    EspecieDocumento = TipoEspecieDocumento.DS,
                    ImprimirValoresAuxiliares = true,
					DataJuros = DateTime.TryParse(dadosBoleto.DataJuros?.ToString(), out var dataJuros) ? dataJuros : DateTime.MinValue,
					TipoJuros = Enum.TryParse<TipoJuros>(dadosBoleto.TipoJuros?.ToString(), out var tipoJuros) ? tipoJuros : TipoJuros.Isento,
					ValorJurosDia = decimal.TryParse(dadosBoleto.ValorJurosDia?.ToString(), out var valorJurosDia) ? valorJurosDia : 0m,
				};
                return boleto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Pagador GerarPagador(DadosBoleto dadosBoleto)
        {
            return new Pagador
            {
                Nome = dadosBoleto.PagadorResponse.Nome,
                CPFCNPJ = dadosBoleto.PagadorResponse.CPFCNPJ,
                Observacoes = dadosBoleto.PagadorResponse.Observacoes,
                Endereco = new Endereco
                {
                    LogradouroEndereco = dadosBoleto.PagadorResponse.EnderecoResponse.Logradouro,
                    LogradouroNumero = dadosBoleto.PagadorResponse.EnderecoResponse.Numero,
                    Bairro = dadosBoleto.PagadorResponse.EnderecoResponse.Bairro,
                    Cidade = dadosBoleto.PagadorResponse.EnderecoResponse.Cidade,
                    UF = dadosBoleto.PagadorResponse.EnderecoResponse.Estado,
                    CEP = dadosBoleto.PagadorResponse.EnderecoResponse.CEP,
                }
            };
        }
    }
}
