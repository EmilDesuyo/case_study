using System.Collections.Generic;

public class SaleCreateViewModel
{
    public List<SelectedProductViewModel> SelectedItems { get; set; } = new();
    public List<ProductSelectionViewModel> AvailableProducts { get; set; } = new();
}

public class ProductSelectionViewModel
{
    public string ItemCode { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int AvailableQuantity { get; set; }
}

public class SelectedProductViewModel
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}