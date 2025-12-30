using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;

public class PriceModelBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		if (bindingContext == null)
		{
			throw new ArgumentNullException(nameof(bindingContext));
		}

		// Get the value entered by the user
		var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

		if (valueProviderResult == ValueProviderResult.None)
		{
			return Task.CompletedTask;
		}

		bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

		var valueAsString = valueProviderResult.FirstValue;

		if (string.IsNullOrEmpty(valueAsString))
		{
			return Task.CompletedTask;
		}

		// Normalize the string by replacing commas with dots
		valueAsString = valueAsString.Replace(",", ".");

		// Try to parse the value as a double
		if (double.TryParse(valueAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
		{
			bindingContext.Result = ModelBindingResult.Success(result);
		}
		else
		{
			bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid price format.");
		}

		return Task.CompletedTask;
	}
}
