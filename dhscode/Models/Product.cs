using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHSOnlineStore.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Brand { get; set; }
		public string Image { get; set; }

		[ModelBinder(BinderType = typeof(PriceModelBinder))]
		public double Price { get; set; } 

		public string Category { get; set; }
		public string Description { get; set; }
		public int Quantity { get; set; }
		public Stock Stock { get; set; }
		public List<CartDetail> CartDetail { get; set; }
		public List<OrderDetail> OrderDetail { get; set; }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }
	}
}
