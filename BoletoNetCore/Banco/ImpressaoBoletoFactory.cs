using System.Reflection;
using System;
using System.Linq;

namespace BoletoNetCore
{
	internal static class ImpressaoBoletoFactory<T> where T : IBanco
	{
		public static IImpressaoBoleto<T> ObterTodasImpressaoBoletoImplementacoes()
		{
			var assembly = Assembly.GetExecutingAssembly();

			// Encontra todas as classes que implementam IImpressaoBoleto<T> para o tipo T
			var impressaoBoletoType = assembly.GetTypes()
				.Where(t => !t.IsAbstract && !t.IsInterface)
				.FirstOrDefault(t =>
					t.GetInterfaces()
					 .Any(i => i.IsGenericType &&
							   i.GetGenericTypeDefinition() == typeof(IImpressaoBoleto<>) &&
							   i.GetGenericArguments()[0] == typeof(T)));

			if (impressaoBoletoType != null)
			{
				// Cria uma instância da classe que implementa IImpressaoBoleto<T>
				return (IImpressaoBoleto<T>)Activator.CreateInstance(impressaoBoletoType);
			}

			return null;
		}

		public static void ObterImpressaoBoleto(Boleto boleto)
		{
			var instanciaImpressaoBoleto = ObterTodasImpressaoBoletoImplementacoes();
			if (instanciaImpressaoBoleto != null)
				instanciaImpressaoBoleto.ConfigurarImpressaoBoleto(boleto);
		}

	}
}
